using System;
using System.Diagnostics;
using UnityEngine;
using TMPro;

public class HardwareIdReader : MonoBehaviour
{
    string exePath = "Assets/Plugins/HardwareInfoReader.exe";
    static string uniqueIdPath = "System.Lock.txt";
    private bool isIDSet = false;
    public GameObject IDResetUI;
    public TMP_InputField IDRCodeInput;
    public TMP_Text IDRErrorText;

    private float errorTimer = 1f;
    private bool isCodeError = false;

    void Start()
    {
        GetSystemID();
    }

    private void Update()
    {
        if(!isIDSet)
        {
            try
            {
                SaveSystemID();
                
            }
            catch(Exception e)
            {
                // Repeat try untill the ID is saved
            }
        }

        if(isCodeError)
        {
            errorTimer -= Time.deltaTime;
            if(errorTimer <= 0f)
            {
                IDRErrorText.gameObject.SetActive(false);
                isCodeError = false;
            }

        }
        else
        {
            errorTimer = 1f;
        }

    }

    private void GetSystemID()
    {
        Process process = new Process();
        process.StartInfo.FileName = exePath;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        string[] lines = output.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        string cpuId = lines.Length > 0 ? lines[0] : "CPU ID not found";
        string biosId = lines.Length > 1 ? lines[1] : "BIOS Version not found";
        string boardId = lines.Length > 2 ? lines[2] : "Motherboard Product not found";
        string uuid = lines.Length > 3 ? lines[3] : "UUID not found";
        // string tpmId = lines.Length > 3 ? lines[3] : "TPM ID not found";

        StaticVariableManager.currentUniqueSystemID = $"{boardId}|{biosId}|{cpuId}";

        // print(StaticVariableManager.currentUniqueSystemID);
    }

    private void SaveSystemID()
    {
        FileManager.CreateFile(uniqueIdPath);
        string file_read_ID = FileManager.ReadFromFile(uniqueIdPath);
        // print(stored_unique_id);

        if (file_read_ID == "") // If empty
        {
            SaveID(); // Save ID
            StaticVariableManager.isSystemUnlocked = true;
            isIDSet = true;
        }
        else
        {
            StaticVariableManager.isSystemUnlocked = CheckSystemLock(file_read_ID);
            isIDSet = true;
        }
    }

    public static void SaveID()
    {
        FileManager.WriteDataToFile(uniqueIdPath, StaticVariableManager.currentUniqueSystemID.ToString());
    }

    public void ResetID()
    {
        if(IDRCodeInput.text == "IMX-RC01")
        {
            FileManager.ClearDataInFile(uniqueIdPath);
            IDResetUI.SetActive(false);
            IDRCodeInput.text = null;

            //Reset
            GetSystemID();
            SaveSystemID();
            print("ID Reset!!");
        }
        else
        {
            IDRErrorText.gameObject.SetActive(true);
            isCodeError = true;
        }
    }

    public void OpenIDResetUI()
    {
        IDResetUI.SetActive(true);
    }

    public void CloseIDResetUI()
    {
        IDResetUI.SetActive(false);
        IDRCodeInput.text = null; // Reset input.
    }

    private bool CheckSystemLock(string fileRead)
    {
        bool isUnlocked = false;
        string[] curentID = StaticVariableManager.currentUniqueSystemID.Split("|");
        string[] storedId = fileRead.Split("|");

        // NOTE:
        // index 0 - Board ID
        // index 1 - Bios ID
        // index 0 - CPU ID

        if(storedId[0] == curentID[0]) // Board ID comparison
        {
            isUnlocked = true;
        }
        else if(storedId[1] == curentID[1] || storedId[2] == curentID[2]) // CPU and BIOS ID comparison
        {
            isUnlocked = true;
            SaveID(); // to replace Board ID
        }

        // print("is unlocked: " + isUnlocked);

        return isUnlocked;
    }

}

/*
         if (stored_unique_id == "") // if empty
        {
            FileManager.WriteDataToFile(uniqueIdPath, "IMX.AssignID"); // Setup starting point
        }
        else if (stored_unique_id == "IMX.AssignID")
        {
            SaveID(); // Save ID
            StaticVariableManager.savedUniqueSystemID = StaticVariableManager.currentUniqueSystemID;
            isIDSet = true;
        }
        else
        {
            // Read Unique ID
            StaticVariableManager.savedUniqueSystemID = stored_unique_id;
        }
 */