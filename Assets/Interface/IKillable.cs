using UnityEngine;
using UnityEngine.InputSystem;

public interface IKillable 
{
    public void TakeDamage(InputAction.CallbackContext context);
}
