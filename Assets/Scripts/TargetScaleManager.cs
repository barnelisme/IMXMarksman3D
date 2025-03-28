using UnityEngine;
using UnityEngine.InputSystem;
using System.IO; // For file operations
using TMPro;
using UnityEngine.SceneManagement;

public class TargetScaleManager : MonoBehaviour
{
    // Store the initial scale of the object
    private Vector3 initialScale;
    // Scaling factor
    private float scaleSpeed = 0.05f;
    private int scale_loop_value = 5;
    private int startingDistance = 5;
    string scaleString = "";
    string startScale = "";
    private bool upScaleUpdated = false, downScaleUpdated = false;

    private string filePath = "Assets/Resources/IPECScale.txt";

    public TextMeshProUGUI adminDistanceDisplay;
    public TextMeshProUGUI shooterDistanceDisplay;
    string activeScene = "";

    private bool activateScaleUp = false;
    private bool activateScaleDown = false;
    private bool activateScaleDefault = false;

    private void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
       
        print("Starting...");

        if(activeScene.ToLower().Contains("threatening") || activeScene.ToLower().Contains("pointman"))
        {
            filePath = "Assets/Resources/IPECScale.txt";
        }
        else if (activeScene.ToLower().Contains("pointbullseye"))
        {
            filePath = "Assets/Resources/DiverseBullseyeScale.txt";
        }
        else if (activeScene.ToLower().Contains("cyclic"))
        {
            filePath = "Assets/Resources/CyclicTargetScale.txt";
        }
        else if (activeScene.ToLower().Contains("risingplate"))
        {
            filePath = "Assets/Resources/RisingPlateScale.txt";
        }
        LoadInitialScale();
        
    }

    private void Update()
    {
        if(StaticVariableManager.isStopTraining == false)
        {
            // Check for the Up Arrow key (scale up)
            if (Keyboard.current.upArrowKey.isPressed && !Keyboard.current.leftShiftKey.isPressed)
            {
                // print("Up Scaling");
                ScaleObjectDown();
                upScaleUpdated = false;
            }
            else
            {
                if (!upScaleUpdated)
                {
                    initialScale = transform.localScale;
                    upScaleUpdated = true;
                    SaveInitialScale(); // Save updated scale when it's changed
                }
            }

            // Check for the Down Arrow key (scale down)
            if (Keyboard.current.downArrowKey.isPressed && !Keyboard.current.leftShiftKey.isPressed)
            {
                // print("Down Scaling");
                ScaleObjectUp();
                downScaleUpdated = false;
            }
            else
            {
                if (!downScaleUpdated)
                {
                    initialScale = transform.localScale;
                    downScaleUpdated = true;
                    SaveInitialScale(); // Save updated scale when it's changed
                }
            }

            // Increade Distance
            ManageUIScaleUp();

            // Decrease Distance
            ManageUIScaleDown();

            // Set Default
            ManageUIScaleDefault();
        }
    }

    private void ManageUIScaleUp()
    {
        activateScaleUp = StaticVariableManager.activateLane1ScaleUp;

        if (activateScaleUp == true)
        {
            DecreaseDistance();
            activateScaleUp = false;
        }

        StaticVariableManager.activateLane1ScaleUp = false;
    }

    private void ManageUIScaleDown()
    {
        activateScaleDown = StaticVariableManager.activateLane1ScaleDown;
        if (activateScaleDown == true)
        {
            IncreaseDistance();
            activateScaleDown = false;
        }

        StaticVariableManager.activateLane1ScaleDown = false;
    }

    private void ManageUIScaleDefault()
    {
        activateScaleDefault = StaticVariableManager.activateLane1ScaleDefault;
        if (activateScaleDefault == true)
        {
            LoadDefault();
            activateScaleDefault = false;
        }

        StaticVariableManager.activateLane1ScaleDefault = false;
    }

    private void ScaleObjectUp()
    {
        // Scale the object based on the initial scale and the scaleSpeed
        transform.localScale = initialScale * (1 + scaleSpeed);
    }

    private void ScaleObjectDown()
    {
        // Scale the object based on the initial scale and the scaleSpeed
        transform.localScale = initialScale * (1 - scaleSpeed);
    }

    // Save the initial scale to a text file
    private void SaveInitialScale()
    {
        scaleString = initialScale.x + ";" + initialScale.y + ";" + initialScale.z + ":" + startingDistance;
        print("Re: Saved: " + scaleString);

        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }
        else
        {
            //print(scaleString);
            File.WriteAllText(filePath, scaleString);
        }
    }

    // Load the initial scale from the text file
    public void LoadInitialScale()
    {
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }
        else
        {
            scaleString = File.ReadAllText(filePath);
            startScale = scaleString;
            string[] file_data = scaleString.Split(':');
            string[] scaleValues = file_data[0].Split(';');
            
            if (scaleValues.Length == 3)
            {
                float x = float.Parse(scaleValues[0]);
                float y = float.Parse(scaleValues[1]);
                float z = float.Parse(scaleValues[2]);
                initialScale = new Vector3(x, y, z);
                transform.localScale = initialScale; // Apply the scale on start
                scaleString = initialScale.x + ";" + initialScale.y + ";" + initialScale.z ;

                //CheckStartDistance();
                if (activeScene.ToLower().Contains("targetpopupfreeshoot") || activeScene == "BasicTargetPopUpOneHand" || activeScene.ToLower().Contains("threatening") || activeScene.ToLower().Contains("pointman") || activeScene.ToLower().Contains("pointbullseye"))
                {
                    startingDistance = int.Parse(file_data[1]);
                }
                else
                {
                    startingDistance = 5;
                }
                
                UpdateScaleSizeDisplay();
            }
            else
            {
                Debug.LogError("Invalid scale data in the file.");
            }
        }

    }

    public void LoadDefault()
    {
        scaleString = File.ReadAllText(filePath);
        startScale = scaleString;
        string[] file_data = startScale.Split(':');
        string[] scaleValues = file_data[0].Split(';');
        
        if (scaleValues.Length == 3)
        {
            float x = float.Parse(scaleValues[0]);
            float y = float.Parse(scaleValues[1]);
            float z = float.Parse(scaleValues[2]);
            initialScale = new Vector3(x, y, z);
            transform.localScale = initialScale; // Apply the scale on start
            scaleString = initialScale.x + ";" + initialScale.y + ";" + initialScale.z ;
            //CheckStartDistance();

            if (activeScene.ToLower().Contains("targetpopupfreeshoot") || activeScene == "BasicTargetPopUpOneHand" || activeScene.ToLower().Contains("threatening") || activeScene.ToLower().Contains("pointman") || activeScene.ToLower().Contains("pointbullseye"))
            {
                startingDistance = int.Parse(file_data[1]);
            }
            else
            {
                startingDistance = 5;
            }
            UpdateScaleSizeDisplay();
        }
        else
        {
            Debug.LogError("Invalid scale data in the file.");
        }
    }

    public void IncreaseDistance()
    {
        if(startingDistance < 25 && this.enabled)
        {
            int count = 1;
            while(count < scale_loop_value)
            {
                //print("Scale...");
                ScaleObjectDown();
                initialScale = transform.localScale;
                count++;
            }

            //print("RE: " + initialScale);
            startingDistance += scale_loop_value;
            UpdateScaleSizeDisplay();

            if (activeScene.ToLower().Contains("targetpopupfreeshoot") || activeScene == "BasicTargetPopUpOneHand" || activeScene.ToLower().Contains("threatening") || activeScene.ToLower().Contains("pointman") || activeScene.ToLower().Contains("pointbullseye"))
            {
                SaveInitialScale();
            }
        }

    }

    public void DecreaseDistance()
    {
        if(startingDistance > 5 && this.enabled)
        {
            int count = 1;
            while(count < scale_loop_value)
            {
                ScaleObjectUp();
                initialScale = transform.localScale;
                count++;
            }

            //print("RE: " + initialScale);
            startingDistance -= scale_loop_value;
            UpdateScaleSizeDisplay();
            if (activeScene.ToLower().Contains("targetpopupfreeshoot") || activeScene == "BasicTargetPopUpOneHand" || activeScene.ToLower().Contains("threatening") || activeScene.ToLower().Contains("pointman") || activeScene.ToLower().Contains("pointbullseye"))
            {
                SaveInitialScale();
            }
        }
    }

    private void UpdateScaleSizeDisplay()
    {
        //print("Updating data...:" + startingDistance);
        if(adminDistanceDisplay != null)
            adminDistanceDisplay.text = startingDistance + " Meters";
        if(shooterDistanceDisplay != null)
            shooterDistanceDisplay.text = startingDistance + " Meters";
    }


}
