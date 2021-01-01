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
    private TextMeshProUGUI costText;
    [SerializeField]
    private Image image;
    [SerializeField]
    private TextMeshProUGUI titleText;

    private InfoDialog _infoDialog;

    private void Awake() {
        _infoDialog = ReferenceManager.Inst.InfoDialog;
    }

    private void Start() {
        UpdateUI();
    }

    private void UpdateUI() {
        image.sprite = item.sprite;
        upgradeButton.interactable = item.IsUpgradable;
        titleText.text = item.itemName;
        if (item.IsUpgradable)
            costText.text = item.effectiveness[item.Level + 1].cost.ToString();
    }

    public void Upgrade() {
        item.Upgrade();
        UpdateUI();
    }

    public void Info() {
        _infoDialog.Show(item.itemName, item.GetDescription());
    }
}