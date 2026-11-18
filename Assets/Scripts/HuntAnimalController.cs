using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Unity.VisualScripting;

public class HuntAnimalController : MonoBehaviour
{
    public string animalName = "Deer";
    public Animator anim;
    public GameObject animalDetector;
    public GameObject headCollider;

    public float wanderRadius = 30f;
    public float minEatingTime = 5f;
    public float maxEatingTime = 15f;
    private const float MaxSafetyCooldownTime = 20f;

    private string hunt_type = "Guided hunt";

    private NavMeshAgent navAgent;

    [Header("Evasion Settings")]
    public float evadeTriggerDistance = 10f;
    private bool evasionEnabled = false;

    private GameObject player;
    private Transform playerTransform;
    private Vector3 evadeDestination;
    private const float EvadeDistance = 50f;
    private bool evasionPathCalculated = false;

    private int SetWalkSpeed = 2;
    private int SetOutdoorRunSpeed = 4;
    private int SetGroundRunSpeed = 3;

    private int animalHealth = 2;
    private string activeScene = "";
    private float eatingTimer = 0f;
    private float hitTimer = 0.1f;
    private float destroyTimer = 5f;
    private float resetGunTimer = 1f;
    private float safetyCooldownTimer = 0f;

    private string currentAnimationState = "Idle";
    private const float DestinationReachedThreshold = 3f;
    private float stuckCheckTimer = 0f;
    private const float MaxStuckCheckTime = 1.0f;
    private const float StuckVelocityThreshold = 0.1f;

    private bool isFoodFound = false;
    public bool isKilled = false;
    public bool isDetected = false;
    private bool isHiding = false;
    private bool isSafetyCoolingDown = false;
    private bool isLooking = false;
    private bool isHit = false;
    private bool destinationPending = false;

    private const string FoodTag = "deerFood";
    private List<GameObject> deerFood;
    private GameObject currentFoodTarget = null;
    private Queue<GameObject> attemptedTargets = new Queue<GameObject>();
    private const int MaxAttemptedTargets = 4;
    private float runningSoundDelay = 0f;

    private bool isMoose = false;
    private FoodAllocationManager foodAllocationManager;
    private GameObject lastEatenFood = null;
    private float foodCooldownTimer = 0f;
    private const float FoodReselectionCooldown = 3f;

    private const string EvadeTag = "deerEvadePoint";
    private List<GameObject> evadePoints;

    [Header("Audio Settings")]
    public List<AudioClip> animalSounds; // Assign audio clips in inspector
    private AudioSource audioSource;

    [Header("Audio Distance Settings")]
    public float maxHearingDistance = 30f;  // Beyond this, sound is inaudible
    public float minVolume = 0.01f;         // Minimum volume when far
    public float maxVolume = 0.1f;          // Maximum volume when close

