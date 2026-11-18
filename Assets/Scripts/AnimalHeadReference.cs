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
            if (mainBody.transform.name.ToLower().Contains(TestConditionsManager.animalName.ToLower()))
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

        // print("Animal shot...!");
        
        // ⭐ FIXED: Check collider exists before disabling
        CapsuleCollider headCollider = this.GetComponent<CapsuleCollider>();
        if (headCollider != null)
        {
            headCollider.enabled = false;
        }
        
        // ⭐ FIXED: Check collider exists before disabling
        CapsuleCollider bodyCollider = mainBody.GetComponent<CapsuleCollider>();
        if (bodyCollider != null)
        {
            bodyCollider.enabled = false;
        }

        mainBody.GetComponent<HuntAnimalController>().ApplyDamage(mainBody.transform.name);
        targetDestroyed = true;

        if(activeScene.ToLower().Contains("outdoor"))
        {
            // ⭐ FIXED: Check collider exists before disabling
            CapsuleCollider outdoorCollider = this.transform.gameObject.GetComponent<CapsuleCollider>();
            if (outdoorCollider != null)
            {
                outdoorCollider.enabled = false;
            }
        }
        else if(activeScene.ToLower().Contains("ground"))
        {
            // ⭐ FIXED: Check collider exists before disabling
            CapsuleCollider groundCollider = this.transform.gameObject.GetComponent<CapsuleCollider>();
            if (groundCollider != null)
            {
                groundCollider.enabled = false;
            }
        }

    }

}
