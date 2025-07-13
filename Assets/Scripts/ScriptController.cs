using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptController : MonoBehaviour
{

    public GameObject ericCivilian;
    public GameObject oc_ericCivilian;
    //public GameObject gun;
    int selector = 0;

    // Start is called before the first frame update
    void Start()
    {
        selector = 1;// Random.Range(1,4);
        //gun = GameObject.FindGameObjectWithTag("Gun");
    }

    // Update is called once per frame
    void Update()
    {
        Selector();
    }

    void Selector()
    {
        if(selector == 1)
        {
            ericCivilian.GetComponent<civilian_behaviour>().enabled = true;
            oc_ericCivilian.GetComponent<civilian_behaviour>().enabled = true;

            ericCivilian.GetComponent<PoliceBehaviour>().enabled = false;
            oc_ericCivilian.GetComponent<PoliceBehaviour>().enabled = false;
            //gun.SetActive(false);
        }

        if (selector == 2)
        {
            ericCivilian.GetComponent<PoliceBehaviour>().enabled = true;
            oc_ericCivilian.GetComponent<PoliceBehaviour>().enabled = true;

            ericCivilian.GetComponent<civilian_behaviour>().enabled = false;
            oc_ericCivilian.GetComponent<civilian_behaviour>().enabled = false;
        }

        if (selector == 3)
        {
            ericCivilian.GetComponent<civilian_behaviour>().enabled = true;
            oc_ericCivilian.GetComponent<civilian_behaviour>().enabled = true;

            ericCivilian.GetComponent<PoliceBehaviour>().enabled = false;
            oc_ericCivilian.GetComponent<PoliceBehaviour>().enabled = false;
            //gun.SetActive(false);
        }

        if (selector == 4)
        {
            ericCivilian.GetComponent<PoliceBehaviour>().enabled = true;
            oc_ericCivilian.GetComponent<PoliceBehaviour>().enabled = true;

            ericCivilian.GetComponent<civilian_behaviour>().enabled = false;
            oc_ericCivilian.GetComponent<civilian_behaviour>().enabled = false;
        }

    }
}
