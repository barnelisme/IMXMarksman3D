using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RatingsManager : MonoBehaviour
{
    public TextMeshProUGUI trainingNameTxt;
    public TextMeshProUGUI trainingTypeTxt;
    public Image rating;
    string sceneName = "";

    Color brownColor = new Color(150f / 255f, 75f / 255f, 0f);

    void Start()
    {
        rating.enabled = false;
        trainingTypeTxt.enabled = false;
        sceneName = trainingNameTxt.text;
        assignRating(sceneName);
    }

    private void assignRating(string name)
    {
        //print("Test: " + name);

        //Static
        if (name.Contains("Bullseye Challenge"))
        {
            rating.GetComponent<Image>().color = Color.red;
            trainingTypeTxt.text = "MS";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Shifting IPEC Plates"))
        {
            rating.GetComponent<Image>().color = Color.blue;
            trainingTypeTxt.text = "MS";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Bullseye Targets.2"))
        {
            rating.GetComponent<Image>().color = Color.red;
            trainingTypeTxt.text = "MS";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("S.Threat Man (2 Points)"))
        {
            rating.GetComponent<Image>().color = Color.blue;
            trainingTypeTxt.text = "MS";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("S.Threat Man (8 point)"))
        {
            rating.GetComponent<Image>().color = Color.blue;
            trainingTypeTxt.text = "MS";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("S.Threat Hostage IPEC"))
        {
            rating.GetComponent<Image>().color = brownColor;
            trainingTypeTxt.text = "MS/T";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("S.Man IPEC (4 Points)"))
        {
            rating.GetComponent<Image>().color = Color.blue;
            trainingTypeTxt.text = "MS";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("S.Man IPEC (10 Points)"))
        {
            rating.GetComponent<Image>().color = Color.blue;
            trainingTypeTxt.text = "MS";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("S.Bullseye (5 Points)"))
        {
            rating.GetComponent<Image>().color = brownColor;
            trainingTypeTxt.text = "MS/T";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Seqnum Addition"))
        {
            rating.GetComponent<Image>().color = brownColor;
            trainingTypeTxt.text = "C/D/T";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Seqnum"))
        {
            rating.GetComponent<Image>().color = brownColor;
            trainingTypeTxt.text = "D/T/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("ROYGBIV"))
        {
            rating.GetComponent<Image>().color = brownColor;
            trainingTypeTxt.text = "D/T/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("range"))
        {
            rating.GetComponent<Image>().color = Color.blue;
            trainingTypeTxt.text = "T/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }

        //Static Resp.Contains
        else if (name.Contains("Hidden Shape"))
        {
            rating.GetComponent<Image>().color = brownColor;
            trainingTypeTxt.text = "MS/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Falling Plates"))
        {
            rating.GetComponent<Image>().color = Color.green;
            trainingTypeTxt.text = "MS/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Block Target"))
        {
            rating.GetComponent<Image>().color = Color.blue;
            trainingTypeTxt.text = "MS/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Pole Alignment"))
        {
            rating.GetComponent<Image>().color = Color.blue;
            trainingTypeTxt.text = "D/T/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Man Threat 2"))
        {
            rating.GetComponent<Image>().color = brownColor;
            trainingTypeTxt.text = "MS/A/C";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Man Threat 8"))
        {
            rating.GetComponent<Image>().color = brownColor;
            trainingTypeTxt.text = "MS/A//C";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Hostage Situation"))
        {
            rating.GetComponent<Image>().color = Color.red;
            trainingTypeTxt.text = "MS/A/D";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("IPEC Man 4"))
        {
            rating.GetComponent<Image>().color = Color.blue;
            trainingTypeTxt.text = "MS/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("IPEC Man 10"))
        {
            rating.GetComponent<Image>().color = Color.blue;
            trainingTypeTxt.text = "MS/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Diverse Bulls"))
        {
            rating.GetComponent<Image>().color = brownColor;
            trainingTypeTxt.text = "MS/A/T";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Rising Target"))
        {
            rating.GetComponent<Image>().color = Color.red;
            trainingTypeTxt.text = "A/T";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Dueling Tree"))
        {
            rating.GetComponent<Image>().color = Color.black;
            trainingTypeTxt.text = "MS/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Suspect Shoot"))
        {
            rating.GetComponent<Image>().color = Color.red;
            trainingTypeTxt.text = "D/T/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Basic IPEC Board"))
        {
            rating.GetComponent<Image>().color = Color.blue;
            trainingTypeTxt.text = "MS/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Dice Shoot"))
        {
            rating.GetComponent<Image>().color = Color.blue;
            trainingTypeTxt.text = "MS/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Shell Game"))
        {
            rating.GetComponent<Image>().color = Color.red;
            trainingTypeTxt.text = "D/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Racetrack Target"))
        {
            rating.GetComponent<Image>().color = brownColor;
            trainingTypeTxt.text = "C/D/T/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Clay Pigeon"))
        {
            rating.GetComponent<Image>().color = Color.red;
            trainingTypeTxt.text = "D/T/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Rising Shape"))
        {
            rating.GetComponent<Image>().color = brownColor;
            trainingTypeTxt.text = "T/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }

        //Moving
        else if (name.ToLower().Contains("balloon"))
        {
            rating.GetComponent<Image>().color = brownColor;
            rating.enabled = true;
            trainingTypeTxt.text = "C/D/T";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }

        //3D Scenarios
        else if(name.Contains("OpenPlain") || name.Contains("Mall") || name.Contains("restaurant"))
        {
            rating.GetComponent<Image>().color = Color.blue;
            trainingTypeTxt.text = "D/T/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("containers") || name.Contains("parking"))
        {
            rating.GetComponent<Image>().color = brownColor;
            trainingTypeTxt.text = "D/T/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("FOREST") || name.Contains("Hunting"))
        {
            rating.GetComponent<Image>().color = Color.red;
            trainingTypeTxt.text = "D/T/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }
        else if (name.Contains("Animal Target"))
        {
            rating.GetComponent<Image>().color = Color.blue;
            trainingTypeTxt.text = "T/A";

            rating.enabled = true;
            trainingTypeTxt.enabled = true;
        }


        Color currentColor = rating.color;
        float newAlpha = 255f/ 255f;
        Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
        rating.color = newColor;
    }
}
