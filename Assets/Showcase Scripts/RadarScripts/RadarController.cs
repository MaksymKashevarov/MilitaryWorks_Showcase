using UnityEngine;

public class RadarController : MonoBehaviour
{
    public float rotationSpeed = 30f; // Rotation speed in degrees per second
    public Transform rotationMechanism; // Assign the rotation mechanism (cylinder) here
    public Transform radarLaser; // Assign the laser object here
    public float detectionRange = 100f; // The range of the radar's detection
    public LayerMask detectionLayer; // Layer for detection (e.g., Aircraft layer)
    public LayerMask obstructionLayer; // Layer for obstructions (e.g., walls, terrain)
    public float fieldOfViewAngle = 45f; // Field of view angle in degrees
    public int numberOfRaycasts = 10; // Number of raycasts within the field of view
    public float upwardAngle = 45f; // Upward angle for additional raycasts
    public float downwardAngle = 0f; // Downward angle for additional raycasts
    public float raySpeed = 10f; // Speed at which the rays move
    public float lockOnDuration = 4f; // Duration to follow target after losing sight
    public bool showRaysInEditor = true; // Toggle to show/hide rays in editor
    public bool showRaysInGame = true; // Toggle to show/hide rays in game

    private float currentUpwardAngle;
    private float currentDownwardAngle;
    private float currentIndependentAngle;
    private bool movingUpward = true;
    private bool movingDownward = true;
    private bool movingUp = true;
    private Transform target;
    private float targetLostTime;

    // State Machine
    public enum RadarState { Search, LockOnTarget }
    private RadarState currentState;

    public RadarState CurrentState
    {
        get { return currentState; }
    }

