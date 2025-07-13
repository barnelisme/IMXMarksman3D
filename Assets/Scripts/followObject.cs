using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followObject : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    Transform position;
    float speed = 2;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //position = player.GetComponent<Transform>();
        //position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 2 * Time.deltaTime);
    }
}
