using UnityEngine;

public class MissileController : MonoBehaviour
{
    public Transform rightFlap; // Assign the right flap object
    public Transform leftFlap; // Assign the left flap object
    public float flapRotationSpeed = 36f; // Speed at which flaps rotate
    public float flapMaxAngle = 36f; // Maximum rotation angle of the flaps

    public float initialSpeed = 50f; // Initial speed of the missile
    public float maxSpeed = 500f; // Maximum speed of the missile
    public float acceleration = 100f; // Acceleration rate of the missile

    public Transform leadingAxis; // Leading axis for the missile flight
    public Transform thruster; // Assign the thruster object

    private Rigidbody rb;
    private float currentSpeed;
    private float currentFlapAngle = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = initialSpeed;
    }

    void Update()
    {
        HandleFlapControl();
        UpdateFlapRotation();

        if (Input.GetKey(KeyCode.Space))
        {
            ActivateThruster();
        }
    }

    void FixedUpdate()
    {
        HandleMissileMovement();
    }

    void HandleFlapControl()
    {
        if (Input.GetKey(KeyCode.W))
        {
            currentFlapAngle = Mathf.Clamp(currentFlapAngle - flapRotationSpeed * Time.deltaTime, -flapMaxAngle, flapMaxAngle);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            currentFlapAngle = Mathf.Clamp(currentFlapAngle + flapRotationSpeed * Time.deltaTime, -flapMaxAngle, flapMaxAngle);
        }
        else
        {
            currentFlapAngle = Mathf.Lerp(currentFlapAngle, 0, Time.deltaTime * flapRotationSpeed);
        }
    }

    void UpdateFlapRotation()
    {
        if (rightFlap != null && leftFlap != null)
        {
            rightFlap.localRotation = Quaternion.Euler(currentFlapAngle, 0, 0);
            leftFlap.localRotation = Quaternion.Euler(-currentFlapAngle, 0, 0);
        }
    }

    void HandleMissileMovement()
    {
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
        }

        Vector3 forceDirection = leadingAxis.forward;
        rb.AddForce(forceDirection * currentSpeed * Time.deltaTime, ForceMode.Acceleration);

        Vector3 torqueDirection = leadingAxis.right * currentFlapAngle; // Changed to use the right vector for torque
        rb.AddTorque(torqueDirection * flapRotationSpeed * Time.deltaTime, ForceMode.Force);
    }

    void ActivateThruster()
    {
        if (thruster != null)
        {
            // Calculate the thrust force based on the leading axis and apply it along the Z axis
            Vector3 thrustForce = leadingAxis.forward * acceleration;
            rb.AddForce(thrustForce, ForceMode.Acceleration);

            // You can add a visual or sound effect to represent the thruster activation here
        }
    }
}
