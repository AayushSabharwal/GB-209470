using System;
using System.Globalization;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop Item/Upgradable")]
public class UpgradableShopItemData : ScriptableObject, ISaveLoad
{
    [PreviewField, HorizontalGroup("H"), LabelWidth(80f)]
    public Sprite sprite;
    [VerticalGroup("H/V"), LabelWidth(80f)]
    public string itemName;
    [NonSerialized, VerticalGroup("H/V"), LabelWidth(80f), ReadOnly, ShowInInspector]
    public int Level;
    [SerializeField, Multiline]
    private string description;
    public UpgradeLevel[] effectiveness;

    public bool IsUpgradable => Level < effectiveness.Length - 1;

    public string GetDescription() {
        return description.Replace("%CEFF%", $"Current: {effectiveness[Level].effectiveness}")
                          .Replace("%NEFF%", IsUpgradable ? 
                                       $"Next: {effectiveness[Level+1].effectiveness}" : 
                                       string.Empty);
    }
    
    public bool Upgrade() {
        if (!IsUpgradable ||
            !ReferenceManager.Inst.CurrencyManager.TrySubtractCurrency(effectiveness[Level + 1].cost)) return false;
        Level++;
        return true;
    }


    public void Save() {
        ReferenceManager.Inst.ProgressManager.Data.UpgradableItemLevels[itemName] = Level;
    }

    public void Load() {
        Level = ReferenceManager.Inst.ProgressManager.Data.UpgradableItemLevels[itemName];
    }
}

[Serializable]
public struct UpgradeLevel
{
    [HorizontalGroup("A"), LabelWidth(80f)]
    public float effectiveness;
    [HorizontalGroup("A"), LabelWidth(80f)]
    public int cost;
}