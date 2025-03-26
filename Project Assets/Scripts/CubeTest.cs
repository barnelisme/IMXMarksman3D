using System.Collections;
using UnityEngine;
using TMPro;
public class CubeTest : MonoBehaviour
{
    public Canvas canva;
    public Transform mainPlayer;
    public TextMeshProUGUI text;
    string[] strings = { "At ease Soldier ", "A Woman is causing a scene in the city ", "She is said to be ARMED", "\nMission : ", "Your objective ", "Determine the thread level ", "Neutralise at will" };
    //public NavMeshAgent gimic;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(delay());
    }
    IEnumerator delay()
    {
        
        foreach (string txt in strings)
            {
            text.text = text.text + txt;
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(3f);
        canva.enabled = false;
    }
   
}
