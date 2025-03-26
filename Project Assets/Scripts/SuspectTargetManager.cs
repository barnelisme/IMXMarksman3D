using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using System.Globalization;

public class SuspectTargetManager : MonoBehaviour
{
    [SerializeField]
    GameObject target1, target2, target3, target4;

    [SerializeField]
    Texture2D[] threaths, nonThreaths;

    private string attachThreathTag = "threath.target", attachNonThreathTag = "nonThreath.target";
    private List<int> existingLane1NonThreats = new List<int>();
    private List<int> existingLane1Threats = new List<int>();
    private List<int> existingLane2NonThreats = new List<int>();
    private List<int> existingLane2Threats = new List<int>();
    private List<int> usedLane1boards = new List<int>();
    private List<int> usedLane2boards = new List<int>();
    private int numTargetBoards = 4;
    private int numLane1Threats = 1;
    private int numLane2Threats = 1;
    private float lane1StartTimer = 3, lane1StopTimer = 5, rotationSpeed = 1500, lane1RotationPos = 0, lane2RotationPos = 0;
    private float lane2StartTimer = 3, lane2StopTimer = 5;
    private bool lane1BoardsTurned = false, lane1CountComplete = false, lane1RotationSet = false,
        lane2BoardsTurned = false, lane2CountComplete = false, lane2RotationSet = false;
    private string activeScene = " ";

    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        lane1StartTimer = StaticVariableManager.standByTime;
        lane1StopTimer = StaticVariableManager.shootTime;
        lane2StartTimer = StaticVariableManager.standByTime;
        lane2StopTimer = StaticVariableManager.shootTime;
        target1.SetActive(false);
        target2.SetActive(false);
        target3.SetActive(false);
        target4.SetActive(false);
        //loadTargetImages();
        //print("RRE: Lane 1 Threats " + StaticVariableManager.totNumLane1Threats);
        //print("RRE: Lane 2 Threats " + StaticVariableManager.totNumLane2Threats);
    }

    void Update()
    {
        if(countDownStart.start_training)
        {
            if (!StaticVariableManager.isTrainingPause)
            {
                if (!StaticVariableManager.isResetingPoints)
                {
                    startCount();
                    setTargetBoards();
                }
            }
            else
            {
                if (!lane1CountComplete)
                {
                    StaticVariableManager.isLane1BoardSet = true;
                    StaticVariableManager.isLane2BoardSet = true;
                    setTargetBoards();
                }
            }
        }   
    }

    private void startCount()
    {
        if(activeScene.ToLower().Contains("1lane"))
        {
            if (!StaticVariableManager.isLane1BoardSet)
            {
                lane1StartTimer -= Time.deltaTime * 1;
                if (lane1StartTimer <= 0f)
                {
                    StaticVariableManager.isLane1BoardSet = true;
                    StaticVariableManager.isLane2BoardSet = true;
                    loadTargetImages();
                }
                lane1StopTimer = StaticVariableManager.shootTime;
            }
            else
            {
                lane1StartTimer = StaticVariableManager.standByTime;
                lane1StopTimer -= Time.deltaTime;
                if (lane1StopTimer <= 0f)
                {
                    StaticVariableManager.isLane1BoardSet = false;
                    StaticVariableManager.isLane2BoardSet = false;
                }
            }
        }
        else if(activeScene.ToLower().Contains("2lane"))
        {
            handleLane1Timers();
            handleLane2Timers();
        }
    }
    private void handleLane1Timers()
    {
        if (!StaticVariableManager.isLane1BoardSet)
        {
            lane1StartTimer -= Time.deltaTime * 1;
            if (lane1StartTimer <= 0f)
            {
                StaticVariableManager.isLane1BoardSet = true;
                numTargetBoards = 2;
                setupLane1();
            }
            lane1StopTimer = StaticVariableManager.shootTime;
        }
        else if (StaticVariableManager.isLane1BoardSet)
        {
            lane1StartTimer = StaticVariableManager.standByTime;
            lane1StopTimer -= Time.deltaTime;
            if (lane1StopTimer <= 0f)
            {
                StaticVariableManager.isLane1BoardSet = false;
            }
        }
    }
    private void handleLane2Timers()
    {
        if (!StaticVariableManager.isLane2BoardSet)
        {
            lane2StartTimer -= Time.deltaTime * 1;
            if (lane2StartTimer <= 0f)
            {
                StaticVariableManager.isLane2BoardSet = true;
                numTargetBoards = 2;
                setupLane2();
            }
            lane2StopTimer = StaticVariableManager.shootTime;
        }
        else if (StaticVariableManager.isLane2BoardSet)
        {
            lane2StartTimer = StaticVariableManager.standByTime;
            lane2StopTimer -= Time.deltaTime;
            if (lane2StopTimer <= 0f)
            {
                StaticVariableManager.isLane2BoardSet = false;
            }
        }
    }

    private void loadTargetImages()
    {
        if (activeScene.ToLower().Contains("1lane") && !lane1BoardsTurned)
        {
            setLane1TrainingLevel();
            numTargetBoards = 4;
            int x = 1;
            int randonThreathIndex = 0;
            int randonNonThreathIndex = 0;
            int randomTargetBoard = 0;

            while (x <= numLane1Threats)
            {
                randomTargetBoard = chooseLane1RandomBoard(randomTargetBoard);
                randonThreathIndex = chooseRandomLane1ThreatIndex(randonThreathIndex);

                switch (randomTargetBoard)
                {
                    case 1:
                        SetTextureLane1(target1, threaths[randonThreathIndex]);
                        assignLane1TargetCollider(target1, attachThreathTag, threaths[randonThreathIndex].name);
                        break;                                                                    
                    case 2:                                                                       
                        SetTextureLane1(target2, threaths[randonThreathIndex]);
                        assignLane1TargetCollider(target2, attachThreathTag, threaths[randonThreathIndex].name);
                        break;                                                                    
                    case 3:                                                                       
                        SetTextureLane1(target3, threaths[randonThreathIndex]);
                        assignLane1TargetCollider(target3, attachThreathTag, threaths[randonThreathIndex].name);
                        break;                                                                    
                    case 4:                                                                       
                        SetTextureLane1(target4, threaths[randonThreathIndex]);
                        assignLane1TargetCollider(target4, attachThreathTag, threaths[randonThreathIndex].name);
                        break;
                }
                usedLane1boards.Add(randomTargetBoard);
                existingLane1Threats.Add(randonThreathIndex);
                x++;
            }

            x = 0; //Erase Loop condition

            while (x <= numTargetBoards)
            {
                if (!usedLane1boards.Contains(x))
                {
                    randonNonThreathIndex = chooseRandonLane1NonthreatIndex(randonNonThreathIndex);

                    switch (x)
                    {
                        case 1:
                            SetTextureLane1(target1, nonThreaths[randonNonThreathIndex]);
                            assignLane1TargetCollider(target1, attachNonThreathTag, nonThreaths[randonNonThreathIndex].name);

                            break;
                        case 2:
                            SetTextureLane1(target2, nonThreaths[randonNonThreathIndex]);
                            assignLane1TargetCollider(target2, attachNonThreathTag, nonThreaths[randonNonThreathIndex].name);
                            break;
                        case 3:
                            SetTextureLane1(target3, nonThreaths[randonNonThreathIndex]);
                            assignLane1TargetCollider(target3, attachNonThreathTag, nonThreaths[randonNonThreathIndex].name);
                            break;
                        case 4:
                            SetTextureLane1(target4, nonThreaths[randonNonThreathIndex]);
                            assignLane1TargetCollider(target4, attachNonThreathTag, nonThreaths[randonNonThreathIndex].name);
                            break;
                    }
                }
                existingLane1NonThreats.Add(randonNonThreathIndex);
                x++;
            }

            usedLane1boards = new List<int>();
            usedLane2boards = new List<int>();
            existingLane1Threats = new List<int>();
            existingLane1NonThreats = new List<int>();

            lane1BoardsTurned = true;
        }

    }

    private void assignLane1TargetCollider(GameObject target, string tag, string imageName)
    {
        string[] colliderPosition = imageName.Split(';');
        string[] headCollider = colliderPosition[0].Split(',');
        string[] bodyCollider = colliderPosition[1].Split(',');

        //Body Variavles
        float bPosX = 0, bPosY = 0, bPosZ = 0;
        float bScaleX = 0, bScaleY = 0, bScaleZ = 0;
        //Head Variavles
        float hPosX = 0, hPosY = 0, hPosZ = 0;
        float hScaleX = 0, hScaleY = 0, hScaleZ = 0;

        int variableNumber = 1;

        foreach (string item in bodyCollider)
        {
            switch(variableNumber)
            {
                case 1:
                    bPosX = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 2:
                    bPosY = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 3:
                    bPosZ = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 4:
                    bScaleX = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 5:
                    bScaleY = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 6:
                    bScaleZ = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
            }
            variableNumber++;
        }

        variableNumber = 1;
        foreach (string item in headCollider)
        {
            switch (variableNumber)
            {
                case 1:
                    hPosX = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 2:
                    hPosY = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 3:
                    hPosZ = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 4:
                    hScaleX = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 5:
                    hScaleY = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 6:
                    hScaleZ = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
            }
            variableNumber++;
        }

        if(target.name.ToLower().Contains("1.board"))
        {
            target.transform.tag = "Lane1MissCollider";
        }
        if (target.name.ToLower().Contains("2.board"))
        {
            target.transform.tag = "Lane2MissCollider";
        }

        target.GetComponent<TargetMessageReceive>().headCollider.transform.tag = tag;
        target.GetComponent<TargetMessageReceive>().bodyCollider.transform.tag = tag;

        //Set head collider
        target.GetComponent<TargetMessageReceive>().headCollider.transform.localPosition = new Vector3(bPosX, bPosY, bPosZ);
        target.GetComponent<TargetMessageReceive>().headCollider.transform.localScale = new Vector3(bScaleX, bScaleY, bScaleZ);
        //Set body collider
        target.GetComponent<TargetMessageReceive>().bodyCollider.transform.localPosition = new Vector3(hPosX, hPosY, hPosZ);
        target.GetComponent<TargetMessageReceive>().bodyCollider.transform.localScale = new Vector3(hScaleX, hScaleY, hScaleZ);
    }
    private void assignLane2TargetCollider(GameObject target, string tag, string imageName)
    {
        string[] colliderPosition = imageName.Split(';');
        string[] headCollider = colliderPosition[0].Split(',');
        string[] bodyCollider = colliderPosition[1].Split(',');

        //Body Variavles
        float bPosX = 0, bPosY = 0, bPosZ = 0;
        float bScaleX = 0, bScaleY = 0, bScaleZ = 0;
        //Head Variavles
        float hPosX = 0, hPosY = 0, hPosZ = 0;
        float hScaleX = 0, hScaleY = 0, hScaleZ = 0;

        int variableNumber = 1;

        foreach (string item in bodyCollider)
        {
            switch (variableNumber)
            {
                case 1:
                    bPosX = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 2:
                    bPosY = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 3:
                    bPosZ = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 4:
                    bScaleX = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 5:
                    bScaleY = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 6:
                    bScaleZ = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
            }
            variableNumber++;
        }

        variableNumber = 1;
        foreach (string item in headCollider)
        {
            switch (variableNumber)
            {
                case 1:
                    hPosX = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 2:
                    hPosY = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 3:
                    hPosZ = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 4:
                    hScaleX = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 5:
                    hScaleY = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
                case 6:
                    hScaleZ = float.Parse(item, CultureInfo.InvariantCulture);
                    break;
            }
            variableNumber++;
        }

        if (target.name.ToLower().Contains("1.board"))
        {
            target.transform.tag = "Lane1MissCollider";
        }
        if (target.name.ToLower().Contains("2.board"))
        {
            target.transform.tag = "Lane2MissCollider";
        }

        target.GetComponent<TargetMessageReceive>().headCollider.transform.tag = tag;
        target.GetComponent<TargetMessageReceive>().bodyCollider.transform.tag = tag;

        //Set head collider
        target.GetComponent<TargetMessageReceive>().headCollider.transform.localPosition = new Vector3(bPosX, bPosY, bPosZ);
        target.GetComponent<TargetMessageReceive>().headCollider.transform.localScale = new Vector3(bScaleX, bScaleY, bScaleZ);
        //Set body collider
        target.GetComponent<TargetMessageReceive>().bodyCollider.transform.localPosition = new Vector3(hPosX, hPosY, hPosZ);
        target.GetComponent<TargetMessageReceive>().bodyCollider.transform.localScale = new Vector3(hScaleX, hScaleY, hScaleZ);
    }

    private void setTargetBoards()
    {
        if(activeScene.ToLower().Contains("1lane"))
        {
            if (StaticVariableManager.isLane1BoardSet)
            {
                target1.SetActive(true);
                target2.SetActive(true);
                target3.SetActive(true);
                target4.SetActive(true);

                if(!lane1RotationSet)
                {
                    target1.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
                    target2.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
                    target3.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
                    target4.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

                    lane1RotationPos += rotationSpeed * Time.deltaTime;

                    if (lane1RotationPos >= 180)
                    {
                        lane1CountComplete = true;
                        lane1RotationSet = true;

                        target1.transform.rotation = Quaternion.Euler(0f, 360f, 0f);
                        target2.transform.rotation = Quaternion.Euler(0f, 360f, 0f);
                        target3.transform.rotation = Quaternion.Euler(0f, 360f, 0f);
                        target4.transform.rotation = Quaternion.Euler(0f, 360f, 0f);
                    }

                }
            }
            else
            {

                lane1CountComplete = false;
                lane1BoardsTurned = false;
                lane1RotationSet = false;
                lane1RotationPos = 0;
                target1.SetActive(false);
                target2.SetActive(false);
                target3.SetActive(false);
                target4.SetActive(false);

                target1.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                target2.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                target3.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                target4.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
        }

        if (activeScene.ToLower().Contains("2lane"))
        {

            //Lane 1
            if (StaticVariableManager.isLane1BoardSet)
            {
                target1.SetActive(true);
                target2.SetActive(true);

                if (!lane1RotationSet)
                {
                    target1.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
                    target2.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

                    lane1RotationPos += rotationSpeed * Time.deltaTime;
                    if (lane1RotationPos >= 180f)
                    {
                        lane1CountComplete = true;
                        lane1RotationSet = true;

                        target1.transform.rotation = Quaternion.Euler(0f, 360f, 0f);
                        target2.transform.rotation = Quaternion.Euler(0f, 360f, 0f);
                    }
                }
            }
            else if (!StaticVariableManager.isLane1BoardSet)
            {
                target1.SetActive(false);
                target2.SetActive(false);

                lane1CountComplete = false;
                lane1BoardsTurned = false;
                lane1RotationSet = false;
                lane1RotationPos = 0;

                target1.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                target2.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }

            //Lane 2
            if (StaticVariableManager.isLane2BoardSet)
            {
                target3.SetActive(true);
                target4.SetActive(true);
                if (!lane2RotationSet)
                {
                    target3.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
                    target4.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

                    lane2RotationPos += rotationSpeed * Time.deltaTime;
                    if (lane2RotationPos >= 180f)
                    {

                        target3.transform.rotation = Quaternion.Euler(0f, 360f, 0f);
                        target4.transform.rotation = Quaternion.Euler(0f, 360f, 0f);
                        lane2CountComplete = true;
                        lane2RotationSet = true;
                    }
                }
            }
            else if (!StaticVariableManager.isLane2BoardSet)
            {
                target3.SetActive(false);
                target4.SetActive(false);
                lane2CountComplete = false;
                lane2BoardsTurned = false;
                lane2RotationSet = false;
                lane2RotationPos = 0;

                target3.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                target4.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }

        }
    }

    private void setLane1TrainingLevel()
    {
        switch(TestConditionsManager.trainingLevel)
        {
            case "Easy":
                numLane1Threats = 1;
                break;
            case "Medium":
                if (activeScene.ToLower().Contains("1lane"))
                {
                    numLane1Threats = 2;
                }
                if (activeScene.ToLower().Contains("2lane"))
                {
                    numLane1Threats = 1;
                }
                break;
            case "Hard":
                if(activeScene.ToLower().Contains("1lane"))
                {
                    numLane1Threats = 2;
                }
                if (activeScene.ToLower().Contains("2lane"))
                {
                    numLane1Threats = 1;
                }
                break;
        }

        StaticVariableManager.totNumLane1Threats = numLane1Threats;
    }
    private void setLane2TrainingLevel()
    {
        switch (TestConditionsManager.trainingLevel)
        {
            case "Easy":
                numLane2Threats = 1;
                break;
            case "Medium":
                numLane2Threats = 1;
                break;
            case "Hard":
                numLane2Threats = 1;
                break;
        }

        StaticVariableManager.totNumLane2Threats = numLane2Threats;
    }

    private void setupLane1()
    {
        int x = 1;
        int randonThreathIndex = 0;
        int randonNonThreathIndex = 0;
        int randomTargetBoard = 0;
        usedLane1boards = new List<int>();
        setLane1TrainingLevel();

        while (x <= numLane1Threats)
        {
            randomTargetBoard = chooseLane1RandomBoard(randomTargetBoard);
            randonThreathIndex = chooseRandomLane1ThreatIndex(randonThreathIndex);

            switch (randomTargetBoard)
            {
                case 1:
                    SetTextureLane1(target1, threaths[randonThreathIndex]);
                    assignLane1TargetCollider(target1, attachThreathTag, threaths[randonThreathIndex].name);
                    break;
                case 2:
                    SetTextureLane1(target2, threaths[randonThreathIndex]);
                    assignLane1TargetCollider(target2, attachThreathTag, threaths[randonThreathIndex].name);
                    break;
            }
            usedLane1boards.Add(randomTargetBoard);
            existingLane1Threats.Add(randonThreathIndex);
            x++;
        }

        x = 0; //Erase Loop condition

        while (x <= numTargetBoards)
        {
            if (!usedLane1boards.Contains(x))
            {
                randonNonThreathIndex = chooseRandonLane1NonthreatIndex(randonNonThreathIndex);

                switch (x)
                {
                    case 1:
                        SetTextureLane1(target1, nonThreaths[randonNonThreathIndex]);
                        assignLane1TargetCollider(target1, attachNonThreathTag, nonThreaths[randonNonThreathIndex].name);
                        break;
                    case 2:
                        SetTextureLane1(target2, nonThreaths[randonNonThreathIndex]);
                        assignLane1TargetCollider(target2, attachNonThreathTag, nonThreaths[randonNonThreathIndex].name);
                        break;
                }
            }
            existingLane1NonThreats.Add(randonNonThreathIndex);
            x++;
        }

        existingLane1Threats = new List<int>();
        existingLane1NonThreats = new List<int>();

    }
    private void setupLane2()
    {
        int x = 1;
        int randonThreathIndex = 0;
        int randonNonThreathIndex = 0;
        int randomTargetBoard = 0;
        usedLane2boards = new List<int>();
        setLane2TrainingLevel();

        while (x <= numLane2Threats)
        {
            randomTargetBoard = chooseLane2RandomBoard(randomTargetBoard);
            randonThreathIndex = chooseRandomLane2ThreatIndex(randonThreathIndex);

            switch (randomTargetBoard)
            {
                case 1:
                    SetTextureLane2(target3, threaths[randonThreathIndex]);
                    assignLane1TargetCollider(target3, attachThreathTag, threaths[randonThreathIndex].name);
                    break;
                case 2:
                    SetTextureLane2(target4, threaths[randonThreathIndex]);
                    assignLane1TargetCollider(target4, attachThreathTag, threaths[randonThreathIndex].name);
                    break;
            }
            usedLane2boards.Add(randomTargetBoard);
            existingLane2Threats.Add(randonThreathIndex);
            x++;
        }

        x = 0; //Erase Loop condition

        while (x <= numTargetBoards)
        {
            if (!usedLane2boards.Contains(x))
            {
                randonNonThreathIndex = chooseRandonLane2NonthreatIndex(randonNonThreathIndex);

                switch (x)
                {
                    case 1:
                        SetTextureLane2(target3, nonThreaths[randonNonThreathIndex]);
                        assignLane1TargetCollider(target3, attachNonThreathTag, nonThreaths[randonNonThreathIndex].name);
                        break;
                    case 2:
                        SetTextureLane2(target4, nonThreaths[randonNonThreathIndex]);
                        assignLane1TargetCollider(target4, attachNonThreathTag, nonThreaths[randonNonThreathIndex].name);
                        break;
                }
            }
            existingLane2NonThreats.Add(randonNonThreathIndex);
            x++;
        }

        existingLane2Threats = new List<int>();
        existingLane2NonThreats = new List<int>();
    }

    private int chooseRandonLane1NonthreatIndex(int randonNonThreathIndex)
    {
        randonNonThreathIndex = Random.Range(0, nonThreaths.Length - 1);
        while (existingLane1NonThreats.Contains(randonNonThreathIndex))
        {
            randonNonThreathIndex = Random.Range(0, 9);
        }

        return randonNonThreathIndex;
    }

    private int chooseRandomLane1ThreatIndex(int randonThreathIndex)
    {
        randonThreathIndex = Random.Range(0, threaths.Length - 1);
        while (existingLane1Threats.Contains(randonThreathIndex))
        {
            randonThreathIndex = Random.Range(0, 9);
        }

        return randonThreathIndex;
    }

    private int chooseRandonLane2NonthreatIndex(int randonNonThreathIndex)
    {
        randonNonThreathIndex = Random.Range(0, nonThreaths.Length - 1);
        while (existingLane2NonThreats.Contains(randonNonThreathIndex))
        {
            randonNonThreathIndex = Random.Range(0, 9);
        }

        return randonNonThreathIndex;
    }

    private int chooseRandomLane2ThreatIndex(int randonThreathIndex)
    {
        randonThreathIndex = Random.Range(0, threaths.Length - 1);
        while (existingLane2Threats.Contains(randonThreathIndex))
        {
            randonThreathIndex = Random.Range(0, 9);
        }

        return randonThreathIndex;
    }

    private int chooseLane1RandomBoard(int randomThreathtarget)
    {
        randomThreathtarget = Random.Range(1, 4);
        while (usedLane1boards.Contains(randomThreathtarget))
        {
            randomThreathtarget = Random.Range(1, 4);
        }

        return randomThreathtarget;
    }
    private int chooseLane2RandomBoard(int randomThreathtarget)
    {
        randomThreathtarget = Random.Range(1, 2);
        while (usedLane2boards.Contains(randomThreathtarget))
        {
            randomThreathtarget = Random.Range(1, 2);
        }

        return randomThreathtarget;
    }

    void SetTextureLane1(GameObject target, Texture2D texture)
    {
        Renderer renderer = target.GetComponent<Renderer>();

        if (renderer != null)
        {
            Material material = renderer.material;

            if (material != null)
            {
                material.mainTexture = texture;
            }
            else
            {
                Debug.LogError("Material not found on target: " + target.name);
            }
        }
        else
        {
            Debug.LogError("Renderer not found on target: " + target.name);
        }
    }
    void SetTextureLane2(GameObject target, Texture2D texture)
    {
        Renderer renderer = target.GetComponent<Renderer>();

        if (renderer != null)
        {
            Material material = renderer.material;

            if (material != null)
            {
                material.mainTexture = texture;
            }
            else
            {
                Debug.LogError("Material not found on target: " + target.name);
            }
        }
        else
        {
            Debug.LogError("Renderer not found on target: " + target.name);
        }
    }
}
