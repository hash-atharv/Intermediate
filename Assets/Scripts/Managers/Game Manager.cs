using UnityEngine;

public class GameManager : MonoBehaviour
{
   private static GameManager instance = null;


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


    public void EnemyKilled()
    {
        kills ++;
        Debug.Log($"Kills:{kills}");
    }

}
