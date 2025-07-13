using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class globalTimeButtonManager : MonoBehaviour
{
    public GameObject warningCanva;
    string activeScene = "";
    bool warningActivated = false;
    bool rebootCamera = false;
    public GameObject global_time_warning;
    public GameObject global_static_time_warning;

    // Start is called before the first frame update
    void Start()
    {
        rebootCamera = login_Manager.rebootCamera;
        activeScene = SceneManager.GetActiveScene().name;
        //login_Manager.global_active_timer = 304;
    }

    // Update is called once per frame
    void Update()
    {
        manageGlobalActiveTime();
        manageCameraActiveTime();
        if (warningActivated)
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            {
                okButton();
            }
        }
    }

    private void manageCameraActiveTime()
    {
        if (rebootCamera == false)
        {
            login_Manager.global_Camera_active_timer -= Time.deltaTime * 1;
            if (login_Manager.global_Camera_active_timer <= 0f)
            {
                login_Manager.rebootCamera = true;
                rebootCamera = true;
            }
        }
    }

    private void manageGlobalActiveTime()
    {
        login_Manager.global_active_timer -= Time.deltaTime * 1;

        if (login_Manager.global_active_timer <= 300 && warningActivated == false)
        {
            if (activeScene.ToLower().Contains("basic") || activeScene.ToLower().Contains("range"))
            {
                global_time_warning.SetActive(true);
                warningActivated = true;
            }
            else if (activeScene.ToLower().Contains("mall") || activeScene.ToLower().Contains("forest") || activeScene.ToLower().Contains("plain") || activeScene.ToLower().Contains("parking") || activeScene.ToLower().Contains("restaurant"))
            {
                global_static_time_warning.SetActive(true);
                warningActivated = true;
            }

        }
        if (login_Manager.global_active_timer <= 0)
        {
            SceneManager.LoadScene("LOGIN");
        }
    }

    public void okButton()
    {
        global_time_warning.SetActive(false);
        global_static_time_warning.SetActive(false);
    }

    public void logoutButton()
    {
        SceneManager.LoadScene("LOGIN");
    }
}
