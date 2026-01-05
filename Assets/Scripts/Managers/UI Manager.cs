using UnityEngine;

public class UIManager : MonoBehaviour
{

    private static UIManager instance = null;

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
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this && instance != null)
        {
            Destroy(this.gameObject);
        }
    }
}
