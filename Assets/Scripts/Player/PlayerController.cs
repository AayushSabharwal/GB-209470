using System;
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
    private float maxHP = 10f;
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float shootDeadzone = 0.1f;
    [SerializeField]
    private Vector2IntVar initialPosition;

    private void Start() {
        _health = GetComponent<Health>();
        _shooter = GetComponent<Shooter>();
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();

        transform.position = new Vector3(initialPosition.data.x, initialPosition.data.y);
        _health.Respawned(maxHP);
    }

    private void Update() {
        _rb.velocity = _input.Move * moveSpeed;
        if (_input.Shoot.sqrMagnitude >= shootDeadzone * shootDeadzone) {
            transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(_input.Shoot.y, _input.Shoot.x) * Mathf.Rad2Deg,
                                                      Vector3.forward);
            _shooter.Shoot();
        }
    }
}