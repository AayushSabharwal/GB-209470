using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private List<EnemySpawnData> enemies;
    [SerializeField, MinValue(0.1f)]
    private float spawnInterval;

    private MapGenerator _mapGenerator;
    private ObjectPooler _objectPooler;
    private float _spawnTimer;

    private void Start() {
        _mapGenerator = ReferenceManager.Inst.MapGenerator;
        _objectPooler = ReferenceManager.Inst.ObjectPooler;

        for (int i = 0; i < enemies.Count; i++) {
            enemies[i].spawnedCount = 0;
        }
    }

    private void Update() {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0f) {
            SpawnEnemy();
            _spawnTimer = spawnInterval;
        }
    }

    private void SpawnEnemy() {
        float p = Random.value;
        int indexToRemove = -1;
        for (int i = 0; i < enemies.Count; i++) {
            if (p > enemies[i].probability) {
                p -= enemies[i].probability;
                continue;
            }

            enemies[i].spawnedCount += 1;
            if (enemies[i].spawnedCount >= enemies[i].maxSpawnAmount) {
                indexToRemove = i;
            }
            GameObject g = _objectPooler.Request(enemies[i].enemy.poolTag);
            Vector2Int pos = _mapGenerator.Walkable[Random.Range(0, _mapGenerator.Walkable.Count)];
            g.transform.position = new Vector3(pos.x, pos.y, 0f);
            g.GetComponent<Enemy>().data = enemies[i].enemy;
            g.SetActive(true);
            break;
        }

        if (indexToRemove == -1) return;
        
        float divideBy = 1f - enemies[indexToRemove].probability;
        enemies.RemoveAt(indexToRemove);
        for (int i = 0; i < enemies.Count; i++) {
            enemies[i].probability /= divideBy;
        }
        
    }
}

[Serializable]
public class EnemySpawnData
{
    [InlineEditor]
    public EnemyData enemy;
    [PropertyRange(0f, 1f)]
    public float probability;
    public int maxSpawnAmount;

    [HideInInspector]
    public int spawnedCount;
}