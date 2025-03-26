using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

public class CameraManager : MonoBehaviour
{

    string UDP_IP_SavePath = "AddrDat00.txt";
    public float UDPSendTimer = 0.25f;
    public float UDPWaitSendTimer = 5f;
    bool startAutoCamSelect = false;
    float UDPSetTime = 0.5f;
    public bool sendUDP = false;
    bool sendConfigUDP = false;

    //UDP Communiocation
    UdpClient udpClient;
    UdpClient udpClientS;
    public int portnum = 22222;
    IPEndPoint remoteEndPoint;
    private string IP;
    public int Sportnum;
    public static string UDP_ClientIP_Address;

    //CRM Communication Components
    public GameObject cameraPanel;

    //Camera modules
    static bool isBuiltInCam = false;
    static bool isExtCam = false;
    public GameObject cameraImage;
    static int cameraCount = 0;
    static string defaultCam = "none";
    static bool camera_Cheched = false;
    public bool default_cam_available = false;

    List<string> camDevices = new List<string>();
    static int selectedCam = 0;
    List<TextMeshProUGUI> camTextMP;
    public TextMeshProUGUI cam_txt_name_1;
    public TextMeshProUGUI cam_txt_name_2;
    public TextMeshProUGUI cam_txt_name_3;
    public TextMeshProUGUI cam_txt_name_4;
    public GameObject camButton_1;
    public GameObject camButton_2;
    public GameObject camButton_3;
    public GameObject camButton_4;
    public GameObject noCamMessage;
    int total_cameras = 0;
    bool cameras_set = false;
    private bool cameraOpened = false;

    void Start()
    {
        //cameraPanel.SetActive(true);
        camera_Cheched = false;
        CreateFile();
        ReadFile();
        sendConfigUDP = false;
        sendUDP = false;
        //checkDefaultCamera();
      
    }

    // Update is called once per frame
    void Update()
    {
        if (sendUDP)
        {

            UDPSendTimer -= Time.deltaTime * 1;
            if (UDPSendTimer <= 0f)
            {
                sendUDP = false;
            }

            if (sendConfigUDP)
            {
                sendEndless("Calibration");
            }
            if (isBuiltInCam)
            {
                sendEndless("Camera 0");
            }
            if (isExtCam)
            {
                sendEndless("Camera 1");
            }


        }
        else
        {
            UDPSendTimer = UDPSetTime;
        }

        //Rotate Image
        cameraImage.transform.Rotate(new Vector3(0, 45, 0) * Time.deltaTime * 2);
        //checkDefaultCamera();
        if(startAutoCamSelect) //start_secondary_udpsend_process
        {
            UDPWaitSendTimer -= Time.deltaTime * 1;

            if (UDPWaitSendTimer <= 0f)
            {
                if (selectedCam == 1)
                {
                    camButton1();
                }
                if (selectedCam == 2)
                {
                    camButton2();
                }
                if (selectedCam == 3)
                {
                    camButton3();
                }
                if (selectedCam == 4)
                {
                    camButton4();
                }
                UDPWaitSendTimer = 5;
                startAutoCamSelect = false;
            }
        }
        checkDefaultCamera();
    }
    private void manageCamButtons()
    {
        camButton_1.SetActive(false);
        camButton_2.SetActive(false);
        camButton_3.SetActive(false);
        camButton_4.SetActive(false);
        noCamMessage.SetActive(false);
    }
    private void checkDefaultCamera()
    {
        checkExternalCamera();
        if (StaticVariableManager.openCameraOpt)
        {
            //print("Point Reached");
            //Manage Cameras
            if (!camera_Cheched)
            {
                cameraPanel.SetActive(true);
            }
            else
            {
                cameraPanel.SetActive(false);
                startAutoCamSelect = true;
            }
            //camera_Cheched = false;
            StaticVariableManager.openCameraOpt = false;
        }
    }
    private void checkExternalCamera()
    {
        //cameraCount = WebCamTexture.devices.Length;
        //print("Number of connected cameras: " + cameraCount);

        try
        { 
            WebCamDevice[] devices = WebCamTexture.devices;
            int cam_length = devices.Length;

            if (cam_length > 0)
            {
                //print("Total number of webcams: " + devices.Length);
                if (cameras_set == false)
                {
                    manageCamButtons(); //reset buttons
                    camDevices.Clear(); //reset camera list
                    for (int i = 0; i < cam_length; i++)
                    {
                        camDevices.Add(i + "," + devices[i].name);
    
                        if (i == 0)
                        {
                            cam_txt_name_1.text = devices[i].name;
                            camButton_1.SetActive(true);
                        }
                        if (i == 1)
                        {
                            cam_txt_name_2.text = devices[i].name;
                            camButton_2.SetActive(true);
                        }
                        if (i == 2)
                        {
                            cam_txt_name_3.text = devices[i].name;
                            camButton_3.SetActive(true);
                        }
                        if (i == 3)
                        {
                            cam_txt_name_4.text = devices[i].name;
                            camButton_4.SetActive(true);
                        }
                    } // End of loop
    
                    cameras_set = true;
                    total_cameras = cam_length;
                }
                else
                {
                    if(cam_length > total_cameras || cam_length < total_cameras)
                    {
                        cameras_set = false;
                    }
                }
            }
            else
            {
                //UnityEngine.Debug.LogError("No webcam devices found.");
                manageCamButtons(); //reset buttons
                noCamMessage.SetActive(true);
            }

        }
        catch(Exception e)
        {
            // pass
            //Debug.Log("Error when reseting range for text display:" + e.Message);
        }

        //print("RE: Web cams are " + WebCamTexture.devices);
    }

