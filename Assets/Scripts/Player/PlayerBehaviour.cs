using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{

    public PlayerControls controls;
    //[SerializeField] public EnemyData enemyData;
    
    private Enemy currentEnemy;

    
    public float damage = 50f ;


    private void Awake()
    {
        controls = new PlayerControls();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            
            if (other.TryGetComponent<Enemy>(out var enemy))
            {
                currentEnemy = enemy;
               Debug.Log("Enemy in range!");
                //Debug.Log(currentEnemy);

                //controls.Gameplay.Fire.performed += Attack;

            }
        }
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Fire.performed += Attack;
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
        controls.Gameplay.Fire.performed -= Attack;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            currentEnemy= null;
            Debug.Log("Enemy out range!");
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        //enemyData.healthControl(damage);
        //EnemyMovement.health -= damage;
        //currentEnemy.health -= damage;
        //Debug.Log(currentEnemy.health);
        Debug.Log("yes");
        currentEnemy.TakeDamage(damage);

    }

}
