using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CupStartManager : MonoBehaviour
{
    public List<GameObject> startPoints = new List<GameObject>();
    int randomIndex;
    string indexPoints = "";
    bool cup_init_complete = false;

    void Start()
    {

    }

    void Update()
    {
        if(StaticVariableManager.reInitialise_1 == true )
        {
            manageStart();
        }
    }

    private void manageStart()
    {

        AllocateRandomValues(out StaticVariableManager.startPosition_1, out StaticVariableManager.startPosition_2, out StaticVariableManager.startPosition_3);

        StaticVariableManager.cup_init_complete = false;
        print("Re: pos - " + StaticVariableManager.startPosition_1 + " . " + StaticVariableManager.startPosition_2 + " . " + StaticVariableManager.startPosition_3);
        //print("Setup: complete");
    }

    void AllocateRandomValues(out int var1, out int var2, out int var3)
    {
        List<int> values = new List<int> { 0, 1, 2 };
        var1 = Random.Range(1, 3); // Randomly select value for var1

        // Remove var1 from the list to ensure var2 and var3 are different
        values.Remove(var1);

        var2 = values[Random.Range(0, values.Count)]; // Randomly select value for var2

        // Remove var2 from the list to ensure var3 is different
        values.Remove(var2);

        var3 = values[0]; // Whatever is left is assigned to var3
    }

    public bool ContainsDigits(int number, int digit1, int digit2)
    {
        // Convert number to string
        string numStr = number.ToString();

        // Check if both digits are in the string representation of the number
        return numStr.Contains(digit1.ToString()) || numStr.Contains(digit2.ToString());
    }

    public bool Contains1Digits(int number, int digit1)
    {
        // Convert number to string
        string numStr = number.ToString();

        // Check if both digits are in the string representation of the number
        return numStr.Contains(digit1.ToString());
    }

    public bool Contains2Digits(int number, int digit1, int digit2)
    {
        // Convert number to string
        string numStr = number.ToString();

        // Check if both digits are in the string representation of the number
        return numStr.Contains(digit1.ToString()) || numStr.Contains(digit2.ToString());
    }
}
