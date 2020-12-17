using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public int Currency { get; private set; }
    [SerializeField]
    private TextMeshProUGUI displayText;

    private void Start() {
        Currency = 0;
        UpdateUI();
    }

    private void UpdateUI() {
        displayText.text = Currency.ToString();
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
}
