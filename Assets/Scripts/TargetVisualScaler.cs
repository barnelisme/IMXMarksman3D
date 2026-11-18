using UnityEngine;

public class TargetVisualScaler : MonoBehaviour
{
    [Tooltip("Optional. If empty, first child with Renderer will be used.")]
    public Transform visual;

    private Vector3 originalScale;
    private bool initialized;

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        Initialize();

        /*if (TargetScaleUIManager.Instance != null)
        {
            TargetScaleUIManager.Instance.ApplyScaleToTarget(this);
        }*/
    }

    private void Initialize()
    {
        if (initialized) return;

        if (visual == null)
        {
            Renderer r = GetComponentInChildren<Renderer>(true);
            if (r != null)
                visual = r.transform;
        }

        if (visual == null)
        {
            Debug.LogError($"{name}: No visual renderer found!");
            return;
        }

        originalScale = visual.localScale;
        initialized = true;
    }

    public void ApplyScale(float multiplier)
    {
        if (!initialized) return;
        visual.localScale = originalScale * multiplier;
    }
}
