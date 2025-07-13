using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundSetImageManager : MonoBehaviour
{
    public TextMeshProUGUI backgoundNameTxt;
    public TextMeshProUGUI backgroundTypeTxt;
    public Image backgroundImage;
    string backgroundName = "";

    private List<Sprite> backgroundImages = new List<Sprite>();
    private List<Color> backgroundColors = new List<Color> { Color.black, Color.white, Color.blue, Color.red, Color.green, Color.yellow };
    Color brownColor = new Color(150f / 255f, 75f / 255f, 0f);

    private static List<Sprite> backgroundImagesSet = new List<Sprite>();
    private static List<Color> backgroundColorsSet = new List<Color>();
    public static Image backgroundImageDisplay;
    private GameObject settingsManager;

    void Start()
    {
        settingsManager = GameObject.FindGameObjectWithTag("TestSettingsManager");
        backgroundImages = settingsManager.GetComponent<TestConditionsManager>().backgroundImages;

        if (this.transform.name.ToLower().Contains("item"))
        {
            backgroundTypeTxt.enabled = false;
            backgroundName = backgoundNameTxt.text;
            SetBackgoundImage(backgroundName);
        }
        else
        {

            StaticVariableManager.backgroundImages = settingsManager.GetComponent<TestConditionsManager>().backgroundImages;
            StaticVariableManager.backgroundColors = backgroundColors;
            StaticVariableManager.isShowCurrBackground = true;
        }

    }

    private void SetBackgoundImage(string background)
    {

        switch(background)
        {
            case "Black":
                print($"Background: {background}");
                backgroundImage.transform.gameObject.SetActive(true);
                backgroundImage.GetComponent<Image>().color = backgroundColors[0];
                break;
            case "White":
                backgroundImage.transform.gameObject.SetActive(true);
                backgroundImage.GetComponent<Image>().color = backgroundColors[1];
                break;
            case "Blue":
                backgroundImage.transform.gameObject.SetActive(true);
                backgroundImage.GetComponent<Image>().color = backgroundColors[2];
                break;
            case "Red":
                backgroundImage.transform.gameObject.SetActive(true);
                backgroundImage.GetComponent<Image>().color = backgroundColors[3];
                break;
            case "Green":
                backgroundImage.transform.gameObject.SetActive(true);
                backgroundImage.GetComponent<Image>().color = backgroundColors[4];
                break;
            case "Yellow":
                backgroundImage.transform.gameObject.SetActive(true);
                backgroundImage.GetComponent<Image>().color = backgroundColors[5];
                break;
            case "AI Attack":
                backgroundImage.transform.gameObject.SetActive(true);
                backgroundImage.sprite = backgroundImages[0];
                break;
            case "Alien invasion":
                backgroundImage.transform.gameObject.SetActive(true);
                backgroundImage.sprite = backgroundImages[1];
                break;
            case "B City":
                backgroundImage.transform.gameObject.SetActive(true);
                backgroundImage.sprite = backgroundImages[2];
                break;
            case "Living Room":
                backgroundImage.transform.gameObject.SetActive(true);
                backgroundImage.sprite = backgroundImages[3];
                break;
            case "Mother City":
                backgroundImage.transform.gameObject.SetActive(true);
                backgroundImage.sprite = backgroundImages[4];
                break;
            case "Outer Space":
                backgroundImage.transform.gameObject.SetActive(true);
                backgroundImage.sprite = backgroundImages[5];
                break;
            case "African Jungle":
                backgroundImage.transform.gameObject.SetActive(true);
                backgroundImage.sprite = backgroundImages[6];
                break;
            case "Desert":
                backgroundImage.transform.gameObject.SetActive(true);
                backgroundImage.sprite = backgroundImages[7];
                break;
            case "War Desert Weed":
                backgroundImage.transform.gameObject.SetActive(true);
                backgroundImage.sprite = backgroundImages[8];
                break;
            case "War Torn City":
                backgroundImage.transform.gameObject.SetActive(true);
                backgroundImage.sprite = backgroundImages[9];
                break;
        }
    }

    public static void SetBackGroundImage(int val, Image imageDisplay)
    {
        // print("Point reached...");
        bool isBasicColor = false;
        backgroundImagesSet = StaticVariableManager.backgroundImages;
        backgroundColorsSet = StaticVariableManager.backgroundColors;


        if (val <= backgroundColorsSet.Count)
        {
            imageDisplay.sprite = null;
            imageDisplay.GetComponent<Image>().color = backgroundColorsSet[val - 1];
            isBasicColor = true;
        }

        if (!isBasicColor)
        {
            val -= backgroundColorsSet.Count;
            imageDisplay.color = Color.white;
            imageDisplay.sprite = backgroundImagesSet[(int)val - 1];

            val += backgroundColorsSet.Count; // Reset
        }

        StaticVariableManager.currBackgroundImagePos = val;
    }

    public static void SetBackgroundMaterialImage(int val, Material mat)
    {
        if(StaticVariableManager.backgroundColors != null)
        {
            bool isBasicColor = false;
            backgroundImagesSet = StaticVariableManager.backgroundImages;
            backgroundColorsSet = StaticVariableManager.backgroundColors;

            if (val <= backgroundColorsSet.Count)
            {
                // Convert color to a 1x1 texture and apply it to the material
                Color color = backgroundColorsSet[val - 1];
                Texture2D tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, color);
                tex.Apply();
                mat.mainTexture = tex;
                isBasicColor = true;
            }

            if (!isBasicColor)
            {
                val -= backgroundColorsSet.Count;
                mat.mainTexture = backgroundImagesSet[(int)val - 1].texture;
                val += backgroundColorsSet.Count;
            }

            StaticVariableManager.currBackgroundImagePos = val;
        }

    }


}