    void Start()
    {
        ApplySettings();

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.enabled = true;
        navAgent.isStopped = false;

        stuckCheckTimer = MaxStuckCheckTime;
        hunt_type = TestConditionsManager.typeOfHunt;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        activeScene = SceneManager.GetActiveScene().name.ToLower();

        if (this.gameObject.name.ToLower().Contains("moose"))
        {
            animalName = "Moose";
            isMoose = true;
        }
        else if (this.gameObject.name.ToLower().Contains("deer"))
            animalName = "Deer";
        else
            animalName = this.gameObject.name.ToLower();

        string foodTag = (animalName == "Moose") ? "mooseFood" : "deerFood";
        string evadeTag = (animalName == "Moose") ? "deerEvadePoint" : "deerEvadePoint";

        deerFood = GameObject.FindGameObjectsWithTag(foodTag).ToList();
        evadePoints = GameObject.FindGameObjectsWithTag(evadeTag).ToList();

        if (deerFood.Count == 0 && animalName == "Moose")
            deerFood = GameObject.FindGameObjectsWithTag("deerFood").ToList();
        if (evadePoints.Count == 0 && animalName == "Moose")
            evadePoints = GameObject.FindGameObjectsWithTag("deerEvadePoint").ToList();

        isFoodFound = false;

        if (isMoose)
        {
            foodAllocationManager = FoodAllocationManager.GetOrCreate();
            if (foodAllocationManager == null)
            {
                Debug.LogError("Failed to initialize FoodAllocationManager!");
            }
            else
            {
                foodAllocationManager.ResetMooseState(transform);
                lastEatenFood = null;
                foodCooldownTimer = 0f;
            }
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        AnimateState("Idle");
    }

    void Update()
    {
        if (navAgent == null || !navAgent.enabled) return;

        if (evasionEnabled) CheckPlayerProximity();
        CheckSurrounding();
        CheckForStuck();
        ManageGunShotCooldown();
        UpdateFoodCooldown();
        MoveAnimal();
        UpdateAudioVolume(); // 🔹 Distance-based audio scaling
    }

    private void ApplySettings()
    {
        switch (TestConditionsManager.difficultySetting)
        {
            case "Easy":
                evasionEnabled = false;
                maxEatingTime = 50;
                break;

            case "Medium":
                evasionEnabled = true;
                evadeTriggerDistance = 10f;
                SetGroundRunSpeed = 5;
                maxEatingTime = 50;
                break;

            case "Hard":
                evasionEnabled = true;
                evadeTriggerDistance = 15f;
                SetGroundRunSpeed = 10;
                maxEatingTime = 50;
                break;

            default:
                evasionEnabled = true;
                evadeTriggerDistance = 10f;
                break;
        }
    }

    private void CheckPlayerProximity()
    {
        if (!evasionEnabled || playerTransform == null || isHiding || isKilled || isHit) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= evadeTriggerDistance)
        {
            isHiding = true;
            evasionPathCalculated = false;
            isFoodFound = false;
            destinationPending = false;
            isSafetyCoolingDown = false;
            navAgent.isStopped = false;
        }
    }

    private void CheckSurrounding()
    {
        isDetected = animalDetector != null &&
                     animalDetector.GetComponent<RaycastDetect>()?.isObjectClose == true;
    }

    private void CheckForStuck()
    {
        if (navAgent.enabled && navAgent.hasPath && !navAgent.isStopped && !isHiding)
        {
            stuckCheckTimer -= Time.deltaTime;

            if (stuckCheckTimer <= 0f)
            {
                if (navAgent.velocity.magnitude < StuckVelocityThreshold)
                {
                    navAgent.isStopped = true;
                    AnimateState("Idle");
                    isFoodFound = false;
                    destinationPending = false;
                }
                stuckCheckTimer = MaxStuckCheckTime;
            }
        }
    }

    private void MoveAnimal()
{
    if (isKilled) { Die(); return; }
    if (isHit) { HandleHitStun(); return; }

    // ✅ ALWAYS react to gunfire
    if (evasionEnabled && StaticVariableManager.isGunFired)
    {
        isSafetyCoolingDown = false;   // cancel safety cooldown
        isHiding = true;               // force hide
        evasionPathCalculated = false; // recalc evade path
        // ⭐ Release food if moose
        if (isMoose && foodAllocationManager != null && currentFoodTarget != null)
        {
            foodAllocationManager.ReleaseFood(currentFoodTarget, transform);
            currentFoodTarget = null;
        }
    }

    if (evasionEnabled && isHiding)
    {
        EvadeToSafety();
        return;
    }

    if (evasionEnabled && isSafetyCoolingDown)
    {
        HandleSafetyCooldown();
        return;
    }

    if (isLooking) { GoToIdle(); return; }

    ExecuteNormalMovement();
}


    private void HandleSafetyCooldown()
    {
        navAgent.isStopped = true;
        AnimateState("Idle");
        StopRunningSound();
        CheckPlayerProximity();
        if (isHiding) return;

        safetyCooldownTimer -= Time.deltaTime;

        if (safetyCooldownTimer <= 0f)
        {
            isSafetyCoolingDown = false;
            isHiding = false;
            evasionPathCalculated = false;
            isFoodFound = false;
            destinationPending = false;
            navAgent.isStopped = false;
        }
    }

