using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shooter : SerializedMonoBehaviour
{
    [InlineEditor, SerializeField, PropertyOrder(-1f)]
    public GunData gun;
    [SceneObjectsOnly, SerializeField]
    protected Transform shootPoint;

    private float _shotTimer;
    [HideInInspector]
    public AmmoData AmmoData;
    private ObjectPooler _objectPooler;
    [NonSerialized]
    public EventHandler OnShoot;

    private void Awake() {
        _objectPooler = ReferenceManager.Inst.ObjectPooler;
    }

    protected virtual void OnEnable() {
        if(gun == null)
            return;
        AmmoData = new AmmoData(gun.clipSize, gun.reloadTime, gun.isInfiniteAmmo);
    }

    protected virtual void Update() {
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

        _shotTimer = 1f / gun.fireRate;
        OnShoot?.Invoke(this, EventArgs.Empty);
    }

    protected void MakeBullets(BulletData bulletType, float offsetAngle, float spread, int groupSize) {
        for (int j = 0; j < groupSize; j++) {
            GameObject bullet = _objectPooler.Request(bulletType.poolTag);
            bullet.transform.position = Quaternion.AngleAxis(offsetAngle, Vector3.forward) * shootPoint.position;
            bullet.transform.rotation = shootPoint.rotation *
                                        Quaternion.AngleAxis(offsetAngle + Random.Range(-spread, spread),
                                                             Vector3.forward);
            bullet.GetComponent<Bullet>().data = bulletType;
            bullet.SetActive(true);
        }
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

    public AmmoData(int maxAmmo, float reloadTime, bool infiniteAmmo) {
        _maxAmmo = maxAmmo;
        _reloadTime = reloadTime;
        _reloadAmount = _maxAmmo;
        _infiniteAmmo = infiniteAmmo;
        RemainingAmmo = _maxAmmo;
        ReloadTimer = 0f;
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
        if(_infiniteAmmo)    return;
        if (ReloadTimer < 0f) {
            ReloadTimer = 0f;
            RemainingAmmo = _reloadAmount;
        }
        else if(ReloadTimer > 0f)
            ReloadTimer -= Time.deltaTime;
    }
}