using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundSetImageManager : MonoBehaviour
{
    public TextMeshProUGUI backgoundNameTxt;
    public TextMeshProUGUI backgroundTypeTxt;
    public Image backgroundImage;
    string backgroundName = "";
    private static bool updateBackgoundData = false;

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
            // HandleExteralBackgrounds(TestConditionsManager.backgroundOptions_Static);

            StaticVariableManager.backgroundImages = settingsManager.GetComponent<TestConditionsManager>().backgroundImages;
            StaticVariableManager.backgroundColors = backgroundColors;
            StaticVariableManager.isShowCurrBackground = true;
        }

    }

    private void Update()
    {
        if (updateBackgoundData)
        {
            UpdateClassData();
            updateBackgoundData = false;
        }
    }

    public static void HandleExteralBackgrounds(TMP_Dropdown dropDown)
    {
        // Debug.Log("Adding external backgrounds...");

        // ----------------------------
        // 1️⃣ External Documents Folder
        // ----------------------------
        string documentsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

        string externalImageLocation = Path.Combine(
            documentsFolder,
            "IMX",
            "BackgroundImages"
        );

        if (!Directory.Exists(externalImageLocation))
        {
            Directory.CreateDirectory(externalImageLocation);
            Debug.Log("Created external image folder: " + externalImageLocation);
        }

        string[] extensions = { "*.png", "*.jpg", "*.jpeg" };


        // ----------------------------
        // 4️⃣ Load Images From Game Folder
        // ----------------------------
        List<string> newOptions = new List<string>();

        foreach (string extension in extensions)
        {
            string[] files = Directory.GetFiles(externalImageLocation, extension);

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);

                // Skip if already in dropdown
                bool alreadyExistsInDropdown = dropDown.options.Exists(option => option.text == fileName);
                if (alreadyExistsInDropdown)
                    continue;

                byte[] imageBytes = File.ReadAllBytes(file);

                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(imageBytes))
                {
                    texture.wrapMode = TextureWrapMode.Clamp;
                    texture.filterMode = FilterMode.Bilinear;

                    Sprite sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );

                    // 🔥 Ensure sprite name matches file name exactly
                    sprite.name = fileName;

                    StaticVariableManager.backgroundImages.Add(sprite);
                    newOptions.Add(fileName);
                }
            }
        }

        if (newOptions.Count > 0)
        {
            dropDown.AddOptions(newOptions);
            Debug.Log("Added " + newOptions.Count + " new external backgrounds.");
        }
        else
        {
            Debug.Log("No new external backgrounds found.");
        }
    }

    public static void HandleExteralBackgrounds_2(TMP_Dropdown dropDown)
    {
        Debug.Log("Adding external backgrounds...");

        // ----------------------------
        // 1️⃣ External Documents Folder
        // ----------------------------
        string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

        string externalImageLocation = Path.Combine(
            documentsPath,
            "IMX",
            "BackgroundImages"
        );

        if (!Directory.Exists(externalImageLocation))
        {
            Directory.CreateDirectory(externalImageLocation);
            Debug.Log("Created external image folder: " + externalImageLocation);
        }

        // ----------------------------
        // 2️⃣ Internal Game Folder
        // ----------------------------
        string imageLocation = Path.Combine(
            FileManager.gamePath,
            "Background Images",
            "Training Background"
        );

        if (!Directory.Exists(imageLocation))
        {
            Directory.CreateDirectory(imageLocation);
            Debug.Log("Created game background folder: " + imageLocation);
        }

        string[] extensions = { "*.png", "*.jpg", "*.jpeg" };

        // ----------------------------
        // 3️⃣ Copy NEW Images Only
        // ----------------------------
        foreach (string extension in extensions)
        {
            string[] externalFiles = Directory.GetFiles(externalImageLocation, extension);

            foreach (string externalFile in externalFiles)
            {
                string fileName = Path.GetFileName(externalFile);
                string destinationPath = Path.Combine(imageLocation, fileName);

                // Copy only if file does not already exist
                if (!File.Exists(destinationPath))
                {
                    File.Copy(externalFile, destinationPath);
                    Debug.Log("Copied new external image: " + fileName);
                }
            }
        }

        // ----------------------------
        // 4️⃣ Load Images From Game Folder
        // ----------------------------
        List<string> newOptions = new List<string>();

        foreach (string extension in extensions)
        {
            string[] files = Directory.GetFiles(imageLocation, extension);

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);

                // Skip if already in dropdown
                bool alreadyExistsInDropdown = dropDown.options.Exists(option => option.text == fileName);
                if (alreadyExistsInDropdown)
                    continue;

                byte[] imageBytes = File.ReadAllBytes(file);

                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(imageBytes))
                {
                    texture.wrapMode = TextureWrapMode.Clamp;
                    texture.filterMode = FilterMode.Bilinear;

                    Sprite sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );

                    // 🔥 Ensure sprite name matches file name exactly
                    sprite.name = fileName;

                    StaticVariableManager.backgroundImages.Add(sprite);
                    newOptions.Add(fileName);
                }
            }
        }

        if (newOptions.Count > 0)
        {
            dropDown.AddOptions(newOptions);
            Debug.Log("Added " + newOptions.Count + " new external backgrounds.");
        }
        else
        {
            Debug.Log("No new external backgrounds found.");
        }
    }

    private void UpdateClassData()
    {
        // backgroundImages = settingsManager.GetComponent<TestConditionsManager>().backgroundImages;
    }

    public void AddExternalImages()
    {
        // Open files
        // allow a person to navigate to their Image, then when they double click the Image or
        // click open, the Image should be copied to the project folder using the file manager FileManager.CopyImageToProject()
        print("Adding Image");
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

        // Handle external backgrounds
        int index = 0;
        foreach(Sprite image in StaticVariableManager.backgroundImages)
        {
            if(background == image.name)
            {
                backgroundImage.transform.gameObject.SetActive(true);
                backgroundImage.GetComponent<Image>().sprite = image;
            }

            index++;
        }

    }

    public static void SetBackGroundImage(int val, Image imageDisplay, string activeScene)
    {
        if (activeScene.ToLower().Contains("duelingtree"))
        {
            StaticVariableManager.backgroundColors.Remove(Color.red);
            StaticVariableManager.backgroundColors.Remove(Color.yellow);
            StaticVariableManager.currDuelingTreeBackgroundPos = val;
        }
        else
        {
            StaticVariableManager.currBackgroundBackupPos = val;
        }

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

    public static void SetHuntGroundImage(int val, List<Sprite> backgroundImages, Image imageDisplay)
    {
        imageDisplay.sprite = backgroundImages[val];
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
