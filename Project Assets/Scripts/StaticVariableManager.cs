using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticVariableManager : MonoBehaviour
{
    //Computer Data variables
    public static string SystemID = "";

    //Main Data Files Directory
    public static string main_data_directory = "";
    public static bool transfer_sim_data = false;

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
    public static bool platesDisabled;
    public static bool isScoreLanesReady = false;
    public static bool OMPScoreSent = false;
    public static bool isScoreDataSet = false;

    //Hunting scenes
    public static bool isGunFired = false;
    public static int totalTargetAnimalsKilled = 0;
    public static int totalTargetAnimalHeadKills = 0;
    public static int totalTargetCasualtiesKilled = 0;
    public static int totalHeadShots = 0;
    public static int totalBodyShots = 0;

    //Baloon Shooting variables
    public static float verticalSpeed = 1f;       //max = 2 && min = 1.5
    public static float horizontalDistance = 1.5f;  //max = 4 && min = 1.5
    public static float horizontalSpeed = 1;        //max = 2 && min = 1
    public static float intensity = 3f;             //max = 4 && min = 2
    public static string currentLane1Color = "";
    public static string nextLane1Color = "";
    public static string currentLane2Color = "";
    public static string nextLane2Color = "";
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
    public static bool isLane1TargetMissed = false;
    public static bool isLane2TargetMissed = false;
    public static bool isLane3TargetMissed = false;
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

    //Car GAme scene
    public static bool redCarMoveComplete = false;
    public static bool greenCarMoveComplete = false;
    public static bool blueCarMoveComplete = false;

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
    public static bool flickerSet = false;

    public static bool isRedShotFired = false;
    public static bool isGreenShotFired = false;
    public static bool isBlueShotFired = false;

    //Sequence Number
    public static int currentGoal = 0;
    public static int currentProgress = 0;
    public static int correctNumberHits = 0;
    public static int wrongNumberHits = 0;
    public static int numberPlate = 8;

    //Color Sequence
    public static bool isColorDisplayed = false;
    public static string currentTargetColor = "";
    public static bool sequenceCreated = false;
    public static int correctColorHits = 0;
    public static int wrongColorHits = 0;
    public static float colorDisplayTimer = 5;

    //Clay Pigeon
    public static float pigeonSpeed = 3;
    public static float spawnTimer = 0;
    public static string directionUsed = "";

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
}
