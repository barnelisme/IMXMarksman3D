using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBreaker : MonoBehaviour
{
    public int numberOfPieces = 5; // Adjust to control the number of fragments
    public float explosionForce = 5f;
    public float destroyTime = 3f;

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh = Instantiate(meshFilter.mesh); // Create a readable copy of the mesh
        }
    }

    void OnMouseDown() // Trigger when clicked
    {
        BreakObject();
    }

    public void BreakObject()
    {
        if (meshFilter == null || meshRenderer == null)
            return;

        Mesh originalMesh = meshFilter.mesh;
        Vector3[] vertices = originalMesh.vertices;
        int[] triangles = originalMesh.triangles;

        for (int i = 0; i < numberOfPieces; i++)
        {
            print("Creating fragements...");
            CreateFragment(vertices, triangles);
        }

        Destroy(gameObject); // Remove the original object
    }

    void CreateFragment(Vector3[] vertices, int[] triangles)
    {
        GameObject fragment = new GameObject("Fragment");
        fragment.transform.position = transform.position;
        fragment.transform.rotation = transform.rotation;

        Mesh newMesh = new Mesh();
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (Random.value > 0.5f) // Randomly assign triangles to fragments
            {
                int indexA = triangles[i];
                int indexB = triangles[i + 1];
                int indexC = triangles[i + 2];

                newVertices.Add(vertices[indexA]);
                newVertices.Add(vertices[indexB]);
                newVertices.Add(vertices[indexC]);

                int lastIndex = newVertices.Count - 1;
                newTriangles.Add(lastIndex - 2);
                newTriangles.Add(lastIndex - 1);
                newTriangles.Add(lastIndex);
            }
        }

        newMesh.vertices = newVertices.ToArray();
        newMesh.triangles = newTriangles.ToArray();
        newMesh.RecalculateNormals();

        fragment.AddComponent<MeshFilter>().mesh = newMesh;
        fragment.AddComponent<MeshRenderer>().material = meshRenderer.material;
        Rigidbody rb = fragment.AddComponent<Rigidbody>();

        rb.AddExplosionForce(explosionForce, transform.position, 2f);
        Destroy(fragment, destroyTime); // Remove fragments after time
    }
}

