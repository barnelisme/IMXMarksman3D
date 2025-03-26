using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class TargetScaleManager_V2 : MonoBehaviour
{
    private Vector3 modeScale; // Store the initial scale of the object
    private Vector3 initialScale;
    private float scaleSpeed = 0.05f;
    private int scaleLoopValue = 5;
    private int scaleDistance = 5;
    private int initialDistance = 5;
    private bool upScaleUpdated = false, downScaleUpdated = false;

    private string scaleFilePath = "Assets/Resources/AllScales.txt"; // Single file for all scales
    private Dictionary<string, string> scaleData = new Dictionary<string, string>();
    private string activeScene;

    public TextMeshProUGUI adminDistanceDisplay;
    public TextMeshProUGUI shooterDistanceDisplay;

    private void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        SetTextStartColors();
        LoadAllScales();

        if (scaleData.TryGetValue(activeScene, out string scaleInfo))
        {
            ParseScaleData(scaleInfo);
        }
        else
        {
            Debug.LogWarning($"No scale data found for the active scene: {activeScene}. Using default values.");
            modeScale = transform.localScale;
        }
    }

    private void Update()
    {
        if (!StaticVariableManager.isStopTraining)
        {
            // Check for the Up Arrow key (scale up)
            if ((Keyboard.current.upArrowKey.isPressed && Keyboard.current.sKey.isPressed) && !Keyboard.current.leftShiftKey.isPressed)
            {
                // print("Up Scaling");
                ScaleObjectDown();
                upScaleUpdated = false;
            }
            else
            {
                if (!upScaleUpdated)
                {
                    modeScale = transform.localScale;
                    upScaleUpdated = true;
                    SaveScaleData(); // Save updated scale when it's changed
                }
            }

            // Check for the Down Arrow key (scale down)
            if ((Keyboard.current.downArrowKey.isPressed && Keyboard.current.sKey.isPressed) && !Keyboard.current.leftShiftKey.isPressed)
            {
                // print("Down Scaling");
                ScaleObjectUp();
                downScaleUpdated = false;
            }
            else
            {
                if (!downScaleUpdated)
                {
                    modeScale = transform.localScale;
                    downScaleUpdated = true;
                    SaveScaleData(); // Save updated scale when it's changed
                }
            }
        }
    }

    public void ScaleObjectUp()
    {
        transform.localScale = modeScale * (1 + scaleSpeed);
        //initialScale = transform.localScale;
        //SaveScaleData();
    }

    public void ScaleObjectDown()
    {
        transform.localScale = modeScale * (1 - scaleSpeed);
        //initialScale = transform.localScale;
        //SaveScaleData();
    }

    public void IncreaseDistance()
    {

        if (scaleDistance < 25 && this.enabled)
        {
            int count = 1;
            while (count < scaleLoopValue)
            {
                //print("Scale...");
                ScaleObjectDown();
                modeScale = transform.localScale;
                count++;
            }

            //print("RE: " + initialScale);
            scaleDistance += scaleLoopValue;
            UpdateScaleSizeDisplay();

            //activeScene = SceneManager.GetActiveScene().name;
            if (activeScene.ToLower().Contains("targetpopupfreeshoot") || activeScene == "BasicTargetPopUpOneHand" 
                || activeScene.ToLower().Contains("threatening") || activeScene.ToLower().Contains("pointman") 
                || activeScene.ToLower().Contains("pointbullseye"))
            {
                SaveScaleData();
            }
        }
    }
    public void DecreaseDistance()
    {
        if (scaleDistance > 5 && this.enabled)
        {
            int count = 1;
            while (count < scaleLoopValue)
            {
                ScaleObjectUp();
                modeScale = transform.localScale;
                count++;
            }

            //print("RE: " + initialScale);
            scaleDistance -= scaleLoopValue;
            UpdateScaleSizeDisplay();

            //activeScene = SceneManager.GetActiveScene().name;
            if (activeScene.ToLower().Contains("targetpopupfreeshoot") || activeScene == "BasicTargetPopUpOneHand" 
                || activeScene.ToLower().Contains("threatening") || activeScene.ToLower().Contains("pointman") 
                || activeScene.ToLower().Contains("pointbullseye"))
            {
                SaveScaleData();
            }
        }
    }

    public void LoadDefault()
    {
        transform.localScale = initialScale;
        modeScale = initialScale;
        if (activeScene.ToLower().Contains("targetpopupfreeshoot") || activeScene == "BasicTargetPopUpOneHand" 
            || activeScene.ToLower().Contains("threatening") || activeScene.ToLower().Contains("pointman") 
            || activeScene.ToLower().Contains("pointbullseye"))
        {
            scaleDistance = initialDistance;
        }
        else
        {
            scaleDistance = 5;
        }
        UpdateScaleSizeDisplay();
    }

    public void LoadRecurringScale()
    {
        LoadAllScales();
        activeScene = SceneManager.GetActiveScene().name;
        if (scaleData.TryGetValue(activeScene, out string scaleInfo))
        {
            ParseScaleData(scaleInfo);
        }
        else
        {
            Debug.LogWarning($"No scale data found for the active scene: {activeScene}. Using default values.");
            modeScale = transform.localScale;
        }
    }

    private void SaveScaleData()
    {
        string scaleInfo = $"{transform.localScale.x};{transform.localScale.y};{transform.localScale.z}:{scaleDistance}";
        if (scaleData.ContainsKey(activeScene))
        {
            scaleData[activeScene] = scaleInfo;
        }
        else
        {
            scaleData.Add(activeScene, scaleInfo);
        }

        //print("Saving : " + scaleInfo);

        SaveAllScales();
    }

    private void LoadAllScales()
    {
        if (!File.Exists(scaleFilePath))
        {
            Debug.LogWarning($"Scale file not found at {scaleFilePath}. Creating a new file.");
            File.Create(scaleFilePath).Dispose();
            return;
        }

        string[] lines = File.ReadAllLines(scaleFilePath);
        scaleData.Clear();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || !line.Contains("=")) continue;

            string[] parts = line.Split('=');
            string sceneName = parts[0].Trim();
            string scaleInfo = parts[1].Trim();

            if (!scaleData.ContainsKey(sceneName))
            {
                scaleData.Add(sceneName, scaleInfo);
            }
        }
    }

    private void SaveAllScales()
    {
        List<string> lines = new List<string>();

        foreach (var entry in scaleData)
        {
            lines.Add($"{entry.Key}={entry.Value}");
            //print("Saving: " + entry.Key + ":" + entry.Value);
        }
        
        File.WriteAllLines(scaleFilePath, lines.ToArray());
    }

    public void ParseScaleData(string scaleInfo)
    {
        string[] parts = scaleInfo.Split(':');
        string[] scaleValues = parts[0].Split(';');

        if (scaleValues.Length == 3 &&
            float.TryParse(scaleValues[0], out float x) &&
            float.TryParse(scaleValues[1], out float y) &&
            float.TryParse(scaleValues[2], out float z))
        {
            modeScale = new Vector3(x, y, z);
            initialScale = modeScale;
            transform.localScale = modeScale;

            if (parts.Length > 1 && int.TryParse(parts[1], out int distance))
            {
                scaleDistance = distance;
                initialDistance = scaleDistance;
            }

            UpdateScaleSizeDisplay();
        }
        else
        {
            Debug.LogError($"Invalid scale data for scene {activeScene}: {scaleInfo}");
        }
    }

    private void SetTextStartColors()
    {
        if(shooterDistanceDisplay != null)
        {
            if (Scoring.ammo_setting.ToLower().Contains("live") || Scoring.ammo_setting.ToLower().Contains("infrared"))
            {

                shooterDistanceDisplay.color = Color.white;
            }
            else
            {

                shooterDistanceDisplay.color = Color.black;
            }
        }
    }

    private void UpdateScaleSizeDisplay()
    {
        if (adminDistanceDisplay != null)
            adminDistanceDisplay.text = $"{scaleDistance} Meters";
        if (shooterDistanceDisplay != null)
            shooterDistanceDisplay.text = $"{scaleDistance} Meters";
    }
}
