using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;

public class TargetMessageReceive : MonoBehaviour
{
    //Local Variables 
    bool isTargetTextActive;
    float textActiveTimer;
    float setTime = 1;
    float destroyTimer = 5;
    string activeScene = " ";
    int nextTargetSelector;
    public float animTimeOut = 2f;
    float animSetTime = 8f;
    bool targetShot = false;
    bool addSoundSourse = false;

    //Global variables
    public int objectLive = 0;
    public bool isNotActive = false;
    private bool isPlateShot = false;
    public TextMeshProUGUI targetText;
    public GameObject player;
    public Animator anim;

    //Fallinf Plate Variables
    [Header("Falling plates variables")]
    public GameObject next_Target;
    public GameObject currect_Target;
    private Vector3 currentPosition;
    float fallingTimer = 5;
    float fallingSetTimer = 5;
    bool isPlateHit = false;
    private int hitFlag = 0;
    int LIFE = 1;

   //RifflePole SHooting
   [Header("Rifflepole variables")]
    public GameObject n_Target1;
    public GameObject n_Target2;
    public GameObject n_Target3;
    public GameObject targetController;

    //Sound Varibales
    public AudioSource platHitSound;
    public AudioClip platHitAudio;

    //Basi Resetable Scene Variables 
    bool isRandomState = false;

    //Basic IPEC Board Scene
    [SerializeField]
    GameObject ipec_head;
    [SerializeField]
    GameObject ipec_body;
    static Material objectMaterial;
    Material objectMateria2;
    Material objectMateria3;
    // Load the color of the material
    float r;
    float g;
    float b;
    float a;
    static float ipec_setTimerValue = .18f;
    float bodyHitTimer = ipec_setTimerValue;
    public static bool body_head_ishit = false;

    //Basic Break Moving plates
    float speed = 3f;
    private bool edgeSet = false;
    private Vector3 edgeVertex = Vector3.zero;
    private Vector2 edgeUV = Vector2.zero;
    private Plane edgePlane = new Plane();

    public int CutCascades = 1;
    public float ExplodeForce = 0;

    //Basic Break Moving plates variables
    private bool selected = false;
    private Vector3 startingPos;
    private Collider col;

    //Basic Baloon shooting variables
    [Header("Baloon variables")]
    public GameObject holder;

    [Header("Suspect Shoot:")]
    //Suspect Shoot Variables
    public GameObject headCollider;
    public GameObject bodyCollider;

    [Header("Hidden Target Shoot:")]
    public GameObject targetSpawner;
    bool isNextTargetSet = false;

    //[Header("Shell Game:")]
    private bool isCupHit = false;
    private float hitRevealTimer = 1f;
    private float setHitReveal = 1.5f;
    private float moveDistance = 0.014f; 
    private float moveSpeed = 8f;

    private float initialUpY; 
    private float distanceMovedUp = 0.0f;
    private float initialDownY;
    private float distanceMovedDown = 0.0f;
    bool moveUp = false;
    bool moveDown = false;
    bool holderRevealed = false;

    //Rising Plate Shape
    private float currPlateSpeed = 0;
    float previousPlateSpeed = 0;

