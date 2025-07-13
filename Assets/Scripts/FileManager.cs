using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    public static string gamepath = "Assets/Resources/";

    public static void CreateFile(string filename)
    {
        string initial_data = "";

        // Create directory and file
        if (!Directory.Exists(gamepath))
        {
            Directory.CreateDirectory(gamepath);
            File.Create(gamepath + filename);
        }
        else
        {
            if (!File.Exists(gamepath + filename))
            {
                File.Create(gamepath + filename);
            }
        }
    }
    public static void WriteDataToFile(string filepath, string data)
    {
        string path = gamepath + filepath;
       // print("data to write:"+data);

        System.IO.File.WriteAllText(@path, data);

    }
    public static void WriteLineDataToFile(string filepath, string data)
    {
        string path = gamepath + filepath;

        // Ensure the file exists before reading it
        if (!System.IO.File.Exists(path))
        {
            System.IO.File.WriteAllText(path, data + Environment.NewLine);
            return;
        }

        // Read all existing lines from the file
        string[] lines = System.IO.File.ReadAllLines(path);
        if (!lines.Contains(data))
        {
            System.IO.File.AppendAllText(path, data + Environment.NewLine);
        }
    }

    public static string ReadFromFile(string Filepath)
    {
        string file_content = "";
        string path = gamepath + Filepath;

        // Ensure the file exists
        if (!File.Exists(path))
        {
            File.WriteAllText(path, ""); // Create empty file
        }
        else
        {
            using (StreamReader reader = new StreamReader(gamepath + Filepath))
            {
                while (!reader.EndOfStream) // reading the file while we haven't reched the END
                {
                    file_content = file_content + reader.ReadLine();
                }
            }
        }

        //print("Content of file:" + file_content);
        return file_content;
        
    }
    public static string[] ReadLinesFromFile(string Filepath)
    {
        string[] file_content = new string[20];

        file_content = System.IO.File.ReadAllLines(gamepath + Filepath);

        //print("Content of file:" + file_content);
        return file_content;
    }

    public static void ClearDataInFile(string filename)
    {
        string path = gamepath + filename;

        if (File.Exists(gamepath + filename))
        {
            System.IO.File.WriteAllText(@path, "");
        }
    }

    public static void incriptToFile(string filepath, string data)
    {
        Encryption encrypt = new Encryption();
        string base64 = encrypt.AESEncryption(data);
        string path = gamepath + filepath;

        System.IO.File.WriteAllText(@path, base64);
    }
    public static string decryptFile(string Filepath)
    {
        string file_content = "";

        using (StreamReader reader = new StreamReader(gamepath + Filepath))
        {
            while (!reader.EndOfStream) // reading the file while we haven't reched the END
            {
                file_content = file_content + reader.ReadLine();
            }
        }

        string base64 = file_content;
        Encryption encrypt = new Encryption();
        file_content = (encrypt.AESDecryption(base64)).ToString();

        return file_content;

    }

    // CODE UPDATE
    public static string ReadSetting(string key)
    {
        string path = gamepath + "Settings.txt";

        // Ensure the file exists
        if (!File.Exists(path))
        {
            File.WriteAllText(path, ""); // Create empty file
            return "0";
        }

        try
        {
            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {

                if (line.StartsWith(key + ":"))
                {
                    int index = line.IndexOf(":") + 1;
                    string rawValue = line.Substring(index).Trim();

                    if (key == "Installation" || key == "System User")
                    {
                        return rawValue; // No decryption for Installation
                    }
                    else
                    {
                        Encryption encrypt = new Encryption();
                        string decrypted = encrypt.AESDecryption(rawValue);
                        return decrypted;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[ReadSetting] Error while reading key '{key}': {e.Message}");
        }

        return "0"; // Not found or error
    }

    public static void SaveSetting(string key, string newData)
    {
        string path = gamepath + "Settings.txt";

        // Ensure the file exists
        if (!File.Exists(path))
        {
            File.WriteAllText(path, ""); // Create empty file
        }

        try
        {
            string toSave = newData;
            if (key != "Installation" && key != "System User")
            {
                Encryption encrypt = new Encryption();
                toSave = encrypt.AESEncryption(newData); // Encrypt other keys
            }

            string[] lines = File.ReadAllLines(path);
            bool keyFound = false;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(key + ":"))
                {
                    lines[i] = key + ":" + toSave;
                    keyFound = true;
                    break;
                }
            }

            if (!keyFound)
            {
                List<string> updatedLines = new List<string>(lines);
                updatedLines.Add(key + ":" + toSave);
                lines = updatedLines.ToArray();
            }

            File.WriteAllLines(path, lines);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSetting] Error while saving key '{key}': {e.Message}");
        }
    }



}