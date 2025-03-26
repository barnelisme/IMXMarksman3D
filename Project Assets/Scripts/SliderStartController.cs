using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SliderStartController : MonoBehaviour
{
    [SerializeField] private Slider intensitySlider;
    [SerializeField] private Slider verticalSpeedSlider;
    [SerializeField] private Slider horizontalSpeedSlider;
    [SerializeField] private Slider horizontalLineSlider;
    [SerializeField] private Slider standbyTimeSlider;
    [SerializeField] private Slider shootTimeSlider;
    [SerializeField] private Slider tableSpeed;

    public TextMeshProUGUI standByTimeTxt;
    public TextMeshProUGUI shootTimeTxt;
    public TextMeshProUGUI tableSpeedTxt;

    public GameObject controllerPanel;
    string activeScene = " ";

    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;

        if(activeScene.ToLower().Contains("suspectshoot"))
        {
            standbyTimeSlider.value = StaticVariableManager.standByTime;
            shootTimeSlider.value = StaticVariableManager.shootTime;
            standByTimeTxt.text = StaticVariableManager.standByTime.ToString("0");
            shootTimeTxt.text = StaticVariableManager.shootTime.ToString("0");
            //print("RRE: Suspect time..:" + StaticVariableManager.shootTime);
        }
        else if(activeScene.ToLower().Contains("hidden"))
        {
            tableSpeed.value = StaticVariableManager.tableSpeed;
            tableSpeedTxt.text = StaticVariableManager.tableSpeed.ToString("0.0");
        }
        else if (activeScene.ToLower().Contains("rifflepole"))
        {
            tableSpeed.value = StaticVariableManager.tableSpeed;
            //tableSpeedTxt.text = StaticVariableManager.tableSpeed.ToString("0.0");
        }
        else if (activeScene.ToLower().Contains("shell"))
        {
            tableSpeed.value = StaticVariableManager.cupMoveSpeed;
            tableSpeedTxt.text = StaticVariableManager.cupMoveSpeed.ToString("0.0");
        }
        else if (activeScene.ToLower().Contains("cargame"))
        {
            tableSpeed.value = StaticVariableManager.carTargetSpeed;
            tableSpeedTxt.text = StaticVariableManager.carTargetSpeed.ToString("0.0");
        }
        else if (activeScene.ToLower().Contains("colorsequence"))
        {
            tableSpeed.value = StaticVariableManager.colorDisplayTimer;
            tableSpeedTxt.text = StaticVariableManager.colorDisplayTimer.ToString("0");
        }
        else if (activeScene.ToLower().Contains("claypigeon") || activeScene.ToLower().Contains("shapeplat"))
        {
            tableSpeed.value = StaticVariableManager.pigeonSpeed;
            tableSpeedTxt.text = StaticVariableManager.pigeonSpeed.ToString("0");
        }
        else if (activeScene.ToLower().Contains("animaltarget"))
        {
            tableSpeed.value = StaticVariableManager.prepTime;
            tableSpeedTxt.text = StaticVariableManager.prepTime.ToString("0");
        }
        else
        {

            if (activeScene.ToLower().Contains("1lane"))
            {
                switch (StaticVariableManager.intensity)
                {
                    case 4:
                        intensitySlider.value = 1;
                        break;
                    case 3.5f:
                        intensitySlider.value = 2;
                        break;
                    case 2.75f:
                        intensitySlider.value = 3;
                        break;
                    case 2:
                        intensitySlider.value = 4;
                        break;
                    case 1.5f:
                        intensitySlider.value = 5;
                        break;
                    case 1.2f:
                        intensitySlider.value = 6;
                        break;
                }
            }
            if (activeScene.ToLower().Contains("2lane"))
            {
                switch (StaticVariableManager.intensity)
                {
                    case 3f:
                        intensitySlider.value = 1;
                        break;
                    case 2.7f:
                        intensitySlider.value = 2;
                        break;
                    case 2.2f:
                        intensitySlider.value = 3;
                        break;
                    case 1.5f:
                        intensitySlider.value = 4;
                        break;
                    case 1.2f:
                        intensitySlider.value = 5;
                        break;
                    case 1f:
                        intensitySlider.value = 6;
                        break;
                }
            }

            if (StaticVariableManager.horizontalDistance == 5.5f)
            {
                horizontalLineSlider.value = 6;
            }
            else
            {
                horizontalLineSlider.value = StaticVariableManager.horizontalDistance;
            }

            verticalSpeedSlider.value = StaticVariableManager.verticalSpeed;
            horizontalSpeedSlider.value = StaticVariableManager.horizontalSpeed;
            controllerPanel.SetActive(true);
        }

    }
    private void Update()
    {
        if(StaticVariableManager.isTrainingPause)
        {
            controllerPanel.SetActive(false);
        }
    }

}
