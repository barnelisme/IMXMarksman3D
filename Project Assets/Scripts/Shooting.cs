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
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

public class Shooting : MonoBehaviour
{
    // Start is called before the first frame update

    string Url = "http://192.168.137.1/imagistix/datareq.php";
    string stress_vest_url;
    public float UDPSendTimer = 0f;
    public float UDPSendSettingTimer = 0f;
    float UDPSetTime = 0f;
    public bool sendUDP = false;
    public bool sendUDPReset = false;
    public bool sendUDPSimSetting = true;
    public bool bullseye_hit = false;

    string calibrationPath = "";
    string DelaySavePath = "";
    string UDP_IP_SavePath = "";
    public string[] guns;
    public string gunsTextPath = "";
    string AccSavePath = "";

    //UI Button Variables
    public GameObject ui_canvas;
    private GameObject adminCanvas;
    private GameObject countDownCanvas;
    GraphicRaycaster ui_raycaster;
    PointerEventData click_data;
    List<RaycastResult> click_results;
    private bool ui_mouse_pressed = false;
    private bool ui_mouse_reset = false;

    public AudioSource handgunSound;
    public AudioSource baloonPopSound;
    public AudioSource ladyScreamSound;
    public AudioSource humanScreamSound;
    public AudioSource randomTalkSound;
    public AudioSource mallTalkSound;
    public AudioSource CitySound;
    public AudioSource CitySound2;
    public AudioSource ForestSound;
    public AudioSource ForestSound2;

    public AudioClip handgunAudio;
    public AudioClip baloonPopAudio;
    //public AudioClip BottlehandgunAudio;
    public AudioClip ladyscreamAudio;
    public AudioSource buzzerSound;
    public AudioSource gunCockingSound;
    public AudioClip buzzerAudio;
    public AudioClip gunCockingAudio;
    public AudioClip humanScreamAudio;
    public AudioClip randomTalkAudio;
    public AudioClip mallTalkAudio;
    public AudioClip CityAudio;
    public AudioClip CityAudio2;
    public AudioClip ForestAudio;
    public AudioClip ForestAudio2;

    public TextMeshProUGUI totalshots;
    //int bulletCount;
    //public Calibration calibration;
    public GameObject enemySoldier;
    public GameObject enemySoldier2;
    public GameObject Forest_enemySoldier;
    int enemySelectorFlip = 0;
    public int enemySelector;
    public GameObject eric;
    public GameObject alison;
    public GameObject lilly;
    //Containers
    public GameObject oc_eric;
    public GameObject oc_alison;
    public GameObject oc_lilly;

    public GameObject civilian;
    GameObject[] all_civilians;
    GameObject[] all_soldiers;
    GameObject[] all_bullets;
    Boolean flagCalibrator = false;
    /// <summary>
    /// for Gametime variables
    /// </summary>
    //public float damage= 2f;
    public float range = 100f;
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public GameObject blood;
    public GameObject bullethole;
    public GameObject bullethole_2;
    public GameObject bullethole_3;
    private List<GameObject> bulletHoles = new List<GameObject>();
    UdpClient udpClient;
    UdpClient udpClientImg;
    public int portnum = 22222;
    public int portnumImg = 22223;
    Boolean FlagBulletRx = false;
    int CountShotFired = 0;
    Vector2Int CalibratePoint1 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint2 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint3 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint4 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint5 = new Vector2Int(0, 0);

    float Ball1X = 0f;
    float Ball1Y = 0f;
    int CalibrateOffsetX = 0;
    int CalibrateOffsetY = 0;
    Ray ray;

    public string activeScene;
    public string instructor;
    public bool is3DScene = false;
    bool isLiveAmmunition = false;
    static public int mainPlayerLives = 50;

    static public int shotsFired = 0, civilianShot = 0;
    static public int enemyshot = 0;

    static public int numberOfEnemies = 0;
    static public int numberOfCivilians = 0;
    //bruces additions
    //sending udp
    UdpClient udpClientS;
    public int Sportnum;
    IPEndPoint remoteEndPoint;
    private string IP;

    [Header("3D Results Variables Results")]
    public TextMeshProUGUI score;
    public TextMeshProUGUI Result;
    public TextMeshProUGUI civiliansShot;
    public TextMeshProUGUI enemiesKilled;
    public TextMeshProUGUI totalEnemies;
    public TextMeshProUGUI Location;
    public TextMeshProUGUI trainee_name;
    public TextMeshProUGUI total_3d_training_time;
    public TextMeshProUGUI total_3d_shots_fired;


    public TextMeshProUGUI winnerScore_carGame;
    public TextMeshProUGUI life;
    public GameObject shooterScorePanel;
    public GameObject ScoreCanvas;
    public GameObject splitTimePanel;
    public GameObject adminScorePanel;
    public GameObject mainPanel;
    public TextMesh buzzerText;
    public bool isSplitTimeOpen = false;
    static bool isScoreOpen = false;
    bool isShotsReplayOpen = false;
    bool allShotsDisplayed = false;
    private bool chrono = false;
    static public bool buzzerFlag = false;
    //public Button startBTN;
    public TMP_InputField TimeLimit;
    bool dead = false;

    public TextMeshProUGUI performanceScore;
    static public string targetHitParent = " ";
    static public string targetHit = " ";
    //static public bool shot = false;
    float xPos = 0f;
    float yPos = 0f;
    static string unityCamValues = "0:0";
    public static float accSpot_xPos = 0f;
    public static float accSpot_yPos = 0f;

    Vector2 point = new Vector2();
    static public List<Vector2> bulletPoint1 = new List<Vector2>();
    static public List<Vector2> bulletPoint2 = new List<Vector2>();
    static public List<Vector2> bulletPoint3 = new List<Vector2>();
    List<Vector2> bulletPoint4 = new List<Vector2>();
    List<Vector2> bulletPoint5 = new List<Vector2>();

    public TextMesh timerText;
    static public float startTime = 0;
    private float buzzerTime = 5f;

    float minGrouping = 0.236f;//0.190f;
    float medGrouping = 0.282f;//0.236f;
    float maxGrouping = 0.328f;
    static public string rangeTime = "0";

    //Reference Valuables
    public ControlCenter controlCenter;
    public bool RestartPressed = false;
    public GameObject player;
    public GameObject ColorDynamic;
    public GameObject bottleMan;

    //New Operation Varibales
    private int shootingPrecision = 70;
    private int ShootCondition = 0;
    float VolumeValue;
    public float shootingTimeOut = 60;
    bool isShootState = false;
    public LayerMask layerMask;
    public float carLife = 2;
    bool GameOver = false;
    public int numEnemies = 0;
    public int reloadCount = 15;
    public float reloadTimeOut = 4;
    public TextMeshProUGUI ShootSateText;
    public TextMeshProUGUI timeActive;
    public GameObject ReloadIndicator;


    //Admin view Objects
    public TextMeshProUGUI shootDelayAdmin;
    public TextMeshProUGUI gunTypeAdmin;
    public TextMeshProUGUI camStatusAdmin;
    public static float timeActiveCounter;
    float lane1TimeActiveCounter = 0;
    float lane2TimeActiveCounter = 0;
    float lane3TimeActiveCounter = 0;
    //static string timeactive = timeActiveCounter.ToString();

    public TextMeshProUGUI ShootDelayTxt; //Shooting delay TXT gameobject variables
    float ShootDelayTxt_timeout = 1;      //shooting delay display timeout
    float incrementDelay = 0.0f;          //ShootDelay button increment delay
    float delaySetting = 0;
    bool isShootDelay = false;            //button pressed code trigger

    float shootTextTimeOut = 1;
    public float shootDelay = 0;
    bool shotFired = false;
    public GameObject aimingImage;

    //File stream variables
    public static string fileData;
    public static string UDP_ClientIP_Address;

    //Hide Shoot Variables
    [Header("VALUE")]
    [Space(5)]
    [SerializeField] int rayDistance = 1000;
    [Space(5)]
    [SerializeField] float hitTime = 0.5f;
    [Space(5)]
    [SerializeField] LayerMask layer;

    [Header("COMPONENTS")]
    [Space(5)]
    [SerializeField] new Camera camera;
    [Space(5)]
    [SerializeField] AudioClip audioClip;
    [Space(5)]
    [SerializeField] GameObject hitUi;
    [Space(5)]

    new AudioSource audio;
    Animator _anim;
    Transform enemy;
    bool isReset = false, isSaveImage = false, isSaveVideo = false;
    bool isCalibrate = false;
    int udpSendOnceFlag = 0;

    //Outdoord BLOCK Shooting Varibales
    public static int numBlockesHit = 0;
    public int totalNumBlockes = 24;

    //Script Refernece variables
    GetTrainees getTrainees;
    public GameObject Trainees;
    ProjectVariables project_variables;

    //SCENE: Target popUp Variables
    public static int headShots;
    public static int bodyShots;
    public TextMeshProUGUI HeadShots;
    public TextMeshProUGUI BodyShots;
    WallTargetControl targetControl;
    public static bool isChangeColor = false;
    float colorChangeTimer;
    float colorChangeTimerReset = 0.2f;
    Material currentTargetColr;
    static Material hitColor;
    float r, g, b, a;

    //SCENE: Palling Plate
    int platesHit = 0;
    int totPlates = 20;
    int plate_score = 0;

    //SCENE: BasicResetableTargets
    [Header("Resertable targets")]
    public AudioSource platHitSound;
    public AudioClip platHitAudio;

    //GAME SETTINGS
    string sim_ammo_setting;
    public GameObject assistPanel;
    public TextMeshProUGUI assistText;
    static bool isAssistOpen;
    bool f1Pressed = false;
    float f1OpenHoldCountDown = 0.1f;
    float f1CloseHoldCountDown = 0.1f;
    float f1HoldSetValue = 0.1f;

    //SCENE: IPEC Boad
    int boardHit = 0;
    bool targetFinished = false;
    bool boardDestroyed = false;

    public static List<string> playerPosition;
    public GameObject acc_Bullet;
    public GameObject colorHolder;
    [Header("IPEC Scene")]
    public GameObject head_1;
    public GameObject head_2;
    public GameObject body_1;
    public GameObject body_2;
    public GameObject body_3;

    [Header("Falling plate Scene")]
    public TextMeshProUGUI targetText;

    //SCENE: Accurate point Calibration
    static bool isAccMode = false;
    float Ey = 0;
    float Ex = 0;
    float Emin = 1;
    float y_CentrePos = 0;
    float x_CentrePos = 0;
    float y_LowestMax = 0;
    float y_HighestMax = 0;
    float bottom_x_LowestMax = 0;
    float upper_x_LowestMax = 0;
    float bottom_x_HighestMax = 0;
    float upper_x_HighestMax = 0;


    //public InputField error_input;
    //-calibration, Variables
    //-lowest point
    public static float lowPoint_xPos = 0f;
    public static float lowPoint_yPos = 0f;
    static float low_XErrorPoint = 0;
    static float low_YErrorPoint = 0;       //lowest y error point
    //-highest point
    public static float highPoint_xPos = 0f;
    public static float highPoint_yPos = 0f;
    static float high_point_xError = 0;
    static float high_point_yError = 0;     //highest y error point

    //-far Up left point
    public static float upLeftPoint_xPos = 0f;
    public static float upLeftPoint_yPos = 0f;
    static float upLeft_point_xError = 0;
    static float upLeft_point_yError = 0;
    //-far Lower left point
    public static float lowerLeftPoint_xPos = 0f;
    public static float lowerLeftPoint_yPos = 0f;
    static float lowerLeft_point_xError = 0;
    static float lowerLeft_point_yError = 0;

    //-far Up Right point
    public static float upRightPoint_xPos = 0f;
    public static float upRightPoint_yPos = 0f;
    static float upRight_point_xError = 0;
    static float upRight_point_yError = 0;
    //-far Lower Right point
    public static float lowerRightPoint_xPos = 0f;
    public static float lowerRightPoint_yPos = 0f;
    static float lowerRight_point_xError = 0;
    static float lowerRight_point_yError = 0;


    static float LwError = 0;
    static float HP_errorValue = 0;
    static float LfP_errorValue = 0;
    static float RP_errorValue = 0;

    //ADMIN
    [Header("Main screen points")]
    public TextMeshProUGUI left_pointTxt;
    public TextMeshProUGUI right_pointTxt;
    public TextMeshProUGUI low_pointTxt;
    public TextMeshProUGUI high_pointTxt;
    public TextMeshProUGUI accP_pointTx;
    public TextMeshProUGUI errorP_pointTx;

    [Header("Low Error Variables")]
    public TextMeshProUGUI low_y_max_txt;
    public TextMeshProUGUI low_error_correcPoint_txt;
    public TextMeshProUGUI errorValue_txt;
    public TextMeshProUGUI eyValue_txt;
    public TextMeshProUGUI orgBallpoint_txt;
    public TextMeshProUGUI newBallpoint_txt;

    [Header("High Error Variables")]
    public TextMeshProUGUI high_y_max_txt;
    public TextMeshProUGUI high_error_correcPoint_txt;
    public TextMeshProUGUI H_errorValue_txt;
    public TextMeshProUGUI H_eyValue_txt;
    public TextMeshProUGUI H_orgBallpoint_txt;
    public TextMeshProUGUI H_newBallpoint_txt;

    [Header("Left Error Variables")]
    public TextMeshProUGUI left_x_max_txt;
    public TextMeshProUGUI left_error_correcPoint_txt;
    public TextMeshProUGUI L_errorValue_txt;
    public TextMeshProUGUI L_exValue_txt;
    public TextMeshProUGUI L_orgBallpoint_txt;
    public TextMeshProUGUI L_newBallpoint_txt;

    [Header("Right Error Variables")]
    public TextMeshProUGUI right_x_max_txt;
    public TextMeshProUGUI right_error_correcPoint_txt;
    public TextMeshProUGUI R_errorValue_txt;
    public TextMeshProUGUI R_exValue_txt;
    public TextMeshProUGUI R_orgBallpoint_txt;
    public TextMeshProUGUI R_newBallpoint_txt;

    //Acc Switches
    [Header("Screen Switches")]
    bool lowerAcc = true;
    bool lowerLeftAcc = false, lowerRightAcc = false;
    bool upAcc = false;
    bool upLeftAcc = false, upRightAcc = false;
    bool leftAcc = false;
    bool rightAcc = false;
    public GameObject topIndicator;
    public GameObject bottomIndicator;
    public GameObject leftIndicator;
    public GameObject rightIndicator;

    // Canca variables
    public Image accLight;
    string AccSaveData;
    static int num_targets_input = 10;
    public TMP_InputField numTargetInput;
    int currentInput = 0;
    int inputChangeSwitch = 0;

    //Shot Ignoring and reset variables
    List<string> listOfPoint = new List<string>();
    bool pointInRadiusG = false, isResetDelay = false;
    bool perform_AutoReset = true;
    public GameObject standby_txt;
    public static bool startTraining = false, mouseShotFire = false;
    bool timeFinished = false, targetsFinished = false, ammaFinished = false;
    float resetIgnoreDelay = 2.1f;
    float setIgnoreDelay = 2.1f;
    bool trainingStarted = false;
    float runtime_before_standby = 10f;
    float runtime_reset = 10f;
    bool isShooting = false;
    float restartTimer = 7;
    bool trainingPaused = false;
    bool final_target_flag = false;

    //SCENE: Lane Selectable
    int point_1_hits = 0;
    int point_2_hits = 0;
    int point_3_hits = 0;
    int point_4_hits = 0;
    int point_5_hits = 0;
    int lane1_strike_count = 0;
    int lane2_strike_count = 0;
    int lane3_strike_count = 0;
    List<string> lane1SplitTime = new List<string>(); //Split time memory
    List<string> lane2SplitTime = new List<string>();
    List<string> lane3SplitTime = new List<string>();
    List<string> lane4SplitTime = new List<string>();
    List<string> lane1ResponseTime = new List<string>(); //Split time memory
    List<string> lane2ResponseTime = new List<string>();
    List<string> lane3ResponseTime = new List<string>();
    List<string> lane4ResponseTime = new List<string>();
    string lane1SplitTimeString = ""; //split time string addition chain
    string lane2SplitTimeString = "";
    string lane3SplitTimeString = "";
    string lane4SplitTimeString = "";
    string lane1ResponseTimeString = ""; //response time string addition chain
    string lane2ResponseTimeString = "";
    string lane3ResponseTimeString = "";
    string lane4ResponseTimeString = "";
    float lane1SplitTimeCounter = 0;  //split time counter
    float lane2SplitTimeCounter = 0;
    float lane3SplitTimeCounter = 0;
    float lane4SplitTimeCounter = 0;
    float lane1ResponseTimeCounter = 0;  //response time counter
    float lane2ResponseTimeCounter = 0;
    float lane3ResponseTimeCounter = 0;
    float lane4ResponseTimeCounter = 0;
    bool startLane1Count = false;  //timer flag
    bool startLane2Count = false;
    bool startLane3Count = false;
    bool startLane4Count = false;
    bool startLane1ResponseCount = false;  //timer flag
    bool startLane2ResponseCount = false;
    bool startLane3ResponseCount = false;
    bool startLane4ResponseCount = false;
    public TextMeshProUGUI lane1SplitTimeDisplay;  //Split-time displays
    public TextMeshProUGUI lane2SplitTimeDisplay;
    public TextMeshProUGUI lane3SplitTimeDisplay;
    public TextMeshProUGUI lane4SplitTimeDisplay;
    public TextMeshProUGUI lane1ShotsFiredDisplay;  //Shots fired displays
    public TextMeshProUGUI lane2ShotsFiredDisplay;
    public TextMeshProUGUI lane3ShotsFiredDisplay;
    public TextMeshProUGUI lane4ShotsFiredDisplay;
    public TextMeshProUGUI lane1TraineeNameDisplay;
    public TextMeshProUGUI lane2TraineeNameDisplay;
    public TextMeshProUGUI lane3TraineeNameDisplay;
    int numLane1Splits = 0;   //total number of split times
    int numLane2Splits = 0;
    int numLane3Splits = 0;
    int numLane4Splits = 0;
    int numLane1ResponseTimes = 0;   //total number of response times times
    int numLane2ResponseTimes = 0;
    int numLane3ResponseTimes = 0;
    int numLane4ResponseTimes = 0;
    public static int numLane1ShotsFired = 0;   //total number of shots fired
    public static int numLane2ShotsFired = 0;
    public static int numLane3ShotsFired = 0;
    int numLane4ShotsFired = 0;
    public static int numLane1ShotsMissed = 0;   //total number of shots Missed
    public static int numLane2ShotsMissed = 0;
    public static int numLane3ShotsMissed = 0;
    int numLane4ShotsMissed = 0;
    public Camera adminCam;
    public static List<string> point_lane_1 = new List<string>(); //score storage
    public static List<string> point_lane_2 = new List<string>();
    public static List<string> point_lane_3 = new List<string>();
    public static List<GameObject> lane_1_impact_list = new List<GameObject>(); //score storage
    public static List<GameObject> lane_2_impact_list = new List<GameObject>();
    public static List<GameObject> lane_3_impact_list = new List<GameObject>();
    private int lane_1_impact_index = 0,lane_2_impact_index = 0,lane_3_impact_index = 0;
    int laneSelected = 1;
    int lane1PointsHit = 0;  //target hit count
    int lane2PointsHit = 0;
    int lane3PointsHit = 0;
    int lane4PointsHit = 0;
    public static bool lane1TargetsComplete = false; //stop condition
    public static bool lane2TargetsComplete = false;
    public static bool lane3TargetsComplete = false;
    public static bool lane4TargetsComplete = false;
    public static float lane1TrainingScorePoints = 0;
    public static float lane2TrainingScorePoints = 0;
    public static float lane3TrainingScorePoints = 0;
    string lane1TraineeName = "";
    string lane2TraineeName = "";
    string lane3TraineeName = "";
    public static float lane1PercentagePoints = 0;
    public static float lane2PercentagePoints = 0;
    public static float lane3PercentagePoints = 0;
    bool lane1SplitTimeSet = false;
    bool lane2SplitTimeSet = false;
    bool lane3SplitTimeSet = false;
    List <string> sceneScores, scenePercentages, sceneTargetsHit, sceneShotsMissed, laneTraineeNames, laneSplitTime, laneTrainingTime, laneResponseTime;
    public TextMeshProUGUI lane_number_Txt;
    bool adminScoreUpdated = false;
    int scoreScrollValue = 1;
    string[] lane1Header, lane2Header, lane3Header;

    [Header("Admin Results")]
    public TextMeshProUGUI admin_Lane1_Header;
    public TextMeshProUGUI admin_Lane1_ShotsFiredTxt;
    public TextMeshProUGUI admin_Lane1_ShotsMissedTxt;
    public TextMeshProUGUI admin_Lane1_HitTimeTxt;
    public TextMeshProUGUI admin_Lane1_HitShotsTxt;
    public TextMeshProUGUI admin_Lane1_ScoreTxt;
    public TextMeshProUGUI admin_Lane1_PercentageTxt;
    public TextMeshProUGUI admin_Lane2_Header;
    public TextMeshProUGUI admin_Lane2_ShotsFiredTxt;
    public TextMeshProUGUI admin_Lane2_ShotsMissedTxt;
    public TextMeshProUGUI admin_Lane2_HitTimeTxt;
    public TextMeshProUGUI admin_Lane2_HitShotsTxt;
    public TextMeshProUGUI admin_Lane2_ScoreTxt;
    public TextMeshProUGUI admin_Lane2_PercentageTxt;
    public TextMeshProUGUI admin_Lane3_Header;
    public TextMeshProUGUI admin_Lane3_ShotsFiredTxt;
    public TextMeshProUGUI admin_Lane3_ShotsMissedTxt;
    public TextMeshProUGUI admin_Lane3_HitTimeTxt;
    public TextMeshProUGUI admin_Lane3_HitShotsTxt;
    public TextMeshProUGUI admin_Lane3_ScoreTxt;
    public TextMeshProUGUI admin_Lane3_PercentageTxt;
    public TextMeshProUGUI admin_Lane4_Header;
    public TextMeshProUGUI admin_Lane4_ShotsFiredTxt;
    public TextMeshProUGUI admin_Lane4_ShotsMissedTxt;
    public TextMeshProUGUI admin_Lane4_HitTimeTxt;
    public TextMeshProUGUI admin_Lane4_HitShotsTxt;
    public TextMeshProUGUI admin_Lane4_ScoreTxt;
    public TextMeshProUGUI admin_Lane4_PercentageTxt;
    public GameObject distanceSimulatorPanel;

    [Header("Shooter Results")]
    public TextMeshProUGUI shooter_Lane1_Header;
    public TextMeshProUGUI shooter_Lane1_SplitTime_Header;
    public TextMeshProUGUI shooter_Lane1_ShotsFiredTxt;
    public TextMeshProUGUI shooter_Lane1_ShotsMissedTxt;
    public TextMeshProUGUI shooter_Lane1_HitTimeTxt;
    public TextMeshProUGUI shooter_Lane1_HitShotsTxt;
    public TextMeshProUGUI shooter_Lane1_ScoreTxt;
    public TextMeshProUGUI shooter_Lane1_PercentageTxt;
    public TextMeshProUGUI shooter_Lane2_Header;
    public TextMeshProUGUI shooter_Lane2_SplitTime_Header;
    public TextMeshProUGUI shooter_Lane2_ShotsFiredTxt;
    public TextMeshProUGUI shooter_Lane2_ShotsMissedTxt;
    public TextMeshProUGUI shooter_Lane2_HitTimeTxt;
    public TextMeshProUGUI shooter_Lane2_HitShotsTxt;
    public TextMeshProUGUI shooter_Lane2_ScoreTxt;
    public TextMeshProUGUI shooter_Lane2_PercentageTxt;
    public TextMeshProUGUI shooter_Lane3_Header;
    public TextMeshProUGUI shooter_Lane3_SplitTime_Header;
    public TextMeshProUGUI shooter_Lane3_ShotsFiredTxt;
    public TextMeshProUGUI shooter_Lane3_ShotsMissedTxt;
    public TextMeshProUGUI shooter_Lane3_HitTimeTxt;
    public TextMeshProUGUI shooter_Lane3_HitShotsTxt;
    public TextMeshProUGUI shooter_Lane3_ScoreTxt;
    public TextMeshProUGUI shooter_Lane3_PercentageTxt;
    public TextMeshProUGUI shooter_Lane4_Header;
    public TextMeshProUGUI shooter_Lane4_SplitTime_Header;
    public TextMeshProUGUI shooter_Lane4_ShotsFiredTxt;
    public TextMeshProUGUI shooter_Lane4_ShotsMissedTxt;
    public TextMeshProUGUI shooter_Lane4_HitTimeTxt;
    public TextMeshProUGUI shooter_Lane4_HitShotsTxt;
    public TextMeshProUGUI shooter_Lane4_ScoreTxt;
    public TextMeshProUGUI shooter_Lane4_PercentageTxt;


    [Header("Stop Indicators")]
    public GameObject lane1StopSignal;
    public GameObject lane2StopSignal;
    public GameObject lane3StopSignal;

    [Header("Lane Timers")]
    public TextMeshProUGUI lane1Timer;
    public TextMeshProUGUI lane2Timer;
    public TextMeshProUGUI lane3Timer;
    public static float lane1ActiveTimeCounter;
    public static float lane2ActiveTimeCounter;
    public static float lane3ActiveTimeCounter;
    float lane4ActiveTimeCounter;

    [Header("Baloon Scene Variables")]
    public GameObject[] lane1StrikesArray;
    public GameObject[] lane2StrikesArray;
    public GameObject lane1StrikeOutSignal;
    public GameObject lane2StrikeOutSignal;
    bool lane1SrikeOut = false;
    bool lane2SrikeOut = false;
    bool lane3SrikeOut = false;
    public TextMeshProUGUI strikesScoreText;

    [Header("Suspect Shoot Scene")]
    public GameObject lane1ResultPanel;
    public GameObject lane2ResultPanel;
    public TextMeshProUGUI lane1Result;
    public TextMeshProUGUI lane2Result;
    private bool lane1Scored = false, lane2Scored = false;
    private bool lane1Missed = false, lane2Missed = false;
    int lane1ThreatsShot = 0, lane1NonThreatsShot = 0;
    int lane2ThreatsShot = 0, lane2NonThreatsShot = 0;

    Matrix<double> homographyMatrix = Matrix<double>.Build.Dense(3, 3); // 3x3 empty matrix

    public void Start()
    {
        try
        {
            activeScene = SceneManager.GetActiveScene().name;
            ResetUIPanels();
            ResetStaticVariables();
            SetDataDirectory();

            ReloadIndicator = GameObject.FindGameObjectWithTag("ReloadIndicator");
            ReloadIndicator.SetActive(false);
            bullethole_2 = bullethole_3;

            if (activeScene.ToLower().Contains("suspect") || activeScene.ToLower().Contains("baloon"))
            {

            }
            else
            {
                startLane1ResponseCount = true;
                startLane2ResponseCount = true;
                startLane3ResponseCount = true;
                startLane4ResponseCount = true;
            }

            if (activeScene.ToLower().Contains("hunting"))
            {
                player.GetComponent<FirstPersonController>().enabled = false;
            }

            loadBackgroundColor();
            loadTraineeNames();
            trainingPaused = false;



            trainingStarted = true;
            isReset = true;
            startTraining = false;
            if (Scoring.ammo_setting.ToLower().Contains("laser"))
            {
                Calibration.isAccMode = false;
            }
            isAccMode = Calibration.isAccMode;

            Reset_Time();
            headShots = 0;
            bodyShots = 0;

            timeActiveCounter = 0;
            lane1ActiveTimeCounter = 0;
            lane2ActiveTimeCounter = 0;
            lane3ActiveTimeCounter = 0;
            lane4ActiveTimeCounter = 0;

            lane1TargetsComplete = false;
            lane2TargetsComplete = false;
            lane3TargetsComplete = false;
            lane4TargetsComplete = false;

            lane1SrikeOut = false;
            lane2SrikeOut = false;
            lane3SrikeOut = false;

            numBlockesHit = 0;
            CreateFile();
            ReadFile();
            timeFinished = false;
            targetsFinished = false;
            targetFinished = false;
            ammaFinished = false;
            sim_ammo_setting = Scoring.ammo_setting;
            initTargetLanes();

            //global_time_warning = GameObject.FindGameObjectWithTag("warning");
            if (activeScene != " ")
            {
                //standby_txt = GameObject.FindGameObjectWithTag("standbytxt");
                //standby_txt.SetActive(false);
            }
            sortTrainingTargert();

            //Manage User Guide start conditions
            isAssistOpen = StaticVariableManager.isAssistOpen;
            if (login_Manager.EmailText.ToLower().Contains("range"))
            {
                isAssistOpen = false;
            }
            else
            {
                isAssistOpen = true;
            }

            if (isAssistOpen)
            {
                openAssistance();
            }
            //_END Guide Conditions_//

            if (Scoring.ammo_setting.ToLower().Contains("live"))
            {
                delaySetting = 0.0f;
            }
            else
            {
                print("RE: Delay is " + fileData);
                delaySetting = float.Parse(fileData);
            }

            if (delaySetting <= 0 || delaySetting == null)
            {
                delaySetting = 0;
            }

            shootDelayAdmin.text = "Shoot Delay: " + delaySetting.ToString("0.0");
            gunTypeAdmin.text = "Gun Type: " + Scoring.gun;
            hitUi.SetActive(false);
            instructor = (login_Manager.EmailText);

            UDPSetTime = 0.05f;
            UDPSendTimer = UDPSetTime;
            UDPSendSettingTimer = UDPSetTime;

            numEnemies = 2;
            GameOver = false;
            ColorDynamic.SetActive(false);
            mainPlayerLives = 10;
            timeActive.enabled = true;

            LoadSounds();
            shootingTimeOut = 60 * 3;
            MultipleScreens();
            enemySelector = 1;//Random.Range(1, 3);

            //aimingImage.SetActive(true);

            ConfigureGuns();
            if (activeScene.ToLower().Contains("range"))
            {
                resetIndoorPapers();
            }

            if (activeScene.ToLower().Contains("basic") && Scoring.ammo_setting.ToLower().Contains("live"))
            {
                //Scoring.simulation_type = "training";
            }

            if (activeScene.ToLower().Contains("hunting"))
            {
                StaticVariableManager.totalTargetAnimalsKilled = 0;
                StaticVariableManager.totalTargetCasualtiesKilled = 0;
                StaticVariableManager.totalHeadShots = 0;
                StaticVariableManager.totalBodyShots = 0;
            }


            //print("RE: try 1 complete...");
            ShootSateText.enabled = false;
            ShootDelayTxt.enabled = false;
        }
        catch (Exception e)
        {
            Debug.Log("Error when instantiating uDp:" + e.Message);

            
        }


        try
        {
            all_soldiers = GameObject.FindGameObjectsWithTag("soldier");
            //numberOfEnemies = all_soldiers.Length;
            VolumeValue = 0.5f;

            LoadCalibration();
            udpClient = new UdpClient(portnum);
            udpClientImg = new UdpClient(portnumImg);
            remoteEndPoint = null;
            init();
            shooterScorePanel.SetActive(false);
            InitialiseVariables();
            LoadSounds();

            //Debug.Log("Active scene is:" + activeScene);
            if (activeScene.ToLower().Contains("range")/*|| activeScene.ToLower().Contains("bottle")*/)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                is3DScene = false;
                //FirstPersonController.
                //Byte[] sendBytes = Encoding.ASCII.GetBytes("Indoor");
                //udpClient.Send(sendBytes, "Indoor".Length, "127.0.0.1", 48901);

            }
            else if (activeScene.ToLower().Contains("hunting"))
            {
                is3DScene = true;
            }
            else if (activeScene == "OpenPlain")
            {
                is3DScene = true;
                ///Byte[] sendBytes = Encoding.ASCII.GetBytes("OpenPlain");
                //udpClient.Send(sendBytes, "Indoor".Length, "127.0.0.1", 48901);
                LoadEnemySoldier(278, 330, 266, 300);
            }
            else if (activeScene.ToLower() == "minicity")
            {
                is3DScene = true;
                ///Byte[] sendBytes = Encoding.ASCII.GetBytes("OpenPlain");
                //udpClient.Send(sendBytes, "Indoor".Length, "127.0.0.1", 48901);
                LoadEnemySoldier(278, 330, 266, 300);
            }

            else if (activeScene.ToLower() == "Crouching Targets_Rain")
            {
                is3DScene = true;
                ///Byte[] sendBytes = Encoding.ASCII.GetBytes("OpenPlain");
                //udpClient.Send(sendBytes, "Indoor".Length, "127.0.0.1", 48901);
                //LoadEnemySoldier(278, 330, 266, 300);
            }


            else if (activeScene == "Mall" || activeScene == "Underground_parking" || activeScene.ToLower().Contains("restaurant") || (activeScene.ToLower().Contains("outdoor") || activeScene.ToLower().Contains("rooms") && !activeScene.ToLower().Contains("range")) && !activeScene.ToLower().Contains("hunting"))
            {
                is3DScene = true;
                //Byte[] sendBytes = Encoding.ASCII.GetBytes("Mall");
                //udpClient.Send(sendBytes, "Indoor".Length, "127.0.0.1", 48901);
                LoadEnemySoldier(175, 230, 425, 482);
            }
            //Debug.Log(" ActiveScene is:" + isActiveScene);

            if (is3DScene && activeScene != "Outdoor_FOREST")
            {


                if (activeScene == "OpenPlain")
                {
                    randomTalkSound.loop = true;
                    randomTalkSound.PlayDelayed(1);

                    CitySound.loop = true;
                    CitySound.PlayDelayed(1);
                }


            }

            if (activeScene == "Outdoor_FOREST")
            {
                ForestSound.loop = true;
                ForestSound.PlayDelayed(1);
            }

            if (activeScene.Contains("range") || activeScene.ToLower().Contains("bottle"))
            {
                Destroy(this.GetComponent<FirstPersonController>());
                Destroy(this.GetComponent<movePlayer>());
            }
            //buzzerText = GameObject.Find("Buzzer").GetComponent<TextMesh>();
            //print("RE: try 2 complete...");
        }
        catch (Exception e)
        {
            Debug.Log("Error when instantiating uDp:" + e.Message);
        }


