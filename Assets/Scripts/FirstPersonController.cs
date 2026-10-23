using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Referensi")]
    [Tooltip("Child object yang berisi Main Camera (taruh setinggi mata, sekitar y = 1.6)")]
    [SerializeField] private Transform cameraPivot;

    [Header("Status")]
    [SerializeField] public int HP = 100;
    [SerializeField] public int LV = 10;
    [SerializeField] public int MANA = 100;

    [Header("Gerak")]
    [SerializeField] private float walkSpeed = 0.5f;
    [SerializeField] private float sprintSpeed = 1f;
    [SerializeField] private float jumpHeight = 0.2f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private bool allowJump = false; // dungeon biasanya tanpa lompat

    [Header("Kamera")]
    [SerializeField] private float mouseSensitivity = 0.3f;
    [SerializeField] private float gamepadSensitivity = 120f;
    [SerializeField] private float maxLookAngle = 85f;

    private CharacterController controller;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction sprintAction;
    private InputAction jumpAction;

    private float pitch;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // WASD + analog kiri gamepad
        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.AddBinding("<Gamepad>/leftStick");

        // Mouse + analog kanan gamepad
        lookAction = new InputAction("Look", InputActionType.Value);
        lookAction.AddBinding("<Mouse>/delta");
        lookAction.AddBinding("<Gamepad>/rightStick");

        // Lari
        sprintAction = new InputAction("Sprint", InputActionType.Button);
        sprintAction.AddBinding("<Keyboard>/leftShift");
        sprintAction.AddBinding("<Gamepad>/leftStickPress");

        // Lompat
        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        sprintAction.Enable();
        jumpAction.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        sprintAction.Disable();
        jumpAction.Disable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
    }

    private void HandleLook()
    {
        Vector2 look = lookAction.ReadValue<Vector2>();

        // Mouse delta sudah per-frame, stick gamepad perlu dikali waktu
        bool usingGamepad = lookAction.activeControl != null &&
                            lookAction.activeControl.device is Gamepad;

        float sensitivity = usingGamepad
            ? gamepadSensitivity * Time.deltaTime
            : mouseSensitivity;

        // Putar badan kiri-kanan (yaw)
        transform.Rotate(Vector3.up * (look.x * sensitivity));

        // Putar kamera atas-bawah (pitch)
        pitch -= look.y * sensitivity;
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        float speed = sprintAction.IsPressed() ? sprintSpeed : walkSpeed;

        Vector3 move = (transform.right * input.x + transform.forward * input.y);
        move = Vector3.ClampMagnitude(move, 1f) * speed;

        // Gravitasi dan lompat
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f; // sedikit menempel ke lantai

        if (allowJump && controller.isGrounded && jumpAction.WasPressedThisFrame())
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }
}