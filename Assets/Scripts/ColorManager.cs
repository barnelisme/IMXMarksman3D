using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ColorManager : MonoBehaviour
{
    [Header("Color Variables")]
    public TextMeshProUGUI currentPlateColor;
    public TextMeshProUGUI currentBackroundColor;

    static bool runChange = false;
    float timer = 0.2f;
    static Renderer currentTargetColor;

    [SerializeField]
    GameObject ipec_head;
    [SerializeField]
    GameObject ipec_body;
    Material objectMaterial;
    Material objectMateria2;
    Material objectMateria3;
    // Load the color of the material
    static float r, g, b ,a;
    static float ipec_setTimerValue = .18f;
    float bodyHitTimer = ipec_setTimerValue;
    public static bool body_head_ishit = false;
    string activeScene = "";

    private void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;

        if(activeScene.ToLower().Contains("ipec"))
        {
            LoadTargetStartInfo();
            print("RE: Data loaded");
        }

    }
    void Update()
    {
        if (runChange == true)
        {
            timer -= Time.deltaTime * 1;
            if (timer <= 0f)
            {
                //revertTargetColor();
                //print("RE: Color Reset");
                runChange = false;
            }

        }
        else
        {
            timer = 0.2f;
        }
    }
    void LoadTargetStartInfo()
    {
        objectMateria2 = ipec_head.GetComponent<Renderer>().material;
        objectMateria3 = ipec_body.GetComponent<Renderer>().material;
    }
    private void revertTargetColor()
    {
        if(activeScene.ToLower().Contains("ipec"))
        {
            objectMateria3.color = new Color(r, g, b, a);
            objectMateria2.color = new Color(r, g, b, a);
            print("RE: Color reset");
        }
        else
        {
            currentTargetColor.material.color = Color.black;
        }
    }
    public static void revert_head_body(RaycastHit hit)
    {
        //print("RE: change request received");
        currentTargetColor = hit.transform.gameObject.GetComponent<Renderer>();
        runChange = true;
    }
    public static void revert_head_body(float rf, float gf, float bf, float af)
    {
        //print("RE: change request received");
        //currentTargetColor = hit.transform.gameObject.GetComponent<Renderer>();
        r = rf;
        g = gf;
        b = bf;
        a = af;

        runChange = true;
    }
    public void setBlackPlate()
    {
        currentPlateColor.text = "Black";
        StaticVariableManager.targetColorSetting = "Black";
    }
    public void setWhitePlate()
    {
        currentPlateColor.text = "White";
        StaticVariableManager.targetColorSetting = "White";
    }
    public void setBluePlate()
    {
        currentPlateColor.text = "Blue";
        StaticVariableManager.targetColorSetting = "Blue";
    }
    public void setRedPlate()
    {
        currentPlateColor.text = "Red";
        StaticVariableManager.targetColorSetting = "Red";
    }
    public void setGreenPlate()
    {
        currentPlateColor.text = "Green";
        StaticVariableManager.targetColorSetting = "Green";
    }
    public void setYellowPlate()
    {
        currentPlateColor.text = "Yellow";
        StaticVariableManager.targetColorSetting = "Yellow";
    }

    public void setBlackBackground()
    {
        currentBackroundColor.text = "Black";
        StaticVariableManager.backgroundColorSetting = "Black";
    }
    public void setWhiteBackground()
    {
        currentBackroundColor.text = "White";
        StaticVariableManager.backgroundColorSetting = "White";
    }
    public void setBlueBackground()
    {
        currentBackroundColor.text = "Blue";
        StaticVariableManager.backgroundColorSetting = "Blue";
    }
    public void setRedBackgrounde()
    {
        currentBackroundColor.text = "Red";
        StaticVariableManager.backgroundColorSetting = "Red";
    }
    public void setGreenBackground()
    {
        currentBackroundColor.text = "Green";
        StaticVariableManager.backgroundColorSetting = "Green";
    }
    public void setYellowBackground()
    {
        currentBackroundColor.text = "Yellow";
        StaticVariableManager.backgroundColorSetting = "Yellow";
    }
}
