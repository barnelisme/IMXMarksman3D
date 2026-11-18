using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.IO;
using Newtonsoft.Json;
using System;
using TMPro;

[System.Serializable]
public class TTrackData
{
    public int id;
    public float headX, headY, headZ;
    public float chestX, chestY, chestZ;
    public float headRotationX, headRotationY, headRotationZ;
    public float chestRotationX, chestRotationY, chestRotationZ;
    public float headScaleX, headScaleY, headScaleZ;
    public float chestScaleX, chestScaleY, chestScaleZ;

    [JsonIgnore]
    public Vector3 head
    {
        get => new Vector3(headX, headY, headZ);
        set { headX = value.x; headY = value.y; headZ = value.z; }
    }

    [JsonIgnore]
    public Vector3 chest
    {
        get => new Vector3(chestX, chestY, chestZ);
        set { chestX = value.x; chestY = value.y; chestZ = value.z; }
    }

    [JsonIgnore]
    public Vector3 headRotation
    {
        get => new Vector3(headRotationX, headRotationY, headRotationZ);
        set { headRotationX = value.x; headRotationY = value.y; headRotationZ = value.z; }
    }

    [JsonIgnore]
    public Vector3 chestRotation
    {
        get => new Vector3(chestRotationX, chestRotationY, chestRotationZ);
        set { chestRotationX = value.x; chestRotationY = value.y; chestRotationZ = value.z; }
    }

    [JsonIgnore]
    public Vector3 headScale
    {
        get => new Vector3(headScaleX, headScaleY, headScaleZ);
        set { headScaleX = value.x; headScaleY = value.y; headScaleZ = value.z; }
    }

    [JsonIgnore]
    public Vector3 chestScale
    {
        get => new Vector3(chestScaleX, chestScaleY, chestScaleZ);
        set { chestScaleX = value.x; chestScaleY = value.y; chestScaleZ = value.z; }
    }
}

[System.Serializable]
public class TFrameData
{
    public long frame;
    public List<TTrackData> tracks = new List<TTrackData>();
}

[System.Serializable]
public class TVideoTrackingData
{
    public List<TFrameData> frames = new List<TFrameData>();
}

[System.Serializable]
public class TVideoMarkers
{
    public string videoName;
    public List<Transform> headMarkers = new List<Transform>();
    public List<Transform> chestMarkers = new List<Transform>();
    public List<Transform> startMarker = new List<Transform>();
}

public class TransitionVideoTracker : MonoBehaviour
{
    public static int currentVideoIndex = 0;
    public bool isSettingUp = false;
    public GameObject tempCollider;
    public GameObject deadShader;
    public TextMeshProUGUI stepBackText;
    public static List<string> hardTargetVideos = new List<string>() {"G13P9", "G13P10" }; // Potential End of training videos. // "G13P9_Video1", "G13P9_Video3" 
    private List<string> vestRepeatVideos = new List<string> { "G13P10" }; // PVR (bullet Proof Video Repeat)

    [Header("Video Players")]
    public List<VideoPlayer> videoPlayers = new List<VideoPlayer>();
    private List<int> currActiveIndexes = new List<int>();
    private List<int> nonThreatIndexes = new List<int>();

    [Header("Markers Per Video")]
    public List<TVideoMarkers> videoMarkersList = new List<TVideoMarkers>();

    private TVideoTrackingData trackingData = new TVideoTrackingData();
    private string trackingFolder;
    private string currentJsonPath;

    public static bool isVideoPlaying = false;
    public static bool isVideoFinished = false;
    public bool isMarkersEnabled = false;
    private bool freezeFrame = false;
    private bool freezeComplete = true;
    public static VideoPlayer vp;

    // Drag & scale state
    private Transform selectedMarker = null;
    private bool isMoving = false;
    private bool isScaling = false;
    private Vector3 dragOffset;

    // Scrubbing
    private float scrubAccumulator = 0f;
    private float scrubFPS = 20f;
    private float actionVideoSpeed = 1f;
    private float stepBackTimerSet = 10f;
    private float stepBackTimer = 5f;
    private float frameFreezeSet = .7f;
    private float frameFreezeTimer = 2f;

