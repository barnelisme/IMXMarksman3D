using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;

public class TestConditionsManager : MonoBehaviour
{

    [Header("Color Variables")]
    public TextMeshProUGUI currentPlateColor;
    public TextMeshProUGUI currentBackroundColor;
    [SerializeField] GameObject targetColorPanel;

    [Header("Input Fields")]
    public TMP_InputField num_bullets_per_Meg;
    public TMP_InputField num_StopSeconds;
    public TMP_InputField num_Megs;
    public TMP_InputField num_Lanes;
    public TMP_InputField num_strikes;
    public TMP_Dropdown HuntTypeHandler;
    public TextMeshProUGUI huntTypeLabel;
    public TMP_Dropdown AnimalNameHandler;
    public TMP_Dropdown colorIndicatorHandler;
    public TMP_Dropdown trainingLevelHandler;
    public TMP_Dropdown trainingModeHandler;
    public TMP_Dropdown lane1TraineeHandler;
    public TMP_Dropdown lane2TraineeHandler;
    public TMP_Dropdown lane3TraineeHandler;
    public TMP_Dropdown targetTypeHandler;
    public TMP_Dropdown numTargetHandler;
    public TextMeshProUGUI animaNameLabel;
    public TextMeshProUGUI colorIndicatorLabel;
    public TextMeshProUGUI targetTypeLabel;
    public TextMeshProUGUI numTargetLabel;
    public TextMeshProUGUI trainingLevelLabel;
    public TextMeshProUGUI trainingModeLabel;
    public TextMeshProUGUI lane1TraineeLabel;
    public TextMeshProUGUI lane2TraineeLabel;
    public TextMeshProUGUI lane3TraineeLabel;
    public TMP_InputField num_Hits;
    public TMP_InputField totalTestTime;
    public TMP_InputField num_LaneTargets;
    public TMP_InputField start_counter_value;

    [Header("Labels")]
    public TextMeshProUGUI num_lanes_shooters;
    public TextMeshProUGUI scene_name_label;

    [Header("Holders")]
    public GameObject numStopSecondsHolder;
    public GameObject numBulletsHolder;
    public GameObject numMegsHolder;
    public GameObject numLanesHolder;
    public GameObject numStrikesHolder;
    public GameObject typeOfHuntHolder;
    public GameObject animalHolder;
    public GameObject numHitsHolder;
    public GameObject colorIndicatorHolder;
    public GameObject trainingLevelHolder;
    public GameObject trainingModeHolder;
    public GameObject numLaneTargetsHolder;
    public GameObject targetTypeHolder;
    public GameObject numTargetHolder;
    public GameObject mainCamera;
    public GameObject startCounterHolder;
    float speed = 0f;

    //Gloabal Variables
    [Header("Variables")]
    public static int numBullets = 10000;
    public static int setStopSeconds = 5;
    public static int numMegs = 1;
    public static int numLanes = 1;
    public static int numLanesLimit = 3;
    public static int setNumLanes = 1;
    public static int numLaneTargets = 1;
    public static int totalAllowedHitShots = 100;
    public static int numStrikes = 4;
    public static int total_test_time = 10000;
    public static string colorIndicator = "Shape"; //Default Setting
    public static string trainingLevel = "Easy"; //Default Setting
    public static string trainingMode = "Once-off"; //Default Setting
    public static string baloonDirection = "upwards";
    public static string typeOfHunt = "...select...";
    public static string animalName = "...select...";
    public static string lane1TraineeName = "Lane 1";
    public static string lane2TraineeName = "Lane 2";
    public static string lane3TraineeName = "Lane 3";
    public static string targetType = "Circle";

    [Header("Trainee Handlers")]
    public GameObject trainees_link;
    private List<string> loadedTrainees = new List<string>();
    //local Variables
    int range;
    string activeScene = " ";
    bool moveCamera = false;

    //global time variables
    public GameObject global_time_warning;
    bool warningActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        scene_name_label.text = DropDown.ompSceneName;
        numLanes = setNumLanes; // Reset to original setting
        MainMenuUDPSetup.FindLocalIP();

        ResetStaticVariables();
        if (true)
        {
            startCounterHolder.SetActive(true);
            start_counter_value.text = StaticVariableManager.start_counter.ToString();
            //ConfigureGuns();
        }

