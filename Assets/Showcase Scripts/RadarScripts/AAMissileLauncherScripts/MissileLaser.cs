using UnityEngine;

public class MissileLaser : MonoBehaviour
{
    public Transform laserOrigin; // The origin point of the laser (e.g., the nose of the missile)
    public LayerMask engineLayer; // Layer for engine objects
    public float laserRange = 1000f; // Range of the laser
    public LineRenderer laserLine; // LineRenderer component to visualize the laser

    private Transform targetEngine; // The target engine to lock on to

    // State Machine
    private enum LaserState { Waiting, Locked }
    private LaserState currentState;

    void Start()
    {
        // Set the initial state to Waiting
        currentState = LaserState.Waiting;

        if (laserLine == null)
        {
            Debug.LogWarning("LineRenderer component is not assigned.");
        }
    }

    void Update()
    {
        if (currentState == LaserState.Locked)
        {
            CastLaserToEngine();
        }
        else if (laserLine != null)
        {
            laserLine.enabled = false; // Disable the laser when not locked
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
        if (targetEngine == null) return;

        // Cast a ray from the laser origin to the target engine
        Vector3 directionToEngine = (targetEngine.position - laserOrigin.position).normalized;
        Ray ray = new Ray(laserOrigin.position, directionToEngine);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, laserRange, engineLayer))
        {
            if (hit.transform == targetEngine)
            {
                // Draw the laser
                if (laserLine != null)
                {
                    laserLine.enabled = true;
                    laserLine.SetPosition(0, laserOrigin.position);
                    laserLine.SetPosition(1, hit.point);
                }
            }
        }
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

        Vector3 directionToTarget = (targetEngine.position - laserOrigin.position).normalized;
        float dotProduct = Vector3.Dot(laserOrigin.forward, directionToTarget);

        // Check if the laser is approximately facing the target
        return dotProduct > 0.99f;
    }

    public void ReceiveCommandFromMissile(Transform targetPosition)
    {
        SetTargetEngine(targetPosition);
    }
}
