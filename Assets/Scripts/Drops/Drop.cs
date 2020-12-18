using UnityEngine;

public class Drop : MonoBehaviour
{
    [SerializeField]
    public DropData data;

    private ObjectPooler _objectPooler;
    private SpriteRenderer _spriteRenderer;

    private float _lifetimer;
    
    protected virtual void Awake() {
        _objectPooler = ReferenceManager.Inst.ObjectPooler;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable() {
        if (data == null) return;

        _spriteRenderer.sprite = data.sprite;
        _spriteRenderer.color = data.color;
        _lifetimer = data.lifetime;
    }

    private void Update() {
        _lifetimer -= Time.deltaTime;
        if(_lifetimer <= 0f)
            _objectPooler.Return(data.poolTag, gameObject);
    }

    protected virtual void OnPickup() {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer != 10) return;
        
        OnPickup();
        _objectPooler.Return(data.poolTag, gameObject);
    }
}