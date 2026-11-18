using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    // --- Camera / Input / Movement Variables ---
    public Camera[] playerCameras;
    public int defaultCameraIndex = 0;
    public InputActionProperty moveAction, jumpAction, runAction, crouchAction, lookAction;
    public float walkSpeed = 6f, runSpeed = 12f;
    private Vector3 _movementSmoothVelocity;
    public float moveSmoothTime = 0.35f, jumpPower = 7f, gravity = 10f, lookSpeed = 5f;
    public float guidedLookRotationSpeed = 12f, lookXLimit = 45f;
    public float defaultHeight = 2f, crouchHeight = 1f, crouchSpeed = 3f;
    public float shootingDistance = 20f, lookSmoothTime = 0.05f;
    public float animalSwitchTime = 2f, trackingRotationSmoothFactor = 10f, lookInputDeadZone = 0.05f;
    public Transform[] startPositions;

    // --- Private Variables ---
    private CharacterController characterController;
    private NavMeshAgent navMeshAgent;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private bool canMove = true, isCrouching = false;
    private Vector2 currentMoveInput, currentLookInput, _smoothLookVelocity;
    private Vector2 _guidedManualMoveInput, _guidedManualLookInput;
    private string activeScene = "";
    private GameObject player, ButtonsManager;
    public Canvas ui_canvas;
    private GraphicRaycaster ui_raycaster;
    private PointerEventData click_data;
    private List<RaycastResult> click_results;
    private bool mouseReleased = false, mouse_locked = true, cursor_unlocked = false;
    public List<GameObject> animalModels = new List<GameObject>();
    private GameObject currentTargetAnimal = null;
    private string requiredAnimalTag;
    private bool search_animal = false;
    private float animalSwitchDefault;
    private bool isUnstucking = false;
    private bool isPedalForwardActive = false;

    // Animal setting variables
    private bool isAnimalsFound = false;
    private bool animalAssignmentComplete = false;

    // Slope Alignment Variables
    [Header("Slope Settings")]
    public float slopeRotationSmoothTime = 3f; // Slower transition for natural feel
    public LayerMask groundLayer = ~0;
    public float slopeThreshold = 1.5f; // Ignore slopes smaller than this (degrees)
    private float smoothedSlopePitch = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetStartPosition();
        LockMouse();
        SetupCameras();
        ApplySettings();

        activeScene = SceneManager.GetActiveScene().name;
        player = GameObject.FindGameObjectWithTag("Player");
        ButtonsManager = GameObject.FindGameObjectWithTag("ControlButtonsManager");
        GameObject canvasObj = GameObject.FindGameObjectWithTag("StartCanvas");
        if (canvasObj != null) ui_canvas = canvasObj.GetComponent<Canvas>();
        if (ui_canvas != null) ui_raycaster = ui_canvas.GetComponent<GraphicRaycaster>();
        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>();

        requiredAnimalTag = TestConditionsManager.animalName.ToLower();
        animalSwitchDefault = animalSwitchTime;

        if (navMeshAgent != null)
        {
            navMeshAgent.updatePosition = false;
            navMeshAgent.updateRotation = false;
            navMeshAgent.speed = runSpeed;
            navMeshAgent.stoppingDistance = shootingDistance;
            navMeshAgent.autoBraking = false;
        }

        FindAnimalModels();
    }

    private readonly List<int> recentStartIndices = new List<int>(2);
    private void SetStartPosition()
    {
        if (startPositions == null || startPositions.Length == 0)
        {
            Debug.LogError("Start positions array is null or empty!");
            return;
        }

        if (startPositions.Length <= recentStartIndices.Count)
        {
            recentStartIndices.Clear();
        }

        int newIndex;
        int safety = 0;

        do
        {
            newIndex = Random.Range(0, startPositions.Length);
            safety++;
        }
        while (recentStartIndices.Contains(newIndex) && safety < 50);

        recentStartIndices.Add(newIndex);
        if (recentStartIndices.Count > 2)
            recentStartIndices.RemoveAt(0);

        Transform startPos = startPositions[newIndex];
        transform.position = startPos.position;
        transform.rotation = startPos.rotation;
    }

    void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        runAction.action.Enable();
        crouchAction.action.Enable();
        lookAction.action.Enable();

        jumpAction.action.performed += OnJump;
        crouchAction.action.started += OnCrouchStart;
        crouchAction.action.canceled += OnCrouchEnd;
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
        runAction.action.Disable();
        crouchAction.action.Disable();
        lookAction.action.Disable();

        jumpAction.action.performed -= OnJump;
        crouchAction.action.started -= OnCrouchStart;
        crouchAction.action.canceled -= OnCrouchEnd;
    }

    private void ApplySettings()
    {
        float single_rotation_speed = 3.5f;

        // Settings for controlling shooting distance
        switch (TestConditionsManager.difficultySetting)
        {
            case "Easy": shootingDistance = 10; break;
            case "Medium": shootingDistance = 18; break;
            case "Hard": shootingDistance = 30; break;
        }

        // Settings for controlling navigation speed
        switch (StaticVariableManager.playerMoveSetting)
        {
            case "Slow": walkSpeed = 1; runSpeed = 7; lookSpeed = single_rotation_speed; break;
            case "Medium": walkSpeed = 2; runSpeed = 7; lookSpeed = single_rotation_speed; break;
            case "Fast": walkSpeed = 4; runSpeed = 7; lookSpeed = single_rotation_speed; break;
        }
    }

    private void SetupCameras()
    {
        if (playerCameras != null && playerCameras.Length > 0 && playerCameras[0] != null)
        {
            rotationX = playerCameras[0].transform.localRotation.eulerAngles.x;
            if (rotationX > 180) rotationX -= 360;
        }
    }

    void Update()
    {
        bool isGuidedHuntMode = false;

        if (activeScene.ToLower().Contains("foresthunting"))
            isGuidedHuntMode = TestConditionsManager.typeOfHunt == "Guided hunt";

        if (!isAnimalsFound && isGuidedHuntMode)
        {
            FindAnimalModels();
        }
        else if (isAnimalsFound && !animalAssignmentComplete)
        {
            SetupGuidedHunt();
            animalAssignmentComplete = true;
        }

        handleUI();
        if (isGuidedHuntMode && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            StartCoroutine(UnstuckMovement());
            return;
        }

        ManagePedalMovement(isGuidedHuntMode);

        if (isPedalForwardActive)
        {
            ApplyGravity();
            characterController.Move(moveDirection * Time.deltaTime);
            return;
        }

        _guidedManualMoveInput = moveAction.action.ReadValue<Vector2>();
        _guidedManualLookInput = lookAction.action.ReadValue<Vector2>();

        if (isGuidedHuntMode && Keyboard.current != null && Keyboard.current.sKey.wasPressedThisFrame)
            FindAnotherAnimal();

        if (search_animal)
        {
            animalSwitchTime -= Time.deltaTime;
            if (animalSwitchTime <= 0)
            {
                SetAnimalTarget();
                animalSwitchTime = animalSwitchDefault;
                search_animal = false;
            }
        }

        if (!mouseReleased)
        {
            if (!canMove || characterController == null)
            {
                ApplyGravity();
                characterController.Move(moveDirection * Time.deltaTime);
                return;
            }

            if (isGuidedHuntMode && !isUnstucking && animalAssignmentComplete)
                HandleGuidedMovement();
            else
            {
                HandleMovementInput();
                HandleLookInput();
            }

            HandleCrouch();
            ApplyGravity();
            characterController.Move(moveDirection * Time.deltaTime);
        }
        else if (!cursor_unlocked)
        {
            UnlockCursor();
            cursor_unlocked = true;
            ApplyGravity();
            characterController.Move(moveDirection * Time.deltaTime);
        }
        else
        {
            ApplyGravity();
            characterController.Move(moveDirection * Time.deltaTime);
        }
    }

    private void SetupGuidedHunt()
    {
        if (TestConditionsManager.typeOfHunt == "Guided hunt" && activeScene.ToLower().Contains("foresthunting"))
            SelectNewTarget(false);
    }

    private void ManagePedalMovement(bool isGuidedHuntMode)
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        bool leftPedal = kb.aKey.isPressed;
        bool rightPedal = kb.cKey.isPressed;

        if (kb.bKey.wasPressedThisFrame)
            isPedalForwardActive = !isPedalForwardActive;

        if (isGuidedHuntMode && animalAssignmentComplete)
        {
            isUnstucking = isPedalForwardActive || leftPedal || rightPedal;
            if (navMeshAgent != null)
                navMeshAgent.isStopped = isUnstucking;
        }

        float rotationSpeed = lookSpeed * 15f;
        float rotationInput = 0f;

        if (leftPedal) rotationInput -= 1f;
        if (rightPedal) rotationInput += 1f;

        if (rotationInput != 0f)
        {
            Quaternion deltaRotation = Quaternion.Euler(0f, rotationInput * rotationSpeed * Time.deltaTime, 0f);
            transform.rotation = transform.rotation * deltaRotation;
        }

        if (isPedalForwardActive)
        {
            float y = moveDirection.y;
            Vector3 forwardMove = transform.forward * walkSpeed;
            moveDirection = new Vector3(forwardMove.x, y, forwardMove.z);
        }
        else if (!isGuidedHuntMode)
        {
            moveDirection.x = 0f;
            moveDirection.z = 0f;
        }
    }

    public void FindAnotherAnimal() => search_animal = true;

    private void SetAnimalTarget()
    {
        FindAnimalModels();
        SelectNewTarget(false);
    }

    private void FindAnimalModels()
    {
        animalModels.Clear();
        const string DEAD_ANIMAL_TAG = "dead_animal";

        if (string.IsNullOrEmpty(requiredAnimalTag))
        {
            Debug.LogError("[Guided Hunt] Target animal name (tag) is empty.");
            return;
        }

        string requiredNameLower = requiredAnimalTag.ToLowerInvariant();
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(requiredAnimalTag);

        foreach (GameObject obj in taggedObjects)
        {
            if (!obj.activeInHierarchy) continue;
            if (obj.CompareTag(DEAD_ANIMAL_TAG)) continue;
            if (!obj.name.ToLowerInvariant().Contains(requiredNameLower)) continue;
            if (!obj.CompareTag(requiredAnimalTag)) continue;

            animalModels.Add(obj);
        }

        if (animalModels.Count == 0)
        {
            isAnimalsFound = false;
        }
        else if (animalModels.Count == AnimalSpawner.animalPopulation)
        {
            isAnimalsFound = true;
        }
    }

    private void SelectNewTarget(bool findRandom)
    {
        animalModels.RemoveAll(g => g == null || !g.activeInHierarchy || !g.CompareTag(requiredAnimalTag));

        if (animalModels.Count == 0)
        {
            currentTargetAnimal = null;
            if (navMeshAgent != null) navMeshAgent.isStopped = true;
            return;
        }

        GameObject targetToExclude = currentTargetAnimal;
        GameObject newTarget = FindNearestAnimal(targetToExclude);

        if (newTarget != null)
        {
            currentTargetAnimal = newTarget;
            if (navMeshAgent != null)
            {
                if (!navMeshAgent.isOnNavMesh)
                {
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
                        navMeshAgent.Warp(hit.position);
                    else
                        return;
                }

                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(currentTargetAnimal.transform.position);
                navMeshAgent.speed = runSpeed;
            }
        }
        else
        {
            currentTargetAnimal = null;
            if (navMeshAgent != null) navMeshAgent.isStopped = true;
        }
    }

    private GameObject FindNearestAnimal(GameObject excludeTarget = null)
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject animal in animalModels)
        {
            if (animal == excludeTarget) continue;
            float dSqrToTarget = (animal.transform.position - currentPosition).sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = animal;
            }
        }
        return bestTarget;
    }

    private void HandleGuidedMovement()
    {
        if (navMeshAgent == null) return;

        GameObject target = currentTargetAnimal;
        Vector3 desiredHorizontalMove = Vector3.zero;
        Vector3 targetVelocity;
        float movementDirectionY;
        Vector3 dampedVelocity;

        if (target == null || !target.activeInHierarchy || !target.CompareTag(requiredAnimalTag))
        {
            navMeshAgent.isStopped = true;
            targetVelocity = Vector3.zero;
            movementDirectionY = moveDirection.y;
            dampedVelocity = Vector3.SmoothDamp(new Vector3(moveDirection.x, 0, moveDirection.z), targetVelocity, ref _movementSmoothVelocity, moveSmoothTime);
            moveDirection = new Vector3(dampedVelocity.x, movementDirectionY, dampedVelocity.z);
            currentTargetAnimal = null;
            return;
        }

        bool hasManualMoveInput = _guidedManualMoveInput.sqrMagnitude > 0.0001f;

        if (hasManualMoveInput)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float currentSpeed = runAction.action.IsPressed() ? runSpeed : walkSpeed;
            if (isCrouching) currentSpeed = crouchSpeed;
            float curSpeedX = _guidedManualMoveInput.y * currentSpeed;
            float curSpeedY = _guidedManualMoveInput.x * currentSpeed;
            desiredHorizontalMove = forward * curSpeedX + right * curSpeedY;
            navMeshAgent.isStopped = true;
        }
        else
        {
            Vector3 direction = (target.transform.position - transform.position);
            direction.y = 0;
            float distance = direction.magnitude;
            Vector3 directionNormalized = direction.normalized;

            if (distance > shootingDistance + 2f)
            {
                desiredHorizontalMove = directionNormalized * runSpeed;
                navMeshAgent.speed = runSpeed;
            }
            else if (distance > shootingDistance)
            {
                desiredHorizontalMove = directionNormalized * walkSpeed;
                navMeshAgent.speed = walkSpeed;
            }
            else
            {
                desiredHorizontalMove = Vector3.zero;
                navMeshAgent.speed = walkSpeed;
            }

            navMeshAgent.isStopped = false;
        }

        targetVelocity = new Vector3(desiredHorizontalMove.x, 0, desiredHorizontalMove.z);
        movementDirectionY = moveDirection.y;
        dampedVelocity = Vector3.SmoothDamp(new Vector3(moveDirection.x, 0, moveDirection.z), targetVelocity, ref _movementSmoothVelocity, moveSmoothTime);
        moveDirection = new Vector3(dampedVelocity.x, movementDirectionY, dampedVelocity.z);

        HandleLookAtTarget();
    }

    private void HandleLookAtTarget()
    {
        if (navMeshAgent == null) return;
        float rotationRate = Time.deltaTime * trackingRotationSmoothFactor;
        GameObject target = currentTargetAnimal;

        if (target != null)
        {
            // 1. Handle Horizontal Rotation (Y Axis)
            Vector3 targetDirection = target.transform.position - transform.position;
            Vector3 targetDirectionHorizontal = targetDirection;
            targetDirectionHorizontal.y = 0;

            if (targetDirectionHorizontal.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotY = Quaternion.LookRotation(targetDirectionHorizontal);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotY, rotationRate);
            }

            // 2. Optimized Slope-Aware Pitch (X Axis)
            // Instead of one point, we sample the average normal of the terrain in front of the player
            // to ignore small bumps/holes and focus on the overall slope.
            Vector3 averageNormal = Vector3.up;
            int samples = 0;
            float sampleDistance = 1.0f; // Distance between samples

            Vector3[] offsets = { Vector3.zero, transform.forward * sampleDistance, transform.forward * -sampleDistance };

            foreach (Vector3 offset in offsets)
            {
                RaycastHit hit;
                Vector3 rayOrigin = transform.position + offset + Vector3.up * 1.0f;
                if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 3.0f, groundLayer))
                {
                    averageNormal += hit.normal;
                    samples++;
                }
            }

            if (samples > 0)
            {
                averageNormal = (averageNormal / samples).normalized;
                Vector3 localForwardOnSlope = Vector3.ProjectOnPlane(transform.forward, averageNormal);
                float targetSlopePitch = -Mathf.Asin(Vector3.Dot(localForwardOnSlope, Vector3.up)) * Mathf.Rad2Deg;

                // Threshold filter: If the slope change is tiny, don't update the target
                if (Mathf.Abs(targetSlopePitch - smoothedSlopePitch) < slopeThreshold)
                {
                    // Stay at current smoothed pitch to avoid micro-jitter
                }
                else
                {
                    // Apply heavy smoothing to the slope transition
                    smoothedSlopePitch = Mathf.Lerp(smoothedSlopePitch, targetSlopePitch, Time.deltaTime * slopeRotationSmoothTime);
                }
            }

            // 3. Handle Camera Pitch (Looking at animal + Slope)
            float distance = targetDirection.magnitude;
            float lookAtAnimalPitch = Mathf.Clamp(-Mathf.Asin(targetDirection.y / distance) * Mathf.Rad2Deg, -lookXLimit, lookXLimit);

            float combinedPitch = lookAtAnimalPitch + smoothedSlopePitch;
            rotationX = Mathf.Lerp(rotationX, combinedPitch, rotationRate);

            foreach (Camera cam in playerCameras)
            {
                if (cam != null)
                    cam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            }
        }
        else
        {
            rotationX = Mathf.Lerp(rotationX, 0f, rotationRate * 0.5f);
            foreach (Camera cam in playerCameras)
                if (cam != null) cam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }

    private void HandleMovementInput()
    {
        currentMoveInput = moveAction.action.ReadValue<Vector2>();
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        float currentSpeed = runAction.action.IsPressed() ? runSpeed : walkSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;
        float curSpeedX = currentMoveInput.y * currentSpeed;
        float curSpeedY = currentMoveInput.x * currentSpeed;
        float movementDirectionY = moveDirection.y;
        moveDirection = forward * curSpeedX + right * curSpeedY;
        moveDirection.y = movementDirectionY;
    }

    private void ApplyGravity()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;
        else if (moveDirection.y < 0)
            moveDirection.y = -2f;
    }

    private void HandleLookInput()
    {
        Vector2 rawLookInput = lookAction.action.ReadValue<Vector2>();
        if (rawLookInput.sqrMagnitude < lookInputDeadZone * lookInputDeadZone)
            rawLookInput = Vector2.zero;

        Vector2 smoothLook = Vector2.SmoothDamp(currentLookInput, rawLookInput, ref _smoothLookVelocity, lookSmoothTime, Mathf.Infinity, Time.deltaTime);
        currentLookInput = smoothLook;

        rotationX += -currentLookInput.y * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        foreach (Camera cam in playerCameras)
            if (cam != null) cam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        transform.rotation *= Quaternion.Euler(0, currentLookInput.x * lookSpeed, 0);
    }

    private void LockMouse()
    {
        if (TestConditionsManager.typeOfHunt.ToLower().Contains("guided hunt") && activeScene.ToLower().Contains("foresthunting")) return;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mouse_locked = true;
        canMove = true;
        if (!lookAction.action.enabled) lookAction.action.Enable();
        if (!moveAction.action.enabled) moveAction.action.Enable();
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        mouse_locked = false;
        canMove = false;
        if (lookAction.action.enabled) lookAction.action.Disable();
        if (moveAction.action.enabled) moveAction.action.Disable();
    }

    private void handleUI()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.isPressed)
        {
            mouseReleased = true;
            cursor_unlocked = false;
            mouse_locked = false;
            UnlockCursor();
        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame
            && StaticVariableManager.isStopTraining == false
            && countDownStart.start_training)
        {
            bool clickedUI = GetUiElementsClicked();
            if (!clickedUI && !TestConditionsManager.typeOfHunt.ToLower().Contains("guided hunt"))
            {
                LockMouse();
                mouseReleased = false;
            }
        }
    }

    private bool GetUiElementsClicked()
    {
        if (ui_raycaster == null || EventSystem.current == null) return false;
        click_data.position = Mouse.current.position.ReadValue();
        click_results.Clear();
        ui_raycaster.Raycast(click_data, click_results);
        return click_results.Count > 0;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (canMove && characterController.isGrounded)
            moveDirection.y = jumpPower;
    }

    private void OnCrouchStart(InputAction.CallbackContext context) { if (canMove) isCrouching = true; }
    private void OnCrouchEnd(InputAction.CallbackContext context) { if (canMove) isCrouching = false; }
    private void HandleCrouch() { characterController.height = isCrouching ? crouchHeight : defaultHeight; }
    public void SwitchCamera(int index) { }

    private System.Collections.IEnumerator UnstuckMovement()
    {
        isUnstucking = true;

        if (navMeshAgent != null)
            navMeshAgent.isStopped = true;

        float moveTime = .25f;
        float distanceMoved = 5f;

        Vector3 backwardTarget = transform.position - transform.forward * distanceMoved;
        float t = 0;
        while (t < moveTime / 2)
        {
            t += Time.deltaTime;
            Vector3 dir = backwardTarget - transform.position;
            dir.y = 0;
            characterController.SimpleMove(dir.normalized * distanceMoved);
            yield return null;
        }

        Vector3 rightTarget = transform.position + transform.right * distanceMoved;
        t = 0;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            Vector3 dir = rightTarget - transform.position;
            dir.y = 0;
            characterController.SimpleMove(dir.normalized * distanceMoved);
            yield return null;
        }

        if (navMeshAgent != null)
            navMeshAgent.isStopped = false;

        isUnstucking = false;
    }
}