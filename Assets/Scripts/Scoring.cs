using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO.Compression;
using System.Text;



public class Scoring : MonoBehaviour
{
    //string Url = "http://192.168.0.123/imx/activescene.php";
    string Url = "http://127.0.0.1:8000/admin/scores";
    string scoring_endpoint;
    string range_scoring_endpoint;
    string lane_image_endpoint;
    //private string url="";
    string activeScene;
    string scoringPath = "Assets/Resources/activescenes.csv";
    static public string logPath = "Assets/Resources/logs.txt";
    public string[] calibrationPoints;
    public float[] AimingPoint = new float[5];//will save points for aiming (x axis,horizontally done)
    public float[] BreathingPoint = new float[5];//will save points for breathing (y axis,vertically done)
    public float[] GroupingPoint = new float[5];

    public float[] AvgAimingPoint = new float[5];//will save points for aiming (x axis,horizontally done)
    public float[] AvgBreathingPoint = new float[5];//will save points for breathing (y axis,vertically done)
    public float[] AvgGroupingPoint = new float[5];

    private float[] positionX;
    private float[] positionY;
    static public int[] scoreSheet = new int[5]; 
    private Transform target;
    static public string displayText, logs;
    Text scoring;
    static TextMesh Lane1, suggestionTexts, Lane3;
    string groupingScore = "";
    string aimingScore = "";
    string breathingScore = "";
    static public float elapsedTime = 0f;
    static public List<float> elapsedTimes = new List<float>();
    static private float averageShotTime = 0f;
    
    //string ID;

    static public List<float> averageShotTimes = new List<float>();
    static public List<float> elapsedTimeEachLane = new List<float>();
    static public Dictionary<int,List<float>> elapsedTimePerLane =  new Dictionary<int, List<float>>();

    static public List<double> averageAiming = new List<double>();
    static public List<float> SpreadingEachLane = new List<float>();
    static public Dictionary<int, List<float>> SpreadingPerLane = new Dictionary<int, List<float>>();

    static public List<double> averageVert = new List<double>();
    static public List<float> SpreadingEachLaneY = new List<float>();
    static public Dictionary<int, List<float>> SpreadingPerLaneY = new Dictionary<int, List<float>>();


    static public List<Vector2> positionXYgrouping = new List<Vector2>();
    static public string[] suggestX = new string[5];
    static public string[] suggestY = new string[5];
    string remarks = "";

    float minGrouping = 0.354f * 1.5f;//0.236f;//0.190f;
    float medGrouping = 0.423f * 1.5f;//0.282f;//0.236f;
    float maxGrouping = 0.656f * 1.5f;//0.328f;

    static private List<GameObject> bulletHoles = new List<GameObject>();


    static public string gun = "handgun",trainee_id=" ", trainee_name = " ", exercise_name = " ", instructor = " ", time = " ", number_of_civilian_hit = " ", number_of_enemy_hit = " ", precision_percentage = " ", number_of_rounds_used = " ";
    static public string ammo_setting = "infrared laser", totNumLanes = "1"; //"Infrared Laser"
    static public List<string> basic_lane_trainee_names, basic_scene_score, basic_scene_percentage, basic_scene_targets_hit, basic_scene_shots_missed, 
        basic_lane_split_times, basic_lane_training_times, basic_lane_response_times;
    static public string simulation_type = "training"; //Range = training
    static public string shooting_PaperRoll_Setting = "static"; //Range = training


    private BackupManager backupManager;


    // Start is called before the first frame update
    void Start()
    {
        /*AddComponent(this.GetComponent<FirstPersonController>());
        AddComponentMenu(this.GetComponent<ReceiveUdp>());*/
        backupManager = new BackupManager();
        backupManager.CheckBackupFolder();
        // backupManager.StartBackupManager();

        activeScene = SceneManager.GetActiveScene().name;
        elapsedTime = 0f;
        elapsedTimes = new List<float>();
        averageShotTime = 0f;


        StaticVariableManager.OMPScoreSent = false;
        ScoreFilesManager();

        elapsedTimePerLane.Clear();
        SpreadingPerLane.Clear();
        SpreadingPerLaneY.Clear();
        if (activeScene.ToLower().Contains("range"))
        {
            scoring = GameObject.Find("Player/Canvas Display 2/groupingScore").GetComponent<Text>();         
            for (int i = 0; i < 5; i++)
            {
                elapsedTimeEachLane.Add(0f);
                elapsedTimePerLane.Add(i, elapsedTimeEachLane);
                averageShotTimes.Add(0f);
                averageAiming.Add(0f);
                SpreadingEachLane.Add(0f);
                SpreadingPerLane.Add(i, new List<float>(0));

                averageVert.Add(0f);
                SpreadingEachLaneY.Add(0f);
                SpreadingPerLaneY.Add(i, new List<float>(0));
            }
        }
       
    }

    private void Update()
    {

        if (StaticVariableManager.isScoreLanesReady && StaticVariableManager.OMPScoreSent == false)
        {
            StartCoroutine(SendWrappedDataToServer());

            StaticVariableManager.OMPScoreSent = true;
        }
    }

    static public void ResetRange()
    {
        elapsedTimePerLane.Clear();
        SpreadingPerLane.Clear();
        SpreadingPerLaneY.Clear();
        // displayText = "";
        for (int i = 0; i < 3; i++)
        {
            scoreSheet[i] = 0;
            elapsedTimeEachLane.Add(0f);
            elapsedTimePerLane.Add(i, elapsedTimeEachLane);
            averageShotTimes.Add(0f);
            averageAiming.Add(0f);
            SpreadingEachLane.Add(0f);
            SpreadingPerLane.Add(i, new List<float>(0));

            averageVert.Add(0f);
            SpreadingEachLaneY.Add(0f);
            SpreadingPerLaneY.Add(i, new List<float>(0));
            suggestX[i] = "";
            suggestY[i] = "";
            try
            {
                if (SceneManager.GetActiveScene().name != "moving_targets_indoor_range")
                {
                    Lane1 = GameObject.Find("Lane_" + (i + 1)).GetComponent<TextMesh>();
                    if(Lane1 != null) Lane1.text = "Lane " + (i + 1);
                }
            }
            catch(Exception e)
            {
                // Debug.Log("Error when reseting range for text display:" + e.Message);
            }
        }
        Scoring.elapsedTime = 0;
        foreach (GameObject impactGo in bulletHoles)
        {
            Destroy(impactGo);
        }
        Shooting.bulletPoint1.Clear();
        Shooting.bulletPoint2.Clear();
        Shooting.bulletPoint3.Clear();


    }
    // Update is called once per frame

