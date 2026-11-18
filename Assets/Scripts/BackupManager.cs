using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class BackupManager : MonoBehaviour
{
    private string OnlineBackupFolder = "Assets/Resources/Hubscore files/Online set";
    private string OfflineBackupFolder = "Assets/Resources/Hubscore files/Offline set";
    string Url = "";
    string range_scoring_endpoint = "";
    string lane_image_endpoint = "";

    WWWForm lane1_image_form;
    WWWForm lane2_image_form;
    WWWForm lane3_image_form;

    string[] lane_1_trainee_Data;
    string lane_1_trainee_id = "";
    string lane_1_trainee_name = "";
    string lane_1_user_under = "";
    string lane_1_instructor_n = "";
    string lane_1_gun_type = "";
    string lane_1_trainee_email = "";
    string lane_1_created_at = "";
    string lane_1_user_under_email = "";
    string lane_1_training_time = "";

    string[] lane_2_trainee_Data;
    string lane_2_trainee_id = "";
    string lane_2_trainee_name = "";
    string lane_2_user_under = "";
    string lane_2_instructor_n = "";
    string lane_2_gun_type = "";
    string lane_2_trainee_email = "";
    string lane_2_created_at = "";
    string lane_2_user_under_email = "";
    string lane_2_training_time = "";

    string[] lane_3_trainee_Data;
    string lane_3_trainee_id = "";
    string lane_3_trainee_name = "";
    string lane_3_user_under = "";
    string lane_3_instructor_n = "";
    string lane_3_gun_type = "";
    string lane_3_trainee_email = "";
    string lane_3_created_at = "";
    string lane_3_user_under_email = "";
    string lane_3_training_time = "";

    string lane1_image_key = "";
    string lane2_image_key = "";
    string lane3_image_key = "";

    private float checkTime = 0;
    private float setCheckTime = 2;

    private void Start()
    {

        Url = configuration.ConfigAllUrls["server_url"];
        range_scoring_endpoint = configuration.ConfigAllEndpoints["range_scoring_endpoint"];
        lane_image_endpoint = configuration.ConfigAllEndpoints["lane_image_endpoint"];

        CheckBackupFolder();
        checkTime = setCheckTime;
        StartCoroutine(StartBackupManager());
    }

    private void Update()
    {

    }

    public IEnumerator StartBackupManager()
    {
        bool isInternetAvailable = false;
        yield return StartCoroutine(CheckInternetConnection((bool isConnected) =>
        {
            isInternetAvailable = isConnected;
        }));

        if(isInternetAvailable)
        {
            StartOnlineBackup();
            StartOfflineBackup();
            // StartCoroutine(StartOfflineBackup());
        }
    }

    private void StartOnlineBackup()
    {
        string[] files = Directory.GetFiles(OnlineBackupFolder, "*.json");

        foreach (var file in files)
        {
            string json = File.ReadAllText(file);
            bool handled = false;

            // Try to parse as Score backup
            try
            {
                ScoreDataBackup scoreData = JsonUtility.FromJson<ScoreDataBackup>(json);

                if (scoreData != null && scoreData.JsonPayload != null && !string.IsNullOrEmpty(scoreData.Url))
                {
                    SendScoreDataAndCleanup(scoreData.JsonPayload, scoreData.Url, file);
                    handled = true;
                }
            }
            catch { }

            // If not score, try to parse as Image backup
            if (!handled)
            {
                try
                {
                    ImageDataBackup imageData = JsonUtility.FromJson<ImageDataBackup>(json);
                    if (imageData != null && imageData.ImageBytes != null && !string.IsNullOrEmpty(imageData.Url))
                    {
                        StartCoroutine(SendImageFormAndCleanup(imageData.ImageBytes, imageData.ImageKey, imageData.Url, file));
                        handled = true;
                    }
                }
                catch { }
            }

            if (!handled)
            {
                Debug.LogWarning($"[Backup] Could not parse backup file: {file}");
            }
        }
    }


    private void StartOfflineBackup()
    {
        string[] files = Directory.GetFiles(OfflineBackupFolder, "*.json");
        GetTrainees traineeManager = new GetTrainees();
        // yield return StartCoroutine(traineeManager.getTrainees());

        Url = configuration.ConfigAllUrls["server_url"];
        range_scoring_endpoint = configuration.ConfigAllEndpoints["range_scoring_endpoint"];
        lane_image_endpoint = configuration.ConfigAllEndpoints["lane_image_endpoint"];

        if(GetTrainees.trainingNamesList.Count > 0)
        {
            lane1_image_form = new WWWForm();
            lane2_image_form = new WWWForm();
            lane3_image_form = new WWWForm();

            GetTrainees.TraineeLane_1 = GetTrainees.trainingNamesList[0];
            GetTrainees.TraineeLane_2 = GetTrainees.trainingNamesList[1];
            GetTrainees.TraineeLane_3 = GetTrainees.trainingNamesList[2];

            lane_1_trainee_Data = GetTrainees.TraineeLane_1.Split(":");
            lane_2_trainee_Data = GetTrainees.TraineeLane_2.Split(":");
            lane_3_trainee_Data = GetTrainees.TraineeLane_3.Split(":");

            lane_1_trainee_id = lane_1_trainee_Data[0];
            lane_1_trainee_name = lane_1_trainee_Data[1];
            lane_1_user_under = lane_1_trainee_Data[2];
            lane_1_instructor_n = lane_1_trainee_Data[3];
            lane_1_gun_type = lane_1_trainee_Data[4];
            lane_1_trainee_email = lane_1_trainee_Data[5];
            lane_1_created_at = lane_1_trainee_Data[6];
            lane_1_user_under_email = lane_1_trainee_Data[7];

            lane_2_trainee_id = lane_2_trainee_Data[0];
            lane_2_trainee_name = lane_2_trainee_Data[1];
            lane_2_user_under = lane_2_trainee_Data[2];
            lane_2_instructor_n = lane_2_trainee_Data[3];
            lane_2_gun_type = lane_2_trainee_Data[4];
            lane_2_trainee_email = lane_2_trainee_Data[5];
            lane_2_created_at = lane_2_trainee_Data[6];
            lane_2_user_under_email = lane_2_trainee_Data[7];

            lane_3_trainee_id = lane_3_trainee_Data[0];
            lane_3_trainee_name = lane_3_trainee_Data[1];
            lane_3_user_under = lane_3_trainee_Data[2];
            lane_3_instructor_n = lane_3_trainee_Data[3];
            lane_3_gun_type = lane_3_trainee_Data[4];
            lane_3_trainee_email = lane_3_trainee_Data[5];
            lane_3_created_at = lane_3_trainee_Data[6];
            lane_3_user_under_email = lane_3_trainee_Data[7];

            int allowedIterations = 10;
            int iterationCount = 0;
            foreach (var file in files)
            {
                bool handled = false;
                string json = File.ReadAllText(file);
                Lane1Backup lane1ScoreData = JsonUtility.FromJson<Lane1Backup>(json);

                string number_of_lanes = lane1ScoreData.LaneHeader.Split("*")[2];
                string backup_email = lane1ScoreData.Email;

                if (login_Manager.EmailText == backup_email)
                {
                    switch (number_of_lanes)
                    {
                        case "1":
                            Process1LaneData(file);
                            break;
                        case "2":
                            Process2LaneData(file);
                            break;
                        case "3":
                            Process3LaneData(file);
                            break;
                    }
                }
                else
                {
                    // print("Email not matching...");
                    // Pass. Do not process.
                }
                // Try to parse as Score backup


                if (!handled)
                {
                    Debug.LogWarning($"[Backup] Could not parse backup file: {file}");
                }

                iterationCount++;
                if(iterationCount >= allowedIterations) // Limit the backup to only 10 files incase there are more files saved
                {
                    break; 
                }
            }
        }
        else
        {
            // print("No trainees");
        }

    }

    private void Process1LaneData(string file)
    {
        string json = File.ReadAllText(file);
        Lane1Backup lane1ScoreData = JsonUtility.FromJson<Lane1Backup>(json);

        string[] lane_header = lane1ScoreData.LaneHeader.Split("*");
        string numLanes = lane_header[2];
        string exercise_name = lane_header[3];
        string time = lane_header[4];

        byte[] lane1_image_byte = lane1ScoreData.Lane1ImageBytes;

        string[] lane1_data = lane1ScoreData.Lane1Data.Split("*");
        string lane1_percentage = lane1_data[0];
        string lane1_shots_hit = lane1_data[1];
        string lane1_shots_missed = lane1_data[2];
        string lane1_splittime = lane1_data[3];
        string lane1_reactiontime = lane1_data[4];
        lane_1_training_time = lane1_data[5];


        lane1_image_key = lane_1_user_under_email + "_" + lane_1_trainee_id + "_" + exercise_name + "_" + time + "_" + lane_1_trainee_name;

        lane1_image_key = ReplaceCharacter(lane1_image_key, ' ', '-');

        // Prepare JSON payload
        Dictionary<string, object> jsonData_1 = new Dictionary<string, object>
                {
                    { "status", "success" },
                    { "message", "Data inserted" },
                    { "num_lanes", numLanes },
                    { "scenario_type", exercise_name },
                    { "time", time }
                };

        // Create dictionaries for each lane
        Dictionary<string, string> lane1_dictionary = new Dictionary<string, string>
                {
                    { "trainee", lane_1_trainee_name },
                    { "trainee_id", lane_1_trainee_id },
                    { "trainee_percentage", lane1_percentage },
                    { "target_hits", lane1_shots_hit },
                    { "target_missed", lane1_shots_missed },
                    { "training_time", lane_1_training_time },
                    { "user_under", lane_1_user_under },
                    { "instructor", lane_1_instructor_n },
                    { "weapon", lane_1_gun_type },
                    { "split_time", lane1_splittime },
                    { "reaction_time", lane1_reactiontime },
                    { "user_under_email", lane_1_user_under_email },
                    { "trainee_email", lane_1_trainee_email },
                    { "created_at", lane_1_created_at },
                    { "image_key", lane1_image_key}
                };
            
        // Process score data
        jsonData_1.Add("lane_1", lane1_dictionary); // Add Lane1 Data
        string jsonString = JsonConvert.SerializeObject(jsonData_1);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
        string payLoad = System.Text.Encoding.UTF8.GetString(bodyRaw);

        StartCoroutine(SendScoreData(bodyRaw, Url + range_scoring_endpoint)); // Send score data HTTP request
        StartCoroutine(SendImageFormAndCleanup(lane1_image_byte, lane1_image_key, Url + lane_image_endpoint, file)); // send image data form request

        // print("JSON Payload: " + payLoad);
    }

    private void Process2LaneData(string file)
    {
        string json = File.ReadAllText(file);
        Lane2Backup lane2ScoreData = JsonUtility.FromJson<Lane2Backup>(json);

        string[] lane_header = lane2ScoreData.LaneHeader.Split("*");
        string numLanes = lane_header[2];
        string exercise_name = lane_header[3];
        string time = lane_header[4];

        byte[] lane1_image_byte = lane2ScoreData.Lane1ImageBytes;
        byte[] lane2_image_byte = lane2ScoreData.Lane2ImageBytes;

        string[] lane1_data = lane2ScoreData.Lane1Data.Split("*");
        string lane1_percentage = lane1_data[0];
        string lane1_shots_hit = lane1_data[1];
        string lane1_shots_missed = lane1_data[2];
        string lane1_splittime = lane1_data[3];
        string lane1_reactiontime = lane1_data[4];
        lane_1_training_time = lane1_data[5];

        string[] lane2_data = lane2ScoreData.Lane2Data.Split("*");
        string lane2_percentage = lane2_data[0];
        string lane2_shots_hit = lane2_data[1];
        string lane2_shots_missed = lane2_data[2];
        string lane2_splittime = lane2_data[3];
        string lane2_reactiontime = lane2_data[4];
        lane_2_training_time = lane2_data[5];


        lane1_image_key = lane_1_user_under_email + "_" + lane_1_trainee_id + "_" + exercise_name + "_" + time + "_" + lane_1_trainee_name;
        lane2_image_key = lane_2_user_under_email + "_" + lane_2_trainee_id + "_" + exercise_name + "_" + time + "_" + lane_2_trainee_name;

        lane1_image_key = ReplaceCharacter(lane1_image_key, ' ', '-');
        lane2_image_key = ReplaceCharacter(lane2_image_key, ' ', '-');

        // Prepare JSON payload
        Dictionary<string, object> jsonData_1 = new Dictionary<string, object>
                {
                    { "status", "success" },
                    { "message", "Data inserted" },
                    { "num_lanes", numLanes },
                    { "scenario_type", exercise_name },
                    { "time", time }
                };

        // Create dictionaries for each lane
        Dictionary<string, string> lane1_dictionary = new Dictionary<string, string>
                {
                    { "trainee", lane_1_trainee_name },
                    { "trainee_id", lane_1_trainee_id },
                    { "trainee_percentage", lane1_percentage },
                    { "target_hits", lane1_shots_hit },
                    { "target_missed", lane1_shots_missed },
                    { "training_time", lane_1_training_time },
                    { "user_under", lane_1_user_under },
                    { "instructor", lane_1_instructor_n },
                    { "weapon", lane_1_gun_type },
                    { "split_time", lane1_splittime },
                    { "reaction_time", lane1_reactiontime },
                    { "user_under_email", lane_1_user_under_email },
                    { "trainee_email", lane_1_trainee_email },
                    { "created_at", lane_1_created_at },
                    { "image_key", lane1_image_key}
                };

        // Create dictionaries for each lane
        Dictionary<string, string> lane2_dictionary = new Dictionary<string, string>
                {
                    { "trainee", lane_2_trainee_name },
                    { "trainee_id", lane_2_trainee_id },
                    { "trainee_percentage", lane2_percentage },
                    { "target_hits", lane2_shots_hit },
                    { "target_missed", lane2_shots_missed },
                    { "training_time", lane_2_training_time },
                    { "user_under", lane_2_user_under },
                    { "instructor", lane_2_instructor_n },
                    { "weapon", lane_2_gun_type },
                    { "split_time", lane2_splittime },
                    { "reaction_time", lane2_reactiontime },
                    { "user_under_email", lane_2_user_under_email },
                    { "trainee_email", lane_2_trainee_email },
                    { "created_at", lane_2_created_at },
                    { "image_key", lane2_image_key}
                };

        // Process score data
        jsonData_1.Add("lane_1", lane1_dictionary); // Add Lane1 Data
        jsonData_1.Add("lane_2", lane2_dictionary); // Add Lane2 Data

        string jsonString = JsonConvert.SerializeObject(jsonData_1);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
        string payLoad = System.Text.Encoding.UTF8.GetString(bodyRaw);


        StartCoroutine(SendScoreDataAndCleanup(bodyRaw, Url + range_scoring_endpoint, file)); // Send score data HTTP request
        StartCoroutine(SendImageFormAndCleanup(lane1_image_byte, lane1_image_key, Url + lane_image_endpoint, file)); // send image data form request

        print("JSON Payload: " + payLoad);

    }

    private void Process3LaneData(string file)
    {
        string json = File.ReadAllText(file);
        Lane3Backup lane3ScoreData = JsonUtility.FromJson<Lane3Backup>(json);

        string[] lane_header = lane3ScoreData.LaneHeader.Split("*");
        string numLanes = lane_header[2];
        string exercise_name = lane_header[3];
        string time = lane_header[4];

        byte[] lane1_image_byte = lane3ScoreData.Lane1ImageBytes;
        byte[] lane2_image_byte = lane3ScoreData.Lane2ImageBytes;
        byte[] lane3_image_byte = lane3ScoreData.Lane3ImageBytes;

        string[] lane1_data = lane3ScoreData.Lane1Data.Split("*");
        string lane1_percentage = lane1_data[0];
        string lane1_shots_hit = lane1_data[1];
        string lane1_shots_missed = lane1_data[2];
        string lane1_splittime = lane1_data[3];
        string lane1_reactiontime = lane1_data[4];
        lane_1_training_time = lane1_data[5];

        string[] lane2_data = lane3ScoreData.Lane2Data.Split("*");
        string lane2_percentage = lane2_data[0];
        string lane2_shots_hit = lane2_data[1];
        string lane2_shots_missed = lane2_data[2];
        string lane2_splittime = lane2_data[3];
        string lane2_reactiontime = lane2_data[4];
        lane_2_training_time = lane2_data[5];

        string[] lane3_data = lane3ScoreData.Lane3Data.Split("*");
        string lane3_percentage = lane3_data[0];
        string lane3_shots_hit = lane3_data[1];
        string lane3_shots_missed = lane3_data[2];
        string lane3_splittime = lane3_data[3];
        string lane3_reactiontime = lane3_data[4];
        lane_3_training_time = lane3_data[5];


        lane1_image_key = lane_1_user_under_email + "_" + lane_1_trainee_id + "_" + exercise_name + "_" + time + "_" + lane_1_trainee_name;
        lane2_image_key = lane_2_user_under_email + "_" + lane_2_trainee_id + "_" + exercise_name + "_" + time + "_" + lane_2_trainee_name;
        lane3_image_key = lane_3_user_under_email + "_" + lane_3_trainee_id + "_" + exercise_name + "_" + time + "_" + lane_3_trainee_name;

        lane1_image_key = ReplaceCharacter(lane1_image_key, ' ', '-');
        lane2_image_key = ReplaceCharacter(lane2_image_key, ' ', '-');
        lane3_image_key = ReplaceCharacter(lane3_image_key, ' ', '-');

        // Prepare JSON payload
        Dictionary<string, object> jsonData_1 = new Dictionary<string, object>
                {
                    { "status", "success" },
                    { "message", "Data inserted" },
                    { "num_lanes", numLanes },
                    { "scenario_type", exercise_name },
                    { "time", time }
                };


        // Create dictionaries for each lane
        Dictionary<string, string> lane1_dictionary = new Dictionary<string, string>
                {
                    { "trainee", lane_1_trainee_name },
                    { "trainee_id", lane_1_trainee_id },
                    { "trainee_percentage", lane1_percentage },
                    { "target_hits", lane1_shots_hit },
                    { "target_missed", lane1_shots_missed },
                    { "training_time", lane_1_training_time },
                    { "user_under", lane_1_user_under },
                    { "instructor", lane_1_instructor_n },
                    { "weapon", lane_1_gun_type },
                    { "split_time", lane1_splittime },
                    { "reaction_time", lane1_reactiontime },
                    { "user_under_email", lane_1_user_under_email },
                    { "trainee_email", lane_1_trainee_email },
                    { "created_at", lane_1_created_at },
                    { "image_key", lane1_image_key}
                };

        // Create dictionaries for each lane
        Dictionary<string, string> lane2_dictionary = new Dictionary<string, string>
                {
                    { "trainee", lane_2_trainee_name },
                    { "trainee_id", lane_2_trainee_id },
                    { "trainee_percentage", lane2_percentage },
                    { "target_hits", lane2_shots_hit },
                    { "target_missed", lane2_shots_missed },
                    { "training_time", lane_2_training_time },
                    { "user_under", lane_2_user_under },
                    { "instructor", lane_2_instructor_n },
                    { "weapon", lane_2_gun_type },
                    { "split_time", lane2_splittime },
                    { "reaction_time", lane2_reactiontime },
                    { "user_under_email", lane_2_user_under_email },
                    { "trainee_email", lane_2_trainee_email },
                    { "created_at", lane_2_created_at },
                    { "image_key", lane2_image_key}
                };

        // Create dictionaries for each lane
        Dictionary<string, string> lane3_dictionary = new Dictionary<string, string>
                {
                    { "trainee", lane_3_trainee_name },
                    { "trainee_id", lane_3_trainee_id },
                    { "trainee_percentage", lane3_percentage },
                    { "target_hits", lane3_shots_hit },
                    { "target_missed", lane3_shots_missed },
                    { "training_time", lane_3_training_time },
                    { "user_under", lane_3_user_under },
                    { "instructor", lane_3_instructor_n },
                    { "weapon", lane_3_gun_type },
                    { "split_time", lane3_splittime },
                    { "reaction_time", lane3_reactiontime },
                    { "user_under_email", lane_3_user_under_email },
                    { "trainee_email", lane_3_trainee_email },
                    { "created_at", lane_3_created_at },
                    { "image_key", lane3_image_key}
                };

        // Process score data
        jsonData_1.Add("lane_1", lane1_dictionary); // Add Lane1 Data
        jsonData_1.Add("lane_2", lane2_dictionary); // Add Lane1 Data
        jsonData_1.Add("lane_3", lane3_dictionary); // Add Lane1 Data

        string jsonString = JsonConvert.SerializeObject(jsonData_1);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
        string payLoad = System.Text.Encoding.UTF8.GetString(bodyRaw);


        StartCoroutine(SendScoreDataAndCleanup(bodyRaw, Url + range_scoring_endpoint, file)); // Send score data HTTP request
        StartCoroutine(SendImageFormAndCleanup(lane1_image_byte, lane1_image_key, Url + lane_image_endpoint, file)); // send image data form request

        print("JSON Payload: " + payLoad);

    }


    // Wrap score sending + cleanup
    private IEnumerator SendScoreDataAndCleanup(byte[] payload, string url, string filePath)
    {
        print("Point 2 reached...");
        yield return StartCoroutine(SendScoreData(payload, url));

        // If successful, delete the file
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"[Backup] Cleaned up file after upload: {filePath}");
        }
    }

    // Wrap image sending + cleanup
    private IEnumerator SendImageFormAndCleanup(byte[] imageBytes, string imageKey, string url, string filePath)
    {
        yield return StartCoroutine(SendImageForm(imageBytes, imageKey, url));

        // If successful, delete the file
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            // Debug.Log($"[Backup] Cleaned up file after upload: {filePath}");
        }
    }

    private IEnumerator SendScoreData(byte[] pay_load, string url_endpoint)
    {
        using (UnityWebRequest www = new UnityWebRequest(url_endpoint, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(pay_load);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Data sent successfully: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error sending data for lane: " + www.error);
            }
        }
    }

    public IEnumerator SendImageForm(byte[] imageByte, string image_key, string url_endpoint)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("image_blob", imageByte, "image1.png", "image/png");
        form.AddField("image_key", image_key);

        // Create the UnityWebRequest to send the POST request
        using (UnityWebRequest www = UnityWebRequest.Post(url_endpoint, form))
        {
            // Send the request and wait for a response
            yield return www.SendWebRequest();

            // Check if there were any errors
            if (www.result == UnityWebRequest.Result.Success)
            {
                // If the request was successful, print the response
                Debug.Log("Response: " + www.downloadHandler.text);
            }
            else
            {
                // If there was an error, print the error message
                Debug.LogError("Request failed: " + www.error);
            }

        }
    }

    public void CheckBackupFolder()
    {
        if (!Directory.Exists(OnlineBackupFolder))
            Directory.CreateDirectory(OnlineBackupFolder);
        if (!Directory.Exists(OfflineBackupFolder))
            Directory.CreateDirectory(OfflineBackupFolder);
    }

    // Save JSON + Image info into a file
    public void SaveImageBackupFile(byte[] imageBytes, string imageKeys, string url_endpoint)
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
        string filePath = Path.Combine(OnlineBackupFolder, $"backup_{timestamp}.json");

        ImageDataBackup data = new ImageDataBackup
        {
            ImageBytes = imageBytes,
            ImageKey = imageKeys,
            Url = url_endpoint
        };

        string fileContent = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, fileContent);

        Debug.Log($"[Backup] Saved backup file: {filePath}");
    }

    public void SaveScoreBackupFile(byte[] jsonPayload, string url_endpoint)
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
        string filePath = Path.Combine(OnlineBackupFolder, $"backup_{timestamp}.json");

        ScoreDataBackup data = new ScoreDataBackup
        {
            JsonPayload = jsonPayload,
            Url = url_endpoint
        };

        string fileContent = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, fileContent);

        Debug.Log($"[Backup] Saved backup file: {filePath}");
    }

    public void BackupLane1OfflineData(string email, string lane_header, string lane1_data, byte[] lane1_image)
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
        string filePath = Path.Combine(OfflineBackupFolder, $"backup_{timestamp}.json");

        Lane1Backup data = new Lane1Backup
        {
            Email = email,
            LaneHeader = lane_header,
            Lane1Data = lane1_data,
            Lane1ImageBytes = lane1_image
        };

        string fileContent = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, fileContent);

        Debug.Log($"[Backup] Saved backup file: {filePath}");
    }

    public void BackupLane2OfflineData(string email, string lane_header, string lane1_data, string lane2_data, byte[] lane1_image, byte[] lane2_image)
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
        string filePath = Path.Combine(OfflineBackupFolder, $"backup_{timestamp}.json");

        Lane2Backup data = new Lane2Backup
        {
            Email = email,
            LaneHeader = lane_header,
            Lane1Data = lane1_data,
            Lane2Data = lane2_data,
            Lane1ImageBytes = lane1_image,
            Lane2ImageBytes = lane2_image
        };

        string fileContent = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, fileContent);

        Debug.Log($"[Backup] Saved backup file: {filePath}");
    }

    public void BackupLane3OfflineData(string email, string lane_header, string lane1_data, string lane2_data, string lane3_data,
        byte[] lane1_image, byte[] lane2_image, byte[] lane3_image)
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
        string filePath = Path.Combine(OfflineBackupFolder, $"backup_{timestamp}.json");

        Lane3Backup data = new Lane3Backup
        {
            Email = email,
            LaneHeader = lane_header,
            Lane1Data = lane1_data,
            Lane2Data = lane2_data,
            Lane3Data = lane3_data,
            Lane1ImageBytes = lane1_image,
            Lane2ImageBytes = lane2_image,
            Lane3ImageBytes = lane3_image
        };

        string fileContent = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, fileContent);

        Debug.Log($"[Backup] Saved backup file: {filePath}");
    }

    public IEnumerator CheckInternetConnection(System.Action<bool> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Head("https://www.google.com"))
        {
            request.timeout = 5;
            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            bool isConnected = !(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError);
#else
            bool isConnected = !(request.isNetworkError || request.isHttpError);
#endif

            callback(isConnected);
        }
    }

    string ReplaceCharacter(string originalString, char charToReplace, char replacementChar)
    {

        return originalString.Replace(charToReplace, replacementChar);
    }
}

// Helper class to store both JSON + image info
public class ScoreDataBackup
{
    public byte[] JsonPayload;
    public string Url;
}

public class ImageDataBackup
{
    public byte[] ImageBytes;
    public string ImageKey;
    public string Url;
}


public class Lane1Backup
{
    public string Email;
    public string LaneHeader;
    public byte[] Lane1ImageBytes;
    public string Lane1Data;
}


public class Lane2Backup
{
    public string Email;
    public string LaneHeader;
    public byte[] Lane1ImageBytes;
    public byte[] Lane2ImageBytes;
    public string Lane1Data;
    public string Lane2Data;
}


public class Lane3Backup
{
    public string Email;
    public string LaneHeader;
    public byte[] Lane1ImageBytes;
    public byte[] Lane2ImageBytes;
    public byte[] Lane3ImageBytes;
    public string Lane1Data;
    public string Lane2Data;
    public string Lane3Data;
}
