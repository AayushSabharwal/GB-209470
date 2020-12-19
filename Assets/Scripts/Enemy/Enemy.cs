using System;
using System.Collections.Generic;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    [InlineEditor]
    public EnemyData data;


    private SpriteRenderer _spriteRenderer;
    private Health _health;
    private DropManager _dropManager;
    private Transform _player;
    private ObjectPooler _objectPooler;
    private SharedDataManager _sharedDataManager;
    private Seeker _seeker;
    private Rigidbody2D _rb;

    private Path _path;
    private float _repathRate;
    private float _repathTimer;
    private int _currentWaypoint;
    public bool ReachedEndOfPath { get; private set; }
    private Vector2 _targetDir;
    private Vector2 _targetDirNorm;
    private bool _isPaused;
    private bool _isPlayerDead;
    private Vector2 _savedVelocity;

    protected virtual void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _health = GetComponent<Health>();
        _player = ReferenceManager.Inst.PlayerHealth.transform;
        _objectPooler = ReferenceManager.Inst.ObjectPooler;
        _dropManager = ReferenceManager.Inst.DropManager;
        _sharedDataManager = ReferenceManager.Inst.SharedDataManager;
        _rb = GetComponent<Rigidbody2D>();
        _seeker = GetComponent<Seeker>();
        _isPlayerDead = false;
        _isPaused = false;

        _health.OnDeath += OnDeath;
        ReferenceManager.Inst.PlayerHealth.OnDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath(object sender, EventArgs e) {
        _isPlayerDead = true;
    }

    protected virtual void Start() {
        ReferenceManager.Inst.UIManager.OnPause += OnPause;
    }

    private void OnPause(bool isPaused) {
        _isPaused = isPaused;
        if (_isPaused) {
            _savedVelocity = _rb.velocity;
            _rb.velocity = Vector2.zero;
        }
        else
            _rb.velocity = _savedVelocity;
    }

    protected virtual void OnEnable() {
        if (data == null) return;

        _spriteRenderer.sprite = data.sprite;
        _spriteRenderer.color = data.color;

        _health.Respawned(data.health);
    }

    protected virtual void Update() {
        if (_isPaused || _isPlayerDead) return;

        CheckRepathRate();

        _repathTimer -= Time.deltaTime;
        if (_repathTimer <= 0f && _seeker.IsDone() && !ReachedEndOfPath) {
            _seeker.StartPath(_rb.position, _player.position, OnPathComplete);
            _repathTimer = _repathRate;
        }

        ReachedEndOfPath = (transform.position - _player.position).sqrMagnitude <
                           data.approachRadius * data.approachRadius;

        if (ReachedEndOfPath) transform.right = _player.position - transform.position;
    }

    private void FixedUpdate() {
        if (_path == null || _path.error || _currentWaypoint >= _path.vectorPath.Count || 
            ReachedEndOfPath || _isPaused || _isPlayerDead)
            return;


        _rb.AddForce(_targetDirNorm * data.speed);
        transform.right = _rb.velocity;

        if ((_path.vectorPath[_currentWaypoint] - transform.position).sqrMagnitude >
            data.pathfindingRadius * data.pathfindingRadius) return;

        _currentWaypoint++;

        if (_currentWaypoint >= _path.vectorPath.Count) return;

        _targetDir = _path.vectorPath[_currentWaypoint] - transform.position;
        _targetDirNorm = _targetDir.normalized;
    }

    private void OnPathComplete(Path p) {
        if (p.error) return;
        _path = p;
        _currentWaypoint = 0;
    }

    private void CheckRepathRate() {
        for (int i = 0; i < _sharedDataManager.repathRateScaling.Length; i++) {
            if (!((transform.position - _player.position).sqrMagnitude >
                  _sharedDataManager.repathRateScaling[i].Distance *
                  _sharedDataManager.repathRateScaling[i].Distance)) continue;

            _repathRate = _sharedDataManager.repathRateScaling[i].Value;
            break;
        }
    }

    protected virtual void OnDeath(object sender, EventArgs e) {
        int coinValue = Random.Range(data.coinsToDrop.x, data.coinsToDrop.y + 1);
        _dropManager.RequestCoins(coinValue, out List<GameObject> coins);
        for (int i = 0; i < coins.Count; i++) {
            coins[i].transform.position = transform.position +
                                          Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward) *
                                          Vector3.right * Random.Range(data.scatterRadius.x, data.scatterRadius.y);
            coins[i].SetActive(true);
        }

        if (Random.value < data.ammoDropChance) {
            int ammoCount = Random.Range(data.ammoDropCount.x, data.ammoDropCount.y + 1);
            _dropManager.RequestAmmo(ammoCount, out List<GameObject> ammo);
            for (int i = 0; i < ammo.Count; i++) {
                ammo[i].transform.position = transform.position +
                                             Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward) *
                                             Vector3.right * Random.Range(data.scatterRadius.x, data.scatterRadius.y);
                ammo[i].SetActive(true);
            }
        }

        _objectPooler.Return(data.poolTag, gameObject);
    }
}