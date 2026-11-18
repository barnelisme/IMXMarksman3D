using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class ControlButtonsManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject resetPanel;
    [SerializeField] private GameObject resetButton;
    [SerializeField] private GameObject gunShotSwitchImage;
    [SerializeField] private GameObject gunShotSwitch;
    [SerializeField] private GameObject smokeImpactSwitchImage;
    [SerializeField] private GameObject smokeImpactSwitch;
    private Image panelImage;
    private Color currentColor;
    float resetTimer , setResetTime = 10, newAlpha = 0.5f;
    private bool pointsReset = true, incrementAlpha = true, decrementAlpha = false, closingReset = false;
    private float crementSpeed = 0.355f;
    private string activeScene = "";
    private bool isTrainingPaused = false;
    private bool isTrainingStopped = false;
    private bool isScoreBoardShown = false;
    private bool isEnterButtonPressed = false;

    [Header("Public Objects")]
    public TextMeshProUGUI pause_continue_txt;

    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        player = GameObject.FindGameObjectWithTag("Player");

        StaticVariableManager.isEnded = false;
        //resetButton.SetActive(false); // disable after assigning
        resetTimer = setResetTime;
        isTrainingStopped = false;

        if (StaticVariableManager.isResetingPoints)
        {
            resetPanel.SetActive(true);
            pointsReset = false;
        }

        //Setup reset panel
        setUpResetPanel();
        // pause_continue_txt.text = "Pause";
        //

        string smokeImpactSetting = FileManager.ReadSetting("SmokeImpactSetting");
        string gunSetting = FileManager.ReadSetting("GunShotSetting");

        if (Scoring.ammo_setting.ToLower().Contains("live"))
        {
            Scoring.shooting_PaperRoll_Setting = "static";
            if (Scoring.shooting_PaperRoll_Setting.ToLower().Contains("static"))
            {
                resetButton.SetActive(true);
            }
            else
            {
                resetButton.SetActive(false);
            }

            SetupSmokeImpact(smokeImpactSetting); // Setup smoke impact switch
        }
        else if(Scoring.ammo_setting.ToLower().Contains("laser"))
        {
            resetButton.SetActive(false);

            SetupGunSoundSetting(gunSetting); // Setup gun sound setting switch
            SetupSmokeImpact(smokeImpactSetting); // Setup smoke impact switch
        }

    }

    private void SetupGunSoundSetting(string setting)
    {
        // Setup gun shot settings
        switch (setting)
        {
            case "True":
                StaticVariableManager.gunShotSetting = true;
                gunShotSwitchImage.GetComponent<Image>().color = Color.green;
                break;
            case "False":
                StaticVariableManager.gunShotSetting = false;
                gunShotSwitchImage.GetComponent<Image>().color = Color.grey;
                break;

        }


        // Enable switch
        gunShotSwitch.SetActive(true);
    }

    private void SetupSmokeImpact(string setting)
    {
        // Setup smoke impact settings
        // print("Setting is: " + setting);
        switch (setting)
        {
            case "True":
                StaticVariableManager.smokeImpactSetting = true;
                smokeImpactSwitchImage.GetComponent<Image>().color = Color.green;
                break;
            case "False":
                StaticVariableManager.smokeImpactSetting = false;
                smokeImpactSwitchImage.GetComponent<Image>().color = Color.grey;
                break;

        }

        // Enable switch
        smokeImpactSwitch.SetActive(true);
    }

    private void setUpResetPanel()
    {
        panelImage = resetPanel.GetComponent<Image>();
        currentColor = panelImage.color;
        Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, 50/255f);
        panelImage.color = newColor;
        newAlpha = 50 / 255f;
    }

    // Update is called once per frame
    void Update()
    {

        if (Keyboard.current.enterKey.isPressed && !StaticVariableManager.startCountDown)
        {
            StartScenario();
            isEnterButtonPressed = true;
        }

        if (!pointsReset)
        {

            if (resetTimer <= 0f)
            {
                if (!closingReset)
                {
                    closingReset = true;
                    crementSpeed = 0.355f;
                }
                else
                {
                    newAlpha = newAlpha - Time.deltaTime * crementSpeed;
                    Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
                    panelImage.color = newColor;
                    if ((newAlpha * 255) < 25)
                    {
                        resetPanel.SetActive(false);
                        resetTimer = setResetTime;
                        StaticVariableManager.isResetingPoints = false;
                        player.GetComponent<Shooting>().sendEndless("Simulate");
                        pointsReset = true;
                        closingReset = false;
                        incrementAlpha = true;
                        decrementAlpha = false;
                    }
                }
            }
            else
            {
                resetTimer -= Time.deltaTime;
                if (incrementAlpha)
                {
                    if((newAlpha * 255) <= 250)
                    {
                        newAlpha = newAlpha + Time.deltaTime * crementSpeed;
                        Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
                        panelImage.color = newColor;
                        if((newAlpha * 255) > 245)
                        {
                            incrementAlpha = false;
                            decrementAlpha = true;
                            crementSpeed = 0.05f;
                        }
                    }
                }
                if(decrementAlpha)
                {
                    if ((newAlpha * 255) >= 220)
                    {
                        newAlpha = newAlpha - Time.deltaTime * crementSpeed;
                        Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
                        panelImage.color = newColor;
                        if ((newAlpha * 255) < 230)
                        {
                            incrementAlpha = true;
                            decrementAlpha = false;
                        }
                    }
                }
            }
        }
    }

    public void sendUDPResetSignal()
    {
        if (StaticVariableManager.isResetingPoints) return; // Do not run if Resetting

        Time.timeScale = 1f;
        player.GetComponent<Shooting>().sendEndless("Reset");
        resetPanel.SetActive(true);
        pointsReset = false;
        StaticVariableManager.isResetingPoints = true;
        setUpResetPanel();
        StaticVariableManager.recentPoints.Clear();
        //resetPanel.GetComponent<Texture2D>()
    }

    public void loadMainMenu()
    {
        Time.timeScale = 1f;
        StaticVariableManager.isResetingPoints = false;

        if (activeScene.ToLower().Contains("calibration"))
        {
            SceneManager.LoadScene("MainMenu");
        }
        else if(activeScene.ToLower().Contains("hunting"))
        {
            if(activeScene.ToLower().Contains("birdhunting"))
            {
                SceneManager.LoadScene("TestConditionSetting");
            }
            else
            {
                SceneManager.LoadScene("HuntingConditionSetting");
            }
        }
        else if (DropDown.scene_type == "3D")
        {
            SceneManager.LoadScene("SceneManager");
        }
        else if (activeScene.Contains("FallingPlatTest"))
        {
            SceneManager.LoadScene("Calib_Acc_Page");
        }
        else
        {
            StaticVariableManager.startCountDown = false;
            SceneManager.LoadScene("TestConditionSetting");
        }
    }

    public void StartScenario()
    {
        if (StaticVariableManager.isResetingPoints) return; // Do not run if Resetting

        if (StaticVariableManager.startCountDown || StaticVariableManager.isStopTraining)
        {
            Time.timeScale = 1f;
            StaticVariableManager.startCountDown = false;
            SceneManager.LoadScene(activeScene);
        }
        else
        {
            StaticVariableManager.startCountDown = true;

            if(DropDown.scene_type == "3D")
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                // player.GetComponent<FirstPersonController>().LockMouse();
            }
        }
    }

    public void HandlePauseContinue()
    {
        if (StaticVariableManager.isResetingPoints) return; // Do not run if Resetting
        //print("Resetting...");
        if (isTrainingPaused == false)
        {
            pause_continue_txt.text = "Resume";
            Time.timeScale = 0f;
            StaticVariableManager.isEnded = true;
            isTrainingPaused = true;
        }
        else
        {
            pause_continue_txt.text = "Pause";
            Time.timeScale = 1f;
            StaticVariableManager.isEnded = false;
            isTrainingPaused = false;
        }
    }

    public void HandleGunShotSetting()
    {
        //print("Resetting...");
        if (StaticVariableManager.gunShotSetting == false)
        {
            StaticVariableManager.gunShotSetting = true;
            // Set gun shot color to green
            Color newColor = Color.green;
            gunShotSwitchImage.GetComponent<Image>().color = newColor;
            FileManager.SaveSetting("GunShotSetting", StaticVariableManager.gunShotSetting.ToString());
        }
        else
        {
            StaticVariableManager.gunShotSetting = false;
            // Set gun shot color to black
            Color newColor = Color.grey;
            gunShotSwitchImage.GetComponent<Image>().color = newColor;
            FileManager.SaveSetting("GunShotSetting", StaticVariableManager.gunShotSetting.ToString());
        }
    }

    public void HandleSmokeImpactSetting()
    {
        //print("Resetting...");
        if (StaticVariableManager.smokeImpactSetting == false)
        {
            StaticVariableManager.smokeImpactSetting = true;
            // Set gun shot color to green
            Color newColor = Color.green;
            smokeImpactSwitchImage.GetComponent<Image>().color = newColor;
            FileManager.SaveSetting("SmokeImpactSetting", StaticVariableManager.smokeImpactSetting.ToString());
        }
        else
        {
            StaticVariableManager.smokeImpactSetting = false;
            // Set gun shot color to black
            Color newColor = Color.grey;
            smokeImpactSwitchImage.GetComponent<Image>().color = newColor;
            FileManager.SaveSetting("SmokeImpactSetting", StaticVariableManager.smokeImpactSetting.ToString());
        }

        // print("Setting is: " + StaticVariableManager.smokeImpactSetting.ToString());
    }

    public void HandleScoreButton()
    {
        //GameObject controlManager = GameObject.FindGameObjectWithTag("Player");
        if (StaticVariableManager.isResetingPoints) return; // Do not run if Resetting

        if (StaticVariableManager.isStopTraining == false)
        {
            StaticVariableManager.isStopTraining = true;
            isTrainingStopped = true;
            isScoreBoardShown = false;
        }
        else
        {

            if (player.GetComponent<Shooting>().is3DScene)
            {
                if (isScoreBoardShown)
                {
                    //Disable Scores
                    player.GetComponent<Shooting>().shooterScorePanel.SetActive(false);
                    isScoreBoardShown = false;
                }
                else
                {
                    //Enable scores
                    player.GetComponent<Shooting>().shooterScorePanel.SetActive(true);
                    isScoreBoardShown = true;
                }
            }
            else
            {
                if (isScoreBoardShown)
                {
                    //Disable Scores
                    player.GetComponent<Shooting>().adminScorePanel.SetActive(false);
                    player.GetComponent<Shooting>().shooterScorePanel.SetActive(false);
                    player.GetComponent<Shooting>().splitTimePanel.SetActive(false);
                    player.GetComponent<Shooting>().isSplitTimeOpen = false;  // was static on shotting

                    if(ShotsReplayManager.isShotsReplayOpen)
                    {
                        player.GetComponent<ShotsReplayManager>().handleShotsReplayButton();
                    }


                    isScoreBoardShown = false;
                }
                else
                {
                    //Enable scores
                    player.GetComponent<Shooting>().adminScorePanel.SetActive(true);
                    player.GetComponent<Shooting>().shooterScorePanel.SetActive(true);


                    isScoreBoardShown = true;
                }
            }

        }

    }

}
