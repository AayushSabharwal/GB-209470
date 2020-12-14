using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ObjectPooler : SerializedMonoBehaviour
{
    [SerializeField]
    private Dictionary<string, PooledObject> pooledObjects;
    private Dictionary<string, Stack<GameObject>> _pools;

    private void Start() {
        _pools = new Dictionary<string, Stack<GameObject>>();

        foreach (string key in pooledObjects.Keys) {
            _pools[key] = new Stack<GameObject>();
            pooledObjects[key].pooledObject.SetActive(false);

            if (pooledObjects[key].prewarm)
                for (int i = 0; i < pooledObjects[key].amount; i++) {
                    GameObject g = Instantiate(pooledObjects[key].pooledObject, Vector3.zero, Quaternion.identity);
                    g.SetActive(false);
                    _pools[key].Push(g);
                }
        }
    }

    public GameObject Request(string key) {
        if (!_pools.ContainsKey(key)) {
            Debug.LogError($"ObjectPooler/Request: Invalid key {key}");
            return null;
        }

        return _pools[key].Count == 0
            ? Instantiate(pooledObjects[key].pooledObject, Vector3.zero, Quaternion.identity)
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
    [AssetsOnly, FoldoutGroup("Object")]
    public GameObject pooledObject;

    [HorizontalGroup("Object/Prewarm")]
    public bool prewarm;
    [ShowIf("prewarm"), HorizontalGroup("Object/Prewarm")]
    public int amount;
}