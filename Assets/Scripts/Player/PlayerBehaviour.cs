using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour, IKillable
{

    public PlayerControls controls;
    [SerializeField] public EnemyData enemyData;


    public float damage = 50 ;
    private void Awake()
    {
        controls = new PlayerControls();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {

            controls.Gameplay.Fire.performed += TakeDamage;
        }
    }

    public void TakeDamage(InputAction.CallbackContext context)
    {
        enemyData.healthControl(damage);

    }
    
}
