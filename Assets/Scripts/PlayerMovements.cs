using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Transform cameraTarget;
    public Animator animator;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float acceleration = 15f;
    public float deceleration = 15f;

    [Header("Gravity")]
    public float gravity = -20f;
    private float verticalVelocity;

    [Header("Camera")]
    public float mouseSensitivity = 0.15f;
    public float minCameraAngle = -70f;
    public float maxCameraAngle = 70f;

    private float currentSpeed;
    private float cameraPitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        CameraAndPlayerLook();
        Movement();
        ApplyGravity();
    }

    void CameraAndPlayerLook()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouse = Mouse.current.delta.ReadValue();

        float mouseX = mouse.x * mouseSensitivity;

        transform.Rotate(
            Vector3.up * mouseX
        );

        cameraPitch -= mouse.y * mouseSensitivity;

        cameraPitch = Mathf.Clamp(
            cameraPitch,
            minCameraAngle,
            maxCameraAngle
        );

        cameraTarget.localRotation =
            Quaternion.Euler(
                cameraPitch,
                0f,
                0f
            );
    }

    void Movement()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            input.y += 1;

        if (Keyboard.current.sKey.isPressed)
            input.y -= 1;

        if (Keyboard.current.dKey.isPressed)
            input.x += 1;

        if (Keyboard.current.aKey.isPressed)
            input.x -= 1;

        input = Vector2.ClampMagnitude(
            input,
            1f
        );

        Vector3 direction =
            transform.forward * input.y +
            transform.right * input.x;

        bool isRunning =
            Keyboard.current.leftShiftKey.isPressed;

        float targetSpeed =
            isRunning
                ? runSpeed
                : walkSpeed;

        if (input.magnitude > 0.01f)
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                targetSpeed,
                acceleration * Time.deltaTime
            );
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                0f,
                deceleration * Time.deltaTime
            );
        }

        controller.Move(
            direction *
            currentSpeed *
            Time.deltaTime
        );

        float animationSpeed = 0f;

        if (input.y < -0.01f)
        {
            if (isRunning)
                animationSpeed = 110f;
            else
                animationSpeed = 65f;
        }
        else if (input.x < -0.01f)
        {
            if (isRunning)
                animationSpeed = 80f;
            else
                animationSpeed = 35f;
        }
        else if (input.x > 0.01f)
        {
            if (isRunning)
                animationSpeed = 95f;
            else
                animationSpeed = 50f;
        }
        else if (input.y > 0.01f)
        {
            if (isRunning)
                animationSpeed = 20f;
            else
                animationSpeed = 5f;
        }

        if (input.magnitude < 0.01f)
        {
            animationSpeed = 0f;
        }

        animator.SetFloat(
            "Speed",
            animationSpeed
        );
    }

    void ApplyGravity()
    {
        if (controller.isGrounded &&
            verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity +=
            gravity *
            Time.deltaTime;

        controller.Move(
            Vector3.up *
            verticalVelocity *
            Time.deltaTime
        );
    }
}