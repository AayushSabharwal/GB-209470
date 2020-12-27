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
    [SerializeField, InlineEditor, ValidateInput("@enrage != null && enrage.itemName == \"Enrage\"")]
    private UpgradableShopItemData enrage;
    [SerializeField, PropertyRange(0f, 1f)]
    private float enrageThreshold;

    private int _currentGun;
    private PlayerHealth _health;
    private AmmoData[] _ammoData;

    protected override void Start() {
        base.Start();
        _health = GetComponent<PlayerHealth>();
        _currentGun = defaultGun;
        _ammoData = new AmmoData[guns.Length];
        for (int i = 0; i < guns.Length; i++) {
            _ammoData[i] =
                new
                    AmmoData(ammo[guns[i].ammoType].CurrentAmmo >= guns[i].clipSize ? guns[i].clipSize : ammo[guns[i].ammoType].CurrentAmmo,
                             guns[i].reloadTime,
                             guns[i].isInfiniteAmmo);
            _ammoData[i].OnReload += UpdateUI;
            ammo[guns[i].ammoType].CurrentAmmo -= _ammoData[i].RemainingAmmo;
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
        if (AmmoData.ReloadTimer >= 0f) reloadProgress.fillAmount = AmmoData.ReloadTimer / gun.reloadTime;
    }

    private void HandleAmmo() {
        if (AmmoData.RemainingAmmo > 0 || AmmoData.ReloadTimer > 0f || AmmoData.ReloadTimer < 0f)
            return;
        Reload();
    }

    public override void Shoot() {
        base.Shoot();
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
            $"{AmmoData.RemainingAmmo}/{(!gun.isInfiniteAmmo ? ammo[gun.ammoType].CurrentAmmo.ToString() : "\u221E")}";
    }

    public void Reload() {
        if (AmmoData.RemainingAmmo == gun.clipSize || ammo[gun.ammoType].CurrentAmmo == 0) return;

        // TODO: Visual notification for being unable to reload

        AmmoData.Reload(ammo[gun.ammoType].CurrentAmmo >= gun.clipSize ? -1 : ammo[gun.ammoType].CurrentAmmo);
        ammo[gun.ammoType].CurrentAmmo = Mathf.Max(ammo[gun.ammoType].CurrentAmmo - gun.clipSize, 0);
        UpdateUI();
    }

    public void ChangeGun(int to) {
        _ammoData[_currentGun] = AmmoData;
        gun = guns[to];
        AmmoData = _ammoData[to];
        _currentGun = to;
        
        UpdateUI();
    }

    public void AddAmmo(AmmoType type, int amount) {
        ammo[type].CurrentAmmo = Mathf.Min(ammo[type].CurrentAmmo + amount, ammo[type].MaxAmmo);
    }

    public void Save() {
        for (int i = 0; i < guns.Length; i++)
            ammo[guns[i].ammoType].CurrentAmmo += _ammoData[i].RemainingAmmo;

        ReferenceManager.Inst.ProgressManager.Data.Ammo = ammo;
        ReferenceManager.Inst.ProgressManager.Data.EquippedGuns = guns;
    }

    public void Load() {
        ammo = ReferenceManager.Inst.ProgressManager.Data.Ammo;
        guns = ReferenceManager.Inst.ProgressManager.Data.EquippedGuns;
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