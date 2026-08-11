using UnityEngine;

public class GameManager : MonoBehaviour
{
   private static GameManager instance = null;
    public GameObject inventory;


    private int kills = 0;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject temp = new GameObject();
                instance = temp.AddComponent<GameManager>();
            }
            return instance;
        }

    }

    private void Awake()
    {

        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this && instance != null)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        inventory.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (inventory.activeSelf)
            {
                inventory.SetActive(true);
            }
            else
                inventory.SetActive(false);
        }
    }
    public void EnemyKilled()
    {
        kills ++;
        Debug.Log($"Kills:{kills}");
    }

}
