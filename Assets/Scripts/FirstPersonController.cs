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
    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 150f;
    [SerializeField] private float rotationSmoothSpeed = 12f;

    private float yaw;
    private float pitch;
    private bool cursor_unlocked = false;
    private string activeScene = "";
    private bool mouseReleased = true;
    private bool mouse_locked = false;
    private bool training_stopped = false;

    private List<Camera> cameras = new List<Camera>();
    private GameObject player;
    private GameObject ButtonsManager;

    // UI handling
    public GameObject ui_canvas;
    GraphicRaycaster ui_raycaster;
    PointerEventData click_data;
    List<RaycastResult> click_results;

    private void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        player = GameObject.FindGameObjectWithTag("Player");
        ButtonsManager = GameObject.FindGameObjectWithTag("ControlButtonsManager");

        // ✅ Initialize UI Raycaster
        if (ui_canvas == null)
            ui_canvas = GameObject.FindGameObjectWithTag("StartCanvas"); // or your canvas tag name

        if (ui_canvas != null)
            ui_raycaster = ui_canvas.GetComponent<GraphicRaycaster>();
        else
            Debug.LogWarning("UI Canvas not found! Make sure it's tagged correctly.");

        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>();

        if (!activeScene.ToLower().Contains("hunting"))
        {
            // LockMouse();
        }
    }


    private void Update()
    {
        if (!activeScene.ToLower().Contains("hunting"))
        {
            handleUI();

            if (!mouseReleased)
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
        Vector2 mouseInput = Mouse.current.delta.ReadValue() * mouseSensitivity * Time.deltaTime;

        yaw += mouseInput.x;
        pitch -= mouseInput.y;
        pitch = Mathf.Clamp(pitch, -80f, 80f); // prevent over-rotation vertically

        // Smoothly interpolate player rotation (yaw)
        Quaternion targetBodyRotation = Quaternion.Euler(0f, yaw, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetBodyRotation, rotationSmoothSpeed * Time.deltaTime);

        // Smoothly interpolate camera rotations (pitch)
        foreach (Camera cam in cameras)
        {
            if (cam != null)
            {
                Quaternion targetCamRotation = Quaternion.Euler(pitch, 0f, 0f);
                cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, targetCamRotation, rotationSmoothSpeed * Time.deltaTime);
            }
        }
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LockMouse()
    {
        if(!DropDown.softwareSceneName.ToLower().Contains("hunting"))
        {
            cameras.Clear();
            cameras.AddRange(FindObjectsOfType<Camera>());

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            mouse_locked = true;
            mouseReleased = false;
        }
    }

    private void handleUI()
    {
        // If ESC is pressed → unlock mouse and show cursor
        if (Keyboard.current.escapeKey.isPressed)
        {
            mouseReleased = true;
            cursor_unlocked = false;
            mouse_locked = false;
            UnlockCursor();
        }
        // Handle left-click: only lock mouse if NOT clicking UI
        else if (Mouse.current.leftButton.wasPressedThisFrame && StaticVariableManager.isStopTraining == false
            && countDownStart.start_training )
        {
            // Check if the click hit any UI element
            bool clickedUI = GetUiElementsClicked();

            // Only lock mouse if click was outside UI
            if (!clickedUI)
            {
                LockMouse();
                mouseReleased = false;
            }
        }
    }


    private bool GetUiElementsClicked()
    {
        if (ui_raycaster == null)
            return false;

        click_data.position = Mouse.current.position.ReadValue();
        click_results.Clear();

        ui_raycaster.Raycast(click_data, click_results);

        if (click_results.Count == 0)
            return false;

        foreach (RaycastResult result in click_results)
        {
            GameObject ui_element = result.gameObject;

            // Example action triggers (optional)
            if (ui_element.name.ToLower().Contains("start"))
            {
                // ButtonsManager.GetComponent<ControlButtonsManager>().StartScenario();
            }
            else if (ui_element.name.ToLower().Contains("exit"))
            {
                // ButtonsManager.GetComponent<ControlButtonsManager>().loadMainMenu();
            }
        }

        return true; // A UI element was clicked
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
