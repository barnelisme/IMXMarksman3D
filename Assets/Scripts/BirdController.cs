using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class BirdController : MonoBehaviour
{
    [Header("Flight Points")]
    public Transform[] flyingPoints;
    public GameObject[] wings;
    public GameObject[] legs;
    public GameObject head;

    [Header("Flight Settings")]
    public float takeOffHeight = 3f;
    public float flyRadius = 2f;
    public float moveSpeed = 6f;
    public float steeringSmoothness = 2f;
    public float floatDuration = 10f; // Time after last shot before returning

    [Header("Death Settings")]
    public float deathForwardForce = 2f;
    public float deathSpinTorque = 200f;

    private Vector3 groundPosition;
    private Vector3 selectedFlyPoint;
    private Vector3 currentTarget;
    private Vector3 velocity;

    private bool isFlying = false;
    private bool isFloating = false;
    private bool isDead = false;

    private float lastShotTime = -999f;

    private Rigidbody rb;
    public Animator anim;

    private Quaternion groundRotation;
    [SerializeField] private float realignDistance = 1.0f;
    [SerializeField] private float landingAlignSpeed = 5f;

    void Start()
    {
        groundPosition = transform.position;
        groundRotation = transform.rotation;
        velocity = transform.forward * moveSpeed;


        rb = GetComponent<Rigidbody>();
        anim = this.GetComponent<Animator>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // PlayEatAnimation();
        StartCoroutine(StartEating());
        if (flyingPoints == null || flyingPoints.Length == 0)
        {
            Debug.LogError("No flying points assigned!");
        }
    }

    void Update()
    {
        if (isDead) return;

        // Detect shot trigger
        if (StaticVariableManager.isGunFired)
        {
            lastShotTime = Time.time;

            if (!isFlying)
            {
                StartCoroutine(FlyRoutine());
            }
        }

        if (isFloating)
        {
            SmoothSteerTowards(currentTarget);

            // If no shots for floatDuration → return
            if (Time.time - lastShotTime > floatDuration)
            {
                StartCoroutine(ReturnToGround());
            }
        }
    }

    IEnumerator StartEating()
    {
        // Random small delay to desynchronize multiple birds
        float randomDelay = Random.Range(0f, 1f); // delay between 0 and 1 second
        yield return new WaitForSeconds(randomDelay);

        // Play the eating animation
        PlayEatAnimation();
    }


    IEnumerator FlyRoutine()
    {
        isFlying = true;

        // Take off
        Vector3 takeOffTarget = groundPosition + Vector3.up * takeOffHeight;
        PlayFlyAnimation();

        /*while (Vector3.Distance(transform.position, takeOffTarget) > 0.2f)
        {
            SmoothSteerTowards(takeOffTarget);
            yield return null;
        }*/

        // Select fly point
        selectedFlyPoint = flyingPoints[Random.Range(0, flyingPoints.Length)].position;

        // Fly directly to it
        while (Vector3.Distance(transform.position, selectedFlyPoint) > 0.5f)
        {
            SmoothSteerTowards(selectedFlyPoint);
            yield return null;
        }

        isFloating = true;
        ChooseNewFloatTarget();

        // 🔥 Do NOT control floating here.
        // Update() already handles it.
    }


    IEnumerator ReturnToGround()
    {
        isFloating = false;

        print("Landing...");

        while (Vector3.Distance(transform.position, groundPosition) > 0.2f)
        {
            // 🔥 If gun fires during landing → abort and fly again
            if (Time.time - lastShotTime <= 0.1f)
            {
                print("Landing interrupted — taking off again");

                isFlying = false; // allow FlyRoutine to restart
                StartCoroutine(FlyRoutine());
                yield break; // stop this coroutine immediately
            }

            float distance = Vector3.Distance(transform.position, groundPosition);

            // Always move toward ground
            SmoothSteerTowards(groundPosition);

            // When close, align upright
            if (distance < realignDistance)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    groundRotation,
                    landingAlignSpeed * Time.deltaTime
                );
            }

            yield return null;
        }

        // Snap perfectly aligned on landing
        transform.position = groundPosition;
        transform.rotation = groundRotation;

        PlayEatAnimation();
        print("Landing complete");

        isFlying = false;
    }




    void ChooseNewFloatTarget()
    {
        Vector3 offset = Random.insideUnitSphere * flyRadius;
        offset.y *= 0.5f;
        currentTarget = selectedFlyPoint + offset;
    }

    void SmoothSteerTowards(Vector3 target)
    {
        Vector3 desiredDirection = (target - transform.position).normalized;
        Vector3 desiredVelocity = desiredDirection * moveSpeed;

        velocity = Vector3.Lerp(
            velocity,
            desiredVelocity,
            steeringSmoothness * Time.deltaTime
        );

        transform.position += velocity * Time.deltaTime;

        if (velocity.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(velocity.normalized);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                steeringSmoothness * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, currentTarget) < 0.5f)
        {
            ChooseNewFloatTarget();
        }
    }

    private void PlayFlyAnimation()
    {
        // anim.Play("Swan_goose_run");
        anim.enabled = false;

        foreach (GameObject leg in legs)
        {
            Vector3 scale = leg.transform.localScale;
            scale.x = 0f;
            leg.transform.localScale = scale;
        }

        Vector3 headScale = head.transform.localScale;
        headScale.x = 0.5f;
        head.transform.localScale = headScale;

        foreach (GameObject wing in wings)
        {
            wing.SetActive(true);
        }
    }

    private void PlayEatAnimation()
    {
        anim.enabled = true;
        foreach (GameObject wing in wings)
        {
            wing.SetActive(false);
        }

        anim.Play("Swan_goose_eat");
    }
    private void PlayWalkAnimation()
    {
        anim.Play("Swan_goose_ward");
    }
    private void PlayDieAnimation()
    {
        anim.enabled = true;
        anim.Play("Swan_goose_die");
    }

    // =========================
    // 💀 DEATH WITH RIGIDBODY
    // =========================
    private void ApplyDamage(string tagged)
    {
        print("Point reached...");
        if (isDead) return;

        print("Killed");

        isDead = true;
        StaticVariableManager.totalTargetAnimalsKilled++;
        foreach (GameObject wing in wings)
        {
            // wing.SetActive(false);
        }
        PlayDieAnimation();

        StopAllCoroutines();
        isFlying = false;
        isFloating = false;

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.linearVelocity = velocity.normalized * deathForwardForce;
        rb.angularVelocity = Random.insideUnitSphere * deathSpinTorque;
    }
}