    private Vector3 headStartPosition;
    private Vector3 chestStartPosition;

    private void Start()
    {
        // StaticVariableManager.currentVideoGroup = "G13"; // Manual group setting
        currentVideoIndex = 0; // Set default index

        // Set timer settings
        stepBackTimer = stepBackTimerSet;
        frameFreezeTimer = frameFreezeSet;


        isVideoPlaying = false;
        VideoOptionSelector.stepBackInitialised = false;
        vp = null;

        trackingFolder = FileManager.gamePath + "Video Tracking/";
        if (!Directory.Exists(trackingFolder))
            Directory.CreateDirectory(trackingFolder);

        // Disable all markers initially
        ResetMarkers();

        // Always start from first player slot
        SetCurrentindex();

        // 🔑 Assign random clip BEFORE accessing vp.clip
        AssignRandomClipToCurrentPlayer();

        vp = videoPlayers[currentVideoIndex];

        StaticVariableManager.isThreatVideo = SetThreatStatus();
        LoadTrackingForVideo(currentVideoIndex);
        // TemporalTargetTracker(); //// TEMPORAL ////

        vp.Play();
        vp.Pause();
    }

    private void SetCurrentindex()
    {
        // currentVideoIndex = 0;
        StaticVariableManager.previousIndex = currentVideoIndex;
    }

    private void AssignRandomClipToCurrentPlayer()
    {
        var selector = videoPlayers[currentVideoIndex].GetComponent<VideoOptionSelector>();
        if (selector == null)
        {
            Debug.LogError("VideoOptionSelector missing on VideoPlayer");
            return;
        }

        selector.SelectRandomVideo();
    }

    void Update()
    {
        var keyboard = Keyboard.current;

        // isSettingUp = true;
        if (isSettingUp)
        {
            HandleMarkerDragging();
            HandleScrubbing();

            if (keyboard.pKey.wasPressedThisFrame) ManageStartAndPause();
            if (keyboard.sKey.wasPressedThisFrame) SaveCurrentFrame();
            if (keyboard.eKey.wasPressedThisFrame) ExportJSON();

        }

        // print(StaticVariableManager.isStopTraining);

        if(!StaticVariableManager.isStopTraining) // If training is not stopped
        {
            if (countDownStart.start_training && !isVideoPlaying && !StaticVariableManager.initializeStepBack)
            {
                vp.Play();
                isVideoPlaying = true;
            }

            if (isVideoPlaying) // Execute only when the video is playing
            {
                if (!isSettingUp)
                {
                    TrackVideo();
                }

                if (trackingData.frames.Count > 0 && vp.frame >= 0 && !StaticVariableManager.isStopTraining)
                {
                    UpdateMarkersForFrame(vp.frame);
                }

                if (StaticVariableManager.initializeStepBack)
                {
                    vp.Pause();
                    isVideoPlaying = false;
                    if (deadShader != null) deadShader.SetActive(true);
                    if (stepBackText != null) stepBackText.gameObject.SetActive(true);
                    // print("Initialising step back...");
                }

                if (StaticVariableManager.isVideoTrainingComplete)
                {
                    vp.Pause();
                    countDownStart.start_training = false;
                    isVideoPlaying = false;
                }

            }
            else
            {
                // print("Video not playing...");
                // Execute stepback
                if (StaticVariableManager.initializeStepBack && !StaticVariableManager.isLastVideoNTS)
                {
                    stepBackTimer -= Time.deltaTime;
                    // print(stepBackTimer.ToString("0"));

                    if (stepBackTimer > 8f) { stepBackText.text = "You were killed"; }
                    if (stepBackTimer > 5.5f && stepBackTimer <= 8f) { stepBackText.text = "Standby"; }
                    if (stepBackTimer <= 5.5f && stepBackTimer >= 1) { stepBackText.text = stepBackTimer.ToString("0"); }

                    if (stepBackTimer <= 0.5f)
                    {
                        // Go one step back from current video index
                        if (currentVideoIndex > 0)
                            currentVideoIndex --;
                        else
                            currentVideoIndex = -1;

                        ApplyVideoTransition();
                        vp.Play();
                        vp.playbackSpeed = 1f; // normal speed
                        isVideoPlaying = true;
                        stepBackTimer = stepBackTimerSet;
                        StaticVariableManager.initializeStepBack = false;
                        if (deadShader != null) deadShader.SetActive(false);
                        if (stepBackText != null) stepBackText.gameObject.SetActive(false);

                    }
                }
            }
        }

    }

    

