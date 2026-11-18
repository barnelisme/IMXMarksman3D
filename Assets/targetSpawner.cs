using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetSpawner : MonoBehaviour
{
    public Transform spawnLocations;
    public GameObject whatToSpawnPrefab;
    public GameObject whatToSpawnClone;
    public Transform parentTransform;

    public GameObject[] evasivePoints;
    public GameObject[] shootPoints;
    public GameObject safePoint;

    public GameObject table;
    bool isPointReached = false;
    int pointIndex = 0;
    float threshold = 10f;
    float speed = 2f;

    void Start()
    {
        pointIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        manageMovements();
    }
    private void manageMovements()
    {
       
        if (countDownStart.start_training && !StaticVariableManager.isTrainingPause && StaticVariableManager.tableSpeed > 0f)
        {
            if (!isPointReached)
            {
                if (Vector3.Distance(table.transform.position, evasivePoints[pointIndex].transform.position) < threshold)
                {
                    //isPointReached = true;
                    if(pointIndex < evasivePoints.Length - 1)
                    {
                        pointIndex++;
                    }
                    else
                    {
                        pointIndex = 0;
                    }
                }
                else
                {
                    print("Moving");
                    table.transform.position= Vector3.MoveTowards(table.transform.position, evasivePoints[pointIndex].transform.position, StaticVariableManager.tableSpeed * Time.deltaTime);
                }
            }
        }
    }
    public void spawnPlate()
    {
        //whatToSpawnClone = Instantiate(whatToSpawnPrefab, spawnLocations.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;

        whatToSpawnClone = Instantiate(whatToSpawnPrefab, spawnLocations.transform.position, Quaternion.Euler(0, 0, 0), parentTransform) as GameObject;

        // Optionally, you can reset the local position and rotation of the instantiated object relative to its parent
        //whatToSpawnClone.transform.localPosition = Vector3.zero; // Adjust if needed
        //whatToSpawnClone.transform.localRotation = Quaternion.identity; // Adjust if needed
    }
}
