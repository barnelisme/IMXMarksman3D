using UnityEngine;
using TMPro;

public class WorldSpaceTextFollow : MonoBehaviour
{
    public Material alwaysOnTopTextMaterial;

    void Start()
    {
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        if (text != null && alwaysOnTopTextMaterial != null)
        {
            text.fontMaterial = new Material(alwaysOnTopTextMaterial);
        }
    }

}
