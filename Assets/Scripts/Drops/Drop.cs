using UnityEngine;

public class Drop : MonoBehaviour
{
    [SerializeField]
    public DropData data;

    private ObjectPooler _objectPooler;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _collider;

    private float _lifetimer;
    private bool _isPaused;

    protected virtual void Awake() {
        _objectPooler = ReferenceManager.Inst.ObjectPooler;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void Start() {
        _isPaused = false;
        ReferenceManager.Inst.UIManager.OnPause += OnPause;
    }

    private void OnPause(bool isPaused) {
        _isPaused = isPaused;
    }

    protected virtual void OnEnable() {
        if (data == null) return;

        _spriteRenderer.sprite = data.sprite;
        _spriteRenderer.color = data.color;
        _collider.size = data.colliderDimensions;
        transform.localScale = data.scale;
        _lifetimer = data.lifetime;
    }

    protected virtual void Update() {
        if (_isPaused) return;
        _lifetimer -= Time.deltaTime;
        if (_lifetimer <= 0f)
            _objectPooler.Return(data.poolTag, gameObject);
    }

    protected virtual void OnPickup() {
        ReferenceManager.Inst.SfxAudio.pitch = data.pitch + Random.Range(-data.pitchVariance, data.pitchVariance);
        ReferenceManager.Inst.SfxAudio.PlayOneShot(data.pickupSound);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer != 10) return;

        OnPickup();
        _objectPooler.Return(data.poolTag, gameObject);
    }
}