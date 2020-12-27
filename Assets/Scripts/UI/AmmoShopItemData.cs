using UnityEngine;

[CreateAssetMenu(menuName = "Shop Item/Ammo")]
public class AmmoShopItemData : ScriptableObject
{
    public string displayName;
    [SerializeField, Multiline]
    private string description;
    public int maxPurchaseAmount;
    public int costPerAmmo;
    public AmmoDropData ammo;

    public string GetDescription() {
        return description.Replace("%CPA%", costPerAmmo.ToString());
    }
}
