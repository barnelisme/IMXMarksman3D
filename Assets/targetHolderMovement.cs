using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetHolderMovement : MonoBehaviour
{
    public float speed = 5f;         // Speed of movement
    public float distance = 1f;      // Total distance to move
    private float startPositionX;    // Starting position on x-axis
    private float endPositionX;       // Ending position on x-axis
    private float direction = 1f;    // Movement direction: 1 for right, -1 for left

    void Start()
    {
        startPositionX = transform.position.x;  // Get starting x position
        endPositionX = startPositionX ; // Calculate end position based on distance
    }

    void Update()
    {
        // Calculate the new x position based on current direction and speed
        float newX = transform.position.x + direction * StaticVariableManager.tableSpeed * Time.deltaTime;

        // Check if the object has reached the boundaries
        if ((direction == 1 && newX >= startPositionX + .7f) || (direction == -1 && newX <= startPositionX - .7f))
        {
            // Reverse direction
            direction *= -1f;
        }

        // Update the transform's position with the new x value
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}
