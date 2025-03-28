using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class AdminShortcutManager : MonoBehaviour
{
    public TextMeshProUGUI buttonLabel;
    public TextMeshProUGUI laneNumberText;
    public GameObject shortcutButton;
    public GameObject sceneNamesOptions;
    public GameObject controlButtonsManager;

    //Control Variables
    private bool isLoadTestPressed = false;
    private bool isResetPressed = false;
    private bool isStartPressed = false;
    private bool isButtonSet = false;
    private bool currResetStatus = false;
    private bool currTrainigStatus = false;
    private bool currCountDownStatus = false;

    void Start()
    {
        buttonLabel.text = "Reset Paper";
        controlButtonsManager = GameObject.FindGameObjectWithTag("ControlButtonsManager");
        UpdateShortcutLaneText();
    }

    // Update is called once per frame
    void Update()
    {
        ManageButton();
    }

    private void ManageButton()
    {
        if (currResetStatus != StaticVariableManager.isResetingPoints)
        {
            if (StaticVariableManager.isResetingPoints)
            {
                shortcutButton.SetActive(false);
            }
            else
            {
                shortcutButton.SetActive(true);
            }

            currResetStatus = StaticVariableManager.isResetingPoints;
            //isButtonSet = false;
        }
        if (currTrainigStatus != StaticVariableManager.isTrainingPause)
        {
            currTrainigStatus = StaticVariableManager.isTrainingPause;
            isStartPressed = true;
            isResetPressed = false;

            isButtonSet = false;
        }
        if(currCountDownStatus != StaticVariableManager.startCountDown)
        {
            currCountDownStatus = StaticVariableManager.startCountDown;

            if(StaticVariableManager.startCountDown)
            {
                shortcutButton.SetActive(false);
                isStartPressed = true;
            }
        }

        if(isButtonSet == false)
        {
            if (isResetPressed)
            {
                buttonLabel.text = "Start";
            }
            else if (isStartPressed)
            {
                buttonLabel.text = "Load Test";
            }

            if(StaticVariableManager.isTrainingPause == true)
            {
                shortcutButton.SetActive(true);
            }

            isButtonSet = true;
        }
    }

    public void IncreaseNumLanes()
    {
        if (TestConditionsManager.numLanes < TestConditionsManager.numLanesLimit)
        {
            TestConditionsManager.numLanes++;
            UpdateShortcutLaneText();
        }
    }

    public void DecreaseNumLanes()
    {
        if (TestConditionsManager.numLanes > 1)
        {
            TestConditionsManager.numLanes--;
            UpdateShortcutLaneText();
        }
    }

    private void UpdateShortcutLaneText()
    {
        laneNumberText.text = "Lane: " + TestConditionsManager.numLanes;
    }

    public void HandleButton()
    {
        if (StaticVariableManager.isTrainingPause == false)
        { 
        
            if(isResetPressed == false)
            {
                controlButtonsManager.GetComponent<ControlButtonsManager>().sendUDPResetSignal();
                isResetPressed = true;
                shortcutButton.SetActive(false);

                isButtonSet = false;
            }
            else
            {
                controlButtonsManager.GetComponent<ControlButtonsManager>().StartScenario();
                isStartPressed = true;
                isResetPressed = false;
                shortcutButton.SetActive(false);

                isButtonSet = false;
            }
        }
        else
        {
            print("Pressed");
            //buttonLabel.text = "Load Test";
            if(isLoadTestPressed == false)
            {
                isLoadTestPressed = true;
                sceneNamesOptions.SetActive(true);
                DropDown.softwareSceneName = "";
            }
            else
            {
                if(DropDown.softwareSceneName != "")
                {
                    //TestConditionsManager.numLanes = TestConditionsManager.setNumLanes; // Reset to original setting
                    TestConditionsManager.OpenScenario();
                }
            }
        }

    }
}
