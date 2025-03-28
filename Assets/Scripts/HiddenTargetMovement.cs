using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenTargetMovement : MonoBehaviour
{

    public GameObject [] shootPoints ;
    public GameObject safePoint;
    public GameObject targetSpawner;

    bool isShootPointReached;
    bool isSafePointReached;

    float threshold = 0.5f;
    float speed = 10f;
    float setDelayTime = 2;
    float plateDelayTime = 2;
    int shootIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        isShootPointReached = false;
        isSafePointReached = true;

        //all_soldiers = GameObject.FindGameObjectsWithTag("soldier");
        targetSpawner = GameObject.FindGameObjectWithTag("targetSpawner");
        shootPoints = targetSpawner.GetComponent<targetSpawner>().shootPoints;
        safePoint = targetSpawner.GetComponent<targetSpawner>().safePoint;
        assignShootIndex();
    }

    private void assignShootIndex()
    {
        shootIndex = Random.Range(0, 9);
        do
        {
            shootIndex = Random.Range(0, 9);
        } while (shootIndex == StaticVariableManager.currentIndex);
        StaticVariableManager.currentIndex = shootIndex;
    }

    // Update is called once per frame
    void Update()
    {
        manageMovements(); 
        
    }

    private void manageMovements()
    {
        if(countDownStart.start_training && !StaticVariableManager.isTrainingPause)
        {
            if (!isShootPointReached)
            {
                if (Vector3.Distance(transform.position, shootPoints[shootIndex].transform.position) < threshold)
                {
                    plateDelayTime -= Time.deltaTime * 1;
                    if (plateDelayTime < 0f)
                    {

                        isSafePointReached = false;
                        isShootPointReached = true;

                        plateDelayTime = 0;
                    }
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, shootPoints[shootIndex].transform.position, speed * Time.deltaTime);
                }
            }


            if (!isSafePointReached)
            {

                if (Vector3.Distance(transform.position, safePoint.transform.position) < threshold)
                {
                    plateDelayTime -= Time.deltaTime * 1;
                    if (plateDelayTime < 0f)
                    {

                        isSafePointReached = true;
                        isShootPointReached = false;
                        assignShootIndex();
                        plateDelayTime = setDelayTime;
                    }
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, safePoint.transform.position, speed * Time.deltaTime);
                }
            }
        }
    }
}
