using UnityEngine;
using static UnityEditor.Progress;

public class ownedData : MonoBehaviour
{
    public static int amount;

    public static string weapon = "unowned";
    public static int greCount;
    public static int smoCount;

    private void Update()
    {
        if (greCount <0 || greCount > 5)
        {
            if (greCount < 0)
            {
                greCount = 0;
                Debug.Log("Empty");
            }
            else if (greCount >5)
            {
                greCount = 5;
                Debug.Log("Full");
            }
        }

        if (smoCount < 0 || smoCount > 5)
        {
            if (smoCount < 0)
            {
                smoCount = 0;
                Debug.Log("Empty");
            }
            else if (greCount > 5)
            {
                smoCount = 5;
                Debug.Log("Full");
            }
        }
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
            amount -= 500;
            smoCount++;
        }
        if (item == "Grenades")
        {
            amount -= 700;
           greCount++;
        }

    }

    public void sellItems(string item)
    {
        if (item == "Weapons") { weapon = "Unowned"; }
        if (item == "Smokes") { smoCount--; }
        if (item == "Grenades") { greCount--; }
    }

}
