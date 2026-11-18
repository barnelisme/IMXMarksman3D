using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
public class BearAIController : MonoBehaviour
{
    [Header("Identity")]
    public string animalName = "Bear";

    [Header("Components")]
    public Animator anim;
    public NavMeshAgent agent;
    public AudioSource audioSource;

    [Header("Sounds")]
    public AudioClip chaseLoop;
    public AudioClip attackLoop;
    public AudioClip deathSound;

    [Header("Targets")]
    public Transform player;
    public List<GameObject> bear_food;
    public List<GameObject> bear_evade_points;

    [Header("Distances")]
    public float detectPlayerDistance = 12f;
    public float attackDistance = 3f;

    [Header("Speeds")]
    public float walkSpeed = 2f;
    public float runSpeed = 4.5f;

    [Header("Evade")]
    public float evadeIdleTime = 10f;

    [Header("Eating")]
    public float minEatTime = 5f;
    public float maxEatTime = 10f;

    [Header("State")]
    public bool isKilled;

    bool isEvading;
    bool isEating;
    bool evadeReached;
    bool playerKilled;

    float eatTimer;
    float evadeIdleTimer;

    GameObject currentFood;
    Vector3 evadeDestination;

    GameObject lastEvadePoint;
    string currentAnim;

    const float DEST_THRESHOLD = 1.5f;

    float attackTimer;
    bool attackTimerRunning;
    const float ATTACK_DURATION = 5f;

    Vector3 lastPlayerPosition; // ✅ NEW

    // =========================
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.updateRotation = true;
        agent.isStopped = false;

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // bear_food = GameObject.FindGameObjectsWithTag("bearFood").ToList();
        // bear_evade_points = GameObject.FindGameObjectsWithTag("bearEvadePoint").ToList();

