using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shooter : SerializedMonoBehaviour
{
    [InlineEditor, OdinSerialize, PropertyOrder(-1f)]
    protected virtual GunData Gun { get; set; }
    [SceneObjectsOnly, SerializeField]
    protected Transform shootPoint;

    protected float ShotTimer;
    protected AmmoData AmmoData;
    protected ObjectPooler ObjectPooler;
    [NonSerialized]
    public EventHandler OnShoot;

    private void Awake() {
        ObjectPooler = ReferenceManager.Inst.ObjectPooler;
    }

    protected virtual void Start() {
        AmmoData = new AmmoData(Gun.clipSize, Gun.reloadTime);
    }

    protected virtual void Update() {
        if (ShotTimer > 0f)
            ShotTimer -= Time.deltaTime;
        AmmoData.Update();
    }

    public virtual void Shoot() {
        if (ShotTimer > 0f || !AmmoData.CanShoot)
            return;

        AmmoData.Shoot();
        for (int i = 0; i < Gun.shots.Length; i++) {
            MakeBullets(Gun.shots[i].bullet,
                        Gun.shots[i].offsetAngle,
                        Gun.shots[i].applySpread ? Gun.spreadAngle : 0f,
                        Gun.shots[i].groupSize);
        }

        ShotTimer = 1f / Gun.fireRate;
        OnShoot?.Invoke(this, EventArgs.Empty);
    }

    protected void MakeBullets(BulletData bulletType, float offsetAngle, float spread, int groupSize) {
        for (int j = 0; j < groupSize; j++) {
            GameObject bullet = ObjectPooler.Request(bulletType.poolTag);
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
    public bool CanShoot => RemainingAmmo > 0 && ReloadTimer <= 0f;
    private int _maxAmmo;
    private int _reloadAmount;
    private float _reloadTime;

    public AmmoData(int maxAmmo, float reloadTime) {
        _maxAmmo = maxAmmo;
        _reloadTime = reloadTime;
        _reloadAmount = _maxAmmo;
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
        if (ReloadTimer < 0f) {
            ReloadTimer = 0f;
            RemainingAmmo = _reloadAmount;
        }
        else if(ReloadTimer > 0f)
            ReloadTimer -= Time.deltaTime;
    }
}