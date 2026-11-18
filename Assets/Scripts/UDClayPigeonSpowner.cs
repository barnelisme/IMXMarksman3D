using UnityEngine;
using UnityEngine.SceneManagement;

public class UDClayPigeonSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] spawnLocations;
    public GameObject targetPrefab;

    [Header("Launch Settings")]
    public float minLaunchForce = 18f;
    public float maxLaunchForce = 24f;
    public float upwardBias = 0.25f;
    public bool addSpin = true;
    public float spinForce = 15f;

    [Header("Firing Settings")]
    public float timeBetweenShots = 2f;

    private float shotTimer;

    private bool isShapePlateScene;
    private bool isClayPigeonScene;
    private string activeScene;

    [Header("Clay Mode Settings")]
    public float delayAfterHit = 0.5f;

    private float postHitTimer = 0f;
    private bool waitingAfterHit = false;


    void Start()
    {
        activeScene = SceneManager.GetActiveScene().name.ToLower();

        isShapePlateScene = activeScene.Contains("shapeplate");
        isClayPigeonScene = activeScene.Contains("claypigeon");

        shotTimer = timeBetweenShots;

        if (isClayPigeonScene)
        {
            StaticVariableManager.plateDestroyed = true;
        }
    }

    void Update()
    {
        if (!countDownStart.start_training)
            return;

        if (isShapePlateScene)
        {
            HandleTimedSpawning();
        }
        else if (isClayPigeonScene)
        {
            delayAfterHit = StaticVariableManager.delayAfterHit;
            maxLaunchForce = StaticVariableManager.launchForce;
            upwardBias = StaticVariableManager.upwardsBias;


            if (StaticVariableManager.plateDestroyed && !waitingAfterHit)
            {
                waitingAfterHit = true;
                postHitTimer = delayAfterHit;
                StaticVariableManager.plateDestroyed = false;
            }

            if (waitingAfterHit)
            {
                postHitTimer -= Time.deltaTime;

                if (postHitTimer <= 0f)
                {
                    SpawnPlate();
                    waitingAfterHit = false;
                }
            }

        }
    }

    private void HandleTimedSpawning()
    {
        shotTimer -= Time.deltaTime;

        if (shotTimer <= 0f)
        {
            SpawnPlate();
            shotTimer = timeBetweenShots;
        }
    }

    private void SpawnPlate()
    {
        if (spawnLocations == null || spawnLocations.Length == 0 || targetPrefab == null)
        {
            Debug.LogWarning("Spawner not configured correctly.");
            return;
        }

        int index = Random.Range(0, spawnLocations.Length);
        Transform spawnPoint = spawnLocations[index].transform;

        GameObject plate = Instantiate(
            targetPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        Rigidbody rb = plate.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogWarning("Target prefab missing Rigidbody.");
            return;
        }

        rb.useGravity = true;

        Vector3 launchDirection = spawnPoint.forward;
        launchDirection.y += upwardBias;
        launchDirection.Normalize();

        float launchForce = Random.Range(minLaunchForce, maxLaunchForce);

        rb.linearVelocity = launchDirection * launchForce;

        if (addSpin)
        {
            rb.angularVelocity = new Vector3(
                Random.Range(-spinForce, spinForce),
                Random.Range(-spinForce, spinForce),
                Random.Range(-spinForce, spinForce)
            );
        }
    }
}
