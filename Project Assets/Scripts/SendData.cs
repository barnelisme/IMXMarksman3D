using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SendData : MonoBehaviour
{
    static string Url= "http://192.168.137.1/exercise/activescene.php";
    string dataToSend;
    string field;
    string traineeID, tName, ExerciseName, Instrucor, dTime, NUmCivilian, numEnem, precisePer, numRounds;
    Text errorMessage;

    /*public InputField email;
    public InputField password;
    public Text errorMessage;*/
    // Start is called before the first frame update
    void Start()
    {
    }
    public void setData()
    { 
        StartCoroutine(UploadScore());
    }
     IEnumerator UploadScore()
    {
        WWWForm form= new WWWForm();

        form.AddField("tarinee_id", Scoring.trainee_id);
        form.AddField("trainee_name", Scoring.trainee_name);
        form.AddField("exercise_name", Scoring.exercise_name);
        form.AddField("instructor", Scoring.instructor);
        form.AddField("date", System.DateTime.Now.ToString());
        form.AddField("number_of_civilian_hit", Scoring.number_of_civilian_hit);
        form.AddField("number_of_enemy_hit", Scoring.number_of_enemy_hit);
        form.AddField("precision_percentage", Scoring.precision_percentage);
        form.AddField("number_of_rounds_used", Scoring.number_of_rounds_used);


        WWW www = new WWW(Url, form);
        //yield return www.SendWebRequest();
        yield return www;
        print("Response:" + www.text);
        if (www.error != null)// != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (www.text.Contains("success"))
            {
                Debug.Log("Success");
            }      
            else
            {
                errorMessage.text = "Invalid password or username";
            }
        }
        www.Dispose();
    }
}



