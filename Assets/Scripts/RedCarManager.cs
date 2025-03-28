using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCarManager : MonoBehaviour
{

    public List<GameObject> movePoints = new List<GameObject>();
    public Material redCarMaterial;
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
            StaticVariableManager.redCarMoveSteps = 0;
            currentMovePoint = movePoints[currentPointIndex].transform;
            startColor = Color.red;
            flickerDownTimer = setFlickerTimer;
            flickerUpTimer = setFlickerTimer;
            //transform.LookAt(currentMovePoint.position);
        }
    }

    void Update()
    {
        if (!StaticVariableManager.redCarMoveComplete && StaticVariableManager.isStopTraining == false && countDownStart.start_training == true && StaticVariableManager.flickerSet == false)
        {
            // Check if we reached the current movePoint
            if (Vector3.Distance(transform.position, currentMovePoint.position) < 0.1f)
            {
                print("Stopping");
                // Move to the next movePoint
                currentPointIndex = (currentPointIndex + 1) % movePoints.Count;
                currentMovePoint = movePoints[currentPointIndex].transform;
                moveSteps++;
                StaticVariableManager.redCarCompletedPoints++;
                if (StaticVariableManager.redCarCompletedPoints >= movePoints.Count)
                {
                    StaticVariableManager.isStopTraining = true;
                }
                print(StaticVariableManager.redCarCompletedPoints);

                StartCoroutine(RotateTowardsTarget(currentMovePoint.position));
            }
            else
            {
                if (movePoints.Count == 0)
                    return;
                
                if (nonFlickerPointSet == false)
                {
                    moveSteps = StaticVariableManager.redCarMoveSteps;
                    currentPointIndex = (currentPointIndex + 1) % movePoints.Count;
                    currentMovePoint = movePoints[currentPointIndex].transform;

                    flickerPointSet = false;
                    nonFlickerPointSet = true;
                }

                // Move towards the current movePoint
                if (moveSteps >= StaticVariableManager.redCarMoveSteps)
                {
                    StaticVariableManager.redCarMoveComplete = true;
                    moveSteps = 0;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, currentMovePoint.position, speed * Time.deltaTime);
                }
            }
        }
        else if (!StaticVariableManager.redCarMoveComplete && StaticVariableManager.isStopTraining == false && countDownStart.start_training == true && StaticVariableManager.flickerSet == true)
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
                StaticVariableManager.redCarCompletedPoints--;
                if (StaticVariableManager.redCarCompletedPoints >= movePoints.Count + 1)
                {
                    StaticVariableManager.isStopTraining = true;
                }
                //print(StaticVariableManager.redCarCompletedPoints);

                StartCoroutine(RotateTowardsTarget(currentMovePoint.position));
            }
            else
            {
                if (movePoints.Count == 0)
                    return;

                if (flickerPointSet == false)
                {
                    moveSteps = StaticVariableManager.redCarMoveSteps;
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
                if (moveSteps >= StaticVariableManager.redCarMoveSteps)
                {
                    StaticVariableManager.redCarMoveComplete = true;
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
