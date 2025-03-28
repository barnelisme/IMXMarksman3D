using UnityEngine;
using UnityEngine.InputSystem;
using System.IO; // For file operations
using TMPro;

public class DistanceSimulator : MonoBehaviour
{
    // Store the initial scale of the object
    private Vector3 initialScale;
    // Scaling factor
    public float scaleSpeed = 0.1f;
    private int scale_loop_value = 5;
    private int startingDistance = 5;
    string scaleString = "";
    string startScale = "";
    private bool upScaleUpdated = false, downScaleUpdated = false;

    private string filePath = "Assets/Resources/initialScale.txt";

    public TextMeshProUGUI adminDistanceDisplay;
    public TextMeshProUGUI shooterDistanceDisplay;


    private void Start()
    {

        LoadInitialScale();
        
    }

    private void Update()
    {
        // Check for the Up Arrow key (scale up)
        if (Keyboard.current.upArrowKey.isPressed)
        {
            // print("Up Scaling");
            ScaleObjectUp();
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
        if (Keyboard.current.downArrowKey.isPressed)
        {
            // print("Down Scaling");
            ScaleObjectDown();
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
        scaleString = initialScale.x + ";" + initialScale.y + ";" + initialScale.z + ":" + StaticVariableManager.startingDistance;

        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }
        else
        {
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
                StaticVariableManager.startingDistance = int.Parse(file_data[1]);
                adminDistanceDisplay.text = StaticVariableManager.startingDistance + " Meters";
                shooterDistanceDisplay.text = StaticVariableManager.startingDistance + " Meters";
            }
            else
            {
                Debug.LogError("Invalid scale data in the file.");
            }
        }

    }

    public void LoadDefault()
    {
        //scaleString = File.ReadAllText(filePath);
        //startScale = scaleString;
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
            StaticVariableManager.startingDistance = int.Parse(file_data[1]);
            adminDistanceDisplay.text = StaticVariableManager.startingDistance + " Meters";
            shooterDistanceDisplay.text = StaticVariableManager.startingDistance + " Meters";
        }
        else
        {
            Debug.LogError("Invalid scale data in the file.");
        }
    }

    public void CheckStartDistance()
    {
        bool reset_size = false;
        int set_counter = 0;

        if(StaticVariableManager.startingDistance / 5 != 1 && StaticVariableManager.enableStartResize)
        {
            reset_size = true;
            set_counter = StaticVariableManager.startingDistance / 5;
            print(set_counter);
        }


        if(reset_size)
        {
            while(set_counter > 1)
            {
                int count = 1;
                while(count < scale_loop_value)
                {
                    //print("Scale...");
                    ScaleObjectDown();
                    initialScale = transform.localScale;
                    count++;
                }
                set_counter --;
            }
            StaticVariableManager.enableStartResize = false;
        }

    }

    public void IncreaseDistance()
    {
        if(StaticVariableManager.startingDistance < 25)
        {
            int count = 1;
            while(count < scale_loop_value)
            {
                //print("Scale...");
                ScaleObjectDown();
                initialScale = transform.localScale;
                count++;
            }
    
            StaticVariableManager.startingDistance += scale_loop_value;
            adminDistanceDisplay.text = StaticVariableManager.startingDistance + " Meters";
            shooterDistanceDisplay.text = StaticVariableManager.startingDistance + " Meters";
            SaveInitialScale();
        }

    }

    public void DecreaseDistance()
    {
        if(StaticVariableManager.startingDistance > 5)
        {
            int count = 1;
            while(count < scale_loop_value)
            {
                ScaleObjectUp();
                initialScale = transform.localScale;
                count++;
            }

            StaticVariableManager.startingDistance -= scale_loop_value;
            adminDistanceDisplay.text = StaticVariableManager.startingDistance + " Meters";
            shooterDistanceDisplay.text = StaticVariableManager.startingDistance + " Meters";
            SaveInitialScale();
        }
    }
}
