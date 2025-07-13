/*using System;
using System.IO;
using UnityEngine;
using Vosk;

public class VoskDemo : MonoBehaviour
{
    private VoskRecognizer recognizer;
    private Model model;

    void Start()
    {
        // Set logging level for Vosk (0 to disable logs)
        Vosk.Vosk.SetLogLevel(0);

        // Path to the Vosk model in StreamingAssets folder
        string modelPath = Path.Combine(Application.streamingAssetsPath, "vosk-model");

        // Initialize the Vosk model
        model = new Model(modelPath);

        // Demo with byte buffer
        DemoBytes(model);

        // Demo with float buffer
        DemoFloats(model);

        // Demo with speaker model
        DemoSpeaker(model);
    }

    public void DemoBytes(Model model)
    {
        // Demo byte buffer
        VoskRecognizer rec = new VoskRecognizer(model, 16000.0f);
        rec.SetMaxAlternatives(0);
        rec.SetWords(true);

        string filePath = Path.Combine(Application.streamingAssetsPath, "test.wav");

        if (File.Exists(filePath))
        {
            using (Stream source = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (rec.AcceptWaveform(buffer, bytesRead))
                    {
                        Debug.Log(rec.Result());
                    }
                    else
                    {
                        Debug.Log(rec.PartialResult());
                    }
                }
            }
            Debug.Log(rec.FinalResult());
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }

    public void DemoFloats(Model model)
    {
        // Demo float array
        VoskRecognizer rec = new VoskRecognizer(model, 16000.0f);

        string filePath = Path.Combine(Application.streamingAssetsPath, "test.wav");

        if (File.Exists(filePath))
        {
            using (Stream source = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    float[] fbuffer = new float[bytesRead / 2];
                    for (int i = 0, n = 0; i < fbuffer.Length; i++, n += 2)
                    {
                        fbuffer[i] = BitConverter.ToInt16(buffer, n);
                    }
                    if (rec.AcceptWaveform(fbuffer, fbuffer.Length))
                    {
                        Debug.Log(rec.Result());
                    }
                    else
                    {
                        Debug.Log(rec.PartialResult());
                    }
                }
            }
            Debug.Log(rec.FinalResult());
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }

    public void DemoSpeaker(Model model)
    {
        // Output speakers
        SpkModel spkModel = new SpkModel("model-spk");
        VoskRecognizer rec = new VoskRecognizer(model, 16000.0f);
        rec.SetSpkModel(spkModel);

        string filePath = Path.Combine(Application.streamingAssetsPath, "test.wav");

        if (File.Exists(filePath))
        {
            using (Stream source = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (rec.AcceptWaveform(buffer, bytesRead))
                    {
                        Debug.Log(rec.Result());
                    }
                    else
                    {
                        Debug.Log(rec.PartialResult());
                    }
                }
            }
            Debug.Log(rec.FinalResult());
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }
}
*/