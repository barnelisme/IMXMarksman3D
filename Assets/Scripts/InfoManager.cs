using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InfoManager : MonoBehaviour
{
    public GameObject ui_canvas;
    GraphicRaycaster ui_raycaster;

    PointerEventData click_data;
    List<RaycastResult> click_results;

    public RectTransform uiElement; 
    public RectTransform targetPanel;
    public RectTransform openPos;
    public RectTransform closePos;

    public float moveSpeed = 200f; 
    public float minSpeed = 10f;
    private float distanceThreshold = 50f;
    private float openSpeed = 800f;
    private float closeSpeed = 1000f;

    private Vector2 targetPosition;
    private bool isMoving = false;
    private bool isOpenInfo = false;
    private string moveStatus = "";

    void Start()
    {
        targetPanel = openPos;
        targetPosition = targetPanel.anchoredPosition;

        ui_raycaster = ui_canvas.GetComponent<GraphicRaycaster>();
        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>();

        if(this.transform.name.ToLower().Contains("info"))
        {

        }
        else if(this.transform.name.ToLower().Contains("beginner"))
        {
            openSpeed = 1000f;
            closeSpeed = 1200f;
            distanceThreshold = 150f;
        }
    }

    void Update()
    {
        if (isMoving)
        {
            // Calculate direction towards target
            Vector2 direction = (targetPosition - uiElement.anchoredPosition).normalized;
            float distance = Vector2.Distance(uiElement.anchoredPosition, targetPosition);


            // Check if we reached the target position
            if (distance > 0.1f)
            {
                //print("Distance " + distanceThreshold);
                float currentSpeed = Mathf.Lerp(minSpeed, moveSpeed, distance / distanceThreshold);
                uiElement.anchoredPosition += direction * currentSpeed * Time.deltaTime;
            }
            else
            {
                uiElement.anchoredPosition = targetPosition;
                isMoving = false; // Stop movement

                //Allocate Current Info Status
                switch (moveStatus)
                {
                    case "opening":
                        isOpenInfo = true;
                        break;

                    case "closing":
                        isOpenInfo = false;
                        break;
                }
            }
        }

        // use wasReleasedThisFrame if you wish to ray cast just once per click:
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            GetUiElementsClicked();
        }
    }

    public void openPanel()
    {
        //print("Test: opening...");
        moveStatus = "opening";
        targetPanel = openPos;
        moveSpeed = openSpeed;
        targetPosition = targetPanel.anchoredPosition;
        isMoving = true;
    }

    public void closeInfo()
    {
        //print("Test: closing...");
        moveStatus = "closing";
        targetPanel = closePos;
        moveSpeed = closeSpeed;
        targetPosition = targetPanel.anchoredPosition;
        isMoving = true;
    }

    void GetUiElementsClicked()
    {

        //print("Mouse Active...");
        click_data.position = Mouse.current.position.ReadValue();
        click_results.Clear();

        ui_raycaster.Raycast(click_data, click_results);

       
        if(isOpenInfo)
        {
            foreach (RaycastResult result in click_results)
            {
                GameObject ui_element = result.gameObject;
                //print(ui_element.name);

                //break point
                if(ui_element.name.ToLower().Contains("beginner panel"))
                {
                    break;
                }

                
                if (ui_element.name.ToLower().Contains("static") || ui_element.name.ToLower().Contains("moving") 
                    || ui_element.name.ToLower().Contains("3d"))
                {
                    closeInfo();
                }
            }
        }
    }

}

