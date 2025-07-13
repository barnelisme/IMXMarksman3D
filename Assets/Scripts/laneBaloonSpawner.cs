using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class laneBaloonSpawner : MonoBehaviour
{

    public GameObject[] spawnLocations;
    public GameObject[] whatToSpawnPrefab;
    public GameObject[] whatToSpawnClone;
    public GameObject[] leftColorIndicators;
    public GameObject[] rightColorIndicators;
    public List<string> baloonNamesList;

    string activeScene = "";
    int prefebIndex = 0, cloneIndex = 0;
    int spawn_location_index = 0;
    int spawn_Preferb_index1 = 0;
    int spawn_Preferb_index2 = 0;
    int spawn_Preferb_Center = 0;
    int totBaloons = 0;
    int numBaloonsToSpawn = 0;

    //Spawn TIMERS Variables
    float timeToRespawn;
    float timeToRespawn2;
    float timeToRespawnCenter;
    float setTimeToRespawn = 2f;

    //Lane 1 Color Indicator Varibles
    string nextLane1ColorName = "";
    string currentLane1ColorName = "";
    int lane1HolderSwitch = 0;
    int lane1ColorSwitchFlag = 0;
    bool nextLane1ColorSet = false;
    int currentLane1Color = 1;
    int nextLane1Color = 1;
    float lane1SwitchTimer;
    float lane1SwitchTimerSetVal = .5f;
    float lane1SwitchDelay;
    float lane1SwitchDelaySetVal = .2f;

    //Lane 2 Color Indicator Varibles
    string nextLane2ColorName = "";
    string currentLane2ColorName = "";
    int lane2HolderSwitch = 0;
    int lane2ColorSwitchFlag = 0;
    bool nextLane2ColorSet = false;
    int currentLane2Color = 1;
    int nextLane2Color = 1;
    float lane2SwitchTimer;
    float lane2SwitchTimerSetVal = .5f;
    float lane2SwitchDelay;
    float lane2SwitchDelaySetVal = .2f;

    bool isRightIndicator = false;
    bool isLeftIndicator = false;
    bool trainingStarted = false;
    int centerSpawnSelect = 0;

    void Start()
    {
        isLeftIndicator = true;
        //spawnLocations = GameObject.FindGameObjectsWithTag("baloonSpawnPoint");
        activeScene = SceneManager.GetActiveScene().name;
        setTimeToRespawn = StaticVariableManager.intensity;
        timeToRespawn = 0;
        timeToRespawn2 = 0;
        timeToRespawnCenter = 0;

        StaticVariableManager.lane1StrikeOut = false;
        StaticVariableManager.lane2StrikeOut = false;
        StaticVariableManager.lane3StrikeOut = false;

        //Lane 1 init
        lane1SwitchTimer = lane1SwitchTimerSetVal;
        lane1SwitchDelay = 0;
        lane1SwitchTimerSetVal = StaticVariableManager.switchTimer;

        //Lane 2 init
        lane2SwitchTimer = lane2SwitchTimerSetVal;
        lane2SwitchDelay = 0;
        lane2SwitchTimerSetVal = StaticVariableManager.switchTimer;
        setLane1NextColor();
        setLane2NextColor();
        nextLane1ColorSet = true;
        nextLane2ColorSet = true;
    }

    // Update is called once per frame
    void Update()
    {

        if(activeScene.ToLower().Contains("1lane"))
        {
            if (StaticVariableManager.isTrainingPause == false)
            {
                if (StaticVariableManager.lane1StrikeOut == false && Shooting.lane1TargetsComplete == false)
                {
                    if (countDownStart.start_training)
                    {
                        manage1LaneColorIndicator();
                        manage1LaneColorHolder();
                        //trainingStarted = true;
                    }
                    else
                    {
                        switchOff1LaneIndicator();
                    }
                }
            }
            else
            {
                foreach (GameObject obj in leftColorIndicators)
                {
                    obj.SetActive(false);
                }
                foreach (GameObject obj in rightColorIndicators)
                {
                    obj.SetActive(false);
                }
            }


            if (trainingStarted)
            {
                if (countDownStart.start_training)
                {
                    manage1LaneTimers();
                }
            }
            else
            {
                manage1LaneTimers();
            }

        }
        else if(activeScene.ToLower().Contains("2lane"))
        {
            if(StaticVariableManager.isTrainingPause == false)
            {
                if (this.transform.name.ToLower().Contains("lane 1") && !StaticVariableManager.lane1StrikeOut && !Shooting.lane1TargetsComplete)
                {
                    if (countDownStart.start_training)
                    {
                        isLeftIndicator = true;
                        manage1LaneColorIndicator();
                        //manage1LaneColorHolder();
                        //trainingStarted = true;
                    }
                    else
                    {
                        switchOff1LaneIndicator();
                    }
                }
                else if (this.transform.name.ToLower().Contains("lane 2") && !StaticVariableManager.lane2StrikeOut && !Shooting.lane2TargetsComplete)
                {
                    if (countDownStart.start_training)
                    {
                        isRightIndicator = true;
                        manage2LaneColorIndicator();
                        //manage2LaneColorHolder();
                        //trainingStarted = true;
                    }
                    else
                    {
                        switchOff2LaneIndicator();
                    }
                }
            }

            if (StaticVariableManager.lane1StrikeOut == true || Shooting.lane1TargetsComplete || StaticVariableManager.isTrainingPause == true)
            {
                foreach (GameObject obj in leftColorIndicators)
                {
                    obj.SetActive(false);
                }
            }
            if (StaticVariableManager.lane2StrikeOut == true || Shooting.lane2TargetsComplete || StaticVariableManager.isTrainingPause == true)
            {
                foreach (GameObject obj in rightColorIndicators)
                {
                    obj.SetActive(false);
                }
            }


            if (trainingStarted)
            {
                if (countDownStart.start_training)
                {
                    manage2LaneTimers();
                }
            }
            else
            {
                manage2LaneTimers();
            }

        }


        //Update
        updateSpawningData();
    }

    private void updateLane1GlobalVaribales()
    {
        if (nextLane1ColorSet == false)
        {
            StaticVariableManager.currentLane1Color = currentLane1ColorName;
            StaticVariableManager.nextLane1Color = nextLane1ColorName;
        }

    }
    private void updateLane2GlobalVaribales()
    {
        if (nextLane2ColorSet == false)
        {
            StaticVariableManager.currentLane2Color = currentLane2ColorName;
            StaticVariableManager.nextLane2Color = nextLane2ColorName;
        }

    }

    private void manage1LaneColorHolder()
    {
        if (lane1SwitchTimer <= 0f)
        {
            lane1HolderSwitch = Random.Range(1, 4);
            switch(lane1HolderSwitch)
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

    private void manage1LaneColorIndicator()
    {
        
        if(lane1SwitchTimer <= 0f)
        {
            switchOff1LaneIndicator();
            if(lane1SwitchDelay <= 0f)
            {
                changeIndicatorColor();
                lane1SwitchTimer = lane1SwitchTimerSetVal;
                lane1SwitchDelay = lane1SwitchDelaySetVal;
                currentLane1Color = lane1ColorSwitchFlag;
                nextLane1ColorSet = false;
            }
            else
            {
                //print("RE: delay" + switchDelay);
                lane1SwitchDelay -= Time.deltaTime * 1;
            }
        }
        else
        {
            if(nextLane1ColorSet == false)
            {
                setLane1NextColor();
                nextLane1ColorSet = true;
            }

            lane1SwitchTimer -= Time.deltaTime * 1;
        }
    }
    private void manage2LaneColorIndicator()
    {

        if (lane2SwitchTimer <= 0f)
        {
            switchOff1LaneIndicator();
            if (lane2SwitchDelay <= 0f)
            {
                changeIndicatorColor();
                lane2SwitchTimer = lane2SwitchTimerSetVal;
                lane2SwitchDelay = lane2SwitchDelaySetVal;
                currentLane2Color = lane2ColorSwitchFlag;
                nextLane2ColorSet = false;
            }
            else
            {
                //print("RE: delay" + switchDelay);
                lane2SwitchDelay -= Time.deltaTime * 1;
            }
        }
        else
        {
            if (nextLane2ColorSet == false)
            {
                setLane2NextColor();
                nextLane2ColorSet = true;
            }

            lane2SwitchTimer -= Time.deltaTime * 1;
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

        updateLane1GlobalVaribales();
    }
    private void setLane2NextColor()
    {
        lane2ColorSwitchFlag = Random.Range(1, whatToSpawnPrefab.Length);
        if (lane2ColorSwitchFlag == nextLane2Color)
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

        updateLane2GlobalVaribales();
    }

    private void switchOff1LaneIndicator()
    {
        if (isLeftIndicator)
        {
            foreach (GameObject obj in leftColorIndicators)
            {
                obj.SetActive(false);
            }
        }
        if (isRightIndicator)
        {
            foreach (GameObject obj in rightColorIndicators)
            {
                obj.SetActive(false);
            }
        }
        StaticVariableManager.nextColorSet = false;
    }
    private void switchOff2LaneIndicator()
    {
        if (isLeftIndicator)
        {
            foreach (GameObject obj in leftColorIndicators)
            {
                obj.SetActive(false);
            }
        }
        if (isRightIndicator)
        {
            foreach (GameObject obj in rightColorIndicators)
            {
                obj.SetActive(false);
            }
        }
        StaticVariableManager.nextColorSet = false;
    }
    private void changeIndicatorColor()
    {

        if(this.transform.name.ToLower().Contains("lane 1"))
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
            if (isRightIndicator)
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
        if (this.transform.name.ToLower().Contains("lane 2"))
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
            if (isRightIndicator)
            {
                int counter = 1;
                foreach (GameObject obj in rightColorIndicators)
                {
                    if (counter == lane2ColorSwitchFlag)
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
        StaticVariableManager.nextColorSet = true;
    }

    private void manage1LaneTimers()
    {
        int randomInc = 0;
        int randomIndex = 0;

        //spawner 1
        if (timeToRespawn <= 0f)
        {

            randomIndex = Random.Range(0, whatToSpawnPrefab.Length + 8);
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
        if (timeToRespawn2 <= 0f)
        {
            randomIndex = Random.Range(0, whatToSpawnPrefab.Length + 8);
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

        //spawner center
        if (timeToRespawnCenter <= 0f)
        {
            randomIndex = Random.Range(0, whatToSpawnPrefab.Length + 8);
            if (randomIndex < whatToSpawnPrefab.Length)
            {
                spawn_Preferb_Center = randomIndex;
            }
            else
            {
                spawn_Preferb_Center = nextLane1Color - 1;
            }
            spawnCenterBaloon();

            randomInc = Random.Range(1, 4);
            switch (randomInc)
            {
                case 1:
                    timeToRespawnCenter = setTimeToRespawn + 0.4f;
                    break;
                case 2:
                    timeToRespawnCenter = setTimeToRespawn + 0.6f;
                    break;
                case 3:
                    timeToRespawnCenter = setTimeToRespawn + 0.8f;
                    break;
                case 4:
                    timeToRespawnCenter = setTimeToRespawn + 1.4f;
                    break;

            }
        }
        else
        {
            timeToRespawnCenter -= Time.deltaTime * 1;
        }
    }
    private void manage2LaneTimers()
    {

        int randomInc = 0;
        int randomIndex = 0;

        if(this.transform.name.ToLower().Contains("lane 1") && StaticVariableManager.lane1StrikeOut == false)
        {
            //spawner 1
            if (timeToRespawn <= 0f)
            {

                randomIndex = Random.Range(0, whatToSpawnPrefab.Length + 11);
                if (randomIndex < whatToSpawnPrefab.Length)
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
        }

        if (this.transform.name.ToLower().Contains("lane 2") && StaticVariableManager.lane2StrikeOut == false)
        {
            //spawner 2
            if (timeToRespawn2 <= 0f)
            {
                randomIndex = Random.Range(0, whatToSpawnPrefab.Length + 11);
                if (randomIndex < whatToSpawnPrefab.Length)
                {
                    spawn_Preferb_index2 = randomIndex;
                }
                else
                {
                    spawn_Preferb_index2 = nextLane2Color - 1;
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
        lane1SwitchTimerSetVal = StaticVariableManager.switchTimer;
        lane2SwitchTimerSetVal = StaticVariableManager.switchTimer;
    }

    private void spawnBaloon_1()
    {
        if(TestConditionsManager.baloonDirection.ToLower().Contains("upwards"))
        {

            whatToSpawnClone[spawn_Preferb_index1] = Instantiate(whatToSpawnPrefab[spawn_Preferb_index1], spawnLocations[0].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
        }
        if(TestConditionsManager.baloonDirection.ToLower().Contains("downwards"))
        {

            whatToSpawnClone[spawn_Preferb_index1] = Instantiate(whatToSpawnPrefab[spawn_Preferb_index1], spawnLocations[2].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
        }

    }

    private void spawnBaloon_2()
    {
        if (TestConditionsManager.baloonDirection.ToLower().Contains("upwards"))
        {
            whatToSpawnClone[spawn_Preferb_index2] = Instantiate(whatToSpawnPrefab[spawn_Preferb_index2], spawnLocations[1].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
        }
        if(TestConditionsManager.baloonDirection.ToLower().Contains("downwards"))
        {
            whatToSpawnClone[spawn_Preferb_index2] = Instantiate(whatToSpawnPrefab[spawn_Preferb_index2], spawnLocations[3].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
        }
    }

    private void spawnCenterBaloon()
    {
        if (TestConditionsManager.baloonDirection.ToLower().Contains("upwards"))
        {
            whatToSpawnClone[spawn_Preferb_Center] = Instantiate(whatToSpawnPrefab[spawn_Preferb_Center], spawnLocations[4].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
        }
        if (TestConditionsManager.baloonDirection.ToLower().Contains("downwards"))
        {
            whatToSpawnClone[spawn_Preferb_Center] = Instantiate(whatToSpawnPrefab[spawn_Preferb_Center], spawnLocations[5].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
        }
    }
}
