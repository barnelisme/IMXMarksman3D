using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTypeManager : MonoBehaviour
{

    [Header("Target Type Variables")]
    public List<GameObject> targetTypes = new List<GameObject>();
    public Transform spawnLocation;
    private GameObject whatToSpawnPrefab;
    private GameObject whatToSpawnClone;


    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        switch(StaticVariableManager.targetType)
        {
            case "circle":
                prefab = targetTypes[0];
                break;
            case "square":
                prefab = targetTypes[1];
                break;
            case "rhombas":
                prefab = targetTypes[2];
                break;
            case "triangle":
                prefab = targetTypes[3];
                break;
            case "pentagon":
                prefab = targetTypes[4];
                break;
            case "octagon":
                prefab = targetTypes[5];
                break;
        }
        setTargetType();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void setTargetType()
    {
        if (prefab != null)
        {
            // Get the MeshFilter component from the prefab
            MeshFilter prefabMeshFilter = prefab.GetComponentInChildren<MeshFilter>();
            if (prefabMeshFilter != null)
            {
                // Get the Mesh from the prefab's MeshFilter
                Mesh prefabMesh = prefabMeshFilter.sharedMesh;

                // Replace the mesh of this GameObject
                MeshFilter currentMeshFilter = GetComponent<MeshFilter>();
                if (currentMeshFilter != null)
                {
                    currentMeshFilter.mesh = prefabMesh;

                    //Take transform setting before mesh changes
                    float newXScalePoints = (32.52f - this.transform.localScale.x);
                    float newYScalePoints = (30.52f - this.transform.localScale.x);
                    this.transform.localScale = prefab.transform.localScale;

                    //Calculate and apply New transform setting
                    Vector3 currentScale = this.transform.localScale;
                    currentScale.x -= newXScalePoints * 2.35f;
                    currentScale.y -= newXScalePoints * 2.35f;
                    this.transform.localScale = currentScale;

                    this.transform.localRotation = prefab.transform.localRotation;

                    // Reset and align MeshCollider
                    MeshCollider meshCollider = GetComponent<MeshCollider>();
                    if (meshCollider != null)
                    {
                        meshCollider.sharedMesh = null; // Reset the mesh collider
                        meshCollider.sharedMesh = prefabMesh; // Reassign the new mesh
                    }
                }
                else
                {
                    Debug.LogError("MeshFilter component is missing on the current GameObject.");
                }
            }
            else
            {
                Debug.LogError("MeshFilter component is missing on the prefab.");
            }
        }
        else
        {
            Debug.LogError("Prefab is not assigned.");
        }
    }
    private void manageTargetType()
    {
        if (StaticVariableManager.targetType == "circle")
        {
            this.gameObject.GetComponent<MeshCollider>().enabled = true;
        }
        else if (StaticVariableManager.targetType == "square")
        {
            whatToSpawnClone = targetTypes[0];
            whatToSpawnPrefab = targetTypes[0];
            spawnLocation = this.transform;
            whatToSpawnClone = Instantiate(whatToSpawnPrefab, spawnLocation.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
            this.gameObject.SetActive(false);
        }
        else if (StaticVariableManager.targetType == "diamond")
        {
            whatToSpawnClone = targetTypes[1];
            whatToSpawnPrefab = targetTypes[1];
            spawnLocation = this.transform;
            whatToSpawnClone = Instantiate(whatToSpawnPrefab, spawnLocation.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
            this.gameObject.SetActive(false);
        }
    }

}
