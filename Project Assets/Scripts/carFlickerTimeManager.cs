using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carFlickerTimeManager : MonoBehaviour
{
    float flickerEnableTimer = 0;
    float flickerDisableTimer = 0;
    float setFlickerCheckTimer = 8;
    void Start()
    {
        flickerEnableTimer = setFlickerCheckTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if(countDownStart.start_training && StaticVariableManager.isStopTraining == false)
        {
            if (StaticVariableManager.flickerSet == false)
            {
                if (flickerEnableTimer <= 0f)
                {
                    flickerDisableTimer = 5;
                    StaticVariableManager.flickerSet = true;
                    //print("Flicker Active!");
                }
                else
                {
                    flickerEnableTimer -= Time.deltaTime * 1;
                }

            }
            else
            {
                if (flickerDisableTimer <= 0)
                {
                    flickerEnableTimer = setFlickerCheckTimer;
                    StaticVariableManager.flickerSet = false;
                    //print("Flicker disabled...");
                }
                else
                {
                    flickerDisableTimer -= Time.deltaTime * 1;
                }

            }
        }


    }
}
