using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField]
    private List<PooledObject> pooledObjects;

    private Dictionary<string, GameObject> _gameobjectMap;
    private Dictionary<string, Stack<GameObject>> _pools;

    private void Start() {
        _pools = new Dictionary<string, Stack<GameObject>>();
        _gameobjectMap = new Dictionary<string, GameObject>();
        foreach (PooledObject pool in pooledObjects) {
            _pools[pool.itemName] = new Stack<GameObject>();
            _gameobjectMap[pool.itemName] = pool.pooledObject;
        }
    }

    public GameObject Request(string key) {
        if (!_pools.ContainsKey(key)) {
            Debug.LogError($"ObjectPooler/Request: Invalid key {key}");
            return null;
        }

        return _pools[key].Count == 0
            ? Instantiate(_gameobjectMap[key], Vector3.zero, Quaternion.identity)
            : _pools[key].Pop();
    }

    public void Return(string key, GameObject go) {
        if (!_pools.ContainsKey(key)) {
            Debug.LogError($"ObjectPooler/Return: Invalid key {key}");
            return;
        }

        go.SetActive(false);
        _pools[key].Push(go);
    }
}

[Serializable]
public struct PooledObject
{
    public string itemName;
    [AssetsOnly]
    public GameObject pooledObject;
}