using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [System.Serializable]
    public class UITypeMapping
    {
        public UIType uiType;
        public GameObject uiObject;
    }

    // List to dynamically assign UI types and their corresponding GameObjects in the Inspector
    public List<UITypeMapping> uiTypeMappings;

    // Dictionary to store the mappings for quick lookup
    private Dictionary<UIType, GameObject> uiDictionary;

    private TMP_Text accelerationText;

    private void Awake()
    {
        // Ensure there is only one instance of UIManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep the UIManager persistent across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize the dictionary and hide all UI types at the start
        InitializeUIDictionary();

        // Find and store the reference to the accelerationText component
        InitializeAccelerationText();
    }

    private void InitializeUIDictionary()
    {
        uiDictionary = new Dictionary<UIType, GameObject>();

        foreach (var mapping in uiTypeMappings)
        {
            if (mapping.uiObject != null)
            {
                uiDictionary[mapping.uiType] = mapping.uiObject;
                mapping.uiObject.SetActive(false); // Hide the UI by default
            }
        }
    }

    private void InitializeAccelerationText()
    {
        if (uiDictionary.ContainsKey(UIType.AircraftUI))
        {
            GameObject aircraftUI = uiDictionary[UIType.AircraftUI];
            accelerationText = aircraftUI.GetComponentInChildren<TMP_Text>();

            if (accelerationText != null)
            {
                // Initialize with default text
                accelerationText.text = "Acceleration: 0%";
            }
            else
            {
                Debug.LogWarning("Acceleration TextMeshPro component not found in AircraftUI.");
            }
        }
        else
        {
            Debug.LogWarning("AircraftUI not found in UIManager.");
        }
    }

    // This method will be called by other objects to show or hide their specific UI
    public void ToggleUI(UIType uiType, bool show)
    {
        if (uiDictionary.ContainsKey(uiType))
        {
            uiDictionary[uiType].SetActive(show);
        }
        else
        {
            Debug.LogWarning($"UIType {uiType} not found in the UIManager.");
        }
    }

    // This method updates the acceleration percentage
    public void UpdateAcceleration(float percentage)
    {
        if (accelerationText != null)
        {
            accelerationText.text = $"Acceleration: {percentage}%";
        }
        else
        {
            Debug.LogWarning("Acceleration TextMeshPro component is not assigned.");
        }
    }
}

public enum UIType
{
    AircraftUI,
    // Add more UI types as needed
}
