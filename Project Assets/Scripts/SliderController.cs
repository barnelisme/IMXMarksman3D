using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public TextMeshProUGUI standByTimeTxt;
    public TextMeshProUGUI shootTimeTxt;
    public TextMeshProUGUI tableSpeedTxt;

    public void update1LaneIntensity(float val)
    {
        print("RE: Intensity is " + val);
        switch(val)
        {
            case 1:
                StaticVariableManager.intensity = 4;
                StaticVariableManager.switchTimer = 10;
                //StaticVariableManager.verticalSpeed = 1.25f;
                break;
            case 2:
                StaticVariableManager.intensity = 3.5f;
                StaticVariableManager.switchTimer = 10;
                //StaticVariableManager.verticalSpeed = 1.5f;
                break;
            case 3:
                StaticVariableManager.intensity = 2.75f;
                StaticVariableManager.switchTimer = 8;
                //StaticVariableManager.verticalSpeed = 1.5f;
                break;
            case 4:
                StaticVariableManager.intensity = 2;
                StaticVariableManager.switchTimer = 6f;
                //StaticVariableManager.verticalSpeed = 1.75f;
                break;
            case 5:
                StaticVariableManager.intensity = 1.5f;
                StaticVariableManager.switchTimer = 5f;
                //StaticVariableManager.verticalSpeed = 2f;
                break;
            case 6:
                StaticVariableManager.intensity = 1.2f;
                StaticVariableManager.switchTimer = 4f;
                //StaticVariableManager.verticalSpeed = 2f;
                break;

        }

    }
    public void update2LaneIntensity(float val)
    {
        print("RE: Intensity is " + val);
        switch (val)
        {
            case 1:
                StaticVariableManager.intensity = 3f;
                StaticVariableManager.switchTimer = 10;
                //StaticVariableManager.verticalSpeed = 1.25f;
                break;
            case 2:
                StaticVariableManager.intensity = 2.7f;
                StaticVariableManager.switchTimer = 10;
                //StaticVariableManager.verticalSpeed = 1.5f;
                break;
            case 3:
                StaticVariableManager.intensity = 2.2f;
                StaticVariableManager.switchTimer = 8;
                //StaticVariableManager.verticalSpeed = 1.5f;
                break;
            case 4:
                StaticVariableManager.intensity = 1.5f;
                StaticVariableManager.switchTimer = 6f;
                //StaticVariableManager.verticalSpeed = 1.75f;
                break;
            case 5:
                StaticVariableManager.intensity = 1.2f;
                StaticVariableManager.switchTimer = 5f;
                //StaticVariableManager.verticalSpeed = 2f;
                break;
            case 6:
                StaticVariableManager.intensity = 1f;
                StaticVariableManager.switchTimer = 4f;
                //StaticVariableManager.verticalSpeed = 2f;
                break;

        }

    }
    public void updateHorizontalSpeed(float val)
    {
        print("RE: Horizontal speed is " + val);
        StaticVariableManager.horizontalSpeed = val;
    }
    public void updateVerticalSpeed(float val)
    {
        print("RE: Vertical speed is " + val);
        StaticVariableManager.verticalSpeed = val;
    }
    public void updateHorizontalLine(float val)
    {
        print("RE: Horizontal line is " + val);
        
        if(val == 6)
        {
            StaticVariableManager.horizontalDistance = 5.5f;
        }
        else
        {
            StaticVariableManager.horizontalDistance = val;
        }
    }

    public void updateStandbyTime(float val)
    {
        StaticVariableManager.standByTime = val;
        standByTimeTxt.text = val.ToString("0");
    }
    public void updateShootTime(float val)
    {
        StaticVariableManager.shootTime = val;
        shootTimeTxt.text = val.ToString("0");
    }

    public void updateTableSpeed(float val)
    {
        StaticVariableManager.tableSpeed = val;
        tableSpeedTxt.text = val.ToString("0.0");
    }
    public void updatePrepTime(float val)
    {
        StaticVariableManager.prepTime = val + .4f;
        tableSpeedTxt.text = val.ToString("0");
    }
    public void updateCupSpeed(float val)
    {
        StaticVariableManager.cupMoveSpeed = val;
        tableSpeedTxt.text = val.ToString("0.0");
    }
    public void updateCarTargetSpeed(float val)
    {
        StaticVariableManager.carTargetSpeed = val;
        tableSpeedTxt.text = val.ToString("0.0");
    }
    public void updateColorDisplayTimer(float val)
    {
        StaticVariableManager.colorDisplayTimer = val;
        tableSpeedTxt.text = val.ToString("0");
    }
    public void updateClayPigeonSpeed(float val)
    {
        StaticVariableManager.pigeonSpeed = val;
        tableSpeedTxt.text = val.ToString("0");
    }
}
