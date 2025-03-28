using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TargetBoardColorManager : MonoBehaviour
{
    private float current_r;
    private float current_g;
    private float current_b;
    private float current_a;

    private float current_txt_r;
    private float current_txt_g;
    private float current_txt_b;
    private float current_txt_a;

    private float colorUdjustmentValue = 0;
    private float txt_colorUdjustmentValue = 0;
    private string activeScene = "";

    public List<TextMesh> pointTwoNumbers = new List<TextMesh>();

    // Start is called before the first frame update
    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        updateTargetColor();
        updateTxtColor();


        //current_txt_r = updateTxtColor.r * 255;
        //current_txt_g = updateTxtColor.g * 255;
        //current_txt_b = updateTxtColor.b * 255;
        //current_txt_a = updateTxtColor.a * 255;
        
        //print("Test: Current R " + current_txt_r);
        //print("Test: Current G " + current_txt_g);
        //print("Test: Current B " + current_txt_b);

        //this.GetComponent<Renderer>().material.color = Color.red;
    }

    // Update is called once per frame
    void updateTargetColor()
    {
        //Get Current Color
        current_r = this.GetComponent<Renderer>().material.color.r * 255;
        current_g = this.GetComponent<Renderer>().material.color.g * 255;
        current_b = this.GetComponent<Renderer>().material.color.b * 255;
        current_a = this.GetComponent<Renderer>().material.color.a * 255;

        //Assign adjustment Value
        if (Scoring.ammo_setting.ToLower().Contains("live") || Scoring.ammo_setting.ToLower().Contains("infrared"))
        {
            colorUdjustmentValue = 0;
        }
        else
        {
            if (activeScene.ToLower().Contains("cyclic") || activeScene.ToLower().Contains("dice"))
            {
                colorUdjustmentValue = 80f;
            }
            else if (activeScene.ToLower().Contains("ipecboard"))
            {
                colorUdjustmentValue = 42f;
            }
            else
            {
                colorUdjustmentValue = 155f;
            }
        }

        //update current RGB Value
        current_r = current_r - colorUdjustmentValue;
        current_g = current_g - colorUdjustmentValue;
        current_b = current_b - colorUdjustmentValue;

        //Update target color
        Material updateColor = this.GetComponent<Renderer>().material;
        updateColor.color = new Color(current_r / 255f, current_g / 255f, current_b / 255f, 255 / 255);
        this.GetComponent<Renderer>().material = updateColor;
    }

    void updateTxtColor()
    {
        if (activeScene.ToLower().Contains("cyclic"))
        {
            //get Current text color
            current_txt_r = pointTwoNumbers[0].GetComponent<TextMesh>().color.r * 255;
            current_txt_g = pointTwoNumbers[0].GetComponent<TextMesh>().color.g * 255;
            current_txt_b = pointTwoNumbers[0].GetComponent<TextMesh>().color.b * 255;
            current_txt_a = pointTwoNumbers[0].GetComponent<TextMesh>().color.a * 255;


            //Assign color adjustment value
            if (Scoring.ammo_setting.ToLower().Contains("live") || Scoring.ammo_setting.ToLower().Contains("infrared"))
            {
                txt_colorUdjustmentValue = 50f;
            }
            else
            {
                txt_colorUdjustmentValue = 145f;
            }

            //update current RGB Value

            current_txt_r = current_txt_r - txt_colorUdjustmentValue;
            current_txt_g = current_txt_g - txt_colorUdjustmentValue;
            current_txt_b = current_txt_b - txt_colorUdjustmentValue;


            //Update text color
            Color updateTxtColor = pointTwoNumbers[0].GetComponent<TextMesh>().color;
            updateTxtColor = new Color(current_txt_r / 255f, current_txt_g / 255f, current_txt_b / 255f, 255 / 255);
            pointTwoNumbers[0].GetComponent<TextMesh>().color = updateTxtColor;
            pointTwoNumbers[1].GetComponent<TextMesh>().color = updateTxtColor;
        }
    }
}
