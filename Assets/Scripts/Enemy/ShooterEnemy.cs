using UnityEngine;

public class ShooterEnemy : MonoBehaviour
{
    private Enemy _self;
    private Shooter _shooter;
    private bool _isPaused;

    private void Awake() {
        _self = GetComponent<Enemy>();
        _shooter = GetComponent<Shooter>();
    }

    private void Start() {
        _isPaused = false;
        ReferenceManager.Inst.UIManager.OnPause += OnPause;
    }

    private void OnPause(bool isPaused) {
        _isPaused = isPaused;
    }

    private void OnEnable() {
        _shooter.gun = _self.data.gun;
        _shooter.AmmoData = new AmmoData(_shooter.gun.clipSize, _shooter.gun.reloadTime, _shooter.gun.isInfiniteAmmo);
    }

    private void Update() {
        if (_isPaused || _self.IsPlayerDead) return;

        if (_self.ReachedEndOfPath)
            _shooter.Shoot();
    }
}