using UnityEngine;
using static UnityEditor.Progress;

public class ownedData : MonoBehaviour
{
    public static int amount;

    public static string weapon;
    public static int greCount;
    public static int smoCount;


    private void Start()
    {
        weapon = "unowned";
    }
    private void Update()
    {
      
    }

    public void buyItems(string item)
    {
        if (item == "Weapons")
        { 
            
            weapon = "Owned";
            amount -= 1500;
        }
        if (item == "Smokes")
        {
            if (smoCount >= 5)
            {
                Debug.Log("Full");
            }
            else if (amount > 500)
            {
                amount -= 500;
                smoCount++;
            }

        }
        if (item == "Grenades")
        {
            if (greCount >= 5)
            {
                greCount = 5;
                Debug.Log("Full");
            }
            else if (amount > 700)
            {
                amount -= 700;
                greCount++;
            }
        }

    }

    public void sellItems(string item)
    {
        if (item == "Weapons")
        {
            weapon = "Unowned";
            amount += 1500;
        }
        if (item == "Smokes")
        {
            if (smoCount <= 0)
            {
                Debug.Log("Empty");
            }
            else
            {
                smoCount--;
                amount += 500;
            }
            

        }
        if (item == "Grenades")
        {
            if (greCount <= 0)
            {
                Debug.Log("Empty");
            }
            else
            {
                greCount--;
                amount += 700;
            }
        }
    }

}