        //print("Scene name is: " + DropDown.softwareSceneName);
        if (DropDown.softwareSceneName.ToLower().Contains("hunting") && DropDown.softwareSceneName.ToLower().Contains("outdoor"))
        {
            print("Scene name is: " + DropDown.softwareSceneName);
            num_Hits.text = totalAllowedHitShots.ToString("0");
            totalTestTime.text = total_test_time.ToString("0");
            huntTypeLabel.text = typeOfHunt;
            animaNameLabel.text = animalName;
            populateOutdoorDropDown();
            moveCamera = false;
        }
        else if (DropDown.softwareSceneName.ToLower().Contains("hunting") && DropDown.softwareSceneName.ToLower().Contains("ground"))
        {
            print("Scene name is: " + DropDown.softwareSceneName);
            num_Hits.text = totalAllowedHitShots.ToString("0");
            totalTestTime.text = total_test_time.ToString("0");
            huntTypeLabel.text = typeOfHunt;
            animaNameLabel.text = animalName;
            populateGroundDropDown();
            moveCamera = false;
        }
        else if(DropDown.softwareSceneName.ToLower().Contains("lane"))
        {
            targetColorPanel.SetActive(false);
            populateTraineeDropDown();

            if (Scoring.ammo_setting.ToLower().Contains("laser"))
            {
                startCounterHolder.SetActive(true);
                //ConfigureGuns();
            }
            if(DropDown.softwareSceneName.ToLower().Contains("baloon"))
            {
                //numStrikesHolder.SetActive(true);
                colorIndicatorHolder.transform.position = numStrikesHolder.transform.position;
                colorIndicatorHolder.SetActive(true);
                populateColorIndicatorDropDown();

                if (DropDown.softwareSceneName.ToLower().Contains("upwards"))
                {
                    baloonDirection = "upwards";
                }
                if (DropDown.softwareSceneName.ToLower().Contains("downwards"))
                {
                    baloonDirection = "downwards";
                }
            }
            if (DropDown.softwareSceneName.ToLower().Contains("lane_suspect"))
            {
                populatetrainingLevelDropDown();
                populatetrainingModeDropDown();
                trainingLevelHolder.SetActive(true);
                trainingModeHolder.SetActive(false);
                num_StopSeconds.text = setStopSeconds.ToString("0");
                trainingModeLabel.text = trainingMode;
                trainingLevelLabel.text = trainingLevel;
                if (trainingMode == "Once-off")
                {
                    numStopSecondsHolder.SetActive(false);
                }
                else
                {
                    numStopSecondsHolder.SetActive(true);
                }
                trainingLevelHolder.transform.position = numStrikesHolder.transform.position;
            }
            if (DropDown.softwareSceneName.ToLower().Contains("cargame"))
            {
                num_lanes_shooters.text = "Number of Shooters:";
            }


            if (Scoring.ammo_setting.ToLower().Contains("live") || Scoring.ammo_setting.ToLower().Contains("laser"))
            {
                if(DropDown.softwareSceneName.ToLower().Contains("rising.laneplates"))//Rising.LanePlates
                {
                    targetTypeHolder.transform.position = colorIndicatorHolder.transform.position;
                }
                else
                {
                    targetTypeHolder.transform.position = numStrikesHolder.transform.position;
                    numTargetHolder.transform.position = colorIndicatorHolder.transform.position;
                }
                numLaneTargetsHolder.transform.position = numStrikesHolder.transform.position;
                numLanesHolder.transform.position = numMegsHolder.transform.position;
                numMegsHolder.transform.position = numBulletsHolder.transform.position;
                numBulletsHolder.transform.position = numHitsHolder.transform.position;
                
                numBulletsHolder.SetActive(true);
                numMegsHolder.SetActive(true);
                numHitsHolder.SetActive(false);
            }
            else
            {
                numLaneTargetsHolder.transform.position = numLanesHolder.transform.position;
                colorIndicatorHolder.transform.position = numStrikesHolder.transform.position;
                numStrikesHolder.transform.position = numLanesHolder.transform.position;
                numLanesHolder.transform.position = numBulletsHolder.transform.position;
            }

            //print(DropDown.softwareSceneName);
            if (DropDown.softwareSceneName.ToLower().Contains("plat") || DropDown.softwareSceneName.ToLower().Contains("dice") || DropDown.softwareSceneName.ToLower().Contains("lightpole")
                || DropDown.softwareSceneName.ToLower().Contains("dice") || DropDown.softwareSceneName.ToLower().Contains("hidden") || DropDown.softwareSceneName.ToLower().Contains("sequencenum")
                || DropDown.softwareSceneName.ToLower().Contains("colorsequence") || DropDown.softwareSceneName.ToLower().Contains("distancesimulator") )
            {
                targetType = StaticVariableManager.targetType;
                populateTargetTypeDropDown();
                targetTypeHolder.SetActive(true);

                if(DropDown.softwareSceneName.ToLower().Contains("colorsequence") || DropDown.softwareSceneName.ToLower().Contains("sequencenum"))
                {
                    populateNumTargetDropDown();
                    numTargetHolder.SetActive(true);
                }
            }

            moveCamera = true;
            numLanesHolder.SetActive(true);
            if(DropDown.softwareSceneName.ToLower().Contains("laneplates"))
            {
                numLaneTargetsHolder.SetActive(true);
            }

            num_bullets_per_Meg.text = numBullets.ToString("0");
            num_Megs.text = numMegs.ToString("0");
            num_Lanes.text = numLanes.ToString("0");
            num_strikes.text = numStrikes.ToString("0");
            num_Hits.text = totalAllowedHitShots.ToString("0");
            totalTestTime.text = total_test_time.ToString("0");
            num_LaneTargets.text = numLaneTargets.ToString("0");
            num_StopSeconds.text = setStopSeconds.ToString("0");
            colorIndicatorLabel.text = colorIndicator;
            targetTypeLabel.text = targetType;
        }
        else
        {
            targetColorPanel.SetActive(false);
            populateTraineeDropDown();

            
            if (Scoring.ammo_setting.ToLower().Contains("laser"))
            {
                //ConfigureGuns();
            }

            num_bullets_per_Meg.text = numBullets.ToString("0");
            num_Megs.text = numMegs.ToString("0");
            num_Hits.text = totalAllowedHitShots.ToString("0");
            totalTestTime.text = total_test_time.ToString("0");
            num_Lanes.text = numLanes.ToString("0");
            num_strikes.text = numStrikes.ToString("0");
            num_LaneTargets.text = numLaneTargets.ToString("0");

            if (Scoring.ammo_setting.ToLower().Contains("live"))
            {
                if(Scoring.ammo_setting.ToLower().Contains("remote"))
                {
                    numBulletsHolder.SetActive(false);
                    numMegsHolder.SetActive(false);
                }
                else
                {
                    numMegsHolder.transform.position = numBulletsHolder.transform.position;
                    numBulletsHolder.transform.position = numHitsHolder.transform.position;

                    numBulletsHolder.SetActive(true);
                    numMegsHolder.SetActive(true);
                    numHitsHolder.SetActive(false);
                }
            }
            else
            {
                numBulletsHolder.SetActive(true);
                numMegsHolder.SetActive(true);
            }

            moveCamera = true;
        }

