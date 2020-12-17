using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooter : Shooter
{
    [SerializeField, InlineEditor, PropertyOrder(0f)]
    private GunData[] guns;
    [SerializeField]
    private int defaultGun;
    [SerializeField]
    private TextMeshProUGUI ammoText;
    [SerializeField]
    private Image reloadProgress;
    [ShowInInspector]
    private bool test;
    [SerializeField, ShowIf("test")]
    private int[] _ammo;
    private int _currentGun;
    private AmmoData[] _ammoData;

    protected override void Start() {
        if (_ammo == null || _ammo.Length < Enum.GetNames(typeof(AmmoType)).Length)
            _ammo = new int[guns.Length];
        
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
        ammoText.text = $"{AmmoData.RemainingAmmo}/{(Gun.useAmmo ? _ammo[(int) Gun.ammoType].ToString() : "\u221E")}";
    }

    public void Reload() {
        if(AmmoData.RemainingAmmo == Gun.clipSize) return;
        int index = (int) Gun.ammoType;
        AmmoData.Reload(_ammo[index] >= Gun.clipSize ? -1 : _ammo[index]);
        _ammo[index] = Mathf.Max(_ammo[index] - Gun.clipSize, 0);
    }

    public void ChangeGun(int to) {
        _ammoData[_currentGun] = AmmoData;
        Gun = guns[to];
        AmmoData = _ammoData[to];
        _currentGun = to;
    }
    
}