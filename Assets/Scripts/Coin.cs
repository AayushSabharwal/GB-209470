using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    public CoinData data;

    private ObjectPooler _objectPooler;
    private CurrencyManager _currencyManager;
    private SpriteRenderer _spriteRenderer;

    private float _lifetimer;
    
    private void Awake() {
        _objectPooler = ReferenceManager.Inst.ObjectPooler;
        _currencyManager = ReferenceManager.Inst.CurrencyManager;
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

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer != 10) return;

        _currencyManager.AddCurrency(data.value);
        _objectPooler.Return(data.poolTag, gameObject);
    }
}