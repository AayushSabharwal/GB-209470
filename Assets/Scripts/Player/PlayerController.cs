using System;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerShooter))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Health _health;
    private Shooter _shooter;
    private PlayerInput _input;
    private Rigidbody2D _rb;

    [SerializeField]
    private float maxHp = 10f;
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField, FoldoutGroup("Dash")]
    private float dashSpeed = 10f;
    [SerializeField, FoldoutGroup("Dash")]
    private float dashDuration = 0.5f;
    [SerializeField, PropertyRange(0f, 1f)]
    private float stickDeadzone = 0.1f;
    [SerializeField, PropertyRange(0f, 1f)]
    private float shootDeadzone = 0.4f;
    [SerializeField]
    private RectTransform shootDeadzoneDisplay;

    private bool _isPaused;
    private float _dashTimer;
    private Vector2 _dashDirection;

    private void Start() {
        _health = GetComponent<Health>();
        _shooter = GetComponent<Shooter>();
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        
        shootDeadzoneDisplay.sizeDelta = Vector2.one * 400f * shootDeadzone; 
        transform.position = new Vector3(ReferenceManager.Inst.SharedDataManager.playerStartPosition.x + 0.5f,
                                         ReferenceManager.Inst.SharedDataManager.playerStartPosition.y + 0.5f) * 1.5f;
        _health.Respawned(maxHp);

        _isPaused = false;
        ReferenceManager.Inst.UIManager.OnPause += OnPause;
    }

    private void OnPause(bool isPaused) {
        _isPaused = isPaused;
        if (_isPaused) _rb.velocity = Vector2.zero;
    }

    private void Update() {
        if (_isPaused)
            return;
        if (_input.Shoot.sqrMagnitude >= stickDeadzone * stickDeadzone) {
            transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(_input.Shoot.y, _input.Shoot.x) * Mathf.Rad2Deg,
                                                      Vector3.forward);
        }
    }

    private void LateUpdate() {
        if(_input.Shoot.sqrMagnitude >= shootDeadzone * shootDeadzone)
            _shooter.Shoot();
    }

    private void FixedUpdate() {
        if (_dashTimer <= 0f)
            _rb.velocity = _input.Move * moveSpeed;
        else {
            _rb.velocity = _dashDirection * dashSpeed;
            _dashTimer -= Time.fixedDeltaTime;
        }
    }

    public bool Dash() {
        if (_input.Move.sqrMagnitude < stickDeadzone * stickDeadzone) return false;

        _dashDirection = _input.Move.normalized;
        _dashTimer = dashDuration;
        return true;
    }
}