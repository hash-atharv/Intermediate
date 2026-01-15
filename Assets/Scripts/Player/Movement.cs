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
    Vector3 temporary;
    [SerializeField] private float speed = 5f;

    void Awake()
    {
        controls = new PlayerControls();
        characterController = GetComponent<CharacterController>();
        
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





        // Attack 
        controls.Gameplay.Fire.performed += Attack;


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

    private void Attack(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Attack");
    }

    void MovePlayer()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        
        characterController.Move(move * speed * Time.deltaTime);
        Moving(move);
        
        animator.SetFloat("Xaxis", moveInput.x);
        animator.SetFloat("Yaxis", moveInput.y);

        //direction= new Vector3(moveInput.x,0, moveInput.y);

    }

    

    private void Moving(Vector3 move)
    {
        if (temporary == null)
        { temporary = move; }

        if (temporary != move) { isMoving = true; }
        else if(temporary == move) { isMoving = false; }
    }

}
