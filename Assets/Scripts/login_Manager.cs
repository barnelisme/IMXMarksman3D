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
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.InputSystem;
using System.Linq;
#if UNITY_STANDALONE_WIN
using System.Management;
#endif

public class login_Manager : MonoBehaviour
{
    private string Url= "http://192.168.137.1/imagistix/login.php";
    string UDP_IP_SavePath = "AddrDat00.txt";
    string Offline_LoginFile_Savepath = "ZeroDat.txt";
    string lockedMacAddress = "D843AE65F630"; //00:FF:B1:B9:AF:6E //84:A9:38:0F:AB:77//D8:43:AE:65:F6:30//"B4:A9:FC:9F:A9:C0"//16:13:33:D3:B0:A3
    string lockedMacAddress2 = "D8:43:AE:65:F6:30"; //04:7C:16:AC:31:8E //FA89D2815C21//F2:A7:31:4A:35:30/"BC542FCD6479"//96:13:33:D3:B0:A3
    public float UDPSendTimer =0.5f;
    float UDPSetTime = 0.5f;
    public bool sendUDP = false;
    bool sendConfigUDP = false;

    private string login_endpoint;
    public InputField email;
    public InputField password;
    public Text errorMessage;
    static public string EmailText = "admin";
    static public string passwordText = "";
    static public bool default_admin = false;
    bool offline_admin_login_attempted = false;

    public GameObject severMan;
    public bool loginPressed = false;
    bool hasAlreadyLogged = false;
    public static string loginData;
    public static float globalTimerSet = 50400;       //Reset variables
    public static float global_active_timer = 50400;  //14 Hours in seconds //50400  //300 = 5 Minutes
    public static float globalCAmeraTimerSet = 3600;  //Reset variables
    public static float global_Camera_active_timer = globalCAmeraTimerSet;  //3 Hours in seconds
    public static bool rebootCamera = false;

    //UDP Communiocation
    UdpClient udpClient;
    UdpClient udpClientS;
    public int portnum = 22222;
    IPEndPoint remoteEndPoint;
    private string IP;
    public int Sportnum;
    public static string UDP_ClientIP_Address;

    //CRM Communication Components
    public TextMeshProUGUI crm_response;
    public GameObject crm_panel;

    //Offline Date authorisation variables
    DateTime date;
    string currentDate;
    bool isAuthorised = false;
    int num_authorised_days = 4;
    int deadline;
    int access_lock_flag = 0;
    private bool lock_key_found = false;

