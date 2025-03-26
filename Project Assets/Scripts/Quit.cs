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

public class Quit : MonoBehaviour
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

    //string[] File;
    string gunPath = "Assets/Resources/gun.txt";
    string fileData = "";

    //global time variables
    public GameObject global_time_warning;
    bool warningActivated = false;

    public void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;

        if(activeScene.ToLower().Contains("main"))
        {
            //typeOfGun.options.Add(new TMP_Dropdown.OptionData() { text = "___SELECT___" });
            //typeOfGun.options.Add(new TMP_Dropdown.OptionData() { text = "Handgun" });
            //typeOfGun.options.Add(new TMP_Dropdown.OptionData() { text = "Riffle" });
            //typeOfGun.options.Add(new TMP_Dropdown.OptionData() { text = "Short gun" });
            //typeOfGun.options.Add(new TMP_Dropdown.OptionData() { text = "CZ 75 B omega" });
            //typeOfGun.options.Add(new TMP_Dropdown.OptionData() { text = "Glock 42 slimline" });
            //typeOfGun.options.Add(new TMP_Dropdown.OptionData() { text = "Taurus G2C" });
            //typeOfGun.options.Add(new TMP_Dropdown.OptionData() { text = "Ruger 9mms LCP" });
            //typeOfGun.options.Add(new TMP_Dropdown.OptionData() { text = "CO2 Hand Gun" });


            ammoSetting.options.Add(new TMP_Dropdown.OptionData() { text = "_Ammo Type_" });
            ammoSetting.options.Add(new TMP_Dropdown.OptionData() { text = "Live Ammo" });
            ammoSetting.options.Add(new TMP_Dropdown.OptionData() { text = "Redeye Laser" });
            ammoSetting.options.Add(new TMP_Dropdown.OptionData() { text = "Infrared Laser" });
            //gameSetting.options.Add(new TMP_Dropdown.OptionData() { text = "GAME Shoot" });
            //scoring.setting == "normal"

            //paperSetting
            //paperSetting.options.Add(new TMP_Dropdown.OptionData() { text = "___SELECT___" });
            paperSetting.options.Add(new TMP_Dropdown.OptionData() { text = "Static" });
            paperSetting.options.Add(new TMP_Dropdown.OptionData() { text = "Dynamic" });


            //simType.options.Add(new TMP_Dropdown.OptionData() { text = "___SELECT___" });
            simType.options.Add(new TMP_Dropdown.OptionData() { text = "Practice" });
            simType.options.Add(new TMP_Dropdown.OptionData() { text = "Training" });

            if (Scoring.ammo_setting == " ")
            {
                //Scoring.ammo_setting = "Laser";
            }
        }

    }
    void Update()
    {
        //Scoring.trainee_id = PlayerID.text;
        //rotateMainCamera();
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
        SceneManager.LoadScene("SceneManager");
    }
    public void loadOmp()
    {
        Process.Start(new ProcessStartInfo("https://www.imx-omp.com/signin") { UseShellExecute = true });
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
    public void HandleInputDataSettiing(int val)
    {
        print(val);
        switch (val)
        {
            case 1:
                Scoring.ammo_setting = "Live";
                break;
            case 2:
                Scoring.ammo_setting = "Laser";
                break;
            case 3:
                Scoring.ammo_setting = "Laser.Infrared";
                break;
        }
        print(Scoring.ammo_setting);
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

        if(val == 1)
        {
            print("Simulation type is: Training");
        }
        else if(val == 2)
        {
            print("Simulation type is: Test");
        }
    }

}
