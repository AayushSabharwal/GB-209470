using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
    [OdinSerialize,
     ValidateInput("@ammo != null && ammo.Count == System.Enum.GetNames(typeof(AmmoType)).Length",
                   "Account for all AmmoType")]
    private Dictionary<AmmoType, AmmoTracker> ammo;

    private int _currentGun;
    private AmmoData[] _ammoData;

    protected override void Start() {
        base.Start();
        _currentGun = defaultGun;
        _ammoData = new AmmoData[guns.Length];
        for (int i = 0; i < guns.Length; i++)
            _ammoData[i] = new AmmoData(guns[i].clipSize, guns[i].reloadTime, guns[i].isInfiniteAmmo);
        gun = guns[defaultGun];
        AmmoData = _ammoData[defaultGun];

        UpdateUI();
    }
    protected override void Update() {
        if (IsPaused || IsPlayerDead) return;
        
        base.Update();
        if (AmmoData.ReloadTimer >= 0f) reloadProgress.fillAmount = AmmoData.ReloadTimer / gun.reloadTime;
    }

    private void HandleAmmo() {
        if (AmmoData.RemainingAmmo > 0)
            return;
        Reload();
    }

    public override void Shoot() {
        base.Shoot();
        UpdateUI();
        HandleAmmo();
    }

    private void UpdateUI() {
        ammoText.text =
            $"{AmmoData.RemainingAmmo}/{(!gun.isInfiniteAmmo ? ammo[gun.ammoType].CurrentAmmo.ToString() : "\u221E")}";
    }

    public void Reload() {
        if (AmmoData.RemainingAmmo == gun.clipSize || ammo[gun.ammoType].CurrentAmmo == 0) return;

        // TODO: Visual notification for being unable to reload

        AmmoData.Reload(ammo[gun.ammoType].CurrentAmmo >= gun.clipSize ? -1 : ammo[gun.ammoType].CurrentAmmo);
        ammo[gun.ammoType].CurrentAmmo = Mathf.Max(ammo[gun.ammoType].CurrentAmmo - gun.clipSize, 0);
    }

    public void ChangeGun(int to) {
        _ammoData[_currentGun] = AmmoData;
        gun = guns[to];
        AmmoData = _ammoData[to];
        _currentGun = to;
    }

    public void AddAmmo(AmmoType type, int amount) {
        ammo[type].CurrentAmmo = Mathf.Min(ammo[type].CurrentAmmo + amount, ammo[type].MaxAmmo);
    }

    public void Save() {
        foreach (KeyValuePair<AmmoType, AmmoTracker> kvp in ammo)
            ReferenceManager.Inst.ProgressManager.Data.Ammo[kvp.Key] = kvp.Value.CurrentAmmo;
    }

    public void Load() {
        foreach (KeyValuePair<AmmoType, int> kvp in ReferenceManager.Inst.ProgressManager.Data.Ammo)
            ammo[kvp.Key] = new AmmoTracker(kvp.Value, ammo[kvp.Key].MaxAmmo);
    }
}

public class AmmoTracker
{
    public int CurrentAmmo;
    public readonly int MaxAmmo;

    public AmmoTracker(int currentAmmo, int maxAmmo) {
        CurrentAmmo = currentAmmo;
        MaxAmmo = maxAmmo;
    }
}