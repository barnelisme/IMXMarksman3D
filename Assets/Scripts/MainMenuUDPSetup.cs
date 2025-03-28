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

public class MainMenuUDPSetup : MonoBehaviour
{
    static string UDP_IP_SavePath = "AddrDat00.txt";
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
    public static string UDP_ClientIP_Address;

    //IP Address Variables
    static string LocalIP = " ";
    static string FinalM_LocalIP = "";
    static bool ipFound = false;


    void Start()
    {
        sendUDPCamStatus = true;
        ipFound = false;

        //init();
        //sendEndless("Stop");
        CreateFile();
        FindLocalIP();
        ReadFile();
        sendEndless("stop");
        //sendEndless("Simulate");
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

    void init()
    {
        udpClient = new UdpClient(portnum);
        remoteEndPoint = null;
        udpClientS = new UdpClient();
        IP = "192.168.0.118";
        Sportnum = 55555;
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), Sportnum);

    }
    private void sendString(string message)
    {
        if (udpClientS.Available > 0)
        {
            // Daten mit der UTF8-Kodierung in das Bin�rformat kodieren.
            byte[] data = Encoding.UTF8.GetBytes(message);
            // Den message zum Remote-Client senden.
            udpClientS.Send(data, data.Length, remoteEndPoint);
        }
        else
        {
            print("error in connection");
        }
    }
    private void sendEndless(string testStr)
    {
        udpClientS = new UdpClient();
        //IP = "192.168.8.255";
        IP = UDP_ClientIP_Address;
        Sportnum = 55552;
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), Sportnum);

        // Daten mit der UTF8-Kodierung in das Bin�rformat kodieren.
        byte[] data = Encoding.UTF8.GetBytes(testStr);
        //print("sending " + testStr + " to " + remoteEndPoint);
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
                    //print("RE: The Final IP is: " + FinalM_LocalIP);
                    WriteFile(FinalM_LocalIP);

                    ipFound = true;
                }
            }
        }

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
    private static  void WriteFile(string val)
    {
        //print("Encrypting....");
        Encryption encrypt = new Encryption();
        string base64 = encrypt.AESEncryption(val);//encrypt username and password
        //print("IP File Encrypted:" + base64);
        FileManager.CreateFile(UDP_IP_SavePath);
        FileManager.WriteDataToFile(UDP_IP_SavePath, base64);
    }
}
