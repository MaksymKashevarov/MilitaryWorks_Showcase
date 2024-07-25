using UnityEngine;

public class MissileLaser : MonoBehaviour
{
    public Transform laserOrigin; // The origin point of the laser (e.g., the nose of the missile)
    public LayerMask engineLayer; // Layer for engine objects
    public float laserRange = 1000f; // Range of the laser
    public Transform noseObject; // The nose object of the missile

    private LineRenderer noseLaserLine; // LineRenderer component on the nose object
    private Transform targetEngine; // The target engine to lock on to

    // State Machine
    private enum LaserState { Waiting, Locked }
    private LaserState currentState;

    void Start()
    {
        // Set the initial state to Waiting
        currentState = LaserState.Waiting;

        // Get the LineRenderer component from the nose object
        if (noseObject != null)
        {
            noseLaserLine = noseObject.GetComponent<LineRenderer>();
            if (noseLaserLine == null)
            {
                Debug.LogWarning("LineRenderer component is not assigned to the nose object.");
            }
        }
        else
        {
            Debug.LogWarning("Nose object is not assigned.");
        }
    }

    void Update()
    {
        if (currentState == LaserState.Locked)
        {
            CastLaserToEngine();
            DebugLaserRotationAngle();
        }
        else if (noseLaserLine != null)
        {
            noseLaserLine.enabled = false; // Disable the laser when not locked
        }
    }

    public void SetTargetEngine(Transform engine)
    {
        targetEngine = engine;
        currentState = LaserState.Locked;
        Debug.Log("Missile Laser Locked on target: " + engine.name);
    }

    void CastLaserToEngine()
    {
        if (targetEngine == null || noseLaserLine == null) return;

        // Cast a ray from the nose object to the target engine
        Vector3 directionToEngine = (targetEngine.position - noseObject.position).normalized;
        Ray ray = new Ray(noseObject.position, directionToEngine);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, laserRange, engineLayer))
        {
            if (hit.transform == targetEngine)
            {
                // Draw the laser
                noseLaserLine.enabled = true;
                noseLaserLine.SetPosition(0, noseObject.position);
                noseLaserLine.SetPosition(1, hit.point);
            }
        }
    }

    void DebugLaserRotationAngle()
    {
        Vector3 laserEulerAngles = noseObject.eulerAngles;
        Debug.Log("Nose Laser Rotation Angle: " + laserEulerAngles);
    }

    public Vector3 GetLockedTargetPosition()
    {
        if (targetEngine != null)
        {
            return targetEngine.position;
        }
        return Vector3.zero;
    }

    public bool IsFacingTarget()
    {
        if (targetEngine == null) return false;

        Vector3 directionToTarget = (targetEngine.position - noseObject.position).normalized;
        float dotProduct = Vector3.Dot(noseObject.forward, directionToTarget);

        // Check if the laser is approximately facing the target
        return dotProduct > 0.99f;
    }

    public Vector3 GetLaserAngle()
    {
        return noseObject.eulerAngles;
    }
}
