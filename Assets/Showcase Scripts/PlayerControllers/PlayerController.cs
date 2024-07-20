using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookSensitivity = 2f;
    public Camera playerCamera;

    private Vector3 moveDirection;
    private CharacterController characterController;

    // State Machine
    private enum PlayerState { Idle, Moving }
    private PlayerState currentState;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        currentState = PlayerState.Idle;
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

        moveDirection = new Vector3(moveX, 0, moveZ);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= moveSpeed * Time.deltaTime;

        characterController.Move(moveDirection);
    }

    void HandleRotation()
    {
        float rotateHorizontal = Input.GetAxis("Mouse X") * lookSensitivity;
        float rotateVertical = -Input.GetAxis("Mouse Y") * lookSensitivity;

        transform.Rotate(0, rotateHorizontal, 0);
        playerCamera.transform.Rotate(rotateVertical, 0, 0);
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

        Debug.Log("Current State: " + currentState);
    }
}
