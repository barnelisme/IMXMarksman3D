using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class DescriptionManager : MonoBehaviour
{
    public TextMeshProUGUI sceneName;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI timeLeft;
    public TextMeshProUGUI startCommand;
    float displayTimer =20;
    public string lines;
    public float textSpeed = 0.001f;
    float loadingTimer = 0f;

    //global time variables
    public GameObject global_time_warning;
    bool warningActivated = false;

    private int index;

    // Start is called before the first frame update
    void Start()
    {
        resetTime();
        sceneName.text = "Name: " + DropDown.softwareSceneName;
        Description.text = string.Empty;

        if (DropDown.softwareSceneName.ToLower().Contains("mall") || DropDown.softwareSceneName.ToLower().Contains("plain") || DropDown.softwareSceneName.ToLower().Contains("parking") || DropDown.softwareSceneName.ToLower().Contains("containers"))
        {
            lines = "On the Loading training, the objective is to shoot the people that look suspicious, and save the civilians. Note that shooting any civilian" +
                "will lead to a drop in your score.";
            StartDialogue();
        }
        else if (DropDown.softwareSceneName.ToLower().Contains("forest"))
        {
            lines = "On the Loading simulation, the objective is to shoot the enemy soldiers around the forest. Note that if the enemies kill you first or you use more ammo missing shots, your score will drop " +
                "and lead to a mission failure.";
            StartDialogue();
        }
        else if (DropDown.softwareSceneName.ToLower().Contains("fallingplat"))
        {
            sceneName.text = "Name: Falling Plate";
            lines = "On the Loading simulation, the objective is to shoot the plates as they appear in random positions. Shoot all the set amount of plates accurately to obtain good results. " +
                "Note that not finishing the plates in time, or running out of time will lead to a mission failure.";
            StartDialogue();
        }
        else if (DropDown.softwareSceneName.ToLower().Contains("rifflepole"))
        {
            sceneName.text = "Name: Basic Riffle Pole";
            lines = "On the Loading simulation, the objective is to shoot the plates attached to the pole. Each target has an indicator light above it, " +
                "make sure you Only shoot the target that has its indicator light turned ON. Note that shooting a target without its indicator light turned on, will" +
                "drop your final score.";
            StartDialogue();
        }
        else if (DropDown.softwareSceneName.ToLower().Contains("dueling"))
        {
            sceneName.text = "Name: Basic Dueling Tree";
            lines = "On the Loading simulation, the objective is to shoot the plates attached to the pole. Note that missing the targets will drop your " +
                "final score. ";
            StartDialogue();
        }
        else if (DropDown.softwareSceneName.ToLower().Contains("resetable"))
        {
            sceneName.text = "Name: Resetable Plates";
            lines = "On the Loading simulation, the objective is to shoot the plates attached to the pole. Note that missing the targets will drop your " +
                "final score. ";
            StartDialogue();
        }
        else if (DropDown.softwareSceneName.ToLower().Contains("ipec"))
        {
            sceneName.text = "Name: IPEC Board";
            lines = "On the Loading simulation, the objective is to shoot the plates next to the board. Make sure you know the total number of plates to shoot for the training to complete" +
                ". Note that shooting the board five times will lead to a mission failure.";
            StartDialogue();
        }
        else if (DropDown.softwareSceneName.ToLower().Contains("targetpopup"))
        {
            sceneName.text = "Name: IPEC Board";
            lines = "On the Loading simulation, the objective is to shoot the set targets on the board. " +
                "Note that missing the heard or body target will lead to a mission failure.";
            StartDialogue();
        }
        else if (DropDown.softwareSceneName.ToLower().Contains("circular"))
        {
            sceneName.text = "Name: Cyclic targets";
            lines = "On the Loading simulation, the objective is to shoot the plates on your dedicated lane. Make sure you know the total number of targets to shoot for the training to complete" +
                ". Note that missing shots will lead to a mission failure.";
            StartDialogue();
        }
        else if (DropDown.softwareSceneName.ToLower().Contains("lane_baloons"))
        {
            sceneName.text = "Name: Cyclic targets";
            lines = "On the Loading simulation, the objective is to shoot the baloon on your dedicated lane. Make sure you pay attention to the color indicator on the bottom" +
                " to know which baloon to shoot for the training to complete. Note that shooting the wrong baloon will lead to a mission failure.";
            StartDialogue();
        }
        else
        {
            lines = "Still in preparations...";
            StartDialogue();
        }

    }

    private void resetTime()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.escapeKey.isPressed)
        {
            SceneManager.LoadScene("MainMenu");
        }

        if (Keyboard.current.enterKey.isPressed)
        {
            if (DropDown.softwareSceneName.ToLower().Contains("lane"))
            {
                if(DropDown.softwareSceneName.ToLower().Contains("circular"))
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
                else if(DropDown.softwareSceneName.ToLower().Contains("laneplates"))
                {
                   
                    switch (TestConditionsManager.numLanes)
                    {
                        case 1:
                            switch (TestConditionsManager.numLaneTargets)
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
                            switch (TestConditionsManager.numLaneTargets)
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
                            switch (TestConditionsManager.numLaneTargets)
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
                    switch (TestConditionsManager.numLanes)
                    {
                        case 1:
                            switch (TestConditionsManager.colorIndicator)
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
                            switch (TestConditionsManager.colorIndicator)
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
                    switch (TestConditionsManager.numLanes)
                    {
                        case 1:
                            SceneManager.LoadScene("Basic1LaneSuspectShoot");
                            break;
                        case 2:
                            SceneManager.LoadScene("Basic2LaneSuspectShoot");
                            break;
                    }
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

        if (displayTimer <= 0f)
        {
            

            loadingTimer += Time.deltaTime * 1.2f;
            if(loadingTimer >=0 && loadingTimer < 1)
            {
                //startCommand.color = Color.red;
                startCommand.text = "Press Enter to START";
                startCommand.enabled = true;
            }
            else if(loadingTimer >= 1 && loadingTimer < 2)
            {
                startCommand.enabled = false;
            }
            else if (loadingTimer >= 2)
            {
                loadingTimer = 0f;
            }

            if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return))
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
        else
        {
            displayTimer -= Time.deltaTime * 1;
            //timeLeft.text = "Load Time: " + displayTimer.ToString("0");

            loadingTimer += Time.deltaTime * 1.5f;
            if(loadingTimer >= 0 && loadingTimer < 1)
            {
                startCommand.text = "Check Other Display";
            }
            else if (loadingTimer >= 1 && loadingTimer < 2)
            {
                startCommand.text = ".Check Other Display.";
            }
            else if (loadingTimer >= 2 && loadingTimer < 3)
            {
                startCommand.text = "..Check Other Display..";
            }
            else if (loadingTimer >= 3 && loadingTimer < 4)
            {
                startCommand.text = "...Check Other Display...";
            }
            else if (loadingTimer >= 4)
            {
                loadingTimer = 0f;
            }

        }
        manageGlobalActiveTime();
    }
    private void manageGlobalActiveTime()
    {
        login_Manager.global_active_timer -= Time.deltaTime * 1;

        if (login_Manager.global_active_timer <= 5 && warningActivated == false)
        {
            //global_time_warning.SetActive(true);
            warningActivated = true;
        }
        if (login_Manager.global_active_timer <= 0)
        {
            SceneManager.LoadScene("LOGIN");
        }
    }
    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }
    IEnumerator TypeLine()
    {
        foreach(char c in lines.ToCharArray())
        {
            Description.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}
