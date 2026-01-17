using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] public Animator animator;
    [SerializeField] public GameObject Enemy;
    private float speed = -2f;
    private float currentSpeed;
    private bool isMoving;



    private void Awake()
    {
        //animator = GetComponent<Animator>();
        //speed = 5f;
        currentSpeed = speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy trigger"))
        {
            speed = -(speed);

            if (speed > 0)
            {
                animator.SetBool("Left", true);
            }
            else if (speed < 0)
            {
                animator.SetBool("Left", false);
            }

        }

        
        if (other.CompareTag("Player"))
        {
           
            currentSpeed = 0;
            animator.SetBool("isMoving", false);

        }
        else
        {
            currentSpeed = speed;
            animator.SetBool("isMoving", true);
            //animator.SetBool("isInRange", false);

        }

    }



    

    public void Attack()
    {

    }

    public void MoveEnemy()
    {
        if (isMoving)
        {
            Enemy.transform.position += new Vector3(currentSpeed , 0, 0)* Time.deltaTime;
        }
        
    }

    public void MoveCheck()
    {
        if (speed != 0)
        {
            isMoving = true;
        }
        else if (speed == 0)
        {
            isMoving = false;
        }
    }
    private void Update()
    {
        MoveCheck();
        MoveEnemy();
        



        
    }

}