    private void ExecuteNormalMovement()
    {
        if (currentFoodTarget != null &&
            destinationPending == false &&
            navAgent.remainingDistance <= DestinationReachedThreshold)
        {
            isFoodFound = true;
            navAgent.isStopped = true;
            HandleEating();
            return;
        }

        if (destinationPending && navAgent.remainingDistance > DestinationReachedThreshold)
        {
            navAgent.isStopped = false;
            navAgent.speed = SetWalkSpeed;
            AnimateState("Walk");
        }

        if (destinationPending &&
            navAgent.remainingDistance <= DestinationReachedThreshold)
        {
            navAgent.isStopped = true;
            destinationPending = false;
            isFoodFound = true;
        }

        if (isFoodFound)
        {
            HandleEating();
        }
        else
        {
            SearchForFood();
        }
    }

    private void HandleEating()
    {
        if (eatingTimer <= 0f)
        {
            eatingTimer = Random.Range(minEatingTime, maxEatingTime);
            AnimateState("Eat");
            PlaySoundByState("Eat"); // 🔹 Play eating sound
        }
        else
        {
            AnimateState("Eat");
        }

        eatingTimer -= Time.deltaTime;

        if (eatingTimer <= 0f)
        {
            isFoodFound = false;
            destinationPending = false;
            eatingTimer = 0f;
            isDetected = false;

            if (isMoose && currentFoodTarget != null && foodAllocationManager != null)
            {
                lastEatenFood = currentFoodTarget;
                foodCooldownTimer = FoodReselectionCooldown;
                foodAllocationManager.ReleaseFood(currentFoodTarget, transform);
            }

            navAgent.isStopped = false;
            currentFoodTarget = null;
            SearchForFood();
        }
    }

    private void UpdateFoodCooldown()
    {
        if (foodCooldownTimer > 0f)
        {
            foodCooldownTimer -= Time.deltaTime;
            if (foodCooldownTimer <= 0f)
            {
                lastEatenFood = null;
            }
        }
    }

    private void SearchForFood()
    {
        if (!destinationPending)
        {
            if (isMoose && foodAllocationManager != null)
            {
                currentFoodTarget = foodAllocationManager.GetAvailableFood(transform, lastEatenFood);
                if (currentFoodTarget != null)
                {
                    if (!foodAllocationManager.ClaimFood(currentFoodTarget, transform))
                        currentFoodTarget = null;
                }
            }
            else
            {
                List<GameObject> availableTargets = deerFood
                    .Where(t => t != currentFoodTarget)
                    .Except(attemptedTargets)
                    .ToList();

                if (availableTargets.Count == 0 && deerFood.Count > 0)
                {
                    attemptedTargets.Clear();
                    availableTargets = deerFood
                        .Where(t => t != currentFoodTarget)
                        .ToList();
                }

                currentFoodTarget = availableTargets
                    .OrderBy(t => Random.value)
                    .FirstOrDefault();
            }

            if (currentFoodTarget != null)
            {
                Vector3 newPos = currentFoodTarget.transform.position;
                SetNavDestination(newPos);
                destinationPending = true;
                navAgent.isStopped = false;
                navAgent.speed = SetWalkSpeed;
                AnimateState("Walk");

                if (!isMoose)
                {
                    attemptedTargets.Enqueue(currentFoodTarget);
                    while (attemptedTargets.Count > MaxAttemptedTargets)
                        attemptedTargets.Dequeue();
                }
            }
            else
            {
                navAgent.isStopped = true;
                AnimateState("Idle");
            }
        }

        if (destinationPending && navAgent.isStopped)
        {
            AnimateState("Idle");
        }
    }

    private void EvadeToSafety()
    {
        if (!evasionPathCalculated)
        {
            navAgent.isStopped = false;

            if (playerTransform == null || evadePoints.Count == 0)
            {
                isHiding = false;
                AnimateState("Idle");
                StopRunningSound();
                return;
            }

            GameObject selectedEvadePoint = evadePoints
                .OrderByDescending(p => Vector3.Distance(p.transform.position, playerTransform.position))
                .FirstOrDefault();

            if (selectedEvadePoint != null)
            {
                evadeDestination = selectedEvadePoint.transform.position;
                SetNavDestination(evadeDestination);
                evasionPathCalculated = true;
                PlayRunningSound();
            }
            else
            {
                isHiding = false;
                evasionPathCalculated = false;
                navAgent.isStopped = true;
                AnimateState("Idle");
                StopRunningSound();
                return;
            }
        }

        int runSpeed = activeScene.Contains("outdoor") ? SetOutdoorRunSpeed + 1 : SetGroundRunSpeed;
        navAgent.speed = runSpeed;

        if (navAgent.remainingDistance <= DestinationReachedThreshold && navAgent.remainingDistance >= 0)
        {
            navAgent.isStopped = true;
            AnimateState("Idle");

            isHiding = false;
            evasionPathCalculated = false;
            isSafetyCoolingDown = true;
            safetyCooldownTimer = MaxSafetyCooldownTime;

            StopRunningSound();
        }
        else
        {
            AnimateState("Run");
            navAgent.isStopped = false;
        }
    }