        PlayAnim("Idle");
    }

    // =========================
    void Update()
    {
        if (isKilled || agent == null) return;

        // ✅ AFTER PLAYER DEATH — IGNORE PLAYER COMPLETELY
        if (playerKilled)
        {
            NormalBehavior();
            return;
        }

        if (attackTimerRunning)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= ATTACK_DURATION)
            {
                // Debug.Log("Player killed");
                StaticVariableManager.isBearKilledPlayer = true;
                StaticVariableManager.isStopTraining = true;

                attackTimerRunning = false;
                playerKilled = true;
                lastPlayerPosition = player.position; // ✅ STORE LAST POSITION

                StopAllLoops();
                PlayAnim("Idle");

                agent.ResetPath();
                agent.isStopped = false;
                return;
            }
        }

        if (StaticVariableManager.isGunFired && player != null)
        {
            float playerDistance = Vector3.Distance(transform.position, player.position);

            if (playerDistance <= (detectPlayerDistance * 2))
            {
                ForceNewEvade();
                return;
            }
        }

        if (isEvading)
        {
            EvadeBehavior();
            return;
        }

        if (CheckAttack())
            return;

        NormalBehavior();
    }

    // =========================
    // EVADE
    // =========================
    void ForceNewEvade()
    {
        if (bear_evade_points == null || bear_evade_points.Count == 0)
            return;

        isEvading = true;
        isEating = false;
        evadeReached = false;
        evadeIdleTimer = 0f;

        attackTimerRunning = false;

        agent.ResetPath();
        agent.isStopped = false;
        agent.speed = runSpeed;

        GameObject selectedEvadePoint;

        if (bear_evade_points.Count == 1)
        {
            selectedEvadePoint = bear_evade_points[0];
        }
        else
        {
            var available = bear_evade_points
                .Where(p => p != lastEvadePoint)
                .OrderBy(p => Vector3.Distance(transform.position, p.transform.position))
                .ToList();

            selectedEvadePoint = available.Count > 0 ? available[0] : bear_evade_points[0];
        }

        lastEvadePoint = selectedEvadePoint;
        evadeDestination = selectedEvadePoint.transform.position;

        agent.SetDestination(evadeDestination);
        PlayAnim("Run");
        PlayChaseSound();
    }

    void EvadeBehavior()
    {
        if (player && Vector3.Distance(transform.position, player.position) <= detectPlayerDistance)
        {
            isEvading = false;
            return;
        }

        if (!evadeReached)
        {
            if (!agent.hasPath)
                agent.SetDestination(evadeDestination);

            PlayAnim("Run");

            if (agent.remainingDistance <= DEST_THRESHOLD)
            {
                evadeReached = true;
                agent.isStopped = true;
                agent.ResetPath();

                StopAllLoops();
                PlayAnim("Idle");

                evadeIdleTimer = evadeIdleTime;
            }
            return;
        }

        evadeIdleTimer -= Time.deltaTime;
        if (evadeIdleTimer <= 0f)
        {
            isEvading = false;
            evadeReached = false;
            agent.isStopped = false;
        }
    }

    // =========================
    // ATTACK
    // =========================
    bool CheckAttack()
    {
        if (!player || playerKilled) return false;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > detectPlayerDistance)
        {
            attackTimerRunning = false;
            return false;
        }

        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.SetDestination(player.position);

        FacePlayer();

        if (dist <= attackDistance)
        {
            agent.isStopped = true;
            FacePlayer();

            PlayAnim("Attack");
            PlayAttackSound();

            if (!attackTimerRunning)
            {
                attackTimerRunning = true;
                attackTimer = 0f;
            }
        }
        else
        {
            PlayAnim("Run");
            PlayChaseSound();
        }

        return true;
    }

    // =========================
    // FOOD / MOVEMENT
    // =========================
    void NormalBehavior()
    {
        if (isEating)
        {
            eatTimer -= Time.deltaTime;
            if (eatTimer <= 0f)
            {
                isEating = false;
                currentFood = null;
                agent.isStopped = false;
            }
            return;
        }

        if (currentFood == null)
        {
            if (bear_food.Count == 0) return;

            // ✅ FILTER FOOD AWAY FROM PLAYER CORPSE
            var validFood = bear_food
                .Where(f => !playerKilled || Vector3.Distance(f.transform.position, lastPlayerPosition) >= 5f)
                .ToList();

            if (validFood.Count == 0) return;

            currentFood = validFood[Random.Range(0, validFood.Count)];

            agent.speed = walkSpeed;
            agent.isStopped = false;
            agent.SetDestination(currentFood.transform.position);
            PlayAnim("Walk"); // ✅ ENSURE NO SLIDING
        }

        if (agent.remainingDistance <= DEST_THRESHOLD)
        {
            agent.isStopped = true;
            eatTimer = Random.Range(minEatTime, maxEatTime);
            PlayAnim("Eat");
            isEating = true;
        }
    }

    // =========================
    // DAMAGE
    // =========================
    public void ApplyDamage(string tag)
    {
        if (!tag.ToLower().Contains(animalName.ToLower())) return;

        isKilled = true;
        agent.isStopped = true;

        StopAllLoops();

        audioSource.loop = false;
        audioSource.clip = deathSound;
        audioSource.Play();

        PlayAnim("Die");
    }

    // =========================
    // HELPERS
    // =========================
    void FacePlayer()
    {
        if (!player) return;

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * 8f);
    }

    void PlayAnim(string state)
    {
        if (currentAnim == state) return;
        anim.Play("Bear_" + state.ToLower());
        currentAnim = state;
    }

    void PlayChaseSound()
    {
        if (audioSource.clip == chaseLoop && audioSource.isPlaying) return;
        audioSource.loop = true;
        audioSource.clip = chaseLoop;
        audioSource.Play();
    }

    void PlayAttackSound()
    {
        if (audioSource.clip == attackLoop && audioSource.isPlaying) return;
        audioSource.loop = true;
        audioSource.clip = attackLoop;
        audioSource.Play();
    }

    void StopAllLoops()
    {
        audioSource.loop = false;
        audioSource.Stop();
    }
}
