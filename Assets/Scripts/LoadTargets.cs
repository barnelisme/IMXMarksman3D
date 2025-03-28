using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.InputSystem;
using TMPro;

public class LoadTargets : MonoBehaviour
{
    Vector3 positionIndoorTarget1 = new Vector3(619.83f, 1.69f, 552.2f);
    Vector3 positionIndoorTarget2 = new Vector3(623.479f, 1.69f, 552.2f);
    Vector3 positionIndoorTarget3 = new Vector3(626.7042f, 1.69f, 552.2f);

    Vector3 bottleM = new Vector3(502.51001f, 1.25999999f, 333.22699f);
    GameObject[] all_targets;
    public GameObject White_DD5_target_1;
    public GameObject White_DD5_target_2;
    public GameObject White_DD5_target_3;

    public GameObject A4_target_1;
    public GameObject A4_target_2;
    public GameObject A4_target_3;

    public GameObject DD5_target_1;
    public GameObject DD5_target_2;
    public GameObject DD5_target_3;

    public TextMeshProUGUI distanceText;

    private MouseLook[] mous;
    private FirstPersonController[] fps;

    int index = 0;
    int distance;
    string activeScene = " ";

    [Header("Training Targets")]
    public GameObject target1;
    public GameObject target2;
    public GameObject target3;

    List<string> targetNames = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        targetNames.Add("White_DD5_target");
        targetNames.Add("A4_target");
        targetNames.Add("DD5_target");
        //mous = GetComponentsInChildren<MouseLook>();
        //fps = GetComponentsInChildren<FirstPersonController>();
        //foreach (FirstPersonController m in fps)
        //{
        //m.= false;

        //}

        activeScene = SceneManager.GetActiveScene().name.ToLower();

