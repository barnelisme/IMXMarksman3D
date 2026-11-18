using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem; // new Input System

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // Disable autoPlay so it doesn't start immediately
        videoPlayer.playOnAwake = false;
    }

    void Update()
    {
        // Play video when "P" is pressed
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (!videoPlayer.isPlaying)
                videoPlayer.Play();
        }

        // Stop video when "S" is pressed
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            if (videoPlayer.isPlaying)
                videoPlayer.Stop();
        }
    }
}
