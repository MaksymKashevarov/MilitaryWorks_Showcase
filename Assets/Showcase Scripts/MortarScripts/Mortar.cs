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
    public float angle; // Angle to set the mortar
    public float azimuth; // Azimuth to set the mortar

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
        // Adjust the mortar's angle and azimuth based on the input values
        transform.localEulerAngles = new Vector3(-angle, azimuth, 0);

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
    }

    void HandleEmptyState()
    {
        // Implement the behavior for the Empty state
        // For example, waiting for reload input
    }

    void HandleArmedState()
    {
        // Implement the behavior for the Armed state
        // For example, waiting for launch input
    }

    void HandleLaunchState()
    {
        // Implement the behavior for the Launch state
        // For example, applying the launch force and transitioning to Empty state
        if (currentShell != null)
        {
            Rigidbody rb = currentShell.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(transform.up * launchForce, ForceMode.Impulse);
                currentShell.transform.SetParent(null);
            }
        }

        Invoke("SwitchToEmptyState", 3f);
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

    void SwitchToEmptyState()
    {
        currentState = MortarState.Empty;
        Debug.Log("Mortar state: Empty");
    }
}
