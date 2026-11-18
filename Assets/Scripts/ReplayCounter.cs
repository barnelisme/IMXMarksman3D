using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ShotData
{
    public GameObject bullet;
    public List<GameObject> targets;
    public ColorSequenceData colorData;
    public NumberSequenceData numberData;

    public ShotData(GameObject bullet, List<GameObject> targets, Color currentColor, string currentNumber)
    {
        this.bullet = bullet;
        this.targets = targets;
        this.colorData = new ColorSequenceData(currentColor);
        this.numberData = new NumberSequenceData(currentNumber);
    }
}

[System.Serializable]
public class ColorSequenceData
{
    public Color currentBoardColor;

    public ColorSequenceData(Color currColor)
    {
        currentBoardColor = currColor;
    }
}

[System.Serializable]
public class NumberSequenceData
{
    public string currentBoardNumber;

    public NumberSequenceData(string currBoardNum)
    {
        currentBoardNumber = currBoardNum;
    }
}

public class ReplayCounter : MonoBehaviour
{
    // Grouping lane specific variables into arrays (Index 0 = Lane 1, Index 1 = Lane 2, Index 2 = Lane 3)
    private Dictionary<int, ShotData>[] laneShotSequences = new Dictionary<int, ShotData>[3]
    {
        new Dictionary<int, ShotData>(),
        new Dictionary<int, ShotData>(),
        new Dictionary<int, ShotData>()
    };

    private int[] laneCurrentSequences = new int[3] { 0, 0, 0 };
    private int[] currLaneReplaySeqs = new int[3] { 0, 0, 0 };
    private bool[] isLaneAllShots = new bool[3] { false, false, false };

    public GameObject[] replayBoard;
    public GameObject[] replayManagers;
    private GameObject currReplayBoard;
    private GameObject currReplayManager;

    public GameObject colorIndicator;
    public TextMesh targetBoardText;
    private GameObject currSetScoreBoard;
    private GameObject mainPlayer;
    public int currReplayLane = 1; // 1-based indexing for UI and configuration logic
    private string signalOrigin = "replay button";
    private string activeScene = "";
    private List<string> replayBoardData = new List<string>();
    private List<string> backup_replayData = new List<string>();

    [Header("Text variables")]
    public TextMeshProUGUI[] currLaneText;
    public TextMeshProUGUI[] currShotText;

