using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class movePlayer : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction moveAction;
    float moveSpeed = 2.5f;
    float verticalSpeed = 2.0f; // Speed for up and down movement
    private string activeScene = "";

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
        activeScene = SceneManager.GetActiveScene().name;
    }

    private void Update()
    {
        if(StaticVariableManager.isStopTraining == false)
        {
            moveMyPlayer();
        }

    }

    void moveMyPlayer()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();

        if (Keyboard.current.leftShiftKey.isPressed)
        {
            float verticalInput = direction.y; // Assuming y-axis is used for vertical movement
            Vector3 moveDirection = new Vector3(0, verticalInput, 0) * Time.deltaTime * verticalSpeed;
            transform.Translate(moveDirection, Space.World);
        }
        else
        {
            // Move horizontally based on horizontal input
            if(activeScene.ToLower().Contains("cyclic") || activeScene.ToLower().Contains("plat") 
                || activeScene.ToLower().Contains("ipec") || activeScene.ToLower().Contains("threatening")
                || activeScene.ToLower().Contains("pointma") || activeScene.ToLower().Contains("pointbullseye")
                || activeScene.ToLower().Contains("sequence") || activeScene.ToLower().Contains("distancesimulator")
                || activeScene.ToLower().Contains("hiddentarget") || activeScene.ToLower().Contains("diceflipping")
                || activeScene.ToLower().Contains("fallingplat") || activeScene.ToLower().Contains("duelingtree"))
            {

                if(!Keyboard.current.sKey.isPressed)
                {
                    float verticalInput = direction.y; // Assuming y-axis is used for vertical movement
                    Vector3 moveDirection = new Vector3(0, verticalInput, 0) * Time.deltaTime * verticalSpeed;
                    transform.Translate(moveDirection, Space.World);
                }

                transform.position += new Vector3(direction.x, 0, 0) * Time.deltaTime * moveSpeed;
            }
            else
            {
                transform.position += new Vector3(direction.x, 0, direction.y) * Time.deltaTime * moveSpeed;
            }    

        }
    }
}

