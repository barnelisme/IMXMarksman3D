using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Threading;

public class CBehaviour : MonoBehaviour
{

    public TextMeshProUGUI results;
    public GameObject resultsPanel;
    private enum states { wandering,run,dying,phone,gun,surrender,fool};
    private states state;
    Animator anim;
    public NavMeshAgent karen;
    public Transform mainPlayer;
    private float speed = 10f;
    private string current_state;
    string tagged;
    AudioSource karenSound;
    AudioSource gun;
    public AudioClip gunsound;
    int choose=0;

    public Collider col;

    float trackTime=0.00f;
    public bool commanderControl=false;
    bool success=false;
    bool safe = false;
    // Start is called before the first frame update
    void Start()
    {
        col = this.GetComponent<Collider>();
        anim = this.GetComponent<Animator>();
        karen = new NavMeshAgent();
        karenSound = this.GetComponent<AudioSource>();
        gun = gameObject.AddComponent<AudioSource>();
        gun.clip = gunsound;
        resultsPanel.SetActive(false);
        state = states.wandering;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case states.wandering:
                
                anim.Play("yelling1");
                wandering();
                    break;
            case states.run:
                run();
                Results();
                
                break;
            case states.dying:
                dying();
                Results();
                
                break;
            case states.phone:
                KarenView(90f);
                anim.Play("phone");
                break;
            case states.gun:
                KarenView(90f);
                
                anim.Play("takingGunOUt");
                GunSounds(1.23f);
               
                break;
            case states.surrender:
                anim.Play("kneel");
                surrender();
                break;
            case states.fool:
                print("fool");
              
                anim.Play("kneelingShoot");
                GunSounds(0.26f);
                break;
        }
        if(karenSound.isPlaying)
        {
            Vector3 LookDir = mainPlayer.position - this.gameObject.transform.position;
            KarenView(LookDir.z);
        }
        //ApplyDamage(tag);
    }
    private void ApplyDamage(string tag)
    {
        tagged = tag;
        if (state != states.dying)
        {
            karenSound.Stop();
            
            print(tagged);
            if (tag == "panic")
            {
                state = states.run;
            }
            if (tag == "shot")
            {
                state = states.dying;
            }
        }
        if (state==states.gun||state==states.fool)
        {
            success = true;
        }
        else if (state != states.dying)
        {
            success = false;
        }
    }
    void wandering()
    {
       
        if (!commanderControl)
        {
            if (!karenSound.isPlaying&& tagged != "panic")
            {
                choose = Random.Range(1, 5);
                if (choose == 1)
                {
                    state = states.phone;
                }
                else if (choose == 2)
                {
                    state = states.gun;
                }
                else if (choose == 3 || choose == 4)
                {
                    state = states.surrender;
                }
            }
            else
            {

            }

        }
        else
        {

        }
    }
    void run()
    {
        float distance = Vector3.Distance(this.gameObject.transform.position, mainPlayer.position);
        if (distance>10f)
        {
            changeState(states.wandering);
        }
       else 
        {
            Vector3 LookDir = mainPlayer.position + this.gameObject.transform.position;
            LookDir.y = 0;
            transform.LookAt(this.gameObject.transform.position + LookDir, Vector3.up);
            transform.position += transform.forward * Time.deltaTime * speed;
        }
        anim.Play("run");
        resultsPanel.active = true;
        col.isTrigger = true;
    }
    void dying()
    {
        anim.Play("dying");
        resultsPanel.active = true;
        col.isTrigger = true;
        Destroy(this);
    }
    void surrender()
    {
        if (choose == 4)
        {
            trackTime += Time.deltaTime;
            print(trackTime);
            if (trackTime >= 4.28f)
            {
                changeState(states.fool);
            }
        }
        if (tagged=="shot"&&choose==3)
        {
            changeState(states.dying);
        }
    }
    private void changeState(states st)
    {
        state = st;
    }
    void KarenView(float z)
    {
        Vector3 LookDir = mainPlayer.position - this.gameObject.transform.position;
        LookDir.y = 0;
        LookDir.z = z;
        transform.LookAt(this.gameObject.transform.position + LookDir, Vector3.up);
    }
    void GunSounds(float time)
    {
        //trackTime = 0f;
        trackTime += Time.deltaTime;
        print(trackTime);
        if (trackTime >= time)
        {
            //Shooting.handgunSound.Play();
            gun.Play();
        }
    }
    void Results()
    {
        if(success)
        {
            results.text = "Scenario completed";
        }
        else
        {
            results.text = "Scenario failed!!!";
        }
    }
}
