using UnityEngine;

public class AGM88Controller : MonoBehaviour
{
    public float fuelAmount = 70f; // Amount of fuel in liters
    public float fuelConsumptionRate = 10f; // Fuel consumption rate per second
    public LineRenderer trajectoryLine; // LineRenderer for visualizing the trajectory

    private bool isLaunched = false;
    private float currentFuel;
    private Vector3 targetPosition; // Target position to follow
    private ThrusterScript thruster; // Reference to the thruster
    private Gyroscope gyroscope; // Reference to the gyroscope
    private MissileLaser missileLaser; // Reference to the missile laser

    private float initialFlightDuration = 1.0f; // Duration for the initial flight phase

    // State Machine
    private enum MissileState { Idle, InitialFlight, Aiming, Homing }
    private MissileState currentState;

    void Start()
    {
        currentFuel = fuelAmount;
        thruster = GetComponentInChildren<ThrusterScript>();
        gyroscope = GetComponentInChildren<Gyroscope>();
        missileLaser = GetComponentInChildren<MissileLaser>();
        currentState = MissileState.Idle;

        if (trajectoryLine == null)
        {
            trajectoryLine = gameObject.AddComponent<LineRenderer>();
            trajectoryLine.startWidth = 0.1f;
            trajectoryLine.endWidth = 0.1f;
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case MissileState.Idle:
                // Do nothing
                break;

            case MissileState.InitialFlight:
                HandleInitialFlight();
                break;

            case MissileState.Aiming:
                HandleAiming();
                break;

            case MissileState.Homing:
                HandleHoming();
                break;
        }

        UpdateTrajectory();
    }

    void HandleInitialFlight()
    {
        if (currentFuel > 0)
        {
            // The missile flies up
            transform.position += transform.up * thruster.GetCurrentSpeed() * Time.deltaTime;
            gyroscope.SetTargetAngle(targetPosition); // Ensure gyroscope starts updating
            Invoke("SwitchToAimingState", initialFlightDuration);
        }
        else
        {
            HandleOutOfFuel();
        }
    }

    void HandleAiming()
    {
        if (gyroscope.HasReachedTargetAngle() && missileLaser.IsFacingTarget())
        {
            currentState = MissileState.Homing;
            Debug.Log("Missile state changed to Homing");
            thruster.SwitchState(ThrusterScript.ThrusterState.Acceleration);
        }
        else
        {
            gyroscope.SetTargetAngle(missileLaser.GetLockedTargetPosition());
        }
    }

    void HandleHoming()
    {
        if (currentFuel > 0)
        {
            currentFuel -= fuelConsumptionRate * Time.deltaTime;

            Vector3 directionToTarget = (targetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * thruster.GetCurrentAcceleration());
            transform.position += transform.forward * thruster.GetCurrentSpeed() * Time.deltaTime;

            // Continuously update gyroscope to ensure it's facing the target
            gyroscope.SetTargetAngle(targetPosition);
        }
        else
        {
            HandleOutOfFuel();
        }
    }

    void HandleOutOfFuel()
    {
        Debug.Log("Missile out of fuel.");
        thruster.SwitchState(ThrusterScript.ThrusterState.Idle);
    }

    void SwitchToAimingState()
    {
        thruster.SwitchState(ThrusterScript.ThrusterState.Idle);
        currentState = MissileState.Aiming;
        Debug.Log("Missile state changed to Aiming");
    }

    public void LaunchMissile()
    {
        if (currentState == MissileState.Idle)
        {
            currentState = MissileState.InitialFlight;
            thruster.SwitchState(ThrusterScript.ThrusterState.Acceleration);
            Debug.Log("Missile state changed to InitialFlight");
        }
    }

    public void SetTargetHeatSource(Transform heatSource)
    {
        targetPosition = heatSource.position;
        Debug.Log("Missile aimed at heat source: " + heatSource.name);

        // Set the target in the MissileLaser script
        if (missileLaser != null)
        {
            missileLaser.SetTargetEngine(heatSource);
        }
    }

    public void ReceiveLaunchCommand()
    {
        if (!isLaunched)
        {
            LaunchMissile();
        }
    }

    public void OnGyroscopeRotationComplete()
    {
        if (currentState == MissileState.Aiming)
        {
            Debug.Log("Gyroscope rotation complete, missile can proceed to Homing state");
            currentState = MissileState.Homing;
            thruster.SwitchState(ThrusterScript.ThrusterState.Acceleration);
        }
    }

    private void UpdateTrajectory()
    {
        trajectoryLine.positionCount = 3;
        trajectoryLine.SetPosition(0, transform.position);

        if (currentState == MissileState.InitialFlight)
        {
            Vector3 initialFlightEnd = transform.position + transform.up * initialFlightDuration * thruster.GetCurrentSpeed();
            trajectoryLine.SetPosition(1, initialFlightEnd);
            trajectoryLine.SetPosition(2, targetPosition);
        }
        else if (currentState == MissileState.Aiming || currentState == MissileState.Homing)
        {
            trajectoryLine.SetPosition(1, transform.position);
            trajectoryLine.SetPosition(2, targetPosition);
        }
    }
}
