using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTypeManager : MonoBehaviour
{
    [Header("Target Type Variables")]
    public List<GameObject> targetTypes = new List<GameObject>();
    public Transform spawnLocation;

    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        bool changeMaterial = false;
        // StaticVariableManager.isMaterialUpdated = false;

        switch (StaticVariableManager.targetType)
        {
            case "circle": prefab = targetTypes[0]; break;
            case "square": prefab = targetTypes[1]; break;
            case "rhombus": prefab = targetTypes[2]; break;
            case "triangle": prefab = targetTypes[3]; break;
            case "pentagon": prefab = targetTypes[4]; break;
            case "octagon": prefab = targetTypes[5]; break;

            // Targets that need material replacement
            case "squirrel": prefab = targetTypes[6]; changeMaterial = true; break;
            case "rabbit": prefab = targetTypes[7]; changeMaterial = true; break;
            case "bird": prefab = targetTypes[8]; changeMaterial = true; break;
            case "clown": prefab = targetTypes[9]; changeMaterial = true; break;
            case "laughing clown": prefab = targetTypes[10]; changeMaterial = true; break;
            case "monster pumpkin": prefab = targetTypes[11]; changeMaterial = true; break;
            case "pumpkin": prefab = targetTypes[12]; changeMaterial = true; break;
            case "race car": prefab = targetTypes[13]; changeMaterial = true; break;
            case "spaceship": prefab = targetTypes[14]; changeMaterial = true; break;
            case "twin zombies": prefab = targetTypes[15]; changeMaterial = true; break;
            case "zombie candy": prefab = targetTypes[16]; changeMaterial = true; break;
        }

        if (prefab == null)
        {
            Debug.LogError("Prefab is not assigned. Check targetTypes list and StaticVariableManager.targetType");
            return;
        }

        setTargetType(changeMaterial);
    }

    // Update is called once per frame
    void Update() { }

    private void setTargetType(bool changeMaterial)
    {
        if (prefab != null)
        {
            // Get the MeshFilter component from the prefab
            MeshFilter prefabMeshFilter = prefab.GetComponentInChildren<MeshFilter>();
            MeshRenderer prefabRenderer = prefab.GetComponentInChildren<MeshRenderer>();

            if (prefabMeshFilter != null)
            {
                // Get the Mesh from the prefab's MeshFilter
                Mesh prefabMesh = prefabMeshFilter.sharedMesh;

                // Replace the mesh of this GameObject
                MeshFilter currentMeshFilter = GetComponent<MeshFilter>();
                MeshRenderer currentRenderer = GetComponent<MeshRenderer>();

                if (currentMeshFilter != null)
                {
                    currentMeshFilter.mesh = prefabMesh;

                    // Take transform setting before mesh changes
                    float newXScalePoints = (32.52f - this.transform.localScale.x);
                    float newYScalePoints = (30.52f - this.transform.localScale.x);
                    this.transform.localScale = prefab.transform.localScale;

                    // Calculate and apply new transform setting
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

                    // ✅ Apply materials if needed
                    if (changeMaterial && prefabRenderer != null && currentRenderer != null)
                    {
                        currentRenderer.sharedMaterials = prefabRenderer.sharedMaterials;

                        // ✅ Set color to RGB 150,150,150 for all materials
                        foreach (var mat in currentRenderer.materials)
                        {
                            if (mat != null && mat.HasProperty("_Color"))
                                mat.color = new Color(150f / 255f, 150f / 255f, 150f / 255f);
                        }

                        StaticVariableManager.isMaterialUpdated = true;
                    }
                    else
                    {
                        StaticVariableManager.isMaterialUpdated = false;
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
}