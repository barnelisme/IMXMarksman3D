using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro; // If using TextMeshPro for UI

public class SpeechToText : MonoBehaviour
{
    private DictationRecognizer dictationRecognizer;
    public TMP_Text displayText; // Assign in Unity Inspector (optional)

    void Start()
    {
        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.Log("Recognized: " + text);
            if (displayText != null)
                displayText.text = text; // Display recognized text
        };

        dictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogError("Speech Recognition Error: " + error);
        };

        dictationRecognizer.Start();
    }

    void OnDestroy()
    {
        if (dictationRecognizer != null && dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            dictationRecognizer.Stop();
            dictationRecognizer.Dispose();
        }
    }
}

