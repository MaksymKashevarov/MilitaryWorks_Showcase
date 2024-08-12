using UnityEngine;

public class FighterJetController : MonoBehaviour
{
    public float thrustForce = 1000f;
    public Transform[] wheels; // Array to hold the wheels of the aircraft
    public float wheelRadius = 0.5f; // Radius of the wheels
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        ApplyThrust();
        ApplyLift();
        ApplyDrag();
        RotateWheels();
        MoveWithWheels();
        HandleTerrainAngle();
    }

    void ApplyThrust()
    {
        rb.AddForce(transform.forward * thrustForce);
    }

    void ApplyLift()
    {
        float velocity = rb.velocity.magnitude;
        float lift = 0.5f * GlobalEnvironment.GetAirDensity() * velocity * velocity * GlobalEnvironment.GetLiftCoefficient();
        rb.AddForce(Vector3.up * lift);
    }

    void ApplyDrag()
    {
        float velocity = rb.velocity.magnitude;
        float drag = 0.5f * GlobalEnvironment.GetAirDensity() * velocity * velocity * GlobalEnvironment.GetDragCoefficient();
        rb.AddForce(-rb.velocity.normalized * drag);
    }

    void RotateWheels()
    {
        float wheelCircumference = 2 * Mathf.PI * wheelRadius;
        float rotationAngle = (rb.velocity.magnitude / wheelCircumference) * 360 * Time.deltaTime;

        foreach (Transform wheel in wheels)
        {
            wheel.Rotate(Vector3.right, rotationAngle);
        }
    }

    void MoveWithWheels()
    {
        Vector3 forwardMovement = transform.forward * thrustForce * Time.deltaTime;
        rb.MovePosition(rb.position + forwardMovement);
    }

    void HandleTerrainAngle()
    {
        // Get the ground normal
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, wheelRadius + 0.1f))
        {
            Vector3 groundNormal = hit.normal;

            // Align the aircraft with the ground normal
            Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, groundNormal);
            rb.MoveRotation(slopeRotation * rb.rotation);

            // Apply sliding force down the slope
            Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, groundNormal).normalized;
            rb.AddForce(slideDirection * GlobalEnvironment.GetGravity() * rb.mass);
        }
    }
}
