using UnityEngine;

public class ShooterEnemy : MonoBehaviour
{
    private Enemy _self;
    private Shooter _shooter;

    private void Awake() {
        _self = GetComponent<Enemy>();
        _shooter = GetComponent<Shooter>();
    }

    private void OnEnable() {
        _shooter.gun = _self.data.gun;
        _shooter.AmmoData = new AmmoData(_shooter.gun.clipSize, _shooter.gun.reloadTime, _shooter.gun.isInfiniteAmmo);
    }

    private void Update() {
        if (_self.ReachedEndOfPath)
            _shooter.Shoot();
    }
}