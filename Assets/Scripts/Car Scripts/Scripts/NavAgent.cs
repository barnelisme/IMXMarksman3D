using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAgent : MonoBehaviour
{

    public NavMeshAgent agent;
    public Rigidbody rigid;
    Animator animator;
    //Patrolling
    [SerializeField]
    private GameObject[] points;

    private int currPoint;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        agent.autoBraking = false;
        currPoint = 0;
        //agent.speed = 1f;
        agent.destination = points[currPoint].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(this.transform.position, points[currPoint].transform.position) <= 4f)
        {
            Iterate();
        }
    }

    void Iterate()
    {
        if (currPoint < points.Length - 1)// 1234 in our list is 0123
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
}
