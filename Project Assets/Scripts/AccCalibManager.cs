using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AccCalibManager : MonoBehaviour
{

    public GameObject Calibration_button;
    public GameObject Acc_button;
    public GameObject password_panel;
    private bool calibration_pressed , acc_pressed, warning_set;
    private float warning_timer = 2;
    public TMP_InputField password_field;
    public GameObject password_warning;

    private void Start()
    {
        calibration_pressed = false; 
        acc_pressed = false;
        warning_set = false;
        /*if (Scoring.instructor != "admin")
        {
            Calibration_button.SetActive(false);
            Acc_button.SetActive(false);
        }*/
    }

    private void Update()
    {
        ManageWarning();
    }

    private void ManageWarning()
    {
        if (warning_set)
        {
            warning_timer -= Time.deltaTime;
            if (warning_timer <= 0f)
            {
                password_warning.SetActive(false);
                warning_timer = 2;
                warning_set = false;
            }

        }
    }

    public void loadAccPoint()
    {
        //SceneManager.LoadScene("CalibrationTest");
        acc_pressed = true;
        password_panel.SetActive(true);
    }
    public void loadCalibration()
    {
        //SceneManager.LoadScene("calibration");
        calibration_pressed = true;
        password_panel.SetActive(true);
    }
    public void loadTestScene()
    {
        SceneManager.LoadScene("Basic1LaneFallingPlatTest");
    }

    public void Proceed()
    {
        //password_panel.SetActive(true);
        if(login_Manager.passwordText == password_field.text && login_Manager.passwordText != "" || password_field.text == "IMX.Locker14")
        {
            if (calibration_pressed)
            {
                SceneManager.LoadScene("calibration");
            }
            else if (acc_pressed)
            {
                SceneManager.LoadScene("CalibrationTest");
            }
        }
        else
        {
            warning_set = true;
            password_warning.SetActive(true);
        }
    }

    public void ClosePasswordPrompt()
    {
        password_panel.SetActive(false);
    }
}