    private void ScoreFilesManager()
    {
        //FileManager
        if (!Directory.Exists("Assets/Resources"))
        {
            Directory.CreateDirectory("Assets/Resources");
            File.Create(scoringPath);
            InitialiseScore("tarinee_id", "trainee_name", "exercise_name", "instructor", "time", "number_of_civilian_hit", "number_of_enemy_hit", "precision_percentage", "number_of_rounds_used");
            Debug.Log("Empty Score line Created");
        }
        else
        {
            //Debug.Log("Resources Already exists");
            if (!File.Exists(scoringPath))
            {
                File.Create(scoringPath);
            }
           
        }
        if (!Directory.Exists("Assets/Resources"))
        {
            Directory.CreateDirectory("Assets/Resources");
            File.Create(logPath);
            //InitialiseScore("tarinee_id", "trainee_name", "exercise_name", "instructor", "time", "number_of_civilian_hit", "number_of_enemy_hit", "precision_percentage", "number_of_rounds_used");
            Debug.Log("Empty log file Created");
        }
        else
        {
            //Debug.Log("Resources Already exists");
            if (!File.Exists(logPath))
            {
                File.Create(logPath);
            }
          
        }
    }

    public void InitialiseScore(string trainee_id, string trainee_name, string exercise_name, string instructor, string time, string number_of_civilian_hit, string number_of_enemy_hit, string precision_percentage, string number_of_rounds_used)
    {

        System.IO.File.WriteAllText(@scoringPath, trainee_id + "," + trainee_name + "," + exercise_name + "," + instructor + "," + time + "," + number_of_civilian_hit + "," + number_of_enemy_hit + "," + precision_percentage + "," + number_of_rounds_used + Environment.NewLine);

    }

    public void SaveTrainingScore(string tarinee_ID, string trainee_Name, string exercise_Name, string Instructor, string Time, List<string> score, List<string> percentages, string numLanes, List<string> sceneTargetsHit, List<string> sceneShotsMissed, List<string> laneTraineeNames, List<string> laneSplitTimes, List<string> laneTrainingTime, List<string> laneResponseTimes)
    {
        trainee_id = tarinee_ID;
        trainee_name = trainee_Name;
        exercise_name = SceneManager.GetActiveScene().name;
        //gun = Quit.typeOfGun.text;
        instructor = Instructor;
        time = Time;
        basic_scene_score = score;
        basic_scene_percentage = percentages;
        totNumLanes = numLanes;
        exercise_name = exercise_Name;
        basic_scene_targets_hit = sceneTargetsHit;
        basic_scene_shots_missed = sceneShotsMissed;
        basic_lane_trainee_names = laneTraineeNames;
        basic_lane_split_times = laneSplitTimes;
        basic_lane_training_times = laneTrainingTime;
        basic_lane_response_times = laneResponseTimes;


        //Debug.Log("Initialising send process...");
        //SendData senddata = new SendData();
        //senddata.setData();

        try
        {
            //StartCoroutine(SendTrainingScore());
            //StartCoroutine(SendDataToServer());
            //StartCoroutine(SendWrappedDataToServer());
        }
        catch (Exception e)
        {
            Debug.Log("Error when instantiating uDp:" + e.Message);
        }

        //sender.displayText();
        //Save to csv file!
        System.IO.File.AppendAllText(@scoringPath, trainee_id + "," + trainee_name + "," + exercise_name + "," + instructor + "," + time + "," + number_of_rounds_used + Environment.NewLine);

    }

