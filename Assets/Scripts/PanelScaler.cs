using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Globalization;  // Needed for invariant culture

public class PanelScalerAndMover : MonoBehaviour
{
    [Header("Assign your panel RectTransform here")]
    public RectTransform panel;

    [Header("Adjust step values")]
    private float scaleStep = 0.005f;   // how much to scale per press
    private float moveStep = 2f;       // how many units to move per press

    private string saveFilePath;

    private void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "scale_settings.txt");
        LoadSettings();
    }

    private void Update()
    {
        if (panel == null) return;

        // Must hold LeftShift
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            bool altHeld = Keyboard.current.leftAltKey.isPressed;

            if (altHeld)
            {
                // Movement mode
                if (Keyboard.current.upArrowKey.wasPressedThisFrame)
                    MovePanel(Vector2.up * moveStep);
                if (Keyboard.current.downArrowKey.wasPressedThisFrame)
                    MovePanel(Vector2.down * moveStep);
                if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
                    MovePanel(Vector2.left * moveStep);
                if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
                    MovePanel(Vector2.right * moveStep);
            }
            else
            {
                // Scale mode
                if (Keyboard.current.upArrowKey.wasPressedThisFrame)
                    AdjustScale(scaleStep);
                if (Keyboard.current.downArrowKey.wasPressedThisFrame)
                    AdjustScale(-scaleStep);
            }
        }
    }

    private void AdjustScale(float delta)
    {
        float newScale = Mathf.Clamp(panel.localScale.x + delta, 0.1f, 5f);
        panel.localScale = Vector3.one * newScale;
        SaveSettings();
    }

    private void MovePanel(Vector2 delta)
    {
        panel.anchoredPosition += delta;
        SaveSettings();
    }

    private void SaveSettings()
    {
        // Force '.' decimal separator regardless of system locale
        string data = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}",
            panel.localScale.x, panel.anchoredPosition.x, panel.anchoredPosition.y);

        File.WriteAllText(saveFilePath, data);

        // Debug.Log("Saved " + data);
    }

    private void LoadSettings()
    {
        if (File.Exists(saveFilePath))
        {
            string[] savedValues = File.ReadAllText(saveFilePath).Split(',');
            if (savedValues.Length == 3)
            {
                // Debug.Log("Point Reached...");
                // Debug.Log(savedValues[0]);
                // Debug.Log(savedValues[1]);
                // Debug.Log(savedValues[2]);

                if (float.TryParse(savedValues[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float savedScale) &&
                    float.TryParse(savedValues[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float savedX) &&
                    float.TryParse(savedValues[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float savedY))
                {
                    if (panel != null)
                    {
                        panel.localScale = Vector3.one * savedScale;
                        panel.anchoredPosition = new Vector2(savedX, savedY);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Saved file has wrong format: " + File.ReadAllText(saveFilePath));
            }
        }
    }
}
