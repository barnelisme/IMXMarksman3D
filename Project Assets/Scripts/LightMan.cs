using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[ExecuteAlways]
public class LightMan : MonoBehaviour
{
    [SerializeField] public Light DirectonalLight;
    [SerializeField] private lightingPreset preset;
    //public  Toggle isDay;
    //public bool ChooseTime;
    float time = 170f;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField, Range(0, 200)] private float TimeOfDay;
    

    private void Start()
    {
        //StartCoroutine(delayDestroy());
        /*if(ChooseTime)
        {
            isDay.enabled = true;
            dayText.enabled = true;
        }*/
    }

    private void FixedUpdate()
    {
       // if (!ChooseTime)
       // {
            if (preset == null)
                return;
            if (Application.isPlaying)
            {

                TimeOfDay += Time.deltaTime / 5;
                TimeOfDay %= time;
                UpdateLighting(TimeOfDay / time);

            }
            else
            {

                UpdateLighting(TimeOfDay / time);
            
            }
        //}
       /* else
        {

            if (isDay)
            {
                TimeOfDay = 12f;
            }
            else
            {
                TimeOfDay = 24f;
            }
            UpdateLighting(TimeOfDay / 24f);
        }*/
    }
    /*IEnumerator delayDestroy()
    {
        yield return new WaitForSeconds(5f);
        isDay.enabled = false;
        dayText.enabled = false;
    }*/
    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.FogColor.Evaluate(timePercent);
        if(DirectonalLight!=null)
        {
            DirectonalLight.color = preset.DirectionalColor.Evaluate(timePercent);
            DirectonalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0)); ;
        }
    }
    // Start is called before the first frame update
    private void OnValidate()
    {
        if (DirectonalLight != null)
        {
            return;
        }
        if (RenderSettings.sun != null)
        {
            DirectonalLight = RenderSettings.sun;
        }
        else
        {
            Light light = GameObject.FindObjectOfType<Light>();


            if (light.type == LightType.Directional)
            {
                DirectonalLight = light;
            }
        }
    }
}

