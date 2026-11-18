using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class AnimalSpawner : MonoBehaviour
{
    public static int animalPopulation = 10; // Initial population
    public static int minPopulation = 1;
    public static int maxPopulation = 25;

    [Header("Manual Spawn Control")]
    public List<Transform> spawn_locations;
    public GameObject[] whatToSpawnPrefab;

    [Header("Spawn Proximity Control")]
    public float minSpawnDistance = 50f;
    public float maxSpawnDistance = 150f;

    public Terrain terrain;

    private Vector3 terrainMin;
    private Vector3 terrainMax;
    private float maxSafeHeight;

    private List<string> animalNamesList = new List<string>();
    private string activeScene = "";
    private int totAnimals = 0;
    private Transform playerTransform;

    private float timeToRespawn;
    private float setTimeToRespawn = 5f;

    private List<int> target_animal_indices = new List<int>();

    private int mooseMalePrefabIndex = -1;
    private int mooseFemalePrefabIndex = -1;
    private HashSet<int> spawnLocationsUsedForMoose = new HashSet<int>();

    private void Start()
    {
        // TestConditionsManager.animalName = "Moose";
        // animalPopulation = 50;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
        else
        {
            Debug.LogError("Player not found");
            playerTransform = new GameObject("DummyPlayer").transform;
        }

        GameObject terrObj = GameObject.FindGameObjectWithTag("Terrain");
        if (terrObj != null) terrain = terrObj.GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogWarning("No terrain found! Terrain-based context variables will be limited.");
        }
        else
        {
            TerrainData td = terrain.terrainData;
            terrainMin = terrain.transform.position;
            terrainMax = terrainMin + td.size;
            maxSafeHeight = terrainMin.y + td.size.y * 0.95f;
        }

        if (spawn_locations == null || spawn_locations.Count == 0)
        {
            Debug.LogError("No manual spawn locations assigned in the Inspector! Spawning disabled.");
            enabled = false;
            return;
        }

        activeScene = SceneManager.GetActiveScene().name;
        timeToRespawn = setTimeToRespawn;

        assignAnimalNames();

        if (TestConditionsManager.animalName.ToLower().Contains("moose"))
        {
            FindMoosePrefabIndices();
        }

        SpawnInitialPopulation();

        if (activeScene.ToLower().Contains("direct")) setTimeToRespawn = 3.5f;
        else if (activeScene.ToLower().Contains("avoid")) setTimeToRespawn = 2f;

        timeToRespawn = setTimeToRespawn;
    }

    private void Update()
    {
        totAnimals = GameObject.FindGameObjectsWithTag("deer").Length;

        if (timeToRespawn <= 0f && !StaticVariableManager.isTrainingPause && totAnimals < animalPopulation)
        {
            timeToRespawn = setTimeToRespawn;
            int prefabIndex = GetPrefabIndexBasedOnScene();

            if (prefabIndex >= 0 && prefabIndex != mooseMalePrefabIndex && prefabIndex != mooseFemalePrefabIndex)
            {
                spawnRandomAnimal(prefabIndex);
            }
        }
        else
        {
            timeToRespawn -= Time.deltaTime;
        }
    }

    private void SpawnInitialPopulation()
    {
        if (TestConditionsManager.animalName.ToLower().Contains("moose") && mooseMalePrefabIndex >= 0 && mooseFemalePrefabIndex >= 0)
        {
            int totalMooseToSpawn = animalPopulation;

            // ⭐ BALANCED RATIO LOGIC
            // Start with a strict 50/50 split
            int maleCount = totalMooseToSpawn / 2;

            // If the number is odd, flip a coin to decide if the "extra" moose is male or female
            if (totalMooseToSpawn % 2 != 0)
            {
                if (Random.value > 0.5f) maleCount++;
            }

            int femaleCount = totalMooseToSpawn - maleCount;

            Debug.Log($"[AnimalSpawner] Balanced Spawning: {maleCount} males, {femaleCount} females (Total: {totalMooseToSpawn})");

            // Create assignment list
            List<bool> genderAssignments = new List<bool>();
            for (int i = 0; i < maleCount; i++) genderAssignments.Add(true);
            for (int i = 0; i < femaleCount; i++) genderAssignments.Add(false);

            // Shuffle assignments so they don't spawn in "blocks" of gender
            for (int i = genderAssignments.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                bool temp = genderAssignments[i];
                genderAssignments[i] = genderAssignments[randomIndex];
                genderAssignments[randomIndex] = temp;
            }

            // Identify "Safe" locations far from player
            List<int> safeLocationIndices = new List<int>();
            for (int i = 0; i < spawn_locations.Count; i++)
            {
                float dist = Vector3.Distance(playerTransform.position, spawn_locations[i].position);
                if (dist >= minSpawnDistance && dist <= maxSpawnDistance)
                {
                    safeLocationIndices.Add(i);
                }
            }

            if (safeLocationIndices.Count == 0)
            {
                Debug.LogError("[AnimalSpawner] No safe manual spawn locations! Try moving the player or adjusting distance settings.");
                return;
            }

            // Spawning loop - meeting the EXACT total count
            for (int i = 0; i < totalMooseToSpawn; i++)
            {
                int randomIndexFromSafe = safeLocationIndices[Random.Range(0, safeLocationIndices.Count)];
                SpawnMooseAtLocation(randomIndexFromSafe, genderAssignments[i]);
            }
        }
        else
        {
            // Default logic for other animals
            for (int i = 0; i < animalPopulation; i++)
            {
                int prefabIndex = GetPrefabIndexBasedOnScene();
                if (prefabIndex >= 0) spawnRandomAnimal(prefabIndex);
            }
        }
    }

    private void SpawnMooseAtLocation(int locationIndex, bool isMale)
    {
        // Validation check for index
        if (locationIndex < 0 || locationIndex >= spawn_locations.Count) return;

        int prefabIndex = isMale ? mooseMalePrefabIndex : mooseFemalePrefabIndex;
        if (prefabIndex < 0) return;

        Vector3 spawnPoint = spawn_locations[locationIndex].position;

        // Ensure spawn point is on NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPoint, out hit, 10f, NavMesh.AllAreas))
        {
            spawnPoint = hit.position;
            Instantiate(whatToSpawnPrefab[prefabIndex], spawnPoint, Quaternion.identity);

            // Track used locations (useful for debugging or logic elsewhere)
            if (!spawnLocationsUsedForMoose.Contains(locationIndex))
                spawnLocationsUsedForMoose.Add(locationIndex);

            string genderLabel = isMale ? "male" : "female";
            Debug.Log($"[AnimalSpawner] {char.ToUpper(genderLabel[0])}{genderLabel.Substring(1)} moose spawned at location {locationIndex}.");
        }
        else
        {
            Debug.LogWarning($"[AnimalSpawner] Location {locationIndex} is not on NavMesh. Skipping this specific spawn.");
        }
    }

    private int GetPrefabIndexBasedOnScene()
    {
        if (target_animal_indices.Count > 0)
            return target_animal_indices[Random.Range(0, target_animal_indices.Count)];

        string s = activeScene.ToLower();

        if (s.Contains("direct"))
        {
            Debug.LogWarning("Direct scene but no target animals found. Skipping spawn.");
            return -1;
        }
        else if (s.Contains("avoid"))
        {
            return Random.Range(0, animalNamesList.Count);
        }
        else
        {
            return Random.Range(0, whatToSpawnPrefab.Length);
        }
    }

    private void spawnRandomAnimal(int prefabIndex)
    {
        if (prefabIndex < 0 || prefabIndex >= whatToSpawnPrefab.Length)
        {
            Debug.LogError("Invalid prefab index");
            return;
        }

        if (spawn_locations == null || spawn_locations.Count == 0)
        {
            Debug.LogError("No spawn locations assigned");
            return;
        }

        int randomIndex = Random.Range(0, spawn_locations.Count);
        Transform spawnTransform = spawn_locations[randomIndex];

        if (spawnTransform == null)
        {
            Debug.LogError("Spawn location is NULL");
            return;
        }

        Instantiate(
            whatToSpawnPrefab[prefabIndex],
            spawnTransform.position,
            Quaternion.identity
        );
    }


    private void assignAnimalNames()
    {
        target_animal_indices.Clear();
        animalNamesList.Clear();

        string targetName = TestConditionsManager.animalName.ToLower();

        int i = 0;
        foreach (GameObject prefab in whatToSpawnPrefab)
        {
            animalNamesList.Add(prefab.name);
            if (prefab.name.ToLower().Contains(targetName))
                target_animal_indices.Add(i);
            i++;
        }

        if (target_animal_indices.Count == 0 && !string.IsNullOrEmpty(targetName))
        {
            Debug.LogWarning($"Target animal '{targetName}' not found in any prefab name.");
        }
    }

    private Vector3 GetManualSpawnPoint()
    {
        Vector3 playerPos = playerTransform.position;

        for (int attempts = 0; attempts < 50; attempts++)
        {
            int index = Random.Range(0, spawn_locations.Count);
            Transform candidateTransform = spawn_locations[index];
            Vector3 candidatePoint = candidateTransform.position;

            float dist = Vector3.Distance(playerPos, candidatePoint);
            if (dist < minSpawnDistance || dist > maxSpawnDistance) continue;

            return candidatePoint;
        }

        Debug.LogWarning("Failed to find a manual spawn point outside the player's proximity after 50 attempts.");
        return Vector3.zero;
    }

    private void FindMoosePrefabIndices()
    {
        mooseMalePrefabIndex = -1;
        mooseFemalePrefabIndex = -1;

        if (!TestConditionsManager.animalName.ToLower().Contains("moose"))
        {
            Debug.Log("[AnimalSpawner] Test condition is not for moose. Skipping moose prefab search.");
            return;
        }

        for (int i = 0; i < whatToSpawnPrefab.Length; i++)
        {
            string prefabNameLower = whatToSpawnPrefab[i].name.ToLower();

            if (prefabNameLower.Contains("moose") &&
               (prefabNameLower.Contains("male") || prefabNameLower.Contains("_m") || prefabNameLower.EndsWith("m")))
            {
                mooseMalePrefabIndex = i;
                Debug.Log($"[AnimalSpawner] Found male moose prefab at index {i}: {whatToSpawnPrefab[i].name}");
            }

            if (prefabNameLower.Contains("moose") &&
               (prefabNameLower.Contains("female") || prefabNameLower.Contains("_f") || prefabNameLower.EndsWith("f")))
            {
                mooseFemalePrefabIndex = i;
                Debug.Log($"[AnimalSpawner] Found female moose prefab at index {i}: {whatToSpawnPrefab[i].name}");
            }
        }

        if (mooseMalePrefabIndex < 0 && mooseFemalePrefabIndex < 0)
        {
            for (int i = 0; i < whatToSpawnPrefab.Length; i++)
            {
                if (whatToSpawnPrefab[i].name.ToLower().Contains("moose"))
                {
                    Debug.LogWarning("[AnimalSpawner] Found generic moose prefab, but no male/female variants. Please ensure prefabs are named with '_m' or '_f' suffixes.");
                    break;
                }
            }
        }
    }

    private void SpawnMooseAtLocation_2(int locationIndex, bool isMale)
    {
        if (locationIndex < 0 || locationIndex >= spawn_locations.Count)
        {
            Debug.LogWarning($"[AnimalSpawner] Invalid location index: {locationIndex}");
            return;
        }

        int prefabIndex = isMale ? mooseMalePrefabIndex : mooseFemalePrefabIndex;
        if (prefabIndex < 0)
        {
            string gender = isMale ? "male" : "female";
            Debug.LogError($"[AnimalSpawner] Invalid prefab index for {gender} moose");
            return;
        }

        Vector3 spawnPoint = spawn_locations[locationIndex].position;

        // ⭐ NEW: Ensure spawn point is on a NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPoint, out hit, 10f, NavMesh.AllAreas))
        {
            spawnPoint = hit.position;
            Instantiate(whatToSpawnPrefab[prefabIndex], spawnPoint, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Moose spawn point not on NavMesh at location index: " + locationIndex);
        }

        spawnLocationsUsedForMoose.Add(locationIndex);
        string genderLabel = isMale ? "male" : "female";
        Debug.Log($"[AnimalSpawner] {char.ToUpper(genderLabel[0])}{genderLabel.Substring(1)} moose spawned at location {locationIndex}. Total moose: {spawnLocationsUsedForMoose.Count}");
    }

}
