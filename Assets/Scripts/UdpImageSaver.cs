using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using System.Threading;
using TMPro;
using System.Globalization;
using UnityStandardAssets.Characters.FirstPerson;

public class UdpImageSaver : MonoBehaviour
{
    string IP = " ";
    public int Sportnum;
    public int portNum = 22223;
    UdpClient udpClientS;
    UdpClient udpClient;
    IPEndPoint remoteEndPoint;
    static byte[] imageBytes;
    public string message = "";

    public string saveFolderPath = "Assets/ReceivedImage";

    bool isSaveImage = true;
    int udpSendOnceFlag = 1;
    static bool saveImage = false;


    void Start()
    {
        init(); 
    }

    // Update is called once per frame
    void Update()
    {
        if(saveImage)
        {
            saveUDPImage();
        }
    }

    /// </ImageProcessing Functions>
    ///
    public static void receiveImageBytes(byte[] bytes)
    {
        imageBytes = bytes;
        saveImage = true;
    }
    private void saveUDPImage()
    {
        print("RE: Saving Image...");
        StartCoroutine(convertImage());
        saveImage = false;
    }
    IEnumerator convertImage()
    {
        while (true)
        {
            Texture2D receivedTexture = BytesToTexture(imageBytes);
            saveTextureToFile(receivedTexture);
            yield return null;
        }

    }
    public static Texture2D BytesToTexture(byte[] bytes)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);


        return texture;
    }
    public void saveTextureToFile(Texture2D texture)
    {
        if(!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        string fileName = $"Image_{DateTime.Now:yyyyMMddHHmmss}.png";

        string filePath = Path.Combine(saveFolderPath,fileName);

        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        Debug.Log($"RE: Texture saved to: {filePath}");

    }

    /// </ImageProcessing Endpoint>
    /// <param name="bytes"></param>

    public void saveScoreScreenCapture()
    {

        sendEndless(message);
        if (Scoring.ammo_setting == "Live")
        {
            sendEndless(message);
        }
        if (Scoring.ammo_setting == "Laser" && udpSendOnceFlag == 0)
        {
            print("Saving Unity Screen capture");

            udpSendOnceFlag = 1;
        }

    }
    private void sendEndless(string testStr)
    {
        IP = Shooting.UDP_ClientIP_Address;
        //print("UDP IP IS " + IP);
        Sportnum = 55552;
        udpClientS = new UdpClient();
        //----------------------------
        // Sending
        //----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), Sportnum);

        // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
        byte[] data = Encoding.UTF8.GetBytes(testStr);
        print("sending " + testStr + " to " + remoteEndPoint);
        // Den message zum Remote-Client senden.
        udpClientS.Send(data, data.Length, remoteEndPoint);
    }
    void init()
    {
        udpClient = new UdpClient();
        remoteEndPoint = null;
        udpClientS = new UdpClient();
        IP = "192.168.0.118";
        Sportnum = 55554;
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), Sportnum);

    }
}
