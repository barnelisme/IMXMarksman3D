using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IPSCExtendedManager : MonoBehaviour
{
    public GameObject[] IPSCOptions;
    public GameObject[] SpawnLocations;
    public Image[] targetUIImage;
    public Sprite[] targetImages;

    [Header("Num of shots Settings")]
    public TMP_InputField[] numShotsInputField;
    public TextMeshProUGUI[] shotsMessageTxt;

    [Header ("Target Option dropdown")]
    public TMP_Dropdown target1ScenarionOptions;
    public TMP_Dropdown target2ScenarionOptions;
    public TMP_Dropdown target3ScenarionOptions;
    public TMP_Dropdown target1DistanceOptions;
    public TMP_Dropdown target2DistanceOptions;
    public TMP_Dropdown target3DistanceOptions;
    public TextMeshProUGUI target1DistanceTxt;
    public TextMeshProUGUI target2DistanceTxt;
    public TextMeshProUGUI target3DistanceTxt;
    public TextMeshProUGUI numOptionsTxt;

    [Header("(Menu) Target position")]
    public GameObject[] menuTargetOption1;
    public GameObject[] menuTargetOption2;
    public GameObject[] menuTargetOption3;
    public GameObject[] targetOptionsHolders;

    [Header("(Scenario) Target position")]
    public GameObject[] sceneTargetPosition1;
    public GameObject[] sceneTargetPosition2;
    public GameObject[] sceneTargetPosition3;
    public GameObject[] targetPositionHolders;

    private string activeScene = "";
    private string target_1_option = "";

    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;

        if(activeScene == "IPSCExtendedSceneOptions")
        {
            ManageNumTargetOptions();

            numShotsInputField[0].text = StaticVariableManager.numTarget1ShotsSetting.ToString();
            numShotsInputField[1].text = StaticVariableManager.numTarget2ShotsSetting.ToString();
            numShotsInputField[2].text = StaticVariableManager.numTarget3ShotsSetting.ToString();

            StaticVariableManager.target1DistanceSetting = 5;
            StaticVariableManager.target2DistanceSetting = 5;
            StaticVariableManager.target3DistanceSetting = 5;
            StaticVariableManager.target4DistanceSetting = 5;
            StaticVariableManager.target5DistanceSetting = 5;

            target1DistanceTxt.text = StaticVariableManager.target1DistanceSetting.ToString();
            target2DistanceTxt.text = StaticVariableManager.target2DistanceSetting.ToString();
            target3DistanceTxt.text = StaticVariableManager.target3DistanceSetting.ToString();

            StaticVariableManager.IPSCTargetOptions[0] = "IPSC Pole 1";
            StaticVariableManager.IPSCTargetOptions[1] = "IPSC Pole 1";
            StaticVariableManager.IPSCTargetOptions[2] = "IPSC Pole 1";

            // ------------------- Setup scenarios -------------------------
            target1ScenarionOptions.options.Clear();
            target2ScenarionOptions.options.Clear();
            target3ScenarionOptions.options.Clear();

            List<string> scenarioNames = new List<string>();
            scenarioNames.Add( "IPSC Pole 1" );
            scenarioNames.Add( "IPSC Pole 2" );
            scenarioNames.Add( "IPSC Pole 3" );
            scenarioNames.Add( "Texas Wheel" );
            scenarioNames.Add( "IPSC Slider 1" );
            scenarioNames.Add( "IPSC Slider 2" );
            scenarioNames.Add( "Steel Plates" );
            scenarioNames.Add( "Pepper Popper Set" );
            scenarioNames.Add( "Pendulum" );


            foreach (string scene_name in scenarioNames)
            {
                target1ScenarionOptions.options.Add(new TMP_Dropdown.OptionData() { text = scene_name });
                target2ScenarionOptions.options.Add(new TMP_Dropdown.OptionData() { text = scene_name });
                target3ScenarionOptions.options.Add(new TMP_Dropdown.OptionData() { text = scene_name });
            }


            // ------------------ Setup distance ----------------------------
            List<string> distances = new List<string>();
            distances.Add( "5" );
            distances.Add( "10" );
            distances.Add( "15" );
            distances.Add( "20" );
            distances.Add( "25" );

            foreach(string distance in distances)
            {
                target1DistanceOptions.options.Add(new TMP_Dropdown.OptionData() { text = distance });
                target2DistanceOptions.options.Add(new TMP_Dropdown.OptionData() { text = distance });
                target3DistanceOptions.options.Add(new TMP_Dropdown.OptionData() { text = distance });
            }


        }
        else if(activeScene == "Basic1LaneIPSCExtended")
        {
            SetTargetPosition();
            for (int i = 1; i <= StaticVariableManager.numIPSCTargetOption; i++)
            {

                switch (i)
                {
                    case 1:
                        AssignTarget(StaticVariableManager.IPSCTargetOptions[0], SpawnLocations[i - 1], "Target 1");
                        break;
                    case 2:
                        AssignTarget(StaticVariableManager.IPSCTargetOptions[1], SpawnLocations[i - 1], "Target 2");
                        break;
                    case 3:
                        AssignTarget(StaticVariableManager.IPSCTargetOptions[2], SpawnLocations[i - 1], "Target 3");
                        break;
                }

            }
        }


    }

    void Update()
    {

    }

    private void ManageNumTargetOptions()
    {
        DeactivateHolders(targetOptionsHolders);
        switch (StaticVariableManager.numIPSCTargetOption)
        {
            case 1:
                // Set position
                targetOptionsHolders[0].transform.position = menuTargetOption1[0].transform.position;

                // Activate
                targetOptionsHolders[0].SetActive(true);
                break;
            case 2:
                // Set position
                targetOptionsHolders[0].transform.position = menuTargetOption2[0].transform.position;
                targetOptionsHolders[1].transform.position = menuTargetOption2[1].transform.position;

                // Activate
                targetOptionsHolders[0].SetActive(true);
                targetOptionsHolders[1].SetActive(true);
                break;
            case 3:
                // Set position
                targetOptionsHolders[0].transform.position = menuTargetOption3[0].transform.position;
                targetOptionsHolders[1].transform.position = menuTargetOption3[1].transform.position;
                targetOptionsHolders[2].transform.position = menuTargetOption3[2].transform.position;

                // Activate
                targetOptionsHolders[0].SetActive(true);
                targetOptionsHolders[1].SetActive(true);
                targetOptionsHolders[2].SetActive(true);
                break;
        }

        numOptionsTxt.text = "Targets: " + StaticVariableManager.numIPSCTargetOption.ToString();
    }

    private void SetTargetPosition()
    {
        DeactivateHolders(targetPositionHolders);
        switch (StaticVariableManager.numIPSCTargetOption)
        {
            case 1:
                // Set position
                targetPositionHolders[0].transform.position = sceneTargetPosition1[0].transform.position;

                // Activate
                targetPositionHolders[0].SetActive(true);
                break;
            case 2:
                // Set position
                targetPositionHolders[0].transform.position = sceneTargetPosition2[0].transform.position;
                targetPositionHolders[1].transform.position = sceneTargetPosition2[1].transform.position;

                // Activate
                targetPositionHolders[0].SetActive(true);
                targetPositionHolders[1].SetActive(true);
                break;
            case 3:
                // Set position
                targetPositionHolders[0].transform.position = sceneTargetPosition3[0].transform.position;
                targetPositionHolders[1].transform.position = sceneTargetPosition3[1].transform.position;
                targetPositionHolders[2].transform.position = sceneTargetPosition3[2].transform.position;

                // Activate
                targetPositionHolders[0].SetActive(true);
                targetPositionHolders[1].SetActive(true);
                targetPositionHolders[2].SetActive(true);
                break;
        }

    }

    private void DeactivateHolders(GameObject[] holders)
    {
        foreach(GameObject holder in holders)
        {
            holder.SetActive(false);
        }
    }

    private void AssignTarget(string target_name, GameObject spawn_location, string targetNumber)
    {
        Transform spawn = spawn_location.transform;
        // 
        foreach(GameObject target in IPSCOptions)
        {
            if(target.transform.name == target_name)
            {
                GameObject instance = Instantiate(target, spawn.position, spawn.rotation);
                instance.name = target.name + " " + targetNumber; // Add "Target 1" to the name

                if(targetNumber == "Target 1")
                {
                    SetTargetDistance(instance, StaticVariableManager.target1DistanceSetting);
                }
                if (targetNumber == "Target 2")
                {
                    SetTargetDistance(instance, StaticVariableManager.target2DistanceSetting);
                }
                if (targetNumber == "Target 3")
                {
                    SetTargetDistance(instance, StaticVariableManager.target3DistanceSetting);
                }
            }
        }
    }

    private void SetTargetDistance(GameObject target, int size)
    {
        switch(size)
        {
            case 10:
                target.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f); // Example scale
                break;
            case 15:
                target.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f); // Example scale
                break;
            case 20:
                target.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); // Example scale
                break;
            case 25:
                target.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f); // Example scale
                break;
        }
    }

    public void HandleTarget1(int val)
    {

        Image img = targetUIImage[0].GetComponent<Image>();
        string option = target1ScenarionOptions.options[val].text;
        StaticVariableManager.IPSCTargetOptions[0] = option;
        target_1_option = option;

        if(option == "Steel Plates")
        {
            StaticVariableManager.numIPSCTargetOption = 1;
            ManageNumTargetOptions();
        }

        UpdateNumShotsText(shotsMessageTxt[0], option, 1);
        UpdateImage(option, img);

    }

    public void HandleTarget2(int val)
    {

        if(target2ScenarionOptions.options[val].text == "Steel Plates")
        {
            StaticVariableManager.numIPSCTargetOption = 1;
            ManageNumTargetOptions();

            HandleTarget1(val);
        }
        else 
        {
            Image img = targetUIImage[1].GetComponent<Image>();

            string option = target2ScenarionOptions.options[val].text;
            StaticVariableManager.IPSCTargetOptions[1] = option;

            UpdateNumShotsText(shotsMessageTxt[1], option, 2);
            UpdateImage(option, img);
        }

    }

    public void HandleTarget3(int val)
    {
        if (target3ScenarionOptions.options[val].text == "Steel Plates")
        {
            StaticVariableManager.numIPSCTargetOption = 1;
            ManageNumTargetOptions();

            HandleTarget1(val);
        }
        else
        {
            Image img = targetUIImage[2].GetComponent<Image>();

            string option = target3ScenarionOptions.options[val].text;
            StaticVariableManager.IPSCTargetOptions[2] = option;

            UpdateNumShotsText(shotsMessageTxt[2], option, 3);
            UpdateImage(option, img);
        }
    }

    public void HandleTarget1Distance(int val)
    {
        StaticVariableManager.target1DistanceSetting = int.Parse(target1DistanceOptions.options[val].text);
    }
    public void HandleTarget2Distance(int val)
    {
        StaticVariableManager.target2DistanceSetting = int.Parse(target2DistanceOptions.options[val].text);
    }
    public void HandleTarget3Distance(int val)
    {
        StaticVariableManager.target3DistanceSetting = int.Parse(target3DistanceOptions.options[val].text);
    }

    public void AddNumOptions()
    {
        if (target_1_option == "Steel Plates")
        {
            // Notify 
        }
        else
        {
            if (StaticVariableManager.numIPSCTargetOption < 3) StaticVariableManager.numIPSCTargetOption++;
            ManageNumTargetOptions();
        }

    }

    public void SubtractNumOptions()
    {
        if (StaticVariableManager.numIPSCTargetOption > 1) StaticVariableManager.numIPSCTargetOption--;
        ManageNumTargetOptions();
    }

    private void UpdateNumShotsText(TextMeshProUGUI shotsTxt, string targetName, int targetNumber)
    {
        print(targetName);

        if(targetName == "Texas Wheel")
        {
            shotsTxt.text = "Plate drop shots: ";
            switch(targetNumber)
            {
                case 1:
                    StaticVariableManager.numTarget1ShotsSetting = 1;
                    numShotsInputField[0].text = StaticVariableManager.numTarget1ShotsSetting.ToString();
                    break;
                case 2:
                    StaticVariableManager.numTarget2ShotsSetting = 1;
                    numShotsInputField[1].text = StaticVariableManager.numTarget2ShotsSetting.ToString();
                    break;
                case 3:
                    StaticVariableManager.numTarget3ShotsSetting = 1;
                    numShotsInputField[2].text = StaticVariableManager.numTarget3ShotsSetting.ToString();
                    break;
                case 4:
                    StaticVariableManager.numTarget4ShotsSetting = 1;
                    numShotsInputField[3].text = StaticVariableManager.numTarget4ShotsSetting.ToString();
                    break;
                case 5:
                    StaticVariableManager.numTarget5ShotsSetting = 1;
                    numShotsInputField[4].text = StaticVariableManager.numTarget5ShotsSetting.ToString();
                    break;
            }
        }
        else if(targetName == "Steel Plates")
        {
            shotsTxt.text = "Number of plates: ";
            switch (targetNumber)
            {
                case 1:
                    StaticVariableManager.numTarget1ShotsSetting = 1;
                    numShotsInputField[0].text = StaticVariableManager.numIPSCSteelPlates.ToString();
                    break;
                case 2:
                    StaticVariableManager.numTarget2ShotsSetting = 1;
                    numShotsInputField[1].text = StaticVariableManager.numTarget2ShotsSetting.ToString();
                    break;
                case 3:
                    StaticVariableManager.numTarget3ShotsSetting = 1;
                    numShotsInputField[2].text = StaticVariableManager.numTarget3ShotsSetting.ToString();
                    break;
                case 4:
                    StaticVariableManager.numTarget4ShotsSetting = 1;
                    numShotsInputField[3].text = StaticVariableManager.numTarget4ShotsSetting.ToString();
                    break;
                case 5:
                    StaticVariableManager.numTarget5ShotsSetting = 1;
                    numShotsInputField[4].text = StaticVariableManager.numTarget5ShotsSetting.ToString();
                    break;
            }
        }
        else if(targetName == "Pendulum")
        {
            shotsTxt.text = "Number of shots: ";
            int numTexasPlates = 3;

            print("Point reached...");
            switch (targetNumber)
            {
                case 1:
                    if (StaticVariableManager.numTarget1ShotsSetting == 1) StaticVariableManager.numTarget1ShotsSetting = 10;
                    numShotsInputField[0].text = StaticVariableManager.numTarget1ShotsSetting.ToString();
                    break;
                case 2:
                    if (StaticVariableManager.numTarget2ShotsSetting == 1) StaticVariableManager.numTarget2ShotsSetting = 10;
                    numShotsInputField[1].text = StaticVariableManager.numTarget2ShotsSetting.ToString();
                    break;
                case 3:
                    if (StaticVariableManager.numTarget3ShotsSetting == 1) StaticVariableManager.numTarget3ShotsSetting = 10;
                    numShotsInputField[2].text = StaticVariableManager.numTarget3ShotsSetting.ToString();
                    break;
                case 4:
                    if (StaticVariableManager.numTarget4ShotsSetting == 1) StaticVariableManager.numTarget4ShotsSetting = 10;
                    numShotsInputField[3].text = StaticVariableManager.numTarget4ShotsSetting.ToString();
                    break;
                case 5:
                    if (StaticVariableManager.numTarget5ShotsSetting == 1) StaticVariableManager.numTarget5ShotsSetting = 10;
                    numShotsInputField[4].text = StaticVariableManager.numTarget5ShotsSetting.ToString();
                    break;
            }
        }
        else
        {
            shotsTxt.text = "Number of shots: ";
            switch (targetNumber)
            {
                case 1:
                    if(StaticVariableManager.numTarget1ShotsSetting == 1) StaticVariableManager.numTarget1ShotsSetting = 10;
                    numShotsInputField[0].text = StaticVariableManager.numTarget1ShotsSetting.ToString();
                    break;
                case 2:
                    if (StaticVariableManager.numTarget2ShotsSetting == 1) StaticVariableManager.numTarget2ShotsSetting = 10;
                    numShotsInputField[1].text = StaticVariableManager.numTarget2ShotsSetting.ToString();
                    break;
                case 3:
                    if (StaticVariableManager.numTarget3ShotsSetting == 1) StaticVariableManager.numTarget3ShotsSetting = 10;
                    numShotsInputField[2].text = StaticVariableManager.numTarget3ShotsSetting.ToString();
                    break;
                case 4:
                    if (StaticVariableManager.numTarget4ShotsSetting == 1) StaticVariableManager.numTarget4ShotsSetting = 10;
                    numShotsInputField[3].text = StaticVariableManager.numTarget4ShotsSetting.ToString();
                    break;
                case 5:
                    if (StaticVariableManager.numTarget5ShotsSetting == 1) StaticVariableManager.numTarget5ShotsSetting = 10;
                    numShotsInputField[4].text = StaticVariableManager.numTarget5ShotsSetting.ToString();
                    break;
            }
        }

    }

    private void UpdateImage(string option, Image img)
    {
        // Ensure all Image names match the Image Options

        int index = 0;
        foreach(Sprite image in targetImages)
        {

            if(image.name == option)
            {
                img.sprite = targetImages[index];
            }

            index++;
        }
    }

    public void Exit()
    {
        SceneManager.LoadScene("SceneManager");
    }
    public void Continue()
    {
        ManageNumShotsSettings();
        SceneManager.LoadScene("TestConditionSetting");
    }

    private void ManageNumShotsSettings()
    {

        for (int i = 0; i < StaticVariableManager.IPSCTargetOptions.Length; i++)
        {
            string target_name = StaticVariableManager.IPSCTargetOptions[i];

            SetNumShotsSetting(i + 1, int.Parse(numShotsInputField[i].text), target_name);

            /*switch (target_name)
            {
                case "IPSC Pole 1":
                    StaticVariableManager.numIPSCPole1Shots = int.Parse(numShotsInputField[i].text);
                    print(target_name + ": " + StaticVariableManager.numIPSCPole1Shots);
                    break;
                case "IPSC Pole 2":
                    StaticVariableManager.numIPSCPole2Shots = int.Parse(numShotsInputField[i].text);
                    print(target_name + ": " + StaticVariableManager.numIPSCPole2Shots);
                    break;
                case "IPSC Pole 3":
                    StaticVariableManager.numIPSCPole3Shots = int.Parse(numShotsInputField[i].text);
                    print(target_name + ": " + StaticVariableManager.numIPSCPole3Shots);
                    break;
                case "Texas Wheel":
                    StaticVariableManager.numTexasWheelShots = int.Parse(numShotsInputField[i].text);
                    print(target_name + ": " + StaticVariableManager.numTexasWheelShots);
                    break;
                case "IPSC Slider 1":
                    StaticVariableManager.numIPSCSlider1Shots = int.Parse(numShotsInputField[i].text);
                    print(target_name + ": " + StaticVariableManager.numIPSCSlider1Shots);
                    break;
                case "IPSC Slider 2":
                    StaticVariableManager.numIPSCSlider2Shots = int.Parse(numShotsInputField[i].text);
                    print(target_name + ": " + StaticVariableManager.numIPSCSlider2Shots);
                    break;
            }*/
        }
    }

    private void SetNumShotsSetting(int targetNum, int value, string targetName)
    {
        bool isModified = false;
        // print($"Target {targetNum}: {value}");
        if (targetName == "Texas Wheel")
        {
            value = value * 5; // Setup texas wheel
            isModified = true;
        }
        else if (targetName == "Steel Plates")
        {
            value = SetNumberSteelPlates(value); // Setup Steel plates
            isModified = true;

        }
        else if (targetName == "Pepper Popper Set")
        {
            value = 3;
            isModified = true;
        }
        else if(targetName == "Pendulum")
        {
            value += 3;
            isModified = true;
        }

        if(isModified)
        {
            switch (targetNum)
            {
                case 1:
                    StaticVariableManager.numTarget1ShotsSetting = value;
                    break;

                case 2:
                    StaticVariableManager.numTarget2ShotsSetting = value;
                    break;

                case 3:
                    StaticVariableManager.numTarget3ShotsSetting = value;
                    break;
            }
        }

    }

    private int SetNumberSteelPlates(int value)
    {
        if (value < 4)
        {
            value = 4;
        }
        else if (value > 10)
        {
            value = 10;
        }

        StaticVariableManager.numIPSCSteelPlates = value;
        return value;
    }
}
