using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.IO;
using Newtonsoft.Json;
using System;

[System.Serializable]
public class TrackData
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

    [JsonIgnore]  // <-- Added this to prevent duplicate JSON entries
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
public class FrameData
{
    public long frame;
    public List<TrackData> tracks = new List<TrackData>();
}

[System.Serializable]
public class VideoTrackingData
{
    public List<FrameData> frames = new List<FrameData>();
}

[System.Serializable]
public class VideoMarkers
{
    public string videoName;
    public List<Transform> headMarkers = new List<Transform>();
    public List<Transform> chestMarkers = new List<Transform>();
    public List<Transform> startMarker = new List<Transform>();
}

public class ManualVideoTracker : MonoBehaviour
{
    public int currentVideoIndex = 0; // Video index

    [Header("Video Players")]
    public List<VideoPlayer> videoPlayers = new List<VideoPlayer>();
    private List<int> currActiveIndexes = new List<int>();
    private List<int> nonThreatIndexes = new List<int>();
    [Header("Markers Per Video")]
    public List<VideoMarkers> videoMarkersList = new List<VideoMarkers>();

    private VideoTrackingData trackingData = new VideoTrackingData();
    private string trackingFolder;
    private string currentJsonPath;

    public static bool isVideoPlaying = false;
    public static VideoPlayer vp;

    // Drag & scale state
    private Transform selectedMarker = null;
    private bool isMoving = false;
    private bool isScaling = false;
    private bool isSettingUp = false;
    private Vector3 dragOffset;

    // scrubbing
    private float scrubAccumulator = 0f;
    private float scrubFPS = 20f; // how many frames per second to move while key held
    private float actionVideoSpeed = 0.72f; // 30% slower than normal speed

    private Vector3 headStartPosition;
    private Vector3 chestStartPosition;

    void Start()
    {
        isVideoPlaying = false;
        vp = null;

        // Create tracking folder if it doesn't exist
        trackingFolder = FileManager.gamePath + "Video Tracking/";
        if (!Directory.Exists(trackingFolder))
            Directory.CreateDirectory(trackingFolder);

        // Disable all markers initially
        foreach (var vm in videoMarkersList)
        {
            foreach (var m in vm.headMarkers) m.gameObject.SetActive(false);
            foreach (var m in vm.chestMarkers) m.gameObject.SetActive(false);
        }

        // --- Hook video end event for each player ---
        foreach (var vp in videoPlayers)
        {
            if (vp != null)
            {
                vp.loopPointReached += OnVideoEnd;
            }
        }

        // Load first video tracking if exists
        SetCurrentindex();
        StaticVariableManager.isThreatVideo = SetThreatStatus();
        LoadTrackingForVideo(currentVideoIndex);
        vp = videoPlayers[currentVideoIndex];
        vp.Play();
        vp.Pause();
    }

