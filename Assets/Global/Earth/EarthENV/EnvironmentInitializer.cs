using UnityEngine;

public class EnvironmentInitializer : MonoBehaviour
{
    void Start()
    {
        // Access the static class to trigger its static constructor
        float gravity = GlobalEnvironment.GetGravity();
    }
}