    void Start()
    {
        objectLive = 1;
        player = GameObject.FindGameObjectWithTag("Player");
        body_head_ishit = false;
        activeScene = SceneManager.GetActiveScene().name;

        if(activeScene.ToLower().Contains("shell"))
        {
            //isCupHit = true;
            //moveUp = true;
            initialUpY = transform.localPosition.y;

        }
        if(activeScene.ToLower().Contains("cyclic"))
        {
            col = GetComponent<Collider>();
            selected = false;
            startingPos = transform.position;
        }
        if (activeScene.ToLower().Contains("humanpopup"))
        {
            anim = this.gameObject.GetComponent<Animator>();
            print("IN THE ANIMATOR CODE");
        }
        else if (activeScene.ToLower().Contains("shootingtarget"))
        {
            targetText.enabled = false;
            isTargetTextActive = false;
            setTime = 1;
        }
        else if (activeScene.ToLower().Contains("ipec"))
        {
            if (gameObject.name.ToLower().Contains("body") || gameObject.name.ToLower().Contains("head"))
            {
                LoadTargetStartInfo();
            }
            else
            {
                loadTargetColor();
            }
            
        }
        else if(activeScene.ToLower().Contains("plat") || activeScene.ToLower().Contains("hidden") || activeScene.ToLower().Contains("sequencenum") 
            || activeScene.ToLower().Contains("distancesimulator") || activeScene.ToLower().Contains("duelingtree"))
        {
            loadTargetColor();
        }
        else if(activeScene.ToLower().Contains("suspectshoot"))
        {
            headCollider.tag = this.transform.gameObject.tag;
            bodyCollider.tag = this.transform.gameObject.tag;
        }
        if(activeScene.ToLower().Contains("hiddentarget"))
        {
            //print("RRE: Point Reached...");
            targetSpawner = GameObject.FindGameObjectWithTag("targetSpawner");
        }
        if (activeScene.ToLower().Contains("claypigeon"))
        {
            //print("RRE: Point Reached...");
            assignDestroyTime();
            isPlateHit = true;
            targetSpawner = GameObject.FindGameObjectWithTag("targetSpawner");
        }
        if (addSoundSourse)
        {
            platHitSound = gameObject.AddComponent<AudioSource>();
            platHitSound.clip = platHitAudio;
        }

        LoadTargetStartInfo();
    }
    // Update is called once per frame
    void Update()
    {

        if (isTargetTextActive && !activeScene.ToLower().Contains("risingplate"))
        {
            //targetText.enabled = true;
            textActiveTimer -= Time.deltaTime;
            if (textActiveTimer <= 0f)
            {
                //targetText.enabled = false;
                isTargetTextActive = false;
            }
        }
        else
        {
            //targetText.enabled = false;
            textActiveTimer = setTime;
        }
        if(isNotActive)
        {
            destroyTimer -= Time.deltaTime * 1;

            if(destroyTimer <= 0f && currect_Target != null)
            {
                currect_Target.SetActive(false);
                isNotActive = false;
            }
        }

        if(isPlateHit)
        {
            destroyTimer -= Time.deltaTime * 1;
            if(destroyTimer <= 0f)
            {
                // StaticVariableManager.plateDestroyed = true;
                Destroy(this.gameObject);
            }
        }
        if(targetShot)
        {
            animTimeOut -= Time.deltaTime * 1;
            if (animTimeOut <= 0f)
            {
                anim.Play("idle");
                targetShot = false;
            }

        }
        else
        {
            animTimeOut = animSetTime;
        }
        if(body_head_ishit == true)
        {
            Time.timeScale = 1f;
            bodyHitTimer -= Time.deltaTime * 1;
            
            if(bodyHitTimer <= 0f)
            {
                revertTargetColor();
                body_head_ishit = false;
            }     
        }
        else
        {
            bodyHitTimer = ipec_setTimerValue;
        }
        if(activeScene == ("BasicBreakMTarget"))
        {
            this.transform.position += new Vector3(0, speed * Time.deltaTime, 0);  // move UP
        }
        if(activeScene.ToLower().Contains("claypigeon"))
        {
            if(currPlateSpeed != StaticVariableManager.pigeonSpeed)
            {
                assignDestroyTime();
                currPlateSpeed = StaticVariableManager.pigeonSpeed;
                previousPlateSpeed = currPlateSpeed;
            }

        }

        if (activeScene.ToLower().Contains("shell") && countDownStart.start_training == true )
        {
            if (transform.name.ToLower().Contains("holder") && holderRevealed == false)
            {
                ApplyDamage(transform.name);

                holderRevealed = true;
                StaticVariableManager.isHolderRevealed = true;
            }
            else if(transform.name.ToLower().Contains("holder") && StaticVariableManager.isHolderRevealed == false)
            {
                ApplyDamage(transform.name);
                StaticVariableManager.isHolderRevealed = true;
            }
        }

        if (isCupHit)
        {
            if(moveUp == true)
            {
                // Move the GameObject upwards along the y-axis
                transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);

                // Calculate distance moved along the y-axis
                distanceMovedUp = (transform.localPosition.y + initialUpY) * -1;
                //print(distanceMovedUp);
                if (distanceMovedUp >= moveDistance)
                {
                    moveUp = false;
                    moveDown = true;
                    hitRevealTimer = setHitReveal;
                    initialDownY = transform.localPosition.y;
                }
            }
            else if (moveDown == true)
            {

                if(hitRevealTimer <= 0f)
                {
                    transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

                    // Calculate distance moved along the y-axis
                    distanceMovedDown = (transform.localPosition.y + initialDownY) * -1;
                    //print(distanceMovedDown);
                    if (distanceMovedDown <= moveDistance)
                    {
                        //StaticVariableManager.reInitialise_1 = true;
                        //StaticVariableManager.reInitialise_2 = true;
                        //StaticVariableManager.reInitialise_3 = true;
                        //StaticVariableManager.cup_init_complete = true;
                        StaticVariableManager.startMoving = true;
                        moveDown = false;
                    }
                }
                else
                {
                    hitRevealTimer -= Time.deltaTime;
                }
            }
        }

