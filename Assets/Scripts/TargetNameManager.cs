using UnityEngine;

public class TargetNameManager : MonoBehaviour
{

    public GameObject[] targetSet;
    private bool pepperNameSet = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string name = this.transform.name;

        int index = name.IndexOf("target", System.StringComparison.OrdinalIgnoreCase);
        if (index >= 0 && pepperNameSet == false) // "target" found
        {
            string targetName = name.Substring(index);
            // Debug.Log("Extracted target name: " + targetName);

            foreach (GameObject plate in targetSet)
            {
                plate.transform.name += " " + targetName;
            }

            pepperNameSet = true;
        }
    }

}