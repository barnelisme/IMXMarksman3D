// Updated version of MovingTargetsReplaySystem_DualDisplay
// Fixes delayed playback by allowing frames to play immediately
// while encoding continues in background.
// Also supports proper pause/resume and rewind.

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class MovingTargetsReplaySystem_DualDisplay : MonoBehaviour
{
    public RawImage replayRawImage_D1;
    public RawImage replayRawImage_D2;
    public GameObject replayManager;

    public float playbackFPS = 60f;
    [Range(0.1f, 5f)] public float playbackSpeedMultiplier = 1f;
    [Range(1, 100)] public int jpgQuality = 50;

    private float captureInterval;
    private float lastCaptureTime;

    private bool isRecording = false;
    private bool isPlaying = false;
    private bool isRecordingStarted = false;
    private bool isRecordingStopped = false;

    private List<byte[]> replayFrames = new List<byte[]>();
    private List<byte[]> rawFrames = new List<byte[]>();
    private int currentPlaybackFrame = 0;

    private Texture2D sharedPlaybackTexture;
    private Texture2D tempEncodeTexture;

    private Coroutine playbackCoroutine;

    private int captureWidth = 1920;
    private int captureHeight = 1080;

    private bool encodingInProgress = false;

    private RenderTexture persistentRT;
    private Texture2D persistentCaptureTex;

    // NEW: Pause state
    private bool isPaused = false;

    void Start()
    {
        captureInterval = 1f / playbackFPS;

        persistentRT = new RenderTexture(captureWidth, captureHeight, 24);
        persistentCaptureTex = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);

        if (replayRawImage_D1) replayRawImage_D1.gameObject.SetActive(false);
        if (replayRawImage_D2) replayRawImage_D2.gameObject.SetActive(false);
        if (replayManager != null) replayManager.SetActive(false);

        Debug.Log("[Replay] Initialized.");
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.rKey.wasPressedThisFrame) StartRecording();
            if (Keyboard.current.sKey.wasPressedThisFrame) StopRecording();

            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                if (!isPlaying) StartPlayback();
                else if (playbackCoroutine != null) TogglePause();
            }

            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                if (replayFrames.Count > 0)
                    RewindFrames(10);
            }
        }

        if (countDownStart.start_training && !isRecordingStarted)
        {
            StartRecording();
            isRecording = true;
            isRecordingStarted = true;
            isRecordingStopped = false;
        }
        else if (StaticVariableManager.isStopTraining && !isRecordingStopped)
        {
            StopRecording();
            isRecording = false;
            isRecordingStopped = true;
            if (replayManager != null) replayManager.SetActive(true);
        }

        if (isRecording && Time.time > lastCaptureTime + captureInterval)
        {
            lastCaptureTime = Time.time;
            CaptureRawFrame();
        }
    }

    private void StartRecording()
    {
        if (isRecording) return;

        replayFrames.Clear();
        rawFrames.Clear();
        currentPlaybackFrame = 0;
        lastCaptureTime = Time.time;

        isRecording = true;

        Debug.Log("[Replay] Recording started.");
    }

    private void StopRecording()
    {
        if (!isRecording) return;
        isRecording = false;

        Debug.Log("[Replay] Recording stopped. Frames captured (raw): " + rawFrames.Count);

        if (!encodingInProgress)
        {
            StartCoroutine(EncodeAllFramesCoroutine_Parallel());
        }
    }

    private void CaptureRawFrame()
    {
        Camera captureCam = null;
        foreach (Camera cam in Camera.allCameras)
        {
            if (cam.targetDisplay == 1)
            {
                captureCam = cam;
                break;
            }
        }
        if (captureCam == null) captureCam = Camera.main;
        if (captureCam == null) return;

        captureCam.targetTexture = persistentRT;
        captureCam.Render();

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = persistentRT;

        persistentCaptureTex.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        persistentCaptureTex.Apply();

        rawFrames.Add(persistentCaptureTex.GetRawTextureData().ToArray());

        RenderTexture.active = prev;
        captureCam.targetTexture = null;
    }

    private IEnumerator EncodeAllFramesCoroutine_Parallel()
    {
        if (encodingInProgress) yield break;
        encodingInProgress = true;

        if (tempEncodeTexture == null)
            tempEncodeTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);

        Debug.Log("[Replay] Starting background encoding with immediate playback...");

        List<byte[]> localRaw = new List<byte[]>(rawFrames);
        rawFrames.Clear();

        Task.Run(() =>
        {
            for (int i = 0; i < localRaw.Count; i++)
            {
                byte[] imgData = localRaw[i];
                if (imgData == null || imgData.Length == 0)
                {
                    replayFrames.Add(new byte[0]);
                    continue;
                }

                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    tempEncodeTexture.LoadRawTextureData(imgData);
                    tempEncodeTexture.Apply();
                    byte[] jpg = tempEncodeTexture.EncodeToJPG(jpgQuality);
                    replayFrames.Add(jpg);
                });
            }

            encodingInProgress = false;
            Debug.Log("[Replay] Encoding complete.");
        });

        yield break;
    }

    public void PlayVideo()
    {
        StartPlayback();
    }

    public void PauseVideo()
    {
        TogglePause();
    }

    public void RewindVideo()
    {
        if (replayFrames.Count > 0)
            RewindFrames(10);
        TogglePause();
    }

    private void StartPlayback()
    {
        if (replayFrames.Count == 0) return;

        if (playbackCoroutine != null) StopCoroutine(playbackCoroutine);
        isPlaying = false;
        isPaused = false;
        currentPlaybackFrame = 0;

        if (sharedPlaybackTexture == null)
            sharedPlaybackTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);

        replayRawImage_D1.texture = sharedPlaybackTexture;
        replayRawImage_D2.texture = sharedPlaybackTexture;

        replayRawImage_D1.gameObject.SetActive(true);
        replayRawImage_D2.gameObject.SetActive(true);

        Time.timeScale = 1f;

        if (isPlaying) return;

        isPlaying = true;
        playbackCoroutine = StartCoroutine(PlaybackLoop());

        Debug.Log("[Replay] Playback started.");
    }

    private IEnumerator PlaybackLoop()
    {
        float frameDelay = 1f / playbackFPS;

        while (true)
        {
            // Pause support
            while (isPaused)
                yield return null;

            // End of video
            if (currentPlaybackFrame >= replayFrames.Count)
            {
                isPlaying = false;
                yield break;
            }

            var frame = replayFrames[currentPlaybackFrame];
            if (frame != null && frame.Length > 0)
                sharedPlaybackTexture.LoadImage(frame);

            currentPlaybackFrame++;
            yield return new WaitForSeconds(frameDelay / playbackSpeedMultiplier);
        }
    }

    private void TogglePause()
    {
        isPaused = true; //!isPaused;
        Debug.Log("[Replay] Playback " + (isPaused ? "paused" : "resumed"));
    }

    private void RewindFrames(int frameCount)
    {
        if (replayFrames.Count == 0) return;

        currentPlaybackFrame = Mathf.Max(0, currentPlaybackFrame - frameCount);
        sharedPlaybackTexture.LoadImage(replayFrames[currentPlaybackFrame]);
    }

    void OnDestroy()
    {
        if (persistentRT != null) Destroy(persistentRT);
        if (persistentCaptureTex != null) Destroy(persistentCaptureTex);
        if (sharedPlaybackTexture != null) Destroy(sharedPlaybackTexture);
        if (tempEncodeTexture != null) Destroy(tempEncodeTexture);
    }
}
