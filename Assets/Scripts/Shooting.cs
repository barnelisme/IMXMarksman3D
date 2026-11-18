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
using System.Linq;

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
    private bool isCurrShotProcessed = false;

    string calibrationPath = "";
    string DelaySavePath = "";
    public string[] guns;
    public string gunsTextPath = "";

    //UI Button Variables
    public GameObject ui_canvas;
    public GameObject adminCanvas;
    public GameObject countDownCanvas;
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
    public AudioSource HuntingForestSound;
    public AudioSource BirdHuntingSound;

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
    public AudioClip daytime_HuntingForestAudio;
    public AudioClip nighttime_HuntingForestAudio;
    public AudioClip birdHuntingIdleAudio;
    public AudioClip birdHuntingEvadeAudio;

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
    public GameObject backgroundImageDisplay;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public GameObject impactEffect_TargetHit;
    public GameObject impactEffect_PlateHit;
    public GameObject blood;
    public GameObject bullethole;
    public GameObject bullethole_2;
    public GameObject bullethole_3;
    private List<GameObject> bulletHoles = new List<GameObject>();
    UdpClient udpClient;
    UdpClient udpClientImg;
    public int portnum = 22222;
    public int portnumImg = 22223;
    // Store up to 10 recent coordinates
    private List<Vector2> recentPoints = new List<Vector2>();
    private const int MaxRecentPoints = 10;

    Boolean FlagBulletRx = false;
    int CountShotFired = 0;
    Vector2Int CalibratePoint1 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint2 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint3 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint4 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint5 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint6 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint7 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint8 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint9 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint10 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint11 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint12 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint13 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint14 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint15 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint16 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint17 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint18 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint19 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint20 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint21 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint22 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint23 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint24 = new Vector2Int(0, 0);
    Vector2Int CalibratePoint25 = new Vector2Int(0, 0);

    float eX = 0, eY = 0;
    float minX = 0, maxX = 0, minY = 0, maxY = 0;

    private List<Vector2Int> MultiCalibPoints = new List<Vector2Int> { new Vector2Int(0, 0) };


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
    private int lane1ShotsFired = 0;
    private int lane2ShotsFired = 0;
    private int lane3ShotsFired = 0;

    static public int totalNumberOfEnemies = 0;
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
    public int remainingNumEnemies = 0;
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
    public static float lane1TimeActiveCounter = 0;
    public static float lane2TimeActiveCounter = 0;
    public static float lane3TimeActiveCounter = 0;
    //static string timeactive = timeActiveCounter.ToString();

    public TextMeshProUGUI ShootDelayTxt; //Shooting delay TXT gameobject variables
    float ShootDelayTxt_timeout = 1;      //shooting delay display timeout
    float incrementDelay = 0.0f;          //ShootDelay button increment delay
    float delaySetting = 0;
    bool isShootDelay = false;            //button pressed code trigger

    float shootTextTimeOut = 1;
    public float lane1ShootDelayTimer = 0;
    public float lane2ShootDelayTimer = 0;
    public float lane3ShootDelayTimer = 0;
    bool lane1ShotDelayActive = false;
    bool lane2ShotDelayActive = false;
    bool lane3ShotDelayActive = false;
    public GameObject aimingImage;

    //File stream variables
    public static string delayFileData;
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
    string sim_ammo_setting = "";
    public GameObject assistPanel;
    public TextMeshProUGUI assistText;
    static bool isAssistOpen;
    bool f1Pressed = false;
    float f1OpenHoldCountDown = 0.1f;
    float f1CloseHoldCountDown = 0.1f;
    float f1HoldSetValue = 0.1f;
    private float gunResetTimer = 4f;
    private float setGunTimer = 4f;

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
    static bool isAccMode = true;
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
    int num_set_shots = 10;
    int num_targets_to_hit = 10000;
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
    private bool isCIScene = false;

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
    public List<string> lane1ResponseTime = new List<string>(); //Split time memory
    public List<string> lane2ResponseTime = new List<string>();
    public List<string> lane3ResponseTime = new List<string>();
    public List<string> lane4ResponseTime = new List<string>();
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
    float responseOverflowValue = -0.025f;
    float lane1ResponseTimeCounter = 0;  //response time counter
    float lane2ResponseTimeCounter = 0;
    float lane3ResponseTimeCounter = 0;
    float lane4ResponseTimeCounter = 0;
    bool startLane1Count = false;  //timer flag
    bool startLane2Count = false;
    bool startLane3Count = false;
    bool startLane4Count = false;
    bool startLane1ResponseCount = true;  //timer flag
    bool startLane2ResponseCount = true;
    bool startLane3ResponseCount = true;
    bool startLane4ResponseCount = true;
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
    public TextMeshProUGUI admin_Lane1_CorrectHitsTxt;
    public TextMeshProUGUI admin_Lane1_IncorrectHitsTxt;
    public TextMeshProUGUI admin_Lane2_Header;
    public TextMeshProUGUI admin_Lane2_ShotsFiredTxt;
    public TextMeshProUGUI admin_Lane2_ShotsMissedTxt;
    public TextMeshProUGUI admin_Lane2_HitTimeTxt;
    public TextMeshProUGUI admin_Lane2_HitShotsTxt;
    public TextMeshProUGUI admin_Lane2_ScoreTxt;
    public TextMeshProUGUI admin_Lane2_PercentageTxt;
    public TextMeshProUGUI admin_Lane2_CorrectHitsTxt;
    public TextMeshProUGUI admin_Lane2_IncorrectHitsTxt;
    public TextMeshProUGUI admin_Lane3_Header;
    public TextMeshProUGUI admin_Lane3_ShotsFiredTxt;
    public TextMeshProUGUI admin_Lane3_ShotsMissedTxt;
    public TextMeshProUGUI admin_Lane3_HitTimeTxt;
    public TextMeshProUGUI admin_Lane3_HitShotsTxt;
    public TextMeshProUGUI admin_Lane3_ScoreTxt;
    public TextMeshProUGUI admin_Lane3_PercentageTxt;
    public TextMeshProUGUI admin_Lane3_CorrectHitsTxt;
    public TextMeshProUGUI admin_Lane3_IncorrectHitsTxt;
    public TextMeshProUGUI admin_Lane4_Header;
    public TextMeshProUGUI admin_Lane4_ShotsFiredTxt;
    public TextMeshProUGUI admin_Lane4_ShotsMissedTxt;
    public TextMeshProUGUI admin_Lane4_HitTimeTxt;
    public TextMeshProUGUI admin_Lane4_HitShotsTxt;
    public TextMeshProUGUI admin_Lane4_ScoreTxt;
    public TextMeshProUGUI admin_Lane4_PercentageTxt;
    public TextMeshProUGUI admin_Lane4_CorrectHitsTxt;
    public TextMeshProUGUI admin_Lane4_IncorrectHitsTxt;
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
    public TextMeshProUGUI shooter_Lane1_CorrectHitsTxt;
    public TextMeshProUGUI shooter_Lane1_IncorrectHitsTxt;
    public TextMeshProUGUI shooter_Lane2_Header;
    public TextMeshProUGUI shooter_Lane2_SplitTime_Header;
    public TextMeshProUGUI shooter_Lane2_ShotsFiredTxt;
    public TextMeshProUGUI shooter_Lane2_ShotsMissedTxt;
    public TextMeshProUGUI shooter_Lane2_HitTimeTxt;
    public TextMeshProUGUI shooter_Lane2_HitShotsTxt;
    public TextMeshProUGUI shooter_Lane2_ScoreTxt;
    public TextMeshProUGUI shooter_Lane2_PercentageTxt;
    public TextMeshProUGUI shooter_Lane2_CorrectHitsTxt;
    public TextMeshProUGUI shooter_Lane2_IncorrectHitsTxt;
    public TextMeshProUGUI shooter_Lane3_Header;
    public TextMeshProUGUI shooter_Lane3_SplitTime_Header;
    public TextMeshProUGUI shooter_Lane3_ShotsFiredTxt;
    public TextMeshProUGUI shooter_Lane3_ShotsMissedTxt;
    public TextMeshProUGUI shooter_Lane3_HitTimeTxt;
    public TextMeshProUGUI shooter_Lane3_HitShotsTxt;
    public TextMeshProUGUI shooter_Lane3_ScoreTxt;
    public TextMeshProUGUI shooter_Lane3_PercentageTxt;
    public TextMeshProUGUI shooter_Lane3_CorrectHitsTxt;
    public TextMeshProUGUI shooter_Lane3_IncorrectHitsTxt;
    public TextMeshProUGUI shooter_Lane4_Header;
    public TextMeshProUGUI shooter_Lane4_SplitTime_Header;
    public TextMeshProUGUI shooter_Lane4_ShotsFiredTxt;
    public TextMeshProUGUI shooter_Lane4_ShotsMissedTxt;
    public TextMeshProUGUI shooter_Lane4_HitTimeTxt;
    public TextMeshProUGUI shooter_Lane4_HitShotsTxt;
    public TextMeshProUGUI shooter_Lane4_ScoreTxt;
    public TextMeshProUGUI shooter_Lane4_PercentageTxt;
    public TextMeshProUGUI shooter_Lane4_CorrectHitsTxt;
    public TextMeshProUGUI shooter_Lane4_IncorrectHitsTxt;


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

    [Header("Video Training")]
    public GameObject deadShader;

    void Awake()
    {
        // ⭐ PERFORMANCE BOOST: Unlock frame rate and sync with camera
        // Application.targetFrameRate = 120;      // 120 FPS target
        // QualitySettings.vSyncCount = 0;         // Disable VSync
        // Time.fixedDeltaTime = 0.008f;           // Match 120 FPS physics
        
        // Debug.Log("Performance: 120 FPS mode enabled!");
    }
    
    public void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;



        try
        {
            SetUIPanels();
            ResetStaticVariables();
            SetDataDirectory();
            loadBackgroundColor();
            ReadFiles();
            SendUDPSignal();
            // sendSceneName();

            if (activeScene.ToLower().Contains("range_"))
            {
                resetIndoorPapers();
            }

            bullethole_2 = bullethole_3;

            if (activeScene.ToLower().Contains("suspect") || activeScene.ToLower().Contains("baloon"))
            {

            }
            else
            {
                // startLane1ResponseCount = true;
                // startLane2ResponseCount = true;
                // startLane3ResponseCount = true;
                // startLane4ResponseCount = true;
            }

            if (activeScene.ToLower().Contains("hunting") && !activeScene.ToLower().Contains("directforesthunting"))
            {
                // player.GetComponent<FirstPersonController>().enabled = false;
            }

            loadTraineeNames();
            trainingPaused = false;



            trainingStarted = true;
            isReset = true;
            startTraining = false;
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
                delaySetting = 0.3f;
                // print("RE: Delay is " + delayFileData);
                // delaySetting = float.Parse(delayFileData);
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

            remainingNumEnemies = 2;
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
            Debug.Log("Error:" + e.Message);
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


            else if (activeScene == "Mall" || activeScene == "Underground_parking" || activeScene.ToLower().Contains("restaurant") || (activeScene.ToLower().Contains("outdoor") || activeScene.ToLower().Contains("rooms") && !activeScene.ToLower().Contains("range")) && !activeScene.ToLower().Contains("hunting"))
            {
                is3DScene = true;
                //Byte[] sendBytes = Encoding.ASCII.GetBytes("Mall");
                //udpClient.Send(sendBytes, "Indoor".Length, "127.0.0.1", 48901);
                LoadEnemySoldier(175, 230, 425, 482);
            }
            else if (activeScene.ToLower().Contains("closeupsuspect"))
            {
                is3DScene = true;
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
            Debug.Log("Error:" + e.Message);
        }


        try
        {
            if (activeScene == "OpenPlain" || activeScene == "MiniCity")
            {
                CitySound2.loop = true;
                CitySound2.PlayDelayed(1);
            }
            else if (activeScene.ToLower().Contains("foresthunting"))
            {
                HuntingForestSound.loop = true;
                HuntingForestSound.PlayDelayed(1);
            }
            else if(activeScene.ToLower().Contains("birdhunting"))
            {
                BirdHuntingSound.loop = true;
                BirdHuntingSound.PlayDelayed(1);
            }

            if(aimingImage!= null) aimingImage.SetActive(false);

            if (DropDown.scene_type == "3D" && TestConditionsManager.typeOfHunt.ToLower().Contains("search"))
            {
                aimingImage.SetActive(true);
            }

            if (activeScene.ToLower().Contains("basic"))
            {
                if (sim_ammo_setting.ToLower().Contains("live") || sim_ammo_setting.ToLower().Contains("laser"))
                {
                    num_set_shots = TestConditionsManager.numBullets * TestConditionsManager.numMegs;
                    num_targets_to_hit = TestConditionsManager.totalAllowedHitShots;
                }
            }
            else if(activeScene.ToLower().Contains("hunting"))
            {
                num_set_shots = TestConditionsManager.totalAllowedHitShots; //initialize condition after setup
            }
            else 
            {
                num_set_shots = TestConditionsManager.numBullets * TestConditionsManager.numMegs;
                num_targets_to_hit = TestConditionsManager.totalAllowedHitShots;
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

            //print("RE: try 3 complete...");
        }
        catch (Exception e)
        {
             Debug.Log("Error:" + e.Message);
        }

        // print("Num targets: " + TestConditionsManager.numLaneTargets);
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
                ui_mouse_reset = true;
                // print("UI Pressed: " + ui_mouse_pressed);
            }
            else
            {
                ui_mouse_pressed = false;
            }

            // trainingPaused = StaticVariableManager.isPaused;
            if (!trainingPaused && !ui_mouse_pressed)
            {
                // print("Running...");
                if (sendUDPSimSetting && countDownStart.start_training)
                {
                    // SendUDPSignal();
                    //print("Sent!");
                    sendUDPSimSetting = false;
                }

                if (!is3DScene)
                {

                    if (buzzerFlag)
                    {

                        buzzerTime -= Time.deltaTime;
                        buzzerText.text = "Stand by:" + ((int)buzzerTime).ToString();
                        // print("BuzzerTime==" + buzzerTime.ToString());
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
                    startTimer();
                }
                if ((Keyboard.current.escapeKey.isPressed && Keyboard.current.enterKey.isPressed) || startTime <= 0)//Start/stop timer
                {
                    chrono = false;
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
                    remainingNumEnemies = (totalNumberOfEnemies - enemyshot);
                    //life.text = "Life: " + mainPlayerLives;
                    //scoreMain.text = "Threat:" + numEnemies + " civilians " + numberOfCivilians;
                }

                if (isShootState == true)
                {
                    shootingTimeOut -= Time.deltaTime * 1;
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
                    // aimingImage.SetActive(false);
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
                        else if(activeScene.ToLower().Contains("closeup"))
                        {
                            StartActiveCount();
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

                //Mouse click shoot method
                ManageMouseShoot();

                //Split time counters
                manageSplitAndReactionTime();

                // Stop Conditions
                StopConditions();

                // Manage Gunshot cooldown
                if (activeScene.ToLower().Contains("hunting"))
                {
                    ManageGunShotCooldown();
                }
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

            manageGlobalTrainingVariable();

        }
        catch (Exception ex)
        {
            Scoring.logs += "\n" + ex.Message + ":" + ex.StackTrace;
            Debug.LogError("Shooting Update:" + ex.StackTrace);
        }


        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 vector3 = new Vector3(mousePosition.x, mousePosition.y, -1);
        Vector3 cameraVector = Camera.main.ScreenToWorldPoint(vector3);

        //UDP Receive Function
        udpDataProcessing(); // if(countDownStart.start_training) 

        //|Call acc after receiving udp
        loadAccValues();
        if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.rightAltKey.isPressed)
        {
            // isAccMode = false;
        }
        if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.leftAltKey.isPressed)
        {
            isAccMode = true;
            if (activeScene == "CalibrationTest")
            {
                saveAcc();
                Calibration.isAccSet = false;
            }
        }

    }

    private void ManageGunShotCooldown()
    {
        if (StaticVariableManager.isGunFired)
        {
            gunResetTimer -= Time.deltaTime;
            if (gunResetTimer <= 0f)
            {
                StaticVariableManager.isGunFired = false;
                gunResetTimer = setGunTimer;
            }
        }
    }

    private void ManageMouseShoot()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && xPos == -1f && yPos == -1f && !ui_mouse_pressed)
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

    private void SetDataDirectory() // Verbatim String Literals
    {
        calibrationPath = Path.Combine(StaticVariableManager.main_data_directory, "Calibration.txt");
    }

    private void manageSplitAndReactionTime()
    {

        if(countDownStart.start_training && !StaticVariableManager.isStopTraining)
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

            if(activeScene.Contains("VideoTraining"))
            {
                //Response Time
                if (startLane1ResponseCount && StaticVariableManager.isStartVideoTime)
                {
                    lane1ResponseTimeCounter += Time.deltaTime * 1;
                    // print("Reaction Time: " + lane1ResponseTimeCounter);
                }
            }
            else
            {
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
            /*Color newColor;
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

            */

            backgroundImageDisplay = GameObject.FindGameObjectWithTag("BackgroundImage");
            if (backgroundImageDisplay != null)
            {
                Material displayMat = backgroundImageDisplay.GetComponent<MeshRenderer>().material;
                BackgroundSetImageManager.SetBackgroundMaterialImage(StaticVariableManager.currBackgroundImagePos, displayMat);
            }

        }
        else
        {
            backgroundImageDisplay = GameObject.FindGameObjectWithTag("BackgroundImage");
            if (backgroundImageDisplay != null)
            {
                backgroundImageDisplay.gameObject.SetActive(false);
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

    private void StopConditions()
    {
        manageMouseClickStopCondition();

        // IPSC Extended Stop condition
        ManageIPSCExtStopCondition();

        //Hunting scenes Stop Conditions
        manageHuntTrainingStopCondition();

        //Lane shooting Stop Condition
        manageLaneTarinigStopCondition();

        //Suspect Shoot stop condition
        manageSuspectShootStopCondition();

        //Video Training Stop Condition
        ManageVideoStopCondition();
    }

    private void manageSuspectShootStopCondition()
    {
        if(StaticVariableManager.isStopTraining)
        {
            
            trainingPaused = true;
            StopTraining();
        }
    }

    private void ManageIPSCExtStopCondition()
    {
        switch (StaticVariableManager.numIPSCTargetOption)
        {
            case 1:
                if (StaticVariableManager.isTarget1Complete)
                {
                    //print("Test: Point reached");
                    StopTraining();

                    StaticVariableManager.isTarget1Complete = false;
                    StaticVariableManager.isTarget2Complete = false;
                    StaticVariableManager.isTarget3Complete = false;
                }
                break;
            case 2:
                if (StaticVariableManager.isTarget1Complete && StaticVariableManager.isTarget2Complete)
                {
                    //print("Test: Point reached");
                    StopTraining();

                    StaticVariableManager.isTarget1Complete = false;
                    StaticVariableManager.isTarget2Complete = false;
                    StaticVariableManager.isTarget3Complete = false;
                }
                break;
            case 3:
                if (StaticVariableManager.isTarget1Complete && StaticVariableManager.isTarget2Complete && StaticVariableManager.isTarget3Complete)
                {
                    //print("Test: Point reached");
                    StopTraining();

                    StaticVariableManager.isTarget1Complete = false;
                    StaticVariableManager.isTarget2Complete = false;
                    StaticVariableManager.isTarget3Complete = false;
                }
                break;
        }
    }
    
    private void manageHuntTrainingStopCondition()
    {

        if (activeScene.ToLower().Contains("hunting"))
        {
            if (activeScene.ToLower().Contains("direct"))
            {
                if (StaticVariableManager.totalTargetAnimalsKilled >= num_set_shots)
                {
                    targetsFinished = true;
                    targetFinished = true;
                    StopTraining();
                }
            }
            else if (activeScene.ToLower().Contains("avoid"))
            {
                if (StaticVariableManager.totalTargetAnimalsKilled + StaticVariableManager.totalTargetCasualtiesKilled >= num_set_shots)
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
            if (Mouse.current.rightButton.wasPressedThisFrame && !Keyboard.current.leftShiftKey.isPressed && !activeScene.ToLower().Contains("indoor_range"))
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


        manageShotCount();
    }

    private void ManagerScenarioStopCondition()
    {
        if(activeScene.Contains("2LaneDuelingTree"))
        {
            if(StaticVariableManager.duelingTreeComplete)
            {
                StopTraining();
            }
        }
        else if(activeScene.Contains("DecisionPTShoot")) // activeScene.Contains("TargetRecognition")
        {
            if (StaticVariableManager.isTRTTrainingComplete)
            {
                StopTraining();
            }
        }
        else if(activeScene.Contains("VideoTraining"))
        {
            if(StaticVariableManager.isVideoTrainingComplete || !StaticVariableManager.isStartVideoTime)
            {
                StopTraining();
            }
        }
    }

    private void ManageVideoStopCondition()
    {
        if (StaticVariableManager.isVideoTrainingComplete)
        {
            StopTraining();
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
            timerText.text = "Timer: " + TestConditionsManager.total_test_time;
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
        try
        {
            const int MAX_PACKETS_PER_FRAME = 45;
            const float MAX_TIME_MS = 1.5f; // Hard cap to protect Unity frame time

            int packetsProcessed = 0;
            float startTimeMs = Time.realtimeSinceStartup * 1000f;

            while (udpClient.Available > 0 && packetsProcessed < MAX_PACKETS_PER_FRAME)
            {
                // ⛔ Time safety guard
                if ((Time.realtimeSinceStartup * 1000f - startTimeMs) > MAX_TIME_MS)
                    break;

                packetsProcessed++;

                IPEndPoint remoteEP = null;
                byte[] data;

                try
                {
                    data = udpClient.Receive(ref remoteEP);
                }
                catch (SocketException)
                {
                    break;
                }

                // Fast reject (no allocation)
                if (data.Length < 6 || data[0] != 'C')
                    continue;

                string message = Encoding.ASCII.GetString(data);

                // Handle non-COORD commands
                if (!message.StartsWith("COORD,", StringComparison.Ordinal))
                {
                    if (message.IndexOf("exit test calibration", StringComparison.OrdinalIgnoreCase) >= 0 &&
                        activeScene.IndexOf("calibration", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        SceneManager.LoadScene("Calib_Acc_Page");
                    }
                    continue;
                }

                // Manual parse instead of Split()
                int firstComma = message.IndexOf(',', 6);
                if (firstComma < 0)
                    continue;

                if (!float.TryParse(message.AsSpan(6, firstComma - 6),
                                    NumberStyles.Float,
                                    CultureInfo.InvariantCulture,
                                    out float parsedX))
                    continue;

                if (!float.TryParse(message.AsSpan(firstComma + 1),
                                    NumberStyles.Float,
                                    CultureInfo.InvariantCulture,
                                    out float parsedY))
                    continue;

                // Duplicate rejection (fast)
                if (StaticVariableManager.IsDuplicate(parsedX, parsedY))
                    continue;

                StaticVariableManager.AddPoint(parsedX, parsedY);

                xPos = parsedX;
                yPos = parsedY;

                if (!countDownStart.start_training)
                    continue;

                Shoot();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("UDP Processing Error: " + e);
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

    public void StartActiveCount() // Lane timer display
    {
        ManageShotsDelayTimers();        

        if (!StaticVariableManager.isResetingPoints)
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
                if (!lane1TargetsComplete && !StaticVariableManager.isLane1TargetMissed) //&& startLane1Count
                {
                    if (!lane1SrikeOut)
                    { 
                        lane1TimeActiveCounter += Time.deltaTime;
                        lane1Timer.text = " " + lane1TimeActiveCounter.ToString("0.0");
                        //lane1ActiveTimeCounter = timeActiveCounter;
                    }
                }
                if (!lane2TargetsComplete && !StaticVariableManager.isLane2TargetMissed) //&& startLane2Count
                {
                    if (!lane2SrikeOut)
                    {
                        lane2TimeActiveCounter += Time.deltaTime;
                        lane2Timer.text = " " + lane2TimeActiveCounter.ToString("0.0");
                        //lane2ActiveTimeCounter = timeActiveCounter;
                    }
                }
                if (!lane3TargetsComplete && !StaticVariableManager.isLane3TargetMissed) //&& startLane3Count
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
            else if (is3DScene || activeScene.ToLower().Contains("suspect"))
            { //3D Scenarios
                lane1TimeActiveCounter += Time.deltaTime;
                lane1Timer.text = " " + lane1TimeActiveCounter.ToString("0.0");
            }
        }
    }

    private void ManageShotsDelayTimers()
    {
        //shoot delay decrement called every frame
        if(lane1ShotDelayActive)
        {
            lane1ShootDelayTimer -= Time.deltaTime * 1;
            if (lane1ShootDelayTimer <= 0)
            {
                lane1ShootDelayTimer = delaySetting;
                lane1ShotDelayActive = false;
            }
        }

        if (lane2ShotDelayActive)
        {
            lane2ShootDelayTimer -= Time.deltaTime * 1;
            if (lane2ShootDelayTimer <= 0)
            {
                lane2ShootDelayTimer = delaySetting;
                lane2ShotDelayActive = false;
            }
        }

        if (lane3ShotDelayActive)
        {
            lane3ShootDelayTimer -= Time.deltaTime * 1;
            if (lane3ShootDelayTimer <= 0)
            {
                lane3ShootDelayTimer = delaySetting;
                lane3ShotDelayActive = false;
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

        range = 10000;
    }
    public void SendUDPSignal()
    {
        UDPSetup.FindLocalIP(); // Set the Global IP string incase it has changed

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
            }
            else
            {
                sendEndless("Laser");
            }

            // sendEndless("Simulate");
        }

    }

    private void ReadFiles()
    {
        try
        {
            UDP_ClientIP_Address = FileManager.ReadSetting("UDP IP");
            delayFileData = FileManager.ReadSetting("Delay");
            Scoring.ammo_setting = FileManager.ReadSetting("Ammo Setting");

            if (delayFileData.Contains("."))
            {
                delayFileData.Replace(".", ",");
            }

        }
        catch (Exception e)
        {
            print("Error retrieving zero file:" + e);
            throw new Exception("First login");
        }

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
            FileManager.SaveSetting("Delay", delaySetting.ToString("0.0"));
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
            FileManager.SaveSetting("Delay", delaySetting.ToString("0.0"));
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

        // If video training and step back procedure is running. Do Not SHoot.
        if(activeScene.Contains("Video") && StaticVariableManager.initializeStepBack)
        {
            return; 
        }

        if ((startTraining == true && isResetDelay == false && trainingStarted == true && trainingPaused == false
            && StaticVariableManager.isResetingPoints == false && StaticVariableManager.isEnded == false) || activeScene == "CalibrationTest")
        {

            ApplyDelay(); // Apply delay
            ApplySoundChanges();  // Manage sounds

            RaycastHit hit;
            isShootState = true;
            totalshots.text = "Shots Fired : ";
            totalshots.text += shotsFired;
            if (is3DScene) shotsFired++; // This variable is only used in 3D
            if (is3DScene && activeScene.ToLower().Contains("hunting"))
                StaticVariableManager.isGunFired = true;


            // Apply point calculation
            ApplyShotCoordinate();

            // Handle Raycasting
            if (Physics.Raycast(ray, out hit, range)) // if Hit
            {
                // StaticVariableManager.isGunFired = true;
                /////////////////////Manage Shot Delay////////////////////
                if (lane1ShotDelayActive && is3DScene) return;
                if ((lane1ShotDelayActive || lane1TargetsComplete) && (hit.transform.name.ToLower().StartsWith("1") || hit.transform.tag.ToLower().Contains("lane1misscollider"))) return;
                if ((lane2ShotDelayActive || lane2TargetsComplete) && (hit.transform.name.ToLower().StartsWith("2") || hit.transform.tag.ToLower().Contains("lane2misscollider"))) return;
                if ((lane3ShotDelayActive || lane3TargetsComplete) && (hit.transform.name.ToLower().StartsWith("3") || hit.transform.tag.ToLower().Contains("lane3misscollider"))) return;
                ManageShootDelay(hit);
                ///////////////////////////////////////////////////////////////////////////////

                // Handle hit impact
                ApplyImpact(hit);

                // print(hit.transform.name);
                // is3DScene = true;
                if (is3DScene)
                {
                    Debug.Log("Hit=" + hit.transform.name);
                    sceneOpenPlainShoot(hit);
                }
                else if (activeScene.ToLower().Contains("closeupsuspect"))
                {
                    sceneOpenPlainShoot(hit);
                }
                else if (activeScene.ToLower().Contains("basic"))
                {
                    sceneOpenPlainShoot(hit);
                }
                else
                {
                    Debug.Log("Hit=" + hit.transform.name);
                    sceneIndoorShoot(hit);
                }

            }

            HandlExtMissedShots(hit);
            DisplayLaneShotsFired();

            ManagerScenarioStopCondition(); // Ensure this is called after all tarining data is update!!
            SaveCalibration();
        }

    }

    private void ApplySoundChanges()
    {

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

        if (Scoring.ammo_setting.ToLower().Contains("laser") && StaticVariableManager.gunShotSetting)
        {
            // print("Shooing Laser Sound");
            handgunSound.Play();
            handgunSound.volume = 0.75f;
        }
    }

    private void ApplyDelay()
    {
        if (lane1ShootDelayTimer <= 0)
        {
            //set delay;
            lane1ShootDelayTimer = delaySetting;
        }
    }

    private void ApplyShotCoordinate()
    {
        if (xPos == -1f && yPos == -1f)
        {
            ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); //new Vector3(xPos, yPos, -1)
        }
        else
        {
            isAccMode = true; // Set by default
            if (isAccMode == false)
            {
                DrawMatrixLines1(new Vector2Int((int)xPos, (int)yPos));
            }
            else if (isAccMode == true)
            {
                // NEW, CORRECT CALL: Use Vector2 instead of Vector2Int.
                DrawMatrixLines(new Vector2(xPos, yPos));
            }
        }
    }

    private void ApplyImpact(RaycastHit hit)
    {
        if (activeScene.ToLower().Contains("calibration"))
        {
            //print("RE: Shooting Calib impact");
            GameObject impactGo = Instantiate(acc_Bullet, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGo, 0.1f);
        }
        else
        {
            if (hit.transform.tag.ToLower().Contains("soldier") || hit.transform.tag.ToLower().Contains("civilian"))
            {
                // Do Nothing for now
                HandleHitImpact(hit);
            }
            else
            {

                if (Scoring.ammo_setting == "Live")
                {
                    if (pointInRadiusG == false && isCalibrate == false)
                    {
                        HandleHitImpact(hit);
                    }
                    else if (isCalibrate == true)
                    {
                        GameObject impactGo = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                        Destroy(impactGo, 0.4f);
                    }
                }
                else
                {
                    HandleHitImpact(hit);
                }
            }

        }
    }

    private void HandleHitImpact(RaycastHit hit)
    {
        // print("Playing hit impact: " + LayerMask.LayerToName(hit.collider.gameObject.layer));

        if (impactEffect_TargetHit != null && (LayerMask.LayerToName(hit.collider.gameObject.layer) == "animal" ||
            LayerMask.LayerToName(hit.collider.gameObject.layer) == "civilian"))
        {
            GameObject impactGo = Instantiate(impactEffect_TargetHit, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGo, 0.4f);
        }
        else if (StaticVariableManager.smokeImpactSetting && impactEffect_PlateHit != null )
        {
            if(activeScene.ToLower().Contains("3dclaypigeon") || activeScene.ToLower().Contains("hunting") 
                || sim_ammo_setting.ToLower().Contains("live"))
            {
                if((hit.transform.tag == "target"))
                {
                    GameObject impactGo = Instantiate(impactEffect_PlateHit, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impactGo, 0.4f);
                }
            }
            else
            {
                GameObject impactGo = Instantiate(impactEffect_PlateHit, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGo, 0.4f);
            }

        }
        else
        {
            GameObject impactGo = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGo, 0.4f);
        }
    }

    private void manageShotCount()
    {
        if (sim_ammo_setting.ToLower().Contains("laser") && activeScene != "CalibrationTest")
        {
            if (shotsFired >= (TestConditionsManager.numBullets * TestConditionsManager.numMegs))
            {
                ammaFinished = true;
                StopTraining();
            }
            // print("In Laser");
        }
        else if (sim_ammo_setting == "Live" && DropDown.scene_type == "3D")
        {
            if (shotsFired >= num_set_shots && !activeScene.ToLower().Contains("hunting") && !activeScene.ToLower().Contains("calibration"))
            {
                GameOver = true;
                targetsFinished = true;
                targetFinished = true;
                StopTraining();
            }
            // print("In Live");
        }

    }

    private void ManageShootDelay(RaycastHit hit)
    {

        if (Scoring.ammo_setting.ToLower().Contains("laser"))
        {
            if(is3DScene)
            {
                lane1ShotDelayActive = true;
                //print("3D Scene");
            }
            else if (hit.transform.name.StartsWith("1") || hit.transform.tag.ToLower().Contains("lane1misscollider"))
            {
                lane1ShotDelayActive = true;
                //print("Lane 1");
            }
            else if (hit.transform.name.StartsWith("2") || hit.transform.tag.ToLower().Contains("lane2misscollider"))
            {
                lane2ShotDelayActive = true;
                //print("Lane 2");
            }
            else if (hit.transform.name.StartsWith("3") || hit.transform.tag.ToLower().Contains("lane3misscollider"))
            {
                lane3ShotDelayActive = true;
                //print("Lane 3");
            }
        }
    }

    private void DrawMatrixLines(Vector2 normalizedPoint)
    {
        /************* Variables to draw bullets on targets ********************/
        // The input 'normalizedPoint' is now (0.0 to 1.0)
        // print("Drawing: " + normalizedPoint.x + "," + normalizedPoint.y);

        // Convert normalized point to screen pixel coordinates
        float finalX_Screen = normalizedPoint.x * Screen.width;
        // NOTE: normalizedPoint.y is already Unity-inverted (0.0=bottom, 1.0=top) from Python.
        float finalY_Screen = normalizedPoint.y * Screen.height;

        // Create the final screen position Vector
        Vector2 finalScreenPos = new Vector2(finalX_Screen, finalY_Screen);

        // --- OLD LOGIC REMOVED: All Quadrant/Sub-Quadrant Checks and CalibratePoint logic ---
        // The goal of the old logic was to map a distorted point (x,y) to a screen coordinate (eX, eY)
        // using interpolation between calibration points. Since Python provides the FINAL, CORRECTED (0-1) point, 
        // we just need the direct scaling.

        // Assign the new, corrected screen coordinates to your output variables
        eX = finalScreenPos.x;
        eY = finalScreenPos.y;

        // Merge ball points (SetBallPoint should use eX, eY)
        SetBallPoint(eX, eY, minX, maxX, minY, maxY);

        // Since the point is normalized, it will always be within the 0-1 range.
        // The check for "Point is outside the calibration grid!" is now unnecessary 
        // unless you want to check if it's outside a smaller sub-region.
        if (eX < 0 || eX > Screen.width || eY < 0 || eY > Screen.height)
        {
            print("ERROR: Final Screen Point is outside screen boundaries!");
            return; // Ignore points outside the screen
        }

    }
    // NOTE: All the SetQuadrantOne, SetQuadrantTwo, SetQuadrantThree, SetQuadrantFour functions 
    // are now redundant for **drawing the shot marker** and can be safely removed or commented out.
    // If they are used for other side effects (like triggering other events), you will need to re-integrate 
    // that logic using the new normalized xPos/yPos values.

    private void SetQuadrantOne(Vector2 point)
    {

        /////////////////////// QUADRANT 1 (CPS: [2 - TOP RIGHT] [5 - BOTTOM LEFT] [7 - CENTER])
        ///////////////////////////////////////////////
        /////////////// SUB QUADRANTS 1.1 ////////////
        // Quadrant 1.1.1 (Top-Right)
        if (point.x >= CalibratePoint13.x && point.x <= CalibratePoint2.x &&
                 point.y <= CalibratePoint2.y && point.y >= CalibratePoint13.y)
        {
            print("Point is in Quadrant 1.1.1 (Top-Right)");

            (eX, eY) = CalculateExEy(point, CalibratePoint13.x, CalibratePoint2.x, CalibratePoint13.y, CalibratePoint2.y);

            minX = Screen.width - Screen.width / 8; maxX = Screen.width;
            minY = Screen.height - Screen.height / 8; maxY = Screen.height;
        }

        // Quadrant 1.1.2 (Top-Left)
        else if (point.x >= CalibratePoint7.x && point.x <= CalibratePoint13.x &&
            point.y <= CalibratePoint2.y && point.y >= CalibratePoint13.y)
        {
            print("Point is in Quadrant 1.1.2 (Top-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint7.x, CalibratePoint13.x, CalibratePoint13.y, CalibratePoint2.y);

            minX = Screen.width - Screen.width / 4; maxX = Screen.width - Screen.width / 8;
            minY = Screen.height - Screen.height / 8; maxY = Screen.height; // minY = Screen.height - Screen.height / 2
        }

        // Quadrant 1.1.3 (Bottom-Left)
        else if (point.x >= CalibratePoint7.x && point.x <= CalibratePoint13.x &&
                 point.y <= CalibratePoint13.y && point.y >= CalibratePoint7.y)
        {
            print("Point is in Quadrant 1.1.3 (Bottom-Left)");
            (eX, eY) = CalculateExEy(point, CalibratePoint7.x, CalibratePoint13.x, CalibratePoint7.y, CalibratePoint13.y);

            minX = Screen.width - Screen.width / 4; maxX = Screen.width - Screen.width / 8;
            minY = Screen.height - Screen.height / 4; maxY = Screen.height - Screen.height / 8;
        }

        // Quadrant 1.1.4 (Bottom-Right)
        else if (point.x >= CalibratePoint13.x && point.x <= CalibratePoint2.x &&
                 point.y <= CalibratePoint13.y && point.y >= CalibratePoint7.y)
        {
            print("Point is in Quadrant 1.4 (Bottom-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint13.x, CalibratePoint2.x, CalibratePoint7.y, CalibratePoint13.y);

            minX = Screen.width - Screen.width / 8; maxX = Screen.width;
            minY = Screen.height - Screen.height / 4; maxY = Screen.height - Screen.height / 8;
        }

        //////////////////////////////////////////////
        /////////////// SUB QUADRANTS 1.2 ////////////
        // Quadrant 1.2.1 (Top-Right)
        else if (point.x >= CalibratePoint12.x && point.x <= CalibratePoint7.x &&
                 point.y <= CalibratePoint2.y && point.y >= CalibratePoint12.y)
        {
            print("Point is in Quadrant 1.2.1 (Top-Right)");

            (eX, eY) = CalculateExEy(point, CalibratePoint12.x, CalibratePoint7.x, CalibratePoint12.y, CalibratePoint2.y);

            minX = Screen.width - (Screen.width / 8) * 3; maxX = Screen.width - (Screen.width / 4);
            minY = Screen.height - Screen.height / 8; maxY = Screen.height;
        }

        // Quadrant 1.2.2 (Top-Left)
        else if (point.x >= CalibratePoint5.x && point.x <= CalibratePoint12.x &&
            point.y <= CalibratePoint2.y && point.y >= CalibratePoint12.y)
        {
            print("Point is in Quadrant 1.2.2 (Top-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint5.x, CalibratePoint12.x, CalibratePoint12.y, CalibratePoint2.y);

            minX = Screen.width / 2; maxX = Screen.width - (Screen.width / 8) * 3;
            minY = Screen.height - Screen.height / 8; maxY = Screen.height; // minY = Screen.height - Screen.height / 2
        }

        // Quadrant 1.2.3 (Bottom-Left)
        else if (point.x >= CalibratePoint5.x && point.x <= CalibratePoint12.x &&
                 point.y <= CalibratePoint12.y && point.y >= CalibratePoint7.y)
        {
            print("Point is in Quadrant 1.2.3 (Bottom-Left)");
            (eX, eY) = CalculateExEy(point, CalibratePoint5.x, CalibratePoint12.x, CalibratePoint7.y, CalibratePoint12.y);

            minX = Screen.width / 2; maxX = Screen.width - (Screen.width / 8) * 3;
            minY = Screen.height - Screen.height / 4; maxY = Screen.height - Screen.height / 8;
        }

        // Quadrant 1.2.4 (Bottom-Right)
        else if (point.x >= CalibratePoint12.x && point.x <= CalibratePoint7.x &&
                 point.y <= CalibratePoint12.y && point.y >= CalibratePoint7.y)
        {
            print("Point is in Quadrant 1.2.4 (Bottom-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint12.x, CalibratePoint7.x, CalibratePoint7.y, CalibratePoint12.y);

            minX = Screen.width - (Screen.width / 8) * 3; maxX = Screen.width - (Screen.width / 4);
            minY = Screen.height - Screen.height / 4; maxY = Screen.height - Screen.height / 8;
        }

        //////////////////////////////////////////////
        /////////////// SUB QUADRANTS 1.3 ////////////
        // Quadrant 1.3.1 (Top-Right)
        else if (point.x >= CalibratePoint16.x && point.x <= CalibratePoint7.x &&
                 point.y <= CalibratePoint7.y && point.y >= CalibratePoint16.y)
        {
            print("Point is in Quadrant 1.3.1 (Top-Right)");

            (eX, eY) = CalculateExEy(point, CalibratePoint16.x, CalibratePoint7.x, CalibratePoint16.y, CalibratePoint7.y);

            minX = Screen.width - (Screen.width / 8) * 3; maxX = Screen.width - (Screen.width / 4);
            minY = Screen.height - (Screen.height / 8) * 3; maxY = Screen.height - (Screen.height / 4);
        }

        // Quadrant 1.3.2 (Top-Left)
        else if (point.x >= CalibratePoint5.x && point.x <= CalibratePoint16.x &&
            point.y <= CalibratePoint7.y && point.y >= CalibratePoint16.y)
        {
            print("Point is in Quadrant 1.3.2 (Top-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint5.x, CalibratePoint16.x, CalibratePoint16.y, CalibratePoint7.y);

            minX = Screen.width / 2; maxX = Screen.width - (Screen.width / 8) * 3;
            minY = Screen.height - (Screen.height / 8) * 3; maxY = Screen.height - Screen.height / 4; // minY = Screen.height - Screen.height / 2
        }

        // Quadrant 1.3.3 (Bottom-Left)
        else if (point.x >= CalibratePoint5.x && point.x <= CalibratePoint16.x &&
                 point.y <= CalibratePoint16.y && point.y >= CalibratePoint5.y)
        {
            print("Point is in Quadrant 1.3.3 (Bottom-Left)");
            (eX, eY) = CalculateExEy(point, CalibratePoint5.x, CalibratePoint16.x, CalibratePoint5.y, CalibratePoint16.y);

            minX = Screen.width / 2; maxX = Screen.width - (Screen.width / 8) * 3;
            minY = Screen.height / 2; maxY = Screen.height - (Screen.height / 8) * 3;
        }

        // Quadrant 1.3.4 (Bottom-Right)
        else if (point.x >= CalibratePoint16.x && point.x <= CalibratePoint7.x &&
                 point.y <= CalibratePoint16.y && point.y >= CalibratePoint5.y)
        {
            print("Point is in Quadrant 1.3.4 (Bottom-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint16.x, CalibratePoint7.x, CalibratePoint5.y, CalibratePoint16.y);

            minX = Screen.width - (Screen.width / 8) * 3; maxX = Screen.width - (Screen.width / 4);
            minY = Screen.height / 2; maxY = Screen.height - (Screen.height / 8) * 3;
        }

        //////////////////////////////////////////////
        /////////////// SUB QUADRANTS 1.4 ////////////
        // Quadrant 1.4.1 (Top-Right)
        else if (point.x >= CalibratePoint17.x && point.x <= CalibratePoint2.x &&
                 point.y <= CalibratePoint7.y && point.y >= CalibratePoint17.y)
        {
            print("Point is in Quadrant 1.4.1 (Top-Right)");

            (eX, eY) = CalculateExEy(point, CalibratePoint17.x, CalibratePoint2.x, CalibratePoint17.y, CalibratePoint7.y);

            minX = Screen.width - (Screen.width / 8); maxX = Screen.width;
            minY = Screen.height - (Screen.height / 8) * 3; maxY = Screen.height - (Screen.height / 4);
        }

        // Quadrant 1.4.2 (Top-Left)
        else if (point.x >= CalibratePoint7.x && point.x <= CalibratePoint17.x &&
            point.y <= CalibratePoint7.y && point.y >= CalibratePoint17.y)
        {
            print("Point is in Quadrant 1.4.2 (Top-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint7.x, CalibratePoint17.x, CalibratePoint17.y, CalibratePoint7.y);

            minX = Screen.width - Screen.width / 4; maxX = Screen.width - (Screen.width / 8);
            minY = Screen.height - (Screen.height / 8) * 3; maxY = Screen.height - Screen.height / 4; // minY = Screen.height - Screen.height / 2
        }

        // Quadrant 1.4.3 (Bottom-Left)
        else if (point.x >= CalibratePoint7.x && point.x <= CalibratePoint17.x &&
                 point.y <= CalibratePoint17.y && point.y >= CalibratePoint5.y)
        {
            print("Point is in Quadrant 1.4.3 (Bottom-Left)");
            (eX, eY) = CalculateExEy(point, CalibratePoint7.x, CalibratePoint17.x, CalibratePoint5.y, CalibratePoint17.y);

            minX = Screen.width - Screen.width / 4; maxX = Screen.width - (Screen.width / 8);
            minY = Screen.height / 2; maxY = Screen.height - (Screen.height / 8) * 3;
        }

        // Quadrant 1.4.4 (Bottom-Right)
        else if (point.x >= CalibratePoint17.x && point.x <= CalibratePoint2.x &&
                 point.y <= CalibratePoint17.y && point.y >= CalibratePoint5.y)
        {
            print("Point is in Quadrant 1.4.4 (Bottom-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint17.x, CalibratePoint2.x, CalibratePoint5.y, CalibratePoint17.y);

            minX = Screen.width - (Screen.width / 8); maxX = Screen.width;
            minY = Screen.height / 2; maxY = Screen.height - (Screen.height / 8) * 3;
        }


    }

    private void SetQuadrantTwo(Vector2 point)
    {

        /////////////////////// QUADRANT 2 (CPS: [1 - TOP LEFT] [5 - BOTTOM RIGHT] [6 - CENTER])
        ///////////////////////////////////////////////
        /////////////// SUB QUADRANTS 2.1 ////////////
        // Quadrant 2.1.1 (Top-Right)
        if (point.x >= CalibratePoint11.x && point.x <= CalibratePoint5.x &&
                 point.y <= CalibratePoint1.y && point.y >= CalibratePoint11.y)
        {
            print("Point is in Quadrant 2.1.1 (Top-Right)");

            (eX, eY) = CalculateExEy(point, CalibratePoint11.x, CalibratePoint5.x, CalibratePoint11.y, CalibratePoint1.y);

            minX = Screen.width - (Screen.width / 8) * 5; maxX = Screen.width / 2;
            minY = Screen.height - Screen.height / 8; maxY = Screen.height;
        }

        // Quadrant 2.1.2 (Top-Left)
        else if (point.x >= CalibratePoint6.x && point.x <= CalibratePoint11.x &&
            point.y <= CalibratePoint1.y && point.y >= CalibratePoint11.y)
        {
            print("Point is in Quadrant 2.1.2 (Top-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint6.x, CalibratePoint11.x, CalibratePoint11.y, CalibratePoint1.y);

            minX = Screen.width / 4; maxX = Screen.width - (Screen.width / 8) * 5;
            minY = Screen.height - Screen.height / 8; maxY = Screen.height; // minY = Screen.height - Screen.height / 2
        }

        // Quadrant 2.1.3 (Bottom-Left)
        else if (point.x >= CalibratePoint6.x && point.x <= CalibratePoint11.x &&
                 point.y <= CalibratePoint11.y && point.y >= CalibratePoint6.y)
        {
            print("Point is in Quadrant 2.1.3 (Bottom-Left)");
            (eX, eY) = CalculateExEy(point, CalibratePoint6.x, CalibratePoint11.x, CalibratePoint6.y, CalibratePoint11.y);

            minX = Screen.width / 4; maxX = Screen.width - (Screen.width / 8) * 5;
            minY = Screen.height - Screen.height / 4; maxY = Screen.height - Screen.height / 8;
        }

        // Quadrant 2.1.4 (Bottom-Right)
        else if (point.x >= CalibratePoint11.x && point.x <= CalibratePoint5.x &&
                 point.y <= CalibratePoint11.y && point.y >= CalibratePoint6.y)
        {
            print("Point is in Quadrant 2.1.4 (Bottom-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint11.x, CalibratePoint5.x, CalibratePoint6.y, CalibratePoint11.y);

            minX = Screen.width - (Screen.width / 8) * 5; maxX = Screen.width / 2;
            minY = Screen.height - Screen.height / 4; maxY = Screen.height - Screen.height / 8;
        }

        //////////////////////////////////////////////
        /////////////// SUB QUADRANTS 2.2 ////////////
        // Quadrant 2.2.1 (Top-Right)
        else if (point.x >= CalibratePoint10.x && point.x <= CalibratePoint6.x &&
                 point.y <= CalibratePoint1.y && point.y >= CalibratePoint10.y)
        {
            print("Point is in Quadrant 2.2.1 (Top-Right)");

            (eX, eY) = CalculateExEy(point, CalibratePoint10.x, CalibratePoint6.x, CalibratePoint10.y, CalibratePoint1.y);

            minX = Screen.width - (Screen.width / 8) * 7; maxX = (Screen.width / 4);
            minY = Screen.height - Screen.height / 8; maxY = Screen.height;
        }

        // Quadrant 2.2.2 (Top-Left)
        else if (point.x >= CalibratePoint1.x && point.x <= CalibratePoint10.x &&
            point.y <= CalibratePoint1.y && point.y >= CalibratePoint10.y)
        {
            print("Point is in Quadrant 2.2.2 (Top-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint1.x, CalibratePoint10.x, CalibratePoint10.y, CalibratePoint1.y);

            minX = 0; maxX = Screen.width - (Screen.width / 8) * 7;
            minY = Screen.height - Screen.height / 8; maxY = Screen.height; // minY = Screen.height - Screen.height / 2
        }

        // Quadrant 2.2.3 (Bottom-Left)
        else if (point.x >= CalibratePoint1.x && point.x <= CalibratePoint10.x &&
                 point.y <= CalibratePoint10.y && point.y >= CalibratePoint6.y)
        {
            print("Point is in Quadrant 2.2.3 (Bottom-Left)");
            (eX, eY) = CalculateExEy(point, CalibratePoint1.x, CalibratePoint10.x, CalibratePoint6.y, CalibratePoint10.y);

            minX = 0; maxX = Screen.width - (Screen.width / 8) * 7;
            minY = Screen.height - Screen.height / 4; maxY = Screen.height - Screen.height / 8;
        }

        // Quadrant 2.2.4 (Bottom-Right)
        else if (point.x >= CalibratePoint10.x && point.x <= CalibratePoint6.x &&
                 point.y <= CalibratePoint10.y && point.y >= CalibratePoint6.y)
        {
            print("Point is in Quadrant 2.2.4 (Bottom-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint10.x, CalibratePoint6.x, CalibratePoint6.y, CalibratePoint10.y);

            minX = Screen.width - (Screen.width / 8) * 7; maxX = (Screen.width / 4);
            minY = Screen.height - Screen.height / 4; maxY = Screen.height - Screen.height / 8;
        }

        //////////////////////////////////////////////
        /////////////// SUB QUADRANTS 2.3 ////////////
        // Quadrant 2.3.1 (Top-Right)
        else if (point.x >= CalibratePoint14.x && point.x <= CalibratePoint6.x &&
                 point.y <= CalibratePoint6.y && point.y >= CalibratePoint14.y)
        {
            print("Point is in Quadrant 2.3.1 (Top-Right)");

            (eX, eY) = CalculateExEy(point, CalibratePoint14.x, CalibratePoint6.x, CalibratePoint14.y, CalibratePoint6.y);

            minX = Screen.width - (Screen.width / 8) * 7; maxX = (Screen.width / 4);
            minY = Screen.height - (Screen.height / 8) * 3; maxY = Screen.height - (Screen.height / 4);
        }

        // Quadrant 2.3.2 (Top-Left)
        else if (point.x >= CalibratePoint1.x && point.x <= CalibratePoint14.x &&
            point.y <= CalibratePoint6.y && point.y >= CalibratePoint14.y)
        {
            print("Point is in Quadrant 2.3.2 (Top-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint1.x, CalibratePoint14.x, CalibratePoint14.y, CalibratePoint6.y);

            minX = 0; maxX = Screen.width - (Screen.width / 8) * 7;
            minY = Screen.height - (Screen.height / 8) * 3; maxY = Screen.height - Screen.height / 4; // minY = Screen.height - Screen.height / 2
        }

        // Quadrant 2.3.3 (Bottom-Left)
        else if (point.x >= CalibratePoint1.x && point.x <= CalibratePoint14.x &&
                 point.y <= CalibratePoint14.y && point.y >= CalibratePoint5.y)
        {
            print("Point is in Quadrant 2.3.3 (Bottom-Left)");
            (eX, eY) = CalculateExEy(point, CalibratePoint1.x, CalibratePoint14.x, CalibratePoint5.y, CalibratePoint14.y);

            minX = 0; maxX = Screen.width - (Screen.width / 8) * 7;
            minY = Screen.height / 2; maxY = Screen.height - (Screen.height / 8) * 3;
        }

        // Quadrant 2.3.4 (Bottom-Right)
        else if (point.x >= CalibratePoint14.x && point.x <= CalibratePoint6.x &&
                 point.y <= CalibratePoint14.y && point.y >= CalibratePoint5.y)
        {
            print("Point is in Quadrant 2.3.4 (Bottom-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint14.x, CalibratePoint6.x, CalibratePoint5.y, CalibratePoint14.y);

            minX = Screen.width - (Screen.width / 8) * 7; maxX = (Screen.width / 4);
            minY = Screen.height / 2; maxY = Screen.height - (Screen.height / 8) * 3;
        }

        //////////////////////////////////////////////
        /////////////// SUB QUADRANTS 2.4 ////////////
        // Quadrant 2.4.1 (Top-Right)
        else if (point.x >= CalibratePoint15.x && point.x <= CalibratePoint5.x &&
                 point.y <= CalibratePoint6.y && point.y >= CalibratePoint15.y)
        {
            print("Point is in Quadrant 2.4.1 (Top-Right)");

            (eX, eY) = CalculateExEy(point, CalibratePoint15.x, CalibratePoint5.x, CalibratePoint15.y, CalibratePoint6.y);

            minX = Screen.width - (Screen.width / 8) * 5; maxX = Screen.width / 2;
            minY = Screen.height - (Screen.height / 8) * 3; maxY = Screen.height - (Screen.height / 4);
        }

        // Quadrant 2.4.2 (Top-Left)
        else if (point.x >= CalibratePoint6.x && point.x <= CalibratePoint15.x &&
            point.y <= CalibratePoint6.y && point.y >= CalibratePoint15.y)
        {
            print("Point is in Quadrant 2.4.2 (Top-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint6.x, CalibratePoint15.x, CalibratePoint15.y, CalibratePoint6.y);

            minX = Screen.width / 4; maxX = Screen.width - (Screen.width / 8) * 5;
            minY = Screen.height - (Screen.height / 8) * 3; maxY = Screen.height - Screen.height / 4; // minY = Screen.height - Screen.height / 2
        }

        // Quadrant 2.4.3 (Bottom-Left)
        else if (point.x >= CalibratePoint6.x && point.x <= CalibratePoint15.x &&
                 point.y <= CalibratePoint15.y && point.y >= CalibratePoint5.y)
        {
            print("Point is in Quadrant 2.4.3 (Bottom-Left)");
            (eX, eY) = CalculateExEy(point, CalibratePoint6.x, CalibratePoint15.x, CalibratePoint5.y, CalibratePoint15.y);

            minX = Screen.width / 4; maxX = Screen.width - (Screen.width / 8) * 5;
            minY = Screen.height / 2; maxY = Screen.height - (Screen.height / 8) * 3;
        }

        // Quadrant 2.4.4 (Bottom-Right)
        else if (point.x >= CalibratePoint15.x && point.x <= CalibratePoint5.x &&
                 point.y <= CalibratePoint15.y && point.y >= CalibratePoint5.y)
        {
            print("Point is in Quadrant 2.4.4 (Bottom-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint15.x, CalibratePoint5.x, CalibratePoint5.y, CalibratePoint15.y);

            minX = Screen.width - (Screen.width / 8) * 5; maxX = Screen.width / 2;
            minY = Screen.height / 2; maxY = Screen.height - (Screen.height / 8) * 3;
        }



    }

    private void SetQuadrantThree(Vector2 point)
    {

        /////////////////////// QUADRANT 3 (CPS: [4 - BOTTOM LEFT] [5 - TOP RIGHT] [9 - CENTER])
        ///////////////////////////////////////////////
        /////////////// SUB QUADRANTS 3.1 ////////////
        // Quadrant 3.1.1 (Top-Right)
        if (point.x >= CalibratePoint19.x && point.x <= CalibratePoint5.x &&
                 point.y <= CalibratePoint5.y && point.y >= CalibratePoint19.y)
        {
            print("Point is in Quadrant 3.1.1 (Top-Right)");

            (eX, eY) = CalculateExEy(point, CalibratePoint19.x, CalibratePoint5.x, CalibratePoint19.y, CalibratePoint5.y);

            minX = Screen.width - (Screen.width / 8) * 5; maxX = Screen.width / 2;
            minY = Screen.height - (Screen.height / 8) * 5; maxY = Screen.height / 2;
        }

        // Quadrant 3.1.2 (Top-Left)
        else if (point.x >= CalibratePoint9.x && point.x <= CalibratePoint19.x &&
            point.y <= CalibratePoint5.y && point.y >= CalibratePoint19.y)
        {
            print("Point is in Quadrant 3.1.2 (Top-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint9.x, CalibratePoint19.x, CalibratePoint19.y, CalibratePoint5.y);

            minX = Screen.width / 4; maxX = Screen.width - (Screen.width / 8) * 5;
            minY = Screen.height - (Screen.height / 8) * 5; maxY = Screen.height / 2;
        }

        // Quadrant 3.1.3 (Bottom-Left)
        else if (point.x >= CalibratePoint9.x && point.x <= CalibratePoint19.x &&
                 point.y <= CalibratePoint19.y && point.y >= CalibratePoint9.y)
        {
            print("Point is in Quadrant 3.1.3 (Bottom-Left)");
            (eX, eY) = CalculateExEy(point, CalibratePoint9.x, CalibratePoint19.x, CalibratePoint9.y, CalibratePoint19.y);

            minX = Screen.width / 4; maxX = Screen.width - (Screen.width / 8) * 5;
            minY = Screen.height / 4; maxY = Screen.height - (Screen.height / 8) * 5;
        }

        // Quadrant 3.1.4 (Bottom-Right)
        else if (point.x >= CalibratePoint19.x && point.x <= CalibratePoint5.x &&
                 point.y <= CalibratePoint19.y && point.y >= CalibratePoint9.y)
        {
            print("Point is in Quadrant 3.1.4 (Bottom-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint19.x, CalibratePoint5.x, CalibratePoint9.y, CalibratePoint19.y);

            minX = Screen.width - (Screen.width / 8) * 5; maxX = Screen.width / 2;
            minY = Screen.height / 4; maxY = Screen.height - (Screen.height / 8) * 5;
        }

        //////////////////////////////////////////////
        /////////////// SUB QUADRANTS 3.2 ////////////
        // Quadrant 3.2.1 (Top-Right)
        else if (point.x >= CalibratePoint18.x && point.x <= CalibratePoint9.x &&
                 point.y <= CalibratePoint5.y && point.y >= CalibratePoint18.y)
        {
            print("Point is in Quadrant 3.2.1 (Top-Right)");

            (eX, eY) = CalculateExEy(point, CalibratePoint18.x, CalibratePoint9.x, CalibratePoint18.y, CalibratePoint5.y);

            minX = Screen.width - (Screen.width / 8) * 7; maxX = (Screen.width / 4);
            minY = Screen.height - (Screen.height / 8) * 5; maxY = Screen.height / 2;
        }

        // Quadrant 3.2.2 (Top-Left)
        else if (point.x >= CalibratePoint4.x && point.x <= CalibratePoint18.x &&
            point.y <= CalibratePoint5.y && point.y >= CalibratePoint18.y)
        {
            print("Point is in Quadrant 3.2.2 (Top-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint4.x, CalibratePoint18.x, CalibratePoint18.y, CalibratePoint5.y);

            minX = 0; maxX = Screen.width - (Screen.width / 8) * 7;
            minY = Screen.height - (Screen.height / 8) * 5; maxY = Screen.height / 2;
        }

        // Quadrant 3.2.3 (Bottom-Left)
        else if (point.x >= CalibratePoint4.x && point.x <= CalibratePoint18.x &&
                 point.y <= CalibratePoint18.y && point.y >= CalibratePoint9.y)
        {
            print("Point is in Quadrant 3.2.3 (Bottom-Left)");
            (eX, eY) = CalculateExEy(point, CalibratePoint4.x, CalibratePoint18.x, CalibratePoint9.y, CalibratePoint18.y);

            minX = 0; maxX = Screen.width - (Screen.width / 8) * 7;
            minY = Screen.height / 4; maxY = Screen.height - (Screen.height / 8) * 5;
        }

        // Quadrant 3.2.4 (Bottom-Right)
        else if (point.x >= CalibratePoint18.x && point.x <= CalibratePoint9.x &&
                 point.y <= CalibratePoint18.y && point.y >= CalibratePoint9.y)
        {
            print("Point is in Quadrant 3.2.4 (Bottom-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint18.x, CalibratePoint9.x, CalibratePoint9.y, CalibratePoint18.y);

            minX = Screen.width - (Screen.width / 8) * 7; maxX = (Screen.width / 4);
            minY = Screen.height / 4; maxY = Screen.height - (Screen.height / 8) * 5;
        }

        //////////////////////////////////////////////
        /////////////// SUB QUADRANTS 3.3 ////////////
        // Quadrant 3.3.1 (Top-Right)
        else if (point.x >= CalibratePoint22.x && point.x <= CalibratePoint9.x &&
                 point.y <= CalibratePoint9.y && point.y >= CalibratePoint22.y)
        {
            print("Point is in Quadrant 3.3.1 (Top-Right)");

            (eX, eY) = CalculateExEy(point, CalibratePoint22.x, CalibratePoint9.x, CalibratePoint22.y, CalibratePoint9.y);

            minX = Screen.width - (Screen.width / 8) * 7; maxX = (Screen.width / 4);
            minY = Screen.height - (Screen.height / 8) * 7; maxY = Screen.height / 4;
        }

        // Quadrant 3.3.2 (Top-Left)
        else if (point.x >= CalibratePoint4.x && point.x <= CalibratePoint22.x &&
            point.y <= CalibratePoint9.y && point.y >= CalibratePoint22.y)
        {
            print("Point is in Quadrant 3.3.2 (Top-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint4.x, CalibratePoint22.x, CalibratePoint22.y, CalibratePoint9.y);

            minX = 0; maxX = Screen.width - (Screen.width / 8) * 7;
            minY = Screen.height - (Screen.height / 8) * 7; maxY = Screen.height / 4;
        }

        // Quadrant 3.3.3 (Bottom-Left)
        else if (point.x >= CalibratePoint4.x && point.x <= CalibratePoint22.x &&
                 point.y <= CalibratePoint22.y && point.y >= CalibratePoint4.y)
        {
            print("Point is in Quadrant 3.3.3 (Bottom-Left)");
            (eX, eY) = CalculateExEy(point, CalibratePoint4.x, CalibratePoint22.x, CalibratePoint4.y, CalibratePoint22.y);

            minX = 0; maxX = Screen.width - (Screen.width / 8) * 7;
            minY = 0; maxY = Screen.height - (Screen.height / 8) * 7;
        }

        // Quadrant 3.3.4 (Bottom-Right)
        else if (point.x >= CalibratePoint22.x && point.x <= CalibratePoint9.x &&
                 point.y <= CalibratePoint22.y && point.y >= CalibratePoint4.y)
        {
            print("Point is in Quadrant 3.3.4 (Bottom-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint22.x, CalibratePoint9.x, CalibratePoint4.y, CalibratePoint22.y);

            minX = Screen.width - (Screen.width / 8) * 7; maxX = (Screen.width / 4);
            minY = 0; maxY = Screen.height - (Screen.height / 8) * 7;
        }

        //////////////////////////////////////////////
        /////////////// SUB QUADRANTS 3.4 ////////////
        // Quadrant 3.4.1 (Top-Right)
        else if (point.x >= CalibratePoint23.x && point.x <= CalibratePoint5.x &&
                 point.y <= CalibratePoint9.y && point.y >= CalibratePoint23.y)
        {
            print("Point is in Quadrant 3.4.1 (Top-Right)");

            (eX, eY) = CalculateExEy(point, CalibratePoint23.x, CalibratePoint5.x, CalibratePoint23.y, CalibratePoint9.y);

            minX = Screen.width - (Screen.width / 8) * 5; maxX = Screen.width / 2;
            minY = Screen.height - (Screen.height / 8) * 7; maxY = Screen.height / 4;
        }

        // Quadrant 3.4.2 (Top-Left)
        else if (point.x >= CalibratePoint9.x && point.x <= CalibratePoint23.x &&
            point.y <= CalibratePoint9.y && point.y >= CalibratePoint23.y)
        {
            print("Point is in Quadrant 3.4.2 (Top-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint9.x, CalibratePoint23.x, CalibratePoint23.y, CalibratePoint9.y);

            minX = Screen.width / 4; maxX = Screen.width - (Screen.width / 8) * 5;
            minY = Screen.height - (Screen.height / 8) * 7; maxY = Screen.height / 4;
        }

        // Quadrant 3.4.3 (Bottom-Left)
        else if (point.x >= CalibratePoint9.x && point.x <= CalibratePoint23.x &&
                 point.y <= CalibratePoint23.y && point.y >= CalibratePoint4.y)
        {
            print("Point is in Quadrant 3.4.3 (Bottom-Left)");
            (eX, eY) = CalculateExEy(point, CalibratePoint9.x, CalibratePoint23.x, CalibratePoint4.y, CalibratePoint23.y);

            minX = Screen.width / 4; maxX = Screen.width - (Screen.width / 8) * 5;
            minY = 0; maxY = Screen.height - (Screen.height / 8) * 7;
        }

        // Quadrant 3.4.4 (Bottom-Right)
        else if (point.x >= CalibratePoint23.x && point.x <= CalibratePoint5.x &&
                 point.y <= CalibratePoint23.y && point.y >= CalibratePoint4.y)
        {
            print("Point is in Quadrant 3.4.4 (Bottom-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint23.x, CalibratePoint5.x, CalibratePoint4.y, CalibratePoint23.y);

            minX = Screen.width - (Screen.width / 8) * 5; maxX = Screen.width / 2;
            minY = 0; maxY = Screen.height - (Screen.height / 8) * 7;
        }



    }

    private void SetQuadrantFour(Vector2 point)
    {

        /////////////////////// QUADRANT 4 (CPS: [3 - BOTTOM RIGHT] [5 - TOP LEFT] [8 - CENTER])
        ///////////////////////////////////////////////
        /////////////// SUB QUADRANTS 4.1 ////////////
        // Quadrant 4.1.1 (Top-Right)
        if (point.x >= CalibratePoint21.x && point.x <= CalibratePoint3.x &&
                 point.y <= CalibratePoint5.y && point.y >= CalibratePoint21.y)
        {
            print("Point is in Quadrant 4.1.1 (Top-Right)");

            (eX, eY) = CalculateExEy(point, CalibratePoint21.x, CalibratePoint3.x, CalibratePoint21.y, CalibratePoint5.y);

            minX = Screen.width - Screen.width / 8; maxX = Screen.width;
            minY = Screen.height - (Screen.height / 8) * 5; maxY = Screen.height / 2;
        }

        // Quadrant 4.1.2 (Top-Left)
        else if (point.x >= CalibratePoint8.x && point.x <= CalibratePoint21.x &&
            point.y <= CalibratePoint5.y && point.y >= CalibratePoint21.y)
        {
            print("Point is in Quadrant 4.1.2 (Top-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint8.x, CalibratePoint21.x, CalibratePoint21.y, CalibratePoint5.y);

            minX = Screen.width - Screen.width / 4; maxX = Screen.width - Screen.width / 8;
            minY = Screen.height - (Screen.height / 8) * 5; maxY = Screen.height / 2;
        }

        // Quadrant 4.1.3 (Bottom-Left)
        else if (point.x >= CalibratePoint8.x && point.x <= CalibratePoint21.x &&
                 point.y <= CalibratePoint21.y && point.y >= CalibratePoint8.y)
        {
            print("Point is in Quadrant 4.1.3 (Bottom-Left)");
            (eX, eY) = CalculateExEy(point, CalibratePoint8.x, CalibratePoint21.x, CalibratePoint8.y, CalibratePoint21.y);

            minX = Screen.width - Screen.width / 4; maxX = Screen.width - Screen.width / 8;
            minY = Screen.height / 4; maxY = Screen.height - (Screen.height / 8) * 5;
        }

        // Quadrant 4.1.4 (Bottom-Right)
        else if (point.x >= CalibratePoint21.x && point.x <= CalibratePoint3.x &&
                 point.y <= CalibratePoint21.y && point.y >= CalibratePoint8.y)
        {
            print("Point is in Quadrant 4.1.4 (Bottom-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint21.x, CalibratePoint3.x, CalibratePoint8.y, CalibratePoint21.y);

            minX = Screen.width - Screen.width / 8; maxX = Screen.width;
            minY = Screen.height / 4; maxY = Screen.height - (Screen.height / 8) * 5;
        }

        //////////////////////////////////////////////
        /////////////// SUB QUADRANTS 4.2 ////////////
        // Quadrant 4.2.1 (Top-Right)
        else if (point.x >= CalibratePoint20.x && point.x <= CalibratePoint8.x &&
                 point.y <= CalibratePoint5.y && point.y >= CalibratePoint20.y)
        {
            print("Point is in Quadrant 4.2.1 (Top-Right)");

            (eX, eY) = CalculateExEy(point, CalibratePoint20.x, CalibratePoint8.x, CalibratePoint20.y, CalibratePoint5.y);

            minX = Screen.width - (Screen.width / 8) * 3; maxX = Screen.width - (Screen.width / 4);
            minY = Screen.height - (Screen.height / 8) * 5; maxY = Screen.height / 2;
        }

        // Quadrant 4.2.2 (Top-Left)
        else if (point.x >= CalibratePoint5.x && point.x <= CalibratePoint20.x &&
            point.y <= CalibratePoint5.y && point.y >= CalibratePoint20.y)
        {
            print("Point is in Quadrant 4.2.2 (Top-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint5.x, CalibratePoint20.x, CalibratePoint20.y, CalibratePoint5.y);

            minX = Screen.width / 2; maxX = Screen.width - (Screen.width / 8) * 3;
            minY = Screen.height - (Screen.height / 8) * 5; maxY = Screen.height / 2;
        }

        // Quadrant 4.2.3 (Bottom-Left)
        else if (point.x >= CalibratePoint5.x && point.x <= CalibratePoint20.x &&
                 point.y <= CalibratePoint20.y && point.y >= CalibratePoint8.y)
        {
            print("Point is in Quadrant 4.2.3 (Bottom-Left)");
            (eX, eY) = CalculateExEy(point, CalibratePoint5.x, CalibratePoint20.x, CalibratePoint8.y, CalibratePoint20.y);

            minX = Screen.width / 2; maxX = Screen.width - (Screen.width / 8) * 3;
            minY = Screen.height / 4; maxY = Screen.height - (Screen.height / 8) * 5;
        }

        // Quadrant 4.2.4 (Bottom-Right)
        else if (point.x >= CalibratePoint20.x && point.x <= CalibratePoint8.x &&
                 point.y <= CalibratePoint20.y && point.y >= CalibratePoint8.y)
        {
            print("Point is in Quadrant 4.2.4 (Bottom-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint20.x, CalibratePoint8.x, CalibratePoint8.y, CalibratePoint20.y);

            minX = Screen.width - (Screen.width / 8) * 3; maxX = Screen.width - (Screen.width / 4);
            minY = Screen.height / 4; maxY = Screen.height - (Screen.height / 8) * 5;
        }

        //////////////////////////////////////////////
        /////////////// SUB QUADRANTS 4.3 ////////////
        // Quadrant 4.3.1 (Top-Right)
        else if (point.x >= CalibratePoint24.x && point.x <= CalibratePoint8.x &&
                 point.y <= CalibratePoint8.y && point.y >= CalibratePoint24.y)
        {
            print("Point is in Quadrant 4.3.1 (Top-Right)");

            (eX, eY) = CalculateExEy(point, CalibratePoint24.x, CalibratePoint8.x, CalibratePoint24.y, CalibratePoint8.y);

            minX = Screen.width - (Screen.width / 8) * 3; maxX = Screen.width - (Screen.width / 4);
            minY = Screen.height - (Screen.height / 8) * 7; maxY = Screen.height / 4;
        }

        // Quadrant 4.3.2 (Top-Left)
        else if (point.x >= CalibratePoint5.x && point.x <= CalibratePoint24.x &&
            point.y <= CalibratePoint8.y && point.y >= CalibratePoint24.y)
        {
            print("Point is in Quadrant 4.3.2 (Top-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint5.x, CalibratePoint24.x, CalibratePoint24.y, CalibratePoint8.y);

            minX = Screen.width / 2; maxX = Screen.width - (Screen.width / 8) * 3;
            minY = Screen.height - (Screen.height / 8) * 7; maxY = Screen.height / 4;
        }

        // Quadrant 4.3.3 (Bottom-Left)
        else if (point.x >= CalibratePoint5.x && point.x <= CalibratePoint24.x &&
                 point.y <= CalibratePoint24.y && point.y >= CalibratePoint3.y)
        {
            print("Point is in Quadrant 4.3.3 (Bottom-Left)");
            (eX, eY) = CalculateExEy(point, CalibratePoint5.x, CalibratePoint24.x, CalibratePoint3.y, CalibratePoint24.y);

            minX = Screen.width / 2; maxX = Screen.width - (Screen.width / 8) * 3;
            minY = 0; maxY = Screen.height - (Screen.height / 8) * 7;
        }

        // Quadrant 4.3.4 (Bottom-Right)
        else if (point.x >= CalibratePoint24.x && point.x <= CalibratePoint8.x &&
                 point.y <= CalibratePoint24.y && point.y >= CalibratePoint3.y)
        {
            print("Point is in Quadrant 4.3.4 (Bottom-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint24.x, CalibratePoint8.x, CalibratePoint3.y, CalibratePoint24.y);

            minX = Screen.width - (Screen.width / 8) * 3; maxX = Screen.width - (Screen.width / 4);
            minY = 0; maxY = Screen.height - (Screen.height / 8) * 7;
        }

        //////////////////////////////////////////////
        /////////////// SUB QUADRANTS 4.4 ////////////
        // Quadrant 4.4.1 (Top-Right)
        else if (point.x >= CalibratePoint25.x && point.x <= CalibratePoint3.x &&
                 point.y <= CalibratePoint8.y && point.y >= CalibratePoint25.y)
        {
            print("Point is in Quadrant 4.4.1 (Top-Right)");

            (eX, eY) = CalculateExEy(point, CalibratePoint25.x, CalibratePoint3.x, CalibratePoint25.y, CalibratePoint8.y);

            minX = Screen.width - Screen.width / 8; maxX = Screen.width;
            minY = Screen.height - (Screen.height / 8) * 7; maxY = Screen.height / 4;
        }

        // Quadrant 4.4.2 (Top-Left)
        else if (point.x >= CalibratePoint8.x && point.x <= CalibratePoint25.x &&
            point.y <= CalibratePoint8.y && point.y >= CalibratePoint25.y)
        {
            print("Point is in Quadrant 4.4.2 (Top-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint8.x, CalibratePoint25.x, CalibratePoint25.y, CalibratePoint8.y);

            minX = Screen.width - Screen.width / 4; maxX = Screen.width - Screen.width / 8;
            minY = Screen.height - (Screen.height / 8) * 7; maxY = Screen.height / 4;
        }

        // Quadrant 4.4.3 (Bottom-Left)
        else if (point.x >= CalibratePoint8.x && point.x <= CalibratePoint25.x &&
                 point.y <= CalibratePoint25.y && point.y >= CalibratePoint3.y)
        {
            print("Point is in Quadrant 4.4.3 (Bottom-Left)");
            (eX, eY) = CalculateExEy(point, CalibratePoint8.x, CalibratePoint25.x, CalibratePoint3.y, CalibratePoint25.y);

            minX = Screen.width - Screen.width / 4; maxX = Screen.width - Screen.width / 8;
            minY = 0; maxY = Screen.height - (Screen.height / 8) * 7;
        }

        // Quadrant 4.4.4 (Bottom-Right)
        else if (point.x >= CalibratePoint25.x && point.x <= CalibratePoint3.x &&
                 point.y <= CalibratePoint25.y && point.y >= CalibratePoint3.y)
        {
            print("Point is in Quadrant 4.4.4 (Bottom-Right)");
            (eX, eY) = CalculateExEy(point, CalibratePoint25.x, CalibratePoint3.x, CalibratePoint3.y, CalibratePoint25.y);

            minX = Screen.width - Screen.width / 8; maxX = Screen.width;
            minY = 0; maxY = Screen.height - (Screen.height / 8) * 7;
        }
    }

    private void SetBallPoint(float eX, float eY, float minX, float maxX, float minY, float maxY)
    {
        // The parameters minX, maxX, minY, maxY are now redundant 
        // because eX and eY are already the final, calculated screen pixel coordinates.

        try
        {
            // --- MODIFICATION START ---
            // 1. Directly assign the final screen pixel coordinates.
            // The screen mapping happened in DrawMatrixLines:
            // e.g., eX = normalizedPoint.x * Screen.width;

            Ball1X = eX;
            Ball1Y = eY;

            // print("Final Screen X - " + Ball1X);
            // print("Final Screen Y - " + Ball1Y);

            // 2. Preserve your data serialization
            unityCamValues = Ball1X.ToString() + ":" + Ball1Y.ToString();

            // 3. Create the Raycast
            // NOTE: Camera.main.ScreenPointToRay expects a Vector3 in screen space, 
            // which Ball1X and Ball1Y now correctly represent.
            ray = Camera.main.ScreenPointToRay(new Vector3(Ball1X, Ball1Y, 0));
            // We use float values directly, though ScreenPointToRay usually takes integers.
            // It's generally safe to rely on the engine's implicit casting here.
            // If you were to explicitly cast: (int)(Ball1X), (int)(Ball1Y)
            // I'll leave the (int) casts removed as the input to this function (eX, eY)
            // is based on a float operation, but ScreenPointToRay often uses integers.
            // I've removed the (int) cast on the ray creation for now.
            // If you still see issues, reinstate the (int) casts.
            // --- MODIFICATION END ---
        }
        catch (Exception e)
        {
            Debug.Log("Error in SetBallPoint: " + e.Message);
        }
    }

    private (float eX, float eY) CalculateExEy(Vector2 point, float lowest_x_point, float highest_x_point, float lowest_y_point, float highest_y_point)
    {
        float eX = 0, eY = 0;

        // Calculate eX and eY based on the input point and the bounds
        eX = (point.x - lowest_x_point) / (highest_x_point - lowest_x_point);
        eY = 1 - ((point.y - highest_y_point) / (lowest_y_point - highest_y_point));  // Flip Y

        // Return eX and eY as a tuple
        return (eX, eY);
    }

    private void DrawMatrixLines1(Vector2Int p)
    {
        /************* Variables to draw bullets on targets ********************/
        Vector2 point = new Vector2(p.x, p.y); // Convert raw OpenCV point

        // Debugging the detected point
        // print("Detected Point: " + point);

        float eX = 0, eY = 0;
        float minX = 0, maxX = 0, minY = 0, maxY = 0;

        if (point.x >= CalibratePoint1.x && point.x <= CalibratePoint5.x &&
            point.y <= CalibratePoint1.y && point.y >= CalibratePoint5.y)
        {
            print("Point is in Quadrant 1 (Top-Left)");

            eX = (point.x - CalibratePoint1.x) / (CalibratePoint5.x - CalibratePoint1.x );
            eY = 1 - ((point.y - CalibratePoint1.y) / (CalibratePoint5.y - CalibratePoint1.y));  // Flip Y

            minX = 0; maxX = Screen.width / 2;
            minY = Screen.height / 2; maxY = Screen.height;
        }
        else if (point.x >= CalibratePoint5.x && point.x <= CalibratePoint2.x &&
                 point.y <= CalibratePoint2.y && point.y >= CalibratePoint5.y)
        {
            print("Point is in Quadrant 2 (Top-Right)");

            eX = (point.x - CalibratePoint5.x) / (CalibratePoint2.x - CalibratePoint5.x );
            eY = 1 - ((point.y - CalibratePoint2.y) / (CalibratePoint5.y - CalibratePoint2.y ));  // Flip Y

            minX = Screen.width / 2; maxX = Screen.width;
            minY = Screen.height / 2; maxY = Screen.height;
        }
        else if (point.x >= CalibratePoint5.x && point.x <= CalibratePoint3.x &&
                 point.y <= CalibratePoint5.y && point.y >= CalibratePoint3.y)
        {
            print("Point is in Quadrant 3 (Bottom-Right)");

            eX = (point.x - CalibratePoint5.x) / (CalibratePoint3.x - CalibratePoint5.x );
            eY = 1 - ((point.y - CalibratePoint5.y) / (CalibratePoint3.y - CalibratePoint5.y ));  // Flip Y

            minX = Screen.width / 2; maxX = Screen.width;
            minY = 0; maxY = Screen.height / 2;
        }
        else if (point.x >= CalibratePoint4.x && point.x <= CalibratePoint5.x &&
                 point.y <= CalibratePoint5.y && point.y >= CalibratePoint4.y)
        {
            print("Point is in Quadrant 4 (Bottom-Left)");

            eX = (point.x - CalibratePoint4.x) / (CalibratePoint5.x - CalibratePoint4.x );
            eY = 1 - ((point.y - CalibratePoint5.y) / (CalibratePoint4.y - CalibratePoint5.y ));  // Flip Y

            minX = 0; maxX = Screen.width / 2;
            minY = 0; maxY = Screen.height / 2;
        }
        else
        {
            print("Point is outside the calibration grid!");
            return; // Ignore points outside the grid
        }

        try
        {
            // Map to quadrant-specific screen space
            Ball1X = Mathf.Lerp(minX, maxX, eX);
            Ball1Y = Mathf.Lerp(minY, maxY, eY);

            //print("Mapped X - " + Ball1X);
            //print("Mapped Y - " + Ball1Y);

            unityCamValues = Ball1X.ToString() + ":" + Ball1Y.ToString();

            ray = Camera.main.ScreenPointToRay(new Vector3((int)(Ball1X), (int)(Ball1Y), 0));
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.Message);
        }
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

    private void LoadCalibration()
    {
        if (!File.Exists(calibrationPath))
        {
            File.Create(calibrationPath);
        }
        else
        {
            string[] calibrationPoints = FileManager.ReadLinesFromFile(calibrationPath);

            MultiCalibPoints.Clear(); // Clear any previous points

            foreach (string line in calibrationPoints)
            {
                if (line.Contains("points"))
                {
                    string[] parts = line.Split(':'); // Split once to improve efficiency
                    int numPoints = 25;

                    for (int i = 1; i <= numPoints; i++) // Iterate over 9 points
                    {
                        string[] coords = parts[i].Split(','); // Extract coordinates
                        int x = Int32.Parse(coords[0]);
                        int y = Int32.Parse(coords[1]);

                        Vector2Int point = new Vector2Int(x, y);
                        MultiCalibPoints.Add(point);

                        // Assign to corresponding CalibratePoint variable (if needed)
                        switch (i)
                        {
                            case 1: CalibratePoint1 = point; break;
                            case 2: CalibratePoint2 = point; break;
                            case 3: CalibratePoint3 = point; break;
                            case 4: CalibratePoint4 = point; break;
                            case 5: CalibratePoint5 = point; break;
                            case 6: CalibratePoint6 = point; break;
                            case 7: CalibratePoint7 = point; break;
                            case 8: CalibratePoint8 = point; break;
                            case 9: CalibratePoint9 = point; break;
                            case 10: CalibratePoint10 = point; break;
                            case 11: CalibratePoint11 = point; break;
                            case 12: CalibratePoint12 = point; break;
                            case 13: CalibratePoint13 = point; break;
                            case 14: CalibratePoint14 = point; break;
                            case 15: CalibratePoint15 = point; break;
                            case 16: CalibratePoint16 = point; break;
                            case 17: CalibratePoint17 = point; break;
                            case 18: CalibratePoint18 = point; break;
                            case 19: CalibratePoint19 = point; break;
                            case 20: CalibratePoint20 = point; break;
                            case 21: CalibratePoint21 = point; break;
                            case 22: CalibratePoint22 = point; break;
                            case 23: CalibratePoint23 = point; break;
                            case 24: CalibratePoint24 = point; break;
                            case 25: CalibratePoint25 = point; break;
                        }
                    }
                }

            }

            // Load homography matrix using 9 points
            // SetHomography();
        }
    }

    void sceneOpenPlainShoot(RaycastHit hit)
    {
        // print("Point reached..." + hit.transform.name);
        HandleIsShotFired(); // Updates shots fired booleans


        // print("Point Reached... " + hit.transform.name );

        all_soldiers = GameObject.FindGameObjectsWithTag("soldier");

        if (!activeScene.ToLower().Contains("crouching"))
        {
            foreach (GameObject sol in all_soldiers)
            {
                if(!activeScene.ToLower().Contains("suspect"))sol.transform.gameObject.GetComponent<PoliceBehaviour>().SendMessage("ApplyDamage", "change");
            }
        }
        
        // Debug.Log("hit:" + hit.transform.name);
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
        else if (hit.transform.name.Contains("soldier"))
        {
            if(activeScene.ToLower().Contains("basic"))
            {
                hit.transform.gameObject.GetComponent<TargetMessageReceive>().SendMessage("ApplyDamage", hit.transform.name);
            }
            else if (activeScene.ToLower().Contains("suspectshoot"))
            {
                //print("Shooting");
                hit.transform.gameObject.GetComponent<SuspectShootAnimator>().SendMessage("ApplyDamage", hit.transform.name);
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
        else if (hit.transform.tag.ToLower().Contains("target"))
        {
            // print("Point Reached...");

            if(activeScene.ToLower().Contains("basic"))
            {
                string targetHitName = SetHitSearchName(hit);

                //Apply Targate Impact
                if (sendHitSignal(hit) == true)
                    return;

                PlayHitSound(hit); // Play target sound
                ManageShootingPoints(hit, targetHitName);
            }
        }
        else if(activeScene.ToLower().Contains("hunting")) // (hit.transform.name.ToLower().Contains(TestConditionsManager.animalName) || hit.transform.tag == "animal_head")
        {
            if(activeScene.Contains("ForestHunting"))
            {
                print("Point reached..." + hit.transform.tag);
                if (hit.transform.tag == "deer" || hit.transform.tag == "boar" || hit.transform.tag == "buffalo" || hit.transform.tag == "moose")
                {
                    hit.transform.gameObject.GetComponent<HuntAnimalController>().SendMessage("ApplyDamage", hit.transform.name);
                }
                else if (hit.transform.tag == "bear")
                {
                    hit.transform.gameObject.GetComponent<BearAIController>().SendMessage("ApplyDamage", hit.transform.name);
                }
                else if (hit.transform.tag == "animal_head")
                {
                    hit.transform.gameObject.GetComponent<AnimalHeadReference>().SendMessage("ApplyDamage", hit.transform.name);
                }
            }
            else
            {
                if (hit.transform.tag == "deer")
                {
                    hit.transform.gameObject.GetComponent<AnimalController>().SendMessage("ApplyDamage", hit.transform.tag);
                }
                else if (hit.transform.tag == "animal_head")
                {
                    hit.transform.gameObject.GetComponent<AnimalHeadReference>().SendMessage("ApplyDamage", hit.transform.GetComponent<AnimalHeadReference>().mainBody.transform.name);
                }
            }

        }

        //Target Popup SHooting
        else if(activeScene.Contains("TargetRecognition") || activeScene.Contains("DecisionPTShoot"))
        {
            UpdateTRTSequence(hit);
            string targetHitName = SetHitSearchName(hit);

            //Apply Targate Impact
            if (sendHitSignal(hit) == true)
                return;

            ManageShootingPoints(hit, targetHitName);;

            // GameObject trt_manager = GameObject.FindGameObjectWithTag("TRTManager");
            // trt_manager.GetComponent<TRTManager>().GenerateTargetGroup();
        }
        else if (hit.transform.tag.ToLower().Contains("head") || hit.transform.tag.ToLower().Contains("body"))
        {
            string targetHitName = SetHitSearchName(hit);
            ManageShootingPoints(hit, targetHitName);

            PlayHitSound(hit);
            ManageIPECTargetSignals(hit);
        }

        manageLaneShotsMissed(hit);
    }

    private void HandleIsShotFired()
    {
        if (activeScene.Contains("VideoTraining"))
        {
            StaticVariableManager.isVideoShotFired = true;
        }

    }

    private string SetHitSearchName(RaycastHit hit)
    {
        string targetSearchName = "";


        if (activeScene.ToLower().Contains("5target") || activeScene.ToLower().Contains("cyclic"))
        {

            targetSearchName = "point";

        }
        else if (activeScene.ToLower().Contains("shell"))
        {
            targetSearchName = "holder";
        }
        else if (activeScene.ToLower().Contains("plat") || activeScene.ToLower().Contains("dueling") || activeScene.ToLower().Contains("dice") || activeScene.ToLower().Contains("distancesimulator")
            || activeScene.ToLower().Contains("rifflepole") || activeScene.ToLower().Contains("ipec") || activeScene.ToLower().Contains("cargame") || activeScene.ToLower().Contains("hiddentarget"))
        {
            targetSearchName = "plate";
        }
        else if (activeScene.ToLower().Contains("sequencenum") || activeScene.ToLower().Contains("colorsequence") || activeScene.ToLower().Contains("expressionshoot"))
        {
            targetSearchName = "plate";
        }
        else if (activeScene.ToLower().Contains("baloon"))
        {

            targetSearchName = "baloon";

        }
        else if (activeScene.ToLower().Contains("suspectshoot"))
        {
            targetSearchName = "board";
        }
        else if (activeScene.ToLower().Contains("block"))
        {
            targetSearchName = "boxtarget";
        }
        else if (hit.transform.name.ToLower().Contains("target") && (hit.transform.tag.ToLower().Contains("head") || hit.transform.tag.ToLower().Contains("body")))
        {
            targetSearchName = "target";
        }
        else if (hit.transform.name.ToLower().Contains("cover") && (hit.transform.tag.ToLower().Contains("head") || hit.transform.tag.ToLower().Contains("body")))
        {
            targetSearchName = "cover";
        }
        else if (activeScene.Contains("TargetRecognition") || activeScene.Contains("DecisionPTShoot"))
        {
            if(!StaticVariableManager.TRTTargetsShot.Contains(hit.transform.gameObject))
            {
                StaticVariableManager.TRTTargetsShot.Add(hit.transform.gameObject);

                if (hit.transform.name.ToLower().Contains("head"))
                {
                    targetSearchName = "head";
                    if (hit.transform.name.ToLower().Contains("suspect"))
                    {
                        targetSearchName = "suspect";
                        StaticVariableManager.numSuspectHeadShot++;
                    }
                }
                if (hit.transform.name.ToLower().Contains("body"))
                {
                    targetSearchName = "body";
                    if (hit.transform.name.ToLower().Contains("suspect"))
                    {
                        targetSearchName = "suspect";
                        StaticVariableManager.numSuspectBodyShot++;
                    }
                }

                if (hit.transform.name.ToLower().Contains("civilian"))
                {
                    targetSearchName = "civilian";
                    StaticVariableManager.numCiviliansHit++;
                }
                else if (hit.transform.name.ToLower().Contains("suspect"))
                {
                    targetSearchName = "suspect";
                    StaticVariableManager.numSuspectsHit++;
                }

                ManageTRTBoard(hit, targetSearchName);
            }

        }
        else if(activeScene.Contains("VideoTraining") || activeScene.Contains("VideoTransition"))
        {
            // print("Hit: " + hit.transform.name);
            if (hit.transform.name.Contains("(Threat)"))
            {
                StaticVariableManager.isThreatShot = true;
                // StaticVariableManager.isVideoTrainingComplete = true;
                if(!StaticVariableManager.videoThreatsShot.Contains(hit.transform.gameObject))
                {
                    StaticVariableManager.numVideoThreatsShot++;
                    //print("Threats: " + StaticVariableManager.numVideoThreatsShot);
                    StaticVariableManager.videoThreatsShot.Add(hit.transform.gameObject);
                    StaticVariableManager.videoThreatsShot.Add(hit.transform.gameObject.
                        GetComponent<TargetMessageReceive>().pair); // Add pair

                }

                targetSearchName = "threat";
            }
            else if (hit.transform.name.Contains("(None Threat)"))
            {
                StaticVariableManager.isNoneThreatShot = true;
                StaticVariableManager.numVideoNoneThreatsShot++;
                // StaticVariableManager.isVideoTrainingComplete = true;
                targetSearchName = "none threat";
            }

            if(StaticVariableManager.isVideoTrainingComplete) ManualVideoTracker.ManageStartAndPause();
        }

        return targetSearchName;
    }

    private void ManageTRTBoard(RaycastHit hit, string target_search_name)
    {
        if (hit.transform.name.ToLower().Contains("suspect"))
        {
            StaticVariableManager.threatCount++;
        }

        if (StaticVariableManager.threatCount >= StaticVariableManager.numCurrThreats)
        {
            if (hit.transform.name.ToLower().Contains(target_search_name) && target_search_name != "")
            {
                // print(target_search_name);
                //GameObject trt_manager = GameObject.FindGameObjectWithTag("TRTManager");
                //trt_manager.GetComponent<TRTManager>().GenerateTargetGroup();
            }

            if(activeScene.Contains("DecisionPTShoot")) CheckTRTSequence();
        }

    }

    private void UpdateTRTSequence(RaycastHit hit)
    {
        Transform parent = hit.transform.parent;

        while (parent != null)
        {
            if (parent.name.Contains("Level_"))
            {
                // Extract the "Level_*" part
                int idx = parent.name.IndexOf("Level_");
                string temp = parent.name.Substring(idx);

                // If there’s extra stuff after the number, trim it
                int spaceIdx = temp.IndexOfAny(new char[] { ' ', '(', ')' });
                string levelName = (spaceIdx > 0) ? temp.Substring(0, spaceIdx) : temp;

                // Debug.Log("" + levelName + "");

                switch (levelName)
                {
                    case "Level_1":
                        StaticVariableManager.TRTFollowUpSequence += "1";
                        break;
                    case "Level_2":
                        StaticVariableManager.TRTFollowUpSequence += "2";
                        break;
                    case "Level_3":
                        StaticVariableManager.TRTFollowUpSequence += "3";
                        break;
                    case "Level_4":
                        StaticVariableManager.TRTFollowUpSequence += "4";
                        break;
                }

                return;
            }

            parent = parent.parent;
        }

        Debug.Log("No Level_ found in parents.");
    }

    private void CheckTRTSequence()
    {

        foreach(String sequence in StaticVariableManager.TRTShootSequence)
        {

            // print($"Set sequence {sequence} Follow up sequence {StaticVariableManager.TRTFollowUpSequence}");

            if(StaticVariableManager.TRTFollowUpSequence == sequence)
            {
                // print("Correct sequence");
                StaticVariableManager.isCorrectTRTRequence = true;
            }
            else
            {
                // print("Incorrect sequence");
                StaticVariableManager.isCorrectTRTRequence = false;
            }
            StaticVariableManager.isTRTTrainingComplete = true;
        }

    }

    private void ManageShootingPoints(RaycastHit hit, string target_search_name)
    {
        print(target_search_name);
        if (hit.transform.name.ToLower().Contains(target_search_name))
        {
            //Evaluate Current Lane
            string tempTargetName = hit.transform.name;
            char tempFirstChar = tempTargetName[0];
            isCurrShotProcessed = true;
            switch (tempFirstChar)
            {
                case '1':
                    if (lane1TargetsComplete == false)
                    {
                        // print("Point Reached...");
                        //Calculate Split time
                        updateLane1SplitTime("hit", "c");
                        updateLane1ResponseTime("hit", "c");
                        generateBulletImpact(hit);
                        ManageCorrectColorHit(hit, "lane 1");
                        ManageSuspectTargetScoring(hit, "lane 1");

                        lane1ActiveTimeCounter = lane1TimeActiveCounter; //Assign Current Time
                        point_lane_1 = updateLanePoint(tempTargetName, point_lane_1);
                        lane1PointsHit++;
                        UpdateShotsFired("1");

                        //StaticVariableManager.lane1PredelayActive = true;
                    }
                    break;

                case '2':
                    if (lane2TargetsComplete == false)
                    {
                        //Calculate Split time
                        updateLane2SplitTime("hit", "c");
                        updateLane2ResponseTime("hit", "c");
                        generateBulletImpact(hit);
                        ManageCorrectColorHit(hit, "lane 2");
                        ManageSuspectTargetScoring(hit, "lane 2");

                        lane2ActiveTimeCounter = lane2TimeActiveCounter; //Assign Current Time

                        point_lane_2 = updateLanePoint(tempTargetName, point_lane_2);
                        lane2PointsHit++;
                        UpdateShotsFired("2");

                        //StaticVariableManager.lane2PredelayActive = true;
                    }
                    break;

                case '3':
                    if (lane3TargetsComplete == false)
                    {
                        //Calculate Split time
                        updateLane3SplitTime("hit", "c");
                        updateLane3ResponseTime("hit", "c");
                        generateBulletImpact(hit);
                        ManageCorrectColorHit(hit, "lane 3");
                        ManageSuspectTargetScoring(hit, "lane 3");

                        lane3ActiveTimeCounter = lane3TimeActiveCounter; //Assign Current Time

                        point_lane_3 = updateLanePoint(tempTargetName, point_lane_3);
                        lane3PointsHit++;
                        UpdateShotsFired("3");

                        //StaticVariableManager.lane3PredelayActive = true;
                    }
                    break;
            }

            // manageLaneTarinigStopCondition();
        }

        // Additional points
        if (activeScene.ToLower().Contains("ipec"))
        {

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
        else if (activeScene.ToLower().Contains("5pointbullseye"))
        {
            bullseye_hit = true;
        }
    }



    private void PlayHitSound(RaycastHit hit)
    {
        // print("Hit: " + hit.transform.name);

        if (!activeScene.ToLower().Contains("risingplate"))
        {
            platesHit++;
        }
        if (hit.transform.name.ToLower().Contains("plate"))
        {
            platHitSound.Play();
        }
        else if (hit.transform.name.ToLower().Contains("baloon"))
        {
            baloonPopSound.Play();
        }
        else if(hit.transform.tag.ToLower().Contains("head") || hit.transform.tag.ToLower().Contains("body"))
        {
            PlayIPECHitSound(hit);
        }
    }


    private void manageLaneShotsMissed(RaycastHit hit)
    {
        if (hit.transform.tag.ToLower().Contains("misscollider"))
        {

            if (hit.transform.tag.Contains("Lane1") && !lane1TargetsComplete)
            {
                generateBulletImpact2(hit);

                numLane1ShotsMissed++;
                updateLane1ResponseTime("miss", "c");
                updateLane1SplitTime("miss", "c");
                UpdateShotsFired("1");
            }
            else if (hit.transform.tag.Contains("Lane2") && !lane2TargetsComplete)
            {
                generateBulletImpact2(hit);

                numLane2ShotsMissed++;
                updateLane2ResponseTime("miss", "c");
                updateLane2SplitTime("miss", "c");
                UpdateShotsFired("2");
            }
            else if (hit.transform.tag.Contains("Lane3") && !lane3TargetsComplete)
            {
                generateBulletImpact2(hit);

                numLane3ShotsMissed++;
                updateLane3ResponseTime("miss", "c");
                updateLane3SplitTime("miss", "c");
                UpdateShotsFired("3");
            }

            if (activeScene.ToLower().Contains("suspectshoot"))
            {
                if (StaticVariableManager.isLane1TargetMissed || StaticVariableManager.isLane2TargetMissed || StaticVariableManager.isLane3TargetMissed)
                {
                    showSuspectResult(hit);
                }
            }
            else if(activeScene.Contains("VideoTraining") /*&& (!StaticVariableManager.isStartVideoTime || !StaticVariableManager.isThreatVideo)*/) // only when the video is not in threat state
            {
                // StaticVariableManager.isVideoTrainingComplete = true;
                // ManualVideoTracker.ManageStartAndPause();
            }
        }
        else if (hit.transform.name.ToLower().Contains("body.") || hit.transform.name.ToLower().Contains("head."))
        {
            generateBulletImpact2(hit);
            if (hit.transform.name.Contains("1"))
            {
                numLane1ShotsMissed++;
                updateLane1ResponseTime("miss", "c");
                updateLane1SplitTime("miss", "c");
                UpdateShotsFired("1");
            }
            else if (hit.transform.name.Contains("2"))
            {
                numLane2ShotsMissed++;
                updateLane2ResponseTime("miss", "c");
                updateLane2SplitTime("miss", "c");
                UpdateShotsFired("2");
            }
            else if (hit.transform.name.Contains("3"))
            {
                numLane3ShotsMissed++;
                updateLane3ResponseTime("miss", "c");
                updateLane3SplitTime("miss", "c");
                UpdateShotsFired("3");
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
            generateBulletImpact2(hit);
            if (hit.transform.name.Contains("1"))
            {
                numLane1ShotsMissed++;
                updateLane1ResponseTime("miss", "c");
                updateLane1SplitTime("miss", "c");
                generateBulletImpact2(hit);
                UpdateShotsFired("1");
            }
            else if (hit.transform.name.Contains("2"))
            {
                numLane2ShotsMissed++;
                updateLane2ResponseTime("miss", "c");
                updateLane2SplitTime("miss", "c");
                generateBulletImpact2(hit);
                UpdateShotsFired("2");
            }
            else if (hit.transform.name.Contains("3"))
            {
                numLane3ShotsMissed++;
                updateLane3ResponseTime("miss", "c");
                updateLane3SplitTime("miss", "c");
                generateBulletImpact2(hit);
                UpdateShotsFired("3");
            }
            WallTargetControl.requestMove();
        }
        else if (activeScene.ToLower().Contains("shell"))
        {
            generateBulletImpact2(hit);
            if (!hit.transform.name.Contains("holder"))
            {
                numLane1ShotsMissed++;
                updateLane1ResponseTime("miss", "c");
                updateLane1SplitTime("miss", "c");
                UpdateShotsFired("1");
            }
        }
        else if (activeScene.ToLower().Contains("5pointbullseye"))
        {
            generateBulletImpact2(hit);
            if (!bullseye_hit)
            {
                numLane1ShotsMissed++;
                updateLane1ResponseTime("miss", "c");
                updateLane1SplitTime("miss", "c");
                generateBulletImpact2(hit);
                UpdateShotsFired("1");
            }
            bullseye_hit = false;
        }
        else if (activeScene.ToLower().Contains("basic1laneipecboard"))
        {
            //    generateBulletImpact2(hit);
            //    UpdateShotsFired("1");
            //print("Point Reached...");
            //updateLane1ResponseTime("miss", "c");
            //updateLane1SplitTime("miss", "c");
            //generateBulletImpact2(hit);
        }
        else if (hit.transform.tag.ToLower().Contains("wall"))
        {
            generateBulletImpact2(hit);
            switch (hit.transform.tag)
            {
                case "Lane1Wall":
                    numLane1ShotsMissed++;
                    UpdateShotsFired("1");
                    //updateLane1ResponseTime("miss", "c");
                    //updateLane1SplitTime("miss", "c");
                    //StaticVariableManager.isLane1TargetMissed = true;
                    //print("RE: Lane 1 Shots missed is : " + numLane1ShotsMissed);
                    break;
                case "Lane2Wall":
                    numLane2ShotsMissed++;
                    UpdateShotsFired("2");
                    //updateLane2ResponseTime("miss", "c");
                    //updateLane2SplitTime("miss", "c");
                    //StaticVariableManager.isLane2TargetMissed = true;
                    //print("RE: Lane 2 Shots missed is : " + numLane2ShotsMissed);
                    break;
                case "Lane3Wall":
                    numLane3ShotsMissed++;
                    UpdateShotsFired("3");
                    //updateLane3ResponseTime("miss", "c");
                    //updateLane3SplitTime("miss", "c");
                    //StaticVariableManager.isLane3TargetMissed = true;
                    //print("RE: Lane 3 Shots missed is : " + numLane3ShotsMissed);
                    break;
            }
        }
        else if (activeScene == "DirectForestHunting")
        {
            /*if (hit.transform.gameObject.layer != "animal")
            {

                // numLane1ShotsMissed++;
                // updateLane1ResponseTime("miss", "c");
                // updateLane1SplitTime("miss", "c");
                // UpdateShotsFired("1");
            }*/
        }
 
    }

    private void HandlExtMissedShots(RaycastHit hit)
    {
        // print("Point 2 reached...");
        if (activeScene == "Basic1Lane3DClayPigeon")
        {
            // print("Point 3 reached...");
            // isCurrShotProcessed = false;
            if (!isCurrShotProcessed)
            {
                // print("Point 4 reached...");
                // generateBulletImpact2(hit);

                numLane1ShotsMissed++;
                updateLane1ResponseTime("miss", "c");
                updateLane1SplitTime("miss", "c");
                UpdateShotsFired("1");
            }
        }

        isCurrShotProcessed = false; // Reset
    }

    private void DisplayLaneShotsFired()
    {
        //print("Test: Point reached...");
        // numLane1ShotsFired = lane1PointsHit + numLane1ShotsMissed;
        // numLane2ShotsFired = lane2PointsHit + numLane2ShotsMissed;
        // numLane3ShotsFired = lane3PointsHit + numLane3ShotsMissed;

        lane1ShotsFiredDisplay.text = numLane1ShotsFired.ToString("0");
        lane2ShotsFiredDisplay.text = numLane2ShotsFired.ToString("0");
        lane3ShotsFiredDisplay.text = numLane3ShotsFired.ToString("0");
    }

    private void UpdateShotsFired(string lane)
    {
        switch (lane)
        {
            case "1":
                numLane1ShotsFired++;
                if (numLane1ShotsFired >= num_set_shots || lane1PointsHit >= num_targets_to_hit)
                {
                    SetLaneStopIndicators(ref lane1TargetsComplete, ref lane1ActiveTimeCounter, lane1StopSignal);
                    // print("Lane 1 Target Complete: " + lane1TargetsComplete);
                }
                break;
            case "2":
                numLane2ShotsFired++;
                if (numLane2ShotsFired >= num_set_shots || lane2PointsHit >= num_targets_to_hit)
                {
                    SetLaneStopIndicators(ref lane2TargetsComplete, ref lane2ActiveTimeCounter, lane2StopSignal);
                    // print("Lane 2 Target Complete: " + lane1TargetsComplete);
                }
                break;
            case "3":
                numLane3ShotsFired++;
                if (numLane3ShotsFired >= num_set_shots || lane3PointsHit >= num_targets_to_hit)
                {
                    SetLaneStopIndicators(ref lane3TargetsComplete, ref lane3ActiveTimeCounter, lane3StopSignal);
                    // print("Lane 3 Target Complete: " + lane1TargetsComplete);
                }
                break;
        }
    }

    private void SetLaneStopIndicators(ref bool targetComplete, ref float activeTimer, GameObject stopSignal)
    {
        targetComplete = true;
        activeTimer = timeActiveCounter;
        stopSignal.SetActive(true);
    }

    private void generateBulletImpact(RaycastHit hit)
    {
        if (Scoring.ammo_setting == "Live" || Scoring.ammo_setting.ToLower().Contains("laser"))
        {
            GameObject impactGo = null;
            if (activeScene.Contains("IPSCExtended") || activeScene.Contains("VideoTraining") || activeScene.Contains("VideoTransition")) // || activeScene.Contains("VideoTraining")
            {
                // Store the prefab's intended local scale
                Vector3 prefabScale = bullethole_2.transform.localScale;

                // Instantiate the bullet hole
                impactGo = Instantiate(bullethole_2, hit.point, Quaternion.LookRotation(hit.normal));
                // NOTE: Still NO parenting (impactGo.transform.SetParent(hit.transform) is GONE)

                // Set the initial world scale to match the prefab's local scale
                impactGo.transform.localScale = prefabScale;

                // Get the KeepUpright script
                KeepUpright keeper = impactGo.GetComponent<KeepUpright>();

                if (keeper != null)
                {
                    // Pass the tracking data AND the correct target scale
                    keeper.Initialize(hit.transform, hit.transform.InverseTransformPoint(hit.point), prefabScale);
                }
            }
            else
            {
                impactGo = Instantiate(bullethole_2, hit.point, Quaternion.LookRotation(hit.normal));
            }

            impactGo.transform.name = "Hit";


            switch (hit.transform.name[0])
            {
                case '1':
                    lane_1_impact_list.Add(impactGo);
                    impactGo.GetComponent<BulletManager>().bullet_time_text.text = (timeActiveCounter).ToString("0.00") + " sec";
                    //print("RRE: adding for lane 1");
                    HandleTargetHit(hit, impactGo);
                    SetReplayCounter("1", impactGo);
                    break;
                case '2':
                    lane_2_impact_list.Add(impactGo);
                    impactGo.GetComponent<BulletManager>().bullet_time_text.text = (timeActiveCounter).ToString("0.00") + " sec";
                    //print("RRE: adding for lane 2");
                    HandleTargetHit(hit, impactGo);
                    SetReplayCounter("2", impactGo);
                    break;
                case '3':
                    lane_3_impact_list.Add(impactGo);
                    impactGo.GetComponent<BulletManager>().bullet_time_text.text = (timeActiveCounter).ToString("0.00") + " sec";
                    //print("RRE: adding for lane 3");
                    HandleTargetHit(hit, impactGo);
                    SetReplayCounter("3", impactGo);
                    break;
            }
            //Destroy(impactGo, 1.0f);
            if (!activeScene.ToLower().Contains("distance") && Scoring.ammo_setting == "Live")
            {
                // impactGo.SetActive(false);
            }

            if (StaticVariableManager.backgroundColorSetting.ToLower().Contains("red"))
            {
                impactGo.GetComponent<Renderer>().material.color = Color.yellow;
            }
            else
            {
                impactGo.GetComponent<Renderer>().material.color = Color.red;
            }

            if (activeScene.ToLower().Contains("suspect") || activeScene.Contains("VideoTransition"))
            {
                Destroy(impactGo, 1f);
            }
        }
    }

    private void generateBulletImpact2(RaycastHit hit)
    {
        if (Scoring.ammo_setting == "Live" || Scoring.ammo_setting.ToLower().Contains("laser"))
        {
            // 1. Store prefab scale ONCE.
            Vector3 prefabScale = bullethole_3.transform.localScale;

            // 2. Instantiate bullet hole ONCE.
            GameObject impactGo = Instantiate(bullethole_3, hit.point, Quaternion.LookRotation(hit.normal));

            // 3. Set the initial world scale to match the prefab's local scale
            impactGo.transform.localScale = prefabScale;

            // 4. Initialization (Only necessary if the MISS target might move, which is unusual)
            KeepUpright keeper = impactGo.GetComponent<KeepUpright>();

            if (keeper != null)
            {
                // Pass the tracking data AND the correct target scale
                // NOTE: If the hit object is static (a true 'miss' wall), the tracking will be redundant but harmless.
                keeper.Initialize(hit.transform, hit.transform.InverseTransformPoint(hit.point), prefabScale);
            }


            // GameObject impactGo = Instantiate(bullethole_3, hit.point, Quaternion.LookRotation(hit.normal));
            // impactGo.transform.name = "Miss";
            //print("Test: Point Reached...");

            if (hit.transform.tag == "Lane1MissCollider" || hit.transform.tag == "lane1hostage")
            {
                AssignMissImpact(hit, impactGo, lane_1_impact_list);
                SetReplayCounter("1", impactGo);
            }
            else if (hit.transform.tag == "Lane2MissCollider" || hit.transform.tag == "lane2hostage")
            {
                AssignMissImpact(hit, impactGo, lane_2_impact_list);
                SetReplayCounter("2", impactGo);
            }
            else if (hit.transform.tag == "Lane3MissCollider" || hit.transform.tag == "lane3hostage")
            {
                AssignMissImpact(hit, impactGo, lane_3_impact_list);
                SetReplayCounter("3", impactGo);
            }
            else if (activeScene.ToLower().Contains("pointbullseye"))
            {
                AssignMissImpact(hit, impactGo, lane_1_impact_list);
            }

            if (activeScene.ToLower().Contains("suspect") || activeScene.Contains("VideoTransition"))
            {
                Destroy(impactGo, 1f);
            }
        }
    }

    private void SetReplayCounter(string lane, GameObject bullet)
    {
        if((DropDown.scene_type == "static" || DropDown.scene_type == "static response" || activeScene.ToLower().Contains("hiddentarget"))
            && !is3DScene)
        {
            switch (lane)
            {
                case "1":
                    if (activeScene.ToLower().Contains("colorsequence"))
                    {
                        this.GetComponent<ReplayCounter>().RecordLane1Shot(bullet, StaticVariableManager.currLane1Targets, 
                            StaticVariableManager.currLane1IndicatorColor);
                    }
                    else if(activeScene.ToLower().Contains("sequencenum"))
                    {
                        this.GetComponent<ReplayCounter>().RecordLane1Shot(bullet, StaticVariableManager.currLane1Targets,
                        StaticVariableManager.currLane1TargetNum);
                    }
                    else
                    {
                        this.GetComponent<ReplayCounter>().RecordLane1Shot(bullet, StaticVariableManager.currLane1Targets);
                    }
                    break;
                case "2":
                    if (activeScene.ToLower().Contains("colorsequence"))
                    {
                        this.GetComponent<ReplayCounter>().RecordLane2Shot(bullet, StaticVariableManager.currLane2Targets,
                            StaticVariableManager.currLane2IndicatorColor);
                    }
                    else if (activeScene.ToLower().Contains("sequencenum"))
                    {
                        this.GetComponent<ReplayCounter>().RecordLane2Shot(bullet, StaticVariableManager.currLane2Targets,
                        StaticVariableManager.currLane2TargetNum);
                    }
                    else
                    {
                        this.GetComponent<ReplayCounter>().RecordLane2Shot(bullet, StaticVariableManager.currLane2Targets);
                    }
                    break;
                case "3":
                    if (activeScene.ToLower().Contains("colorsequence"))
                    {
                        this.GetComponent<ReplayCounter>().RecordLane3Shot(bullet, StaticVariableManager.currLane3Targets,
                            StaticVariableManager.currLane3IndicatorColor);
                    }
                    else if (activeScene.ToLower().Contains("sequencenum"))
                    {
                        this.GetComponent<ReplayCounter>().RecordLane3Shot(bullet, StaticVariableManager.currLane3Targets,
                        StaticVariableManager.currLane3TargetNum);
                    }
                    else
                    {
                        this.GetComponent<ReplayCounter>().RecordLane3Shot(bullet, StaticVariableManager.currLane3Targets);
                    }
                    break;
            }
        }
    }

    private void ManageIPECTargetSignals(RaycastHit hit)
    {
        if (targetsFinished == false && targetFinished == false)
        {
            changeColor(hit);

            if (hit.transform.tag.ToLower().Contains("head"))
            {
                headShots++;
                if (activeScene.ToLower().Contains("pointbullseye"))
                {
                    HeadShots.text = ("Side Target: " + headShots.ToString());
                    if (hit.transform.name.ToLower().Contains("cover.target")) { WallTargetControl.requestMove(); }
                }
                else
                {
                    HeadShots.text = ("Head Shots: " + headShots.ToString());
                    WallTargetControl.requestMove();
                }

                hit.transform.gameObject.GetComponent<HeadTarget>().SendMessage("ApplyDamage", hit.transform.name);
            }
            else if (hit.transform.tag.ToLower().Contains("body"))
            {

                bodyShots++;
                if (activeScene.ToLower().Contains("pointbullseye"))
                {
                    BodyShots.text = ("Center Target: " + bodyShots.ToString());
                    if (hit.transform.name.ToLower().Contains("cover.target")) { WallTargetControl.requestMove(); }
                }
                else
                {
                    BodyShots.text = ("Body Shots: " + bodyShots.ToString());
                    WallTargetControl.requestMove();
                }

                hit.transform.gameObject.GetComponent<BodyTarget>().SendMessage("ApplyDamage", hit.transform.name);
            }

        }
    }
    void changeColor(RaycastHit hit)
    {
        /*float colorChangeTimer;
    float colorChangeTimerReset = 1;*/
        colorHolder = GameObject.FindGameObjectWithTag("IPECColor");
        currentTargetColr = hit.transform.gameObject.GetComponent<Renderer>().material;
        if(currentTargetColr != null) hitColor = colorHolder.transform.gameObject.GetComponent<Renderer>().material;

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

    private void ManageCorrectColorHit(RaycastHit hit, string lane)
    {
        string currentLaneColor = "";

        if(activeScene.ToLower().Contains("baloon"))
        {
            switch (lane)
            {
                case "lane 1":
                    currentLaneColor = StaticVariableManager.currentLane1Color;
                    if (hit.transform.name.ToLower().Contains(currentLaneColor.ToLower()))
                    {
                        StaticVariableManager.lane1CorrectTargetsHits++;
                    }
                    else
                    {
                        StaticVariableManager.lane1IncorrectTargetsHits++;
                    }
                    break;
                case "lane 2":
                    currentLaneColor = StaticVariableManager.currentLane2Color;
                    if (hit.transform.name.ToLower().Contains(currentLaneColor.ToLower()))
                    {
                        StaticVariableManager.lane2CorrectTargetsHits++;
                    }
                    else
                    {
                        StaticVariableManager.lane2IncorrectTargetsHits++;
                    }
                    break;
                case "lane 3":
                    currentLaneColor = StaticVariableManager.currentLane3Color;
                    if (hit.transform.name.ToLower().Contains(currentLaneColor.ToLower()))
                    {
                        StaticVariableManager.lane3CorrectTargetsHits++;
                    }
                    else
                    {
                        StaticVariableManager.lane3IncorrectTargetsHits++;
                    }
                    break;
            }

        }
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

    private bool sendHitSignal(RaycastHit hit)
    {
        if (activeScene.ToLower().Contains("ipec"))
        {
            if (TargetMessageReceive.body_head_ishit == false && !targetsFinished && !ammaFinished)
            {
                hit.transform.gameObject.GetComponent<TargetMessageReceive>().SendMessage("ApplyDamage", hit.transform.name);
            }
        }
        else if(activeScene.ToLower().Contains("lanesuspectshoot"))
        {
            if(hit.transform.name.ToLower().Contains("head"))
            {
                hit.transform.gameObject.GetComponent<HeadTarget>().SendMessage("ApplyDamage", hit.transform.tag);
            }
            else if (hit.transform.name.ToLower().Contains("body"))
            {
                hit.transform.gameObject.GetComponent<BodyTarget>().SendMessage("ApplyDamage", hit.transform.tag);
            }

            StaticVariableManager.isLane1BoardSet = false;
        }
        else if (activeScene.ToLower().Contains("ipscextended") )
        {
            print(hit.transform.name);

            if (hit.transform.gameObject.GetComponent<IPSCPlateHandler>().targetHolder.transform.name.Contains("Target 1"))
            {
                StaticVariableManager.numTarget1Hits++;

                if(StaticVariableManager.numTarget1Hits <= StaticVariableManager.numTarget1ShotsSetting)
                {
                    // print("Target 1: " + StaticVariableManager.numTarget1Hits);
                    hit.transform.gameObject.GetComponent<IPSCPlateHandler>().SendMessage("ApplyDamage", hit.transform.name);
                    if (StaticVariableManager.numTarget1Hits == StaticVariableManager.numTarget1ShotsSetting)
                        StaticVariableManager.isTarget1Complete = true;
                }
                
                if (StaticVariableManager.numTarget1Hits > StaticVariableManager.numTarget1ShotsSetting)
                {
                    StaticVariableManager.isTarget1Complete = true;
                    return true;
                }

            }

            if (hit.transform.gameObject.GetComponent<IPSCPlateHandler>().targetHolder.transform.name.Contains("Target 2"))
            {
                StaticVariableManager.numTarget2Hits++;

                if (StaticVariableManager.numTarget2Hits <= StaticVariableManager.numTarget2ShotsSetting)
                {
                    print("Target 2: " + StaticVariableManager.numTarget2Hits);
                    hit.transform.gameObject.GetComponent<IPSCPlateHandler>().SendMessage("ApplyDamage", hit.transform.name);
                    if (StaticVariableManager.numTarget2Hits == StaticVariableManager.numTarget2ShotsSetting)
                        StaticVariableManager.isTarget2Complete = true;
                }
                
                if (StaticVariableManager.numTarget2Hits > StaticVariableManager.numTarget2ShotsSetting)
                {
                    StaticVariableManager.isTarget2Complete = true;
                    return true;
                }

            }

            if (hit.transform.gameObject.GetComponent<IPSCPlateHandler>().targetHolder.transform.name.Contains("Target 3"))
            {
                StaticVariableManager.numTarget3Hits++;


                if (StaticVariableManager.numTarget3Hits <= StaticVariableManager.numTarget3ShotsSetting)
                {
                    print("Target 3: " + StaticVariableManager.numTarget3Hits);
                    hit.transform.gameObject.GetComponent<IPSCPlateHandler>().SendMessage("ApplyDamage", hit.transform.name);

                    if (StaticVariableManager.numTarget3Hits == StaticVariableManager.numTarget3ShotsSetting)
                        StaticVariableManager.isTarget3Complete = true;
                }
                
                if (StaticVariableManager.numTarget3Hits > StaticVariableManager.numTarget3ShotsSetting)
                {
                    StaticVariableManager.isTarget3Complete = true;
                    return true;
                }

            }

        }
        else if(activeScene.ToLower().Contains("targetrecognition") || activeScene.ToLower().Contains("decisionptshoot"))
        {
            // NA
        }
        else if (activeScene.Contains("VideoTraining"))
        {
            hit.transform.gameObject.GetComponent<TargetMessageReceive>().SendMessage("ApplyDamage", hit.transform.name); //
        }
        else if (activeScene.Contains("Bird"))
        {
            hit.transform.gameObject.GetComponent<BirdController>().SendMessage("ApplyDamage", hit.transform.name); //
        }
        else if (!activeScene.ToLower().Contains("ipec")) // && platesHit < num_set_shots
        {
            hit.transform.gameObject.GetComponent<TargetMessageReceive>().SendMessage("ApplyDamage", hit.transform.name); //
        }


        return false;
    }

    public void updateLane1ResponseTime(string label, string response_type)
    {
        label = updateLabel(label);

        //print("Updating...");

        switch(response_type)
        {
            case "c":
                if (startLane1ResponseCount)
                {
                    numLane1ResponseTimes++;
                    lane1ResponseTimeCounter += responseOverflowValue;
                    lane1ResponseTime.Add((lane1ResponseTimeCounter).ToString("0.00") + " sec" + ":" + label);

                    lane1ResponseTimeCounter = 0;
                }
                break;

            case "d":
                if (startLane1ResponseCount)
                {
                    numLane1ResponseTimes++;
                    lane1ResponseTimeCounter += responseOverflowValue;
                    lane1ResponseTime.Add((lane1ResponseTimeCounter).ToString("0.00") + " sec" + ":" + label);

                    lane1ResponseTimeCounter = 0;
                    startLane1ResponseCount = false;
                }
                break;
        }

    }
     
    public void updateLane2ResponseTime(string label, string response_type)
    {
        label = updateLabel(label);

        switch (response_type)
        {
            case "c":
                if (startLane2ResponseCount)
                {
                    numLane2ResponseTimes++;
                    lane2ResponseTimeCounter += responseOverflowValue;
                    lane2ResponseTime.Add((lane2ResponseTimeCounter).ToString("0.00") + " sec" + ":" + label);

                    lane2ResponseTimeCounter = 0;

                }
                break;

            case "d":
                if (startLane2ResponseCount)
                {
                    numLane2ResponseTimes++;
                    lane2ResponseTimeCounter += responseOverflowValue;
                    lane2ResponseTime.Add((lane2ResponseTimeCounter).ToString("0.00") + " sec" + ":" + label);

                    lane2ResponseTimeCounter = 0;
                    startLane2ResponseCount = false;
                }
                break;
        }
    }
     
    public void updateLane3ResponseTime(string label, string response_type)
    {
        label = updateLabel(label);

        switch (response_type)
        {
            case "c":
                if (startLane3ResponseCount)
                {
                    numLane3ResponseTimes++;
                    lane3ResponseTimeCounter += responseOverflowValue;
                    lane3ResponseTime.Add((lane3ResponseTimeCounter).ToString("0.00") + " sec" + ":" + label);

                    lane3ResponseTimeCounter = 0;

                }
                break;

            case "d":
                if (startLane3ResponseCount)
                {
                    numLane3ResponseTimes++;
                    lane3ResponseTimeCounter += responseOverflowValue;
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

        // print(startLane1Count);

        if (startLane1Count)
        {
            if(label == "H")
            {
                numLane1Splits++;
                lane1SplitTime.Add((lane1SplitTimeCounter).ToString("0.00") + " sec");

                lane1SplitTimeCounter = 0;
            }
            else
            {
                numLane1Splits++;
                lane1SplitTime.Add((lane1SplitTimeCounter).ToString("0.00") + " sec");
            }
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
                ManageSuspectTargetScoring(hit, "lane 1");
            }
        }
        if (activeScene.ToLower().Contains("2lane"))
        {
            if ((lane1TargetsComplete || StaticVariableManager.isLane1TargetMissed) && lane1Scored == false)
            {
                ManageSuspectTargetScoring(hit, "lane 2");
                //lane1Scored = true;
            }
            if ((lane2TargetsComplete || StaticVariableManager.isLane2TargetMissed) && lane2Scored == false)
            {
                ManageSuspectTargetScoring(hit, "lane 2");
                //lane2Scored = true;
            }

            if ((lane1TargetsComplete && lane2TargetsComplete) || (StaticVariableManager.isLane1TargetMissed && StaticVariableManager.isLane2TargetMissed))
            {
                //trainingPaused = true;
            }
        }

        
    }

    private void ManageSuspectTargetScoring(RaycastHit hit, string lane)
    {
        if(activeScene.ToLower().Contains("suspect"))
        {
            switch (lane)
            {
                case "lane 1":
                    if (hit.transform.tag.ToLower().Contains("nonthreat"))
                    {
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
                    break;
                case "lane 2":
                    if (hit.transform.tag.ToLower().Contains("nonthreat"))
                    {
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
                    break;
                case "lane 3":
                    // Empty
                    break;
            }
        }
    }


    private void AssignMissImpact(RaycastHit hit, GameObject impactGo, List<GameObject> impact_list)
    {
        //print("Test: " + hit.transform.name);
        impact_list.Add(impactGo);
        impactGo.GetComponent<BulletManager>().bullet_time_text.text = (timeActiveCounter).ToString("0.00") + " sec";
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
            // impactGo.SetActive(false);
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
        if(activeScene.ToLower().Contains("cyclic"))
        {
            //Variables
            string[] pointOptions;
            int pointValue;

            targetName = targetName.Substring(2);

            foreach (char c in targetName)
            {
                if (char.IsDigit(c))
                {
                    switch (c)
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
        }
        else
        {
            //Variables
            string[] pointOptions;
            int pointValue;

            pointOptions = point_lane[0].Split(',');
            point_lane[0] = ""; //clear
            pointValue = int.Parse(pointOptions[1]) + 1;
            point_lane[0] = pointOptions[0] + "," + pointValue;
        }

        return point_lane;
    }

    private void StopTraining()
    {

        //Pause();
        if (ManualVideoTracker.isVideoPlaying) ManualVideoTracker.ManageStartAndPause();

        // ⭐ CRITICAL: Reset FoodAllocationManager before stopping training
        // This ensures all food claims are released and the singleton is reset for the next scenario
        FoodAllocationManager.ResetInstance();

        trainingPaused = true;
        isScoreOpen = true;

        StaticVariableManager.isTrainingPause = trainingPaused;
        countDownStart.start_training = false;
        StaticVariableManager.isStopTraining = true;
        StaticVariableManager.startCountDown = false;
        // if(DropDown.scene_type == "3D") this.GetComponent<FirstPersonController>().UnlockCursor();

        targetFinished = true;
        targetsFinished = true;

        trainingStarted = false;
        ForestSound.Stop();
        ForestSound2.Stop();
        CitySound2.Stop();
        //standby_txt.SetActive(false);
        //ReloadIndicator.SetActive(false);
        sendEndless("stop");
        // manageBaloonScoring();

        lane1ActiveTimeCounter = lane1TimeActiveCounter;
        lane2ActiveTimeCounter = lane2TimeActiveCounter;
        lane3ActiveTimeCounter = lane3TimeActiveCounter;

        if (!dead && !activeScene.ToLower().Contains("range"))
        {
            DisplayLaneShotsFired();
            if (is3DScene) shooterScorePanel.SetActive(true);

            if (TypeOfTraining() == "3D animations") 
            {
                print("In 3D");

                GameOver = true;
                Pause();
                shooterScorePanel.SetActive(true);

                if (activeScene.ToLower().Contains("closeupsuspect"))
                {
                    float scoreResult = 0;

                    if (mainPlayerLives <= 0f && remainingNumEnemies >= 0f)
                    {
                        scoreResult = (scoreResult - (scoreResult * 0.5f));
                    }

                    if (scoreResult < 0)
                    {
                        scoreResult = 0;
                    }


                    if (StaticVariableManager.limitOverTime >= 0 && StaticVariableManager.numberNoneSuspectsHit == 0)
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

                    total_3d_training_time.text += " " + timeActiveCounter.ToString("0") + " sec";
                    total_3d_shots_fired.text += " " + shotsFired;
                    enemiesKilled.text += " " + enemyshot;
                    totalEnemies.text = "Incorrect Hit: " + StaticVariableManager.numberNoneSuspectsHit;
                    civiliansShot.text = "Shots Missed: " + numLane1ShotsMissed;
                    score.text = "Over time: " + StaticVariableManager.limitOverTime.ToString("0") + " sec";

                }
                else
                {
                    float Enemy_Civ_TotalShots = ((enemyshot * 2));
                    float percResult = (((((Enemy_Civ_TotalShots / shotsFired) * 60) - (remainingNumEnemies)) + ((enemyshot / totalNumberOfEnemies)) * 40));
                    float scoreResult = percResult;

                    if (mainPlayerLives <= 0f && remainingNumEnemies >= 0f)
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
                    enemiesKilled.text += " " + enemyshot + " out of " + totalNumberOfEnemies;
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

            }
            else if (TypeOfTraining() == "3D hunting")
            {
                score.enabled = true;
                if (activeScene.ToLower().Contains("direct"))
                {
                    totPlates = TestConditionsManager.totalAllowedHitShots;

                    float targetsKilled = StaticVariableManager.totalTargetAnimalsKilled;
                    float ShotsMissed = shotsFired - targetsKilled;
                    //float scoreResult = percResult;

                    if (ShotsMissed < 0)
                    {
                        ShotsMissed = 0;
                    }

                    total_3d_shots_fired.text += " " + shotsFired;
                    total_3d_training_time.text = "Time: " + timeActiveCounter.ToString("0") + " Seconds ";
                    enemiesKilled.text = "Targets Shot: " + targetsKilled;
                    civiliansShot.text = "Shots Missed : " + ShotsMissed;
                    score.text = "Head Shots : " + StaticVariableManager.totalHeadShots;

                    //plate_score = platesHit; // omp score

                    if (activeScene == "Outdoor_FOREST" || activeScene.Contains("range"))
                    {
                        civiliansShot.enabled = false;
                    }

                    //score.text = "SCORE : " + scoreResult.ToString("0");

                    if (StaticVariableManager.isBearKilledPlayer)
                    {
                        Result.text = "<color=#FF0000>Failed!! \nkilled by a Bear</color>";
                    }
                    else
                    {
                        Result.text = "...Complete...";
                    }
                }
                else if (activeScene.ToLower().Contains("bird"))
                {
                    totPlates = TestConditionsManager.totalAllowedHitShots;

                    float targetsKilled = StaticVariableManager.totalTargetAnimalsKilled;
                    float ShotsMissed = shotsFired - targetsKilled;
                    //float scoreResult = percResult;

                    if (ShotsMissed < 0)
                    {
                        ShotsMissed = 0;
                    }

                    total_3d_shots_fired.text += " " + shotsFired;
                    total_3d_training_time.text = "Time: " + timeActiveCounter.ToString("0") + " Seconds ";
                    enemiesKilled.text = "Targets Shot: " + targetsKilled;
                    civiliansShot.text = "Shots Missed : " + ShotsMissed;
                    // score.text = "Head Shots : " + StaticVariableManager.totalHeadShots;
                    score.enabled = false;

                    //plate_score = platesHit; // omp score

                    if (activeScene == "Outdoor_FOREST" || activeScene.Contains("range"))
                    {
                        civiliansShot.enabled = false;
                    }

                    //score.text = "SCORE : " + scoreResult.ToString("0");

                    if (StaticVariableManager.isBearKilledPlayer)
                    {
                        Result.text = "<color=#FF0000>Failed!! \nkilled by a Bear</color>";
                    }
                    else
                    {
                        Result.text = "...Complete...";
                    }
                }
                else
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
                    if( TestConditionsManager.animalName != "...select...")
                    {
                        enemiesKilled.text = TestConditionsManager.animalName + "s Shot: " + targetsKilled;
                    } 
                    else
                    {
                        enemiesKilled.text = "Animals Killed: " + targetsKilled;
                    }
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
            }
            else if (TypeOfTraining() == "plate or IPSC shoot")
            {
                managePlateTargetScoring();
            }
            else if (TypeOfTraining() == "baloon shoot")
            {
                manageBaloonScoring();
            }
            else if (TypeOfTraining() == "board shoot")
            {
                manageSuspectShootScoring();
            }
            else if (TypeOfTraining() == "video shoot")
            {
                manageVideoShootScoring();
            }
            else
            {
                managePlateTargetScoring();
            }

            if (Scoring.simulation_type == "training")
            {
                saveScore();
            }
            
            dead = true;
        }

        StaticVariableManager.isScoreDataSet = true;
    }

    private string TypeOfTraining()
    {
        string typeOfTraining = "";

        // 3d scenes
        if (activeScene.ToLower().Contains("outdoor_forest") || activeScene.ToLower().Contains("openplain") || activeScene.ToLower().Contains("mall") || activeScene.ToLower().Contains("container")
            || activeScene.ToLower().Contains("parking") || activeScene.ToLower().Contains("restaurant") || activeScene.ToLower().Contains("closeupsuspect"))
        {
            typeOfTraining = "3D animations";
        }
        // Hunting scenes
        else if (activeScene.ToLower().Contains("hunting"))
        {
            typeOfTraining = "3D hunting";
        }
        // Basic scenes
        else if (activeScene.ToLower().Contains("targetpopup") || activeScene.ToLower().Contains("threatening") || activeScene.ToLower().Contains("point")
            || activeScene.ToLower().Contains("animaltarget") || activeScene.ToLower().Contains("hiddentarget") || activeScene.ToLower().Contains("risingplate")
            || activeScene.ToLower().Contains("dueling") || activeScene.ToLower().Contains("rifflepole") || activeScene.ToLower().Contains("ipec")
            || activeScene.ToLower().Contains("fallingplat") || activeScene.ToLower().Contains("claypigeon") || activeScene.ToLower().Contains("dice") ||
            activeScene.ToLower().Contains("shell") || activeScene.ToLower().Contains("risingshape") || activeScene.ToLower().Contains("distancesimulator")
            || activeScene.ToLower().Contains("block") || activeScene.ToLower().Contains("cargame") || activeScene.ToLower().Contains("cyclic")
            || activeScene.ToLower().Contains("colorsequence") || activeScene.ToLower().Contains("expressionshoot") || activeScene.ToLower().Contains("sequencenum"))
        {
            typeOfTraining = "plate or IPSC shoot";
        }
        // Baloon
        else if (activeScene.ToLower().Contains("baloon"))
        {
            typeOfTraining = "baloon shoot";
        }
        // Board scenes
        else if (activeScene.ToLower().Contains("suspectshoot") || activeScene.ToLower().Contains("targetrecognition") || activeScene.ToLower().Contains("decisionptshoot")) // 
        {
            typeOfTraining = "board shoot";
        }
        else if(activeScene.Contains("VideoTraining") || activeScene.Contains("VideoTransition"))
        {
            typeOfTraining = "video shoot";
        }
        else
        {
            typeOfTraining = "Unknown";
        }

        return typeOfTraining;
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
            GetTrainees.trainee_name = "shooter 1";
        }
    }


    private void manageSuspectShootScoring()
    {
        print("Managing suspect score");

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
            lane3ResponseTimeString = generateResponseTimeString(lane3ResponseTime);

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
            lane2ResponseTimeString = generateResponseTimeString(lane2ResponseTime);
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
            lane1ResponseTimeString = generateResponseTimeString(lane1ResponseTime);

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

        if (activeScene.ToLower().Contains("1lane"))
        {

            if(StaticVariableManager.numCurrThreats <= 0)
            {
                lane1Result.text = "Civilians Shot: " + StaticVariableManager.numCiviliansHit + "\n";
            }
            else
            {
                lane1Result.text = "Threats Shot: " + StaticVariableManager.numSuspectsHit + "\n"
                    + "Civilians Shot: " + StaticVariableManager.numCiviliansHit + "\n";
            }


                // + "Lethal Shots: " + StaticVariableManager.numSuspectHeadShot + "\n"
                // + "Non-Lethal Shots: " + StaticVariableManager.numSuspectBodyShot + "\n";

            if(activeScene.ToLower().Contains("targetrecognition"))
            {
                if (StaticVariableManager.numCiviliansHit == 0 && StaticVariableManager.numCurrThreats == StaticVariableManager.numSuspectsHit)
                {
                    lane1Result.text += "<b> Success </b>";
                }
                else if (StaticVariableManager.numCiviliansHit > 0)
                {
                    lane1Result.text += "<b> Failed. Civilian shot. </b>";
                }
                else if(StaticVariableManager.numCurrThreats != StaticVariableManager.numSuspectsHit)
                {
                    /*lane1Result.text += $"{StaticVariableManager.threatCount} threats shot out of {StaticVariableManager.numCurrThreats}\n" +
                        $"<b> Failed </b>";*/
                }
            }
            else if(activeScene.ToLower().Contains("decisionptshoot"))
            {
                if (StaticVariableManager.isCorrectTRTRequence)
                {
                    lane1Result.text += "<b> Correct Sequence </b>";
                }
                else
                {
                    lane1Result.text += "<b> Incorrect Sequence </b>";
                }
            }

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
    private void manageVideoShootScoring()
    {
        // print("Managing suspect score");

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
            lane3ResponseTimeString = generateResponseTimeString(lane3ResponseTime);

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
            lane2ResponseTimeString = generateResponseTimeString(lane2ResponseTime);
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
            lane1ResponseTimeString = generateResponseTimeString(lane1ResponseTime);

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

        if(activeScene.Contains("VideoTraining"))
        {
            if (StaticVariableManager.isThreatVideo)
            {
                if (StaticVariableManager.isVideoShotFired)
                {
                    if (StaticVariableManager.isThreatShot && !StaticVariableManager.isNoneThreatShot)
                    {
                        string reactionTime = lane1ResponseTime[0].Split(":")[0];

                        if (StaticVariableManager.currVideoThreats == 1)
                        {
                            lane1Result.text = $"shot {StaticVariableManager.numVideoThreatsShot} assailant of {StaticVariableManager.currVideoThreats}\n";
                        }
                        else if (StaticVariableManager.currVideoThreats > 1)
                        {
                            lane1Result.text = $"shot {StaticVariableManager.numVideoThreatsShot} assailants of {StaticVariableManager.currVideoThreats}\n";
                        }

                        lane1Result.text += "Reaction Time: " + reactionTime + "\n";

                        if (StaticVariableManager.currVideoThreats == StaticVariableManager.numVideoThreatsShot)
                        {
                            lane1Result.text += "<b> Success <b>";
                        }
                        else
                        {
                            lane1Result.text += "<b> Failed <b>";
                            deadShader.SetActive(true);
                        }

                        // "<b> Success <b>";
                    }
                    else if (StaticVariableManager.isNoneThreatShot)
                    {
                        string reactionTime = lane1ResponseTime[0].Split(":")[0];

                        if (StaticVariableManager.currVideoThreats == 1)
                        {
                            lane1Result.text = $"shot {StaticVariableManager.numVideoThreatsShot} assailant of {StaticVariableManager.currVideoThreats}\n";
                        }
                        else if (StaticVariableManager.currVideoThreats > 1)
                        {
                            lane1Result.text = $"shot {StaticVariableManager.numVideoThreatsShot} assailants of {StaticVariableManager.currVideoThreats}\n";
                        }

                        lane1Result.text += "You shot a non-threat! \n" +
                            //"Reaction Time: " + reactionTime + "\n" +
                            "<b> Failed <b>";

                        // deadShader.SetActive(true);
                    }
                    else if (!StaticVariableManager.isStartVideoTime)
                    {
                        lane1Result.text = "Shots fired early\n" +
                            "No threat yet \n" +
                            // "Reaction Time: NA \n" +
                            "<b> Failed <b>";
                    }
                    else
                    {
                        lane1Result.text = "Assailant not shot \n" +
                          // "Reaction Time: " + reactionTime + "\n" +
                          "<b> Failed <b>";
                        deadShader.SetActive(true);
                    }
                }
                else
                {
                    if (StaticVariableManager.isTrainingEndReached)
                    {
                        lane1Result.text = "No Reaction \n" +
                            "<b> Failed <b>";
                    }
                    else
                    {
                        lane1Result.text = "No Reaction \n" +
                            "<b> Failed <b>";
                    }
                    deadShader.SetActive(true);
                }


                lane1ResultPanel.SetActive(true);
            }
            else
            {

                if (StaticVariableManager.isVideoShotFired)
                {
                    if (StaticVariableManager.isNoneThreatShot)
                    {
                        // string reactionTime = lane1ResponseTime[0].Split(":")[0];

                        lane1Result.text = "Civilian shot \n" +
                                "No threat visible \n" +
                                "<b> Failed <b>";

                    }
                    else
                    {
                        lane1Result.text = "Shots fired \n" +
                            "No threat visible \n" +
                            "<b> Failed <b>";
                    }
                }
                else
                {
                    if (!StaticVariableManager.isNoneThreatShot)
                    {
                        lane1Result.text = "Civilian not shot \n" +
                            // "Reaction Time: NA \n" +
                            "<b> Success <b>";

                    }
                }

                lane1ResultPanel.SetActive(true);
            }
        }
        else if(activeScene.Contains("VideoTransitionTraining"))
        {
            // StaticVariableManager.isTrainingEndReached
            // StaticVariableManager.currVideoThreats
            // assailant of {StaticVariableManager.currVideoThreats}
            if (StaticVariableManager.numVideoThreatsShot == 1)
            {
                lane1Result.text = $" Assailants Shot: {StaticVariableManager.numVideoThreatsShot} \n";
            }
            else
            {
                lane1Result.text = $" Assailants Shot: {StaticVariableManager.numVideoThreatsShot} \n";
            }

            // Non Threats shot: {StaticVariableManager.numVideoNoneThreatsShot}
            if (StaticVariableManager.isNoneThreatShot) 
                lane1Result.text += $"Non Threats shot: {StaticVariableManager.numVideoNoneThreatsShot}\n";

            if (StaticVariableManager.isTrainingEndReached)
            {
                lane1Result.text += "<b> Training Complete <b>"; // \nSuccess
            }
            else
            {
                lane1Result.text += "<b> Training Complete <b>"; // \nSuccess
                // lane1Result.text += " <b> Training Incomplete "; // \nFailed<b> 
                // deadShader.SetActive(true);
            }

            lane1ResultPanel.SetActive(true);
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

        if (activeScene.ToLower().Contains("expressionshoot"))
        {
            isCIScene = true;
        }
        else if (activeScene.ToLower().Contains("colorsequence") || activeScene.ToLower().Contains("add") 
            || activeScene.ToLower().Contains("sequencenum"))
        {
            isCIScene = true;
        }
        else if (activeScene.ToLower().Contains("cargame"))
        {
            HandleCarGameScoring();
        }
        else if (activeScene.ToLower().Contains("duelingtree"))
        {
            // print("dueling tree complete: " + StaticVariableManager.duelingTreeS1Points);
            // print("dueling tree complete: " + StaticVariableManager.duelingTreeS2Points);

            if (StaticVariableManager.duelingTreeS1Points > StaticVariableManager.duelingTreeS2Points)
            {
                //winnerScore_carGame.text = ("Winner: Shooter 1");
            }
            else if (StaticVariableManager.duelingTreeS2Points > StaticVariableManager.duelingTreeS1Points)
            {
                //winnerScore_carGame.text = ("Winner: Shooter 2");
            }
            else if (StaticVariableManager.duelingTreeS1Points == StaticVariableManager.duelingTreeS2Points)
            {
                //winnerScore_carGame.text = ("");
            }

            if (winnerScore_carGame != null) winnerScore_carGame.color = Color.black;
            // winnerScore_carGame.text = (StaticVariableManager.correctTargetsHits + " CORRECT \n" + StaticVariableManager.wrongTargetsHits + " INCORRECT");
        }
        else
        {
            //winnerScore_carGame.text = (StaticVariableManager.correctNumberHits + " CORRECT \n" + StaticVariableManager.wrongNumberHits + " INCORRECT");
            //winnerScore_carGame.color = Color.black;
        }

        if (laneSelected == 3 || !adminScoreUpdated)
        {
            //print("RRE: lane3Selected");
            lane_number_Txt.text = "Lane 3";
            float totalPoints = CalculatePoints(point_lane_3);
            totalPoints = CalculateCIPoints("lane 3", totalPoints);

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
            lane3ResponseTimeString = generateResponseTimeString(lane3ResponseTime);

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

            float totalPoints = CalculatePoints(point_lane_2);
            totalPoints = CalculateCIPoints("lane 2", totalPoints);

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
            lane2ResponseTimeString = generateResponseTimeString(lane2ResponseTime);

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

            float totalPoints = CalculatePoints(point_lane_1);
            totalPoints = CalculateCIPoints("lane 1", totalPoints);

            lane1TrainingScorePoints = totalPoints;
            percentage1 = generatePercentage(numLane1ShotsFired, numLane1ShotsMissed);

            if(activeScene.ToLower().Contains("rifflepole"))
            {
                lane1PointsHit = TargetController.num_pass_hits;
                numLane1ShotsMissed = numLane1ShotsMissed + TargetController.num_fail_hits;
            }
            else if (activeScene.ToLower().Contains("animaltarget"))
            {
                lane1TrainingScorePoints = AnimalTargetPointManager.pointsCounter;
            }

            // print("Test: active time is " + lane1ActiveTimeCounter);
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
            lane1ResponseTimeString = generateResponseTimeString(lane1ResponseTime);

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

        // print("Point Reached...");

        if (adminScoreUpdated == false)
        {
            constructSaveData(percentage1, percentage2, percentage3);
            updateScoreDisplay();
            adminScoreUpdated = true;
        }
    }
    private void HandleCarGameScoring()
    {

        StaticVariableManager.redCarMoveSteps = 0;
        StaticVariableManager.blueCarMoveSteps = 0;
        StaticVariableManager.greenCarMoveSteps = 0;

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

    }

    private void UpdateCarGamePoints(string lane, TextMeshProUGUI textComponent)
    {
        switch(lane)
        {
            case "lane 1":
                textComponent.text = StaticVariableManager.redCarPoints.ToString();
                // print("Point reached: " + StaticVariableManager.redCarPoints.ToString());
                break;
            case "lane 2":
                textComponent.text = StaticVariableManager.blueCarPoints.ToString();
                // print("Point reached: " + StaticVariableManager.blueCarPoints.ToString());
                break;
            case "lane 3":
                textComponent.text = StaticVariableManager.greenCarPoints.ToString();
                // print("Point reached: " + StaticVariableManager.greenCarPoints.ToString());
                break;
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
        isCIScene = true; // Enable correct and incorrect score.

        if (activeScene.ToLower().Contains("2lane"))
        {
            lane1StrikeOutSignal.SetActive(false);
            lane2StrikeOutSignal.SetActive(false);
        }

        if (laneSelected == 3 || !adminScoreUpdated)
        {
            lane_number_Txt.text = "Lane 3";

            float totalPoints = CalculateCIPoints("lane 3", 0);

            if (totalPoints < 0)
            {
                totalPoints = 0;
            }
            lane3TrainingScorePoints = totalPoints;
            percentage3 = generatePercentage(numLane3ShotsFired, numLane3ShotsMissed);
            lane3ResponseTimeString = generateResponseTimeString(lane3ResponseTime);
            lane3SplitTimeString = generateSpliTimeString(lane3SplitTime, lane3SplitTimeDisplay, ref lane3SplitTimeSet);

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

            float totalPoints = CalculateCIPoints("lane 2", 0);

            if (totalPoints < 0)
            {
                totalPoints = 0;
            }
            lane2TrainingScorePoints = totalPoints;
            percentage2 = generatePercentage(numLane2ShotsFired, numLane2ShotsMissed);
            lane2ResponseTimeString = generateResponseTimeString(lane2ResponseTime);
            lane2SplitTimeString = generateSpliTimeString(lane2SplitTime, lane2SplitTimeDisplay, ref lane2SplitTimeSet);

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

            float totalPoints = CalculateCIPoints("lane 1", 0);

            if (totalPoints < 0)
            {
                totalPoints = 0;
            }
            lane1TrainingScorePoints = totalPoints;
            percentage1 = generatePercentage(numLane1ShotsFired, numLane1ShotsMissed);
            lane1ResponseTimeString = generateResponseTimeString(lane1ResponseTime);
            lane1SplitTimeString = generateSpliTimeString(lane1SplitTime, lane1SplitTimeDisplay, ref lane1SplitTimeSet);

            Location.text = "Run Time: " + lane1ActiveTimeCounter.ToString("0.0") + " Sec";
            enemiesKilled.text = "Hit Shots: " + lane1PointsHit;
            civiliansShot.text = "Total points: " + totalPoints;
            strikesScoreText.text = "Shots Missed : " + numLane1ShotsMissed.ToString("0");
            Result.text = percentage1.ToString("0.0") + "%";

            admin_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";
            shooter_Lane1_PercentageTxt.text = percentage1.ToString("0.0") + "%";


            lane1SplitTimeDisplay.text = "No split time.";
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

    private float CalculateCIPoints(string lane, float defaultValue)
    {
        float points = defaultValue;

        if(isCIScene)
        {
            switch (lane)
            {
                case "lane 1":
                    points = StaticVariableManager.lane1CorrectTargetsHits * 10;
                    break;
                case "lane 2":
                    points = StaticVariableManager.lane2CorrectTargetsHits * 10;
                    break;
                case "lane 3":
                    points = StaticVariableManager.lane3CorrectTargetsHits * 10;
                    break;
            }
        }

        return points;
    }

    private float CalculatePoints( List<string> lane_points)
    {
        string[] pointOptions;
        float totalPoints = 0.0f;

        foreach (string line in lane_points)
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

        if(activeScene.ToLower().Contains("cyclic"))
        {
            totalPoints = (point_1_hits * 10) + (point_2_hits * 5) + (point_3_hits * 2);
        }
        else
        {
            totalPoints = (point_1_hits * 2);
        }

        return totalPoints;
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
            if(isCIScene) admin_Lane1_CorrectHitsTxt.text = StaticVariableManager.lane1CorrectTargetsHits.ToString();
            if (isCIScene) admin_Lane1_IncorrectHitsTxt.text = StaticVariableManager.lane1IncorrectTargetsHits.ToString();

            shooter_Lane1_Header.text = lane1Header[0];  //Header
            shooter_Lane1_SplitTime_Header.text = lane1Header[0];  //Header
            shooter_Lane1_ShotsFiredTxt.text = numLane1ShotsFired.ToString("0");
            shooter_Lane1_ShotsMissedTxt.text = numLane1ShotsMissed.ToString("0");
            shooter_Lane1_HitTimeTxt.text = lane1ActiveTimeCounter.ToString("0.0") + " sec";
            shooter_Lane1_HitShotsTxt.text = lane1PointsHit.ToString("0");
            shooter_Lane1_ScoreTxt.text = lane1TrainingScorePoints.ToString("0");
            if (isCIScene) shooter_Lane1_CorrectHitsTxt.text = StaticVariableManager.lane1CorrectTargetsHits.ToString();
            if (isCIScene) shooter_Lane1_IncorrectHitsTxt.text = StaticVariableManager.lane1IncorrectTargetsHits.ToString();

            if (activeScene.Contains("LaneCarGame")) // Assign the proper Car Game points
            {
                UpdateCarGamePoints("lane 1", admin_Lane1_ScoreTxt);
                UpdateCarGamePoints("lane 1", shooter_Lane1_ScoreTxt);
            }

            ManageAdminShooterScorePanel();
        }
        if (activeScene.ToLower().Contains("2lane"))
        {
            admin_Lane1_Header.text = lane1Header[0];  //Header
            admin_Lane1_ShotsFiredTxt.text = numLane1ShotsFired.ToString("0");
            admin_Lane1_ShotsMissedTxt.text = numLane1ShotsMissed.ToString("0");
            admin_Lane1_HitTimeTxt.text = lane1ActiveTimeCounter.ToString("0.0") + " sec";
            admin_Lane1_HitShotsTxt.text = lane1PointsHit.ToString("0");
            admin_Lane1_ScoreTxt.text = lane1TrainingScorePoints.ToString("0");
            if (isCIScene) admin_Lane1_CorrectHitsTxt.text = StaticVariableManager.lane1CorrectTargetsHits.ToString();
            if (isCIScene) admin_Lane1_IncorrectHitsTxt.text = StaticVariableManager.lane1IncorrectTargetsHits.ToString();
            admin_Lane2_Header.text = lane2Header[0];  //Header
            admin_Lane2_ShotsFiredTxt.text = numLane2ShotsFired.ToString("0");
            admin_Lane2_ShotsMissedTxt.text = numLane2ShotsMissed.ToString("0");
            admin_Lane2_HitTimeTxt.text = lane2ActiveTimeCounter.ToString("0.0") + " sec";
            admin_Lane2_HitShotsTxt.text = lane2PointsHit.ToString("0");
            admin_Lane2_ScoreTxt.text = lane2TrainingScorePoints.ToString("0");
            if (isCIScene) admin_Lane2_CorrectHitsTxt.text = StaticVariableManager.lane2CorrectTargetsHits.ToString();
            if (isCIScene) admin_Lane2_IncorrectHitsTxt.text = StaticVariableManager.lane2IncorrectTargetsHits.ToString();

            shooter_Lane1_Header.text = lane1Header[0];  //Header
            shooter_Lane1_SplitTime_Header.text = lane1Header[0];  //Header
            shooter_Lane1_ShotsFiredTxt.text = numLane1ShotsFired.ToString("0");
            shooter_Lane1_ShotsMissedTxt.text = numLane1ShotsMissed.ToString("0");
            shooter_Lane1_HitTimeTxt.text = lane1ActiveTimeCounter.ToString("0.0") + " sec";
            shooter_Lane1_HitShotsTxt.text = lane1PointsHit.ToString("0");
            shooter_Lane1_ScoreTxt.text = lane1TrainingScorePoints.ToString("0");
            if (isCIScene) shooter_Lane1_CorrectHitsTxt.text = StaticVariableManager.lane1CorrectTargetsHits.ToString();
            if (isCIScene) shooter_Lane1_IncorrectHitsTxt.text = StaticVariableManager.lane1IncorrectTargetsHits.ToString();
            shooter_Lane2_Header.text = lane2Header[0];  //Header
            shooter_Lane2_SplitTime_Header.text = lane2Header[0];  //Header
            shooter_Lane2_ShotsFiredTxt.text = numLane2ShotsFired.ToString("0");
            shooter_Lane2_ShotsMissedTxt.text = numLane2ShotsMissed.ToString("0");
            shooter_Lane2_HitTimeTxt.text = lane2ActiveTimeCounter.ToString("0.0") + " sec";
            shooter_Lane2_HitShotsTxt.text = lane2PointsHit.ToString("0");
            shooter_Lane2_ScoreTxt.text = lane2TrainingScorePoints.ToString("0");
            if (isCIScene) shooter_Lane2_CorrectHitsTxt.text = StaticVariableManager.lane2CorrectTargetsHits.ToString();
            if (isCIScene) shooter_Lane2_IncorrectHitsTxt.text = StaticVariableManager.lane2IncorrectTargetsHits.ToString();

            if (activeScene.Contains("LaneCarGame")) // Assign the proper Car Game points
            {
                UpdateCarGamePoints("lane 1", admin_Lane1_ScoreTxt);
                UpdateCarGamePoints("lane 1", shooter_Lane1_ScoreTxt);

                UpdateCarGamePoints("lane 2", admin_Lane2_ScoreTxt);
                UpdateCarGamePoints("lane 2", shooter_Lane2_ScoreTxt);
            }

            ManageAdminShooterScorePanel();

            // print("Point Reached...");
        }
        if (activeScene.ToLower().Contains("3lane"))
        {
            admin_Lane1_Header.text = lane1Header[0];  //Header
            admin_Lane1_ShotsFiredTxt.text = numLane1ShotsFired.ToString("0");
            admin_Lane1_ShotsMissedTxt.text = numLane1ShotsMissed.ToString("0");
            admin_Lane1_HitTimeTxt.text = lane1ActiveTimeCounter.ToString("0.0") + " sec";
            admin_Lane1_HitShotsTxt.text = lane1PointsHit.ToString("0");
            admin_Lane1_ScoreTxt.text = lane1TrainingScorePoints.ToString("0");
            if (isCIScene) admin_Lane1_CorrectHitsTxt.text = StaticVariableManager.lane1CorrectTargetsHits.ToString();
            if (isCIScene) admin_Lane1_IncorrectHitsTxt.text = StaticVariableManager.lane1IncorrectTargetsHits.ToString();
            admin_Lane2_Header.text = lane2Header[0];  //Header
            admin_Lane2_ShotsFiredTxt.text = numLane2ShotsFired.ToString("0");
            admin_Lane2_ShotsMissedTxt.text = numLane2ShotsMissed.ToString("0");
            admin_Lane2_HitTimeTxt.text = lane2ActiveTimeCounter.ToString("0.0") + " sec";
            admin_Lane2_HitShotsTxt.text = lane2PointsHit.ToString("0");
            admin_Lane2_ScoreTxt.text = lane2TrainingScorePoints.ToString("0");
            if (isCIScene) admin_Lane2_CorrectHitsTxt.text = StaticVariableManager.lane2CorrectTargetsHits.ToString();
            if (isCIScene) admin_Lane2_IncorrectHitsTxt.text = StaticVariableManager.lane2IncorrectTargetsHits.ToString();
            admin_Lane3_Header.text = lane3Header[0];  //Header
            admin_Lane3_ShotsFiredTxt.text = numLane3ShotsFired.ToString("0");
            admin_Lane3_ShotsMissedTxt.text = numLane3ShotsMissed.ToString("0");
            admin_Lane3_HitTimeTxt.text = lane3ActiveTimeCounter.ToString("0.0") + " sec";
            admin_Lane3_HitShotsTxt.text = lane3PointsHit.ToString("0");
            admin_Lane3_ScoreTxt.text = lane3TrainingScorePoints.ToString("0");
            if (isCIScene) admin_Lane3_CorrectHitsTxt.text = StaticVariableManager.lane3CorrectTargetsHits.ToString();
            if (isCIScene) admin_Lane3_IncorrectHitsTxt.text = StaticVariableManager.lane3IncorrectTargetsHits.ToString();

            shooter_Lane1_Header.text = lane1Header[0];  //Header
            shooter_Lane1_SplitTime_Header.text = lane1Header[0];  //Header
            shooter_Lane1_ShotsFiredTxt.text = numLane1ShotsFired.ToString("0");
            shooter_Lane1_ShotsMissedTxt.text = numLane1ShotsMissed.ToString("0");
            shooter_Lane1_HitTimeTxt.text = lane1ActiveTimeCounter.ToString("0.0") + " sec";
            shooter_Lane1_HitShotsTxt.text = lane1PointsHit.ToString("0");
            shooter_Lane1_ScoreTxt.text = lane1TrainingScorePoints.ToString("0");
            if (isCIScene) shooter_Lane1_CorrectHitsTxt.text = StaticVariableManager.lane1CorrectTargetsHits.ToString();
            if (isCIScene) shooter_Lane1_IncorrectHitsTxt.text = StaticVariableManager.lane1IncorrectTargetsHits.ToString();
            shooter_Lane2_Header.text = lane2Header[0];  //Header
            shooter_Lane2_SplitTime_Header.text = lane2Header[0];  //Header
            shooter_Lane2_ShotsFiredTxt.text = numLane2ShotsFired.ToString("0");
            shooter_Lane2_ShotsMissedTxt.text = numLane2ShotsMissed.ToString("0");
            shooter_Lane2_HitTimeTxt.text = lane2ActiveTimeCounter.ToString("0.0") + " sec";
            shooter_Lane2_HitShotsTxt.text = lane2PointsHit.ToString("0");
            shooter_Lane2_ScoreTxt.text = lane2TrainingScorePoints.ToString("0");
            if (isCIScene) shooter_Lane2_CorrectHitsTxt.text = StaticVariableManager.lane2CorrectTargetsHits.ToString();
            if (isCIScene) shooter_Lane2_IncorrectHitsTxt.text = StaticVariableManager.lane2IncorrectTargetsHits.ToString();
            shooter_Lane3_Header.text = lane3Header[0];  //Header
            shooter_Lane3_SplitTime_Header.text = lane3Header[0];  //Header
            shooter_Lane3_ShotsFiredTxt.text = numLane3ShotsFired.ToString("0");
            shooter_Lane3_ShotsMissedTxt.text = numLane3ShotsMissed.ToString("0");
            shooter_Lane3_HitTimeTxt.text = lane3ActiveTimeCounter.ToString("0.0") + " sec";
            shooter_Lane3_HitShotsTxt.text = lane3PointsHit.ToString("0");
            shooter_Lane3_ScoreTxt.text = lane3TrainingScorePoints.ToString("0");
            if (isCIScene) shooter_Lane3_CorrectHitsTxt.text = StaticVariableManager.lane3CorrectTargetsHits.ToString();
            if (isCIScene) shooter_Lane3_IncorrectHitsTxt.text = StaticVariableManager.lane3IncorrectTargetsHits.ToString();

            if (activeScene.Contains("LaneCarGame")) // Assign the proper Car Game points
            {
                UpdateCarGamePoints("lane 1", admin_Lane1_ScoreTxt);
                UpdateCarGamePoints("lane 1", shooter_Lane1_ScoreTxt);

                UpdateCarGamePoints("lane 2", admin_Lane2_ScoreTxt);
                UpdateCarGamePoints("lane 2", shooter_Lane2_ScoreTxt);

                UpdateCarGamePoints("lane 3", admin_Lane3_ScoreTxt);
                UpdateCarGamePoints("lane 3", shooter_Lane3_ScoreTxt);
            }

            ManageAdminShooterScorePanel();
        }
    }
    private void ManageAdminShooterScorePanel()
    {
        if(activeScene.ToLower().Contains("cargame") || activeScene.ToLower().Contains("baloon") ||
            activeScene.ToLower().Contains("claypigeon") || activeScene.ToLower().Contains("risingshape") 
            || activeScene.ToLower().Contains("colorsequence") || activeScene.ToLower().Contains("sequencenum"))
        {
            adminScorePanel.SetActive(true);
            shooterScorePanel.SetActive(true);
        }
        else
        {
            //adminScorePanel.SetActive(true);
            //shooterScorePanel.SetActive(true);
        }
    }
    private string generateResponseTimeString(List<string> laneResponseTime)
    {
        int pos = 1;
        string tempResponseString = "";

        //print("RRE: Response data size is " + laneResponseTime.Count);
        foreach (string time in laneResponseTime)
        {
            
            if (pos <= laneResponseTime.Count)
            {
                //print("RRE: time" + tim);
                tempResponseString += time;
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

    private string generateSpliTimeString( List<string> splitTimes, TextMeshProUGUI textDisplay, ref bool isSplitTimeSet)
    {
        string splitTimeString = "";
        int pos = 1;
        foreach (string time in splitTimes)
        {
            if (isSplitTimeSet == false)
            {
                textDisplay.text = "";
                isSplitTimeSet = true;
            }
            textDisplay.text += time + "\n";

            splitTimeString += time;
            if (pos < lane1SplitTime.Count)
            {
                splitTimeString += ";";
                pos++;
            }

        }

        return splitTimeString;
    }

    private void constructSaveData(float perc1, float perc2, float perc3)
    {
        Scoring.simulation_type = "training";
        if (Scoring.simulation_type == "training")
        {
            // print("Test: Trainee - " + lane1TraineeName);

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

            //print("Reaction time: " + lane1ResponseTimeString);
            //print("Split time: " + lane1SplitTimeString);

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
            totalNumberOfEnemies = 10;
        }
        else
        {
            totalNumberOfEnemies = Random.Range(3, 8);
        }


        if (activeScene.ToLower().Contains("forest"))
        {
            totalNumberOfEnemies = totalNumberOfEnemies * 2;
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

        for (int i = 0; i < totalNumberOfEnemies; i++)
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
        if (mainPlayerLives <= 0f || shootingTimeOut <= 0f || remainingNumEnemies <= 0f )
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
        else if (mainPlayerLives > 0f || shootingTimeOut > 0f || remainingNumEnemies > 0f)
        {
            if(activeScene != "BlockShooting")
            {
                GameOver = false;
            }
        }
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

    public void sendEndless(string testStr)
    {
        try
        {
            IP = UDP_ClientIP_Address;
            //print("UDP IP IS " + IP);
            Sportnum = 55552;
            udpClientS = new UdpClient();
            //----------------------------
            // Sending
            //----------------------------
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), Sportnum);

            // Daten mit der UTF8-Kodierung in das Bin rformat kodieren.
            byte[] data = Encoding.UTF8.GetBytes(testStr);
            // print("sending " + testStr + " to " + remoteEndPoint);
            // Den message zum Remote-Client senden.
            udpClientS.Send(data, data.Length, remoteEndPoint);
            udpClientS.Send(data, data.Length, remoteEndPoint);
            udpClientS.Send(data, data.Length, remoteEndPoint);
            udpClientS.Send(data, data.Length, remoteEndPoint);
            udpClientS.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception e)
        {
            // Pass
            print("Error: " + e);
        }
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
        totalNumberOfEnemies = 0;
        mainPlayerLives = 10;
        numberOfCivilians = 0;
        shotsFired = 0;
        civilianShot = 0;

        lane1ShootDelayTimer = delaySetting;
        lane2ShootDelayTimer = delaySetting;
        lane3ShootDelayTimer = delaySetting;
    }
    private void ResetStaticVariables()
    {
        StaticVariableManager.isGunFired = false;
        // StaticVariableManager.recentPoints.Clear();

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
        StaticVariableManager.greenCarPoints = 0;
        StaticVariableManager.redCarCompletedPoints = 0;
        StaticVariableManager.blueCarCompletedPoints = 0; //
        StaticVariableManager.greenCarCompletedPoints = 0;
        StaticVariableManager.redCarPlateHit = false;
        StaticVariableManager.blueCarPlateHit = false;
        StaticVariableManager.greenCarPlateHit = false;
        StaticVariableManager.redCarMoveSteps = 0;
        StaticVariableManager.blueCarMoveSteps = 0;
        StaticVariableManager.greenCarMoveSteps = 0;
        StaticVariableManager.carTargetSpeed = 1;
        StaticVariableManager.flickerSet = false;
        StaticVariableManager.startRedCarMovement = false;
        StaticVariableManager.startGreenCarMovement = false;
        StaticVariableManager.startBlueCarMovement = false;
        StaticVariableManager.RedCarHitOnFlicker = false;
        StaticVariableManager.BlueCarHitOnFlicker = false;
        StaticVariableManager.GreenCarHitOnFlicker = false;

        //Sequence Number
        StaticVariableManager.currentGoal = 0;
        StaticVariableManager.currentProgress = 0;
        StaticVariableManager.correctNumberHits = 0;
        StaticVariableManager.wrongNumberHits = 0;

        //Color Sequence
        StaticVariableManager.isColorDisplayed = false;
        StaticVariableManager.sequenceCreated = false;
        StaticVariableManager.lane1CorrectTargetsHits = 0;
        StaticVariableManager.lane2CorrectTargetsHits = 0;
        StaticVariableManager.lane3CorrectTargetsHits = 0;
        StaticVariableManager.lane1IncorrectTargetsHits = 0;
        StaticVariableManager.lane2IncorrectTargetsHits = 0;
        StaticVariableManager.lane3IncorrectTargetsHits = 0;

        // Shots Replay Variables
        StaticVariableManager.currLane1Targets.Clear();
        StaticVariableManager.currLane2Targets.Clear();
        StaticVariableManager.currLane3Targets.Clear();
        StaticVariableManager.isLane1StartTargetSet = false;
        StaticVariableManager.isLane2StartTargetSet = false;
        StaticVariableManager.isLane3StartTargetSet = false;
        StaticVariableManager.lane1TargetStartCounts = 0;
        StaticVariableManager.lane2TargetStartCounts = 0;
        StaticVariableManager.lane3TargetStartCounts = 0;
        StaticVariableManager.lanePlatesDisabledCount = 0;
        StaticVariableManager.lanePlatesDisabled = false;
        StaticVariableManager.isPlatesDisplayed = false;
        ShotsReplayManager.isShotsReplayOpen = false;

        // IPSC Extended
        StaticVariableManager.numTarget1Hits = 0;
        StaticVariableManager.numTarget2Hits = 0;
        StaticVariableManager.numTarget3Hits = 0;
        StaticVariableManager.numTarget4Hits = 0;
        StaticVariableManager.numTarget5Hits = 0;
        StaticVariableManager.numIPSCPole1Shots = 0;
        StaticVariableManager.numIPSCPole2Shots = 0;
        StaticVariableManager.numIPSCPole3Shots = 0;
        StaticVariableManager.numIPSCSlider1Shots = 0;
        StaticVariableManager.numIPSCSlider2Shots = 0;


        // Dueling Tree
        StaticVariableManager.duelingTreeS1Points = 0;
        StaticVariableManager.duelingTreeS2Points = 0;
        StaticVariableManager.duelingTreeComplete = false;

        // Target Recognition
        StaticVariableManager.numCiviliansHit = 0;
        StaticVariableManager.numSuspectsHit = 0;
        StaticVariableManager.numSuspectHeadShot = 0;
        StaticVariableManager.numSuspectBodyShot = 0;
        StaticVariableManager.TRTShootSequence.Clear();
        StaticVariableManager.TRTFollowUpSequence = "";
        StaticVariableManager.TRTTargetsShot.Clear();
        StaticVariableManager.isTRTTrainingComplete = false;
        StaticVariableManager.isCorrectTRTRequence = false;
        StaticVariableManager.isThreatAvailable = false;

        // Video Scenario
        StaticVariableManager.isStartVideoTime = false;
        StaticVariableManager.isTrainingEndReached = false;
        StaticVariableManager.isVideoTrainingComplete = false;
        StaticVariableManager.isThreatShot = false;
        StaticVariableManager.isNoneThreatShot = false;
        StaticVariableManager.isVideoShotFired = false;
        StaticVariableManager.numVideoThreatsShot = 0;
        StaticVariableManager.numVideoNoneThreatsShot = 0;
        StaticVariableManager.videoThreatsShot.Clear();
        StaticVariableManager.currentSetVideo = "";
        StaticVariableManager.isLastVideoNTS = false;
        StaticVariableManager.initializeStepBack = false;
        StaticVariableManager.isHeadShot = false;
        StaticVariableManager.currSetVideoIndex = 0;

        // Hunting scenarion
        StaticVariableManager.isBearKilledPlayer = false;

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
        lane1TimeActiveCounter = 0;
        lane2TimeActiveCounter = 0;
        lane3TimeActiveCounter = 0;
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

        BirdHuntingSound = gameObject.AddComponent <AudioSource>();
        BirdHuntingSound.clip = birdHuntingIdleAudio;

        if(TestConditionsManager.timeOfDay.ToLower().Contains("day"))
        {
            HuntingForestSound = gameObject.AddComponent<AudioSource>(); // Hunting forest
            if(daytime_HuntingForestAudio != null) HuntingForestSound.clip = daytime_HuntingForestAudio;
        }
        else if (TestConditionsManager.timeOfDay.ToLower().Contains("night"))
        {
            HuntingForestSound = gameObject.AddComponent<AudioSource>(); // Hunting forest
            if(nighttime_HuntingForestAudio != null) HuntingForestSound.clip = nighttime_HuntingForestAudio;
        }


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
            resetIndoorPapers();
            chrono = false;
            buzzerFlag = true;
            buzzerTime = 6;
            startTime = TestConditionsManager.total_test_time;
            rangeTime = startTime.ToString();
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
        else if (activeScene.ToLower().Contains("indoor_range") || DropDown.scene_type == "3D")
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

    public void handleSplitTimeDisplay()
    {
        if (!isSplitTimeOpen)
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

        if(!activeScene.ToLower().Contains("calibration") && !activeScene.ToLower().Contains("range"))
        {
            GameObject canvas;

            canvas = GameObject.FindGameObjectWithTag("LaneSplitterCanvas");
            process_result = check_canvas(process_result, canvas);

            canvas = GameObject.FindGameObjectWithTag("StartCanvas");
            process_result = check_canvas(process_result, canvas);

            canvas = GameObject.FindGameObjectWithTag("AdminCanva");
            process_result = check_canvas(process_result, canvas);

        }

        return process_result;
    }

    private bool check_canvas(bool status, GameObject curr_canvas)
    {
        if (curr_canvas != null)
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
                // print("Element pressed is: " + ui_element.name);

                status = true;
                if (ui_element.name.ToLower().Contains("reset") || ui_element.name.ToLower().Contains("pause") || ui_element.name.ToLower().Contains("start")
                    || ui_element.name.ToLower().Contains("exit") || ui_element.name.ToLower().Contains("distance manager") || ui_element.name.ToLower().Contains("target control panel")
                    || ui_element.name.ToLower().Contains("button background") || ui_element.name.ToLower().Contains("end") || ui_element.name.ToLower().Contains("score")
                    || ui_element.name.ToLower().Contains("distance manager") || ui_element.name.ToLower().Contains("next") || ui_element.name.ToLower().Contains("prev")
                    || ui_element.name.ToLower().Contains("control") || ui_element.name.ToLower().Contains("Table Speed Panel")) // Table Speed Panel
                {
                    status = true;
                    // print("Element pressed is: " + ui_element.name);
                }
            }
        }

        return status;
    }

    private void SetUIPanels()
    {
        //Set Canvas
        adminCanvas = GameObject.FindGameObjectWithTag("LaneSplitterCanvas");
        countDownCanvas = GameObject.FindGameObjectWithTag("StartCanvas");
        backgroundImageDisplay = GameObject.FindGameObjectWithTag("BackgroundImage");

        //Set UI variable data
        /*if (!activeScene.ToLower().Contains("calibration"))
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

        } */

    }

}