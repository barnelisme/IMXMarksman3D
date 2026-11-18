using UnityEngine;
using UnityEngine.SceneManagement;

public class PepperPopperManager : MonoBehaviour
{
    private float speed = 180f; // degrees per second
    private float currentYRotation = 0f;
    private Vector3 startPosition;     // store initial position
    private Quaternion startRotation;  // store initial rotation

    public GameObject[] platesSet;

    private bool movePlate = false;
    private bool resetPlate = true;
    private string activeScene = "";
    private bool pepperNameSet = false;

    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;

        if (this.transform.name.Contains("Steel Plates"))
        {
            print("Point Reached...");
        }
        else
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
        }

    }

    void Update()
    {
        if (this.transform.name.Contains("Pepper Popper Set"))
        {
            string name = this.transform.name;

            int index = name.IndexOf("target", System.StringComparison.OrdinalIgnoreCase);
            if (index >= 0 && pepperNameSet == false) // "target" found
            {
                string targetName = name.Substring(index);
                Debug.Log("Extracted target name: " + targetName);

                foreach(GameObject plate in platesSet)
                {
                    plate.transform.name += " " + targetName;
                }

                pepperNameSet = true;
            }
        }
        else
        {
            if (movePlate && !StaticVariableManager.isStopTraining)
            {
                float step = speed * Time.deltaTime; // rotation step this frame
                transform.Rotate(step, 0, 0);       // apply rotation
                currentYRotation += step;            // keep track of current angle

                if (currentYRotation >= 100f)
                {
                    movePlate = false;
                }
            }
            else if (StaticVariableManager.isStopTraining && resetPlate)
            {
                // transform.position = startPosition;
                transform.rotation = startRotation;  // also reset rotation
                currentYRotation = 0f;               // reset counter
                resetPlate = false;
            }
        }

    }

    public void MovePlate()
    {
        movePlate = true;
    }

}
