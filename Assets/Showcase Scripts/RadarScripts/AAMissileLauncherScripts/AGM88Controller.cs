using UnityEngine;

public class AGM88Controller : MonoBehaviour
{
    public Transform noseObject; // Assign the nose object of the missile here
    public float initialSpeed = 10f; // Initial speed of the missile
    public float maxSpeed = 1000f; // Maximum speed of the missile
    public float acceleration = 50f; // Acceleration rate of the missile
    public float fuelAmount = 70f; // Amount of fuel in liters
    public float fuelConsumptionRate = 10f; // Fuel consumption rate per second

    private bool isLaunched = false;
    private float currentSpeed;
    private float currentFuel;
    private Transform targetHeatSource; // Target heat source to follow
    private float launchTime; // Time when the missile was launched

    void Start()
    {
        currentSpeed = initialSpeed;
        currentFuel = fuelAmount;
    }

    void Update()
    {
        if (isLaunched)
        {
            if (Time.time - launchTime < 0.5f)
            {
                // Fly straight up for 0.5 seconds
                transform.position += Vector3.up * currentSpeed * Time.deltaTime;
            }
            else
            {
                if (currentFuel > 0)
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
    }

    public void LaunchMissile()
    {
        isLaunched = true;
        launchTime = Time.time;
    }

    public void SetTargetHeatSource(Transform heatSource)
    {
        targetHeatSource = heatSource;
        Debug.Log("Missile aimed at heat source: " + heatSource.name);
    }
}