    private void HandleHitStun()
    {
        navAgent.isStopped = true;
        AnimateState("Idle");

        hitTimer -= Time.deltaTime;
        if (hitTimer <= 0f)
        {
            isHit = false;
            hitTimer = 0.1f;
            navAgent.isStopped = false;
        }
    }

    private void Die()
    {
        navAgent.isStopped = true;
        navAgent.enabled = false;
        AnimateState("Die");
        StopRunningSound();

        if (GetComponent<Rigidbody>() != null)
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = false;
        if (headCollider != null)
            Destroy(headCollider.gameObject);

        if (headCollider != null &&
            headCollider.GetComponent<AnimalHeadReference>()?.isHeadShot != true)
        {
            UpdateKillStatistics();
        }

        if (isMoose && foodAllocationManager != null)
        {
            foodAllocationManager.ReleaseAllFoodForMoose(transform);
        }

        destroyTimer -= Time.deltaTime;
        if (destroyTimer <= 0f)
            Destroy(this.gameObject);
    }

    private void GoToIdle()
    {
        navAgent.isStopped = true;
        AnimateState("Idle");
        StopRunningSound();

        destroyTimer -= Time.deltaTime;

        if (destroyTimer <= 0f)
        {
            isLooking = false;
            destroyTimer = 5f;
            navAgent.isStopped = false;
        }
    }

