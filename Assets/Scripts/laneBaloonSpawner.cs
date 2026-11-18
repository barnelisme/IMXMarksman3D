using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class laneBaloonSpawner : MonoBehaviour
{
    [System.Serializable]
    public class LaneData
    {
        public string nextColorName = "";
        public string currentColorName = "";
        public int holderSwitch = 0;
        public int colorSwitchFlag = 0;
        public bool nextColorSet = false;
        public int currentColor = 1;
        public int nextColor = 1;
        public float switchTimer;
        public float switchTimerSetVal = 0.25f;
        public float switchDelay;
        public float switchDelaySetVal = 0.2f;
        public float timeToRespawnTimer;
    }

    public GameObject[] spawnLocations;
    public GameObject[] whatToSpawnPrefab;
    public GameObject[] whatToSpawnClone;
    public GameObject[] leftColorIndicators;
    public GameObject[] rightColorIndicators;
    public List<string> baloonNamesList;

    string activeScene = "";
    int prefebIndex = 0, cloneIndex = 0;
    int spawn_location_index = 0;
    int totBaloons = 0;
    int numBaloonsToSpawn = 0;

    // Unified collections for lanes
    private Dictionary<int, LaneData> lanes = new Dictionary<int, LaneData>();
    private int currentInstanceLaneNumber = 1; // Determines which lane this specific script instance handles

    // Central timer for the center spawner behavior (originally tied to 1lane logic)
    float timeToRespawnCenter;
    int spawn_Preferb_Center = 0;

    float setTimeToRespawn = 2f;
    bool isRightIndicator = false;
    bool isLeftIndicator = false;
    bool trainingStarted = false;
    int centerSpawnSelect = 0;

    void Start()
    {
        isLeftIndicator = true;
        activeScene = SceneManager.GetActiveScene().name;
        setTimeToRespawn = StaticVariableManager.intensity;
        timeToRespawnCenter = 0;

        StaticVariableManager.lane1StrikeOut = false;
        StaticVariableManager.lane2StrikeOut = false;
        StaticVariableManager.lane3StrikeOut = false;

        // Detect which lane instance this script is attached to dynamically based on hierarchy naming
        string objectName = this.transform.name.ToLower();
        if (objectName.Contains("lane 1")) currentInstanceLaneNumber = 1;
        else if (objectName.Contains("lane 2")) currentInstanceLaneNumber = 2;
        else if (objectName.Contains("lane 3")) currentInstanceLaneNumber = 3;
        else currentInstanceLaneNumber = 1; // Default fallback

        // Initialize tracking data for lanes matching original initial delay cycles
        InitializeLane(1, 0.25f);
        InitializeLane(2, 0.5f);
        InitializeLane(3, 0.5f);
    }

    private void InitializeLane(int laneNum, float defaultTimerSet)
    {
        LaneData data = new LaneData();
        data.switchTimerSetVal = defaultTimerSet;
        data.switchTimer = data.switchTimerSetVal;
        data.switchDelay = 0f;

        lanes[laneNum] = data;
        SetLaneNextColor(laneNum);
        data.nextColorSet = true;

        // Re-sync standard config reference values after initial structural set
        data.switchTimerSetVal = StaticVariableManager.switchTimer;
    }

    void Update()
    {
        bool isTrainingPaused = StaticVariableManager.isTrainingPause;

        if (activeScene.ToLower().Contains("1lane"))
        {
            if (!isTrainingPaused)
            {
                if (!StaticVariableManager.lane1StrikeOut && !Shooting.lane1TargetsComplete)
                {
                    if (countDownStart.start_training)
                    {
                        isLeftIndicator = true;
                        isRightIndicator = false;
                        ManageLaneColorIndicator(1);
                        Manage1LaneColorHolder();
                    }
                    else
                    {
                        SwitchOffLaneIndicator(1);
                    }
                }
            }
            else
            {
                ClearAllIndicators();
            }

            // Manage Timers for Single Lane mode
            if (trainingStarted && !countDownStart.start_training)
            {
                // Idle context from original script structure
            }
            else
            {
                Manage1LaneModeTimers();
            }
        }
        else if (activeScene.ToLower().Contains("2lane") || activeScene.ToLower().Contains("3lane"))
        {
            // Execute logic matching the active lane assigned to this instance
            if (!isTrainingPaused)
            {
                bool strikeOut = GetLaneStrikeOut(currentInstanceLaneNumber);
                bool targetsComplete = GetLaneTargetsComplete(currentInstanceLaneNumber);

                if (!strikeOut && !targetsComplete)
                {
                    if (countDownStart.start_training)
                    {
                        // Dynamic indicator group assignments matching original target maps
                        if (currentInstanceLaneNumber == 1)
                        {
                            isLeftIndicator = true;
                            isRightIndicator = false;
                        }
                        else if (currentInstanceLaneNumber == 2 || currentInstanceLaneNumber == 3)
                        {
                            isLeftIndicator = false;
                            isRightIndicator = true;
                        }

                        ManageLaneColorIndicator(currentInstanceLaneNumber);
                    }
                    else
                    {
                        SwitchOffLaneIndicator(currentInstanceLaneNumber);
                    }
                }
            }

            // Deactivate UI elements dynamically if strikes/completions/pauses occur
            if (StaticVariableManager.lane1StrikeOut || Shooting.lane1TargetsComplete || isTrainingPaused)
            {
                ToggleIndicatorGroup(leftColorIndicators, false);
            }
            if (StaticVariableManager.lane2StrikeOut || Shooting.lane2TargetsComplete || StaticVariableManager.lane3StrikeOut || Shooting.lane3TargetsComplete || isTrainingPaused)
            {
                ToggleIndicatorGroup(rightColorIndicators, false);
            }

            // Timer and Spawning progression updates
            ManageMultiLaneTimers(currentInstanceLaneNumber);
        }

        UpdateSpawningConfigData();
    }

    private void UpdateLaneGlobalVariables(int laneNumber, LaneData data)
    {
        if (data.nextColorSet == false)
        {
            if (laneNumber == 1)
            {
                StaticVariableManager.currentLane1Color = data.currentColorName;
                StaticVariableManager.nextLane1Color = data.nextColorName;
            }
            else if (laneNumber == 2)
            {
                StaticVariableManager.currentLane2Color = data.currentColorName;
                StaticVariableManager.nextLane2Color = data.nextColorName;
            }
            else if (laneNumber == 3)
            {
                StaticVariableManager.currentLane3Color = data.currentColorName;
                StaticVariableManager.nextLane3Color = data.nextColorName;
            }
        }
    }

    private void Manage1LaneColorHolder()
    {
        LaneData data = lanes[1];
        if (data.switchTimer <= 0f)
        {
            data.holderSwitch = Random.Range(1, 5);
            switch (data.holderSwitch)
            {
                case 1:
                case 3:
                    isLeftIndicator = true;
                    isRightIndicator = false;
                    break;
                case 2:
                case 4:
                    isLeftIndicator = false;
                    isRightIndicator = true;
                    break;
            }
        }
    }

    private void ManageLaneColorIndicator(int laneNumber)
    {
        if (!lanes.ContainsKey(laneNumber)) return;
        LaneData data = lanes[laneNumber];

        if (data.switchTimer <= 0f)
        {
            SwitchOffLaneIndicator(laneNumber);
            if (data.switchDelay <= 0f)
            {
                ChangeIndicatorColor(laneNumber);
                data.switchTimer = data.switchTimerSetVal;
                data.switchDelay = data.switchDelaySetVal;
                data.currentColor = data.colorSwitchFlag;
                data.nextColorSet = false;
            }
            else
            {
                data.switchDelay -= Time.deltaTime;
            }
        }
        else
        {
            if (!data.nextColorSet)
            {
                SetLaneNextColor(laneNumber);
                data.nextColorSet = true;
            }
            data.switchTimer -= Time.deltaTime;
        }
    }

    private void SetLaneNextColor(int laneNumber)
    {
        LaneData data = lanes[laneNumber];
        data.colorSwitchFlag = Random.Range(1, whatToSpawnPrefab.Length);

        if (data.colorSwitchFlag == data.nextColor)
        {
            if (data.colorSwitchFlag < whatToSpawnPrefab.Length) data.colorSwitchFlag++;
            else data.colorSwitchFlag = 1;
        }
        data.nextColor = data.colorSwitchFlag;

        string[] colorNames = { "Black", "Blue", "Brown", "Green", "Red" };

        int nextIdx = data.nextColor - 1;
        if (nextIdx >= 0 && nextIdx < colorNames.Length) data.nextColorName = colorNames[nextIdx];

        int currentIdx = data.currentColor - 1;
        if (currentIdx >= 0 && currentIdx < colorNames.Length) data.currentColorName = colorNames[currentIdx];

        UpdateLaneGlobalVariables(laneNumber, data);
    }

    private void SwitchOffLaneIndicator(int laneNumber)
    {
        if (isLeftIndicator) ToggleIndicatorGroup(leftColorIndicators, false);
        if (isRightIndicator) ToggleIndicatorGroup(rightColorIndicators, false);
        StaticVariableManager.nextColorSet = false;
    }

    private void ChangeIndicatorColor(int laneNumber)
    {
        LaneData data = lanes[laneNumber];

        if (isLeftIndicator) SetActiveIndicatorByFlag(leftColorIndicators, data.colorSwitchFlag);
        if (isRightIndicator) SetActiveIndicatorByFlag(rightColorIndicators, data.colorSwitchFlag);

        StaticVariableManager.nextColorSet = true;
    }

    private void SetActiveIndicatorByFlag(GameObject[] indicators, int flagValue)
    {
        int counter = 1;
        foreach (GameObject obj in indicators)
        {
            if (obj != null)
            {
                obj.SetActive(counter == flagValue);
            }
            counter++;
        }
    }

    private void Manage1LaneModeTimers()
    {
        UpdateRespawnBaseValue();
        float[] additions = { 1f, 1.5f, 1.6f, 1.8f };

        // Process Lane 1 Spawner
        LaneData lane1 = lanes[1];
        if (lane1.timeToRespawnTimer <= 0f)
        {
            int randomIndex = Random.Range(0, whatToSpawnPrefab.Length + 8);
            int spawnIndex = (randomIndex < whatToSpawnPrefab.Length) ? randomIndex : (lane1.nextColor - 1);

            SpawnBalloonDynamic(spawnIndex, 0, 2);
            lane1.timeToRespawnTimer = setTimeToRespawn + additions[Random.Range(0, additions.Length)];
        }
        else
        {
            lane1.timeToRespawnTimer -= Time.deltaTime;
        }

        // Process Lane 2 Spawner
        LaneData lane2 = lanes[2];
        if (lane2.timeToRespawnTimer <= 0f)
        {
            int randomIndex = Random.Range(0, whatToSpawnPrefab.Length + 8);
            int spawnIndex = (randomIndex < whatToSpawnPrefab.Length) ? randomIndex : (lane1.nextColor - 1);

            SpawnBalloonDynamic(spawnIndex, 1, 3);
            lane2.timeToRespawnTimer = setTimeToRespawn + additions[Random.Range(0, additions.Length)];
        }
        else
        {
            lane2.timeToRespawnTimer -= Time.deltaTime;
        }

        // Process Spawner Center
        if (timeToRespawnCenter <= 0f)
        {
            int randomIndex = Random.Range(0, whatToSpawnPrefab.Length + 8);
            spawn_Preferb_Center = (randomIndex < whatToSpawnPrefab.Length) ? randomIndex : (lane1.nextColor - 1);

            SpawnBalloonDynamic(spawn_Preferb_Center, 4, 5);
            timeToRespawnCenter = setTimeToRespawn + additions[Random.Range(0, additions.Length)];
        }
        else
        {
            timeToRespawnCenter -= Time.deltaTime;
        }
    }

    private void ManageMultiLaneTimers(int laneNumber)
    {
        UpdateRespawnBaseValue();
        if (!lanes.ContainsKey(laneNumber)) return;

        bool strikeOut = GetLaneStrikeOut(laneNumber);
        if (strikeOut) return;

        LaneData data = lanes[laneNumber];
        float[] additions = { 1f, 1.5f, 1.6f, 1.8f };

        if (data.timeToRespawnTimer <= 0f)
        {
            int randomIndex = Random.Range(0, whatToSpawnPrefab.Length + 11);
            int spawnPrefabIndex = (randomIndex < whatToSpawnPrefab.Length) ? randomIndex : (data.nextColor - 1);

            int totalLanesInScene = activeScene.ToLower().Contains("3lane") ? 3 : 2;

            int upLocationIdx = laneNumber - 1;
            int downLocationIdx = (laneNumber - 1) + totalLanesInScene;

            SpawnBalloonDynamic(spawnPrefabIndex, upLocationIdx, downLocationIdx);
            data.timeToRespawnTimer = setTimeToRespawn + additions[Random.Range(0, mathMin(additions.Length, 3))];
        }
        else
        {
            data.timeToRespawnTimer -= Time.deltaTime;
        }
    }

    private void SpawnBalloonDynamic(int prefabIndex, int upwardSpawnLocIdx, int downwardSpawnLocIdx)
    {
        if (prefabIndex < 0 || prefabIndex >= whatToSpawnPrefab.Length) return;

        string dir = TestConditionsManager.baloonDirection.ToLower();
        int targetSpawnIdx = -1;

        if (dir.Contains("upwards")) targetSpawnIdx = upwardSpawnLocIdx;
        else if (dir.Contains("downwards")) targetSpawnIdx = downwardSpawnLocIdx;

        if (targetSpawnIdx != -1 && targetSpawnIdx < spawnLocations.Length && spawnLocations[targetSpawnIdx] != null)
        {
            whatToSpawnClone[prefabIndex] = Instantiate(whatToSpawnPrefab[prefabIndex], spawnLocations[targetSpawnIdx].transform.position, Quaternion.identity) as GameObject;
        }
    }

    private void UpdateRespawnBaseValue()
    {
        float speed = StaticVariableManager.verticalSpeed;
        if (speed <= 1) setTimeToRespawn = 3.5f;
        else if (speed > 1 && speed < 2) setTimeToRespawn = 2f;
        else if (speed >= 2) setTimeToRespawn = 0.8f;
    }

    private void UpdateSpawningConfigData()
    {
        setTimeToRespawn = StaticVariableManager.intensity;
        foreach (var laneKey in lanes.Keys)
        {
            lanes[laneKey].switchTimerSetVal = StaticVariableManager.switchTimer;
        }
    }

    private void ToggleIndicatorGroup(GameObject[] indicators, bool state)
    {
        foreach (GameObject obj in indicators)
        {
            if (obj != null) obj.SetActive(state);
        }
    }

    private void ClearAllIndicators()
    {
        ToggleIndicatorGroup(leftColorIndicators, false);
        ToggleIndicatorGroup(rightColorIndicators, false);
    }

    private bool GetLaneStrikeOut(int laneNum)
    {
        if (laneNum == 1) return StaticVariableManager.lane1StrikeOut;
        if (laneNum == 2) return StaticVariableManager.lane2StrikeOut;
        if (laneNum == 3) return StaticVariableManager.lane3StrikeOut;
        return false;
    }

    private bool GetLaneTargetsComplete(int laneNum)
    {
        if (laneNum == 1) return Shooting.lane1TargetsComplete;
        if (laneNum == 2) return Shooting.lane2TargetsComplete;
        if (laneNum == 3) return Shooting.lane3TargetsComplete;
        return false;
    }

    private int mathMin(int a, int b) { return (a < b) ? a : b; }
}