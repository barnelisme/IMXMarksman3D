using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class lane1BaloonSpawner : MonoBehaviour
{

    public Transform[] spawnLocations;
    public GameObject[] whatToSpawnPrefab;
    public GameObject[] whatToSpawnClone;
    public GameObject[] leftColorIndicators;
    public GameObject[] rightColorIndicators;
    public GameObject[] colorHolders;
    public List<string> baloonNamesList;

    string activeScene = "";
    int prefebIndex = 0, cloneIndex = 0;
    int spawn_location_index = 0;
    int spawn_Preferb_index1 = 0;
    int spawn_Preferb_index2 = 0;
    int totBaloons = 0;
    int numBaloonsToSpawn = 0;

    //Spawn TIMERS Variables
    float timeToRespawn;
    float timeToRespawn2;
    float setTimeToRespawn = 2f;

    //Color Indicator Varibles
    string nextLane1ColorName = "";
    string currentLane1ColorName = "";
    string nextLane2ColorName = "";
    string currentLane2ColorName = "";
    int holderSwitch = 0;
    int holderIndex = 0;
    int lane1ColorSwitchFlag = 0;
    bool lane1NextColorSet = false;
    int currentLane1Color = 1;
    int nextLane1Color = 1;
    int lane2ColorSwitchFlag = 0;
    bool lane2NextColorSet = false;
    int currentLane2Color = 1;
    int nextLane2Color = 1;
    float switchTimer;
    float switchTimerSetVal = 5f;
    float switchDelay;
    float switchDelaySetVal = .2f;
    int numColors = 5;
    bool isRightIndicator = false;
    bool isLeftIndicator = false;
    bool trainingStarted = false;
   
    void Start()
    {
        isLeftIndicator = true;
        switchTimer = switchTimerSetVal;
        switchDelay = 0;
        activeScene = SceneManager.GetActiveScene().name;
        setTimeToRespawn = StaticVariableManager.intensity;
        timeToRespawn = 0;
        switchTimerSetVal = StaticVariableManager.switchTimer;
    }

    // Update is called once per frame
    void Update()
    {
        updateSpawningData();
        if(countDownStart.start_training)
        {
            manageColorIndicator();
            if(activeScene.ToLower().Contains("lane1"))
            {
                manageColorHolder();
            }
            //trainingStarted = true;
        }
        else
        {
            switchOffIndicator();
        }


        if (trainingStarted)
        {
            if (countDownStart.start_training)
            {
                manageTimers();
            }
        }
        else
        {
            manageTimers();
        }

        //Update
        switchTimerSetVal = StaticVariableManager.switchTimer;
    }

    private void updateGlobalVaribales()
    {
        if (lane1NextColorSet == false)
        {
            StaticVariableManager.currentLane1Color = currentLane1ColorName;
            StaticVariableManager.currentLane2Color = currentLane2ColorName;
            StaticVariableManager.nextLane1Color = nextLane1ColorName;
            StaticVariableManager.nextLane2Color = nextLane2ColorName;
        }
    }

    private void manageColorHolder()
    {
        if (switchTimer <= 0f)
        {
            holderSwitch = Random.Range(1, 4);
            switch(holderSwitch)
            {
                case 1:
                    isLeftIndicator = true;
                    isRightIndicator = false;
                    break;
                case 2:
                    isLeftIndicator = false;
                    isRightIndicator = true;
                    break;
                case 3:
                    isLeftIndicator = true;
                    isRightIndicator = false;
                    break;
                case 4:
                    isLeftIndicator = false;
                    isRightIndicator = true;
                    break;

            }
        }
    }

    private void manageColorIndicator()
    {
        
        if(switchTimer <= 0f)
        {
            switchOffIndicator();
            if(switchDelay <= 0f)
            {
                changeIndicatorColor();
                switchTimer = switchTimerSetVal;
                switchDelay = switchDelaySetVal;
                currentLane1Color = lane1ColorSwitchFlag;
                lane1NextColorSet = false;
            }
            else
            {
                //print("RE: delay" + switchDelay);
                switchDelay -= Time.deltaTime * 1;
            }
        }
        else
        {
            if(lane1NextColorSet == false)
            {
                setLane1NextColor();
                lane1NextColorSet = true;
            }

            switchTimer -= Time.deltaTime * 1;
        }
    }

    private void setLane1NextColor()
    {
        lane1ColorSwitchFlag = Random.Range(1, whatToSpawnPrefab.Length);
        if (lane1ColorSwitchFlag == nextLane1Color)
        {
            if (lane1ColorSwitchFlag < whatToSpawnPrefab.Length)
            {
                lane1ColorSwitchFlag++;
            }
            else
            {
                lane1ColorSwitchFlag = 1;
            }
            nextLane1Color = lane1ColorSwitchFlag;
        }
        else
        {
            nextLane1Color = lane1ColorSwitchFlag;
        }

        switch(nextLane1Color - 1)
        {
            case 0:
                nextLane1ColorName = "Black";
                break;
            case 1:
                nextLane1ColorName = "Blue";
                break;
            case 2:
                nextLane1ColorName = "Brown";
                break;
            case 3:
                nextLane1ColorName = "Green";
                break;
            case 4:
                nextLane1ColorName = "Red";
                break;

        }
        switch (currentLane1Color - 1)
        {
            case 0:
                currentLane1ColorName = "Black";
                break;
            case 1:
                currentLane1ColorName = "Blue";
                break;
            case 2:
                currentLane1ColorName = "Brown";
                break;
            case 3:
                currentLane1ColorName = "Green";
                break;
            case 4:
                currentLane1ColorName = "Red";
                break;

        }

        updateGlobalVaribales();
    }
    private void setLane2NextColor()
    {
        lane2ColorSwitchFlag = Random.Range(1, whatToSpawnPrefab.Length);
        if (lane2ColorSwitchFlag == nextLane1Color)
        {
            if (lane2ColorSwitchFlag < whatToSpawnPrefab.Length)
            {
                lane2ColorSwitchFlag++;
            }
            else
            {
                lane2ColorSwitchFlag = 1;
            }
            nextLane2Color = lane2ColorSwitchFlag;
        }
        else
        {
            nextLane2Color = lane2ColorSwitchFlag;
        }

        switch (nextLane2Color - 1)
        {
            case 0:
                nextLane2ColorName = "Black";
                break;
            case 1:
                nextLane2ColorName = "Blue";
                break;
            case 2:
                nextLane2ColorName = "Brown";
                break;
            case 3:
                nextLane2ColorName = "Green";
                break;
            case 4:
                nextLane2ColorName = "Red";
                break;

        }
        switch (currentLane2Color - 1)
        {
            case 0:
                currentLane2ColorName = "Black";
                break;
            case 1:
                currentLane2ColorName = "Blue";
                break;
            case 2:
                currentLane2ColorName = "Brown";
                break;
            case 3:
                currentLane2ColorName = "Green";
                break;
            case 4:
                currentLane2ColorName = "Red";
                break;

        }

        updateGlobalVaribales();
    }

    private void switchOffIndicator()
    {
        if (isLeftIndicator)
        {
            foreach (GameObject obj in leftColorIndicators)
            {
                obj.SetActive(false);
            }
        }
        else if (isRightIndicator)
        {
            foreach (GameObject obj in rightColorIndicators)
            {
                obj.SetActive(false);
            }
        }
    }
    private void changeIndicatorColor()
    {
        if (isLeftIndicator)
        {
            int counter = 1;
            foreach (GameObject obj in leftColorIndicators)
            {
                if (counter == lane1ColorSwitchFlag)
                {
                    obj.SetActive(true);
                }
                else
                {
                    //obj.SetActive(false);
                }
                counter++;
            }
        }
        else if (isRightIndicator)
        {
            int counter = 1;
            foreach (GameObject obj in rightColorIndicators)
            {
                if (counter == lane1ColorSwitchFlag)
                {
                    obj.SetActive(true);
                }
                else
                {
                    //obj.SetActive(false);
                }
                counter++;
            }
        }
    }

    private void manageTimers()
    {
        int randomInc = 0;
        int randomIndex = 0;
        //spawner 1
        if (timeToRespawn <= 0f)
        {

            randomIndex = Random.Range(0, whatToSpawnPrefab.Length + 9);
            if(randomIndex < whatToSpawnPrefab.Length)
            {
                spawn_Preferb_index1 = randomIndex;
            }
            else
            {
                spawn_Preferb_index1 = nextLane1Color - 1;
            }
            spawnBaloon_1();

            randomInc = Random.Range(1, 3);
            switch (randomInc)
            {
                case 1:
                    timeToRespawn = setTimeToRespawn + 0.5f;
                    break;
                case 2:
                    timeToRespawn = setTimeToRespawn + 0.75f;
                    break;
                case 3:
                    timeToRespawn = setTimeToRespawn + 0.9f;
                    break;
                case 4:
                    timeToRespawn = setTimeToRespawn + 1.2f;
                    break;

            }

        }
        else
        {
            timeToRespawn -= Time.deltaTime * 1;
        }

        //spawner 2
        if(activeScene.ToLower().Contains("Lane1"))
        {
            if (timeToRespawn2 <= 0f)
            {
                randomIndex = Random.Range(0, whatToSpawnPrefab.Length + 9);
                if (randomIndex < whatToSpawnPrefab.Length)
                {
                    spawn_Preferb_index2 = randomIndex;
                }
                else
                {
                    spawn_Preferb_index2 = nextLane1Color - 1;
                }
                spawnBaloon_2();

                randomInc = Random.Range(1, 4);
                switch (randomInc)
                {
                    case 1:
                        timeToRespawn2 = setTimeToRespawn + 0.4f;
                        break;
                    case 2:
                        timeToRespawn2 = setTimeToRespawn + 0.6f;
                        break;
                    case 3:
                        timeToRespawn2 = setTimeToRespawn + 0.8f;
                        break;
                    case 4:
                        timeToRespawn2 = setTimeToRespawn + 1.4f;
                        break;

                }
            }
            else
            {
                timeToRespawn2 -= Time.deltaTime * 1;
            }
        }


    }

    private void updateSpawningData()
    {
        setTimeToRespawn = StaticVariableManager.intensity;
    }

    private void spawnBaloon_1()
    {
        whatToSpawnClone[spawn_Preferb_index1] = Instantiate(whatToSpawnPrefab[spawn_Preferb_index1], spawnLocations[0].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
    }

    private void spawnBaloon_2()
    {
        whatToSpawnClone[spawn_Preferb_index2] = Instantiate(whatToSpawnPrefab[spawn_Preferb_index2], spawnLocations[1].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
    }
}
