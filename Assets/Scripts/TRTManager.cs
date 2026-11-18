using System.Collections.Generic;
using UnityEngine;

public class TRTManager : MonoBehaviour
{[Header ("Spawn Location")]
    public GameObject[] SpawnLocations;

    [Header ("Target Holders")]
    public GameObject[] level1TArgets;
    public GameObject[] level2TArgets;
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

        if(!isTargetBoardsSet && countDownStart.start_training)
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

        targetSet = Random.Range(1,4);
        string allowed_sequence = "";

        // print(targetSet);

        switch (targetSet)
        {
            case 1:
                randomTarget1Index = Random.Range(0, level1TArgets.Length);
                currActiveTargets.Add(level1TArgets[randomTarget1Index]);

                randomTarget2Index = Random.Range(0, level1TArgets.Length);
                currActiveTargets.Add(level1TArgets[randomTarget2Index]);

                randomTarget3Index = Random.Range(0, level2TArgets.Length);
                currActiveTargets.Add(level2TArgets[randomTarget3Index]);

                randomTarget4Index = Random.Range(0, level3TArgets.Length);
                currActiveTargets.Add(level3TArgets[randomTarget4Index]);

                allowed_sequence = "32"; // Add Sequence
                StaticVariableManager.TRTShootSequence.Add(allowed_sequence);

                StaticVariableManager.numCurrThreats = 2;
                break;

            case 2:
                randomTarget1Index = Random.Range(0, level1TArgets.Length);
                currActiveTargets.Add(level1TArgets[randomTarget1Index]);

                randomTarget2Index = Random.Range(0, level2TArgets.Length);
                currActiveTargets.Add(level2TArgets[randomTarget2Index]);

                randomTarget3Index = Random.Range(0, level3TArgets.Length);
                currActiveTargets.Add(level3TArgets[randomTarget3Index]);

                randomTarget4Index = Random.Range(0, level3TArgets.Length);
                currActiveTargets.Add(level3TArgets[randomTarget4Index]);

                allowed_sequence = "332"; // Add Sequence
                StaticVariableManager.TRTShootSequence.Add(allowed_sequence);

                StaticVariableManager.numCurrThreats = 3;
                break;

            case 3:
                randomTarget1Index = Random.Range(0, level1TArgets.Length);
                currActiveTargets.Add(level1TArgets[randomTarget1Index]);

                randomTarget2Index = Random.Range(0, level2TArgets.Length);
                currActiveTargets.Add(level2TArgets[randomTarget2Index]);

                randomTarget3Index = Random.Range(0, level3TArgets.Length);
                currActiveTargets.Add(level3TArgets[randomTarget3Index]);

                randomTarget4Index = Random.Range(0, level4TArgets.Length);
                currActiveTargets.Add(level4TArgets[randomTarget4Index]);

                allowed_sequence = "342"; // Add Sequence
                StaticVariableManager.TRTShootSequence.Add(allowed_sequence);
                allowed_sequence = "432"; // Add Sequence
                StaticVariableManager.TRTShootSequence.Add(allowed_sequence);

                StaticVariableManager.numCurrThreats = 3;
                break;
        }

        /*foreach(string sequence in StaticVariableManager.TRTShootSequence)
        {
            print($"Set sequence {sequence}.");
        }*/

    }

    private void ActivateTargets()
    {
        int spawn_index = 0;

        Shuffle(currActiveTargets); // Shuffle before using

        foreach (GameObject target in currActiveTargets)
        {
            SpawnTarget(SpawnLocations[spawn_index], target);
            spawn_index++;
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
