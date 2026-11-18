 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Diagnostics;
using UnityEngine.UI;

public class MenuSettingsManager : MonoBehaviour
{
    public Canvas mainMenu;
    public GameObject quitMenu;
    public GameObject Exit_Menu;
    public TMP_Dropdown typeOfGun;
    public TMP_Dropdown ammoSetting;
    public TMP_Dropdown paperSetting;
    public TMP_Dropdown simType;
    public TMP_InputField PlayerID;
    public GameObject mainCamera;
    public string activeScene = "";
    public Image AC_Light;
    public Image Battery_Light;

    //string[] File;
    string gunPath = "Assets/Resources/gun.txt";
    string fileData = "";

    //global time variables
    public GameObject global_time_warning;
    bool warningActivated = false;
    private bool isSceneManagerCalled = false;

    public void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;

        if(activeScene.ToLower().Contains("main"))
        {
            if (FileManager.ReadSetting("Ammo Setting") != "0") Scoring.ammo_setting = FileManager.ReadSetting("Ammo Setting");
            if (FileManager.ReadSetting("Paper Setting") != "0") Scoring.shooting_PaperRoll_Setting = FileManager.ReadSetting("Paper Setting");
            if (FileManager.ReadSetting("Sim Setting") != "0") Scoring.simulation_type = FileManager.ReadSetting("Sim Setting");

            ammoSetting.options.Add(new TMP_Dropdown.OptionData() { text = "_Ammo Type_" });
            ammoSetting.options.Add(new TMP_Dropdown.OptionData() { text = "Live Ammo" });
            ammoSetting.options.Add(new TMP_Dropdown.OptionData() { text = "Infrared Laser" });
            ammoSetting.options.Add(new TMP_Dropdown.OptionData() { text = "Redeye Laser" });

            //paperSetting
            //paperSetting.options.Add(new TMP_Dropdown.OptionData() { text = "___SELECT___" });
            paperSetting.options.Add(new TMP_Dropdown.OptionData() { text = "Static" });
            paperSetting.options.Add(new TMP_Dropdown.OptionData() { text = "Dynamic" });


            //simType.options.Add(new TMP_Dropdown.OptionData() { text = "___SELECT___" });
            simType.options.Add(new TMP_Dropdown.OptionData() { text = "Practice" });
            simType.options.Add(new TMP_Dropdown.OptionData() { text = "Training" });

            SendAmmoSetting(); // Send signal to the camera to alight settings.
            SetPowerSettings(); // Read power settings from file and set the lights accordingly
        }

    }
    void Update()
    {
        //Scoring.trainee_id = PlayerID.text;
        //rotateMainCamera();

        if (isSceneManagerCalled) loadSceneManager(); // Keep is true until scenarios are set

        manageGlobalActiveTime();
    }

    private void manageGlobalActiveTime()
    {
        login_Manager.global_active_timer -= Time.deltaTime * 1;

        if (login_Manager.global_active_timer <= 5 && warningActivated == false)
        {
            //global_time_warning.SetActive(true);
            warningActivated = true;
        }
        if (login_Manager.global_active_timer <= 0)
        {
            SceneManager.LoadScene("LOGIN");
        }
    }
    public void CreateFile()
    {

        if (!File.Exists(gunPath))
        {
            File.Create(gunPath).Close();
            
        }

    }
    public static void Manip(string[] args)
    {
        string text = "C# is a fun programming language";

        // split string 
        string[] result = text.Split(" ");

        Console.Write("Result: ");
        foreach (String str in result)
        {
            Console.Write(str + ", ");
        }
        Console.ReadLine();
    }
    private void ReadFile()
    {
        using (StreamReader reader = new StreamReader(gunPath))
        {
            while (!reader.EndOfStream) // reading the file while we haven't reched the END
            {
                //keep reading
                fileData = reader.ReadLine();
            }

            string text = fileData;

            // split string 
            string[] result = text.Split(",");

            WriteStringFile("100000");
            //Console.Write("Result: ");
            foreach (String str in result)
            {
                Console.Write(str + ", ");
            }
            Console.ReadLine();

        }

    }
    private void WriteStringFile(string text)
    {
        using (StreamWriter writer = new StreamWriter(gunPath))
        {
            writer.WriteLine(text);
        }
    }
    public void LogOut()
    {
        mainMenu.enabled = false;
        quitMenu.SetActive(true);

    }
    public void QuitBtn()
    {
        mainMenu.enabled = false;
        Exit_Menu.SetActive(true);
    }
    public void EX_Yes_button()
    {
        Application.Quit();
    }
    public void Ex_No_button()
    {
        mainMenu.enabled = true;
        Exit_Menu.SetActive(false);
    }
    public void Ybutton()
    {
        SceneManager.LoadScene("LOGIN");
    }
    public void Nbutton()
    {
        mainMenu.enabled = true;
        quitMenu.SetActive(false);
    }
    public void load_Calib_Acc_Page()
    {
        SceneManager.LoadScene("Calib_Acc_Page");
    }
    
    public void loadSceneManager()
    {
        if (DropDown.isScenarioListSet) // Open scenario menu when scenareos are loaded successfully
            SceneManager.LoadScene("SceneManager");

        isSceneManagerCalled = true;
    }
    public void loadHubScore()
    {
        Process.Start(new ProcessStartInfo("https://www.hubscorex.com/signin") { UseShellExecute = true });
    }
    public void loadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void HandleInputData(int val)
    {
        print(val);
        switch (val)
        {
            case 1:
                Scoring.gun = "Handgun";
   
                break;
            case 2:
                Scoring.gun = "Riffle";

                break;
            case 3:
                Scoring.gun = "Short gun";
                break;

            case 4:
                Scoring.gun = "CZ 75 B omega";

                break;
            case 5:
                Scoring.gun = "Glock 42 slimline";

                break;
            case 6:
                Scoring.gun = "Taurus G2C";
                break;
            case 7:
                Scoring.gun = "Ruger 9mms LCP";
                break;
            case 8:
                Scoring.gun = "CO2 Hand Gun";
                break;


        }

        if(val != 0 || val != 1 || val!= 2)
        {
            val = 1;
        }
        

    }
    public void HandleAmmoSetting(int val)
    {
        // print(val);
        switch (val)
        {
            case 1:
                Scoring.ammo_setting = "Live";
                break;
            case 2:
                Scoring.ammo_setting = "Infrared Laser";
                break;
            case 3:
                Scoring.ammo_setting = "Laser";
                break;
        }

        FileManager.SaveSetting("Ammo Setting", Scoring.ammo_setting);
        SendAmmoSetting();

        // print(Scoring.ammo_setting);
    }

    private void SendAmmoSetting()
    {
        UDPSetup.FindLocalIP();
        if (Scoring.ammo_setting.ToLower().Contains("live"))
        {
            this.transform.gameObject.GetComponent<UDPSetup>().sendEndless("Live");
        }
        else if (Scoring.ammo_setting.ToLower().Contains("laser"))
        {
            this.transform.gameObject.GetComponent<UDPSetup>().sendEndless("Laser");
        }

    }

    public void HandlePaperRollSettiing(int val)
    {
        //print(val);
        switch (val)
        {
            case 0:
                Scoring.shooting_PaperRoll_Setting = "Static";
                break;
            case 1:
                Scoring.shooting_PaperRoll_Setting = "Dynamic";
                break;

        }

        FileManager.SaveSetting("Paper Setting", Scoring.shooting_PaperRoll_Setting);
    }
    private void rotateMainCamera()
    {
        mainCamera.transform.Rotate(Vector3.up, 4 * Time.deltaTime);
    }
    public void HandleSimInputData(int val)
    {
        switch (val)
        {
            case 0:
                Scoring.simulation_type = "practice";
                break;

            case 1:
                Scoring.simulation_type = "training";
                break;


        }

        FileManager.SaveSetting("Sim Setting", Scoring.simulation_type);
    }

    private void SetPowerSettings()
    {
        if (FileManager.ReadSetting("Power Setting") == "AC Power")
        {
            SetAcPower();
        }
        else if (FileManager.ReadSetting("Power Setting") == "Battery Power")
        {
            SetBatteryPower();
        }
        else
        {
            AC_Light.color = Color.gray;
            Battery_Light.color = Color.gray;
        }
    }

    public void SetAcPower()
    {
        if(AC_Light != null)
        {
            AC_Light.color = Color.green;
        }
        if(Battery_Light != null)
        {
            Battery_Light.color = Color.black;
        }
        FileManager.SaveSetting("Power Setting", "AC Power");
        this.transform.gameObject.GetComponent<UDPSetup>().sendEndless("Power.AC");
    }

    public void SetBatteryPower()
    {  
        if(AC_Light != null)
        {
            AC_Light.color = Color.black;
        }
        if(Battery_Light != null)
        {
            Battery_Light.color = Color.green;
        }
        FileManager.SaveSetting("Power Setting", "Battery Power");
        this.transform.gameObject.GetComponent<UDPSetup>().sendEndless("Power.Battery");
    }

}
