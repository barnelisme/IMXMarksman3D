using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using System;
using System.IO;

public class MovingShotsReplayManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject screen;

    private bool isRecording = false;
    private bool isPlaying = false;
    private string videoPath;
    private Camera captureCamera;
    private RenderTexture renderTexture;
    private Texture2D frameTexture;
    private FileStream fileStream;
    private BinaryWriter writer;

    void Start()
    {
        // Get camera assigned to display 2 (if any)
        foreach (Camera cam in Camera.allCameras)
        {
            if (cam.targetDisplay == 1) // Display 2 (0-based index)
            {
                captureCamera = cam;
                break;
            }
        }

        if (captureCamera == null)
        {
            Debug.LogError("No camera found on Display 2!");
        }

        // Prepare recording directory
        string dir = Path.Combine(Application.persistentDataPath, "Replays");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        // Set up render texture (1080p default)
        renderTexture = new RenderTexture(1920, 1080, 24);
        captureCamera.targetTexture = renderTexture;

        frameTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
    }

    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
            StartRecording();

        if (Keyboard.current.sKey.wasPressedThisFrame)
            StopRecording();

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (!isPlaying)
                StartPlayback();
            else
                TogglePause();
        }

        if (isRecording)
        {
            SaveFrame();
        }
    }

    private void StartRecording()
    {
        if (isRecording) return;

        videoPath = Path.Combine(Application.persistentDataPath, "Replays", $"Replay_{DateTime.Now:yyyyMMdd_HHmmss}.raw");
        fileStream = new FileStream(videoPath, FileMode.Create);
        writer = new BinaryWriter(fileStream);
        isRecording = true;

        Debug.Log("Recording started...");
    }

    private void StopRecording()
    {
        if (!isRecording) return;

        isRecording = false;
        writer.Close();
        fileStream.Close();

        Debug.Log($"Recording stopped. Saved to: {videoPath}");
    }

    private void SaveFrame()
    {
        RenderTexture.active = renderTexture;
        frameTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        frameTexture.Apply();
        RenderTexture.active = null;

        byte[] bytes = frameTexture.EncodeToJPG(50);
        writer.Write(bytes.Length);
        writer.Write(bytes);
    }

    private void StartPlayback()
    {
        if (string.IsNullOrEmpty(videoPath) || !File.Exists(videoPath))
        {
            Debug.LogWarning("No recorded video available to play.");
            return;
        }

        screen.SetActive(true);
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoPath;
        videoPlayer.Play();

        isPlaying = true;
        Debug.Log("Playback started...");
    }

    private void TogglePause()
    {
        if (!isPlaying) return;

        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            Debug.Log("Playback paused.");
        }
        else
        {
            videoPlayer.Play();
            Debug.Log("Playback resumed.");
        }
    }
}
