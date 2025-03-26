using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TargetMovementManager : MonoBehaviour
{
    float verticalSpeed;
    float horizontalSpeed;
    float horizontalDistance;
    float startPosition;
    bool moveLeft = false;
    bool moveRight = false;
    int directionFlag = 0;
    float additionalSpeed = 0;
    private float scaleReductionRate = 7f;
    private float zScaleReductionRate = 0.001f;
    private string activeScene = "";

    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;

        //startPosition = this.gameObject.transform.position.x;
        verticalSpeed = StaticVariableManager.verticalSpeed;
        horizontalSpeed = .5f;
        horizontalDistance = StaticVariableManager.horizontalDistance;
        moveLeft = false;
        moveRight = false;

        assignStartConditions();

    }

    void Update()
    {
        if (verticalSpeed != StaticVariableManager.pigeonSpeed)
        {
            assignSpeed();
        }

        transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime);
        manageScale();

        if (moveLeft)
        {
            transform.Translate(Vector3.left * horizontalSpeed * Time.deltaTime);
        }
        else if (moveRight)
        {
            transform.Translate(Vector3.right * horizontalSpeed * Time.deltaTime);
        }
    }

    private void manageScale()
    {
        if(activeScene.ToLower().Contains("pigeon"))
        {
            float scaleChange = scaleReductionRate * Time.deltaTime;
            float zScaleChange = zScaleReductionRate * Time.deltaTime;
            transform.localScale -= new Vector3(scaleChange, scaleChange, zScaleChange);

            if (transform.localScale.x <= 0)
            {
                transform.localScale = Vector3.zero;
            }
        }
    }

    private void assignStartConditions()
    {
        verticalSpeed = 3;
        horizontalSpeed = 1.5f;

        if (verticalSpeed != StaticVariableManager.pigeonSpeed)
        {
            assignSpeed();
        }

        assignDirection();
    }

    private void assignDirection()
    {
        directionFlag = Random.Range(1, 4);
        if (directionFlag == 1)
        {
            if (StaticVariableManager.directionUsed == "left")
            {
                moveRight = true;
                StaticVariableManager.directionUsed = "right";
            }
            else
            {
                moveLeft = true;
                StaticVariableManager.directionUsed = "left";
            }
        }
        else if (directionFlag == 2)
        {

            if (StaticVariableManager.directionUsed == "right")
            {
                moveLeft = true;
                StaticVariableManager.directionUsed = "left";
            }
            else
            {
                moveRight = true;
                StaticVariableManager.directionUsed = "right";
            }

        }
    }
    private void assignSpeed()
    {
        verticalSpeed = StaticVariableManager.pigeonSpeed;

        switch (StaticVariableManager.pigeonSpeed)
        {
            case 1:
                horizontalSpeed = .3f;
                scaleReductionRate = 6.5f;
                break;
            case 2:
                horizontalSpeed = .4f;
                scaleReductionRate = 7f;
                break;
            case 3:
                horizontalSpeed = .45f;
                scaleReductionRate = 8f;
                break;
            case 4:
                horizontalSpeed = 1f;
                scaleReductionRate = 10f;
                break;
            case 5:
                horizontalSpeed = 1.5f; ;
                scaleReductionRate = 10f;
                break;

        }
    }
}
