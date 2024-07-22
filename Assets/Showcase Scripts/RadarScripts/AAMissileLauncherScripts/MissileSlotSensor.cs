using UnityEngine;

public class MissileSlotSensor : MonoBehaviour
{
    public LayerMask missileLayer; // Layer for the missile
    public bool launchMissileManually = false; // Switch to launch missile manually
    public MissileLauncher missileLauncher; // Assign the MissileLauncher script here
    public LayerMask engineLayer; // Layer for engine objects

    private Transform lockedTarget; // The locked target from the missile launcher
    private bool targetLockedDebugged = false; // To ensure the target is debugged only once
    private bool heatDetected = false; // To ensure the heat is debugged only once
    private Transform heatSource; // Detected heat source

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

                // Check for engine objects with heat signature
                CheckForHeatSignature();
            }
        }
        else
        {
            targetLockedDebugged = false; // Reset when not in LockedForAttack state
            heatDetected = false; // Reset when not in LockedForAttack state
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

    void CheckForHeatSignature()
    {
        Collider[] engineColliders = Physics.OverlapSphere(lockedTarget.position, 10f, engineLayer); // Adjust the radius as needed

        foreach (Collider col in engineColliders)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Engine"))
            {
                EngineHeatGenerator heatGenerator = col.GetComponent<EngineHeatGenerator>();
                if (heatGenerator != null)
                {
                    if (!heatDetected)
                    {
                        heatSource = col.transform;
                        Debug.Log("Heat detected from engine: " + col.gameObject.name + " with intensity: " + heatGenerator.GetHeatIntensity());
                        heatDetected = true; // Ensure the heat is debugged only once
                        AimMissileAtHeatSource();
                    }
                    return;
                }
            }
        }
        if (!heatDetected)
        {
            Debug.Log("No heat detected for target: " + lockedTarget.name);
        }
    }

    void AimMissileAtHeatSource()
    {
        // Assign the heat source to the missile
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity, missileLayer);

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("AA_Missile"))
            {
                AGM88Controller missileController = col.GetComponent<AGM88Controller>();
                if (missileController != null)
                {
                    missileController.SetTargetHeatSource(heatSource);
                }
            }
        }
    }

    public Transform GetHeatSource()
    {
        return heatSource;
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

            // Switch thruster state to acceleration for the missile in this slot
            Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity, missileLayer);

            foreach (Collider col in colliders)
            {
                if (col.CompareTag("AA_Missile"))
                {
                    ThrusterScript thruster = col.GetComponentInChildren<ThrusterScript>();
                    if (thruster != null)
                    {
                        thruster.SwitchState(ThrusterScript.ThrusterState.Acceleration);
                    }
                }
            }
        }
    }

    public string GetCurrentState()
    {
        return currentState.ToString();
    }
}
