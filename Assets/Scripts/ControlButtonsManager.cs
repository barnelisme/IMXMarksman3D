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
    private Image panelImage;
    private Color currentColor;
    float resetTimer , setResetTime = 2, newAlpha = 0.5f;
    private bool pointsReset = true, incrementAlpha = true, decrementAlpha = false, closingReset = false;
    private float crementSpeed = 0.355f;
    private string activeScene = "";
    private bool isTrainingPaused = false;
    private bool isTrainingStopped = false;
    private bool isScoreShown = false;
    private bool isEnterButtonPressed = false;

    [Header("Public Objects")]
    public TextMeshProUGUI pause_continue_txt;

    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        player = GameObject.FindGameObjectWithTag("Player");
        resetButton = GameObject.FindGameObjectWithTag("ResetButton");

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
        //

        if (Scoring.ammo_setting.ToLower().Contains("live"))
        {
            if (Scoring.shooting_PaperRoll_Setting.ToLower().Contains("static"))
            {
                resetButton.SetActive(true);
            }
            else
            {
                resetButton.SetActive(false);
            }
        }

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
        Time.timeScale = 1f;
        player.GetComponent<Shooting>().sendEndless("Reset");
        resetPanel.SetActive(true);
        pointsReset = false;
        StaticVariableManager.isResetingPoints = true;
        setUpResetPanel();
        //resetPanel.GetComponent<Texture2D>()
    }

    public void loadMainMenu()
    {
        Time.timeScale = 1f;
        if (activeScene.ToLower().Contains("calibration"))
        {
            SceneManager.LoadScene("MainMenu");
        }
        else if(activeScene.ToLower().Contains("indoor_range"))
        {
            SceneManager.LoadScene("SceneManager");
        }
        else
        {
            StaticVariableManager.startCountDown = false;
            SceneManager.LoadScene("TestConditionSetting");
        }
    }

    public void StartScenario()
    {
        if (StaticVariableManager.startCountDown)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(activeScene);
        }
        else
        {
            StaticVariableManager.startCountDown = true;
        }
    }

    public void HandlePauseContinue()
    {
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
            pause_continue_txt.text = "End";
            Time.timeScale = 1f;
            StaticVariableManager.isEnded = false;
            isTrainingPaused = false;
        }
    }

    public void HandleScoreButton()
    {
        //GameObject controlManager = GameObject.FindGameObjectWithTag("Player");

        if (StaticVariableManager.isStopTraining == false)
        {
            StaticVariableManager.isStopTraining = true;
            isTrainingStopped = true;
            isScoreShown = true;
        }
        else
        {

            if (player.GetComponent<Shooting>().is3DScene)
            {
                if (isScoreShown)
                {
                    //Disable Scores
                    player.GetComponent<Shooting>().shooterScorePanel.SetActive(false);
                    isScoreShown = false;
                }
                else
                {
                    //Enable scores
                    player.GetComponent<Shooting>().shooterScorePanel.SetActive(true);
                    isScoreShown = true;
                }
            }
            else
            {
                if (isScoreShown)
                {
                    //Disable Scores
                    player.GetComponent<Shooting>().adminScorePanel.SetActive(false);
                    player.GetComponent<Shooting>().shooterScorePanel.SetActive(false);
                    player.GetComponent<Shooting>().splitTimePanel.SetActive(false);
                    player.GetComponent<Shooting>().isSplitTimeOpen = false;  // was static on shotting

                    player.GetComponent<ShotsReplayManager>().ManageScoringReplay();

                    isScoreShown = false;
                }
                else
                {
                    //Enable scores
                    player.GetComponent<Shooting>().adminScorePanel.SetActive(true);
                    player.GetComponent<Shooting>().shooterScorePanel.SetActive(true);
                    //player.GetComponent<Shooting>().splitTimePanel.SetActive(true);

                    player.GetComponent<ShotsReplayManager>().DisableReplayScore();


                    isScoreShown = true;
                }
            }

        }

    }

}