    private void ExportJSON()
    {
        if (vp == null || vp.clip == null)
        {
            Debug.LogWarning("No video clip assigned to the VideoPlayer. Cannot export tracking data.");
            return;
        }

        // Save using the current video clip name
        string clipName = StaticVariableManager.currentSetVideo;
        string path = trackingFolder + clipName + " Tracking.json";

        File.WriteAllText(path, JsonConvert.SerializeObject(trackingData, Formatting.Indented));
        Debug.Log($"Saved tracking for clip '{clipName}' to {path}");
    }


    private void TrackVideo()
    {
        if (vp == null || !vp.isPrepared)
            return;

        // print($"isVideoFinished: {isVideoFinished}");
        // print($"vp.isPlaying: {vp.isPlaying}");
        // print($"vp.time: {vp.time}");

        if (!isVideoFinished && !vp.isPlaying && vp.time > 0)
        {
            // Freeze current video.
            if(freezeFrame)
            {
                frameFreezeTimer -= Time.deltaTime;
                // print(frameFreezeTimer.ToString("0"));

                if (frameFreezeTimer <= 0)
                {
                    freezeFrame = false;
                }
            }

            // Apply video conditions.
            if (!freezeFrame) HandleGroupCondition();

        }
    }

    private void HandleGroupCondition()
    {
        List<string> nonThreatVideos;
        List<string> preEndVideos;
        List<string> InbetweenEndVideos;
        List<string> nonLethalVideos;

        switch (StaticVariableManager.currentVideoGroup)
        {
            case "G13":
                nonThreatVideos = new List<string>() { "G13P2_Video1", "G13P2_Video2" }; // Non threat videos that do not require shooting.
                preEndVideos = new List<string>() { "G13P10_Video1", "G13P10_Video2" }; // Video before the last video
                InbetweenEndVideos = new List<string>() { "G13P9_Video2" }; // " Last video between the normal size of player. Video before the expected end.
                nonLethalVideos = new List<string>() { "G13P1_Video1", "G13P1_Video1", "G13P2_Video2" };

                if (!nonLethalVideos.Contains(StaticVariableManager.currentSetVideo) && freezeComplete == true 
                    && !isTrainingEnd() && !InbetweenEndVideos.Contains(StaticVariableManager.currentSetVideo))
                {
                    freezeFrame = true;
                    freezeComplete = false;
                }
                else if (InbetweenEndVideos.Contains(StaticVariableManager.currentSetVideo))
                {
                    StaticVariableManager.isTrainingEndReached = true;
                    StaticVariableManager.isVideoTrainingComplete = true;
                }
                else
                {
                    if (isTrainingEnd() == false && !nonThreatVideos.Contains(StaticVariableManager.currentSetVideo))
                    {
                        StaticVariableManager.initializeStepBack = true;
                        VideoOptionSelector.stepBackInitialised = true; // Set step back on video selector
                    }
                    else if (nonThreatVideos.Contains(StaticVariableManager.currentSetVideo))
                    {
                        ApplyVideoTransition();
                    }
                    else if (preEndVideos.Contains(StaticVariableManager.currentSetVideo) || isTrainingEnd())
                    {
                        StaticVariableManager.isTrainingEndReached = true;
                        StaticVariableManager.isVideoTrainingComplete = true;
                        if (preEndVideos.Contains(StaticVariableManager.currentSetVideo))
                            StaticVariableManager.currVideoThreats -= 1; // Substract the remaining number of threats
                                                                         // print("Is video end reached...");
                    }
                    else
                    {
                        StaticVariableManager.isVideoTrainingComplete = true;
                    }

                    ResetFreezeTimer(); // Reset Freeze timer.
                }


                break;
        }

    }

