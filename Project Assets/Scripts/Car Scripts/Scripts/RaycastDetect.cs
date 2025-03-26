using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaycastDetect : MonoBehaviour
{

    public LayerMask layerMask;
    //public LayerMask layerMask2;
    public bool isObjectClose = false;
    public bool isCivilianClose = false;
    public bool isAnimalClose = false;

    string activeScene = "";
    public GameObject holder;

    // Start is called before the first frame update
    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        RayCastCars();
        //RayCastCivilians();
    }

    private void RayCastCars()
    {
        RaycastHit hit;
        Ray downray = new Ray(transform.position, Vector3.forward);

        if(activeScene.ToLower().Contains("hunting"))
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {

                if (hit.distance < 5)
                {
                    //Debug.Log("Too CLOSE!!");
                    //print(hit.distance);
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
                    isObjectClose = true;
                }
                else
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    //Debug.Log("Hit");
                    isObjectClose = false;
                }

            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10, Color.white);
                //Debug.Log("Not Hit");
                isObjectClose = false;
            }
        }
        else if (activeScene.ToLower().Contains("shell"))
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {

                if (hit.distance < 1)
                {
                    //Debug.Log("Too CLOSE!!");
                    //print(hit.distance);
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
                    isObjectClose = true;

                    //print(hit.transform.name);

                    switch (holder.transform.name)
                    {
                        case "Cup (1)":
                            //holder cup does not stop
                            break;

                        case "Cup (2)":
                            if(hit.transform.name.Contains("Cup 1") )
                            {
                                holder.GetComponent<CupController>().stopMoving = true;
                            }
                            break;

                        case "Cup (3)":
                            if (hit.transform.name.Contains("Cup 2") || hit.transform.name.Contains("Cup 3"))
                            {
                                holder.GetComponent<CupController>().stopMoving = true;
                            }
                            break;
                    }

                }
                else
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    //Debug.Log("Hit");
                    isObjectClose = false;
                    holder.GetComponent<CupController>().stopMoving = false;
                }

            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10, Color.white);
                //Debug.Log("Not Hit");
                isObjectClose = false;
            }
        }
        else
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {

                if (hit.distance < 30)
                {
                    //Debug.Log("Too CLOSE!!");
                    //print(hit.distance);
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
                    isObjectClose = true;
                    //holder.GetComponent<CupController>().stopMoving = true;
                }
                else
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    //Debug.Log("Hit");
                    isObjectClose = false;
                    
                }

            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10, Color.white);
                //Debug.Log("Not Hit");
                isObjectClose = false;
            }
        }

    }

    //private void RayCastCivilians()
    //{
    //    RaycastHit hit;
    //    Ray downray = new Ray(transform.position, Vector3.forward);

    //    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask2))
    //    {

    //        if (hit.distance < 15)
    //        {
    //            //Debug.Log("Too CLOSE!!");
    //            //print(hit.distance);
    //            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
    //            isCivilianClose = true;
    //        }
    //        else
    //        {
    //            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
    //            //Debug.Log("Hit");
    //            isCivilianClose = false;
    //        }

    //    }
    //    else if(!(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask2)) && !(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask)))
    //    {
    //        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10, Color.white);
    //        //Debug.Log("Not Hit");
    //        isCivilianClose = false;
    //    }
    //}

}
