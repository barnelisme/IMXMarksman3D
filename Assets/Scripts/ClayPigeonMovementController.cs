using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovementController : MonoBehaviour
{
    float verticalSpeed;
    float horizontalSpeed;
    float horizontalDistance;
    float startPosition;
    bool moveLeft = false;
    bool moveRight = false;
    int directionFlag = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime);
    }
}
