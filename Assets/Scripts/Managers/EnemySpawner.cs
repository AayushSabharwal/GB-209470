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
    private Dictionary<int, Enemy> _enemyMap;
    private int EnemiesLeftToKill {
        get => _enemiesLeftToKill;
        set {
            _enemiesLeftToKill = value;
            if (_enemiesLeftToKill == 0)
                OnLevelEnd?.Invoke();
        }
    }
    private float _spawnTimer;
    private int _level;
    private int _enemiesLeftToKill;
    private bool _isPaused;
    private bool _isPlayerDead;

    public event Action OnLevelEnd;

    private void Start() {
        _mapGenerator = ReferenceManager.Inst.MapGenerator;
        _objectPooler = ReferenceManager.Inst.ObjectPooler;
        _enemyMap = new Dictionary<int, Enemy>();

        for (int i = 0; i < enemies.Count; i++) {
            enemies[i].spawnedCount = 0;
            enemies[i].SpawnAmount = Mathf.Min((int) enemies[i].spawnAmountGenerator.Generate(_level),
                                               enemies[i].spawnCap);
            _enemiesLeftToKill = enemies[i].SpawnAmount;
        }

        _isPaused = false;
        _isPlayerDead = false;
        ReferenceManager.Inst.UIManager.OnPause += OnPause;
        ReferenceManager.Inst.PlayerHealth.OnDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath(object sender, EventArgs e) {
        _level--;
        _isPlayerDead = true;
    }

    private void OnPause(bool isPaused) {
        _isPaused = isPaused;
    }

    private void Update() {
        if (_isPaused || _isPlayerDead) return;

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

            GetEnemy(g).data = enemies[i].enemy;
            g.SetActive(true);
            break;
        }

        if (indexToRemove == -1) return;

        float divideBy = 1f - enemies[indexToRemove].probabilityMultiplier;
        enemies.RemoveAt(indexToRemove);
        for (int i = 0; i < enemies.Count; i++)
            enemies[i].probabilityMultiplier /= divideBy;
    }

    private Enemy GetEnemy(GameObject g) {
        int instanceId = g.GetInstanceID();
        if (!_enemyMap.ContainsKey(instanceId)) {
            _enemyMap[instanceId] = g.GetComponent<Enemy>();
            g.GetComponent<Health>().OnDeath += (_, __) => EnemiesLeftToKill--;
        }

        return _enemyMap[instanceId];
    }

    public void Save() {
        ReferenceManager.Inst.ProgressManager.Data.Level = _level + 1;
    }

    public void Load() {
        _level = ReferenceManager.Inst.ProgressManager.Data.Level;
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