    void Start()
    {
        // Set the initial state to Search
        currentState = RadarState.Search;
        currentUpwardAngle = upwardAngle;
        currentDownwardAngle = downwardAngle;
        currentIndependentAngle = -45f; // Start the independent rays from -45 degrees
        targetLostTime = 0f;
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

            // Update the current angles for raycasts
            UpdateUpwardAngle();
            UpdateDownwardAngle();
            UpdateIndependentAngle();

            // Perform multiple raycasts within the field of view
            PerformRaycasts(radarLaser.forward, numberOfRaycasts); // Horizontal raycasts
            PerformRaycasts(Quaternion.Euler(currentUpwardAngle, 0, 0) * radarLaser.forward, numberOfRaycasts); // Upward raycasts
            PerformRaycasts(Quaternion.Euler(currentDownwardAngle, 0, 0) * radarLaser.forward, numberOfRaycasts); // Downward to upward raycasts
            PerformRaycasts(Quaternion.Euler(currentIndependentAngle, 0, 0) * radarLaser.forward, numberOfRaycasts); // Independent rays
        }
        else
        {
            Debug.LogWarning("Rotation mechanism is not assigned.");
        }
    }

    void UpdateUpwardAngle()
    {
        float angleChange = raySpeed * Time.deltaTime;

        if (movingUpward)
        {
            currentUpwardAngle += angleChange;
            if (currentUpwardAngle >= upwardAngle)
            {
                currentUpwardAngle = upwardAngle;
                movingUpward = false;
            }
        }
        else
        {
            currentUpwardAngle -= angleChange;
            if (currentUpwardAngle <= downwardAngle)
            {
                currentUpwardAngle = downwardAngle;
                movingUpward = true;
            }
        }
    }

    void UpdateDownwardAngle()
    {
        float angleChange = raySpeed * Time.deltaTime;

        if (movingDownward)
        {
            currentDownwardAngle += angleChange;
            if (currentDownwardAngle >= upwardAngle)
            {
                currentDownwardAngle = upwardAngle;
                movingDownward = false;
            }
        }
        else
        {
            currentDownwardAngle -= angleChange;
            if (currentDownwardAngle <= downwardAngle)
            {
                currentDownwardAngle = downwardAngle;
                movingDownward = true;
            }
        }
    }

    void UpdateIndependentAngle()
    {
        float angleChange = raySpeed * Time.deltaTime;

        if (movingUp)
        {
            currentIndependentAngle += angleChange;
            if (currentIndependentAngle >= 45f)
            {
                currentIndependentAngle = 45f;
                movingUp = false;
            }
        }
        else
        {
            currentIndependentAngle -= angleChange;
            if (currentIndependentAngle <= -45f)
            {
                currentIndependentAngle = -45f;
                movingUp = true;
            }
        }
    }

    void PerformRaycasts(Vector3 baseDirection, int numberOfRays)
    {
        float halfFOV = fieldOfViewAngle / 2;
        float angleStep = fieldOfViewAngle / numberOfRays;

        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = -halfFOV + angleStep * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * baseDirection;
            RaycastHit hit;
            if (Physics.Raycast(radarLaser.position, direction, out hit, detectionRange, detectionLayer | obstructionLayer))
            {
                if (hit.transform.CompareTag("Aircraft"))
                {
                    if (!Physics.Linecast(radarLaser.position, hit.point, obstructionLayer))
                    {
                        target = hit.transform;
                        currentState = RadarState.LockOnTarget;
                        Debug.Log("Target Locked: " + target.name);
                        targetLostTime = 0f; // Reset the target lost timer
                        break;
                    }
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

            // Perform raycast to check if the target is still in sight
            if (!IsTargetInSight())
            {
                targetLostTime += Time.deltaTime;
                if (targetLostTime >= lockOnDuration)
                {
                    // Return to search state if target is not found within the lock-on duration
                    currentState = RadarState.Search;
                    Debug.Log("Target lost for too long, returning to search state.");
                    target = null;
                }
            }
            else
            {
                targetLostTime = 0f; // Reset the target lost timer if the target is still in sight
            }
        }
        else
        {
            // Return to search state if the target is lost
            currentState = RadarState.Search;
            Debug.Log("Target lost, returning to search state.");
        }
    }

    bool IsTargetInSight()
    {
        Vector3 directionToTarget = target.position - radarLaser.position;
        RaycastHit hit;
        if (Physics.Raycast(radarLaser.position, directionToTarget, out hit, detectionRange, detectionLayer | obstructionLayer))
        {
            if (hit.transform == target)
            {
                return !Physics.Linecast(radarLaser.position, hit.point, obstructionLayer);
            }
        }
        return false;
    }

    void OnDrawGizmos()
    {
        if (showRaysInEditor && !Application.isPlaying)
        {
            Gizmos.color = Color.red;

            // Draw horizontal raycasts
            DrawRaycastGizmos(radarLaser.forward, numberOfRaycasts);

            // Draw upward raycasts
            DrawRaycastGizmos(Quaternion.Euler(currentUpwardAngle, 0, 0) * radarLaser.forward, numberOfRaycasts);

            // Draw downward to upward raycasts
            DrawRaycastGizmos(Quaternion.Euler(currentDownwardAngle, 0, 0) * radarLaser.forward, numberOfRaycasts);

            // Draw independent rays
            DrawRaycastGizmos(Quaternion.Euler(currentIndependentAngle, 0, 0) * radarLaser.forward, numberOfRaycasts);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (showRaysInGame && Application.isPlaying)
        {
            Gizmos.color = Color.green;

            // Draw horizontal raycasts
            DrawRaycastGizmos(radarLaser.forward, numberOfRaycasts);

            // Draw upward raycasts
            DrawRaycastGizmos(Quaternion.Euler(currentUpwardAngle, 0, 0) * radarLaser.forward, numberOfRaycasts);

            // Draw downward to upward raycasts
            DrawRaycastGizmos(Quaternion.Euler(currentDownwardAngle, 0, 0) * radarLaser.forward, numberOfRaycasts);

            // Draw independent rays
            DrawRaycastGizmos(Quaternion.Euler(currentIndependentAngle, 0, 0) * radarLaser.forward, numberOfRaycasts);
        }
    }

    void DrawRaycastGizmos(Vector3 baseDirection, int numberOfRays)
    {
        float halfFOV = fieldOfViewAngle / 2;
        float angleStep = fieldOfViewAngle / numberOfRays;

        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = -halfFOV + angleStep * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * baseDirection;
            Gizmos.DrawRay(radarLaser.position, direction * detectionRange);
        }
    }

    public Transform GetLockedTarget()
    {
        return target;
    }
}




