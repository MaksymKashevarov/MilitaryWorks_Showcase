using UnityEngine;

public class AGM88Controller : MonoBehaviour
{
    public Transform noseObject; // Assign the nose object of the missile here
    public float initialSpeed = 10f; // Initial speed of the missile
    public float maxSpeed = 1000f; // Maximum speed of the missile
    public float acceleration = 50f; // Acceleration rate of the missile
    public float fuelAmount = 70f; // Amount of fuel in liters
    public float fuelConsumptionRate = 10f; // Fuel consumption rate per second
    public float initialFlightDuration = 0.5f; // Duration of initial upward flight
    public LineRenderer rayLine; // Line renderer for the targeting ray

    private bool isLaunched = false;
    private float currentSpeed;
    private float currentFuel;
    private float launchTime;
    private Transform targetHeatSource; // Target heat source to follow
    private bool isAligned = false; // To check if missile is aligned with the target
    private bool rayDebugged = false; // To ensure the ray is debugged only once

    void Start()
    {
        currentSpeed = initialSpeed;
        currentFuel = fuelAmount;
        if (rayLine != null)
        {
            rayLine.enabled = false;
        }
    }

    void Update()
    {
        if (isLaunched)
        {
            HandleLaunch();
        }
        else if (targetHeatSource != null)
        {
            HandleTargetingRay();
        }
    }

    public void LaunchMissile()
    {
        isLaunched = true;
        launchTime = Time.time;
        if (rayLine != null)
        {
            rayLine.enabled = false; // Disable the ray when the missile is launched
        }
    }

    public void SetTargetHeatSource(Transform heatSource)
    {
        targetHeatSource = heatSource;
        Debug.Log("Missile aimed at heat source: " + heatSource.name);
        if (rayLine != null)
        {
            rayLine.enabled = true; // Enable the ray for targeting
        }
    }

    private void HandleLaunch()
    {
        if (Time.time - launchTime < initialFlightDuration)
        {
            // Initial upward flight
            transform.position += Vector3.up * currentSpeed * Time.deltaTime;
        }
        else
        {
            if (currentFuel > 0)
            {
                if (!isAligned)
                {
                    AlignWithTarget();
                }
                else
                {
                    // Accelerate the missile
                    currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
                    currentFuel -= fuelConsumptionRate * Time.deltaTime;

                    // Move the missile towards the heat source
                    if (targetHeatSource != null)
                    {
                        Vector3 directionToHeatSource = (targetHeatSource.position - transform.position).normalized;
                        Quaternion targetRotation = Quaternion.LookRotation(directionToHeatSource);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * acceleration);
                        transform.position += transform.forward * currentSpeed * Time.deltaTime;
                    }
                }
            }
            else
            {
                Debug.Log("Missile out of fuel.");
                // Gradually slow down the missile and eventually fall
                currentSpeed = Mathf.Max(currentSpeed - acceleration * Time.deltaTime, 0);
                if (currentSpeed <= 0)
                {
                    // Optional: Add behavior for when the missile has fully stopped
                }
            }
        }
    }

    private void HandleTargetingRay()
    {
        if (rayLine != null && targetHeatSource != null)
        {
            rayLine.SetPosition(0, noseObject.position);
            rayLine.SetPosition(1, targetHeatSource.position);

            Vector3 directionToHeatSource = (targetHeatSource.position - noseObject.position).normalized;
            Ray ray = new Ray(noseObject.position, directionToHeatSource);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == targetHeatSource && !rayDebugged)
                {
                    Debug.Log("Ray is aligned with the target heat source.");
                    rayDebugged = true;
                }
            }
        }
    }

    private void AlignWithTarget()
    {
        if (targetHeatSource != null)
        {
            Vector3 directionToHeatSource = (targetHeatSource.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToHeatSource);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);

            // Check if aligned
            Vector3 noseDirection = noseObject.forward;
            Vector3 toTarget = (targetHeatSource.position - noseObject.position).normalized;
            if (Vector3.Dot(noseDirection, toTarget) > 0.99f) // Adjust threshold as needed
            {
                isAligned = true;
                Debug.Log("Missile is aligned with the target heat source.");
            }
        }
    }
}
