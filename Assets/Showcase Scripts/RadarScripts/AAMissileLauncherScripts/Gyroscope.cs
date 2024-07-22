using UnityEngine;

public class Gyroscope : MonoBehaviour
{
    public float rotationSpeed = 5f; // Speed at which the gyroscope rotates
    private Quaternion targetRotation; // Target rotation for the gyroscope
    private bool isRotating = false; // Flag to check if rotation is in progress

    void Update()
    {
        if (isRotating)
        {
            // Rotate towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Check if the rotation is close enough to the target rotation
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
                Debug.Log("Gyroscope rotation complete.");
            }
        }
    }

    // Method to set the target angle and start rotation
    public void SetTargetAngle(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        targetRotation = Quaternion.LookRotation(direction);
        isRotating = true;
        Debug.Log("Gyroscope set to rotate towards target angle.");
    }

    // Method to check if the rotation is complete
    public bool HasReachedTargetAngle()
    {
        return !isRotating;
    }

    // Method to reset the gyroscope to its default angle
    public void ResetToDefaultAngle()
    {
        targetRotation = Quaternion.Euler(0, 0, 0);
        isRotating = true;
        Debug.Log("Gyroscope reset to default angle.");
    }
}
