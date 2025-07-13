using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine.SceneManagement;
using System;

public class ReceiveUdp : MonoBehaviour
{
    UdpClient udpClient;
    int portnum = 22222;

    void Start()
    {
        udpClient = new UdpClient(portnum);
        //SetPoint();
    }
    void Update()
    {
        ManageUdpMessages();
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

                if (message.ToLower().Contains("recalibrate"))
                {
                    SceneManager.LoadScene("calibration");
                }

            }
        }
        catch (Exception e)
        {
            Scoring.logs += "\n" + e.Message + ":" + e.StackTrace;
            Debug.Log("Exception:" + e.StackTrace + " " + e.Message);
        }
    }

}

