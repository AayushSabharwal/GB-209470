using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunShopItem : MonoBehaviour
{
    [SerializeField, InlineEditor]
    private GunShopItemData item;
    [SerializeField]
    private Image image;
    [SerializeField]
    private TextMeshProUGUI itemName;
    [SerializeField]
    private GameObject buyButton;
    [SerializeField]
    private TextMeshProUGUI buyButtonText;
    [SerializeField]
    private GameObject equipButton;
    
    private EquipGunDialog _equipGunDialog;
    private InfoDialog _infoDialog;
    private ShopManager _shopManager;

    private void Awake() {
        _shopManager = ReferenceManager.Inst.ShopManager;
        _equipGunDialog = ReferenceManager.Inst.EquipGunDialog;
        _infoDialog = ReferenceManager.Inst.InfoDialog;
        
        _shopManager.OnGunShopItemUpdateUI += UpdateUI;
    }

    private void Start() {
        image.sprite = item.gun.image;
        itemName.text = item.displayName;
        buyButtonText.text = $"Buy ({item.cost})";
    }

    private void UpdateUI() {
        buyButton.SetActive(!_shopManager.IsOwned(item));
        equipButton.SetActive(!_shopManager.IsEquipped(item.gun));
    }

    public void Buy() {
        if (!_shopManager.TryPurchaseGun(item)) return;
        buyButton.SetActive(false);
        equipButton.SetActive(true);
    }

    public void Equip() {
        _equipGunDialog.Equip(item.gun);
        equipButton.SetActive(false);
    }

    public void Info() {
        _infoDialog.Show(item.displayName, item.GetDescription());
    }
}
