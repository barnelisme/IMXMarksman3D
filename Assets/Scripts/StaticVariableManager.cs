using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaticVariableManager : MonoBehaviour
{
    //Systen Data variables
    public static string SystemID = "";
    public static string currentUniqueSystemID = "";
    public static string storedUniqueSystemID = "";
    public static bool isSystemUnlocked = false;
    public static bool isMaterialUpdated = false;
    public static List<Vector2> recentPoints = new List<Vector2>();
    public static bool gunShotSetting = false;
    public static bool smokeImpactSetting = false;
    public static bool isBearKilledPlayer = false;
    public static List<string> CIScenes = new List<string>() { "Balloon Shooting", "ROYGBIV", "Numseq" };
    public static bool isBatteryPower = false;

    // Calibration Variables
    public static bool isCalibrationComplete = false;


    // Shots Replay Variables
    public static List<GameObject> currLane1Targets = new List<GameObject>();
    public static List<GameObject> currLane2Targets = new List<GameObject>();
    public static List<GameObject> currLane3Targets = new List<GameObject>();
    public static GameObject lane1PrevHitTarget;
    public static bool isLane1StartTargetSet = false;
    public static bool isLane2StartTargetSet = false;
    public static bool isLane3StartTargetSet = false;
    public static int lane1TargetStartCounts = 0;
    public static int lane2TargetStartCounts = 0;
    public static int lane3TargetStartCounts = 0;
    public static bool lanePlatesDisabled = false;
    public static int lanePlatesDisabledCount = 0;
    public static bool isPlatesDisplayed = false;
    public static GameObject currTarget;

    //Main Data Files Directory
    public static string main_data_directory = "";
    public static bool transfer_sim_data = false;
    public static string online_scene_data = "";

    //UI Button Controls
    public static bool UI_Reset_Pressed = false;
    public static int start_counter = 5;

    //Target Type
    public static string targetType = "circle";

    //Assist Panel Varibales
    public static bool isAssistOpen = true;

    //Base Variables
    public static bool isTrainingPause = false;
    public static bool isStopTraining = false;
    public static bool startCountDown = false;
    public static bool isTimeOut = false;
    public static bool isResetingPoints = false;
    public static bool isEnded = false;
    public static bool set_trainees = false;
    public static bool isScoreLanesReady = false;
    public static bool isOnlineDependenciesSet = false;
    public static bool OMPScoreSent = false;
    public static bool isScoreDataSet = false;

    //Hunting scenes
    public static bool isGunFired = false;
    public static int totalTargetAnimalsKilled = 0;
    public static int totalTargetAnimalHeadKills = 0;
    public static int totalTargetCasualtiesKilled = 0;
    public static int totalHeadShots = 0;
    public static int totalBodyShots = 0;
    public static string currentHuntingEnv = "";
    public static string playerMoveSetting = "Medium";

    //Baloon Shooting variables
    public static float verticalSpeed = .8f;       //max = 2 && min = 1.5
    public static float horizontalDistance = 1.5f;  //max = 4 && min = 1.5
    public static float horizontalSpeed = 1;        //max = 2 && min = 1
    public static float intensity = 3f;             //max = 4 && min = 2
    public static string currentLane1Color = "";
    public static string nextLane1Color = "";
    public static string currentLane2Color = "";
    public static string nextLane2Color = "";
    public static string currentLane3Color = "";
    public static string nextLane3Color = "";
    public static float switchTimer = 10;
    public static bool lane1StrikeOut = false;
    public static bool lane2StrikeOut = false;
    public static bool lane3StrikeOut = false;
    public static bool nextColorSet = false;

    //Global color variables
    public static string backgroundColorSetting = "blue";
    public static string targetColorSetting = "yellow";
    public static float br = 0, bg = 0, bb = 0, ba = 0;
    public static float tr = 0, tg = 0, tb = 0, ta = 0;

    //Suspect Shoot Scene
    public static int totNumLane1Threats = 1;
    public static int totNumLane2Threats = 1;
    public static bool isLane1BoardSet = false;
    public static bool isLane2BoardSet = false;
    public static bool isLane1TargetMissed = true;
    public static bool isLane2TargetMissed = true;
    public static bool isLane3TargetMissed = true;
    public static int allowedMisses = 1;
    public static float standByTime = 5;
    public static float shootTime = 5;

    //Hidden Target Scene
    public static float tableSpeed = 0;
    public static int currentIndex = 0;
    public static int preceedIndex = 0;

    //dice flipping
    public static int numDieTargets = 0;

    //Shell game
    public static int startPosition_1 = 1;
    public static int startPosition_3 = 0;
    public static int startPosition_2 = 2;
    public static bool cup_init_complete = true;
    public static float cupMoveSpeed = 10f;
    public static bool reInitialise_1 = false;
    public static bool reInitialise_2 = false;
    public static bool reInitialise_3 = false;
    public static bool startMoving = false;

    //predelay variables
    public static bool lane1PredelayActive = false;
    public static bool lane2PredelayActive = false;
    public static bool lane3PredelayActive = false;
    public static bool isHolderRevealed = false;

    //5 point scene
    public static string usedRandomPoints = "";
    public static bool target1Active = false;
    public static bool target2Active = false;
    public static bool target3Active = false;
    public static bool resetTargets = true;
    public static string pointHitName = "";

    //Car Game scene
    public static bool redCarPlateHit = false;
    public static bool greenCarPlateHit = false;
    public static bool blueCarPlateHit = false;

    public static int redCarMoveSteps = 0;
    public static int greenCarMoveSteps = 0;
    public static int blueCarMoveSteps = 0;

    public static int redCarPoints = 0;
    public static int blueCarPoints = 0;
    public static int greenCarPoints = 0;

    public static int redCarCompletedPoints = 0;
    public static int greenCarCompletedPoints = 0;
    public static int blueCarCompletedPoints = 0;

    public static float carTargetSpeed = 1;
    public static float redCarSpeed = 1;
    public static float blueCarSpeed = 1;
    public static float greenCarSpeed = 1;

    public static float speedTimerSetValue = 1;
    public static float redCarSpeedTimer = 0;
    public static float blueCarSpeedTimer = 0;
    public static float greenCarSpeedTimer = 0;

    public static bool flickerSet = false;

    public static bool startRedCarMovement = false;
    public static bool startBlueCarMovement = false;
    public static bool startGreenCarMovement = false;

    public static bool RedCarHitOnFlicker = false;
    public static bool BlueCarHitOnFlicker = false;
    public static bool GreenCarHitOnFlicker = false;

    //Sequence Number
    public static int currentGoal = 0;
    public static int currentProgress = 0;
    public static int correctNumberHits = 0;
    public static int wrongNumberHits = 0;
    public static int numberPlate = 8;
    public static string currLane1TargetNum = "0";
    public static string currLane2TargetNum = "0";
    public static string currLane3TargetNum = "0";

    //Color Sequence
    public static bool isColorDisplayed = false;
    public static string currentTargetColor = "";
    public static Color currLane1IndicatorColor = Color.black;
    public static Color currLane2IndicatorColor = Color.black;
    public static Color currLane3IndicatorColor = Color.black;
    public static bool sequenceCreated = false;
    public static int lane1CorrectTargetsHits = 0;
    public static int lane2CorrectTargetsHits = 0;
    public static int lane3CorrectTargetsHits = 0;
    public static int lane1IncorrectTargetsHits = 0;
    public static int lane2IncorrectTargetsHits = 0;
    public static int lane3IncorrectTargetsHits = 0;
    public static float colorDisplayTimer = 5;

    //Clay Pigeon
    public static float pigeonSpeed = 3;
    public static float spawnTimer = 0;
    public static string directionUsed = "";
    public static float launchForce = 100;
    public static float upwardsBias = 1f;
    public static float delayAfterHit = 2;

    //Rising shape plates
    public static bool plateDestroyed = false;

    //Camera Variables
    public static bool openCameraOpt = false;

    //Distance Simulator
    public static int startingDistance = 5;
    public static bool enableStartResize = false;
    public static bool lane1Scaled = false;
    public static bool lane2Scaled = false;
    public static bool lane3Scaled = false;
    public static bool lane1MoveControl = false;
    public static bool lane2MoveControl = false;
    public static bool lane3MoveControl = false;
    public static bool activateLane1ScaleUp = false;
    public static bool activateLane2ScaleUp = false;
    public static bool activateLane3ScaleUp = false;

    public static bool activateLane1ScaleDown = false;
    public static bool activateLane2ScaleDown = false;
    public static bool activateLane3ScaleDown = false;

    public static bool activateLane1ScaleDefault = false;
    public static bool activateLane2ScaleDefault = false;
    public static bool activateLane3ScaleDefault = false;

    //Animal IPEC Target
    public static float prepTime = 10;

    //Expression 
    public static bool setPlates = false;
    public static string mostUsedColor = "";
    public static float expRedHitTimes = 1;
    public static float expYellowHitTimes = 1;
    public static float expBlueHitTimes = 1;
    public static float expGreenHitTimes = 1;
    public static float expBlackHitTimes = 1;

    //3D Suspect Shoot
    public static bool isSuspectsSet = false;
    public static float numberSuspectsHit = 0;
    public static float numberNoneSuspectsHit = 0;
    public static bool trainingLevelReset = false;
    public static float trainingLevel = 1;
    public static float limitTime = 1;
    public static float limitOverTime = 0;
    public static float level_1_time = 5.4f;
    public static float level_2_time = 7.4f;
    public static float level_3_time = 10.4f;
    public static float level_4_time = 13.4f;
    public static float level_5_time = 15.4f;

    // Background Images
    public static List<Sprite> backgroundImages;
    public static List<Color> backgroundColors;
    public static int currBackgroundImagePos = 3;
    public static int currDuelingTreeBackgroundPos = 3;
    public static int currBackgroundBackupPos = currBackgroundImagePos;
    public static int currHuntGroundImagePos = 1;
    public static bool isShowCurrBackground = false;

    // Pepper Popper Shooting
    public static bool isPopperBoard1Set = false;
    public static bool isPopperBoard2Set = false;
    public static bool isPopperBoard3Set = false;

    // IPSC Extended
    public static string[] IPSCTargetOptions = new string [3];
    public static int numIPSCTargetOption = 1;

    public static int numTarget1ShotsSetting = 10;
    public static int numTarget2ShotsSetting = 10;
    public static int numTarget3ShotsSetting = 10;
    public static int numTarget4ShotsSetting = 10;
    public static int numTarget5ShotsSetting = 10;

    public static int numIPSCSteelPlates = 6;

    public static int target1DistanceSetting = 5;
    public static int target2DistanceSetting = 5;
    public static int target3DistanceSetting = 5;
    public static int target4DistanceSetting = 5;
    public static int target5DistanceSetting = 5;

    public static int numTarget1Hits = 0;
    public static int numTarget2Hits = 0;
    public static int numTarget3Hits = 0;
    public static int numTarget4Hits = 0;
    public static int numTarget5Hits = 0;

    public static bool isTarget1Complete = false;
    public static bool isTarget2Complete = false;
    public static bool isTarget3Complete = false;
    public static bool isTarget4Complete = false;
    public static bool isTarget5Complete = false;

    public static int numIPSCPole1Shots = 0;
    public static int numIPSCPole2Shots = 0;
    public static int numIPSCPole3Shots = 0;
    public static int numIPSCSlider1Shots = 0;
    public static int numIPSCSlider2Shots = 0;
    public static int numTexasWheelShots = 0;


    // Dueling Tree Scenario
    public static int numDuelingTreeTargets = 2;
    public static int duelingTreeS1Points = 0;
    public static int duelingTreeS2Points = 0;
    public static bool duelingTreeComplete;

    // Target Recognition Variables
    public static int numCiviliansHit = 0;
    public static int numSuspectsHit = 0;
    public static int numSuspectHeadShot = 0;
    public static int numSuspectBodyShot = 0;
    public static int numCurrThreats = 0;
    public static int threatCount = 0;
    public static int TRTShootPoints = 0;
    public static List<string> TRTShootSequence = new List<string>();
    public static string TRTFollowUpSequence = "";
    public static List<GameObject> TRTTargetsShot = new List<GameObject>();
    public static bool isCorrectTRTRequence = false;
    public static bool isTRTTrainingComplete = false;
    public static bool isThreatAvailable = false;
    public static int numTargetBoards = 4;

    // Video Scenario
    public static bool isStartVideoTime = false;
    public static bool isTrainingEndReached = false;
    public static bool isVideoTrainingComplete = false;
    public static bool isThreatShot = false;
    public static bool isNoneThreatShot = false;
    public static bool isThreatVideo = true;
    public static string currentVideoGroup = "G5";
    public static string currentVideoOption = "";
    public static string currentSetVideo = "";
    public static int previousIndex = 0;
    public static int currVideoThreats = 1;
    public static int numVideoThreatsShot = 0;
    public static int numVideoNoneThreatsShot = 0;
    public static bool isVideoShotFired = false;
    public static List<GameObject> videoThreatsShot = new List<GameObject>();
    // Transition
    public static bool isTransitionVideo = false;
    public static bool isLastVideoNTS = false; // Last video None Threat Shot (NTS)
    public static bool initializeStepBack = false; // When the trainee fails to shoot
    public static int stepBackIndex = 0;
    public static int currSetVideoIndex = 0;
    public static bool isHeadShot = false;


    // UDP Data packats management
    private const int MaxRecentPoints = 10;
    private static readonly Vector2[] buffer = new Vector2[MaxRecentPoints];
    private static int index = 0;
    private static int count = 0;

    public static bool IsDuplicate(float x, float y)
    {
        // print("Checking duplications.");
        for (int i = 0; i < count; i++)
        {
            if (Mathf.Approximately(buffer[i].x, x) &&
                Mathf.Approximately(buffer[i].y, y))
                return true;
        }
        return false;
    }

    public static void AddPoint(float x, float y)
    {
        // print("Adding point");
        buffer[index] = new Vector2(x, y);
        index = (index + 1) % MaxRecentPoints;
        count = Mathf.Min(count + 1, MaxRecentPoints);
    }

}