        if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.escapeKey.isPressed)
        {
            Destroy(this.gameObject);
        }
    }

    void LoadTargetStartInfo()
    {
        string name = this.transform.name;

        if (gameObject.name.ToLower().Contains("body") || gameObject.name.ToLower().Contains("head"))
        {
            objectMaterial = gameObject.GetComponent<Renderer>().material;
            objectMateria2 = ipec_head.GetComponent<Renderer>().material;
            objectMateria3 = ipec_body.GetComponent<Renderer>().material;

            PlayerPrefs.SetFloat("ObjectColorR", objectMaterial.color.r);
            PlayerPrefs.SetFloat("ObjectColorG", objectMaterial.color.g);
            PlayerPrefs.SetFloat("ObjectColorB", objectMaterial.color.b);
            PlayerPrefs.SetFloat("ObjectColorA", objectMaterial.color.a);

            // Load the color of the material
            r = PlayerPrefs.GetFloat("ObjectColorR");
            g = PlayerPrefs.GetFloat("ObjectColorG");
            b = PlayerPrefs.GetFloat("ObjectColorB");
            a = PlayerPrefs.GetFloat("ObjectColorA");
        }

        if(!activeScene.ToLower().Contains("colorexpression") && (DropDown.scene_type == "static" || DropDown.scene_type == "static response"))
        {
            if (name[0].ToString().Contains("1"))
            {
                if (!StaticVariableManager.isLane1StartTargetSet) player.GetComponent<ReplayCounter>().SetCurrReplayTarget("start", this.gameObject, this.gameObject);
            }
            if (name[0].ToString().Contains("2"))
            {
                if (!StaticVariableManager.isLane2StartTargetSet) player.GetComponent<ReplayCounter>().SetCurrReplayTarget("start", this.gameObject, this.gameObject);
            }
            if (name[0].ToString().Contains("3"))
            {
                if (!StaticVariableManager.isLane3StartTargetSet) player.GetComponent<ReplayCounter>().SetCurrReplayTarget("start", this.gameObject, this.gameObject);
            }
        }


        if(activeScene.ToLower().Contains("hiddentarget"))
        {
            targetSpawner.GetComponent<RisingPlatesDisabler>().currHiddenTarget = this.gameObject;
        }
    }

    private void loadTargetColor()
    {
        if(Scoring.ammo_setting.ToLower().Contains("live") || Scoring.ammo_setting.ToLower().Contains("infrared"))
        {
            Renderer renderer = this.GetComponent<Renderer>();
            //StaticVariableManager.targetColorSetting = "black";
            switch (StaticVariableManager.targetColorSetting.ToLower())
            {
                case "black":
                    r = 42f / 255f;
                    g = 42f / 255f;
                    b = 42f / 255f;
                    a = 1f;
                    renderer.material.color = new Color(r, g, b, a);

                    break;
                case "white":
                    r = 212f / 255f;
                    g = 212f / 255f;
                    b = 212f / 255f;
                    a = 1f;
                    renderer.material.color = new Color(r, g, b, a);

                    break;
                case "yellow":
                    //fpsCam.backgroundColor = Color.blue;
                    r = 217f / 255f;
                    g = 200f / 255f;
                    b = 0f / 255f;
                    a = 1f;
                    renderer.material.color = new Color(r, g, b, a);

                    break;
                case "red":
                    r = 234f / 255f;
                    g = 54f / 255f;
                    b = 54f / 255f;
                    a = 1f;
                    renderer.material.color = new Color(r, g, b, a);
                    break;
                case "blue":
                    //fpsCam.backgroundColor = Color.blue;
                    r = 59f / 255f;
                    g = 59f / 255f;
                    b = 236f / 255f;
                    a = 1f;
                    renderer.material.color = new Color(r, g, b, a);
                    break;

                case "green":
                    r = 54f / 255f;
                    g = 181f / 255f;
                    b = 54f / 255f;
                    a = 1f;
                    renderer.material.color = new Color(r, g, b, a);
                    break;
            }
        }
    }
    void redoStart()
    {
        //platHitSound = gameObject.AddComponent<AudioSource>();
        //platHitSound.clip = platHitAudio;

        activeScene = player.GetComponent<Shooting>().activeScene;

        if (activeScene.ToLower().Contains("humanpopup"))
        {
            anim = this.gameObject.GetComponent<Animator>();
            print("IN THE ANIMATOR CODE");
        }
        else if (activeScene.ToLower().Contains("shootingtarget"))
        {
            targetText.enabled = false;
            isTargetTextActive = false;
            setTime = 1;
        }
        //targetText.enabled = false;
    }

    private void assignDestroyTime()
    {
        switch (StaticVariableManager.pigeonSpeed)
        {
            case 1:
                if(previousPlateSpeed > StaticVariableManager.pigeonSpeed)
                {
                    destroyTimer = 16f - destroyTimer;
                }
                else
                {
                    destroyTimer = 15f;
                }
                break;
            case 2:
                if (previousPlateSpeed > StaticVariableManager.pigeonSpeed)
                {
                    destroyTimer = 8f - destroyTimer;
                }
                else
                {
                    destroyTimer = 8f;
                }
                break;
            case 3:
                if (previousPlateSpeed > StaticVariableManager.pigeonSpeed)
                {
                    destroyTimer = 5f - destroyTimer;
                }
                else
                {
                    destroyTimer = 5f;
                }
                break;
            case 4:
                if (previousPlateSpeed > StaticVariableManager.pigeonSpeed)
                {
                    destroyTimer = 4f - destroyTimer;
                }
                else
                {
                    destroyTimer = 4f;
                }
                break;
            case 5:
                if(destroyTimer > 3.5f)
                {
                    destroyTimer -= 3f;
                }
                else
                {
                    destroyTimer = 3.5f;
                }
                break;

        }
    }
    private void ApplyDamage(string tagged)
    {
        //received = false;
        redoStart();
        activeScene = player.GetComponent<Shooting>().activeScene;
        // Debug.Log("I was hit:" + transform.name + " apply Damage sent:" + tagged);

        if (this.transform.name == tagged)
        {
            //print("I JUST HIT SOMETHING");  

            if (activeScene == "BasicShootingTarget")
            {
                //print("HIT IS " + this.gameObject.name + "!!!");
                currect_Target.SetActive(true);
                destroyTimer = 1f;
                isNotActive = true;

                //this.gameObject.GetComponent<Renderer>().material.color = Color.blue;
            }
            if(activeScene.ToLower().Contains("block"))
            {
                //Shooting.numBlockesHit++;
                //received = false;

                Debug.Log("I was hit:" + transform.name + " apply Damage sent:" + tagged);

                if (this.transform.name == tagged)
                {
                    Shooting.numBlockesHit++;
                    print("DEADLY SHOT!!!!!!!!");
                    LIFE--;
                    if (LIFE <= 0)
                    {
                        Destroy(this.gameObject);
                    }
                }
                else if (tagged.Contains("change"))
                {
                    //seen = true;
                    //changeState(states.patrol);
                }
            }
            if(activeScene.ToLower().Contains("fallingplat"))
            {
                objectLive--;
                isTargetTextActive = true;
                //platHitSound.Play();
                currentPosition = gameObject.transform.position;
                //targetText.text = "Hit " + this.gameObject.name + "";
                //print("Head is DEADLY SHOT!!!!!!!!");
                if (objectLive <= 0f)
                {
                    player.GetComponent<ReplayCounter>().SetCurrReplayTarget(("next"), next_Target, this.gameObject); // Set current target to the currently activated target.
                    this.transform.gameObject.GetComponent<MeshCollider>().enabled = false;

                    next_Target.SetActive(true);
                    CloneAndRelease(this.transform.gameObject);

                    // this.transform.gameObject.SetActive(false);


                    StaticVariableManager.lane1PrevHitTarget = this.gameObject;
                }

            }
            if (activeScene.ToLower().Contains("dice"))
            {
                objectLive--;         
                if (objectLive <= 0f)
                {
                    StaticVariableManager.numDieTargets--;
                    this.gameObject.SetActive(false);
                }

            }
            if (activeScene.ToLower().Contains("shell"))
            {
                //print("Target Shot...");
                objectLive--;
                if (objectLive <= 0f)
                {
                    moveUp = true;
                    isCupHit = true;
                    //this.gameObject.SetActive(false);

                    StaticVariableManager.reInitialise_1 = true;
                    StaticVariableManager.reInitialise_2 = true;
                    StaticVariableManager.reInitialise_3 = true;
                    StaticVariableManager.startMoving = false;
                    StaticVariableManager.isHolderRevealed = false;
                }

            }
            if (activeScene.ToLower().Contains("cargame"))
            {
                float center_point_speed = 5;
                float side_point_speed = 3.5f;

                print("I JUST HIT SOMETHING " + tagged);

                switch (transform.name)
                {
                    case "1.plate":
                        StaticVariableManager.redCarPlateHit = true;
                        StaticVariableManager.redCarSpeedTimer = StaticVariableManager.speedTimerSetValue; // Reset Timer
                        StaticVariableManager.redCarPoints += 1;

                        if(!StaticVariableManager.startRedCarMovement)
                        {
                            StaticVariableManager.startRedCarMovement = true;
                        }
                        else
                        {
                            StaticVariableManager.redCarSpeed = side_point_speed;
                        }
                        //print("Test: Target hit..." + transform.name);
                        break;

                    case "2.plate":
                        StaticVariableManager.blueCarPlateHit = true;
                        StaticVariableManager.blueCarSpeedTimer = StaticVariableManager.speedTimerSetValue; // Reset Timer
                        StaticVariableManager.blueCarPoints += 1;

                        if (!StaticVariableManager.startBlueCarMovement)
                        {
                            StaticVariableManager.startBlueCarMovement = true;
                        }
                        else
                        {
                            StaticVariableManager.blueCarSpeed = side_point_speed;
                        }
                        //print("Test: Target hit..." + transform.name);
                        break;

                    case "3.plate":
                        StaticVariableManager.greenCarPlateHit = true;
                        StaticVariableManager.greenCarSpeedTimer = StaticVariableManager.speedTimerSetValue; // Reset Timer
                        StaticVariableManager.greenCarPoints += 1;

                        if (!StaticVariableManager.startGreenCarMovement)
                        {
                            StaticVariableManager.startGreenCarMovement = true;
                        }
                        else
                        {
                            StaticVariableManager.greenCarSpeed = side_point_speed;
                        }
                        //print("Test: Target hit..." + transform.name);
                        break;

                    case "1.plate.bullseye":
                        StaticVariableManager.redCarPlateHit = true;
                        StaticVariableManager.redCarSpeedTimer = StaticVariableManager.speedTimerSetValue; // Reset Timer
                        StaticVariableManager.redCarPoints += 2;

                        if (!StaticVariableManager.startRedCarMovement)
                        {
                            StaticVariableManager.startRedCarMovement = true;
                        }
                        else
                        {
                            StaticVariableManager.redCarSpeed = center_point_speed;
                        }
                        // print("Test: Target hit..." + transform.name);
                        break;

                    case "2.plate.bullseye":
                        StaticVariableManager.blueCarPlateHit = true;
                        StaticVariableManager.blueCarSpeedTimer = StaticVariableManager.speedTimerSetValue; // Reset Timer
                        StaticVariableManager.blueCarPoints += 2;

                        if (!StaticVariableManager.startBlueCarMovement)
                        {
                            StaticVariableManager.startBlueCarMovement = true;
                        }
                        else
                        {
                            StaticVariableManager.blueCarSpeed = center_point_speed;
                        }
                        //print("Test: Target hit..." + transform.name);
                        break;

                    case "3.plate.bullseye":
                        StaticVariableManager.greenCarPlateHit = true;
                        StaticVariableManager.greenCarSpeedTimer = StaticVariableManager.speedTimerSetValue; // Reset Timer
                        StaticVariableManager.greenCarPoints += 2;

                        if (!StaticVariableManager.startGreenCarMovement)
                        {
                            StaticVariableManager.startGreenCarMovement = true;
                        }
                        else
                        {
                            StaticVariableManager.greenCarSpeed = side_point_speed;
                        }
                        //print("Test: Target hit..." + transform.name);
                        break;
                }

                if(StaticVariableManager.flickerSet)
                {
                    if(tagged.ToLower().Contains("1."))
                    {
                        StaticVariableManager.RedCarHitOnFlicker = true;
                        if(StaticVariableManager.redCarPoints > 0) StaticVariableManager.redCarPoints -= 1;
                    }
                    if (tagged.ToLower().Contains("2."))
                    {
                        StaticVariableManager.BlueCarHitOnFlicker = true;
                        if (StaticVariableManager.blueCarPoints > 0) StaticVariableManager.blueCarPoints -= 1;
                    }
                    if (tagged.ToLower().Contains("3."))
                    {
                        StaticVariableManager.GreenCarHitOnFlicker = true;
                        if (StaticVariableManager.greenCarPoints > 0) StaticVariableManager.greenCarPoints -= 1;
                    }
                }

            }
            if (activeScene.ToLower().Contains("sequencenum"))
            {
                objectLive--;
                string targetName = this.transform.name.Substring(2);

                

                Regex regex = new Regex(@"\d+");
                Match match = regex.Match(targetName);
                string progressValue = match.Value;

                print("Target hit: " + targetName);
                print("Progress Value is:" + progressValue);

                SequenceManager.updateProgressValue(progressValue);
            }
            if (activeScene.ToLower().Contains("colorsequence"))
            {
                objectLive--;

                if(this.transform.name.ToLower().Contains(StaticVariableManager.currentTargetColor))
                {
                    StaticVariableManager.correctTargetsHits++;
                    //print("Correct hits: " + StaticVariableManager.correctColorHits);
                }
                else
                {
                    StaticVariableManager.wrongTargetsHits++;
                    //print("Wrong hits: " + StaticVariableManager.wrongColorHits);
                }

                SequenceManager.receiveTargetName(transform.name);
                // StaticVariableManager.isColorDisplayed = false;
            }
            if (activeScene.ToLower().Contains("expressionshoot"))
            {
                objectLive--;
                
                ExpressionSceneManager.SetHitMessage(this.gameObject);
                //StaticVariableManager.isColorDisplayed = false;
            }
            if (activeScene.ToLower().Contains("rifflepole"))
            {
                objectLive--;
                //isTargetTextActive = true;

                //targetText.text = "Hit " + this.gameObject.name + "";
                //print("Head is DEADLY SHOT!!!!!!!!");
                

                targetController.transform.gameObject.GetComponent<TargetController>().SendMessage("ReceiveMessage", tagged);

                //currect_Target.SetActive(false);
                //print("Current target name is: " + currect_Target.gameObject.name);

                destroyTimer = .0f;
                isNotActive = true;
            }
            if (activeScene == "BasicHumanPopUp")
            {
                if(this.gameObject.name.ToLower().Contains("soldier"))
                {
                    anim.Play("gun_hit_reaction");
                    //anim.Play("gun_hit_low_reaction");
                }
                else if (this.gameObject.name.ToLower().Contains("alison"))
                {

                    print("I AM IN ALISON");
                    anim.Play("dying");
                    targetShot = true;
                }
                else if (this.gameObject.name.ToLower().Contains("eric"))
                {
                    anim.Play("dying");
                    print("I AM IN Eric");
                    targetShot = true;
                }


            }
            if(activeScene.ToLower().Contains("dueling"))
            {
                player.GetComponent<ReplayCounter>().SetCurrReplayTarget("next", next_Target, this.gameObject); // Set current target to the currently activated target.

                //platHitSound.Play();
                next_Target.SetActive(true);
                currect_Target.SetActive(false);
            }
            if(activeScene == "BasicResetable")
            {
                platHitSound.Play();
                isRandomState = targetController.GetComponent<TargetController>().isRandomState;
                if(!isRandomState)
                {
                    DropTarget();
                }
                targetController.gameObject.GetComponent<TargetController>().SendMessage("ReceiveMessage", this.transform.name);
            }
            if (activeScene.ToLower().Contains("ipecboard"))
            {

                player.GetComponent<ReplayCounter>().SetCurrReplayTarget("next", next_Target, this.gameObject); // Set current target to the currently activated target.

                if (this.gameObject.name.ToLower().Contains("plate"))
                {
                    //platHitSound.Play();
                    currect_Target.SetActive(false);
                    next_Target.SetActive(true);
                }
                else if (gameObject.name.ToLower().Contains("body") || gameObject.name.ToLower().Contains("head"))
                {
                    // changeTargetColor();
                    ColorManager.revert_head_body(r,g,b,a);
                    body_head_ishit = true;
                }
                //print("Target is HITTTTTT...");
            }
            if (activeScene == ("BasicBreakMTarget"))
            {

                platHitSound.Play();
                print("Target Hit------------------");
                //DestroyMesh();
                Destroy(this.gameObject);

            }
            if(activeScene.ToLower().Contains("basiccyclic"))
            {
                objectLive--;
                isTargetTextActive = true;
                //platHitSound.Play();
                currentPosition = gameObject.transform.position;

                if (objectLive <= 0f)
                {

                    //this.gameObject.GetComponent<SphereCollider>().enabled = false;
                    //this.gameObject.GetComponent<Rigidbody>().useGravity = true;
                    isNotActive = true;

                    //next_Target.SetActive(true);
                }

                
            }
            if (activeScene.ToLower().Contains("hiddentarget"))
            {
                objectLive--;
                isTargetTextActive = true;

                if (objectLive <= 0f && isNextTargetSet == false)
                {
                    this.gameObject.SetActive(false);
                    this.gameObject.GetComponent<MeshCollider>().enabled = false;
                    targetSpawner.GetComponent<targetSpawner>().spawnPlate();

                    player.GetComponent<ReplayCounter>().SetCurrReplayTarget("next", this.gameObject, this.gameObject);

                }
            }
            if (activeScene.ToLower().Contains("risingplate"))
            {
                objectLive--;
                isTargetTextActive = true;
                char tempFirstChar = tagged[0];
                bool applyDamage = true;

                switch(tempFirstChar)
                {
                    case '1':
                        if(Shooting.lane1TargetsComplete)
                        {
                            applyDamage = false;
                        }
                        break;
                    case '2':
                        if (Shooting.lane2TargetsComplete)
                        {
                            applyDamage = false;
                        }
                        break;
                    case '3':
                        if (Shooting.lane3TargetsComplete)
                        {
                            applyDamage = false;
                        }
                        break;
                    case '4':
                        if (Shooting.lane4TargetsComplete)
                        {
                            applyDamage = false;
                        }
                        break;
                }

                //print("Hit " + this.gameObject.name + ".");
                //print("Test: I am Hit..");
                if (objectLive <= 0f)
                {
                    //this.gameObject.GetComponent<BoxCollider>().enabled = false;
                    this.gameObject.GetComponent<MeshCollider>().enabled = false;
                    player.GetComponent<ReplayCounter>().SetCurrReplayTarget("next", next_Target, this.gameObject); // Set current target to the currently activated target.

                    if (applyDamage)
                    {
                        if(next_Target != null)
                        {
                            next_Target.SetActive(true);
                        }
                        this.transform.gameObject.SetActive(false);
                    }
                }
                


            }
            if (activeScene.ToLower().Contains("shapeplat"))
            {
                objectLive--;

                //print("Hit " + this.gameObject.name + ".");
                if (objectLive <= 0f)
                {
                    //this.gameObject.GetComponent<BoxCollider>().enabled = false;
                    Destroy(this.gameObject);
                }



            }
            if (activeScene.ToLower().Contains("baloon"))
            {
                objectLive--;
                
                //print("RE: Hit " + this.gameObject.name + ".");
                //print("RE: Current is: " + StaticVariableManager.currentLane1Color.ToLower());
                //print("RE: Next is: " + StaticVariableManager.nextLane1Color.ToLower());
                if (objectLive <= 0f && tagged.ToLower().Contains("1"))
                {
                    Destroy(this.gameObject);
                }
                if (objectLive <= 0f && tagged.ToLower().Contains("2"))
                {
                    Destroy(this.gameObject);
                }

            }
            if (activeScene.ToLower().Contains("suspectshoot"))
            {
                objectLive--;
                isTargetTextActive = true;
                char tempFirstChar = tagged[0];
                bool applyDamage = true;

                print("RE: Hit " + this.gameObject.name + ".");
                /*switch (tempFirstChar)
                {
                    case '1':
                        if (Shooting.lane1TargetsComplete)
                        {
                            applyDamage = false;
                        }
                        break;
                    case '2':
                        if (Shooting.lane2TargetsComplete)
                        {
                            applyDamage = false;
                        }
                        break;
                    case '3':
                        if (Shooting.lane3TargetsComplete)
                        {
                            applyDamage = false;
                        }
                        break;
                    case '4':
                        if (Shooting.lane4TargetsComplete)
                        {
                            applyDamage = false;
                        }
                        break;
                }

                if (objectLive <= 0f)
                {
                    //this.gameObject.GetComponent<BoxCollider>().enabled = false;
                    this.gameObject.GetComponent<SphereCollider>().enabled = false;
                    this.gameObject.GetComponent<Rigidbody>().useGravity = true;
                    isNotActive = true;
                    isPlateHit = true;
                    if (applyDamage)
                    {
                        next_Target.SetActive(true);
                    }
                }*/

            }
            if (activeScene.ToLower().Contains("claypigeon"))
            {
                StaticVariableManager.plateDestroyed = true;
                Destroy(this.gameObject);
            }
            // ManageReactionTime(tagged);
        }
        else if (tagged.Contains("change"))
        {
            //seen = true;
            //changeState(states.patrol);
        }


    }
    private void CloneAndRelease(GameObject plate)
    {
        // STEP 1: Create a blank GameObject in world space
        GameObject clone = new GameObject("DetachedPlateClone");

        // STEP 2: Copy world transform from the plate
        clone.transform.position = plate.transform.position;
        clone.transform.rotation = plate.transform.rotation;
        clone.transform.localScale = plate.transform.lossyScale;

        // STEP 3: Copy the MeshFilter and MeshRenderer
        MeshFilter originalFilter = plate.GetComponent<MeshFilter>();
        MeshRenderer originalRenderer = plate.GetComponent<MeshRenderer>();

        if (originalFilter != null)
        {
            MeshFilter cloneFilter = clone.AddComponent<MeshFilter>();
            cloneFilter.sharedMesh = originalFilter.sharedMesh;
        }

        if (originalRenderer != null)
        {
            MeshRenderer cloneRenderer = clone.AddComponent<MeshRenderer>();
            cloneRenderer.sharedMaterials = originalRenderer.sharedMaterials;
        }

        // STEP 4: Copy collider (optional: BoxCollider or MeshCollider)
        Collider originalCollider = plate.GetComponent<Collider>();
        if (originalCollider != null)
        {
            if (originalCollider is BoxCollider originalBox)
            {
                BoxCollider cloneBox = clone.AddComponent<BoxCollider>();
                cloneBox.center = originalBox.center;
                cloneBox.size = originalBox.size;
                cloneBox.isTrigger = originalBox.isTrigger;
                cloneBox.material = originalBox.material;
            }
            else if (originalCollider is SphereCollider originalSphere)
            {
                SphereCollider cloneSphere = clone.AddComponent<SphereCollider>();
                cloneSphere.center = originalSphere.center;
                cloneSphere.radius = originalSphere.radius;
                cloneSphere.isTrigger = originalSphere.isTrigger;
                cloneSphere.material = originalSphere.material;
            }
            else if (originalCollider is MeshCollider originalMesh)
            {
                MeshCollider cloneMesh = clone.AddComponent<MeshCollider>();
                cloneMesh.sharedMesh = originalMesh.sharedMesh;
                cloneMesh.convex = originalMesh.convex;
                cloneMesh.isTrigger = originalMesh.isTrigger;
                cloneMesh.material = originalMesh.material;
            }
            // Add more collider types here as needed
        }


        // STEP 5: Add Rigidbody
        Rigidbody rb = clone.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;

        // STEP 6: Optional – Copy custom script value
        TargetMessageReceive receiver = clone.AddComponent<TargetMessageReceive>();
        receiver.isPlateHit = true;

        // STEP 7: Disable the original nested plate
        plate.SetActive(false);
    }




    private void ManageReactionTime(string target_name)
    {
        //Evaluate Current Lane
        char tempFirstChar = target_name[0];
        switch (tempFirstChar)
        {
            case '1':
                player.GetComponent<Shooting>().updateLane1ResponseTime("hit", "c");
                break;

            case '2':
                player.GetComponent<Shooting>().updateLane2ResponseTime("hit", "c");
                break;

            case '3':
                player.GetComponent<Shooting>().updateLane3ResponseTime("hit", "c");
                break;
        }
    }

    private void changeTargetColor()
    {
        if (gameObject.name.ToLower().Contains("body") || gameObject.name.ToLower().Contains("head"))
        {
            objectMateria3.color = Color.black;
            objectMateria2.color = Color.black;
        }
    }
    private void revertTargetColor()
    {
        if (gameObject.name.ToLower().Contains("body") || gameObject.name.ToLower().Contains("head"))
        {
            objectMateria3.color = new Color(r, g, b, a);
            objectMateria2.color = new Color(r, g, b, a);
            print("RE: Color reset");
        }
    }
    public void SwitchTarget()
    {
        currect_Target.SetActive(true);
        next_Target.SetActive(false);
    }
    public void DropTarget()
    {
        next_Target.SetActive(true);
        currect_Target.SetActive(false);
    }
    private void LoadSound()
    {
        platHitSound = gameObject.AddComponent<AudioSource>();
        platHitSound.clip = platHitAudio;
    }
    private void DestroyMesh()
    {
        var originalMesh = GetComponent<MeshFilter>().mesh;
        originalMesh.RecalculateBounds();
        var parts = new List<PartMesh>();
        var subParts = new List<PartMesh>();

        var mainPart = new PartMesh()
        {
            UV = originalMesh.uv,
            Vertices = originalMesh.vertices,
            Normals = originalMesh.normals,
            Triangles = new int[originalMesh.subMeshCount][],
            Bounds = originalMesh.bounds
        };
        for (int i = 0; i < originalMesh.subMeshCount; i++)
            mainPart.Triangles[i] = originalMesh.GetTriangles(i);

        parts.Add(mainPart);

        for (var c = 0; c < CutCascades; c++)
        {
            for (var i = 0; i < parts.Count; i++)
            {
                var bounds = parts[i].Bounds;
                bounds.Expand(0.5f);

                var plane = new Plane(UnityEngine.Random.onUnitSphere, new Vector3(UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                                                                                   UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                                                                                   UnityEngine.Random.Range(bounds.min.z, bounds.max.z)));


                subParts.Add(GenerateMesh(parts[i], plane, true));
                subParts.Add(GenerateMesh(parts[i], plane, false));
            }
            parts = new List<PartMesh>(subParts);
            subParts.Clear();
        }

        for (var i = 0; i < parts.Count; i++)
        {
            parts[i].MakeGameobject(this);
            parts[i].GameObject.GetComponent<Rigidbody>().AddForceAtPosition(parts[i].Bounds.center * ExplodeForce, transform.position);
        }

        Destroy(gameObject);
    }
    private PartMesh GenerateMesh(PartMesh original, Plane plane, bool left)
    {
        var partMesh = new PartMesh() { };
        var ray1 = new Ray();
        var ray2 = new Ray();


        for (var i = 0; i < original.Triangles.Length; i++)
        {
            var triangles = original.Triangles[i];
            edgeSet = false;

            for (var j = 0; j < triangles.Length; j = j + 3)
            {
                var sideA = plane.GetSide(original.Vertices[triangles[j]]) == left;
                var sideB = plane.GetSide(original.Vertices[triangles[j + 1]]) == left;
                var sideC = plane.GetSide(original.Vertices[triangles[j + 2]]) == left;

                var sideCount = (sideA ? 1 : 0) +
                                (sideB ? 1 : 0) +
                                (sideC ? 1 : 0);
                if (sideCount == 0)
                {
                    continue;
                }
                if (sideCount == 3)
                {
                    partMesh.AddTriangle(i,
                                         original.Vertices[triangles[j]], original.Vertices[triangles[j + 1]], original.Vertices[triangles[j + 2]],
                                         original.Normals[triangles[j]], original.Normals[triangles[j + 1]], original.Normals[triangles[j + 2]],
                                         original.UV[triangles[j]], original.UV[triangles[j + 1]], original.UV[triangles[j + 2]]);
                    continue;
                }

                //cut points
                var singleIndex = sideB == sideC ? 0 : sideA == sideC ? 1 : 2;

                ray1.origin = original.Vertices[triangles[j + singleIndex]];
                var dir1 = original.Vertices[triangles[j + ((singleIndex + 1) % 3)]] - original.Vertices[triangles[j + singleIndex]];
                ray1.direction = dir1;
                plane.Raycast(ray1, out var enter1);
                var lerp1 = enter1 / dir1.magnitude;

                ray2.origin = original.Vertices[triangles[j + singleIndex]];
                var dir2 = original.Vertices[triangles[j + ((singleIndex + 2) % 3)]] - original.Vertices[triangles[j + singleIndex]];
                ray2.direction = dir2;
                plane.Raycast(ray2, out var enter2);
                var lerp2 = enter2 / dir2.magnitude;

                //first vertex = ancor
                AddEdge(i,
                        partMesh,
                        left ? plane.normal * -1f : plane.normal,
                        ray1.origin + ray1.direction.normalized * enter1,
                        ray2.origin + ray2.direction.normalized * enter2,
                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2));

                if (sideCount == 1)
                {
                    partMesh.AddTriangle(i,
                                        original.Vertices[triangles[j + singleIndex]],
                                        //Vector3.Lerp(originalMesh.vertices[triangles[j + singleIndex]], originalMesh.vertices[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        //Vector3.Lerp(originalMesh.vertices[triangles[j + singleIndex]], originalMesh.vertices[triangles[j + ((singleIndex + 2) % 3)]], lerp2),
                                        ray1.origin + ray1.direction.normalized * enter1,
                                        ray2.origin + ray2.direction.normalized * enter2,
                                        original.Normals[triangles[j + singleIndex]],
                                        Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 2) % 3)]], lerp2),
                                        original.UV[triangles[j + singleIndex]],
                                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2));

                    continue;
                }

                if (sideCount == 2)
                {
                    partMesh.AddTriangle(i,
                                        ray1.origin + ray1.direction.normalized * enter1,
                                        original.Vertices[triangles[j + ((singleIndex + 1) % 3)]],
                                        original.Vertices[triangles[j + ((singleIndex + 2) % 3)]],
                                        Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        original.Normals[triangles[j + ((singleIndex + 1) % 3)]],
                                        original.Normals[triangles[j + ((singleIndex + 2) % 3)]],
                                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        original.UV[triangles[j + ((singleIndex + 1) % 3)]],
                                        original.UV[triangles[j + ((singleIndex + 2) % 3)]]);
                    partMesh.AddTriangle(i,
                                        ray1.origin + ray1.direction.normalized * enter1,
                                        original.Vertices[triangles[j + ((singleIndex + 2) % 3)]],
                                        ray2.origin + ray2.direction.normalized * enter2,
                                        Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        original.Normals[triangles[j + ((singleIndex + 2) % 3)]],
                                        Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 2) % 3)]], lerp2),
                                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        original.UV[triangles[j + ((singleIndex + 2) % 3)]],
                                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2));
                    continue;
                }


            }
        }

        partMesh.FillArrays();

        return partMesh;
    }

    private void AddEdge(int subMesh, PartMesh partMesh, Vector3 normal, Vector3 vertex1, Vector3 vertex2, Vector2 uv1, Vector2 uv2)
    {
        if (!edgeSet)
        {
            edgeSet = true;
            edgeVertex = vertex1;
            edgeUV = uv1;
        }
        else
        {
            edgePlane.Set3Points(edgeVertex, vertex1, vertex2);

            partMesh.AddTriangle(subMesh,
                                edgeVertex,
                                edgePlane.GetSide(edgeVertex + normal) ? vertex1 : vertex2,
                                edgePlane.GetSide(edgeVertex + normal) ? vertex2 : vertex1,
                                normal,
                                normal,
                                normal,
                                edgeUV,
                                uv1,
                                uv2);
        }
    }

    public class PartMesh
    {
        private List<Vector3> _Verticies = new List<Vector3>();
        private List<Vector3> _Normals = new List<Vector3>();
        private List<List<int>> _Triangles = new List<List<int>>();
        private List<Vector2> _UVs = new List<Vector2>();
        public Vector3[] Vertices;
        public Vector3[] Normals;
        public int[][] Triangles;
        public Vector2[] UV;
        public GameObject GameObject;
        public Bounds Bounds = new Bounds();

        public PartMesh()
        {

        }

        public void AddTriangle(int submesh, Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 normal1, Vector3 normal2, Vector3 normal3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
        {
            if (_Triangles.Count - 1 < submesh)
                _Triangles.Add(new List<int>());

            _Triangles[submesh].Add(_Verticies.Count);
            _Verticies.Add(vert1);
            _Triangles[submesh].Add(_Verticies.Count);
            _Verticies.Add(vert2);
            _Triangles[submesh].Add(_Verticies.Count);
            _Verticies.Add(vert3);
            _Normals.Add(normal1);
            _Normals.Add(normal2);
            _Normals.Add(normal3);
            _UVs.Add(uv1);
            _UVs.Add(uv2);
            _UVs.Add(uv3);

            Bounds.min = Vector3.Min(Bounds.min, vert1);
            Bounds.min = Vector3.Min(Bounds.min, vert2);
            Bounds.min = Vector3.Min(Bounds.min, vert3);
            Bounds.max = Vector3.Min(Bounds.max, vert1);
            Bounds.max = Vector3.Min(Bounds.max, vert2);
            Bounds.max = Vector3.Min(Bounds.max, vert3);
        }

        public void FillArrays()
        {
            Vertices = _Verticies.ToArray();
            Normals = _Normals.ToArray();
            UV = _UVs.ToArray();
            Triangles = new int[_Triangles.Count][];
            for (var i = 0; i < _Triangles.Count; i++)
                Triangles[i] = _Triangles[i].ToArray();
        }

        public void MakeGameobject(MeshDestroy original)
        {
            GameObject = new GameObject(original.name);
            GameObject.transform.position = original.transform.position;
            GameObject.transform.rotation = original.transform.rotation;
            GameObject.transform.localScale = original.transform.localScale;

            var mesh = new Mesh();
            mesh.name = original.GetComponent<MeshFilter>().mesh.name;

            mesh.vertices = Vertices;
            mesh.normals = Normals;
            mesh.uv = UV;
            for (var i = 0; i < Triangles.Length; i++)
                mesh.SetTriangles(Triangles[i], i, true);
            Bounds = mesh.bounds;

            var renderer = GameObject.AddComponent<MeshRenderer>();
            renderer.materials = original.GetComponent<MeshRenderer>().materials;

            var filter = GameObject.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            var collider = GameObject.AddComponent<MeshCollider>();
            collider.convex = true;

            var rigidbody = GameObject.AddComponent<Rigidbody>();
            var meshDestroy = GameObject.AddComponent<MeshDestroy>();
            meshDestroy.CutCascades = original.CutCascades;
            meshDestroy.ExplodeForce = original.ExplodeForce;

        }

        internal void MakeGameobject(TargetMessageReceive targetMessageReceive)
        {
            throw new NotImplementedException();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //print("I have collided with... " + collision.transform.name);
    }

}

/*int x = 2;
            while (x >= 0)
            {
                bodyHitTimer -= Time.deltaTime * 1;

                if (bodyHitTimer <= 0f)
                {
                    if (inStatetimeOUt)
                    {
                        changeTargetColor();
                        inStatetimeOUt = false;
                        
                    }
                    else
                    {
                        revertTargetColor();
                        inStatetimeOUt = true;
                    }
                    bodyHitTimer = ipec_setTimerValue;
                }
                x++;
            }

            body_head_ishit = false;*/