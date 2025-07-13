using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimalSpawner : MonoBehaviour
{
    public Transform[] spawnLocations;
    public GameObject[] whatToSpawnPrefab;
    public GameObject[] whatToSpawnClone;
    public List<string> animalNamesList;

    string activeScene = "";
    int prefebIndex = 0, cloneIndex = 0;
    int totAnimals = 0;
    int numAnimalsToSpawn = 0;

    //TIMERS Variables
    float timeToRespawn;
    float setTimeToRespawn = 5f;
    int animal_index = 0;
    int target_animal_index = 0;
    int spawn_location_index = 0;
    bool spawnLocationSet = false;

    private void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        timeToRespawn = setTimeToRespawn;
        animal_index = Random.Range(1, 4);
        assignAnimalNames();
        numAnimalsToSpawn = 25;

        if (activeScene.ToLower().Contains("direct"))
        {
            timeToRespawn = 3.5f;

            if (TestConditionsManager.animalName.ToLower().Contains("deer"))
            {
                animal_index = Random.Range(1, 4);
            }
            else
            {
                animal_index = Random.Range(1, 2);
            }

            allocateAnimalPreferb();
        }
        else if (activeScene.ToLower().Contains("avoid"))
        {
            timeToRespawn = 2f;

            if (TestConditionsManager.animalName.ToLower().Contains("deer"))
            {
                animal_index = Random.Range(0, animalNamesList.Count - 1);
            }
            else
            {
                animal_index = Random.Range(0, animalNamesList.Count + 2);
            }

            spawn_location_index = Random.Range(0, 1);
            spawnMammals();
        }

    }

    private void Update()
    {
        //Find Total number of animals in the scene
        totAnimals = GameObject.FindGameObjectsWithTag("deer").Length;

        if (timeToRespawn <= 0f && StaticVariableManager.isTrainingPause == false && totAnimals < numAnimalsToSpawn)
        {
            //timeToRespawn = setTimeToRespawn;

            if (activeScene.ToLower().Contains("direct"))
            {
                timeToRespawn = 3.5f;
                
                if(TestConditionsManager.animalName == "deer")
                {
                    animal_index = Random.Range(1, 4);
                }
                else
                {
                    animal_index = Random.Range(1, 3);
                }

                allocateAnimalPreferb();
            }
            else if (activeScene.ToLower().Contains("avoid"))
            {
                timeToRespawn = 2f;

                if (TestConditionsManager.animalName.ToLower().Contains("deer"))
                {
                    animal_index = Random.Range(0, animalNamesList.Count);
                }
                else
                {
                    animal_index = Random.Range(0, animalNamesList.Count + 2);
                }

                spawnMammals();
            }

            spawnLocationSet = false;
            //print("Number of animals is: " + totAnimals);
        }
        else
        {
            if(!spawnLocationSet)
            {
                switch (spawn_location_index)
                {
                    case 0:
                        spawn_location_index = 1;
                        break;
                    case 1:
                        spawn_location_index = 0;
                        break;
                }

                spawnLocationSet = true;
            }

            timeToRespawn -= Time.deltaTime * 1;
        }
    }
    private void assignAnimalNames()
    {
        int tempIndex = 0;
        foreach (GameObject name in whatToSpawnPrefab)
        {
            animalNamesList.Add(name.transform.name.ToString());
        }

        foreach (string name in animalNamesList)
        {
            if (name.ToLower().Contains(TestConditionsManager.animalName.ToLower()))
            {
                print("RE: name is " + name + " and index is " + tempIndex);
                target_animal_index = tempIndex;
            }
            tempIndex++;
        }

    }
    private void allocateAnimalPreferb()
    {
        //totAnimals++;
        if(TestConditionsManager.animalName.ToLower().Contains("deer"))
        {
            spawnDeer();
        }
        else if(TestConditionsManager.animalName.ToLower().Contains("boar"))
        {
            spawnBoar();
        }
        else if (TestConditionsManager.animalName.ToLower().Contains("buffalo"))
        {
            spawnBuffalo();
        }
    }
    void spawnDeer()
    {
        switch(animal_index)
        {
            case 1:
                whatToSpawnClone[0] = Instantiate(whatToSpawnPrefab[0], spawnLocations[spawn_location_index].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
                break;
            case 2:
                whatToSpawnClone[1] = Instantiate(whatToSpawnPrefab[1], spawnLocations[spawn_location_index].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
                break;
        }  
    }
    void spawnBoar()
    {
        switch (animal_index)
        {
            case 1:
                whatToSpawnClone[2] = Instantiate(whatToSpawnPrefab[2], spawnLocations[spawn_location_index].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
                break;
            case 2:
                whatToSpawnClone[2] = Instantiate(whatToSpawnPrefab[2], spawnLocations[spawn_location_index].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
                break;
        }
    }
    void spawnBuffalo()
    {
        switch (animal_index)
        {
            case 1:
                whatToSpawnClone[3] = Instantiate(whatToSpawnPrefab[3], spawnLocations[spawn_location_index].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
                break;
            case 2:
                whatToSpawnClone[3] = Instantiate(whatToSpawnPrefab[3], spawnLocations[spawn_location_index].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
                break;
            case 3:
                whatToSpawnClone[3] = Instantiate(whatToSpawnPrefab[3], spawnLocations[spawn_location_index].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
                break;
        }
    }
    void spawbLeopard()
    {
        switch (animal_index)
        {
            case 1:
                whatToSpawnClone[4] = Instantiate(whatToSpawnPrefab[4], spawnLocations[0].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
                break;
            case 2:
                whatToSpawnClone[4] = Instantiate(whatToSpawnPrefab[4], spawnLocations[1].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
                break;
        }
    }
    void spawnMammals()
    {
        //totAnimals++;
        if(TestConditionsManager.animalName.ToLower().Contains("deer"))
        {     
            if (animal_index < animalNamesList.Count)
            {
                whatToSpawnClone[animal_index] = Instantiate(whatToSpawnPrefab[animal_index], spawnLocations[spawn_location_index].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
            }
            else
            {
                whatToSpawnClone[3] = Instantiate(whatToSpawnPrefab[3], spawnLocations[spawn_location_index].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
            }
        }
        else 
        {
            if (animal_index >= animalNamesList.Count)
            {
                whatToSpawnClone[target_animal_index] = Instantiate(whatToSpawnPrefab[target_animal_index], spawnLocations[spawn_location_index].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
            }
            else
            {
                whatToSpawnClone[animal_index] = Instantiate(whatToSpawnPrefab[animal_index], spawnLocations[spawn_location_index].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
            }
        }
        
    }
}
