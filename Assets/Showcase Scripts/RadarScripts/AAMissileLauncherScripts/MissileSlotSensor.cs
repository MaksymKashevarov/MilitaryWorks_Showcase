using UnityEngine;

public class MissileSlotSensor : MonoBehaviour
{
    public LayerMask missileLayer; // Layer for the missile
    public bool launchMissileManually = false; // Switch to launch missile manually
    public MissileLauncher missileLauncher; // Assign the MissileLauncher script here

    private Transform lockedTarget; // The locked target from the missile launcher
    private bool targetLockedDebugged = false; // To ensure the target is debugged only once

    // State Machine
    private enum SlotState { Idle, Armed, Launch }
    private SlotState currentState;
    private SlotState previousState;

    void Start()
    {
        // Set the initial state to Idle
        currentState = SlotState.Idle;
        previousState = currentState;
        CheckMissileInSlot();
    }

    void Update()
    {
        // Handle manual launch switch
        if (launchMissileManually)
        {
            if (currentState == SlotState.Armed)
            {
                SetStateLaunch();
                launchMissileManually = false; // Reset switch
            }
            else
            {
                Debug.LogWarning("Cannot launch missile manually when not in armed state.");
                launchMissileManually = false; // Reset switch
            }
        }

        // Read the locked target from the missile launcher
        if (missileLauncher != null && missileLauncher.CurrentState == MissileLauncher.LauncherState.LockedForAttack)
        {
            lockedTarget = missileLauncher.GetLockedTarget();
            if (!targetLockedDebugged)
            {
                Debug.Log("Slot " + gameObject.name + " locked on target: " + lockedTarget.name);
                targetLockedDebugged = true; // Ensure the target is debugged only once
            }
        }
        else
        {
            targetLockedDebugged = false; // Reset when not in LockedForAttack state
        }
    }

    void CheckMissileInSlot()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity, missileLayer);

        if (colliders.Length > 0)
        {
            foreach (Collider col in colliders)
            {
                if (col.CompareTag("AA_Missile"))
                {
                    Debug.Log("Missile ready in slot: " + gameObject.name + " | Missile Name: " + col.gameObject.name);
                    return;
                }
            }
        }
        Debug.Log("No missile in slot: " + gameObject.name);
    }

    public void SetStateIdle()
    {
        currentState = SlotState.Idle;
        if (currentState != previousState)
        {
            Debug.Log("Slot " + gameObject.name + " is now Idle.");
            previousState = currentState;
        }
    }

    public void SetStateArmed()
    {
        currentState = SlotState.Armed;
        if (currentState != previousState)
        {
            Debug.Log("Slot " + gameObject.name + " is now Armed.");
            previousState = currentState;
        }
    }

    public void SetStateLaunch()
    {
        currentState = SlotState.Launch;
        if (currentState != previousState)
        {
            Debug.Log("Slot " + gameObject.name + " is now Launching.");
            previousState = currentState;
        }
    }

    public string GetCurrentState()
    {
        return currentState.ToString();
    }
}
