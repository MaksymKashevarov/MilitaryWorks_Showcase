using UnityEngine;

public class AGM88Controller : MonoBehaviour
{
    public Transform nose; // Assign the nose object in the inspector
    public float initialSpeed = 10f; // Initial speed at which the missile flies
    public float maxSpeed = 1000f; // Maximum speed the missile tries to reach
    public float acceleration = 20f; // Acceleration rate per second
    public float fuelAmount = 70f; // Amount of fuel in liters
    public float fuelConsumptionRate = 10f; // Fuel consumption rate per second
    public float decelerationRate = 1f; // Rate at which the missile decelerates after fuel is exhausted

    private MissileSlotSensor slotSensor; // Reference to the slot sensor
    private bool isLaunched = false; // Indicates if the missile has been launched
    private bool fuelDepleted = false; // Indicates if the fuel is depleted
    private float currentSpeed; // Current speed of the missile

    void Start()
    {
        currentSpeed = initialSpeed;
        // Initially try to find and connect to any slot
        FindAndConnectToSlot();
    }

    void Update()
    {
        // Continuously check to connect to any slot if not already connected
        if (slotSensor == null)
        {
            FindAndConnectToSlot();
        }

        // Check the slot state and launch if necessary
        if (slotSensor != null && slotSensor.GetCurrentState() == "Launch" && !isLaunched)
        {
            isLaunched = true;
        }

        if (isLaunched)
        {
            Fly();
        }
    }

    void FindAndConnectToSlot()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f); // Adjust radius as needed
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Missile_Slot"))
            {
                slotSensor = collider.GetComponent<MissileSlotSensor>();
                if (slotSensor != null)
                {
                    Debug.Log("Missile connected to slot: " + collider.gameObject.name);
                    break;
                }
            }
        }
    }

    void Fly()
    {
        if (fuelAmount > 0)
        {
            // Consume fuel
            fuelAmount -= fuelConsumptionRate * Time.deltaTime;

            // Accelerate the missile until it reaches max speed
            if (currentSpeed < maxSpeed)
            {
                currentSpeed += acceleration * Time.deltaTime;
                if (currentSpeed > maxSpeed)
                {
                    currentSpeed = maxSpeed;
                }
            }
        }
        else
        {
            if (!fuelDepleted)
            {
                Debug.Log("Missile out of fuel!");
                fuelDepleted = true;
            }

            // Decelerate the missile after fuel is exhausted
            currentSpeed -= decelerationRate * Time.deltaTime;

            if (currentSpeed < 0)
            {
                currentSpeed = 0;
            }
        }

        // Move the missile in the direction of its nose
        transform.position += nose.up * currentSpeed * Time.deltaTime;

        // Rotate the missile to always follow the nose direction
        transform.rotation = Quaternion.Slerp(transform.rotation, nose.rotation, Time.deltaTime * 5);
    }
}
