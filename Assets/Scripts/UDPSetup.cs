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
using System.IO;
using System;

public class UDPSetup : MonoBehaviour
{
    public float UDPSendTimer = 0.01f;
    float UDPSetTime = 0.01f;
    public bool sendUDPCamStatus = false;
    public bool sendUDPGunType = false;
    public bool sendUdpReset = false;

    //UDP Communiocation
    UdpClient udpClient;
    UdpClient udpClientS;
    public int portnum = 22222;
    IPEndPoint remoteEndPoint;
    private static string IP;
    public int Sportnum;
    public static string UDP_ClientIP_Address = "127.0.0.1";

    //IP Address Variables
    static string LocalIP = " ";
    static string FinalM_LocalIP = "";
    static bool ipFound = false;
    string activeScene = "";

    void Start()
    {
        sendUDPCamStatus = true;
        ipFound = false;
        activeScene = SceneManager.GetActiveScene().name;

        //init();
        //sendEndless("Stop");
        CreateFile();
        FindLocalIP();

        // sendEndless("stop");
        // if(activeScene == "MainMenu") SendAmmoSetting(); // Signal for the raspberry pi to align with current settings.

        if (login_Manager.rebootCamera)
        {
            sendEndless("reboot");
            login_Manager.global_Camera_active_timer = login_Manager.globalCAmeraTimerSet;
            login_Manager.rebootCamera = false;
        }
    }
    private void Update()
    {
        
        if (sendUDPCamStatus)
        {
            UDPSendTimer -= Time.deltaTime * 1;
            if (UDPSendTimer <= 0f)
            {
                sendUDPCamStatus = false;
                sendUDPGunType = true;
                UDPSendTimer = UDPSetTime;
            }
            //sendEndless("Calibration");
            //sendEndless("Laser");
        }
        else if (sendUDPGunType)
        {
            UDPSendTimer -= Time.deltaTime * 1;
            if (UDPSendTimer <= 0f)
            {
                sendUDPGunType = false;
                sendUdpReset = true;
                UDPSendTimer = UDPSetTime;
            }
            //sendEndless("Laser");
        } //sendEndless("Reset");
        else if (sendUdpReset)
        {
            UDPSendTimer -= Time.deltaTime * 1;
            if (UDPSendTimer <= 0f)
            {
                sendUdpReset = false;
                UDPSendTimer = UDPSetTime;
            }
            //sendEndless("Reset");
        }
    }

    private void SendAmmoSetting()
    {
        if (Scoring.ammo_setting.ToLower().Contains("live"))
        {
            sendEndless("Live");
        }
        else if (Scoring.ammo_setting.ToLower().Contains("laser"))
        {
            sendEndless("Laser");
        }
    }

    public void sendEndless(string testStr)
    {
        udpClientS = new UdpClient();
        //IP = "192.168.8.255";
        IP = UDP_ClientIP_Address;
        Sportnum = 55552;
        // print($"IP is: {UDP_ClientIP_Address}");
        // print($"Sportnum is: {Sportnum}");
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), Sportnum);

        // Daten mit der UTF8-Kodierung in das Bin�rformat kodieren.
        byte[] data = Encoding.UTF8.GetBytes(testStr);
        // print("sending " + testStr + " to " + remoteEndPoint);
        // Den message zum Remote-Client senden.

        int numberOfPackets = 5;
        // Send a packet multiple times
        for (int i = 0; i < numberOfPackets; i++)
        {
            udpClientS.Send(data, data.Length, remoteEndPoint);
        }
    }
    public void CreateFile()
    {
        //FileManager.CreateFile(UDP_IP_SavePath);
    }

    //Local IP Finder
    public static void FindLocalIP()
    {
        IPHostEntry host;                           //create Host
        host = Dns.GetHostEntry(Dns.GetHostName()); //Get current DNS Server
        FinalM_LocalIP = "";
        ipFound = false;

        foreach (IPAddress ip in host.AddressList)   //Read all IP's in the Current DNS and Load the IPv4
        {
            if (ipFound == false)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    LocalIP = ip.ToString();
                    ManipulateIP(LocalIP);                     //Function for correcting the IP for UDP broadcasting
                    // print("RE: The Final IP is: " + FinalM_LocalIP);
                    
                    FileManager.SaveSetting("UDP IP", FinalM_LocalIP);

                    ipFound = true;
                }
            }
        }

        UDP_ClientIP_Address = FinalM_LocalIP;
    }
    private static void ManipulateIP(string ip)
    {
        //LocalIP

        int x = 3;
        while (x >= 1)
        {
            FinalM_LocalIP += ip.Substring(0, ip.IndexOf('.') + 1); // Load Current value before pound '.' to the new IP variable
            ip = ip.Remove(0, ip.IndexOf('.') + 1);            //Delete Current Value before pount '.'

            if (x == 1)
            {
                FinalM_LocalIP += "255";
            }
            x--;
        }
    }

}
