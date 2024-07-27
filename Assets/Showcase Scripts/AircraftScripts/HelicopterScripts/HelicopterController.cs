using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    // Define the states
    private enum HelicopterState { Idle, InitialStart, Flying }
    [SerializeField] private HelicopterState currentState;

    // Rotor references
    public Transform mainRotor;
    public Transform backRotor;

    // Rotor speed parameters
    public float maxRotorSpeed = 1000f; // Maximum rotor speed
    private float rotorAcceleration; // Rotor acceleration rate
    private float currentRotorSpeed = 0f; // Current rotor speed

    // Audio clips
    public AudioClip engineStartUpSound;
    public AudioClip flightSound;
    private AudioSource audioSource;

    private bool hasReachedMaxSpeed = false; // Flag to check if max speed is reached
    private bool hasStoppedSpinning = true; // Flag to check if rotor has stopped

    // Sound radius settings
    public float minSoundDistance = 1f; // Minimum distance for full volume
    public float maxSoundDistance = 50f; // Maximum distance for zero volume
    public float maxVolume = 1.0f; // Maximum volume of the sound

    void Start()
    {
        // Initialize the state
        Debug.Log("Helicopter state: " + currentState);

        // Initialize the audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.spatialBlend = 1.0f; // Make the sound 3D
        audioSource.minDistance = minSoundDistance;
        audioSource.maxDistance = maxSoundDistance;
        audioSource.volume = maxVolume;
    }

    void Update()
    {
        // Handle state transitions
        switch (currentState)
        {
            case HelicopterState.Idle:
                HandleIdleState();
                break;

            case HelicopterState.InitialStart:
                HandleInitialStartState();
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
        if (Input.GetKeyDown(KeyCode.S))
        {
            SwitchToInitialStartState();
        }

        // Update rotor speed and rotation
        UpdateRotors();
    }

    void HandleIdleState()
    {
        // Implement the behavior for the Idle state
        // For example, checking for an input to switch to the InitialStart state
    }

    void HandleInitialStartState()
    {
        if (currentRotorSpeed < maxRotorSpeed)
        {
            // Calculate dynamic acceleration so rotors reach max speed when engine startup sound ends
            if (!audioSource.isPlaying)
            {
                audioSource.clip = engineStartUpSound;
                audioSource.Play();
            }

            rotorAcceleration = maxRotorSpeed / engineStartUpSound.length;
            currentRotorSpeed += rotorAcceleration * Time.deltaTime;

            if (currentRotorSpeed >= maxRotorSpeed)
            {
                currentRotorSpeed = maxRotorSpeed;
                if (!hasReachedMaxSpeed)
                {
                    Debug.Log("Rotor reached maximum speed, ready for takeoff.");
                    hasReachedMaxSpeed = true;
                    SwitchToFlyingState();
                }
            }
        }
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
            if (currentState == HelicopterState.InitialStart || currentState == HelicopterState.Flying)
            {
                // Accelerate rotor speed in InitialStart state
                if (currentRotorSpeed < maxRotorSpeed && currentState == HelicopterState.InitialStart)
                {
                    currentRotorSpeed += rotorAcceleration * Time.deltaTime;
                    if (currentRotorSpeed >= maxRotorSpeed)
                    {
                        currentRotorSpeed = maxRotorSpeed;
                        if (!hasReachedMaxSpeed)
                        {
                            Debug.Log("Rotor reached maximum speed, ready for takeoff.");
                            hasReachedMaxSpeed = true;
                            SwitchToFlyingState();
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
                            audioSource.Stop();
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
        audioSource.clip = flightSound;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void SwitchToIdleState()
    {
        currentState = HelicopterState.Idle;
        Debug.Log("Helicopter state: Idle");
    }

    public void SwitchToInitialStartState()
    {
        currentState = HelicopterState.InitialStart;
        Debug.Log("Helicopter state: InitialStart");
    }
}