        try
        {
            if (sim_ammo_setting.ToLower().Contains("laser") || sim_ammo_setting.ToLower().Contains("live"))
            {
                aimingImage.SetActive(false);
            }
            else if (sim_ammo_setting == "Game")
            {
                aimingImage.SetActive(true);
            }
            else
            {
                aimingImage.SetActive(true);
            }

            if (activeScene.ToLower().Contains("basic"))
            {
                if (sim_ammo_setting.ToLower().Contains("live") || sim_ammo_setting.ToLower().Contains("laser"))
                {
                    if(sim_ammo_setting.ToLower().Contains("remote"))
                    {
                        //num_targets_input = TestConditionsManager.totalAllowedHitShots; //initialize condition after setup
                        num_targets_input = TestConditionsManager.numBullets * TestConditionsManager.numMegs;
                    }
                    else
                    {
                        num_targets_input = TestConditionsManager.numBullets * TestConditionsManager.numMegs;
                    }
                }
                else
                {
                    if(activeScene.ToLower().Contains("1lane"))
                    {
                        num_targets_input = TestConditionsManager.totalAllowedHitShots; //Initialize condition after setup
                    }
                    else if (activeScene.ToLower().Contains("2lane"))
                    {
                        num_targets_input = TestConditionsManager.totalAllowedHitShots; //Initialize condition after setup
                    }
                    else if (activeScene.ToLower().Contains("3lane"))
                    {
                        num_targets_input = TestConditionsManager.totalAllowedHitShots; //Initialize condition after setup
                    }
                    else
                    {
                        num_targets_input = TestConditionsManager.totalAllowedHitShots; //Initialize condition after setup
                    }
                }
            }
            else if(activeScene.ToLower().Contains("hunting"))
            {
                num_targets_input = TestConditionsManager.totalAllowedHitShots; //initialize condition after setup
            }
            else 
            {
                num_targets_input = TestConditionsManager.numBullets * TestConditionsManager.numMegs;
            }

            if (activeScene == "OpenPlain" || activeScene == "MiniCity")
            {
                CitySound2.loop = true;
                CitySound2.PlayDelayed(1);
            }
            if (activeScene.ToLower().Contains("basic"))
            {
                platHitSound = gameObject.AddComponent<AudioSource>();
                platHitSound.clip = platHitAudio;
            }

            assignAccValues();

            if (activeScene == "CalibrationTest" || activeScene.ToLower().Contains("range"))
            {
                startTraining = true;

                if (activeScene.ToLower().Contains("range"))
                {
                    TimeLimit.enabled = true;
                }

            }
            timeActive.text = "TIME ACTIVE: 0";

            if (activeScene.Contains("TargetPopUp"))
            {
                headShots = 0;
                bodyShots = 0;
                HeadShots.text = ("Head Shots: " + headShots.ToString());
                BodyShots.text = ("Head Shots: " + headShots.ToString());
            }
            assistPanel.SetActive(false);

            sendSceneName();

            //print("RE: try 3 complete...");
        }
        catch (Exception e)
        {
            Debug.Log("Error when instantiating uDp:" + e.Message);
        }
    }

    // Update is called once per frame
    void Update() 
    {
        //ShootInput();
        //LoadSounds();
        //print("RE: num bullets is" + TestConditionsManager.numTargets);

        try
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                ui_mouse_pressed = GetUiElementsClicked();
                ui_mouse_reset = false;
            }
            else
            {
                if(!ui_mouse_reset)
                {
                    ui_mouse_pressed = false;
                    ui_mouse_reset = true;
                }
            }

            //trainingPaused = StaticVariableManager.isPaused;
            if (!trainingPaused && !ui_mouse_pressed)
            {
                if (sendUDPSimSetting && countDownStart.start_training)
                {
                    SendUDPSignal();
                    //print("Sent!");
                    sendUDPSimSetting = false;
                }

                if (!activeScene.ToLower().Contains("calibration"))
                {
                    if (reloadCount <= 0)
                    {
                        shootTextTimeOut = 1;
                        //ShootSateText.enabled = true;
                        //ShootSateText.text = "RELOAD!!";
                        ReloadIndicator.SetActive(true);
                        reloadTimeOut -= Time.deltaTime * 1;
                        if (reloadTimeOut <= 0)
                        {
                            ConfigureGuns();
                        }
                    }
                    else
                    {
                        //ShootSateText.text = "GO!!";

                        shootTextTimeOut -= Time.deltaTime * 1;
                        if (shootTextTimeOut <= 0f)
                        {
                            //ShootSateText.enabled = false;
                            ReloadIndicator.SetActive(false);
                        }
                    }
                }

                if (!is3DScene)
                {

                    if (buzzerFlag)
                    {

                        buzzerTime -= Time.deltaTime;
                        buzzerText.text = "Stand by:" + ((int)buzzerTime).ToString();
                        print("BuzzerTime==" + buzzerTime.ToString());
                        if (Math.Round(buzzerTime, 2) == 1.0f || Math.Round(buzzerTime, 2) == 2.0f || Math.Round(buzzerTime, 2) == 3.0f || Math.Round(buzzerTime, 2) == 4.0f || Math.Round(buzzerTime, 2) == 5.0f)
                        {
                            gunCockingSound.Play();
                            print("BuzzerTime:" + buzzerTime.ToString());
                        }
                        //gunCockingSound.Play();
                        if (buzzerTime <= 0)
                        {
                            buzzerFlag = false;
                            buzzerSound.Play();
                            buzzerText.text = "Go";
                            chrono = true;
                        }
                    }
                    if (chrono)
                    {
                        startTime -= Time.deltaTime;
                    }
                    if (activeScene != "bottleShooting" && !activeScene.ToLower().Contains("basic"))
                    {
                        if (activeScene.ToLower().Contains("range"))
                        {
                            timerText.text = "Timer: " + ((int)startTime).ToString();
                        }
                    }

                }
                //Debug.Log("Input.mousePosition:" + Input.mousePosition.ToString());
                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.escapeKey.isPressed)
                {
                    saveAcc();
                    GoToMainMenu();

                    if (is3DScene)
                    {
                        //gameObject.GetComponent<Scoring>().SaveScore("12345", "DefaultTrainee", activeScene, "System", DateTime.Now.ToString(), civilianShot.ToString(), enemyshot.ToString(), ((int)(enemyshot * 100 / shotsFired)).ToString(), shotsFired.ToString());
                    }
                }
                if (isAssistOpen)
                {

                    if (Keyboard.current.f1Key.isPressed)
                    {
                        assistPanel.SetActive(false);
                        f1Pressed = true;
                    }
                    else if ((!Keyboard.current.f1Key.isPressed) && f1Pressed == true)
                    {
                        isAssistOpen = false;
                        f1Pressed = false;
                    }
                    StaticVariableManager.isAssistOpen = isAssistOpen;
                }
                else
                {
                    if (Keyboard.current.f1Key.isPressed)
                    {
                        openAssistance();
                        f1Pressed = true;
                    }
                    else if (!Keyboard.current.f1Key.isPressed && f1Pressed == true)
                    {
                        isAssistOpen = true;
                        f1Pressed = false;
                    }
                    StaticVariableManager.isAssistOpen = isAssistOpen;
                }

                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.backspaceKey.isPressed)//Start/stop timer
                {
                    resetIndoorPapers();
                }

                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.sKey.isPressed)//Save scoring
                {
                    saveScore();
                }


                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.enterKey.isPressed)//Start/stop timer
                {
                    buzzerFlag = true;
                    buzzerTime = 6;
                    startTime = float.Parse(TimeLimit.text);
                    rangeTime = TimeLimit.text;
                    //print(TimeLimit.text);
                }
                if ((Keyboard.current.escapeKey.isPressed && Keyboard.current.enterKey.isPressed) || startTime <= 0)//Start/stop timer
                {
                    chrono = false;
                }

                try
                {
                    Vector3 mousePosition = Mouse.current.position.ReadValue();
                    Vector3 vector3 = new Vector3(mousePosition.x, mousePosition.y, -1);
                    Vector3 cameraVector = Camera.main.ScreenToWorldPoint(vector3);

                    //UDP Receive Function
                    udpDataProcessing();

                    //|Call acc after receiving udp
                    loadAccValues();
                    if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.rightAltKey.isPressed)
                    {
                        isAccMode = false;
                    }
                    if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.leftAltKey.isPressed)
                    {
                        isAccMode = true;
                        if (activeScene == "CalibrationTest")
                        {
                            saveAcc();
                            writeAccToFile(AccSaveData);
                            Calibration.isAccSet = false;
                        }
                    }


                    //float h = Input.GetAxisRaw("Horizontal");
                    //float v = Input.GetAxisRaw("Vertical");

                    //xPos = -1f;
                    //yPos = -1f;

                    if (Mouse.current.leftButton.wasPressedThisFrame && xPos == -1f && yPos == -1f)
                    {
                        mouseShotFire = true;

                        //Debug.Log("Shooting with mouse!");

                        if (activeScene.ToLower().Contains("range") && startTime > 0 && buzzerTime <= 0)
                        {
                            Shoot();
                        }
                        if (!activeScene.ToLower().Contains("range"))
                        {
                            Shoot();
                        }



                    }
                    else
                    {
                        mouseShotFire = false;
                    }
                }
                catch (Exception e)
                {
                    Scoring.logs += "\n" + e.Message + ":" + e.StackTrace;
                    Debug.Log("Exception:" + e.StackTrace + " " + e.Message);
                }
                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.f3Key.isPressed)
                {
                    Debug.Log("Saving:" + calibrationPath + " points:" + CalibratePoint1.x.ToString() + "," + CalibratePoint1.y.ToString() + ":" + CalibratePoint2.x.ToString() + "," + CalibratePoint2.y.ToString());
                    System.IO.File.WriteAllText(@calibrationPath, "Newpoints:" + CalibratePoint1.x.ToString() + "," + CalibratePoint1.y.ToString() + ":" + CalibratePoint2.x.ToString() + "," + CalibratePoint2.y.ToString());
                }
                xPos = -1f; yPos = -1f;//reset to cater for mouse 
                                       //sendEndless("bruce");
                if (activeScene.ToLower().Contains("range"))
                {
                    life.enabled = false;
                    score.enabled = false;

                }
                else if (!activeScene.ToLower().Contains("basic"))
                {
                    numEnemies = (numberOfEnemies - enemyshot);
                    //life.text = "Life: " + mainPlayerLives;
                    //scoreMain.text = "Threat:" + numEnemies + " civilians " + numberOfCivilians;
                }

                if (isShootState == true)
                {
                    shootingTimeOut -= Time.deltaTime * 1;
                }

                //shoot delay decrement called every frame
                if (shootDelay > 0)
                {
                    shootDelay -= Time.deltaTime * 1;
                }

                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.cKey.isPressed)//Start/stop timer
                {
                    isCalibrate = true;
                    sendUDP = true;  //UDP Sending switch
                }
                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.rKey.isPressed)//Start/stop timer
                {

                    isReset = true;
                    sendUDP = true;  //UDP Sending switch
                    isResetDelay = true;
                }
                if (Keyboard.current.leftShiftKey.isPressed || activeScene.ToLower().Contains("moving"))
                {
                    aimingImage.SetActive(false);
                }
                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.cKey.isPressed)//Start/stop timer
                {
                    isReset = false;
                    isCalibrate = false;
                    sendUDP = true;  //UDP Sending switch
                }


                if (startTraining == false)
                {
                    startTraining = countDownStart.start_training;
                }

                gameOver();
                ShootDelayText();
                if (countDownStart.start_training == true && isResetDelay == false && trainingPaused == false)
                {
                    if (activeScene.ToLower().Contains("suspectshoot"))
                    {
                        if (StaticVariableManager.isLane1BoardSet)
                        {
                            StartActiveCount();
                            startLane1ResponseCount = true;
                            startLane2ResponseCount = true;
                            startLane3ResponseCount = true;
                            startLane4ResponseCount = true;
                        }
                    }
                    else
                    {
                        StartActiveCount();
                    }

                }
                //maintiain_height();
                //manageGlobalActiveTime();

                if (sim_ammo_setting.ToLower().Contains("live") && activeScene != "CalibrationTest" && perform_AutoReset == true)
                {
                    isResetDelay = true;
                    if (isResetDelay == true)
                    {
                        resetIgnoreDelay -= Time.deltaTime * 1;
                        if (!activeScene.ToLower().Contains("range"))
                        {
                            //standby_txt.SetActive(true);
                        }

                        if (resetIgnoreDelay <= 0f)
                        {
                            runtime_before_standby = runtime_reset;
                            isShooting = false;
                            isResetDelay = false;
                            //standby_txt.SetActive(false);

                            //Exclude scenes from auto reset
                            perform_AutoReset = false;
                        }
                        //print("RE: On reset delay " + resetIgnoreDelay.ToString("0"));
                    }
                    else if (isResetDelay == false)
                    {
                        resetIgnoreDelay = setIgnoreDelay;
                        if (isShooting)//Only start counting when shooting starts
                        {
                            runtime_before_standby -= Time.deltaTime * 1;
                        }

                        if (runtime_before_standby <= 0f)
                        {
                            isResetDelay = true;
                            isReset = true;
                            sendUDP = true;  //UDP Sending switch
                        }
                    }
                }

                if (StaticVariableManager.isStopTraining == true)
                {
                    //print("Test: Point reached");
                    //StopTraining();
                }

                //Split time counters
                manageSplitAndReactionTime();

                //Hunting scenes Stop Conditions
                manageHuntTrainingStopCondition();

                //Lane shooting Stop Condition
                manageLaneTarinigStopCondition();

                //Suspect Shoot stop condition
                manageSuspectShootStopCondition();
            }
            if (sim_ammo_setting == "Live")
            {
                //Condition for auto restarting the scenarion
                if (trainingPaused && TestConditionsManager.trainingMode == "Continuous")
                {
                    restartTimer -= Time.deltaTime * 1;

                    if (restartTimer <= 0f)
                    {
                        SceneManager.LoadScene(activeScene);
                    }
                }
                else if (!trainingPaused)
                {
                    restartTimer = TestConditionsManager.setStopSeconds;
                }
            } //Auto restart and move the paper roll in the range

            manageMouseClickStopCondition();
            manageGlobalTrainingVariable();

        }
        catch (Exception ex)
        {
            Scoring.logs += "\n" + ex.Message + ":" + ex.StackTrace;
            Debug.LogError("Shooting Update:" + ex.StackTrace);
        }

    }

    private void SetDataDirectory() // Verbatim String Literals
    {
        // Ensure the path is set correctly for the directory
        //StaticVariableManager.main_data_directory = @"C:\Users\barne\OneDrive\Documents\IMX 3D Project\IMX Data Files\Sim Data";

        // Construct file paths
        calibrationPath = Path.Combine(StaticVariableManager.main_data_directory, "Calibration.txt");
        DelaySavePath = Path.Combine(StaticVariableManager.main_data_directory, "delDat00.txt");
        UDP_IP_SavePath = Path.Combine(StaticVariableManager.main_data_directory, "AddrDat00.txt");
        gunsTextPath = Path.Combine(StaticVariableManager.main_data_directory, "guns.txt");
        AccSavePath = Path.Combine(StaticVariableManager.main_data_directory, "AccDatInc.txt");
    }


    private void manageSplitAndReactionTime()
    {

        if(countDownStart.start_training)
        {
            //print("RE: Point Reached............");
            //Split Time
            ByPassSplitTime();

            if (startLane1Count)
            {
                lane1SplitTimeCounter += Time.deltaTime * 1;
            }
            if (startLane2Count)
            {
                lane2SplitTimeCounter += Time.deltaTime * 1;
            }
            if (startLane3Count)
            {
                lane3SplitTimeCounter += Time.deltaTime * 1;
            }
            if (startLane4Count)
            {
                lane4SplitTimeCounter += Time.deltaTime * 1;
            }

            //Response Time
            if (startLane1ResponseCount)
            {
                lane1ResponseTimeCounter += Time.deltaTime * 1;
                //print("Test: Time " + lane1ResponseTimeCounter);
            }
            if (startLane2ResponseCount)
            {
                lane2ResponseTimeCounter += Time.deltaTime * 1;
            }
            if (startLane3ResponseCount)
            {
                lane3ResponseTimeCounter += Time.deltaTime * 1;
            }
            if (startLane4ResponseCount)
            {
                lane4ResponseTimeCounter += Time.deltaTime * 1;
            }
        }

    }

    private void ByPassSplitTime()
    {
        if(activeScene.ToLower().Contains("cyclic"))
        {
            if(activeScene.ToLower().Contains("1lane"))
            {
                startLane1Count = true;
            }
            if (activeScene.ToLower().Contains("2lane"))
            {
                startLane1Count = true;
                startLane2Count = true;
            }
            if (activeScene.ToLower().Contains("3lane"))
            {
                startLane1Count = true;
                startLane2Count = true;
                startLane3Count = true;
            }
        }
    }

    private void loadTraineeNames()
    {
        if (activeScene.ToLower().Contains("basic"))
        {
            lane1TraineeNameDisplay.text = TestConditionsManager.lane1TraineeName;
            lane2TraineeNameDisplay.text = TestConditionsManager.lane2TraineeName;
            lane3TraineeNameDisplay.text = TestConditionsManager.lane3TraineeName;
        }
    }

    private void loadBackgroundColor()
    {

        if (Scoring.ammo_setting.ToLower().Contains("live") || Scoring.ammo_setting.ToLower().Contains("infrared"))
        {
            Color newColor;
            switch (StaticVariableManager.backgroundColorSetting.ToLower())
            {
                case "black":
                    r = 0f / 255f;
                    g = 0f / 255f;
                    b = 0f / 255f;
                    a = 1f;
                    fpsCam.backgroundColor = new Color(r, g, b, a);

                    break;
                case "white":
                    r = 212f / 255f;
                    g = 212f / 255f;
                    b = 212f / 255f;
                    a = 1f;
                    fpsCam.backgroundColor = new Color(r, g, b, a);

                    break;
                case "yellow":
                    //fpsCam.backgroundColor = Color.blue;
                    r = 236f / 255f;
                    g = 185f / 255f;
                    b = 0f / 255f;
                    a = 1f;
                    fpsCam.backgroundColor = new Color(r, g, b, a);

                    break;
                case "red":
                    r = 212f / 255f;
                    g = 45f / 255f;
                    b = 33f / 255f;
                    a = 1f;
                    fpsCam.backgroundColor = new Color(r, g, b, a);
                    break;
                case "blue":
                    //fpsCam.backgroundColor = Color.blue;
                    r = 52f / 255f;
                    g = 52f / 255f;
                    b = 217f / 255f;
                    a = 1f;
                    fpsCam.backgroundColor = new Color(r, g, b, a);

                    break;
                case "green":
                    r = 0f / 255f;
                    g = 181f / 255f;
                    b = 0f / 255f;
                    a = 1f;
                    fpsCam.backgroundColor = new Color(r, g, b, a);

                    break;
            }
        }
    }

    private void sendSceneName()
    {
        if (activeScene.ToLower().Contains("cyclic"))
        {
            sendEndless("scene,StaticCircles");
        }
        if (activeScene.ToLower().Contains("risingplate"))
        {
            sendEndless("scene,RisingPlates");
        }
        if (activeScene.ToLower().Contains("baloon"))
        {
            sendEndless("scene,BaloonTargets");
        }
    }
    private void manageSuspectShootStopCondition()
    {
        if(StaticVariableManager.isStopTraining)
        {
            
            trainingPaused = true;
            StopTraining();
        }
    }

    private void manageHuntTrainingStopCondition()
    {

        if (activeScene.ToLower().Contains("hunting"))
        {
            if (activeScene.ToLower().Contains("direct"))
            {
                if (StaticVariableManager.totalTargetAnimalsKilled >= num_targets_input)
                {
                    targetsFinished = true;
                    targetFinished = true;
                    StopTraining();
                }
            }
            else if (activeScene.ToLower().Contains("avoid"))
            {
                if (StaticVariableManager.totalTargetAnimalsKilled + StaticVariableManager.totalTargetCasualtiesKilled >= num_targets_input)
                {
                    targetsFinished = true;
                    targetFinished = true;
                    StopTraining();
                }
            }

        }
    }
    private void manageMouseClickStopCondition()
    {
        //sim_ammo_setting == "Laser" || sim_ammo_setting.ToLower().Contains("remote")
        if (!trainingPaused)
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                StopTraining();
            }
        }
        else if(activeScene.ToLower().Contains("basic"))
        {
            /*float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            // Check if there is any scroll input
            if (Mathf.Abs(scrollInput) > 0)
            {
                switch (scrollInput)
                {
                    case -0.1f:
                        scoreScrollValue--;
                        if (scoreScrollValue < 0)
                            scoreScrollValue = 0;
                        break;
                    case 0.1f:
                        scoreScrollValue++;
                        if (scoreScrollValue > 3)
                            scoreScrollValue = 3;
                        break;
                }
                HandleLaneInputData(scoreScrollValue);
            }*
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                SceneManager.LoadScene(activeScene);
            }*/
        }

    }
    private void manageLaneTarinigStopCondition()
    {
        if (activeScene.ToLower().Contains("1lane"))
        {
            if (lane1TargetsComplete || lane1SrikeOut)
            {
                targetFinished = true;
                targetsFinished = true;
                StopTraining();
            }
        }
        if (activeScene.ToLower().Contains("2lane"))
        {
            if ((lane1TargetsComplete && lane2TargetsComplete) || (lane1SrikeOut && lane2SrikeOut) || (lane1TargetsComplete && lane2SrikeOut) || (lane2TargetsComplete && lane1SrikeOut))
            {
                targetFinished = true;
                targetsFinished = true;
                StopTraining();
            }
        }
        if (activeScene.ToLower().Contains("3lane"))
        {
            if ((lane1TargetsComplete && lane2TargetsComplete && lane3TargetsComplete) || lane3SrikeOut)
            {
                targetFinished = true;
                targetsFinished = true;
                StopTraining();
            }
        }

        //Response time
        if(activeScene.ToLower().Contains("baloon"))
        {
            //Flag condition for response counter
            if(StaticVariableManager.nextColorSet == true)
            {
                startLane1ResponseCount = true;
                startLane2ResponseCount = true;
                startLane3ResponseCount = true;
                startLane4ResponseCount = true;
                StaticVariableManager.nextColorSet = false;
            } 
        }


        if (sim_ammo_setting == "Live")
        {
            if (shotsFired >= num_targets_input && !activeScene.ToLower().Contains("hunting") && !activeScene.ToLower().Contains("calibration"))
            { 
                GameOver = true;
                targetsFinished = true;
                targetFinished = true;
                StopTraining();
            }
        }

    }
    private void manageGlobalTrainingVariable()
    {
        StaticVariableManager.isTrainingPause = trainingPaused;
    }
    private void resetIndoorPapers()
    {
        ResetRange(bulletHoles);
        isCalibrate = false;
        isReset = true;

        sendUDP = true;  //UDP Sending switch
        listOfPoint = new List<string>();
        
        if(activeScene.ToLower().Contains("range"))
        {
            //timerText.text = "Timer: 0";
        }
    

        if (activeScene.ToLower().Contains("targetpopup"))
        {
            headShots = 0;
            bodyShots = 0;
            HeadShots.text = ("Head Shots: " + headShots.ToString());
            BodyShots.text = ("Head Shots: " + headShots.ToString());
        }
    }
    private void sortTrainingTargert()
    {
        if (activeScene.ToLower().Contains("ipec"))
        {
            head_1.SetActive(true);
            head_2.SetActive(false);
            body_1.SetActive(false);
            body_2.SetActive(false);
            body_3.SetActive(true);
        }
    }
    private void udpDataProcessing()
    {
        IPEndPoint remoteEP = null;
        if (udpClient.Available > 0)   // Cam Points
        {
            //print("RE: In UDP Client 1");
            byte[] data = udpClient.Receive(ref remoteEP);
            string message = Encoding.ASCII.GetString(data);

            //Debug.Log(message + " from " + remoteEP.Address.ToString());
            print("cam:" + message);

            xPos = float.Parse(message.Split(':')[0]);
            yPos = float.Parse(message.Split(':')[1]);

            if (Ball1Y > y_CentrePos)
            {
                lowerAcc = false;
                upAcc = true;

                if (Ball1X > x_CentrePos)  //Right
                {
                    upLeftAcc = false;
                    upRightAcc = true;    //Priority
                    lowerLeftAcc = false;
                    lowerRightAcc = false;
                }
                else if (Ball1X < x_CentrePos)
                {
                    upLeftAcc = true;     //Priority
                    upRightAcc = false;
                    lowerLeftAcc = false;
                    lowerRightAcc = false;

                }

            } //High point
            else if (Ball1Y < y_CentrePos) //Low Point
            {
                lowerAcc = true;
                upAcc = false;

                if (Ball1X > x_CentrePos) //Right
                {
                    upLeftAcc = false;
                    upRightAcc = false;
                    lowerLeftAcc = false;
                    lowerRightAcc = true;  //priority
                }
                else if (Ball1X < x_CentrePos) //Left
                {
                    upLeftAcc = false;
                    upRightAcc = false;
                    lowerLeftAcc = true;   //priority
                    lowerRightAcc = false;

                }

            }

            if (activeScene.ToLower().Contains("range") && startTime > 0 && buzzerTime <= 0)
            {
                Shoot();
            }
            else
            {
                Shoot();
            }
            
        }
        if (udpClientImg.Available > 0)   //Score Image Saver
        {
            print("RE: In UDP Client 2");
            byte[] dataImg = udpClientImg.Receive(ref remoteEP);
            UdpImageSaver.receiveImageBytes(dataImg);
        }
    }
    private void openAssistance()
    {
        if(login_Manager.EmailText.ToLower().Contains("range"))
        {
            assistText.text = "";
            assistText.text += "Main Menu : Press Exit Button \n";
            assistText.text += "Reset Scene : Press Restart Button \n";
            assistText.text += "Reset Paper : Press Left Shift + R \n";
            assistText.text += "Close user guide : Press F1 \n";
            assistPanel.SetActive(true);
        }
        else
        {
            assistText.text = "";
            assistText.text += "Main Menu : Press Exit Button \n";
            assistText.text += "Reset Scene : Press Restart Button \n";
            assistText.text += "Close user guide : Press F1 \n";
            assistPanel.SetActive(true);

        }
    }
    void loadErrorVals()
    {
        //maxError_Val = float.Parse(error_input.text);
    }
    private void maintiain_height()
    {
        //print("position is: " + this.transform.position.y);
        //OnTriggerEnter(this.GetComponent<CapsuleCollider>());
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "terrain")
        {
            //print("I have collided");
            //Destroy(col.gameObject);
        }
        else
        {
            //print("I have floating");
            //Destroy(col.gameObject);
        }
    }

    public void StartActiveCount()
    {
        if(!StaticVariableManager.isResetingPoints)
        {

            timeActiveCounter += Time.deltaTime;
            timeActive.text = "TIME ACTIVE: " + timeActiveCounter.ToString("0");
            if (activeScene.ToLower().Contains("basic") || activeScene.ToLower().Contains("hunting"))
            {
                if (timeActiveCounter >= TestConditionsManager.total_test_time)
                {
                    timeFinished = true;
                    targetFinished = true;
                    StopTraining();
                }
            }

            if (activeScene.ToLower().Contains("lane"))
            {
                //&& startLane1Count
                if (!lane1TargetsComplete && !StaticVariableManager.isLane1TargetMissed && startLane1Count)
                {
                    if (!lane1SrikeOut)
                    { 
                        lane1TimeActiveCounter += Time.deltaTime;
                        lane1Timer.text = " " + lane1TimeActiveCounter.ToString("0.0");
                        //lane1ActiveTimeCounter = timeActiveCounter;
                    }
                }
                if (!lane2TargetsComplete && !StaticVariableManager.isLane2TargetMissed && startLane2Count)
                {
                    if (!lane2SrikeOut)
                    {
                        lane2TimeActiveCounter += Time.deltaTime;
                        lane2Timer.text = " " + lane2TimeActiveCounter.ToString("0.0");
                        //lane2ActiveTimeCounter = timeActiveCounter;
                    }
                }
                if (!lane3TargetsComplete && !StaticVariableManager.isLane3TargetMissed && startLane3Count)
                {
                    lane3TimeActiveCounter += Time.deltaTime;
                    lane3Timer.text = " " + lane3TimeActiveCounter.ToString("0.0");
                    //lane3ActiveTimeCounter = timeActiveCounter;
                }
                if (!lane4TargetsComplete)
                {
                    //lane4Timer.text = " " + timeActiveCounter.ToString("0");
                }
            }
            else if (is3DScene)
            { //3D Scenarios
                lane1TimeActiveCounter += Time.deltaTime;
                lane1Timer.text = " " + lane1TimeActiveCounter.ToString("0.0");
            }
        }
    }

    public void ConfigureGuns()
    {
        if (activeScene.ToLower().Contains("basic"))
        {
            reloadCount = TestConditionsManager.numBullets;
            if (Scoring.gun == "Handgun")
            {
                range = 55;
            }
            else if (Scoring.gun == "Riffle")
            {
                range = 200;
            }
            else if (Scoring.gun == "Short gun")
            {
                range = 40;
            }
            else if (Scoring.gun == "CZ 75 B omega")
            {
                range = 40;
            }
            else if (Scoring.gun == "Glock 42 slimline")
            {
                range = 40;
            }
            else if (Scoring.gun == "Taurus G2C")
            {
                range = 40;
            }
            else if (Scoring.gun == "Ruger 9mms LCP")
            {
                range = 40;
            }
            else if (Scoring.gun == "CO2 Hand Gun")
            {
                range = 40;
            }
            else
            {
                range = 55;
            }
        }
        else
        {
            if (Scoring.gun == "Handgun")
            {
                reloadCount = 15;
                range = 55;
            }
            else if (Scoring.gun == "Riffle")
            {
                reloadCount = 30;
                range = 200;
            }
            else if (Scoring.gun == "Short gun")
            {
                reloadCount = 16;
                range = 40;
            }
            else if (Scoring.gun == "CZ 75 B omega")
            {
                reloadCount = 16;
                range = 40;
            }
            else if (Scoring.gun == "Glock 42 slimline")
            {
                reloadCount = 6;
                range = 40;
            }
            else if (Scoring.gun == "Taurus G2C")
            {
                reloadCount = 16;
                range = 40;
            }
            else if (Scoring.gun == "Ruger 9mms LCP")
            {
                reloadCount = 6;
                range = 40;
            }
            else if (Scoring.gun == "CO2 Hand Gun")
            {
                reloadCount = 15;
                range = 40;
            }
            else
            {
                reloadCount = 15;
                range = 55;
            }
        }
    }
    public void SendUDPSignal()
    {
        if(activeScene.ToLower().Contains("calibration"))
        {
            sendEndless("Calibrate");
        }
        else if (activeScene.ToLower().Contains("test"))
        {
            sendEndless("Test");
        }
        else
        {
            if (Scoring.ammo_setting.ToLower().Contains("live"))
            {
                sendEndless("Live");
                if (Scoring.shooting_PaperRoll_Setting.ToLower().Contains("dynamic"))
                {
                    sendEndless("Reset");
                }
            }
            else
            {
                sendEndless("Laser");
            }

            sendEndless("Simulate");
        }

    }
    public void CreateFile()
    {

        if (!File.Exists(DelaySavePath))
        {
            FileManager.CreateFile(DelaySavePath);

            Encryption encrypt = new Encryption();
            //string base64 = encrypt.AESEncryption("0");
            //FileManager.WriteLineToFile(DelaySavePath, base64);
        }

        FileManager.CreateFile(AccSavePath);
        FileManager.CreateFile(UDP_IP_SavePath);

    }
    private void ReadFile()
    {
        try
        {
            string base64 = FileManager.ReadFromFile(UDP_IP_SavePath);
            string base65 = FileManager.ReadFromFile(DelaySavePath);

            Encryption encrypt = new Encryption();
            Encryption encrypt2 = new Encryption();

            UDP_ClientIP_Address = encrypt.AESDecryption(base64);                    //decryption code
            fileData = encrypt2.AESDecryption(base65);                    //decryption code
            
            if(fileData.Contains("."))
            {
                fileData.Replace(".", ",");
            }

            //print("Delay Data is " + fileData);
            //print("UDP Data is: " + UDP_ClientIP_Address);

        }
        catch (Exception e)
        {
            print("Error retrieving zero file:" + e);
            throw new Exception("First login");
        }

    }
    private void WriteFile(float val)
    {
        using (StreamWriter writer = new StreamWriter(DelaySavePath))
        {
            writer.WriteLine(val);
        }

        ReadFile();
    }
    private void IncriptsFile(string val)
    {

        print("Encrypting....");
        Encryption encrypt = new Encryption();
        string base64 = encrypt.AESEncryption(val);//encrypt username and password
        print("Delay Data Encrypted:" + base64);
        FileManager.CreateFile(DelaySavePath);
        FileManager.WriteDataToFile(DelaySavePath, base64);
        //ReadFile();
    }
    private void writeAccToFile(string val)
    {
        print("Encrypting Acc....");
        Encryption encrypt = new Encryption();
        string base64 = encrypt.AESEncryption(val);//encrypt username and password
        print("Acc Data Encrypted:" + base64);
        FileManager.CreateFile(AccSavePath);
        FileManager.WriteDataToFile(AccSavePath, base64);
    }
    
    void ShootDelayText()
    {
        if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.equalsKey.isPressed)//increment the shoot delay 
        {
            isShootDelay = true;

            incrementDelay -= Time.deltaTime * 1;
            if(incrementDelay <= 0)
            {
                delaySetting = delaySetting + 0.01f;
                incrementDelay = 0.0f;
            }

            ShootDelayTxt.enabled = true;
            ShootDelayTxt.text = "" + delaySetting.ToString("0.0");
            shootDelayAdmin.text = "Shoot Delay: " + delaySetting.ToString("0.0");
            //WriteFile(delaySetting);
            IncriptsFile(delaySetting.ToString("0.0"));
        }

        if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.minusKey.isPressed)//decrement the shoot delay 
        {
            isShootDelay = true;

            incrementDelay -= Time.deltaTime * 1;
            if (incrementDelay <= 0)
            {
                delaySetting = delaySetting - 0.01f;
                incrementDelay = 0.0f;
            }

            ShootDelayTxt.enabled = true;

            if(delaySetting < 0)
            {
                delaySetting = 0;
            }

            ShootDelayTxt.text = "" + delaySetting.ToString("0.0");
            shootDelayAdmin.text = "Shoot Delay: " + delaySetting.ToString("0.0");
            //WriteFile(delaySetting);
            IncriptsFile(delaySetting.ToString("0.0"));
        }

        //send delay setting to file

        if(isShootDelay)
        {
            ShootDelayTxt_timeout -= Time.deltaTime;

            if (ShootDelayTxt_timeout <= 0)
            {
                ShootDelayTxt.enabled = false;
                ShootDelayTxt_timeout = 1;
                isShootDelay = false;
            }
        }
    }
    void Shoot()
    {
        isShooting = true;
        //print("RE: startTraining " + startTraining);
        //print("RE: isResetDelay " + isResetDelay);
        //print("RE: trainingStarted " + trainingStarted);
        //print("RE: trainingPaused" + trainingPaused);
        //
        //print("RE: " + sim_ammo_setting);
        //print("Test: Shooting...");
        if ((startTraining == true && isResetDelay == false && trainingStarted == true && trainingPaused == false
            && StaticVariableManager.isResetingPoints == false && StaticVariableManager.isEnded == false) || activeScene == "CalibrationTest")
        {
            if (shootDelay <= 0 )
            {

                if (reloadCount <= 0)
                {
                    //Dont Shoot
                }
                else
                {
                    shotsFired++;
                    
                    if (Array.Exists(guns, element => element == Scoring.gun))
                    {

                    }
                    

                    //ShootSateText.text = "Shoot";
                    reloadTimeOut = 2;
                    isShootState = true;

                    //num_targets_input


                    totalshots.text = "Shots Fired : ";
                    totalshots.text += shotsFired;
                    //print(totalshots.text);
                    //ShootRayCast();
                    if (activeScene == "Outdoor_FOREST" && !ForestSound2.isPlaying)
                    {
                        //ForestSound.Stop();
                        ForestSound2.loop = true;
                        ForestSound2.PlayDelayed(1);
                    }
                    if (randomTalkSound.isPlaying)
                    {
                        randomTalkSound.Stop();
                        CitySound.Stop();

                        if (activeScene == "OpenPlain" || activeScene == "MiniCity")
                        {
                            CitySound2.loop = true;
                            CitySound2.PlayDelayed(1);
                        }


                    }
                    //handgunSound.Play();
                    RaycastHit hit;
                    //Ray ray;
                    if (xPos == -1f && yPos == -1f)
                    {
                        ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); //new Vector3(xPos, yPos, -1)
                        //print("Shooting in a range");
                    }
                    else
                    {
                        //ray = Camera.main.ScreenPointToRay(new Vector3(xPos, yPos, 0));
                        // isAccMode = false;
                        if (isAccMode == false)
                        {
                            DrawOrgLines(new Vector2Int((int)xPos, (int)yPos));
                            //DrawUpdatedLines(new Vector2Int((int)xPos, (int)yPos));
                        }
                        else if(isAccMode == true)
                        {
                            DrawMatrixLines(new Vector2Int((int)xPos, (int)yPos));
                        }
                    }
                    Ray downray = new Ray(transform.position, Vector3.forward);
                    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
                    {
                        carLife--;
                    }

                    if (Physics.Raycast(ray, out hit, range))//(Physics.Raycast(new Vector3(posx, 1.6f, -10f), fpsCam.transform.forward, out hit, range))//Physics.Raycast(ray, out hit, range)  or Physics.Raycast(transform.position, out hit, range)
                    {
                        //CODE FOR ADJUSTED Calibration
                        //print("RE: active scene is..." + activeScene);
                        if (activeScene.ToLower().Contains("calibration"))
                        {
                            //print("RE: Shooting Calib impact");
                            GameObject impactGo = Instantiate(acc_Bullet, hit.point, Quaternion.LookRotation(hit.normal));
                            Destroy(impactGo, 0.1f);

                            //all_bullets = GameObject.FindGameObjectsWithTag("bullet");
                            /*foreach (GameObject bullet in all_bullets)
                            {
                                Destroy(bullet);
                            }*/
                            //print("I AM SHOOTING...");
                            //impactEffect
                            //acc_Bullet
                        }
                        else
                        {
                            if (hit.transform.tag.ToLower().Contains("soldier") || hit.transform.tag.ToLower().Contains("civilian"))
                            {
                                //Do Nothing for now
                            }
                            else
                            {
                                if (Scoring.ammo_setting == "Live")
                                {
                                    if (pointInRadiusG == false && isCalibrate == false)
                                    {
                                        GameObject impactGo = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                                        Destroy(impactGo, 0.4f);
                                    }
                                    else if (isCalibrate == true)
                                    {
                                        GameObject impactGo = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                                        Destroy(impactGo, 0.4f);
                                    }
                                }
                                else
                                {
                                    GameObject impactGo_2 = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                                    Destroy(impactGo_2, 0.4f);
                                }
                            }

                        }
                        //END FOR ADJUSTED Calibration
                        //Debug.Log("Hit=" + hit.transform.tag);

                        if (is3DScene)
                        {
                            sceneOpenPlainShoot(hit);
                            //Debug.Log("Hit=" + hit.transform.name);

                            if (activeScene.ToLower().Contains("hunting"))
                            {
                                StaticVariableManager.isGunFired = true;
                            }
                        }
                        else if (activeScene.ToLower().Contains("crouching"))
                        {
                            sceneOpenPlainShoot(hit);

                        }
                        else if (activeScene.ToLower().Contains("minicity"))
                        {
                            sceneOpenPlainShoot(hit);

                        }
                        else if (activeScene.ToLower().Contains("moving_targets"))
                        {

                            //target for moving targets
                            targetHitParent = hit.transform.parent.name;
                            targetHit = hit.transform.name;

                        }
                        else if (activeScene == ("BasicAimAndShoot"))
                        {
                            sceneIndoorShoot(hit);
                        }
                        else if (activeScene == ("BlockShooting"))
                        {
                            sceneOpenPlainShoot(hit);
                            //MovingBoxTarget
                            //sceneIndoorShoot(hit);
                        }
                        else if (activeScene.ToLower().Contains("basic"))
                        {
                            sceneOpenPlainShoot(hit);
                            //MovingBoxTarget
                            //sceneIndoorShoot(hit);
                        }
                        else
                        {
                            //Debug.Log("Hit=" + hit.transform.name);
                            sceneIndoorShoot(hit);
                        }

                        //print("Test: " + mouseShotFire);
                        if (mouseShotFire || !mouseShotFire) //mouseShotFire && (hit.transform.tag.ToLower().Contains("miss") || hit.transform.name.ToLower().Contains("body.") || hit.transform.name.ToLower().Contains("head."))
                        {
                            //do not shoot
                        }
                        else
                        {

                            

                        }


                    }

                    SaveCalibration();
                    manageReloadCount();
                    //xPos = -1f; yPos = -1f;
                }

                
                //set delay;
                shootDelay = delaySetting;
            }
            else
            {
                shootDelay -= Time.deltaTime * 1;
            }
        }

    }

    private void manageReloadCount()
    {
        //Guns Code
        if (sim_ammo_setting.ToLower().Contains("laser") && Scoring.simulation_type == "training" && activeScene != "CalibrationTest")
        {
            //Apply Reload
            if (shotsFired >= (TestConditionsManager.numBullets * TestConditionsManager.numMegs))
            {
                ammaFinished = true;
                StopTraining();
            }
            else
            {
                reloadCount--;
            }
        }
        else
        {
            //DO Not Stop Training For Reaload.
        }
    }

    private bool CheckNewPoint(string incomingPoint)
    {
        double radius = 2; 

        double newPointX = float.Parse(incomingPoint.Split(":")[0]); 
        double newPointY = float.Parse(incomingPoint.Split(":")[1]);
        bool isPointWittin = false;

        // Calculate the distance between the center point and the given point
        double distance = 0.0f;

        /*
        if (listOfPoint.Contains(incomingPoint))
        {
            //return point within
            isPointWittin = true;
            //dont shoot
            print("New point in radius");
        }
        else
        {
            foreach (string exPoint in listOfPoint)
            {
                distance = Math.Sqrt(Math.Pow(newPointX - float.Parse(exPoint.Split(":")[0]), 2) + Math.Pow(newPointY - float.Parse(exPoint.Split(":")[1]), 2)); ;

                if (distance < Math.Pow(radius,2))
                {
                    // The point is outside the desired area
                    isPointWittin = true;
                    //dont shoot
                    print("New point in radius");
                    break;
                }
            }

            if (isPointWittin == false)
            {
                listOfPoint.Add(incomingPoint);
                print("New point out of radius");
            }
        } */

        return isPointWittin;
    }

    private void DrawMatrixLines(Vector2Int p)
    {
        /*************Variables to draw bullets on targets********************/
        // Debug.Log("p up:" + p.ToString() + " vs X" + CalibratePoint1.ToString() + " vs Y:" + CalibratePoint2.ToString());

        // Applying the homography matrix transformation to the received point (p)
        Vector2 point = new Vector2(p.x, p.y);

        // Assuming `homographyMatrix` is pre-calculated and is available
        Vector2 transformedPoint = ApplyHomographyMatrix(homographyMatrix, point);

        // Now the transformedPoint is in Unity's screen space, use that for further calculations or drawing

        float eX = 0, eY = 0;

        if (transformedPoint.x > CalibratePoint1.x && transformedPoint.x < CalibratePoint3.x && transformedPoint.y < CalibratePoint1.y && transformedPoint.y > CalibratePoint3.y)
        {

            print("");
            print("Generating homographic line");
            if (FlagBulletRx == false)
            {
                FlagBulletRx = true;
                CountShotFired++; // increment every time a shot is detected
            }

            eX = Mathf.Abs(transformedPoint.x - (CalibratePoint1.x));
            eX = Mathf.Abs((CalibratePoint3.x - CalibratePoint1.x)) / eX;
            eY = Mathf.Abs(transformedPoint.y - (CalibratePoint3.y));
            eY = Mathf.Abs(CalibratePoint3.y - CalibratePoint1.y) / eY;

            Debug.Log("transformedPoint:" + transformedPoint.ToString() + "  eX:" + eX + " eY" + eY);

            try
            {
                Ball1X = Screen.width / eX;
                Ball1Y = Screen.height / eY;
                unityCamValues = Ball1X.ToString() + ":" + Ball1Y.ToString();

                if (Scoring.ammo_setting == "Live")
                {
                    ray = Camera.main.ScreenPointToRay(new Vector3((int)(Ball1X * 1f), (int)((Ball1Y * 1f)), 0));
                }
                else
                {
                    ray = Camera.main.ScreenPointToRay(new Vector3((int)(Ball1X * 1f), (int)((Ball1Y * 1f)), 0));
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error:" + e.Message);
            }
        }

        // Reset point
        p = new Vector2Int(0, 0);
    }

    private Vector2 ApplyHomographyMatrix(Matrix<double> homographyMatrix, Vector2 point)
    {
        // Convert the point to homogeneous coordinates (x, y, 1)
        var homogeneousPoint = DenseVector.OfArray(new double[] { point.x, point.y, 1 });

        // Apply the homography transformation (Matrix * Point)
        var transformedPoint = homographyMatrix * homogeneousPoint;

        // Convert back to 2D by normalizing the point (x, y)
        float x = (float)(transformedPoint[0] / transformedPoint[2]);
        float y = (float)(transformedPoint[1] / transformedPoint[2]);

        return new Vector2(x, y);
    }

    private Matrix<double> CalculateHomographyMatrix(Vector2Int point1, Vector2Int point2, Vector2Int point3, Vector2Int point4)
    {
        // Define the coordinates of the four points in OpenCV space (source) and Unity space (destination)
        // This matrix should be computed using the source and destination points
        var sourcePoints = new double[,] {
        { point1.x, point1.y },
        { point2.x, point2.y },
        { point3.x, point3.y },
        { point4.x, point4.y }
    };

        var destPoints = new double[,] {
        { 0, 0 },  // Example Unity screen coordinates, adjust accordingly
        { Screen.width, 0 },
        { Screen.width, Screen.height },
        { 0, Screen.height }
    };

        // Use a math library to calculate the homography matrix (Matrix computation)
        // You can use OpenCV (in C# via Emgu CV or similar) for homography calculation, or implement the calculation here.
        // Returning a dummy identity matrix for illustration purposes (you would calculate this from the source & destination points)
        return Matrix<double>.Build.DenseOfArray(new double[,] {
        { 1, 0, 0 },
        { 0, 1, 0 },
        { 0, 0, 1 }
    });
    }

    private void loadAccValues()
    {
        ////////////////////////////////////////
        //Code For storing Calibration/////////

        if (isAccMode)
        {
            accLight.color = Color.green;
        }
        else
        {
            accLight.color = Color.black;
        }

        if (activeScene == "CalibrationTest")
        {
            if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.f12Key.isPressed)
            {
                accSpot_xPos = float.Parse(unityCamValues.Split(':')[0]);
                accSpot_yPos = float.Parse(unityCamValues.Split(':')[1]);

                print("cam: values saved are: " + accSpot_xPos + " : " + accSpot_yPos);

                accP_pointTx.text = "Accurate point: (" + accSpot_xPos.ToString("0.0") + ":" + accSpot_yPos.ToString("0.0") + ")";
                y_CentrePos = accSpot_yPos;
                x_CentrePos = accSpot_xPos;

            }

            if (lowerAcc)
            {

                topIndicator.SetActive(false);
                bottomIndicator.SetActive(true);

                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.f8Key.isPressed)
                {
                    lowPoint_xPos = float.Parse(unityCamValues.Split(':')[0]);
                    lowPoint_yPos = float.Parse(unityCamValues.Split(':')[1]);
                    low_pointTxt.text = "Low point: (" + lowPoint_xPos.ToString("0.0") + ":" + lowPoint_yPos.ToString("0.0") + ")";

                    y_LowestMax = lowPoint_yPos;
                    low_y_max_txt.text = "Lowest y-max point: ( " + y_LowestMax.ToString("0.0") + " )";
                }
                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.f9Key.isPressed)
                {
                    //error correction point
                    low_XErrorPoint = float.Parse(unityCamValues.Split(':')[0]);
                    low_YErrorPoint = float.Parse(unityCamValues.Split(':')[1]);

                    errorP_pointTx.text = "error point: (" + low_XErrorPoint.ToString("0.0") + ":" + low_YErrorPoint.ToString("0.0") + ")";

                    low_error_correcPoint_txt.text = "error correction point: (" + low_YErrorPoint.ToString("0.0") + ")";
                }

            }
            if (upAcc)
            {
                topIndicator.SetActive(true);
                bottomIndicator.SetActive(false);

                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.f8Key.isPressed)
                {
                    highPoint_xPos = float.Parse(unityCamValues.Split(':')[0]);
                    highPoint_yPos = float.Parse(unityCamValues.Split(':')[1]);
                    high_pointTxt.text = "High point: (" + highPoint_xPos.ToString("0.0") + ":" + highPoint_yPos.ToString("0.0") + ")";

                    y_HighestMax = highPoint_yPos;
                    high_y_max_txt.text = "Highest y-max point: ( " + y_HighestMax.ToString("0.0") + " )";

                }

                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.f9Key.isPressed)
                {
                    //error correction point
                    high_point_xError = float.Parse(unityCamValues.Split(':')[0]);
                    high_point_yError = float.Parse(unityCamValues.Split(':')[1]);

                    errorP_pointTx.text = "error point: (" + low_XErrorPoint.ToString("0.0") + ":" + low_YErrorPoint.ToString("0.0") + ")"; //constant

                    high_error_correcPoint_txt.text = "error correction point: (" + high_point_yError.ToString("0.0") + ")";

                }
            }

            if(upLeftAcc)
            {
                leftIndicator.SetActive(true);
                rightIndicator.SetActive(false);

                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.f10Key.isPressed)
                {
                    upLeftPoint_xPos = float.Parse(unityCamValues.Split(':')[0]);
                    upLeftPoint_yPos = float.Parse(unityCamValues.Split(':')[1]);
                    left_pointTxt.text = "Left point: (" + upLeftPoint_xPos.ToString("0.0") + ":" + upLeftPoint_xPos.ToString("0.0") + ")";

                    upper_x_LowestMax = upLeftPoint_xPos;
                    left_x_max_txt.text = "Lowest x-max point: ( " + upper_x_LowestMax.ToString("0.0") + " )";

                }
                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.f11Key.isPressed)
                {
                    //error correction point
                    upLeft_point_xError = float.Parse(unityCamValues.Split(':')[0]);
                    upLeft_point_yError = float.Parse(unityCamValues.Split(':')[1]);

                    errorP_pointTx.text = "error point: (" + upLeft_point_xError.ToString("0.0") + ":" + upLeft_point_xError.ToString("0.0") + ")"; //constant

                    left_error_correcPoint_txt.text = "error correct point: (" + upLeft_point_xError.ToString("0.0") + ")";

                }
            }
            if (lowerLeftAcc)
            {
                leftIndicator.SetActive(true);
                rightIndicator.SetActive(false);

                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.f10Key.isPressed)
                {
                    lowerLeftPoint_xPos = float.Parse(unityCamValues.Split(':')[0]);
                    lowerLeftPoint_yPos = float.Parse(unityCamValues.Split(':')[1]);
                    left_pointTxt.text = "Left point: (" + lowerLeftPoint_xPos.ToString("0.0") + ":" + lowerLeftPoint_xPos.ToString("0.0") + ")";

                    bottom_x_LowestMax = lowerLeftPoint_xPos;
                    left_x_max_txt.text = "Lowest x-max point: ( " + bottom_x_LowestMax.ToString("0.0") + " )";

                }

                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.f11Key.isPressed)
                {
                    //error correction point
                    lowerLeft_point_xError = float.Parse(unityCamValues.Split(':')[0]);
                    lowerLeft_point_yError = float.Parse(unityCamValues.Split(':')[1]);

                    errorP_pointTx.text = "error point: (" + lowerLeft_point_xError.ToString("0.0") + ":" + lowerLeft_point_xError.ToString("0.0") + ")"; //constant

                    left_error_correcPoint_txt.text = "error correct point: (" + lowerLeft_point_xError.ToString("0.0") + ")";

                }
            }

            if (upRightAcc)
            {
                leftIndicator.SetActive(false);
                rightIndicator.SetActive(true);

                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.f10Key.isPressed)
                {
                    upRightPoint_xPos = float.Parse(unityCamValues.Split(':')[0]);
                    upRightPoint_yPos = float.Parse(unityCamValues.Split(':')[1]);
                    right_pointTxt.text = "Right point: (" + upRightPoint_xPos.ToString("0.0") + ":" + upRightPoint_xPos.ToString("0.0") + ")";

                    upper_x_HighestMax = upRightPoint_xPos;
                    right_x_max_txt.text = "Highest x-max point: ( " + upper_x_HighestMax.ToString("0.0") + " )";

                }

                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.f11Key.isPressed)
                {
                    //error correction point
                    upRight_point_xError = float.Parse(unityCamValues.Split(':')[0]);
                    upRight_point_yError = float.Parse(unityCamValues.Split(':')[1]);

                    errorP_pointTx.text = "error point: (" + upRight_point_xError.ToString("0.0") + ":" + upRight_point_xError.ToString("0.0") + ")"; //constant

                    right_error_correcPoint_txt.text = "error correct point: (" + upRight_point_xError.ToString("0.0") + ")";

                }
            }
            if (lowerRightAcc)
            {
                leftIndicator.SetActive(false);
                rightIndicator.SetActive(true);

                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.f10Key.isPressed)
                {
                    lowerRightPoint_xPos = float.Parse(unityCamValues.Split(':')[0]);
                    lowerRightPoint_yPos = float.Parse(unityCamValues.Split(':')[1]);
                    right_pointTxt.text = "Right point: (" + lowerRightPoint_xPos.ToString("0.0") + ":" + lowerRightPoint_xPos.ToString("0.0") + ")";

                    bottom_x_HighestMax = lowerRightPoint_xPos;
                    right_x_max_txt.text = "Highest x-max point: ( " + bottom_x_HighestMax.ToString("0.0") + " )";

                }

                if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.f11Key.isPressed)
                {
                    //error correction point
                    lowerRight_point_xError = float.Parse(unityCamValues.Split(':')[0]);
                    lowerRight_point_yError = float.Parse(unityCamValues.Split(':')[1]);

                    errorP_pointTx.text = "error point: (" + lowerRight_point_xError.ToString("0.0") + ":" + lowerRight_point_xError.ToString("0.0") + ")"; //constant

                    right_error_correcPoint_txt.text = "error correct point: (" + lowerRight_point_xError.ToString("0.0") + ")";

                }
            }
            
        }   

        ///////////////////END//////////////////
        ///////////////////////////////////////
        //error_Val
    }
    private void DrawOrgLines(Vector2Int p)
    {
        /*************Variables to draw bullets on targets********************/
        //Debug.Log("p up:" + p.ToString() + " vs X" + CalibratePoint1.ToString() + " vs Y:" + CalibratePoint2.ToString());

        float eX = 0, eY = 0;

        if (p.x > CalibratePoint1.x && p.x < CalibratePoint3.x && p.y < CalibratePoint1.y && p.y > CalibratePoint3.y)
        {
            print("In original lines...");

            if (FlagBulletRx == false)
            {
                FlagBulletRx = true;
                CountShotFired++;//increment every time a shot is detected
            }
            eX = Mathf.Abs(p.x - (CalibratePoint1.x));
            eX = Mathf.Abs((CalibratePoint3.x - CalibratePoint1.x)) / eX;
            eY = Mathf.Abs(p.y - (CalibratePoint3.y));
            eY = Mathf.Abs(CalibratePoint3.y - CalibratePoint1.y) / eY;

            Debug.Log("p:" + p.ToString() + "  eX:" + eX + " eY" + eY);

            try
            {
                //Debug.Log("Backpoint=" + p.ToString() + ", P1:" + CalibratePoint1.ToString() + ", P2:" + CalibratePoint2.ToString());
                //print("I am in draw original line");
                Ball1X = Screen.width / eX;
                Ball1Y = Screen.height / eY;
                unityCamValues = Ball1X.ToString() + ":" + Ball1Y.ToString();

                //string newPoint = Ball1X.ToString() + ":" + Ball1Y.ToString();
                //bool pointInRadius = false;

                //pointInRadius = CheckNewPoint(newPoint);
                //pointInRadiusG = pointInRadius;

                if (Scoring.ammo_setting == "Live")
                {
                    ray = Camera.main.ScreenPointToRay(new Vector3((int)(Ball1X * 1f), (int)((Ball1Y * 1f)), 0));
                }
                else
                {
                    ray = Camera.main.ScreenPointToRay(new Vector3((int)(Ball1X * 1f), (int)((Ball1Y * 1f)), 0));
                }

            }
            catch (Exception e)
            {
                Debug.Log("Error:" + e.Message);
            }



        }//end of  if (p.x > CalibratePoint1.x && p.x < CalibratePoint2.x
        //Debug.Log("p:"+ p.ToString() + "  BallX:" + Ball1X + " BallY" + Ball1Y);
        p = new Vector2Int(0, 0);



    }//end of DrawNewLines
    void display_1()
    {
        //Display
        if (activeScene == "CalibrationTest")
        {
            errorValue_txt.text = "Error Value: (" + LwError.ToString("0.0") + ")";
            eyValue_txt.text = "Ey Value: (" + Ey.ToString("0.0") + ")";
        }
    }
    void display_2()
    {
        //Display
        if (activeScene == "CalibrationTest" )
        {
            H_errorValue_txt.text = "Error Value: (" + HP_errorValue.ToString("0.0") + ")";
            H_eyValue_txt.text = "Ey Value: (" + Ey.ToString("0.0") + ")";
        }
    }
    private void display_3()
    {
        //Display
        if (activeScene == "CalibrationTest" )
        {
            L_errorValue_txt.text = "Error Value: (" + LfP_errorValue.ToString("0.0") + ")";
            L_exValue_txt.text = "EX Value: (" + Ex.ToString("0.0") + ")";
        }
    }
    private void display_4()
    {
        //Display
        if (activeScene == "CalibrationTest" )
        {
            R_errorValue_txt.text = "Error Value: (" + RP_errorValue.ToString("0.0") + ")";
            R_exValue_txt.text = "EX Value: (" + Ex.ToString("0.0") + ")";
        }
    }
    void saveAcc()
    {
        Calibration.isAccMode = isAccMode;

        //-lowest point
        Calibration.lowPoint_yPos = lowPoint_yPos ;
        Calibration.low_YErrorPoint = low_YErrorPoint ;       //lowest y error point
        
        //-highest point
        Calibration.highPoint_yPos = highPoint_yPos ;
        Calibration.high_point_yError = high_point_yError ;     //highest y error point

        //-far left point
        Calibration.upLeftPoint_xPos = upLeftPoint_xPos ;
        Calibration.upLeft_point_xError = upLeft_point_xError ;
        Calibration.lowerLeftPoint_xPos = lowerLeftPoint_xPos;
        Calibration.lowerLeft_point_xError = lowerLeft_point_xError;

        //-far right point
        Calibration.upRightPoint_xPos = upRightPoint_xPos ;
        Calibration.upRight_point_xError = upRight_point_xError ;
        Calibration.lowerRightPoint_xPos = lowerRightPoint_xPos;
        Calibration.lowerRight_point_xError = lowerRight_point_xError;

        //Centre point
        Calibration.y_CentrePos = y_CentrePos ;
        Calibration.x_CentrePos = x_CentrePos ;
        //END

        AccSaveData = lowPoint_yPos + ":" + low_YErrorPoint + ":" + highPoint_yPos + ":" + high_point_yError + ":" + upLeftPoint_xPos + ":" + upLeft_point_xError + ":" + upRightPoint_xPos + ":" + upRight_point_xError + ":" + lowerLeftPoint_xPos + ":" + lowerLeft_point_xError + ":" + lowerRightPoint_xPos + ":" + lowerRight_point_xError + ":"+ y_CentrePos+ ":"+ x_CentrePos;
    }
    private void assignAccValues()
    {
        //lowest point
        lowPoint_yPos = Calibration.lowPoint_yPos;
        low_YErrorPoint = Calibration.low_YErrorPoint;       //lowest y error point

        //-highest point
        highPoint_yPos = Calibration.highPoint_yPos;
        high_point_yError = Calibration.high_point_yError;     //highest y error point

        //-far up left point
        upLeftPoint_xPos = Calibration.upLeftPoint_xPos;
        upLeft_point_xError = Calibration.upLeft_point_xError;
        //-far lower left point
        lowerLeftPoint_xPos = Calibration.lowerLeftPoint_xPos;
        lowerLeft_point_xError = Calibration.lowerLeft_point_xError;

        //-far up right point
        upRightPoint_xPos = Calibration.upRightPoint_xPos;
        upRight_point_xError = Calibration.upRight_point_xError;
        //-far lower right point
        lowerRightPoint_xPos = Calibration.lowerRightPoint_xPos;
        lowerRight_point_xError = Calibration.lowerRight_point_xError;

        //centre point
        y_CentrePos = Calibration.y_CentrePos;
        x_CentrePos = Calibration.x_CentrePos;

        y_LowestMax = lowPoint_yPos;
        y_HighestMax = highPoint_yPos;
        bottom_x_LowestMax = lowerLeftPoint_xPos;
        bottom_x_HighestMax = lowerRightPoint_xPos;
        upper_x_LowestMax = upLeftPoint_xPos;
        upper_x_HighestMax = upRightPoint_xPos;


        //Display
        if (activeScene == "CalibrationTest")
        {
            accP_pointTx.text = "Accurate point: (" + accSpot_xPos.ToString("0.0") + ":" + accSpot_yPos.ToString("0.0") + ")";

            low_y_max_txt.text = "Lowest y-max point: ( " + y_LowestMax.ToString("0.0") + " )";
            errorP_pointTx.text = "error point: (" + low_XErrorPoint.ToString("0.0") + ":" + low_YErrorPoint.ToString("0.0") + ")";

            low_error_correcPoint_txt.text = "error correction point: (" + low_YErrorPoint.ToString("0.0") + ")";
            high_y_max_txt.text = "Highest y-max point: ( " + y_HighestMax.ToString("0.0") + " )";
            high_error_correcPoint_txt.text = "error correction point: (" + high_point_yError.ToString("0.0") + ")";
            errorP_pointTx.text = "error point: (" + low_XErrorPoint.ToString("0.0") + ":" + low_YErrorPoint.ToString("0.0") + ")"; //constant

            left_x_max_txt.text = "Lowest x-max point: ( " + bottom_x_LowestMax.ToString("0.0") + " )";
            errorP_pointTx.text = "error point: (" + upLeft_point_xError.ToString("0.0") + ":" + upLeft_point_xError.ToString("0.0") + ")"; //constant

            left_error_correcPoint_txt.text = "error correct point: (" + upLeft_point_xError.ToString("0.0") + ")";
            right_x_max_txt.text = "Highest x-max point: ( " + bottom_x_HighestMax.ToString("0.0") + " )";

            errorP_pointTx.text = "error point: (" + upRight_point_xError.ToString("0.0") + ":" + upRight_point_xError.ToString("0.0") + ")"; //constant

            right_error_correcPoint_txt.text = "error correct point: (" + upRight_point_xError.ToString("0.0") + ")";
        }

        //print("RE: Acc Assigned...");
        //END
    }
    private float calculateError(float currP, float centre_point, float maxError, float highestAxesP)
    {
        //calculation 
        float result = Emin + (currP - centre_point) * ((maxError - Emin) / (highestAxesP - centre_point));

        return (result);
    }
    private void LoadCalibration()
    {
        if (!File.Exists(calibrationPath))
        {
            File.Create(calibrationPath);

        }
        else
        {
            //FileManager.ReadFromFile(DelaySavePath);
            //Debug.Log("Calibration Found");

            //string[] calibrationPoints = System.IO.File.ReadAllLines(calibrationPath);
            //
            string[] calibrationPoints = FileManager.ReadLinesFromFile(calibrationPath);

            foreach (string line in calibrationPoints)
            {
                if (line.Contains("points"))
                {
                    int x1 = Int32.Parse((line.Split(':')[1]).Split(',')[0]);
                    int y1 = Int32.Parse((line.Split(':')[1]).Split(',')[1]);
                    CalibratePoint1 = new Vector2Int(x1, y1);
                    //Debug.Log("Loaded Point1 as" + CalibratePoint1.ToString());

                    int x2 = Int32.Parse((line.Split(':')[2]).Split(',')[0]);
                    int y2 = Int32.Parse((line.Split(':')[2]).Split(',')[1]);
                    CalibratePoint2 = new Vector2Int(x2, y2);
                    //Debug.Log("Loaded Point2 as" + CalibratePoint2.ToString());

                    int x3 = Int32.Parse((line.Split(':')[3]).Split(',')[0]);
                    int y3 = Int32.Parse((line.Split(':')[3]).Split(',')[1]);
                    CalibratePoint3 = new Vector2Int(x3, y3);
                    //Debug.Log("Loaded Point3 as" + CalibratePoint3.ToString());

                    int x4 = Int32.Parse((line.Split(':')[4]).Split(',')[0]);
                    int y4 = Int32.Parse((line.Split(':')[4]).Split(',')[1]);
                    CalibratePoint4 = new Vector2Int(x4, y4);
                    //Debug.Log("Loaded Point4 as" + CalibratePoint4.ToString());

                    int x5 = Int32.Parse((line.Split(':')[5]).Split(',')[0]);
                    int y5 = Int32.Parse((line.Split(':')[5]).Split(',')[1]);
                    CalibratePoint5 = new Vector2Int(x5, y5);
                    //Debug.Log("Loaded Point5 as" + CalibratePoint5.ToString());
                }
            }

            // Load homography matrix
            homographyMatrix = CalculateHomographyMatrix(CalibratePoint1, CalibratePoint2, CalibratePoint3, CalibratePoint4);
        }
    }
    void sceneOpenPlainShoot(RaycastHit hit)
    {
        all_soldiers = GameObject.FindGameObjectsWithTag("soldier");

        if (!activeScene.ToLower().Contains("crouching"))
        {
            foreach (GameObject sol in all_soldiers)
            {
                sol.transform.gameObject.GetComponent<PoliceBehaviour>().SendMessage("ApplyDamage", "change");
            }
        }
        
        //Debug.Log("hit:" + hit.transform.name);
        if (hit.transform.name.Contains("civilian"))
        {
            hit.rigidbody.AddForce(-hit.normal * 10F);
            Animator hit_anim = hit.transform.gameObject.GetComponent<Animator>();
            //hit_anim.Play("dying");
            if (hit.transform.name.Contains("Karen"))
            {
                hit.transform.gameObject.GetComponent<CBehaviour>().SendMessage("ApplyDamage", "shot");
            }
            else
            {
                hit.transform.gameObject.GetComponent<civilian_behaviour>().SendMessage("ApplyDamage", "shot");
                hit.transform.GetComponent<CapsuleCollider>().enabled = false;
                hit.transform.GetComponent<Rigidbody>().isKinematic = false;
                hit.transform.GetComponent<Rigidbody>().useGravity = false;
            }
            //hit.transform.gameObject.GetComponent<civilian_behaviour>().SendMessage("ApplyDamage", "shot");
            //hit.transform.GetComponent<CapsuleCollider>().enabled = false;

            //GameObject impactGo = Instantiate(blood, hit.point, Quaternion.LookRotation(hit.normal));
            //Destroy(impactGo, 2f);


            ladyScreamSound.Play();
            civilianShot++;

        }
        //HumanTarget
        else if(hit.transform.name.Contains("HumanTarget"))
        {
            hit.transform.gameObject.GetComponent<TargetMessageReceive>().SendMessage("ApplyDamage", hit.transform.name);
            //print("I AM IN the New Scene NOOOOW...");
        }
        else if (hit.transform.name.Contains("soldier"))
        {
            if(activeScene.ToLower().Contains("basic"))
            {
                hit.transform.gameObject.GetComponent<TargetMessageReceive>().SendMessage("ApplyDamage", hit.transform.name);
            }
            else
            {

                Component enemy = new Component();
                hit.rigidbody.AddForce(-hit.normal * 2F);
                hit.transform.gameObject.GetComponent<PoliceBehaviour>().SendMessage("ApplyDamage", hit.transform.name);
            }

            //GameObject impactGo = Instantiate(blood, hit.point, Quaternion.LookRotation(hit.normal));
            //Destroy(impactGo, 2f);

            //enemyshot++;
        }
        else if (hit.transform.name.Contains("swat"))
        {

            Component enemy = new Component();
            hit.rigidbody.AddForce(-hit.normal * 2F);
            //hit.transform.gameObject.GetComponent<SwatHideBehaviour>().SendMessage("ApplyDamage", hit.transform.name);

      
            //GameObject impactGo = Instantiate(blood, hit.point, Quaternion.LookRotation(hit.normal));
            //Destroy(impactGo, 2f);

            //enemyshot++;
        }
        else if (hit.transform.tag.ToLower().Contains("target"))
        {       
            Component enemy = new Component();
            //print("Hit point reached...");
            if (activeScene.ToLower().Contains("targetpopup") || activeScene.ToLower().Contains("block"))
            {
                //hit.transform.gameObject.GetComponent<TargetMovement>().SendMessage("ApplyDamage", hit.transform.name);
            }
            if(activeScene.ToLower().Contains("basic"))
            {
                //Apply Targate Impact
                sendHitSignal(hit); //Apply Damage

                if (!activeScene.ToLower().Contains("risingplate"))
                {
                    platesHit++;
                }
                if (hit.transform.name.ToLower().Contains("plate"))
                {
                    //platHitSound.Stop();
                    platHitSound.Play();
                }

                if (activeScene.ToLower().Contains("ipec"))
                {
                    if (hit.transform.name.ToLower().Contains("plate"))
                    {
                        //Evaluate Current Lane
                        string tempTargetName = hit.transform.name;
                        char tempFirstChar = tempTargetName[0];
                        switch (tempFirstChar)
                        {
                            case '1':
                                if (lane1TargetsComplete == false)
                                {
                                    //Calculate Split time
                                    updateLane1SplitTime("hit", "c");

                                    updateLane1ResponseTime("hit", "c");
                                    generateBulletImpact(hit);

                                    lane1ActiveTimeCounter = lane1TimeActiveCounter; //Assign Current Time

                                    point_lane_1 = updateLanePoint(point_lane_1);
                                    lane1PointsHit++;
                                    //print("RE: " + lane1PointsHit);
                                    //print("RE: Targets imput is" + num_targets_input);


                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane1PointsHit + (shotsFired - lane1PointsHit) >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            //lane1StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane1PointsHit >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            lane1StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane1ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;

                            case '2':
                                if (lane2TargetsComplete == false)
                                {
                                    //Calculate Split time
                                    updateLane2SplitTime("hit", "c");

                                    updateLane2ResponseTime("hit","c");
                                    generateBulletImpact(hit);

                                    lane2ActiveTimeCounter = lane2TimeActiveCounter; //Assign Current Time

                                    point_lane_2 = updateLanePoint(point_lane_2);
                                    lane2PointsHit++;
                                    print("RE: " + lane2PointsHit);
                                    print("RE: Targets imput is" + num_targets_input);

                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane2PointsHit + (shotsFired - lane2PointsHit) >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            //lane2StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane2PointsHit >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            lane2StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane2ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;

                            case '3':
                                if (lane3TargetsComplete == false)
                                {
                                    //Calculate Split time
                                    updateLane3SplitTime("hit", "c");

                                    updateLane3ResponseTime("hit","c");
                                    generateBulletImpact(hit);

                                    lane3ActiveTimeCounter = lane3TimeActiveCounter; //Assign Current Time

                                    point_lane_3 = updateLanePoint(point_lane_3);
                                    lane3PointsHit++;
                                    print("RE: " + lane3PointsHit);

                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane3PointsHit + (shotsFired - lane3PointsHit) >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            //lane3StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane3PointsHit >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            lane3StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane3ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;
                        }

                        manageLaneTarinigStopCondition();
                    }
                    else
                    {
                        platesHit--;
                        if (platesHit <= 0)
                        {
                            platesHit = 0;
                        }
                    }

                    if ((hit.transform.name.ToLower().Contains("body") || hit.transform.name.ToLower().Contains("head")))
                    {
                        boardHit = boardHit + 1;

                        print("Number of board hits is: " + boardHit);
                        if (boardHit >= 5)
                        {
                            //boardDestroyed = true;
                            //GameOver = true;
                            //StopTraining();
                        }

                    }
                }
                if (activeScene.ToLower().Contains("rifflepole"))
                {

                    //Evaluate Current Lane
                    string tempTargetName = hit.transform.name;
                    char tempFirstChar = tempTargetName[0];
                    switch (tempFirstChar)
                    {
                        case '1':
                            if (lane1TargetsComplete == false)
                            {
                                //Calculate Split time
                                updateLane1SplitTime("hit", "c");

                                updateLane1ResponseTime("hit","c");
                                generateBulletImpact(hit);

                                lane1ActiveTimeCounter = lane1TimeActiveCounter; //Assign Current Time

                                point_lane_1 = updateLanePoint(point_lane_1);
                                lane1PointsHit++;
                                //print("RE: " + lane1PointsHit);
                                //print("RE: Targets imput is" + num_targets_input);


                                if (Scoring.ammo_setting == "Live")
                                {
                                    //targetText.text = "Hit Plate" + platesHit;
                                    /*if (lane1PointsHit + (shotsFired - lane1PointsHit) >= num_targets_input)
                                    {
                                        lane1TargetsComplete = true;
                                        lane1ActiveTimeCounter = timeActiveCounter;
                                        //lane1StopSignal.SetActive(true);
                                    }*/
                                }
                                else
                                {
                                    //targetText.text = "Hit Plate" + platesHit;
                                    if (lane1PointsHit >= num_targets_input)
                                    {
                                        lane1TargetsComplete = true;
                                        lane1ActiveTimeCounter = timeActiveCounter;
                                        lane1StopSignal.SetActive(true);
                                    }
                                }

                                if (targetFinished && targetsFinished)
                                {
                                    lane1ActiveTimeCounter = timeActiveCounter;
                                }
                            }
                            break;

                        case '2':
                            if (lane2TargetsComplete == false)
                            {
                                //Calculate Split time
                                updateLane2SplitTime("hit", "c");

                                updateLane2ResponseTime("hit","c");
                                generateBulletImpact(hit);

                                lane2ActiveTimeCounter = lane2TimeActiveCounter; //Assign Current Time

                                point_lane_2 = updateLanePoint(point_lane_2);
                                lane2PointsHit++;
                                print("RE: " + lane2PointsHit);
                                print("RE: Targets imput is" + num_targets_input);

                                if (Scoring.ammo_setting == "Live")
                                {
                                    //targetText.text = "Hit Plate" + platesHit;
                                    /*if (lane2PointsHit + (shotsFired - lane2PointsHit) >= num_targets_input)
                                    {
                                        lane2TargetsComplete = true;
                                        lane2ActiveTimeCounter = timeActiveCounter;
                                        //lane2StopSignal.SetActive(true);
                                    }*/
                                }
                                else
                                {
                                    //lane1ActiveTimeCounter = timeActiveCounter;
                                    //targetText.text = "Hit Plate" + platesHit;
                                    if (lane2PointsHit >= num_targets_input)
                                    {
                                        lane2TargetsComplete = true;
                                        lane2ActiveTimeCounter = timeActiveCounter;
                                        lane2StopSignal.SetActive(true);
                                    }
                                }

                                if (targetFinished && targetsFinished)
                                {
                                    lane2ActiveTimeCounter = timeActiveCounter;
                                }
                            }
                            break;

                        case '3':
                            if (lane3TargetsComplete == false)
                            {
                                //Calculate Split time
                                updateLane3SplitTime("hit", "c");

                                updateLane3ResponseTime("hit","c");
                                generateBulletImpact(hit);

                                lane3ActiveTimeCounter = lane3TimeActiveCounter; //Assign Current Time

                                point_lane_3 = updateLanePoint(point_lane_3);
                                lane3PointsHit++;
                                print("RE: " + lane3PointsHit);

                                if (Scoring.ammo_setting == "Live")
                                {
                                    //targetText.text = "Hit Plate" + platesHit;
                                    /*if (lane3PointsHit + (shotsFired - lane3PointsHit) >= num_targets_input)
                                    {
                                        lane3TargetsComplete = true;
                                        lane3ActiveTimeCounter = timeActiveCounter;
                                        //lane3StopSignal.SetActive(true);
                                    }*/
                                }
                                else
                                {
                                    //lane1ActiveTimeCounter = timeActiveCounter;
                                    //targetText.text = "Hit Plate" + platesHit;
                                    if (lane3PointsHit >= num_targets_input)
                                    {
                                        lane3TargetsComplete = true;
                                        lane3ActiveTimeCounter = timeActiveCounter;
                                        lane3StopSignal.SetActive(true);
                                    }
                                }

                                if (targetFinished && targetsFinished)
                                {
                                    lane3ActiveTimeCounter = timeActiveCounter;
                                }
                            }
                            break;
                    }

                    manageLaneTarinigStopCondition();

                    /*if (hit.transform.name.ToLower().Contains("plate") || hit.transform.name.ToLower().Contains("target "))
                    {
                        if (Scoring.simulation_type == "training")
                        {
                            targetText.text = "Hit Plate" + platesHit;
                            if (platesHit + (shotsFired - platesHit) >= num_targets_input)
                            {
                                targetsFinished = true;
                                targetFinished = true;
                                StopTraining();
                            }
                        }
                        else
                        {
                            targetText.text = "Hit Plate" + platesHit;
                            if (platesHit >= num_targets_input)
                            {
                                targetsFinished = true;
                                targetFinished = true;
                                StopTraining();
                            }
                        }
                    }
                    else
                    {
                        platesHit--;
                        if (platesHit <= 0)
                        {
                            platesHit = 0;
                        }
                    }*/

                }
                if (activeScene.ToLower().Contains("cyclic"))
                {
                    
                    //num_targets_input
                    print("RE: HIT "+ hit.transform.name );
                    if (hit.transform.name.ToLower().Contains("point"))
                    {
                        //Evaluate Current Lane
                        string tempTargetName = hit.transform.name;
                        char tempFirstChar = tempTargetName[0];
                        switch(tempFirstChar)
                        {
                            case '1':
                                if(lane1TargetsComplete == false)
                                {
                                    generateBulletImpact(hit);
                                    //Calculate Split time
                                    updateLane1SplitTime("hit", "c");
                                    lane1ActiveTimeCounter = lane1TimeActiveCounter; //Assign Current Time

                                    point_lane_1 = updateLanePoint(tempTargetName, point_lane_1);
                                    lane1PointsHit++;
                                    print("RE: " + lane1PointsHit);
                                    print("RE: Targets imput is" + num_targets_input);


                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane1PointsHit + (shotsFired - lane1PointsHit) >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            //lane1StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane1PointsHit >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            lane1StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane1ActiveTimeCounter = timeActiveCounter;
                                    }
                                }                                
                                break;

                            case '2':
                                if (lane2TargetsComplete == false)
                                {
                                    generateBulletImpact(hit);
                                    //Calculate Split time
                                    updateLane2SplitTime("hit", "c");
                                    lane2ActiveTimeCounter = lane2TimeActiveCounter; //Assign Current Time

                                    point_lane_2 = updateLanePoint(tempTargetName, point_lane_2);
                                    lane2PointsHit++;
                                    print("RE: " + lane2PointsHit);
                                    print("RE: Targets imput is" + num_targets_input);

                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane2PointsHit + (shotsFired - lane2PointsHit) >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            //lane2StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane2PointsHit >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            lane2StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane2ActiveTimeCounter = timeActiveCounter;
                                    }
                                }                                
                                break;

                            case '3':
                                if (lane3TargetsComplete == false)
                                {
                                    generateBulletImpact(hit);
                                    //Calculate Split time
                                    updateLane3SplitTime("hit", "c");
                                    lane3ActiveTimeCounter = lane3TimeActiveCounter; //Assign Current Time

                                    point_lane_3 = updateLanePoint(tempTargetName, point_lane_3);
                                    lane3PointsHit++;
                                    print("RE: " + lane3PointsHit);

                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane3PointsHit + (shotsFired - lane3PointsHit) >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            //lane3StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane3PointsHit >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            lane3StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane3ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;
                        }

                        manageLaneTarinigStopCondition();
                    }
                    else
                    {
                        platesHit--;
                        if (platesHit <= 0)
                        {
                            platesHit = 0;
                        }
                    }

                }
                if (activeScene.ToLower().Contains("5target"))
                {

                    //num_targets_input
                    //print("RE: HIT " + hit.transform.name);
                    if (hit.transform.name.ToLower().Contains("point"))
                    {
                        //Evaluate Current Lane
                        string tempTargetName = hit.transform.name;
                        char tempFirstChar = tempTargetName[0];
                        switch (tempFirstChar)
                        {
                            case '1':
                                if (lane1TargetsComplete == false)
                                {
                                    generateBulletImpact(hit);
                                    //Calculate Split time
                                    if (startLane1Count)
                                    {
                                        numLane1Splits++;
                                        if (lane1SplitTimeCounter < 1)
                                        {
                                            lane1SplitTime.Add(lane1SplitTimeCounter.ToString("0.00") + "ms");
                                        }
                                        else
                                        {
                                            lane1SplitTime.Add(lane1SplitTimeCounter.ToString("0.00") + "sec");
                                        }

                                        lane1SplitTimeCounter = 0;
                                    }
                                    else
                                    {
                                        startLane1Count = true;
                                    }
                                    lane1ActiveTimeCounter = lane1TimeActiveCounter; //Assign Current Time


                                    point_lane_1 = updateLanePoint(tempTargetName, point_lane_1);
                                    lane1PointsHit++;
                                    print("RE: " + lane1PointsHit);
                                    print("RE: Targets imput is" + num_targets_input);


                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane1PointsHit + (shotsFired - lane1PointsHit) >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            //lane1StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane1PointsHit >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            lane1StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane1ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;

                            case '2':
                                if (lane2TargetsComplete == false)
                                {
                                    generateBulletImpact(hit);
                                    //Calculate Split time
                                    if (startLane2Count)
                                    {
                                        numLane2Splits++;
                                        if (lane2SplitTimeCounter < 1)
                                        {
                                            lane2SplitTime.Add(lane2SplitTimeCounter.ToString("0.00") + "ms");
                                        }
                                        else
                                        {
                                            lane2SplitTime.Add(lane2SplitTimeCounter.ToString("0.00") + "sec");
                                        }

                                        lane2SplitTimeCounter = 0;
                                    }
                                    else
                                    {
                                        startLane2Count = true;
                                    }
                                    lane2ActiveTimeCounter = lane2TimeActiveCounter; //Assign Current Time

                                    point_lane_2 = updateLanePoint(tempTargetName, point_lane_2);
                                    lane2PointsHit++;
                                    print("RE: " + lane2PointsHit);
                                    print("RE: Targets imput is" + num_targets_input);

                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane2PointsHit + (shotsFired - lane2PointsHit) >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            //lane2StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane2PointsHit >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            lane2StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane2ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;

                            case '3':
                                if (lane3TargetsComplete == false)
                                {
                                    generateBulletImpact(hit);
                                    //Calculate Split time
                                    if (startLane3Count)
                                    {
                                        numLane3Splits++;
                                        if (lane3SplitTimeCounter < 1)
                                        {
                                            lane3SplitTime.Add(lane3SplitTimeCounter.ToString("0.00") + "ms");
                                        }
                                        else
                                        {
                                            lane3SplitTime.Add(lane3SplitTimeCounter.ToString("0.00") + "sec");
                                        }

                                        lane3SplitTimeCounter = 0;
                                    }
                                    else
                                    {
                                        startLane3Count = true;
                                    }
                                    lane3ActiveTimeCounter = lane3TimeActiveCounter; //Assign Current Time

                                    point_lane_3 = updateLanePoint(tempTargetName, point_lane_3);
                                    lane3PointsHit++;
                                    print("RE: " + lane3PointsHit);

                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane3PointsHit + (shotsFired - lane3PointsHit) >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            //lane3StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane3PointsHit >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            lane3StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane3ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;
                        }

                        manageLaneTarinigStopCondition();
                    }
                    else
                    {
                        platesHit--;
                        if (platesHit <= 0)
                        {
                            platesHit = 0;
                        }
                    }

                }
                if(activeScene.ToLower().Contains("shell"))
                {
                    if (hit.transform.name.ToLower().Contains("holder"))
                    {
                        //print("point reached");
                        //Evaluate Current Lane
                        string tempTargetName = hit.transform.name;
                        char tempFirstChar = tempTargetName[0];
                        switch (tempFirstChar)
                        {
                            case '1':
                                if (lane1TargetsComplete == false)
                                {
                                    //Calculate Split time
                                    updateLane1SplitTime("hit", "c");
                                    updateLane1ResponseTime("hit", "c");

                                    lane1ActiveTimeCounter = lane1TimeActiveCounter; //Assign Current Time

                                    point_lane_1 = updateLanePoint(point_lane_1);
                                    lane1PointsHit++;
                                    //print("RE: " + lane1PointsHit);
                                    //print("RE: Targets imput is" + num_targets_input);


                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane1PointsHit + (shotsFired - lane1PointsHit) >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            //lane1StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane1PointsHit >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            lane1StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane1ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;

                            case '2':
                                if (lane2TargetsComplete == false)
                                {
                                    //Calculate Split time
                                    updateLane2SplitTime("hit", "c");
                                    updateLane2ResponseTime("hit", "c");

                                    lane2ActiveTimeCounter = lane2TimeActiveCounter; //Assign Current Time

                                    point_lane_2 = updateLanePoint(point_lane_2);
                                    lane2PointsHit++;
                                    print("RE: " + lane2PointsHit);
                                    print("RE: Targets imput is" + num_targets_input);

                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane2PointsHit + (shotsFired - lane2PointsHit) >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            //lane2StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane2PointsHit >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            lane2StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane2ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;

                            case '3':
                                if (lane3TargetsComplete == false)
                                {
                                    //Calculate Split time
                                    updateLane3SplitTime("hit", "c");
                                    updateLane3ResponseTime("hit", "c");

                                    lane3ActiveTimeCounter = lane3TimeActiveCounter; //Assign Current Time

                                    point_lane_3 = updateLanePoint(point_lane_3);
                                    lane3PointsHit++;
                                    print("RE: " + lane3PointsHit);

                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane3PointsHit + (shotsFired - lane3PointsHit) >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            //lane3StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane3PointsHit >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            lane3StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane3ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;
                        }

                        manageLaneTarinigStopCondition();
                    }
                    else
                    {
                        platesHit--;
                        if (platesHit <= 0)
                        {
                            platesHit = 0;
                        }
                    }
                }
                if (activeScene.ToLower().Contains("cargame"))
                {
                    if (hit.transform.name.ToLower().Contains("plate"))
                    {
                        //print("point reached");
                        //Evaluate Current Lane
                        string tempTargetName = hit.transform.name;
                        char tempFirstChar = tempTargetName[0];
                        switch (tempFirstChar)
                        {
                            case '1':
                                if (lane1TargetsComplete == false)
                                {
                                    //Calculate Split time
                                    updateLane1SplitTime("hit", "c");
                                    updateLane1ResponseTime("hit", "c");

                                    lane1ActiveTimeCounter = lane1TimeActiveCounter; //Assign Current Time

                                    point_lane_1 = updateLanePoint(point_lane_1);
                                    lane1PointsHit++;
                                    //print("RE: " + lane1PointsHit);
                                    //print("RE: Targets imput is" + num_targets_input);


                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane1PointsHit + (shotsFired - lane1PointsHit) >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            //lane1StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane1PointsHit >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            lane1StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane1ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;

                            case '2':
                                if (lane2TargetsComplete == false)
                                {
                                    //Calculate Split time
                                    updateLane2SplitTime("hit", "c");
                                    updateLane2ResponseTime("hit", "c");

                                    lane2ActiveTimeCounter = lane2TimeActiveCounter; //Assign Current Time

                                    point_lane_2 = updateLanePoint(point_lane_2);
                                    lane2PointsHit++;
                                    //print("RE: " + lane2PointsHit);
                                    //print("RE: Targets imput is" + num_targets_input);

                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane2PointsHit + (shotsFired - lane2PointsHit) >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            //lane2StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane2PointsHit >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            lane2StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane2ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;

                            case '3':
                                if (lane3TargetsComplete == false)
                                {
                                    //Calculate Split time
                                    updateLane3SplitTime("hit", "c");
                                    updateLane3ResponseTime("hit", "c");

                                    lane3ActiveTimeCounter = lane3TimeActiveCounter; //Assign Current Time

                                    point_lane_3 = updateLanePoint(point_lane_3);
                                    lane3PointsHit++;
                                    print("RE: " + lane3PointsHit);

                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane3PointsHit + (shotsFired - lane3PointsHit) >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            //lane3StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane3PointsHit >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            lane3StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane3ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;
                        }

                        manageLaneTarinigStopCondition();
                    }
                    else
                    {
                        platesHit--;
                        if (platesHit <= 0)
                        {
                            platesHit = 0;
                        }
                    }
                }
                if (activeScene.ToLower().Contains("plat") || activeScene.ToLower().Contains("dueling") || activeScene.ToLower().Contains("dice") || activeScene.ToLower().Contains("distancesimulator"))
                {
                    if (hit.transform.name.ToLower().Contains("plate"))
                    {
                        //Evaluate Current Lane
                        string tempTargetName = hit.transform.name;
                        char tempFirstChar = tempTargetName[0];
                        switch (tempFirstChar)
                        {
                            case '1':
                                if (lane1TargetsComplete == false && !StaticVariableManager.lane1PredelayActive)
                                {
                                    //Calculate Split time
                                    updateLane1SplitTime("hit", "c");
                                    updateLane1ResponseTime("hit","c");
                                    generateBulletImpact(hit);

                                    lane1ActiveTimeCounter = lane1TimeActiveCounter; //Assign Current Time

                                    point_lane_1 = updateLanePoint(point_lane_1);
                                    lane1PointsHit++;
                                    //print("RE: " + lane1PointsHit);
                                    //print("RE: Targets imput is" + num_targets_input);


                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane1PointsHit + (shotsFired - lane1PointsHit) >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            //lane1StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane1PointsHit >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            lane1StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane1ActiveTimeCounter = timeActiveCounter;
                                    }

                                    //StaticVariableManager.lane1PredelayActive = true;
                                }
                                break;

                            case '2':
                                if (lane2TargetsComplete == false && !StaticVariableManager.lane2PredelayActive)
                                {
                                    //Calculate Split time
                                    updateLane2SplitTime("hit", "c");
                                    updateLane2ResponseTime("hit","c");
                                    generateBulletImpact(hit);

                                    lane2ActiveTimeCounter = lane2TimeActiveCounter; //Assign Current Time

                                    point_lane_2 = updateLanePoint(point_lane_2);
                                    lane2PointsHit++;
                                    print("RE: " + lane2PointsHit);
                                    print("RE: Targets imput is" + num_targets_input);

                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane2PointsHit + (shotsFired - lane2PointsHit) >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            //lane2StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane2PointsHit >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            lane2StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane2ActiveTimeCounter = timeActiveCounter;
                                    }

                                    //StaticVariableManager.lane2PredelayActive = true;
                                }
                                break;

                            case '3':
                                if (lane3TargetsComplete == false && !StaticVariableManager.lane3PredelayActive)
                                {
                                    //Calculate Split time
                                    updateLane3SplitTime("hit", "c");
                                    updateLane3ResponseTime("hit","c");
                                    generateBulletImpact(hit);

                                    lane3ActiveTimeCounter = lane3TimeActiveCounter; //Assign Current Time

                                    point_lane_3 = updateLanePoint(point_lane_3);
                                    lane3PointsHit++;
                                    print("RE: " + lane3PointsHit);

                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane3PointsHit + (shotsFired - lane3PointsHit) >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            //lane3StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane3PointsHit >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            lane3StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane3ActiveTimeCounter = timeActiveCounter;
                                    }

                                    //StaticVariableManager.lane3PredelayActive = true;
                                }
                                break;
                        }

                        manageLaneTarinigStopCondition();
                    }
                    else
                    {
                        platesHit--;
                        if (platesHit <= 0)
                        {
                            platesHit = 0;
                        }
                    }

                }
                if(activeScene.ToLower().Contains("sequencenum") || activeScene.ToLower().Contains("colorsequence"))
                {
                    if (hit.transform.name.ToLower().Contains("plate"))
                    {
                        //Evaluate Current Lane
                        string tempTargetName = hit.transform.name;
                        char tempFirstChar = tempTargetName[0];
                        switch (tempFirstChar)
                        {
                            case '1':
                                if (lane1TargetsComplete == false && !StaticVariableManager.lane1PredelayActive)
                                {
                                    //Calculate Split time
                                    updateLane1SplitTime("hit", "c");
                                    updateLane1ResponseTime("hit", "c");
                                    generateBulletImpact(hit);

                                    lane1ActiveTimeCounter = lane1TimeActiveCounter; //Assign Current Time

                                    point_lane_1 = updateLanePoint(point_lane_1);
                                    lane1PointsHit++;
                                    //print("RE: " + lane1PointsHit);
                                    //print("RE: Targets imput is" + num_targets_input);


                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane1PointsHit + (shotsFired - lane1PointsHit) >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            //lane1StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane1PointsHit >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            lane1StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane1ActiveTimeCounter = timeActiveCounter;
                                    }

                                    //StaticVariableManager.lane1PredelayActive = true;
                                }
                                break;

                            case '2':
                                if (lane2TargetsComplete == false && !StaticVariableManager.lane2PredelayActive)
                                {
                                    //Calculate Split time
                                    updateLane2SplitTime("hit", "c");
                                    updateLane2ResponseTime("hit", "c");
                                    generateBulletImpact(hit);

                                    lane2ActiveTimeCounter = lane2TimeActiveCounter; //Assign Current Time

                                    point_lane_2 = updateLanePoint(point_lane_2);
                                    lane2PointsHit++;
                                    print("RE: " + lane2PointsHit);
                                    print("RE: Targets imput is" + num_targets_input);

                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane2PointsHit + (shotsFired - lane2PointsHit) >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            //lane2StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane2PointsHit >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            lane2StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane2ActiveTimeCounter = timeActiveCounter;
                                    }

                                    //StaticVariableManager.lane2PredelayActive = true;
                                }
                                break;

                            case '3':
                                if (lane3TargetsComplete == false && !StaticVariableManager.lane3PredelayActive)
                                {
                                    //Calculate Split time
                                    updateLane3SplitTime("hit", "c");
                                    updateLane3ResponseTime("hit", "c");
                                    generateBulletImpact(hit);

                                    lane3ActiveTimeCounter = lane3TimeActiveCounter; //Assign Current Time

                                    point_lane_3 = updateLanePoint(point_lane_3);
                                    lane3PointsHit++;
                                    print("RE: " + lane3PointsHit);

                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane3PointsHit + (shotsFired - lane3PointsHit) >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            //lane3StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane3PointsHit >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            lane3StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane3ActiveTimeCounter = timeActiveCounter;
                                    }

                                    //StaticVariableManager.lane3PredelayActive = true;
                                }
                                break;
                        }

                        manageLaneTarinigStopCondition();
                    }
                    else
                    {
                        platesHit--;
                        if (platesHit <= 0)
                        {
                            platesHit = 0;
                        }
                    }
                }
                if (activeScene.ToLower().Contains("hiddentarget"))
                {

                    //num_targets_input
                    //print("RE: HIT " + hit.transform.name);
                    if (hit.transform.name.ToLower().Contains("plate"))
                    {
                        //Evaluate Current Lane
                        string tempTargetName = hit.transform.name;
                        char tempFirstChar = tempTargetName[0];
                        switch (tempFirstChar)
                        {
                            case '1':
                                if (lane1TargetsComplete == false)
                                {
                                    //Calculate Split time
                                    updateLane1SplitTime("hit", "c");

                                    updateLane1ResponseTime("hit","c");

                                    lane1ActiveTimeCounter = lane1TimeActiveCounter; //Assign Current Time

                                    point_lane_1 = updateLanePoint(point_lane_1);
                                    lane1PointsHit++;
                                    //print("RE: lane 1 hit " + lane1PointsHit);
                                    //print("RE: Targets imput is" + num_targets_input);


                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane1PointsHit + (shotsFired - lane1PointsHit) >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            //lane1StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane1PointsHit >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            lane1StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane1ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;

                            case '2':
                                if (lane2TargetsComplete == false)
                                {
                                    //Calculate Split time
                                    updateLane2SplitTime("hit", "c");

                                    updateLane2ResponseTime("hit","c");

                                    lane2ActiveTimeCounter = lane2TimeActiveCounter; //Assign Current Time

                                    point_lane_2 = updateLanePoint(point_lane_2);
                                    lane2PointsHit++;
                                    print("RE: " + lane2PointsHit);
                                    print("RE: Targets imput is" + num_targets_input);

                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane2PointsHit + (shotsFired - lane2PointsHit) >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            //lane2StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane2PointsHit >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            lane2StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane2ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;

                            case '3':
                                if (lane3TargetsComplete == false)
                                {
                                    //Calculate Split time
                                    updateLane3SplitTime("hit", "c");

                                    updateLane3ResponseTime("hit","c");

                                    lane3ActiveTimeCounter = lane3TimeActiveCounter; //Assign Current Time

                                    point_lane_3 = updateLanePoint(point_lane_3);
                                    lane3PointsHit++;
                                    print("RE: " + lane3PointsHit);

                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        /*if (lane3PointsHit + (shotsFired - lane3PointsHit) >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            //lane3StopSignal.SetActive(true);
                                        }*/
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane3PointsHit >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            lane3StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane3ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;
                        }

                        manageLaneTarinigStopCondition();
                    }
                    else
                    {
                        platesHit--;
                        if (platesHit <= 0)
                        {
                            platesHit = 0;
                        }
                    }

                }
                if (activeScene.ToLower().Contains("baloon"))
                {
                    //num_targets_input
                    if (hit.transform.name.ToLower().Contains("baloon"))
                    {
                        baloonPopSound.Play();
                        //Evaluate Current Lane
                        string tempTargetName = hit.transform.name;
                        char tempFirstChar = tempTargetName[0];
                        switch (tempFirstChar)
                        {
                            case '1':
                                if (lane1TargetsComplete == false)
                                {
                                    //Calculate Split time
                                    updateLane1SplitTime("hit", "c");
                                    updateLane1ResponseTime("hit", "c");

                                    lane1ActiveTimeCounter = lane1TimeActiveCounter; //Assign Current Time

                                    //point_lane_1 = updateLanePoint(point_lane_1);
                                    lane1PointsHit++;

                                    if (hit.transform.name.ToLower().Contains(StaticVariableManager.currentLane1Color.ToLower()))
                                    {
                                        StaticVariableManager.correctColorHits++;
                                    }
                                    else
                                    {
                                        StaticVariableManager.wrongColorHits++;
                                    }


                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane1PointsHit + (shotsFired - lane1PointsHit) >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            //lane1StopSignal.SetActive(true);
                                        }
                                    }
                                    else
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane1PointsHit >= num_targets_input)
                                        {
                                            lane1TargetsComplete = true;
                                            lane1ActiveTimeCounter = timeActiveCounter;
                                            lane1StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane1ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;

                            case '2':
                                if (lane2TargetsComplete == false)
                                {
                                    print("RE: target name is " + hit.transform.name);
                                    print("RE: current target name is " + StaticVariableManager.currentLane2Color);

                                    //Calculate Split time
                                    updateLane2SplitTime("hit", "c");
                                    updateLane2ResponseTime("hit", "c");
                                    lane2ActiveTimeCounter = lane2TimeActiveCounter;//Assign Current Time

                                    //point_lane_2 = updateLanePoint(point_lane_2);
                                    lane2PointsHit++;

                                    if (hit.transform.name.ToLower().Contains(StaticVariableManager.currentLane2Color.ToLower()))
                                    {
                                        StaticVariableManager.correctColorHits++;
                                    }
                                    else
                                    {
                                        StaticVariableManager.wrongColorHits++;
                                    }

                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane2PointsHit + (shotsFired - lane2PointsHit) >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            lane2StopSignal.SetActive(true);
                                        }
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane2PointsHit >= num_targets_input)
                                        {
                                            lane2TargetsComplete = true;
                                            lane2ActiveTimeCounter = timeActiveCounter;
                                            lane2StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane2ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;

                            case '3':
                                if (lane3TargetsComplete == false)
                                {
                                   if (hit.transform.name.ToLower().Contains(StaticVariableManager.currentLane1Color))
                                    {
                                        //Calculate Split time
                                        updateLane3SplitTime("hit", "c");
                                        updateLane3ResponseTime("hit","c");

                                        lane3ActiveTimeCounter = lane3TimeActiveCounter; ; //Assign Current Time

                                        //point_lane_3 = updateLanePoint(point_lane_3);
                                        lane3PointsHit++;
                                    }
                                    else
                                    {
                                        numLane3ShotsMissed++;
                                        /*if (!lane3SrikeOut)
                                        {
                                            lane3_strike_count++;
                                            if (Scoring.ammo_setting.ToLower().Contains("laser"))
                                            {
                                                lane1StrikesArray[lane3_strike_count - 1].SetActive(true);
                                                if (lane3_strike_count == TestConditionsManager.numStrikes)
                                                {
                                                    lane3SrikeOut = true;
                                                }
                                            }
                                            
                                        }*/
                                    }


                                    if (Scoring.ammo_setting == "Live")
                                    {
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane3PointsHit + (shotsFired - lane3PointsHit) >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            //lane3StopSignal.SetActive(true);
                                        }
                                    }
                                    else
                                    {
                                        //lane1ActiveTimeCounter = timeActiveCounter;
                                        //targetText.text = "Hit Plate" + platesHit;
                                        if (lane3PointsHit >= num_targets_input)
                                        {
                                            lane3TargetsComplete = true;
                                            lane3ActiveTimeCounter = timeActiveCounter;
                                            lane3StopSignal.SetActive(true);
                                        }
                                    }

                                    if (targetFinished && targetsFinished)
                                    {
                                        lane3ActiveTimeCounter = timeActiveCounter;
                                    }
                                }
                                break;
                        }

                        manageLaneTarinigStopCondition();
                    }
                    else
                    {
                        platesHit--;
                        if (platesHit <= 0)
                        {
                            platesHit = 0;
                        }
                    }

                }
                if(activeScene.ToLower().Contains("suspectshoot"))
                {
                    if (hit.transform.name.ToLower().Contains("board"))
                    {
                        //Evaluate Current Lane
                        string tempTargetName = hit.transform.name;
                        char tempFirstChar = tempTargetName[0];
                        switch (tempFirstChar)
                        {
                            case '1':
                                if (lane1TargetsComplete == false)
                                {
                                    point_lane_1 = updateLanePoint(point_lane_1);
                                    lane1PointsHit++;
                                    StaticVariableManager.isLane1BoardSet = false;
                                    handleLane1(hit);
                                    startLane1Count = true;
                                    if (lane1PointsHit >= StaticVariableManager.totNumLane1Threats)
                                    {
                                        //lane1TargetsComplete = true;
                                    }

                                    //Calculate Response time
                                    updateLane1ResponseTime("hit","c");
                                }
                                break;

                            case '2':
                                if (lane2TargetsComplete == false)
                                {
                                    point_lane_2 = updateLanePoint(point_lane_2);
                                    lane2PointsHit++;
                                    startLane2Count = true;
                                    StaticVariableManager.isLane2BoardSet = false;
                                    handleLane2(hit);

                                    if (lane2PointsHit >= StaticVariableManager.totNumLane1Threats)
                                    {
                                        //lane2TargetsComplete = true;
                                    }

                                    //Calculate Response time
                                    updateLane2ResponseTime("hit","c");
                                }
                                break;
                        }
                        //generateBulletImpact(hit);
                        showSuspectResult(hit);
                        manageLaneTarinigStopCondition();
                    }
                    else
                    {
                        platesHit--;
                        if (platesHit <= 0)
                        {
                            platesHit = 0;
                        }
                    }
                }
                if (activeScene.ToLower().Contains("block"))
                {
                    if (hit.transform.name.ToLower().Contains("boxtarget"))
                    {
                        
                        //Evaluate Current Lane
                        string tempTargetName = hit.transform.name;
                        char tempFirstChar = tempTargetName[0];
                        switch (tempFirstChar)
                        {
                            case '1':
                                if (lane1TargetsComplete == false)
                                {
                                    point_lane_1 = updateLanePoint(point_lane_1);
                                    lane1PointsHit++;
                                    StaticVariableManager.isLane1BoardSet = false;
                                    handleLane1(hit);
                                    startLane1Count = true;
                                    if (lane1PointsHit >= StaticVariableManager.totNumLane1Threats)
                                    {
                                        //lane1TargetsComplete = true;
                                    }
                                    lane1ActiveTimeCounter = lane1TimeActiveCounter; //Assign Current Time

                                    updateLane1SplitTime("hit", "c");
                                    //Calculate Response time
                                    updateLane1ResponseTime("hit","c");
                                }
                                break;

                            case '2':
                                if (lane2TargetsComplete == false)
                                {
                                    point_lane_2 = updateLanePoint(point_lane_2);
                                    lane2PointsHit++;
                                    startLane2Count = true;
                                    StaticVariableManager.isLane2BoardSet = false;
                                    handleLane2(hit);

                                    if (lane2PointsHit >= StaticVariableManager.totNumLane1Threats)
                                    {
                                        //lane2TargetsComplete = true;
                                    }
                                    lane2ActiveTimeCounter = lane2TimeActiveCounter; //Assign Current Time

                                    updateLane2SplitTime("hit", "c");
                                    //Calculate Response time
                                    updateLane2ResponseTime("hit","c");
                                }
                                break;
                        }
                        //generateBulletImpact(hit);
                        showSuspectResult(hit);
                        manageLaneTarinigStopCondition();
                    }
                    else
                    {
                        platesHit--;
                        if (platesHit <= 0)
                        {
                            platesHit = 0;
                        }
                    }
                }

            }
        }
        else if(hit.transform.tag == "deer" || hit.transform.tag == "animal_head")
        {
            if(hit.transform.tag == "deer")
            {
                hit.transform.gameObject.GetComponent<AnimalController>().SendMessage("ApplyDamage", hit.transform.tag);
            }
            else if(hit.transform.tag == "animal_head")
            {
                hit.transform.gameObject.GetComponent<AnimalHeadReference>().SendMessage("ApplyDamage", hit.transform.GetComponent<AnimalHeadReference>().mainBody.transform.name);
            }

        }

        //Target Popup SHooting
        else if (hit.transform.tag.ToLower().Contains("head"))
        {
            if (hit.transform.name.ToLower().Contains("target") || hit.transform.name.ToLower().Contains("cover"))
            {

                //Evaluate Current Lane
                string tempTargetName = hit.transform.name;
                char tempFirstChar = tempTargetName[0];
                bullseye_hit = true;
                PlayIPECHitSound(hit);

                switch (tempFirstChar)
                {
                    case '1':                   
                        if (lane1TargetsComplete == false)
                        {
                            //Calculate Split time
                            updateLane1SplitTime("hit", "c");

                            updateLane1ResponseTime("hit", "c");
                            generateBulletImpact(hit);

                            lane1ActiveTimeCounter = lane1TimeActiveCounter; //Assign Current Time

                            point_lane_1 = updateLanePoint(point_lane_1);
                            lane1PointsHit++;
                            //print("RRE: " + lane1PointsHit);
                            //print("RE: Targets imput is" + num_targets_input);


                            if (Scoring.ammo_setting == "Live")
                            {
                                //targetText.text = "Hit Plate" + platesHit;
                                /*if (lane1PointsHit + (shotsFired - lane1PointsHit) >= num_targets_input)
                                {
                                    lane1TargetsComplete = true;
                                    lane1ActiveTimeCounter = timeActiveCounter;
                                    //lane1StopSignal.SetActive(true);
                                }*/
                            }
                            else
                            {
                                //targetText.text = "Hit Plate" + platesHit;
                                if (lane1PointsHit >= num_targets_input)
                                {
                                    lane1TargetsComplete = true;
                                    lane1ActiveTimeCounter = timeActiveCounter;
                                    lane1StopSignal.SetActive(true);
                                }
                            }

                            if (targetFinished && targetsFinished)
                            {
                                lane1ActiveTimeCounter = timeActiveCounter;
                            }
                        }
                        break;

                    case '2':
                        if (lane2TargetsComplete == false)
                        {
                            //Calculate Split time
                            updateLane2SplitTime("hit", "c");

                            updateLane2ResponseTime("hit", "c");
                            generateBulletImpact(hit);

                            lane2ActiveTimeCounter = lane2TimeActiveCounter; //Assign Current Time

                            point_lane_2 = updateLanePoint(point_lane_2);
                            lane2PointsHit++;
                            print("RE: " + lane2PointsHit);
                            print("RE: Targets imput is" + num_targets_input);

                            if (Scoring.ammo_setting == "Live")
                            {
                                //targetText.text = "Hit Plate" + platesHit;
                                /*if (lane2PointsHit + (shotsFired - lane2PointsHit) >= num_targets_input)
                                {
                                    lane2TargetsComplete = true;
                                    lane2ActiveTimeCounter = timeActiveCounter;
                                    //lane2StopSignal.SetActive(true);
                                }*/
                            }
                            else
                            {
                                //lane1ActiveTimeCounter = timeActiveCounter;
                                //targetText.text = "Hit Plate" + platesHit;
                                if (lane2PointsHit >= num_targets_input)
                                {
                                    lane2TargetsComplete = true;
                                    lane2ActiveTimeCounter = timeActiveCounter;
                                    lane2StopSignal.SetActive(true);
                                }
                            }

                            if (targetFinished && targetsFinished)
                            {
                                lane2ActiveTimeCounter = timeActiveCounter;
                            }
                        }
                        break;

                    case '3':
                        if (lane3TargetsComplete == false)
                        {
                            //Calculate Split time
                            updateLane3SplitTime("hit", "c");

                            updateLane3ResponseTime("hit", "c");
                            generateBulletImpact(hit);

                            lane3ActiveTimeCounter = lane3TimeActiveCounter; //Assign Current Time

                            point_lane_3 = updateLanePoint(point_lane_3);
                            lane3PointsHit++;
                            print("RE: " + lane3PointsHit);

                            if (Scoring.ammo_setting == "Live")
                            {
                                //targetText.text = "Hit Plate" + platesHit;
                                /*if (lane3PointsHit + (shotsFired - lane3PointsHit) >= num_targets_input)
                                {
                                    lane3TargetsComplete = true;
                                    lane3ActiveTimeCounter = timeActiveCounter;
                                    //lane3StopSignal.SetActive(true);
                                }*/
                            }
                            else
                            {
                                //lane1ActiveTimeCounter = timeActiveCounter;
                                //targetText.text = "Hit Plate" + platesHit;
                                if (lane3PointsHit >= num_targets_input)
                                {
                                    lane3TargetsComplete = true;
                                    lane3ActiveTimeCounter = timeActiveCounter;
                                    lane3StopSignal.SetActive(true);
                                }
                            }

                            if (targetFinished && targetsFinished)
                            {
                                lane3ActiveTimeCounter = timeActiveCounter;
                            }
                        }
                        break;
                }

                manageLaneTarinigStopCondition();
            }
            else
            {
                //platesHit--;
                if (platesHit <= 0)
                {
                    platesHit = 0;
                }
            }

            if (targetsFinished == false && targetFinished ==false)
            {
                Component enemy = new Component();
                //hit.rigidbody.AddForce(-hit.normal * 2F);
                hit.transform.gameObject.GetComponent<HeadTarget>().SendMessage("ApplyDamage", hit.transform.tag);
                //print("Object hit is:" + hit.transform.tag);

                if (activeScene.ToLower().Contains("targetpopupfreeshoot") || activeScene == "BasicTargetPopUpOneHand" || activeScene.ToLower().Contains("threatening") || activeScene.ToLower().Contains("pointman") || activeScene.ToLower().Contains("pointbullseye"))
                {
                    WallTargetControl.requestMove();
                }

                headShots++;
                if(activeScene.ToLower().Contains("pointbullseye"))
                {
                    HeadShots.text = ("Side Target: " + headShots.ToString());
                }
                else
                {
                    HeadShots.text = ("Head Shots: " + headShots.ToString());
                }

                if (Scoring.simulation_type == "training")
                {
                    if ((headShots + bodyShots) + (shotsFired - (headShots + bodyShots)) >= num_targets_input)
                    {
                        GameOver = true;
                        StopTraining();
                    }
                }
                else
                {
                    if ((headShots + bodyShots) >= num_targets_input)
                    {
                        GameOver = true;
                        StopTraining();
                    }
                }

                changeColor(hit);

                //targetControl.HeadShots.text = ("Head Shots: " + headShots.ToString());
                //enemyshot++;
            }
        }
        else if (hit.transform.tag.ToLower().Contains("body"))
        {

            
            if (hit.transform.name.ToLower().Contains("target") || hit.transform.name.ToLower().Contains("cover"))
            {
                //print("Test: Point Reached");
                //Evaluate Current Lane
                generateBulletImpact(hit);
                string tempTargetName = hit.transform.name;
                char tempFirstChar = tempTargetName[0];
                bullseye_hit = true;
                PlayIPECHitSound(hit);

                switch (tempFirstChar)
                {
                    case '1':
                        if (lane1TargetsComplete == false)
                        {
                            //Calculate Split time
                            updateLane1SplitTime("hit", "c");

                            updateLane1ResponseTime("hit", "c");

                            lane1ActiveTimeCounter = lane1TimeActiveCounter; //Assign Current Time

                            point_lane_1 = updateLanePoint(point_lane_1);
                            lane1PointsHit++;
                            //print("RRE: " + lane1PointsHit);
                            //print("RE: Targets imput is" + num_targets_input);


                            if (Scoring.ammo_setting == "Live")
                            {
                                //targetText.text = "Hit Plate" + platesHit;
                                /*if (lane1PointsHit + (shotsFired - lane1PointsHit) >= num_targets_input)
                                {
                                    lane1TargetsComplete = true;
                                    lane1ActiveTimeCounter = timeActiveCounter;
                                    //lane1StopSignal.SetActive(true);
                                }*/
                            }
                            else
                            {
                                //targetText.text = "Hit Plate" + platesHit;
                                if (lane1PointsHit >= num_targets_input)
                                {
                                    lane1TargetsComplete = true;
                                    lane1ActiveTimeCounter = timeActiveCounter;
                                    lane1StopSignal.SetActive(true);
                                }
                            }

                            if (targetFinished && targetsFinished)
                            {
                                lane1ActiveTimeCounter = timeActiveCounter;
                            }
                        }
                        break;

                    case '2':
                        if (lane2TargetsComplete == false)
                        {
                            //Calculate Split time
                            updateLane2SplitTime("hit", "c");

                            updateLane2ResponseTime("hit", "c");

                            lane2ActiveTimeCounter = lane2TimeActiveCounter; //Assign Current Time

                            point_lane_2 = updateLanePoint(point_lane_2);
                            lane2PointsHit++;
                            print("RE: " + lane2PointsHit);
                            print("RE: Targets imput is" + num_targets_input);

                            if (Scoring.ammo_setting == "Live")
                            {
                                //targetText.text = "Hit Plate" + platesHit;
                                /*if (lane2PointsHit + (shotsFired - lane2PointsHit) >= num_targets_input)
                                {
                                    lane2TargetsComplete = true;
                                    lane2ActiveTimeCounter = timeActiveCounter;
                                    //lane2StopSignal.SetActive(true);
                                }*/
                            }
                            else
                            {
                                //lane1ActiveTimeCounter = timeActiveCounter;
                                //targetText.text = "Hit Plate" + platesHit;
                                if (lane2PointsHit >= num_targets_input)
                                {
                                    lane2TargetsComplete = true;
                                    lane2ActiveTimeCounter = timeActiveCounter;
                                    lane2StopSignal.SetActive(true);
                                }
                            }

                            if (targetFinished && targetsFinished)
                            {
                                lane2ActiveTimeCounter = timeActiveCounter;
                            }
                        }
                        break;

                    case '3':
                        if (lane3TargetsComplete == false)
                        {
                            //Calculate Split time
                            updateLane1SplitTime("hit", "c");

                            updateLane3ResponseTime("hit", "c");

                            lane3ActiveTimeCounter = lane3TimeActiveCounter; //Assign Current Time

                            point_lane_3 = updateLanePoint(point_lane_3);
                            lane3PointsHit++;
                            print("RE: " + lane3PointsHit);

                            if (Scoring.ammo_setting == "Live")
                            {
                                //targetText.text = "Hit Plate" + platesHit;
                                /*if (lane3PointsHit + (shotsFired - lane3PointsHit) >= num_targets_input)
                                {
                                    lane3TargetsComplete = true;
                                    lane3ActiveTimeCounter = timeActiveCounter;
                                    //lane3StopSignal.SetActive(true);
                                }*/
                            }
                            else
                            {
                                //lane1ActiveTimeCounter = timeActiveCounter;
                                //targetText.text = "Hit Plate" + platesHit;
                                if (lane3PointsHit >= num_targets_input)
                                {
                                    lane3TargetsComplete = true;
                                    lane3ActiveTimeCounter = timeActiveCounter;
                                    lane3StopSignal.SetActive(true);
                                }
                            }

                            if (targetFinished && targetsFinished)
                            {
                                lane3ActiveTimeCounter = timeActiveCounter;
                            }
                        }
                        break;
                }

                manageLaneTarinigStopCondition();
            }
            else
            {
                //platesHit--;
                if (platesHit <= 0)
                {
                    platesHit = 0;
                }
            }
            if (targetsFinished == false && targetFinished == false)
            {
                Component enemy = new Component();
                //hit.rigidbody.AddForce(-hit.normal * 2F);
                hit.transform.gameObject.GetComponent<BodyTarget>().SendMessage("ApplyDamage", hit.transform.tag);
                //print("Object hit is:" + hit.transform.tag);

                if (activeScene.ToLower().Contains("targetpopupfreeshoot") || activeScene == "BasicTargetPopUpOneHand" || activeScene.ToLower().Contains("threatening") || activeScene.ToLower().Contains("pointman") || activeScene.ToLower().Contains("pointbullseye"))
                {
                    WallTargetControl.requestMove();
                }

                bodyShots++;

                if (activeScene.ToLower().Contains("pointbullseye"))
                {
                    BodyShots.text = ("Center Target: " + bodyShots.ToString());
                }
                else
                {
                    BodyShots.text = ("Body Shots: " + bodyShots.ToString());
                }

                if (Scoring.simulation_type == "training")
                {
                    if ((headShots + bodyShots) + (shotsFired - (headShots + bodyShots)) >= num_targets_input)
                    {
                        GameOver = true;
                        StopTraining();
                    }
                }
                else
                {
                    if ((headShots + bodyShots) >= num_targets_input)
                    {
                        GameOver = true;
                        StopTraining();
                    }
                }

               
                changeColor(hit);
                //targetControl.BodyShots.text = ("Body Shots: " + bodyShots.ToString());
                if (activeScene.ToLower().Contains("block"))
                {
                    //GameObject impactGo = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    //Destroy(impactGo, 0.5f);
                }

            }
        }

        manageLaneShotsMissed(hit);
        assignShotsFiredData();
    }

    private void PlayIPECHitSound(RaycastHit hit)
    {
        if(activeScene.ToLower().Contains("pointbullseye"))
        {
            if(hit.transform.name.ToLower().Contains("cover.target"))
            {
                platHitSound.Play();
            }
        }
        else
        {
            platHitSound.Play();
        }
    }

    private void sendHitSignal(RaycastHit hit)
    {

        if (activeScene.ToLower().Contains("ipec"))
        {
            if (TargetMessageReceive.body_head_ishit == false && !targetsFinished && !ammaFinished)
            {
                hit.transform.gameObject.GetComponent<TargetMessageReceive>().SendMessage("ApplyDamage", hit.transform.name);
            }
        }
        else if (!activeScene.ToLower().Contains("ipec") && platesHit < num_targets_input)
        {
            //print("Applying Damage");
            hit.transform.gameObject.GetComponent<TargetMessageReceive>().SendMessage("ApplyDamage", hit.transform.name); //
        }
    }

    private void updateLane1ResponseTime(string label, string response_type)
    {
        label = updateLabel(label);
        switch(response_type)
        {
            case "c":
                if (startLane1ResponseCount)
                {
                    numLane1ResponseTimes++;
                    lane1ResponseTime.Add((lane1ResponseTimeCounter).ToString("0.00") + " sec" + ":" + label);

                    lane1ResponseTimeCounter = 0;
                }
                break;

            case "d":
                if (startLane1ResponseCount)
                {
                    numLane1ResponseTimes++;
                    lane1ResponseTime.Add((lane1ResponseTimeCounter).ToString("0.00") + " sec" + ":" + label);

                    lane1ResponseTimeCounter = 0;
                    startLane1ResponseCount = false;
                }
                break;
        }
    }

    private void updateLane2ResponseTime(string label, string response_type)
    {
        label = updateLabel(label);

        switch (response_type)
        {
            case "c":
                if (startLane2ResponseCount)
                {
                    numLane2ResponseTimes++;
                    lane2ResponseTime.Add((lane2ResponseTimeCounter).ToString("0.00") + " sec" + ":" + label);

                    lane2ResponseTimeCounter = 0;
                }
                break;

            case "d":
                if (startLane2ResponseCount)
                {
                    numLane2ResponseTimes++;
                    lane2ResponseTime.Add((lane2ResponseTimeCounter).ToString("0.00") + " sec" + ":" + label);

                    lane2ResponseTimeCounter = 0;
                    startLane2ResponseCount = false;
                }
                break;
        }
     
    }

    private void updateLane3ResponseTime(string label, string response_type)
    {
        label = updateLabel(label);

        switch (response_type)
        {
            case "c":
                if (startLane3ResponseCount)
                {
                    numLane3ResponseTimes++;
                    lane3ResponseTime.Add((lane3ResponseTimeCounter).ToString("0.00") + " sec" + ":" + label);

                    lane3ResponseTimeCounter = 0;
                }
                break;

            case "d":
                if (startLane3ResponseCount)
                {
                    numLane3ResponseTimes++;
                    lane3ResponseTime.Add((lane3ResponseTimeCounter).ToString("0.00") + " sec" + ":" + label);

                    lane3ResponseTimeCounter = 0;
                    startLane3ResponseCount = false;
                }
                break;
        }

        
    }

    private void updateLane1SplitTime(string label, string response_type)
    {
        label = updateLabel(label);

        if (startLane1Count)
        {
            numLane1Splits++;
            lane1SplitTime.Add((lane1SplitTimeCounter).ToString("0.00") + " sec");

            lane1SplitTimeCounter = 0;
        }
        else
        {
            startLane1Count = true;
        }
    }

    private void updateLane2SplitTime(string label, string response_type)
    {
        label = updateLabel(label);

        if (startLane2Count)
        {
            numLane2Splits++;
            lane2SplitTime.Add((lane2SplitTimeCounter).ToString("0.00") + " sec");

            lane2SplitTimeCounter = 0;
        }
        else
        {
            startLane2Count = true;
        }

    }

    private void updateLane3SplitTime(string label, string response_type)
    {
        label = updateLabel(label);

        if (startLane3Count)
        {
            numLane3Splits++;
            lane3SplitTime.Add((lane3SplitTimeCounter).ToString("0.00") + " sec");

            lane3SplitTimeCounter = 0;
        }
        else
        {
            startLane3Count = true;
        }

    }

    private string updateLabel(string label)
    {
        string update_label = "";
        switch (label)
        {
            case "hit":
                update_label = "H";
                break;
            case "miss":
                update_label = "M";
                break;
        }

        return update_label;
    }

    private void showSuspectResult(RaycastHit hit)
    {
        if(activeScene.ToLower().Contains("1lane"))
        {
            if ((lane1TargetsComplete) || (StaticVariableManager.isLane1TargetMissed))
            {
                //trainingPaused = true;
                handleLane1(hit);
            }
        }
        if (activeScene.ToLower().Contains("2lane"))
        {
            if ((lane1TargetsComplete || StaticVariableManager.isLane1TargetMissed) && lane1Scored == false)
            {
                handleLane1(hit);
                //lane1Scored = true;
            }
            if ((lane2TargetsComplete || StaticVariableManager.isLane2TargetMissed) && lane2Scored == false)
            {
                handleLane2(hit);
                //lane2Scored = true;
            }

            if ((lane1TargetsComplete && lane2TargetsComplete) || (StaticVariableManager.isLane1TargetMissed && StaticVariableManager.isLane2TargetMissed))
            {
                //trainingPaused = true;
            }
        }

        
    }

    private void handleLane1(RaycastHit hit)
    {
        if (hit.transform.tag.ToLower().Contains("nonthreat"))
        {
            //lane1Result.text = "Fail \n" + "Non threath shot.";
            //lane1ResultPanel.SetActive(true);
            //trainingPaused = true;
            lane1NonThreatsShot++;
            if (startLane1ResponseCount)
            {
                numLane1ResponseTimes++;
                if (lane1ResponseTimeCounter < 1)
                {
                    lane1ResponseTime.Add(lane1ResponseTimeCounter.ToString("0.00") + "ms");
                }
                else
                {
                    lane1ResponseTime.Add(lane1ResponseTimeCounter.ToString("0.00") + "sec");
                }

                lane1ResponseTimeCounter = 0;
                startLane1ResponseCount = false;
            }
        }
        else if (hit.transform.tag.ToLower().Contains("threat"))
        {
            /*switch (lane1PointsHit)
            {
                case 1:
                    lane1Result.text = "Pass \n" + lane1PointsHit + " Threat shot.";
                    break;
                case 2:
                    lane1Result.text = "Pass \n" + lane1PointsHit + " Threats shot.";
                    break;
                case 3:
                    lane1Result.text = "Pass \n" + lane1PointsHit + " Threats shot.";
                    break;
                case 4:
                    lane1Result.text = "Pass \n" + lane1PointsHit + " Threats shot.";
                    break;
            }*/
            //lane1ResultPanel.SetActive(true);
            //trainingPaused = true;
            lane1ThreatsShot++;
            if (startLane1ResponseCount)
            {
                numLane1ResponseTimes++;
                if (lane1ResponseTimeCounter < 1)
                {
                    lane1ResponseTime.Add(lane1ResponseTimeCounter.ToString("0.00") + "ms");
                }
                else
                {
                    lane1ResponseTime.Add(lane1ResponseTimeCounter.ToString("0.00") + "sec");
                }

                lane1ResponseTimeCounter = 0;
                startLane1ResponseCount = false;
            }
        }
        else
        {
            //lane1Result.text = "Fail \n" + "shot missed";
            //lane1ResultPanel.SetActive(true);
            //trainingPaused = true;
            if (startLane1ResponseCount)
            {
                numLane1ResponseTimes++;
                if (lane1ResponseTimeCounter < 1)
                {
                    lane1ResponseTime.Add("0.0");
                }
                else
                {
                    lane1ResponseTime.Add("0.0");
                }

                lane1ResponseTimeCounter = 0;
            }
        }
        
    }
    private void handleLane2(RaycastHit hit)
    {
        if (hit.transform.tag.ToLower().Contains("nonthreat"))
        {
            //lane2Result.text = "Fail \n" + "Non threath shot.";
            //lane2ResultPanel.SetActive(true);
            //trainingPaused = true;
            lane2NonThreatsShot++;
            if (startLane2ResponseCount)
            {
                numLane2ResponseTimes++;
                if (lane2ResponseTimeCounter < 1)
                {
                    lane2ResponseTime.Add(lane2ResponseTimeCounter.ToString("0.00") + "ms");
                }
                else
                {
                    lane2ResponseTime.Add(lane2ResponseTimeCounter.ToString("0.00") + "sec");
                }

                lane2ResponseTimeCounter = 0;
                startLane2ResponseCount = false;
            }
        }
        else if (hit.transform.tag.ToLower().Contains("threat"))
        {
            /*switch (lane2PointsHit)
            {
                case 1:
                    lane2Result.text = "Pass \n" + lane2PointsHit + " Threat shot.";
                    break;
                case 2:
                    lane2Result.text = "Pass \n" + lane2PointsHit + " Threats shot.";
                    break;
                case 3:
                    lane2Result.text = "Pass \n" + lane2PointsHit + " Threats shot.";
                    break;
                case 4:
                    lane2Result.text = "Pass \n" + lane2PointsHit + " Threats shot.";
                    break;
            }*/
            //lane2ResultPanel.SetActive(true);
            //trainingPaused = true;
            lane2ThreatsShot++;
            if (startLane2ResponseCount)
            {
                numLane2ResponseTimes++;
                if (lane2ResponseTimeCounter < 1)
                {
                    lane2ResponseTime.Add(lane2ResponseTimeCounter.ToString("0.00") + "ms");
                }
                else
                {
                    lane2ResponseTime.Add(lane2ResponseTimeCounter.ToString("0.00") + "sec");
                }

                lane2ResponseTimeCounter = 0;
                startLane2ResponseCount = false;
            }
        }
        else
        {
            //lane2Result.text = "Fail \n" + "shot missed";
            //lane2ResultPanel.SetActive(true);
            //trainingPaused = true;
            if (startLane2ResponseCount)
            {
                numLane2ResponseTimes++;
                if (lane2ResponseTimeCounter < 1)
                {
                    lane2ResponseTime.Add("0.0");
                }
                else
                {
                    lane2ResponseTime.Add("0.0");
                }

                lane2ResponseTimeCounter = 0;
            }
        }
    }

    private void manageLaneShotsMissed(RaycastHit hit)
    {
        if(hit.transform.tag.ToLower().Contains("misscollider"))
        {

            generateBulletImpact2(hit);
            switch(hit.transform.tag)
            {
                case "Lane1MissCollider":
                    numLane1ShotsMissed++;
                    updateLane1ResponseTime("miss","c");
                    updateLane1SplitTime("miss", "c");
                    //StaticVariableManager.isLane1TargetMissed = true;
                    //print("RE: Lane 1 Shots missed is : " + numLane1ShotsMissed);
                    break;
                case "Lane2MissCollider":
                    numLane2ShotsMissed++;
                    updateLane2ResponseTime("miss", "c");
                    updateLane2SplitTime("miss", "c");
                    //StaticVariableManager.isLane2TargetMissed = true;
                    //print("RE: Lane 2 Shots missed is : " + numLane2ShotsMissed);
                    break;
                case "Lane3MissCollider":
                    numLane3ShotsMissed++;
                    updateLane3ResponseTime("miss", "c");
                    updateLane3SplitTime("miss", "c");
                    //StaticVariableManager.isLane3TargetMissed = true;
                    //print("RE: Lane 3 Shots missed is : " + numLane3ShotsMissed);
                    break;
            }

            if (activeScene.ToLower().Contains("suspectshoot"))
            {
                if (StaticVariableManager.isLane1TargetMissed || StaticVariableManager.isLane2TargetMissed || StaticVariableManager.isLane3TargetMissed)
                {
                    showSuspectResult(hit);
                }
            }
        }
        else if (hit.transform.name.ToLower().Contains("body.") || hit.transform.name.ToLower().Contains("head."))
        {
            if(hit.transform.name.Contains("1"))
            {
                numLane1ShotsMissed++;
                updateLane1ResponseTime("miss", "c");
                updateLane1SplitTime("miss", "c");
            }
            else if(hit.transform.name.Contains("2"))
            {
                numLane2ShotsMissed++;
                updateLane2ResponseTime("miss", "c");
                updateLane2SplitTime("miss", "c");
            }
            else if (hit.transform.name.Contains("3"))
            {
                numLane3ShotsMissed++;
                updateLane3ResponseTime("miss", "c");
                updateLane3SplitTime("miss", "c");
            }

            if (activeScene.ToLower().Contains("suspectshoot"))
            {
                if (StaticVariableManager.isLane1TargetMissed || StaticVariableManager.isLane2TargetMissed || StaticVariableManager.isLane3TargetMissed)
                {
                    showSuspectResult(hit);
                }
            }
        }
        else if (hit.transform.tag.ToLower().Contains("hostage"))
        {
            if (hit.transform.name.Contains("1"))
            {
                numLane1ShotsMissed++;
                updateLane1ResponseTime("miss", "c");
                updateLane1SplitTime("miss", "c");
                generateBulletImpact2(hit);
            }
            else if (hit.transform.name.Contains("2"))
            {
                numLane2ShotsMissed++;
                updateLane2ResponseTime("miss", "c");
                updateLane2SplitTime("miss", "c");
                generateBulletImpact2(hit);
            }
            else if (hit.transform.name.Contains("3"))
            {
                numLane3ShotsMissed++;
                updateLane3ResponseTime("miss", "c");
                updateLane3SplitTime("miss", "c");
                generateBulletImpact2(hit);
            }
            WallTargetControl.requestMove();
        }
        else if (activeScene.ToLower().Contains("shell"))
        {
            if (!hit.transform.name.Contains("holder"))
            {
                numLane1ShotsMissed++;
                updateLane1ResponseTime("miss", "c");
                updateLane1SplitTime("miss", "c");
            }
        }
        else if (activeScene.ToLower().Contains("5pointbullseye"))
        {
            if (!bullseye_hit)
            {
                numLane1ShotsMissed++;
                updateLane1ResponseTime("miss", "c");
                updateLane1SplitTime("miss", "c");
                generateBulletImpact2(hit);
            }
            bullseye_hit = false;
        }
    }


    private void generateBulletImpact(RaycastHit hit)
    {
        if (Scoring.ammo_setting == "Live" || Scoring.ammo_setting.ToLower().Contains("laser"))
        {
            GameObject impactGo = Instantiate(bullethole_2, hit.point, Quaternion.LookRotation(hit.normal));
            impactGo.transform.name = "Hit";

            switch(hit.transform.name[0])
            {
                case '1':
                    lane_1_impact_list.Add(impactGo);
                    impactGo.GetComponent<BulletManager>().bullet_text.text = (timeActiveCounter).ToString("0.00") + " sec";
                    //print("RRE: adding for lane 1");
                    HandleTargetHit(hit, impactGo);
                    break;
                case '2':
                    lane_2_impact_list.Add(impactGo);
                    impactGo.GetComponent<BulletManager>().bullet_text.text = (timeActiveCounter).ToString("0.00") + " sec";
                    //print("RRE: adding for lane 2");
                    HandleTargetHit(hit, impactGo);
                    break;
                case '3':
                    lane_3_impact_list.Add(impactGo);
                    impactGo.GetComponent<BulletManager>().bullet_text.text = (timeActiveCounter).ToString("0.00") + " sec";
                    //print("RRE: adding for lane 3");
                    HandleTargetHit(hit, impactGo);
                    break;
            }
            //Destroy(impactGo, 1.0f);
            if(!activeScene.ToLower().Contains("distance") && Scoring.ammo_setting == "Live")
            {
                impactGo.SetActive(false);
            }

            if (StaticVariableManager.backgroundColorSetting.ToLower().Contains("red"))
            {
                impactGo.GetComponent<Renderer>().material.color = Color.yellow;
            }
            else
            {
                impactGo.GetComponent<Renderer>().material.color = Color.red;
            }

            if(activeScene.ToLower().Contains("suspect"))
            {
                //Destroy(impactGo, 0.0f);
            }
        }
        else
        {
            GameObject impactGo = Instantiate(bullethole_2, hit.point, Quaternion.LookRotation(hit.normal));
            switch (hit.transform.name[0])
            {
                case '1':
                    lane_1_impact_list.Add(impactGo);
                    print("RRE: adding for lane 1");
                    break;
                case '2':
                    lane_2_impact_list.Add(impactGo);
                    print("RRE: adding for lane 2");
                    break;
                case '3':
                    lane_3_impact_list.Add(impactGo);
                    print("RRE: adding for lane 3");
                    break;
            }
            //Destroy(impactGo, 0.15f);

            if (StaticVariableManager.backgroundColorSetting.ToLower().Contains("red"))
            {
                impactGo.GetComponent<Renderer>().material.color = Color.yellow;
            }
            else
            {
                impactGo.GetComponent<Renderer>().material.color = Color.red;
            }

            if(!activeScene.ToLower().Contains("distance"))
            {
                impactGo.SetActive(false);
            }
        }
    }
    private void generateBulletImpact2(RaycastHit hit)
    {
        if (Scoring.ammo_setting == "Live" || Scoring.ammo_setting.ToLower().Contains("laser"))
        {

            GameObject impactGo = Instantiate(bullethole_3, hit.point, Quaternion.LookRotation(hit.normal));
            impactGo.transform.name = "Miss";
            //print("Test: Point Reached...");

            if (hit.transform.tag == "Lane1MissCollider" || hit.transform.tag == "lane1hostage")
            {
                AssignMissImpact(hit, impactGo, lane_1_impact_list);
            }
            else if (hit.transform.tag == "Lane2MissCollider" || hit.transform.tag == "lane2hostage")
            {
                AssignMissImpact(hit, impactGo, lane_2_impact_list);
            }
            else if (hit.transform.tag == "Lane3MissCollider" || hit.transform.tag == "lane3hostage")
            {
                AssignMissImpact(hit, impactGo, lane_3_impact_list);
            }
            else if(activeScene.ToLower().Contains("pointbullseye"))
            {
                AssignMissImpact(hit, impactGo, lane_1_impact_list);
            }      
        }
    }

    private void AssignMissImpact(RaycastHit hit, GameObject impactGo, List<GameObject> impact_list)
    {
        //print("Test: " + hit.transform.name);
        impact_list.Add(impactGo);
        impactGo.GetComponent<BulletManager>().bullet_text.text = (timeActiveCounter).ToString("0.00") + " sec";
        //print("Test: adding for lane 1");

        if (StaticVariableManager.backgroundColorSetting.ToLower().Contains("blue"))
        {
            impactGo.GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            impactGo.GetComponent<Renderer>().material.color = Color.blue;
        }
        //Destroy(impactGo, 1.0f);
        if (!activeScene.ToLower().Contains("distance") && Scoring.ammo_setting == "Live")
        {
            impactGo.SetActive(false);
        }
    }

    private void HandleTargetHit(RaycastHit hit, GameObject impactGo)
    {
        //activeScene.ToLower().Contains("plat")
        //DropDown.scene_type.ToLower().Contains("static response")
        if (activeScene.ToLower().Contains("plat") || activeScene.ToLower().Contains("hiddentarget") || activeScene.ToLower().Contains("ipecboard"))
        {
            impactGo.GetComponent<BulletManager>().target_hit = hit.transform.gameObject;
        }
    }


    private List<string> updateLanePoint(string targetName, List<string> point_lane)
    {
        //Variables
        string[] pointOptions;
        int pointValue;

        targetName = targetName.Substring(2);

        foreach (char c in targetName)
        {
            if (char.IsDigit(c))
            {
                switch(c)
                 {
                    case '1':
                        pointOptions = point_lane[0].Split(',');
                        point_lane[0] = ""; //clear
                        pointValue = int.Parse(pointOptions[1]) + 1;
                        point_lane[0] = pointOptions[0] + "," + pointValue;
                        break;

                    case '2':
                        pointOptions = point_lane[1].Split(',');
                        point_lane[1] = ""; //clear
                        pointValue = int.Parse(pointOptions[1]) + 1;
                        point_lane[1] = pointOptions[0] + "," + pointValue;
                        break;

                    case '3':
                        pointOptions = point_lane[2].Split(',');
                        point_lane[2] = ""; //clear
                        pointValue = int.Parse(pointOptions[1]) + 1;
                        point_lane[2] = pointOptions[0] + "," + pointValue;
                        break;
                }
            }
                                
        }

        return point_lane;
    }
    private List<string> updateLanePoint(List<string> point_lane)
    {
        //Variables
        string[] pointOptions;
        int pointValue;

        pointOptions = point_lane[0].Split(',');
        point_lane[0] = ""; //clear
        pointValue = int.Parse(pointOptions[1]) + 1;
        point_lane[0] = pointOptions[0] + "," + pointValue;

        return point_lane;
    }
    void changeColor(RaycastHit hit)
    {
        /*float colorChangeTimer;
    float colorChangeTimerReset = 1;*/
        colorHolder = GameObject.FindGameObjectWithTag("IPECColor");
         currentTargetColr = hit.transform.gameObject.GetComponent<Renderer>().material;
        hitColor = colorHolder.transform.gameObject.GetComponent<Renderer>().material;

        PlayerPrefs.SetFloat("ObjectColorR", currentTargetColr.color.r);
        PlayerPrefs.SetFloat("ObjectColorG", currentTargetColr.color.g);
        PlayerPrefs.SetFloat("ObjectColorB", currentTargetColr.color.b);
        PlayerPrefs.SetFloat("ObjectColorA", currentTargetColr.color.a);

        // Load the color of the material
        r = PlayerPrefs.GetFloat("ObjectColorR");
        g = PlayerPrefs.GetFloat("ObjectColorG");
        b = PlayerPrefs.GetFloat("ObjectColorB");
        a = PlayerPrefs.GetFloat("ObjectColorA");

        currentTargetColr.color = hitColor.color;
        ColorManager.revert_head_body(hit);
        //print("Test:Color changed");
    }
    void revertColor()
    {
        currentTargetColr.color = new Color(r, g, b, a);
        print("RE: Color Reset");
    }
    private void StopTraining()

    {
        
        //Pause();
        trainingPaused = true;
        isScoreOpen = true;

        StaticVariableManager.isTrainingPause = trainingPaused;
        countDownStart.start_training = false;
        StaticVariableManager.isStopTraining = true;
        StaticVariableManager.startCountDown = false;

        targetFinished = true;
        targetsFinished = true;

        trainingStarted = false;
        ForestSound.Stop();
        ForestSound2.Stop();
        CitySound2.Stop();
        //standby_txt.SetActive(false);
        ReloadIndicator.SetActive(false);
        sendEndless("stop");
        //print("RE: in stop training...");

        if (!dead && !activeScene.ToLower().Contains("range"))
        {
            //Destroy(this.GetComponent<FirstPersonController>());
            //Destroy(this.GetComponent<Shooting>());
            // ((((enemyshot * 2) - civilianShot) / shotsFired) * 100);

            //score.enabled = false;
            assignShotsFiredData();
            //shooterScorePanel.SetActive(true);


            if (activeScene.ToLower().Contains("forest") || activeScene.ToLower().Contains("openplain") || activeScene.ToLower().Contains("mall") || activeScene.ToLower().Contains("container")
            || activeScene.ToLower().Contains("parking") || activeScene.ToLower().Contains("restaurant")) 
            {
                GameOver = true;
                Pause();

                float Enemy_Civ_TotalShots = ((enemyshot * 2));
                float percResult = (((((Enemy_Civ_TotalShots / shotsFired) * 60) - (numEnemies)) + ((enemyshot / numberOfEnemies)) * 40));
                float scoreResult = percResult - (0);

                if (mainPlayerLives <= 0f && numEnemies >= 0f)
                {
                    scoreResult = (scoreResult - (scoreResult * 0.5f));
                }

                if (scoreResult < 0)
                {
                    scoreResult = 0;
                }


                if (scoreResult >= 70)
                {
                    Result.text = "Well Done!!";
                }
                else
                {
                    Result.text = "Try again..";
                }

                // trainee_name.text += " : " + name;
                //totalEnemies.text += " " + numberOfEnemies;
                Location.text += " " + activeScene;
                enemiesKilled.text += " " + enemyshot + " out of " + numberOfEnemies;
                total_3d_training_time.text += " " + timeActiveCounter.ToString("0") + " sec";
                total_3d_shots_fired.text += " " + shotsFired;
                score.text += " " + scoreResult.ToString("0") + "%";

                if (activeScene.ToLower().Contains("forest"))
                {
                    //civiliansShot.enabled = false;
                    civiliansShot.text += " NA";
                }
                else
                {
                    civiliansShot.text += " " + civilianShot;
                }

            }
            else if (activeScene.ToLower().Contains("block"))
            {
                //print("Test: In block shooting score...");
                managePlateTargetScoring();
            }
            else if (activeScene.ToLower().Contains("fallingplat") || activeScene.ToLower().Contains("claypigeon") || activeScene.ToLower().Contains("dice") ||
                activeScene.ToLower().Contains("shell") || activeScene.ToLower().Contains("risingshape") || activeScene.ToLower().Contains("distancesimulator"))
            {
                if(activeScene.ToLower().Contains("distancesimulator"))
                {
                    //distanceSimulatorPanel.SetActive(false);
                }
                managePlateTargetScoring();
            }
            else if (activeScene.ToLower().Contains("cargame"))
            {
                manageCarGameScoring();
            }
            else if(activeScene.ToLower().Contains("sequencenum"))
            {
                manageSequenceNumScoring();
            }
            else if (activeScene.ToLower().Contains("colorsequence"))
            {

                manageColorSequenceScoring();
            }
            else if (activeScene.ToLower().Contains("ipec"))
            {
                manageRisingPlatesScoring();

                //head_1.SetActive(true);
                //head_2.SetActive(true);
                //body_1.SetActive(true);
                //body_2.SetActive(true);
                //body_3.SetActive(true);
            }
            else if (activeScene.ToLower().Contains("rifflepole"))
            {
                managePlateTargetScoring();
            }
            else if (activeScene.ToLower().Contains("dueling"))
            {
                manageDuelingTreeScoring();
            }
            else if (activeScene == "BasicResetable")
            {
                totPlates = TestConditionsManager.totalAllowedHitShots;

                float plateRatio = (platesHit / totPlates);
                float percResult = plateRatio * 100;
                float scoreResult = percResult;

                if (scoreResult < 0)
                {
                    scoreResult = 0;
                }

                trainee_name.text += " : " + name;
                Location.text = "Time: " + timeActiveCounter.ToString("0") + " Seconds ";
                enemiesKilled.text = "Plates Shot: " + platesHit;
                civiliansShot.text = "Shots Missed : " + (shotsFired - platesHit);


                plate_score = platesHit; // omp score

                if (activeScene == "Outdoor_FOREST" || activeScene.Contains("range"))
                {
                    civiliansShot.enabled = false;
                }

                //score.text = "SCORE : " + scoreResult.ToString("0");

                if (timeFinished && !targetsFinished)
                {
                    Result.text = "Mission Failed, Timeout!";
                }
                else if (ammaFinished && !targetsFinished)
                {
                    Result.text = "Mission Failed, Out of ammo!";
                }
                else if (targetsFinished && !ammaFinished && !timeFinished)
                {
                    Result.text = "Mission Succesful!";
                }

            }
            else if (activeScene.ToLower().Contains("targetpopup") || activeScene.ToLower().Contains("threatening") || activeScene.ToLower().Contains("point")
                || activeScene.ToLower().Contains("animaltarget"))
            {
                manageTargetPopUpScoring();
            }
            else if (activeScene.ToLower().Contains("hunting"))
            {
                score.enabled = true;
                if (activeScene.ToLower().Contains("direct"))
                {
                    totPlates = TestConditionsManager.totalAllowedHitShots;

                    float ShotsMissed = (shotsFired - ((StaticVariableManager.totalBodyShots * 2) + StaticVariableManager.totalHeadShots));
                    float targetsKilled = StaticVariableManager.totalTargetAnimalsKilled;
                    //float scoreResult = percResult;

                    if (ShotsMissed < 0)
                    {
                        ShotsMissed = 0;
                    }

                    total_3d_shots_fired.text += " " + shotsFired;
                    total_3d_training_time.text = "Time: " + timeActiveCounter.ToString("0") + " Seconds ";
                    enemiesKilled.text = TestConditionsManager.animalName + "s Shot: " + targetsKilled;
                    civiliansShot.text = "Shots Missed : " + ShotsMissed;
                    score.text = "Head Shots : " + StaticVariableManager.totalHeadShots;

                    //plate_score = platesHit; // omp score

                    if (activeScene == "Outdoor_FOREST" || activeScene.Contains("range"))
                    {
                        civiliansShot.enabled = false;
                    }

                    //score.text = "SCORE : " + scoreResult.ToString("0");

                    Result.text = "...Complete...";
                }
                else if(activeScene.ToLower().Contains("avoid"))
                {
                    float ShotsMissed = (shotsFired - ((StaticVariableManager.totalBodyShots * 2) + StaticVariableManager.totalHeadShots));
                    float targetsKilled = StaticVariableManager.totalTargetAnimalsKilled;
                    //float scoreResult = percResult;

                    if (ShotsMissed < 0)
                    {
                        ShotsMissed = 0;
                    }

                    total_3d_shots_fired.text += " " + shotsFired;
                    total_3d_training_time.text = "Time: " + timeActiveCounter.ToString("0") + " Seconds ";
                    enemiesKilled.text = TestConditionsManager.animalName + "s Shot: " + targetsKilled;
                    civiliansShot.text = "Casualties : " + StaticVariableManager.totalTargetCasualtiesKilled;
                    score.text = "Head Shots : " + StaticVariableManager.totalHeadShots;

                    //plate_score = platesHit; // omp score

                    if (activeScene == "Outdoor_FOREST" || activeScene.Contains("range"))
                    {
                        civiliansShot.enabled = false;
                    }

                    //score.text = "SCORE : " + scoreResult.ToString("0");

                    Result.text = "...Complete...";
                }
            }
            else if (activeScene.ToLower().Contains("cyclic"))
            {

                if(distanceSimulatorPanel != null)
                {
                    //distanceSimulatorPanel.SetActive(false);
                }
                manageCyclicTargetsScoring();

            }
            else if (activeScene.ToLower().Contains("5target"))
            {
                manageCyclicTargetsScoring();
            }
            else if (activeScene.ToLower().Contains("risingplate"))
            {
                manageRisingPlatesScoring();
            }
            else if (activeScene.ToLower().Contains("hiddentarget"))
            {
                manageHiddenTargetScoring();
            }
            else if (activeScene.ToLower().Contains("baloon"))
            {
                manageBaloonScoring();
            }
            else if (activeScene.ToLower().Contains("suspectshoot"))
            {
                manageSuspectShootScoring();
            }
            else
            {
                float Enemy_Civ_TotalShots = ((enemyshot * 2) - civilianShot);
                float percResult = ((Enemy_Civ_TotalShots / shotsFired) * 100);
                float scoreResult = percResult - (numberOfEnemies - enemyshot);

                if (mainPlayerLives <= 0f && numEnemies >= 0f)
                {
                    scoreResult = (scoreResult - (scoreResult * 0.5f));
                }
                if (scoreResult < 0)
                {
                    scoreResult = 0;
                }



                if (scoreResult >= 50)
                {
                    Result.text = " ";
                }
                else
                {
                    Result.text = " ";
                }

                trainee_name.text += " : " + name;
                Location.text += " : " + activeScene;
                enemiesKilled.text += " : " + enemyshot;
                civiliansShot.text += " : " + civilianShot;
                score.text += " : " + scoreResult.ToString("0");
            }


            if (Scoring.simulation_type == "training")
            {
                saveScore();
                //saveScoreScreenCapture();
            }
            //print("RE: Game Over is " + GameOver);
            //mainPanel.SetActive(true);
            if (Scoring.ammo_setting.ToLower().Contains("live"))
            {
                if(activeScene.ToLower().Contains("ipec") || activeScene.ToLower().Contains("basictargetpopup"))
                {
                    //shooterScorePanel.SetActive(false);
                }
            }
            

            //Debug.Log("%%%%%%%%%% OVER %%%%%%%%%%%%");
            //sendEndless("dead");
            dead = true;
            //Pause();

            //deactivate firstPerson movement script
            //player.GetComponent<FirstPersonMovement>().enabled = false;
            //player.GetComponent<FirstPersonController>().enabled = false;
            //player.GetComponent<NewFirstPersonController>().enabled = false;
        }

        StaticVariableManager.isScoreDataSet = true;
    }

    void saveScore()
    {
        Thread.Sleep(500);
        if (GetTrainees.trainee_id == null)
        {
            GetTrainees.trainee_id = "1234";
        }
        if (GetTrainees.trainee_name == null)
        {
            GetTrainees.trainee_name = "default";
        }
        if (activeScene.ToLower().Contains("range") || activeScene.ToLower().Contains("basic"))
        {
            //this.GetComponent<Scoring>().SaveScore(GetTrainees.trainee_id, GetTrainees.trainee_name, activeScene, login_Manager.EmailText, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), plate_score.ToString(), plate_score.ToString());
        }
        else
        {
            this.GetComponent<Scoring>().SaveScore(GetTrainees.trainee_id, GetTrainees.trainee_name, activeScene, login_Manager.EmailText, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), civilianShot.ToString(), enemyshot.ToString(), ((int)(((enemyshot * 2) - civilianShot) / shotsFired) * 100).ToString(), shotsFired.ToString());

        }
        // this.GetComponent<Scoring>().SaveScore(GetTrainees.trainee_id, GetTrainees.trainee_name, activeScene, login_Manager.EmailText, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), civilianShot.ToString(), enemyshot.ToString(), ((int)(((enemyshot * 2) - civilianShot) / shotsFired) * 100).ToString(), shotsFired.ToString());
    }
    private void assignShotsFiredData()
    {
        //print("Test: Point reached...");
        numLane1ShotsFired = lane1PointsHit + numLane1ShotsMissed;
        numLane2ShotsFired = lane2PointsHit + numLane2ShotsMissed;
        numLane3ShotsFired = lane3PointsHit + numLane3ShotsMissed;

        lane1ShotsFiredDisplay.text = numLane1ShotsFired.ToString("0");
        lane2ShotsFiredDisplay.text = numLane2ShotsFired.ToString("0");
        lane3ShotsFiredDisplay.text = numLane3ShotsFired.ToString("0");
    }

    private void manageCyclicTargetsScoring()
    {
        float percentage1 = 0;
        float percentage2 = 0;
        float percentage3 = 0;

        assistPanel.SetActive(false);
        lane1StopSignal.SetActive(false);
        lane2StopSignal.SetActive(false);
        lane3StopSignal.SetActive(false);

        if (laneSelected == 3 || !adminScoreUpdated)
        {
            lane_number_Txt.text = "Lane 3";
            string[] pointOptions;
            foreach (string line in point_lane_3)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane3TrainingScorePoints = totalPoints;
            
            percentage3 = generatePercentage(numLane3ShotsFired, numLane3ShotsMissed);

            Location.text = "Run Time: " + lane3ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane3PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane3ShotsMissed.ToString("0");
            Result.text = percentage3.ToString("0.0") + "%";
            admin_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";
            shooter_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";
            lane3SplitTimeDisplay.text = "No split time.";

            int pos = 1;
            foreach (string tim in lane3SplitTime)
            {
                if (lane3SplitTimeSet == false)
                {
                    lane3SplitTimeDisplay.text = "";
                    lane3SplitTimeSet = true;
                }
                lane3SplitTimeDisplay.text += tim + "\n";

                lane3SplitTimeString += tim;
                if (pos < lane3SplitTime.Count)
                {
                    lane3SplitTimeString += ";";
                    pos++;
                }
            }

            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            
        }
        if (laneSelected == 2 || !adminScoreUpdated)
        {
            lane_number_Txt.text = "Lane 2";
            string[] pointOptions;
            foreach (string line in point_lane_2)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane2TrainingScorePoints = totalPoints;
            percentage2 = generatePercentage(numLane2ShotsFired, numLane2ShotsMissed);

            Location.text = "Run Time: " + lane2ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane2PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane2ShotsMissed.ToString("0");
            Result.text = percentage2.ToString("0.0") + "%";
            admin_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";
            shooter_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";
            lane2SplitTimeDisplay.text = "No split time.";

            int pos = 1;
            foreach (string tim in lane2SplitTime)
            {
                if (lane2SplitTimeSet == false)
                {
                    lane2SplitTimeDisplay.text = "";
                    lane2SplitTimeSet = true;
                }
                lane2SplitTimeDisplay.text += tim + "\n";

                lane2SplitTimeString += tim;
                if (pos < lane2SplitTime.Count)
                {
                    lane2SplitTimeString += ";";
                    pos++;
                }
            }
            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            
        }
        if (laneSelected == 1 || !adminScoreUpdated)
        {
            lane_number_Txt.text = "Lane 1";
            string[] pointOptions;
            foreach (string line in point_lane_1)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane1TrainingScorePoints = totalPoints;
            percentage1 = generatePercentage(numLane1ShotsFired, numLane1ShotsMissed);

            Location.text = "Run Time: " + lane1ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane1PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane1ShotsMissed.ToString("0");
            Result.text = percentage1.ToString("0.0") + "%";
            admin_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";
            shooter_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";
            lane1SplitTimeDisplay.text = "No split time.";

            int pos = 1;
            foreach (string tim in lane1SplitTime)
            {
                if (lane1SplitTimeSet == false)
                {
                    lane1SplitTimeDisplay.text = "";
                    lane1SplitTimeSet = true;
                }
                lane1SplitTimeDisplay.text += tim + "\n";

                lane1SplitTimeString += tim;
                if (pos < lane1SplitTime.Count)
                {
                    lane1SplitTimeString += ";";
                    pos++;
                }
            }
            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
           
        }

        if (adminScoreUpdated == false)
        {
            constructSaveData(percentage1, percentage2, percentage3);
            updateScoreDisplay();

            adminScoreUpdated = true;
        }
    }
    private void manageTargetPopUpScoring()
    {
        float percentage1 = 0;
        float percentage2 = 0;
        float percentage3 = 0;
        assistPanel.SetActive(false);
        lane1StopSignal.SetActive(false);
        lane2StopSignal.SetActive(false);
        lane3StopSignal.SetActive(false);

        if (laneSelected == 3 || !adminScoreUpdated)
        {

            lane_number_Txt.text = "Lane 3";
            string[] pointOptions;
            foreach (string line in point_lane_3)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane3TrainingScorePoints = totalPoints;
            percentage3 = generatePercentage(numLane3ShotsFired, numLane3ShotsMissed);

            Location.text = "Run Time: " + lane3ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane3PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane3ShotsMissed.ToString("0");
            Result.text = percentage3.ToString("0.0") + "%";
            admin_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";
            shooter_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";

            lane3SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane3SplitTime)
            {
                if (lane3SplitTimeSet == false)
                {
                    lane3SplitTimeDisplay.text = "";
                    lane3SplitTimeSet = true;
                }
                lane3SplitTimeDisplay.text += tim + "\n";

                lane3SplitTimeString += tim;
                if (pos < lane3SplitTime.Count)
                {
                    lane3SplitTimeString += ";";
                    pos++;
                }
            }

            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 2 || !adminScoreUpdated)
        {
            lane_number_Txt.text = "Lane 2";
            string[] pointOptions;
            foreach (string line in point_lane_2)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane2TrainingScorePoints = totalPoints;
            percentage2 = generatePercentage(numLane2ShotsFired, numLane2ShotsMissed);

            Location.text = "Run Time: " + lane2ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane2PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane2ShotsMissed.ToString("0");
            Result.text = percentage2.ToString("0.0") + "%";
            admin_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";
            shooter_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";

            lane2SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane2SplitTime)
            {
                if (lane2SplitTimeSet == false)
                {
                    lane2SplitTimeDisplay.text = "";
                    lane2SplitTimeSet = true;
                }
                lane2SplitTimeDisplay.text += tim + "\n";

                lane2SplitTimeString += tim;
                if (pos < lane2SplitTime.Count)
                {
                    lane2SplitTimeString += ";";
                    pos++;
                }
            }
            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 1 || !adminScoreUpdated)
        {
            lane_number_Txt.text = "Lane 1";
            string[] pointOptions;
            foreach (string line in point_lane_1)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 5) + (point_2_hits * 5) + (point_3_hits * 2);

            if(activeScene.ToLower().Contains("animaltarget"))
            {
                lane1TrainingScorePoints = AnimalTargetPointManager.pointsCounter;
            }
            else
            {
                lane1TrainingScorePoints = totalPoints;
            }

            percentage1 = generatePercentage(numLane1ShotsFired, numLane1ShotsMissed);

            //print("RE: Lane percentage is " + numLane1ShotsMissed);

            Location.text = "Run Time: " + lane1ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane1PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane1ShotsMissed.ToString("0");
            Result.text = percentage1.ToString("0.0") + "%";
            admin_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";
            shooter_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";

            lane1SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane1SplitTime)
            {
                if (lane1SplitTimeSet == false)
                {
                    lane1SplitTimeDisplay.text = "";
                    lane1SplitTimeSet = true;
                }
                lane1SplitTimeDisplay.text += tim + "\n";

                lane1SplitTimeString += tim;
                if (pos < lane1SplitTime.Count)
                {
                    lane1SplitTimeString += ";";
                    pos++;
                }

            }
            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }

        if (adminScoreUpdated == false)
        {
            constructSaveData(percentage1, percentage2, percentage3);
            updateScoreDisplay();
            adminScoreUpdated = true;
        }
    }
    private void manageRisingPlatesScoring()
    {
        float percentage1 = 0;
        float percentage2 = 0;
        float percentage3 = 0;
        assistPanel.SetActive(false);
        lane1StopSignal.SetActive(false);
        lane2StopSignal.SetActive(false);
        lane3StopSignal.SetActive(false);

        if (laneSelected == 3 || !adminScoreUpdated)
        {
            
            lane_number_Txt.text = "Lane 3";
            string[] pointOptions;
            foreach (string line in point_lane_3)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane3TrainingScorePoints = totalPoints;
            percentage3 = generatePercentage(numLane3ShotsFired, numLane3ShotsMissed);

            Location.text = "Run Time: " + lane3ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane3PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane3ShotsMissed.ToString("0");
            Result.text = percentage3.ToString("0.0") + "%";
            admin_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";
            shooter_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";

            lane3SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane3SplitTime)
            {
                if (lane3SplitTimeSet == false)
                {
                    lane3SplitTimeDisplay.text = "";
                    lane3SplitTimeSet = true;
                }
                lane3SplitTimeDisplay.text += tim + "\n";

                lane3SplitTimeString += tim;
                if(pos < lane3SplitTime.Count)
                {
                    lane3SplitTimeString += ";";
                    pos++;
                }
            }

            //Respose Time Assignment
            lane3ResponseTimeString = generateResponseString(lane3ResponseTime);

            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 2 || !adminScoreUpdated)
        {
            lane_number_Txt.text = "Lane 2";
            string[] pointOptions;
            foreach (string line in point_lane_2)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane2TrainingScorePoints = totalPoints;
            percentage2 = generatePercentage(numLane2ShotsFired, numLane2ShotsMissed);

            Location.text = "Run Time: " + lane2ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane2PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane2ShotsMissed.ToString("0");
            Result.text = percentage2.ToString("0.0") + "%";
            admin_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";
            shooter_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";

            lane2SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane2SplitTime)
            {
                if (lane2SplitTimeSet == false)
                {
                    lane2SplitTimeDisplay.text = "";
                    lane2SplitTimeSet = true;
                }
                lane2SplitTimeDisplay.text += tim + "\n";

                lane2SplitTimeString += tim;
                if (pos < lane2SplitTime.Count)
                {
                    lane2SplitTimeString += ";";
                    pos++;
                }
            }

            //Response Time Assignemnt
            lane2ResponseTimeString = generateResponseString(lane2ResponseTime);
            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 1 || !adminScoreUpdated)
        {
            lane_number_Txt.text = "Lane 1";
            string[] pointOptions;
            foreach (string line in point_lane_1)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane1TrainingScorePoints = totalPoints;
            percentage1 = generatePercentage(numLane1ShotsFired, numLane1ShotsMissed);

            
            //print("RE: Lane percentage is " + numLane1ShotsMissed);

            Location.text = "Run Time: " + lane1ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane1PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane1ShotsMissed.ToString("0");
            Result.text = percentage1.ToString("0.0") + "%";
            admin_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";
            shooter_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";

            lane1SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane1SplitTime)
            {
                if (lane1SplitTimeSet == false)
                {
                    lane1SplitTimeDisplay.text = "";
                    lane1SplitTimeSet = true;
                }
                lane1SplitTimeDisplay.text += tim + "\n";

                lane1SplitTimeString += tim;
                if (pos < lane1SplitTime.Count)
                {
                    lane1SplitTimeString += ";";
                    pos++;
                }

            }
            //Response Time Assignemnt
            lane1ResponseTimeString = generateResponseString(lane1ResponseTime);

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }

        if (adminScoreUpdated == false)
        {
            constructSaveData(percentage1, percentage2, percentage3);
            updateScoreDisplay();
            adminScoreUpdated = true;
        }
    }
    private void manageHiddenTargetScoring()
    {
        float percentage1 = 0;
        float percentage2 = 0;
        float percentage3 = 0;
        assistPanel.SetActive(false);
        lane1StopSignal.SetActive(false);
        lane2StopSignal.SetActive(false);
        lane3StopSignal.SetActive(false);

        if (laneSelected == 3 || !adminScoreUpdated)
        {

            lane_number_Txt.text = "Lane 3";
            string[] pointOptions;
            foreach (string line in point_lane_3)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane3TrainingScorePoints = totalPoints;
            percentage3 = generatePercentage(numLane3ShotsFired, numLane3ShotsMissed);

            Location.text = "Run Time: " + lane3ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane3PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane3ShotsMissed.ToString("0");
            Result.text = percentage3.ToString("0.0") + "%";
            admin_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";
            shooter_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";

            lane3SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane3SplitTime)
            {
                if (lane3SplitTimeSet == false)
                {
                    lane3SplitTimeDisplay.text = "";
                    lane3SplitTimeSet = true;
                }
                lane3SplitTimeDisplay.text += tim + "\n";

                lane3SplitTimeString += tim;
                if (pos < lane3SplitTime.Count)
                {
                    lane3SplitTimeString += ";";
                    pos++;
                }
            }

            //Respose Time Assignment
            lane3ResponseTimeString = generateResponseString(lane3ResponseTime);

            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 2 || !adminScoreUpdated)
        {
            lane_number_Txt.text = "Lane 2";
            string[] pointOptions;
            foreach (string line in point_lane_2)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane2TrainingScorePoints = totalPoints;
            percentage2 = generatePercentage(numLane2ShotsFired, numLane2ShotsMissed);

            Location.text = "Run Time: " + lane2ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane2PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane2ShotsMissed.ToString("0");
            Result.text = percentage2.ToString("0.0") + "%";
            admin_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";
            shooter_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";

            lane2SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane2SplitTime)
            {
                if (lane2SplitTimeSet == false)
                {
                    lane2SplitTimeDisplay.text = "";
                    lane2SplitTimeSet = true;
                }
                lane2SplitTimeDisplay.text += tim + "\n";

                lane2SplitTimeString += tim;
                if (pos < lane2SplitTime.Count)
                {
                    lane2SplitTimeString += ";";
                    pos++;
                }
            }

            //Response Time Assignemnt
            lane2ResponseTimeString = generateResponseString(lane2ResponseTime);
            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 1 || !adminScoreUpdated)
        {
            lane_number_Txt.text = "Lane 1";
            string[] pointOptions;
            foreach (string line in point_lane_1)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane1TrainingScorePoints = totalPoints;
            percentage1 = generatePercentage(numLane1ShotsFired, numLane1ShotsMissed);


            //print("RE: Lane percentage is " + numLane1ShotsMissed);

            Location.text = "Run Time: " + lane1ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane1PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane1ShotsMissed.ToString("0");
            Result.text = percentage1.ToString("0.0") + "%";
            admin_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";
            shooter_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";

            lane1SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane1SplitTime)
            {
                if (lane1SplitTimeSet == false)
                {
                    lane1SplitTimeDisplay.text = "";
                    lane1SplitTimeSet = true;
                }
                lane1SplitTimeDisplay.text += tim + "\n";

                lane1SplitTimeString += tim;
                if (pos < lane1SplitTime.Count)
                {
                    lane1SplitTimeString += ";";
                    pos++;
                }

            }
            //Response Time Assignemnt
            lane1ResponseTimeString = generateResponseString(lane1ResponseTime);

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }

        if (adminScoreUpdated == false)
        {
            constructSaveData(percentage1, percentage2, percentage3);
            updateScoreDisplay();
            adminScoreUpdated = true;
        }
    }
    private void manageSuspectShootScoring()
    {
        shooterScorePanel.SetActive(false);
        adminScorePanel.SetActive(false);

        float percentage1 = 0;
        float percentage2 = 0;
        float percentage3 = 0;
        assistPanel.SetActive(false);
        lane1StopSignal.SetActive(false);
        lane2StopSignal.SetActive(false);
        lane3StopSignal.SetActive(false);

        if (laneSelected == 3 || !adminScoreUpdated)
        {

            lane_number_Txt.text = "Lane 3";
            string[] pointOptions;
            foreach (string line in point_lane_3)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane3TrainingScorePoints = totalPoints;
            percentage3 = generatePercentage(numLane3ShotsFired, numLane3ShotsMissed);

            Location.text = "Run Time: " + lane3ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane3PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane3ShotsMissed.ToString("0");
            Result.text = percentage3.ToString("0.0") + "%";
            admin_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";
            shooter_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";

            lane3SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane3SplitTime)
            {
                if (lane3SplitTimeSet == false)
                {
                    lane3SplitTimeDisplay.text = "";
                    lane3SplitTimeSet = true;
                }
                lane3SplitTimeDisplay.text += tim + "\n";

                lane3SplitTimeString += tim;
                if (pos < lane3SplitTime.Count)
                {
                    lane3SplitTimeString += ";";
                    pos++;
                }
            }

            //Respose Time Assignment
            lane3ResponseTimeString = generateResponseString(lane3ResponseTime);

            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 2 || !adminScoreUpdated)
        {
            lane_number_Txt.text = "Lane 2";
            string[] pointOptions;
            foreach (string line in point_lane_2)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane2TrainingScorePoints = totalPoints;
            percentage2 = generatePercentage(numLane2ShotsFired, numLane2ShotsMissed);

            Location.text = "Run Time: " + lane2ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane2PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane2ShotsMissed.ToString("0");
            Result.text = percentage2.ToString("0.0") + "%";
            admin_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";
            shooter_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";

            lane2SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane2SplitTime)
            {
                if (lane2SplitTimeSet == false)
                {
                    lane2SplitTimeDisplay.text = "";
                    lane2SplitTimeSet = true;
                }
                lane2SplitTimeDisplay.text += tim + "\n";

                lane2SplitTimeString += tim;
                if (pos < lane2SplitTime.Count)
                {
                    lane2SplitTimeString += ";";
                    pos++;
                }
            }

            //Response Time Assignemnt
            lane2ResponseTimeString = generateResponseString(lane2ResponseTime);
            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 1 || !adminScoreUpdated)
        {
            lane_number_Txt.text = "Lane 1";
            string[] pointOptions;
            foreach (string line in point_lane_1)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane1TrainingScorePoints = totalPoints;
            percentage1 = generatePercentage(numLane1ShotsFired, numLane1ShotsMissed);


            //print("RE: Lane percentage is " + numLane1ShotsMissed);

            Location.text = "Run Time: " + lane1ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane1PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane1ShotsMissed.ToString("0");
            Result.text = percentage1.ToString("0.0") + "%";
            admin_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";
            shooter_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";

            lane1SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane1SplitTime)
            {
                if (lane1SplitTimeSet == false)
                {
                    lane1SplitTimeDisplay.text = "";
                    lane1SplitTimeSet = true;
                }
                lane1SplitTimeDisplay.text += tim + "\n";

                lane1SplitTimeString += tim;
                if (pos < lane1SplitTime.Count)
                {
                    lane1SplitTimeString += ";";
                    pos++;
                }

            }
            //Response Time Assignemnt
            lane1ResponseTimeString = generateResponseString(lane1ResponseTime);

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }

        if (adminScoreUpdated == false)
        {
            constructSaveData(percentage1, percentage2, percentage3);
            //updateScoreDisplay();
            adminScoreUpdated = true;
        }

        if (activeScene.ToLower().Contains("1lane"))
        {
            lane1Result.text = "Threats Shot: " + lane1ThreatsShot + " \n"
                + "Non-Threats Shot: " + lane1NonThreatsShot + "\n"
                + "Shots Missed: " + numLane1ShotsMissed;
            lane1ResultPanel.SetActive(true);
        }
        else if (activeScene.ToLower().Contains("2lane"))
        {
            if (!lane1TargetsComplete && !StaticVariableManager.isLane1TargetMissed)
            {
                lane1Result.text = "Threats Shot: " + lane1ThreatsShot + " \n"
                + "Non-Threats Shot: " + lane1NonThreatsShot + "\n"
                + "Shots Missed: " + numLane1ShotsMissed;
                lane1ResultPanel.SetActive(true);
            }
            if (!lane2TargetsComplete && !StaticVariableManager.isLane2TargetMissed)
            {
                lane2Result.text = "Threats Shot: " + lane2ThreatsShot + " \n"
                + "Non-Threats Shot: " + lane2NonThreatsShot + "\n"
                + "Shots Missed: " + numLane2ShotsMissed;
                lane2ResultPanel.SetActive(true);
            }
        }
    }
    private void managePlateTargetScoring()
    {
        float percentage1 = 0;
        float percentage2 = 0;
        float percentage3 = 0;
        assistPanel.SetActive(false);
        lane1StopSignal.SetActive(false);
        lane2StopSignal.SetActive(false);
        lane3StopSignal.SetActive(false);

        if (laneSelected == 3 || !adminScoreUpdated)
        {
            //print("RRE: lane3Selected");
            lane_number_Txt.text = "Lane 3";
            string[] pointOptions;
            foreach (string line in point_lane_3)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 2);
            lane3TrainingScorePoints = totalPoints;
            percentage3 = generatePercentage(numLane3ShotsFired, numLane3ShotsMissed);

            Location.text = "Run Time: " + lane3ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane3PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane3ShotsMissed.ToString("0");
            Result.text = percentage3.ToString("0.0") + "%";
            admin_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";
            shooter_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";

            lane3SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane3SplitTime)
            {
                if (lane3SplitTimeSet == false)
                {
                    lane3SplitTimeDisplay.text = "";
                    lane3SplitTimeSet = true;
                }
                lane3SplitTimeDisplay.text += tim + "\n";

                lane3SplitTimeString += tim;
                if (pos < lane3SplitTime.Count)
                {
                    lane3SplitTimeString += ";";
                    pos++;
                }
            }

            //Respose Time Assignment
            lane3ResponseTimeString = generateResponseString(lane3ResponseTime);

            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 2 || !adminScoreUpdated)
        {
            //print("RRE: lane2Selected");
            lane_number_Txt.text = "Lane 2";
            string[] pointOptions;
            foreach (string line in point_lane_2)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 2);
            lane2TrainingScorePoints = totalPoints;
            percentage2 = generatePercentage(numLane2ShotsFired, numLane2ShotsMissed);

            Location.text = "Run Time: " + lane2ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane2PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane2ShotsMissed.ToString("0");
            Result.text = percentage2.ToString("0.0") + "%";
            admin_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";
            shooter_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";

            lane2SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane2SplitTime)
            {
                if (lane2SplitTimeSet == false)
                {
                    lane2SplitTimeDisplay.text = "";
                    lane2SplitTimeSet = true;
                }
                lane2SplitTimeDisplay.text += tim + "\n";

                lane2SplitTimeString += tim;
                if (pos < lane2SplitTime.Count)
                {
                    lane2SplitTimeString += ";";
                    pos++;
                }
            }
            //splitTimeDisplay.text = "None";

            //Respose Time Assignment
            lane2ResponseTimeString = generateResponseString(lane2ResponseTime);

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 1 || !adminScoreUpdated)
        {
            //print("RRE: lane1Selected");
            lane_number_Txt.text = "Lane 1";
            string[] pointOptions;
            foreach (string line in point_lane_1)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 2);
            lane1TrainingScorePoints = totalPoints;
            percentage1 = generatePercentage(numLane1ShotsFired, numLane1ShotsMissed);

            if(activeScene.ToLower().Contains("rifflepole"))
            {
                lane1PointsHit = TargetController.num_pass_hits;
                numLane1ShotsMissed = numLane1ShotsMissed + TargetController.num_fail_hits;
            }

            print("Test: active time is " + lane1ActiveTimeCounter);
            Location.text = "Run Time: " + lane1ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane1PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane1ShotsMissed.ToString("0");
            Result.text = percentage1.ToString("0.0") + "%";
            admin_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";
            shooter_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";

            lane1SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane1SplitTime)
            {
                if (lane1SplitTimeSet == false)
                {
                    lane1SplitTimeDisplay.text = "";
                    lane1SplitTimeSet = true;
                }
                lane1SplitTimeDisplay.text += tim + "\n";

                lane1SplitTimeString += tim;
                if (pos < lane1SplitTime.Count)
                {
                    lane1SplitTimeString += ";";
                    pos++;
                }

            }
            //splitTimeDisplay.text = "None";

            //Respose Time Assignment
            lane1ResponseTimeString = generateResponseString(lane1ResponseTime);

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }

        if (adminScoreUpdated == false)
        {
            constructSaveData(percentage1, percentage2, percentage3);
            updateScoreDisplay();
            adminScoreUpdated = true;
        }
    }
    private void manageCarGameScoring()
    {
        float percentage1 = 0;
        float percentage2 = 0;
        float percentage3 = 0;
        assistPanel.SetActive(false);
        lane1StopSignal.SetActive(false);
        lane2StopSignal.SetActive(false);
        lane3StopSignal.SetActive(false);

        if (laneSelected == 3 || !adminScoreUpdated)
        {
            //print("RRE: lane3Selected");
            lane_number_Txt.text = "Lane 3";
            string[] pointOptions;
            foreach (string line in point_lane_3)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 5) + (point_2_hits * 5) + (point_3_hits * 2);
            lane3TrainingScorePoints = totalPoints;
            percentage3 = generatePercentage(numLane3ShotsFired, numLane3ShotsMissed);

            Location.text = "Run Time: " + lane3ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane3PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane3ShotsMissed.ToString("0");
            Result.text = percentage3.ToString("0.0") + "%";
            admin_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";
            shooter_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";

            lane3SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane3SplitTime)
            {
                if (lane3SplitTimeSet == false)
                {
                    lane3SplitTimeDisplay.text = "";
                    lane3SplitTimeSet = true;
                }
                lane3SplitTimeDisplay.text += tim + "\n";

                lane3SplitTimeString += tim;
                if (pos < lane3SplitTime.Count)
                {
                    lane3SplitTimeString += ";";
                    pos++;
                }
            }

            //Respose Time Assignment
            lane3ResponseTimeString = generateResponseString(lane3ResponseTime);

            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 2 || !adminScoreUpdated)
        {
            //print("RRE: lane2Selected");
            lane_number_Txt.text = "Lane 2";
            string[] pointOptions;
            foreach (string line in point_lane_2)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane2TrainingScorePoints = (StaticVariableManager.blueCarCompletedPoints * 2) - numLane2ShotsMissed;
            percentage2 = generatePercentage(numLane2ShotsFired, numLane2ShotsMissed);

            if (lane2TrainingScorePoints < 0)
            {
                lane2TrainingScorePoints = 0;
            }

            Location.text = "Run Time: " + lane2ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane2PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane2ShotsMissed.ToString("0");
            Result.text = percentage2.ToString("0.0") + "%";
            admin_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";
            shooter_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";

            lane2SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane2SplitTime)
            {
                if (lane2SplitTimeSet == false)
                {
                    lane2SplitTimeDisplay.text = "";
                    lane2SplitTimeSet = true;
                }
                lane2SplitTimeDisplay.text += tim + "\n";

                lane2SplitTimeString += tim;
                if (pos < lane2SplitTime.Count)
                {
                    lane2SplitTimeString += ";";
                    pos++;
                }
            }
            //splitTimeDisplay.text = "None";

            //Respose Time Assignment
            lane2ResponseTimeString = generateResponseString(lane2ResponseTime);

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 1 || !adminScoreUpdated)
        {
            //print("RRE: lane1Selected");
            lane_number_Txt.text = "Lane 1";
            string[] pointOptions;
            foreach (string line in point_lane_1)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane1TrainingScorePoints = (StaticVariableManager.redCarCompletedPoints * 2) - numLane1ShotsMissed;
            //print("Points: " + StaticVariableManager.redCarPoints);
            //print("Missed: " + numLane1ShotsMissed);
            if(lane1TrainingScorePoints < 0)
            {
                lane1TrainingScorePoints = 0;
            }

            percentage1 = generatePercentage(numLane1ShotsFired, numLane1ShotsMissed);

            if (activeScene.ToLower().Contains("rifflepole"))
            {
                lane1PointsHit = TargetController.num_pass_hits;
                numLane1ShotsMissed = numLane1ShotsMissed + TargetController.num_fail_hits;
            }

            //print("Test: active time is " + lane1ActiveTimeCounter);
            Location.text = "Run Time: " + lane1ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane1PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane1ShotsMissed.ToString("0");
            Result.text = percentage1.ToString("0.0") + "%";
            admin_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";
            shooter_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";

            lane1SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane1SplitTime)
            {
                if (lane1SplitTimeSet == false)
                {
                    lane1SplitTimeDisplay.text = "";
                    lane1SplitTimeSet = true;
                }
                lane1SplitTimeDisplay.text += tim + "\n";

                lane1SplitTimeString += tim;
                if (pos < lane1SplitTime.Count)
                {
                    lane1SplitTimeString += ";";
                    pos++;
                }

            }
            //splitTimeDisplay.text = "None";

            //Respose Time Assignment
            lane1ResponseTimeString = generateResponseString(lane1ResponseTime);

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }

        StaticVariableManager.redCarMoveSteps = 0;
        StaticVariableManager.blueCarMoveSteps = 0;

        if (StaticVariableManager.redCarCompletedPoints > StaticVariableManager.blueCarCompletedPoints &&
            StaticVariableManager.redCarCompletedPoints > StaticVariableManager.greenCarCompletedPoints)
        {
            winnerScore_carGame.text = "WINNER: Red Shooter";
            winnerScore_carGame.color = Color.red;
        }
        else if (StaticVariableManager.blueCarCompletedPoints > StaticVariableManager.redCarCompletedPoints &&
            StaticVariableManager.blueCarCompletedPoints > StaticVariableManager.greenCarCompletedPoints)
        {
            winnerScore_carGame.text = "WINNER: Blue Shooter";
            winnerScore_carGame.color = Color.blue;
        }
        else if (StaticVariableManager.greenCarCompletedPoints > StaticVariableManager.redCarCompletedPoints &&
    StaticVariableManager.greenCarCompletedPoints > StaticVariableManager.redCarCompletedPoints)
        {
            winnerScore_carGame.text = "WINNER: Green Shooter";
            winnerScore_carGame.color = Color.green;
        }
        else
        {
            winnerScore_carGame.text = "WINNER: NA";
            winnerScore_carGame.color = Color.black;
        }


        if (adminScoreUpdated == false)
        {
            constructSaveData(percentage1, percentage2, percentage3);
            updateScoreDisplay();
            adminScoreUpdated = true;
        }
    }
    private void manageSequenceNumScoring()
    {
        float percentage1 = 0;
        float percentage2 = 0;
        float percentage3 = 0;
        assistPanel.SetActive(false);
        lane1StopSignal.SetActive(false);
        lane2StopSignal.SetActive(false);
        lane3StopSignal.SetActive(false);

        if (laneSelected == 3 || !adminScoreUpdated)
        {
            //print("RRE: lane3Selected");
            lane_number_Txt.text = "Lane 3";
            string[] pointOptions;
            foreach (string line in point_lane_3)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 5) + (point_2_hits * 5) + (point_3_hits * 2);
            lane3TrainingScorePoints = totalPoints;
            percentage3 = generatePercentage(numLane3ShotsFired, numLane3ShotsMissed);

            Location.text = "Run Time: " + lane3ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane3PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane3ShotsMissed.ToString("0");
            Result.text = percentage3.ToString("0.0") + "%";
            admin_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";
            shooter_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";

            lane3SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane3SplitTime)
            {
                if (lane3SplitTimeSet == false)
                {
                    lane3SplitTimeDisplay.text = "";
                    lane3SplitTimeSet = true;
                }
                lane3SplitTimeDisplay.text += tim + "\n";

                lane3SplitTimeString += tim;
                if (pos < lane3SplitTime.Count)
                {
                    lane3SplitTimeString += ";";
                    pos++;
                }
            }

            //Respose Time Assignment
            lane3ResponseTimeString = generateResponseString(lane3ResponseTime);

            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 2 || !adminScoreUpdated)
        {
            //print("RRE: lane2Selected");
            lane_number_Txt.text = "Lane 2";
            string[] pointOptions;
            foreach (string line in point_lane_2)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane2TrainingScorePoints = (StaticVariableManager.blueCarCompletedPoints * 2) - numLane2ShotsMissed;
            percentage2 = generatePercentage(numLane2ShotsFired, numLane2ShotsMissed);

            if (lane2TrainingScorePoints < 0)
            {
                lane2TrainingScorePoints = 0;
            }

            Location.text = "Run Time: " + lane2ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane2PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane2ShotsMissed.ToString("0");
            Result.text = percentage2.ToString("0.0") + "%";
            admin_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";
            shooter_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";

            lane2SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane2SplitTime)
            {
                if (lane2SplitTimeSet == false)
                {
                    lane2SplitTimeDisplay.text = "";
                    lane2SplitTimeSet = true;
                }
                lane2SplitTimeDisplay.text += tim + "\n";

                lane2SplitTimeString += tim;
                if (pos < lane2SplitTime.Count)
                {
                    lane2SplitTimeString += ";";
                    pos++;
                }
            }
            //splitTimeDisplay.text = "None";

            //Respose Time Assignment
            lane2ResponseTimeString = generateResponseString(lane2ResponseTime);

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 1 || !adminScoreUpdated)
        {
            //print("RRE: lane1Selected");
            lane_number_Txt.text = "Lane 1";
            string[] pointOptions;
            foreach (string line in point_lane_1)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 2) ;
            lane1TrainingScorePoints = totalPoints - (StaticVariableManager.currentProgress - StaticVariableManager.currentGoal);
            //print("Points: " + StaticVariableManager.redCarPoints);
            //print("Missed: " + numLane1ShotsMissed);
            if (lane1TrainingScorePoints < 0)
            {
                lane1TrainingScorePoints = 0;
            }

            percentage1 = generatePercentage(numLane1ShotsFired, numLane1ShotsMissed);

            if (activeScene.ToLower().Contains("rifflepole"))
            {
                lane1PointsHit = TargetController.num_pass_hits;
                numLane1ShotsMissed = numLane1ShotsMissed + TargetController.num_fail_hits;
            }

            //print("Test: active time is " + lane1ActiveTimeCounter);
            Location.text = "Run Time: " + lane1ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane1PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane1ShotsMissed.ToString("0");
            Result.text = percentage1.ToString("0.0") + "%";
            admin_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";
            shooter_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";

            lane1SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane1SplitTime)
            {
                if (lane1SplitTimeSet == false)
                {
                    lane1SplitTimeDisplay.text = "";
                    lane1SplitTimeSet = true;
                }
                lane1SplitTimeDisplay.text += tim + "\n";

                lane1SplitTimeString += tim;
                if (pos < lane1SplitTime.Count)
                {
                    lane1SplitTimeString += ";";
                    pos++;
                }

            }
            //splitTimeDisplay.text = "None";

            //Respose Time Assignment
            lane1ResponseTimeString = generateResponseString(lane1ResponseTime);

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }

        StaticVariableManager.redCarMoveSteps = 0;
        StaticVariableManager.blueCarMoveSteps = 0;

        if(activeScene.ToLower().Contains("add"))
        {
            if (StaticVariableManager.currentProgress == StaticVariableManager.currentGoal)
            {
                winnerScore_carGame.text = "WELL DONE!";
                winnerScore_carGame.color = Color.black;
            }
            else if (StaticVariableManager.currentProgress > StaticVariableManager.currentGoal)
            {
                int extraValues = StaticVariableManager.currentProgress - StaticVariableManager.currentGoal;

                if (extraValues == 1)
                {
                    winnerScore_carGame.text = (extraValues).ToString() + " EXTRA VALUE";
                }
                else
                {
                    winnerScore_carGame.text = (extraValues).ToString() + " EXTRA VALUES";
                }
                winnerScore_carGame.color = Color.black;
            }
            else
            {
                winnerScore_carGame.text = (StaticVariableManager.currentGoal - StaticVariableManager.currentProgress).ToString() + " VALUES SHORT";
                winnerScore_carGame.color = Color.black;
            }
        }
        else
        {
            winnerScore_carGame.text = (StaticVariableManager.correctNumberHits + " CORRECT \n" + StaticVariableManager.wrongNumberHits + " INCORRECT");
            winnerScore_carGame.color = Color.black;
        }


        if (adminScoreUpdated == false)
        {
            constructSaveData(percentage1, percentage2, percentage3);
            updateScoreDisplay();
            adminScoreUpdated = true;
        }
    }
    private void manageColorSequenceScoring()
    {
        float percentage1 = 0;
        float percentage2 = 0;
        float percentage3 = 0;
        assistPanel.SetActive(false);
        lane1StopSignal.SetActive(false);
        lane2StopSignal.SetActive(false);
        lane3StopSignal.SetActive(false);

        if (laneSelected == 3 || !adminScoreUpdated)
        {
            //print("RRE: lane3Selected");
            lane_number_Txt.text = "Lane 3";
            string[] pointOptions;
            foreach (string line in point_lane_3)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 5) + (point_2_hits * 5) + (point_3_hits * 2);
            lane3TrainingScorePoints = totalPoints;
            percentage3 = generatePercentage(numLane3ShotsFired, numLane3ShotsMissed);

            Location.text = "Run Time: " + lane3ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane3PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane3ShotsMissed.ToString("0");
            Result.text = percentage3.ToString("0.0") + "%";
            admin_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";
            shooter_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";

            lane3SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane3SplitTime)
            {
                if (lane3SplitTimeSet == false)
                {
                    lane3SplitTimeDisplay.text = "";
                    lane3SplitTimeSet = true;
                }
                lane3SplitTimeDisplay.text += tim + "\n";

                lane3SplitTimeString += tim;
                if (pos < lane3SplitTime.Count)
                {
                    lane3SplitTimeString += ";";
                    pos++;
                }
            }

            //Respose Time Assignment
            lane3ResponseTimeString = generateResponseString(lane3ResponseTime);

            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 2 || !adminScoreUpdated)
        {
            //print("RRE: lane2Selected");
            lane_number_Txt.text = "Lane 2";
            string[] pointOptions;
            foreach (string line in point_lane_2)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane2TrainingScorePoints = (StaticVariableManager.blueCarCompletedPoints * 2) - numLane2ShotsMissed;
            percentage2 = generatePercentage(numLane2ShotsFired, numLane2ShotsMissed);

            if (lane2TrainingScorePoints < 0)
            {
                lane2TrainingScorePoints = 0;
            }

            Location.text = "Run Time: " + lane2ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane2PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane2ShotsMissed.ToString("0");
            Result.text = percentage2.ToString("0.0") + "%";
            admin_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";
            shooter_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";

            lane2SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane2SplitTime)
            {
                if (lane2SplitTimeSet == false)
                {
                    lane2SplitTimeDisplay.text = "";
                    lane2SplitTimeSet = true;
                }
                lane2SplitTimeDisplay.text += tim + "\n";

                lane2SplitTimeString += tim;
                if (pos < lane2SplitTime.Count)
                {
                    lane2SplitTimeString += ";";
                    pos++;
                }
            }
            //splitTimeDisplay.text = "None";

            //Respose Time Assignment
            lane2ResponseTimeString = generateResponseString(lane2ResponseTime);

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 1 || !adminScoreUpdated)
        {
            //print("RRE: lane1Selected");
            lane_number_Txt.text = "Lane 1";
            string[] pointOptions;
            foreach (string line in point_lane_1)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 2);
            lane1TrainingScorePoints = totalPoints - (StaticVariableManager.currentProgress - StaticVariableManager.currentGoal);
            //print("Points: " + StaticVariableManager.redCarPoints);
            //print("Missed: " + numLane1ShotsMissed);
            if (lane1TrainingScorePoints < 0)
            {
                lane1TrainingScorePoints = 0;
            }

            percentage1 = generatePercentage(numLane1ShotsFired, numLane1ShotsMissed);

            if (activeScene.ToLower().Contains("rifflepole"))
            {
                lane1PointsHit = TargetController.num_pass_hits;
                numLane1ShotsMissed = numLane1ShotsMissed + TargetController.num_fail_hits;
            }

            //print("Test: active time is " + lane1ActiveTimeCounter);
            Location.text = "Run Time: " + lane1ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane1PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane1ShotsMissed.ToString("0");
            Result.text = percentage1.ToString("0.0") + "%";
            admin_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";
            shooter_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";

            lane1SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane1SplitTime)
            {
                if (lane1SplitTimeSet == false)
                {
                    lane1SplitTimeDisplay.text = "";
                    lane1SplitTimeSet = true;
                }
                lane1SplitTimeDisplay.text += tim + "\n";

                lane1SplitTimeString += tim;
                if (pos < lane1SplitTime.Count)
                {
                    lane1SplitTimeString += ";";
                    pos++;
                }

            }
            //splitTimeDisplay.text = "None";

            //Respose Time Assignment
            lane1ResponseTimeString = generateResponseString(lane1ResponseTime);

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }

        StaticVariableManager.redCarMoveSteps = 0;
        StaticVariableManager.blueCarMoveSteps = 0;

        if (StaticVariableManager.wrongColorHits == 0)
        {
            //winnerScore_carGame.text = "WELL DONE: ";
            winnerScore_carGame.text = (StaticVariableManager.correctColorHits + " CORRECT \n" + StaticVariableManager.wrongColorHits + " INCORRECT");
            winnerScore_carGame.color = Color.black;
        }
        else
        {
            winnerScore_carGame.text = (StaticVariableManager.correctColorHits + " CORRECT \n" + StaticVariableManager.wrongColorHits + " INCORRECT");
            winnerScore_carGame.color = Color.black;
        }


        if (adminScoreUpdated == false)
        {
            constructSaveData(percentage1, percentage2, percentage3);
            updateScoreDisplay();
            adminScoreUpdated = true;
        }
    }
    private void manageDuelingTreeScoring()
    {
        float percentage1 = 0;
        float percentage2 = 0;
        float percentage3 = 0;
        assistPanel.SetActive(false);
        lane1StopSignal.SetActive(false);
        lane2StopSignal.SetActive(false);
        lane3StopSignal.SetActive(false);

        if (laneSelected == 3 || !adminScoreUpdated)
        {
            print("RRE: lane3Selected");
            lane_number_Txt.text = "Lane 3";
            string[] pointOptions;
            foreach (string line in point_lane_3)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane3TrainingScorePoints = totalPoints;
            percentage3 = generatePercentage(numLane3ShotsFired, numLane3ShotsMissed);

            Location.text = "Run Time: " + lane3ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane3PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane3ShotsMissed.ToString("0");
            Result.text = percentage3.ToString("0.0") + "%";
            admin_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";
            shooter_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";

            lane3SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane3SplitTime)
            {
                if (lane3SplitTimeSet == false)
                {
                    lane3SplitTimeDisplay.text = "";
                    lane3SplitTimeSet = true;
                }
                lane3SplitTimeDisplay.text += tim + "\n";

                lane3SplitTimeString += tim;
                if (pos < lane3SplitTime.Count)
                {
                    lane3SplitTimeString += ";";
                    pos++;
                }
            }

            //Respose Time Assignment
            lane3ResponseTimeString = generateResponseString(lane3ResponseTime);

            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 2 || !adminScoreUpdated)
        {
            print("RRE: lane2Selected");
            lane_number_Txt.text = "Lane 2";
            string[] pointOptions;
            foreach (string line in point_lane_2)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane2TrainingScorePoints = totalPoints;
            percentage2 = generatePercentage(numLane2ShotsFired, numLane2ShotsMissed);

            Location.text = "Run Time: " + lane2ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane2PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane2ShotsMissed.ToString("0");
            Result.text = percentage2.ToString("0.0") + "%";
            admin_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";
            shooter_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";

            lane2SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane2SplitTime)
            {
                if (lane2SplitTimeSet == false)
                {
                    lane2SplitTimeDisplay.text = "";
                    lane2SplitTimeSet = true;
                }
                lane2SplitTimeDisplay.text += tim + "\n";

                lane2SplitTimeString += tim;
                if (pos < lane2SplitTime.Count)
                {
                    lane2SplitTimeString += ";";
                    pos++;
                }
            }
            //splitTimeDisplay.text = "None";

            //Respose Time Assignment
            lane2ResponseTimeString = generateResponseString(lane2ResponseTime);

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }
        if (laneSelected == 1 || !adminScoreUpdated)
        {
            print("RRE: lane1Selected");
            lane_number_Txt.text = "Lane 1";
            string[] pointOptions;
            foreach (string line in point_lane_1)
            {
                pointOptions = line.Split(",");
                if (pointOptions[0].ToLower().Contains("point1"))
                {
                    point_1_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point2"))
                {
                    point_2_hits = int.Parse(pointOptions[1]);
                }
                if (pointOptions[0].ToLower().Contains("point3"))
                {
                    point_3_hits = int.Parse(pointOptions[1]);
                }
            }
            float totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
            lane1TrainingScorePoints = totalPoints;
            percentage1 = generatePercentage(numLane1ShotsFired, numLane1ShotsMissed);

            //print("RE: Lane percentage is " + numLane1ShotsMissed);

            Location.text = "Run Time: " + lane1ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane1PointsHit;
            civiliansShot.text = "Total points : " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane1ShotsMissed.ToString("0");
            Result.text = percentage1.ToString("0.0") + "%";
            admin_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";
            shooter_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";

            lane1SplitTimeDisplay.text = "No split time.";
            int pos = 1;
            foreach (string tim in lane1SplitTime)
            {
                if (lane1SplitTimeSet == false)
                {
                    lane1SplitTimeDisplay.text = "";
                    lane1SplitTimeSet = true;
                }
                lane1SplitTimeDisplay.text += tim + "\n";

                lane1SplitTimeString += tim;
                if (pos < lane1SplitTime.Count)
                {
                    lane1SplitTimeString += ";";
                    pos++;
                }

            }
            //splitTimeDisplay.text = "None";

            //Respose Time Assignment
            lane1ResponseTimeString = generateResponseString(lane1ResponseTime);

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            if (timeFinished && !targetsFinished)
            {
                //Result.text = "Timeout!";
            }
            else
            {
                //Result.text = "...Complete...";
            }
        }

        if (adminScoreUpdated == false)
        {
            constructSaveData(percentage1, percentage2, percentage3);
            updateScoreDisplay();
            adminScoreUpdated = true;
        }
    }
    private void manageBaloonScoring()
    {
        float percentage1 = 0;
        float percentage2 = 0;
        float percentage3 = 0;
        assistPanel.SetActive(false);
        lane1StopSignal.SetActive(false);
        lane2StopSignal.SetActive(false);
        lane3StopSignal.SetActive(false);

        if (activeScene.ToLower().Contains("2lane"))
        {
            lane1StrikeOutSignal.SetActive(false);
            lane2StrikeOutSignal.SetActive(false);
        }

        if (laneSelected == 3 || !adminScoreUpdated)
        {
            lane_number_Txt.text = "Lane 3";

            float totalPoints = (lane3PointsHit * 10) - (5 * lane3_strike_count);

            if (totalPoints < 0)
            {
                totalPoints = 0;
            }
            lane3TrainingScorePoints = totalPoints;
            percentage3 = generatePercentage(numLane3ShotsFired, numLane3ShotsMissed);

            int pos = 1;
            foreach (string tim in lane3ResponseTime)
            {
                /*if (lane3SplitTimeSet == false)
                {
                    lane3SplitTimeDisplay.text = "";
                    lane3SplitTimeSet = true;
                }*/
                //lane3SplitTimeDisplay.text += tim + "\n";

                lane3ResponseTimeString += tim;
                if (pos < lane3ResponseTime.Count)
                {
                    lane3ResponseTimeString += ":";
                    pos++;
                }
            }

            Location.text = "Run Time: " + lane3ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane3PointsHit;
            civiliansShot.text = "Total points: " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane3ShotsMissed.ToString("0");
            Result.text = percentage3.ToString("0.0") + "%";
            admin_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";
            shooter_Lane3_PercentageTxt.text = percentage3.ToString("0.0") + "%";
            lane3SplitTimeDisplay.text = "No split time.";

            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            
        }
        if (laneSelected == 2 || !adminScoreUpdated)
        {
            lane_number_Txt.text = "Lane 2";

            float totalPoints = (lane2PointsHit * 10) - (5 * lane2_strike_count);

            if (totalPoints < 0)
            {
                totalPoints = 0;
            }
            lane2TrainingScorePoints = totalPoints;
            percentage2 = generatePercentage(numLane2ShotsFired, numLane2ShotsMissed);

            int pos = 1;
            foreach (string tim in lane2ResponseTime)
            {
                /*if (lane3SplitTimeSet == false)
                {
                    lane3SplitTimeDisplay.text = "";
                    lane3SplitTimeSet = true;
                }*/
                //lane3SplitTimeDisplay.text += tim + "\n";

                lane2ResponseTimeString += tim;
                if (pos < lane2ResponseTime.Count)
                {
                    lane2ResponseTimeString += ":";
                    pos++;
                }
            }

            Location.text = "Run Time: " + lane2ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane2PointsHit;
            civiliansShot.text = "Total points: " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane2ShotsMissed.ToString("0");
            Result.text = percentage2.ToString("0.0") + "%";
            admin_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";
            shooter_Lane2_PercentageTxt.text = percentage2.ToString("0.0") + "%";
            lane2SplitTimeDisplay.text = "No split time.";
            //splitTimeDisplay.text = "None";

            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            
        }
        if (laneSelected == 1 || !adminScoreUpdated)
        {
            lane_number_Txt.text = "Lane 1";

            float totalPoints = (lane1PointsHit * 10) - (5 * lane1_strike_count);

            if (totalPoints < 0)
            {
                totalPoints = 0;
            }
            lane1TrainingScorePoints = totalPoints;
            percentage1 = generatePercentage(numLane1ShotsFired, numLane1ShotsMissed);

            int pos = 1;
            foreach (string tim in lane1ResponseTime)
            {
                /*if (lane3SplitTimeSet == false)
                {
                    lane3SplitTimeDisplay.text = "";
                    lane3SplitTimeSet = true;
                }*/
                //lane3SplitTimeDisplay.text += tim + "\n";

                lane1ResponseTimeString += tim;
                if (pos < lane1ResponseTime.Count)
                {
                    lane1ResponseTimeString += ":";
                    pos++;
                }
            }

            Location.text = "Run Time: " + lane1ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane1PointsHit;
            civiliansShot.text = "Total points: " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane1ShotsMissed.ToString("0");
            Result.text = percentage1.ToString("0.0") + "%";
            admin_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";
            shooter_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";
            lane1SplitTimeDisplay.text = "No split time.";
            //splitTimeDisplay.text = "None";

            winnerScore_carGame.text = (StaticVariableManager.correctColorHits + " CORRECT \n" + StaticVariableManager.wrongColorHits + " INCORRECT");
            winnerScore_carGame.color = Color.black;
            plate_score = platesHit; // omp score

            //score.text = "SCORE : " + scoreResult.ToString("0");
            
        }


        if (adminScoreUpdated == false)
        {
            constructSaveData(percentage1, percentage2, percentage3);
            updateScoreDisplay();

            adminScoreUpdated = true;
        }
    }
    private void updateScoreDisplay()
    {
        if (activeScene.ToLower().Contains("1lane"))
        {
            //admin_Lane1_HitTimeTxt.text = lane1ActiveTimeCounter.ToString("0.0") + " sec";
            admin_Lane1_Header.text = lane1Header[0];  //Header
            admin_Lane1_ShotsFiredTxt.text = numLane1ShotsFired.ToString("0");
            admin_Lane1_ShotsMissedTxt.text = numLane1ShotsMissed.ToString("0");
            admin_Lane1_HitTimeTxt.text = lane1ActiveTimeCounter.ToString("0.0") + " sec";
            admin_Lane1_HitShotsTxt.text = lane1PointsHit.ToString("0");
            admin_Lane1_ScoreTxt.text = lane1TrainingScorePoints.ToString("0");
            
            shooter_Lane1_Header.text = lane1Header[0];  //Header
            shooter_Lane1_SplitTime_Header.text = lane1Header[0];  //Header
            shooter_Lane1_ShotsFiredTxt.text = numLane1ShotsFired.ToString("0");
            shooter_Lane1_ShotsMissedTxt.text = numLane1ShotsMissed.ToString("0");
            shooter_Lane1_HitTimeTxt.text = lane1ActiveTimeCounter.ToString("0.0") + " sec";
            shooter_Lane1_HitShotsTxt.text = lane1PointsHit.ToString("0");
            shooter_Lane1_ScoreTxt.text = lane1TrainingScorePoints.ToString("0");

            //adminScorePanel.SetActive(true);
            //shooterScorePanel.SetActive(true);
        }
        if (activeScene.ToLower().Contains("2lane"))
        {
            admin_Lane1_Header.text = lane1Header[0];  //Header
            admin_Lane1_ShotsFiredTxt.text = numLane1ShotsFired.ToString("0");
            admin_Lane1_ShotsMissedTxt.text = numLane1ShotsMissed.ToString("0");
            admin_Lane1_HitTimeTxt.text = lane1ActiveTimeCounter.ToString("0.0") + " sec";
            admin_Lane1_HitShotsTxt.text = lane1PointsHit.ToString("0");
            admin_Lane1_ScoreTxt.text = lane1TrainingScorePoints.ToString("0");
            admin_Lane2_Header.text = lane2Header[0];  //Header
            admin_Lane2_ShotsFiredTxt.text = numLane2ShotsFired.ToString("0");
            admin_Lane2_ShotsMissedTxt.text = numLane2ShotsMissed.ToString("0");
            admin_Lane2_HitTimeTxt.text = lane2ActiveTimeCounter.ToString("0.0") + " sec";
            admin_Lane2_HitShotsTxt.text = lane2PointsHit.ToString("0");
            admin_Lane2_ScoreTxt.text = lane2TrainingScorePoints.ToString("0");
            shooter_Lane1_Header.text = lane1Header[0];  //Header
            shooter_Lane1_SplitTime_Header.text = lane1Header[0];  //Header
            shooter_Lane1_ShotsFiredTxt.text = numLane1ShotsFired.ToString("0");
            shooter_Lane1_ShotsMissedTxt.text = numLane1ShotsMissed.ToString("0");
            shooter_Lane1_HitTimeTxt.text = lane1ActiveTimeCounter.ToString("0.0") + " sec";
            shooter_Lane1_HitShotsTxt.text = lane1PointsHit.ToString("0");
            shooter_Lane1_ScoreTxt.text = lane1TrainingScorePoints.ToString("0");
            shooter_Lane2_Header.text = lane2Header[0];  //Header
            shooter_Lane2_SplitTime_Header.text = lane2Header[0];  //Header
            shooter_Lane2_ShotsFiredTxt.text = numLane2ShotsFired.ToString("0");
            shooter_Lane2_ShotsMissedTxt.text = numLane2ShotsMissed.ToString("0");
            shooter_Lane2_HitTimeTxt.text = lane2ActiveTimeCounter.ToString("0.0") + " sec";
            shooter_Lane2_HitShotsTxt.text = lane2PointsHit.ToString("0");
            shooter_Lane2_ScoreTxt.text = lane2TrainingScorePoints.ToString("0");
            
            //adminScorePanel.SetActive(true);
            //shooterScorePanel.SetActive(true);
        }
        if (activeScene.ToLower().Contains("3lane"))
        {
            admin_Lane1_Header.text = lane1Header[0];  //Header
            admin_Lane1_ShotsFiredTxt.text = numLane1ShotsFired.ToString("0");
            admin_Lane1_ShotsMissedTxt.text = numLane1ShotsMissed.ToString("0");
            admin_Lane1_HitTimeTxt.text = lane1ActiveTimeCounter.ToString("0.0") + " sec";
            admin_Lane1_HitShotsTxt.text = lane1PointsHit.ToString("0");
            admin_Lane1_ScoreTxt.text = lane1TrainingScorePoints.ToString("0");
            admin_Lane2_Header.text = lane2Header[0];  //Header
            admin_Lane2_ShotsFiredTxt.text = numLane2ShotsFired.ToString("0");
            admin_Lane2_ShotsMissedTxt.text = numLane2ShotsMissed.ToString("0");
            admin_Lane2_HitTimeTxt.text = lane2ActiveTimeCounter.ToString("0.0") + " sec";
            admin_Lane2_HitShotsTxt.text = lane2PointsHit.ToString("0");
            admin_Lane2_ScoreTxt.text = lane2TrainingScorePoints.ToString("0");
            admin_Lane3_Header.text = lane3Header[0];  //Header
            admin_Lane3_ShotsFiredTxt.text = numLane3ShotsFired.ToString("0");
            admin_Lane3_ShotsMissedTxt.text = numLane3ShotsMissed.ToString("0");
            admin_Lane3_HitTimeTxt.text = lane3ActiveTimeCounter.ToString("0.0") + " sec";
            admin_Lane3_HitShotsTxt.text = lane3PointsHit.ToString("0");
            admin_Lane3_ScoreTxt.text = lane3TrainingScorePoints.ToString("0");

            shooter_Lane1_Header.text = lane1Header[0];  //Header
            shooter_Lane1_SplitTime_Header.text = lane1Header[0];  //Header
            shooter_Lane1_ShotsFiredTxt.text = numLane1ShotsFired.ToString("0");
            shooter_Lane1_ShotsMissedTxt.text = numLane1ShotsMissed.ToString("0");
            shooter_Lane1_HitTimeTxt.text = lane1ActiveTimeCounter.ToString("0.0") + " sec";
            shooter_Lane1_HitShotsTxt.text = lane1PointsHit.ToString("0");
            shooter_Lane1_ScoreTxt.text = lane1TrainingScorePoints.ToString("0");
            shooter_Lane2_Header.text = lane2Header[0];  //Header
            shooter_Lane2_SplitTime_Header.text = lane2Header[0];  //Header
            shooter_Lane2_ShotsFiredTxt.text = numLane2ShotsFired.ToString("0");
            shooter_Lane2_ShotsMissedTxt.text = numLane2ShotsMissed.ToString("0");
            shooter_Lane2_HitTimeTxt.text = lane2ActiveTimeCounter.ToString("0.0") + " sec";
            shooter_Lane2_HitShotsTxt.text = lane2PointsHit.ToString("0");
            shooter_Lane2_ScoreTxt.text = lane2TrainingScorePoints.ToString("0");
            shooter_Lane3_Header.text = lane3Header[0];  //Header
            shooter_Lane3_SplitTime_Header.text = lane3Header[0];  //Header
            shooter_Lane3_ShotsFiredTxt.text = numLane3ShotsFired.ToString("0");
            shooter_Lane3_ShotsMissedTxt.text = numLane3ShotsMissed.ToString("0");
            shooter_Lane3_HitTimeTxt.text = lane3ActiveTimeCounter.ToString("0.0") + " sec";
            shooter_Lane3_HitShotsTxt.text = lane3PointsHit.ToString("0");
            shooter_Lane3_ScoreTxt.text = lane3TrainingScorePoints.ToString("0");
            
            //adminScorePanel.SetActive(true);
            //shooterScorePanel.SetActive(true);
        }
    }
    private string generateResponseString(List<string> laneResponseTime)
    {
        int pos = 1;
        string tempResponseString = "";

        //print("RRE: Response data size is " + laneResponseTime.Count);
        foreach (string tim in laneResponseTime)
        {
            
            if (pos <= laneResponseTime.Count)
            {
                //print("RRE: time" + tim);
                tempResponseString += tim;
                if (pos < laneResponseTime.Count)
                {
                    tempResponseString += "-";
                    pos++;
                }
            }
        }
        //print("RRE: Response is " + tempResponseString);
        return tempResponseString;
    }
    private void constructSaveData(float perc1, float perc2, float perc3)
    {
        Scoring.simulation_type = "training";
        if (Scoring.simulation_type == "training")
        {
            string numLanes = "0";
            sceneScores = new List<string>();
            scenePercentages = new List<string>();
            laneTraineeNames = new List<string>();
            sceneTargetsHit = new List<string>();
            sceneShotsMissed = new List<string>();
            laneSplitTime = new List<string>();
            laneResponseTime = new List<string>();
            laneTrainingTime = new List<string>();
            lane1Header = new string[2]; lane1Header[0] = "Lane 1";
            lane2Header = new string[2]; lane2Header[0] = "Lane 2";
            lane3Header = new string[2]; lane3Header[0] = "Lane 3";
            
            //set percentage
            lane1PercentagePoints = perc1;
            lane2PercentagePoints = perc2;
            lane3PercentagePoints = perc3;

            if (activeScene.ToLower().Contains("cargame"))
            {
                lane1Header[0] = "Red";
                lane2Header[0] = "Blue";
            }

            if (GetTrainees.TraineeLane_1 != "")
            {
                lane1Header = new string[2];
                lane1TraineeName = GetTrainees.TraineeLane_1;
                if(TestConditionsManager.lane1TraineeName.ToLower().Contains("lane"))
                {
                    lane1Header[0] = TestConditionsManager.lane1TraineeName;
                }
                else
                {
                    lane1Header = TestConditionsManager.lane1TraineeName.Split(" ");
                }

            }
            if (GetTrainees.TraineeLane_2 != "")
            {
                lane2Header = new string[2];
                lane2TraineeName = GetTrainees.TraineeLane_2;
                if (TestConditionsManager.lane1TraineeName.ToLower().Contains("lane"))
                {
                    lane2Header[0] = TestConditionsManager.lane2TraineeName;
                }
                else
                {
                    lane2Header = TestConditionsManager.lane2TraineeName.Split(" ");
                }
            }
            if (GetTrainees.TraineeLane_3 != "")
            {
                lane3Header = new string[2];
                lane3TraineeName = GetTrainees.TraineeLane_3;
                if (TestConditionsManager.lane1TraineeName.ToLower().Contains("lane"))
                {
                    lane3Header[0] = TestConditionsManager.lane3TraineeName;
                }
                else
                {
                    lane3Header = TestConditionsManager.lane3TraineeName.Split(" ");
                }
            }

            lane1SplitTimeString = ReplaceCharacter(lane1SplitTimeString, ';', ':');
            lane2SplitTimeString = ReplaceCharacter(lane2SplitTimeString, ';', ':');
            lane3SplitTimeString = ReplaceCharacter(lane3SplitTimeString, ';', ':');

            if (activeScene.ToLower().Contains("1lane"))
            {
                sceneScores.Add(lane1TrainingScorePoints.ToString("0"));
                scenePercentages.Add(perc1.ToString("0"));
                laneTraineeNames.Add(lane1TraineeName);
                sceneTargetsHit.Add(lane1PointsHit.ToString("0"));
                sceneShotsMissed.Add(numLane1ShotsMissed.ToString("0"));
                laneSplitTime.Add(lane1SplitTimeString);
                laneResponseTime.Add(lane1ResponseTimeString);
                laneTrainingTime.Add(lane1ActiveTimeCounter.ToString("0.0"));
                numLanes = "1";
                //print("RRE: lane 1 response time " + lane1ResponseTimeString);
            }
            else if (activeScene.ToLower().Contains("2lane"))
            {
                sceneScores.Add(lane1TrainingScorePoints.ToString("0"));
                sceneScores.Add(lane2TrainingScorePoints.ToString("0"));
                scenePercentages.Add(perc1.ToString("0"));
                scenePercentages.Add(perc2.ToString("0"));
                laneTraineeNames.Add(lane1TraineeName);
                laneTraineeNames.Add(lane2TraineeName);
                sceneTargetsHit.Add(lane1PointsHit.ToString("0"));
                sceneTargetsHit.Add(lane2PointsHit.ToString("0"));
                sceneShotsMissed.Add(numLane1ShotsMissed.ToString("0"));
                sceneShotsMissed.Add(numLane2ShotsMissed.ToString("0"));
                laneSplitTime.Add(lane1SplitTimeString);
                laneSplitTime.Add(lane2SplitTimeString);
                laneResponseTime.Add(lane1ResponseTimeString);
                laneResponseTime.Add(lane2ResponseTimeString);
                laneTrainingTime.Add(lane1ActiveTimeCounter.ToString("0.0"));
                laneTrainingTime.Add(lane2ActiveTimeCounter.ToString("0.0"));
                numLanes = "2";
                print("RRE: lane 2 response time " + lane1ResponseTimeString);
            }
            else if (activeScene.ToLower().Contains("3lane"))
            {
                sceneScores.Add(lane1TrainingScorePoints.ToString("0"));
                sceneScores.Add(lane2TrainingScorePoints.ToString("0"));
                sceneScores.Add(lane2TrainingScorePoints.ToString("0"));
                scenePercentages.Add(perc1.ToString("0"));
                scenePercentages.Add(perc2.ToString("0"));
                scenePercentages.Add(perc3.ToString("0"));
                laneTraineeNames.Add(lane1TraineeName);
                laneTraineeNames.Add(lane2TraineeName);
                laneTraineeNames.Add(lane3TraineeName);
                sceneTargetsHit.Add(lane1PointsHit.ToString("0"));
                sceneTargetsHit.Add(lane2PointsHit.ToString("0"));
                sceneTargetsHit.Add(lane3PointsHit.ToString("0"));
                sceneShotsMissed.Add(numLane1ShotsMissed.ToString("0"));
                sceneShotsMissed.Add(numLane2ShotsMissed.ToString("0"));
                sceneShotsMissed.Add(numLane3ShotsMissed.ToString("0"));
                laneSplitTime.Add(lane1SplitTimeString);
                laneSplitTime.Add(lane2SplitTimeString);
                laneSplitTime.Add(lane3SplitTimeString);
                laneResponseTime.Add(lane1ResponseTimeString);
                laneResponseTime.Add(lane2ResponseTimeString);
                laneResponseTime.Add(lane3ResponseTimeString);
                laneTrainingTime.Add(lane1ActiveTimeCounter.ToString("0.0"));
                laneTrainingTime.Add(lane2ActiveTimeCounter.ToString("0.0"));
                laneTrainingTime.Add(lane3ActiveTimeCounter.ToString("0.0"));
                numLanes = "3";
                //print("RRE: lane 1 response time " + lane1ResponseTimeString);
                //print("RRE: lane 1 response time " + lane2ResponseTimeString);
                //print("RRE: lane 3 response time " + lane3ResponseTimeString);
            }

            //Function Call
            string ompSceneName = DropDown.ompSceneName.ToString();
            //print("Check: " + activeScene);
            //print("Test: " + lane1ResponseTimeString);
            //print("Test: " + lane2ResponseTimeString);
            //print("Test: " + lane3ResponseTimeString);
            this.GetComponent<Scoring>().SaveTrainingScore(GetTrainees.trainee_id, GetTrainees.trainee_name, ompSceneName, login_Manager.EmailText, DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss"), sceneScores, scenePercentages, numLanes, sceneTargetsHit, sceneShotsMissed, laneTraineeNames, laneSplitTime, laneTrainingTime, laneResponseTime);
        }
    }
    string ReplaceCharacter(string originalString, char charToReplace, char replacementChar)
    {
        return originalString.Replace(charToReplace, replacementChar);
    }
    private float generatePercentage(int shotsFired, int shotsMissed)
    {
        float percentage = 0;

        percentage = (shotsFired - shotsMissed);
        percentage = percentage / shotsFired;
        percentage = percentage * 100;

        if (percentage < 0 || percentage.ToString().ToLower().Contains("nan"))
        {
            percentage = 0;
        }
        //print("RE: Percente is : " + percentage);
        return percentage;
    }

    private void saveScoreScreenCapture()
    {
        if (Scoring.ammo_setting == "Live" && udpSendOnceFlag == 0)
        {
            isSaveImage = true;
            if (isSaveImage)
            {
                sendEndless("saveImage");
            }

            udpSendOnceFlag = 1;
        }

        if (sim_ammo_setting.ToLower().Contains("laser") && udpSendOnceFlag == 0)
        {
            print("Saving Unity Screen capture");

            udpSendOnceFlag = 1;
        }

    }
    void sceneIndoorShoot(RaycastHit hit)
    {

        int nbrOfLane = 3;
        try
        {
            //Debug.Log("Target hit is:" + hit.transform.name);   
            Scoring.logs = "Target hit is:" + hit.transform.name;
            Scoring.writeLog("Target hit is:" + hit.transform.name);
            //print("I AM IN...");
            //Debug.Log("Variable name:" + nameof(nbrOfLane));
            if (hit.transform.name.Contains("target") || hit.transform.name.ToLower().Contains("bullet"))
            {
                GameObject impactGo;
                if (activeScene.ToLower().Contains("moving"))
                {
                    impactGo = Instantiate(bullethole, new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.identity, hit.transform);
                }
                else
                {
                   impactGo = Instantiate(bullethole, new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.identity);
                }
                bullethole.transform.position = hit.transform.position;
                //impactGo.transform.name = "bullet" + hit.transform.name;
                bulletHoles.Add(impactGo);
                //(new Vector3(90,hit.normal.y,hit.normal.z)));
                //Destroy(impactGo, 2f);

                if(Scoring.ammo_setting == "Live")
                {
                    Destroy(impactGo); //only on a live Simulator
                }

                if (hit.transform.name.ToLower().Contains("bullet"))
                {
                    Debug.Log("We hit a bullet!!!!");
                    Scoring.writeLog("We hit a bullet!!!!");
                }
                if (hit.transform.name.ToLower().Contains("_1"))
                {
                    bulletPoint1.Add(new Vector2(hit.point.x, hit.point.y));
                    //print("Just added:" + bulletPoint1.ToArray().ToString());
                    //Scoring.logs += "Just added:" + bulletPoint1.ToArray().ToString();
                    gameObject.GetComponent<Scoring>().GetPoints(bulletPoint1, 0, hit.transform.name);
                    Debug.Log("Number of bullets shots lane[1]:" + bulletPoint1.Count + "  @ hit.point" + hit.point.ToString());
                }
                else if (hit.transform.name.ToLower().Contains("_2"))
                {
                    bulletPoint2.Add(new Vector2(hit.point.x, hit.point.y));
                    Scoring.logs += "Just added:" + bulletPoint1.ToArray().ToString();
                    gameObject.GetComponent<Scoring>().GetPoints(bulletPoint2, 1, hit.transform.name);
                    Debug.Log("Number of bullets shots lane[2]:" + bulletPoint2.Count + "  @ hit.point" + hit.point.ToString());
                }
                else if (hit.transform.name.ToLower().Contains("_3"))
                {
                    bulletPoint3.Add(new Vector2(hit.point.x, hit.point.y));
                    Scoring.logs += "Just added:" + bulletPoint1.ToArray().ToString();
                    gameObject.GetComponent<Scoring>().GetPoints(bulletPoint3, 2, hit.transform.name);
                    Debug.Log("Number of bullets shots lane[3]:" + bulletPoint3.Count + "  @ hit.point" + hit.point.ToString());
                }
            }
            else if (activeScene.ToLower().Contains("bottle"))
            {
                //GameObject impactGo = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                //GameObject bottleMan = GameObject.FindGameObjectWithTag("bottleMan");
                //Destroy(impactGo, 0.35f);
                if (bottle.run)
                {
                    if (hit.transform.name.ToLower().Contains("bottle"))
                    {
                        bottleMan.GetComponent<bottle>().SendMessage("ApplyDamage", "shot");
                        Destroy(hit.transform.gameObject);
                    }
                    else
                    {
                        bottleMan.GetComponent<bottle>().SendMessage("ApplyDamage", "missed");
                    }
                }
            }
            else if (hit.transform.name.Contains("Swat"))
            {

                Component enemy = new Component();
                //hit.rigidbody.AddForce(-hit.normal * 2F);
                //hit.transform.gameObject.GetComponent<ShootController>().SendMessage("ApplyDamage", hit.transform.name);


                //GameObject impactGo = Instantiate(blood, hit.point, Quaternion.LookRotation(hit.normal));
                //Destroy(impactGo, 2f);

                //enemyshot++;
            }
            else if (hit.transform.name.Contains("M_Target"))
            {

                Component enemy = new Component();
                //hit.rigidbody.AddForce(-hit.normal * 2F);
                hit.transform.gameObject.GetComponent<BasicTargetMovement>().SendMessage("ApplyDamage", hit.transform.name);


                //GameObject impactGo = Instantiate(blood, hit.point, Quaternion.LookRotation(hit.normal));
                //Destroy(impactGo, 2f);

                //enemyshot++;
            }
        }
        catch (Exception ex)
        {
            Scoring.logs += "\n" + ex.Message + ":" + ex.StackTrace;
            Scoring.writeLog("Shooting sceneIndoorShoot Error:" + ex.StackTrace);
            Debug.LogError("Shooting sceneIndoorShoot Error:" + ex.StackTrace);
        }
    }
    private void ResetRange(List<GameObject> bulletHoles)
    {
        enemyshot = 0;
        civilianShot = 0;
        Scoring.elapsedTime = 0;
        foreach (GameObject impactGo in bulletHoles)
        {
            Destroy(impactGo);
        }
        Scoring.ResetRange();
        bulletPoint1.Clear();
        bulletPoint2.Clear();
        bulletPoint3.Clear();

    }
    private void LoadEnemySoldier(int x1, int x2, int z1, int z2)
    {
        if (activeScene.Contains("range") || activeScene == "bottleShooting")
        {
            numberOfEnemies = 10;
        }
        else
        {
            numberOfEnemies = Random.Range(3, 8);
        }


        if (activeScene.ToLower().Contains("forest"))
        {
            numberOfEnemies = numberOfEnemies * 2;
        }
       
        numberOfCivilians = Random.Range(3, 4);
        //numberOfEnemies += numberOfCivilians;
        for (int i = 0; i < numberOfCivilians; i++)
        {
            int x = Random.Range(x1, x2);
            int z = Random.Range(z1, z2);
            int gender = Random.Range(0, 2);
            //Debug.Log("Creating gender:" + gender);
            if (activeScene != "Outdoor_FOREST")
            {
                if (!activeScene.ToLower().Contains("forest"))
                {
                    //Instantiate(enemySoldier, new Vector3(x, 0, z), Quaternion.identity);
                    if (gender == 0)
                    {
                        Quaternion quat = Quaternion.identity;
                        quat.x = quat.x * Random.Range(0, 15);
                        quat.y = quat.y * Random.Range(0, 15);

                        if (activeScene == "Outdoor_containers")
                        {
                            Instantiate(oc_eric, new Vector3(x + 1, 0, z - 1), quat);
                        }
                        else
                        {
                            Instantiate(eric, new Vector3(x + 1, 0, z - 1), quat);
                        }

                    }
                    else
                    {

                        if (activeScene == "Outdoor_containers")
                        {
                            Quaternion quat = Quaternion.identity;
                            quat.x = quat.x * Random.Range(3, 15);
                            Instantiate(oc_alison, new Vector3(x + 1, 0, z - 1), quat);
                            Instantiate(oc_lilly, new Vector3(x + 4, 0, z - 1), quat);
                        }
                        else
                        {
                            Quaternion quat = Quaternion.identity;
                            quat.x = quat.x * Random.Range(3, 15);
                            Instantiate(alison, new Vector3(x + 1, 0, z - 1), quat);
                            Instantiate(lilly, new Vector3(x + 4, 0, z - 1), quat);
                        }

                    }
                }
            }


        }

        for (int i = 0; i < numberOfEnemies; i++)
        {
            int x = Random.Range(x1, x2);
            int z = Random.Range(z1, z2);
            int hostage = Random.Range(0, 3);
            int gender = Random.Range(0, 2);
            //Debug.Log("gender:" + gender);


            if (enemySelector == 1)
            {

                if (activeScene == "Outdoor_FOREST")
                {
                    Instantiate(Forest_enemySoldier, new Vector3(x + 10, 0, z - 2), Quaternion.identity);
                    //Instantiate(Forest_enemySoldier, new Vector3(x + 5, 0, z + 8), Quaternion.identity);
                }
                else
                {

                    if(enemySelectorFlip == 0)
                    {
                        Instantiate(enemySoldier, new Vector3(x, 0, z), Quaternion.identity);
                        enemySelectorFlip = 1;
                    }
                    else
                    {
                        Instantiate(enemySoldier2, new Vector3(x, 0, z), Quaternion.identity);
                        enemySelectorFlip = 0;
                    }

                }

                //Forest_enemySoldier
            }

            if (hostage == 1 && activeScene != "Outdoor_FOREST")
            {
                if (gender == 0)
                {

                    if (activeScene == "outdoor_containers")
                    {
                        Instantiate(oc_eric, new Vector3(x + 1, 0, z - 1), Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(eric, new Vector3(x + 1, 0, z - 1), Quaternion.identity);
                    }
                }
                else
                {
                    //Instantiate(alison, new Vector3(x + 1, 0, z - 1), Quaternion.identity);
                    //Instantiate(lilly, new Vector3(x + 1, 0, z - 1), Quaternion.identity);

                    if (activeScene == "outdoor_containers")
                    {
                        Instantiate(oc_alison, new Vector3(x + 1, 0, z - 1), Quaternion.identity);
                        Instantiate(oc_lilly, new Vector3(x + 4, 0, z - 1), Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(alison, new Vector3(x + 1, 0, z - 1), Quaternion.identity);
                        Instantiate(lilly, new Vector3(x + 4, 0, z - 1), Quaternion.identity);
                    }
                }

            }
        }
    }
    private void SaveCalibration()
    {
        if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.f1Key.isPressed)
        {
            Debug.Log(" New X:" + xPos + " and New Y:" + yPos);
            CalibratePoint1.x = (int)xPos;
            CalibratePoint1.y = (int)yPos;
        }
        if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.f2Key.isPressed)
        {
            Debug.Log(" New X2:" + xPos + " and New Y2:" + yPos);
            CalibratePoint2.x = (int)xPos;
            CalibratePoint2.y = (int)yPos;
        }
    }
    private void ApplyDamage(string tagged)
    {
        Debug.Log("Main Player was hit by " + tagged + ". Remaining live(s)=" + mainPlayerLives);

        if (!GameOver)
        {
            ShootCondition = Random.Range(1, shootingPrecision);
            if (ShootCondition == 2)
            {
                mainPlayerLives = mainPlayerLives - 1;
                sendEndless("hit");
            }
            if (shootingTimeOut <= 0f)
            {
                mainPlayerLives = 0;
            }

        }
        if (GameOver)
        {
            StopTraining();
        }
        //print ("Number Of Hits:" + NumberOfHits);
    }
    void gameOver()
    {
        if (numBlockesHit == totalNumBlockes)
        {
            GameOver = true;
            //print("The Game is Over...");
            StopTraining();
        }


        if (mainPlayerLives <= 4)
        {
            ColorDynamic.SetActive(true);
        }
        if (shootingTimeOut <= 50)
        {
            if (!activeScene.ToLower().Contains("range") || !activeScene.ToLower().Contains("bottle") || !activeScene.ToLower().Contains("block"))
            {
                //TimeLeft.enabled = true;
                //TimeLeft.text = "Time Left: " + shootingTimeOut.ToString("0");
            }

        }
        if (mainPlayerLives <= 0f || shootingTimeOut <= 0f || numEnemies <= 0f )
        {

            if (activeScene == "Indoor_range" || activeScene == "Outdoor_range" || activeScene == "bottleShooting")
            {
                GameOver = false;
            }
            else
            {
                GameOver = true;
            }
        }
        else if (mainPlayerLives > 0f || shootingTimeOut > 0f || numEnemies > 0f)
        {
            if(activeScene != "BlockShooting")
            {
                GameOver = false;
            }
        }
    }
    private void Savescore()
    {
        Debug.Log("Saving:" + calibrationPath + " points:" + CalibratePoint1.x.ToString() + "," + CalibratePoint1.y.ToString() + ":" + CalibratePoint2.x.ToString() + "," + CalibratePoint2.y.ToString());
        System.IO.File.WriteAllText(@calibrationPath, "Newpoints:" + CalibratePoint1.x.ToString() + "," + CalibratePoint1.y.ToString() + ":" + CalibratePoint2.x.ToString() + "," + CalibratePoint2.y.ToString());
    }
    //bruces additions
    public void init()
    {
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        //print("UDPSend.init()");
        // get configuration details
        //stress_vest_url = configuration.ConfigAllUrls["stress_vest_url"];
        //Debug.Log("stress_vest_url is " + stress_vest_url);
        IP = "192.168.0.118";
        IP = stress_vest_url;
        Sportnum = 55554;
        //----------------------------
        // Sending
        //----------------------------
        //remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), Sportnum);
        udpClientS = new UdpClient();
        // status
        //print("Sending to " + IP + " : " + Sportnum);
        //print("Testing: nc -lu " + IP + " : " + Sportnum);

        if(activeScene.ToLower().Contains("Crouching"))
        {
            
        }
    }
    void init2()
    {
        udpClient = new UdpClient(portnum);
        udpClientImg = new UdpClient(portnumImg);
        remoteEndPoint = null;
        udpClientS = new UdpClient();
        IP = "192.168.0.118";
        Sportnum = 55554;
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), Sportnum);

    } 
    void initTargetLanes()
    {
        //lane 1
        point_lane_1.Add("point1,0");
        point_lane_1.Add("point2,0");
        point_lane_1.Add("point3,0");

        //lane 2
        point_lane_2.Add("point1,0");
        point_lane_2.Add("point2,0");
        point_lane_2.Add("point3,0");

        //lane 2
        point_lane_3.Add("point1,0");
        point_lane_3.Add("point2,0");
        point_lane_3.Add("point3,0");

        if (activeScene.ToLower().Contains("1lane"))
        {
            admin_Lane1_HitTimeTxt.text = "0 sec";
            admin_Lane1_HitShotsTxt.text = "0";
            admin_Lane1_ScoreTxt.text = "0";

            //admin_Lane1_TotalTimeTxt.text = TestConditionsManager.total_test_time.ToString("0.0") + " sec";
        }
        if (activeScene.ToLower().Contains("2lane"))
        {
            admin_Lane1_HitTimeTxt.text = "0 sec";
            admin_Lane1_HitShotsTxt.text = "0";
            admin_Lane1_ScoreTxt.text = "0";
            admin_Lane2_HitTimeTxt.text = "0 sec";
            admin_Lane2_HitShotsTxt.text = "0";
            admin_Lane2_ScoreTxt.text = "0";

            //admin_Lane1_TotalTimeTxt.text = TestConditionsManager.total_test_time.ToString("0.0") + " sec";
            //admin_Lane2_TotalTimeTxt.text = TestConditionsManager.total_test_time.ToString("0.0") + " sec";
        }
        if (activeScene.ToLower().Contains("3lane"))
        {
            admin_Lane1_HitTimeTxt.text = "0 sec";
            admin_Lane1_HitShotsTxt.text = "0";
            admin_Lane1_ScoreTxt.text = "0";
            admin_Lane2_HitTimeTxt.text = "0 sec";
            admin_Lane2_HitShotsTxt.text = "0";
            admin_Lane2_ScoreTxt.text = "0";
            admin_Lane3_HitTimeTxt.text = "0 sec";
            admin_Lane3_HitShotsTxt.text = "0";
            admin_Lane3_ScoreTxt.text = "0";

            //admin_Lane1_TotalTimeTxt.text = TestConditionsManager.total_test_time.ToString("0.0") + " sec";
            //admin_Lane2_TotalTimeTxt.text = TestConditionsManager.total_test_time.ToString("0.0") + " sec";
            //admin_Lane3_TotalTimeTxt.text = TestConditionsManager.total_test_time.ToString("0.0") + " sec";
        }
        if (activeScene.ToLower().Contains("4lane"))
        {
            admin_Lane1_HitTimeTxt.text = "0 sec";
            admin_Lane1_HitShotsTxt.text = "0";
            admin_Lane1_ScoreTxt.text = "0";
            admin_Lane2_HitTimeTxt.text = "0 sec";
            admin_Lane2_HitShotsTxt.text = "0";
            admin_Lane2_ScoreTxt.text = "0";
            admin_Lane3_HitTimeTxt.text = "0 sec";
            admin_Lane3_HitShotsTxt.text = "0";
            admin_Lane3_ScoreTxt.text = "0";
            admin_Lane4_HitTimeTxt.text = "0 sec";
            admin_Lane4_HitShotsTxt.text = "0";
            admin_Lane4_ScoreTxt.text = "0";

            //admin_Lane1_TotalTimeTxt.text = TestConditionsManager.total_test_time.ToString("0.0") + " sec";
            //admin_Lane2_TotalTimeTxt.text = TestConditionsManager.total_test_time.ToString("0.0") + " sec";
            //admin_Lane3_TotalTimeTxt.text = TestConditionsManager.total_test_time.ToString("0.0") + " sec";
            //admin_Lane4_TotalTimeTxt.text = TestConditionsManager.total_test_time.ToString("0.0") + " sec";
        }
    }
    private void sendString(string message)
    {
        if (udpClientS.Available > 0)
        {
            // Daten mit der UTF8-Kodierung in das Bin�rformat kodieren.
            byte[] data = Encoding.UTF8.GetBytes(message);
            // Den message zum Remote-Client senden.
            udpClientS.Send(data, data.Length, remoteEndPoint);
        }
        else
        {
            print("error in connection");
        }
    }
    public void sendEndless(string testStr)
    {
        IP = UDP_ClientIP_Address;
        //print("UDP IP IS " + IP);
        Sportnum = 55552;
        udpClientS = new UdpClient();
        //----------------------------
        // Sending
        //----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), Sportnum);

        // Daten mit der UTF8-Kodierung in das Bin�rformat kodieren.
        byte[] data = Encoding.UTF8.GetBytes(testStr);
        //print("sending " + testStr + " to " + remoteEndPoint);
        // Den message zum Remote-Client senden.
        udpClientS.Send(data, data.Length, remoteEndPoint);
    }
    private void MultipleScreens()
    {
        //Debug.Log(Display.displays.Length + " is/are connected");
        //Display.displays[0] is the primary and is always on
        //checking if any aditional screans are connected and activating them
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }

    }
    private void InitialiseVariables()
    {
        enemyshot = 0;
        numberOfEnemies = 0;
        mainPlayerLives = 10;
        numberOfCivilians = 0;
        shotsFired = 0;
        civilianShot = 0;

    }
    private void ResetStaticVariables()
    {
        StaticVariableManager.isTrainingPause = trainingPaused;
        StaticVariableManager.isStopTraining = false;
        StaticVariableManager.isTimeOut = false;
        StaticVariableManager.isLane1BoardSet = false;
        StaticVariableManager.isLane2BoardSet = false;
        StaticVariableManager.isLane1TargetMissed = false;
        StaticVariableManager.isLane2TargetMissed = false;
        StaticVariableManager.isLane3TargetMissed = false;
        StaticVariableManager.isResetingPoints = false;
        StaticVariableManager.OMPScoreSent = false;
        StaticVariableManager.isScoreLanesReady = false;
        StaticVariableManager.isScoreDataSet = false;

        //Car Variables
        StaticVariableManager.redCarPoints = 0;
        StaticVariableManager.blueCarPoints = 0;
        StaticVariableManager.redCarCompletedPoints = 0;
        StaticVariableManager.blueCarCompletedPoints = 0;
        StaticVariableManager.redCarMoveComplete = false;
        StaticVariableManager.blueCarMoveComplete = false;
        StaticVariableManager.redCarMoveSteps = 0;
        StaticVariableManager.blueCarMoveSteps = 0;
        StaticVariableManager.redCarPoints = 0;
        StaticVariableManager.blueCarPoints = 0;
        StaticVariableManager.redCarCompletedPoints = 0;
        StaticVariableManager.blueCarCompletedPoints = 0;
        StaticVariableManager.carTargetSpeed = 1;
        StaticVariableManager.flickerSet = false;
        StaticVariableManager.isRedShotFired = false;
        StaticVariableManager.isBlueShotFired = false;

        //Sequence Number
        StaticVariableManager.currentGoal = 0;
        StaticVariableManager.currentProgress = 0;
        StaticVariableManager.correctNumberHits = 0;
        StaticVariableManager.wrongNumberHits = 0;

        //Color Sequence
        StaticVariableManager.isColorDisplayed = false;
        StaticVariableManager.sequenceCreated = false;
        StaticVariableManager.correctColorHits = 0;
        StaticVariableManager.wrongColorHits = 0;


        //Main Variables
        point_lane_1 = new List<string>();
        point_lane_2 = new List<string>();
        point_lane_3 = new List<string>();
        lane_1_impact_list = new List<GameObject>();
        lane_2_impact_list = new List<GameObject>();
        lane_3_impact_list = new List<GameObject>();
        numLane1ShotsFired = 0;
        numLane2ShotsFired = 0;
        numLane3ShotsFired = 0;
        numLane1ShotsMissed = 0;
        numLane2ShotsMissed = 0;
        numLane3ShotsMissed = 0;
        lane1ActiveTimeCounter = 0f;
        lane2ActiveTimeCounter = 0f;
        lane3ActiveTimeCounter = 0f;
        lane1TrainingScorePoints = 0;
        lane2TrainingScorePoints = 0;
        lane3TrainingScorePoints = 0;
    }
    private void LoadSounds()
    {
        handgunSound = gameObject.AddComponent<AudioSource>();
        handgunSound.clip = handgunAudio;
        baloonPopSound = gameObject.AddComponent<AudioSource>();
        baloonPopSound.clip = baloonPopAudio;
        ladyScreamSound = gameObject.AddComponent<AudioSource>();
        ladyScreamSound.clip = ladyscreamAudio;
        humanScreamSound = gameObject.AddComponent<AudioSource>();
        humanScreamSound.clip = humanScreamAudio;
        randomTalkSound = gameObject.AddComponent<AudioSource>();
        randomTalkSound.clip = randomTalkAudio;

        CitySound = gameObject.AddComponent<AudioSource>();
        CitySound.clip = CityAudio;

        CitySound2 = gameObject.AddComponent<AudioSource>();
        CitySound2.clip = CityAudio2;

        ForestSound = gameObject.AddComponent<AudioSource>();
        ForestSound.clip = ForestAudio;

        ForestSound2 = gameObject.AddComponent<AudioSource>();
        ForestSound2.clip = ForestAudio2;

        mallTalkSound = gameObject.AddComponent<AudioSource>();
        mallTalkSound.clip = mallTalkAudio;

        buzzerSound = gameObject.AddComponent<AudioSource>();
        buzzerSound.clip = buzzerAudio;

        gunCockingSound = gameObject.AddComponent<AudioSource>();
        gunCockingSound.clip = gunCockingAudio;
    }
    public void startTimer()
    {
        if (buzzerFlag == false)
        {
            buzzerFlag = true;
            buzzerTime = 6;
            startTime = float.Parse(TimeLimit.text);
            rangeTime = TimeLimit.text;
        }
        else
        {
            buzzerFlag = false;
        }

        if (activeScene.ToLower().Contains("range"))
        {
            try
            {
                Trainees.GetComponent<GetTrainees>().destroyDropdownObjects();
                //getTrainees.destroy = true;
                //getTrainees.destroyDropdownObjects();
            }
            catch (System.Exception)
            {
                //throw;
            }
        }

    }
    public void okBtn()
    {
        mainPanel.SetActive(false);
        shooterScorePanel.SetActive(false);

        SceneManager.LoadScene("SceneManager");
    }
    public void Pause()
    {
        if (GameOver)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Reset_Time();
        }

    }
    public void Reset_Time()
    {
        Time.timeScale = 1f;
    }
    public void GoToMainMenu()
    {
        if(activeScene.ToLower().Contains("calibration"))
        {
            SceneManager.LoadScene("MainMenu");
        }
        else if (activeScene.ToLower().Contains("indoor_range"))
        {
            SceneManager.LoadScene("SceneManager");
        }
        else
        {
            Time.timeScale = 1f;
            StaticVariableManager.startCountDown = false;
            SceneManager.LoadScene("TestConditionSetting");
            //Destroy(player.gameObject);
        }
    }
    void OnGUI()
    {
        //Create a horizontal Slider that controls volume levels. Its highest value is 1 and lowest is 0
        VolumeValue = GUI.HorizontalSlider(new Rect(25, 25, 200, 60), VolumeValue, 0.0F, 1.0F);
        //Makes the volume of the Audio match the Slider value
        CitySound2.volume = VolumeValue;
    }
    public void HandleLaneInputData(int val)
    {
        //print("RE: Selected " + val);
        switch (val)
        {
            case 1:
                laneSelected = 1;
                break;

            case 2:
                laneSelected = 2;
                break;

            case 3:
                laneSelected = 3;
                break;

            case 4:
                laneSelected = 4;
                break;

        }

        //Update Score
        if (activeScene.ToLower().Contains("cyclic"))
        {
            manageCyclicTargetsScoring();
        }
        else if (activeScene.ToLower().Contains("risingplate"))
        {
            manageRisingPlatesScoring();
        }
        else if (activeScene.ToLower().Contains("baloon"))
        {
            manageBaloonScoring();
        }

    }
    public void handleSplitTimeDisplay()
    {
        if(!isSplitTimeOpen)
        {
            shooterScorePanel.SetActive(false);
            splitTimePanel.SetActive(true);
            isSplitTimeOpen = true;
        }
        else
        {
            shooterScorePanel.SetActive(true);
            splitTimePanel.SetActive(false);
            isSplitTimeOpen = false;
        }
    }
    public void handleScoreDisplay()
    {
        if (isScoreOpen)
        {
            ScoreCanvas.SetActive(false);
            isScoreOpen = false;
        }
        else //not open
        {
            ScoreCanvas.SetActive(true);
            isScoreOpen = true;
        }
    }

    private bool GetUiElementsClicked()
    {
        bool process_result = false;

        if(!activeScene.ToLower().Contains("calibration"))
        {
            process_result = check_canvas(process_result, adminCanvas);
            process_result = check_canvas(process_result, countDownCanvas);
        }

        return process_result;
    }

    private bool check_canvas(bool status, GameObject curr_canvas)
    {
        ui_canvas = curr_canvas;
        ui_raycaster = ui_canvas.GetComponent<GraphicRaycaster>();
        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>();

        click_data.position = Mouse.current.position.ReadValue();
        click_results.Clear();

        ui_raycaster.Raycast(click_data, click_results);

        foreach (RaycastResult result in click_results)
        {
            GameObject ui_element = result.gameObject;
            //print("Element pressed is: " + ui_element.name);

            status = true;
            if (ui_element.name.ToLower().Contains("reset") || ui_element.name.ToLower().Contains("pause") || ui_element.name.ToLower().Contains("start")
                || ui_element.name.ToLower().Contains("exit") || ui_element.name.ToLower().Contains("distance manager") || ui_element.name.ToLower().Contains("target control panel")
                || ui_element.name.ToLower().Contains("button background") || ui_element.name.ToLower().Contains("end") || ui_element.name.ToLower().Contains("score") 
                || ui_element.name.ToLower().Contains("distance manager") || ui_element.name.ToLower().Contains("next") || ui_element.name.ToLower().Contains("prev")
                || ui_element.name.ToLower().Contains("control"))
            {
                status = true;
                //print("Element pressed is: " + ui_element.name);
            }
        }

        return status;
    }

    private void ResetUIPanels()
    {
        //Set Canvas
        adminCanvas = GameObject.FindGameObjectWithTag("AdminCanva");
        countDownCanvas = GameObject.FindGameObjectWithTag("StartCanvas");

        //Set UI variable data
        if (!activeScene.ToLower().Contains("calibration"))
        {
            //distanceSimulatorPanel.SetActive(true);

            if (Scoring.ammo_setting.ToLower().Contains("live") || Scoring.ammo_setting.ToLower().Contains("infrared"))
            {
                lane1Timer.color = Color.white;
                lane2Timer.color = Color.white;
                lane3Timer.color = Color.white;

                lane1TraineeNameDisplay.color = Color.white;
                lane2TraineeNameDisplay.color = Color.white;
                lane3TraineeNameDisplay.color = Color.white;
            }
            else
            {
                lane1Timer.color = Color.black;
                lane2Timer.color = Color.black;
                lane3Timer.color = Color.black;

                lane1TraineeNameDisplay.color = Color.black;
                lane2TraineeNameDisplay.color = Color.black;
                lane3TraineeNameDisplay.color = Color.black;
            }

        }

    }

}
//START CODE
/*if(activeScene.ToLower().Contains("basic"))
{
    if(Scoring.simulation_type == "training")
    {
        numTargetInput.text = TestConditionsManager.numTargets.ToString();
        if(activeScene.ToLower().Contains("fallingplat"))
        {
            if (int.Parse(numTargetInput.text) >= 20)
            {
                numTargetInput.text = "20";
            }
        }
    }
    else
    {
        numTargetInput.text = TestConditionsManager.numTargets.ToString();
    }

    num_targets_input = int.Parse(numTargetInput.text);   //initialize condition after setup
}*/

