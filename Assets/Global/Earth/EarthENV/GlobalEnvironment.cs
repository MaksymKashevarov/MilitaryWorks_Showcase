using UnityEngine;

public static class GlobalEnvironment
{
    // Environment settings
    private static float gravity = -9.81f;
    private static float airDensity = 1.225f; // kg/m^3 at sea level
    private static float dragCoefficient = 0.47f; // For a sphere, can be adjusted
    private static float liftCoefficient = 1.0f; // Placeholder value, can be adjusted

    static GlobalEnvironment()
    {
        // This static constructor is called only once, before any static members are accessed
        Debug.Log("GlobalEnvironment initialized with the following settings:");
        Debug.Log("Gravity: " + gravity);
        Debug.Log("Air Density: " + airDensity);
        Debug.Log("Drag Coefficient: " + dragCoefficient);
        Debug.Log("Lift Coefficient: " + liftCoefficient);
    }

    // Get and set methods for gravity
    public static float GetGravity()
    {
        return gravity;
    }

    public static void SetGravity(float newGravity)
    {
        gravity = newGravity;
        Debug.Log("Gravity set to: " + gravity);
    }

    // Get and set methods for air density
    public static float GetAirDensity()
    {
        return airDensity;
    }

    public static void SetAirDensity(float newAirDensity)
    {
        airDensity = newAirDensity;
        Debug.Log("Air Density set to: " + airDensity);
    }

    // Get and set methods for drag coefficient
    public static float GetDragCoefficient()
    {
        return dragCoefficient;
    }

    public static void SetDragCoefficient(float newDragCoefficient)
    {
        dragCoefficient = newDragCoefficient;
        Debug.Log("Drag Coefficient set to: " + dragCoefficient);
    }

    // Get and set methods for lift coefficient
    public static float GetLiftCoefficient()
    {
        return liftCoefficient;
    }

    public static void SetLiftCoefficient(float newLiftCoefficient)
    {
        liftCoefficient = newLiftCoefficient;
        Debug.Log("Lift Coefficient set to: " + liftCoefficient);
    }
}
