using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class ReferenceManager : SerializedMonoBehaviour
{
    public static ReferenceManager Inst;

    [OdinSerialize, ShowInInspector]
    public Transform Player { get; private set; }
    [OdinSerialize, ShowInInspector]
    public MapGenerator MapGenerator { get; private set; }
    [OdinSerialize, ShowInInspector]
    public ObjectPooler ObjectPooler { get; private set; }
    [OdinSerialize, ShowInInspector]
    public CurrencyManager CurrencyManager { get; private set; }
    [OdinSerialize, ShowInInspector]
    public CoinManager CoinManager { get; private set; }
    
    private void Awake() {
        if (Inst == null)
            Inst = this;
        else if (Inst != this) {
            Destroy(gameObject);
            return;
        }
    }
}
