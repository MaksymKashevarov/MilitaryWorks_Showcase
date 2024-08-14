using UnityEngine;

public class FighterJetController : MonoBehaviour
{
    public float thrustForce = 1000f;
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
}
