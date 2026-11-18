using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    public static string gamePath = "Assets/Resources/";

    public static void CreateFile(string filename)
    {
        string initial_data = "";

        // Create directory and file
        if (!Directory.Exists(gamePath))
        {
            Directory.CreateDirectory(gamePath);
            File.Create(gamePath + filename);
        }
        else
        {
            if (!File.Exists(gamePath + filename))
            {
                File.Create(gamePath + filename);
            }
        }
    }
    public static void WriteDataToFile(string filepath, string data)
    {
        string path = gamePath + filepath;
        // print("data to write:"+data);
        ClearDataInFile(path);

        System.IO.File.WriteAllText(@path, data);

    }
    public static void WriteLineDataToFile(string filepath, string data)
    {
        string path = gamePath + filepath;

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

    public static string[] ReadLinesFromFile(string Filepath)
    {
        string[] file_content = new string[20];

        file_content = System.IO.File.ReadAllLines(gamePath + Filepath);

        //print("Content of file:" + file_content);
        return file_content;
    }

    public static void ClearDataInFile(string filename)
    {
        string path = gamePath + filename;

        if (File.Exists(gamePath + filename))
        {
            System.IO.File.WriteAllText(@path, "");
        }
    }

    public static string ReadFromFile(string Filepath)
    {
        string path = gamePath + Filepath;

        // Ensure file exists
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "");
            return "";
        }

        try
        {
            // Try decrypting first
            return decryptFile(Filepath);
        }
        catch
        {
            // If decrypt fails, read normally
            string file_content = "";

            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    file_content += reader.ReadLine();
                }
            }

            return file_content;
        }
    }

    public static void incriptToFile(string filepath, string data)
    {
        Encryption encrypt = new Encryption();
        string base64 = encrypt.AESEncryption(data);
        string path = gamePath + filepath;

        System.IO.File.WriteAllText(@path, base64);
    }

    public static string decryptFile(string Filepath)
    {
        string file_content = "";

        using (StreamReader reader = new StreamReader(gamePath + Filepath))
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


    public static string[] incriptToArray(string[] data)
    {
        // Combines the "Key:Value" entries using system newlines 
        string combinedData = string.Join(Environment.NewLine, data);

        Encryption encrypt = new Encryption();
        // LAYER 1 GLOBAL PAYLOAD ENCRYPTION
        string base64 = encrypt.AESEncryption(combinedData).ToString();

        // Package as single-element array to execute File.WriteAllLines cleanly
        return new string[] { base64 };
    }

    public static string[] decryptFileToArray(string relativePath)
    {
        string encrypted_content = "";
        string[] decrypted_lines = null;
        string fullPath = gamePath + relativePath;

        if (!File.Exists(fullPath)) return new string[0];

        using (StreamReader reader = new StreamReader(fullPath))
        {
            encrypted_content = reader.ReadToEnd();
        }

        try
        {
            Encryption encrypt = new Encryption();
            // LAYER 1 GLOBAL PAYLOAD DECRYPTION
            string decrypted_content = encrypt.AESDecryption(encrypted_content).ToString();

            decrypted_lines = decrypted_content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        }
        catch (Exception e)
        {
            Debug.LogError("Global Decryption Layer 1 failed, returning raw file lines: " + e.Message);
            decrypted_lines = encrypted_content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        }

        return decrypted_lines;
    }


    public static string ReadSetting(string key)
    {
        string fileName = "Settings.txt";
        string fullPath = gamePath + fileName;

        if (!File.Exists(fullPath))
        {
            File.WriteAllText(fullPath, "");
            return "0";
        }

        // Decrypt the outer shell file block structure (Layer 1 Decryption)
        string[] lines = decryptFileToArray(fileName);

        try
        {
            foreach (string line in lines)
            {
                if (line.StartsWith(key + ":"))
                {
                    int index = line.IndexOf(":") + 1;
                    string rawValue = line.Substring(index).Trim();

                    // Check for exceptional keys that ignore inner value encryption
                    if (key == "System User" || key == "Installation")
                    {
                        return rawValue;
                    }
                    else
                    {
                        // LAYER 2 INNER VALUE DECRYPTION
                        Encryption encrypt = new Encryption();
                        string doubleDecryptedValue = encrypt.AESDecryption(rawValue).ToString();
                        return doubleDecryptedValue;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[ReadSetting] Error while running second decryption layer on '{key}': {e.Message}");
        }

        return "0"; // Key not found or error
    }

    public static void SaveSetting(string key, string newData)
    {
        string fileName = "Settings.txt";
        string fullPath = gamePath + fileName;

        if (!File.Exists(fullPath))
        {
            File.WriteAllText(fullPath, "");
        }

        try
        {
            // Always read and decrypt the file into plain-text structural lines first
            string[] lines = decryptFileToArray(fileName);

            if (lines == null || (lines.Length == 1 && string.IsNullOrEmpty(lines[0])))
            {
                lines = new string[0];
            }

            // LAYER 2 INNER VALUE ENCRYPTION
            string encryptedValue = newData;
            if (key != "System User" && key != "Installation")
            {
                Encryption encrypt = new Encryption();
                encryptedValue = encrypt.AESEncryption(newData).ToString();
            }

            // Assemble the line with the inner layer now securely processed
            string newLineEntry = key + ":" + encryptedValue;
            bool keyFound = false;

            // Search the structure to see if we update an existing key
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(key + ":"))
                {
                    lines[i] = newLineEntry;
                    keyFound = true;
                    break;
                }
            }

            // Add the entry to the end of the data list if it's completely new
            if (!keyFound)
            {
                List<string> updatedLines = new List<string>(lines);
                updatedLines.Add(newLineEntry);
                lines = updatedLines.ToArray();
            }

            // LAYER 1 GLOBAL PAYLOAD ENCRYPTION (Encrypts the entire structure)
            string[] finalEncryptedBlock = incriptToArray(lines);

            // Commit the resulting double-encrypted dataset straight to the local file
            File.WriteAllLines(fullPath, finalEncryptedBlock);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSetting] Error while double-encrypting key '{key}': {e.Message}");
        }
    }


    public static void CopyImageToProject(string sourcePath)
    {
        string destinationFolder = Path.Combine(gamePath, "Background Images", "Training Background");

        if (!Directory.Exists(destinationFolder))
            Directory.CreateDirectory(destinationFolder);

        string fileName = Path.GetFileName(sourcePath);
        string destinationPath = Path.Combine(destinationFolder, fileName);

        File.Copy(sourcePath, destinationPath, true);
    }


}