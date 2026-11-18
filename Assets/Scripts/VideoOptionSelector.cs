using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

[RequireComponent(typeof(VideoPlayer))]
public class VideoOptionSelector : MonoBehaviour
{
    [Tooltip("All possible video clips for this player slot")]
    public List<VideoClip> videoOptions = new List<VideoClip>();
    public static bool stepBackInitialised = false;
    private VideoPlayer vp;

    private void Awake()
    {
        vp = GetComponent<VideoPlayer>();
    }

    /// <summary>
    /// Assigns a random clip to the VideoPlayer.
    /// MUST be called before any logic that reads vp.clip.
    /// </summary>
    public void SelectRandomVideo()
    {
        if (videoOptions == null || videoOptions.Count == 0)
        {
            Debug.LogError($"[VideoOptionSelector] No video options assigned on {gameObject.name}");
            return;
        }

        int videoIndex = 0;
        if (stepBackInitialised)
        {
            // use the same index
            videoIndex = StaticVariableManager.currSetVideoIndex;

            stepBackInitialised = false;
        }
        else
        {
            videoIndex = Random.Range(0, videoOptions.Count); // Assign random index
            StaticVariableManager.currSetVideoIndex = videoIndex;
        }

        vp.clip = videoOptions[videoIndex];
        StaticVariableManager.currentSetVideo = vp.clip.name.ToString();
        vp.Prepare();
    }
}
