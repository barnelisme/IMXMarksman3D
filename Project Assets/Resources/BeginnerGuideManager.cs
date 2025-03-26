using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class BeginnerGuideManager : MonoBehaviour
{
    //public variables
    public TextMeshProUGUI header;
    public TextMeshProUGUI GunSafetyText;
    public TextMeshProUGUI GunHandlingText;
    public TextMeshProUGUI shootingStanceText;
    public GameObject gunSafetyPanel;
    public GameObject gunHandlingPanel;
    public GameObject shootingStancePanel;
    //Image data variables
    [Header("Gun Handling")]
    public Image gunHandlingImageField;
    public List<GameObject> handlingControlButtons = new List<GameObject>();
    public List<Sprite> gunHandlingImageSprite = new List<Sprite>();
    private List<string> gunHandlingImageInfo = new List<string>();

    [Header("Shooting Stance")]
    public Image shootingStanceImageField;
    public List<GameObject> stanceControlButtons = new List<GameObject>();
    public List<Sprite> shootingStanceImageSprite = new List<Sprite>();
    private List<string> shootingStanceImageInfo = new List<string>();

    //Private variable
    public List<GameObject> activeControlButtons = new List<GameObject>();
    private int imageIndex = 0;
    private string lines = "";
    private float textSpeed = 0.000001f;
    private bool imageChanged = true;
    private bool typeLineProcessComplete = false;
    private string imageButtonSelector = "All";

    void Start()
    {
        //GunSafetyText.text = string.Empty;
        imageIndex = 0;
        activeControlButtons = handlingControlButtons;
        //uiImage.sprite = gunHandlingImageSprite[imageIndex];

        SetupImageInfo();
    }

    // Update is called once per frame
    void Update()
    {
        ManageControlButtons();
        
    }

    private void ManageControlButtons()
    {
        if (imageChanged)
        {
            if (typeLineProcessComplete)
            {
                switch (imageButtonSelector)
                {
                    case "left":
                        activeControlButtons[0].SetActive(true);
                        activeControlButtons[1].SetActive(false);
                        break;
                    case "right":
                        activeControlButtons[0].SetActive(false);
                        activeControlButtons[1].SetActive(true);
                        break;
                    case "all":
                        activeControlButtons[0].SetActive(true);
                        activeControlButtons[1].SetActive(true);
                        break;
                }

                //print("Area reset...");
                imageChanged = false; //Reset
            }
        }
    }

    private void SetupImageInfo()
    {
        string tempImageInfo = "";
        //Gun Handlin
        //Image 1
        tempImageInfo = "<b><u>Hand Position:</u></b>\n \n" +
            "Make sure to use both hands when holding a gun. The trigger hand must be the first to hold the gun. The second " +
            "hand should be on top of the first hand for balance as illustrated on the image.";
            gunHandlingImageInfo.Add(tempImageInfo);

        //Image 2
        tempImageInfo = "<b><u>Gun Grip:</u></b>\n \n" +
            "Make sure you are comfortably holding the gun in your hands, and grip it tightly to be able to handle it during recoil." +
            " When you fire a shot, the gun will push you back, you need to be able to hold it in your hands.";
            gunHandlingImageInfo.Add(tempImageInfo);

        //Image 3
        tempImageInfo = "<b><u>Sight Focus:</u></b>\n \n" +
            "When you hold your gun straight pointing to a target, you will notice that on the slide at the top of the gun " +
            "there are three dots, two at the back and one up front. The two at the back are called reer sights, and the " +
            "one up front is called front sight. To aim properly, point the gun to a target and lineup the front sight with " +
            "the reer sight as shown in the picture. Make sure the front sight is on target and the gun is at the same" +
            " horizontal line with your aiming eye.";
            gunHandlingImageInfo.Add(tempImageInfo);

        //Shooting Stance
        //Image 1
        tempImageInfo = "<b><u>Standing Position:</u></b>\n \n" +
            "To have a proper stance, ensure that you follow steps bellow: \n \n" +
            "- First point your gun down to the ground. \n" +
            "- Stand up straight. \n" +
            "- Make sure your feet are a proper distance apart for balance. \n" +
            "- band your knees a little to improve balance. \n" +
            "- Now lean forward a little bit, and raise the gun and aim at the target. \n " +
            "- Make sure you stretch your hands all the way forward. \n" +
            "\n" +
            "After following these steps, your standing form should be the same as the person on the image.";
        shootingStanceImageInfo.Add(tempImageInfo);

        //Image 1
        tempImageInfo = "<b><u>Gun Control:</u></b>\n \n" +
            "Before shooting, ensure that your hands are stretched all the way forward, and you are " +
            "holding the gun tightly in your hands while maintaining the stance position. This will " +
            "allow you to have proper body balance, and gun control when firing.";
        shootingStanceImageInfo.Add(tempImageInfo);

    }

    public void ManageGunSafety()
    {
        SetPanel("Gun Safety");

        //Load Safety rules//  
        lines = "<b><u>Four Primary Rules of Gun Safety:</u></b>\n \n" +

            "  - Treat every firearm as if it is loaded. \n" +
            "  - Never point a firearm at anything you are not willing to destroy. \n" +
            "  - Keep your finger off the trigger until you are ready to shoot. \n" +
            "  - Be sure of your target and what is beyond it \n \n" +

            "<b><u>Additional Gun Safety Rules:</u></b> \n \n" +

            "  - Store firearms securely when not in use. \n" +
            "  - Know how your firearm operates. \n " +
            "  - Use the correct ammunition. \n" +
            "  - Wear proper eye and ear protection. \n" +
            "  - Never use a gun while under the influence of drugs or alcohol. \n" +
            "  - Maintain and inspect your firearm regularly. \n" +
            "  - Be aware of your surroundings when handling firearms. \n" +
            "  - Only carry and use firearms in permitted areas. \n";
        StartDialogue(GunSafetyText);
    }

    public void ManageGunHandling()
    {
        SetPanel("Gun Handling");

        gunHandlingImageField.sprite = gunHandlingImageSprite[imageIndex];
        lines = gunHandlingImageInfo[0];

        activeControlButtons = handlingControlButtons;
        foreach (GameObject button in activeControlButtons) { button.SetActive(false); }

        StartDialogue(GunHandlingText);
    }

    public void ManageShootingStance()
    {
        SetPanel("Shooting Stance");
        shootingStanceImageField.sprite = shootingStanceImageSprite[imageIndex];
        lines = shootingStanceImageInfo[0];

        activeControlButtons = stanceControlButtons;
        foreach (GameObject button in activeControlButtons){button.SetActive(false);}

        StartDialogue(shootingStanceText);
    }

    public void NextGunHandlingImage()
    {
        LoadGunImage("next", gunHandlingImageSprite, gunHandlingImageField, gunHandlingImageInfo, GunHandlingText);
    }

    public void PrevGunHandlingImage()
    {
        LoadGunImage("prev",gunHandlingImageSprite, gunHandlingImageField, gunHandlingImageInfo, GunHandlingText);
    }

    public void NextShootingStanceImage()
    {
        LoadGunImage("next", shootingStanceImageSprite, shootingStanceImageField, shootingStanceImageInfo, shootingStanceText);
    }

    public void PrevShootingStanceImage()
    {
        LoadGunImage("prev", shootingStanceImageSprite, shootingStanceImageField, shootingStanceImageInfo, shootingStanceText);
    }


    private void SetPanel(string panel_key)
    {
        header.text = panel_key; //set header
        imageButtonSelector = "right";
        imageChanged = true;

        switch (panel_key) //set panels
        {
            case "Gun Safety":
                gunSafetyPanel.SetActive(true);
                gunHandlingPanel.SetActive(false);
                shootingStancePanel.SetActive(false);
                break;
            case "Gun Handling":
                gunSafetyPanel.SetActive(false);
                gunHandlingPanel.SetActive(true);
                shootingStancePanel.SetActive(false);
                imageIndex = 0;
                break;
            case "Shooting Stance":
                gunSafetyPanel.SetActive(false);
                gunHandlingPanel.SetActive(false);
                shootingStancePanel.SetActive(true);
                imageIndex = 0;
                break;
        }
    }

    private void LoadGunImage(string direction_key, List<Sprite> images, Image image_field, List<string> image_info, TextMeshProUGUI info_text_space)
    {

        if(typeLineProcessComplete)
        {
            //Update image index
            switch (direction_key)
            {
                case "next":
                    if (imageIndex < images.Count - 1)
                    {
                        imageIndex++;
                        imageChanged = true;
                    }
                    break;

                case "prev":
                    if (imageIndex > 0)
                    {
                        imageIndex--;
                        imageChanged = true;
                    }
                    break;
            }
            //Manage control buttons
            if (imageIndex == 0)
            {
                imageButtonSelector = "right";
            }
            else if (imageIndex == images.Count - 1)
            {
                imageButtonSelector = "left";
            }
            else
            {
                imageButtonSelector = "all";
            }

            //Update image areas
            image_field.sprite = images[imageIndex];
            lines = image_info[imageIndex];
            if (imageChanged)
            {
                StartDialogue(info_text_space);
                //foreach (GameObject button in activeControlButtons) { button.SetActive(false); }
            }
        }
    }

    void StartDialogue(TextMeshProUGUI textSpace)
    {
        //index = 0;
        textSpace.text = string.Empty;
        typeLineProcessComplete = false;
        StartCoroutine(TypeLine(textSpace));
    }
    IEnumerator TypeLine(TextMeshProUGUI textSpace)
    {
        foreach (char c in lines.ToCharArray())
        {
            textSpace.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        typeLineProcessComplete = true;
    }
}
