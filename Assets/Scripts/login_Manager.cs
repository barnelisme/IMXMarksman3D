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
    string Offline_LoginFile_Savepath = "ZeroDat.txt";
    string lockedMacAddress = "D843AE65F630"; //00:FF:B1:B9:AF:6E //84:A9:38:0F:AB:77//D8:43:AE:65:F6:30//"B4:A9:FC:9F:A9:C0"//16:13:33:D3:B0:A3
    string lockedMacAddress2 = "D8:43:AE:65:F6:30"; //04:7C:16:AC:31:8E //FA89D2815C21//F2:A7:31:4A:35:30/"BC542FCD6479"//96:13:33:D3:B0:A3
    public float UDPSendTimer =0.5f;
    float UDPSetTime = 0.5f;
    public bool sendUDP = false;
    bool sendConfigUDP = false;
    private bool isLoggedIn = false;
    private bool offlineEmailFound = false;

    private string login_endpoint;
    public InputField email;
    public InputField password;
    public Text errorMessage;
    static public string EmailText = "admin"; // Empty
    static public string passwordText = "";
    static public bool isSuperAdmin = false;
    bool standard_admin_login_attempted = false;

    public GameObject severMan;
    public bool loginPressed = false;
    bool hasAlreadyLogged = false;
    public static string loginData;
    private string[] splittedLoginData;
    private int loginCounter = 0;
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
        isSuperAdmin = false;
        MultipleScreens();
        sendConfigUDP = true ;
        sendUDP = true;
        CreateFile();
        ReadFileSettings();
        init();
        //openPyFile();
        //openExeFile();
        checkLoginDate();
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

        ReadFileSettings();
        
        IP = UDP_ClientIP_Address;
        Sportnum = 55552;

        // print(UDP_ClientIP_Address);

        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), Sportnum);
        
        // Daten mit der UTF8-Kodierung in das Bin�rformat kodieren.
        byte[] data = Encoding.UTF8.GetBytes(testStr);
        //print("sending " + testStr + " to " + remoteEndPoint);
        // Den message zum Remote-Client senden.
        udpClientS.Send(data, data.Length, remoteEndPoint);
    }

    public void CreateFile()
    {
        // Offline_LoginFile_Savepath 
        //FileManager.CreateFile(UDP_IP_SavePath);
        FileManager.CreateFile(Offline_LoginFile_Savepath);

        //FileManager.WriteDataToFile(Offline_LoginFile_Savepath, "no set");
    }
    private void ReadFileSettings()
    {
        //UDP_ClientIP_Address = reader.ReadLine();
        try
        {
            UDP_ClientIP_Address = FileManager.ReadSetting("UDP IP");                    //decryption code
        }
        catch (Exception e)
        {
            print("Error retrieving data from file:" + e);
            throw new Exception("First login");
        }


    }

    [Obsolete]
    IEnumerator Login()
    {
        EmailText = email.text;
        passwordText = password.text;
        isSuperAdmin = false;

        ManageSuperAdminLogin();

        // if(!isSuperAdmin && StaticVariableManager.isSystemUnlocked) RunOfflineLogin(); // Run if it's not super admin login
        if (isLoggedIn || isSuperAdmin) yield return null; // Break if offline login went through

        WWWForm form = new WWWForm();
        AssignFormData(form);
        WWW www = new WWW(Url + login_endpoint, form);
        yield return www;

        try
        {

            if (isAuthorised && StaticVariableManager.isSystemUnlocked) //check authorisation
            {
                if (!isSuperAdmin) ManageStandardAdminLogin(www);  //Not super admin.
            }
            else
            {
                AccessDeniedCRMFallback();
            }

        }
        catch (Exception e)
        {
            print("A serious error just happened, probably first login!!" + e.Source);
        }
    }

    private void CheckSystemUser()
    {
        // Check user to set background image
        // Below are The Shooting Range Access email
        //
        // NOTE: ONLY SET FOR THE SHOOTING RANGE
        if (EmailText == "mecaeladefault1@gmail.com" || EmailText == "normandefault1@gmail.com" || EmailText == "michelenell00@gmail.com") 
        {
            FileManager.SaveSetting("System User", "The Shooting Range");
        }
        else if(EmailText == "jan@aimandfire.co.za" || EmailText == "marinus@aimandfire.co.za")
        {
            // 
            FileManager.SaveSetting("System User", "Aim And Shoot");
        }
        else if (EmailText == "bullseyebay@example.com")
        {
            // 
            FileManager.SaveSetting("System User", "Presentation");
        }

    }

    private void AssignFormData(WWWForm form)
    {
        Url = configuration.ConfigAllUrls["server_url"];
        //print("I am Working...");
        login_endpoint = configuration.ConfigAllEndpoints["login_endpoint"];
        //Debug.Log("from configuration file, url is " + Url + login_endpoint);

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
    }

    private void ManageSuperAdminLogin()
    {
        if (email.text == "admin" && password.text == "GrantAccess@12")//defaut credentials
        {
            Scoring.instructor = "admin";
            isSuperAdmin = true;
            // print("Super admin Login");
        }
        else if (email.text.ToLower().Contains("range") && password.text == "password.range")//defaut credentials
        {
            if (email.text == "j.range")
            {
                Scoring.instructor = "JHB.Range Admin";
                isSuperAdmin = true;
            }
            else if (email.text == "c.range")
            {
                Scoring.instructor = "Centurion.Range Admin";
                isSuperAdmin = true;
            }
        }

        // Login
        if (isSuperAdmin) OpenMenu();
    }

    [Obsolete] //IMPORTANT, DO NOT REMOVE
    private void ManageStandardAdminLogin(WWW www)
    {
        Scoring.writeLog("Sending login to :" + Url + login_endpoint);
        Scoring.writeLog("Response:" + www.text);
        print("RRE: Login Response:" + www.text);
        

        checkLoginDate();
        if (email.text == "")
        {
            EmptySpaceCRMFallback(); // If email or password space is empty
        }
        else //Input place holders not empty
        {
            if (www.error != null) // if hub score response contains an error go directly to offline login
            {
                RunOfflineLogin();
            }
            else
            {
                splittedLoginData = www.text.Trim().Replace(" ", "").Split(','); // Unpack the login response

                bool isCredentialsValid = splittedLoginData.Length > 0 && splittedLoginData[0] == "success";
                bool isSubscriptionActive = splittedLoginData.Length > 0 && (splittedLoginData[2].ToLower() == "active" || splittedLoginData[2].ToLower() == "*");


                if ( isCredentialsValid && isSubscriptionActive) // www.text.Contains("success")
                {
                    // print("Count Setting: " + splittedLoginData[1]);
                    FileManager.SaveSetting("Login Check Number", splittedLoginData[1]); // Save current check setting
                    SaveLoginInfo();
                    CheckSystemUser();

                    OpenMenu();
                }
                else if(!isSubscriptionActive )
                {
                    string message = "Your subscription is currently inactive. Please check your billing details.";
                    DisplayCRM(message); // Display message
                }
                else // offline login 
                {
                    RunOfflineLogin();
                }

            }
        }

    }

    private void OfflineCRMFallback()
    {
        if (offlineEmailFound == false)
        {
            crm_response.text = "<b>Email not registered or incorrect.</b> \n" +
                    "\n" +
                    "Please check your details, your Internet connection and try again, or request " +
                    "authorisation from the developer. \n";
            crm_panel.SetActive(true);
        }
    }

    private void EmptySpaceCRMFallback()
    {
        crm_response.text = "<b>Input space empty.</b> \n \n" +
                "Please enter login details.";
        crm_panel.SetActive(true);
    }

    private void AccessDeniedCRMFallback()
    {
        crm_response.text = "<b>Access Denied.</b>\n" +
            "\n" +
            "Please connect to the internet for your login authorisation, or request authorisation from " +
            "the developer. \n";
        crm_panel.SetActive(true);
    }

    private void IncorrectInputsCRMFallBack()
    {
        crm_response.text = "<b>Incorrect Email or Password.</b> \n" + "Please insert correct login details.";
        crm_panel.SetActive(true);
    }

    private void ConnectionErrorCRMFallBack()
    {
        crm_response.text = "- error connecting to the internet.";
        crm_panel.SetActive(true);
    }

    private void DisplayCRM(string message)
    {
        crm_response.text = message;
        crm_panel.SetActive(true);
    }

    [Obsolete] //IMPORTANT, DO NOT REMOVE
    void RunOfflineLogin()
    {
        // print("Test: Offline Login");
        try
        {
            string[] offlineData = GetOfflineLoginData();

            if (email.text == "")
            {
                EmptySpaceCRMFallback(); // If email or password space is empty
                return;
            }

            foreach (string credential_line in offlineData) // run through stored offline data line by line
            {
                if (credential_line.Contains(email.text)) // check if line contains email
                {
                    string[] credentials = credential_line.Split(":");
                    offlineEmailFound = true;

                    // Index 0 - Stored email
                    // Index 1 - Stored corresponding password

                    if (email.text == credentials[0] && password.text == credentials[1])
                    {
                        //SceneManager.LoadScene("calibration");
                        OpenMenu();
                        isLoggedIn = true;
                        // print("Offline Login");
                    }
                    else if ((password.text != credentials[1]) || (email.text != credentials[0]))
                    {
                        IncorrectInputsCRMFallBack(); // If email or password is incorrect
                    }
                }

            }

        }
        catch (Exception e)
        {
            ConnectionErrorCRMFallBack(); // If there is an execution error from offline code
        }

        OfflineCRMFallback(); // If offline login is not succesful

    }

    private void OpenMenu()
    {
        bool isLoginOpen = CheckLoginStatus(); // Check login status

        if(this.GetComponent<DropDown>().IsAdmin(email.text))
        {
            print("Subscription check skipped.");
            SceneManager.LoadScene("MainMenu");
        }
        else if (!isLoginOpen)
        {
            print("Login: " + loginCounter);
            return;
        }
        else
        {
            print("Login: " + loginCounter);
            // Load Menu scenario
            SceneManager.LoadScene("MainMenu");
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
            FileManager.SaveSetting("Login Counter", "0"); // Reset login Counter
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


        loginDate = FileManager.ReadSetting("Log Date");
        // print("Login date: " + loginDate);

        if (loginDate != "0" && loginDate != null)
        {
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


                if (accessNumber >= num_authorised_days )
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
        }
        else
        {
            writeLastLoginDate();
        }

        isAuthorised = true; // Temporarily disabled
    }
    private int chechActiveDays()
    {
        int accessNumber = 0;

        try
        {
            string[] data = (FileManager.ReadSetting("Access").Split(','));
            accessNumber = int.Parse(data[1]);
        }
        catch
        {
            // Pass
        }

        return accessNumber;
    }

    public bool CheckLoginStatus()
    {
        bool loginStatus = false;
        loginCounter = int.Parse(FileManager.ReadSetting("Login Counter")); // Get current count
        string message = "Unable to connect to remote server, please connect to the internet.";

        if (int.Parse(FileManager.ReadSetting("Login Check Number")) == 0) // If login counter is not setup
        {
            // loginStatus = true;
            if(isSuperAdmin)
            {
                loginStatus = true;
            }
            else
            {
                DisplayCRM(message); // Display message
            }
        }
        else if (loginCounter < int.Parse(FileManager.ReadSetting("Login Check Number")))
        {
            loginCounter+= 1; // Increment the login counter
            FileManager.SaveSetting("Login Counter", loginCounter.ToString()); // Update login counter in the settings

            loginStatus = true;
        }
        else
        {
            // Display login countout response
            DisplayCRM(message); // Display message
        }

        return loginStatus;
    }

    private void writeLastLoginDate()
    {
        FileManager.SaveSetting("Log Date", currentDate.ToString());
        // print("RE: Date Stored");
    }
    private void resetAccessNumber()
    {

        FileManager.SaveSetting("Access", "access,0");
        //print("RE: Access number restored");
    }
    private void incriAccessNumber(int num)
    {
        FileManager.SaveSetting("Access", "access," + num.ToString());
    }
    void SaveCredentials()
    {
        
        Encryption encrypt = new Encryption();
        //string base64 = encrypt.AESEncryption(email.text + ":" + password.text + ":" + IPFinder.loginMacAddress);//encrypt username and password
        string login_credentials = email.text + ":" + password.text;

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