using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class WeatherController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Rain maker")]
    public GameObject rainMaker;

    [Header("Rain maker")]
    public List<GameObject> cameras = new List<GameObject>();

    [Header("Sky boxes")]
    public List<Material> skyBoxes = new List<Material>();

    [Header("Light")]
    public GameObject environmentLight;

    // Player
    private GameObject mainCamera;

    void Start()
    {
        // Manager cameras
        cameras = GameObject.FindGameObjectsWithTag("MainCamera").ToList();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        if (TestConditionsManager.timeOfDay == "Day")
        {
            foreach (GameObject camera in cameras)
            {
                // assign the skybox skyBoxes[1] to the camera skybox component
                Skybox sky = camera.GetComponent<Skybox>();
                if (sky != null)
                {
                    sky.material = skyBoxes[0];
                }
            }
        }
        else if (TestConditionsManager.timeOfDay == "Night")
        {
            if (environmentLight != null) environmentLight.SetActive(false);

            foreach (GameObject camera in cameras)
            {
                // assign the skybox skyBoxes[1] to the camera skybox component
                Skybox sky = camera.GetComponent<Skybox>();
                if (sky != null)
                {
                    sky.material = skyBoxes[2];
                }
            }
        }

        if (TestConditionsManager.seasonWeather == "Rain")
        {
            if(environmentLight != null) environmentLight.SetActive(true);
            if (rainMaker != null) rainMaker.SetActive(true);

            foreach (GameObject camera in cameras)
            {
                // assign the skybox skyBoxes[1] to the camera skybox component
                Skybox sky = camera.GetComponent<Skybox>();
                if (sky != null && TestConditionsManager.timeOfDay == "Day")
                {
                    sky.material = skyBoxes[1];
                    // Drop post processing
                    if (mainCamera != null)
                    {
                        PostProcessVolume volume = mainCamera.GetComponent<PostProcessVolume>();
                        if (volume != null)
                        {
                            volume.weight = .2f;   // Drop the weight to zero
                        }
                    }

                    // Drop environment light
                    environmentLight.GetComponent<Light>().intensity = 1;
                }
                else if (TestConditionsManager.timeOfDay == "Night")
                {
                    // Drop post processing
                    if(mainCamera != null)
                    {
                        PostProcessVolume volume = mainCamera.GetComponent<PostProcessVolume>();
                        if (volume != null)
                        {
                            volume.weight = .05f;   // Drop the weight to zero
                        }
                    }

                    // Drop environment light
                    environmentLight.GetComponent<Light>().intensity = 1;
                }
            }
        }

    }
}
