using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections.Generic;

public class VideoSettingsManager : MonoBehaviour
{
    public Image imageObject;
    public Sprite[] videoOptions;
    public TextMeshProUGUI currentOptionText;
    private int optionIndex = 0;
    private List<string> transitionGroups = new List<string>() { "G13" };
    // string currentTargetName = "";

    void Start()
    {
        // imageObject.im
        SetStartOption();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateImage(Image img)
    {
        // Ensure all Image names match the Image Options
        string currentOptionName = videoOptions[optionIndex].name;
        StaticVariableManager.currentVideoOption = currentOptionName;



        int index = 0;
        foreach (Sprite image in videoOptions)
        {

            if (image.name.Contains(currentOptionName))
            {
                img.sprite = videoOptions[index];

                // Extract ID before the first space (e.g., G2, G22, G202)
                string idPart = currentOptionName.Split(' ')[0];

                StaticVariableManager.currentVideoGroup = idPart;
                // print("Current group: " + StaticVariableManager.currentVideoGroup);
                if (transitionGroups.Contains(StaticVariableManager.currentVideoGroup))
                {
                    StaticVariableManager.isTransitionVideo = true;
                }
                else
                {
                    StaticVariableManager.isTransitionVideo = false;
                }

                // Extract text inside parentheses
                int trainingNameStartPt = currentOptionName.IndexOf("(") + 1;
                int trainingNameEndpt = currentOptionName.IndexOf(")");

                string trainingName = currentOptionName;
                if (trainingNameStartPt > 0 && trainingNameEndpt > trainingNameStartPt)
                {
                    trainingName = currentOptionName.Substring(trainingNameStartPt, trainingNameEndpt - trainingNameStartPt);
                }

                // Example: show both separately or together
                if(trainingName == "Outdoor Car")
                {
                    currentOptionText.text = $"Car robbery";
                }
                else if (trainingName == "House Robbery")
                {
                    currentOptionText.text = $"House breaking";
                }
                else if (trainingName == "Outdoor Woods")
                {
                    currentOptionText.text = $"Woods chasing";
                }
                else
                {
                    currentOptionText.text = $"{trainingName}";
                }
                DropDown.ompSceneName = "Video_" + currentOptionText.text;
            }


            index++;
        }

        // print("Option: " + StaticVariableManager.currentVideoOption);
        // print("Group: " + StaticVariableManager.currentVideoGroup);
    }

    private void SetStartOption()
    {
        StaticVariableManager.isTransitionVideo = false;
        StaticVariableManager.currentVideoGroup = "";
        UpdateImage(imageObject);
    }

    public void NextVideoOption()
    {
        StaticVariableManager.currentVideoOption = "";
        if ( optionIndex < videoOptions.Length - 1 ) optionIndex++;
        UpdateImage(imageObject);
    }

    public void PrevVideoOption()
    {
        StaticVariableManager.currentVideoOption = "";
        if (optionIndex > 0) optionIndex-- ;
        UpdateImage(imageObject);
    }



    public void Exit()
    {
        SceneManager.LoadScene("SceneManager");
    }
    public void Continue()
    {
        // ManageNumShotsSettings();
        StaticVariableManager.previousIndex = 0;

        if(StaticVariableManager.isTransitionVideo)
        {
            DropDown.softwareSceneName = "Basic1LaneVideoTransitionTraining";
        }
        else
        {
            DropDown.softwareSceneName = "Basic1LaneVideoTraining";
        }

        SceneManager.LoadScene("TestConditionSetting");
    }
}
