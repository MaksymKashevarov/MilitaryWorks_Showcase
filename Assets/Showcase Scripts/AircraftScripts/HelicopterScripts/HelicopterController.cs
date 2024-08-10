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

    // Lift-off parameters
    public float liftOffSpeed = 2f; // Speed at which the helicopter lifts off
    public float liftOffHeight = 5f; // Height the helicopter should reach during lift-off
    public float bounceHeight = 0.5f; // Height for the bounce effect
    public float bounceSpeed = 1f; // Speed for the bounce effect
    private bool isLiftingOff = false;

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

    // Door reference
    public Transform helicopterDoor;
    private Vector3 doorOpenPosition = new Vector3(0f, 0f, -7.89f); // Local open position of the door
    private Vector3 doorClosedPosition = Vector3.zero; // Assuming the default local position is the closed position
    public float doorSlideSpeed = 2f; // Speed at which the door slides
    private bool isClosingDoor = false;

    // Rigidbody reference
    private Rigidbody rb;

    // Control parameters
    public float pitchSpeed = 5f; // Speed of pitching
    public float yawSpeed = 5f;   // Speed of yawing
    public float rollSpeed = 5f;  // Speed of rolling
    public float throttleSpeed = 10f; // Speed of throttle (vertical control)
    public float maxThrust = 500f; // Maximum thrust force

    // Aerodynamic parameters
    private float dragCoefficient;
    private float liftCoefficient;
    private float airDensity;

    // Keycodes for controls
    public KeyCode pitchUpKey = KeyCode.W;
    public KeyCode pitchDownKey = KeyCode.S;
    public KeyCode yawLeftKey = KeyCode.A;
    public KeyCode yawRightKey = KeyCode.D;
    public KeyCode rollLeftKey = KeyCode.Q;
    public KeyCode rollRightKey = KeyCode.E;
    public KeyCode throttleUpKey = KeyCode.LeftShift;
    public KeyCode throttleDownKey = KeyCode.LeftControl;

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

        if (helicopterDoor != null)
        {
            doorClosedPosition = helicopterDoor.localPosition; // Save the initial local position as closed position
        }

        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing!");
        }

        // Set the Rigidbody parameters to ensure realistic physics
        rb.useGravity = true;
        rb.drag = 1f;
        rb.angularDrag = 2f;

        // Get aerodynamic parameters from the global environment
        airDensity = GlobalEnvironment.GetAirDensity();
        dragCoefficient = GlobalEnvironment.GetDragCoefficient();
        liftCoefficient = GlobalEnvironment.GetLiftCoefficient();
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

        // Update rotor speed and rotation
        UpdateRotors();

        // Update door position
        UpdateDoorPosition();
    }

    void FixedUpdate()
    {
        // Apply controls and aerodynamics only if in Flying state
        if (currentState == HelicopterState.Flying)
        {
            ApplyFlightControls();
            ApplyAerodynamics();
        }
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
        if (!isLiftingOff)
        {
            isLiftingOff = true;
            StartCoroutine(LiftOff());
        }
    }

    System.Collections.IEnumerator LiftOff()
    {
        float startY = transform.position.y;
        float targetY = startY + liftOffHeight;

        // Lift off smoothly to the target height
        while (transform.position.y < targetY)
        {
            transform.position += Vector3.up * liftOffSpeed * Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
        Debug.Log("Helicopter has lifted off.");

        // Implement the bounce effect
        float bounceTargetY = targetY + bounceHeight;
        while (transform.position.y < bounceTargetY)
        {
            transform.position += Vector3.up * bounceSpeed * Time.deltaTime;
            yield return null;
        }

        while (transform.position.y > targetY)
        {
            transform.position -= Vector3.up * bounceSpeed * Time.deltaTime;
            yield return null;
        }

        Debug.Log("Helicopter has completed lift-off and bounce.");
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

    void UpdateDoorPosition()
    {
        if (helicopterDoor != null)
        {
            if (currentState == HelicopterState.Idle)
            {
                helicopterDoor.localPosition = Vector3.Lerp(helicopterDoor.localPosition, doorOpenPosition, Time.deltaTime * doorSlideSpeed);
                isClosingDoor = false;
            }
            else if (!isClosingDoor)
            {
                isClosingDoor = true;
                Invoke("StartClosingDoor", 5f);
            }
        }
    }

    void StartClosingDoor()
    {
        StartCoroutine(CloseDoor());
    }

    System.Collections.IEnumerator CloseDoor()
    {
        while (helicopterDoor.localPosition != doorClosedPosition)
        {
            helicopterDoor.localPosition = Vector3.Lerp(helicopterDoor.localPosition, doorClosedPosition, Time.deltaTime * doorSlideSpeed);
            yield return null;
        }
        isClosingDoor = false; // Reset the flag once the door is closed
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

    void ApplyFlightControls()
    {
        // Get input for pitch, yaw, roll, and throttle using KeyCodes
        float pitch = 0f;
        if (Input.GetKey(pitchUpKey)) pitch = pitchSpeed;
        if (Input.GetKey(pitchDownKey)) pitch = -pitchSpeed;

        float yaw = 0f;
        if (Input.GetKey(yawLeftKey)) yaw = -yawSpeed;
        if (Input.GetKey(yawRightKey)) yaw = yawSpeed;

        float roll = 0f;
        if (Input.GetKey(rollLeftKey)) roll = -rollSpeed;
        if (Input.GetKey(rollRightKey)) roll = rollSpeed;

        float throttle = 0f;
        if (Input.GetKey(throttleUpKey)) throttle = throttleSpeed;
        if (Input.GetKey(throttleDownKey)) throttle = -throttleSpeed;

        // Calculate lift force based on the throttle and environmental air density
        float liftForce = throttle * maxThrust * airDensity;

        // Apply lift force
        rb.AddForce(Vector3.up * liftForce);

        // Apply pitch (forward/backward tilt)
        rb.AddTorque(transform.right * pitch);

        // Apply yaw (left/right rotation)
        rb.AddTorque(transform.up * yaw);

        // Apply roll (tilting left/right)
        rb.AddTorque(transform.forward * -roll);

        // Gravity and drag are naturally applied by the Rigidbody component
    }

    void ApplyAerodynamics()
    {
        // Get the helicopter's velocity
        Vector3 velocity = rb.velocity;

        // Calculate lift force
        float liftMagnitude = liftCoefficient * airDensity * velocity.sqrMagnitude * 0.5f;
        Vector3 liftDirection = transform.up;
        Vector3 liftForce = liftMagnitude * liftDirection;

        // Calculate drag force
        float dragMagnitude = dragCoefficient * airDensity * velocity.sqrMagnitude * 0.5f;
        Vector3 dragForce = -dragMagnitude * velocity.normalized;

        // Apply the aerodynamic forces
        rb.AddForce(liftForce);
        rb.AddForce(dragForce);
    }
}
