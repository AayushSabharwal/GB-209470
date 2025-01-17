﻿using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


public class ProjectileBullet : Bullet
{
    private float _lifetimer;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Light2D _light;
    private bool _isPaused;
    private Collider2D[] _explosionHits;
    
    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _light = GetComponent<Light2D>();
    }

    protected override void Start() {
        base.Start();
        ReferenceManager.Inst.UIManager.OnPause += OnPause;
        _explosionHits = new Collider2D[20];
    }

    private void OnEnable() {
        if (data == null) return;
        _light.color = data.colour;
        _lifetimer = data.lifetime;
        _rb.velocity = transform.right * data.speed;
        _spriteRenderer.sprite = data.sprite;
        _spriteRenderer.color = data.colour;
        transform.localScale = data.scale;
    }

    private void OnDisable() {
        _rb.velocity = Vector2.zero;
    }

    private void Update() {
        _lifetimer -= Time.deltaTime;
        if (_lifetimer < 0f) {
            _lifetimer = 0f;
            ObjectPooler.Return(data.poolTag, gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (((1 << other.gameObject.layer) & data.collisionMask.value) > 0) {
            if (data.isExplosive) {
                int count = Physics2D.OverlapCircleNonAlloc(transform.position, data.explosionRadius, _explosionHits,
                                                data.damageMask);
                for(int i = 0; i < count; i++)
                    TryDamage(_explosionHits[i].gameObject);
            }
            else
                TryDamage(other.gameObject);

            DoOnHitFX();
            
            ObjectPooler.Return(data.poolTag, gameObject);
        }
    }
    
    private void OnPause(bool isPaused) {
        _isPaused = isPaused;
        _rb.velocity = _isPaused ? Vector2.zero : (Vector2) transform.right * data.speed;
    }
}