using UnityEngine;

[CreateAssetMenu(menuName = "Shop Item/Ammo")]
public class AmmoShopItemData : ScriptableObject
{
    public string displayName;
    public int maxPurchaseAmount;
    public int costPerAmmo;
    public AmmoDropData ammo;
}
