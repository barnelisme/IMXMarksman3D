using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMove : MonoBehaviour
{
    public NavMeshAgent agent;
    public Rigidbody rigid;
    Animator animator;
    //Patrolling
    [SerializeField]
    private GameObject[] points;

    private int currPoint;

    //Steer wheels
    public float maxSteerAngle = 40f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public Vector3 CenterOfMass;
    float speed = 140;
    float currSpeed;
    float maxSpeed = 660;

    //Speed variable
    float decceleration = 1.1f;

    //determing Stop sign
    private bool stopSign = false;

    //Timer variables
    float currentTime = 0f;
    float startingTime = 5f;

    //Script refencing
    RaycastDetect raycastDetect;
    public GameObject raycaster;
    public bool isCarClose;

    //Awake function, which gets called first before start
    private void Awake()
    {
        //raycastDetect = GameObject.Find("vanCar1").GetComponent<RaycastDetect>();
        raycastDetect = raycaster.GetComponent<RaycastDetect>();
        isCarClose = raycastDetect.isObjectClose;
        agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        GetComponent<Rigidbody>().centerOfMass = CenterOfMass;
        agent.autoBraking = false;
        currPoint = 0;
        //agent.speed = 1f;
        agent.destination = points[currPoint].transform.position;
        //Timer
        currentTime = startingTime;

    }

    //Start function
    /*void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        GetComponent<Rigidbody>().centerOfMass = CenterOfMass;
        raycastDetect = raycaster.GetComponent<RaycastDetect>();
        agent.autoBraking = false;
        currPoint = 0;
        //agent.speed = 1f;
        //agent.destination = points[currPoint].transform.position;
        //Timer
        currentTime = startingTime;
    }*/

    // Update is called once per frame
    void Update()
    {
        //Call stop sign fuction
        StopSign();
        //isCarClose = raycastDetect.isCarClose;

        //drive if stop sign is not reched( stopSign = false)
        if (stopSign == false)
        {
            currentTime = 5f;
            applySteer();
            Drive();
            Speed();
            if (Vector3.Distance(this.transform.position, points[currPoint].transform.position) <= 4f)
            {
                Iterate();
            }
        }
        else
        {
            //decrese the speed
            Speed();
            Drive();
            applySteer();
        }
        
  
    }

    //assign new target when currect target is reached.
    private void Iterate()
    {
        if(currPoint < points.Length -1)// 1234 in our list is 0123
        {
            currPoint++;
        }
        else
        {
            currPoint = 0;
        }
        agent.destination = points[currPoint].transform.position;
        //print(currPoint);
    }

    //rotate wheels, and point to the target.
    private void applySteer()
    {
        if(stopSign == false)
        {
            Vector3 relativeVector = transform.InverseTransformPoint(points[currPoint].transform.position);
            //relativeVector /= relativeVector.magnitude;
            //changing the rotation of the wheels to look at the destination node
            float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
            wheelFL.steerAngle = newSteer;
            wheelFR.steerAngle = newSteer;
        }

        if (stopSign == true)
        {
            float newSteer = 0f;
            wheelFL.steerAngle = newSteer;
            wheelFR.steerAngle = newSteer;
        }
    }

    //drive wheels.
    private void Drive()
    {
        if(stopSign == false)
        {
            //move the car using it's wheel colider torgues  
            currSpeed = 2 * Mathf.PI * wheelFL.rpm * 60 / 1000;
            //checking if we are moving
            if (currSpeed < maxSpeed)
            {
                wheelFL.motorTorque = speed;
                wheelFR.motorTorque = speed;
            }
            else
            {
                wheelFL.motorTorque = 0;
                wheelFR.motorTorque = 0;
            }
            rigid.linearDamping = 0f;
        }

        if(stopSign == true || isCarClose == true)
        {
            speed = 0f;
            maxSpeed = 0f;
            currSpeed = 0f;
            wheelFL.motorTorque = 0f;
            wheelFR.motorTorque = 0f;
            rigid.linearDamping = 2f;
        }

    }

    //maintain the speed 
    private void Speed()
    {

        if(stopSign == true || isCarClose == true)
        {
            while (agent.speed > 0)
            {
                agent.speed -= 3 * Time.deltaTime;
            }
        }
        else
        {
            if (Vector3.Distance(this.transform.position, points[currPoint].transform.position) <= 5f)
            {
                while (agent.speed >= 2f)
                {
                    agent.speed -= 2 * Time.deltaTime * decceleration;
                }
            }
            else
            {
                while (agent.speed <= 5f)
                {
                    agent.speed += 1 * Time.deltaTime;
                }
            }
        }

    }

    //Stop for a moment when, way point tag is "StopSign"
    private void StopSign()
    {
        if(points[currPoint].CompareTag("StopSign"))
        {
            //check distance between car and stop sign
            if (Vector3.Distance(this.transform.position, points[currPoint].transform.position) <= 5)
            {
                stopSign = true;

            }
        }

        if (stopSign == true)
        {
            //wait four seconds then turn stoSign to false
            currentTime -= 1 * Time.deltaTime;

            if (currentTime < 1f)
            {
                stopSign = false;
                Iterate();
            }
            else
            {
                //print(currentTime);
            }
        }

    }

    //collision

}

//On mouce click. 0 is Left click, and 1 is right click
/*if(Input.GetMouseButtonDown(0))
{
    Ray movePosition = Camera.main.ScreenPointToRay(Input.mousePosition);
    if(Physics.Raycast(movePosition, out var hitInfo))
    {
        agent.SetDestination(hitInfo.point);

    }
}*/
