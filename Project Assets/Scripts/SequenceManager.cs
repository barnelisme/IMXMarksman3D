using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SequenceManager : MonoBehaviour
{
    [Header("Plates")]
    public List<GameObject> plates = new List<GameObject>();
    public List<TextMesh> platesTexts = new List<TextMesh>();
    private List<string> availableColors = new List<string>();

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

    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        int index = 0;
        int randomVal = 0;
        switchOffTimer = setSwitchOffTimer;
        targetRevealTimer = setTargetRevealTimer;

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
            }
        }
        if(activeScene.ToLower().Contains("color"))
        {
            targetRevealed = true;
            progressUpdated = false;
            defaulColor = plates[0].GetComponent<MeshRenderer>().material.color;
            boards[0].GetComponent<MeshRenderer>().material.color = defaulColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
       if(countDownStart.start_training)
       {
            if(activeScene.ToLower().Contains("num"))
            {
                if(activeScene.ToLower().Contains("add"))
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
                    if(targetRevealed)
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
                                randomIndex = Random.Range(0, randomValues.Count - 1);

                            } while (previousIndexes.Contains(randomIndex));

                            if(previousIndexes.Count < 2)
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
                            targetRevealed = false;
                            targetRevealTimer = setTargetRevealTimer;
                            progressUpdated = true;
                        }
                    }
                    else
                    {
                        //print("Test: point reached...");
                        if(progressUpdated == false)
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
                                    if(incomingProgressValue == currentProgressValue)
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
            else if(activeScene.ToLower().Contains("color"))
            {
                if(targetRevealed)
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
                                int randomIndex  = 0;

                                do
                                {
                                    randomIndex = Random.Range(0, availableColors.Count - 1);

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
       }
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
         StaticVariableManager.correctColorHits = 0;
         StaticVariableManager.wrongColorHits = 0;
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
    public static void receiveTargetName(string name)
    {
        hitTargetName = name;
        progressUpdated = false;
    }
}
