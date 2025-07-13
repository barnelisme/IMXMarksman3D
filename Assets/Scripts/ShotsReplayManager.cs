using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ShotsReplayManager : MonoBehaviour
{
    string activeScene = "";
    private int lane_1_impact_index = 0, lane_2_impact_index = 0, lane_3_impact_index = 0;
    public bool isShotsReplayOpen = false;
    private int current_replay_lane = 1;
    private bool allShotsDisplayed = false;
    private bool disable_target_hit = false;
    private bool move_to_next_target = false;
    private bool move_to_prev_target = false;
    private bool secondary_prev_point = false;
    private bool replayTargetsAllSet = false;
    private int current_target_index = 0;
    private string signalOrigin = "score button";
    public GameObject shotsReplayButton;
    private GameObject[] RisingPlatesManager;

    private List<string> point_lane_1 = new List<string>(); //score storage
    private List<string> point_lane_2 = new List<string>();
    private List<string> point_lane_3 = new List<string>();
    private List<GameObject> lane_1_impact_list = new List<GameObject>(); //score storage
    private List<GameObject> lane_2_impact_list = new List<GameObject>();
    private List<GameObject> lane_3_impact_list = new List<GameObject>();

    public TextMeshProUGUI shots_replay_counter;
    public TextMeshProUGUI replay_lane_header;
    public GameObject shotsReplayPanel;
    public GameObject adminScorePanel;
    public GameObject shooterScorePanel;
    public GameObject splitTimePanel;

    [Header("Training Data")]
    private int numLane1ShotsFired = 0;   //total number of shots fired
    private int numLane2ShotsFired = 0;
    private int numLane3ShotsFired = 0;
    private int numLane1ShotsMissed = 0;   //total number of shots Missed
    private int numLane2ShotsMissed = 0;
    private int numLane3ShotsMissed = 0;
    private float lane1ActiveTimeCounter;
    private float lane2ActiveTimeCounter;
    private float lane3ActiveTimeCounter;
    private float lane1TrainingPercentage = 0;
    private float lane2TrainingPercentage = 0;
    private float lane3TrainingPercentage = 0;
    private List<GameObject> targets = new List<GameObject>();

    [Header("Replay Score")]
    public GameObject replay_camera_1;
    public GameObject replay_camera_2;
    public GameObject replay_camera_3;
    public GameObject replay_score_1;
    public TextMeshProUGUI replay_lane1_shots_fired;
    public TextMeshProUGUI replay_lane1_shots_missed;
    public TextMeshProUGUI replay_lane1_total_time;
    public TextMeshProUGUI replay_lane1_percentage;

    public GameObject replay_score_2;
    public TextMeshProUGUI replay_lane2_shots_fired;
    public TextMeshProUGUI replay_lane2_shots_missed;
    public TextMeshProUGUI replay_lane2_total_time;
    public TextMeshProUGUI replay_lane2_percentage;

    public GameObject replay_score_3;
    public TextMeshProUGUI replay_lane3_shots_fired;
    public TextMeshProUGUI replay_lane3_shots_missed;
    public TextMeshProUGUI replay_lane3_total_time;
    public TextMeshProUGUI replay_lane3_percentage;

    // Start is called before the first frame update
    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        RisingPlatesManager = GameObject.FindGameObjectsWithTag("RisingPlatesHandler");

        //DropDown.scene_type.ToLower().Contains("static")
        if (shotsReplayButton != null && (DropDown.scene_type == "static" || DropDown.scene_type == "static response"))
        {
            shotsReplayButton.SetActive(true);
        }
        else if(activeScene.ToLower().Contains("hiddentarget"))
        {
            shotsReplayButton.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
        if((DropDown.scene_type == "static" || DropDown.scene_type == "static response"))
        {

            if(StaticVariableManager.isStopTraining && replayTargetsAllSet == false)
            {
                //Open all lanes...
                if (activeScene.ToLower().Contains("plat"))
                {
                    if (StaticVariableManager.lanePlatesDisabled)
                    {
                        ManageSRScoringReplay();
                        replayTargetsAllSet = true;
                    }
                }
                else
                {
                    ManageSRScoringReplay();
                    replayTargetsAllSet = true;
                }
            }

        }
        else if (DropDown.scene_type == "moving")
        {
            if(StaticVariableManager.isStopTraining && replayTargetsAllSet == false)
            {
                ManageMovingScoringReplay();

                replayTargetsAllSet = true;
            }
        }

    }

    public void ManageSRScoringReplay()
    {
        Init();
        showLaneShots();
       
    }

    public void ManageMovingScoringReplay()
    {
        StaticVariableManager.isScoreLanesReady = true;
    }

    public void DisableReplayScore()
    {
        if (activeScene.ToLower().Contains("1lane"))
        {
            replay_score_1.SetActive(false);
        }
        else if (activeScene.ToLower().Contains("2lane"))
        {
            replay_score_1.SetActive(false);
            replay_score_2.SetActive(false);
        }
        else if (activeScene.ToLower().Contains("3lane"))
        {
            replay_score_1.SetActive(false);
            replay_score_2.SetActive(false);
            replay_score_3.SetActive(false);
        }

        shotsReplayPanel.SetActive(false);
        isShotsReplayOpen = false;

        // DisableLaneBullets();

    }

    private void DisableLaneBullets()
    {
        foreach (GameObject bullet in lane_1_impact_list)
        {
            bullet.SetActive(false);
            bullet.GetComponent<BulletManager>().time_canvas.SetActive(false);
            bullet.GetComponent<BulletManager>().number_canvas.SetActive(false);

            if (bullet.transform.name.ToLower().Contains("hit") && bullet.GetComponent<BulletManager>().target_hit != null)
            {
                bullet.GetComponent<BulletManager>().target_hit.SetActive(false);
            }

        }
        foreach (GameObject bullet in lane_2_impact_list)
        {
            bullet.SetActive(false);
            bullet.GetComponent<BulletManager>().time_canvas.SetActive(false);
            bullet.GetComponent<BulletManager>().number_canvas.SetActive(false);

            if (bullet.transform.name.ToLower().Contains("hit") && bullet.GetComponent<BulletManager>().target_hit != null)
            {
                bullet.GetComponent<BulletManager>().target_hit.SetActive(false);
            }
        }
        foreach (GameObject bullet in lane_3_impact_list)
        {
            bullet.SetActive(false);
            bullet.GetComponent<BulletManager>().time_canvas.SetActive(false);
            bullet.GetComponent<BulletManager>().number_canvas.SetActive(false);

            if (bullet.transform.name.ToLower().Contains("hit") && bullet.GetComponent<BulletManager>().target_hit != null)
            {
                bullet.GetComponent<BulletManager>().target_hit.SetActive(false);
            }
        }
    }

    public void showLaneShots()
    {
        //print("replay lane: " + current_replay_lane);

        if (activeScene.ToLower().Contains("1lane"))
        {
            setupReplayLane("1");
        }
        else if (activeScene.ToLower().Contains("2lane"))
        {
            setupReplayLane("1");
            setupReplayLane("2");
        }
        else if (activeScene.ToLower().Contains("3lane"))
        {
            setupReplayLane("1");
            setupReplayLane("2");
            setupReplayLane("3");
        }
    }

    private void setupReplayLane(string lane)
    {

        switch (lane)
        {
            case "1":
                this.GetComponent<ReplayCounter>().SetupScoreReplay(lane, signalOrigin);

                // replay_score_1.SetActive(true);
                showScore(replay_lane1_shots_fired, replay_lane1_shots_missed, replay_lane1_total_time, replay_lane1_percentage,
                    numLane1ShotsFired, numLane1ShotsMissed, lane1ActiveTimeCounter, lane1TrainingPercentage);
                SendPrintSignal(replay_camera_1, 1); // Capture Image
                break;

            case "2":
                this.GetComponent<ReplayCounter>().SetupScoreReplay(lane, signalOrigin);

                // replay_score_2.SetActive(true);
                showScore(replay_lane2_shots_fired, replay_lane2_shots_missed, replay_lane2_total_time, replay_lane2_percentage,
                    numLane2ShotsFired, numLane2ShotsMissed, lane2ActiveTimeCounter, lane2TrainingPercentage);
                SendPrintSignal(replay_camera_2, 2); // Capture Image
                break;

            case "3":
                this.GetComponent<ReplayCounter>().SetupScoreReplay(lane, signalOrigin);

                // replay_score_3.SetActive(true);
                showScore(replay_lane3_shots_fired, replay_lane3_shots_missed, replay_lane3_total_time, replay_lane3_percentage,
                    numLane3ShotsFired, numLane3ShotsMissed, lane3ActiveTimeCounter, lane3TrainingPercentage);
                SendPrintSignal(replay_camera_3, 3); // Capture Image
                break;
        }

    }

    private void showScore(TextMeshProUGUI shortsFiredText, TextMeshProUGUI shortsMissedText, TextMeshProUGUI totalTimeText, TextMeshProUGUI percentageText,
        int shotsFired, int shotsMissed, float activeTime, float percentage)
    {
        shortsFiredText.text = "Shots fired: " + shotsFired.ToString("0");
        shortsMissedText.text = "Shots missed: " + shotsMissed.ToString("0");
        totalTimeText.text = "Total time: " + activeTime.ToString("0.0") + " sec";
        percentageText.text = "Percentage: " + percentage.ToString("0") + "%";
    }

    public void Init()
    {
        Shooting shooting = new Shooting();

        point_lane_1 = Shooting.point_lane_1;
        point_lane_2 = Shooting.point_lane_2;
        point_lane_3 = Shooting.point_lane_3;

        lane_1_impact_list = Shooting.lane_1_impact_list;
        lane_2_impact_list = Shooting.lane_2_impact_list;
        lane_3_impact_list = Shooting.lane_3_impact_list;

        numLane1ShotsFired = Shooting.numLane1ShotsFired;
        numLane2ShotsFired = Shooting.numLane2ShotsFired;
        numLane3ShotsFired = Shooting.numLane3ShotsFired;

        numLane1ShotsMissed = Shooting.numLane1ShotsMissed;
        numLane2ShotsMissed = Shooting.numLane2ShotsMissed;
        numLane3ShotsMissed = Shooting.numLane3ShotsMissed;

        lane1ActiveTimeCounter = Shooting.lane1TimeActiveCounter;
        lane2ActiveTimeCounter = Shooting.lane2TimeActiveCounter;
        lane3ActiveTimeCounter = Shooting.lane3TimeActiveCounter;

        lane1TrainingPercentage = Shooting.lane1PercentagePoints;
        lane2TrainingPercentage = Shooting.lane2PercentagePoints;
        lane3TrainingPercentage = Shooting.lane3PercentagePoints;
    }

    public void handleShotsReplayButton()
    {
        lane_1_impact_index = 0;
        lane_2_impact_index = 0;
        lane_3_impact_index = 0;
        Init();

        if (isShotsReplayOpen)
        {
            shotsReplayPanel.SetActive(false);
            adminScorePanel.SetActive(true);
            shooterScorePanel.SetActive(true);

            if (activeScene.ToLower().Contains("distancesimulator"))
            {
                foreach (GameObject bullet in lane_1_impact_list)
                {
                    bullet.SetActive(true);
                    bullet.GetComponent<BulletManager>().time_canvas.SetActive(false);
                }
            }
            else
            {
                closeAllPoints();
            }

            isShotsReplayOpen = false;
        }
        else //not open
        {
            shooterScorePanel.SetActive(false);
            adminScorePanel.SetActive(false);
            splitTimePanel.SetActive(false);
            shotsReplayPanel.SetActive(true);

            DisableScoreBoards();
            this.GetComponent<ReplayCounter>().ResetReplay();

            // setReplaySetting();
            isShotsReplayOpen = true;
        }
    }

    private void setReplaySetting()
    {
        //current_replay_lane = 1;
        lane_1_impact_index = 0;
        lane_2_impact_index = 0;
        lane_3_impact_index = 0;

        DisableLaneBullets();

        switch (current_replay_lane)
        {
            case 1:
                closeAllPoints();
                allShotsDisplayed = false;
                //replay_camera_1.SetActive(true);
                //replay_camera_2.SetActive(false);
                //replay_camera_3.SetActive(false);

                replay_lane_header.text = "Lane: " + current_replay_lane.ToString();

                if (lane_1_impact_list.Count != 0)
                {
                    lane_1_impact_list[lane_1_impact_index].SetActive(true);
                    lane_1_impact_list[lane_1_impact_index].GetComponent<BulletManager>().time_canvas.SetActive(true);
                    HandleTargetHit(lane_1_impact_list[lane_1_impact_index], "enable");
                }

                SetTargetHit(lane_1_impact_list);
                shots_replay_counter.text = "Shot: " + (lane_1_impact_index + 1).ToString();

                break;

            case 2:
                closeAllPoints();
                allShotsDisplayed = false;
                //replay_camera_1.SetActive(false);
                //replay_camera_2.SetActive(true);
                //replay_camera_3.SetActive(false);

                replay_lane_header.text = "Lane: " + current_replay_lane.ToString();

                if (lane_2_impact_list.Count != 0)
                {
                    lane_2_impact_list[lane_2_impact_index].SetActive(true);
                    lane_2_impact_list[lane_2_impact_index].GetComponent<BulletManager>().time_canvas.SetActive(true);
                    HandleTargetHit(lane_2_impact_list[lane_2_impact_index], "enable");
                }

                SetTargetHit(lane_2_impact_list);
                shots_replay_counter.text = "Shot: " + (lane_2_impact_index + 1).ToString();
                break;

            case 3:
                closeAllPoints();
                allShotsDisplayed = false;
                //replay_camera_1.SetActive(false);
                //replay_camera_2.SetActive(false);
                //replay_camera_3.SetActive(true);

                replay_lane_header.text = "Lane: " + current_replay_lane.ToString();
                if (lane_3_impact_list.Count != 0)
                {
                    lane_3_impact_list[lane_3_impact_index].SetActive(true);
                    lane_3_impact_list[lane_3_impact_index].GetComponent<BulletManager>().time_canvas.SetActive(true);
                    HandleTargetHit(lane_3_impact_list[lane_3_impact_index], "enable");
                }

                SetTargetHit(lane_3_impact_list);
                shots_replay_counter.text = "Shot: " + (lane_3_impact_index + 1).ToString();
                break;

        }


    }

    private void SetTargetHit(List<GameObject> bullets)
    {
        //DisableTargetsHit();
        targets = new List<GameObject>();
        move_to_next_target = false;
        move_to_prev_target = false;
        //RisingPlatesDisabler.disablePlates();

        if (bullets.Count != 0)
        {
            //print("Test: Hit setting reached...");
            foreach (GameObject bullet in bullets)
            {
                if (bullet.transform.name.ToLower().Contains("hit") && bullet.GetComponent<BulletManager>().target_hit != null)
                {
                    targets.Add(bullet.GetComponent<BulletManager>().target_hit);
                }
            }
            //GameObject extratarget = GameObject.FindGameObjectWithTag("target");

            //Enable first target
            current_target_index = 0;
            if (targets.Count != 0) targets[current_target_index].SetActive(true);
            if (bullets[0].transform.name.ToLower().Contains("hit")) move_to_next_target = true;
        }

    }

    private void HandleTargetHit(GameObject bullet, string action)
    {
        ///////////////Next///////////////
        if (bullet.transform.name.ToLower().Contains("hit"))
        {
            //print("Test: " + bullet.transform.name);
            //bullet.GetComponent<BulletManager>().target_hit.SetActive(false);
            move_to_next_target = true;
        }
        else
        {
            move_to_next_target = false;
        }

        ///////////////Prev///////////////
        if (bullet.transform.name.ToLower().Contains("prev_revert_point"))
        {
            //print("Test: " + bullet.transform.name);
            move_to_prev_target = true;
            //secondary_prev_point = false;
        }
        else if(bullet.transform.name.ToLower().Contains("prev_revert_point.sec") && action == "prev")
        {
            move_to_prev_target = true;
            //secondary_prev_point = true;
        }
        else
        {
            move_to_prev_target = false;
        }

    }

    private void DisableScoreBoards()
    {
        if (activeScene.ToLower().Contains("1lane"))
        {
            replay_score_1.SetActive(false);
        }
        else if (activeScene.ToLower().Contains("2lane"))
        {
            replay_score_1.SetActive(false);
            replay_score_2.SetActive(false);
        }
        else if (activeScene.ToLower().Contains("3lane"))
        {
            replay_score_1.SetActive(false);
            replay_score_2.SetActive(false);
            replay_score_3.SetActive(false);
        }
    }

    private void closeAllPoints()
    {

        //Close All Cameras
        // replay_camera_1.SetActive(false);
        // replay_camera_2.SetActive(false);
        // replay_camera_3.SetActive(false);

        //Close All Replay Scores
        if (activeScene.ToLower().Contains("1lane"))
        {
            replay_score_1.SetActive(false);
            this.GetComponent<ReplayCounter>().SetupScoreReplay("1", signalOrigin);
        }
        else if (activeScene.ToLower().Contains("2lane"))
        {
            replay_score_1.SetActive(false);
            replay_score_2.SetActive(false);
            this.GetComponent<ReplayCounter>().SetupScoreReplay("1", signalOrigin);
            this.GetComponent<ReplayCounter>().SetupScoreReplay("2", signalOrigin);
        }
        else if (activeScene.ToLower().Contains("3lane"))
        {
            replay_score_1.SetActive(false);
            replay_score_2.SetActive(false);
            replay_score_3.SetActive(false);
            this.GetComponent<ReplayCounter>().SetupScoreReplay("1", signalOrigin);
            this.GetComponent<ReplayCounter>().SetupScoreReplay("2", signalOrigin);
            this.GetComponent<ReplayCounter>().SetupScoreReplay("3", signalOrigin);
        }

    }
    

    public void HandleScorePrinting()
    {
        //print("Printing...");
        switch (current_replay_lane)
        {
            case 1:
                //replay_camera_1.GetComponent<CameraCapture>().CaptureImage("1");
                SendPrintSignal(replay_camera_1, 1);
                break;
            case 2:
                //replay_camera_2.GetComponent<CameraCapture>().CaptureImage("2");
                SendPrintSignal(replay_camera_2, 2);
                break;
            case 3:
                //replay_camera_3.GetComponent<CameraCapture>().CaptureImage("3");
                SendPrintSignal(replay_camera_3, 3);

                break;
        }

    }

    private void SendPrintSignal(GameObject camera, int camera_number)
    {
        float crop_percentage = 0;
        if (activeScene.ToLower().Contains("1lane"))
        {
            crop_percentage = 0.25f;
            camera.GetComponent<CameraCapture>().CaptureAndSaveImage(camera_number, "center", crop_percentage);
        }
        if (activeScene.ToLower().Contains("2lane"))
        {
            crop_percentage = 0.5f;
            switch (camera_number)
            {
                case 1:
                    camera.GetComponent<CameraCapture>().CaptureAndSaveImage(camera_number, "left", crop_percentage);
                    break;

                case 2:
                    camera.GetComponent<CameraCapture>().CaptureAndSaveImage(camera_number, "right", crop_percentage);
                    break;
            }
        }
        if (activeScene.ToLower().Contains("3lane"))
        {

            switch (camera_number)
            {
                case 1:
                    crop_percentage = 0.659f;
                    camera.GetComponent<CameraCapture>().CaptureAndSaveImage(camera_number, "left", crop_percentage);
                    break;

                case 2:
                    crop_percentage = 0.64f;
                    camera.GetComponent<CameraCapture>().CaptureAndSaveImage(camera_number, "center", crop_percentage);
                    break;

                case 3:
                    crop_percentage = 0.685f;
                    camera.GetComponent<CameraCapture>().CaptureAndSaveImage(camera_number, "right", crop_percentage);
                    break;
            }
        }

    }


}
