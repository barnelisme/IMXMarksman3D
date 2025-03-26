using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HumanPopupControl : MonoBehaviour
{
    //VARIABLE DECLERATION
    //Movement Varables
    float MoveSpeed;
    public float SetSpeed = 0f;
    bool Move = true;
    Rigidbody rd;

    //Direction Variables
    [SerializeField]
    bool moveLeft = true;
    [SerializeField]
    bool moveRight = true;

    //Conditions
    public bool isHit = false;
    int directionSwitch = 0;
    public int targetSwitch = 0;
    public float targetStopTime = 4;
    public float StopDistance = 4;
    float currentXPosTG1 = 0;
    float currentXPosTG2 = 0;
    float currentXPosTG3 = 0;
    float currentXPosTG4 = 0;
    float activeTargetXPos;

    //GAMEOBJECT Variables
    public GameObject ScorePanel;
    public GameObject TG1;
    public GameObject TG2;
    public GameObject TG3;
    public GameObject TG4;
    public GameObject TG5;
    private GameObject ActiveTarget;

    //Text Objects
    public TextMeshProUGUI HeadShots;
    public TextMeshProUGUI BodyShots;

    string activeScene = " ";
    public GameObject player;

    void Start()
    {
        activeScene = player.GetComponent<Shooting>().activeScene;
        SetSpeed = 1f;

        if (activeScene == "BasicHumanPopUp")
        {
            StopDistance = 1;
        }

        directionSwitch = 0;
        targetSwitch = 0;
        targetStopTime = 4;
        moveLeft = true;
        moveRight = true;

        //Setting Up the Current X position of each target
        currentXPosTG1 = TG1.transform.position.x;
        currentXPosTG2 = TG2.transform.position.x;
        currentXPosTG3 = TG3.transform.position.x;
        currentXPosTG4 = TG4.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (directionSwitch == 0)
        {

            if (targetSwitch == 0)
            {
                ActiveTarget = TG1;
                ActiveTarget.SetActive(true);
                activeTargetXPos = currentXPosTG1;

            }
            else
            {
                ActiveTarget = TG2;
                ActiveTarget.SetActive(true);
                activeTargetXPos = currentXPosTG2;

            }

            MoveLeft();

        }

        else if (directionSwitch == 1)
        {

            if (targetSwitch == 0)
            {
                ActiveTarget = TG3;
                ActiveTarget.SetActive(true);
                activeTargetXPos = currentXPosTG3;

            }
            else
            {
                ActiveTarget = TG4;
                ActiveTarget.SetActive(true);
                activeTargetXPos = currentXPosTG4;

            }

            MoveRight();
        }

        if (isHit)
        {
            SetSpeed = 0f;
            ScorePanel.SetActive(true);
        }
        else
        {
            MoveSpeed = SetSpeed;
            ScorePanel.SetActive(false);
        }

        if (Input.GetKey(KeyCode.S))//Reset Indoor range
        {
            isHit = true;
        }
        else if (!Input.GetKey(KeyCode.S))
        {
            isHit = false;
        }
        EnemyView();
    }

    public void MoveLeft()
    {
        if (moveLeft)
        {

            print("Moving Left");
            if (ActiveTarget.transform.position.x <= activeTargetXPos - StopDistance)
            {
                moveLeft = false;

            }
            //print("X Position is "+ActiveTarget.transform.position.x);
            MoveSpeed = -SetSpeed;
            ActiveTarget.transform.Translate(MoveSpeed * Time.deltaTime, 0, 0);
        }
        else // If Stop Point is Reached
        {

            targetStopTime -= Time.deltaTime * 1;

            if (targetStopTime <= 0f)
            {
                if (ActiveTarget.transform.position.x >= activeTargetXPos)
                {
                    ActiveTarget.SetActive(false);
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
            print("Moving Right");
            if (ActiveTarget.transform.position.x >= activeTargetXPos + StopDistance)
            {
                moveRight = false;

            }
            //print("X Position is " + ActiveTarget.transform.position.x);
            MoveSpeed = SetSpeed;
            ActiveTarget.transform.Translate(MoveSpeed * Time.deltaTime, 0, 0);
        }
        else // If Stop Point is Reached
        {

            //print("Moving Left on Right FC");
            targetStopTime -= Time.deltaTime * 1;

            if (targetStopTime <= 0f)
            {
                if (ActiveTarget.transform.position.x <= activeTargetXPos)
                {
                    ActiveTarget.SetActive(false);
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

    private void EnemyView()
    {
        //if(distance <= EnemyShootFromFarDistance)
        //{
        //	Vector3 LookDir = mainPlayer.position - this.gameObject.transform.position;
        //	LookDir.y = 0;
        //	transform.LookAt(this.gameObject.transform.position + LookDir, Vector3.up);
        //}//end of void EnemyView()
        Vector3 LookDir = player.transform.position - ActiveTarget.transform.position;
        LookDir.y = 0;
        ActiveTarget.transform.LookAt(player.transform.position + LookDir, Vector3.up);
        //print("Enemy View");
    }
}
