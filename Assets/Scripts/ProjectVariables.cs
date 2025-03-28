using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using System.Threading;
using TMPro;
using System.Globalization;
using UnityStandardAssets.Characters.FirstPerson;

public class ProjectVariables : MonoBehaviour
{
    //ARRAYS
    public string[] guns;
    public string gunsTextPath = "Assets/Resources/guns.txt";

    // Start is called before the first frame update
    void Start()
    {
        CreateFile();
        ReadFile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateFile()
    {
        if (!File.Exists(gunsTextPath))
        {
            File.Create(gunsTextPath).Close();
        }

    }
    private void ReadFile()
    {
        /*using (StreamReader reader = new StreamReader(gunsTextPath))
        {
            while (!reader.EndOfStream) // reading the file while we haven't reched the END
            {
                //keep reading
                //fileData = reader.ReadLine();
            }
        }*/

        guns = File.ReadAllLines(gunsTextPath);
        foreach(string line in guns)
        {
            print(line);
        }
    }
    private void WriteFile(float val)
    {
        using (StreamWriter writer = new StreamWriter(gunsTextPath))
        {
            writer.WriteLine(val);
        }
        ReadFile();
    }

}
