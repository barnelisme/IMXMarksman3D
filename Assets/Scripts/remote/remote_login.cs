using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class remote_login : MonoBehaviour
{

    // Replace "your-php-server.com/check-mac-and-ip.php" with the URL of your PHP script
    private string url = "http://your-php-server.com/check-mac-and-ip.php";

    IEnumerator SendRequest()
    {
        // Get the MAC address of the computer
        string macAddress = SystemInfo.deviceUniqueIdentifier;

        // Get the IP address of the computer
        // Get the IP address of the local machine
        string ipAddress = "";
        IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
        foreach (IPAddress address in localIPs)
        {
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                ipAddress = address.ToString();
                break;
            }
        }

        // Print the IP address to the console
        Debug.Log("Local IP Address: " + ipAddress);

        // Create a web request
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";

        // Create the payload
        string postData = "macAddress=" + macAddress + "&ipAddress=" + ipAddress;
        byte[] byteArray = Encoding.UTF8.GetBytes(postData);
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = byteArray.Length;

        // Write the payload to the request stream
        Stream dataStream = request.GetRequestStream();
        dataStream.Write(byteArray, 0, byteArray.Length);
        dataStream.Close();

        // Send the request and wait for the response
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string responseText = reader.ReadToEnd();
        reader.Close();
        response.Close();

        // Handle the response
        if (responseText == "success")
        {
            Debug.Log("MAC and IP address found in the database.");
        }
        else
        {
            Debug.Log("MAC and IP address not found in the database.");
        }

        yield return null;
    }

    void Start()
    {
        StartCoroutine(SendRequest());
    }
}
