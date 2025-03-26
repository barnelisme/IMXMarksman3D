using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieTargetController : MonoBehaviour
{

    public List<GameObject> targets = new List<GameObject>();                         

    public void initialiseTargets()
    {
        foreach(GameObject target in targets)
        {
            target.SetActive(true);
            target.GetComponent<MeshCollider>().enabled = false;
        }
    }
}
