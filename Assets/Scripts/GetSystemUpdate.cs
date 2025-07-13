using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.InputSystem;
using System.IO;

public class GetSystemUpdate : MonoBehaviour
{
    string Url = "http://192.168.137.1/imagistix/datareq.php";
    string comsTxtSavePath = "CommsDtf.txt";
    static string gamepath = "Assets/Resources/";

    string get_update_endpoint;
    public GameObject crm_response;
    public GameObject updateButton;
    public GameObject checkItOutButton;
    public TextMeshProUGUI crm_message;
    static public string currentVersion = "Enterprise"; //"Home.23.1.0";
    static string updateLink = "https://www.imagistix.co.za/";
    string[] comms;
    static bool crmDisplayed = false;

    string fileMessage = " ";

    void Start()
    {
        try
        {
            FileManager.CreateFile(comsTxtSavePath);
            StartCoroutine(getUpdate());
        }
        catch (Exception e)
        {
            print(e.StackTrace);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.uKey.isPressed)
        {
            crm_response.SetActive(true);
        }
    }

    IEnumerator getUpdate()
    {     
        Url = configuration.ConfigAllUrls["server_url"];
        get_update_endpoint = configuration.ConfigAllEndpoints["get_update_endpoint"];
        WWWForm form = new WWWForm();
        string data;

        form.AddField("email", login_Manager.EmailText);
        form.AddField("version", currentVersion);

        WWW www = new WWW(Url + get_update_endpoint, form);
        yield return www;
        data = www.text;
        //print("The Update Data is: " + data);
        bool containsCrmData = Regex.IsMatch(data, @"[a-zA-Z]");

        if (containsCrmData && !data.ToLower().Contains("error"))
        {
            comms = data.Split("*");
            if (!data.Contains("br") && !data.Contains("<!") && !crmDisplayed)
            {
                bool containsData = Regex.IsMatch(comms[1], @"[a-zA-Z]");
                LoadComsFromFile();

                if(fileMessage.ToLower().Contains(comms[1].ToLower()))
                {
                    //Do Not display
                    crm_response.SetActive(false);
                }
                else
                {
                    if (comms[0] == "update")
                    {
                        crm_message.text = comms[1];
                        updateLink = comms[2];

                        if (containsData)
                        {
                            checkItOutButton.SetActive(false);
                            updateButton.SetActive(true);
                            crm_response.SetActive(true);
                            crmDisplayed = true;
                        }
                    }
                    else if (comms[0] == "comm")
                    {
                        crm_message.text = comms[1];

                        if (containsData)
                        {
                            updateButton.SetActive(false);
                            checkItOutButton.SetActive(true);
                            crm_response.SetActive(true);
                            crmDisplayed = true;
                        }

                    }

                    writeComsToFile(comms[1]);
                }

            }
        }
        www.Dispose();
    }

    public static void CreateFile(string filename)
    {

        if (!Directory.Exists(gamepath))
        {
            Directory.CreateDirectory(gamepath);
            File.Create(gamepath + filename);
            FileManager.WriteDataToFile(gamepath + filename, "3cByUFSHOa0C97iMU7rflRfbimB9ttrS9c3fFDDIfrEiT5THQHSbOU6JEh0wTVhD");
            //Debug.Log("Resources created and calibration needs to be done");

        }
        else
        {
            if (!File.Exists(gamepath + filename))
            {
                File.Create(gamepath + filename);
                FileManager.WriteDataToFile(gamepath + filename, "3cByUFSHOa0C97iMU7rflRfbimB9ttrS9c3fFDDIfrEiT5THQHSbOU6JEh0wTVhD");
            }

        }

    }
    public void onButtonClickClose()
    {
        crm_response.SetActive(false);
        updateButton.SetActive(false);
    }
    public void onButtonClickLoadPage()
    {
        print("Loading page...");
        System.Diagnostics.Process.Start(updateLink);
        crm_response.SetActive(false);
        updateButton.SetActive(false);
        checkItOutButton.SetActive(false);
    }

    private void writeComsToFile(string val)
    {
        print("Encrypting Comms....");
        Encryption encrypt = new Encryption();
        string base64 = encrypt.AESEncryption(val);//encrypt username and password

        print("comms Data Encrypted:" + base64);
        FileManager.CreateFile(comsTxtSavePath);
        FileManager.WriteDataToFile(comsTxtSavePath, base64);
    }

    public void LoadComsFromFile()
    {

        try
        {
            string base64 = FileManager.ReadFromFile(comsTxtSavePath);

            Encryption encrypt = new Encryption();

            fileMessage = encrypt.AESDecryption(base64);                    //decryption code                  //decryption code
            //print("Acc Data is " + fileMessage);

        }
        catch (Exception e)
        {
            print("Error retrieving zero file:" + e);
            throw new Exception("First login");
        }

    }

}
