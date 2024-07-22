using UnityEngine;

public class HeatSignature : MonoBehaviour
{
    [Tooltip("Radius of the heat field in meters")]
    public float heatFieldRadius = 0.5f; // Radius of the heat field
    public float baseHeatIntensity = 100f; // Base intensity of the heat signature
    public float intensityFactor = 200f; // Factor to adjust the heat intensity

    private float heatIntensity;

    void Start()
    {
        // Adjust heat intensity based on the initial radius
        AdjustHeatIntensityToRadius();
    }

    void Update()
    {
        // Continuously adjust heat intensity to match the heat field radius
        AdjustHeatIntensityToRadius();
    }

    void AdjustHeatIntensityToRadius()
    {
        heatIntensity = baseHeatIntensity + (heatFieldRadius * intensityFactor);
    }

    void OnValidate()
    {
        // Ensure the heat field radius is within reasonable bounds when changed in the inspector
        heatFieldRadius = Mathf.Clamp(heatFieldRadius, 0.01f, 100f);
        AdjustHeatIntensityToRadius();
    }

    void OnDrawGizmos()
    {
        // Draw the heat field in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, heatFieldRadius);
    }

    public float GetHeatIntensity()
    {
        return heatIntensity;
    }
}
