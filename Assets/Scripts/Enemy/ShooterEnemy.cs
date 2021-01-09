using UnityEngine;

public class ShooterEnemy : MonoBehaviour
{
    [SerializeField]
    private Transform shootPoint;
    
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
        ReferenceManager.Inst.EnemySpawner.OnLevelEnd += OnLevelEnd;
    }

    private void OnLevelEnd() {
        _isPaused = true;
    }

    private void OnPause(bool isPaused) {
        _isPaused = isPaused;
    }

    private void OnEnable() {
        shootPoint.localPosition = _self.data.shootPointPosition;
        _shooter.gun = _self.data.gun;
        _shooter.AmmoData = new AmmoData(_shooter.gun.clipSize, _shooter.gun.reloadTime, _shooter.gun.isInfiniteAmmo);
    }

    private void Update() {
        if (_isPaused || _self.IsPlayerDead) return;

        if (_self.ReachedEndOfPath)
            _shooter.Shoot();
    }
}