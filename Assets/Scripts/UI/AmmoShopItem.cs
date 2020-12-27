using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoShopItem : MonoBehaviour
{
    [SerializeField, InlineEditor]
    private AmmoShopItemData item;
    [SerializeField]
    private Image image;
    [SerializeField]
    private TextMeshProUGUI itemName;
    [SerializeField]
    private Button buyButton;
    [SerializeField]
    private TextMeshProUGUI buyButtonText;

    private int _purchasableAmmo;
    private ProgressManager _progressManager;
    private InfoDialog _infoDialog;

    private void Awake() {
        _progressManager = ReferenceManager.Inst.ProgressManager;
        _infoDialog = ReferenceManager.Inst.InfoDialog;
    }

    private void Start() {
        image.sprite = item.ammo.sprite;
        itemName.text = item.displayName;
        UpdateUI();
    }

    private void UpdateUI() {
        _purchasableAmmo =
            Mathf.Min(_progressManager.Data.Ammo[item.ammo.type].MaxAmmo - _progressManager.Data.Ammo[item.ammo.type].CurrentAmmo,
                      item.maxPurchaseAmount);
        buyButtonText.text = $"Buy ({item.costPerAmmo * _purchasableAmmo})";
        buyButton.interactable = _purchasableAmmo > 0;
    }

    public void Buy() {
        if (!ReferenceManager.Inst.CurrencyManager.TrySubtractCurrency(_purchasableAmmo * item.costPerAmmo)) return;
        _progressManager.Data.Ammo[item.ammo.type].CurrentAmmo += _purchasableAmmo;
        UpdateUI();
    }

    public void Info() {
        _infoDialog.Show(item.displayName, item.GetDescription());
    }
}