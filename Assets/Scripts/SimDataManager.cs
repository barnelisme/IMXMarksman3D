using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class SimDataManager : MonoBehaviour
{
    string configurationPath = "Assets/Resources/Configuration.txt";
    string sourceFolderPath = "Assets/Resources/";

    void Start()
    {
        CheckAndCopyFilesBasedOnInstallation();
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

}
