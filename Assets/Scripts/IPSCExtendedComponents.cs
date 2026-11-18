using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IPSCExtendedComponents : MonoBehaviour
{
    public Image[] targetUIImage;
    public Sprite[] targetImages;

    [Header("Num of shots Settings")]
    public TMP_InputField[] numShotsInputField;
    public TextMeshProUGUI[] shotsMessageTxt;

    [Header("Target Option dropdown")]
    public TMP_Dropdown target1ScenarionOptions;
    public TMP_Dropdown target2ScenarionOptions;
    public TMP_Dropdown target3ScenarionOptions;
}
