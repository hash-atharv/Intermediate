using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    private static readonly int SpeedHash = Animator.StringToHash("speed");
    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int IsRunningHash = Animator.StringToHash("isRunning");
    private static readonly int XaxisHash = Animator.StringToHash("Xaxis");
    private static readonly int YaxisHash = Animator.StringToHash("Yaxis");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int JumpHash = Animator.StringToHash("Jump");

    [SerializeField] private Animator animator;
    [SerializeField] private Transform cameraTransform;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float runSpeedMultiplier = 1.75f; // applied to speed while Shift is held and moving
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.2f;

    public bool IsMoving { get; private set; }
    public bool IsRunning { get; private set; }   // true while sprinting AND actually moving
    public bool IsGrounded { get; private set; }

    private PlayerControls controls;
    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector3 direction;
    private Vector3 velocity;
    private float currentSpeed;                    // actual speed used this frame (walk or run)

    private static bool IsGamePlaying =>
        GameManager.Instance == null || GameManager.Instance.CurrentState == GameState.Playing;

    private void Awake()
    {
        controls = new PlayerControls();
        characterController = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Fire.performed += OnFirePerformed;
    }

    private void OnDisable()
    {
        controls.Gameplay.Fire.performed -= OnFirePerformed;
        controls.Gameplay.Disable();
    }

    private void OnDestroy()
    {
        controls?.Dispose();
    }

    private void Update()
    {
        if (!IsGamePlaying) return;

        // Player always faces the direction the camera is looking (Cinemachine FreeLook handles the mouse)
        FaceCameraDirection();

        moveInput = controls.Gameplay.Move.ReadValue<Vector2>();

        bool sprintHeld = Keyboard.current != null && Keyboard.current.shiftKey.isPressed;
        bool jumpPressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

        MovePlayer(sprintHeld, jumpPressed);
        UpdateAnimator();
    }

    private void OnFirePerformed(InputAction.CallbackContext context)
    {
        if (!IsGamePlaying || animator == null) return;

        animator.SetTrigger(AttackHash);
    }

    private void FaceCameraDirection()
    {
        if (cameraTransform == null) return;

        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;

        if (camForward.sqrMagnitude < 0.0001f) return; // camera looking straight up/down

        transform.rotation = Quaternion.LookRotation(camForward.normalized);
    }

    private void MovePlayer(bool sprintHeld, bool jumpPressed)
    {
        IsMoving = moveInput.sqrMagnitude > 0.0001f;
        IsRunning = IsMoving && sprintHeld;
        currentSpeed = IsRunning ? speed * runSpeedMultiplier : speed;

        direction = Vector3.ClampMagnitude(transform.right * moveInput.x + transform.forward * moveInput.y, 1f);

        IsGrounded = characterController.isGrounded;
        if (IsGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        if (IsGrounded && jumpPressed)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animator != null) animator.SetTrigger(JumpHash);
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = (direction * currentSpeed) + new Vector3(0f, velocity.y, 0f);
        characterController.Move(finalMove * Time.deltaTime);
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetFloat(XaxisHash, moveInput.x);
        animator.SetFloat(YaxisHash, moveInput.y);
        animator.SetFloat(SpeedHash, IsMoving ? currentSpeed : 0f);
        animator.SetBool(IsWalkingHash, IsMoving);
        animator.SetBool(IsRunningHash, IsRunning);
    }


    public void Teleport(Vector3 position)
    {
        characterController.enabled = false;
        transform.position = position;
        velocity = Vector3.zero;
        characterController.enabled = true;
    }
}