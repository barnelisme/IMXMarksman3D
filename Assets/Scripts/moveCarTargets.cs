using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCarTargets : MonoBehaviour
{
    public List<GameObject> movePoints = new List<GameObject>();
    private float speed = 5f;

    private int currentPointIndex = 0;
    private Transform currentMovePoint;

    void Start()
    {
        if (movePoints.Count > 0)
        {
            currentMovePoint = movePoints[currentPointIndex].transform;
            speed = StaticVariableManager.carTargetSpeed;
        }
    }

    void Update()
    {
        if (movePoints.Count == 0)
            return;

        // Move towards the current movePoint
        transform.position = Vector3.MoveTowards(transform.position, currentMovePoint.position, speed * Time.deltaTime);
        if(speed != StaticVariableManager.carTargetSpeed)
            speed = StaticVariableManager.carTargetSpeed;

        // Check if we reached the current movePoint
        if (Vector3.Distance(transform.position, currentMovePoint.position) < 0.5f)
        {
            currentPointIndex = Random.Range(0, movePoints.Count - 1);
            // Move to the next movePoint
            currentPointIndex = (currentPointIndex + 1) % movePoints.Count;
            currentMovePoint = movePoints[currentPointIndex].transform;

            // Rotate to face the new movePoint direction
            //StartCoroutine(RotateTowardsTarget(currentMovePoint.position));
        }
    }
}