    private void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        mainPlayer = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (StaticVariableManager.isStopTraining && ShotsReplayManager.isShotsReplayOpen)
        {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                SetPrevShot();
            }
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                SetNextShot();
            }
        }
    }

    // ==========================================
    // UNIFIED RECORD SHOT METHODS (All Lanes Link Here)
    // ==========================================
    public void RecordLaneShot(int lane, GameObject bullet, List<GameObject> target, Color boardColor, string currTargetNum)
    {
        int index = lane - 1;
        if (index < 0 || index >= laneShotSequences.Length) return;

        laneCurrentSequences[index]++;
        laneShotSequences[index][laneCurrentSequences[index]] = new ShotData(bullet, new List<GameObject>(target), boardColor, currTargetNum);
    }

    // Overloads mirroring your original entry points for backward-compatibility
    public void RecordLane1Shot(GameObject bullet, List<GameObject> target) => RecordLaneShot(1, bullet, target, Color.black, "");
    public void RecordLane1Shot(GameObject bullet, List<GameObject> target, Color BoardColor) => RecordLaneShot(1, bullet, target, BoardColor, "");
    public void RecordLane1Shot(GameObject bullet, List<GameObject> target, string currTargetNum) => RecordLaneShot(1, bullet, target, Color.black, currTargetNum);

    public void RecordLane2Shot(GameObject bullet, List<GameObject> target) => RecordLaneShot(2, bullet, target, Color.black, "");
    public void RecordLane2Shot(GameObject bullet, List<GameObject> target, Color boardColor) => RecordLaneShot(2, bullet, target, boardColor, "");
    public void RecordLane2Shot(GameObject bullet, List<GameObject> target, string currTargetNum) => RecordLaneShot(2, bullet, target, Color.black, currTargetNum);

    public void RecordLane3Shot(GameObject bullet, List<GameObject> target) => RecordLaneShot(3, bullet, target, Color.black, "");
    public void RecordLane3Shot(GameObject bullet, List<GameObject> target, Color boardColor) => RecordLaneShot(3, bullet, target, boardColor, "");
    public void RecordLane3Shot(GameObject bullet, List<GameObject> target, string currTargetNum) => RecordLaneShot(3, bullet, target, Color.black, currTargetNum);

    // ==========================================
    // REPLAY CONTROLS & MANAGEMENT
    // ==========================================
    private int GetLaneLimit()
    {
        if (activeScene.ToLower().Contains("1lane")) return 1;
        if (activeScene.ToLower().Contains("2lane")) return 2;
        if (activeScene.ToLower().Contains("3lane")) return 3;
        return 1;
    }

    public void SetNextLane()
    {
        int laneLimit = GetLaneLimit();
        if (currReplayLane < laneLimit) currReplayLane++;
        UpdateLaneUIText();
        ResetReplay();
    }

    public void SetPrevLane()
    {
        if (currReplayLane > 1) currReplayLane--;
        UpdateLaneUIText();
        ResetReplay();
    }

    private void UpdateLaneUIText()
    {
        foreach (TextMeshProUGUI UI_text in currLaneText)
        {
            UI_text.text = "Lane: " + currReplayLane;
        }
        print("Current lane: " + currReplayLane);
    }

    public void SetNextShot()
    {
        int idx = currReplayLane - 1;
        if (idx >= 0 && idx < laneShotSequences.Length)
        {
            LaneNextShot(ref isLaneAllShots[idx], laneShotSequences[idx], ref currLaneReplaySeqs[idx]);
        }
    }

    public void SetPrevShot()
    {
        int idx = currReplayLane - 1;
        if (idx >= 0 && idx < laneShotSequences.Length)
        {
            LanePrevShot(laneShotSequences[idx], ref currLaneReplaySeqs[idx]);
        }
    }

    public void SetAllShot()
    {
        int idx = currReplayLane - 1;
        if (idx >= 0 && idx < laneShotSequences.Length)
        {
            ShotsReplayManager manager = this.GetComponent<ShotsReplayManager>();
            GameObject laneScoreBoard = idx == 0 ? manager.replay_score_1 : (idx == 1 ? manager.replay_score_2 : manager.replay_score_3);

            LaneAllShots(ref isLaneAllShots[idx], laneShotSequences[idx], ref currLaneReplaySeqs[idx], laneScoreBoard);
        }
    }

    public void SetupScoreReplay(string scoreLane, string signal_origin)
    {
        int replayLaneResetVal = currReplayLane;
        signalOrigin = signal_origin;

        currReplayLane = int.Parse(scoreLane);
        SetAllShot();
        currReplayLane = replayLaneResetVal; // Reset to original replay lane setting.
    }

    private void LaneNextShot(ref bool isAllShots, Dictionary<int, ShotData> sequence, ref int currReplaySequence)
    {
        if (isAllShots)
        {
            LaneAllShots(ref isAllShots, sequence, ref currReplaySequence, currSetScoreBoard);
        }
        else
        {
            DeactivateCurrReplay(sequence, currReplaySequence);
            if (currReplaySequence < sequence.Count) currReplaySequence++;

            ActivateCurrReplay(sequence, currReplaySequence, "one hit");
            HandleReplayData(currReplaySequence, "next");
        }
    }

    private void LanePrevShot(Dictionary<int, ShotData> sequence, ref int currReplaySequence)
    {
        DeactivateCurrReplay(sequence, currReplaySequence);
        if (currReplaySequence > 1) currReplaySequence--;

        ActivateCurrReplay(sequence, currReplaySequence, "one hit");
        HandleReplayData(currReplaySequence, "prev");
    }

    private void LaneAllShots(ref bool isAllShots, Dictionary<int, ShotData> sequence, ref int currReplaySequence, GameObject scoreBoard)
    {

        currSetScoreBoard = scoreBoard;
        if (isAllShots == false || signalOrigin == "score button")
        {


            foreach (KeyValuePair<int, ShotData> entry in sequence)
            {
                DeactivateCurrReplay(sequence, entry.Key);
            }

            foreach (KeyValuePair<int, ShotData> entry in sequence)
            {
                ActivateCurrReplay(sequence, entry.Key, "all hits");
            }

            foreach (GameObject board in replayBoard) { board.SetActive(false); }
            if (currReplayBoard != null) currReplayBoard.GetComponent<ReplayDataTextManager>().replayText.text = "";

            signalOrigin = "replay button";
            // currSetScoreBoard.SetActive(true);
            // if (!StaticVariableManager.CIScenes.Contains(DropDown.ompSceneName))
            //  scoreBoard.SetActive(true);
            isAllShots = true;
        }
        else
        {
            currReplayBoard.GetComponent<ReplayDataTextManager>().replayText.text = "";
            ResetReplayBoardData();
            HandleReplayBoard();
            ResetLaneData(ref isAllShots, sequence, ref currReplaySequence);
            scoreBoard.SetActive(false);
        }
    }

    private void HandleReplayData(int currReplay, string call_origin)
    {
        if (!currReplayBoard.GetComponent<ReplayDataTextManager>().replayText.text.ToLower().Contains($"shot {currReplay}")
            || call_origin == "prev")
        {
            string replayString = "";
            string splitTime = GetSplitTime(currReplay);

            switch (call_origin)
            {
                case "reset":
                    replayBoardData.Add($"Shot {currReplay}: {splitTime}");
                    break;

                case "next":
                    if (replayBoardData.Count >= 14)
                    {
                        replayBoardData.Add($"Shot {currReplay}: {splitTime}");
                        backup_replayData.Add(replayBoardData[0]);
                        replayBoardData.RemoveAt(0);
                    }
                    else
                    {
                        if (currReplay != 0) replayBoardData.Add($"Shot {currReplay}: {splitTime}");
                    }
                    break;

                case "prev":
                    if (!replayBoardData[0].Contains("Shot 1:"))
                    {
                        replayBoardData.RemoveAt(replayBoardData.Count - 1);

                        List<string> tempList = new List<string> { backup_replayData[backup_replayData.Count - 1] };
                        backup_replayData.RemoveAt(backup_replayData.Count - 1);

                        tempList.AddRange(replayBoardData);
                        replayBoardData = tempList;
                    }
                    else
                    {
                        if (replayBoardData.Count != 1)
                            replayBoardData.RemoveAt(replayBoardData.Count - 1);
                    }
                    break;
            }

            int lastIndex = replayBoardData.Count - 1;
            for (int i = 0; i <= lastIndex; i++)
            {
                string data = replayBoardData[i];
                if (i == lastIndex)
                    replayString += $"<color=red>{data}</color>\n";
                else
                    replayString += data + "\n";
            }

            currReplayBoard.GetComponent<ReplayDataTextManager>().replayText.text = replayString;
        }
    }

    private string GetSplitTime(int currReplay)
    {
        string split_time = "NA";
        Shooting shootingScript = mainPlayer.GetComponent<Shooting>();

        if (currReplayLane == 1 && shootingScript.lane1ResponseTime.Count != 0)
            split_time = shootingScript.lane1ResponseTime[currReplay - 1];
        else if (currReplayLane == 2 && shootingScript.lane2ResponseTime.Count != 0)
            split_time = shootingScript.lane2ResponseTime[currReplay - 1];
        else if (currReplayLane == 3 && shootingScript.lane3ResponseTime.Count != 0)
            split_time = shootingScript.lane3ResponseTime[currReplay - 1];

        if (split_time.Contains("M"))
            split_time = split_time.Replace(":M", " | Miss");
        else if (split_time.Contains("H"))
            split_time = split_time.Replace(":H", " | Hit");

        return split_time;
    }

    private void HandleReplayBoard()
    {
        if (activeScene.ToLower().Contains("1lane"))
        {
            currReplayBoard = replayBoard[0];
            currReplayManager = replayManagers[0];
        }
        else if (activeScene.ToLower().Contains("2lane"))
        {
            currReplayBoard = currReplayLane == 1 ? replayBoard[1] : replayBoard[0];
            currReplayManager = currReplayLane == 1 ? replayManagers[0] : replayManagers[1];
        }
        else if (activeScene.ToLower().Contains("3lane"))
        {
            currReplayBoard = currReplayLane == 2 ? replayBoard[1] : replayBoard[0];
            currReplayManager = currReplayLane == 2 ? replayManagers[0] : replayManagers[1];
        }

        foreach (GameObject board in replayBoard) { board.SetActive(false); }
        foreach (GameObject manager in replayManagers) { manager.SetActive(false); }

        currReplayBoard.SetActive(true);
        currReplayManager.SetActive(true);

        currReplayBoard.GetComponent<ReplayDataTextManager>().replayText.text = "";
        HandleReplayData(1, "reset");
    }

    public void ResetReplay()
    {
        print("Point reached...");
        if (currSetScoreBoard != null) currSetScoreBoard.SetActive(false);
        if (currReplayBoard != null) currReplayBoard.SetActive(false);
        ResetReplayBoardData();
        HandleReplayBoard();

        for (int i = 0; i < 3; i++)
        {
            ResetLaneData(ref isLaneAllShots[i], laneShotSequences[i], ref currLaneReplaySeqs[i]);
        }
    }

    private void ResetLaneData(ref bool isAllShots, Dictionary<int, ShotData> sequence, ref int currReplaySequence)
    {
        foreach (KeyValuePair<int, ShotData> entry in sequence)
        {
            DeactivateCurrReplay(sequence, entry.Key);
        }

        currReplaySequence = 0;
        isAllShots = false;

        if (isAllShots)
        {
            LaneAllShots(ref isAllShots, sequence, ref currReplaySequence, currSetScoreBoard);
        }
        else
        {
            DeactivateCurrReplay(sequence, currReplaySequence);
            if (currReplaySequence < sequence.Count) currReplaySequence++;
            ActivateCurrReplay(sequence, currReplaySequence, "one hit");
        }
    }

    private void ActivateCurrReplay(Dictionary<int, ShotData> sequence, int currSeq, string replayType)
    {
        if (currSeq == 0 || !sequence.ContainsKey(currSeq)) return;

        ShotData data = sequence[currSeq];


        if (data.bullet != null)
        {
            data.bullet.SetActive(true);
            print("Activating Target: " + data.bullet.name);
            foreach (TextMeshProUGUI UI_text in currShotText)
            {
                UI_text.text = currSeq.ToString();
            }

            BulletManager bulletManager = data.bullet.GetComponent<BulletManager>();
            if (replayType == "all hits")
            {
                bulletManager.number_canvas.SetActive(true);
                bulletManager.bullet_number_text.text = currSeq.ToString();
            }
            else if (replayType == "one hit")
            {
                bulletManager.time_canvas.SetActive(true);
            }
        }

        if (data.targets != null)
        {
            // print("Targets: " + data.targets.Count);
            foreach (GameObject target in data.targets)
            {
                if (target != null)
                {
                    //print("Activating Target: " + target.name);
                    target.SetActive(true);
                }
            }
        }

        if (data.colorData != null && activeScene.ToLower().Contains("colorsequence"))
        {
            colorIndicator.GetComponent<MeshRenderer>().material.color = data.colorData.currentBoardColor;
        }

        if (data.numberData != null && activeScene.ToLower().Contains("sequencenum"))
        {
            targetBoardText.text = data.numberData.currentBoardNumber;
        }
    }

    private void DeactivateCurrReplay(Dictionary<int, ShotData> sequence, int currSeq)
    {
        if (currSeq == 0 || !sequence.ContainsKey(currSeq)) return;

        ShotData data = sequence[currSeq];

        if (data.bullet != null)
        {
            data.bullet.SetActive(false);
            BulletManager bulletManager = data.bullet.GetComponent<BulletManager>();
            bulletManager.number_canvas.SetActive(false);
            bulletManager.time_canvas.SetActive(false);
        }

        if (data.targets != null)
        {
            foreach (GameObject target in data.targets)
            {
                if (target != null) target.SetActive(false);
            }
        }

        if (data.colorData != null && activeScene.ToLower().Contains("colorsequence"))
        {
            colorIndicator.GetComponent<MeshRenderer>().material.color = Color.black;
        }

        if (data.numberData != null && activeScene.ToLower().Contains("sequencenum"))
        {
            targetBoardText.text = "0";
        }
    }

    private void ResetReplayBoardData()
    {
        replayBoardData.Clear();
        backup_replayData.Clear();
    }

    // ==========================================
    // EXTERNAL TARGET SETUP TRACKING CONTROLLER
    // ==========================================
    public void SetCurrReplayTarget(string status, GameObject currTarget, GameObject callerObject)
    {
        if (callerObject == null) return;
        string name = callerObject.transform.name;

        if (name.Contains("1"))
        {
            SetTarget(status, currTarget, ref StaticVariableManager.currLane1Targets, ref StaticVariableManager.lane1TargetStartCounts, ref StaticVariableManager.isLane1StartTargetSet, callerObject);
        }
        else if (name.Contains("2"))
        {
            SetTarget(status, currTarget, ref StaticVariableManager.currLane2Targets, ref StaticVariableManager.lane2TargetStartCounts, ref StaticVariableManager.isLane2StartTargetSet, callerObject);
        }
        else if (name.Contains("3"))
        {
            SetTarget(status, currTarget, ref StaticVariableManager.currLane3Targets, ref StaticVariableManager.lane3TargetStartCounts, ref StaticVariableManager.isLane3StartTargetSet, callerObject);
        }
    }

    private void SetTarget(string status, GameObject currTarget, ref List<GameObject> currLaneTargets, ref int laneStartTargetCount, ref bool isLaneStartTargetsSet, GameObject callerObject)
    {
        string sceneLower = activeScene.ToLower();

        if (sceneLower.Contains("plat") || sceneLower.Contains("duelingtree") || sceneLower.Contains("sequence"))
        {
            if (status == "start")
            {
                currLaneTargets.Add(currTarget);
                if (laneStartTargetCount < TestConditionsManager.numLaneTargets)
                {
                    laneStartTargetCount++;
                    if (laneStartTargetCount >= TestConditionsManager.numLaneTargets)
                    {
                        isLaneStartTargetsSet = true;
                    }
                }
            }
            else
            {
                currLaneTargets.Remove(callerObject);
                currLaneTargets.Add(currTarget);
            }
        }
        else if (sceneLower.Contains("ipecboard"))
        {
            if (!callerObject.transform.tag.ToLower().Contains("misscollider"))
            {
                if (status == "start")
                {
                    currLaneTargets.Add(currTarget);
                    if (laneStartTargetCount < TestConditionsManager.numLaneTargets)
                    {
                        laneStartTargetCount++;
                        if (laneStartTargetCount >= TestConditionsManager.numLaneTargets)
                        {
                            isLaneStartTargetsSet = true;
                        }
                    }
                }
                else
                {
                    currLaneTargets.Remove(callerObject);
                    currLaneTargets.Add(currTarget);
                }
            }
        }
        else if (sceneLower.Contains("pointman") || sceneLower.Contains("lanethreateningman") ||
                 sceneLower.Contains("lanethreateninghostage") || sceneLower.Contains("pointbullseye") ||
                 sceneLower.Contains("animaltarget") || sceneLower.Contains("hiddentarget"))
        {
            if (status != "start" || sceneLower.Contains("hiddentarget"))
            {
                if (currLaneTargets.Count != 0) currLaneTargets.RemoveAt(0);
                currLaneTargets.Add(currTarget);
            }
        }
        else if (!sceneLower.Contains("colorexpressionshoot"))
        {
            string tagLower = currTarget.transform.tag.ToLower();
            if (tagLower.Contains("targetsholder") || tagLower.Contains("misscollider"))
            {
                currLaneTargets.Add(currTarget);
                isLaneStartTargetsSet = true;
            }
        }
    }
}