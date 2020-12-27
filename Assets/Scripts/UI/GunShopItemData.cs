using UnityEngine;

[CreateAssetMenu(menuName = "Shop Item/Gun")]
public class GunShopItemData : ScriptableObject, ISerializeReferenceByAssetGuid
{
    public string displayName;
    [SerializeField, Multiline]
    private string description;
    public int cost;
    public GunData gun;

    public string GetDescription() {
        return description.Replace("%AMMO%", gun.ammoType.ToString());
    }
}