    private void ResetFreezeTimer()
    {
        frameFreezeTimer = frameFreezeSet; // Reset Freeze timer.
        freezeComplete = true; // Reset Freeze.
    }

    public void ApplyVideoTransition()
    {
        // 1. Define videos that require repetition (Bulletproof Vest logic)

        // 2. Check if we are at the end of the list
        if (currentVideoIndex >= videoPlayers.Count - 1)
        {
            StaticVariableManager.isTrainingEndReached = true;
            StaticVariableManager.isVideoTrainingComplete = true;
            return;
        }

        // 3. Reset state for current transition
        vp.Pause();
        ResetMarkers();
        LastVideoCheck();
        ResetFreezeTimer(); // Reset Freeze timer.

        if (!StaticVariableManager.isVideoTrainingComplete)
        {
            // 4. Determine Next Index: Repeat if Bulletproof, otherwise increment
            int nextIndex = currentVideoIndex;
            // Set the current Group position
            string currVGroupPos = GetPrefix(StaticVariableManager.currentSetVideo);
            // Chech if the repeating videos assigned match the position of the current video


            // print("Group position: " + currVGroupPos);
            bool isRepeating = vestRepeatVideos.Contains(currVGroupPos); 

            if (!isRepeating)
            {
                nextIndex = currentVideoIndex + 1;
            }
            else
            {
                print("Bulletproof video detected - Repeating current index.");
            }

            // Update index if headshop from hardtarget
            if (StaticVariableManager.isHeadShot && hardTargetVideos.Contains(currVGroupPos))
            {
                nextIndex++;
            }

            // 5. Update Global Reference and State
            currentVideoIndex = nextIndex;
            vp = videoPlayers[currentVideoIndex]; // Switch reference BEFORE preparing

            StaticVariableManager.isThreatVideo = SetThreatStatus();
            AssignRandomClipToCurrentPlayer(); // Assumes this uses currentVideoIndex or vp
            LoadTrackingForVideo(currentVideoIndex);

            // 6. Reset Frame and Prepare
            // Calling Stop() ensures the VideoPlayer internal clock is completely reset to 0
            vp.Stop();

            // Prevent duplicate event subscriptions
            vp.prepareCompleted -= OnVideoPrepared;
            vp.prepareCompleted += OnVideoPrepared;

            isMarkersEnabled = false;

            // 7. Initialize playback
            // If it's the same video, we still call Prepare to ensure a fresh start
            vp.Prepare();

            // Note: setting frame = 0 here is a safety net, but Stop() does the heavy lifting
            vp.frame = 0;
        }
    }

    private string GetPrefix(string inputString)
    {
        if (string.IsNullOrEmpty(inputString)) return "";

        // Split the string at the underscore and take the first part
        string[] parts = inputString.Split('_');
        return parts[0];
    }

    private void LastVideoCheck()
    {
        if(currentVideoIndex == videoPlayers.Count - 2) // If current video is the second last video.
        {
            // print("Checking last video!!");
            // Check if target hit is Threat or Non Threat
            if (StaticVariableManager.isLastVideoNTS == true)
            {
                // print("None Threat Shot");
                // Mission failed. Stop scenario.
                // StaticVariableManager.isTrainingEndReached = true;
                StaticVariableManager.isVideoTrainingComplete = true;
            }
        }
        else 
        {
            // print("Threat Shot");
            StaticVariableManager.isLastVideoNTS = false;
        }
    }

    private void OnVideoPrepared(VideoPlayer preparedPlayer)
    {
        preparedPlayer.prepareCompleted -= OnVideoPrepared; // unsubscribe
        vp.Play();
    }

