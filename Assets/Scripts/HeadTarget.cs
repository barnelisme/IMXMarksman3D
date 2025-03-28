using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HeadTarget : MonoBehaviour
{
    public TextMeshProUGUI targetText;
    bool isTargetTextActive;
    float textActiveTimer;
    float setTime = 1;
    string activeScene = "";

    //color change variables
    bool isChangeColor = false;
    float colorChangeTimer;
    float colorChangeTimerReset = 0.0f;
    Material currentTargetColor;
    private Renderer targetRenderer;
    private Color originalColor;
    float r, g, b, a;

    //Animal Target Shoot
    public GameObject targetController;

    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            originalColor = targetRenderer.material.color; // Save the original color
        }

        Vector3 currentPo = this.gameObject.transform.position;
        currentPo.x = 0;
        currentPo.y = 0;
        currentPo.z = 0;
        if (this.transform.name.ToLower().Contains("cover") && !activeScene.ToLower().Contains("5pointbullseye"))
        {
            currentTargetColor = this.transform.gameObject.GetComponent<Renderer>().material;
            revertColor();
        }
        targetText.enabled = false;
        isTargetTextActive = false;
        setTime = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTargetTextActive)
        {
            targetText.enabled = true;
            textActiveTimer -= Time.deltaTime;
            if (textActiveTimer <= 0f)
            {
                targetText.enabled = false;
                isTargetTextActive = false;
            }
        }
        else
        {
            //targetText.enabled = false;
            textActiveTimer = setTime;
        }

        if(!activeScene.ToLower().Contains("5pointbullseye"))
        {
            if (isChangeColor == true)
            {
                colorChangeTimer -= Time.deltaTime;
                //print("RE: Processing color");
                if (colorChangeTimer <= 0f)
                {
                    revertColor();
                    isChangeColor = false;
                }
            }
            else
            {
                colorChangeTimer = colorChangeTimerReset;
            }
        }

        

    }

    private void ApplyDamage(string tagged)
    {
        //received = false;

        //Debug.Log("I was hit:" + transform.name + " apply Damage sent:" + tagged);

        if (this.transform.tag == tagged)
        {
            //isTargetTextActive = true;
            //Shooting.numBlockesHit++;
            //targetText.text = "HEAD SHOT";
            if (this.transform.name.ToLower().Contains("cover") && !activeScene.ToLower().Contains("5pointbullseye"))
            {
               isChangeColor = true;
            }
            //print("Head is DEADLY SHOT!!!!!!!!");
            StaticVariableManager.resetTargets = true;

            if (activeScene.ToLower().Contains("animaltarget"))
            {
                string points = this.transform.name.Split(" ")[1];
                AnimalTargetPointManager.ManagePoints(points, "head");
            }

        }
        else if (tagged.Contains("change"))
        {
            //seen = true;
            //changeState(states.patrol);
        }


    }

    public void revertColor()
    {
        if (this.transform.name.ToLower().Contains("cover"))
        {
            currentTargetColor.color = originalColor;
            //print("RE: Color Reset");
        }
    }

}
