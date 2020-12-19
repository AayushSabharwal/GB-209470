using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    private Health _playerHealth;
    private Enemy _self;
    private float _attackTimer;
    private bool _isPaused;

    private void Awake() {
        _self = GetComponent<Enemy>();
        _playerHealth = ReferenceManager.Inst.PlayerHealth;
    }

    private void Start() {
        _isPaused = false;
        ReferenceManager.Inst.UIManager.OnPause += OnPause;
    }

    private void OnPause(bool isPaused) {
        _isPaused = isPaused;
    }

    private void Update() {
        if (_isPaused) return;

        if (_self.ReachedEndOfPath && _attackTimer <= 0f) {
            _playerHealth.TakeDamage(_self.data.damage);
            _attackTimer = 1f / _self.data.hitRate;
        }

        _attackTimer -= Time.deltaTime;
    }
}