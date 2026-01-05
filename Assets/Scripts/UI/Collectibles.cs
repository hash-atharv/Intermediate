using UnityEngine;
using TMPro;

public class Collectibles : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro ownedGrenades;
    [SerializeField] private TextMeshPro ownedSmokes;
    [SerializeField] private TextMeshPro amountText;
    public static int greCount;
    public static int smoCount;
    
    

    private int amount;

    private void Start()
    {
        amount = 3000;
        greCount = 0;
        smoCount = 0;
    }


    private void Update()
    {
        amountText.text = $"Amount:{amount}";
        ownedGrenades.text = $"Grenades:{greCount}";
        ownedSmokes.text = $"Smokes:{smoCount}";


    }
}
