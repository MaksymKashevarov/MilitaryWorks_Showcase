using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    // Define the states
    private enum HelicopterState { Idle, Flying }
    [SerializeField] private HelicopterState currentState;

    // Rotor references
    public Transform mainRotor;
    public Transform backRotor;

    // Rotor speed parameters
    public float maxRotorSpeed = 1000f; // Maximum rotor speed
    public float rotorAcceleration = 500f; // Rotor acceleration rate

    private float currentRotorSpeed = 0f; // Current rotor speed
    private bool hasReachedMaxSpeed = false; // Flag to check if max speed is reached
    private bool hasStoppedSpinning = true; // Flag to check if rotor has stopped

    void Start()
    {
        // Initialize the state
        Debug.Log("Helicopter state: " + currentState);
    }

    void Update()
    {
        // Handle state transitions
        switch (currentState)
        {
            case HelicopterState.Idle:
                HandleIdleState();
                break;

            case HelicopterState.Flying:
                HandleFlyingState();
                break;
        }

        // Allow state change from Inspector for debugging purposes
        if (Input.GetKeyDown(KeyCode.I))
        {
            SwitchToIdleState();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            SwitchToFlyingState();
        }

        // Update rotor speed and rotation
        UpdateRotors();
    }

    void HandleIdleState()
    {
        // Implement the behavior for the Idle state
        // For example, checking for an input to switch to the Flying state
    }

    void HandleFlyingState()
    {
        // Implement the behavior for the Flying state
        // For example, handling the flight mechanics
    }

    void UpdateRotors()
    {
        if (mainRotor != null && backRotor != null)
        {
            if (currentState == HelicopterState.Flying)
            {
                // Accelerate rotor speed
                if (currentRotorSpeed < maxRotorSpeed)
                {
                    currentRotorSpeed += rotorAcceleration * Time.deltaTime;
                    if (currentRotorSpeed >= maxRotorSpeed)
                    {
                        currentRotorSpeed = maxRotorSpeed;
                        if (!hasReachedMaxSpeed)
                        {
                            Debug.Log("Rotor reached maximum speed, ready for takeoff.");
                            hasReachedMaxSpeed = true;
                        }
                    }
                }
            }
            else
            {
                // Decelerate rotor speed
                if (currentRotorSpeed > 0)
                {
                    currentRotorSpeed -= rotorAcceleration * Time.deltaTime;
                    if (currentRotorSpeed <= 0)
                    {
                        currentRotorSpeed = 0;
                        if (!hasStoppedSpinning)
                        {
                            Debug.Log("Rotor has stopped spinning.");
                            hasStoppedSpinning = true;
                        }
                    }
                }
            }

            // Rotate the main rotor
            mainRotor.Rotate(Vector3.up, currentRotorSpeed * Time.deltaTime, Space.Self);
            // Rotate the back rotor
            backRotor.Rotate(Vector3.right, currentRotorSpeed * Time.deltaTime, Space.Self);

            // Reset flags if necessary
            if (currentRotorSpeed < maxRotorSpeed && hasReachedMaxSpeed)
            {
                hasReachedMaxSpeed = false;
            }
            if (currentRotorSpeed > 0 && hasStoppedSpinning)
            {
                hasStoppedSpinning = false;
            }
        }
    }

    public void SwitchToFlyingState()
    {
        currentState = HelicopterState.Flying;
        Debug.Log("Helicopter state: Flying");
    }

    public void SwitchToIdleState()
    {
        currentState = HelicopterState.Idle;
        Debug.Log("Helicopter state: Idle");
    }
}
