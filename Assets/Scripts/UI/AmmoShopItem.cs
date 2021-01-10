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
    private Vector2 imageMaxDimensions;
    [SerializeField]
    private TextMeshProUGUI itemName;
    [SerializeField]
    private Button buyButton;
    [SerializeField]
    private TextMeshProUGUI costText;
    [SerializeField]
    private TextMeshProUGUI ammoCounterText;
    [SerializeField]
    private AudioClip cancelAudio;
    [SerializeField]
    private AudioClip pressAudio;

    private int _purchasableAmmo;
    private ProgressManager _progressManager;
    private InfoDialog _infoDialog;
    private AudioSource _audioSource;

    private void Awake() {
        _progressManager = ReferenceManager.Inst.ProgressManager;
        _infoDialog = ReferenceManager.Inst.InfoDialog;
        _audioSource = ReferenceManager.Inst.SfxAudio;
    }

    private void Start() {
        image.sprite = item.ammo.sprite;
        image.SetNativeSize();
        Vector2 factor = new Vector2(imageMaxDimensions.x / image.rectTransform.sizeDelta.x, imageMaxDimensions.y / image.rectTransform.sizeDelta.y);
        image.rectTransform.sizeDelta *= Mathf.Min(factor.x, factor.y);
        itemName.text = item.displayName;
        UpdateUI();
    }

    private void UpdateUI() {
        _purchasableAmmo =
            Mathf.Min(_progressManager.Data.Ammo[item.ammo.type].MaxAmmo - _progressManager.Data.Ammo[item.ammo.type].CurrentAmmo,
                      item.maxPurchaseAmount);
        costText.text = (item.costPerAmmo * _purchasableAmmo).ToString();
        buyButton.interactable = _purchasableAmmo > 0;
        ammoCounterText.text = $"You have: {_progressManager.Data.Ammo[item.ammo.type].CurrentAmmo}";
    }

    public void Buy() {
        if (!ReferenceManager.Inst.CurrencyManager.TrySubtractCurrency(_purchasableAmmo * item.costPerAmmo)) {
            _audioSource.PlayOneShot(cancelAudio);
            return;
        }
        _audioSource.PlayOneShot(pressAudio);
        _progressManager.Data.Ammo[item.ammo.type].CurrentAmmo += _purchasableAmmo;
        UpdateUI();
    }

    public void Info() {
        _infoDialog.Show(item.displayName, item.GetDescription());
    }
}