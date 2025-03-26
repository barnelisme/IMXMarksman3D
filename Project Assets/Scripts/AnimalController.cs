using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimalController : MonoBehaviour
{
    public GameObject[] wonderPoints;
    public GameObject[] evadePoint;
    public GameObject[] avoidRunPoints;
    int entrydirection;
    int index = 0;
    int moveSpeed;
    int setWalkSpeed = 2;
    int setOutdoorRunSpeed = 4;
    int setGroundRunSpeed = 3;
    bool isFoodFound = false;
    bool isEvadePointSet = false;
    float eatingTimer = 5f;
    float setEatingTime = 10f;
    public Animator anim;
    int run_walk_Flag = 0;
    bool isRunWalkFlagSet = false;
    public bool isKilled = false;
    float destroyTimer = 5f;
    bool hide = false;
    bool look = false;
    float resetGunTimer = .5f;
    float idleTimer = 4f;
    int evadeFlag = 0;
    int killedFlag = 0;
    int deerLive = 2;
    string activeScene = "";
    bool isHit = false;
    float hitTimer = 0.1f;
    public bool animal_detected = false;
    public GameObject animalDetor;
    public GameObject headCollider;

    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;

        wonderPoints = GameObject.FindGameObjectsWithTag("deerFood");
        evadePoint = GameObject.FindGameObjectsWithTag("deerEvadePoint");

        if(activeScene.ToLower().Contains("avoid"))
        {
            avoidRunPoints = GameObject.FindGameObjectsWithTag("avoidRunPoint");
            index = Random.Range(0, avoidRunPoints.Length - 1);
        }
        else if(activeScene.ToLower().Contains("direct"))
        {
            index = Random.Range(0, wonderPoints.Length - 1);
        }

        entrydirection = Random.Range(1, 4);
        anim = this.GetComponent<Animator>();
        run_walk_Flag = Random.Range(1, 3);

    }

    // Update is called once per frame
    void Update()
    {
        checkSurrounding();
        moveDeer();
        manageGunShotVariable();
    }
     void moveDeer()
    {
        if(!isHit)
        {
            if (activeScene.ToLower().Contains("direct"))
            {
                if (Vector3.Distance(this.transform.position, wonderPoints[index].transform.position) < 2.5f)
                {
                    isFoodFound = true;
                }
                if (!isKilled && !hide && !look)
                {
                    if (animal_detected)
                    {
                        moveSpeed = 0;
                        animateEat();
                    }
                    else
                    {
                        searchForFood();
                    }
                }

                if (StaticVariableManager.isGunFired == true)
                {
                    if (evadeFlag == 0)
                    {
                        evadeFlag = Random.Range(1, 4);
                    }

                    switch (evadeFlag)
                    {
                        case 1:
                            hide = true;
                            break;
                        case 2:
                            hide = true;
                            break;
                        case 3:
                            //look = true;
                            hide = true;
                            break;
                        case 4:
                            hide = true;
                            break;

                    }

                    resetGunTimer -= Time.deltaTime * 1;
                    if (resetGunTimer <= 0f)
                    {
                        StaticVariableManager.isGunFired = false;
                        evadeFlag = 0;
                        resetGunTimer = 1f;
                    }
                }

            }
            else if (activeScene.ToLower().Contains("avoid"))
            {

                if (Vector3.Distance(this.transform.position, avoidRunPoints[index].transform.position) < 2.5f)
                {
                    isFoodFound = true;
                }
                if (!isKilled && !hide && !look)
                {
                    if (animal_detected)
                    {
                        moveSpeed = 0;
                        animateEat();
                    }
                    else
                    {
                        runToAvoidPoint();
                    }
                }

                if (StaticVariableManager.isGunFired == true)
                {
                    if (evadeFlag == 0)
                    {
                        evadeFlag = Random.Range(1, 4);
                    }

                    switch (evadeFlag)
                    {
                        case 1:
                            hide = true;
                            break;
                        case 2:
                            hide = true;
                            break;
                        case 3:
                            //look = true;
                            hide = true;
                            break;
                        case 4:
                            hide = true;
                            break;

                    }

                    resetGunTimer -= Time.deltaTime * 1;
                    if (resetGunTimer <= 0f)
                    {
                        StaticVariableManager.isGunFired = false;
                        evadeFlag = 0;
                        resetGunTimer = 1f;
                    }
                }

            }
        }
        else
        {
            if(!isKilled && !hide && !look)
            {
                moveSpeed = 0;
                animateIdle();

                hitTimer -= Time.deltaTime * 1;
                if(hitTimer <= 0f)
                {
                    setGroundRunSpeed += 1;
                    isHit = false;
                }
            }
        }
        
        if(isKilled)
        {
            die();
        }
        else if(hide)
        {
            evadeToSafety();
        }
        else if (look)
        {
            goToIdle();
        }

    }

    private void checkSurrounding()
    {
        if(animalDetor.GetComponent<RaycastDetect>().isObjectClose)
        {
            animal_detected = true;
        }
        else
        {
            animal_detected = false;
        }

    }

    void lookAtFood(int elementIndex)
    {
        Vector3 LookDir = wonderPoints[elementIndex].transform.position - this.gameObject.transform.position;
        LookDir.y = 0;
        transform.LookAt(this.gameObject.transform.position + LookDir, Vector3.up);
    }
    void lookAtAvoidPoint(int elementIndex)
    {
        Vector3 LookDir = avoidRunPoints[elementIndex].transform.position - this.gameObject.transform.position;
        LookDir.y = 0;
        transform.LookAt(this.gameObject.transform.position + LookDir, Vector3.up);
    }
    void lookAtHideSpot(int elementIndex)
    {
        Vector3 LookDir = evadePoint[elementIndex].transform.position - this.gameObject.transform.position;
        LookDir.y = 0;
        transform.LookAt(this.gameObject.transform.position + LookDir, Vector3.up);
    }

    private void ApplyDamage(string tagged)
    {

        if (tagged.ToLower().Contains("deer"))
        {
            deerLive--;
            isHit = true;
            if (deerLive <= 0)
            {
                isKilled = true;
            }
        }

    }

    void searchForFood()
    {
        if (isFoodFound)
        {
            moveSpeed = 0;
            eatingTimer -= Time.deltaTime * 1;
            animateEat();
            if (eatingTimer <= 0f)
            {
                if (index < wonderPoints.Length)
                {
                    //index++;
                    index = Random.Range(0, wonderPoints.Length - 1);
                }
                else
                {
                    index = 0;
                }

                if(!isRunWalkFlagSet)
                {
                    run_walk_Flag = Random.Range(1, 4);
                    isRunWalkFlagSet = true;
                }

                isFoodFound = false;
                eatingTimer = setEatingTime;
            }
        }
        else
        {
            if(this.gameObject.transform.name.ToLower().Contains("deer"))
            {
                switch (run_walk_Flag)
                {
                    case 1:
                        animateWalk();
                        moveSpeed = setWalkSpeed;
                        break;
                    case 2:
                        animateRun();
                        moveSpeed = setOutdoorRunSpeed;
                        break;
                    case 3:
                        animateWalk();
                        moveSpeed = setWalkSpeed;
                        break;
                    case 4:
                        animateRun();
                        moveSpeed = setOutdoorRunSpeed;
                        break;

                }
            }
            else
            {
                animateWalk();
                moveSpeed = setWalkSpeed;
            }

            isRunWalkFlagSet = false;
            lookAtFood(index);
            transform.position += transform.forward * Time.deltaTime * (moveSpeed);
        }
    }
    void runToAvoidPoint()
    {
        if (isFoodFound)
        {
            moveSpeed = 0;
            eatingTimer -= Time.deltaTime * 1;
            animateEat();
            if (eatingTimer <= 0f)
            {
                if (index < avoidRunPoints.Length - 1)
                {
                    //index++;
                    index = Random.Range(0, avoidRunPoints.Length - 1);
                }
                else
                {
                    index = 0;
                }
                run_walk_Flag = Random.Range(1, 3);
                isFoodFound = false;
                eatingTimer = 1f;
            }
        }
        else
        {
            animateRun();
            if (DropDown.softwareSceneName.ToLower().Contains("outdoor"))
            {
                moveSpeed = setOutdoorRunSpeed;
            }
            if (DropDown.softwareSceneName.ToLower().Contains("ground"))
            {
                if(this.gameObject.transform.name.ToLower().Contains("goose"))
                {
                    moveSpeed = setGroundRunSpeed - 1;
                }
                else
                {
                    moveSpeed = setGroundRunSpeed;
                }
            }

            eatingTimer = 1f;
            lookAtAvoidPoint(index);
            transform.position += transform.forward * Time.deltaTime * (moveSpeed);
        }
    }
    void die()
    {
        moveSpeed = 0;
        animateDeath();
        gameObject.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        gameObject.transform.GetComponent<Rigidbody>().useGravity = false;
        gameObject.transform.GetComponent<BoxCollider>().enabled = false;
        gameObject.transform.GetComponent<CapsuleCollider>().enabled = false;
        bool isHeadShot = headCollider.GetComponent<AnimalHeadReference>().isHeadShot;

        if (killedFlag == 0 && !isHeadShot)
        {
            if (activeScene.ToLower().Contains("direct"))
            {
                killedFlag = 1;
                StaticVariableManager.totalTargetAnimalsKilled += 1;
            }
            if (activeScene.ToLower().Contains("avoid"))
            {
                if(this.gameObject.transform.name.ToLower().Contains(TestConditionsManager.animalName.ToLower()))
                {
                    killedFlag = 1;
                    StaticVariableManager.totalTargetAnimalsKilled += 1;
                }
                else
                {
                    killedFlag = 1;
                    StaticVariableManager.totalTargetCasualtiesKilled += 1;
                }
            }

            StaticVariableManager.totalBodyShots += 1;
            print("Body shots is : " + StaticVariableManager.totalBodyShots); 
        }

        destroyTimer -= Time.deltaTime * 1;
        if (destroyTimer <= 0f)
        {
            //Destroy(this.gameObject);
            //Destroy(headCollider.gameObject);
        }
    }

    private void animateRun()
    {
        //Outdoor
        if (this.gameObject.transform.name.ToLower().Contains("deer"))
        {
            anim.Play("Deer_run");
        }
        if (this.gameObject.transform.name.ToLower().Contains("leopard"))
        {
            anim.Play("Leopard_run");
        }
        if (this.gameObject.transform.name.ToLower().Contains("boar"))
        {
            anim.Play("WildBoar_run");
        }
        if (this.gameObject.transform.name.ToLower().Contains("buffalo"))
        {
            anim.Play("Cape_buffalo_run");
        }

        //Ground
        if (this.gameObject.transform.name.ToLower().Contains("rabbit"))
        {
            anim.Play("WildRabbit_run");
        }
        if (this.gameObject.transform.name.ToLower().Contains("goose"))
        {
            anim.Play("Swan_goose_run");
        }
        if (this.gameObject.transform.name.ToLower().Contains("dragon"))
        {
            anim.Play("Comodo_dragon_walk");
            //Comodo_dragon_walk_slow
        }
        if (this.gameObject.transform.name.ToLower().Contains("pig"))
        {
            anim.Play("Domestic_pig_run");
        }

        //Mud_pig_die
    }
    private void animateWalk()
    {
        
        if (this.gameObject.transform.name.ToLower().Contains("deer"))
        {
            anim.Play("Deer_walk");
        }
        if (this.gameObject.transform.name.ToLower().Contains("leopard"))
        {
            anim.Play("Leopard_walk");
        }
        if (this.gameObject.transform.name.ToLower().Contains("boar"))
        {
            anim.Play("WildBoar_walk");
        }
        if (this.gameObject.transform.name.ToLower().Contains("buffalo"))
        {
            anim.Play("Cape_buffalo_walk");
        }

        //Ground
        if (this.gameObject.transform.name.ToLower().Contains("rabbit"))
        {
            anim.Play("WildRabbit_walk");
        }
        if (this.gameObject.transform.name.ToLower().Contains("goose"))
        {
            anim.Play("Swan_goose_walk");
        }
        if (this.gameObject.transform.name.ToLower().Contains("dragon"))
        {
            anim.Play("Comodo_dragon_walk_slow");
        }
        if (this.gameObject.transform.name.ToLower().Contains("pig"))
        {
            anim.Play("Domestic_pig_walk");
        }

    }
    private void animateEat()
    {
        if(this.gameObject.transform.name.ToLower().Contains("deer"))
        {
            anim.Play("Deer_eat");
        }
        if (this.gameObject.transform.name.ToLower().Contains("leopard"))
        {
            anim.Play("Leopard_eat");
        }
        if (this.gameObject.transform.name.ToLower().Contains("boar"))
        {
            anim.Play("WildBoar_eat");
        }
        if (this.gameObject.transform.name.ToLower().Contains("buffalo"))
        {
            anim.Play("Cape_buffalo_eat");
        }

        //Ground
        if (this.gameObject.transform.name.ToLower().Contains("rabbit"))
        {
            anim.Play("WildRabbit_eat");
        }
        if (this.gameObject.transform.name.ToLower().Contains("goose"))
        {
            anim.Play("Swan_goose_eat");
        }
        if (this.gameObject.transform.name.ToLower().Contains("dragon"))
        {
            anim.Play("Comodo_dragon_eat");
        }
        if (this.gameObject.transform.name.ToLower().Contains("pig"))
        {
            anim.Play("Domestic_pig_eat");
        }
    }
    private void animateIdle()
    {
        
        if (this.gameObject.transform.name.ToLower().Contains("deer"))
        {
            anim.Play("Deer_idle");
        }
        if (this.gameObject.transform.name.ToLower().Contains("leopard"))
        {
            anim.Play("Leopard_idle");
        }
        if (this.gameObject.transform.name.ToLower().Contains("boar"))
        {
            anim.Play("WildBoar_idle");
        }
        if (this.gameObject.transform.name.ToLower().Contains("buffalo"))
        {
            anim.Play("Cape_buffalo_idle");
        }

        //Ground
        if (this.gameObject.transform.name.ToLower().Contains("rabbit"))
        {
            anim.Play("WildRabbit_idle");
        }
        if (this.gameObject.transform.name.ToLower().Contains("goose"))
        {
            anim.Play("Swan_goose_idle");
        }
        if (this.gameObject.transform.name.ToLower().Contains("dragon"))
        {
            anim.Play("Comodo_dragon_idle");
        }
        if (this.gameObject.transform.name.ToLower().Contains("pig"))
        {
            anim.Play("Domestic_pig_idle");
        }
    }
    private void animateDeath()
    {
        if (this.gameObject.transform.name.ToLower().Contains("deer"))
        {
            anim.Play("Deer_die");
        }
        if (this.gameObject.transform.name.ToLower().Contains("leopard"))
        {
            anim.Play("Leopard_death");
        }
        if (this.gameObject.transform.name.ToLower().Contains("boar"))
        {
            anim.Play("WildBoar_die");
        }
        if (this.gameObject.transform.name.ToLower().Contains("buffalo"))
        {
            anim.Play("Cape_buffalo_die");
        }

        //Ground
        if (this.gameObject.transform.name.ToLower().Contains("rabbit"))
        {
            anim.Play("WildRabbit_die");
        }
        if (this.gameObject.transform.name.ToLower().Contains("goose"))
        {
            anim.Play("Swan_goose_die");
        }
        if (this.gameObject.transform.name.ToLower().Contains("dragon"))
        {
            anim.Play("Comodo_dragon_die");
        }
        if (this.gameObject.transform.name.ToLower().Contains("pig"))
        {
            anim.Play("Domestic_pig_die");
        }

    }

    void evadeToSafety()
    {
        if (!isEvadePointSet)
        {
            index = Random.Range(0, evadePoint.Length - 1);
            isEvadePointSet = true;
        }

        if (Vector3.Distance(this.transform.position, evadePoint[index].transform.position) < 2.5f)
        {
            animateIdle();
            moveSpeed = 0;
            Destroy(headCollider.gameObject);

            destroyTimer -= Time.deltaTime * 1;
            if (destroyTimer <= 0f)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            if(activeScene.ToLower().Contains("outdoor"))
            {
                moveSpeed = setOutdoorRunSpeed + 1;
            }
            else if(activeScene.ToLower().Contains("ground"))
            {
                moveSpeed = setGroundRunSpeed;
            }
            animateRun();
            //anim.speed = 4;
        }


        lookAtHideSpot(index);
        transform.position += transform.forward * Time.deltaTime * (moveSpeed);
    }
    void goToIdle()
    {
        moveSpeed = setOutdoorRunSpeed + 0;
        anim.Play("Deer_idle");

        idleTimer -= Time.deltaTime * 1;
        if (idleTimer <= 0f)
        {
            look = false;
            idleTimer = 5f;
        }

    }

    private void manageGunShotVariable()
    {



    }
}
