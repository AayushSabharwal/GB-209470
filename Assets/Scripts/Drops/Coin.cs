public class Coin : Drop
{
    private CurrencyManager _currencyManager;
    
    protected override void Awake() {
        base.Awake();
        _currencyManager = ReferenceManager.Inst.CurrencyManager;
    }

    protected override void OnPickup() {
        _currencyManager.AddCurrency(data.value);
    }
}