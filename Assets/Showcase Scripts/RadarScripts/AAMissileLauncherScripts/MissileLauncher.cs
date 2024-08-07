using UnityEngine;

public class MissileLauncher : MonoBehaviour
{
    public RadarController assignedRadar; // Assign the radar with RadarController script here
    public MissileSlotSensor[] missileSlots; // Assign the missile slots with MissileSlotSensor script here

    // State Machine
    public enum LauncherState { Waiting, LockedForAttack }
    private LauncherState currentState;
    private LauncherState previousState;
    private Transform lockedTarget; // The locked target from the radar

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
                    lockedTarget = assignedRadar.GetLockedTarget(); // Save the locked target
                    break;
            }

            // Log state change if the state has changed
            if (currentState != previousState)
            {
                Debug.Log("Missile Launcher State: " + currentState);

                if (currentState == LauncherState.LockedForAttack)
                {
                    Debug.Log("Locked Target: " + lockedTarget.name); // Debug the locked target once
                    CheckMissilesReadyForAttack();
                }

                UpdateSlotStates();

                previousState = currentState;
            }
        }
        else
        {
            Debug.LogWarning("Assigned radar is not set.");
        }
    }

    void CheckMissilesReadyForAttack()
    {
        foreach (MissileSlotSensor slot in missileSlots)
        {
            Collider[] colliders = Physics.OverlapBox(slot.transform.position, slot.transform.localScale / 2, Quaternion.identity, LayerMask.GetMask("AA_Missile"));

            foreach (Collider col in colliders)
            {
                if (col.CompareTag("AA_Missile"))
                {
                    Debug.Log("Missile ready in slot: " + slot.gameObject.name + " | Missile Name: " + col.gameObject.name);
                }
            }
        }
    }

    void UpdateSlotStates()
    {
        foreach (MissileSlotSensor slot in missileSlots)
        {
            if (currentState == LauncherState.LockedForAttack)
            {
                slot.SetStateArmed();
            }
            else
            {
                slot.SetStateIdle();
            }
        }
    }

    public Transform GetLockedTarget()
    {
        return lockedTarget;
    }

    public LauncherState CurrentState
    {
        get { return currentState; }
    }
}
