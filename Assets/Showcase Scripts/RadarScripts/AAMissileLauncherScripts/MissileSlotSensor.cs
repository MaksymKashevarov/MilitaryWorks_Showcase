using UnityEngine;

public class MissileSlotSensor : MonoBehaviour
{
    public LayerMask missileLayer; // Layer for the missile
    public bool launchMissileManually = false; // Switch to launch missile manually

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
