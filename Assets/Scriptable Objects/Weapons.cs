using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "ScriptableObjects/WeaponSO")]

public class Weapons : ScriptableObject 
{
    public int totalInventory;
    public int weaponCost;
    public int smokeCost;
    public int grenadeCost;
}
