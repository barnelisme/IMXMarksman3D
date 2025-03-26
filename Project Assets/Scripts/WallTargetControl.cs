using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class WallTargetControl : MonoBehaviour
{
    //VARIABLE DECLERATION
    //Movement Varables
    float MoveSpeed;
    public float SetSpeed = 1;
    bool Move = true;
    Rigidbody rd;

    //Direction Variables
    bool moveLeft = true;
    bool moveRight = true;
    bool target_enabled = false;

    //Conditions
    public bool isHit = false;
    int directionSwitch = 0;
    public int targetSwitch = 0;
    public float targetStopTime = 4;
    int StopDistance = 4;

    //GAMEOBJECT Variables
    public GameObject ScorePanel;
    public GameObject TG1;
    public GameObject TG2;
    public GameObject TG3;
    public GameObject TG4;
    public GameObject TG5;
    private GameObject ActiveTarget;

    [SerializeField]
    public GameObject autoTarget_1;
    [SerializeField]
    public GameObject autoTarget_2;
    [SerializeField]
    public GameObject autoTarget_3;
    int currentTarget = 1;
    static bool targetMoved = false;
    static bool moveRequestMade = false;
    static bool requestReview = false;
    float targetCounter;
    float targetResetTime = 0.1f;

    //Text Objects
    public TextMeshProUGUI HeadShots;
    public TextMeshProUGUI BodyShots;

    string activeScene = " ";
    public GameObject player;

    void Start()
    {
        targetCounter = targetResetTime;
        moveLeft = true;
        moveRight = true;
        requestReview = false;

        activeScene = SceneManager.GetActiveScene().name;

        if (activeScene.ToLower().Contains("pointbullseye"))
        {
            HeadShots.text = ("Side Target: 0");
            BodyShots.text = ("Center Target: 0");
        }
        if (activeScene == "BasicTargetPopUpDynamic")
        {
            directionSwitch = 0;
        }
        else
        {
            directionSwitch = 100; //DO NOT MOVE
        }

        currentTarget = 2;
        if (activeScene.ToLower().Contains("targetpopupfreeshoot") || activeScene == "BasicTargetPopUpOneHand" 
            || activeScene.ToLower().Contains("threatening") || activeScene.ToLower().Contains("pointman") 
            || activeScene.ToLower().Contains("pointbullseye") || activeScene.ToLower().Contains("animaltarget"))
        {
            if(!activeScene.ToLower().Contains("static"))
            {
                autoTarget_1.SetActive(true);
                autoTarget_2.SetActive(false);
                autoTarget_3.SetActive(false);

                autoTarget_1.GetComponent<TargetScaleManager_V2>().enabled = true;
                autoTarget_2.GetComponent<TargetScaleManager_V2>().enabled = false;
                autoTarget_3.GetComponent<TargetScaleManager_V2>().enabled = false;

            }
            else if(activeScene.ToLower().Contains("5pointbullseye"))
            {
                //StaticVariableManager.target1Active = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleStopTrainig();

        if (isHit)
        {
            SetSpeed = 0f;
            //ScorePanel.SetActive(true);
        }
        else
        {
            SetSpeed = 4f;
            //ScorePanel.SetActive(false);
        }
        if (Keyboard.current.sKey.isPressed)//Reset Indoor range
        {
            isHit = true;
        }
        else if (!Keyboard.current.sKey.isPressed)
        {
            isHit = false;
        }

        if(moveRequestMade)
        {
            targetCounter -= Time.deltaTime * 1;

            if(targetCounter <= 0)
            {
                moveTargets();
                moveRequestMade = false;
            }

            if (targetCounter <= 0)
            {
                if (currentTarget < 3)
                {
                    //currentTarget++;
                }
                else
                {
                    //currentTarget = 1;
                }
            }

        }
        else
        {
            targetCounter = targetResetTime;
        }
        
        if(requestReview)
        {
            reviewTarget();
            //requestReview = false;
        }

        if (directionSwitch == 0)
        {

            if (targetSwitch == 0)
            {
                ActiveTarget = TG1;
            }
            else
            {
                ActiveTarget = TG2;
            }

            MoveLeft();

        }
        else if (directionSwitch == 1)
        {

            if (targetSwitch == 0)
            {
                ActiveTarget = TG3;
            }
            else
            {
                ActiveTarget = TG4;
            }

            MoveRight();
        }
    }

    private void HandleStopTrainig()
    {
        if (StaticVariableManager.isStopTraining && !target_enabled && !activeScene.ToLower().Contains("static"))
        {
            //print("Test: point reached...");
            autoTarget_1.SetActive(true);
            autoTarget_2.SetActive(true);
            autoTarget_3.SetActive(true);

            target_enabled = true;
        }
    }

    public void moveTargets()
    {
        //targetCounter -= Time.deltaTime * 1;
        //Change Target Conditions

        //Change Targets
        if(!activeScene.ToLower().Contains("static"))
        {
            if (currentTarget == 1 && targetMoved == false)
            {
                autoTarget_1.SetActive(true);
                autoTarget_2.SetActive(false);
                autoTarget_3.SetActive(false);

                autoTarget_1.GetComponent<TargetScaleManager_V2>().enabled = true;
                autoTarget_1.GetComponent<TargetScaleManager_V2>().LoadRecurringScale();

                autoTarget_2.GetComponent<TargetScaleManager_V2>().enabled = false;
                autoTarget_3.GetComponent<TargetScaleManager_V2>().enabled = false;

                currentTarget = Random.Range(1, 3);
                if (currentTarget == 1)
                {
                    currentTarget = Random.Range(2, 3);
                }

                //print("RE: Current target is " + currentTarget);
                targetMoved = true;
                StaticVariableManager.target1Active = true;
            }
            if (currentTarget == 2 && targetMoved == false)
            {
                autoTarget_1.SetActive(false);
                autoTarget_2.SetActive(true);
                autoTarget_3.SetActive(false);

                autoTarget_1.GetComponent<TargetScaleManager_V2>().enabled = false;

                autoTarget_2.GetComponent<TargetScaleManager_V2>().enabled = true;
                autoTarget_2.GetComponent<TargetScaleManager_V2>().LoadRecurringScale();

                autoTarget_3.GetComponent<TargetScaleManager_V2>().enabled = false;

                currentTarget = Random.Range(1, 3);
                if (currentTarget == 2)
                {
                    currentTarget = 3;
                }

                //print("RE: Current target is " + currentTarget);
                targetMoved = true;
                StaticVariableManager.target2Active = true;
            }
            if (currentTarget == 3 && targetMoved == false)
            {
                autoTarget_1.SetActive(false);
                autoTarget_2.SetActive(false);
                autoTarget_3.SetActive(true);

                autoTarget_1.GetComponent<TargetScaleManager_V2>().enabled = false;
                autoTarget_2.GetComponent<TargetScaleManager_V2>().enabled = false;

                autoTarget_3.GetComponent<TargetScaleManager_V2>().enabled = true;
                autoTarget_3.GetComponent<TargetScaleManager_V2>().LoadRecurringScale();

                currentTarget = Random.Range(1, 3);
                if (currentTarget == 3)
                {
                    currentTarget = Random.Range(1, 2);
                }

                //print("RE: Current target is " + currentTarget);
                targetMoved = true;
                StaticVariableManager.target3Active = true;
            }
        }
        else
        {
            StaticVariableManager.target1Active = true;
        }
        
    }
    
    private void RevertHitColor(GameObject target)
    {
        //print("point reached");
        //print("target: " + target.transform.name);
        if(target.transform.name.ToLower().Contains("head"))
        {
            print("Reverted");
            target.GetComponent<HeadTarget>().revertColor();
        }
        else if (target.transform.name.ToLower().Contains("body"))
        {
            print("Reverted");
            target.GetComponent<BodyTarget>().revertColor();
        }
    }
    
    public void reviewTarget()
    {
        autoTarget_1.SetActive(true);
        autoTarget_2.SetActive(true);
        autoTarget_3.SetActive(true);
    }
    public static void requestMove()
    {
        moveRequestMade = true;
        targetMoved = false;
    }
    public static void requestTargetReview()
    {
        requestReview = true;
    }
    public void MoveLeft()
    {
        if (moveLeft)
        {
            //print("Moving Left on Left FC");
            if (ActiveTarget.transform.position.x <= 500 - StopDistance)
            {
                moveLeft = false;

            }
            //print("X Position is "+ActiveTarget.transform.position.x);
            MoveSpeed = -SetSpeed;
            ActiveTarget.transform.Translate(MoveSpeed * Time.deltaTime, 0, 0);
        }
        else // If Stop Point is Reached
        {

            //print("Moving Right on Left FC");
            targetStopTime -= Time.deltaTime * 1;

            if(targetStopTime <= 0f)
            {
                if (ActiveTarget.transform.position.x >= 500)
                {
                    directionSwitch = 1;
                    targetStopTime = 4;

                    moveLeft = true;
                }

                MoveSpeed = SetSpeed;
                ActiveTarget.transform.Translate(MoveSpeed * Time.deltaTime, 0, 0);
            }
            else
            {
                MoveSpeed = 0;
                ActiveTarget.transform.Translate(MoveSpeed * Time.deltaTime, 0, 0);
            }
        }
    }
    public void MoveRight()
    {
        if (moveRight)
        {
            print("Moving Right on Right FC");
            if (ActiveTarget.transform.position.x >= 500 + StopDistance)
            {
                moveRight = false;

            }
            print("X Position is " + ActiveTarget.transform.position.x);
            MoveSpeed = SetSpeed;
            ActiveTarget.transform.Translate(MoveSpeed * Time.deltaTime, 0, 0);
        }
        else // If Stop Point is Reached
        {

            print("Moving Left on Right FC");
            targetStopTime -= Time.deltaTime * 1;

            if (targetStopTime <= 0f)
            {
                if (ActiveTarget.transform.position.x <= 500)
                {
                    directionSwitch = 0;
                    targetStopTime = 4;
                    if (targetSwitch == 0)
                    {
                        targetSwitch = 1;
                    }
                    else
                    {
                        targetSwitch = 0;
                    }
                    moveRight = true;
                    
                }

                MoveSpeed = -SetSpeed;
                ActiveTarget.transform.Translate(MoveSpeed * Time.deltaTime, 0, 0);
            }
            else
            {
                MoveSpeed = 0;
                ActiveTarget.transform.Translate(MoveSpeed * Time.deltaTime, 0, 0);
            }
        }
    }

}


//UPDATE CODE
/*if(directionSwitch == 0)
{

    if(targetSwitch == 0)
    {
        ActiveTarget = TG1;
    }
    else
    {
        ActiveTarget = TG2;
    }

    MoveLeft();

}

else if(directionSwitch == 1)
{

    if (targetSwitch == 0)
    {
        ActiveTarget = TG3;
    }
    else
    {
        ActiveTarget = TG4;
    }

    MoveRight();  
}*/
/*if(targetCounter <= 0)
{
    if (currentTarget < 3)
    {
        currentTarget++;
    }
    else
    {
        currentTarget = 1;
    }
    targetCounter = targetResetTime;
}*/