using TMPro;
using UnityEngine;

public class UITextColorManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private TextMeshProUGUI text_UI;
    Color dark_gray = new Color(50f / 255f, 50f / 255f, 50f / 255f);

    void Start()
    {
        // Try to get TextMeshProUGUI from this object or its children
        text_UI = GetComponent<TextMeshProUGUI>();
        if (text_UI == null)
        {
            text_UI = GetComponentInChildren<TextMeshProUGUI>();
        }

        // Set text color based on ammo_setting
        string ammo = Scoring.ammo_setting.ToLower();
        if (ammo.Contains("live") || ammo.Contains("infrared"))
        {
            text_UI.color = Color.white;
        }
        else
        {
            text_UI.color = dark_gray;
        }
    }

}
