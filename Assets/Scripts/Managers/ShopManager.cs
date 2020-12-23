﻿using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : SerializedMonoBehaviour, ISaveLoad
{
    [SerializeField]
    private int equippableGuns;
    [ShowInInspector, ReadOnly]
    private Dictionary<GunShopItemData, bool> _allGuns;
    [ShowInInspector, ReadOnly]
    private GunData[] _equippedGuns;
    [SerializeField]
    private Image[] equipSlots;

    public event Action OnGunShopItemUpdateUI;
    
    private void Start() {
        OnGunShopItemUpdateUI?.Invoke();
        for (int i = 0; i < equippableGuns; i++)
            equipSlots[i].sprite = _equippedGuns[i]?.image;
    }

    public bool TryPurchaseGun(GunShopItemData gun) {
        if (_allGuns[gun])
            return true;
        if (!ReferenceManager.Inst.CurrencyManager.TrySubtractCurrency(gun.cost))
            return false;
        _allGuns[gun] = true;
        OnGunShopItemUpdateUI?.Invoke();
        return true;
    }

    public void EquipSlot(GunData gun, int index) {
        if(index >= equippableGuns) return;
        _equippedGuns[index] = gun;
        equipSlots[index].sprite = gun.image;
        OnGunShopItemUpdateUI?.Invoke();
    }

    public bool IsOwned(GunShopItemData gun) {
        return _allGuns[gun];
    }

    public bool IsEquipped(GunData gun) {
        for(int i = 0; i < equippableGuns; i++)
            if (_equippedGuns[i] == gun)
                return true;
        return false;
    }


    public void Save() {
        throw new System.NotImplementedException();
    }

    public void Load() {
        _equippedGuns = ReferenceManager.Inst.ProgressManager.Data.EquippedGuns;
        _allGuns = ReferenceManager.Inst.ProgressManager.Data.AllGuns;
    }
}