using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PointManager : MonoBehaviour
{

    public List<GameObject> targets = new List<GameObject>();
    private bool target_enabled = false;
    private int randomIndex = 0;
    private bool randomTargetSet = false;
    private string activeScene = "";


    // Start is called before the first frame update
    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        StaticVariableManager.resetTargets = true;
        updateColor();
        ResetRandomTarget();
        StaticVariableManager.resetTargets = false;
        StaticVariableManager.target1Active = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (StaticVariableManager.resetTargets)
        {
            updateColor();
            ResetRandomTarget();
            StaticVariableManager.resetTargets = false;
        }
        else if (StaticVariableManager.resetTargets == false)
        {

            switch (this.transform.name)
            {
                case "TG1":
                    if (StaticVariableManager.target1Active)
                    {
                        updateRandomTarget();
                        //print("point reached");
                        StaticVariableManager.target1Active = false;
                    }
                    break;

                case "TG2":
                    if (StaticVariableManager.target2Active)
                    {
                        //print("Test: point reached...");
                        updateRandomTarget();

                        StaticVariableManager.target2Active = false;
                    }
                    break;

                case "TG3":
                    if (StaticVariableManager.target3Active)
                    {
                        updateRandomTarget();

                        StaticVariableManager.target3Active = false;
                    }
                    break;
            }
        }

    }

    private void updateRandomTarget()
    {
        do
        {
            randomIndex = Random.Range(0, 5);
            if(randomIndex == 5)
            {
                randomIndex = 4;
            }

        } while (StaticVariableManager.usedRandomPoints.Contains(randomIndex.ToString()));

        if (StaticVariableManager.usedRandomPoints.Length <= 1)
        {
            StaticVariableManager.usedRandomPoints += randomIndex;
        }
        else
        {
            //string tempRanPoints = StaticVariableManager.usedRandomPoints;
            StaticVariableManager.usedRandomPoints = StaticVariableManager.usedRandomPoints.Substring(1);

            //StaticVariableManager.usedRandomPoints = tempRanPoints;
            StaticVariableManager.usedRandomPoints += randomIndex;
        }

        //print(StaticVariableManager.usedRandomPoints);

        updateColor();
        if(targets[randomIndex].transform.name.ToLower().Contains("body"))
        {
            targets[randomIndex].transform.name = "1.Body_Cover.Target";
        }
        else if (targets[randomIndex].transform.name.ToLower().Contains("head"))
        {
            targets[randomIndex].transform.name = "1.Head_Cover.Target";
        }
    }

    private void updateColor()
    {
        Material currentMaterial;
        if (StaticVariableManager.resetTargets)
        {
            foreach (GameObject target in targets)
            {
                target.transform.GetComponent<Renderer>().material.color = Color.green;
                currentMaterial = target.transform.GetComponent<Renderer>().material; // Assign Material
                applyColorAdjustemts(currentMaterial, "green");
                if (target.transform.name.ToLower().Contains("body"))
                {
                    target.transform.name = "1.Body_Cover";
                }
                else if (target.transform.name.ToLower().Contains("head"))
                {
                    target.transform.name = "1.Head_Cover";
                }
            }
        }
        else
        {
            targets[randomIndex].GetComponent<Renderer>().material.color = Color.red;
            currentMaterial = targets[randomIndex].GetComponent<Renderer>().material; // Assign Material
            applyColorAdjustemts(currentMaterial, "red");
        }
    }

    private void ResetRandomTarget()
    {
        if(activeScene.ToLower().Contains("static"))
        {
            switch (this.transform.name)
            {
                case "TG1":
                    StaticVariableManager.target1Active = true;
                    break;

                case "TG2":
                    StaticVariableManager.target2Active = true;
                    break;

                case "TG3":
                    StaticVariableManager.target3Active = true;
                    break;
            }
        }
    }

    private void applyColorAdjustemts(Material material, string colorName)
    {
        Material updateColor = material;
        float current_r;
        float current_g;
        float current_b;

        current_r = (updateColor.color.r * 255);
        current_g = (updateColor.color.g * 255);
        current_b = (updateColor.color.b * 255);

        if(Scoring.ammo_setting.ToLower().Contains("laser") && !Scoring.ammo_setting.ToLower().Contains("infrared"))
        {
            if (colorName == "green")
            {
                current_g = 125;
            }
            else if (colorName == "red")
            {
                current_r = 150;
            }
        }

        updateColor.color = new Color(current_r / 255, current_g / 255, current_b / 255, 255 / 255);
    }
}
