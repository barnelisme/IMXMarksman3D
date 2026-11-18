using UnityEngine;

public class KeepUpright : MonoBehaviour
{
    private Transform targetObject;     // The moving object the bullet hole is sticking to
    private Vector3 initialLocalPoint;  // The position of the bullet hole relative to the target object
    private Quaternion initialRotation; // The fixed world rotation
    private Vector3 initialScale;       // <-- NEW: The prefab's desired local/world scale

    private bool isInitialized = false;

    // Call this method immediately after instantiation
    public void Initialize(Transform target, Vector3 localPoint, Vector3 scale)
    {
        targetObject = target;
        initialLocalPoint = localPoint;
        initialScale = scale; // <-- Store the correct scale
        isInitialized = true;
    }

    void Awake()
    {
        // Awake should run after Initialize, but we use this to ensure we capture the rotation
        initialRotation = transform.rotation;

        // If Initialize wasn't called, we capture the current scale as a fallback
        if (!isInitialized)
        {
            initialScale = transform.localScale;
            // Ensure it's marked as initialized if it was placed manually, not instantiated
            isInitialized = true;
        }
    }

    void LateUpdate()
    {
        // Ensure the rotation is locked
        transform.rotation = initialRotation;

        // Lock the scale to the size set on the prefab
        transform.localScale = initialScale;

        // Track Position
        if (isInitialized && targetObject != null)
        {
            // Calculate the current world position of the bullet hole
            transform.position = targetObject.TransformPoint(initialLocalPoint);
        }
    }
}