    private void sendEndless(string testStr)
    {
        udpClientS = new UdpClient();

        ReadFile();
        IP = UDP_ClientIP_Address;
        Sportnum = 55552;

        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), Sportnum);

        // Daten mit der UTF8-Kodierung in das Bin�rformat kodieren.
        byte[] data = Encoding.UTF8.GetBytes(testStr);
        print("sending " + testStr + " to " + remoteEndPoint);
        // Den message zum Remote-Client senden.
        udpClientS.Send(data, data.Length, remoteEndPoint);
    }

    public void CreateFile()
    {

        FileManager.CreateFile(UDP_IP_SavePath);
    }
    private void ReadFile()
    {
        //UDP_ClientIP_Address = reader.ReadLine();
        try
        {
            string base64 = FileManager.ReadFromFile(UDP_IP_SavePath);
            Encryption encrypt = new Encryption();
            UDP_ClientIP_Address = encrypt.AESDecryption(base64);                    //decryption code
        }
        catch (Exception e)
        {
            print("Error retrieving zero file:" + e);
            throw new Exception("First login");
        }


    }
    public void onClickBuiltInCamera()
    {
        isBuiltInCam = true;
        sendUDP = true;
        cameraPanel.SetActive(false);
        enableAutoSelector();
    }
    public void onClickExtCamera()
    {
        isExtCam = true;
        sendUDP = true;
        cameraPanel.SetActive(false);
        enableAutoSelector();
    }
    public void camButton1()
    {
        sendEndless(camDevices[0].ToString());
        cameraPanel.SetActive(false);
        enableAutoSelector();
        selectedCam = 1;
    }
    public void camButton2()
    {
        sendEndless(camDevices[1].ToString());
        cameraPanel.SetActive(false);
        enableAutoSelector();
        selectedCam = 2;
    }
    public void camButton3()
    {
        sendEndless(camDevices[2].ToString());
        cameraPanel.SetActive(false);
        enableAutoSelector();
        selectedCam = 3;
    }
    public void camButton4()
    {
        sendEndless(camDevices[3].ToString());
        cameraPanel.SetActive(false);
        enableAutoSelector();
        selectedCam = 4;
    }
    private void enableAutoSelector()
    {
        camera_Cheched = false; //deactivate auto cam selector
    }
}

/*if (cameraCount <= 1 )
{
    cameraPanel.SetActive(true);
}
else if(cameraCount== 2)
{
    onClickExtCamera();
}
else
{
    cameraPanel.SetActive(true);
}*/