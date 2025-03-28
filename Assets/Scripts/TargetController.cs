using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TargetController : MonoBehaviour
{
    //Local Variables
    int nextTargetSelector;
    int startTarget = 0;
    public GameObject player;
    string activeScene = " ";

    //global variables
    public static int num_fail_hits = 0;
    public static int num_pass_hits = 0;

    //Fallinf Plate Variables
    public GameObject next_Target;
    public GameObject currect_Target;
    public GameObject currect_FailLight;

    //RifflePole SHooting
    public GameObject n_Target1;
    public GameObject n_Target2;
    public GameObject n_Target3;

    public GameObject failLight_1;
    public GameObject failLight_2;
    public GameObject failLight_3;

    //Timers
    float failLight_Timer;
    float failLight_setTime = .2f;
    bool isFail = false;

    //Resetable Scene Variables
    [Header("Resetable Scene Variables")]
    public GameObject r_Target1;
    public GameObject r_Target2;
    public GameObject r_Target3;
    public GameObject r_Target4;
    public GameObject r_Target5;
    public GameObject r_Target6;
    public GameObject rb_Target1;
    public GameObject rb_Target2;
    public GameObject rb_Target3;
    public GameObject rb_Target4;
    public GameObject rb_Target5;
    public GameObject rb_Target6;
    public bool isRandomState = false;
    int targetHitCount = 6;
    GameObject[] rightTargets;


    void Start()
    {
        //activeScene = player.transform.gameObject.GetComponent<Shooting>().activeScene;
        activeScene = SceneManager.GetActiveScene().name;
        num_fail_hits = 0;
        num_pass_hits = 0;

        print("Active Scene is: " + activeScene);
        if(activeScene.ToLower().Contains("riffle"))
        {
            print("IN THE RIFFLE SCENE");
            generateRandomTarget("start");
        }
        if(activeScene.ToLower().Contains("resetable"))
        {
            setCurrentTarget();
            //print("Current target name is " + currect_Target.transform.name);
        }
        //print("Current Target is :" + currect_Target.transform.name);
    }

    private void generateRandomTarget(string tagged)
    {
        if(tagged == "start")
        {
            print("IN THE RIFFLE SCENE");
            startTarget = Random.Range(1, 3);
            if (startTarget == 1)
            {
                n_Target1.SetActive(true);
                currect_Target = n_Target1;
            }
            else if (startTarget == 2)
            {
                n_Target2.SetActive(true);
                currect_Target = n_Target2;
            }
            else if (startTarget == 3)
            {
                n_Target3.SetActive(true);
                currect_Target = n_Target3;
            }
            else
            {
                n_Target1.SetActive(true);
                currect_Target = n_Target1;
            }
        }
        else if (currect_Target.gameObject.name.ToLower().Contains(tagged))
        {
            nextTargetSelector = Random.Range(0, 3);

            num_pass_hits++;
            if (nextTargetSelector == 0)
            {

                if (currect_Target.gameObject.name == n_Target1.gameObject.name)
                {
                    n_Target2.SetActive(true);
                    currect_Target = n_Target2;
                }
                else
                {
                    n_Target1.SetActive(true);
                    currect_Target = n_Target1;
                }
            }
            if (nextTargetSelector == 1)
            {
                if (currect_Target.gameObject.name == n_Target2.gameObject.name)
                {
                    n_Target3.SetActive(true);
                    currect_Target = n_Target3;
                }
                else
                {
                    n_Target2.SetActive(true);
                    currect_Target = n_Target2;
                }
            }
            if (nextTargetSelector == 2)
            {
                if (currect_Target.gameObject.name == n_Target3.gameObject.name)
                {
                    n_Target1.SetActive(true);
                    currect_Target = n_Target1;
                }
                else
                {
                    n_Target3.SetActive(true);
                    currect_Target = n_Target3;
                }
            }
            if (nextTargetSelector == 3)
            {
                if (currect_Target.gameObject.name == n_Target1.gameObject.name)
                {
                    n_Target2.SetActive(true);
                    currect_Target = n_Target2;
                }
                else
                {
                    n_Target1.SetActive(true);
                    currect_Target = n_Target1;
                }
            }

            print("Test: Current Target Name is: " + currect_Target);
        }
        else if (!currect_Target.gameObject.name.Contains(tagged))
        {
            num_fail_hits++;
            if (tagged.ToLower().Contains("target 1"))
            {
                failLight_1.SetActive(true);
                currect_FailLight = failLight_1;
                isFail = true;
            }
            if (tagged.ToLower().Contains("target 2"))
            {
                failLight_2.SetActive(true);
                currect_FailLight = failLight_2;
                isFail = true;
            }
            if (tagged.ToLower().Contains("target 3"))
            {
                failLight_3.SetActive(true);
                currect_FailLight = failLight_3;
                isFail = true;
            }

            print("Test: Shot wrong target.");
        }
    }

    void redoStart()
    {
        activeScene = player.gameObject.GetComponent<Shooting>().activeScene;
        print("Active Scene is: " + activeScene);
        if (activeScene.ToLower().Contains("riffle"))
        {
            //print("IN THE RIFFLE SCENE");
            startTarget = Random.Range(1, 3);
            if (startTarget == 1)
            {
                n_Target1.SetActive(true);
                currect_Target = n_Target1;
            }
            else if (startTarget == 2)
            {
                n_Target2.SetActive(true);
                currect_Target = n_Target2;
            }
            else if (startTarget == 3)
            {
                n_Target3.SetActive(true);
                currect_Target = n_Target3;
            }
            else
            {
                n_Target1.SetActive(true);
                currect_Target = n_Target1;
            }
        }
        if (activeScene.ToLower().Contains("resetable"))
        {
            setCurrentTarget();
            //print("Current target name is " + currect_Target.transform.name);
        }
        //print("Current Target is :" + currect_Target.transform.name);
    }
    // Update is called once per frame
    void Update()
    {
        if(isFail)
        {
            failLight_Timer -= Time.deltaTime * 1;
            if(failLight_Timer <= 0f)
            {
                currect_FailLight.SetActive(false);
                isFail = false;
            }
        }
        else
        {
            failLight_Timer = failLight_setTime;
        }
    }
    void selectNextTarget()
    {
        currect_Target.gameObject.transform.GetComponent<TargetMessageReceive>().SwitchTarget();
    }
    void dropCurrentTarget()
    {
        currect_Target.gameObject.transform.GetComponent<TargetMessageReceive>().DropTarget();
    }
    void setCurrentTarget()
    {
        nextTargetSelector = Random.Range(0, 5);
        if (nextTargetSelector == 0)
        {
            currect_Target = r_Target1;
        }
        if (nextTargetSelector == 1)
        {
            currect_Target = r_Target4;
        }
        if (nextTargetSelector == 2)
        {
            currect_Target = r_Target3;
        }
        if (nextTargetSelector == 3)
        {
            currect_Target = r_Target4;
        }
        if (nextTargetSelector == 4)
        {
            currect_Target = r_Target5;
        }
        if (nextTargetSelector == 5)
        {
            currect_Target = r_Target6;
        }

    }
    public void ReceiveMessage(string tagged)
    {
        //redoStart();
        //activeScene = player.gameObject.GetComponent<Shooting>().activeScene;
        if (activeScene.ToLower().Contains("riffle"))
        {
            print("Test: target hit is...: " + tagged);
            print("Test: current target is...: " + currect_Target.gameObject.name);

            generateRandomTarget(tagged);
        }

        if (activeScene.ToLower().Contains("resetable"))
        {
            
            if (!isRandomState)
            {
                if(tagged.ToLower().Contains("right"))
                {
                    targetHitCount--;
                }
                else if (tagged.ToLower().Contains("left"))
                {
                    targetHitCount++;
                }
                if(targetHitCount <= 0)
                {
                    selectNextTarget();
                    isRandomState = true;
                }
                
                print("TargetHitCount is :" + targetHitCount);
            }
            else
            {
                //selectNextTarget();
                nextTargetSelector = Random.Range(0,5);
                if (currect_Target.gameObject.name.Contains(tagged))
                {
                    //setCurrentTarget();
                    dropCurrentTarget();
                    if (nextTargetSelector == 0)
                    {

                        if (currect_Target.gameObject.name == r_Target1.gameObject.name)
                        {
                            r_Target2.SetActive(true);
                            currect_Target = r_Target2;
                        }
                        else
                        {
                            r_Target1.SetActive(true);
                            currect_Target = r_Target1;
                        }
                    }
                    if (nextTargetSelector == 1)
                    {

                        if (currect_Target.gameObject.name == r_Target2.gameObject.name)
                        {
                            r_Target3.SetActive(true);
                            currect_Target = r_Target3;
                        }
                        else
                        {
                            r_Target2.SetActive(true);
                            currect_Target = r_Target2;
                        }
                    }
                    if (nextTargetSelector == 2)
                    {

                        if (currect_Target.gameObject.name == r_Target3.gameObject.name)
                        {
                            r_Target4.SetActive(true);
                            currect_Target = r_Target4;
                        }
                        else
                        {
                            r_Target3.SetActive(true);
                            currect_Target = r_Target3;
                        }
                    }
                    if (nextTargetSelector == 3)
                    {

                        if (currect_Target.gameObject.name == r_Target4.gameObject.name)
                        {
                            r_Target5.SetActive(true);
                            currect_Target = r_Target5;
                        }
                        else
                        {
                            r_Target4.SetActive(true);
                            currect_Target = r_Target4;
                        }
                    }
                    if (nextTargetSelector == 4)
                    {

                        if (currect_Target.gameObject.name == r_Target5.gameObject.name)
                        {
                            r_Target6.SetActive(true);
                            currect_Target = r_Target6;
                        }
                        else
                        {
                            r_Target5.SetActive(true);
                            currect_Target = r_Target5;
                        }
                    }
                    if (nextTargetSelector == 5)
                    {

                        if (currect_Target.gameObject.name == r_Target6.gameObject.name)
                        {
                            r_Target1.SetActive(true);
                            currect_Target = r_Target1;
                        }
                        else
                        {
                            r_Target6.SetActive(true);
                            currect_Target = r_Target6;
                        }
                    }

                }
                else if (!currect_Target.gameObject.name.Contains(tagged))
                {
                  //Do Nothing
                }
                selectNextTarget();
            }
        }
    }
}
