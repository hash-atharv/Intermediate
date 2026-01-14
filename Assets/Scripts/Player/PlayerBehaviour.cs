using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{

    public PlayerControls controls;


    private void Awake()
    {
        controls = new PlayerControls();
        //enemyData.ex();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            controls.Gameplay.Fire.performed += Damager;
        }
    }

    private void Damager(InputAction.CallbackContext context)
    {
        
    }

}
