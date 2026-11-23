using UnityEngine;
using TMPro;

public class Collectibles : MonoBehaviour
{
    public static Collectibles Instance;
    [SerializeField] private TextMeshProUGUI ownedGrenades;
    [SerializeField] private TextMeshProUGUI ownedSmokes;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI weaponText;
    [SerializeField] private TextMeshProUGUI collectibleText;

    

    

    private void Start()
    {
        Instance = this;
        ownedData.amount = 3000;
        ownedData.smoCount = 0;
        ownedData.greCount = 0;
    }
    public void UpdateCollectibleText(int count)
    {
        collectibleText.text = $"Collection: {count}";
    }

    private void Update()
    {
        amountText.text = $"Amount:{ownedData.amount}";
        amountText.text = $"Amount:{ownedData.amount}";
        ownedGrenades.text = $"Grenades:{ownedData.greCount}";
        ownedSmokes.text = $"Smokes:{ownedData.smoCount}";
        weaponText.text = ownedData.weapon;

    }
}
