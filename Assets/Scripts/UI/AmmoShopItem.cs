﻿using Sirenix.OdinInspector;
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
    private TextMeshProUGUI itemNameShadow;
    [SerializeField]
    private Button buyButton;
    [SerializeField]
    private TextMeshProUGUI costText;
    [SerializeField]
    private TextMeshProUGUI costTextShadow;

    private int _purchasableAmmo;
    private ProgressManager _progressManager;
    private InfoDialog _infoDialog;

    private void Awake() {
        _progressManager = ReferenceManager.Inst.ProgressManager;
        _infoDialog = ReferenceManager.Inst.InfoDialog;
    }

    private void Start() {
        image.sprite = item.ammo.sprite;
        image.SetNativeSize();
        Vector2 factor = new Vector2(imageMaxDimensions.x / image.rectTransform.sizeDelta.x, imageMaxDimensions.y / image.rectTransform.sizeDelta.y);
        image.rectTransform.sizeDelta *= Mathf.Min(factor.x, factor.y);
        itemName.text = item.displayName;
        itemNameShadow.text = itemName.text;
        UpdateUI();
    }

    private void UpdateUI() {
        _purchasableAmmo =
            Mathf.Min(_progressManager.Data.Ammo[item.ammo.type].MaxAmmo - _progressManager.Data.Ammo[item.ammo.type].CurrentAmmo,
                      item.maxPurchaseAmount);
        costText.text = (item.costPerAmmo * _purchasableAmmo).ToString();
        costTextShadow.text = costText.text;
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