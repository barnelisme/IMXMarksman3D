using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BaloonMovementController : MonoBehaviour
{

    float verticalSpeed;
    float horizontalSpeed;
    float horizontalDistance;
    float startPosition;
    bool moveLeft = false;
    bool moveRight = false;
    int directionFlag = 0;

    float activeTimer = 30;
    float horizontalDistanceInc = 0;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = this.gameObject.transform.position.x;
        verticalSpeed = StaticVariableManager.verticalSpeed;
        horizontalSpeed = StaticVariableManager.horizontalSpeed;
        horizontalDistance = StaticVariableManager.horizontalDistance;
        directionFlag = Random.Range(1, 4);

        switch(directionFlag)
        {
            case 1:
                moveLeft = true;
                break;
            case 2:
                moveRight = true;
                break;
            case 3:
                moveLeft = true;
                break;
            case 4:
                moveRight = true;
                break;
        }
        //print("RE: Start direction is " + directionFlag);
    }

    // Update is called once per frame
    void Update()
    {
        if(TestConditionsManager.baloonDirection.ToLower().Contains("upwards"))
        {
            transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime);
        }
        else if (TestConditionsManager.baloonDirection.ToLower().Contains("downwards"))
        {
            transform.Translate(Vector3.up * -verticalSpeed * Time.deltaTime);
        }
        //transform.Translate(Vector3.forward * .2f * Time.deltaTime);
        UpdateMovementData();
        manageRunTime();

        if(moveLeft)
        {
            if(transform.position.x <= startPosition - horizontalDistance)
            {
                moveRight = true;
                moveLeft = false;
            }
            transform.Translate(Vector3.left * horizontalSpeed * Time.deltaTime);
        }
        else if(moveRight)
        {
            if (transform.position.x >= startPosition + horizontalDistance)
            {
                moveRight = false; ;
                moveLeft = true;
            }
            transform.Translate(Vector3.right * horizontalSpeed * Time.deltaTime);
        }
        
    }

    private void manageRunTime()
    {
        if (activeTimer <= 0f)
        {
            Destroy(this.gameObject);
        }
        else
        {
            activeTimer -= Time.deltaTime * 1;
        }
    }

    private void UpdateMovementData()
    {
        verticalSpeed = StaticVariableManager.verticalSpeed;
        horizontalSpeed = StaticVariableManager.horizontalSpeed;
        horizontalDistance = StaticVariableManager.horizontalDistance;
    }
}
