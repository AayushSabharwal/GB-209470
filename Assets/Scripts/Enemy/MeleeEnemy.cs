using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    private Health _playerHealth;
    private Enemy _self;
    private float _attackTimer;
    
    private void Awake() {
        _self = GetComponent<Enemy>();
        _playerHealth = ReferenceManager.Inst.Player.GetComponent<Health>();
    }

    private void Update() {

        if (_self.ReachedEndOfPath && _attackTimer <= 0f) {
            _playerHealth.TakeDamage(_self.data.damage);
            _attackTimer = 1f / _self.data.hitRate;
        }

        _attackTimer -= Time.deltaTime;
    }
}
