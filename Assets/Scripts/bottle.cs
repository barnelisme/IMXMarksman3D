using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;
using System;
using UnityEngine.SceneManagement;
using System.Threading;

public class bottle : MonoBehaviour
{
    public AudioSource bottleSource;
    public AudioClip bottleAudio;
    AudioSource timerSoundSource;
    public AudioClip timerAudio;
    public AudioClip startAudio;

    GameObject[] bottles;
    public GameObject BottleM;
    public GameObject BottleL;
    public GameObject BottleR;
    public GameObject Panel;

    public GameObject SphereBottleM;
    public GameObject SphereBottleL;
    public GameObject SphereBottleR;

    Vector3 positionM= new Vector3(502.51001f, 1.25999999f, 333.22699f);
    Vector3 positionL = new Vector3(498.031006f, 1.34099996f, 333.153992f);
    Vector3 positionR = new Vector3(507.320007f, 1.31099999f, 333.230011f);

    public TextMeshProUGUI score;
    public TextMeshProUGUI time;
    public TextMeshProUGUI startTime;

    public TextMeshProUGUI shotsMissedText;
    public TextMeshProUGUI ShotsOnTargetText;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI location;
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI Results;
    public TextMeshProUGUI bottlesLeft;
    string[] targetNames= { "bottle","sphere"};

    int shots;
    int index = 0;
    int trackTime;
    int chooser;
    float timelimit;
    float startTimer;
    int numberOfTargets;
    int bottlesHit=0;
    int missed=0;
    static public bool run = false;
    public bool isShot = false;

    // Start is called before the first frame update
    void Start()
    {
        init();
        
    }
    void Update()
    {
        try
        {
            changeTarget();
            startTimer -= Time.deltaTime;
            if((trackTime-(int)startTimer) == 1 && run == false)
            {
                if(startTimer >= 1f)
                {
                    timerSoundSource.clip = timerAudio;
                    timerSoundSource.Play();
                    trackTime = (int)startTimer;
                }
            
            }
            startTime.text = "Standby: " + (int)startTimer;
            if ((int)startTimer <= 0)
            {
                if (timelimit > 0)
                {
                    
                    if(!run)
                    {
                        timerSoundSource.clip = startAudio;
                        timerSoundSource.Play();
                        trackTime = 2;
                    }

                    run = true;
                    startTime.text = "GO";
                    timelimit -= Time.deltaTime;
                    time.text = "Time: " + (int)timelimit;
                    bottles = GameObject.FindGameObjectsWithTag("bottle");
                    shots = bottlesHit + missed;
                    score.text = "Precision : " + bottlesHit.ToString() + "/" + shots;
                    if (bottlesHit == numberOfTargets)
                        timelimit = 0;
                    else if (bottles.Length <= 0)
                        ResetBottles();
                }
                else if(timelimit <= 0)
                {
                    run = false;
                    startTime.text = "STOP";
                    Panel.SetActive(true);
                   
                    location.text = SceneManager.GetActiveScene().name;
                    PlayerName.text = "name: " + GetTrainees.trainee_name;
                    ScoreText.text = "Score: "+(((float)bottlesHit / (float)(numberOfTargets))*100).ToString("0.0");
                    shotsMissedText.text = "Missed: "+missed.ToString();
                    ShotsOnTargetText.text = "On Target: " + bottlesHit.ToString();
                    if (((float)bottlesHit / (float)(numberOfTargets)) *100 < 80)
                        Results.text = "Scenario Incomplete";
                    else
                        Results.text = "Scenario complete";
                }
            }
            else
            {
                
            }

            if(isShot == true)
            {

            }
        }
        catch (Exception e)
        {
            print("exception in bottle: " + e.StackTrace);
        }
    }
    public void ApplyDamage(string tagged)
    {
        bottleSource = gameObject.AddComponent<AudioSource>();
        bottleSource.clip = bottleAudio;
        
        if (tagged=="shot")
        {
            bottleSource.Play();
            bottlesHit++;
            bottlesLeft.text = "Targets to shoot:" + (numberOfTargets - bottlesHit);
            Debug.Log("bottles hit: "+bottlesHit.ToString());
        }
        else
        {
            missed++;
            print("missed "+missed);
        }
                
    }
    private void ResetBottles()
    {
        Thread.Sleep(500);
        bottles = GameObject.FindGameObjectsWithTag("bottle");
        foreach (GameObject target in bottles)
        {
            Destroy(target);
        }
        Thread.Sleep(500);
        if (chooser == 2)
        {
            GameObject obj = Instantiate(BottleM, positionM, Quaternion.identity);
            GameObject obj1 = Instantiate(BottleL, positionL, Quaternion.identity);
            GameObject obj2 = Instantiate(BottleR, positionR, Quaternion.identity);
        }
        else
        {
            Vector3 spherePos = positionM;
            spherePos.y += 0.1f;
            GameObject obj = Instantiate(SphereBottleM, spherePos, Quaternion.identity);
            spherePos = positionL;
            spherePos.y += 0.1f;
            GameObject obj1 = Instantiate(SphereBottleL, spherePos, Quaternion.identity);
            spherePos = positionR;
            spherePos.y += 0.1f;
            GameObject obj2 = Instantiate(SphereBottleR, spherePos, Quaternion.identity);
        }
    }
    private void changeTarget()
    {
        if (Input.GetKey(KeyCode.T))//change target
        {
            Thread.Sleep(500);
            bottles = GameObject.FindGameObjectsWithTag("bottle");
            //Debug.Log("Size of Civilians:" + all_civilians.Length.ToString());

            foreach (GameObject target in bottles)
            {
                Destroy(target);
            }
            if (targetNames[index].Contains("sphere"))
            {
                chooser = 1;
                GameObject obj = Instantiate(SphereBottleM, positionM, Quaternion.identity);
                GameObject obj1 = Instantiate(SphereBottleL, positionL, Quaternion.identity);
                GameObject obj2 = Instantiate(SphereBottleR, positionR, Quaternion.identity);
            }
            else if (targetNames[index] == "bottle")
            {
                chooser = 2;
                GameObject obj = Instantiate(BottleM, positionM, Quaternion.identity);
                obj = Instantiate(BottleL, positionL, Quaternion.identity);
                obj = Instantiate(BottleR, positionR, Quaternion.identity);
            }
            
            if (index < targetNames.Length - 1)
            {
                index++;
            }
            else
            {
                index = 0;
            }
        }
    }
    public void restartBtn()
    {
        init();
    }
    private void init()
    {
        Panel.SetActive(false);
        bottles = GameObject.FindGameObjectsWithTag("bottle");
        numberOfTargets = Random.Range(9, 12);
        timelimit = 20; // 1.5f * numberOfTargets;
        startTimer = 6;
        bottlesHit = 0;
        missed = 0;
        time.text = "Time: " + (int)timelimit;
        bottlesLeft.text = "Targets to shoot:" + (numberOfTargets - bottlesHit);
        bottles = GameObject.FindGameObjectsWithTag("bottle");
        
        timerSoundSource = gameObject.AddComponent<AudioSource>();
        trackTime = 6;
        if (bottles[0].transform.name.ToLower().Contains("sphere"))
            chooser = 1;
        else
            chooser = 2;
        ResetBottles();
    }
}
