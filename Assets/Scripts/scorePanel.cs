using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class scorePanel : MonoBehaviour
{
    public TextMeshProUGUI Traineename;
    public TextMeshProUGUI location;
    public TextMeshProUGUI enemiesKilled;
    public TextMeshProUGUI civilianKilled;
    public TextMeshProUGUI score;
    public GameObject Panel;
    public string activeScene;

    public Canvas scoreCanva;
    
    // Start is called before the first frame update
    void Start()
    {
        
        scoreCanva.enabled = false;
        Panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        activeScene = SceneManager.GetActiveScene().name;
        Traineename.text = Scoring.trainee_name;
        location.text = "Location: "+ activeScene;
        enemiesKilled.text = "Enemies killed: "+Shooting.enemyshot.ToString();
        civilianKilled.text = "Civilian Killed: "+Shooting.civilianShot.ToString();
        score.text = "Score: "+Scoring.precision_percentage;
        if (!(activeScene.ToLower().Contains("range")))
        {
            if(Shooting.mainPlayerLives==0)
            {
                scoreCanva.enabled = true;
                Panel.SetActive(true);
            }
        }
    }
}
