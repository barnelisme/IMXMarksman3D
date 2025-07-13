using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NewMoveScript : MonoBehaviour
{
    public Rigidbody rigid;

    //PatrollingPoints
    [SerializeField]
    private GameObject[] points;
    private int currPoint;

    //Steer wheels
    public float maxSteerAngle = 40f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public Vector3 CenterOfMass;
    public float speed = 200;
    float currSpeed;
    float maxSpeed = 660;

    //determing Stop sign
    public bool stopSign = false;

    //Timer variables
    public float currentTime = 5f;
    float startingTime = 5f;

    //Script refencing
    RaycastDetect raycastDetect;
    public GameObject raycaster;
    public bool isCarClose = false;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        GetComponent<Rigidbody>().centerOfMass = CenterOfMass;
        currPoint = 0;
        raycastDetect = raycaster.GetComponent<RaycastDetect>();
    }

    // Update is called once per frame
    void Update()
    {
        isCarClose = raycastDetect.isObjectClose;
        StopSign();

        //drive if stop sign is not reched( stopSign = false)
        if (stopSign == false)
        {
            currentTime = 5f;
            applySteer();
            Drive();
            //Speed();
            if (Vector3.Distance(this.transform.position, points[currPoint].transform.position) <= 4f)
            {
                Iterate();
            }
        }
        else
        {
            //decrese the speed
            //Speed();
            Drive();
            applySteer();
        }
    }

    private void Iterate()
    {
        if (currPoint < points.Length - 1)// 1234 in our list is 0123
        {
            currPoint++;
        }
        else
        {
            currPoint = 0;
        }
        //agent.destination = points[currPoint].transform.position;

    }

    //rotate wheels, and point to the target.
    private void applySteer()
    {
        if (stopSign == false)
        {
            Vector3 relativeVector = transform.InverseTransformPoint(points[currPoint].transform.position);
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
        if (stopSign == false || isCarClose == false)
        {
            //move the car using it's wheel colider torgues  
            currSpeed = 2 * Mathf.PI * wheelFL.rpm * 60 / 1000;
            //checking if we are moving
            speed = 200;
            maxSpeed = 660f;
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

        if (stopSign == true || isCarClose == true)
        {
            speed = 0f;
            maxSpeed = 0f;
            currSpeed = 0f;
            wheelFL.motorTorque = 0f;
            wheelFR.motorTorque = 0f;
            rigid.linearDamping = 2f;
        }


    }

    //Stop for a moment when, way point tag is "StopSign"
    private void StopSign()
    {
        if (points[currPoint].CompareTag("StopSign"))
        {
            //check distance between car and stop sign
            if (Vector3.Distance(this.transform.position, points[currPoint].transform.position) <= 5)
            {
                stopSign = true;
            }
        }
        else
        {
            stopSign = false;
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

    //maintain the speed 
    //private void Speed()
    //{

    //    if (stopSign == true || isCarClose == true)
    //    {
    //        while (agent.speed > 0)
    //        {
    //            agent.speed -= 3 * Time.deltaTime;
    //        }
    //    }
    //    else
    //    {
    //        if (Vector3.Distance(this.transform.position, points[currPoint].transform.position) <= 5f)
    //        {
    //            while (agent.speed >= 2f)
    //            {
    //                agent.speed -= 2 * Time.deltaTime * decceleration;
    //            }
    //        }
    //        else
    //        {
    //            while (agent.speed <= 5f)
    //            {
    //                agent.speed += 1 * Time.deltaTime;
    //            }
    //        }
    //    }

    //}

}
