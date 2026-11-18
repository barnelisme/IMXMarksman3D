using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class laneClayPigeonSpowner : MonoBehaviour
{
    public GameObject[] spawnLocations;
    public GameObject whatToSpawnPrefab;
    public GameObject whatToSpawnClone;
    public GameObject targetReference;

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
        if (targetReference != null && whatToSpawnPrefab != null)
        {
            int count = 0;
            /*int numLanes = GetNumberOfLanes();

            while (count < numLanes)
            {
                // Temporarily off
            }*/

            spawn_location_index = count;

            // Spawn at the spawn location [cite: 185]
            whatToSpawnClone = Instantiate(
                whatToSpawnPrefab,
                spawnLocations[spawn_location_index].transform.position,
                Quaternion.identity
            );

            // Temporarily parent to reference to match scale [cite: 186]
            Transform originalParent = whatToSpawnClone.transform.parent; // [cite: 186]
            whatToSpawnClone.transform.SetParent(targetReference.transform, worldPositionStays: true); // [cite: 186]

            // Copy reference's local scale exactly [cite: 187]
            whatToSpawnClone.transform.localScale = Vector3.one; // [cite: 187]

            // Reparent to reference's parent to keep hierarchy consistent [cite: 188]
            whatToSpawnClone.transform.SetParent(targetReference.transform.parent, worldPositionStays: true); // [cite: 188]

            // --- Dynamic Naming Logic ---
            string baseName = whatToSpawnClone.name;
            string lanePrefix = "";

            // Check the lane dynamically 
            if (spawnLocations[spawn_location_index].transform.name.Contains("Lane 1")) // 
            {
                lanePrefix = "1";
            }
            else if (spawnLocations[spawn_location_index].transform.name.Contains("Lane 2")) // 
            {
                lanePrefix = "2";
            }
            else if (spawnLocations[spawn_location_index].transform.name.Contains("Lane 3")) // 
            {
                lanePrefix = "3";
            }

            // If a valid lane prefix was found and the base name isn't empty, swap the first character dynamically
            if (!string.IsNullOrEmpty(lanePrefix) && !string.IsNullOrEmpty(baseName))
            {
                whatToSpawnClone.name = lanePrefix + baseName.Substring(1);
            }
        }
    }

    private int GetNumberOfLanes()
    {
        if (activeScene.ToLower().Contains("1lane")) return 1;
        if (activeScene.ToLower().Contains("2lane")) return 2;
        if (activeScene.ToLower().Contains("3lane")) return 3;
        if (activeScene.ToLower().Contains("4lane")) return 4;

        return 1;
    }

}
