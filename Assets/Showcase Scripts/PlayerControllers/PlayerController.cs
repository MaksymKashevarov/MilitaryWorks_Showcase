using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookSensitivity = 2f;
    public Camera playerCamera;

    private Vector3 moveDirection;
    private Rigidbody rb;
    private float verticalRotation = 0f;
    private float maxVerticalAngle = 90f;

    // State Machine
    private enum PlayerState { Idle, Moving }
    private PlayerState currentState;
    private PlayerState previousState;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentState = PlayerState.Idle;
        previousState = currentState;

        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Constrain the rotation on the x and z axes to prevent tipping over
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        UpdateState();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        moveDirection = transform.right * moveX + transform.forward * moveZ;
        moveDirection *= moveSpeed * Time.deltaTime;

        rb.MovePosition(transform.position + moveDirection);
    }

    void HandleRotation()
    {
        float rotateHorizontal = Input.GetAxis("Mouse X") * lookSensitivity;
        float rotateVertical = -Input.GetAxis("Mouse Y") * lookSensitivity;

        transform.Rotate(0, rotateHorizontal, 0);

        verticalRotation += rotateVertical;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalAngle, maxVerticalAngle);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void UpdateState()
    {
        if (moveDirection.magnitude > 0)
        {
            currentState = PlayerState.Moving;
        }
        else
        {
            currentState = PlayerState.Idle;
        }

        if (currentState != previousState)
        {
            Debug.Log("Current State: " + currentState);
            previousState = currentState;
        }
    }
}