    void Update()
    {
        var keyboard = Keyboard.current;

        // isSettingUp = true;
        if(Keyboard.current.leftShiftKey.isPressed || isSettingUp)
        {
            // --- Play / Pause toggle (Shift+P) ---
            if (keyboard.pKey.wasPressedThisFrame)
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

            // --- Marker dragging ---
            HandleMarkerDragging();

            // --- Save current frame with S ---
            if (keyboard.sKey.wasPressedThisFrame)
            {
                SaveCurrentFrame();
            }

            // --- Export JSON with Shift+E ---
            if (keyboard.eKey.wasPressedThisFrame)
            {
                ExportJSON();
            }

            // --- Manual scrubbing (paused) ---
            if (!isVideoPlaying)
            {
                bool forwardHeld = keyboard.rightArrowKey.isPressed;
                bool backwardHeld = keyboard.leftArrowKey.isPressed;

                if (forwardHeld != backwardHeld) // only one direction
                {
                    scrubAccumulator += scrubFPS * Time.deltaTime;
                    int framesToMove = Mathf.FloorToInt(scrubAccumulator);

                    if (framesToMove > 0)
                    {
                        scrubAccumulator -= framesToMove;

                        long frameCount = (vp.frameCount > 0) ? (long)vp.frameCount : long.MaxValue;

                        if (forwardHeld)
                        {
                            // Forward: use StepForward repeatedly (more reliable than vp.frame +=)
                            // Ensure player is prepared
                            if (!vp.isPrepared)
                            {
                                vp.Prepare();
                            }
                            else
                            {
                                // Limit how many steps we actually do so we don't pass the end
                                for (int i = 0; i < framesToMove; i++)
                                {
                                    // stop if we're already at last frame
                                    if (vp.frame >= (frameCount > 0 ? frameCount - 1L : long.MaxValue)) break;

                                    vp.StepForward();
                                }
                            }
                        }
                        else // backwardHeld
                        {
                            // Backward: set frame directly (clamp safely)
                            long currentFrame = vp.frame;
                            long targetFrame = currentFrame - framesToMove;
                            if (targetFrame < 0) targetFrame = 0;
                            vp.frame = targetFrame;

                            // Force the player to render the newly selected frame.
                            // Some platforms require a Play()->Pause() to update the texture immediately.
                            vp.Play();
                            vp.Pause();
                        }

                        // Update markers for the new frame (vp.frame will reflect the stepped/assigned frame)
                        UpdateMarkersForFrame(vp.frame);
                    }
                }
                else
                {
                    // no scrubbing (or both keys pressed) -> reset accumulator
                    scrubAccumulator = 0f;
                }
            }
            else
            {
                // if playing normally, reset accumulator
                scrubAccumulator = 0f;
            }

        }



        if (countDownStart.start_training && !isVideoPlaying)
        {
            // vp.playbackSpeed = 0.8f;  // 20% slower than normal speed
            vp.Play();
            isVideoPlaying = true;
        }



        // --- Update markers when video is playing ---
        if (isVideoPlaying && trackingData.frames.Count > 0 && vp.frame >= 0 && !StaticVariableManager.isStopTraining) // && !StaticVariableManager.isStopTraining
        {
            UpdateMarkersForFrame(vp.frame);
        }


    }

