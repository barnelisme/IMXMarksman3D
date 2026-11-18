using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExpressionSceneManager : MonoBehaviour
{
    [Header("Plates")]
    public List<GameObject> defaultPlates = new List<GameObject>();
    public List<TextMesh> defaultPlatesTexts = new List<TextMesh>();
    private List<GameObject> plates = new List<GameObject>();
    private List<TextMesh> platesTexts = new List<TextMesh>();
    private List<string> availableColors = new List<string>();
    private List<GameObject> expressionPlates = new List<GameObject>();
    private static GameObject plateHit;
    private float numHit = 0;
    private float expectedNumHit = 0;
    /// <summary>
    private Dictionary<string, List<string>> colorToPlates = new Dictionary<string, List<string>>();
    List<int> indexesWithMostUsedColor = new List<int>(3);
    private string mostUsedColor = "";
    private float expressionAnswer = 0;
    private int expressionSign = 0;
    /// </summary>


    [Header("Boards")]
    public List<GameObject> boards = new List<GameObject>();
    public List<TextMesh> boardTexts = new List<TextMesh>();

    //aditional colors
    Color brownColor = new Color(150f / 255f, 75f / 255f, 0f);
    Color pinkColor = new Color(255f / 255f, 192f / 255f, 203f / 255f);
    Color purpleColor = new Color(128f / 255f, 0f / 255f, 128f / 255f);
    Color defaulColor;
    Color most_used_color;

    private List<int> randomValues = new List<int>();
    private List<int> previousIndexes = new List<int>();
    private float switchOffTimer;
    private float setSwitchOffTimer = 4;
    private float colorIndicatorTimer;
    private float setColorIndicatorTimer = 0.5f;
    private bool textSwitchedOn = false;
    private bool textSwitchedOff = false;
    private static bool progressUpdated = false;
    private static string hitTargetName = "";
    private int currentProgressValue = 0;
    private static int incomingProgressValue = 0;
    private string activeScene = "";
    private bool colorIndicatorOn = false;
    private bool platesColorsSwitchedOff = false;
    private bool targetRevealed = false;
    private bool revealInProgress = false;
    private GameObject targetHit;
    private TextMesh targetTextHit;
    private float targetRevealTimer = 0;
    private float setTargetRevealTimer = 0.5f;
    private int mostUsedNumber = 0;
    private bool isStartingPointSet = false;
    private float timer = 5; bool timerComplete = false;
    private int prevAnswerIndex = 0;
    private bool isPlatesRevealed = false;
    private bool isTrainingStarted = false;
    private GameObject player;

    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        int index = 0;
        int randomVal = 0;
        switchOffTimer = setSwitchOffTimer;
        targetRevealTimer = setTargetRevealTimer;
        player = GameObject.FindGameObjectWithTag("Player");

        resetStaticVariables();
        setupNumTargets();

        if (activeScene.ToLower().Contains("colorexpression"))
        {
            StaticVariableManager.numberPlate = 8;
            GenerateColorGroup();
            //ManageExpression();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (countDownStart.start_training)
        {
            if (activeScene.ToLower().Contains("colorexpression"))
            {
                
                if (timer <= 0f && timerComplete == false)
                {
                    //SetPlates();
                    timerComplete = true;
                    timer = 5;
                }

                if(StaticVariableManager.setPlates)
                {
                    SetPlates();
                    StaticVariableManager.setPlates = false;
                }

                SetRealtimeSettings();
            }

            isPlatesRevealed = true;
        }
        else
        {
            // Only after training has started
            ShowAllPLates();
            // DeactivatePlates();
            // StaticVariableManager.lanePlatesDisabled = true;
        }

        // print(isStartingPointSet);
    }

    private void ShowAllPLates()
    {
        // countDownStart.start_training
        if(isPlatesRevealed)
        {
            for (int i = 0; i < defaultPlates.Count; i++)
            {
                defaultPlates[i].GetComponent<MeshRenderer>().material.color = Color.black;
                defaultPlates[i].transform.gameObject.SetActive(true);
                defaultPlatesTexts[i].transform.gameObject.SetActive(false);
            }

            isPlatesRevealed = false;
        }

    }

    private void SetRealtimeSettings()
    {
        SetMUColor();
    }

    public void SetPlates()
    {

        if(!isStartingPointSet)
        {
            isStartingPointSet = true;

            ActivatePlates();
            ManageExpression();
        }
        else
        {
            if(expressionPlates[1] == plateHit)
            {
                //Correct Hit
                numHit++;
                if(numHit >= expectedNumHit)
                {
                    StaticVariableManager.numberPlate = 8;
                    setupNumTargets();
                    GenerateColorGroup();
                    ManageExpression();
                }
                StaticVariableManager.lane1CorrectTargetsHits++;
                print("Correct Hit!!");
            }
            else
            {
                //Incorrect Hit
                StaticVariableManager.lane1IncorrectTargetsHits++;
                print("Incorrect Hit!!");
            }
        }
    }

    public static void SetHitMessage(GameObject plate_hit)
    {
        plateHit = plate_hit;
        StaticVariableManager.setPlates = true;
    }

    private void ActivatePlates()
    {
        plates[0].GetComponent<MeshRenderer>().material.color = most_used_color;

        for (int i = 0; i < plates.Count; i++)
        {
            plates[i].SetActive(true);
        }
    }

    private void DeactivatePlates()
    {
        //plates[0].GetComponent<MeshRenderer>().material.color = most_used_color;

        for (int i = 0; i < plates.Count; i++)
        {
            plates[i].SetActive(false);
            plates[i].transform.name = "1.plate";
            platesTexts[i].gameObject.SetActive(false);
        }
    }

    private void setupNumTargets()
    {
        int index = 0;
        numHit = 0;

        int randomTargetIndex;
        List<int> usedTargetsIndex = new List<int>();
        List<GameObject> usedTargets = new List<GameObject>();
        List<TextMesh> usedTargetsText = new List<TextMesh>();
        //List<int> prevMostUsedColorIndexes = new List<int>();

        if (indexesWithMostUsedColor.Count > 0)
        {
            for (int x = 0; x < defaultPlates.Count; x++)
            {
                if (defaultPlates[x] == plates[indexesWithMostUsedColor[1]])
                {
                    prevAnswerIndex = x;
                }
            }
        }

        plates = defaultPlates;
        platesTexts = defaultPlatesTexts;
        DeactivatePlates();

        if (!isStartingPointSet)
        {
            index = 1;
            randomTargetIndex = 12;
            plates[randomTargetIndex].SetActive(true);


            usedTargetsIndex.Add(randomTargetIndex);
            usedTargets.Add(plates[randomTargetIndex]);
            usedTargetsText.Add(platesTexts[randomTargetIndex]);
            // isStartingPoint = true;
        }
        else
        {
            //prevAnswerIndex = prevMostUsedColorIndexes[1]; // Save previous answer index

            index = 1;
            randomTargetIndex = prevAnswerIndex;
            plates[randomTargetIndex].SetActive(true);

            usedTargetsIndex.Add(randomTargetIndex);
            usedTargets.Add(plates[randomTargetIndex]);
            usedTargetsText.Add(platesTexts[randomTargetIndex]);
        }    

        while (index < StaticVariableManager.numberPlate)
        {
            do
            {
                randomTargetIndex = Random.Range(0, plates.Count - 1);
            } while (usedTargetsIndex.Contains(randomTargetIndex));

            if(isStartingPointSet) plates[randomTargetIndex].SetActive(true);

            usedTargetsIndex.Add(randomTargetIndex);
            usedTargets.Add(plates[randomTargetIndex]);
            usedTargetsText.Add(platesTexts[randomTargetIndex]);

            index++;
        }


        plates = usedTargets;
        platesTexts = usedTargetsText;
    }

    private void GenerateColorGroup()
    {
        int index = 0;
        int randomValue = 0;
        bool additionalPlateAdded = false;

        indexesWithMostUsedColor.Clear();
        expressionPlates.Clear();

        if (activeScene.ToLower().Contains("expression"))
        {

            randomValues = GenerateRandomValues();

            foreach (GameObject plate in plates)
            {
                randomValue = randomValues[index];

                switch (randomValue)
                {
                    case 1:
                        plate.GetComponent<MeshRenderer>().material.color = Color.red;
                        platesTexts[index].color = Color.white;

                        plate.transform.name += ".red";
                        availableColors.Add("red");
                        ManageMUColor(randomValue, "red");

                        if (!colorToPlates.ContainsKey("red")) colorToPlates["red"] = new List<string>();
                        colorToPlates["red"].Add(plate.name);
                        break;
                    case 2:
                        plate.GetComponent<MeshRenderer>().material.color = Color.yellow;
                        platesTexts[index].color = Color.black;

                        plate.transform.name += ".yellow";
                        availableColors.Add("yellow");
                        ManageMUColor(randomValue, "yellow");

                        if (!colorToPlates.ContainsKey("yellow")) colorToPlates["yellow"] = new List<string>();
                        colorToPlates["yellow"].Add(plate.name);
                        break;
                    case 3:
                        plate.GetComponent<MeshRenderer>().material.color = Color.blue;
                        platesTexts[index].color = Color.white;

                        plate.transform.name += ".blue";
                        availableColors.Add("blue");
                        ManageMUColor(randomValue, "blue");

                        if (!colorToPlates.ContainsKey("blue")) colorToPlates["blue"] = new List<string>();
                        colorToPlates["blue"].Add(plate.name);
                        break;
                    case 4:
                        plate.GetComponent<MeshRenderer>().material.color = Color.green;
                        platesTexts[index].color = Color.black;

                        plate.transform.name += ".green";
                        availableColors.Add("green");
                        ManageMUColor(randomValue, "green");

                        if (!colorToPlates.ContainsKey("green")) colorToPlates["green"] = new List<string>();
                        colorToPlates["green"].Add(plate.name);
                        break;
                    case 5:
                        plate.GetComponent<MeshRenderer>().material.color = Color.black;
                        platesTexts[index].color = Color.white;

                        plate.transform.name += ".black";
                        availableColors.Add("black");
                        ManageMUColor(randomValue, "black");

                        if (!colorToPlates.ContainsKey("black")) colorToPlates["black"] = new List<string>();
                        colorToPlates["black"].Add(plate.name);
                        break;
                    case 6:
                        plate.GetComponent<MeshRenderer>().material.color = Color.cyan;
                        platesTexts[index].color = Color.black;

                        plate.transform.name += ".sky-blue";
                        availableColors.Add("sky-blue");
                        ManageMUColor(randomValue, "sky-blue");

                        if (!colorToPlates.ContainsKey("sky-blue")) colorToPlates["sky-blue"] = new List<string>();
                        colorToPlates["sky-blue"].Add(plate.name);
                        break;

                    case 7:
                        plate.GetComponent<MeshRenderer>().material.color = Color.grey;
                        platesTexts[index].color = Color.white;

                        plate.transform.name += ".grey";
                        availableColors.Add("grey");
                        ManageMUColor(randomValue, "grey");

                        if (!colorToPlates.ContainsKey("grey")) colorToPlates["grey"] = new List<string>();
                        colorToPlates["grey"].Add(plate.name);
                        break;
                    case 8:
                        plate.GetComponent<MeshRenderer>().material.color = brownColor;
                        platesTexts[index].color = Color.white;

                        plate.transform.name += ".brown";
                        availableColors.Add("brown");
                        ManageMUColor(randomValue, "brown");

                        if (!colorToPlates.ContainsKey("brown")) colorToPlates["brown"] = new List<string>();
                        colorToPlates["brown"].Add(plate.name);
                        break;
                    case 9:
                        plate.GetComponent<MeshRenderer>().material.color = pinkColor;
                        platesTexts[index].color = Color.black;

                        plate.transform.name += ".pink";
                        availableColors.Add("pink");
                        ManageMUColor(randomValue, "pink");

                        if (!colorToPlates.ContainsKey("pink")) colorToPlates["pink"] = new List<string>();
                        colorToPlates["pink"].Add(plate.name);
                        break;
                    case 10:
                        plate.GetComponent<MeshRenderer>().material.color = purpleColor;
                        platesTexts[index].color = Color.white;

                        plate.transform.name += ".purple";
                        availableColors.Add("purple");
                        ManageMUColor(randomValue, "purple");

                        if (!colorToPlates.ContainsKey("purple")) colorToPlates["purple"] = new List<string>();
                        colorToPlates["purple"].Add(plate.name);
                        break;
                }

                index++;
            }

            if(!isStartingPointSet) plates[0].GetComponent<MeshRenderer>().material.color = Color.black;

            for (int i = 0; i < plates.Count; i++)
            {
                if (plates[i].name.Contains("." + mostUsedColor)) // matching ".red", ".blue", etc.
                {
                    indexesWithMostUsedColor.Add(i);
                }
            }

            int additoionalCount = 1;
            for (int i = 0; i < plates.Count; i++)
            {
                if (!plates[i].name.Contains("." + mostUsedColor)) // matching ".red", ".blue", etc.
                {
                    if (!additionalPlateAdded && additoionalCount <= 2)
                    {
                        // print("Expression Plate Set!!");
                        indexesWithMostUsedColor.Add(i);
                    }
                    additoionalCount++;
                }
            }


        }

    }

    public List<int> GenerateRandomValues()
    {
        StaticVariableManager.numberPlate = plates.Count;
        List<int> returnList = new List<int>();


        // Choose a "most used" number between 1 and 5
        mostUsedNumber = Random.Range(1, 6); // 1 to 5 inclusive

        // Add the most used number 3 times
        returnList.Add(mostUsedNumber);
        returnList.Add(mostUsedNumber);
        returnList.Add(mostUsedNumber);

        // Fill the remaining slots with random numbers from 1 to 5
        for (int x = 0; x < StaticVariableManager.numberPlate - 3; x++)
        {
            int randomNum = Random.Range(1, 6);

            do
            {

                randomNum = Random.Range(1, 6); // 1 to 5 inclusive

            } while (randomNum == mostUsedNumber);

            returnList.Add(randomNum);
        }

        // Shuffle the list
        //returnList = returnList.OrderBy(x => Random.value).ToList();

        // print("Most used value: " + mostUsedNumber);

        foreach(int index in returnList)
        {

            // print($"Random value {index}.");
        }

        return returnList;
    }

    private void ManageMUColor(int randomValue, string color)
    {
        if(randomValue == mostUsedNumber)
        {
            mostUsedColor = color;
            StaticVariableManager.mostUsedColor = color;

            SetMUColor();

            //mostUsedColor = Color.
        }
    }

    private void SetMUColor()  // Most color
    {
        switch (mostUsedNumber)
        {
            case 1:
                most_used_color = Color.red;
                expectedNumHit = StaticVariableManager.expRedHitTimes;
                break;
            case 2:
                most_used_color = Color.yellow;
                expectedNumHit = StaticVariableManager.expYellowHitTimes;
                break;
            case 3:
                most_used_color = Color.blue;
                expectedNumHit = StaticVariableManager.expBlueHitTimes;
                break;
            case 4:
                most_used_color = Color.green;
                expectedNumHit = StaticVariableManager.expGreenHitTimes;
                break;
            case 5:
                most_used_color = Color.black;
                expectedNumHit = StaticVariableManager.expBlackHitTimes;
                break;
            case 6:
                most_used_color = Color.cyan;
                break;

            case 7:
                most_used_color = Color.grey;
                break;
            case 8:
                most_used_color = brownColor;
                break;
            case 9:
                most_used_color = pinkColor;
                break;
            case 10:
                most_used_color = purpleColor;
                break;
        }
    }

    private void ManageExpression()
    {
        int index = 0;
        int platePosition = 1;
        int randomNumber = 0;
        bool isExpressionAnsMisplaced = false;
        List<int> randomExpressionNumber = GenerateExpressionValues();
        expressionPlates.Clear();

        foreach (TextMesh text in platesTexts)
        {
            text.gameObject.SetActive(true);

            if (indexesWithMostUsedColor.Contains(index))
            {

                if (platePosition == 1)
                {
                    ShowExpression(text, randomExpressionNumber[0], randomExpressionNumber[1]);
                }
                else if (platePosition == 2)
                {
                    switch (expressionSign)
                    {
                        case 1:
                            expressionAnswer = Addition(text, randomExpressionNumber[0], randomExpressionNumber[1]);
                            break;
                        case 2:
                            expressionAnswer = Subtraction(text, randomExpressionNumber[0], randomExpressionNumber[1]);
                            break;
                        case 3:
                            expressionAnswer = Multiplication(text, randomExpressionNumber[0], randomExpressionNumber[1]);
                            break;
                    }
                }
                else
                {
                    do
                    {
                        randomNumber = Random.Range(1, 10);

                    } while (randomNumber == expressionAnswer);

                    text.text = randomNumber.ToString();
                }

                platePosition++;
                expressionPlates.Add(plates[index]);
            }
            else
            {
                if(!isExpressionAnsMisplaced && platePosition >= 2)
                {
                    text.text = expressionAnswer.ToString();

                    isExpressionAnsMisplaced = true;
                }
                else
                {
                    randomNumber = Random.Range(1, 10);
                    text.text = randomNumber.ToString();
                }
            }

            index++;
        }
    }

    private List<int> GenerateExpressionValues()
    {
        List<int> randomExpressionNumber = Enumerable.Range(1, 10)
                                   .OrderBy(x => Random.value)
                                   .Take(2)
                                   .ToList();

        int tempNum = randomExpressionNumber[0];

        if (randomExpressionNumber[0] < randomExpressionNumber[1])
        {
            randomExpressionNumber[0] = randomExpressionNumber[1];
            randomExpressionNumber[1] = tempNum;
        }

        return randomExpressionNumber;
    }

    private void ShowExpression(TextMesh text, int num1, int num2)
    {
        expressionSign = Random.Range(1, 4);

        switch (expressionSign)
        {
            case 1:
                text.text = ($"{num1.ToString()} + {num2.ToString()}");
                break;
            case 2:
                text.text = ($"{num1.ToString()} - {num2.ToString()}");
                break;
            case 3:
                if (num1 <= 5 || num2 <= 5)
                {
                    text.text = ($"{num1.ToString()} x {num2.ToString()}");
                }
                else
                {
                    expressionSign = 1;
                    text.text = ($"{num1.ToString()} + {num2.ToString()}");
                }

                break;
        }
    }

    private int Addition(TextMesh text, int num1, int num2)
    {
        text.text = (num1 + num2).ToString();
        return num1 + num2;
    }

    private int Subtraction(TextMesh text, int num1, int num2)
    {
        text.text = (num1 - num2).ToString();
        return num1 - num2;
    }

    private int Multiplication(TextMesh text, int num1, int num2)
    {
        text.text = (num1 * num2).ToString();
        return num1 * num2;
    }


    void resetStaticVariables()
    {
        StaticVariableManager.currentGoal = 0;
        StaticVariableManager.currentProgress = 0;
        StaticVariableManager.correctNumberHits = 0;
        StaticVariableManager.wrongNumberHits = 0;
        //StaticVariableManager.numberPlate = 8;
        StaticVariableManager.isColorDisplayed = false;
        StaticVariableManager.currentTargetColor = "";
        StaticVariableManager.sequenceCreated = false;
        StaticVariableManager.lane1CorrectTargetsHits = 0;
        StaticVariableManager.lane1IncorrectTargetsHits = 0;
        StaticVariableManager.colorDisplayTimer = 5;
    }

}
