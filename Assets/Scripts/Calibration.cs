using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Calibration : MonoBehaviour
{
    // Start is called before the first frame update
    string calibrationPath = "Assets/Resources/Calibration.txt";
    public string[] calibrationPoints;
    string activeScene;

    //Manage Input
    public Boolean flagCalibrator = false;

    public UdpClient udpClient;

    [Header("Acc Varibales")]
    //SCENE: Accurate point Calibration
    public static bool isAccMode = false;
    public static bool isAccSet = false;
    public static float y_CentrePos = 0;
    public static float x_CentrePos = 0;

    //-lowest point
    public static float lowPoint_yPos = 0f;
    public static float low_YErrorPoint = 0;       //lowest y error point
    //-highest point
    public static float highPoint_yPos = 0f;
    public static float high_point_yError = 0;     //highest y error point
    //-far left point
    public static float upLeftPoint_xPos = 0f;
    public static float upLeft_point_xError = 0;
    public static float lowerLeftPoint_xPos = 0f;
    public static float lowerLeft_point_xError = 0;
    //-far right point
    public static float upRightPoint_xPos = 0f;
    public static float upRight_point_xError = 0;
    public static float lowerRightPoint_xPos = 0f;
    public static float lowerRight_point_xError = 0;

    string AccSavePath = "AccDatInc.txt";
    string AccSaveData = " ";

    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        IS_FileManager();
        isAccSet = false;

        if (!isAccSet)
        {
            LoadAccFromFile();

            if (AccSaveData.Contains("0:0:0:0:0:0:0:0") || AccSaveData == null)
            {
                isAccMode = false;
                print("Acc file is empty");
            }
            else
            {
                //AccSaveData = lowPoint_yPos + ":" + low_YErrorPoint + ":" + highPoint_yPos + ":" + high_point_yError + ":" + leftPoint_xPos + ":" + left_point_xError + ":" + rightPoint_xPos + ":" + right_point_xError + ":"+ y_CentrePos+ ":"+ x_CentrePos;
                //AccSaveData = lowPoint_yPos + ":" + low_YErrorPoint + ":" + highPoint_yPos + ":" + high_point_yError + ":" + upLeftPoint_xPos + ":" + upLeft_point_xError + ":" + upRightPoint_xPos + ":" + upRight_point_xError + ":" + lowerLeftPoint_xPos + ":" + lowerLeft_point_xError + ":" + lowerRightPoint_xPos + ":" + lowerRight_point_xError + ":" + y_CentrePos + ":" + x_CentrePos;

                lowPoint_yPos = float.Parse(AccSaveData.Split(':')[0]);
                low_YErrorPoint = float.Parse(AccSaveData.Split(':')[1]);
                highPoint_yPos = float.Parse(AccSaveData.Split(':')[2]);
                high_point_yError = float.Parse(AccSaveData.Split(':')[3]);
                upLeftPoint_xPos = float.Parse(AccSaveData.Split(':')[4]);
                upLeft_point_xError = float.Parse(AccSaveData.Split(':')[5]);
                upRightPoint_xPos = float.Parse(AccSaveData.Split(':')[6]);
                upRight_point_xError = float.Parse(AccSaveData.Split(':')[7]);
                lowerLeftPoint_xPos = float.Parse(AccSaveData.Split(':')[8]);
                lowerLeft_point_xError = float.Parse(AccSaveData.Split(':')[9]);
                lowerRightPoint_xPos = float.Parse(AccSaveData.Split(':')[10]);
                lowerRight_point_xError = float.Parse(AccSaveData.Split(':')[11]);
                y_CentrePos = float.Parse(AccSaveData.Split(':')[12]);
                x_CentrePos = float.Parse(AccSaveData.Split(':')[13]);

                //print("Acc Values Loaded");
                if(Scoring.ammo_setting.ToLower().Contains("live"))
                {
                    isAccMode = true;
                }
            }

            isAccSet = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        //ManageInput();
    }
    private void ManageInput()
    {

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.F5))
        {
            Debug.Log("**Current calibration:" + flagCalibrator);
            Debug.Log("**Toggling calibration mode**");
            flagCalibrator = true;
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.F4))
        {
            Debug.Log("**Current calibration:" + flagCalibrator);
            Debug.Log("**Toggling calibration mode**");
            flagCalibrator = false;
        }
    }
    private void IS_FileManager()
    {
        if (!Directory.Exists("Assets/Resources"))
        {
            Directory.CreateDirectory("Assets/Resources");
            File.Create("Assets/Resources/Calibration.txt");
            Debug.Log("Resources created and calibration needs to be done");

        }
        else
        {
            //Debug.Log("Resources Already exists");
            if (!File.Exists(calibrationPath))
            {
                File.Create(calibrationPath);
            }
            else
            {
                //Debug.Log("Calibration Found");
                calibrationPoints = System.IO.File.ReadAllLines(calibrationPath);
                foreach (string line in calibrationPoints)
                {
                    //Debug.Log(line);
                }
            }
        }
    }

    public void LoadAccFromFile()
    {

        string base64 = FileManager.ReadFromFile(AccSavePath);
        Encryption encrypt = new Encryption();
        AccSaveData = encrypt.AESDecryption(base64);                    //decryption code                  //decryption code
        //print("Acc Data is " + AccSaveData);

    }

}