//UPDATE CODE
/*if(activeScene.ToLower().Contains("basic"))
{
    if(numTargetInput.text != num_targets_input.ToString())
    {
        if (Input.GetKey(KeyCode.KeypadEnter ) || Input.GetKey(KeyCode.Return))//Save scoring
        {
            //print("RE: num targets is " + numTargetInput.text);
            //num_targets_input = int.Parse(numTargetInput.text);
            num_targets_input = int.Parse(numTargetInput.text);

           //print("RE: Out");
            //inputChangeSwitch = 1;
        }

    }
}*/

//DUELING TREE SCORING CODE
/*if (hit.transform.name.ToLower().Contains("plate") || hit.transform.name.ToLower().Contains("target "))
                    {
                        //platesHit++;
                        if (Scoring.simulation_type == "training")
                        {
                            targetText.text = "Hit Plate" + platesHit;
                            if (platesHit + (shotsFired - platesHit) >= num_targets_input)
                            {
                                targetsFinished = true;
                                targetFinished = true;
                                StopTraining();
                            }
                        }
                        else
                        {
                            //targetText.text = "Hit Plate" + platesHit;
                            if (platesHit >= num_targets_input)
                            {
                                targetsFinished = true;
                                targetFinished = true;
                                StopTraining();
                            }
                        }
                    }
                    else
                    {
                        platesHit--;
                        if (platesHit <= 0)
                        {
                            platesHit = 0;
                        }
                    }*/

