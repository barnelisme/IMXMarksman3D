using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;


public class ReceiveUdp : MonoBehaviour
{
    public Transform playerBody;
    public int portNum = 33447;
    public UdpClient udpClient2;

    void Start()
    {
        playerBody = GameObject.Find("Player").GetComponent<Transform>();
        udpClient2 = new UdpClient(portNum);
        //SetPoint();
    }
    void Update()
    {
        SetPoint();
    }
    private void SetPoint()
    {
        IPEndPoint remoteEP = null;
        if (udpClient2.Available > 0)
        {
            byte[] data = udpClient2.Receive(ref remoteEP);
            string message = Encoding.ASCII.GetString(data);
            print(message + " from " + remoteEP.Address.ToString());
        }
    }
}

