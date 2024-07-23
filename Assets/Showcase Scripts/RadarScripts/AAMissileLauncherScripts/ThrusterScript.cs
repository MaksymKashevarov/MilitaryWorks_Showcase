using UnityEngine;

public class ThrusterScript : MonoBehaviour
{
    public enum ThrusterState { Idle, Acceleration }
    private ThrusterState currentState;

    public float accelerationForce = 250f; // Acceleration force applied by the thruster
    private float currentSpeed = 0f;
    private float currentAcceleration = 0f;

    void Start()
    {
        currentState = ThrusterState.Idle;
    }

    void FixedUpdate()
    {
        if (currentState == ThrusterState.Acceleration)
        {
            ApplyThrust();
        }
        else
        {
            currentAcceleration = 0f;
            currentSpeed = 0f;
        }
    }

    void ApplyThrust()
    {
        Rigidbody rb = GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            currentAcceleration = accelerationForce;
            currentSpeed = rb.velocity.magnitude;
            rb.AddForce(transform.up * accelerationForce, ForceMode.Acceleration);
        }
    }

    public void SwitchState(ThrusterState newState)
    {
        currentState = newState;
    }

    public ThrusterState CurrentState
    {
        get { return currentState; }
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public float GetCurrentAcceleration()
    {
        return currentAcceleration;
    }

    // Method to receive command from missile
    public void ReceiveCommandFromMissile(ThrusterState newState)
    {
        SwitchState(newState);
    }
}
