using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TargetScaleUIManager : MonoBehaviour
{

    public List<GameObject> targets = new List<GameObject>();

    public Button controlButton; // Assign the button in the Inspector
    public GameObject buttonHolder;
    private Color greenColor = Color.green;
    private Color redColor = Color.black;
    private bool activeStatus = true;
    private string activeScene = "";
    private Image buttonImage;

    private void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        buttonImage = controlButton.GetComponent<Image>();

        if (activeScene.ToLower().Contains("risingplate") || activeScene.ToLower().Contains("cyclic") 
            || activeScene.ToLower().Contains("sequence") || activeScene.ToLower().Contains("ipec") 
            || activeScene.ToLower().Contains("distancesimulator") || activeScene.ToLower().Contains("hiddentarget")
            || activeScene.ToLower().Contains("diceflipping") || activeScene.ToLower().Contains("fallingplat")
            || activeScene.ToLower().Contains("duelingtree"))
        {
            //print("Switching On");
            //controlButton.gameObject.SetActive(true);
            buttonHolder.SetActive(true);
            buttonImage.color = redColor;
            activeStatus = false;
            foreach (GameObject target in targets)
            {
                target.GetComponent<movePlayer>().enabled = false;
            }
        }
        controlButton.onClick.AddListener(ChangeButtonColor);
    }

    private void Update()
    {
        if(StaticVariableManager.isStopTraining == true)
        {
            transform.gameObject.SetActive(false);
        }
    }

    void ChangeButtonColor()
    {
        
        switch (activeStatus)
        {
            case true:
                //print("Deactivating...");
                activeStatus = false;
                buttonImage.color = redColor;
                foreach(GameObject target in targets)
                {
                    target.GetComponent<movePlayer>().enabled = false;
                }
                
                break;
            case false:
                //print("Activating...");
                activeStatus = true;
                buttonImage.color = greenColor;
                foreach (GameObject target in targets)
                {
                    target.GetComponent<movePlayer>().enabled = true;
                }

                break;

        }
    }

    public void scaleUp()
    {
        //StaticVariableManager.activateLane1ScaleUp = true;
        //StaticVariableManager.activateLane2ScaleUp = true;
        //StaticVariableManager.activateLane3ScaleUp = true;

        foreach(GameObject target in targets)
        {
            //target.GetComponent<TargetScaleManager>().DecreaseDistance();
            target.GetComponent<TargetScaleManager_V2>().DecreaseDistance();
        }

    }

    public void scaleDown()
    {
        //StaticVariableManager.activateLane1ScaleDown = true;
        //StaticVariableManager.activateLane2ScaleDown = true;
        //StaticVariableManager.activateLane3ScaleDown = true;

        foreach (GameObject target in targets)
        {
            //target.GetComponent<TargetScaleManager>().IncreaseDistance();
            target.GetComponent<TargetScaleManager_V2>().IncreaseDistance();
        }
    }

    public void setDefault()
    {
        //StaticVariableManager.activateLane1ScaleDefault = true;
        //StaticVariableManager.activateLane2ScaleDefault = true;
        //StaticVariableManager.activateLane3ScaleDefault = true;

        foreach (GameObject target in targets)
        {
            target.GetComponent<TargetScaleManager_V2>().LoadDefault();
        }

    }

}
