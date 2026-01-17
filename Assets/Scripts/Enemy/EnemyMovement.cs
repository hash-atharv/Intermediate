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
        
        currentSpeed = speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy trigger"))
        {
            speed = -(speed);  // to reverse the enemy direction

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
           // booelean to stop the enemy when player is close
            currentSpeed = 0;
            animator.SetBool("isMoving", false);

        }
        else
        {
            currentSpeed = speed;
            animator.SetBool("isMoving", true);

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {   // To move the enemy again after player not in range
            currentSpeed = speed;
            animator.SetBool("isMoving", true);
        }
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
