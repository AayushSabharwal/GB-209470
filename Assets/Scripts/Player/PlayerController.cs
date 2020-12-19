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
    [SerializeField]
    private float shootDeadzone = 0.1f;

    private bool _isPaused;
    
    private void Start() {
        _health = GetComponent<Health>();
        _shooter = GetComponent<Shooter>();
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();

        transform.position = new Vector3(ReferenceManager.Inst.SharedDataManager.PlayerStartPosition.x + 0.5f,
                                         ReferenceManager.Inst.SharedDataManager.PlayerStartPosition.y + 0.5f);
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
        
        _rb.velocity = _input.Move * moveSpeed;
        if (_input.Shoot.sqrMagnitude >= shootDeadzone * shootDeadzone) {
            transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(_input.Shoot.y, _input.Shoot.x) * Mathf.Rad2Deg,
                                                      Vector3.forward);
            _shooter.Shoot();
        }
    }
}