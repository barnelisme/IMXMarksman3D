using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RisingPlatesDisabler : MonoBehaviour
{

    public GameObject[] plates;
    private bool platesDisabled = false;
    string activeScene = "";

    public GameObject currHiddenTarget = null;

    // Expression scene
    public GameObject SequenceManager;

    // Start is called before the first frame update
    void Start()
    {
        StaticVariableManager.lanePlatesDisabled = false;
        StaticVariableManager.isStopTraining = false;
        activeScene = SceneManager.GetActiveScene().name;

        if(SequenceManager != null)
        {
            plates = SequenceManager.GetComponent<ExpressionSceneManager>().defaultPlates.ToArray();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(StaticVariableManager.isStopTraining)
        {
            if(platesDisabled == false)
            {
                if(activeScene.ToLower().Contains("ipecboard") || activeScene.ToLower().Contains("duelingtree"))
                {
                    ActivatePlates();
                }
                else
                {
                    // DeactivatePlates();
                }

                platesDisabled = true;
                StaticVariableManager.lanePlatesDisabled = true;

                /*if(activeScene.ToLower().Contains("risingplate"))
                {
                    int totalNumPlates = TestConditionsManager.numLaneTargets * TestConditionsManager.numLanes;
                    StaticVariableManager.lanePlatesDisabledCount++;
                    // print("Disabled: " + StaticVariableManager.lanePlatesDisabledCount.ToString() + "Out of " + totalNumPlates);

                    if (StaticVariableManager.lanePlatesDisabledCount >= totalNumPlates)
                    {
                        StaticVariableManager.lanePlatesDisabled = true;
                    }
                }
                else
                {
                    StaticVariableManager.lanePlatesDisabled = true;
                }*/

            }
        }
    }

    private void ActivatePlates()
    {
        foreach (GameObject plate in plates)
        {
            // plate.GetComponent<MeshCollider>().enabled = false;
            plate.SetActive(true);
        }
    }

    private void DeactivatePlates()
    {
        if(activeScene.ToLower().Contains("hiddentarget"))
        {
            currHiddenTarget.SetActive(false);
        }
        else
        {
            foreach (GameObject plate in plates)
            {
                plate.GetComponent<MeshCollider>().enabled = false;
                plate.SetActive(false);
            }
        }
    }

}