    public void GetPoints(List<Vector2> bulletPoints, int lane, string targetName)
    {
        Scoring.writeLog("GetPoints Lane[" + lane + "]" + " targetName:" + targetName);
        AimingBreathingCalculation(bulletPoints, bulletPoints.Count, lane, targetName);
    }
    void AimingBreathingCalculation(List<Vector2> positionXY, int counterSize, int LaneNumber, string targetName)
    {
        float MaxX = 0, MinX = 0;
        float TempX, TempY;
        float MaxY = 0, MinY = 0;
        float AimingGroup, BreathGroup;
       
        //string suggestions = "Nice shot";

        try
        {
            target = GameObject.Find(targetName).GetComponent<Transform>();
            Debug.Log("Working scoring on target:" + targetName + " counterSize=" + positionXY.Count);
            writeLog("AimingBreathingCalculation:"+ "Working scoring on target:" + targetName + " counterSize=" + positionXY.Count);

            positionX = new float[counterSize];
            positionY = new float[counterSize];
            for (int cntr = 0; cntr < counterSize; cntr++)
            {
                positionX[cntr] = positionXY[cntr].x;
                positionY[cntr] = positionXY[cntr].y;

                Debug.Log(positionX[cntr].ToString() + "and " + positionY[cntr].ToString());
            }
            /*if((positionY[counterSize-1] - positionY[counterSize - 2]) > minGrouping)
            {
                suggestions = "Correct Vertical";
            }*/
            for (int cntrOut = 0; cntrOut < counterSize; cntrOut++)//apply bubble sorting to get max and lowest values
            {
                for (int cntrIn = 0; cntrIn < counterSize - 1; cntrIn++)
                {
                    if (positionX[cntrIn] > positionX[cntrIn + 1])
                    {
                        TempX = positionX[cntrIn];
                        positionX[cntrIn] = positionX[cntrIn + 1];
                        positionX[cntrIn + 1] = TempX;
                    }
                    if (positionY[cntrIn] > positionY[cntrIn + 1])
                    {
                        TempY = positionY[cntrIn];
                        positionY[cntrIn] = positionY[cntrIn + 1];
                        positionY[cntrIn + 1] = TempY;
                    }
                }

            }//end of bubble sorting
            MinX = positionX[0];
            MaxX = positionX[counterSize - 1];




            //Vector3  objSize = Vector3.Scale(target.transform.localScale, target.transform.gameObject.GetComponent().)
            AimingGroup = (MaxX - MinX);//perform aiming calculation by subtracting the largest x from smallest x

            Debug.Log("SizeX = " + target.transform.lossyScale.x + " Aim group:" + AimingGroup);
            if (AimingGroup <= minGrouping)//10 cm * 64 (ImWidth measured with Tape)/100   15.625
            {
                AimingPoint[LaneNumber] = 30;
            }
            else if (AimingGroup > minGrouping && AimingGroup <= medGrouping)//20 cm * 64 (ImWidth measured with Tape)/100   31.25
            {
                AimingPoint[LaneNumber] = 20;
            }
            else if (AimingGroup > medGrouping && AimingGroup <= maxGrouping)//20 cm * 64 (ImWidth measured with Tape)/100   39.0625
            {
                AimingPoint[LaneNumber] = 10;
            }
            else if (AimingGroup > maxGrouping)//20 cm * 64 (ImWidth measured with Tape)/100   39.0625
            {
                AimingPoint[LaneNumber] = 5;
            }
            //Apply bubble sorting method to get the largest and smallest Y value for breathing
            MinY = positionY[0];
            MaxY = positionY[counterSize - 1];
            BreathGroup = (MaxY - MinY);
            if (BreathGroup <= minGrouping)//10 cm * 80 (ImWidth measured with Tape)/100   12.5
            {
                BreathingPoint[LaneNumber] = 30;
            }
            else if (BreathGroup > minGrouping && BreathGroup <= medGrouping)//20 cm * 80 (ImWidth measured with Tape)/100   25
            {
                BreathingPoint[LaneNumber] = 20;
            }
            else if (BreathGroup > medGrouping && BreathGroup <= maxGrouping)//20 cm * 80 (ImWidth measured with Tape)/100    31.25
            {
                BreathingPoint[LaneNumber] = 10;
            }
            else if (BreathGroup > maxGrouping)//20 cm * 80 (ImWidth measured with Tape)/100    31.25
            {
                BreathingPoint[LaneNumber] = 5;
            }

            GroupingPoint[LaneNumber] = (int)((BreathingPoint[LaneNumber] + AimingPoint[LaneNumber]) / 2);
            if (AimingPoint[LaneNumber] == 30 && BreathingPoint[LaneNumber] == 30)
            {
                GroupingPoint[LaneNumber] = 30;
            }
            else if (AimingPoint[LaneNumber] == 30 && BreathingPoint[LaneNumber] == 15)
            {
                GroupingPoint[LaneNumber] = 15;
            }
            else if (AimingPoint[LaneNumber] == 30 && BreathingPoint[LaneNumber] == 10)
            {
                GroupingPoint[LaneNumber] = 10;
            }
            else if (AimingPoint[LaneNumber] == 15 && BreathingPoint[LaneNumber] == 30)
            {
                GroupingPoint[LaneNumber] = 15;
            }
            else if (AimingPoint[LaneNumber] == 10 && BreathingPoint[LaneNumber] == 30)
            {
                GroupingPoint[LaneNumber] = 10;
            }
            else if (AimingPoint[LaneNumber] == 15 && BreathingPoint[LaneNumber] == 15)
            {
                GroupingPoint[LaneNumber] = 15;
            }
            else if (AimingPoint[LaneNumber] == 10 && BreathingPoint[LaneNumber] == 15)
            {
                GroupingPoint[LaneNumber] = 10;
            }
            else if (AimingPoint[LaneNumber] == 15 && BreathingPoint[LaneNumber] == 10)
            {
                GroupingPoint[LaneNumber] = 10;
            }
            else if (AimingPoint[LaneNumber] == 10 && BreathingPoint[LaneNumber] == 10)
            {
                GroupingPoint[LaneNumber] = 10;
            }
            groupingScore = (int)(GroupingPoint[LaneNumber] * 100 / 30) + "%";
            aimingScore = (int)(AimingPoint[LaneNumber] * 100 / 30) + "%";
            breathingScore = (int)(BreathingPoint[LaneNumber] * 100 / 30) + "%";
            float sumTime = 0f;

            if (elapsedTime == 0)//if it is the first shot
            {
                elapsedTime = Time.time;
                averageShotTime = 0f;
            }
            else
            {
                elapsedTimes.Add(Time.time - elapsedTime);
                elapsedTime = Time.time;
                for (int cntr = 0; cntr < elapsedTimes.Count; cntr++)
                {
                    sumTime = sumTime + elapsedTimes[cntr];
                }
                averageShotTime = sumTime / elapsedTimes.Count;
            }



            Debug.Log("Grouping[" + LaneNumber + "]:" + groupingScore);
            Debug.Log("Aiming[" + LaneNumber + "]:" + aimingScore);
            Debug.Log("Breathing[" + LaneNumber + "]:" + breathingScore);

            Lane1 = GameObject.Find("Lane_" + (LaneNumber + 1)).GetComponent<TextMesh>();
            suggestionTexts = GameObject.Find("Suggestion_" + (LaneNumber + 1)).GetComponent<TextMesh>();
            AverageTimeCalculation(LaneNumber);
            GroupingAnalysisX(LaneNumber, positionXY, counterSize);
            GroupingAnalysisY(LaneNumber, positionXY, counterSize);
            ShotAnalysis(LaneNumber, counterSize);
            ScoreSheet(LaneNumber, targetName);
            ShotAnalysisV2(positionXY, counterSize, LaneNumber, targetName);


            if (scoreSheet[LaneNumber] > 0)
            {
                displayText = "Shots:" + counterSize.ToString() + "\nScore:" + scoreSheet[LaneNumber] + "/" + (5 * counterSize) + "\nGrouping:" + groupingScore + "\nHorizontal:" + aimingScore + "\nVertical:" + breathingScore + "\nAvg Time:" + (Math.Round(averageShotTimes[LaneNumber], 3));// + "\nHor:" + Math.Round(averageAiming[LaneNumber], 3) + "\nVert:" + Math.Round(averageVert[LaneNumber], 3);
            }
            else
            {
                displayText = "Shots:" + counterSize.ToString() + "\nGrouping:" + groupingScore + "\nHorizontal:" + aimingScore + "\nVertical:" + breathingScore;// + "\nHor:" + Math.Round(averageAiming[LaneNumber], 3) + "\nVert:" + Math.Round(averageVert[LaneNumber], 3); // + "\nAvg Time:" + (Math.Round(averageShotTimes[LaneNumber], 3))
            }
            suggestionTexts.text = suggestX[LaneNumber] + "\n" + suggestY[LaneNumber];



            Lane1.text = displayText;
            scoring.text = "";// Average shoot time:" + averageShotTime + " sec";
        }
        catch(Exception ex)
        {
            logs += "\n" + ex.Message + ":" + ex.StackTrace;
            writeLog("Scoring AimingBreathingCalculation Error:" + ex.StackTrace);
            Debug.LogError("Scoring AimingBreathingCalculation Error:" + ex.StackTrace);
        }
    }/*End of Aiming Calculation function*/
    void AverageTimeCalculation(int laneNumber)
    {
           
            float sumTime = 0f;

            if (elapsedTimeEachLane[laneNumber] == 0)//if it is the first shot
            {
                Debug.Log("IF Sizes elapsedTimes:" + elapsedTimeEachLane.Count + " averageShotTimes:" + averageShotTimes.Count + " elapsedTimePerLane:" + elapsedTimePerLane.Count + " LaneNumber:" + laneNumber);
                elapsedTimeEachLane[laneNumber] = Time.time;
                averageShotTimes[laneNumber] = 0f;
                //elapsedTimePerLane.Add(laneNumber-1, elapsedTimeEachLane);
            }
            else
            {
                Debug.Log("Else Sizes elapsedTimeEachLane[laneNumber-1]:" + elapsedTimeEachLane[laneNumber] + " averageShotTimes:" + averageShotTimes.Count + " elapsedTimePerLane:" + elapsedTimePerLane.Count + " LaneNumber:" + laneNumber);
                elapsedTimePerLane[laneNumber].Add(Time.time - elapsedTimeEachLane[laneNumber]);
                elapsedTimeEachLane[laneNumber] = Time.time;
            }

            for (int cntr = 0; cntr < elapsedTimePerLane[laneNumber].Count; cntr++)
            {
                sumTime = sumTime + elapsedTimePerLane[laneNumber][cntr];
            }
            if (averageShotTimes.Count > 0)
            {
                averageShotTimes[laneNumber] = sumTime / elapsedTimePerLane[laneNumber].Count;
            }
    }
    void GroupingAnalysisX(int laneNumber, List<Vector2> positionXY, int counterSize)
    {
            
        float sumX = 0f, sumY = 0f;
       
        if (counterSize == 1)//if it is the first shot
        {
            Debug.Log("positionXY[0].x=" + positionXY[0].x);
            SpreadingEachLane[laneNumber] = positionXY[0].x;
            averageAiming[laneNumber] = 0f;
        }
        else
        {
            //Debug.Log("positionXY["+ (counterSize - 1) + "].x=" + positionXY[counterSize - 1].x + "  SpreadingEachLane[laneNumber]:"+ SpreadingEachLane[laneNumber]);
            float diff = positionXY[counterSize - 1].x - SpreadingEachLane[laneNumber];
            Debug.Log("Diff:" + diff.ToString());
            
            SpreadingPerLane[laneNumber].Add(Math.Abs(diff));
            SpreadingEachLane[laneNumber] = positionXY[counterSize - 1].x;
            foreach (float perLane in SpreadingPerLane[laneNumber])
            {
                Debug.Log("perLane:" + perLane);
                sumX = sumX + perLane;
                //sumY = sumY + SpreadingPerLane[laneNumber][cntr];
            }
            if (counterSize > 0)
            {
                averageAiming[laneNumber] = sumX / SpreadingPerLane[laneNumber].Count;
                Debug.Log("Sum Was=" + sumX + "  averageAiming[" + laneNumber + "]:" + averageAiming[laneNumber]);
            }//*/
            //averageAiming[laneNumber]= SpreadingPerLane[laneNumber].Count > 0 ? SpreadingPerLane[laneNumber].Average() : 0.0;
            
        }

        
        
    }
    void GroupingAnalysisY(int laneNumber, List<Vector2> positionXY, int counterSize)
    {

        float sumX = 0f, sumY = 0f;

        if (counterSize == 1)//if it is the first shot
        {
            Debug.Log("positionXY[0].y=" + positionXY[0].y);
            SpreadingEachLaneY[laneNumber] = positionXY[0].y;
            averageVert[laneNumber] = 0f;
        }
        else
        {
            //Debug.Log("positionXY["+ (counterSize - 1) + "].x=" + positionXY[counterSize - 1].x + "  SpreadingEachLane[laneNumber]:"+ SpreadingEachLane[laneNumber]);
            float diff = positionXY[counterSize - 1].y - SpreadingEachLaneY[laneNumber];
            Debug.Log("Diff:" + diff.ToString());

            SpreadingPerLaneY[laneNumber].Add(Math.Abs(diff));
            SpreadingEachLaneY[laneNumber] = positionXY[counterSize - 1].y;
            foreach (float perLane in SpreadingPerLane[laneNumber])
            {
                Debug.Log("perLane:" + perLane);
                sumY = sumY + perLane;
                //sumY = sumY + SpreadingPerLane[laneNumber][cntr];
            }
            if (counterSize > 0)
            {
                averageVert[laneNumber] = sumY / SpreadingPerLane[laneNumber].Count;
                Debug.Log("Sum Was=" + sumY + "  averageAiming[" + laneNumber + "]:" + averageAiming[laneNumber]);
            }//*/
            //averageVert[laneNumber] = SpreadingPerLaneY[laneNumber].Count > 0 ? SpreadingPerLaneY[laneNumber].Average() : 0.0;
        }


    }
    void ShotAnalysis(int laneNumber, int counterSize)
    {
        if (counterSize > 1)
        {
            if (Math.Round(averageAiming[laneNumber], 3) <= minGrouping)
            {
                //suggestX += "\n++Grouping";
            }
            else if (Math.Round(averageAiming[laneNumber], 3) > minGrouping && Math.Round(averageAiming[laneNumber], 3) <= medGrouping)
            {
                //suggestX += "\n--TriggerControl \n--SightAlignment";
            }
            else if (Math.Round(averageAiming[laneNumber], 3) > medGrouping && Math.Round(averageAiming[laneNumber], 3) < maxGrouping)
            {
                //suggestX += "\n--TriggerControl\n--Sight--Alignment!!";
            }
            else
            {
                //suggestX += "\nSight Alignment \n follow through";
            }

            if (Math.Round(averageVert[laneNumber], 3) <= minGrouping)
            {
                //suggestY += "\nGood Grouping";
            }
            else if (Math.Round(averageVert[laneNumber], 3) > minGrouping && Math.Round(averageVert[laneNumber], 3) <= medGrouping)
            {
                //suggestY += "\nFollow Through";
            }
            else if (Math.Round(averageVert[laneNumber], 3) > medGrouping && Math.Round(averageVert[laneNumber], 3) < maxGrouping)
            {
                //suggestY += "\nCheck Stance";
            }
            else
            {
                //suggestY += "\nCheck posture";
            }
        }

        
    }