//TARGET POPUP SCORING CODE
/* if ((activeScene == "BasicTargetPopUpFreeShoot" || activeScene == "BasicTargetPopUpOneHand") && Scoring.ammo_setting == "Live")
                {
                    WallTargetControl.requestTargetReview();

                }
                else
                {
                    WallTargetControl.requestTargetReview();
                    totPlates = TestConditionsManager.totalAllowedHitShots;

                    float plateRatio = (platesHit / totPlates);
                    float percResult = plateRatio * 100;
                    float scoreResult = percResult;

                    if (scoreResult < 0)
                    {
                        scoreResult = 0;
                    }

                    Tname.text += " : " + name;
                    Location.text = "Time: " + timeActiveCounter.ToString("0") + " Seconds ";
                    enemiesKilled.text = "Targets Hit: " + (headShots + bodyShots);
                    civiliansShot.text = "Shots Missed : " + (shotsFired - (headShots + bodyShots));


                    plate_score = platesHit; // omp score

                    if (activeScene == "Outdoor_FOREST" || activeScene.Contains("range"))
                    {
                        civiliansShot.enabled = false;
                    }

                    //score.text = "SCORE : " + scoreResult.ToString("0");

                    if (timeFinished)
                    {
                        Result.text = "Mission Failed, Timeout!";
                    }
                    else if (ammaFinished)
                    {
                        Result.text = "Mission Failed, Out of ammo!";
                    }
                    else if (targetsFinished && !ammaFinished && !timeFinished)
                    {
                        Result.text = "Mission Succesful!";
                    }
                    
                    ScorePanel.SetActive(true);
                    score.enabled = true;
                    print("RE: IN HERE...");
                }*/