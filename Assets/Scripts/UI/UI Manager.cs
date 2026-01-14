using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    
    public PlayerControls controls;
    private static UIManager instance = null;

    public GameObject uiGameObject;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject temp = new GameObject();
                instance = temp.AddComponent<UIManager>();
            }
            return instance;
        }

    }

    private void Awake()
    {
        controls = new PlayerControls();

       
        controls.Gameplay.UIcontrol.performed += ctx => ToggleTarget();


        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this && instance != null)
        {
            Destroy(this.gameObject);
        }
    }


    private void ToggleTarget()
    {
        if (uiGameObject != null)
        {
            bool isActive = uiGameObject.activeSelf;
            uiGameObject.SetActive(!isActive);
        }
    }

    private void OnEnable() => controls.Gameplay.Enable();
    private void OnDisable() => controls.Gameplay.Disable();

    private void Update()
    {
        


       
    }
}