    void Start()
    {
        default_admin = false;
        MultipleScreens();
        sendConfigUDP = true ;
        sendUDP = true;
        CreateFile();
        ReadFile();
        init();
        //openPyFile();
        //openExeFile();
        //checkLoginDate();
        global_active_timer = globalTimerSet;
        //openExe();
        global_Camera_active_timer = globalCAmeraTimerSet;
    }
    private void Update()
    {
        //sendEndless("Calibration");
        //print("IP is  " +  IPFinder.LocalIP );

        if (Keyboard.current.enterKey.isPressed)
        {
            OnLoginButtonClicked();
        }

        if (sendUDP)
        {

            UDPSendTimer -= Time.deltaTime * 1;
            if (UDPSendTimer <= 0f)
            {
                sendUDP = false;
            }

            if(sendConfigUDP)
            {
                sendEndless("stop");
                sendConfigUDP = false;
            }

           
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
        IP = "192.168.0.126";
        Sportnum = 55554;
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), Sportnum);

    }
    public void OnLoginButtonClicked()
    {
        loginPressed = true;
        StartCoroutine(Login()); 
    }
    public void openPyFile()
    {
        var p = new System.Diagnostics.Process
        {
            StartInfo =
                 {
                     FileName = "python",
                     WorkingDirectory = "assets",
                     Arguments = "BrightestPointSilder.py"
                 }
        }.Start();
    }
    public void openExeFile()
    {
        print("Opening Setup exe");

        //BrightestPointSilder
        //string pathToExe = Application.dataPath + "/datPointCsv.exe";
        string pathToExe = Application.dataPath + "/datPointCsv.exe";
        print("Path to file is: " + pathToExe);
        ProcessStartInfo startInfo = new ProcessStartInfo(pathToExe);
        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();
    }
    private void openExe()
    {

    }

    public void openCameraExe()
    {
        string pathToCameraExe = Application.dataPath + "/../Cam Files/datPointCsv.exe";//Ensure that the exe is outside the asset folder on the same directory with the Unity Exported exe
        string workingGUIDirectory = Path.GetDirectoryName(pathToCameraExe);
        string exeFileName = Path.GetFileName(pathToCameraExe);

        StaticVariableManager.openCameraOpt = true;

        // Terminate running instances of datPointCsv.exe if any
        string targetExeFileName = "datPointCsv.exe"; // Adjust to the correct executable name
        TerminateRunningProcesses(targetExeFileName);

        // Check if the GUIApp.exe is already running
        if (!IsProcessRunning(exeFileName))
        {
            ProcessStartInfo guiStartInfo = new ProcessStartInfo(pathToCameraExe)
            {
                WorkingDirectory = workingGUIDirectory
            };

            using (Process process = new Process())
            {
                process.StartInfo = guiStartInfo;
                process.Start();
            }
        }
        else
        {
            Debug.Log("The datPointCsv.exe is already running.");
        }

        //Process process_1 = new Process();
        //process_1.StartInfo = camStartInfo;
        //process_1.Start();
    }

    public void openCameraSettingsExe()
    {
        string pathToGUIExe = Application.dataPath + "/../Cam Files/GUIApp.exe";
        string workingGUIDirectory = Path.GetDirectoryName(pathToGUIExe);
        string exeFileName = Path.GetFileName(pathToGUIExe);

        // Terminate running instances of datPointCsv.exe if any
        string targetExeFileName = "GUIApp.exe"; // Adjust to the correct executable name
        TerminateRunningProcesses(targetExeFileName);

        // Check if the GUIApp.exe is already running
        if (!IsProcessRunning(exeFileName))
        {
            ProcessStartInfo guiStartInfo = new ProcessStartInfo(pathToGUIExe)
            {
                WorkingDirectory = workingGUIDirectory
            };

            using (Process process = new Process())
            {
                process.StartInfo = guiStartInfo;
                process.Start();
            }
        }
        else
        {
            Debug.Log("The GUIApp.exe is already running.");
        }
    }

    private bool IsProcessRunning(string exeFileName)
    {
        // Get all running processes
        Process[] processes = Process.GetProcesses();

        // Check if any process has the same name as the executable file
        return processes.Any(p => p.ProcessName.Equals(Path.GetFileNameWithoutExtension(exeFileName), StringComparison.OrdinalIgnoreCase));
    }

    private void TerminateRunningProcesses(string exeFileName)
    {
        // Get all running processes
        Process[] processes = Process.GetProcesses();

        // Check if any process has the same name as the executable file and terminate it
        foreach (Process process in processes)
        {
            if (process.ProcessName.Equals(Path.GetFileNameWithoutExtension(exeFileName), StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    process.Kill();
                    process.WaitForExit(); // Ensure process is terminated before proceeding
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error terminating process {exeFileName}: {ex.Message}");
                }
            }
        }
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

        ReadFile();
        
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

    public void CreateFileCRM(string path)
    {
        if (!File.Exists(path))
        {
            //File.Create(path).Close();
            //hasAlreadyLogged = false;
            Encryption encrypt = new Encryption();
            string base64 = encrypt.AESEncryption(email.text + ":" + password.text);//encrypt username and password
            print("Encrypted:" + base64);
            print("Decrypted: " + encrypt.AESDecryption(base64));
        }
        else
        {
            /*using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream) // reading the file while we haven't reched the END
                {
                    //Assigning dellay value
                    loginData = reader.ReadLine();
                }
            }
            if (loginData == null)
            {
                hasAlreadyLogged = false;
            }
            else
            {
                hasAlreadyLogged = true;
            }*/
            
        }
    }
    private void ReadFileCRM(string path)
    {
        

    }

    [Obsolete]
    IEnumerator Login()
    {
        //For The CODE that Runs Mac Address Inspection before login, Check after the Class Endline Below. 
        ///////////////////////////////////////////////////////////////////////
        //RUN LOGIN WITHOU MACADDRESS INSPECTION///////////////////////////////
        WWWForm form;
        form = new WWWForm();
        try //Direct Login
        {

            Url = configuration.ConfigAllUrls["server_url"];
            //print("I am Working...");
            login_endpoint = configuration.ConfigAllEndpoints["login_endpoint"];
            //Debug.Log("from configuration file, url is " + Url + login_endpoint);

            //print("email"+ email.text);
            //print("password"+ password.text);
            //print("ip"+ IPFinder.LocalIP);       // Add IP to form
            //print("mac"+ IPFinder.MacAddress);   // Add Mac to form
            //print("current_version"+ GetSystemUpdate.currentVersion);  
            //print("key"+ Encryption.key);//add key

            form.AddField("email", email.text);
            form.AddField("password", password.text);
            form.AddField("ip", IPFinder.LocalIP);       // Add IP to form
            form.AddField("mac", IPFinder.MacAddress);   // Add Mac to form
            form.AddField("current_version", GetSystemUpdate.currentVersion);   // Add Mac to form
            form.AddField("key", Encryption.key);//add key

            /*********NOW ADDING DEVICE INFO********************************************/
            Dictionary<string, object> device_MAP = new Dictionary<string, object>();
            device_MAP.Add("device_type", SystemInfo.deviceType.ToString());
            device_MAP.Add("device_model", SystemInfo.deviceModel.ToString());
            device_MAP.Add("device_name", SystemInfo.deviceName.ToString());
            device_MAP.Add("device_id", SystemInfo.deviceUniqueIdentifier.ToString());

            // Convert the dictionary to JSON
            string device_info = JsonConvert.SerializeObject(device_MAP);
            form.AddField("device_info", device_info);

            /*********NOW ADDING GRAPHICS CARD INFO*************************************/
            Dictionary<string, object> graphics_MAP = new Dictionary<string, object>();
            graphics_MAP.Add("graphics_name", SystemInfo.graphicsDeviceName.ToString());
            graphics_MAP.Add("graphics_type", SystemInfo.graphicsDeviceType.ToString());
            graphics_MAP.Add("graphics_vendor", SystemInfo.graphicsDeviceVendor.ToString());
            graphics_MAP.Add("graphics_version", SystemInfo.graphicsDeviceVersion.ToString());

            // Convert the dictionary to JSON
            string graphics_info = JsonConvert.SerializeObject(graphics_MAP);
            form.AddField("graphics_info", graphics_info);

            //print("device_info:" + device_info);
            //print("graphics_info:" + graphics_info);

            EmailText = email.text;
            passwordText = password.text;

            if (email.text == "admin" && password.text == "GrantAccess@12")//defaut credentials
            {
                if (true)
                {
                    Scoring.instructor = "admin";
                    SceneManager.LoadScene("MainMenu");
                    default_admin = true;
                }
                else
                {
                    offline_admin_login_attempted = true;
                }

            }
            else if(email.text.ToLower().Contains("range") && password.text == "password.range")//defaut credentials
            {
                //print("Email: " + EmailText);
                //print("password: " + passwordText);

                //print("RRE: Logged In... ");

                //print("Mac Address: " + IPFinder.loginMacAddress);

                if (IPFinder.loginMacAddress.Contains(lockedMacAddress))
                {
                    if (email.text == "j.range")
                    {
                        Scoring.instructor = "JHB.Range Admin";
                        default_admin = true;
                    }
                    else if (email.text == "c.range")
                    {
                        Scoring.instructor = "Centurion.Range Admin";
                        default_admin = true;
                    }
                    
                    if (default_admin)
                    {
                        SceneManager.LoadScene("MainMenu");
                    }
                }
                else
                {
                    offline_admin_login_attempted = true;
                }
            }
        }
        catch (Exception e)
        {
            print("A serious error just happened, probably first login!!" + e);
        }


        if (!default_admin) //Not default admin.
        {
            Scoring.writeLog("Sending login to :" + Url + login_endpoint);
            WWW www = new WWW(Url + login_endpoint, form);
            yield return www;
            Scoring.writeLog("Response:" + www.text);
            //print("RRE: Login Response:" + www.text);

            checkLoginDate();
            if(email.text == "")
            {
                crm_response.text = "<b>Input space empty.</b> \n \n" +
                    "Please enter login details.";
                crm_panel.SetActive(true);
                www.Dispose();
            } //Input place holders checked
            else if (www.error != null)// if the response contains an error go directly to offline login
            {
                RunOfflineLogin(www);

            } // Offline checked
            else
            {
                if (www.text.Contains("success")) 
                {
                    if((getRegisterdID() == "not set" || getRegisterdID() == IPFinder.GetUniqueID()))
                    {
                        //print("Test: Online Login");
                        SaveLoginInfo();
                        SceneManager.LoadScene("MainMenu");
                    }
                    else
                    {
                        crm_response.text = "<b>Access Denied.</b> \n" +
                                "\n Please request authorisation from " +
                                "the developer. \n";
                        crm_panel.SetActive(true);
                        www.Dispose();
                    }
                }
                else //offline login
                {
                    RunOfflineLogin(www);
                }

            }//Online checked

            www.Dispose();
        }

    }

    [Obsolete] //IMPORTANT, DO NOT REMOVE
    void RunOfflineLogin(WWW www)
    {
        print("Test: Offline Login");
        try
        {
            string[] offlineData = GetOfflineLoginData();
            string registeredID = getRegisterdID();

            if (isAuthorised && offline_admin_login_attempted == false) //check authorisation
            {
                bool emailFound = false;
                foreach (string credential_line in offlineData) // run through stored offline data
                {
                    if (credential_line.Contains(email.text))
                    {
                        string[] credentials = credential_line.Split(":");
                        emailFound = true;

                        if(registeredID == IPFinder.GetUniqueID()) //Confirm System ID
                        {
                            if (email.text == credentials[0] && password.text == credentials[1])
                            {
                                //SceneManager.LoadScene("calibration");
                                SceneManager.LoadScene("MainMenu");
                            }
                            else if ((email.text == credentials[0] && password.text != credentials[1]) || (email.text != credentials[0] && password.text == credentials[1]))
                            {
                                crm_response.text = "<b>Incorrect Email or Password.</b> \n" + "Please insert correct login details.";
                                crm_panel.SetActive(true);
                                www.Dispose();
                            }
                            else
                            {
                                crm_response.text = "<b>Incorrect Email or Password.</b> \n" +
                                    "Please double check you login details.";
                                crm_panel.SetActive(true);
                                www.Dispose();
                            }
                        }
                        else
                        {
                            crm_response.text = "<b>Access Denied.</b> \n" +
                                "\n" +
                                "Please connect to the internet for your login authorisation, or request authorisation from " +
                                "the developer. \n";
                            crm_panel.SetActive(true);
                            www.Dispose();
                            break;
                        }
                    }

                }

                if (emailFound == false)
                {
                    crm_response.text = "<b>Email not registered or incorrect.</b> \n" +
                            "\n" +
                            "Please check your details and try again, or request " +
                            "authorisation from the developer. \n";
                    crm_panel.SetActive(true);
                    www.Dispose();
                }

            }
            else
            {
                crm_response.text = "<b>Access Denied.</b>\n" +
                    "\n" +
                    "Please connect to the internet for your login authorisation, or request authorisation from " +
                    "the developer. \n";
                crm_panel.SetActive(true);
                www.Dispose();
            }

        }
        catch (Exception e)
        {
            crm_response.text = "- error connecting to the internet.";
            crm_panel.SetActive(true);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            www.Dispose();
        }

    }

    private string getRegisterdID()
    {
        string[] offlineData = GetOfflineLoginData();
        string ID = "not set";
        try
        {
            string[] credentials = offlineData[0].Split(":");
            ID = credentials[2];
        }
        catch
        {
            //pass
        }

        //print("ID is: " + ID);
        return ID;
    }

    private bool MacLockApproved(string current_mac_addresses, string saved_mac_addresses)
    {
        //Check if saved mac address string contains any of the current detected mac addresses
        //print("Test: Current mac address is " + current_mac_addresses);
        //print("Test: saved mac address is " + saved_mac_addresses);

        string[] mac_addresses = current_mac_addresses.Split(";");

        foreach(string address in mac_addresses)
        {
            if (saved_mac_addresses.Contains(address))
                //print("Mav lock aproved");
                return true;
        }

        //print("Mav lock denied");
        return false;
    }

    void SaveLoginInfo()
    {
        Scoring.instructor = email.text;

        try
        {
            SaveCredentials();
            writeLastLoginDate();
            resetAccessNumber();
        }
        catch (System.Exception e)
        {
            print("Error writting credentials locally." + e);
            Scoring.writeLog("Error writting credentials locally.");
        }
    }

    private void checkLoginDate()
    {
        date = DateTime.Now;
        currentDate = date.ToString();
        currentDate = currentDate.Substring(0, 10);
        string loginDate = "";
        int accessNumber = 0;

        string[] loginDateArray;
        string loginYear, loginMonth, loginDay;
        string[] currentDateArray;
        string currentYear, currentMonth, currentDay;

        FileManager.CreateFile("logDate.txt");
        loginDate = FileManager.decryptFile("logDate.txt");
        loginDateArray = loginDate.Split('/'); //Split online login date
        currentDateArray = currentDate.Split('/'); //split current date

        //Store login date
        loginYear = loginDateArray[0];
        loginMonth = loginDateArray[1];
        loginDay = loginDateArray[2];

        //Store current date
        currentYear = currentDateArray[0];
        currentMonth = currentDateArray[1];
        currentDay = currentDateArray[2];

        if (loginDay != currentDay) //if next day
        {
            accessNumber = chechActiveDays();  //Check access number
            if(accessNumber >= num_authorised_days)
            {
                isAuthorised = false;
            }
            else
            {
                accessNumber += 1;
                incriAccessNumber(accessNumber);
                writeLastLoginDate();
                isAuthorised = true;
            }
        }
        else
        {
            isAuthorised = true;
        }

        /*print("RE: ly is "+ loginYear);
        print("RE: lm is " + loginMonth);
        print("RE: ld is " + loginDay);
        print("RE: cy is " + currentYear);
        print("RE: cm is " + currentMonth);
        print("RE: cd is " + currentDay);*/
        //print("RE: active days is " + accessNumber);
        
        if (isAuthorised)
        {
            //print("RE: Access Granted...");
        }
        else
        {
            //print("RE: Access Denied...");
        }

    }
    private int chechActiveDays()
    {
        int accessNumber = 0;

        FileManager.CreateFile("Access.txt");
        string[] data = (FileManager.decryptFile("Access.txt")).Split(',');
        accessNumber = int.Parse(data[1]);

        return accessNumber;
    }
    private void writeLastLoginDate()
    {
        FileManager.CreateFile("logDate.txt");
        FileManager.incriptToFile("logDate.txt", currentDate.ToString());
        //print("RE: Date Stored");
    }
    private void resetAccessNumber()
    {
        FileManager.CreateFile("Access.txt");
        FileManager.incriptToFile("Access.txt", "access,0");
        //print("RE: Access number restored");
    }
    private void incriAccessNumber(int num)
    {
        FileManager.CreateFile("Access.txt");
        FileManager.incriptToFile("Access.txt", "access," + num.ToString());
    }
    void SaveCredentials()
    {
        
        Encryption encrypt = new Encryption();
        //string base64 = encrypt.AESEncryption(email.text + ":" + password.text + ":" + IPFinder.loginMacAddress);//encrypt username and password
        string login_credentials = email.text + ":" + password.text + ":" + IPFinder.GetUniqueID();

        FileManager.CreateFile(Offline_LoginFile_Savepath);
        FileManager.WriteLineDataToFile(Offline_LoginFile_Savepath, login_credentials);
        //print("Decrypted: " + encrypt.AESDecryption(base64));
        
    }
    public void onButtonClickClose()
    {
        crm_panel.SetActive(false);
    }    
    private string [] GetOfflineLoginData()
    {
        string[] login_data = new string[20];
        string registeredID = "0";

        login_data = FileManager.ReadLinesFromFile(Offline_LoginFile_Savepath);
        /*string base64 = FileManager.ReadFromFile(Offline_LoginFile_Savepath);
        string incription = "";
        try // if data is decryptable
        {
            Encryption encrypt = new Encryption();
            incription = encrypt.AESDecryption(base64);                    //decryption code
        }
        catch(Exception e) // if data is not decryptable 
        {
            incription = base64;
        }

        //print(incription);
        return incription;*/

        return login_data;
    }
    private void MultipleScreens()
    {
        //Debug.Log(Display.displays.Length + " is/are connected");

        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }

    }
}
/*if (severMan.GetComponent<IPFinder>().result == true)
       {
           WWWForm form;
           form = new WWWForm();
           Url = configuration.ConfigAllUrls["server_url"];
           //print("I am Working...");
           login_endpoint = configuration.ConfigAllEndpoints["login_endpoint"];
           Debug.Log("from configuration file, url is " + Url + login_endpoint);

           form.AddField("email", email.text);
           form.AddField("password", password.text);
           form.AddField("ip", IPFinder.LocalIP);       // Add IP to form
           form.AddField("mac", IPFinder.MacAddress);   // Add Mac to form

           print("Messages Sent to Web Page");

           EmailText = email.text;
           if (email.text == "admin" && password.text == "GrantAccess@12")//defaut credentials
           {
               Scoring.instructor = "admin";
               SceneManager.LoadScene("calibration");
           }
           if (email.text != "admin")
           {
               Scoring.writeLog("Sending login to :" + Url + login_endpoint);
               WWW www = new WWW(Url + login_endpoint, form);
               yield return www;
               Scoring.writeLog("Response:" + www.text);
               print("Response:" + www.text);
               if (www.error != null)// != UnityWebRequest.Result.Success)
               {
                   Debug.Log(www.error);
               }
               else
               {
                   if (www.text.Contains("success"))
                   {
                       Scoring.writeLog("We got success");
                       Scoring.instructor = email.text;
                       SceneManager.LoadScene("calibration");
                   }
                   else
                   {
                       Scoring.writeLog("Invalid password or username");
                       errorMessage.text = "Invalid password or username";
                   }
               }
               www.Dispose();
           }
           else if (Scoring.instructor == "admin" && password.text == "GrantAccess@12")
           {

               SceneManager.LoadScene("calibration");

           }
           else
           {
               Scoring.writeLog("invalid credentials");
               Debug.LogError("invalid credentials");
           }
       }*/

/*  
 *  else if (IPFinder.MacAddress != offline[2])
     {
       crm_response.text = "- Unable to login, please use your default set computer, \n" + " or connect to the internet for login reset.";
       crm_panel.SetActive(true);
       www.Dispose();
      }
     if (email.text == offline[0] && password.text == offline[1] && IPFinder.MacAddress == offline[2])
      {
        SceneManager.LoadScene("calibration");
      }
 */