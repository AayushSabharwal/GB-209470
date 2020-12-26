using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradableShopItem : MonoBehaviour
{
    [SerializeField, InlineEditor]
    private UpgradableShopItemData item;
    [SerializeField]
    private Button upgradeButton;
    [SerializeField]
    private TextMeshProUGUI upgradeButtonText;
    [SerializeField]
    private Image image;
    [SerializeField]
    private TextMeshProUGUI titleText;

    private void Start() {
        UpdateUI();
    }

    private void UpdateUI() {
        image.sprite = item.sprite;
        upgradeButton.interactable = item.IsUpgradable;
        titleText.text = item.itemName;
        if (item.IsUpgradable)
            upgradeButtonText.text = $"Upgrade ({item.effectiveness[item.Level + 1].cost})";
    }

    public void Upgrade() {
        item.Upgrade();
        UpdateUI();
    }
}
