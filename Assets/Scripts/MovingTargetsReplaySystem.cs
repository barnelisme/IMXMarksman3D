using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;

public class MovingTargetsReplaySystem : MonoBehaviour
{

    public Camera thisCam;

    public float delayBetweenFrames = 0.05f;
    private List<Texture2D> frames = new List<Texture2D>();
    private bool isRecording = false;
    private bool isPlaying = false;
    public Renderer quadRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thisCam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.rKey.isPressed)
        {
            StartRecording();
        }

        if (Keyboard.current.sKey.isPressed)
        {
            StopRecording();
        }

        if (Keyboard.current.pKey.isPressed)
        {
            StartPlayBack();
        }

    }

    void LateUpdate()
    {
        RecordFrame();
    }

    void StartRecording()
    {
        frames.Clear();
        isRecording = true;
    }

    void StopRecording()
    {
        isRecording = false;
    }

    void RecordFrame()
    {
        if(isRecording)
        {
            Texture2D frame = CaptureFrame();
            frames.Add(frame);
        }
    }

    Texture2D CaptureFrame()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = thisCam.targetTexture;

        Texture2D frame = new Texture2D(thisCam.targetTexture.width, thisCam.targetTexture.height);
        frame.ReadPixels(new Rect(0, 0, thisCam.targetTexture.width, thisCam.targetTexture.height), 0, 0);
        frame.Apply();

        RenderTexture.active = currentRT;

        return frame;
    }

    IEnumerator PlayBack()
    {
        isPlaying = true;
        
        for(int i = 0; i < frames.Count; i++)
        {
            DisplayFrame(frames[i]);
            yield return new WaitForSeconds(delayBetweenFrames);
        }

        isPlaying = false;
    }

    void DisplayFrame(Texture2D frame)
    {
        quadRenderer.material.mainTexture = frame;
    }

    void StartPlayBack()
    {
        if(isPlaying == false && frames.Count > 0)
        {
            StartCoroutine(PlayBack());
        }
    }

}
