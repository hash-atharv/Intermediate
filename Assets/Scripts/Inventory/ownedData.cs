using UnityEngine;
using static UnityEditor.Progress;

public class ownedData : MonoBehaviour
{
    public static int amount;
    private bool isWeapon;
    public static string weapon;
    public static int greCount;
    public static int smoCount;
    public Weapons weaponSO;

    private void Start()
    {
        weapon = "unowned";
        isWeapon = false;
    }
    private void Update()
    {
      
    }

    public void buyItems(string item)
    {
        if (item == "Weapons")
        {
            if (isWeapon == false)
            {
                if (amount >= weaponSO.weaponCost)
                {
                    weapon = "Owned";
                    amount -= weaponSO.weaponCost;
                    isWeapon = true;
                }
            }
        }
        if (item == "Smokes")
        {
            if (smoCount >= 5)
            {
                Debug.Log("Full");
            }
            else if (amount > weaponSO.smokeCost)
            {
                amount -= weaponSO.smokeCost;
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
            else if (amount > weaponSO.grenadeCost)
            {
                amount -= weaponSO.grenadeCost;
                greCount++;
            }
        }

    }

    public void sellItems(string item)
    {
        if (item == "Weapons")
        {
            if(isWeapon == true)
            {
                if (weapon == "Owned")
                {
                    weapon = "Unowned";
                    amount += weaponSO.weaponCost;
                    isWeapon = false;
                }
            }

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
                amount += weaponSO.smokeCost;
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
                amount += weaponSO.grenadeCost;
            }
        }
    }

}
