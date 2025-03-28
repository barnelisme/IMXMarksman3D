using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class laneClayPigeonSpowner : MonoBehaviour
{
    public GameObject[] spawnLocations;
    public GameObject whatToSpawnPrefab;
    public GameObject whatToSpawnClone;

    private int spawn_location_index = 0;
    private int secondarySpawns = 2;
    private int currSecStatus = 0;
    private bool secondaryStatusSet = false;
    private float spawnTimer = 0;
    private float setSpawnTimer = 1f;
    private float spawnTimer2 = 0;
    private float setSpawnTimer2 = 0.8f;
    public float currentSpeed = 0;
    private string activeScene = "";

    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        
        if (activeScene.ToLower().Contains("shapeplate"))
        {
            spawnTimer = setSpawnTimer;
            if (currentSpeed != StaticVariableManager.pigeonSpeed)
            {
                //setSpawnTimer = StaticVariableManager.pigeonSpeed * 0.33333f;
                //setSpawnTimer2 = StaticVariableManager.pigeonSpeed * 0.2666667f;
                //currentSpeed = StaticVariableManager.pigeonSpeed;
            }

            spawnTimer = setSpawnTimer;
            spawnTimer2 = setSpawnTimer2;
        }
        else if(activeScene.ToLower().Contains("claypigeon"))
        {
            StaticVariableManager.plateDestroyed = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(activeScene.ToLower().Contains("shapeplat") && countDownStart.start_training)
        {
            if (currentSpeed != StaticVariableManager.pigeonSpeed)
            {
                //setSpawnTimer = 1 - StaticVariableManager.pigeonSpeed * 0.33333f;
                //setSpawnTimer2 = 0.5f - StaticVariableManager.pigeonSpeed * 0.1666667f;
                currentSpeed = StaticVariableManager.pigeonSpeed;
            }

            if (spawnTimer <= 0f)
            {
                spawnTimer2 -= Time.deltaTime * 1;
                if (spawnTimer2 <= 0)
                {
                    currSecStatus++;
                    spawnPlate();
                    spawnTimer2 = setSpawnTimer2;
                }

                if (currSecStatus >= secondarySpawns)
                {
                    spawnTimer = setSpawnTimer;
                    currSecStatus = 0;
                }
            }
            else
            {
                spawnTimer -= Time.deltaTime;
            }
        }
        else if(activeScene.ToLower().Contains("claypigeon") && countDownStart.start_training)
        {
            if (currentSpeed != StaticVariableManager.pigeonSpeed)
            {
                currentSpeed = StaticVariableManager.pigeonSpeed;
            }

            if(StaticVariableManager.plateDestroyed && countDownStart.start_training)
            {
                spawnPlate();
                StaticVariableManager.plateDestroyed = false;
            }
        }

    }

    private void spawnPlate()
    {
        whatToSpawnClone = Instantiate(whatToSpawnPrefab, spawnLocations[spawn_location_index].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
    }

}
