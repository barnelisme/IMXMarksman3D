using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ControlCenter : MonoBehaviour
{
    public bool RestartPressed = false;
    public GameObject player;
    public GameObject extraLight;
    string activeScene ;

    private void Start()
    {

        activeScene = SceneManager.GetActiveScene().name;

        if (Scoring.ammo_setting.ToLower().Contains("laser") && !Scoring.ammo_setting.ToLower().Contains("infrared"))
        {
            if(!activeScene.ToLower().Contains("cargame"))
            {
                if (extraLight != null)
                    extraLight.SetActive(false);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Input.mousePosition:" + Input.mousePosition.ToString());
        if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.escapeKey.isPressed)
        {
            RestartScene();
        }

    }

    public void RestartScene()
    {
        //Get active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        RestartPressed = true;
        Time.timeScale = 1f;                     //Reset timescale to normal

        //activate firstPerson movement script
        //player.GetComponent<FirstPersonMovement>().enabled = true;
        //player.GetComponent<FirstPersonController>().enabled = false;
    }

    public void Exit()
    {
        Application.Quit();
    }


}
