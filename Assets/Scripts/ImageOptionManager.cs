using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ImageOptionManager : MonoBehaviour
{

    public TextMeshProUGUI optionTxt;
    public Sprite[] targetImages;
    public GameObject inputManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // "IPSC Pole 1" });
        // "IPSC Pole 2" });
        // "IPSC Pole 3" });
        // "IPSC Slider" });
        // "Texas Wheel" });
        if(inputManager!= null) targetImages = inputManager.GetComponent<IPSCExtendedManager>().targetImages;
        Image img = GetComponent<Image>();
        int index = 0;

        foreach(Sprite image in targetImages)
        {
            if(optionTxt.text == image.name)
            {
                img.sprite = targetImages[index];
            }

            index++;
        }

    }

}
