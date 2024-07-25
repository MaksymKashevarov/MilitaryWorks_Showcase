using UnityEngine;

public class Gyroscope : MonoBehaviour
{
    public float rotationSpeed = 5f; // Speed at which the gyroscope rotates
    private Quaternion targetRotation; // Target rotation for the gyroscope
    private bool isRotating = false; // Flag to check if rotation is in progress

    private Transform parentTransform; // Parent transform to rotate

    private enum GyroState { Inactive, Rotating }
    private GyroState currentState;

    void Start()
    {
        parentTransform = transform.parent; // Get the parent transform
        currentState = GyroState.Inactive;
    }

    void Update()
    {
        if (isRotating)
        {
            // Rotate the parent towards the target rotation
            parentTransform.rotation = Quaternion.Slerp(parentTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Check if the rotation is close enough to the target rotation
            if (Quaternion.Angle(parentTransform.rotation, targetRotation) < 0.1f)
            {
                parentTransform.rotation = targetRotation;
                isRotating = false;
                Debug.Log("Gyroscope rotation complete.");

                // Notify the missile that rotation is complete
                parentTransform.GetComponent<AGM88Controller>()?.OnGyroscopeRotationComplete();
            }
        }
    }

    // Method to set the target angle and start rotation
    public void SetTargetAngle(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - parentTransform.position).normalized;
        targetRotation = Quaternion.LookRotation(direction);
        isRotating = true;
        currentState = GyroState.Rotating;
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

    // Method to receive command from missile to rotate
    public void ReceiveCommandFromMissile(Vector3 targetPosition)
    {
        SetTargetAngle(targetPosition);
    }

    // Method to start rotation based on angle data received from missile
    public void StartRotation(Vector3 angleData)
    {
        SetTargetAngle(angleData);
    }
}
