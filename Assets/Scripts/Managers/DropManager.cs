using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class DropManager : SerializedMonoBehaviour, ISaveLoad
{
    [SerializeField]
    private DropData single;
    [SerializeField]
    private DropData ten;
    [SerializeField, FloatingProbabilitySettings(60f)]
    private List<AmmoDropData> droppableAmmo;

    private Dictionary<int, Coin> _coinScriptMap;
    private Dictionary<int, AmmoDrop> _ammoScriptMap;
    private ObjectPooler _objectPooler;

    private void Awake() {
        _objectPooler = ReferenceManager.Inst.ObjectPooler;
    }

    private void Start() {
        _coinScriptMap = new Dictionary<int, Coin>();
        _ammoScriptMap = new Dictionary<int, AmmoDrop>();
    }

    public void RequestCoins(int value, out List<GameObject> coins) {
        coins = new List<GameObject>(value / 10 + value % 10);
        for (int i = 0; i < value / 10; i++) {
            GameObject g = _objectPooler.Request(ten.poolTag);
            if (_coinScriptMap.ContainsKey(g.GetInstanceID()))
                _coinScriptMap[g.GetInstanceID()].data = ten;
            else {
                _coinScriptMap[g.GetInstanceID()] = g.GetComponent<Coin>();
                _coinScriptMap[g.GetInstanceID()].data = ten;
            }

            coins.Add(g);
        }

        for (int i = 0; i < value % 10; i++) {
            GameObject g = _objectPooler.Request(single.poolTag);
            if (_coinScriptMap.ContainsKey(g.GetInstanceID()))
                _coinScriptMap[g.GetInstanceID()].data = single;
            else {
                _coinScriptMap[g.GetInstanceID()] = g.GetComponent<Coin>();
                _coinScriptMap[g.GetInstanceID()].data = single;
            }

            coins.Add(g);
        }
    }

    public void RequestAmmo(int amount, out List<GameObject> ammo) {
        ammo = new List<GameObject>(amount);
        for (int i = 0; i < amount; i++) {
            AmmoDropData data = droppableAmmo[0];
            float f = Random.value;
            for (int j = 0; j < droppableAmmo.Count; j++) {
                if (f > droppableAmmo[i].FloatingProbability) {
                    f -= droppableAmmo[i].FloatingProbability;
                    continue;
                }

                data = droppableAmmo[i];
            }

            GameObject g = _objectPooler.Request(data.poolTag);
            if (_ammoScriptMap.ContainsKey(g.GetInstanceID()))
                _ammoScriptMap[g.GetInstanceID()].data = data;
            else {
                _ammoScriptMap[g.GetInstanceID()] = g.GetComponent<AmmoDrop>();
                _ammoScriptMap[g.GetInstanceID()].data = data;
            }

            ammo.Add(g);
        }
    }

    public void Save() { }

    public void Load() {
        droppableAmmo = ReferenceManager.Inst.ProgressManager.Data.DroppableAmmo;
    }
}