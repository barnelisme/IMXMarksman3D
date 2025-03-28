using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wheelsRotation : MonoBehaviour
{
    public WheelCollider myCollider;
    private Vector3 wheelPosition = new Vector3();
    private Quaternion wheelRotation = new Quaternion();
  
    void Update()
    {
        myCollider.GetWorldPose(out wheelPosition, out wheelRotation);
        transform.position = wheelPosition;
        transform.rotation = wheelRotation;
    }
}
