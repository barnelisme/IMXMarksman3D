using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TargetMovement : MonoBehaviour
{
    //Movement Varables
    public float MoveSpeed;
    public float SetSpeed = 1;
    bool Move = true;
    float minimumX = 523f;
    float maximumX = 540f;
    float minimumY = 3f;
    float maximumY = 9f;

    //Timers
    float StartTime = 3;
    float StopTime = 3;
    float setTime = 3f;

    //Direction Variables
    public bool moveLeft = false;
    public bool moveRight = false;
    public bool moveUp = false;
    public bool moveDown = false;
    public int directionSelector = 0;
    int min_max_selector = 0;

    //Conditions
    public bool isHit;
    bool START = false;
    int LIFE = 1;


    //UI Variables
    //public GameObject ScorePanel;
    //public GameObject DirectionMenu;

    //Text Mesh Variable
    public TextMeshProUGUI output;

    void Start()
    {
        //START = true;
        SetSpeed = 2f;
        MoveSpeed = SetSpeed;
        //ScorePanel.SetActive(false);
        GenerateDirection();
        //moveLeft = true;
        min_max_selector = Random.Range(1, 4);

        switch (min_max_selector)
        {
            case 1:
                minimumX -= 1;
                maximumX += 1;
                minimumY -= 1;
                maximumY += 1;

                break;

            case 2:
                minimumX += 1;
                maximumX -= 1;
                minimumY += 1;
                maximumY -= 1;

                break;

            case 3:
                minimumX -= 2;
                maximumX += 2;
                minimumY -= 2;
                maximumY += 2;

                break;

            case 4:
                minimumX += 2;
                maximumX -= 2;
                minimumY += 2;
                maximumY -= 2;

                break;


        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isHit)
        {
            MoveSpeed = 0f;
            //ScorePanel.SetActive(true);
        }
        else
        {
            MoveSpeed = SetSpeed;
            //ScorePanel.SetActive(false);
        }

        if (Input.GetKey(KeyCode.S))//Reset Indoor range
        {
            isHit = true;
        }
        else if (!Input.GetKey(KeyCode.S))
        {
            isHit = false;
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("BasicAimAndShoot");
        }

        if(!START)
        {
            StartTime -= Time.deltaTime * 1;
            
            if(StartTime <= 0f)
            {
                StopTime = 8;
                START = true;
            }
            this.transform.Translate(0, 0, 0);
        }

        if (START)
        {
            //StopTime -= Time.deltaTime * 1;

            if (StopTime <= 0f)
            {
                StartTime = setTime;
                START = false;
            }

            MoveLeft();
            MoveRight();
            MoveUp();
            MoveDown();
        }

    }
    void GenerateDirection()
    {

        directionSelector = Random.Range(1, 4);

        switch (directionSelector)
        {
            case 1:
                moveLeft = true;
                break;

            case 2:
                moveUp = true;
                break;

            case 3:
                moveRight = true;
                break;

            case 4:
                moveDown = true;
                break;

            case 5:
                moveLeft = true;
                break;

            case 6:
                moveUp = true;
                break;

            case 7:
                moveDown = true;
                break;


        }
    }
    public void move()
    {
        if ((moveLeft || moveRight))
        {

            transform.Translate(MoveSpeed * Time.deltaTime, 0, 0);
        }

        if ((moveUp || moveDown))
        {
            transform.Translate(0, MoveSpeed * Time.deltaTime, 0);
        }

    }
    public void MoveLeft()
    {
        if (moveLeft)
        {
            //print(this.transform.position.x);
            if (this.transform.position.x <= minimumX)
            {
                //GenerateDirection();
                if (directionSelector == 1)
                {
                    moveUp = true;
                    moveLeft = false;
                }
                else
                {
                    moveRight = true;
                    moveLeft = false;
                }
            }

            MoveSpeed = -SetSpeed;
            this.transform.Translate(MoveSpeed * Time.deltaTime, 0, 0);
        }
    }
    public void MoveRight()
    {
        if (moveRight)
        {
            //print("Moving Right");
            if (this.transform.position.x >= maximumX)
            {
                //GenerateDirection();
                if (directionSelector == 1)
                {
                    moveDown = true;
                    moveRight = false;
                }
                else
                {
                    moveLeft = true;
                    moveRight = false;
                }
            }

            MoveSpeed = SetSpeed;
            this.transform.Translate(MoveSpeed * Time.deltaTime, 0, 0);
        }
    }
    public void MoveUp()
    {
        if (moveUp)
        {
            //print("Moving Up");
            if (this.transform.position.y >= maximumY)
            {
                //GenerateDirection();
                if (directionSelector == 1)
                {
                    moveRight = true;
                    moveUp = false;
                }
                else
                {
                    moveDown = true;
                    moveUp = false;
                }
            }

            MoveSpeed = SetSpeed;
            this.transform.Translate(0, MoveSpeed * Time.deltaTime, 0);
        }
    }
    public void MoveDown()
    {
        if (moveDown)
        {
            //print("Moving Down");
            if (this.transform.position.y <= minimumY)
            {
                //GenerateDirection();
                if (directionSelector == 1)
                {
                    moveLeft = true;
                    moveDown = false;
                }
                else
                {
                    moveUp = true;
                    moveDown = false;
                }
                
            }

            MoveSpeed = -SetSpeed;
            this.transform.Translate(0, MoveSpeed * Time.deltaTime, 0);
        }
    }
    public void Stop()
    {

    }

    public void HandleInputData(int val)
    {
        directionSelector = val;
    }

    public void StartScene()
    {
       // DirectionMenu.SetActive(false);
        START = true;
        Start();
    }

    private void ApplyDamage(string tagged)
    {
        


    }

}