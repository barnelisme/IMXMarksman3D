using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    private float mouseSensitivity = 0.5f;
    private Camera m_Camera;
    private float m_YRotation;
    private bool cursor_unlocked = false;
    private string activeScene = "";
    private bool mouseReleased = false;
    private bool mouse_locked = false;

    //UI Button Variables
    public GameObject ui_canvas;
    GraphicRaycaster ui_raycaster;
    PointerEventData click_data;
    List<RaycastResult> click_results;
    private bool ui_mouse_pressed = false;
    private bool ui_mouse_reset = false;
    private GameObject player;
    private GameObject ButtonsManager;

    private void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        player = GameObject.FindGameObjectWithTag("Player");
        ButtonsManager = GameObject.FindGameObjectWithTag("ControlButtonsManager");

        ui_canvas = GameObject.FindGameObjectWithTag("StartCountCanvas");
        ui_raycaster = ui_canvas.GetComponent<GraphicRaycaster>();
        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>();

        if(!activeScene.ToLower().Contains("hunting"))
        {
            LockMouse();
        }
    }

    private void Update()
    {
        if(!activeScene.ToLower().Contains("hunting"))
        {  
            handleUI();
            if (mouseReleased == false)
            {
                RotateView();
            }
            else if (!cursor_unlocked)
            {
                UnlockCursor();
                cursor_unlocked = true;
            }

        }
    }

    private void RotateView()
    {
        Vector2 mouseInput = Mouse.current.delta.ReadValue();
        m_YRotation += mouseInput.x * mouseSensitivity;
        transform.localRotation = Quaternion.Euler(0, m_YRotation, 0);
        
        float xRotation = -mouseInput.y * mouseSensitivity;
        m_Camera.transform.localRotation *= Quaternion.Euler(xRotation, 0, 0);
    }
    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Optionally show the cursor
    }
    private void LockMouse()
    {
        m_Camera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor to the center of the screen
        mouse_locked = true;
    }
    private void handleUI()
    {
        if (Keyboard.current.escapeKey.isPressed)
        {
            mouseReleased = true;
            cursor_unlocked = false;
            mouse_locked = false;
        }
        else if (Mouse.current.leftButton.wasPressedThisFrame && StaticVariableManager.isStopTraining == false)
        {
            ui_mouse_pressed = GetUiElementsClicked();

            if(ui_mouse_pressed == false)
            {
                mouseReleased = false;
                if(mouse_locked == false)
                {
                    LockMouse();
                }
            }
            
        }
    }

    private bool GetUiElementsClicked()
    {
        bool process_result = false;
        //print("Mouse Pressed...");
        click_data.position = Mouse.current.position.ReadValue();
        click_results.Clear();

        ui_raycaster.Raycast(click_data, click_results);

        foreach (RaycastResult result in click_results)
        {
            GameObject ui_element = result.gameObject;
            //print("Element pressed is: " + ui_element.name);
            if(ui_element.name.ToLower().Contains("start"))
            {
                //ButtonsManager.GetComponent<ControlButtonsManager>().StartScenario();
            }
            else if(ui_element.name.ToLower().Contains("exit"))
            {
                //ButtonsManager.GetComponent<ControlButtonsManager>().loadMainMenu();
            }
            process_result = true;
        }
        return process_result;
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Destroy(player.gameObject);
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(activeScene);
        Destroy(player.gameObject);
    }

}
