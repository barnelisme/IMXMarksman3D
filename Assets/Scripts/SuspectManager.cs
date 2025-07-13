using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SuspectManager : MonoBehaviour
{
    [SerializeField] private GameObject[] suspects;
    private GameObject[] emptyArray;
    [SerializeField] private List<GameObject> randomSuspects = new List<GameObject>();
    [SerializeField] private TextMeshProUGUI limitText;
    private bool isSuspectReset = false;
    private float level = 1;

    private float suspectResetTimer = 0;
    private float limitTimer = 0;
    private float overTimer = 0;
    private float STimerValue = 1f; // seconds
    
    void Start()
    {
        suspects = emptyArray; // Reset Array
        suspectResetTimer = STimerValue;
        StaticVariableManager.isSuspectsSet = false;
        StaticVariableManager.numberSuspectsHit = 0;
        StaticVariableManager.numberNoneSuspectsHit = 0;
        StaticVariableManager.limitOverTime = 0;
        limitTimer = StaticVariableManager.limitTime;
    }                         

    // Update is called once per frame
    void Update()
    {
        if(countDownStart.start_training)
        {

            if(!StaticVariableManager.isSuspectsSet)
            {
                suspectResetTimer -= Time.deltaTime;

                if (suspectResetTimer <= 0f)
                {
                    SetSuspects();
                    suspectResetTimer = STimerValue;
                }
            }
            else
            {
                limitTimer -= Time.deltaTime;
                limitText.text = "Time Limit: " + limitTimer.ToString("0");

                if(limitTimer < 0)
                {
                    overTimer += Time.deltaTime;
                    StaticVariableManager.limitOverTime = overTimer;
                }
            }

            if (StaticVariableManager.trainingLevelReset)
            {
                SetSuspects();

                StaticVariableManager.trainingLevelReset = false;
            }

        }

    }

    private void SetSuspects()
    {
        // hit.transform.gameObject.GetComponent<SuspectShootAnimator>().SendMessage("ApplyDamage", hit.transform.name);
        GetSuspects();

        if (StaticVariableManager.isSuspectsSet == false && isSuspectReset == false && !StaticVariableManager.trainingLevelReset && suspects.Length > 0)
        {

            int randomIndex = 0;
            List<int> usedIndexes = new List<int>();
            randomSuspects.Clear();

            level = StaticVariableManager.trainingLevel;
            StaticVariableManager.numberSuspectsHit = 0;
            // Adjust level
            if (level > suspects.Length)
            {
                level = suspects.Length;
            }

            for (int i = 0; i < level; i ++)
            {

                do
                {

                    randomIndex = Random.Range(0, suspects.Length);

                } while (usedIndexes.Contains(randomIndex));

                usedIndexes.Add(randomIndex);

                randomSuspects.Add(suspects[randomIndex]);
            }

            foreach(GameObject suspect in randomSuspects)
            {
                suspect.transform.name += ".suspect";
                suspect.transform.gameObject.GetComponent<SuspectShootAnimator>().SendMessage("ActivateSuspision", suspect.transform.name);
                suspect.GetComponent<SuspectShootAnimator>().gun1.SetActive(true);
            }

            if(randomSuspects.Count < StaticVariableManager.trainingLevel)
            {
                StaticVariableManager.numberSuspectsHit = StaticVariableManager.trainingLevel - randomSuspects.Count;
            }
       
            StaticVariableManager.isSuspectsSet = true;

        }

        isSuspectReset = false;
    }

    private void GetSuspects()
    {
        suspects = emptyArray; // Reset Array
        suspects = GameObject.FindGameObjectsWithTag("soldier");
        limitTimer = StaticVariableManager.limitTime;

        if(suspects.Length == 0)
        {
            suspects = GameObject.FindGameObjectsWithTag("dead_soldier");
            suspectResetTimer = STimerValue;
            isSuspectReset = true;

            foreach (GameObject suspect in suspects)
            {
                suspect.GetComponent<CapsuleCollider>().enabled = true;
                suspect.GetComponent<SuspectShootAnimator>().anim.Play("idle");
                suspect.GetComponent<SuspectShootAnimator>().gun1.SetActive(false);
                suspect.transform.tag = "soldier";
                suspect.transform.name = "soldier";
            }

        }
        else if(StaticVariableManager.trainingLevelReset)
        {
            StaticVariableManager.isSuspectsSet = false;
            suspectResetTimer = STimerValue + 2;

            foreach (GameObject suspect in suspects)
            {
                suspect.transform.tag = "soldier";
                suspect.transform.name = "soldier";
                suspect.GetComponent<SuspectShootAnimator>().anim.Play("idle");
                suspect.GetComponent<SuspectShootAnimator>().gun1.SetActive(false);
            }
        }

    }
}
