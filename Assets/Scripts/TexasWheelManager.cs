using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexasWheelManager : MonoBehaviour
{
    public float rotationSpeed = 0f;
    public float rotationAcceleration = 30f;
    public float maxRotationSpeed = 100f;
    public float deceleration = 10f;

    private Dictionary<string, Vector2> plateDirections;
    private List<string> activePlates = new List<string>();

    private float currentVelocity = 0f;

    void Start()
    {
        // Add all 5 plates
        if(this.transform.name.Contains("Pendulum"))
        {
            activePlates.Add("BottomLeft");
            activePlates.Add("BottomRight");
            activePlates.Add("Top");

            // Direction map: where each plate affects torque
            plateDirections = new Dictionary<string, Vector2>
            {
                { "BottomLeft",  new Vector2(-1, -1) },
                { "BottomRight", new Vector2(1, -1) },
                { "Top",      new Vector2(0, 0) },
            };
        }
        else
        {
            activePlates.Add("TopLeft");
            activePlates.Add("TopRight");
            activePlates.Add("BottomLeft");
            activePlates.Add("BottomRight");
            activePlates.Add("Bottom");

            // Direction map: where each plate affects torque
            plateDirections = new Dictionary<string, Vector2>
            {
                { "TopLeft",     new Vector2(-1, 1) },
                { "TopRight",    new Vector2(1, 1) },
                { "BottomLeft",  new Vector2(-1, -1) },
                { "BottomRight", new Vector2(1, -1) },
                { "Bottom",      new Vector2(0, -1) },
            };
        }

    }

    void Update()
    {
        if (!StaticVariableManager.isTrainingPause && Mathf.Abs(rotationSpeed) > 0.01f)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            // Gradually slow down
            float decel = Mathf.Sign(rotationSpeed) * deceleration * Time.deltaTime / 2;
            if (Mathf.Abs(rotationSpeed) < Mathf.Abs(decel))
                rotationSpeed = 0;
            else
                rotationSpeed -= decel;
        }
    }

    public void OnPlateHit(string plateName)
    {
        if (!activePlates.Contains(plateName)) return;

        activePlates.Remove(plateName);

        Vector2 dir;
        if (plateDirections.TryGetValue(plateName, out dir))
        {
            float torqueDirection = -dir.x; // ✅ Invert direction

            // Apply torque boost depending on remaining side's weight
            rotationSpeed += torqueDirection * rotationAcceleration;

            // Clamp speed
            rotationSpeed = Mathf.Clamp(rotationSpeed, -maxRotationSpeed, maxRotationSpeed);
        }
    }


}
