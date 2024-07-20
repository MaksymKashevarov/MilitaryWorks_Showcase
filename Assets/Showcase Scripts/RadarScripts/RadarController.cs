using UnityEngine;

public class RadarController : MonoBehaviour
{
    public float rotationSpeed = 30f; // Rotation speed in degrees per second
    public Transform rotationMechanism; // Assign the rotation mechanism (cylinder) here
    public Transform radarLaser; // Assign the laser object here
    public float detectionRange = 100f; // The range of the radar's detection
    public LayerMask detectionLayer; // Layer for detection (e.g., Aircraft layer)
    public float fieldOfViewAngle = 45f; // Field of view angle in degrees
    public int numberOfRaycasts = 10; // Number of raycasts within the field of view
    public float upwardAngle = 45f; // Upward angle for additional raycasts
    public float downwardAngle = 0f; // Downward angle for additional raycasts
    public float raySpeed = 10f; // Speed at which the rays move

    private float currentAngle;
    private bool movingDownward = true;
    private Transform target;

    // State Machine
    private enum RadarState { Search, LockOnTarget }
    private RadarState currentState;

    void Start()
    {
        // Set the initial state to Search
        currentState = RadarState.Search;
        currentAngle = upwardAngle;
    }

    void Update()
    {
        // Update the state machine
        switch (currentState)
        {
            case RadarState.Search:
                HandleSearchState();
                break;
            case RadarState.LockOnTarget:
                HandleLockOnTargetState();
                break;
        }
    }

    void HandleSearchState()
    {
        // Rotate the rotation mechanism around its local Y-axis
        if (rotationMechanism != null)
        {
            rotationMechanism.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);

            // Update the current angle for raycasts
            UpdateAngle();

            // Perform multiple raycasts within the field of view
            PerformRaycasts(radarLaser.forward); // Horizontal raycasts
            PerformRaycasts(Quaternion.Euler(currentAngle, 0, 0) * radarLaser.forward); // Upward raycasts
        }
        else
        {
            Debug.LogWarning("Rotation mechanism is not assigned.");
        }
    }

    void UpdateAngle()
    {
        float angleChange = raySpeed * Time.deltaTime;

        if (movingDownward)
        {
            currentAngle -= angleChange;
            if (currentAngle <= downwardAngle)
            {
                currentAngle = downwardAngle;
                movingDownward = false;
            }
        }
        else
        {
            currentAngle += angleChange;
            if (currentAngle >= upwardAngle)
            {
                currentAngle = upwardAngle;
                movingDownward = true;
            }
        }
    }

    void PerformRaycasts(Vector3 baseDirection)
    {
        float halfFOV = fieldOfViewAngle / 2;
        float angleStep = fieldOfViewAngle / numberOfRaycasts;

        for (int i = 0; i < numberOfRaycasts; i++)
        {
            float angle = -halfFOV + angleStep * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * baseDirection;
            RaycastHit hit;
            if (Physics.Raycast(radarLaser.position, direction, out hit, detectionRange, detectionLayer))
            {
                if (hit.transform.CompareTag("Aircraft"))
                {
                    target = hit.transform;
                    currentState = RadarState.LockOnTarget;
                    Debug.Log("Target Locked: " + target.name);
                    break;
                }
            }
        }
    }

    void HandleLockOnTargetState()
    {
        if (target != null)
        {
            // Calculate the direction to the target on the horizontal plane
            Vector3 directionToTarget = target.position - transform.position;
            directionToTarget.y = 0; // Ignore the vertical component to avoid tilting
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            rotationMechanism.rotation = Quaternion.Slerp(rotationMechanism.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // Return to search state if the target is lost
            currentState = RadarState.Search;
            Debug.Log("Target lost, returning to search state.");
        }
    }

    void OnDrawGizmos()
    {
        if (currentState == RadarState.Search)
        {
            Gizmos.color = Color.red;

            // Draw horizontal raycasts
            DrawRaycastGizmos(radarLaser.forward);

            // Draw upward raycasts
            DrawRaycastGizmos(Quaternion.Euler(currentAngle, 0, 0) * radarLaser.forward);
        }
    }

    void DrawRaycastGizmos(Vector3 baseDirection)
    {
        float halfFOV = fieldOfViewAngle / 2;
        float angleStep = fieldOfViewAngle / numberOfRaycasts;

        for (int i = 0; i < numberOfRaycasts; i++)
        {
            float angle = -halfFOV + angleStep * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * baseDirection;
            Gizmos.DrawRay(radarLaser.position, direction * detectionRange);
        }
    }
}
