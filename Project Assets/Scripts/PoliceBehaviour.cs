using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PoliceBehaviour : MonoBehaviour
{
	public GameObject eric;
	public GameObject alison;
	public GameObject blood;
	public GameObject bullet;
	public GameObject impactEffect;
	private Rigidbody rigidBd;

	public Animator anim;
	float speed_walk = 2f;
	Transform mainPlayer;
	private string TypeOfSoldier = "";
	private string[] ListOfTypeOfSoldier = new string[4];
	private int TypesOfEnemy;

	float EnemyChargeDistance = 10;
	float EnemyShootFromFarDistance = 20;
	private int NumberOfHits = 2;
	private float SpeedRun = 8f, SpeedWalk = 1f;

	public AudioSource handgunSound;
	public AudioSource ladyScreamSound;
	public AudioClip handgunAudio;
	public AudioClip ladyscreamAudio;

	Ray ray;
	private int shootingPrecision = 500;
	private int shootingMaxRange = 100;
	string activeScene;

	[SerializeField] private GameObject gun1;
	[SerializeField] private GameObject gun2;

	GameObject[] walkables;

	enum states { patrol, killing, dying };
	states state;
	int index = 0;
	int nextIndex = 0;
	int prevIndex = 0;
	bool seen = false;
	float closeDistance = 1000;
	bool received = false;

	//New funtioning variavbles
	bool hide = false;
	int hideIndex = 0;
	GameObject[] hideSpots;
	public float distance = 0f;
	public float hideCountDown = 2f;
	bool isShot = false;
	float shotAnimTime = 3f;
	float hideSpotTimeOut = 2f;
	float shootSpotTimeOut = 5f;
	bool isHidePointReached = false;
	bool isShootPointReached = false;
	int hideType;
	float ShootCount = 2;
	public bool isPlayerClose = false;

	//Health Bar variables
	public int maxHealth;
	public HealthBar healthBar;
	public int currHealth;
	int soldier_anim_type = 0; // used to select how the soldier reacts in shooting mode
	bool trigger = false;
	float call_time = 0f;
	float roll_time = 0f;
	int direction = 0;
	public AudioSource CallSound;
	public AudioClip CallAudio;
	int shootState = 0;
	int shootRunCondition = 0;
	int hideDistance = 0;
	float shootTime = 1.5f;
	bool shoot = true;
	public GameObject player;
	Shooting shooting;
	int animChangeSwitch = 0;
	float anim_change_timer = 20;
	int side_select = 0;
	int talkRandomiser = 0;
	bool animRandomEnabler = true;
	bool walkRandomEnabler = true;
	int fst_walk_randomiser = 0;
	int entrydirection = 0;
	string pointName;

	private void Start()
	{

		animChangeSwitch = Random.Range(1, 4);
		player = GameObject.FindGameObjectWithTag("Player");
		shooting = player.GetComponent<Shooting>();
		hideDistance = Random.Range(30, 45);
		shootRunCondition = Random.Range(1, 3);
		shootState = Random.Range(0, 2);
		direction = Random.Range(0, 1);
		entrydirection = Random.Range(1, 4);
		currHealth = 1 / 2;
		anim = this.GetComponent<Animator>();

		////////

		if (entrydirection == 1 || entrydirection == 2)
		{
			walkables = GameObject.FindGameObjectsWithTag("policewalkright");
		}
		else if (entrydirection == 3 || entrydirection == 4)
		{
			walkables = GameObject.FindGameObjectsWithTag("policewalkleft");
		}

		index = 1;
		pointName = walkables[index].name;

		////////

		CallSound = gameObject.AddComponent<AudioSource>();
		CallSound.clip = CallAudio;

		hideType = Random.Range(1, 4);

		if (hideType == 1)
		{
			hideSpots = GameObject.FindGameObjectsWithTag("hideSpot");
		}
		if (hideType == 2)
		{
			hideSpots = GameObject.FindGameObjectsWithTag("hideSpot2");
		}
		if (hideType == 3)
		{
			hideSpots = GameObject.FindGameObjectsWithTag("hideSpot3");
		}
		if (hideType == 4)
		{
			hideSpots = GameObject.FindGameObjectsWithTag("hideSpot4");
		}


		mainPlayer = GameObject.Find("Player").GetComponent<Transform>();
		GetTypeOfSoldier();
		activeScene = SceneManager.GetActiveScene().name;

		if (activeScene == "Outdoor_FOREST")
		{
			soldier_anim_type = Random.Range(1, 6);
		}
		else
		{
			soldier_anim_type = 1;//Random.Range(1, 3);
		}

		//Debug.Log("Active scene is:" + activeScene);
		state = states.patrol;
		handgunSound = gameObject.AddComponent<AudioSource>();
		handgunSound.clip = handgunAudio;
		ladyScreamSound = gameObject.AddComponent<AudioSource>();
		ladyScreamSound.clip = ladyscreamAudio;
		rigidBd = GetComponent<Rigidbody>();


		//hideIndex = Random.Range(0, hideSpots.Length);
		hideIndex = 0;


	}
	// Update is called once per frame
	void Update()
	{
		if (StaticVariableManager.isStopTraining == false)
		{
			if (activeScene == "HumanTargetPopup")
            {
            	//Do Nothing
            	//Move();
            	//speed();
            	anim.Play("idle");
            }
            else
            {
            	if (NumberOfHits > 0)
            	{
            		GetDistance();
            		//Move();
            		soldier();
            		//animate();
            	}
            	//The Shot Animation timeout and speed
            	speed();
            	if (distance <= EnemyChargeDistance && NumberOfHits > 0 && !isShot)
            	{
					isPlayerClose = true;         
            		anim.Play("pistol_firing");
            		EnemyView();
            		ShootOfEnemy();        
            		hideCountDown -= Time.deltaTime * 2;
            	}
            	if (distance > EnemyChargeDistance)
            	{
            		isPlayerClose = false;
            	            
            	    if (shooting.numEnemies <= 0 || shooting.shootingTimeOut <= 0)
            	    {
            	    	ShootOfEnemy();
            	    }
                }
		    }
		}
	}

	void speed()
	{

		if (isShot == true)
		{
			SpeedRun = 0;
			SpeedWalk = 0;
			shotAnimTime -= Time.deltaTime * 1;
			if (shotAnimTime <= 0)
			{
				isShot = false;
			}
		}
		else if (!isShot && !isHidePointReached && !isShootPointReached && !isPlayerClose)
		{
			shotAnimTime = 2f;
			SpeedRun = 3f;
			speed_walk = 2f;
		}
	}

	private void EnemyView()
	{
		//if(distance <= EnemyShootFromFarDistance)
		//{
		//	Vector3 LookDir = mainPlayer.position - this.gameObject.transform.position;
		//	LookDir.y = 0;
		//	transform.LookAt(this.gameObject.transform.position + LookDir, Vector3.up);
		//}//end of void EnemyView()
		Vector3 LookDir = mainPlayer.position - this.gameObject.transform.position;
		LookDir.y = 0;
		transform.LookAt(this.gameObject.transform.position + LookDir, Vector3.up);
		//print("Enemy View");
	}
	private void GetDistance()//gets the distance between the enemy and the player
	{
		distance = Vector3.Distance(this.gameObject.transform.position, mainPlayer.position);
		//print ("distance:" + distance + "Soldier Type:"+TypeOfSoldier);

	}//end of GetDistance
	private void GetTypeOfSoldier()
	{
		TypesOfEnemy = 1;// = Random.Range(0, 4);//1,2,3 is field soldier, 4 is snipper
		if (TypesOfEnemy == 4)
		{
			TypesOfEnemy = 3;
		}
		ListOfTypeOfSoldier[0] = "field";
		ListOfTypeOfSoldier[1] = "field";
		ListOfTypeOfSoldier[2] = "field";
		ListOfTypeOfSoldier[3] = "sniper";
		TypeOfSoldier = ListOfTypeOfSoldier[TypesOfEnemy];
		//print("Type of Soldier:" + TypeOfSoldier + "int:" + TypesOfEnemy);
	}
	private void ApplyDamage(string tagged)
	{
		received = false;
		//Debug.Log("I was hit:" + transform.name + " apply Damage sent:" + tagged + " counter=" + NumberOfHits);
		if (transform.name == tagged)
		{
			//anim.StopPlayback();
			//anim.enabled = true;
			if (NumberOfHits > 0)
			{
				isShot = true;
				NumberOfHits--;
				healthBar.UpdateHealth(0.5f);
				int x = Random.Range(0, 2);
				if (x == 0)
				{
					anim.Play("gun_hit_reaction");
				}
				if (x == 1)
				{
					anim.Play("gun_hit_low_reaction");
				}
			}
			if (NumberOfHits <= 0)
			{
				healthBar.UpdateHealth(0);
				anim.Play("dying");
				gameObject.transform.GetComponent<CapsuleCollider>().enabled = false;
				rigidBd.isKinematic = true;
				Destroy(this.gameObject, 50f);
				Shooting.enemyshot++;
			}
			received = true;
		}
		else if (tagged.Contains("change"))
		{
			seen = true;
			//changeState(states.patrol);
		}

		//print("Number Of Hits:" + NumberOfHits);
	}
	void ShootOfEnemy()
	{

		//handgunSound.Stop();

		shootTime -= Time.deltaTime;
		if (shootTime <= 0)
		{
			handgunSound.Play();
			shoot = false;
			shootTime = 1;
		}

		gun1.SetActive(true);
		RaycastHit hit;
		//Ray ray;
		//int doIShoot = Random.Range(0, shootingPrecision);
		//print(doIShoot);

		Vector3 LookDir;
		if (Random.Range(0, 3) == 1)//randomise where to shoot
		{
			LookDir = mainPlayer.position - this.gameObject.transform.position;
		}
		else
		{
			LookDir = transform.forward;
		}
		if (Physics.Raycast(transform.position, LookDir, out hit, shootingMaxRange))
		{
			//Debug.Log("Enemy is shooting:" + hit.transform.name);

			if (hit.transform.name.Contains("civilian"))//if civilian was hit
			{
				hit.transform.gameObject.GetComponent<civilian_behaviour>().SendMessage("ApplyDamage", "shot");
				//GameObject impactGo = Instantiate(blood, hit.point, Quaternion.LookRotation(hit.normal));
				//Destroy(impactGo, 2f);
			}
			else
			{
				if (hit.transform.name.Contains("Player"))//if main player was hit
				{
					hit.transform.gameObject.GetComponent<Shooting>().SendMessage("ApplyDamage", transform.name);
					//GameObject impactGo1 = Instantiate(blood, hit.point, Quaternion.LookRotation(hit.normal));
					//Destroy(impactGo1, 2f);
				}
				GameObject[] civilians = GameObject.FindGameObjectsWithTag("civilian");//getting all the civilians
				foreach (GameObject civi in civilians)//making all civillians panic
				{
					civi.GetComponent<civilian_behaviour>().SendMessage("ApplyDamage", "panic");
				}
				//GameObject impactGo = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
				//Destroy(impactGo, 2f);
			}

		}

	}
	void PolicePatrolView(int elementIndex)
	{
		Vector3 LookDir = walkables[elementIndex].transform.position - this.gameObject.transform.position;
		LookDir.y = 0;
		transform.LookAt(this.gameObject.transform.position + LookDir, Vector3.up);
	}
	void InstantiateSoldierState()
	{
		if (soldier_anim_type != 6)
		{
			soldier_anim_type++;
		}
		else
		{
			soldier_anim_type = 1;
		}
	}

	void soldier()
	{
		switch (state)
		{
			case states.patrol:
				patrol();
				break;
			case states.killing:
				killing();
				break;
		}
	}
	void changeState(states curr)
	{
		state = curr;
	}
	void Move()
	{

		if (!seen && !isPlayerClose)
		{

			if (walkables[index].name.Contains("con") && activeScene != "Outdoor_FOREST")
			{

				if (Vector3.Distance(this.transform.position, walkables[index].transform.position) > 7 && index < walkables.Length - 1)
				{

					anim.SetBool("isTalking", false);
					anim.SetBool("isWaving", false);
					anim.SetBool("isCalling", false);

					playFirstWalkAnim();
					updateAnimTimer();
				}
				else
				{
					if (pointName.ToLower().Contains("sit"))
					{
						if (animRandomEnabler == true)
						{
							talkRandomiser = Random.Range(1, 5);
							animRandomEnabler = false;
						}

						if (talkRandomiser == 1 || talkRandomiser == 3)
						{
							anim.SetBool("isWaving", true);
							anim.Play("wav");
							hide = true;
							SpeedWalk = 0;
						}
						else
						{
							anim.SetBool("isTalking", true);
							anim.Play("talk");
							hide = true;
							SpeedWalk = 0;

						}


					}
					else if (pointName.ToLower().Contains("argue"))
					{
						
						anim.SetBool("isCalling", true);
						anim.Play("talk_on_phone");
						SpeedWalk = 0;
					}


					if (animChangeSwitch == 1 || animChangeSwitch == 2)
					{
						anim_change_timer -= Time.deltaTime * 1;
						if (anim_change_timer <= 0)
						{
							if (index < walkables.Length - 1)
							{
								index++;

							}
							else
							{
								index = 0;
							}

							pointName = walkables[index].name;
							anim.SetBool("isCalling", false);
							anim.SetBool("isTalking", false);
							anim.SetBool("isWaving", false);
							animRandomEnabler = true;
							walkRandomEnabler = true;
						}

					}
				}

			}
			else
			{
				if (Vector3.Distance(this.transform.position, walkables[index].transform.position) < 4 && index < (walkables.Length - 1))
				{
					index++;
					walkRandomEnabler = false;
					pointName = walkables[index].name;
				}
				else if (index == walkables.Length - 1)
				{
					index = 0;
					walkRandomEnabler = false;
					pointName = walkables[index].name;
				}

				playFirstWalkAnim();
				updateAnimTimer();
				print("currenty index is: " + index);

			}



			PolicePatrolView(index);
			if (isShot == false)
			{
				transform.position += transform.forward * Time.deltaTime * (SpeedWalk);
			}

			hideCountDown = 1f;
		}
		else
		{
			//print("now in shooting");
			if (shootState == 0)
			{
				if (distance < hideDistance)
				{
					hide = true;
				}
				else
				{
					//hide = false;
				}

			}
			if (shootState == 1)
			{

				if (distance <= hideDistance)
				{
					hide = true;
				}
				else
				{
					//hide = false;
				}

			}

			if (soldier_anim_type == 1)
			{

				if (hide == true && !isPlayerClose)
				{

					if (Vector3.Distance(this.transform.position, hideSpots[hideIndex].transform.position) < 2 && !isHidePointReached && !isShootPointReached)
					{
						//hide = false;
						isHidePointReached = true;
						hideSpotTimeOut = 4;
						shootSpotTimeOut = 2;
						ShootCount = 2;

					}

					//hide point conditions
					if (isHidePointReached)
					{
						if (!isShot)
							anim.Play("Crouched_Idle"); //play only when not shot at.

						SpeedRun = 0f;
						SpeedWalk = 0f;
						hideSpotTimeOut -= Time.deltaTime * 1;

						if (hideSpotTimeOut <= 0)
						{
							//Instantiate();
							isHidePointReached = false;
							isShootPointReached = true;

							//InstantiateSoldierState();
						}
					}
					else
					{
						//Stop Animation When Shot at
						if (isShot == false && isShootPointReached == false && isHidePointReached == false && !isPlayerClose)
						{
							anim.Play("Crouched_Run");
						}
					}

					//Shoot point conditions
					if (isShootPointReached)
					{
						shootSpotTimeOut -= Time.deltaTime * 1;

						if (shootSpotTimeOut <= 0)
						{
							ShootOfEnemy();
							SpeedRun = 0f;
							SpeedWalk = 0f;
							if (!isShot)
								anim.Play("pistol_firing"); //play only when not shot at.

							ShootCount -= Time.deltaTime * 1;
							if (ShootCount <= 0f)
							{
								isShootPointReached = false;
							}
						}
						else
						{
							if (!isShot)
								anim.Play("pistol_walk");
							SpeedRun = 3f;
							SpeedWalk = 3f;
						}


					}

					if (isShot == false)
					{
						transform.position += transform.forward * Time.deltaTime * SpeedRun;
					}

					if (!isShootPointReached)
					{
						HideSpotView(hideIndex);
					}
					else
					{
						EnemyView();
					}

				}
				else if (hide == false && !isPlayerClose)
				{
					EnemyView();
					hideCountDown = 1f;

					if (isShot == false)
					{
						transform.position += transform.forward * Time.deltaTime * SpeedWalk;
					}
				}
			}
			if (soldier_anim_type == 2)
			{
				if (hide == true && !isPlayerClose)
				{

					//hide = false;
					if (!isShootPointReached && trigger == false)
					{
						isHidePointReached = true;
						hideSpotTimeOut = 4;
						shootSpotTimeOut = 2;
						ShootCount = 2;
					}

					//hide point conditions
					if (isHidePointReached)
					{
						trigger = true;
						if (!isShot)
							anim.Play("L_Hide"); //play only when not shot at.
						EnemyView();

						SpeedRun = 0f;
						SpeedWalk = 0f;
						hideSpotTimeOut -= Time.deltaTime * 1;

						if (hideSpotTimeOut <= 0)
						{
							//Instantiate();
							isHidePointReached = false;
							isShootPointReached = true;
							trigger = false;
							InstantiateSoldierState();
						}
					}

					//Shoot point conditions
					if (isShootPointReached)
					{
						shootSpotTimeOut -= Time.deltaTime * 1;

						ShootOfEnemy();
						SpeedRun = 0f;
						SpeedWalk = 0f;

						if (!isShot)
							anim.Play("pistol_firing"); //play only when not shot at.

						ShootCount -= Time.deltaTime * 1;
						if (ShootCount <= 0f)
						{
							isShootPointReached = false;
							trigger = false;
						}

					}

					if (isShot == false && !isHidePointReached && !isShootPointReached)
					{
						transform.position += transform.forward * Time.deltaTime * SpeedRun;
					}

					if (!isShootPointReached)
					{
						//HideSpotView(hideIndex);
					}
					else
					{
						EnemyView();
					}

				}
				else if (hide == false && !isPlayerClose)
				{
					EnemyView();
					hideCountDown = 1f;

					if (isShot == false)
					{
						transform.position += transform.forward * Time.deltaTime * SpeedWalk;
					}
				}
			}
			if (soldier_anim_type == 3)
			{
				if (hide == true && !isPlayerClose)
				{

					//hide = false;
					if (!isShootPointReached && trigger == false)
					{
						isHidePointReached = true;
						hideSpotTimeOut = 3;
						shootSpotTimeOut = 2;
						ShootCount = 2;
						call_time = 0.9f;
						roll_time = 1.5f;

					}

					//hide point conditions
					if (isHidePointReached)
					{
						trigger = true;
						if (!isShot)
						{
							if (call_time >= 0)
							{
								EnemyView();
								call_time -= Time.deltaTime * 1;
								anim.Play("call"); //play only when not shot at.
								CallSound.loop = true;
								CallSound.PlayDelayed(1);
							}
							else
							{
								CallSound.Stop();
								if (roll_time >= 0)
								{
									roll_time -= Time.deltaTime * 1;

									if (direction == 0)
									{
										anim.Play("run_left"); //play only when not shot at.
									}
									else if (direction == 1)
									{
										anim.Play("straffing_left");
									}

								}
								else
								{
									anim.Play("run_back");
								}
							}
						}



						SpeedRun = 0f;
						SpeedWalk = 0f;
						hideSpotTimeOut -= Time.deltaTime * 1;

						if (hideSpotTimeOut <= 0)
						{
							//Instantiate();
							isHidePointReached = false;
							isShootPointReached = true;
							trigger = false;
							InstantiateSoldierState();
						}
					}

					//Shoot point conditions
					if (isShootPointReached)
					{
						shootSpotTimeOut -= Time.deltaTime * 1;

						ShootOfEnemy();
						SpeedRun = 0f;
						SpeedWalk = 0f;

						if (!isShot)
							anim.Play("pistol_firing"); //play only when not shot at.

						ShootCount -= Time.deltaTime * 1;
						if (ShootCount <= 0f)
						{
							isShootPointReached = false;
							trigger = false;
						}

					}

					if (isShot == false && !isHidePointReached && !isShootPointReached)
					{
						transform.position += transform.forward * Time.deltaTime * SpeedRun;
					}

					if (!isShootPointReached)
					{
						//HideSpotView(hideIndex);
					}
					else
					{
						EnemyView();
					}

				}
				else if (hide == false && !isPlayerClose)
				{
					EnemyView();
					hideCountDown = 1f;

					if (isShot == false)
					{
						transform.position += transform.forward * Time.deltaTime * SpeedWalk;
					}
				}
			}

			//Same state duplicate
			if (soldier_anim_type == 4)
			{
				if (hide == true && !isPlayerClose)
				{

					if (Vector3.Distance(this.transform.position, hideSpots[hideIndex].transform.position) < 2 && !isHidePointReached && !isShootPointReached)
					{
						//hide = false;
						isHidePointReached = true;
						hideSpotTimeOut = 4;
						shootSpotTimeOut = 2;
						ShootCount = 2;

					}

					//hide point conditions
					if (isHidePointReached)
					{
						if (!isShot)
							anim.Play("Crouched_Idle"); //play only when not shot at.

						SpeedRun = 0f;
						SpeedWalk = 0f;
						hideSpotTimeOut -= Time.deltaTime * 1;

						if (hideSpotTimeOut <= 0)
						{
							//Instantiate();
							isHidePointReached = false;
							isShootPointReached = true;
							InstantiateSoldierState();
						}
					}
					else
					{
						//Stop Animation When Shot at
						if (isShot == false && isShootPointReached == false && isHidePointReached == false && !isPlayerClose)
						{
							//anim.Play("pistol_walk");
							anim.Play("Crouched_Run");
						}
					}

					//Shoot point conditions
					if (isShootPointReached)
					{
						shootSpotTimeOut -= Time.deltaTime * 1;

						if (shootSpotTimeOut <= 0)
						{
							ShootOfEnemy();
							SpeedRun = 0f;
							SpeedWalk = 0f;
							if (!isShot)
								anim.Play("pistol_firing"); //play only when not shot at.

							ShootCount -= Time.deltaTime * 1;
							if (ShootCount <= 0f)
							{
								isShootPointReached = false;
							}
						}
						else
						{
							if (!isShot)
								anim.Play("pistol_walk");
							SpeedRun = 3f;
							SpeedWalk = 3f;
						}


					}

					if (isShot == false)
					{
						transform.position += transform.forward * Time.deltaTime * SpeedRun;
					}

					if (!isShootPointReached)
					{
						HideSpotView(hideIndex);
					}
					else
					{
						EnemyView();
					}

				}
				else if (hide == false && !isPlayerClose)
				{
					EnemyView();
					hideCountDown = 1f;

					if (isShot == false)
					{
						transform.position += transform.forward * Time.deltaTime * SpeedWalk;
					}
				}
			}
			if (soldier_anim_type == 5)
			{
				if (hide == true && !isPlayerClose)
				{

					//hide = false;
					if (!isShootPointReached && trigger == false)
					{
						isHidePointReached = true;
						hideSpotTimeOut = 4;
						shootSpotTimeOut = 2;
						ShootCount = 2;
					}

					//hide point conditions
					if (isHidePointReached)
					{
						trigger = true;
						if (!isShot)
							anim.Play("L_Hide"); //play only when not shot at.
						EnemyView();

						SpeedRun = 0f;
						SpeedWalk = 0f;
						hideSpotTimeOut -= Time.deltaTime * 1;

						if (hideSpotTimeOut <= 0)
						{
							//Instantiate();
							isHidePointReached = false;
							isShootPointReached = true;
							trigger = false;
							InstantiateSoldierState();
						}
					}

					//Shoot point conditions
					if (isShootPointReached)
					{
						shootSpotTimeOut -= Time.deltaTime * 1;

						ShootOfEnemy();
						SpeedRun = 0f;
						SpeedWalk = 0f;

						if (!isShot)
							anim.Play("pistol_firing"); //play only when not shot at.

						ShootCount -= Time.deltaTime * 1;
						if (ShootCount <= 0f)
						{
							isShootPointReached = false;
							trigger = false;
						}

					}

					if (isShot == false && !isHidePointReached && !isShootPointReached)
					{
						transform.position += transform.forward * Time.deltaTime * SpeedRun;
					}

					if (!isShootPointReached)
					{
						//HideSpotView(hideIndex);
					}
					else
					{
						EnemyView();
					}

				}
				else if (hide == false && !isPlayerClose)
				{
					EnemyView();
					hideCountDown = 1f;

					if (isShot == false)
					{
						transform.position += transform.forward * Time.deltaTime * SpeedWalk;
					}
				}
			}
			if (soldier_anim_type == 6)
			{
				if (hide == true && !isPlayerClose)
				{

					//hide = false;
					if (!isShootPointReached && trigger == false)
					{
						isHidePointReached = true;
						hideSpotTimeOut = 3;
						shootSpotTimeOut = 2;
						ShootCount = 2;
						call_time = 0.9f;
						roll_time = 1f;
					}

					//hide point conditions
					if (isHidePointReached)
					{
						trigger = true;
						if (!isShot)
						{
							if (call_time >= 0)
							{
								EnemyView();
								call_time -= Time.deltaTime * 1;
								anim.Play("call"); //play only when not shot at.
								CallSound.loop = true;
								CallSound.PlayDelayed(1);
							}
							else
							{
								CallSound.Stop();
								if (roll_time >= 0)
								{
									roll_time -= Time.deltaTime * 1;

									if (direction == 0)
									{
										anim.Play("run_left"); //play only when not shot at.
									}
									else if (direction == 1)
									{
										anim.Play("run_left");
									}

								}
								else
								{
									anim.Play("run_back");
								}
							}
						}



						SpeedRun = 0f;
						SpeedWalk = 0f;
						hideSpotTimeOut -= Time.deltaTime * 1;

						if (hideSpotTimeOut <= 0)
						{
							//Instantiate();
							isHidePointReached = false;
							isShootPointReached = true;
							trigger = false;
							InstantiateSoldierState();
						}
					}

					//Shoot point conditions
					if (isShootPointReached)
					{
						shootSpotTimeOut -= Time.deltaTime * 1;

						ShootOfEnemy();
						SpeedRun = 0f;
						SpeedWalk = 0f;

						if (!isShot)
							anim.Play("pistol_firing"); //play only when not shot at.

						ShootCount -= Time.deltaTime * 1;
						if (ShootCount <= 0f)
						{
							isShootPointReached = false;
							trigger = false;
						}

					}

					if (isShot == false && !isHidePointReached && !isShootPointReached)
					{
						transform.position += transform.forward * Time.deltaTime * SpeedRun;
					}

					if (!isShootPointReached)
					{
						//HideSpotView(hideIndex);
					}
					else
					{
						EnemyView();
					}

				}
				else if (hide == false && !isPlayerClose)
				{
					EnemyView();
					hideCountDown = 1f;

					if (isShot == false)
					{
						transform.position += transform.forward * Time.deltaTime * SpeedWalk;
					}
				}
			}

		}
	}

	private void playFirstWalkAnim()
	{
		if (walkRandomEnabler == true)
		{
			fst_walk_randomiser = Random.Range(1, 4);
			walkRandomEnabler = false;
		}

		if (activeScene == "Outdoor_FOREST")
		{
			if (walkRandomEnabler == true)
			{
				fst_walk_randomiser = Random.Range(1, 2);
				walkRandomEnabler = false;
			}

			anim.Play("walking");    //Starting state
		}
		else
		{
			if (walkRandomEnabler == true)
			{
				fst_walk_randomiser = Random.Range(1, 4);
				walkRandomEnabler = false;
			}

			if (fst_walk_randomiser == 1)
			{
				anim.Play("walking");    //Starting state
			}
			else if (fst_walk_randomiser == 2 || fst_walk_randomiser == 4)
			{
				anim.Play("confident_walk");    //Starting state
			}
			else if (fst_walk_randomiser == 3)
			{
				anim.Play("happy_walk");    //Starting state
			}
		}

	}

	private void updateAnimTimer()
	{
		anim_change_timer = 40;
		SpeedWalk = 2;
	}

	void killing()
	{

		//EnemyView();
		if (TypeOfSoldier == "field")
		{
			if (distance <= EnemyChargeDistance || hide)
			{
				shootingPrecision = (int)(shootingPrecision / 2);

				//Stop Animation When Shot at
				if (isShot == false && isShootPointReached == false && isHidePointReached == false && !isPlayerClose)
				{
					//anim.Play("pistol_walk");
					//anim.Play("Crouched_Run");
				}


				//ShootOfEnemy();
				Move();
			}
			else if (((distance > EnemyChargeDistance && distance <= EnemyShootFromFarDistance) && !hide) || isPlayerClose)
			{
				anim.Play("pistol_firing");
				ShootOfEnemy();

				hideCountDown -= Time.deltaTime * 2;

				if (hideCountDown <= 0f)
				{
					hide = true;

					if (!isPlayerClose)
						Move();
				}

			}
			else
			{
				changeState(states.patrol);
			}
		}
		if (TypeOfSoldier == "sniper")
		{

			if (distance <= EnemyChargeDistance || hide)
			{
				shootingPrecision = (int)(shootingPrecision / 2);

				//Stop Animation When Shot at
				if (isShot == false && isShootPointReached == false && isHidePointReached == false && !isPlayerClose)
				{
					//anim.Play("shooting_rifle_walk");
					//anim.Play("Crouched_Run");
				}

				//ShootOfEnemy();
				Move();
			}
			else if (((distance > EnemyChargeDistance && distance <= EnemyShootFromFarDistance) && !hide) || isPlayerClose)
			{
				//anim.Play("rifle_firing");
				//ShootOfEnemy();
				anim.Play("pistol_firing");
				ShootOfEnemy();

				hideCountDown -= Time.deltaTime * 2;

				if (hideCountDown <= 0f)
				{
					hide = true;
					Move();
				}
			}
			else
			{
				changeState(states.patrol);
			}
		}
	}
	void patrol()
	{
		if (!isPlayerClose)
		{
			if (!seen)
			{
				civ_interaction();
			}
			else
			{
				gun1.SetActive(true);
				//gun2.SetActive(false);
				if (isShot == false)
				{
					anim.Play("pistol_walk");
				}

				Move();
				changeState(states.killing);
				//if (distance <= EnemyChargeDistance)
				//{
				//	seen = true;
				//	changeState(states.killing);
				//}
			}
		}

	}

	void civ_interaction()
	{
		gun1.SetActive(false);
		Move();

		if (distance <= EnemyChargeDistance)
		{
			seen = true;
			changeState(states.killing);
			EnemyView();
		}
	}

	void HideSpotView(int elementIndex)
	{
		Vector3 LookDir = hideSpots[elementIndex].transform.position - this.gameObject.transform.position;
		LookDir.y = 0;
		transform.LookAt(this.gameObject.transform.position + LookDir, Vector3.up);
	}
	void Instantiate()
	{
		if (hideIndex != hideSpots.Length - 1)
		{
			hideIndex++;
			//isShootPointReached = false;
		}
		if (hideIndex == hideSpots.Length - 1)
		{

			//isShootPointReached = true;
			isHidePointReached = false;

			hideIndex = 0;
		}
	}

}

//Sample Codes
/*if (isShootPointReached) //shooting point conditions
				{
					
					if (Vector3.Distance(this.transform.position, hideSpots[hideIndex].transform.position) < 1)
					{
						SpeedRun = 0f;
						SpeedWalk = 0f;
						EnemyView();
						ShootOfEnemy();
						anim.Play("shooting_rifle");
					}
					else
                    {
						SpeedRun = 3f;
						SpeedWalk = 2f;
						anim.Play("pistol_walk");
                    }

					shootSpotTimeOut -= Time.deltaTime * 1;
					if (shootSpotTimeOut <= 0)
					{
						Instantiate();
						isShootPointReached = false;
					}
				}
				if (!isShootPointReached && !isHidePointReached)
				{
					SpeedRun = 3f;
					SpeedWalk = 2f;
				}*/