    private void SetCurrentindex()
    {
        int indexCount = 0;
        int randomMainIndex = 0;

        foreach(VideoPlayer player in videoPlayers)
        {
            if(player.clip.name.Contains(StaticVariableManager.currentVideoGroup))
            {
                // print(player.clip.name + " and index " + indexCount);
                currActiveIndexes.Add(indexCount); // Allocate video for the selected group.
            }

            indexCount++;
        }

        do
        {
            randomMainIndex = currActiveIndexes[Random.Range(0, currActiveIndexes.Count - 1)]; // Set the video index
        } while (randomMainIndex == StaticVariableManager.previousIndex);

        StaticVariableManager.previousIndex = randomMainIndex;
        currentVideoIndex = randomMainIndex;

        // currentVideoIndex = 37;
        if (currentVideoIndex == 24) currentVideoIndex = 23;


    }
    private bool SetThreatStatus()
    {
        bool isThreat = false;
        StaticVariableManager.currVideoThreats = 1; // Reset

        // Set Non threat videos (Video where no shooting is required)
        nonThreatIndexes.Add(3);
        nonThreatIndexes.Add(6);
        nonThreatIndexes.Add(8);
        nonThreatIndexes.Add(15);
        nonThreatIndexes.Add(15);
        nonThreatIndexes.Add(19);
        nonThreatIndexes.Add(21);
        nonThreatIndexes.Add(27);
        nonThreatIndexes.Add(30);
        nonThreatIndexes.Add(31);

        if (nonThreatIndexes.Contains(currentVideoIndex)) // current index links to a non threat video.
        {
            isThreat = false;
        }
        else
        {
            isThreat = true;
        }

        // -------------- Set number of threats
        // StaticVariableManager.currentVideoGroup = "G10";
        switch (StaticVariableManager.currentVideoGroup)
        {
            case "G5":
                StaticVariableManager.currVideoThreats = 2;
                break;

            case "G7":
                StaticVariableManager.currVideoThreats = 3;
                break;
            case "G9":
                if (videoPlayers[currentVideoIndex].clip.name == "G9_Video1")
                {
                    StaticVariableManager.currVideoThreats = 2;
                }
                if (videoPlayers[currentVideoIndex].clip.name == "G9_Video2")
                {
                    StaticVariableManager.currVideoThreats = 1;
                }
                break;
            case "G10":
                StaticVariableManager.currVideoThreats = 1;
                actionVideoSpeed = 0.87f;
                break;
            case "G11":
                StaticVariableManager.currVideoThreats = 3;
                break;

        }

        // print("Is threat video: " + isThreat);
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

    // --- NEW: Video end handler ---
    private void OnVideoEnd(VideoPlayer vp)
    {
        // Debug.Log($"Video '{vp.clip.name}' ended at frame {vp.frame}.");
        // StaticVariableManager.isStartVideoTime = false;
        StaticVariableManager.isTrainingEndReached = true;
        StaticVariableManager.isVideoTrainingComplete = true;
        // isVideoPlaying = false;

        // Optionally update markers at last frame
        // UpdateMarkersForFrame(vp.frame);

        // Optional: auto move to next video
        // currentVideoIndex = (currentVideoIndex + 1) % videoPlayers.Count;
        // LoadTrackingForVideo(currentVideoIndex);
        // videoPlayers[currentVideoIndex].Play();
    }


    private void SaveCurrentFrame()
    {
        VideoMarkers markers = videoMarkersList[currentVideoIndex];
        VideoPlayer vp = videoPlayers[currentVideoIndex];
        if (vp.frame < 0) return;

        long currentFrame = vp.frame;

        // Remove existing frame data (so we don't duplicate)
        trackingData.frames.RemoveAll(f => f.frame == currentFrame);

        FrameData frameData = new FrameData();
        frameData.frame = currentFrame;

        for (int i = 0; i < markers.headMarkers.Count; i++)
        {
            TrackData t = new TrackData
            {
                id = i,
                head = markers.headMarkers[i].position,
                chest = markers.chestMarkers[i].position,

                // ✅ Convert Quaternion to Euler angles
                headRotation = markers.headMarkers[i].rotation.eulerAngles,
                chestRotation = markers.chestMarkers[i].rotation.eulerAngles,

                headScale = markers.headMarkers[i].localScale,
                chestScale = markers.chestMarkers[i].localScale
            };
            frameData.tracks.Add(t);
        }

        trackingData.frames.Add(frameData);
        Debug.Log($"Saved frame {currentFrame} for video '{markers.videoName}' with {markers.headMarkers.Count} markers.");
    }


    private void HandleMarkerDragging()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        // Left click: move marker
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                VideoMarkers markers = videoMarkersList[currentVideoIndex];
                if (markers.headMarkers.Contains(hit.transform) || markers.chestMarkers.Contains(hit.transform))
                {
                    selectedMarker = hit.transform;
                    isMoving = true;
                    isScaling = false;

                    Vector3 worldPos = ray.GetPoint(Vector3.Distance(Camera.main.transform.position, hit.point));
                    dragOffset = selectedMarker.position - worldPos;
                }
            }
        }

        // Right click: scale marker
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                VideoMarkers markers = videoMarkersList[currentVideoIndex];
                if (markers.headMarkers.Contains(hit.transform) || markers.chestMarkers.Contains(hit.transform))
                {
                    selectedMarker = hit.transform;
                    isScaling = true;
                    isMoving = false;
                }
            }
        }

        // Move marker while left held (X,Y only)
        if (isMoving && Mouse.current.leftButton.isPressed && selectedMarker != null)
        {
            Plane plane = new Plane(-Camera.main.transform.forward, selectedMarker.position);
            if (plane.Raycast(ray, out float enter))
            {
                Vector3 worldPos = ray.GetPoint(enter) + dragOffset;
                selectedMarker.position = new Vector3(worldPos.x, worldPos.y, selectedMarker.position.z);
            }
        }

        // Scale marker while right held
        if (isScaling && Mouse.current.rightButton.isPressed && selectedMarker != null)
        {
            float deltaY = Mouse.current.delta.ReadValue().y * 0.01f;
            if (Mathf.Abs(deltaY) > 0.0001f)
            {
                Vector3 newScale = selectedMarker.localScale;
                newScale.x += deltaY * 1.5f;
                newScale.z += deltaY ;
                selectedMarker.localScale = newScale;
            }
        }

        // Release resets mode
        if (Mouse.current.leftButton.wasReleasedThisFrame || Mouse.current.rightButton.wasReleasedThisFrame)
        {
            isMoving = false;
            isScaling = false;
            selectedMarker = null;
        }
    }

    private void ExportJSON()
    {
        VideoMarkers markers = videoMarkersList[currentVideoIndex];
        string path = trackingFolder + markers.videoName + " Tracking.json";
        string json = JsonConvert.SerializeObject(trackingData, Formatting.Indented);
        File.WriteAllText(path, json);
        Debug.Log($"Saved tracking for '{markers.videoName}' to {path}");
    }


    private void LoadTrackingForVideo(int videoIndex)
    {
        if (videoIndex < 0 || videoIndex >= videoMarkersList.Count) return;

        VideoMarkers markers = videoMarkersList[videoIndex];
        currentJsonPath = trackingFolder + markers.videoName + " Tracking.json";

        // Save starting positions (not transforms)
        headStartPosition = markers.headMarkers[0].position;
        chestStartPosition = markers.chestMarkers[0].position;

        if (File.Exists(currentJsonPath))
        {
            string json = File.ReadAllText(currentJsonPath);
            trackingData = JsonConvert.DeserializeObject<VideoTrackingData>(json);
        }
        else
        {
            trackingData = new VideoTrackingData();
        }
    }

    private void UpdateMarkersForFrame(long frame)
    {
        VideoMarkers markers = videoMarkersList[currentVideoIndex];

        // Find frame data
        FrameData f = trackingData.frames.Find(x => x.frame == frame);
        if (f != null && f.tracks.Count > 0)
        {
            for (int i = 0; i < f.tracks.Count && i < markers.headMarkers.Count; i++)
            {
                StaticVariableManager.isStartVideoTime = true;
                vp.playbackSpeed = actionVideoSpeed;

                // Hide start markers when tracking begins
                foreach (var m in markers.startMarker)
                    m.gameObject.SetActive(false);

                // --- Enable colliders only after movement from start position ---
                if (Vector3.Distance(markers.headMarkers[i].position, headStartPosition) > 0.001f)
                    markers.headMarkers[i].GetComponent<MeshCollider>().enabled = true;

                if (Vector3.Distance(markers.chestMarkers[i].position, chestStartPosition) > 0.001f)
                    markers.chestMarkers[i].GetComponent<MeshCollider>().enabled = true;

                // Enable active markers
                markers.headMarkers[i].gameObject.SetActive(true);
                markers.chestMarkers[i].gameObject.SetActive(true);

                // --- Apply position ---
                markers.headMarkers[i].position = f.tracks[i].head;
                markers.chestMarkers[i].position = f.tracks[i].chest;

                // --- Apply rotation ---
                if (f.tracks[i].headRotation != Vector3.zero)
                    markers.headMarkers[i].rotation = Quaternion.Euler(f.tracks[i].headRotation);
                if (f.tracks[i].chestRotation != Vector3.zero)
                    markers.chestMarkers[i].rotation = Quaternion.Euler(f.tracks[i].chestRotation);

                // --- Apply scale ---
                markers.headMarkers[i].localScale = f.tracks[i].headScale;
                markers.chestMarkers[i].localScale = f.tracks[i].chestScale;
            }
        }
    }



}
