using UnityEngine;

public class ThrusterScript : MonoBehaviour
{
    public enum ThrusterState { Idle, Acceleration }
    private ThrusterState currentState;

    public float accelerationForce = 250f; // Acceleration force applied by the thruster

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
    }

    void ApplyThrust()
    {
        Rigidbody rb = GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
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
}
