using UnityEngine;

[CreateAssetMenu(menuName = "Shop Item/Gun")]
public class GunShopItemData : ScriptableObject, ISerializeReferenceByAssetGuid
{
    public string displayName;
    public int cost;
    public GunData gun;
}
