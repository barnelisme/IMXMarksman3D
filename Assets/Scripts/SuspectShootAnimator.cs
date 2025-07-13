using UnityEngine;

public class SuspectShootAnimator : MonoBehaviour
{
    [SerializeField] public GameObject gun1;

    private Rigidbody rigidBd;
    public Animator anim;
    Transform mainPlayerTransform;
    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainPlayerTransform = player.transform;
        anim = this.GetComponent<Animator>();
        rigidBd = GetComponent<Rigidbody>();
        gun1.SetActive(false);
        EnemyView();
    }

    // Update is called once per frame
    void Update()
    {
        // anim.Play("idle");
        // anim.Play("pistol_firing");
        // anim.Play("gun_hit_reaction");
        // anim.Play("gun_hit_low_reaction");
        // anim.Play("dying");
        // anim.Play("talk");
    }

	private void ApplyDamage(string tagged)
	{

        if (transform.name == tagged)
		{
            anim.Play("dying");
            this.GetComponent<Rigidbody>().useGravity = false;
            this.GetComponent<CapsuleCollider>().enabled = false;
            this.transform.tag = "dead_soldier";
            gun1.SetActive(false);
            Shooting.enemyshot++;
            // anim.Play("pistol_firing");
            if (transform.name.ToLower().Contains("suspect"))
            {
                StaticVariableManager.numberSuspectsHit++;

                if(StaticVariableManager.numberSuspectsHit >= StaticVariableManager.trainingLevel)
                {
                    StaticVariableManager.isSuspectsSet = false;
                }
            }
            else
            {
                StaticVariableManager.numberNoneSuspectsHit++;
            }
        }
		else if (tagged.Contains("change"))
		{

		}

		//print("Number Of Hits:" + NumberOfHits);
	}

    private void ActivateSuspision(string tagged)
    {

        if (transform.name == tagged)
        {
            //anim.Play("dying");
            anim.Play("firing");
            EnemyView();
        }
        else if (tagged.Contains("change"))
        {

        }

        //print("Number Of Hits:" + NumberOfHits);
    }

    private void EnemyView()
    {
        //if(distance <= EnemyShootFromFarDistance)
        //{
        //	Vector3 LookDir = mainPlayer.position - this.gameObject.transform.position;
        //	LookDir.y = 0;
        //	transform.LookAt(this.gameObject.transform.position + LookDir, Vector3.up);
        //}//end of void EnemyView()
        Vector3 LookDir = player.transform.position - this.gameObject.transform.position;
        LookDir.y = 0;
        transform.LookAt(this.gameObject.transform.position + LookDir, Vector3.up);
        //print("Enemy View");
    }
}
