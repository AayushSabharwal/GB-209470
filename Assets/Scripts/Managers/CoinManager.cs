using System;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField]
    private DropData single;
    [SerializeField]
    private DropData ten;

    private Dictionary<int, Coin> _scriptMap;
    private ObjectPooler _objectPooler;

    private void Awake() {
        _objectPooler = ReferenceManager.Inst.ObjectPooler;
    }

    private void Start() {
        _scriptMap = new Dictionary<int, Coin>();
    }

    public void Request(int value, out List<GameObject> coins) {
        coins = new List<GameObject>(value/10 + value%10);
        for (int i = 0; i < value / 10; i++) {
            GameObject g = _objectPooler.Request(ten.poolTag);
            if (_scriptMap.ContainsKey(g.GetInstanceID()))
                _scriptMap[g.GetInstanceID()].data = ten;
            else {
                _scriptMap[g.GetInstanceID()] = g.GetComponent<Coin>();
                _scriptMap[g.GetInstanceID()].data = ten;
            }
            coins.Add(g);
        }

        for (int i = 0; i < value % 10; i++) {
            GameObject g = _objectPooler.Request(single.poolTag);
            if (_scriptMap.ContainsKey(g.GetInstanceID()))
                _scriptMap[g.GetInstanceID()].data = single;
            else {
                _scriptMap[g.GetInstanceID()] = g.GetComponent<Coin>();
                _scriptMap[g.GetInstanceID()].data = single;
            }
            coins.Add(g);
        }
    } 
}