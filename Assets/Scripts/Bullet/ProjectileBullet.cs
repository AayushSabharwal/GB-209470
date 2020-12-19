using UnityEngine;


public class ProjectileBullet : Bullet
{
    private float _lifetimer;
    private Rigidbody2D _rb;
    private bool _isPaused;
    
    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
    }

    protected override void Start() {
        base.Start();
        ReferenceManager.Inst.UIManager.OnPause += OnPause;
    }

    private void OnEnable() {
        if (data == null) return;

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

    private void OnTriggerEnter2D(Collider2D other) {
        if (((1 << other.gameObject.layer) & data.collisionMask.value) > 0) {
            TryDamage(other.gameObject);
            ObjectPooler.Return(data.poolTag, gameObject);
        }
    }
    
    private void OnPause(bool isPaused) {
        _isPaused = isPaused;
        _rb.velocity = _isPaused ? Vector2.zero : (Vector2) transform.right * data.speed;
    }
}