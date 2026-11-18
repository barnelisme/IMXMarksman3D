using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FoodAllocationManager : MonoBehaviour
{
    public static FoodAllocationManager Instance { get; private set; }

    private const string FOOD_TAG = "deerFood";
    private const string MOOSE_FOOD_TAG = "mooseFood";

    // ⭐ CHANGED: Now tracks a LIST of transforms per food to allow "sharing" if needed
    private Dictionary<GameObject, List<Transform>> foodOccupancy = new Dictionary<GameObject, List<Transform>>();

    private List<GameObject> allAvailableFood = new List<GameObject>();
    private string lastLoadedScene = "";

    public static FoodAllocationManager GetOrCreate()
    {
        if (Instance == null)
        {
            GameObject manager = new GameObject("FoodAllocationManager");
            Instance = manager.AddComponent<FoodAllocationManager>();
            Instance.Awake();
            Instance.Start();
        }
        return Instance;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != lastLoadedScene)
        {
            lastLoadedScene = scene.name;
            InitializeFoodDatabase();
        }
    }

    private void Start()
    {
        lastLoadedScene = SceneManager.GetActiveScene().name;
        InitializeFoodDatabase();
    }

    public void InitializeFoodDatabase()
    {
        allAvailableFood.Clear();
        foodOccupancy.Clear();

        GameObject[] mooseFoodObjects = GameObject.FindGameObjectsWithTag(MOOSE_FOOD_TAG);

        if (mooseFoodObjects.Length > 0)
        {
            allAvailableFood.AddRange(mooseFoodObjects);
        }
        else
        {
            GameObject[] deerFoodObjects = GameObject.FindGameObjectsWithTag(FOOD_TAG);
            allAvailableFood.AddRange(deerFoodObjects);
        }

        foreach (GameObject food in allAvailableFood)
        {
            if (food != null)
                foodOccupancy[food] = new List<Transform>(); // Initialize empty lists
        }
    }

    /// <summary>
    /// Finds the best food. 
    /// 1. Prioritizes points with 0 moose (Solitary).
    /// 2. If none are empty, finds points with the lowest occupancy (Even distribution).
    /// </summary>
    public GameObject GetAvailableFood(Transform requestingMoose, GameObject excludeFood = null)
    {
        if (requestingMoose == null || foodOccupancy.Count == 0) return null;

        // 1. Try to find a COMPLETELY empty food point first
        var emptyPoints = foodOccupancy
            .Where(kvp => kvp.Value.Count == 0 && kvp.Key != null && kvp.Key != excludeFood)
            .Select(kvp => kvp.Key)
            .ToList();

        if (emptyPoints.Count > 0)
        {
            // Return nearest empty point
            return emptyPoints
                .OrderBy(f => Vector3.Distance(requestingMoose.position, f.transform.position))
                .FirstOrDefault();
        }

        // 2. FALLBACK: If all are taken, find the one with the FEWEST moose assigned to it
        // This ensures that if they must share, they don't all pile onto the same one.
        return foodOccupancy
            .Where(kvp => kvp.Key != null && kvp.Key != excludeFood)
            .OrderBy(kvp => kvp.Value.Count) // Sort by fewest moose
            .ThenBy(kvp => Vector3.Distance(requestingMoose.position, kvp.Key.transform.position)) // Then by distance
            .Select(kvp => kvp.Key)
            .FirstOrDefault();
    }

    public bool ClaimFood(GameObject foodObject, Transform claimingMoose)
    {
        if (foodObject == null || claimingMoose == null || !foodOccupancy.ContainsKey(foodObject)) return false;

        if (!foodOccupancy[foodObject].Contains(claimingMoose))
        {
            foodOccupancy[foodObject].Add(claimingMoose);
        }
        return true;
    }

    public void ReleaseFood(GameObject foodObject, Transform releasingMoose)
    {
        if (foodObject != null && foodOccupancy.ContainsKey(foodObject))
        {
            foodOccupancy[foodObject].Remove(releasingMoose);
        }
    }

    public void ReleaseAllFoodForMoose(Transform mooseTransform)
    {
        if (mooseTransform == null) return;

        foreach (var list in foodOccupancy.Values)
        {
            list.Remove(mooseTransform);
        }
    }

    public void ResetMooseState(Transform mooseTransform)
    {
        ReleaseAllFoodForMoose(mooseTransform);
    }

    public void ReleaseAllFoodForAllMoose()
    {
        foreach (var list in foodOccupancy.Values)
        {
            list.Clear();
        }
    }

    public static void ResetInstance()
    {
        if (Instance != null)
        {
            Instance.ReleaseAllFoodForAllMoose();
            Destroy(Instance.gameObject);
            Instance = null;
        }
    }
}