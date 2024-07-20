using UnityEngine;

public class RadarController : MonoBehaviour
{
    public float rotationSpeed = 30f; // Rotation speed in degrees per second
    public Transform rotationMechanism; // Assign the rotation mechanism (cylinder) here

    // State Machine
    private enum RadarState { Search }
    private RadarState currentState;

    void Start()
    {
        // Set the initial state to Search
        currentState = RadarState.Search;
    }

    void Update()
    {
        // Update the state machine
        switch (currentState)
        {
            case RadarState.Search:
                HandleSearchState();
                break;
                // You can add more states here if needed
        }
    }

    void HandleSearchState()
    {
        // Rotate the rotation mechanism around its local Y-axis
        if (rotationMechanism != null)
        {
            rotationMechanism.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        }
        else
        {
            Debug.LogWarning("Rotation mechanism is not assigned.");
        }
    }
}
