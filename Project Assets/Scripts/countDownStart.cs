using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class countDownStart : MonoBehaviour
{

    public GameObject watchCanvaLaser;
    public GameObject watchCanvaLive;
    public TextMeshProUGUI LasertimeTxt;
    public TextMeshProUGUI LivetimeTxt;
    public TextMeshProUGUI InstructionTxt;
    private float timeCount = 6f;
    float closeCanvTimer = 1f;
    public static bool start_training = false;

    string activeScene = " ";
    bool isStartButtonPressed = false;
    public AudioSource startBeepSource;
    public AudioSource buzzerSource;
    public AudioClip startBeepClip;
    public AudioClip buzzerClip;
    private bool isBeepPlayed = false;
    private bool isBuzzerPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        activeScene = activeScene = SceneManager.GetActiveScene().name;
        setUIStartSetting();
        start_training = false;

        LasertimeTxt.fontSize = 75;
        LasertimeTxt.text = "PREPARE";

        LivetimeTxt.fontSize = 75;
        LivetimeTxt.text = "PREPARE";

        if (activeScene == "CalibrationTest" || Scoring.ammo_setting == " " || activeScene.ToLower().Contains("range") || activeScene.ToLower().Contains("hunting") ) //training
        {
            start_training = true;
        }
        else 
        {
            if (Scoring.ammo_setting.ToLower().Contains("live"))
            {
                watchCanvaLaser.SetActive(true);
                watchCanvaLive.SetActive(false);
                timeCount = StaticVariableManager.start_counter;
                closeCanvTimer = 1.5f;
                start_training = false;
            }
            else
            {
                watchCanvaLaser.SetActive(true);
                watchCanvaLive.SetActive(false);
                timeCount = StaticVariableManager.start_counter;
                closeCanvTimer = 1.5f;
                start_training = false;
            }
        }

        LoadSounds();
        //timeCount = 3f;
    }

    // Update is called once per frame
    void Update()
    {

        if(StaticVariableManager.isStopTraining == false && StaticVariableManager.startCountDown)
        {
            if (Keyboard.current.enterKey.isPressed)
            {
                isStartButtonPressed = true;
            }


            if (Scoring.ammo_setting.ToLower().Contains("live"))
            {

                isStartButtonPressed = true;
                if (isStartButtonPressed)
                {
                    if (start_training == false)
                    {
                        countDownLaserAmmo();
                    }
                    else
                    {
                        watchCanvaLaser.SetActive(false);
                    }
                }

            }
            else if (Scoring.ammo_setting.ToLower().Contains("laser"))
            {
                isStartButtonPressed = true;
                if (isStartButtonPressed)
                {
                    if (start_training == false)
                    {
                        countDownLaserAmmo();
                    }
                    else
                    {
                        watchCanvaLaser.SetActive(false);
                    }
                }

                if (timeCount <= 0.9)
                {


                }
            }
        }
        else if (StaticVariableManager.isStopTraining)
        {
            watchCanvaLaser.SetActive(false);
        }
    }
    private void LoadSounds()
    {
        startBeepSource = gameObject.AddComponent<AudioSource>();
        startBeepSource.clip = startBeepClip;
        buzzerSource = gameObject.AddComponent<AudioSource>();
        buzzerSource.clip = buzzerClip;
    }
    public void countDownLaserAmmo()
    {
        timeCount -= Time.deltaTime * 1;
        if (timeCount >= 1)
        {
            LasertimeTxt.fontSize = 125;
            LasertimeTxt.text = timeCount.ToString("0");
            if(timeCount <= 3.5f)
            {
                if (!isBeepPlayed)
                {
                    startBeepSource.Play();
                    isBeepPlayed = true;
                }
            }
        }
        else if (timeCount <= 0)
        {
            LasertimeTxt.text = "GO";
            if(!isBuzzerPlayed)
            {
                buzzerSource.Play();
                isBuzzerPlayed = true;
            }
            closeCanvTimer -= Time.deltaTime * 1;

            if (closeCanvTimer <= 0f)
            {
                start_training = true;
                watchCanvaLaser.SetActive(false);
            }
        }
    }
    public void countDownLiveAmmo()
    {
        if (timeCount >= -1)
        {
            
            if (timeCount <= 1.5f)
            {
                LivetimeTxt.fontSize = 125;
                LivetimeTxt.text = "GO";
            }

            timeCount = timeCount - Time.deltaTime * 1;
        }
    }
    public void Pause()
    {
        Time.timeScale = 0f;

    }
    public void Resume()
    {
        Time.timeScale = 1f;
    }

    private void setUIStartSetting()
    {
        if (Scoring.ammo_setting.ToLower().Contains("live") || Scoring.ammo_setting.ToLower().Contains("infrared"))
        {
            //InstructionTxt
            LasertimeTxt.color = Color.white;
            InstructionTxt.color = Color.white;
        }
        else
        {
            LasertimeTxt.color = Color.black;
            InstructionTxt.color = Color.black;
        }
    }
}
