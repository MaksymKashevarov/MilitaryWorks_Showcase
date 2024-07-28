using UnityEngine;

public class MortarShell : MonoBehaviour
{
    [Header("Shell Configuration")]
    public float explosionRadius = 5f; // Radius of the explosion
    public float weight = 10f; // Weight of the shell
    public Transform nose; // Nose object representing the leading axis
    public float minVelocityThreshold = 0.5f; // Minimum velocity required to activate the explosion

    private Rigidbody rb;
    private bool hasExploded = false;
    private bool isActive = false; // Indicates if the shell is active and can explode

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.mass = weight;
    }

    void Update()
    {
        // Check if the shell's velocity exceeds the threshold to become active
        if (rb.velocity.magnitude > minVelocityThreshold)
        {
            isActive = true;
        }

        if (!hasExploded && isActive)
        {
            // Align the shell's nose with the velocity direction
            if (rb.velocity != Vector3.zero)
            {
                // Calculate the rotation needed for the nose to point in the direction of the velocity
                Quaternion targetRotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
                nose.rotation = Quaternion.Slerp(nose.rotation, targetRotation, Time.deltaTime * 10f); // Smoothly rotate the nose
                transform.rotation = nose.rotation; // Ensure the shell follows the nose's rotation
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasExploded && isActive)
        {
            hasExploded = true;
            Explode();
        }
    }

    void Explode()
    {
        // Perform explosion raycast to check for affected objects
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log("Object hit by explosion: " + hitCollider.name);
            // Here you can add code to deal damage, apply force, etc.
        }

        // Despawn the shell
        Destroy(gameObject);
    }
}


//Vector3(0.102700002,-0.0258000009,0.745500028)
//Vector3(0.102700002,-0.0258000009,0.745500028)