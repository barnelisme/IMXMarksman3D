using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TVideoGroupSelector : MonoBehaviour
{
    [Header("Video Group Handlers")]
    [SerializeField] private List<GameObject> groupHandlers = new List<GameObject>(); 

    void Start()
    {
        StaticVariableManager.currentVideoGroup = "G13"; // Manual group setting

        foreach (GameObject handler in groupHandlers)
        {
            // Activate the current group handler
            if(handler.transform.name.ToLower().Contains(StaticVariableManager.currentVideoGroup.ToLower()))
            {
                handler.SetActive(true);
            }
        }

    }
}
