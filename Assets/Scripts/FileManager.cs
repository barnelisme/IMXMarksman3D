using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    public static string gamepath = "Assets/Resources/";

    void Start()
    {

    }

    void Update()
    {
        
    }
    public static void WriteString()
    {
        string path = Application.persistentDataPath + "/calibration.txt";
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("Test");
        writer.Close();
        StreamReader reader = new StreamReader(path);
        //Print the text from the file
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }
    public static void ReadString()
    {
        string path = Application.persistentDataPath + "/test.txt";
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }


    public static void CreateFile(string filename)
    {

        if (!Directory.Exists(gamepath))
        {
            Directory.CreateDirectory(gamepath);
            File.Create(gamepath + filename);
            //Debug.Log("Resources created and calibration needs to be done");
            incriptToFile(filename, "0");
        }
        else
        {
            if (!File.Exists(gamepath + filename))
            {
                File.Create(gamepath + filename);
                incriptToFile(filename, "0");
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
        using (StreamReader reader = new StreamReader(gamepath + Filepath))
        {
            while (!reader.EndOfStream) // reading the file while we haven't reched the END
            {
                file_content = file_content + reader.ReadLine();
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

}