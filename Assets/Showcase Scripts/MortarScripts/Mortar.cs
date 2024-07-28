using UnityEngine;
using System.Collections.Generic;

public class Mortar : MonoBehaviour
{
    // Configuration Section
    [Header("Configuration")]
    public float weight; // Weight of the mortar (not used currently)
    public List<GameObject> allowedShells; // List of allowed shell prefabs
    public float launchForce; // Force used to launch the shell

    // Developer Section
    [Header("Developer Tools")]
    public GameObject selectedShell; // Selected shell prefab from the list
    public Transform pointer; // Pointer to set the rotation of the shell when spawned
    public float azimuth; // Azimuth to set the mortar
    public float angle; // Angle to set the mortar
    public bool launchShell; // Checkbox to launch the shell

    // Mortar State Machine
    private enum MortarState { Empty, Armed, Launch }
    [SerializeField] private MortarState currentState;

    // Private variables
    private GameObject currentShell;

    void Start()
    {
        currentState = MortarState.Empty;
        Debug.Log("Mortar state: " + currentState);
    }

    void Update()
    {
        // Adjust the mortar's azimuth and angle based on the input values
        transform.localEulerAngles = new Vector3(angle, azimuth, 0);

        // Check if a shell is selected and switch state to Armed
        if (currentState == MortarState.Empty && selectedShell != null)
        {
            Debug.Log("Shell ready. Mortar state: Armed");
            currentState = MortarState.Armed;
        }

        // Handle state transitions
        switch (currentState)
        {
            case MortarState.Empty:
                HandleEmptyState();
                break;
            case MortarState.Armed:
                HandleArmedState();
                break;
            case MortarState.Launch:
                HandleLaunchState();
                break;
        }

        // Check if launch checkbox is selected
        if (launchShell)
        {
            launchShell = false; // Reset the checkbox
            if (currentState == MortarState.Armed)
            {
                currentState = MortarState.Launch;
                Debug.Log("Mortar state: Launch");
                Invoke("SwitchToEmptyState", 0.2f);
            }
            else
            {
                Debug.LogError("Launch error: Mortar is not armed.");
            }
        }
    }

    void HandleEmptyState()
    {
        // Implement the behavior for the Empty state
    }

    void HandleArmedState()
    {
        // Implement the behavior for the Armed state
    }

    void HandleLaunchState()
    {
        // Implement the behavior for the Launch state
        if (currentShell == null)
        {
            currentShell = Instantiate(selectedShell, transform);
            currentShell.transform.localPosition = new Vector3(0.1027f, -0.0258f, 0.7455f);
            currentShell.transform.rotation = pointer.rotation;

            Rigidbody rb = currentShell.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(transform.up * launchForce, ForceMode.Impulse);
                currentShell.transform.SetParent(null);
            }
            else
            {
                Debug.LogError("Launch error: Shell Rigidbody not found.");
            }
        }
    }

    void SwitchToEmptyState()
    {
        currentState = MortarState.Empty;
        Debug.Log("Mortar state: Empty");
        currentShell = null; // Reset the current shell
    }

    public void Reload()
    {
        if (currentState == MortarState.Empty && selectedShell != null)
        {
            // Spawn the selected shell and set its position and rotation
            currentShell = Instantiate(selectedShell, transform);
            currentShell.transform.localPosition = new Vector3(0.1027f, -0.0258f, 0.7455f);
            currentShell.transform.rotation = pointer.rotation;

            currentState = MortarState.Armed;
            Debug.Log("Mortar state: Armed");
        }
    }

    public void Launch()
    {
        if (currentState == MortarState.Armed)
        {
            currentState = MortarState.Launch;
            Debug.Log("Mortar state: Launch");
        }
    }
}
