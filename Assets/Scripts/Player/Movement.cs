using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;


public class Movement : MonoBehaviour
{
    [SerializeField] Animator animator;

    public PlayerControls controls;
    public Vector2 moveInput;
    public Vector3 direction;
    public Vector3 velocity;
    public float gravity = -9.81f;

    private CharacterController characterController;

    public bool isMoving = false;
    public bool isGrounded ;
    private Vector3 prevMoveData;
    [SerializeField] private float speed = 5f;

    private float cameraXRotation;   // tracks vertical camera tilt
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] Transform cameraTransform;

    void Awake()
    {
        controls = new PlayerControls();
        characterController = GetComponent<CharacterController>();
        controls.Gameplay.Fire.performed += AttackAnim;

    }

    private void OnEnable() => controls.Gameplay.Enable();
    private void OnDisable() => controls.Gameplay.Disable();

    void Update()
    {
        //For Gravity
        //isGrounded = characterController.isGrounded;
        //if (isGrounded && velocity.y < 0f)
        //{
        //    velocity.y = -2f;
        //}
        //velocity.y += gravity * Time.deltaTime;
        //characterController.Move(velocity * Time.deltaTime);





        MouseInput();


        // For movement input
        moveInput = controls.Gameplay.Move.ReadValue<Vector2>();  
        MovePlayer();
        if (isMoving)
        {
            animator.SetFloat("speed", speed);
            animator.SetBool("isWalking", true);
        }
        else if (!isMoving)
        {
            animator.SetFloat("speed", 0);
            animator.SetBool("isWalking", false);
        }
    }

    private void AttackAnim(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Attack");
    }

    void MovePlayer()
    {
        // Build direction relative to where the player is facing
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        characterController.Move(move * speed * Time.deltaTime);
        Moving(move);

        animator.SetFloat("Xaxis", moveInput.x);
        animator.SetFloat("Yaxis", moveInput.y);
    }
    void MouseInput()
    {
        // 1. Get the mouse input 
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 2. Rotate the player body left and right
        transform.Rotate(0, mouseX, 0);

        // 3. Calculate the up and down camera tilt
        cameraXRotation -= mouseY;
        cameraXRotation = Mathf.Clamp(cameraXRotation, -80f, 80f);

        // 4. Apply the tilt to the camera
        cameraTransform.localEulerAngles = new Vector3(cameraXRotation, 0f, 0f);
    }

    private void Moving(Vector3 move)
    {
        // for maintaining the isMoving boolean 
        if (prevMoveData == null)
        { prevMoveData = move; }

        if (prevMoveData != move) { isMoving = true; }
        else if(prevMoveData == move) { isMoving = false; }
    }

}
