using UnityEngine;

public class IPSCSliderManager : MonoBehaviour
{
    private float rotationSpeed = 25f; // degrees per second
    private float currentZ = 0f;
    private float targetZ = 0f;
    private bool rotatingForward = true;
    public bool isPlateHit = false;

    void Update()
    {
        if(this.transform.name.Contains("Pendulum"))
        {

            // Trigger new 7° rotation to the left when hit
            if (isPlateHit)
            {
                targetZ -= 7f;           // move 7 degrees left
                isPlateHit = false;      // reset trigger
            }

            // Rotate towards targetZ smoothly with slowdown
            if (currentZ > targetZ)
            {
                float remaining = currentZ - targetZ;

                // Slow down when getting close (remaining < 2°)
                float speedFactor = Mathf.Clamp01(remaining / 2f); // 1 → full speed, 0 → stop
                float deltaRotation = rotationSpeed * speedFactor * Time.deltaTime;

                currentZ -= deltaRotation;

                if (currentZ < targetZ)
                {
                    deltaRotation -= (targetZ - currentZ); // clamp overshoot
                    currentZ = targetZ;
                }

                transform.Rotate(0, 0, -deltaRotation); // rotate left
            }
        }
        else
        {
            if (!StaticVariableManager.isTrainingPause && countDownStart.start_training)
            {
                float deltaRotation = rotationSpeed * Time.deltaTime;
                if (isPlateHit && rotationSpeed >= 0) rotationSpeed -= Time.deltaTime * 5;

                if (rotatingForward)
                {
                    currentZ += deltaRotation;
                    float limit = 90f;

                    if (currentZ >= limit)
                    {
                        deltaRotation -= (currentZ - limit); // limit to exact 120
                        currentZ = limit;
                        rotatingForward = false;
                    }
                }
                else
                {
                    currentZ -= deltaRotation;

                    if (currentZ <= 0f)
                    {
                        deltaRotation -= (0f - currentZ); // limit to exact 0
                        currentZ = 0f;
                        rotatingForward = true;
                    }

                    deltaRotation = -deltaRotation;
                }

                transform.Rotate(0, 0, deltaRotation);
            }
        }

    }
}
