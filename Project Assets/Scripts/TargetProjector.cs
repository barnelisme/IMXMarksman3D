using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TargetProjector : MonoBehaviour
{
    public Transform targetSpawnPoint;
    public GameObject targetPrefab;
    public float targetSpeed = 5;

    float setDelay = 1;
    float projectionDelay = 1;

    private void Start()
    {
        projectionDelay = Random.Range(1, 10);
    }
    void Update()
    {

        projectTarget();

    }

    private void projectTarget()
    {
        
        if(projectionDelay <= 0f)
        {
            var target = Instantiate(targetPrefab, targetSpawnPoint.position, targetSpawnPoint.rotation);
            //target.transform.position += new Vector3(0, targetSpeed * Time.deltaTime, 0);  // move UP
            projectionDelay = setDelay;
            projectionDelay = Random.Range(1, 10);
        }
        else
        {

            projectionDelay -= Time.deltaTime * 1;
        }
    }
}
