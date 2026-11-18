using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SequenceManager : MonoBehaviour
{
    [Header("Plates")]
    public List<GameObject> plates = new List<GameObject>();
    public List<TextMesh> platesTexts = new List<TextMesh>();
    private static List<GameObject> staticUserPlates = new List<GameObject>();
    private static List<TextMesh> staticUserPlatesTexts = new List<TextMesh>();
    private List<string> availableColors = new List<string>();
    /// <summary>
    private Dictionary<string, List<string>> colorToPlates = new Dictionary<string, List<string>>();
    List<int> indexesWithMostUsedColor = new List<int>();
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
    private bool isTrainingStarted = false;

    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        int index = 0;
        int randomVal = 0;
        switchOffTimer = setSwitchOffTimer;
        targetRevealTimer = setTargetRevealTimer;
        staticUserPlates = plates;
        staticUserPlatesTexts = platesTexts;

        resetStaticVariables();
        setupNumTargets();

        if (activeScene.ToLower().Contains("num"))
        {
            
            if(activeScene.ToLower().Contains("add"))
            {
                progressUpdated = true;
                foreach (GameObject plate in plates)
                {
                    do
                    {
                        randomVal = Random.Range(1, 20);
                    } while (randomValues.Contains(randomVal));

                    randomValues.Add(randomVal);
                    platesTexts[index].text = (randomVal.ToString());
                    platesTexts[index].transform.gameObject.SetActive(false);
                    plate.transform.name += "." + randomVal.ToString();
                    index++;
                }

                randomVal = Random.Range(20, 100);
                boardTexts[0].text = randomVal.ToString();
                boardTexts[1].text = "0";
                StaticVariableManager.currentGoal = randomVal;
                StaticVariableManager.currLane1TargetNum = randomVal.ToString();
            }
            else
            {
                progressUpdated = false;
                targetRevealed = true;
                foreach (GameObject plate in plates)
                {
                    do
                    {
                        randomVal = Random.Range(1, 20);
                    } while (randomValues.Contains(randomVal));

                    randomValues.Add(randomVal);
                    platesTexts[index].text = (randomVal.ToString());
                    platesTexts[index].transform.gameObject.SetActive(false);
                    plate.transform.name += "." + randomVal.ToString();
                    index++;
                }

                boardTexts[0].text = " ";
                boardTexts[1].transform.gameObject.SetActive(false);
                StaticVariableManager.currentGoal = randomVal;
                StaticVariableManager.currLane1TargetNum = randomVal.ToString();
            }
        }
        if(activeScene.ToLower().Contains("colorsequence"))
        {
            targetRevealed = true;
            progressUpdated = false;
            defaulColor = plates[0].GetComponent<MeshRenderer>().material.color;
            boards[0].GetComponent<MeshRenderer>().material.color = defaulColor;
            StaticVariableManager.currLane1IndicatorColor = defaulColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (countDownStart.start_training)
        {
            if (activeScene.ToLower().Contains("num"))
            {
                if (activeScene.ToLower().Contains("add"))
                {
                    if (switchOffTimer <= 0f && !textSwitchedOff)
                    {
                        foreach (TextMesh text in platesTexts)
                        {
                            text.transform.gameObject.SetActive(false);
                        }
                        textSwitchedOff = true;
                    }
                    else if (switchOffTimer > 0f)
                    {
                        switchOffTimer -= Time.deltaTime;
                        if (textSwitchedOn == false)
                        {
                            foreach (TextMesh text in platesTexts)
                            {
                                text.transform.gameObject.SetActive(true);
                            }
                            textSwitchedOn = true;
                        }
                    }

                    if (progressUpdated == false)
                    {
                        currentProgressValue = int.Parse(boardTexts[1].text);
                        boardTexts[1].text = (currentProgressValue + incomingProgressValue).ToString();
                        StaticVariableManager.currentProgress = currentProgressValue + incomingProgressValue;
                        progressUpdated = true;
                    }


                    if (StaticVariableManager.currentProgress >= StaticVariableManager.currentGoal)
                    {
                        StaticVariableManager.isStopTraining = true;
                    }
                }
                else
                {
                    if (targetRevealed)
                    {
                        if (switchOffTimer <= 0f && !textSwitchedOff)
                        {
                            foreach (TextMesh text in platesTexts)
                            {
                                text.transform.gameObject.SetActive(false);
                            }
                            textSwitchedOff = true;
                        }
                        else if (switchOffTimer > 0f)
                        {
                            switchOffTimer -= Time.deltaTime;
                            if (textSwitchedOn == false)
                            {
                                foreach (TextMesh text in platesTexts)
                                {
                                    text.transform.gameObject.SetActive(true);
                                }
                                textSwitchedOn = true;
                            }
                        }


                        if (progressUpdated == false && textSwitchedOff)
                        {
                            //print("Point Reached...");
                            int randomIndex = 0;

                            do
                            {
                                randomIndex = Random.Range(0, randomValues.Count);

                            } while (previousIndexes.Contains(randomIndex));

                            if (previousIndexes.Count < 2)
                            {
                                previousIndexes.Add(randomIndex);
                            }
                            else
                            {
                                previousIndexes.RemoveAt(0);
                                previousIndexes.Add(randomIndex);
                            }

                            currentProgressValue = randomValues[randomIndex];
                            boardTexts[0].text = currentProgressValue.ToString();
                            boardTexts[0].transform.gameObject.SetActive(true);
                            StaticVariableManager.currentProgress = currentProgressValue;
                            StaticVariableManager.currLane1TargetNum = currentProgressValue.ToString();

                            targetRevealed = false;
                            targetRevealTimer = setTargetRevealTimer;
                            progressUpdated = true;
                        }
                    }
                    else
                    {
                        //print("Test: point reached...");
                        if (progressUpdated == false)
                        {
                            if (targetRevealTimer <= 0f)
                            {
                                targetTextHit.transform.gameObject.SetActive(false);
                                revealInProgress = false;
                                targetRevealed = true;
                            }
                            else
                            {
                                targetRevealTimer -= Time.deltaTime;
                                if (revealInProgress == false)
                                {
                                    foreach (TextMesh text in platesTexts)
                                    {
                                        if (text.text == incomingProgressValue.ToString())
                                        {
                                            targetTextHit = text;
                                            text.transform.gameObject.SetActive(true);
                                        }
                                    }
                                    if (incomingProgressValue == currentProgressValue)
                                    {
                                        StaticVariableManager.correctNumberHits++;
                                    }
                                    else
                                    {
                                        StaticVariableManager.wrongNumberHits++;
                                    }
                                    revealInProgress = true;
                                }
                            }
                        }
                    }
                }
            }
            else if (activeScene.ToLower().Contains("colorsequence"))
            {
                if (targetRevealed)
                {
                    if (StaticVariableManager.isColorDisplayed == false)
                    {
                        int index = 0;
                        int randomVal = 0;
                        if (StaticVariableManager.sequenceCreated == false)
                            randomValues.Clear();

                        foreach (GameObject plate in plates)
                        {
                            if (StaticVariableManager.sequenceCreated == false)
                            {
                                do
                                {
                                    randomVal = Random.Range(1, 10);
                                } while (randomValues.Contains(randomVal));

                                randomValues.Add(randomVal);
                            }
                            else
                            {
                                randomVal = randomValues[index];
                                index++;
                            }

                            switch (randomVal)
                            {
                                case 1:
                                    plate.GetComponent<MeshRenderer>().material.color = Color.red;
                                    plate.transform.name += ".red";
                                    availableColors.Add("red");
                                    break;
                                case 2:
                                    plate.GetComponent<MeshRenderer>().material.color = Color.yellow;
                                    plate.transform.name += ".yellow";
                                    availableColors.Add("yellow");
                                    break;
                                case 3:
                                    plate.GetComponent<MeshRenderer>().material.color = Color.blue;
                                    plate.transform.name += ".blue";
                                    availableColors.Add("blue");
                                    break;
                                case 4:
                                    plate.GetComponent<MeshRenderer>().material.color = Color.green;
                                    plate.transform.name += ".green";
                                    availableColors.Add("green");
                                    break;
                                case 5:
                                    plate.GetComponent<MeshRenderer>().material.color = Color.cyan;
                                    plate.transform.name += ".sky-blue";
                                    availableColors.Add("sky-blue");
                                    break;
                                case 6:
                                    plate.GetComponent<MeshRenderer>().material.color = Color.black;
                                    plate.transform.name += ".black";
                                    availableColors.Add("black");
                                    break;
                                case 7:
                                    plate.GetComponent<MeshRenderer>().material.color = Color.grey;
                                    plate.transform.name += ".grey";
                                    availableColors.Add("grey");
                                    break;
                                case 8:
                                    plate.GetComponent<MeshRenderer>().material.color = brownColor;
                                    plate.transform.name += ".brown";
                                    availableColors.Add("brown");
                                    break;
                                case 9:
                                    plate.GetComponent<MeshRenderer>().material.color = pinkColor;
                                    plate.transform.name += ".pink";
                                    availableColors.Add("pink");
                                    break;
                                case 10:
                                    plate.GetComponent<MeshRenderer>().material.color = purpleColor;
                                    plate.transform.name += ".purple";
                                    availableColors.Add("purple");
                                    break;
                            }
                        }

                        StaticVariableManager.sequenceCreated = true;
                        boards[0].GetComponent<MeshRenderer>().material.color = defaulColor;
                        StaticVariableManager.currLane1IndicatorColor = defaulColor;
                        boards[0].SetActive(false);
                        colorIndicatorOn = false;
                        colorIndicatorTimer = setColorIndicatorTimer;

                        platesColorsSwitchedOff = false;
                        switchOffTimer = StaticVariableManager.colorDisplayTimer;//setSwitchOffTimer;
                        StaticVariableManager.isColorDisplayed = true;
                    }
                    else
                    {
                        if (switchOffTimer <= 0 && platesColorsSwitchedOff == false)
                        {
                            foreach (GameObject plate in plates)
                            {
                                plate.GetComponent<Renderer>().material.color = defaulColor;
                            }
                            platesColorsSwitchedOff = true;
                        }
                        else if (switchOffTimer > 0)
                        {
                            switchOffTimer -= Time.deltaTime;
                        }

                        if (platesColorsSwitchedOff)
                        {
                            if (colorIndicatorTimer <= 0 && colorIndicatorOn == false)
                            {
                                int randomIndex = 0;

                                do
                                {
                                    randomIndex = Random.Range(0, availableColors.Count);

                                } while (previousIndexes.Contains(randomIndex));

                                if (previousIndexes.Count < 2)
                                {
                                    previousIndexes.Add(randomIndex);
                                }
                                else
                                {
                                    previousIndexes.RemoveAt(0);
                                    previousIndexes.Add(randomIndex);
                                }

                                string selectedColor = availableColors[randomIndex];
                                StaticVariableManager.currentTargetColor = selectedColor;

                                switch (selectedColor)
                                {
                                    case "red":
                                        boards[0].GetComponent<Renderer>().material.color = Color.red;
                                        break;
                                    case "yellow":
                                        boards[0].GetComponent<Renderer>().material.color = Color.yellow;
                                        break;
                                    case "blue":
                                        boards[0].GetComponent<Renderer>().material.color = Color.blue;
                                        break;
                                    case "green":
                                        boards[0].GetComponent<Renderer>().material.color = Color.green;
                                        break;
                                    case "sky-blue":
                                        boards[0].GetComponent<Renderer>().material.color = Color.cyan;
                                        break;
                                    case "black":
                                        boards[0].GetComponent<Renderer>().material.color = Color.black;
                                        break;
                                    case "grey":
                                        boards[0].GetComponent<Renderer>().material.color = Color.grey;
                                        break;
                                    case "brown":
                                        boards[0].GetComponent<Renderer>().material.color = brownColor;
                                        break;
                                    case "pink":
                                        boards[0].GetComponent<Renderer>().material.color = pinkColor;
                                        break;
                                    case "purple":
                                        boards[0].GetComponent<Renderer>().material.color = purpleColor;
                                        break;
                                }
                                boards[0].SetActive(true);
                                StaticVariableManager.currLane1IndicatorColor = boards[0].GetComponent<Renderer>().material.color;

                                targetRevealTimer = setTargetRevealTimer;
                                colorIndicatorTimer = setColorIndicatorTimer;
                                colorIndicatorOn = true;
                                progressUpdated = true;
                                targetRevealed = false;
                            }
                            else if (colorIndicatorTimer > 0)
                            {
                                colorIndicatorTimer -= Time.deltaTime;
                            }
                        }
                    }
                }
                else
                {
                    if (progressUpdated == false)
                    {
                        if (targetRevealTimer <= 0f)
                        {
                            targetHit.GetComponent<MeshRenderer>().material.color = defaulColor;

                            colorIndicatorOn = false;
                            colorIndicatorTimer = setColorIndicatorTimer;
                            revealInProgress = false;
                            targetRevealed = true;
                        }
                        else
                        {
                            targetRevealTimer -= Time.deltaTime;
                            if (revealInProgress == false)
                            {
                                int randomIndex = 0;
                                foreach (GameObject plate in plates)
                                {
                                    if (plate.transform.name == hitTargetName)
                                    {
                                        targetHit = plate;
                                        string selectedColor = availableColors[randomIndex];
                                        switch (selectedColor)
                                        {
                                            case "red":
                                                plate.GetComponent<Renderer>().material.color = Color.red;
                                                break;
                                            case "yellow":
                                                plate.GetComponent<Renderer>().material.color = Color.yellow;
                                                break;
                                            case "blue":
                                                plate.GetComponent<Renderer>().material.color = Color.blue;
                                                break;
                                            case "green":
                                                plate.GetComponent<Renderer>().material.color = Color.green;
                                                break;
                                            case "sky-blue":
                                                plate.GetComponent<Renderer>().material.color = Color.cyan;
                                                break;
                                            case "black":
                                                plate.GetComponent<Renderer>().material.color = Color.black;
                                                break;
                                            case "grey":
                                                plate.GetComponent<Renderer>().material.color = Color.grey;
                                                break;
                                            case "brown":
                                                plate.GetComponent<Renderer>().material.color = brownColor;
                                                break;
                                            case "pink":
                                                plate.GetComponent<Renderer>().material.color = pinkColor;
                                                break;
                                            case "purple":
                                                plate.GetComponent<Renderer>().material.color = purpleColor;
                                                break;
                                        }
                                    }
                                    randomIndex++;
                                }

                                //print("Test: point reached...");
                                if (incomingProgressValue == currentProgressValue)
                                {
                                    StaticVariableManager.correctNumberHits++;
                                }
                                else
                                {
                                    StaticVariableManager.wrongNumberHits++;
                                }
                                revealInProgress = true;
                            }
                        }
                    }
                }
            }

            isTrainingStarted = true;
        }
        else if (isTrainingStarted)
        {
            if (activeScene.ToLower().Contains("num"))
            {
                foreach (TextMesh text in platesTexts)
                {
                    text.transform.gameObject.SetActive(true);
                }
            }
            else if (activeScene.ToLower().Contains("colorsequence"))
            {
                int randomIndex = 0;
                foreach (GameObject plate in plates)
                {
                    string selectedColor = availableColors[randomIndex];
                    switch (selectedColor)
                    {
                        case "red":
                            plate.GetComponent<Renderer>().material.color = Color.red;
                            break;
                        case "yellow":
                            plate.GetComponent<Renderer>().material.color = Color.yellow;
                            break;
                        case "blue":
                            plate.GetComponent<Renderer>().material.color = Color.blue;
                            break;
                        case "green":
                            plate.GetComponent<Renderer>().material.color = Color.green;
                            break;
                        case "sky-blue":
                            plate.GetComponent<Renderer>().material.color = Color.cyan;
                            break;
                        case "black":
                            plate.GetComponent<Renderer>().material.color = Color.black;
                            break;
                        case "grey":
                            plate.GetComponent<Renderer>().material.color = Color.grey;
                            break;
                        case "brown":
                            plate.GetComponent<Renderer>().material.color = brownColor;
                            break;
                        case "pink":
                            plate.GetComponent<Renderer>().material.color = pinkColor;
                            break;
                        case "purple":
                            plate.GetComponent<Renderer>().material.color = purpleColor;
                            break;
                    }
                    randomIndex++;
                }
            }

            isTrainingStarted = false;
        }

    }

    private void ManageExpression()
    {
        int index = 0;
        int platePosition = 1;
        List<int> randomExpressionNumber = GenerateExpressionValues();

        foreach (TextMesh text in platesTexts)
        {
            text.gameObject.SetActive(true);

            if (indexesWithMostUsedColor.Contains(index))
            {

                if (platePosition == 1)
                {
                    ShowExpression(text, randomExpressionNumber[0], randomExpressionNumber[1]);
                }
                else
                {
                    switch(expressionSign)
                    {
                        case 1:
                            Addition(text, randomExpressionNumber[0], randomExpressionNumber[1]);
                            break;
                        case 2:
                            Subtraction(text, randomExpressionNumber[0], randomExpressionNumber[1]);
                            break;
                        case 3:
                            Multiplication(text, randomExpressionNumber[0], randomExpressionNumber[1]);
                            break;
                    }
                }

                platePosition++;
            }
            else
            {
                int randomNumber = Random.Range(1,10);

                text.text = randomNumber.ToString();
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

        switch(expressionSign)
        {
            case 1:
                text.text = ($"{num1.ToString()} + {num2.ToString()}");
                break;
            case 2:
                text.text = ($"{num1.ToString()} - {num2.ToString()}");
                break;
            case 3:
                if(num1 <= 5 || num2 <= 5)
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

    private void GenerateColorGroup()
    {
        int index = 0;
        int randomValue = 0;
        bool additionalPlateAdded = false;


        if (activeScene.ToLower().Contains("sequence"))
        {
            foreach (GameObject plate in plates)
            {
                if (StaticVariableManager.sequenceCreated == false)
                {
                    do
                    {
                        randomValue = Random.Range(1, 10);
                    } while (randomValues.Contains(randomValue));

                    randomValues.Add(randomValue);
                }
                else
                {
                    randomValue = randomValues[index];
                    index++;
                }

                switch (randomValue)
                {
                    case 1:
                        plate.GetComponent<MeshRenderer>().material.color = Color.red;
                        plate.transform.name += ".red";
                        availableColors.Add("red");
                        break;
                    case 2:
                        plate.GetComponent<MeshRenderer>().material.color = Color.yellow;
                        plate.transform.name += ".yellow";
                        availableColors.Add("yellow");
                        break;
                    case 3:
                        plate.GetComponent<MeshRenderer>().material.color = Color.blue;
                        plate.transform.name += ".blue";
                        availableColors.Add("blue");
                        break;
                    case 4:
                        plate.GetComponent<MeshRenderer>().material.color = Color.green;
                        plate.transform.name += ".green";
                        availableColors.Add("green");
                        break;
                    case 5:
                        plate.GetComponent<MeshRenderer>().material.color = Color.cyan;
                        plate.transform.name += ".sky-blue";
                        availableColors.Add("sky-blue");
                        break;
                    case 6:
                        plate.GetComponent<MeshRenderer>().material.color = Color.black;
                        plate.transform.name += ".black";
                        availableColors.Add("black");
                        break;
                    case 7:
                        plate.GetComponent<MeshRenderer>().material.color = Color.grey;
                        plate.transform.name += ".grey";
                        availableColors.Add("grey");
                        break;
                    case 8:
                        plate.GetComponent<MeshRenderer>().material.color = brownColor;
                        plate.transform.name += ".brown";
                        availableColors.Add("brown");
                        break;
                    case 9:
                        plate.GetComponent<MeshRenderer>().material.color = pinkColor;
                        plate.transform.name += ".pink";
                        availableColors.Add("pink");
                        break;
                    case 10:
                        plate.GetComponent<MeshRenderer>().material.color = purpleColor;
                        plate.transform.name += ".purple";
                        availableColors.Add("purple");
                        break;
                }
            }
        }
        else if(activeScene.ToLower().Contains("expression"))
        {

            /*if (!StaticVariableManager.sequenceCreated)
            {
                // Populate the list with numbers 1 to 10 and shuffle them
                randomValues = Enumerable.Range(1, 10).OrderBy(x => Random.value).ToList();
            }*/

            randomValues = GenerateRandomValues();

            foreach (GameObject plate in plates)
            {
                if (!StaticVariableManager.sequenceCreated)
                {
                    randomValue = randomValues[index];
                    index++;
                }
                else
                {
                    randomValue = randomValues[index];
                    index++;
                }

                switch (randomValue)
                {
                    case 1:
                        plate.GetComponent<MeshRenderer>().material.color = Color.red;
                        plate.transform.name += ".red";
                        availableColors.Add("red");

                        if (!colorToPlates.ContainsKey("red")) colorToPlates["red"] = new List<string>();
                        colorToPlates["red"].Add(plate.name);
                        break;
                    case 2:
                        plate.GetComponent<MeshRenderer>().material.color = Color.yellow;
                        plate.transform.name += ".yellow";
                        availableColors.Add("yellow");

                        if (!colorToPlates.ContainsKey("yellow")) colorToPlates["yellow"] = new List<string>();
                        colorToPlates["yellow"].Add(plate.name);
                        break;
                    case 3:
                        plate.GetComponent<MeshRenderer>().material.color = Color.blue;
                        plate.transform.name += ".blue";
                        availableColors.Add("blue");

                        if (!colorToPlates.ContainsKey("blue")) colorToPlates["blue"] = new List<string>();
                        colorToPlates["blue"].Add(plate.name);
                        break;
                    case 4:
                        plate.GetComponent<MeshRenderer>().material.color = Color.green;
                        plate.transform.name += ".green";
                        availableColors.Add("green");

                        if (!colorToPlates.ContainsKey("green")) colorToPlates["green"] = new List<string>();
                        colorToPlates["green"].Add(plate.name);
                        break;
                    case 5:
                        plate.GetComponent<MeshRenderer>().material.color = Color.cyan;
                        plate.transform.name += ".sky-blue";
                        availableColors.Add("sky-blue");

                        if (!colorToPlates.ContainsKey("sky-blue")) colorToPlates["sky-blue"] = new List<string>();
                        colorToPlates["sky-blue"].Add(plate.name);
                        break;
                    case 6:
                        plate.GetComponent<MeshRenderer>().material.color = Color.black;
                        plate.transform.name += ".black";
                        availableColors.Add("black");

                        if (!colorToPlates.ContainsKey("black")) colorToPlates["black"] = new List<string>();
                        colorToPlates["black"].Add(plate.name);
                        break;
                    case 7:
                        plate.GetComponent<MeshRenderer>().material.color = Color.grey;
                        plate.transform.name += ".grey";
                        availableColors.Add("grey");

                        if (!colorToPlates.ContainsKey("grey")) colorToPlates["grey"] = new List<string>();
                        colorToPlates["grey"].Add(plate.name);
                        break;
                    case 8:
                        plate.GetComponent<MeshRenderer>().material.color = brownColor;
                        plate.transform.name += ".brown";
                        availableColors.Add("brown");

                        if (!colorToPlates.ContainsKey("brown")) colorToPlates["brown"] = new List<string>();
                        colorToPlates["brown"].Add(plate.name);
                        break;
                    case 9:
                        plate.GetComponent<MeshRenderer>().material.color = pinkColor;
                        plate.transform.name += ".pink";
                        availableColors.Add("pink");

                        if (!colorToPlates.ContainsKey("pink")) colorToPlates["pink"] = new List<string>();
                        colorToPlates["pink"].Add(plate.name);
                        break;
                    case 10:
                        plate.GetComponent<MeshRenderer>().material.color = purpleColor;
                        plate.transform.name += ".purple";
                        availableColors.Add("purple");

                        if (!colorToPlates.ContainsKey("purple")) colorToPlates["purple"] = new List<string>();
                        colorToPlates["purple"].Add(plate.name);
                        break;
                }
            }


            // Determine most used color and plates that have it
            var mostUsedColorEntry = colorToPlates.OrderByDescending(kvp => kvp.Value.Count).First();
            mostUsedColor = mostUsedColorEntry.Key;
            List<string> platesWithColor = mostUsedColorEntry.Value;

            // Print or use the data
            Debug.Log($"Most used color: {mostUsedColor} ({platesWithColor.Count} times)");

            for (int i = 0; i < plates.Count; i++)
            {
                if (plates[i].name.Contains("." + mostUsedColor)) // matching ".red", ".blue", etc.
                {
                    indexesWithMostUsedColor.Add(i);
                }
            }

            int additoionalCount = 0;
            for (int i = 0; i < plates.Count; i++)
            {
                if (!plates[i].name.Contains("." + mostUsedColor)) // matching ".red", ".blue", etc.
                {
                    if(!additionalPlateAdded && additoionalCount == 2)
                    {
                        indexesWithMostUsedColor.Add(i);
                    }
                    additoionalCount++;
                }
            }


        }

    }

    public List<int> GenerateRandomValues()
    {
        StaticVariableManager.numberPlate = 8;
        List<int> returnList = Enumerable.Repeat(0, StaticVariableManager.numberPlate).ToList();
        List<int> storedNumbers = new List<int>();
        int randomNumber = Random.Range(1, 10); // Assuming Unity's Random

        if (!StaticVariableManager.sequenceCreated)
        {
            returnList[0] = randomNumber;
            returnList[4] = randomNumber;
            storedNumbers = returnList;

            for (int x = 0; x < returnList.Count; x++)
            {
                if (returnList[x] == 0)
                {
                    do
                    {
                        randomNumber = Random.Range(1, 10);
                    }
                    while (storedNumbers.Contains(randomNumber));

                    returnList[x] = randomNumber;
                    storedNumbers.Add(randomNumber);
                }
            }
        }

        return returnList;
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

    private void setupNumTargets()
    {
        int index;

        index = 0;
        int randomTargetIndex;
        List<int> usedTargetsIndex = new List<int>();
        List<GameObject> usedTargets = new List<GameObject>();
        List<TextMesh> usedTargetsText = new List<TextMesh>();

        while (index < StaticVariableManager.numberPlate)
        {
            do
            {
                randomTargetIndex = Random.Range(0, 15);
            } while (usedTargetsIndex.Contains(randomTargetIndex));

            plates[randomTargetIndex].SetActive(true);

            usedTargetsIndex.Add(randomTargetIndex);
            usedTargets.Add(plates[randomTargetIndex]);
            usedTargetsText.Add(platesTexts[randomTargetIndex]);

            index++;
        }


        plates = usedTargets;
        platesTexts = usedTargetsText;
    }

    public static void updateProgressValue(string value)
    {
        incomingProgressValue = 0;
        incomingProgressValue = int.Parse(value);
        //print(incomingProgressValue);
        progressUpdated = false;
    }

    public static void updateSequencePoints(GameObject target)
    {

        if(staticUserPlates.Contains(target))
        {
            int index = staticUserPlates.IndexOf(target);

            if (staticUserPlatesTexts[index].text == StaticVariableManager.currLane1TargetNum.ToString())
            {
                StaticVariableManager.lane1CorrectTargetsHits++;
                //print("Correct hits: " + StaticVariableManager.correctColorHits);
            }
            else
            {
                StaticVariableManager.lane1IncorrectTargetsHits++;
                //print("Wrong hits: " + StaticVariableManager.wrongColorHits);
            }

        }

    }

    public static void receiveTargetName(string name)
    {
        hitTargetName = name;
        progressUpdated = false;
    }
}


/*
 int colorCount = 10;
            int minRepeats = 2;
            int totalNeeded = 10;

            // Step 1: Create base list with repeated numbers
            List<int> baseList = Enumerable.Range(1, colorCount)
                                           .SelectMany(x => Enumerable.Repeat(x, minRepeats))
                                           .ToList();

            // Step 2: Shuffle the base list
            baseList = baseList.OrderBy(x => Random.value).ToList();

            // Step 3: Guarantee at least one repeated value
            // Pick a random number to force repeat
            int repeatedValue = baseList[Random.Range(0, baseList.Count)];

            // Start the final list with two of the repeated value
            returnList = new List<int> { repeatedValue, repeatedValue };

            // Step 4: Fill the rest of the list with other values from baseList
            foreach (int val in baseList)
            {
                if (returnList.Count >= totalNeeded) break;

                // Allow other values, but make sure we don’t add more of the same repeatedValue unless needed
                if (returnList.Count(v => v == val) < minRepeats || val != repeatedValue)
                {
                    returnList.Add(val);
                }
            }

            // Step 5: If still not enough, add random values (safe fallback)
            while (returnList.Count < totalNeeded)
            {
                int extra = Random.Range(1, colorCount + 1);
                returnList.Add(extra);
            }

            // Step 6: Shuffle final list
            returnList = returnList.OrderBy(x => Random.value).ToList();
 */