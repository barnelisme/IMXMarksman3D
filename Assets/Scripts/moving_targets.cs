using System;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.UI;
public class moving_targets : MonoBehaviour
{
    public Toggle moving;
    public GameObject lane1;
    public GameObject lane2;
    public GameObject lane3;
    public GameObject lane4;
    public GameObject redTarget;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI ShotOnTargetText;
    public TextMeshProUGUI ShotMissedText;
    GameObject[] all_targets;
    GameObject target;
    int index;
    int dir = 1;
    bool moveLeft = true;
    float speed = 2f;
    float animtime = 0f;
    int timeA;
    string myHit;
    string pointOnTarget;
    int shotOnTarget = 0;
    int shotMissed = 0;
    
    int calculated;

    public float range = 100f;

    void Start()
    {
        init();

        // myHit = new Transform();
    }
    void Update()
    {
        try
        {

            if (Shooting.startTime > 0)
            {
                if (moving.isOn)
                {
                    Move();
                }
                else
                {
                    Destroy(lane4);
                }
                RandomTarget(timeA);
                //RaycastHit hit;
                myHit = Shooting.targetHitParent;
                pointOnTarget = Shooting.targetHit;
                ChangeTarget();
            }
        }
        catch (Exception ex)
        {
            Scoring.logs += "\n" + ex.Message + ":" + ex.StackTrace;
            Scoring.writeLog("LoadTagets Update:" + ex.StackTrace);
            Debug.LogError("LoadTagets Update:" + ex.StackTrace);
        }
    }
    void Move()
    {
        if ((lane1.transform.position.x <= 624f && !moveLeft) || (lane1.transform.position.x >= 630.16f && moveLeft))
        {
            dir *= -1;
            moveLeft = !moveLeft;
        }
        lane1.transform.position += lane1.transform.right * Time.deltaTime * speed * dir;
        lane3.transform.position += lane3.transform.right * Time.deltaTime * speed * dir;

        lane2.transform.position += lane2.transform.right * Time.deltaTime * speed * (-dir);
        lane4.transform.position += lane4.transform.right * Time.deltaTime * speed * (-dir);
    }
    void RandomTarget(int time)
    {
        all_targets = GameObject.FindGameObjectsWithTag("target");
        if (animtime < time)
        {
            index = Random.Range(0, all_targets.Length);
            animtime += Time.deltaTime;
        }
        redTarget.transform.position = new Vector3(all_targets[index].transform.position.x, redTarget.transform.position.y, all_targets[index].transform.position.z);
        target = all_targets[index];
    }
    void ChangeTarget()
    {
        if (myHit == target.name)
        {
            print(myHit);
            timeA = 1;
            animtime = 0;
            shotOnTarget++;
            Shooting.targetHitParent = null;
            RandomTarget(timeA);
            if (pointOnTarget.Contains("A_"))
            {
                calculated += 5;
            }
            else if (pointOnTarget.Contains("B_"))
            {
                calculated += 4;
            }
            else if (pointOnTarget.Contains("C_"))
            {
                calculated += 2;
            }
            else if (pointOnTarget.Contains("D_"))
            {
                calculated += 1;
            }
            
            score();
        }
        else
        {
            shotMissed++;
            score();
        }
        
        //score();
    }
    public void score()
    {
        //print("shots on target:" + shotOnTarget + "\nshot missed:" + shotMissed);
        
        Shooting.targetHit = null;
        //display the scores on the score canva
        ShotMissedText.text = "Shot Missed\t\t:"+shotMissed.ToString();
        ShotOnTargetText.text = "Shot on target\t:" +shotOnTarget.ToString();
        scoreText.text = "Score\t\t\t:" +calculated.ToString();
        print("score:" + calculated);
    }
    public void init()
    {
        timeA = 6;
        shotMissed = 0;
        shotOnTarget = 0;
        calculated = 0;
        
    }
}
