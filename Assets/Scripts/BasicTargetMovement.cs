using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BasicTargetMovement : MonoBehaviour
{
    //Movement Varables
    float MoveSpeed;
    public float SetSpeed = 3;
    bool Move = true;

    //Direction Variables
    bool moveLeft = false;
    bool moveRight = false;
    bool moveUp = false;
    bool moveDown = false;
    int directionSelector = 0;

    //Conditions
    public bool isHit;
    bool START = false;
    public int LIFE = 5;

    //UI Variables
    public GameObject ScorePanel;
    public GameObject DirectionMenu;

    //Text Mesh Variable
    public TextMeshProUGUI output;

    void Start()
    {
        //ScorePanel.SetActive(true);
        DirectionMenu.SetActive(false);
        MoveSpeed = 5;
        ScorePanel.SetActive(false);
        START = true;

        if(directionSelector == 0 || directionSelector == 1)
        {
            moveLeft = true;
        }
        else if(directionSelector == 2)
        {
            moveLeft = false;
            moveUp = true;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
       if(isHit)
        {
            SetSpeed = 0f;
            ScorePanel.SetActive(true);
        }
        else
        {
            SetSpeed = 4f;
            ScorePanel.SetActive(false);
        }

        if (Input.GetKey(KeyCode.S) || LIFE <= 0)//Reset Indoor range
        {
            isHit = true;
        }

        else if(!Input.GetKey(KeyCode.S) || LIFE >= 0)
        {
            //isHit = false;
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("BasicAimAndShoot");
        }


        if(START)
        {
            MoveLeft();
            MoveRight();
            MoveUp();
            MoveDown();
        }

    }
    public void move()
    {
        if((moveLeft || moveRight))
        {

            transform.Translate(MoveSpeed * Time.deltaTime, 0, 0);
        }

        if((moveUp || moveDown))
        {
            transform.Translate(0, MoveSpeed * Time.deltaTime, 0);
        }

    }
    public void MoveLeft()
    {
        if(moveLeft)
        {
            //print("Moving Left");
            if (transform.position.x <= 634 - 7)
            {
                if(directionSelector == 1)
                {
                    moveRight = true;
                }
                else if(directionSelector == 0)
                {
                    moveUp = true;
                }
                
                moveLeft = false;
            }

            MoveSpeed = -SetSpeed;
            transform.Translate(MoveSpeed * Time.deltaTime, 0, 0);
        }
    }
    public void MoveRight()
    {
        if (moveRight)
        {
            //print("Moving Right");
            if (transform.position.x >= 634 + 7)
            {
                if (directionSelector == 1)
                {
                    moveLeft = true;
                }
                else if (directionSelector == 0)
                {
                    moveDown = true;
                }

                moveRight = false;
            }

            MoveSpeed = SetSpeed;
            transform.Translate(MoveSpeed * Time.deltaTime, 0, 0);
        }
    }
    public void MoveUp()
    {
        if (moveUp)
        {
            //print("Moving Up");
            if (transform.position.y >= 8)
            {
                if (directionSelector == 2)
                {
                    moveDown = true;
                }
                else if (directionSelector == 0)
                {
                    moveRight = true;
                }

                moveUp = false;
            }

            MoveSpeed = SetSpeed;
            transform.Translate(0, MoveSpeed * Time.deltaTime, 0);
        }
    }
    public void MoveDown()
    {
        if (moveDown)
        {
            //print("Moving Down");
            if (transform.position.y <= 4)
            {
                if (directionSelector == 2)
                {
                    moveUp = true;
                }
                else if (directionSelector == 0)
                {
                    moveLeft = true;
                }

                moveDown = false;
            }

            MoveSpeed = -SetSpeed;
            transform.Translate(0, MoveSpeed * Time.deltaTime, 0);
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
        DirectionMenu.SetActive(false);
        START = true;
        Start();
    }

    private void ApplyDamage(string tagged)
    {
        //received = false;
        
        Debug.Log("I was hit:" + transform.name + " apply Damage sent:" + tagged);

        if (this.transform.name == tagged)
        {

            print("DEADLY SHOT!!!!!!!!");
            LIFE--;
        }
        else if (tagged.Contains("change"))
        {
            //seen = true;
            //changeState(states.patrol);
        }


    }
}


/* if (transform.position.x <= 634 - 7)
        {
            MoveSpeed = 5;

            moveLeft = false;
            moveUp = true;
            moveDown = false;
            moveRight = false;
        }
        if (transform.position.x >= 634 + 7)
        {
            MoveSpeed = -5;
            moveLeft = false;
            moveUp = false;
            moveDown = true;
            moveRight = false;
        }

        //Y Movement
        if (transform.position.y <= 4)
        {
            MoveSpeed = 5;
            moveLeft = true;
            moveUp = false;
            moveDown = false;
            moveRight = false;
        }
        if (transform.position.y >= 8)
        {
            MoveSpeed = -5;
            moveLeft = false;
            moveUp = false;
            moveDown = false;
            moveRight = true;
        }*/