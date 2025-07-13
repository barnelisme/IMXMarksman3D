using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCarManager : MonoBehaviour
{

    public List<GameObject> movePoints = new List<GameObject>();
    public Material redCarMaterial;
    private Color startColor;

    private float speed = 1f;
    private float rotationSpeed = 100f;
    private float flickerUpTimer;
    private float flickerDownTimer;
    private float setFlickerTimer = 0.5f;
    private float speedTimerSetter = 1;
    private float speedSettingTimer = 0;

    private int currentPointIndex = 0;
    private Transform currentMovePoint;
    private int moveSteps = 0;
    private bool runningSpeedTimer = false;
    private bool isFlickerPointSet = false;
    private bool isStartingPointSet = false;

    void Start()
    {
        if (movePoints.Count > 0)
        {
            moveSteps = 0;
            StaticVariableManager.redCarMoveSteps = 0;
            currentMovePoint = movePoints[currentPointIndex].transform;
            startColor = Color.red;
            flickerDownTimer = setFlickerTimer;
            flickerUpTimer = setFlickerTimer;
            StaticVariableManager.redCarSpeedTimer = StaticVariableManager.speedTimerSetValue; // Reset Timer
            //transform.LookAt(currentMovePoint.position);
        }
    }

    void Update()
    {
        if(countDownStart.start_training == true && StaticVariableManager.startRedCarMovement) // && !StaticVariableManager.flickerSet
        {
            // Check if we reached the current movePoint
            if(StaticVariableManager.RedCarHitOnFlicker)
            {
                if (Vector3.Distance(transform.position, currentMovePoint.position) < 0.2f || !isFlickerPointSet)
                {
                    if (currentPointIndex == 0)
                    {
                        currentPointIndex = movePoints.Count - 1;
                    }
                    else
                    {
                        currentPointIndex--;
                    }

                    StaticVariableManager.redCarCompletedPoints--;
                    moveSteps--;

                    currentMovePoint = movePoints[currentPointIndex].transform;
                    StartCoroutine(RotateTowardsTarget(currentMovePoint.position));

                    isFlickerPointSet = true;
                    isStartingPointSet = false;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, currentMovePoint.position, speed * Time.deltaTime);
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, currentMovePoint.position) < 0.2f || !isStartingPointSet)
                {
                    if (currentPointIndex == movePoints.Count - 1)
                    {
                        currentPointIndex = 0;
                    }
                    else
                    {
                        currentPointIndex++;
                    }

                    StaticVariableManager.redCarCompletedPoints++;
                    moveSteps++;

                    currentMovePoint = movePoints[currentPointIndex].transform;
                    StartCoroutine(RotateTowardsTarget(currentMovePoint.position));

                    isStartingPointSet = true;
                    isFlickerPointSet = false;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, currentMovePoint.position, speed * Time.deltaTime);
                }
            }

        }

        ManageStopCondition();
        ManageSpeed();
        ManageFlicker();
    }

    private void ManageStopCondition()
    {
        if (StaticVariableManager.redCarCompletedPoints >= movePoints.Count)
        {
            StaticVariableManager.isStopTraining = true;
        }
    }

    private void ManageSpeed()
    {
        if (StaticVariableManager.isStopTraining == false && countDownStart.start_training == true && StaticVariableManager.flickerSet == false)
        {
            speed = StaticVariableManager.redCarSpeed;
        }
        else if (StaticVariableManager.isStopTraining == false && countDownStart.start_training == true && StaticVariableManager.flickerSet == true)
        {
            speed = StaticVariableManager.redCarSpeed;
        }

        if(StaticVariableManager.redCarPlateHit)
        {

            if (!StaticVariableManager.flickerSet)
            {
                runningSpeedTimer = true;
                StaticVariableManager.redCarSpeedTimer -= Time.deltaTime * 1; // Decrement timer.

                if (StaticVariableManager.redCarSpeedTimer <= 0f)
                {
                    StaticVariableManager.redCarSpeed = 1; // Reset speed
                    StaticVariableManager.redCarSpeedTimer = StaticVariableManager.speedTimerSetValue; // Reset Timer
                    StaticVariableManager.redCarPlateHit = false;
                    runningSpeedTimer = false;
                }
            }
            else
            {
                StaticVariableManager.redCarSpeedTimer = 0; // Reset Timer
                StaticVariableManager.redCarSpeed = 1; // Reset speed
            }
        }
    }

    private void ManageFlicker()
    {
        if (countDownStart.start_training == true && StaticVariableManager.flickerSet == true)
        {
            if (flickerUpTimer <= 0)
            {
                if (flickerDownTimer <= 0)
                {
                    flickerUpTimer = setFlickerTimer;
                }
                else
                {
                    redCarMaterial.color = startColor;
                    flickerDownTimer -= Time.deltaTime;
                }
            }
            else
            {
                redCarMaterial.color = Color.black;
                flickerUpTimer -= Time.deltaTime;
                flickerDownTimer = setFlickerTimer;
            }
        }
        else
        {
            redCarMaterial.color = startColor;
        }
    }

    IEnumerator RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        float angleToTarget = Quaternion.Angle(transform.rotation, lookRotation);

        // Check if the angle to rotate is more than a small threshold
        while (angleToTarget > 0.1f)
        {
            // Calculate the rotation step
            float step = rotationSpeed * Time.deltaTime;

            // If the remaining angle to rotate is less than the step, just set the rotation directly
            if (angleToTarget > step)
            {
                transform.rotation = lookRotation;
            }
            else
            {
                // Rotate towards the target rotation
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, step);
            }

            // Update the angle to target
            angleToTarget = Quaternion.Angle(transform.rotation, lookRotation);

            yield return null;
        }
    }
}
