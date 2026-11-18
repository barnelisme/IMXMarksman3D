using System.Collections.Generic;
using UnityEngine;

public class DPTManager : MonoBehaviour
{

    [Header("Spawn Location")]
    public GameObject[] SpawnLocations_1;
    public GameObject[] SpawnLocations_2;
    public GameObject[] SpawnLocations_3;
    public GameObject[] SpawnLocations_4;

    [Header("Target Holders")]
    public GameObject[] level1TArgets;
    // public GameObject[] level2TArgets;
    public GameObject[] level3TArgets;
    public GameObject[] level4TArgets;
    public List<GameObject> currActiveTargets = new List<GameObject>(); // Initialise 4
    private List<GameObject> currIntances = new List<GameObject>();

    [Header("Process Variables")]
    private bool isTargetBoardsSet;
    private int randomTarget1Index = 0;
    private int randomTarget2Index = 0;
    private int randomTarget3Index = 0;
    private int randomTarget4Index = 0;
    private int targetSet = 0;
    private float timer = .5f;
    private float timerSetValue = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetRandomTargets();
        //ActivateTargets();
    }

    // Update is called once per frame
    void Update()
    {

        if (!isTargetBoardsSet && countDownStart.start_training)
        {
            if (timer <= 0f)
            {
                ActivateTargets();
                timer = timerSetValue;

                isTargetBoardsSet = true;
            }
            else
            {
                timer -= Time.deltaTime * 1;
            }
        }

    }

    public void GenerateTargetGroup()
    {
        SetRandomTargets();
        isTargetBoardsSet = false;
    }

    private void SetRandomTargets()
    {
        DeactivateTargets(); // Deactivate all current targets
        currActiveTargets.Clear();
        StaticVariableManager.TRTShootSequence.Clear();
        StaticVariableManager.TRTTargetsShot.Clear();
        StaticVariableManager.threatCount = 0;

        targetSet = Random.Range(1, 4);
        string allowed_sequence = "";
        List<int> usedIndexes_1 = new List<int>();
        List<int> usedIndexes_3 = new List<int>();

        // print(targetSet);
        // targetSet = 1;

        if (StaticVariableManager.numTargetBoards == 1)
        {
            switch (targetSet)
            {
                case 1:
                    do
                    {
                        randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    } while (usedIndexes_1.Contains(randomTarget1Index));
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);

                    StaticVariableManager.numCurrThreats = 0;
                    break;

                case 2:

                    do
                    {
                        randomTarget3Index = Random.Range(0, level3TArgets.Length);
                    } while (usedIndexes_1.Contains(randomTarget3Index));
                    currActiveTargets.Add(level3TArgets[randomTarget3Index]);


                    StaticVariableManager.numCurrThreats = 1;
                    break;

                case 3:


                    randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);


                    StaticVariableManager.numCurrThreats = 0;
                    break;
            }
        }
        else if (StaticVariableManager.numTargetBoards == 2)
        {
            switch (targetSet)
            {
                case 1:
                    randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);

                    usedIndexes_1.Add(randomTarget1Index);

                    do
                    {
                        randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    } while (usedIndexes_1.Contains(randomTarget1Index));
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);


                    StaticVariableManager.numCurrThreats = 0;
                    break;

                case 2:
                    randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);


                    randomTarget3Index = Random.Range(0, level3TArgets.Length);
                    currActiveTargets.Add(level3TArgets[randomTarget3Index]);


                    StaticVariableManager.numCurrThreats = 1;
                    break;

                case 3:
                    randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);


                    randomTarget3Index = Random.Range(0, level3TArgets.Length);
                    currActiveTargets.Add(level3TArgets[randomTarget3Index]);


                    StaticVariableManager.numCurrThreats = 1;
                    break;
            }
        }
        else if (StaticVariableManager.numTargetBoards == 3)
        {
            switch (targetSet)
            {
                case 1:
                    randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);
                    usedIndexes_1.Add(randomTarget1Index);

                    do
                    {
                        randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    } while (usedIndexes_1.Contains(randomTarget1Index));
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);

                    randomTarget3Index = Random.Range(0, level3TArgets.Length);
                    currActiveTargets.Add(level3TArgets[randomTarget3Index]);


                    StaticVariableManager.numCurrThreats = 1;
                    break;

                case 2:
                    randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);
                    usedIndexes_1.Add(randomTarget1Index);

                    do
                    {
                        randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    } while (usedIndexes_1.Contains(randomTarget1Index));
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);

                    randomTarget3Index = Random.Range(0, level3TArgets.Length);
                    currActiveTargets.Add(level3TArgets[randomTarget3Index]);


                    StaticVariableManager.numCurrThreats = 1;
                    break;

                case 3:
                    randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);
                    usedIndexes_1.Add(randomTarget1Index);

                    do
                    {
                        randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    } while (usedIndexes_1.Contains(randomTarget1Index));
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);


                    randomTarget3Index = Random.Range(0, level3TArgets.Length);
                    currActiveTargets.Add(level3TArgets[randomTarget3Index]);


                    StaticVariableManager.numCurrThreats = 1;
                    break;
            }
        }
        else if (StaticVariableManager.numTargetBoards == 4)
        {
            switch (targetSet)
            {
                case 1:
                    randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);
                    usedIndexes_1.Add(randomTarget1Index);

                    do
                    {
                        randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    } while (usedIndexes_1.Contains(randomTarget1Index));
                    usedIndexes_1.Add(randomTarget1Index);
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);

                    randomTarget3Index = Random.Range(0, level3TArgets.Length);
                    currActiveTargets.Add(level3TArgets[randomTarget3Index]);

                    do
                    {
                        randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    } while (usedIndexes_1.Contains(randomTarget1Index));
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);


                    StaticVariableManager.numCurrThreats = 1;
                    break;

                case 2:
                    randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);
                    usedIndexes_1.Add(randomTarget1Index);

                    do
                    {
                        randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    } while (usedIndexes_1.Contains(randomTarget1Index));
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);

                    randomTarget3Index = Random.Range(0, level3TArgets.Length);
                    currActiveTargets.Add(level3TArgets[randomTarget3Index]);
                    usedIndexes_3.Add(randomTarget3Index);

                    do
                    {
                        randomTarget3Index = Random.Range(0, level3TArgets.Length);
                    } while (usedIndexes_3.Contains(randomTarget3Index));
                    currActiveTargets.Add(level3TArgets[randomTarget3Index]);


                    StaticVariableManager.numCurrThreats = 2;
                    break;

                case 3:
                    randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);
                    usedIndexes_1.Add(randomTarget1Index);

                    do
                    {
                        randomTarget1Index = Random.Range(0, level1TArgets.Length);
                    } while (usedIndexes_1.Contains(randomTarget1Index));
                    currActiveTargets.Add(level1TArgets[randomTarget1Index]);

                    randomTarget3Index = Random.Range(0, level3TArgets.Length);
                    currActiveTargets.Add(level3TArgets[randomTarget3Index]);
                    usedIndexes_3.Add(randomTarget3Index);

                    do
                    {
                        randomTarget3Index = Random.Range(0, level3TArgets.Length);
                    } while (usedIndexes_3.Contains(randomTarget3Index));
                    currActiveTargets.Add(level3TArgets[randomTarget3Index]);


                    StaticVariableManager.numCurrThreats = 2;
                    break;
            }
        }

    }

    private void ActivateTargets()
    {
        int spawn_index = 0;

        Shuffle(currActiveTargets); // Shuffle before using

        if(StaticVariableManager.numTargetBoards == 1)
        {
            foreach (GameObject target in currActiveTargets)
            {
                SpawnTarget(SpawnLocations_1[spawn_index], target);
                spawn_index++;
            }
        }
        else if (StaticVariableManager.numTargetBoards == 2)
        {
            foreach (GameObject target in currActiveTargets)
            {
                SpawnTarget(SpawnLocations_2[spawn_index], target);
                spawn_index++;
            }
        }
        else if (StaticVariableManager.numTargetBoards == 3)
        {
            foreach (GameObject target in currActiveTargets)
            {
                SpawnTarget(SpawnLocations_3[spawn_index], target);
                spawn_index++;
            }
        }
        else if (StaticVariableManager.numTargetBoards == 4)
        {
            foreach (GameObject target in currActiveTargets)
            {
                SpawnTarget(SpawnLocations_4[spawn_index], target);
                spawn_index++;
            }
        }
    }

    private void DeactivateTargets()
    {
        foreach (GameObject target in currIntances)
        {
            Destroy(target);
        }
    }

    private void SpawnTarget(GameObject spawn_location, GameObject target)
    {
        Transform spawn = spawn_location.transform;
        GameObject instance = Instantiate(target, spawn.position, spawn.rotation);
        instance.transform.localScale = spawn_location.transform.localScale;
        instance.transform.SetParent(spawn);

        currIntances.Add(instance);
    }

    // Fisher-Yates shuffle method
    private void Shuffle(List<GameObject> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            GameObject value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


    private void SetTargetDistance(GameObject target, int size)
    {
        switch (size)
        {
            case 10:
                target.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f); // Example scale
                break;
            case 15:
                target.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f); // Example scale
                break;
            case 20:
                target.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); // Example scale
                break;
            case 25:
                target.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f); // Example scale
                break;
        }
    }

}
