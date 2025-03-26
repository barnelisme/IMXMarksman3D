using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimalHeadReference : MonoBehaviour
{

    public GameObject mainBody;
    public GameObject followPoint;
    bool targetDestroyed = false;
    private bool globalVariableUpdated = false;
    public bool isHeadShot = false;
    string activeScene = "";

    private void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        targetDestroyed = false;
    }

    private void Update()
    {
        //follow point only if the object is not dead
        if(!targetDestroyed)
        {
            this.transform.position = followPoint.transform.position;
        }
        else
        {
            //Destroy(this.gameObject);
        }
    }
    private void ApplyDamage(string tagged)
    {
        isHeadShot = true;

        if (!globalVariableUpdated)
        {
            if (tagged.ToLower().Contains(TestConditionsManager.animalName.ToLower()))
            {
                StaticVariableManager.totalTargetAnimalsKilled += 1;
            }
            else
            {
                StaticVariableManager.totalTargetCasualtiesKilled += 1;
            }

            StaticVariableManager.totalHeadShots += 1;
            globalVariableUpdated = true;
            print("Head shots is : " + StaticVariableManager.totalHeadShots);
        }

        mainBody.GetComponent<AnimalController>().isKilled = true;
        targetDestroyed = true;

        if(activeScene.ToLower().Contains("outdoor"))
        {
            this.transform.gameObject.GetComponent<BoxCollider>().enabled = false;
            this.transform.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }
        else if(activeScene.ToLower().Contains("ground"))
        {
            this.transform.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }

    }

}
