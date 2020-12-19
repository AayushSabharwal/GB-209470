using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour, ISaveLoad
{
    [SerializeField]
    private List<EnemySpawnData> enemies;
    [SerializeField, MinValue(0.1f)]
    private float spawnInterval;

    private MapGenerator _mapGenerator;
    private ObjectPooler _objectPooler;
    private float _spawnTimer;
    private int _level;

    private void Start() {
        _mapGenerator = ReferenceManager.Inst.MapGenerator;
        _objectPooler = ReferenceManager.Inst.ObjectPooler;

        for (int i = 0; i < enemies.Count; i++) {
            enemies[i].spawnedCount = 0;
            enemies[i].SpawnAmount = Mathf.Min((int) enemies[i].spawnAmountGenerator.Generate(_level),
                                               enemies[i].spawnCap);
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
            if (p > enemies[i].probabilityMultiplier) {
                p -= enemies[i].probabilityMultiplier;
                continue;
            }

            enemies[i].spawnedCount += 1;
            if (enemies[i].spawnedCount >= enemies[i].SpawnAmount)
                indexToRemove = i;

            GameObject g = _objectPooler.Request(enemies[i].enemy.poolTag);
            Vector2Int pos = _mapGenerator.Walkable[Random.Range(0, _mapGenerator.Walkable.Count)];
            g.transform.position = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0f);
            g.GetComponent<Enemy>().data = enemies[i].enemy;
            g.SetActive(true);
            break;
        }

        if (indexToRemove == -1) return;

        float divideBy = 1f - enemies[indexToRemove].probabilityMultiplier;
        enemies.RemoveAt(indexToRemove);
        for (int i = 0; i < enemies.Count; i++)
            enemies[i].probabilityMultiplier /= divideBy;
    }

    public void Save() {
        ReferenceManager.Inst.ProgressManager.Data.level = _level + 1;
    }

    public void Load() {
        _level = ReferenceManager.Inst.ProgressManager.Data.level;
    }
}

[Serializable]
public class EnemySpawnData : IFloatingProbability
{
    [InlineEditor]
    public EnemyData enemy;
    [InlineEditor]
    public ValueGenerator spawnAmountGenerator;
    [NonSerialized]
    public int SpawnAmount;
    [HorizontalGroup, LabelWidth(70f)]
    public int spawnCap;
    [HorizontalGroup, LabelWidth(120f)]
    public float probabilityMultiplier;

    [HideInInspector]
    public int spawnedCount;
    public float FloatingProbability => probabilityMultiplier;
}