    private bool SetThreatStatus()
    {
        bool isThreat = true;
        StaticVariableManager.currVideoThreats = 1;

        nonThreatIndexes = new List<int> { 3, 6, 8, 15, 19, 21, 27, 30, 31 };
        if (nonThreatIndexes.Contains(currentVideoIndex)) isThreat = false;

        switch (StaticVariableManager.currentVideoGroup)
        {
            case "G5": StaticVariableManager.currVideoThreats = 2; break;
            case "G7": StaticVariableManager.currVideoThreats = 3; break;
            case "G9":
                if (StaticVariableManager.currentSetVideo == "G9_Video1") StaticVariableManager.currVideoThreats = 2;
                if (StaticVariableManager.currentSetVideo == "G9_Video2") StaticVariableManager.currVideoThreats = 1;
                break;
            case "G10": StaticVariableManager.currVideoThreats = 1; actionVideoSpeed = 0.87f; break;
            case "G11": StaticVariableManager.currVideoThreats = 3; break;
            case "G13": StaticVariableManager.currVideoThreats = 8; actionVideoSpeed = .85f; break;
        }

        return isThreat;
    }

    public static void ManageStartAndPause()
    {
        if (!isVideoPlaying)
        {
            vp.Play();
            isVideoPlaying = true;
        }
        else
        {
            vp.Pause();
            isVideoPlaying = false;
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        StaticVariableManager.isTrainingEndReached = true;
        StaticVariableManager.isVideoTrainingComplete = true;
    }

    private void SaveCurrentFrame()
    {
        if (vp == null || vp.clip == null || vp.frame < 0)
            return;

        string clipName = StaticVariableManager.currentSetVideo;
        string jsonPath = trackingFolder + clipName + " Tracking.json";

        // 🔒 ALWAYS load tracking data for THIS clip only
        if (File.Exists(jsonPath))
        {
            trackingData = JsonConvert.DeserializeObject<TVideoTrackingData>(
                File.ReadAllText(jsonPath)
            );
        }
        else
        {
            trackingData = new TVideoTrackingData();
        }

        TVideoMarkers markers = GetMarkersForCurrentClip();
        if (markers == null)
        {
            Debug.LogWarning($"No markers found for clip '{clipName}'");
            return;
        }

        long currentFrame = vp.frame;

        // Remove existing frame entry for this frame
        trackingData.frames.RemoveAll(f => f.frame == currentFrame);

        TFrameData frameData = new TFrameData { frame = currentFrame };

        for (int i = 0; i < markers.headMarkers.Count; i++)
        {
            if (!markers.headMarkers[i].gameObject.activeInHierarchy ||
                !markers.chestMarkers[i].gameObject.activeInHierarchy)
                continue;

            frameData.tracks.Add(new TTrackData
            {
                id = i,
                head = markers.headMarkers[i].position,
                chest = markers.chestMarkers[i].position,
                headRotation = markers.headMarkers[i].rotation.eulerAngles,
                chestRotation = markers.chestMarkers[i].rotation.eulerAngles,
                headScale = markers.headMarkers[i].localScale,
                chestScale = markers.chestMarkers[i].localScale
            });
        }

        if (frameData.tracks.Count == 0)
            return;

        trackingData.frames.Add(frameData);

        // ✅ WRITE ONLY ONE FILE — FOR THIS CLIP
        File.WriteAllText(
            jsonPath,
            JsonConvert.SerializeObject(trackingData, Formatting.Indented)
        );

        Debug.Log($"Saved frame {currentFrame} for clip '{clipName}' ({frameData.tracks.Count} markers)");
    }


    private TVideoMarkers GetMarkersForCurrentClip()
    {
        if (vp == null || vp.clip == null)
            return null;

        return videoMarkersList.Find(m => m.videoName == StaticVariableManager.currentSetVideo);
    }



    private void HandleMarkerDragging()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);


