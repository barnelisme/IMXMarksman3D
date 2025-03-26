using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenCarManager : MonoBehaviour
{

    public List<GameObject> movePoints = new List<GameObject>();
    public Material greenCarMaterial;
    private Color startColor;

    private float speed = 5f;
    private float rotationSpeed = 100f;
    private float flickerUpTimer;
    private float flickerDownTimer;
    private float setFlickerTimer = 0.5f;

    private int currentPointIndex = 0;
    private Transform currentMovePoint;
    private int moveSteps = 0;
    private bool flickerPointSet = false;
    private bool nonFlickerPointSet = false;

    void Start()
    {
        if (movePoints.Count > 0)
        {
            moveSteps = 0;
            StaticVariableManager.greenCarMoveSteps = 0;
            currentMovePoint = movePoints[currentPointIndex].transform;
            startColor = Color.green;
            flickerDownTimer = setFlickerTimer;
            flickerUpTimer = setFlickerTimer;
            //transform.LookAt(currentMovePoint.position);
        }
    }

    void Update()
    {
        if (!StaticVariableManager.greenCarMoveComplete && StaticVariableManager.isStopTraining == false && countDownStart.start_training == true && StaticVariableManager.flickerSet == false)
        {
            // Check if we reached the current movePoint
            if (Vector3.Distance(transform.position, currentMovePoint.position) < 0.1f)
            {
                print("Stopping");
                // Move to the next movePoint
                currentPointIndex = (currentPointIndex + 1) % movePoints.Count;
                currentMovePoint = movePoints[currentPointIndex].transform;
                moveSteps++;
                StaticVariableManager.greenCarCompletedPoints++;
                if (StaticVariableManager.greenCarCompletedPoints >= movePoints.Count)
                {
                    StaticVariableManager.isStopTraining = true;
                }
                print(StaticVariableManager.greenCarCompletedPoints);

                StartCoroutine(RotateTowardsTarget(currentMovePoint.position));
            }
            else
            {
                if (movePoints.Count == 0)
                    return;
                
                if (nonFlickerPointSet == false)
                {
                    moveSteps = StaticVariableManager.greenCarMoveSteps;
                    currentPointIndex = (currentPointIndex + 1) % movePoints.Count;
                    currentMovePoint = movePoints[currentPointIndex].transform;

                    flickerPointSet = false;
                    nonFlickerPointSet = true;
                }

                // Move towards the current movePoint
                if (moveSteps >= StaticVariableManager.greenCarMoveSteps)
                {
                    StaticVariableManager.greenCarMoveComplete = true;
                    moveSteps = 0;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, currentMovePoint.position, speed * Time.deltaTime);
                }
            }
        }
        else if (!StaticVariableManager.greenCarMoveComplete && StaticVariableManager.isStopTraining == false && countDownStart.start_training == true && StaticVariableManager.flickerSet == true)
        {
            // Check if we reached the current movePoint
            if (Vector3.Distance(transform.position, currentMovePoint.position) < 0.1f)
            {

                // Move to the next movePoint
                if(currentPointIndex <= 0)
                {
                    currentPointIndex = movePoints.Count - 1;
                }
                else
                {
                    currentPointIndex--;
                }
                currentMovePoint = movePoints[currentPointIndex].transform;
                moveSteps++;
                StaticVariableManager.greenCarCompletedPoints--;
                if (StaticVariableManager.greenCarCompletedPoints >= movePoints.Count + 1)
                {
                    StaticVariableManager.isStopTraining = true;
                }
                //print(StaticVariableManager.greenCarCompletedPoints);

                StartCoroutine(RotateTowardsTarget(currentMovePoint.position));
            }
            else
            {
                if (movePoints.Count == 0)
                    return;

                if (flickerPointSet == false)
                {
                    moveSteps = StaticVariableManager.greenCarMoveSteps;
                    if (currentPointIndex <= 0)
                    {
                        currentPointIndex = movePoints.Count - 1;
                    }
                    else
                    {
                        currentPointIndex--;
                    }
                    currentMovePoint = movePoints[currentPointIndex].transform;
                    flickerPointSet = true;
                    nonFlickerPointSet = false;
                }

                // Move towards the current movePoint
                if (moveSteps >= StaticVariableManager.greenCarMoveSteps)
                {
                    StaticVariableManager.greenCarMoveComplete = true;
                    moveSteps = 0;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, currentMovePoint.position, speed * Time.deltaTime);
                }
            }
        }
        
        if(countDownStart.start_training == true && StaticVariableManager.flickerSet == true)
        {
            if(flickerUpTimer <= 0)
            {
                if(flickerDownTimer <= 0)
                {
                    flickerUpTimer = setFlickerTimer;
                }
                else
                {
                    greenCarMaterial.color = startColor;
                    flickerDownTimer -= Time.deltaTime;
                }
            }
            else
            {
                greenCarMaterial.color = Color.black;
                flickerUpTimer -= Time.deltaTime;
                flickerDownTimer = setFlickerTimer;
            }
        }
        else
        {
            greenCarMaterial.color = startColor;
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
