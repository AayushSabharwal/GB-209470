﻿using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooter : Shooter, ISaveLoad
{
    [SerializeField, InlineEditor, PropertyOrder(0f)]
    private GunData[] guns;
    [SerializeField]
    private int defaultGun;
    [SerializeField]
    private TextMeshProUGUI ammoText;
    [SerializeField]
    private Image reloadProgress;
    [SerializeField, ValidateInput("@ammo != null && ammo.Count == System.Enum.GetNames(typeof(AmmoType)).Length", "Account for all AmmoType")]
    private Dictionary<AmmoType, AmmoTracker> ammo;
    
    private int _currentGun;
    private AmmoData[] _ammoData;

    protected override void Start() {
        _currentGun = defaultGun;
        _ammoData = new AmmoData[guns.Length];
        for (int i = 0; i < guns.Length; i++)
            _ammoData[i] = new AmmoData(guns[i].clipSize, guns[i].reloadTime);
        Gun = guns[defaultGun];
        AmmoData = _ammoData[defaultGun];

        UpdateUI();
        OnShoot += (_, __) => UpdateUI();
        OnShoot += (_, __) => HandleAmmo();
    }

    protected override void Update() {
        base.Update();
        if (AmmoData.ReloadTimer >= 0f) reloadProgress.fillAmount = AmmoData.ReloadTimer / Gun.reloadTime;
    }

    private void HandleAmmo() {
        if (AmmoData.RemainingAmmo > 0)
            return;
        Reload();
    }

    private void UpdateUI() {
        ammoText.text = $"{AmmoData.RemainingAmmo}/{(Gun.useAmmo ? ammo[Gun.ammoType].CurrentAmmo.ToString() : "\u221E")}";
    }

    public void Reload() {
        if (AmmoData.RemainingAmmo == Gun.clipSize || ammo[Gun.ammoType].CurrentAmmo == 0) return;
        
        // TODO: Visual notification for being unable to reload
        
        AmmoData.Reload(ammo[Gun.ammoType].CurrentAmmo >= Gun.clipSize ? -1 : ammo[Gun.ammoType].CurrentAmmo);
        ammo[Gun.ammoType].CurrentAmmo = Mathf.Max(ammo[Gun.ammoType].CurrentAmmo - Gun.clipSize, 0);
    }

    public void ChangeGun(int to) {
        _ammoData[_currentGun] = AmmoData;
        Gun = guns[to];
        AmmoData = _ammoData[to];
        _currentGun = to;
    }

    public void AddAmmo(AmmoType type, int amount) {
        ammo[type].CurrentAmmo = Mathf.Min(ammo[type].CurrentAmmo + amount, ammo[type].MaxAmmo);
    }

    public void Save() {
        
    }

    public void Load() {
        
    }
}

public class AmmoTracker
{
    public int CurrentAmmo;
    public readonly int MaxAmmo;
}