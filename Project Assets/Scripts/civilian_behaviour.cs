using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class civilian_behaviour : MonoBehaviour
{
    Animator anim;
    // Start is called before the first frame update
    bool panicMode = false;
    string[] initialAnim = { "walking", "idle" };
    int initialAnimation = 0;
    float SpeedWalk = 1f;
    float SpeedPanic = 3f;
    float alive = 1;
    float distance = 0f;
    Transform mainPlayer;
    float safety_distance = 3f;
    private Rigidbody rigidBd;
    int index = 0;
    int evadeIndex = 0;
    bool dying;
    float timer = 0;
    GameObject[] walkables;
    GameObject[] evadePoints;
    public int viewPoint = 0;
    [SerializeField] private GameObject gun;
    public float evadePointDistance = 0;
    bool evadePointReached = false;
    float hideTimout = 5;
    string activeScene;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        InitialAnimation();
        mainPlayer = GameObject.Find("Player").GetComponent<Transform>();
        transform.tag = "civilian";
        rigidBd = GetComponent<Rigidbody>();
        walkables = GameObject.FindGameObjectsWithTag("walk");
        evadePoints = GameObject.FindGameObjectsWithTag("evadePoint");
        index = Random.Range(0, walkables.Length);
        evadeIndex = Random.Range(0, evadePoints.Length);
        viewPoint = Random.Range(1, 5);
        gun = GameObject.FindGameObjectWithTag("Gun");
        activeScene = SceneManager.GetActiveScene().name;

    }
    // Update is called once per frame
    void Update()
    {
        animate();
        if (alive <= 0)
        {
            rigidBd.constraints = RigidbodyConstraints.FreezePosition;
            rigidBd.constraints = RigidbodyConstraints.FreezeRotation;
            rigidBd.useGravity = true;

        }
        //gun.SetActive(false);
        evadePointDistance = Vector3.Distance(this.gameObject.transform.position, evadePoints[evadeIndex].transform.position);
    }
    private void animate()
    {

        if (!panicMode && !dying)
        {
            
            if(activeScene == "HumanTargetPopup")
            {
                anim.Play("idle");
            }
            else
            {
                anim.Play("walking");
                Move();
            }
        }
        else if (panicMode && !dying)
        {
            if (viewPoint == 1)
            {
                if (evadePointDistance < safety_distance)
                {
                    evadePointReached = true;
                    anim.Play("yelling");
                    SpeedWalk = 0f;
                    SpeedPanic = 0f;
                    View();

                    hideTimout -= Time.deltaTime * 1;
                    if (hideTimout <= 0)
                    {
                        Destroy(this.gameObject);
                    }

                }
                else
                {
                    EvadeView(evadeIndex);
                }

            }
            if (viewPoint == 2)
            {
                //Vector3.Distance(this.gameObject.transform.position, mainPlayer.position);
                if (evadePointDistance < safety_distance)
                {
                    evadePointReached = true;
                    anim.Play("yelling");
                    SpeedWalk = 0f;
                    SpeedPanic = 0f;
                    View();

                    hideTimout -= Time.deltaTime * 1;
                    if (hideTimout <= 0)
                    {
                        Destroy(this.gameObject);
                    }

                }
                else
                {
                    EvadeView(evadeIndex);
                }
            }
            if (viewPoint == 3)
            {
                View();

            }
            if (viewPoint == 4)
            {

                if (evadePointDistance < safety_distance)
                {
                    evadePointReached = true;
                    anim.Play("yelling");
                    SpeedWalk = 0f;
                    SpeedPanic = 0f;
                    View();

                    hideTimout -= Time.deltaTime * 1;
                    if (hideTimout <= 0)
                    {
                        Destroy(this.gameObject);
                    }

                }
                else
                {
                    EvadeView(evadeIndex);
                }
            }
            if (viewPoint == 5)
            {
                //Vector3.Distance(this.gameObject.transform.position, mainPlayer.position);
                if (evadePointDistance < safety_distance)
                {
                    evadePointReached = true;
                    anim.Play("yelling");
                    SpeedWalk = 0f;
                    SpeedPanic = 0f;
                    View();

                    hideTimout -= Time.deltaTime * 1;
                    if (hideTimout <= 0)
                    {
                        Destroy(this.gameObject);
                    }

                }
                else
                {
                    EvadeView(evadeIndex);
                }
            }

            if (!evadePointReached)
                anim.Play("running");


            transform.position += transform.forward * Time.deltaTime * SpeedPanic;

        }
        else
        {
            timer += Time.deltaTime;
            if (timer > 30)
            {
                Destroy(this.gameObject);
            }
        }


        if (activeScene == "HumanTargetPopup")
        {
            //Do Nothing
        }
        else
        {
            GetDistance();
            if (distance < safety_distance)
            {
                panicMode = false;
                Destroy(this.gameObject);
            }
        }

    }//end of void animate()
    private void View()
    {
        Vector3 LookDir = mainPlayer.position - this.gameObject.transform.position;
        LookDir.y = 0;
        transform.LookAt(this.gameObject.transform.position + LookDir, Vector3.up);
    }
    private void GetDistance()
    {
        distance = Vector3.Distance(this.gameObject.transform.position, mainPlayer.position);
    }
    private void ApplyDamage(string tagged)
    {
        if (tagged == "panic" && !panicMode)
        {
            if (alive > 0)
            {
                anim.Play("panic");
                transform.position += transform.forward * Time.deltaTime * SpeedPanic;
                panicMode = true;
            }
        }


        if (tagged == "shot")
        {
            if (alive > 0)
            {
                alive--;
            }
            if (alive <= 0)
            {
                anim.Play("dying");

                dying = true;
                panicMode = false;
            }
        }
    }
    private void InitialAnimation()
    {
        initialAnimation = Random.Range(0, 2);
    }
    void civillianView(int elementIndex)
    {
        Vector3 LookDir = walkables[elementIndex].transform.position - this.gameObject.transform.position;
        LookDir.y = 0;
        transform.LookAt(this.gameObject.transform.position + LookDir, Vector3.up);
    }
    void EvadeView(int elementIndex)
    {
        Vector3 LookDir = evadePoints[elementIndex].transform.position - this.gameObject.transform.position;
        LookDir.y = 0;
        transform.LookAt(this.gameObject.transform.position + LookDir, Vector3.up);
    }
    void Move()
    {
        if (Vector3.Distance(this.transform.position, walkables[index].transform.position) < 1)
        {
            //transform.position += transform.forward * Time.deltaTime * SpeedWalk;
            index = Random.Range(0, walkables.Length);
        }
        civillianView(index);
        transform.position += transform.forward * Time.deltaTime * SpeedWalk;
    }
}