    public Vector3 GetRandomNavMeshPoint(Vector3 origin, float dist)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist;
        randomDirection += origin;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, dist, NavMesh.AllAreas))
            return hit.position;

        return origin;
    }

    private void SetNavDestination(Vector3 destination)
    {
        if (!navAgent.enabled) return;

        NavMeshPath path = new NavMeshPath();
        if (navAgent.CalculatePath(destination, path) &&
            path.status == NavMeshPathStatus.PathComplete)
        {
            navAgent.SetPath(path);
        }
    }

    public void ApplyDamage(string tagged)
    {
        if (tagged.ToLower().Contains(animalName.ToLower()))
        {
            animalHealth = 0;
            animalHealth--;
            isHit = true;

            PlaySoundByState("Hurt"); // 🔹 Play hurt sound

            string endName = "dead_animal";

            if (animalHealth <= 0)
            {
                this.transform.name = endName;
                this.transform.tag = endName;

                CapsuleCollider bodyCollider = this.GetComponent<CapsuleCollider>();
                if (bodyCollider != null)
                    bodyCollider.enabled = false;

                if (headCollider != null)
                {
                    CapsuleCollider headCapsuleCollider = headCollider.GetComponent<CapsuleCollider>();
                    if (headCapsuleCollider != null)
                        headCapsuleCollider.enabled = false;
                }

                if (isMoose && foodAllocationManager != null && currentFoodTarget != null)
                {
                    foodAllocationManager.ReleaseFood(currentFoodTarget, transform);
                }

                isKilled = true;
                player.GetComponent<FPSController>().FindAnotherAnimal();
            }
        }
    }

    private void ManageGunShotCooldown()
    {
        if (StaticVariableManager.isGunFired)
        {
            resetGunTimer -= Time.deltaTime;
            if (resetGunTimer <= 0f)
            {
                StaticVariableManager.isGunFired = false;
                resetGunTimer = 1f;
            }
        }
    }

    private void UpdateKillStatistics()
    {
        StaticVariableManager.totalBodyShots += 1;
        StaticVariableManager.totalTargetAnimalsKilled += 1;
    }

    private void AnimateState(string state)
    {
        if (anim == null) return;
        if (currentAnimationState == state) return;

        string animalBaseName = "";

        if (animalName.ToLower().Contains("deer")) animalBaseName = "Deer";
        else if (animalName.ToLower().Contains("leopard")) animalBaseName = "Leopard";
        else if (animalName.ToLower().Contains("boar")) animalBaseName = "WildBoar";
        else if (animalName.ToLower().Contains("buffalo")) animalBaseName = "Cape_buffalo";
        else if (animalName.ToLower().Contains("rabbit")) animalBaseName = "WildRabbit";
        else if (animalName.ToLower().Contains("goose")) animalBaseName = "Swan_goose";
        else if (animalName.ToLower().Contains("dragon")) animalBaseName = "Comodo_dragon";
        else if (animalName.ToLower().Contains("pig")) animalBaseName = "Domestic_pig";
        else if (animalName.ToLower().Contains("moose")) animalBaseName = "Moose";

        string animationClipName = "";

        switch (state)
        {
            case "Run": animationClipName = animalBaseName + "_run"; break;
            case "Walk": animationClipName = animalBaseName + "_walk"; break;
            case "Eat": animationClipName = animalBaseName + "_eat"; break;
            case "Idle": animationClipName = animalBaseName + "_idle"; break;
            case "Die": animationClipName = animalBaseName + "_die"; break;
        }

        if (!string.IsNullOrEmpty(animationClipName))
        {
            anim.Play(animationClipName);
            currentAnimationState = state;

            PlaySoundByState(state); // 🔹 Play associated sound

            if (isMoose)
                Debug.Log($"[{gameObject.name}] Animation: {animationClipName}");
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] No animation clip name generated for state: {state}");
        }
    }

    // 🔹 Distance-based running sound
    private void PlayRunningSound()
    {
        if (audioSource == null || animalSounds == null || animalSounds.Count == 0) return;

        AudioClip clip = animalSounds.FirstOrDefault(c => c != null && c.name == "animal_running");
        if (clip == null) return;

        runningSoundDelay = Random.Range(0.5f, 1.0f);
        audioSource.clip = clip;
        audioSource.loop = true;

        StartCoroutine(PlayRunningSoundDelayed());
    }

    private IEnumerator PlayRunningSoundDelayed()
    {
        yield return new WaitForSeconds(runningSoundDelay);

        if (audioSource != null && audioSource.clip != null)
            audioSource.Play();
    }

    private void StopRunningSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }
    }

    // 🔹 Distance-based audio update
    private void UpdateAudioVolume()
    {
        if (audioSource == null || playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance > maxHearingDistance)
        {
            audioSource.volume = 0f;
            if (audioSource.isPlaying) audioSource.Stop();
        }
        else
        {
            float normalizedDistance = 5f - (distance / maxHearingDistance);
            audioSource.volume = Mathf.Lerp(minVolume, maxVolume, normalizedDistance);

            if (!audioSource.isPlaying && audioSource.clip != null)
                audioSource.Play();
        }
    }

    // 🔹 Play sound based on animation or event
private void PlaySoundByState(string state)
{
    if (audioSource == null || animalSounds == null) return;

    string clipName = "";

    switch (state)
    {
        case "Run":
            clipName = "animal_running";
            break;

        case "Eat":
            clipName = "animal_eating";
            break;

        case "Hurt":
            // dynamically set hurt sound based on the animal name
            clipName = animalName.ToLower() + "_hurt";
            break;
    }

    // print("Playing: " + clipName);

    AudioClip clip = animalSounds.FirstOrDefault(c => c != null && c.name.ToLower() == clipName);
    if (clip == null) return;

    audioSource.clip = clip;

    // Loop only for running and eating, never for Hurt
    audioSource.loop = (state == "Run" || state == "Eat");

    // Set volume for Hurt separately to ensure audibility
    if (state == "Hurt")
    {
        audioSource.volume = maxVolume; // full volume
        audioSource.Play(); // force play immediately
        return; // skip further checks to avoid being overridden
    }

    // For Run and Eat, adjust volume dynamically in UpdateAudioVolume
    if (!audioSource.isPlaying)
    {
        audioSource.Play();
    }
}


}
