using UnityEngine;

public class AGM88Controller : MonoBehaviour
{
    public float initialSpeed = 10f; // Initial speed of the missile
    public float maxSpeed = 1000f; // Maximum speed of the missile
    public float acceleration = 50f; // Acceleration rate of the missile
    public float fuelAmount = 70f; // Amount of fuel in liters
    public float fuelConsumptionRate = 10f; // Fuel consumption rate per second

    private bool isLaunched = false;
    private float currentSpeed;
    private float currentFuel;
    private Vector3 targetPosition; // Target position to follow
    private ThrusterScript thruster; // Reference to the thruster
    private Gyroscope gyroscope; // Reference to the gyroscope
    private MissileLaser missileLaser; // Reference to the missile laser

    void Start()
    {
        currentSpeed = initialSpeed;
        currentFuel = fuelAmount;
        thruster = GetComponentInChildren<ThrusterScript>();
        gyroscope = GetComponentInChildren<Gyroscope>();
        missileLaser = GetComponentInChildren<MissileLaser>();
    }

    void Update()
    {
        if (isLaunched)
        {
            if (currentFuel > 0)
            {
                if (thruster.CurrentState == ThrusterScript.ThrusterState.Acceleration)
                {
                    // Initial flight up
                    InitialFlight();
                }
                else if (thruster.CurrentState == ThrusterScript.ThrusterState.Idle && gyroscope.HasReachedTargetAngle())
                {
                    // Rotate towards target
                    RotateTowardsTarget();
                }
                else if (thruster.CurrentState == ThrusterScript.ThrusterState.Idle && !gyroscope.HasReachedTargetAngle())
                {
                    // Start accelerating towards target
                    AccelerateTowardsTarget();
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

    void InitialFlight()
    {
        currentSpeed = initialSpeed;
        transform.position += transform.up * currentSpeed * Time.deltaTime;
        Invoke("StopAndRotate", 0.5f);
    }

    void StopAndRotate()
    {
        thruster.SwitchState(ThrusterScript.ThrusterState.Idle);
        gyroscope.SetTargetAngle(missileLaser.GetLockedTargetPosition());
    }

    void RotateTowardsTarget()
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * acceleration);
    }

    void AccelerateTowardsTarget()
    {
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
        currentFuel -= fuelConsumptionRate * Time.deltaTime;
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }

    public void LaunchMissile()
    {
        isLaunched = true;
        thruster.SwitchState(ThrusterScript.ThrusterState.Acceleration);
    }

    public void SetTargetHeatSource(Transform heatSource)
    {
        targetPosition = heatSource.position;
        Debug.Log("Missile aimed at heat source: " + heatSource.name);

        // Set the target in the MissileLaser script
        MissileLaser missileLaser = GetComponentInChildren<MissileLaser>();
        if (missileLaser != null)
        {
            missileLaser.SetTargetEngine(heatSource);
        }
    }
}
