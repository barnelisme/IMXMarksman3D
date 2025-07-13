using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DieFlipper : MonoBehaviour
{
    private float rotationSpeed = 450;
    public List<GameObject> die_sides = new List<GameObject>();
    public List<GameObject> die_position = new List<GameObject>();
    private float rollTimer = 0;
    private float setRollTimer = 1f;
    private bool rollComplete = false;
    private bool targetPrepComplete = false;
    private bool targets_initialised = false;
    private int randomSide = 0;

    private void Start()
    {
        rollTimer = setRollTimer;
        initialiseTargets();
    }

    private void initialiseTargets()
    {
        if(!targets_initialised)
        {
            foreach (GameObject side in die_sides)
            {
                side.GetComponent<DieTargetController>().initialiseTargets();
            }

            targets_initialised = true;
        }
    }

    void Update()
    {
        if(countDownStart.start_training)
        {
            if (rollComplete)
            {
                if(!targetPrepComplete)
                {
                    //rollTimer = setRollTimer;
                    randomSide = Random.Range(1, 6);
                    StaticVariableManager.numDieTargets = randomSide;
                    this.transform.rotation = die_position[randomSide - 1].transform.rotation;

                    foreach (GameObject target in die_sides[randomSide - 1].GetComponent<DieTargetController>().targets)
                    {
                        target.GetComponent<MeshCollider>().enabled = true;
                    }
                    

                    targetPrepComplete = true;
                }
                else
                {
                    if(StaticVariableManager.numDieTargets <= 0)
                    {
                        targets_initialised = false; // set trigger for target initialisation
                        rollComplete = false;
                        targetPrepComplete = false;
                        rollTimer = setRollTimer;
                    }
                }
            }
            else
            {
                if (rollTimer > 0f)
                {
                    initialiseTargets(); // initialise targets plates

                    rollTimer -= Time.deltaTime;
                    // Calculate rotation increments based on time
                    float rotationX = Time.deltaTime * rotationSpeed;
                    float rotationY = Time.deltaTime * rotationSpeed;

                    // Apply the rotation increments to the object's current rotation
                    transform.Rotate(rotationX, rotationY, 0);
                }
                else
                {
                    rollComplete = true;
                }
            }
        }
    }
}
