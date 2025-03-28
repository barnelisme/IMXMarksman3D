using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDPBroadcast : MonoBehaviour
{
    //string _listenPort = " ";
    string MyIpAddress = "192.168.2.1";
    // Server Code:

    private void SendUdpBroadcast()
    {
        //var myIpAddressSegments = MyIpAddress.GetAddressBytes();
        //var firstTwoOctetsSegment = string.Join(".", myIpAddressSegments[0], myIpAddressSegments[1]);

        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //var broadcast = IPAddress.Parse(firstTwoOctetsSegment + ".255.255"); // Gets set to 192.168.255.255. Is this correct???

        var sendbuf = Encoding.ASCII.GetBytes("Testing...");
        //var ep = new IPEndPoint(broadcast, _listenPort);

        //s.SendTo(sendbuf, ep);

        Console.WriteLine("Message sent to the broadcast address");
    }
}
