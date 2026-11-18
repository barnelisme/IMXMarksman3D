using UnityEngine;
using System.Collections;

public class RotateTarget : MonoBehaviour
{
    private Quaternion originalLocalRotation;
    public bool isRotationComplete = false;
    private float rotationSpeed = 180f * 4; // degrees per second

    private Coroutine currentRotationCoroutine;

    void Start()
    {
        // Save original local rotation
        originalLocalRotation = transform.localRotation;
    }

    public void Rotate()
    {
        if (currentRotationCoroutine != null)
            StopCoroutine(currentRotationCoroutine);

        currentRotationCoroutine = StartCoroutine(RotateToXMinus90());
    }

    public void ResetRotation()
    {
        if (currentRotationCoroutine != null)
            StopCoroutine(currentRotationCoroutine);

        currentRotationCoroutine = StartCoroutine(RotateToOriginal());
    }

    private IEnumerator RotateToXMinus90()
    {
        isRotationComplete = false;

        Vector3 currentEuler = transform.localEulerAngles;
        float targetX = -90f;

        // Adjust 360 wrap if necessary
        if (currentEuler.x > 180f) currentEuler.x -= 360f;

        Quaternion targetRotation = Quaternion.Euler(targetX, currentEuler.y, currentEuler.z);

        while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.1f)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        transform.localRotation = targetRotation;
        switch(this.transform.name)
        {
            case "Base 1":
                StaticVariableManager.isPopperBoard1Set = true;
                break;
            case "Base 2":
                StaticVariableManager.isPopperBoard2Set = true;

                break;
            case "Base 3":
                StaticVariableManager.isPopperBoard3Set = true;

                break;
        }
    }

    private IEnumerator RotateToOriginal()
    {
        isRotationComplete = false;

        while (Quaternion.Angle(transform.localRotation, originalLocalRotation) > 0.1f)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, originalLocalRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        transform.localRotation = originalLocalRotation;
        isRotationComplete = true;
    }
}
