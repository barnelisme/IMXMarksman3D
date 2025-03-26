using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingPlatesDisabler : MonoBehaviour
{

    public GameObject[] plates;
    private bool platesDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        StaticVariableManager.platesDisabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(StaticVariableManager.isStopTraining )
        {
            if(platesDisabled == false)
            {
                foreach (GameObject plate in plates)
                {
                    plate.GetComponent<MeshCollider>().enabled = false;
                    plate.SetActive(false);
                }

                platesDisabled = true;
                StaticVariableManager.platesDisabled = true;
            }
        }
    }

}
