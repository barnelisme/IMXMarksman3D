using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController1 : MonoBehaviour {

    [SerializeField]
    Vector3 v3Force;
    
    //Controllers
    [SerializeField]
    KeyCode KeyPositive;
    [SerializeField]
    KeyCode KeyNegetive;

    void FixedUpdate()
    {
        if (Input.GetKey(KeyPositive))
            GetComponent<Rigidbody>().linearVelocity += v3Force;
        if (Input.GetKey(KeyNegetive))
            GetComponent<Rigidbody>().linearVelocity -= v3Force;
    }
}