    void ShotAnalysisV2(List<Vector2> positionXY, int counterSize, int laneNumber, string targetName)
    {
        Debug.Log("counterSize v2: " + counterSize);
        if (counterSize > 1)
        {
            float deltaY = Math.Abs(positionXY[counterSize-1].y - positionXY[counterSize - 2].y);
            float deltaX = Math.Abs(positionXY[counterSize-1].x - positionXY[counterSize - 2].x);

            Debug.Log("deltaX:"+ deltaX + " deltaY:" + deltaY);
            
            if (Math.Round(deltaY, 3) > minGrouping && Math.Round(deltaY, 3) <= medGrouping  && (positionXY[counterSize-1].y> positionXY[counterSize-2].y))
            {
                suggestY[laneNumber] = "\n--Stance";
            }
            if (Math.Round(deltaY, 3) > minGrouping && Math.Round(deltaY, 3) <= medGrouping && (positionXY[counterSize-1].y < positionXY[counterSize - 2].y))
            {
                suggestY[laneNumber] = "\n--FollowThrough";
            }
            if (Math.Round(deltaY, 3) > medGrouping && Math.Round(deltaY, 3) < maxGrouping && (positionXY[counterSize-1].y > positionXY[counterSize - 2].y))
            {
                suggestY[laneNumber] = "\n--TrigerControl";
            }
            if (Math.Round(deltaY, 3) > medGrouping && Math.Round(deltaY, 3) < maxGrouping && (positionXY[counterSize-1].y < positionXY[counterSize - 2].y))
            {
                suggestY[laneNumber] = "\n--Grip/Canting";
            }
            if (Math.Round(deltaX, 3) > minGrouping && Math.Round(deltaX, 3) <= medGrouping && (positionXY[counterSize-1].x > positionXY[counterSize - 2].x))
            {
                suggestX[laneNumber] = "\n--Recovery";
            }
            if (Math.Round(deltaX, 3) > minGrouping && Math.Round(deltaX, 3) <= medGrouping && (positionXY[counterSize-1].x < positionXY[counterSize - 2].x))
            {
                suggestX[laneNumber] = "\n--Recovery";
            }
            if (Math.Round(deltaX, 3) > medGrouping && Math.Round(deltaX, 3) < maxGrouping && (positionXY[counterSize-1].x > positionXY[counterSize - 2].x))
            {
                suggestX[laneNumber] = "\n--Aiming\n--Canting";
            }
            if (Math.Round(deltaX, 3) > medGrouping && Math.Round(deltaX, 3) < maxGrouping && (positionXY[counterSize-1].x < positionXY[counterSize - 2].x))
            {
                suggestX[laneNumber] = "\n--Aiming/Canting";
            }
            if (Math.Round(deltaX, 3) > maxGrouping )
            {
                suggestX[laneNumber] = "\n--Grip\n--Alignment";

            }
            if (Math.Round(deltaY, 3) > maxGrouping)
            {
                suggestY[laneNumber] = "\n--Grip\n--Breathing";
            }

            }
    }


