using UnityEngine;

// ReSharper disable once CheckNamespace
public class ProjectileBullet : Bullet
{
    private float _lifetimer;
    private Rigidbody2D _rb;

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        _lifetimer = data.lifetime;
        _rb.velocity = transform.right * data.speed;
    }

    private void OnDisable() {
        _rb.velocity = Vector2.zero;
    }

    private void Update() {
        _lifetimer -= Time.deltaTime;
        if (_lifetimer <= 0f)
            ObjectPooler.Return(data.poolTag, gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (((1 << other.gameObject.layer) & data.collisionMask.value) > 0) {
            TryDamage(other.gameObject);
            ObjectPooler.Return(data.poolTag, gameObject);   
        }
    }
}