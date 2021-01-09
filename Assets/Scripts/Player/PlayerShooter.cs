using System;
using System.Collections.Generic;
using DG.Tweening;
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
    private TextMeshProUGUI shadowText;
    [SerializeField, FoldoutGroup("GUI")]
    private Image reloadProgress;
    [SerializeField, FoldoutGroup("GUI")]
    private Transform ammoTracker;
    [SerializeField, FoldoutGroup("GUI")]
    private float punchForce = 3f;
    [SerializeField, FoldoutGroup("GUI")]
    private float punchDuration = 0.4f;
    [SerializeField, InlineEditor, ValidateInput("@enrage != null && enrage.itemName == \"Enrage\"")]
    private UpgradableShopItemData enrage;
    [SerializeField, PropertyRange(0f, 1f)]
    private float enrageThreshold;

    private Dictionary<AmmoType, AmmoTracker> _ammo;
    private int _currentGun;
    private PlayerHealth _health;
    private AmmoData[] _ammoData;
    private bool _canTween;

    protected override void Start() {
        base.Start();
        _health = GetComponent<PlayerHealth>();
        _currentGun = defaultGun;
        _ammoData = new AmmoData[guns.Length];
        _canTween = true;
        for (int i = 0; i < guns.Length; i++) {
            if (guns[i] == null) continue;

            _ammoData[i] =
                new
                    AmmoData(_ammo[guns[i].ammoType].CurrentAmmo >= guns[i].clipSize ? guns[i].clipSize : _ammo[guns[i].ammoType].CurrentAmmo,
                             guns[i].reloadTime,
                             guns[i].isInfiniteAmmo);
            _ammoData[i].OnReload += UpdateUI;
            _ammo[guns[i].ammoType].CurrentAmmo -= _ammoData[i].RemainingAmmo;
        }

        gun = guns[defaultGun];
        AmmoData = _ammoData[defaultGun];
        ReferenceManager.Inst.EnemySpawner.OnLevelEnd += OnLevelEnd;
        UpdateUI();
    }

    private void OnLevelEnd() {
        IsPaused = true;
    }

    protected override void Update() {
        if (IsPaused || IsPlayerDead) return;

        base.Update();
        if (AmmoData.ReloadTimer >= 0f) reloadProgress.fillAmount = 1f - AmmoData.ReloadTimer / gun.reloadTime;
    }

    private void HandleAmmo() {
        if (AmmoData.RemainingAmmo > 0 || AmmoData.ReloadTimer > 0f || AmmoData.ReloadTimer < 0f)
            return;
        Reload();
    }

    public override void Shoot() {
        if (!AmmoData.CanShoot)
            return;
        base.Shoot();
        reloadProgress.fillAmount = AmmoData.RemainingAmmo / (float) gun.clipSize;
        UpdateUI();
        HandleAmmo();
    }

    protected override void ApplyBulletAttributes(Bullet bullet, BulletData bulletType) {
        base.ApplyBulletAttributes(bullet, bulletType);
        if (_health.CurHp / _health.MaxHp <= enrageThreshold)
            bullet.DamageMultiplier = enrage.effectiveness[enrage.Level].effectiveness;
    }

    private void UpdateUI() {
        ammoText.text =
            $"{AmmoData.RemainingAmmo}/{(!gun.isInfiniteAmmo ? _ammo[gun.ammoType].CurrentAmmo.ToString() : "\u221E")}";
        shadowText.text = ammoText.text;
    }

    public void Reload() {
        if (AmmoData.RemainingAmmo == gun.clipSize || _ammo[gun.ammoType].CurrentAmmo == 0) {
            if (!_canTween) return;
            _canTween = false;
            ammoTracker.DOPunchPosition(Vector3.right * punchForce, punchDuration).onComplete += () => _canTween = true;

            return;
        }

        AmmoData.Reload(_ammo[gun.ammoType].CurrentAmmo >= gun.clipSize ? -1 : _ammo[gun.ammoType].CurrentAmmo);
        _ammo[gun.ammoType].CurrentAmmo =
            Mathf.Max(_ammo[gun.ammoType].CurrentAmmo - gun.clipSize + AmmoData.RemainingAmmo, 0);
        UpdateUI();
    }

    public void ChangeGun(int to) {
        if (to >= guns.Length || guns[to] == null)
            return;
        _ammoData[_currentGun] = AmmoData;
        gun = guns[to];
        AmmoData = _ammoData[to];
        _currentGun = to;
        SetupMuzzleFlash();
        UpdateUI();
    }

    public void AddAmmo(AmmoType type, int amount) {
        print($"BEFORE {_ammo[type].CurrentAmmo} {amount} {_ammo[type].MaxAmmo} {type.ToString()}");
        // _ammo[type].CurrentAmmo = Mathf.Min(_ammo[type].CurrentAmmo + amount, _ammo[type].MaxAmmo);
        _ammo[type].CurrentAmmo += amount;
        print($"AFTER {_ammo[type].CurrentAmmo}");
        if (_ammo[type].CurrentAmmo > _ammo[type].MaxAmmo)
            _ammo[type].CurrentAmmo = _ammo[type].MaxAmmo;
        UpdateUI();
    }

    public void Save() {
        for (int i = 0; i < guns.Length; i++)
            if (guns[i] != null)
                _ammo[guns[i].ammoType].CurrentAmmo += _ammoData[i].RemainingAmmo;

        ReferenceManager.Inst.ProgressManager.Data.Ammo = _ammo;
        ReferenceManager.Inst.ProgressManager.Data.EquippedGuns = guns;
    }

    public void Load() {
        _ammo = ReferenceManager.Inst.ProgressManager.Data.Ammo;
        guns = ReferenceManager.Inst.ProgressManager.Data.EquippedGuns;
    }
}

[Serializable]
public class AmmoTracker
{
    public int CurrentAmmo;
    public int MaxAmmo;

    public AmmoTracker(int currentAmmo, int maxAmmo) {
        CurrentAmmo = currentAmmo;
        MaxAmmo = maxAmmo;
    }
}