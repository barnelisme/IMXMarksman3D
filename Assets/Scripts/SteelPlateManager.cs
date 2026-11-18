using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteelPlateManager : MonoBehaviour
{
    private float speed = 120f; // degrees per second
    private float currentYRotation = 0f;
    private Vector3 startPosition;     // store initial position
    private Quaternion startRotation;  // store initial rotation

    public GameObject[] platesHolders;

    private bool movePlate = false;
    private bool resetPlate = true;
    private string activeScene = "";

    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;

        if(this.transform.name.Contains("Steel Plates"))
        {
            print("Point Reached...");
            SetActivePlates();
        }
        else
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
        }

    }

    void Update()
    {
        if (movePlate)
        {
            float step = speed * Time.deltaTime; // rotation step this frame
            transform.Rotate(0, -step, 0);       // apply rotation
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

    public void MovePlate()
    {
        movePlate = true;
    }

    private void SetActivePlates()
    {
        DeactivateHolders(); // Deactivate all holder before activating the current setting.
        int startingPlatesNum = 4;

        switch (StaticVariableManager.numIPSCSteelPlates)
        {
            case 4:
                platesHolders[StaticVariableManager.numIPSCSteelPlates - startingPlatesNum].SetActive(true); 
                break;
            case 5:
                platesHolders[StaticVariableManager.numIPSCSteelPlates - startingPlatesNum].SetActive(true);
                break;
            case 6:
                platesHolders[StaticVariableManager.numIPSCSteelPlates - startingPlatesNum].SetActive(true);
                break;
            case 7:
                platesHolders[StaticVariableManager.numIPSCSteelPlates - startingPlatesNum].SetActive(true);
                break;
            case 8:
                platesHolders[StaticVariableManager.numIPSCSteelPlates - startingPlatesNum].SetActive(true);
                break;
            case 9:
                platesHolders[StaticVariableManager.numIPSCSteelPlates - startingPlatesNum].SetActive(true);
                break;
            case 10:
                platesHolders[StaticVariableManager.numIPSCSteelPlates - startingPlatesNum].SetActive(true);
                break;
            case 11:
                platesHolders[StaticVariableManager.numIPSCSteelPlates - startingPlatesNum].SetActive(true);
                break;
            case 12:
                platesHolders[StaticVariableManager.numIPSCSteelPlates - startingPlatesNum].SetActive(true);
                break;
        }
    }
    private void DeactivateHolders()
    {
        foreach(GameObject holder in platesHolders)
        {
            holder.SetActive(false);
        }
    }
}
