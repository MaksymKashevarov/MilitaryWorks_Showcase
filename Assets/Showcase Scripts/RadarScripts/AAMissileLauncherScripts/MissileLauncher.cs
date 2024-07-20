using UnityEngine;

public class MissileLauncher : MonoBehaviour
{
    public RadarController assignedRadar; // Assign the radar with RadarController script here

    // State Machine
    private enum LauncherState { Waiting, LockedForAttack }
    private LauncherState currentState;
    private LauncherState previousState;

    void Start()
    {
        // Set the initial state to Waiting
        currentState = LauncherState.Waiting;
        previousState = currentState;
    }

    void Update()
    {
        // Check the state of the assigned radar
        if (assignedRadar != null)
        {
            switch (assignedRadar.CurrentState)
            {
                case RadarController.RadarState.Search:
                    currentState = LauncherState.Waiting;
                    break;
                case RadarController.RadarState.LockOnTarget:
                    currentState = LauncherState.LockedForAttack;
                    break;
            }

            // Log state change if the state has changed
            if (currentState != previousState)
            {
                Debug.Log("Missile Launcher State: " + currentState);
                previousState = currentState;
            }
        }
        else
        {
            Debug.LogWarning("Assigned radar is not set.");
        }
    }
}
