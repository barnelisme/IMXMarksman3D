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
        if (true)
        {
            if (shotsReplayButton != null)
            {
                shotsReplayButton.SetActive(true);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
        if(StaticVariableManager.isStopTraining && replayTargetsAllSet == false && StaticVariableManager.isScoreDataSet)
        {
            //Open all lanes...
            if (activeScene.ToLower().Contains("plat"))
            {
                if(StaticVariableManager.platesDisabled)
                {
                    ManageScoringReplay();
                    replayTargetsAllSet = true;
                }
            }
            else
            {
                ManageScoringReplay();
                    replayTargetsAllSet = true;
            }
        }

    }

    public void ManageScoringReplay()
    {
        Init();
        setupAdminPanel();
        showLaneShots();

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

        DisableLaneBullets();

    }

    private void DisableLaneBullets()
    {
        foreach (GameObject bullet in lane_1_impact_list)
        {
            bullet.SetActive(false);
            bullet.GetComponent<BulletManager>().main_canvas.SetActive(false);
            bullet.GetComponent<BulletManager>().number_canvas.SetActive(false);

            if (bullet.transform.name.ToLower().Contains("hit") && bullet.GetComponent<BulletManager>().target_hit != null)
            {
                bullet.GetComponent<BulletManager>().target_hit.SetActive(false);
            }

        }
        foreach (GameObject bullet in lane_2_impact_list)
        {
            bullet.SetActive(false);
            bullet.GetComponent<BulletManager>().main_canvas.SetActive(false);
            bullet.GetComponent<BulletManager>().number_canvas.SetActive(false);

            if (bullet.transform.name.ToLower().Contains("hit") && bullet.GetComponent<BulletManager>().target_hit != null)
            {
                bullet.GetComponent<BulletManager>().target_hit.SetActive(false);
            }
        }
        foreach (GameObject bullet in lane_3_impact_list)
        {
            bullet.SetActive(false);
            bullet.GetComponent<BulletManager>().main_canvas.SetActive(false);
            bullet.GetComponent<BulletManager>().number_canvas.SetActive(false);

            if (bullet.transform.name.ToLower().Contains("hit") && bullet.GetComponent<BulletManager>().target_hit != null)
            {
                bullet.GetComponent<BulletManager>().target_hit.SetActive(false);
            }
        }
    }

    private void setupAdminPanel()
    {
        shotsReplayPanel.SetActive(true);
        isShotsReplayOpen = true;
    }

    public void showLaneShots()
    {
        //print("replay lane: " + current_replay_lane);
        int currLane = 1;

        if (activeScene.ToLower().Contains("1lane"))
        {
            setupReplayLane(lane_1_impact_list, replay_score_1, "1", lane_1_impact_index);
        }
        else if (activeScene.ToLower().Contains("2lane"))
        {
            setupReplayLane(lane_1_impact_list, replay_score_1, "1", lane_1_impact_index);
            setupReplayLane(lane_2_impact_list, replay_score_2, "2", lane_2_impact_index);
        }
        else if (activeScene.ToLower().Contains("3lane"))
        {
            setupReplayLane(lane_1_impact_list, replay_score_1, "1", lane_1_impact_index);
            setupReplayLane(lane_2_impact_list, replay_score_2, "2", lane_2_impact_index);
            setupReplayLane(lane_3_impact_list, replay_score_3, "3", lane_3_impact_index);
        }
    }

    private void setupReplayLane(List<GameObject> impactList , GameObject replayScore , string lane,int impactIndex = 0)
    {
        int bullet_number = 0;
        //impactIndex = 0;

        //closeAllPoints();

        foreach (GameObject bullet in impactList)
        {
            bullet.SetActive(true);
            bullet.GetComponent<BulletManager>().main_canvas.SetActive(false);

            bullet_number++;
            bullet.GetComponent<BulletManager>().bullet_number_text.text = bullet_number.ToString();
            bullet.GetComponent<BulletManager>().number_canvas.SetActive(true);
        }
        allShotsDisplayed = true;
        shots_replay_counter.text = "All";
        replayScore.SetActive(true);

        SetTargetHit(impactList);
        EnableTargetsHit();

        //print("Opening");
        //Replay Score
        switch (lane)
        {
            case "1":
                showScore(replay_lane1_shots_fired, replay_lane1_shots_missed, replay_lane1_total_time, replay_lane1_percentage,
                    numLane1ShotsFired, numLane1ShotsMissed, lane1ActiveTimeCounter, lane1TrainingPercentage);
                SendPrintSignal(replay_camera_1, 1); //Capture Image
                break;

            case "2":
                showScore(replay_lane2_shots_fired, replay_lane2_shots_missed, replay_lane2_total_time, replay_lane2_percentage,
                    numLane2ShotsFired, numLane2ShotsMissed, lane2ActiveTimeCounter, lane2TrainingPercentage);
                SendPrintSignal(replay_camera_2, 2); //Capture Image
                break;

            case "3":
                showScore(replay_lane3_shots_fired, replay_lane3_shots_missed, replay_lane3_total_time, replay_lane3_percentage,
                    numLane3ShotsFired, numLane3ShotsMissed, lane3ActiveTimeCounter, lane3TrainingPercentage);
                SendPrintSignal(replay_camera_3, 3); //Capture Image
                break;
        }

    }

    private void showScore(TextMeshProUGUI shortsFiredText, TextMeshProUGUI shortsMissedText, TextMeshProUGUI totalTimeText, TextMeshProUGUI percentageText,
        int shotsFired, int shotsMissed, float activeTime, float percentage)
    {
        shortsFiredText.text = "Shots fired: " + shotsFired.ToString("0");
        shortsMissedText.text = "Shots missed: " + shotsMissed.ToString("0");
        totalTimeText.text = "Total time: " + activeTime.ToString("0.0") + " sec";
        percentageText.text = "Percentage: " + percentage.ToString("0.0") + "%";
    }

    public void Init()
    {
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

        lane1ActiveTimeCounter = Shooting.lane1ActiveTimeCounter;
        lane2ActiveTimeCounter = Shooting.lane2ActiveTimeCounter;
        lane3ActiveTimeCounter = Shooting.lane3ActiveTimeCounter;

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
                    bullet.GetComponent<BulletManager>().main_canvas.SetActive(false);
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

            if (activeScene.ToLower().Contains("distancesimulator"))
            {
                foreach (GameObject bullet in lane_1_impact_list)
                {
                    bullet.SetActive(false);
                    bullet.GetComponent<BulletManager>().main_canvas.SetActive(true);
                }
            }

            setReplaySetting();
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
                    lane_1_impact_list[lane_1_impact_index].GetComponent<BulletManager>().main_canvas.SetActive(true);
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
                    lane_2_impact_list[lane_2_impact_index].GetComponent<BulletManager>().main_canvas.SetActive(true);
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
                    lane_3_impact_list[lane_3_impact_index].GetComponent<BulletManager>().main_canvas.SetActive(true);
                    HandleTargetHit(lane_3_impact_list[lane_3_impact_index], "enable");
                }

                SetTargetHit(lane_3_impact_list);
                shots_replay_counter.text = "Shot: " + (lane_3_impact_index + 1).ToString();
                break;

        }


    }
    public void showNextHit()
    {
        switch (current_replay_lane)
        {
            case 1:
                if (lane_1_impact_index < lane_1_impact_list.Count - 1)
                {
                    lane_1_impact_index = HandlePointIndex(lane_1_impact_index, "next", lane_1_impact_list);
                    closeAllPoints();

                    lane_1_impact_list[lane_1_impact_index].SetActive(true);
                    lane_1_impact_list[lane_1_impact_index].GetComponent<BulletManager>().main_canvas.SetActive(true);
                    ///////Target Hit Area///////
                    SetNextTargetHit();
                    HandleTargetHit(lane_1_impact_list[lane_1_impact_index], "next");

                    SetNextRevertPoint(lane_1_impact_list, lane_1_impact_index);
                    ////////////////////////////
                    shots_replay_counter.text = "Shot: " + (lane_1_impact_index + 1).ToString();

                    if (lane_1_impact_index == lane_1_impact_list.Count - 1)
                    {
                        HandleTargetHit(lane_1_impact_list[lane_1_impact_index], "prev");
                    }
                }
                break;

            case 2:
                if (lane_2_impact_index < lane_2_impact_list.Count - 1)
                {
                    lane_2_impact_index = HandlePointIndex(lane_2_impact_index, "next", lane_2_impact_list);
                    closeAllPoints();

                    lane_2_impact_list[lane_2_impact_index].SetActive(true);

                    lane_2_impact_list[lane_2_impact_index].GetComponent<BulletManager>().main_canvas.SetActive(true);
                    SetNextTargetHit();
                    HandleTargetHit(lane_2_impact_list[lane_2_impact_index], "next");

                    SetNextRevertPoint(lane_2_impact_list, lane_2_impact_index);
                    shots_replay_counter.text = "Shot: " + (lane_2_impact_index + 1).ToString();

                    if (lane_2_impact_index == lane_2_impact_list.Count - 1)
                    {
                        HandleTargetHit(lane_2_impact_list[lane_2_impact_index], "prev");
                    }
                }
                break;

            case 3:
                if (lane_3_impact_index < lane_3_impact_list.Count - 1)
                {
                    lane_3_impact_index = HandlePointIndex(lane_3_impact_index, "next", lane_3_impact_list);
                    closeAllPoints();

                    lane_3_impact_list[lane_3_impact_index].SetActive(true);
                    lane_3_impact_list[lane_3_impact_index].GetComponent<BulletManager>().main_canvas.SetActive(true);
                    SetNextTargetHit();
                    HandleTargetHit(lane_3_impact_list[lane_3_impact_index], "next");

                    SetNextRevertPoint(lane_3_impact_list, lane_3_impact_index);
                    shots_replay_counter.text = "Shot: " + (lane_3_impact_index + 1).ToString();

                    if (lane_3_impact_index == lane_3_impact_list.Count - 1)
                    {
                        HandleTargetHit(lane_3_impact_list[lane_3_impact_index], "prev");
                    }
                }
                break;

        }

    }
    private void SetNextTargetHit()
    {

        print("Test: Move to next target" + move_to_next_target);
        print("Targets Count: " + targets.Count);

        if ((move_to_next_target || secondary_prev_point) && current_target_index < targets.Count - 1 && targets.Count != 0)
        {
            targets[current_target_index].SetActive(false);
            current_target_index++;
            targets[current_target_index].SetActive(true);
            secondary_prev_point = false;
        }
        move_to_next_target = false;
    }
    public void showPrevHit()
    {
        if(activeScene.ToLower().Contains("plat") || activeScene.ToLower().Contains("ipec") || activeScene.ToLower().Contains("rifflepole")) //DropDown.scene_type.ToLower().Contains("static response")
        {
            allShotsDisplayed = true;
            showAllHits();
        }
        else //if(DropDown.scene_type.ToLower().Contains("static"))
        {
            switch (current_replay_lane)
            {
                case 1:
                    if (lane_1_impact_index > 0)
                    {
                        HandleTargetHit(lane_1_impact_list[lane_1_impact_index], "prev");
                        SetPrevTargetHit();

                        lane_1_impact_index = HandlePointIndex(lane_1_impact_index, "prev", lane_1_impact_list);
                        closeAllPoints();

                        lane_1_impact_list[lane_1_impact_index].SetActive(true);
                        lane_1_impact_list[lane_1_impact_index].GetComponent<BulletManager>().main_canvas.SetActive(true);

                        shots_replay_counter.text = "Shot: " + (lane_1_impact_index + 1).ToString();
                        //SetPrevRevertPoint(lane_1_impact_list, lane_1_impact_index);

                        if (lane_1_impact_index == 0)
                        {
                            HandleTargetHit(lane_1_impact_list[lane_1_impact_index], "next");
                        }
                    }
                    break;

                case 2:
                    if (lane_2_impact_index > 0)
                    {
                        HandleTargetHit(lane_2_impact_list[lane_2_impact_index], "prev");
                        SetPrevTargetHit();

                        lane_2_impact_index = HandlePointIndex(lane_2_impact_index, "prev", lane_2_impact_list);
                        closeAllPoints();

                        lane_2_impact_list[lane_2_impact_index].SetActive(true);
                        lane_2_impact_list[lane_2_impact_index].GetComponent<BulletManager>().main_canvas.SetActive(true);

                        shots_replay_counter.text = "Shot: " + (lane_2_impact_index + 1).ToString();
                        //SetPrevRevertPoint(lane_2_impact_list, lane_2_impact_index);

                        if (lane_2_impact_index == 0)
                        {
                            HandleTargetHit(lane_2_impact_list[lane_2_impact_index], "next");
                        }
                    }
                    break;

                case 3:
                    if (lane_3_impact_index > 0)
                    {
                        HandleTargetHit(lane_3_impact_list[lane_3_impact_index], "prev");
                        SetPrevTargetHit();

                        lane_3_impact_index = HandlePointIndex(lane_3_impact_index, "prev", lane_3_impact_list);
                        closeAllPoints();

                        lane_3_impact_list[lane_3_impact_index].SetActive(true);
                        lane_3_impact_list[lane_3_impact_index].GetComponent<BulletManager>().main_canvas.SetActive(true);

                        shots_replay_counter.text = "Shot: " + (lane_3_impact_index + 1).ToString();
                        //SetPrevRevertPoint(lane_3_impact_list, lane_3_impact_index);

                        if (lane_3_impact_index == 0)
                        {
                            HandleTargetHit(lane_3_impact_list[lane_3_impact_index], "next");
                        }
                    }
                    break;

            }
        }

    }
    private void SetPrevTargetHit()
    {
        print("Test: Move to prev target hit " + move_to_prev_target);
        if (move_to_prev_target && current_target_index > 0 && targets.Count != 0)
        {
            targets[current_target_index].SetActive(false);
            current_target_index--;
            targets[current_target_index].SetActive(true);

        }
        move_to_prev_target = false;
    }
    public void showAllHits()
    {
        //print("replay lane: " + current_replay_lane);
        int bullet_number = 0;
        DisableLaneBullets();

        switch (current_replay_lane)
        {
            case 1:
                lane_1_impact_index = 0;
                if (allShotsDisplayed && lane_1_impact_list.Count != 0)
                {
                    closeAllPoints();
                    SetTargetHit(lane_1_impact_list);

                    allShotsDisplayed = false;
                    lane_1_impact_list[lane_1_impact_index].SetActive(true);
                    lane_1_impact_list[lane_1_impact_index].GetComponent<BulletManager>().main_canvas.SetActive(true);
                    shots_replay_counter.text = "Shot: " + (lane_1_impact_index + 1).ToString();
                    replay_score_1.SetActive(false);
                }
                else
                {
                    foreach (GameObject bullet in lane_1_impact_list)
                    {
                        bullet.SetActive(true);
                        bullet.GetComponent<BulletManager>().main_canvas.SetActive(false);

                        bullet_number++;
                        bullet.GetComponent<BulletManager>().bullet_number_text.text = bullet_number.ToString();
                        bullet.GetComponent<BulletManager>().number_canvas.SetActive(true);
                    }
                    allShotsDisplayed = true;
                    shots_replay_counter.text = "All";
                    replay_score_1.SetActive(true);
                    EnableTargetsHit();

                    print("Opening");
                    //Replay Score
                    replay_lane1_shots_fired.text = "Shots fired: " + numLane1ShotsFired.ToString("0");
                    replay_lane1_shots_missed.text = "Shots missed: " + numLane1ShotsMissed.ToString("0");
                    replay_lane1_total_time.text = "Total time: " + lane1ActiveTimeCounter.ToString("0.0") + " sec";
                    replay_lane1_percentage.text = "Percentage: " + lane1TrainingPercentage.ToString("0") + "%";

                }
                break;

            case 2:
                lane_2_impact_index = 0;
                if (allShotsDisplayed && lane_2_impact_list.Count != 0)
                {
                    closeAllPoints();
                    SetTargetHit(lane_2_impact_list);

                    allShotsDisplayed = false;
                    lane_2_impact_list[lane_2_impact_index].SetActive(true);
                    lane_2_impact_list[lane_2_impact_index].GetComponent<BulletManager>().main_canvas.SetActive(true);
                    shots_replay_counter.text = "Shot: " + (lane_2_impact_index + 1).ToString();
                    replay_score_2.SetActive(false);
                }
                else
                {
                    foreach (GameObject bullet in lane_2_impact_list)
                    {
                        bullet.SetActive(true);
                        bullet.GetComponent<BulletManager>().main_canvas.SetActive(false);

                        bullet_number++;
                        bullet.GetComponent<BulletManager>().bullet_number_text.text = bullet_number.ToString();
                        bullet.GetComponent<BulletManager>().number_canvas.SetActive(true);
                    }
                    allShotsDisplayed = true;
                    shots_replay_counter.text = "All";
                    replay_score_2.SetActive(true);
                    EnableTargetsHit();

                    replay_lane2_shots_fired.text = "Shots fired: " + numLane2ShotsFired.ToString("0");
                    replay_lane2_shots_missed.text = "Shots missed: " + numLane2ShotsMissed.ToString("0");
                    replay_lane2_total_time.text = "Total time: " + lane2ActiveTimeCounter.ToString("0.0") + " sec";
                    replay_lane2_percentage.text = "Percentage: " + lane2TrainingPercentage.ToString("0") + "%";

                }
                break;

            case 3:
                lane_3_impact_index = 0;
                if (allShotsDisplayed && lane_3_impact_list.Count != 0)
                {
                    closeAllPoints();
                    SetTargetHit(lane_3_impact_list);

                    allShotsDisplayed = false;
                    lane_3_impact_list[lane_3_impact_index].SetActive(true);
                    lane_3_impact_list[lane_3_impact_index].GetComponent<BulletManager>().main_canvas.SetActive(true);
                    shots_replay_counter.text = "Shot: " + (lane_3_impact_index + 1).ToString();
                    replay_score_3.SetActive(false);
                }
                else
                {
                    foreach (GameObject bullet in lane_3_impact_list)
                    {
                        bullet.SetActive(true);
                        bullet.GetComponent<BulletManager>().main_canvas.SetActive(false);

                        bullet_number++;
                        bullet.GetComponent<BulletManager>().bullet_number_text.text = bullet_number.ToString();
                        bullet.GetComponent<BulletManager>().number_canvas.SetActive(true);
                    }
                    allShotsDisplayed = true;
                    shots_replay_counter.text = "All";
                    replay_score_3.SetActive(true);
                    EnableTargetsHit();

                    replay_lane3_shots_fired.text = "Shots fired: " + numLane3ShotsFired.ToString("0");
                    replay_lane3_shots_missed.text = "Shots missed: " + numLane3ShotsMissed.ToString("0");
                    replay_lane3_total_time.text = "Total time: " + lane3ActiveTimeCounter.ToString("0.0") + " sec";
                    replay_lane3_percentage.text = "Percentage: " + lane3TrainingPercentage.ToString("0") + "%";
                }
                break;

        }

    }
    


    private int HandlePointIndex(int index, string action, List<GameObject> impactList)
    {
        if (action == "next")
        {//bullet[index].transform.name += "prev_revert_point";
            if (impactList[index].transform.name.ToLower().Contains("hit") && impactList[index].transform.name.ToLower().Contains("prev_revert_point.end"))
                impactList[index].transform.name = "Hitprev_revert_point";
            //if (impactList[index].transform.name.ToLower().Contains("miss") && impactList[index].transform.name.ToLower().Contains("prev_revert_point.sec"))
            //impactList[index].transform.name = "Miss";

            index++;

            if (impactList[index].transform.name.ToLower().Contains("hit"))
                impactList[index].transform.name= "Hitprev_revert_point.end";
        }
        else if(action == "prev")
        {
            if (impactList[index].transform.name.ToLower().Contains("hit"))
                impactList[index].transform.name = "Hit";
            if (impactList[index].transform.name.ToLower().Contains("miss"))
                impactList[index].transform.name = "Miss";
            index--;

        }

        return index;
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
    private void SetNextRevertPoint(List<GameObject> bullet, int index)
    {
        if (move_to_next_target)
        {
            if (!bullet[index].transform.name.Contains("prev_revert_point") && bullet[index] != null)
            {
                bullet[index].transform.name += "prev_revert_point";
            }
            if (index + 1 < bullet.Count)
            {
                if(!bullet[index + 1].transform.name.ToLower().Contains("hit"))
                    bullet[index + 1].transform.name = "Missprev_revert_point.sec";
            }
        }
    }
    private void SetPrevRevertPoint(List<GameObject> bullet, int index)
    {
        if (bullet[index].transform.name.ToLower().Contains("hit"))
        {
            if (!bullet[index].transform.name.Contains("prev_revert_point") && bullet[index] != null)
            {
                //bullet[index].transform.name += "prev_revert_point";
            }
            if (index + 1 < bullet.Count)
            {
                if (!bullet[index + 1].transform.name.ToLower().Contains("hit"))
                    bullet[index + 1].transform.name = "Missprev_revert_point.sec";
            }
        }
    }
    private void DisableTargetsHit()
    {
        if (targets.Count != 0)
        {
            foreach (GameObject target in targets)
            {
                target.SetActive(false);
            }
        }

    }
    private void EnableTargetsHit()
    {
        if (targets.Count != 0)
        {
            foreach (GameObject target in targets)
            {
                target.SetActive(true);
            }
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



    private void closeAllPoints()
    {

        foreach (GameObject bullet in lane_1_impact_list)
        {
            bullet.SetActive(false);
            bullet.GetComponent<BulletManager>().main_canvas.SetActive(false);
            bullet.GetComponent<BulletManager>().number_canvas.SetActive(false);
            //HandleTargetHit(bullet, "disable");
        }
        foreach (GameObject bullet in lane_2_impact_list)
        {
            bullet.SetActive(false);
            bullet.GetComponent<BulletManager>().main_canvas.SetActive(false);
            bullet.GetComponent<BulletManager>().number_canvas.SetActive(false);
            //HandleTargetHit(bullet, "disable");
        }
        foreach (GameObject bullet in lane_3_impact_list)
        {
            bullet.SetActive(false);
            bullet.GetComponent<BulletManager>().main_canvas.SetActive(false);
            bullet.GetComponent<BulletManager>().number_canvas.SetActive(false);
            //HandleTargetHit(bullet, "disable");
        }

        //Close All Cameras
        replay_camera_1.SetActive(false);
        replay_camera_2.SetActive(false);
        replay_camera_3.SetActive(false);

        //Close All Replay Scores
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

    public void NextReplayLane()
    {
        if (activeScene.ToLower().Contains("2lane") && current_replay_lane < 2)
        {
            current_replay_lane++;
        }
        if (activeScene.ToLower().Contains("3lane") && current_replay_lane < 3)
        {
            current_replay_lane++;
        }
        replay_lane_header.text = "Lane: " + current_replay_lane.ToString();

        setReplaySetting();
    }
    public void PrevReplayLane()
    {
        if (current_replay_lane > 1)
        {
            current_replay_lane--;
        }
        replay_lane_header.text = "Lane: " + current_replay_lane.ToString();
        setReplaySetting();
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
