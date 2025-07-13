using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameSetting : MonoBehaviour
{
    public TMP_Dropdown gameSetting;

    // Start is called before the first frame update
    void Start()
    {
        gameSetting.options.Add(new TMP_Dropdown.OptionData() { text = "Normal" });
        gameSetting.options.Add(new TMP_Dropdown.OptionData() { text = "Live" });
    }

    public void HandleInputData(int val)
    {
        print(val);
        switch (val)
        {
            case 0:
                Scoring.ammo_setting = "Normal";
                break;

            case 1:
                Scoring.ammo_setting = "Live";
                break;

        }

        if (val != 1 || val != 2)
        {
            val = 1;
        }

    }

}
