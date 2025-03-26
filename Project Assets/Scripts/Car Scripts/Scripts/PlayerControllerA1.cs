using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControllerA1 : MonoBehaviour {

    private Rigidbody rb;
    public float speedPoint = 5f;


    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        float Hori = Input.GetAxis("Horizontal");
        float vet = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(Hori, 0, vet);
        rb.MovePosition(transform.position + move * Time.deltaTime * speedPoint);
    }

    void Update()
    {

    }
    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == ("Enemy"))
        {
            print("Enemy detected");
        }
        else if(col.gameObject.tag != ("Enemy"))
        {
            print(" ");
        }
    }
    void movePlayer()
    { 
            rb. MovePosition(transform.position + Vector3.forward * Time.deltaTime * 2f);
    }
    
}
