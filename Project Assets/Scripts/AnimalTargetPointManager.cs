using UnityEngine;
using TMPro;

public class AnimalTargetPointManager : MonoBehaviour
{
    public TextMeshProUGUI pointDisplay;
    static string currPoint = "";
    public static int pointsCounter = 0;
    private static bool displayPoint = false;
    private static float displayCounter = 1;
    private static string hitPart = "";

    public float prepTimer = 20;
    public static float setPrepTime = 20;
    private bool isPrepTime = false;

    void Start()
    {
        pointDisplay.enabled = false;

        //Static variables reset
        displayCounter = 1;
        pointsCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (displayPoint)
        {
            pointDisplay.enabled = true;
            WriteText(currPoint, hitPart);
            displayCounter -= Time.deltaTime * 1;

            if (displayCounter <= 0f)
            {
                //pointDisplay.enabled = false;
                displayPoint = false;
                displayCounter = 1f;
                isPrepTime = true;
            }

        }

        if(isPrepTime)
        {
            prepTimer -= Time.deltaTime;
            WriteText(prepTimer.ToString("0"), "prep time");

            if(prepTimer < 1)
            {
                pointDisplay.enabled = false;
                isPrepTime = false;
                prepTimer = setPrepTime;
            }
        }
        
        
        if(prepTimer != StaticVariableManager.prepTime)
        {
            setPrepTime = StaticVariableManager.prepTime;
            
            if(!isPrepTime)
            {
                prepTimer = setPrepTime;
            }
        }

    }

    private void WriteText(string text, string hit_part)
    {
        switch(hit_part)
        {
            case "body":
                pointDisplay.text = text;// + " body points.";
                break;

            case "head":
                pointDisplay.text = text;// + " head points.";
                break;
            case "prep time":
                pointDisplay.text = "Prepare: " + text;
                break;
        }
    }

    public static void ManagePoints(string point, string hit_part)
    {
        char constructedPoint = point[1];
        displayPoint = true;
        currPoint = point;
        displayCounter = 1f;
        hitPart = hit_part;

        if (point.Contains("+"))
        {
            pointsCounter +=  int.Parse(constructedPoint.ToString());
        }
        else if (point.Contains("-"))
        {
            pointsCounter -= int.Parse(constructedPoint.ToString());
        }

        WallTargetControl.requestMove();
        //print("Called.. " + pointsCounter);
    }
}
