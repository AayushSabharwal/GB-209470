using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shooter : MonoBehaviour
{
    [InlineEditor, SerializeField, PropertyOrder(-1f)]
    public GunData gun;
    [SerializeField]
    protected Transform shootPoint;

    [HideInInspector]
    public AmmoData AmmoData;
    [NonSerialized]
    public EventHandler OnShoot;

    private float _shotTimer;
    private ObjectPooler _objectPooler;
    private AudioSource _audioSource;
    protected bool IsPaused;
    protected bool IsPlayerDead;

    private void Awake() {
        _objectPooler = ReferenceManager.Inst.ObjectPooler;
        _audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Start() {
        IsPaused = false;
        IsPlayerDead = false;
        ReferenceManager.Inst.UIManager.OnPause += OnPause;
        ReferenceManager.Inst.PlayerHealth.OnDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath(object sender, EventArgs e) {
        IsPlayerDead = true;
    }

    private void OnPause(bool isPaused) {
        IsPaused = isPaused;
    }

    protected virtual void OnEnable() {
        if (gun == null)
            return;
        AmmoData = new AmmoData(gun.clipSize, gun.reloadTime, gun.isInfiniteAmmo);
    }

    protected virtual void Update() {
        if (IsPaused || IsPlayerDead) return;
        if (_shotTimer > 0f)
            _shotTimer -= Time.deltaTime;
        AmmoData.Update();
    }

    public virtual void Shoot() {
        if (_shotTimer > 0f || !AmmoData.CanShoot)
            return;

        AmmoData.Shoot();
        for (int i = 0; i < gun.shots.Length; i++) {
            MakeBullets(gun.shots[i].bullet,
                        gun.shots[i].offsetAngle,
                        gun.shots[i].applySpread ? gun.spreadAngle : 0f,
                        gun.shots[i].groupSize);
        }
        
        _audioSource.pitch = gun.pitch + Random.Range(-gun.pitchVariation, gun.pitchVariation);
        _audioSource.PlayOneShot(gun.shootSound);
        _shotTimer = 1f / gun.fireRate;
        OnShoot?.Invoke(this, EventArgs.Empty);
    }

    private void MakeBullets(BulletData bulletType, float offsetAngle, float spread, int groupSize) {
        for (int j = 0; j < groupSize; j++) {
            GameObject bullet = _objectPooler.Request(bulletType.poolTag);
            bullet.transform.position = Quaternion.AngleAxis(offsetAngle, Vector3.forward) * shootPoint.position;
            bullet.transform.rotation = shootPoint.rotation *
                                        Quaternion.AngleAxis(offsetAngle + Random.Range(-spread, spread),
                                                             Vector3.forward);
            ApplyBulletAttributes(bullet.GetComponent<Bullet>(), bulletType);
            bullet.SetActive(true);
        }
    }

    protected virtual void ApplyBulletAttributes(Bullet bullet, BulletData bulletType) {
        bullet.data = bulletType;
    }
}

[Serializable]
public struct AmmoData
{
    public int RemainingAmmo { get; private set; }
    public float ReloadTimer { get; private set; }
    public bool CanShoot => _infiniteAmmo || RemainingAmmo > 0 && ReloadTimer <= 0f;
    private bool _infiniteAmmo;
    private int _maxAmmo;
    private int _reloadAmount;
    private float _reloadTime;
    public event Action OnReload;

    public AmmoData(int maxAmmo, float reloadTime, bool infiniteAmmo) {
        _maxAmmo = maxAmmo;
        _reloadTime = reloadTime;
        _reloadAmount = _maxAmmo;
        _infiniteAmmo = infiniteAmmo;
        RemainingAmmo = _maxAmmo;
        ReloadTimer = 0f;
        OnReload = null;
    }

    public void Shoot() {
        RemainingAmmo--;
    }

    public void Reload(int remainingAmmo = -1) {
        if (ReloadTimer > 0f || remainingAmmo > _maxAmmo) return;

        _reloadAmount = remainingAmmo <= 0 ? _maxAmmo : remainingAmmo;
        ReloadTimer = _reloadTime;
    }

    public void Update() {
        if (_infiniteAmmo) return;
        if (ReloadTimer < 0f) {
            ReloadTimer = 0f;
            RemainingAmmo = _reloadAmount;
            OnReload?.Invoke();
        }
        else if (ReloadTimer > 0f)
            ReloadTimer -= Time.deltaTime;
    }
}