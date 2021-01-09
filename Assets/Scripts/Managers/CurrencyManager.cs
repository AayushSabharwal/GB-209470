using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour, ISaveLoad
{
    [ShowInInspector, ReadOnly]
    public int Currency { get; private set; }
    [SerializeField]
    private TextMeshProUGUI displayText;
    [SerializeField]
    private TextMeshProUGUI shadowText;

    private void Start() {
        UpdateUI();
    }

    private void UpdateUI() {
        displayText.text = Currency.ToString();
        shadowText.text = displayText.text;
    }

    public void AddCurrency(int amount) {
        Currency += amount;
        UpdateUI();
    }

    public bool TrySubtractCurrency(int amount) {
        if (Currency < amount) return false;
        
        Currency -= amount;
        UpdateUI();
        return true;
    }

    public void Save() {
        ReferenceManager.Inst.ProgressManager.Data.Currency = Currency;
    }

    public void Load() {
        Currency = ReferenceManager.Inst.ProgressManager.Data.Currency;
    }
}
