using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] public Animator animator;
    [SerializeField] public GameObject Enemy;
    private float speed = 2f;



    private void Awake()
    {
        //animator = GetComponent<Animator>();
        //speed = 5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy trigger"))
        {
            speed = -(speed);
        }
    }

    private void Update()
    {
        Enemy.transform.position += new Vector3(speed , 0, 0)* Time.deltaTime;



        if (speed > 0)
        {
            animator.SetBool("Left", true);
        }
        else if (speed < 0)
        {
            animator.SetBool("Left", false);
        }
    }

}
