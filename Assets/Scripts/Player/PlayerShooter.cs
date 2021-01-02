using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerShooter : Shooter, ISaveLoad
{
    [SerializeField, InlineEditor, PropertyOrder(0f)]
    private GunData[] guns;
    [SerializeField]
    private int defaultGun;
    [SerializeField]
    private TextMeshProUGUI ammoText;
    [SerializeField, FoldoutGroup("GUI")]
    private Image reloadProgress;
    [SerializeField, FoldoutGroup("GUI")]
    private Transform ammoTracker;
    [SerializeField, FoldoutGroup("GUI")]
    private float punchForce = 3f;
    [SerializeField, FoldoutGroup("GUI")]
    private float punchDuration = 0.4f;
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
    private bool _canTween;

    protected override void Start() {
        base.Start();
        _health = GetComponent<PlayerHealth>();
        _currentGun = defaultGun;
        _ammoData = new AmmoData[guns.Length];
        _canTween = true;
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
        if (AmmoData.ReloadTimer >= 0f) reloadProgress.fillAmount = 1f - AmmoData.ReloadTimer / gun.reloadTime;
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
        if (AmmoData.RemainingAmmo == gun.clipSize || ammo[gun.ammoType].CurrentAmmo == 0) {
            if (!_canTween) return;
            _canTween = false;
            ammoTracker.DOPunchPosition(Vector3.right * punchForce, punchDuration).onComplete += () => _canTween = true;

            return;
        }

        AmmoData.Reload(ammo[gun.ammoType].CurrentAmmo >= gun.clipSize ? -1 : ammo[gun.ammoType].CurrentAmmo);
        ammo[gun.ammoType].CurrentAmmo = Mathf.Max(ammo[gun.ammoType].CurrentAmmo - gun.clipSize + AmmoData.RemainingAmmo, 0);
        UpdateUI();
    }

    public void ChangeGun(int to) {
        if (to >= guns.Length)
            return;
        _ammoData[_currentGun] = AmmoData;
        gun = guns[to];
        AmmoData = _ammoData[to];
        _currentGun = to;
        
        UpdateUI();
    }

    public void AddAmmo(AmmoType type, int amount) {
        ammo[type].CurrentAmmo = Mathf.Min(ammo[type].CurrentAmmo + amount, ammo[type].MaxAmmo);
        UpdateUI();
    }

    public void Save() {
        for (int i = 0; i < guns.Length; i++)
            ammo[guns[i].ammoType].CurrentAmmo += _ammoData[i].RemainingAmmo;

        ReferenceManager.Inst.ProgressManager.Data.Ammo = ammo;
        ReferenceManager.Inst.ProgressManager.Data.EquippedGuns = guns;
    }

    public void Load() {
        ammo = ReferenceManager.Inst.ProgressManager.Data.Ammo;
        guns = ReferenceManager.Inst.ProgressManager.Data.EquippedGuns.Where(g => g!= null).ToArray();
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