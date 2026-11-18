using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UdpReceiver : MonoBehaviour
{
    UdpClient udpClient;
    Thread receiveThread;
    int port = 5005; // Match this with sender

    void Start()
    {
        udpClient = new UdpClient(port);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        // Debug.Log("UDP Receiver started on port " + port);
    }

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string text = Encoding.UTF8.GetString(data);
                Debug.Log("Received: " + text);
            }
            catch (SocketException e)
            {
                Debug.Log("Socket exception: " + e);
                break;
            }
        }
    }

    void OnApplicationQuit()
    {
        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Abort();

        udpClient.Close();
    }
}
