using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.IO;
using System.Windows;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.Management;
using System.Diagnostics;
using System.Numerics;

public class IPFinder : MonoBehaviour
{
    string[] ipAddresses = { "00FFB1B9AF6E", "105.245.98.152" , "04421A89A8E1"};
    string UDP_IP_SavePath = "AddrDat00.txt";
    public static string UDP_ClientIP_Address;

    public static string LocalIP = " ";
    public static string Ipv4Address = " ";
    public static string MacAddress = " ";
    public static string loginMacAddress = " ";
    string FinalM_LocalIP = "";
    int arrayLen = 3;
    string externalIP;
    int len;
    string finalIPString;
    public bool result;
    string strResult;
    public GameObject ErrorMessage;
    public GameObject severMan;
    public bool loginPressed;
    bool ipFound = false;


    // Start is called before the first frame update
    void Start()
    {
        //LoadIP();
        CreateFile();
        GetMACAddress();
        FindLocalIP();  // Function for finding the Current IPv4 Address

        //GenerateUniqueID();
        
    }
    void Update()
    {
        loginPressed = severMan.GetComponent<login_Manager>().loginPressed;

        if (loginPressed)
        {
            FindIPAddress();
        }
        //print("Final IP is : " + FinalM_LocalIP);
    }

    public void GetMACAddress()
    {

        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        int size = 1;
        loginMacAddress = "";
        foreach (NetworkInterface adapter in nics)
        {
            PhysicalAddress address = adapter.GetPhysicalAddress();
            string macAddress = address.ToString();

            if (!string.IsNullOrEmpty(macAddress))
            {
                loginMacAddress += FormatMacAddress(macAddress);
                loginMacAddress = loginMacAddress.Replace(":", "");
                if (size < nics.Length - 1)
                {
                    loginMacAddress += ";";
                }
            }
            size++;
        }


        //print("MAC Address: " + loginMacAddress);

        result = false;

        String sMacAddress = string.Empty;
        foreach (NetworkInterface adapter in nics)
        {
            if (sMacAddress == String.Empty)// only return MAC Address from first card  
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                sMacAddress = adapter.GetPhysicalAddress().ToString();
                MacAddress = sMacAddress;
            }
        }

        // finalIPString = sMacAddress;
        // refactor

        // print("Mac Adrress is :" + MacAddress);
        // print("Mac Address list: " + loginMacAddress);

    }
    string ReplaceCharacter(string originalString, char charToReplace, char replacementChar)
    {
        return originalString.Replace(charToReplace, replacementChar);
    }
    public void LoadIP()
    {
        result = false;
        externalIP = new WebClient().DownloadString("https://icanhazip.com/");
        //externalIP = "41";
        len = externalIP.Length;
        AllignIp();
    }
    private void FindLocalIP()
    {
        IPHostEntry host;                           //create Host
        host = Dns.GetHostEntry(Dns.GetHostName()); //Get current DNS Server

        foreach(IPAddress ip in host.AddressList)   //Read all IP's in the Current DNS and Load the IPv4
        {
            if(ipFound == false)
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
    public void AllignIp()
    {

        //print(len);
        for (int x =0; x < len; x++)
        {

            if(char.IsDigit(externalIP[x]) || char.IsPunctuation(externalIP[x]))
            {
                finalIPString = finalIPString + externalIP[x];
            }

        }

    }

    static string FormatMacAddress(string macAddress)
    {
        // Add colons to the MAC address for better readability
        for (int i = 2; i < macAddress.Length; i += 3)
        {
            macAddress = macAddress.Insert(i, ":");
        }
        return macAddress;
    }

    public void FindIPAddress()
    {
        result = Array.Exists(ipAddresses, element => element == finalIPString);        //Check if the current network IP is available in the registered IP_Array
        //strResult = Array.Find(ipAddresses, element => element == finalIPString);
        //print("IP address is: " + finalIPString);

        if(result == false)
        {
            //print("You Are not authorised to use this product.");
            //print("MacAddress is: " + finalIPString);
            //ErrorMessage.SetActive(true); 
        }
        else
        {
            //print("Access granted");
            //print("MacAddress is: " + finalIPString);
        }

    }
    public void CloseErrorMessage()
    {
        ErrorMessage.SetActive(false);
        severMan.GetComponent<login_Manager>().loginPressed = false;
        LoadIP();
    }
    private void ManipulateIP(string ip)
    {
        //LocalIP
        
        int x = 3;
        while(x >= 1)
        {
            FinalM_LocalIP += ip.Substring(0, ip.IndexOf('.') + 1); // Load Current value before pound '.' to the new IP variable
            ip = ip.Remove(0, ip.IndexOf('.') + 1);            //Delete Current Value before pount '.'
             
            if(x == 1)
            {
                FinalM_LocalIP += "255";
            }
            x--;
        }
    }
    public void CreateFile()
    {
        FileManager.CreateFile(UDP_IP_SavePath);
    }

    private void WriteFile(string val)
    {
        //print("Encrypting....");
        Encryption encrypt = new Encryption();
        string base64 = encrypt.AESEncryption(val);//encrypt username and password
        //print("IP File Encrypted:" + base64);
        FileManager.CreateFile(UDP_IP_SavePath);
        FileManager.WriteDataToFile(UDP_IP_SavePath, base64);
    }


    // Unique identifier generators
    public static string GetUniqueID()
    {
        string ID = "";

        ID = GenerateHardwareID();
        ID = ConvertHashToNumber(ID);

        //print("ID is: " + ID);
        return ID;
    }
    public static string GenerateHardwareID()
    {
        string os = SystemInfo.operatingSystem;
        string rawID = "";

        if (os.Contains("Windows"))
        {
            rawID = GetWindowsHardwareID();
        }
        else if (os.Contains("Mac"))
        {
            rawID = GetMacHardwareID();
        }
        else if (os.Contains("Linux"))
        {
            rawID = GetLinuxHardwareID();
        }
        else
        {
            rawID = SystemInfo.deviceUniqueIdentifier; // Fallback for unknown OS
        }

        return ComputeSHA256(rawID);
    }

    private static string GetWindowsHardwareID()
    {
        return RunCommand("wmic", "bios get serialnumber");
    }

    private static string GetMacHardwareID()
    {
        return RunCommand("ioreg", "-l | grep IOPlatformSerialNumber");
    }

    private static string GetLinuxHardwareID()
    {
        return RunCommand("cat", "/var/lib/dbus/machine-id"); // Works on most Linux distros
    }

    private static string RunCommand(string command, string arguments)
    {
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                string result = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();
                return string.IsNullOrEmpty(result) ? "UNKNOWN" : result;
            }
        }
        catch
        {
            return "UNKNOWN";
        }
    }

    private static string ComputeSHA256(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }

    public static string ConvertHashToNumber(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Convert hash bytes to a big integer
            BigInteger bigInt = new BigInteger(hashBytes);

            // Ensure positive value (SHA-256 can sometimes produce a negative BigInteger)
            if (bigInt < 0)
                bigInt = -bigInt;

            // Convert to a fixed-length numeric string (Example: 20 digits)
            string numericID = bigInt.ToString().Substring(0, 20);
            return numericID;
        }
    }

}
/*for(int x = 0; x < arrayLen; x++)
     {
         if(ipAddresses[x] == finalIPString)
         {
             result = true;
         }

     }*/