        // Select marker (LEFT = move, RIGHT = scale)
        if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 🔍 Find which TVideoMarkers this marker belongs to
                foreach (var markers in videoMarkersList)
                {
                    if (markers.headMarkers.Contains(hit.transform) ||
                        markers.chestMarkers.Contains(hit.transform))
                    {
                        selectedMarker = hit.transform;
                        isMoving = Mouse.current.leftButton.wasPressedThisFrame;
                        isScaling = Mouse.current.rightButton.wasPressedThisFrame;

                        dragOffset = selectedMarker.position -
                            ray.GetPoint(Vector3.Distance(Camera.main.transform.position, hit.point));

                        break;
                    }
                }
            }
        }

        // Move X,Y
        if (isMoving && selectedMarker != null && Mouse.current.leftButton.isPressed)
        {
            Plane plane = new Plane(-Camera.main.transform.forward, selectedMarker.position);
            if (plane.Raycast(ray, out float enter))
            {
                Vector3 worldPos = ray.GetPoint(enter) + dragOffset;
                selectedMarker.position = new Vector3(
                    worldPos.x,
                    worldPos.y,
                    selectedMarker.position.z
                );
            }
        }

        // Scale
        if (isScaling && selectedMarker != null && Mouse.current.rightButton.isPressed)
        {
            float deltaY = Mouse.current.delta.ReadValue().y * 0.01f;
            Vector3 newScale = selectedMarker.localScale;
            newScale.x += deltaY * 1.5f;
            newScale.z += deltaY;
            selectedMarker.localScale = newScale;
        }

        // Release
        if (Mouse.current.leftButton.wasReleasedThisFrame ||
            Mouse.current.rightButton.wasReleasedThisFrame)
        {
            isMoving = false;
            isScaling = false;
            selectedMarker = null;
        }
    }


    private void HandleScrubbing()
    {
        var keyboard = Keyboard.current;
        if (!isVideoPlaying)
        {
            bool forwardHeld = keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed;
            bool backwardHeld = keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed;

            if (forwardHeld != backwardHeld)
            {
                scrubAccumulator += scrubFPS * Time.deltaTime;
                int framesToMove = Mathf.FloorToInt(scrubAccumulator);
                if (framesToMove > 0)
                {
                    scrubAccumulator -= framesToMove;
                    long frameCount = (vp.frameCount > 0) ? (long)vp.frameCount : long.MaxValue;

                    if (forwardHeld)
                    {
                        if (!vp.isPrepared) vp.Prepare();
                        else
                        {
                            for (int i = 0; i < framesToMove; i++)
                            {
                                if (vp.frame >= (frameCount - 1)) break;
                                vp.StepForward();
                            }
                        }
                    }
                    else
                    {
                        long targetFrame = vp.frame - framesToMove;
                        if (targetFrame < 0) targetFrame = 0;
                        vp.frame = targetFrame;

                        // Force the player to render the newly selected frame
                        vp.Play();
                        vp.Pause();
                    }
                    UpdateMarkersForFrame(vp.frame);
                }
            }
            else scrubAccumulator = 0f;
        }
        else scrubAccumulator = 0f;
    }



    private void LoadTrackingForVideo(int videoIndex)
    {
        if (vp == null || vp.clip == null)
            return;

        TVideoMarkers markers = GetMarkersForCurrentClip();
        if (markers == null)
        {
            Debug.LogWarning($"No markers found for clip '{StaticVariableManager.currentSetVideo}'");
            trackingData = new TVideoTrackingData();
            return;
        }

        currentJsonPath = trackingFolder + StaticVariableManager.currentSetVideo + " Tracking.json";

        // print(currentJsonPath);

        if (markers.headMarkers.Count > 0)
            headStartPosition = markers.headMarkers[0].position;

        if (markers.chestMarkers.Count > 0)
            chestStartPosition = markers.chestMarkers[0].position;

        if (File.Exists(currentJsonPath))
        {
            trackingData = JsonConvert.DeserializeObject<TVideoTrackingData>(
                File.ReadAllText(currentJsonPath)
            );
            // print("File exists...");
        }
        else
        {
            trackingData = new TVideoTrackingData();
        }
    }


    private void UpdateMarkersForFrame(long frame)
    {
        vp.playbackSpeed = 1f; // normal speed

        if (vp == null || vp.clip == null)
            return;

        string clipName = StaticVariableManager.currentSetVideo;

        TVideoMarkers markers = videoMarkersList
            .Find(m => m.videoName == clipName);

        if (markers == null)
            return;

        TFrameData f = trackingData.frames.Find(x => x.frame == frame);
        if (f == null || f.tracks.Count == 0)
            return;

        StaticVariableManager.isStartVideoTime = true;

        bool isThreatFrame = false; // 🔑 NEW

        // Hide start markers
        foreach (var m in markers.startMarker)
            m.gameObject.SetActive(false);

        // 🔄 Reset markers
        for (int i = 0; i < markers.headMarkers.Count; i++)
        {
            markers.headMarkers[i].gameObject.SetActive(false);
            markers.chestMarkers[i].gameObject.SetActive(false);

            var hCol = markers.headMarkers[i].GetComponent<MeshCollider>();
            var cCol = markers.chestMarkers[i].GetComponent<MeshCollider>();

            if (hCol) hCol.enabled = false;
            if (cCol) cCol.enabled = false;
        }

        // ✅ Enable & update ONLY markers that exist in this frame
        foreach (var track in f.tracks)
        {
            int id = track.id;

            if (id < 0 ||
                id >= markers.headMarkers.Count ||
                id >= markers.chestMarkers.Count)
                continue;

            Transform head = markers.headMarkers[id];
            Transform chest = markers.chestMarkers[id];

            // 🔑 CHECK NAME FOR THREAT
            // (You can check either head or chest — assuming same naming)
            if (!head.name.Contains("None Threat"))
            {
                isThreatFrame = true;
            }

            // Activate
            if (!head.gameObject.activeSelf)
                head.gameObject.SetActive(true);

            if (!chest.gameObject.activeSelf)
                chest.gameObject.SetActive(true);

            // Position
            head.position = track.head;
            chest.position = track.chest;

            // Rotation
            if (track.headRotation != Vector3.zero)
                head.rotation = Quaternion.Euler(track.headRotation);

            if (track.chestRotation != Vector3.zero)
                chest.rotation = Quaternion.Euler(track.chestRotation);

            // Scale
            head.localScale = track.headScale;
            chest.localScale = track.chestScale;

            // Colliders
            var headCol = head.GetComponent<MeshCollider>();
            var chestCol = chest.GetComponent<MeshCollider>();

            if (headCol) headCol.enabled = true;
            if (chestCol) chestCol.enabled = true;
        }

        // 🔥 APPLY SPEED AFTER PROCESSING MARKERS
        // print("Setting play back speed...");
        if (isThreatFrame)
        {
            vp.playbackSpeed = actionVideoSpeed; // slow down
        }
        else
        {
            vp.playbackSpeed = 1f; // normal speed
        }

        isMarkersEnabled = true;
    }

    private void ResetMarkers()
    {
        foreach (var vm in videoMarkersList)
        {
            foreach (var m in vm.headMarkers)
            {
                m.gameObject.SetActive(false);
                m.GetComponent<MeshRenderer>().enabled = false;

                // Disable the collider pair as well
                GameObject colliderPair = m.GetComponent<TargetMessageReceive>().pair;
                if (colliderPair != null) colliderPair.GetComponent<MeshRenderer>().enabled = false;
            }
            foreach (var m in vm.chestMarkers)
            {
                m.gameObject.SetActive(false);
                m.GetComponent<MeshRenderer>().enabled = false;

                // Disable the collider pair as well
                GameObject colliderPair = m.GetComponent<TargetMessageReceive>().pair;
                if (colliderPair != null) colliderPair.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
    private bool isTrainingEnd()
    {
        bool isTrainingEnd = false;

        if(currentVideoIndex == videoPlayers.Count - 1)
        {
            isTrainingEnd = true;
        }
        else
        {
            isTrainingEnd = false;
        }


        return isTrainingEnd;
    }

}
