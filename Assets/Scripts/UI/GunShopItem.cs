﻿using Sirenix.OdinInspector;
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
    private Vector2 imageMaxDimensions;
    [SerializeField]
    private TextMeshProUGUI itemName;
    [SerializeField]
    private GameObject buyButton;
    [SerializeField]
    private TextMeshProUGUI costText;
    [SerializeField]
    private GameObject equipButton;
    [SerializeField]
    private AudioClip cancelAudio;
    [SerializeField]
    private AudioClip pressAudio;
    
    private EquipGunDialog _equipGunDialog;
    private InfoDialog _infoDialog;
    private ShopManager _shopManager;
    private AudioSource _audioSource;

    private void Awake() {
        _shopManager = ReferenceManager.Inst.ShopManager;
        _equipGunDialog = ReferenceManager.Inst.EquipGunDialog;
        _infoDialog = ReferenceManager.Inst.InfoDialog;
        _audioSource = ReferenceManager.Inst.SfxAudio;
        
        _shopManager.OnGunShopItemUpdateUI += UpdateUI;
    }

    private void Start() {
        image.sprite = item.gun.image;
        image.SetNativeSize();
        Vector2 factor = new Vector2(imageMaxDimensions.x / image.rectTransform.sizeDelta.x, imageMaxDimensions.y / image.rectTransform.sizeDelta.y);
        image.rectTransform.sizeDelta *= Mathf.Min(factor.x, factor.y);
        itemName.text = item.displayName;
        costText.text = item.cost.ToString();
    }

    private void UpdateUI() {
        buyButton.SetActive(!_shopManager.IsOwned(item));
        costText.gameObject.SetActive(buyButton.activeSelf);
        equipButton.SetActive(_shopManager.IsOwned(item) && !_shopManager.IsEquipped(item.gun));
    }

    public void Buy() {
        if (!_shopManager.TryPurchaseGun(item)) {
            _audioSource.PlayOneShot(cancelAudio);
            return;
        }
        _audioSource.PlayOneShot(pressAudio);
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