        //print("RE: " + DropDown.softwareSceneName);
        if(DropDown.softwareSceneName.ToLower().Contains("plat") || DropDown.softwareSceneName.ToLower().Contains("hidden") 
            || DropDown.softwareSceneName.ToLower().Contains("ipec") || DropDown.softwareSceneName.ToLower().Contains("rifflepole") 
            || DropDown.softwareSceneName.ToLower().Contains("sequencenum") || DropDown.softwareSceneName.ToLower().Contains("duelingtree"))
        {
            targetColorPanel.SetActive(true);
        }
        
        if(!DropDown.softwareSceneName.ToLower().Contains("hunting"))
        {
            currentPlateColor.text = StaticVariableManager.targetColorSetting;
            currentBackroundColor.text = StaticVariableManager.backgroundColorSetting;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.enterKey.isPressed)
        {
            loadScene();
        }
        if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.escapeKey.isPressed)
        {
            SceneManager.LoadScene("MainMenu");
        }

        if(StaticVariableManager.set_trainees)
        {
            //populateTraineeDropDown();
            StaticVariableManager.set_trainees = false;
        }

        manageGlobalActiveTime();
        rotateMainCamera();

    }
    private void manageGlobalActiveTime()
    {
        login_Manager.global_active_timer -= Time.deltaTime * 1;

        if (login_Manager.global_active_timer <= 5 && warningActivated == false)
        {
            global_time_warning.SetActive(true);
            warningActivated = true;
        }
        if (login_Manager.global_active_timer <= 0)
        {
            SceneManager.LoadScene("LOGIN");
        }
    }
    private void rotateMainCamera()
    {
        mainCamera.transform.Rotate(Vector3.up, speed * Time.deltaTime);
        if (moveCamera)
        {
            if (speed <= 2.5f)
            {
                speed += Time.deltaTime * 1.8f;
            }
        }
        else
        {
            if (speed <= .25f)
            {
                speed += Time.deltaTime * 1f;
            }
        }

    }
    public void loadScene()
    {
        if (DropDown.softwareSceneName.ToLower().Contains("hunting"))
        {
            totalAllowedHitShots = int.Parse(num_Hits.text);
            total_test_time = int.Parse(totalTestTime.text);

            if (Scoring.ammo_setting.ToLower().Contains("laser"))
            {
                StaticVariableManager.start_counter = int.Parse(start_counter_value.text); ;
            }

            if (DropDown.softwareSceneName.ToLower().Contains("outdoor"))
            {
                if (typeOfHunt.ToLower().Contains("direct"))
                {
                    SceneManager.LoadScene("Outdoor_Direct_Deer_Hunting");
                }
                else if (typeOfHunt.ToLower().Contains("avoid"))
                {
                    SceneManager.LoadScene("Outdoor_Avoid_Deer_Hunting");
                }
            }

            if (DropDown.softwareSceneName.ToLower().Contains("ground"))
            {
                if (typeOfHunt.ToLower().Contains("direct"))
                {
                    SceneManager.LoadScene("Ground_Direct_Hunting");
                }
                else if (typeOfHunt.ToLower().Contains("avoid"))
                {
                    SceneManager.LoadScene("Ground_Avoid_Hunting");
                }
            }

        }
        else
        {
            numBullets = int.Parse(num_bullets_per_Meg.text);
            numMegs = int.Parse(num_Megs.text);
            totalAllowedHitShots = int.Parse(num_Hits.text);
            total_test_time = int.Parse(totalTestTime.text);
            numLanes = int.Parse(num_Lanes.text);
            numStrikes = int.Parse(num_strikes.text);
            numLaneTargets = int.Parse(num_LaneTargets.text);
            lane1TraineeName = lane1TraineeLabel.text;
            lane2TraineeName = lane2TraineeLabel.text;
            lane3TraineeName = lane3TraineeLabel.text;
            //setStopSeconds = int.Parse(num_StopSeconds.text);
            //colorIndicator = colorIndicatorLabel.text;

            if (numLanes > 3) numLanes = 3;
            if (numLanes == 3)
            {
                if (numLaneTargets > 2)
                {
                    numLaneTargets = 2;
                }
            }

            if (Scoring.ammo_setting.ToLower().Contains("live"))
            {
                StaticVariableManager.start_counter = int.Parse(start_counter_value.text);
                OpenScenario();
            }
            else
            {
                StaticVariableManager.start_counter = int.Parse(start_counter_value.text);
                OpenScenario();
            }
        }
    }
    public static void OpenScenario()
    {
        setNumLanes = numLanes; //Do not remove to properly reset number off lanes to selected setting.

        if (DropDown.softwareSceneName.ToLower().Contains("lane"))
        {
            //numLaneTargets

            if (DropDown.softwareSceneName.ToLower().Contains("circular"))
            {
                switch (TestConditionsManager.numLanes)
                {
                    case 1:
                        SceneManager.LoadScene("BasicCyclic1LaneShoot");
                        break;
                    case 2:
                        SceneManager.LoadScene("BasicCyclic2LaneShoot");
                        break;
                    case 3:
                        SceneManager.LoadScene("BasicCyclic3LaneShoot");
                        break;
                }
            }
            else if (DropDown.softwareSceneName.ToLower().Contains("laneplates"))
            {
                switch (numLanes)
                {
                    case 1:
                        switch (numLaneTargets)
                        {
                            case 1:
                                SceneManager.LoadScene("Basic1RisingPlate1LaneShoot");
                                break;
                            case 2:
                                SceneManager.LoadScene("Basic2RisingPlate1LaneShoot");
                                break;
                            case 3:
                                SceneManager.LoadScene("Basic3RisingPlate1LaneShoot");
                                break;
                            case 4:
                                SceneManager.LoadScene("Basic4RisingPlate1LaneShoot");
                                break;
                        }
                        break;
                    case 2:
                        switch (numLaneTargets)
                        {
                            case 1:
                                SceneManager.LoadScene("Basic1RisingPlate2LaneShoot");
                                break;
                            case 2:
                                SceneManager.LoadScene("Basic2RisingPlate2LaneShoot");
                                break;
                            case 3:
                                SceneManager.LoadScene("Basic3RisingPlate2LaneShoot");
                                break;
                            case 4:
                                SceneManager.LoadScene("Basic4RisingPlate2LaneShoot");
                                break;
                        }
                        break;
                    case 3:
                        switch (numLaneTargets)
                        {
                            case 1:
                                SceneManager.LoadScene("Basic1RisingPlate3LaneShoot");
                                break;
                            case 2:
                                SceneManager.LoadScene("Basic2RisingPlate3LaneShoot");
                                break;
                            case 3:
                                SceneManager.LoadScene("Basic2RisingPlate2LaneShoot");
                                break;
                            case 4:
                                SceneManager.LoadScene("Basic2RisingPlate2LaneShoot");
                                break;
                        }
                        break;
                }
            }
            else if (DropDown.softwareSceneName.ToLower().Contains("lane_baloons"))
            {
                if (numLanes > 2) numLanes = 2;
                switch (numLanes)
                {
                    case 1:
                        switch (colorIndicator)
                        {
                            case "Shape":
                                SceneManager.LoadScene("BasicCIBaloon1LaneShoot");
                                break;
                            case "Word":
                                SceneManager.LoadScene("BasicWCIBaloon1LaneShoot");
                                break;
                            case "Opp Word":
                                SceneManager.LoadScene("BasicOWCIBaloon1LaneShoot");
                                break;
                        }
                        break;
                    case 2:
                        switch (colorIndicator)
                        {
                            case "Shape":
                                SceneManager.LoadScene("BasicCIBaloon2LaneShoot");
                                break;
                            case "Word":
                                SceneManager.LoadScene("BasicWCIBaloon2LaneShoot");
                                break;
                            case "Opp Word":
                                SceneManager.LoadScene("BasicOWCIBaloon2LaneShoot");
                                break;
                        }
                        break;
                }
            }
            else if (DropDown.softwareSceneName.ToLower().Contains("lane_suspect"))
            {
                if (numLanes > 2) numLanes = 2;
                switch (numLanes)
                {
                    case 1:
                        SceneManager.LoadScene("Basic1LaneSuspectShoot");
                        break;
                    case 2:
                        SceneManager.LoadScene("Basic2LaneSuspectShoot");
                        break;
                }
            }
            else if (DropDown.softwareSceneName.ToLower().Contains("dueling"))
            {
                if (numLanes > 2) numLanes = 2;
                switch (numLanes)
                {
                    case 1:
                        SceneManager.LoadScene("Basic1LaneDuelingTree");
                        break;
                    case 2:
                        SceneManager.LoadScene("Basic1LaneDuelingTree");
                        break;
                }
            }
            else if (DropDown.softwareSceneName.ToLower().Contains("hiddentarget"))
            {
                if (numLanes > 1) numLanes = 1;
                switch (numLanes)
                {
                    case 1:
                        SceneManager.LoadScene("BasicHiddenTarget1LaneShoot");
                        break;
                    case 2:
                        SceneManager.LoadScene("BasicHiddenTarget1LaneShoot");
                        break;
                }
            }
            else if (DropDown.softwareSceneName.ToLower().Contains("fallingplat"))
            {
                if (numLanes > 1) numLanes = 1;
                switch (numLanes)
                {
                    case 1:
                        SceneManager.LoadScene("Basic1LaneFallingPlat");
                        break;
                    case 2:
                        SceneManager.LoadScene("Basic1LaneFallingPlat");
                        break;
                }
            }
            else if (DropDown.softwareSceneName.ToLower().Contains("block"))
            {
                if (numLanes > 1) numLanes = 1;
                switch (numLanes)
                {
                    case 1:
                        SceneManager.LoadScene("Basic1LaneBlockShooting");
                        break;
                }
            }
            else if (DropDown.softwareSceneName.ToLower().Contains("rifflepole"))
            {
                if (numLanes > 1) numLanes = 1;
                switch (numLanes)
                {
                    case 1:
                        SceneManager.LoadScene("Basic1LaneRifflePoleShooting");
                        break;
                }
            }
            else if (DropDown.softwareSceneName.ToLower().Contains("ipec"))
            {
                if (numLanes > 1) numLanes = 1;
                switch (numLanes)
                {
                    case 1:
                        SceneManager.LoadScene("Basic1LaneIPECBoard");
                        break;
                }
            }
            else if (DropDown.softwareSceneName.ToLower().Contains("freeshoot"))
            {
                if (numLanes > 1) numLanes = 1;
                switch (numLanes)
                {
                    case 1:
                        SceneManager.LoadScene("Basic1LaneTargetPopUpFreeShoot");
                        break;
                }
            }
            else if (DropDown.softwareSceneName.ToLower().Contains("cargame"))
            {
                if (numLanes > 3) numLanes = 3;
                if (numLanes < 2) numLanes = 2;
                switch (numLanes)
                {
                    case 2:
                        SceneManager.LoadScene("Basic2LaneCarGame");
                        break;

                    case 3:
                        SceneManager.LoadScene("Basic3LaneCarGame");
                        break;
                }
            }
            else
            {
                SceneManager.LoadScene(DropDown.softwareSceneName);
            }

        }
        else
        {

            if (DropDown.softwareSceneName.ToLower().Contains("targetpopupfreeshoot"))
            {

                SceneManager.LoadScene("Basic1LaneTargetPopUpFreeShoot");
            }
            else
            {

                SceneManager.LoadScene(DropDown.softwareSceneName);
            }
        }
    }
    private void populateOutdoorDropDown()
    {
        HuntTypeHandler.options.Clear();
        AnimalNameHandler.options.Clear();

        //training type handler
        HuntTypeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "...select..." });
        HuntTypeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Direct hunt" });
        HuntTypeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Avoid hunt" });

        //Animal Name handler
        AnimalNameHandler.options.Add(new TMP_Dropdown.OptionData() { text = "...select..." });
        AnimalNameHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Deer" });
        AnimalNameHandler.options.Add(new TMP_Dropdown.OptionData() { text = "boar" });
        AnimalNameHandler.options.Add(new TMP_Dropdown.OptionData() { text = "buffalo" });
    }
    private void populateColorIndicatorDropDown()
    {
        colorIndicatorHandler.options.Clear();

        //training type handler
        colorIndicatorHandler.options.Add(new TMP_Dropdown.OptionData() { text = "...select..." });
        colorIndicatorHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Shape" });
        colorIndicatorHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Word" });
        colorIndicatorHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Opp Word" });

    }
    private void populateTargetTypeDropDown()
    {
        targetTypeHandler.options.Clear();

        //training type handler
        targetTypeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "..select.." });
        targetTypeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Circle" });
        targetTypeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Square" });
        targetTypeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Rhombas" });
        targetTypeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Triangle" });
        targetTypeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Pentagon" });
        targetTypeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Octagon" });
    }
    private void populateNumTargetDropDown()
    {
        numTargetHandler.options.Clear();

        //training type handler
        numTargetLabel.text = StaticVariableManager.numberPlate.ToString();
        numTargetHandler.options.Add(new TMP_Dropdown.OptionData() {text = "..select.."});

        numTargetHandler.options.Add(new TMP_Dropdown.OptionData() { text = "4" });
        numTargetHandler.options.Add(new TMP_Dropdown.OptionData() { text = "5" });
        numTargetHandler.options.Add(new TMP_Dropdown.OptionData() { text = "6" });
        numTargetHandler.options.Add(new TMP_Dropdown.OptionData() { text = "7" });
        numTargetHandler.options.Add(new TMP_Dropdown.OptionData() { text = "8" });
    }
    private void populateTraineeDropDown()
    {
        lane1TraineeHandler.options.Clear();
        lane2TraineeHandler.options.Clear();
        lane3TraineeHandler.options.Clear();

        string [] traineeData = new string[10]; 
        if(DropDown.softwareSceneName.ToLower().Contains("NA"))
        {
            lane1TraineeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "...select..." });
            lane2TraineeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "N/A" });
            lane3TraineeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "N/A" });
            foreach (string item in GetTrainees.trainingNamesList)
            {
                //print("RE: Item is: "+item);
                traineeData = item.Split(":");
                lane1TraineeHandler.options.Add(new TMP_Dropdown.OptionData() { text = traineeData[1] });
            }
        }//Temporarily Not Applicable
        else if (DropDown.softwareSceneName.ToLower().Contains("NA"))
        {
            lane1TraineeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "...select..." });
            lane2TraineeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "...select..." });
            lane3TraineeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "N/A" });
            foreach (string item in GetTrainees.trainingNamesList)
            {
                //print("RE: Item is: "+item);
                traineeData = item.Split(":");
                lane1TraineeHandler.options.Add(new TMP_Dropdown.OptionData() { text = traineeData[1] });
                lane2TraineeHandler.options.Add(new TMP_Dropdown.OptionData() { text = traineeData[1] });
            }
        } //Temporarily Not Applicable
        else
        {
            lane1TraineeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "...select..." });
            lane2TraineeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "...select..." });
            lane3TraineeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "...select..." });
            lane1TraineeLabel.text = lane1TraineeName;
            lane2TraineeLabel.text = lane2TraineeName;
            lane3TraineeLabel.text = lane3TraineeName;
            int currIndex = 0;
            foreach (string item in GetTrainees.trainingNamesList)
            {
                
                traineeData = item.Split(":");


                /*traineeData[1].ToLower().Contains("lane 1") || traineeData[1].ToLower().Contains("lane_1")
                    || traineeData[1].ToLower().Contains("lane 2") || traineeData[1].ToLower().Contains("lane_2")
                    || traineeData[1].ToLower().Contains("lane 3") || traineeData[1].ToLower().Contains("lane_3")*/

                if (traineeData[1].ToLower().Contains("lane"))
                {
                    //print("Trainee: " + traineeData[1]);

                    if(traineeData[1].Contains("1"))
                    {
                        // GetTrainees.TraineeLane_1
                        GetTrainees.TraineeLane_1 = GetTrainees.trainingNamesList[currIndex];
                        lane1TraineeName = traineeData[1];
                    }
                    else if (traineeData[1].Contains("2"))
                    {
                        GetTrainees.TraineeLane_2 = GetTrainees.trainingNamesList[currIndex];
                        lane2TraineeName = traineeData[1];
                    } 
                    else if (traineeData[1].Contains("3"))
                    {
                        GetTrainees.TraineeLane_3 = GetTrainees.trainingNamesList[currIndex];
                        lane3TraineeName = traineeData[1];
                    }
                    
                }
                else
                {

                    lane1TraineeHandler.options.Add(new TMP_Dropdown.OptionData() { text = traineeData[1] });
                    lane2TraineeHandler.options.Add(new TMP_Dropdown.OptionData() { text = traineeData[1] });
                    lane3TraineeHandler.options.Add(new TMP_Dropdown.OptionData() { text = traineeData[1] });

                    loadedTrainees.Add(item);
                }

                currIndex++;
            
            }
        }

    }

    private void populatetrainingLevelDropDown()
    {
        trainingLevelHandler.options.Clear();

        //training type handler
        trainingLevelHandler.options.Add(new TMP_Dropdown.OptionData() { text = "...select..." });
        trainingLevelHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Easy" });
        trainingLevelHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Medium" });
        trainingLevelHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Hard" });

    }

    private void populatetrainingModeDropDown()
    {
        trainingModeHandler.options.Clear();
                
        //trainiModepe handler
        trainingModeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "...select..." });
        trainingModeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Once-off" });
        trainingModeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Continuous" });
    }

    private void populateGroundDropDown()
    {
        HuntTypeHandler.options.Clear();
        AnimalNameHandler.options.Clear();

        //training type handler
        HuntTypeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "...select..." });
        //HuntTypeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Direct hunt" });
        HuntTypeHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Avoid hunt" });

        //Animal Name handler
        AnimalNameHandler.options.Add(new TMP_Dropdown.OptionData() { text = "...select..." });
        AnimalNameHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Rabbit" });
        AnimalNameHandler.options.Add(new TMP_Dropdown.OptionData() { text = "Goose" });
        AnimalNameHandler.options.Add(new TMP_Dropdown.OptionData() { text = "comodo" });
        AnimalNameHandler.options.Add(new TMP_Dropdown.OptionData() { text = "pig" });
    }
    public void ConfigureGuns()
    {
        if (Scoring.gun == "Handgun")
        {
            numBullets = 15;
        }
        else if (Scoring.gun == "Riffle")
        {
            numBullets = 30;
        }
        else if (Scoring.gun == "Short gun")
        {
            numBullets = 16;
        }
        else if (Scoring.gun == "CZ 75 B omega")
        {
            numBullets = 16;
        }
        else if (Scoring.gun == "Glock 42 slimline")
        {
            numBullets = 6;
        }
        else if (Scoring.gun == "Taurus G2C")
        {
            numBullets = 16;
        }
        else if (Scoring.gun == "Ruger 9mms LCP")
        {
            numBullets = 6;
        }
        else if (Scoring.gun == "CO2 Hand Gun")
        {
            numBullets = 15;
        }
        else
        {
            numBullets = 15;
        }
    }
    public void HandleHuntTypeInput(int val)
    {
        //print(val);
        typeOfHunt = huntTypeLabel.text;

    }
    public void HandleAnimalNameInput(int val)
    {
        //print(val);
        animalName = animaNameLabel.text;

    }
    public void HandleColorIndicatorInput(int val)
    {
        colorIndicator = colorIndicatorLabel.text;
        //print("RE: Color indicator is " + colorIndicator);
    }
    public void HandleTargetTypeInput(int val)
    {
        targetType = targetTypeLabel.text;
        StaticVariableManager.targetType = targetType.ToLower();
        //print("RE: Color indicator is " + colorIndicator);
    }
    public void HandleNumTargetInput(int val)
    {
        StaticVariableManager.numberPlate = int.Parse(numTargetLabel.text);
        //print("RE: Color indicator is " + colorIndicator);
    }
    public void HandleTrainingLevel(int val)
    {
        trainingLevel = trainingLevelLabel.text;
        //print("RE: training level is " + trainingLevel);
    }
    public void HandleTrainingMode(int val)
    {
        trainingMode = trainingModeLabel.text;
        //print("RE: training mode is " + trainingLevel);
        if (trainingMode == "Once-off")
        {
            numStopSecondsHolder.SetActive(false);
        }
        else if (trainingMode == "Continuous")
        {
            numStopSecondsHolder.SetActive(true);
        }
    }
    public void HandleLane1TraineeNames(int val)
    {
        //int traineeIndex = 0;

        GetTrainees.TraineeLane_1 = loadedTrainees[val - 1];
        lane1TraineeName = lane1TraineeLabel.text;

        //print("RRE: Selected Lane 1 trainee " + lane1TraineeName);
        //print("Selected index : " + (val - 1).ToString());
        //print("RRE: Test Lane 1 trainee " + GetTrainees.TraineeLane_1);
    }
    public void HandleLane2TraineeNames(int val)
    {
        GetTrainees.TraineeLane_2 = loadedTrainees[val - 1];
        lane2TraineeName = lane2TraineeLabel.text;
        //print("RRE: Lane 2 trainee " + GetTrainees.TraineeLane_2);
    }
    public void HandleLane3TraineeNames(int val)
    {
        GetTrainees.TraineeLane_3 = loadedTrainees[val - 1];
        lane3TraineeName = lane3TraineeLabel.text;
        //print("RRE: Lane 3 trainee " + GetTrainees.TraineeLane_3);
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("SceneManager");
    }
    private void ResetStaticVariables()
    {
        GetTrainees.TraineeLane_1 = "";
        GetTrainees.TraineeLane_2 = "";
        GetTrainees.TraineeLane_3 = "";

        lane1TraineeName = "Lane 1";
        lane2TraineeName = "Lane 2";
        lane3TraineeName = "Lane 3";
    }   
}       
