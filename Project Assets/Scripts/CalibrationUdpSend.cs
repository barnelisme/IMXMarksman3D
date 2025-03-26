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

public class CalibrationUdpSend : MonoBehaviour
{
    string UDP_IP_SavePath = "AddrDat00.txt";
    public float UDPSendTimer = 0.5f;
    float UDPSetTime = 0.5f;
    public bool sendUDP = true;

    //UDP Communiocation
    UdpClient udpClient;
    UdpClient udpClientS;
    public int portnum = 22222;
    IPEndPoint remoteEndPoint;
    private static string IP;
    public int Sportnum;
    public static string UDP_ClientIP_Address;

    //IP Address Variables
    string LocalIP = " ";
    string FinalM_LocalIP = "";


    void Start()
    {
        //init();
        //sendEndless("Stop");
        CreateFile();
        ReadFile();
        FindLocalIP();
        sendUDP = true;
        sendEndless("Calibrate");
    }

    private void Update()
    {
        //sendEndless("Calibration");
        if (sendUDP)
        {
            UDPSendTimer -= Time.deltaTime * 1;
            if (UDPSendTimer <= 0f)
            {
                sendUDP = false;
            }
            //sendEndless("Calibrate");
        }
        else
        {
            UDPSendTimer = UDPSetTime;
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

    //Local IP Finder
    private void FindLocalIP()
    {
        IPHostEntry host;                           //create Host
        host = Dns.GetHostEntry(Dns.GetHostName()); //Get current DNS Server

        foreach (IPAddress ip in host.AddressList)   //Read all IP's in the Current DNS and Load the IPv4
        {
            if (ip.AddressFamily.ToString() == "InterNetwork")
            {
                LocalIP = ip.ToString();

                if (LocalIP != null)
                {
                    ManipulateIP();                     //Function for correcting the IP for UDP broadcasting
                }

            }

        }

    }
    private void ManipulateIP()
    {
        //LocalIP

        int x = 3;
        while (x >= 1)
        {
            FinalM_LocalIP += LocalIP.Substring(0, LocalIP.IndexOf('.') + 1); // Load Current value before pound '.' to the new IP variable
            LocalIP = LocalIP.Remove(0, LocalIP.IndexOf('.') + 1);            //Delete Current Value before pount '.'

            if (x == 1)
            {
                FinalM_LocalIP += "255";
            }
            x--;
        }

        if ((FinalM_LocalIP != UDP_ClientIP_Address))
        {
            WriteFile(FinalM_LocalIP);
            print("The Current Broadcast IP is " + FinalM_LocalIP);
        }

    }

    private void WriteFile(string val)
    {
        print("Encrypting....");
        Encryption encrypt = new Encryption();
        string base64 = encrypt.AESEncryption(val);//encrypt username and password
        print("IP File Encrypted:" + base64);
        FileManager.CreateFile(UDP_IP_SavePath);
        FileManager.WriteDataToFile(UDP_IP_SavePath, base64);
    }
}
