using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GetTrainees : MonoBehaviour
{
    string Url = "http://192.168.137.1/imagistix/datareq.php";
    string get_trainees_endpoint;
    static public string trainee_id;
    static public string trainee_name;
    public TMP_InputField playerID;
    string[] traineesNames;
    public static List<string> trainingNamesList = new List<string>();
    string activeScene = " ";
    public bool destroy = false;
    static public string TraineeLane_1 = "";
    static public string TraineeLane_2 = "";
    static public string TraineeLane_3 = "";
    public GameObject lane_1;
    public GameObject lane_2;
    public GameObject lane_3;

    // Start is called before the first frame update
    private void Start()
    {
        trainingNamesList = new List<string>();
        resetTime();
        activeScene = SceneManager.GetActiveScene().name;
        try
        {
            if (GetSystemUpdate.currentVersion.Contains("Enterprise"))
            {
                StartCoroutine(getTrainees());
            }
            //StartTraineesProcess();
        }
        catch(Exception e)
        {
            print(e.StackTrace);
        }

    }

    private void resetTime()
    {
        Time.timeScale = 1f;
    }

    void Update()
    {
       
    }

    public void StartTraineesProcess()
    {
       
    }

    public void HandleInputData(int val)
    {

        if (activeScene.ToLower().Contains("range"))
        {
            if (val == 1)
            {
                //TraineeLane_1 = "John";
            }
            TraineeLane_1 = trainingNamesList[val ];
            print("Trainee 1 is: " + TraineeLane_1);
            lane_1.GetComponent<TextMesh>().text = TraineeLane_1;
        }
        else
        {
            print("get trainees val " + val);
            trainee_name = trainingNamesList[val - 1];
            
        }
        
    }

    public void HandleInputData2(int val)
    {

        if (activeScene.ToLower().Contains("range"))
        {
            if (val == 1)
            {
                //TraineeLane_2 = "Lilly";
            }
            TraineeLane_2 = trainingNamesList[val ];
            print("Trainee 2 is: " + TraineeLane_2);
            lane_2.GetComponent<TextMesh>().text = TraineeLane_2;
        }
        else
        {
            print("get trainees val " + val);
            trainee_name = trainingNamesList[val - 1];
            print(trainee_name);
        }


    }

    public void HandleInputData3(int val)
    {

        if (activeScene.ToLower().Contains("range"))
        {
            if (val == 1)
            {
                //TraineeLane_3 = "Nhlanhla";
            }
            TraineeLane_3 = trainingNamesList[val];
            print("Trainee 3 is: " + TraineeLane_3);
            lane_3.GetComponent<TextMesh>().text = TraineeLane_3;
        }
        else
        {
            print("get trainees val " + val);
            trainee_name = trainingNamesList[val - 1];
            print(trainee_name);
        }

    }

    public IEnumerator getTrainees()
    {
        Url = configuration.ConfigAllUrls["server_url"];
        get_trainees_endpoint = configuration.ConfigAllEndpoints["get_trainees_endpoint"];
        WWWForm form = new WWWForm();
        string data;

        form.AddField("email", login_Manager.EmailText);
        //print("RRE: login email is " + login_Manager.EmailText);
        WWW www = new WWW(Url+ get_trainees_endpoint, form);
        yield return www;
        data = www.text;
        //print("RE: The Trainees Data is: " + data);
        //print("RE: Receiving data from: " + Url + get_trainees_endpoint);

        if (data.ToLower().Contains(";"))
        {
            traineesNames = data.Split(';');
            populateDropDown();
        }
        www.Dispose();
    }

    void populateDropDown()
    {

        string[] traineeData = new string[10];
        if(activeScene.ToLower().Contains("range"))
        {
            for (int i = 1; i < traineesNames.Length - 1; i++)
            {
                print(traineesNames[i]);
                trainingNamesList.Add(traineesNames[i]);

            }
            //print("the Trainees list adding on the dropdown");
        }
        else //Does not contain range
        {
            for (int i = 1; i < traineesNames.Length - 1; i++)
            {
                //print(traineesNames[i]);
                trainingNamesList.Add(traineesNames[i]);

            }
        }
        StaticVariableManager.set_trainees = true;
    }

    public void destroyDropdownObjects()
    {
        //print("I am in now, Code Suucesful... and deleted");
        //Destroy(trainees.gameObject);
        //Destroy(trainees_2.gameObject);
        //Destroy(trainees_3.gameObject);
    }
}

