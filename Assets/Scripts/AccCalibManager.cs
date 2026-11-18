using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

public class AccCalibManager : MonoBehaviour
{
    UdpClient udpClient;
    int portnum = 22222;

    public GameObject Calibration_button;
    public GameObject Acc_button;
    public GameObject password_panel;
    private bool calibration_pressed , acc_pressed, warning_set;
    private float warning_timer = 2;
    public TMP_InputField password_field;
    public GameObject password_warning;
    private string activeScene = "";

    // Communications Variables
    public GameObject filterMessageHolder;
    public GameObject cameraImage;
    public TextMeshProUGUI filter_message;
    public TextMeshProUGUI cameraIPText;
    private bool isFilterMessageSet = false;

    private void Start()
    {
        udpClient = new UdpClient(portnum);
        calibration_pressed = false; 
        acc_pressed = false;
        warning_set = false;
        activeScene = SceneManager.GetActiveScene().name;

        cameraIPText.text = "Cam Offline";

        ManageFilterMessage();
    }

    private void Update()
    {
        ManageWarning();
        ManageUdpMessages();

    }

    private void ManageFilterMessage()
    {
        cameraImage.transform.Rotate(new Vector3(0, 45, 0) * Time.deltaTime * 2);

        isFilterMessageSet = false;
        if (!isFilterMessageSet)
        {
            if (StaticVariableManager.isCalibrationComplete)
            {
                filter_message.text = "" +
                    //"<color=#880000>NOTE:</color>\n\n" +
                    "<color=#004d00>COMPLETE</color>: \n\n" +
                    "Please <color=#880000>CLOSE the filter cap</color> on the camera before starting the training.";

                StaticVariableManager.isCalibrationComplete = false;
            }
            else
            {
                filter_message.text = "" +
                    "<color=#880000>NOTE:</color>\n\n" +
                    "Before calibrating, please ensure that you <color=#880000>OPEN the filter cap</color> on the camera.";
            }


            filterMessageHolder.SetActive(true);
            isFilterMessageSet = true;
        }

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

    private void ManageUdpMessages()
    {
        try
        {
            IPEndPoint remoteEP = null;
            if (udpClient.Available > 0)
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string message = Encoding.ASCII.GetString(data);
                
                // Debug.Log(message + " from " + remoteEP.Address.ToString());

                if(message.ToLower().Contains("recalibrate"))
                {
                    SceneManager.LoadScene("calibration");
                }
                else if(message.ToLower().Contains("test calibration"))
                {
                    SceneManager.LoadScene("CalibrationTest");
                }
                else if (message.ToLower().Contains("exit test calibration"))
                {
                    SceneManager.LoadScene("Calib_Acc_Page");
                }
                else if(message.Contains("IP:"))
                {
                    //print(message);
                    if (cameraIPText != null) cameraIPText.text = "Cam " + message;
                }

            }
        }
        catch (Exception e)
        {
            Scoring.logs += "\n" + e.Message + ":" + e.StackTrace;
            Debug.Log("Exception:" + e.StackTrace + " " + e.Message);
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
        StaticVariableManager.isCalibrationComplete = true ;
        SceneManager.LoadScene("calibration");
        //calibration_pressed = true;
        //password_panel.SetActive(true);
    }
    public void loadTestScene()
    {
        SceneManager.LoadScene("Basic1LaneFallingPlatTest");
    }

    public void Proceed()
    {
        //password_panel.SetActive(true);
        if(login_Manager.passwordText == password_field.text && login_Manager.passwordText != "" || password_field.text == "Imagine8")
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

    public void CloseFilterMessage()
    {
        filterMessageHolder.SetActive(false);
    }
}
