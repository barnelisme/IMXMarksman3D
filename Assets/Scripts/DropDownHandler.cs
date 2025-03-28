using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropDownHandler : MonoBehaviour
{
    List<string> names = new List<string>() { "Live", "Game" };

    public TMP_Dropdown dropdown;
    public TMP_Text selectedName;

    public void Dropdown_IndexChanged(int index)
    {
        selectedName.text = names[index];
    }

    // Start is called before the first frame update
    void Start()
    {

        PopulateList();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PopulateList()
    {

        dropdown.AddOptions(names);

    }
}