    private IEnumerator SendWrappedDataToServer()
    {

        Url = configuration.ConfigAllUrls["server_url"];
        range_scoring_endpoint = configuration.ConfigAllEndpoints["range_scoring_endpoint"];
        lane_image_endpoint = configuration.ConfigAllEndpoints["lane_image_endpoint"];

        // Parse trainee data

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

        string lane1_image_path = FileManager.gamePath + "Score Images/Trainee_1.png"; // Adjust path as needed
        string lane2_image_path = FileManager.gamePath + "Score Images/Trainee_2.png";
        string lane3_image_path = FileManager.gamePath + "Score Images/Trainee_3.png";

        /////////////////////////////////////////////////////////////////////////////////////// 


        string lane1_image_key = lane_1_user_under_email + "_" + lane_1_trainee_id + "_" + exercise_name + "_" + time + "_" + lane_1_trainee_name;
        string lane2_image_key = lane_2_user_under_email + "_" + lane_2_trainee_id + "_" + exercise_name + "_" + time + "_" + lane_2_trainee_name;
        string lane3_image_key = lane_3_user_under_email + "_" + lane_3_trainee_id + "_" + exercise_name + "_" + time + "_" + lane_3_trainee_name;


        WWWForm lane1_image_form = new WWWForm();
        WWWForm lane2_image_form = new WWWForm();
        WWWForm lane3_image_form = new WWWForm();

        string payload_header = ($"success*Data inserted*{totNumLanes}*{exercise_name}*{time}");
        string lane_1_data = "";
        string lane_2_data = "";
        string lane_3_data = "";


        bool isInternetAvailable = false;
        bool isBackupEnabled = true;

        yield return StartCoroutine(CheckInternetConnection((bool isConnected) =>
        {
            isInternetAvailable = isConnected;
        }));

        // print(login_Manager.EmailText);
        if (login_Manager.EmailText == "Empty" || login_Manager.EmailText == "admin")
        {
            isBackupEnabled = false;
        }

        //Send Lane Data
        switch (totNumLanes)
        {
            case "1":
                byte[] imageBytes1_lane1 = File.ReadAllBytes(lane1_image_path);

                ////////////////////////////Shooting score processing//////////////
                lane_1_trainee_Data = basic_lane_trainee_names[0].Split(':');
                lane_1_training_time = basic_lane_training_times[0].Replace(',', '.');


                lane_1_data = ($"{basic_scene_percentage[0]}*{basic_scene_targets_hit[0]}*{basic_scene_shots_missed[0]}*" +
                    $"{basic_lane_split_times[0]}*{basic_lane_response_times[0]}*{lane_1_training_time}");

                if (!isBackupEnabled)
                {
                    break;
                }
                else if (lane_1_trainee_Data[0] == "" || isInternetAvailable == false)
                {
                    backupManager.BackupLane1OfflineData(login_Manager.EmailText, payload_header, lane_1_data, imageBytes1_lane1);
                    break;
                }

                // print("Point reached...");

                lane_1_trainee_id = lane_1_trainee_Data[0];
                lane_1_trainee_name = lane_1_trainee_Data[1];
                lane_1_user_under = lane_1_trainee_Data[2];
                lane_1_instructor_n = lane_1_trainee_Data[3];
                lane_1_gun_type = lane_1_trainee_Data[4];
                lane_1_trainee_email = lane_1_trainee_Data[5];
                lane_1_created_at = lane_1_trainee_Data[6];
                lane_1_user_under_email = lane_1_trainee_Data[7];

                lane1_image_key = lane_1_user_under_email + "_" + lane_1_trainee_id + "_" + exercise_name + "_" + time + "_" + lane_1_trainee_name;
                lane1_image_key = ReplaceCharacter(lane1_image_key, ' ', '-');

                // Prepare JSON payload
                Dictionary<string, object> jsonData_1 = new Dictionary<string, object>
                {
                    { "status", "success" },
                    { "message", "Data inserted" },
                    { "num_lanes", totNumLanes },
                    { "scenario_type", exercise_name },
                    { "time", time }
                };

                // Create dictionaries for each lane
                Dictionary<string, string> lane1_data_1 = new Dictionary<string, string>
                {
                    { "trainee", lane_1_trainee_name },
                    { "trainee_id", lane_1_trainee_id },
                    { "trainee_percentage", basic_scene_percentage[0] },
                    { "target_hits", basic_scene_targets_hit[0] },
                    { "target_missed", basic_scene_shots_missed[0] },
                    { "training_time", lane_1_training_time },
                    { "user_under", lane_1_user_under },
                    { "instructor", lane_1_instructor_n },
                    { "weapon", lane_1_gun_type },
                    { "split_time", basic_lane_split_times[0] },
                    { "reaction_time", basic_lane_response_times[0] },
                    { "user_under_email", lane_1_user_under_email },
                    { "trainee_email", lane_1_trainee_email },
                    { "created_at", lane_1_created_at },
                    { "image_key", lane1_image_key}
                };

                print("Training time " + lane_1_training_time);

                // Process score data
                jsonData_1.Add("lane_1", lane1_data_1); // Add Lane1 Data
                string jsonString_1 = JsonConvert.SerializeObject(jsonData_1);
                byte[] bodyRaw_1 = System.Text.Encoding.UTF8.GetBytes(jsonString_1);
                string payLoad = System.Text.Encoding.UTF8.GetString(bodyRaw_1);

                ///////////////////////Process image data//////////////////////////
                lane1_image_form.AddBinaryData("image_blob", imageBytes1_lane1, "image.png", "image/png");
                lane1_image_form.AddField("image_key", lane1_image_key);


                // 
                StartCoroutine(SendScoreData(bodyRaw_1, Url + range_scoring_endpoint)); // Send score data HTTP request
                StartCoroutine(SendImageForm(imageBytes1_lane1, lane1_image_key, Url + lane_image_endpoint)); // send image data form request

                print("JSON Payload: " + payLoad);
                break;

            case "2":
                byte[] imageBytes2_lane1 = File.ReadAllBytes(lane1_image_path);
                byte[] imageBytes2_lane2 = File.ReadAllBytes(lane2_image_path);

                ////////////////////////////Shooting score processing//////////////
                lane_1_trainee_Data = basic_lane_trainee_names[0].Split(':');
                lane_2_trainee_Data = basic_lane_trainee_names[1].Split(':');
                lane_1_training_time = basic_lane_training_times[0].Replace(',', '.');
                lane_2_training_time = basic_lane_training_times[1].Replace(',', '.');

                // if (lane_1_trainee_Data[1] == null) break; // Lane 1 Break Point (Fail Safe)
                // if (lane_2_trainee_Data[1] == null) break; // Lane 2 Break Point (Fail Safe)
                lane_1_data = ($"{basic_scene_percentage[0]}*{basic_scene_targets_hit[0]}*" + 
                    $"{basic_scene_shots_missed[0]}*{basic_lane_split_times[0]}*{basic_lane_response_times[0]}*{lane_1_training_time}");
                lane_2_data = ($"{basic_scene_percentage[1]}*{basic_scene_targets_hit[1]}*" +
                    $"{basic_scene_shots_missed[1]}*{basic_lane_split_times[1]}*{basic_lane_response_times[1]}*{lane_2_training_time}");

                if (lane_1_trainee_Data[0] == "" || isInternetAvailable == false && (login_Manager.EmailText != "Empty" && login_Manager.EmailText != "admin"))
                {
                    backupManager.BackupLane2OfflineData(login_Manager.EmailText,payload_header, lane_1_data, lane_2_data, 
                        imageBytes2_lane1, imageBytes2_lane2);
                    break;
                }


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

                lane1_image_key = lane_1_user_under_email + "_" + lane_1_trainee_id + "_" + exercise_name + "_" + time + "_" + lane_1_trainee_name;
                lane2_image_key = lane_2_user_under_email + "_" + lane_2_trainee_id + "_" + exercise_name + "_" + time + "_" + lane_2_trainee_name;

                lane1_image_key = ReplaceCharacter(lane1_image_key, ' ', '-');
                lane2_image_key = ReplaceCharacter(lane2_image_key, ' ', '-');

                // Prepare JSON payload
                Dictionary<string, object> jsonData_2 = new Dictionary<string, object>
                {
                    { "status", "success" },
                    { "message", "Data inserted" },
                    { "num_lanes", totNumLanes },
                    { "scenario_type", exercise_name },
                    { "time", time }
                };

                // Create dictionaries for each lane
                Dictionary<string, string> lane1_data_2 = new Dictionary<string, string>
                {
                    { "trainee", lane_1_trainee_name },
                    { "trainee_id", lane_1_trainee_id },
                    { "trainee_percentage", basic_scene_percentage[0] },
                    { "target_hits", basic_scene_targets_hit[0] },
                    { "target_missed", basic_scene_shots_missed[0] },
                    { "training_time", lane_1_training_time },
                    { "user_under", lane_1_user_under },
                    { "instructor", lane_1_instructor_n },
                    { "weapon", lane_1_gun_type },
                    { "split_time", basic_lane_split_times[0] },
                    { "reaction_time", basic_lane_response_times[0] },
                    { "user_under_email", lane_1_user_under_email },
                    { "trainee_email", lane_1_trainee_email },
                    { "created_at", lane_1_created_at },
                    { "image_key", lane1_image_key}
                };

                Dictionary<string, string> lane2_data_2 = new Dictionary<string, string>
                {
                    { "trainee", lane_2_trainee_name },
                    { "trainee_id", lane_2_trainee_id },
                    { "trainee_percentage", basic_scene_percentage[1] },
                    { "target_hits", basic_scene_targets_hit[1] },
                    { "target_missed", basic_scene_shots_missed[1] },
                    { "training_time", lane_2_training_time },
                    { "user_under", lane_2_user_under },
                    { "instructor", lane_2_instructor_n },
                    { "weapon", lane_2_gun_type },
                    { "split_time", basic_lane_split_times[1] },
                    { "reaction_time", basic_lane_response_times[1] },
                    { "user_under_email", lane_2_user_under_email },
                    { "trainee_email", lane_2_trainee_email },
                    { "created_at", lane_2_created_at },
                    { "image_key", lane2_image_key}
                };

                // process score data
                jsonData_2.Add("lane_1", lane1_data_2); // Add Lane 1 Data       
                jsonData_2.Add("lane_2", lane2_data_2); // Add Lane 2 Data
                string jsonString_2 = JsonConvert.SerializeObject(jsonData_2);
                byte[] bodyRaw_2 = System.Text.Encoding.UTF8.GetBytes(jsonString_2);

                ///////////////////////Process image data//////////////////////////

                lane1_image_form.AddBinaryData("image_blob", imageBytes2_lane1, "image.png", "image/png");
                lane1_image_form.AddField("image_key", lane1_image_key);

                lane2_image_form.AddBinaryData("image_blob", imageBytes2_lane2, "image.png", "image/png");
                lane2_image_form.AddField("image_key", lane2_image_key);


                StartCoroutine(SendScoreData(bodyRaw_2, Url + range_scoring_endpoint)); // Send score data HTTP post
                StartCoroutine(SendImageForm(imageBytes2_lane1, lane1_image_key, Url + lane_image_endpoint)); // Send image data post
                StartCoroutine(SendImageForm(imageBytes2_lane2, lane2_image_key, Url + lane_image_endpoint)); // Send image data post

                print("JSON Payload: " + System.Text.Encoding.UTF8.GetString(bodyRaw_2));
                break;
            case "3":
                byte[] imageBytes3_lane1 = File.ReadAllBytes(lane1_image_path);
                byte[] imageBytes3_lane2 = File.ReadAllBytes(lane2_image_path);
                byte[] imageBytes3_lane3 = File.ReadAllBytes(lane3_image_path);

                ////////////////////////////Shooting score processing//////////////
                lane_1_trainee_Data = basic_lane_trainee_names[0].Split(':');
                lane_2_trainee_Data = basic_lane_trainee_names[1].Split(':');
                lane_3_trainee_Data = basic_lane_trainee_names[2].Split(':');
                lane_1_training_time = basic_lane_training_times[0].Replace(',', '.');
                lane_2_training_time = basic_lane_training_times[1].Replace(',', '.');
                lane_3_training_time = basic_lane_training_times[2].Replace(',', '.');

                // if (lane_1_trainee_Data[1] == null) break; // Lane 1 Break Point (Fail Safe)
                // if (lane_2_trainee_Data[1] == null) break; // Lane 2 Break Point (Fail Safe)
                // if (lane_3_trainee_Data[1] == null) break; // Lane 3 Break Point (Fail Safe)
                lane_1_data = ($"{basic_scene_percentage[0]}*{basic_scene_targets_hit[0]}*" +
                    $"{basic_scene_shots_missed[0]}*{basic_lane_split_times[0]}*{basic_lane_response_times[0]}*{lane_1_training_time}");
                lane_2_data = ($"{basic_scene_percentage[1]}*{basic_scene_targets_hit[1]}*" +
                    $"{basic_scene_shots_missed[1]}*{basic_lane_split_times[1]}*{basic_lane_response_times[1]}*{lane_2_training_time}");
                lane_3_data = ($"{basic_scene_percentage[2]}*{basic_scene_targets_hit[2]}*" +
                    $"{basic_scene_shots_missed[2]}*{basic_lane_split_times[2]}*{basic_lane_response_times[2]}*{lane_3_training_time}");

                if (lane_1_trainee_Data[0] == "" || isInternetAvailable == false && (login_Manager.EmailText != "Empty" && login_Manager.EmailText != "admin"))
                {
                    backupManager.BackupLane3OfflineData(login_Manager.EmailText, payload_header, lane_1_data, lane_2_data, lane_3_data,
                        imageBytes3_lane1, imageBytes3_lane2, imageBytes3_lane2);
                    break;
                }


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

                lane1_image_key = lane_1_user_under_email + "_" + lane_1_trainee_id + "_" + exercise_name + "_" + time + "_" + lane_1_trainee_name;
                lane2_image_key = lane_2_user_under_email + "_" + lane_2_trainee_id + "_" + exercise_name + "_" + time + "_" + lane_2_trainee_name;
                lane3_image_key = lane_3_user_under_email + "_" + lane_3_trainee_id + "_" + exercise_name + "_" + time + "_" + lane_3_trainee_name;

                lane1_image_key = ReplaceCharacter(lane1_image_key, ' ', '-');
                lane2_image_key = ReplaceCharacter(lane2_image_key, ' ', '-');
                lane3_image_key = ReplaceCharacter(lane3_image_key, ' ', '-');

                // Prepare JSON payload
                Dictionary<string, object> jsonData_3 = new Dictionary<string, object>
                {
                    { "status", "success" },
                    { "message", "Data inserted" },
                    { "num_lanes", totNumLanes },
                    { "scenario_type", exercise_name },
                    { "time", time }
                };
                // Create dictionaries for each lane
                Dictionary<string, string> lane1_data_3 = new Dictionary<string, string>
                {
                    { "trainee", lane_1_trainee_name },
                    { "trainee_id", lane_1_trainee_id },
                    { "trainee_percentage", basic_scene_percentage[0] },
                    { "target_hits", basic_scene_targets_hit[0] },
                    { "target_missed", basic_scene_shots_missed[0] },
                    { "training_time", lane_1_training_time },
                    { "user_under", lane_1_user_under },
                    { "instructor", lane_1_instructor_n },
                    { "weapon", lane_1_gun_type },
                    { "split_time", basic_lane_split_times[0] },
                    { "reaction_time", basic_lane_response_times[0] },
                    { "user_under_email", lane_1_user_under_email },
                    { "trainee_email", lane_1_trainee_email },
                    { "created_at", lane_1_created_at },
                    { "image_key", lane1_image_key}
                };

                Dictionary<string, string> lane2_data_3 = new Dictionary<string, string>
                {
                    { "trainee", lane_2_trainee_name },
                    { "trainee_id", lane_2_trainee_id },
                    { "trainee_percentage", basic_scene_percentage[1] },
                    { "target_hits", basic_scene_targets_hit[1] },
                    { "target_missed", basic_scene_shots_missed[1] },
                    { "training_time", lane_2_training_time },
                    { "user_under", lane_2_user_under },
                    { "instructor", lane_2_instructor_n },
                    { "weapon", lane_2_gun_type },
                    { "split_time", basic_lane_split_times[1] },
                    { "reaction_time", basic_lane_response_times[1] },
                    { "user_under_email", lane_2_user_under_email },
                    { "trainee_email", lane_2_trainee_email },
                    { "created_at", lane_2_created_at },
                    { "image_key", lane2_image_key}
                };

                Dictionary<string, string> lane3_data_3 = new Dictionary<string, string>
                {
                    { "trainee", lane_3_trainee_name },
                    { "trainee_id", lane_3_trainee_id },
                    { "trainee_percentage", basic_scene_percentage[2] },
                    { "target_hits", basic_scene_targets_hit[2] },
                    { "target_missed", basic_scene_shots_missed[2] },
                    { "training_time", lane_3_training_time },
                    { "user_under", lane_3_user_under },
                    { "instructor", lane_3_instructor_n },
                    { "weapon", lane_3_gun_type },
                    { "split_time", basic_lane_split_times[2] },
                    { "reaction_time", basic_lane_response_times[2] },
                    { "user_under_email", lane_3_user_under_email },
                    { "trainee_email", lane_3_trainee_email },
                    { "created_at", lane_3_created_at },
                    { "image_key", lane3_image_key}
                };

                // Add Lane 1 Data
                jsonData_3.Add("lane_1", lane1_data_3); // Add Lane 1 Data
                jsonData_3.Add("lane_2", lane2_data_3); // Add Lane 2 Data
                jsonData_3.Add("lane_3", lane3_data_3); // Add Lane 3 Data
                string jsonString_3 = JsonConvert.SerializeObject(jsonData_3);
                byte[] bodyRaw_3 = System.Text.Encoding.UTF8.GetBytes(jsonString_3);

                ///////////////////////Process image data//////////////////////////
                lane1_image_form.AddBinaryData("image_blob", imageBytes3_lane1, "image1.png", "image/png");
                lane1_image_form.AddField("image_key", lane1_image_key);

                lane2_image_form.AddBinaryData("image_blob", imageBytes3_lane2, "image2.png", "image/png");
                lane2_image_form.AddField("image_key", lane2_image_key);

                lane3_image_form.AddBinaryData("image_blob", imageBytes3_lane3, "image3.png", "image/png");
                lane3_image_form.AddField("image_key", lane3_image_key);


                StartCoroutine(SendScoreData(bodyRaw_3, Url + range_scoring_endpoint)); // Send score data HTTP post
                StartCoroutine(SendImageForm(imageBytes3_lane1, lane1_image_key, Url + lane_image_endpoint)); // Send image data post
                StartCoroutine(SendImageForm(imageBytes3_lane2, lane2_image_key, Url + lane_image_endpoint)); // Send image data post
                StartCoroutine(SendImageForm(imageBytes3_lane3, lane3_image_key, Url + lane_image_endpoint)); // Send image data post

                print("JSON Payload: " + System.Text.Encoding.UTF8.GetString(bodyRaw_3));
                break;
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
                if (backupManager != null)
                    backupManager.SaveScoreBackupFile(pay_load, url_endpoint);
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

                if (backupManager != null)
                    backupManager.SaveImageBackupFile(imageByte, image_key, url_endpoint);
            }

        }   
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
    void ScoreSheet(int laneNumber, string targetName)
    {
        if(targetName.Contains("A_") || targetName.Contains("inner_target") || targetName.Contains("_dot_target") || targetName.Contains("line_"))
        {
            scoreSheet[laneNumber] += 5;
        }
        else if(targetName.Contains("B_"))
        {
            scoreSheet[laneNumber] += 4;
        }
        else if (targetName.Contains("C_"))
        {
            scoreSheet[laneNumber] += 3;
        }
        else if (targetName.Contains("D_"))
        {
            scoreSheet[laneNumber] += 1;
        }
    }

    static public void writeLog(string logline)
    {
        System.IO.File.AppendAllText(@logPath, "\n"+DateTime.Now.ToString()+":"+logline);

    }

    [System.Serializable]
    public class Wrapper
    {
        public List<KeyValue> jsonData = new List<KeyValue>();

        public Wrapper(Dictionary<string, string> dictionary)
        {
            foreach (var kvp in dictionary)
            {
                jsonData.Add(new KeyValue(kvp.Key, kvp.Value));
            }
        }
    }

    [System.Serializable]
    public class KeyValue
    {
        public string key;
        public string value;

        public KeyValue(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }

}