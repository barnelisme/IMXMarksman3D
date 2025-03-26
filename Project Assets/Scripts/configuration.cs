using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class configuration : MonoBehaviour
{
    string[] configurations = {" "};
    static private Dictionary<string, string> configAllUrls = new Dictionary<string, string>();
    static private Dictionary<string, string> configAllEndpoints = new Dictionary<string, string>();
    string configurationPath = "Assets/Resources/Configuration.txt";
    string sourceFolderPath = "Assets/Resources/";

    string activeScene = "";

    // Start is called before the first frame update
    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        if (activeScene.ToLower().Contains("login"))
            CheckAndCopyFilesBasedOnInstallation();
            //CopyConfigurationFiles();

        LoadConfiguration();
    }

    // Update is called once per frame
    void Update()
    {
        if(StaticVariableManager.transfer_sim_data)
        {
            CheckAndCopyFilesBasedOnInstallation();
            StaticVariableManager.transfer_sim_data = false;
        }
    }

    void CheckAndCopyFilesBasedOnInstallation()
    {
        string installationFilePath = Path.Combine(sourceFolderPath, "installation.txt");

        // Step 1: Read the contents of the installation.txt file
        if (File.Exists(installationFilePath))
        {
            string installationData = File.ReadAllText(installationFilePath).Trim();

            // Step 3: Get two steps back directory
            string appDirectory = Application.dataPath;
            string twoStepsBackDirectory = Directory.GetParent(Directory.GetParent(appDirectory).FullName).FullName;

            // Define the "IMX Data Files" and "Sim Data" directories
            string imxDataFilesDirectory = Path.Combine(twoStepsBackDirectory, "IMX Data Files");
            string simDataFolder = Path.Combine(imxDataFilesDirectory, "Sim Data");

            // Step 2: Check if the data is "1"
            if (installationData == "1")
            {

                // Step 4: Check if "IMX Data Files" and "Sim Data" exist
                if (Directory.Exists(simDataFolder))
                {
                    // If Sim Data folder exists, copy its contents to the source folder
                    CopyDirectoryContents(simDataFolder, sourceFolderPath);
                }
                else
                {
                    // If Sim Data does not exist, create the folders and copy files from source folder
                    if (!Directory.Exists(imxDataFilesDirectory))
                    {
                        Directory.CreateDirectory(imxDataFilesDirectory);
                        Debug.Log("Created folder: " + imxDataFilesDirectory);
                    }

                    // Create the Sim Data folder
                    Directory.CreateDirectory(simDataFolder);
                    Debug.Log("Created folder: " + simDataFolder);

                    // Copy files from source folder to Sim Data folder
                    CopyDirectoryContents(sourceFolderPath, simDataFolder);
                }

                // Step 5: Rewrite the contents of installation.txt to "NA"
                File.WriteAllText(installationFilePath, "NA");
                Debug.Log("Installation file updated to 'NA'.");
            }
            else // data is not "1"
            {
                // If Sim Data does not exist, create the folders and copy files from source folder
                if (!Directory.Exists(imxDataFilesDirectory))
                {
                    Directory.CreateDirectory(imxDataFilesDirectory);
                }
                if (!Directory.Exists(simDataFolder))
                {
                    Directory.CreateDirectory(simDataFolder);
                }

                // Copy files from source folder to Sim Data folder
                CopyDirectoryContents(sourceFolderPath, simDataFolder);
            }
        }
        else
        {
            Debug.LogError("installation.txt not found in the source folder.");
        }
    }

    // Helper method to copy contents from one directory to another
    void CopyDirectoryContents(string sourceDirectory, string destinationDirectory)
    {
        // Get all files in the source directory
        string[] files = Directory.GetFiles(sourceDirectory);

        // Ensure destination directory exists
        if (!Directory.Exists(destinationDirectory))
        {
            Directory.CreateDirectory(destinationDirectory);
        }

        // Copy all files from source to destination
        foreach (string file in files)
        {
            string destFile = Path.Combine(destinationDirectory, Path.GetFileName(file));
            File.Copy(file, destFile, true); // Overwrite existing files
            //Debug.Log($"Copied: {file} to {destFile}");
        }
    }


    void CopyConfigurationFiles()
    {
        // Get the directory of the application
        string appDirectory = Application.dataPath;

        // Get two steps back from the Assets folder
        string twoStepsBackDirectory = Directory.GetParent(Directory.GetParent(appDirectory).FullName).FullName;

        // Define the destination directory paths
        string imxDataFilesFolder = Path.Combine(twoStepsBackDirectory, "IMX Data Files");
        string simDataFolder = Path.Combine(imxDataFilesFolder, "Sim Data");
        StaticVariableManager.main_data_directory = simDataFolder;
        print("RE: main directory is " + StaticVariableManager.main_data_directory);

        // Ensure the IMX Data Files folder exists
        if (!Directory.Exists(imxDataFilesFolder))
        {
            Directory.CreateDirectory(imxDataFilesFolder);
            Debug.Log("Created folder: " + imxDataFilesFolder);
        }

        // Ensure the Sim Data folder exists
        if (!Directory.Exists(simDataFolder))
        {
            Directory.CreateDirectory(simDataFolder);
            Debug.Log("Created folder: " + simDataFolder);
        }

        // Ensure the source folder exists
        if (Directory.Exists(sourceFolderPath))
        {
            try
            {
                // Get all the files in the source folder
                string[] files = Directory.GetFiles(sourceFolderPath);

                foreach (string file in files)
                {
                    // Define the destination file path
                    string destFile = Path.Combine(simDataFolder, Path.GetFileName(file));

                    // Copy each file from the source folder to the destination
                    File.Copy(file, destFile, true);
                    //Debug.Log($"File copied: {file} to {destFile}");
                }

                //Debug.Log("All files copied successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError("Error while copying files: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Source folder not found: " + sourceFolderPath);
        }
    }

    private void LoadConfigData()
    {
        configurations[configurations.Length - 1] = "server_url=https://www.imx-omp.com";    //https://www.imx-omp.com //http://127.0.0.1:80 //Add Data
        Array.Resize(ref configurations, configurations.Length + 1);                                                       //Add Memory Space
        configurations[configurations.Length - 1] = "stress_vest_url=192.168.0.255";                 //Add Data
        Array.Resize(ref configurations, configurations.Length + 1);                                                       //Add Memory Space
        configurations[configurations.Length - 1] = "login_endpoint=/imx/login/login.php";             //Add Data
        Array.Resize(ref configurations, configurations.Length + 1);                                                       //Add Memory Space
        configurations[configurations.Length - 1] = "scoring_endpoint=/imx/trainees/setTrainees.php";     //Add Data
        Array.Resize(ref configurations, configurations.Length + 1);                                                       //Add Memory Space
        configurations[configurations.Length - 1] = "get_trainees_endpoint=/imx/trainees/getTrainees.php";    //Add Data
        Array.Resize(ref configurations, configurations.Length + 1);                                                       //Add Memory Space
        configurations[configurations.Length - 1] = "range_scoring_endpoint=/imx/trainees/setTrainees.php";  //Add Data
        Array.Resize(ref configurations, configurations.Length + 1);                                                       //Add Memory Space
        configurations[configurations.Length - 1] = "lane_image_endpoint=/imx/trainees/setImageRes.php";  //Add Data
        Array.Resize(ref configurations, configurations.Length + 1);                                                       //Add Memory Space
        configurations[configurations.Length - 1] = "get_allowed_scenes_endpoint=/imx/scenarios/allowed_scenarios.php";//Add Data
        Array.Resize(ref configurations, configurations.Length + 1);                                                       //Add Memory Space
        configurations[configurations.Length - 1] = "get_update_endpoint=/imx/comms/get_comms.php";    //Add Data
    }
    private void LoadConfiguration()
    {
        
        if (!File.Exists(configurationPath))
        {
            File.Create(configurationPath);

        }
        else
        {
            //Debug.Log("configurations Found");
            //configurations = System.IO.File.ReadAllLines(configurationPath);
            LoadConfigData();

            foreach (string line in configurations)
            {
                //get urls
                try
                {
                    if (line.Contains("_url"))
                    {
                        configAllUrls.Add(line.Split('=')[0], line.Split('=')[1]);
                        //Debug.Log("url found key:" + line.Split('=')[0] + " is " + configAllUrls[line.Split('=')[0]]);
                    }
                    else if (line.Contains("_endpoint"))
                    {
                        configAllEndpoints.Add(line.Split('=')[0], line.Split('=')[1]);
                        //Debug.Log("endpoint found key:" + line.Split('=')[0] + " is " + configAllEndpoints[line.Split('=')[0]]);
                    }
                }
                catch(Exception e)
                {
                    //Debug.Log(e.Message);
                }
            }
        }
    }
    static public Dictionary<string, string> ConfigAllUrls
    {
        get { return configAllUrls; }
        
    }
    static public Dictionary<string, string> ConfigAllEndpoints
    {
        get { return configAllEndpoints; }

    }

}
