using UnityEngine;
using static UnityEditor.Progress;

public class ownedData : MonoBehaviour
{
    private Stack Weapons;
    private Stack Smokes;
    private Stack Grenades;
     


    private void Update()
    {
        
    }

    public void buyItems(string item)
    {
        if (item == "Weapons") { Weapons.push(); }
        if (item == "Smokes")
        {
            Smokes.push();
            Collectibles.smoCount++;
        }
        if (item == "Grenades")
        { 
            Grenades.push();
            Collectibles.greCount++;
        }

    }

    public void sellItems(string item)
    {
        if (item == "Weapons") { Weapons.pop(); }
        if (item == "Smokes") { Smokes.pop(); }
        if (item == "Grenades") { Grenades.pop(); }
    }

}
