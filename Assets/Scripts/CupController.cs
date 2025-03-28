using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupController : MonoBehaviour
{

    public List<GameObject> movePoints = new List<GameObject>();
    public List<GameObject> startPoints = new List<GameObject>();
    public Transform startPosition; 
    private float movementSpeed = 10f; // Speed of movement

    private Vector3 targetPosition; // Target position the object is moving towards
    private int currTargetIndex = 0;
    private float moveTimer = 4;
    private float setMoveTimer = 2;
    private int randomStartPos = 0;
    private bool moveComplete = false;
    private bool startSet = false;
    public bool stopMoving = false;

    void Start()
    {
        //generateStartPos();
        targetPosition = startPosition.transform.position;
        switch (startPosition.name)
        {
            case "Left":
                currTargetIndex = 1;
                break;

            case "Back Left":
                currTargetIndex = 3;
                break;

            case "Right":
                currTargetIndex = 2;
                break;
        }
        currTargetIndex = movePoints.FindIndex(obj => obj == startPosition);
        movementSpeed = StaticVariableManager.cupMoveSpeed;
    }

    private void Update()
    {
        if(StaticVariableManager.cup_init_complete == false)
        {
            switch (transform.name)
            {
                case "Cup (1)":
                    if (StaticVariableManager.reInitialise_1)
                    {
                        //print("point initialised...");
                        startSet = false;
                        moveTimer = setMoveTimer;
                        moveComplete = false;
                        generateStartPos();
                        StaticVariableManager.reInitialise_1 = false;
                    }
                    break;
                case "Cup (2)":
                    if (StaticVariableManager.reInitialise_2)
                    {
                        //print("point initialised...");
                        startSet = false;
                        moveTimer = setMoveTimer;
                        moveComplete = false;
                        generateStartPos();
                        StaticVariableManager.reInitialise_2 = false;
                    }
                    break;
                case "Cup (3)":
                    if (StaticVariableManager.reInitialise_3)
                    {
                        //print("point initialised...");
                        startSet = false;
                        moveTimer = setMoveTimer;
                        moveComplete = false;
                        generateStartPos();
                        StaticVariableManager.reInitialise_3 = false;
                    }
                    break;
            }

            if (!StaticVariableManager.reInitialise_1 && !StaticVariableManager.reInitialise_2 && !StaticVariableManager.reInitialise_3)
            {
                StaticVariableManager.cup_init_complete = true;
            }
        }


        if (StaticVariableManager.startMoving && countDownStart.start_training && !stopMoving)
        {
            MoveObject();
        }
        else if(stopMoving)
        {
            //print("stop moving");
        }
        
        if(movementSpeed != StaticVariableManager.cupMoveSpeed)
        {
            movementSpeed = StaticVariableManager.cupMoveSpeed;
        }

    }

    void generateStartPos()
    {
        if (!startSet)
        {
            switch (this.transform.name)
            {
                case "Cup (1)":
                    startPosition = startPoints[StaticVariableManager.startPosition_1].transform;
                    //print("RE: Cup 1 " + StaticVariableManager.startPosition_1);
                    break;

                case "Cup (2)":
                    startPosition = startPoints[StaticVariableManager.startPosition_2].transform;
                    //print("RE: Cup 2 " + StaticVariableManager.startPosition_2);
                    break;

                case "Cup (3)":
                    startPosition = startPoints[StaticVariableManager.startPosition_3].transform;
                    //print("RE: Cup 3 " + StaticVariableManager.startPosition_3);
                    break;
            }

            //this.transform.position = startPosition.position;
            startSet = true;
        }
        
    }

    void MoveObject()
    {

        if(moveTimer <= 0f || StaticVariableManager.isStopTraining)
        {
            if(!moveComplete)
            {
                targetPosition = startPosition.position;

                Vector3 direction = (targetPosition - transform.position).normalized;
                transform.position += direction * movementSpeed * Time.deltaTime;

                if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
                {
                    moveComplete = true;

                    switch (startPosition.name)
                    {
                        case "Left":
                            currTargetIndex = 1;
                            targetPosition = movePoints[currTargetIndex].transform.position;
                            break;

                        case "Center":
                            currTargetIndex = 3;
                            targetPosition = movePoints[currTargetIndex].transform.position;
                            break;

                        case "Right":
                            currTargetIndex = 2;
                            targetPosition = movePoints[currTargetIndex].transform.position;
                            break;
                    }
                    //StaticVariableManager.cup_init_complete = false;
                    //startSet = false;
                }
            }
        }
        else
        {
            moveTimer -= Time.deltaTime;
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * movementSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, targetPosition) < 1.5f)
            {
                if (currTargetIndex < movePoints.Count - 1)
                {
                    //int temoIndex = movePoints.IndexOf();

                    currTargetIndex++;
                }
                else
                {
                    currTargetIndex = 0;
                }

                targetPosition = movePoints[currTargetIndex].transform.position;
            }
        }

    }
}
