using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredelayManager : MonoBehaviour
{

    private float lane1delayTimer = 0;
    private float lane2delayTimer = 0;
    private float lane3delayTimer = 0;

    private float setDelayTimer = 1f;

    // Update is called once per frame
    void Update()
    {
        
        if(StaticVariableManager.lane1PredelayActive)
        {
            lane1delayTimer -= Time.deltaTime;
            print("RE: lane 1 predelay is " + lane1delayTimer);
            if(lane1delayTimer <= 0f)
            {
                lane1delayTimer = setDelayTimer;
                StaticVariableManager.lane1PredelayActive = false;
            }
        }

        if (StaticVariableManager.lane2PredelayActive)
        {
            lane2delayTimer -= Time.deltaTime;
            print("RE: lane 2 predelay is " + lane2delayTimer);
            if (lane2delayTimer <= 0f)
            {
                lane2delayTimer = setDelayTimer;
                StaticVariableManager.lane2PredelayActive = false;
            }
        }

        if (StaticVariableManager.lane3PredelayActive)
        {
            lane3delayTimer -= Time.deltaTime;
            print("RE: lane 3 predelay is " + lane3delayTimer);
            if (lane3delayTimer <= 0f)
            {
                lane3delayTimer = setDelayTimer;
                StaticVariableManager.lane3PredelayActive = false;
            }
        }

    }
}
