using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;

public class LoadTVTheory : MonoBehaviour
{
    Vector3 positionIndoorTV1 = new Vector3(619.83f, 1.69f, 552.2f);
    Vector3 positionIndoorTV2 = new Vector3(623.479f, 1.69f, 552.2f);
    Vector3 positionIndoorTV3 = new Vector3(626.7042f, 1.69f, 552.2f);

    GameObject[] all_targets;
    public GameObject TV_Grip;
    public GameObject TV_Sight_Alignment;
    public GameObject TV_Good_Stance;
    public GameObject TV_Bad_Stance;
    public GameObject TV_rifle;

    public AudioSource audioSource_grip;
    public AudioSource audioSource_sightAlignment;
    public AudioSource audioSource_Good_Stance;
    public AudioSource audioSource_Bad_Stance;
    public AudioSource audioSource_rifle;


    public AudioClip audioClip_grip;   
    public AudioClip audioClip_sightAlignment;
    public AudioClip audioClip_Good_Stance;
    public AudioClip audioClip_Bad_Stance;
    public AudioClip audioClip_rifle;

    public TextMeshProUGUI distanceText;

    private MouseLook[] mous;
    private FirstPersonController[] fps;

    int index = 0;
    int distance;

    List<string> tvNames = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        tvNames.Add("TV_Grip");
        tvNames.Add("TV_Sight_Alignment");
        tvNames.Add("TV_good_stance");
        tvNames.Add("TV_bad_stance");
        tvNames.Add("TV_rifle_positioning");
        LoadSounds();

    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene("MainMenu");
            }
            if (SceneManager.GetActiveScene().name.ToLower().Contains("range")&& !SceneManager.GetActiveScene().name.ToLower().Contains("moving")  && SceneManager.GetActiveScene().name.ToLower().Contains("theory"))
            {
                if (Input.GetKey(KeyCode.T))//change target
                {
                    Thread.Sleep(500);
                    Scoring.ResetRange();
                    all_targets = GameObject.FindGameObjectsWithTag("tv");
                    StopAllSounds();

                    foreach (GameObject target in all_targets)
                    {
                        Destroy(target);
                    }
                    GameObject[] bulletholes = GameObject.FindGameObjectsWithTag("bullethole");
                    foreach (GameObject bholes in bulletholes)
                    {
                        Destroy(bholes);
                    }

                    if (tvNames[index] == "TV_Grip")
                    {

                        GameObject  obj = Instantiate(TV_Grip, positionIndoorTV2, Quaternion.identity);
                        obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                        audioSource_grip.Play();
                       
                    }
                    else if (tvNames[index] == "TV_Sight_Alignment")
                    {

                        GameObject  obj = Instantiate(TV_Sight_Alignment, positionIndoorTV2, Quaternion.identity);
                        obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                        audioSource_sightAlignment.Play();

                    }
                    else if (tvNames[index] == "TV_good_stance")
                    {
                        GameObject  obj = Instantiate(TV_Good_Stance, positionIndoorTV2, Quaternion.identity);
                        obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                        audioSource_Good_Stance.Play();
                    }
                    else if (tvNames[index] == "TV_bad_stance")
                    {
                        GameObject  obj = Instantiate(TV_Bad_Stance, positionIndoorTV2, Quaternion.identity);
                        obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                        audioSource_Bad_Stance.Play();
                    }
                    else if (tvNames[index] == "TV_rifle_positioning")
                    {

                        GameObject  obj = Instantiate(TV_rifle, positionIndoorTV2, Quaternion.identity);
                        obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                        audioSource_rifle.Play();
                     
                    }
                    if (index < tvNames.Count - 1)
                    {
                        index++; 
                    }
                    else
                    {
                        index = 0;
                    }
                }
                distance = (int)Vector3.Distance(transform.position, positionIndoorTV2);
                //distance = (int)distance;
                distanceText.text = "Distance: " + (distance).ToString();
                /*if (Input.GetKey(KeyCode.UpArrow))
                {
                    Thread.Sleep(100);
                    if (distance == 5)
                    {
                        return;
                    }

                    else if(distance!=50)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + ((1 / 2) * distance));
                    }
                    else if (distance == 50 )
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 30);
                    }
                }*/

                if (Input.GetKey(KeyCode.UpArrow))
                {
                    Thread.Sleep(100);
                    if (distance == 5)
                    {
                        return;
                    }

                    else if (distance != 50)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (distance / 2));
                    }
                    else if (distance == 50)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 30);
                    }

                }


                if (Input.GetKey(KeyCode.DownArrow))
                {
                    Thread.Sleep(100);
                    if (distance == 100)
                    {
                        return;
                    }

                    else if (distance != 20)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - distance);
                    }
                    else if (distance == 20)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 30);
                    }

                }
            }
            
            
        }
        catch(Exception ex)
        {
            Scoring.logs += "\n" + ex.Message + ":" + ex.StackTrace;
            Scoring.writeLog("LoadTVTheory Update:" + ex.StackTrace);
            Debug.LogError("LoadTVTheory Update:" + ex.StackTrace);
        }

    }
    void LoadSounds()
    {
        audioSource_grip = gameObject.AddComponent<AudioSource>();
        audioSource_grip.clip = audioClip_grip;

        audioSource_sightAlignment = gameObject.AddComponent<AudioSource>();
        audioSource_sightAlignment.clip = audioClip_sightAlignment;

        audioSource_Good_Stance = gameObject.AddComponent<AudioSource>();
        audioSource_Good_Stance.clip = audioClip_Good_Stance;

        audioSource_Bad_Stance = gameObject.AddComponent<AudioSource>();
        audioSource_Bad_Stance.clip = audioClip_Bad_Stance;

        audioSource_rifle = gameObject.AddComponent<AudioSource>();
        audioSource_rifle.clip = audioClip_rifle;

    }
    void StopAllSounds()
    {
        if(audioSource_grip.isPlaying)
        {
            audioSource_grip.Stop();
        }
        if (audioSource_sightAlignment.isPlaying)
        {
            audioSource_sightAlignment.Stop();
        }
        if (audioSource_Good_Stance.isPlaying)
        {
            audioSource_Good_Stance.Stop();
        }
        if (audioSource_Bad_Stance.isPlaying)
        {
            audioSource_Bad_Stance.Stop();
        }
        if (audioSource_rifle.isPlaying)
        {
            audioSource_rifle.Stop();
        }
    }
}