        if(activeScene.ToLower().Contains("range"))
        {
            //positionIndoorTarget1 = target1.gameObject.transform.position;
            //positionIndoorTarget2 = target2.gameObject.transform.position;
            //positionIndoorTarget3 = target3.gameObject.transform.position;
        }

    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.escapeKey.isPressed)
            {
                SceneManager.LoadScene("MainMenu");
            }
            if (SceneManager.GetActiveScene().name.ToLower().Contains("range") && !SceneManager.GetActiveScene().name.ToLower().Contains("moving") && !SceneManager.GetActiveScene().name.ToLower().Contains("theory") && !SceneManager.GetActiveScene().name.ToLower().Contains("bottle"))
            {
                if (Keyboard.current.tKey.isPressed)//change target
                {
                    Thread.Sleep(500);
                    Scoring.ResetRange();
                    all_targets = GameObject.FindGameObjectsWithTag("target");
                    //Debug.Log("Size of Civilians:" + all_civilians.Length.ToString());

                    foreach (GameObject target in all_targets)
                    {
                        Destroy(target);
                    }
                    GameObject[] bulletholes = GameObject.FindGameObjectsWithTag("bullethole");
                    foreach (GameObject bholes in bulletholes)
                    {
                        Destroy(bholes);
                    }

                    if (targetNames[index] == "White_DD5_target")
                    {

                        GameObject obj;

                        if (activeScene.Contains("1"))
                        {
                            obj = Instantiate(White_DD5_target_1, positionIndoorTarget1, Quaternion.identity);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                        }
                        if (activeScene.Contains("2"))
                        {
                            obj = Instantiate(White_DD5_target_1, positionIndoorTarget1, Quaternion.identity);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 180);

                            obj = Instantiate(White_DD5_target_2, positionIndoorTarget2, Quaternion.identity);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                        }
                        if (activeScene.Contains("3"))
                        {
                            obj = Instantiate(White_DD5_target_1, positionIndoorTarget1, Quaternion.identity);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 180);

                            obj = Instantiate(White_DD5_target_2, positionIndoorTarget2, Quaternion.identity);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 180);

                            obj = Instantiate(White_DD5_target_3, positionIndoorTarget3, Quaternion.identity);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                        }
                    }
                    else if (targetNames[index] == "A4_target")
                    {
                        GameObject obj;

                        Vector3 tempPosition_1 = positionIndoorTarget1 + new Vector3(1.7f, 0f, 0f);
                        Vector3 tempPosition_2 = positionIndoorTarget2 + new Vector3(1.7f, 0f,0f);
                        Vector3 tempPosition_3 = positionIndoorTarget3 + new Vector3(1.7f, 0f,0f);

                        if (activeScene.Contains("1"))
                        {
                            
                            obj = Instantiate(A4_target_1, tempPosition_1, Quaternion.identity);
                            //obj.transform.rotation = Quaternion.Euler(0, 0, 180);

                        }
                        if (activeScene.Contains("2"))
                        {
                            obj = Instantiate(A4_target_1, tempPosition_1, Quaternion.identity);
                            //obj.transform.rotation = Quaternion.Euler(0, 0, 180);

                            obj = Instantiate(A4_target_2, tempPosition_2, Quaternion.identity);
                            //obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                        }
                        if (activeScene.Contains("3"))
                        {
                            
                            obj = Instantiate(A4_target_1, tempPosition_1, Quaternion.identity);
                            //obj.transform.rotation = Quaternion.Euler(0, 0, 180);

                            obj = Instantiate(A4_target_2, tempPosition_2, Quaternion.identity);
                            //obj.transform.rotation = Quaternion.Euler(0, 0, 180);

                            obj = Instantiate(A4_target_3, tempPosition_3, Quaternion.identity);
                            //obj.transform.rotation = Quaternion.Euler(0, 0, 180);

                        }

                    }
                    else if (targetNames[index] == "DD5_target")
                    {
                        GameObject obj;

                        if (activeScene.Contains("1"))
                        {
                            obj = Instantiate(DD5_target_1, positionIndoorTarget1, Quaternion.identity);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                        }
                        if (activeScene.Contains("2"))
                        {
                            obj = Instantiate(DD5_target_1, positionIndoorTarget1, Quaternion.identity);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 180);

                            obj = Instantiate(DD5_target_2, positionIndoorTarget2, Quaternion.identity);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                        }
                        if (activeScene.Contains("3"))
                        {
                            obj = Instantiate(DD5_target_1, positionIndoorTarget1, Quaternion.identity);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 180);

                            obj = Instantiate(DD5_target_2, positionIndoorTarget2, Quaternion.identity);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 180);

                            obj = Instantiate(DD5_target_3, positionIndoorTarget3, Quaternion.identity);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                        }

                    }
                    if (index < targetNames.Count - 1)
                    {
                        index++;
                    }
                    else
                    {
                        index = 0;
                    }
                }
                distance = (int)Vector3.Distance(transform.position, positionIndoorTarget2);

                //distanceText.text = "Distance: " + (distance).ToString();


                if (Keyboard.current.upArrowKey.isPressed)
                {
                    Thread.Sleep(100);
                    if (distance == 5)
                    {
                        return;
                    }

                    else if (distance != 50)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (distance / 2));
                    }
                    else if (distance == 50)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 30);
                    }

                }


                if (Keyboard.current.downArrowKey.isPressed)
                {
                    Thread.Sleep(100);
                    if (distance == 100)
                    {
                        return;
                    }

                    else if (distance != 20)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - distance);
                    }
                    else if (distance == 20)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 30);
                    }

                }

            }
            if (SceneManager.GetActiveScene().name.ToLower().Contains("bottle"))
            {
                distance = (int)Vector3.Distance(transform.position, bottleM);

                distanceText.text = "Distance: " + (distance).ToString();


                if (Keyboard.current.upArrowKey.isPressed)
                {
                    Thread.Sleep(100);
                    if (distance == 5)
                    {
                        return;
                    }

                    else if (distance != 50)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (distance / 2));
                    }
                    else if (distance == 50)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 30);
                    }

                }


                if (Keyboard.current.downArrowKey.isPressed)
                {
                    Thread.Sleep(100);
                    if (distance == 100)
                    {
                        return;
                    }

                    else if (distance != 20)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - distance);
                    }
                    else if (distance == 20)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 30);
                    }
                }

            }
        }
        catch (Exception ex)
        {
            Scoring.logs += "\n" + ex.Message + ":" + ex.StackTrace;
            Scoring.writeLog("LoadTagets Update:" + ex.StackTrace);
            Debug.LogError("LoadTagets Update:" + ex.StackTrace);
        }

    }
}
