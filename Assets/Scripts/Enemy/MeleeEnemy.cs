using System;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [SerializeField]
    private AudioSource passiveSource;
    
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
        ReferenceManager.Inst.EnemySpawner.OnLevelEnd += OnLevelEnd;
    }

    private void OnLevelEnd() {
        _isPaused = true;
    }

    private void OnPause(bool isPaused) {
        _isPaused = isPaused;
    }

    private void Update() {
        if (_isPaused || _self.IsPlayerDead) return;

        if (_self.ReachedEndOfPath) {
            if (_attackTimer <= 0f) {
                _playerHealth.TakeDamage(_self.data.damage);
                _attackTimer = 1f / _self.data.hitRate;                
            }
            if (_self.data.hasPassiveAttackingLoop && !passiveSource.isPlaying) {
                passiveSource.clip = _self.data.passiveAttackingLoop;
                passiveSource.loop = true;
                passiveSource.Play();
            }
        }
        else if (_self.data.hasPassiveAttackingLoop && passiveSource.isPlaying)
            passiveSource.Stop();

        _attackTimer -= Time.deltaTime;
    }
}