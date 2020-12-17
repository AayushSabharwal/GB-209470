using System;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(AIDestinationSetter))]
[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    [InlineEditor]
    public EnemyData data;
    [SerializeField]
    private DistanceThresholdsVar pathUpdateIntervals;
    
    private SpriteRenderer _spriteRenderer;
    private AIPath _aiPath;
    private AIDestinationSetter _destinationSetter;
    public Health Health { get; private set; }
    private Transform _player;
    private ObjectPooler _objectPooler;
    
    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _aiPath = GetComponent<AIPath>();
        _destinationSetter = GetComponent<AIDestinationSetter>();
        Health = GetComponent<Health>();
        
        Health.OnDeath += OnDeath;
    }
    
    private void Start() {
        _player = ReferenceManager.Inst.Player;
        _objectPooler = ReferenceManager.Inst.ObjectPooler;
        
        _destinationSetter.target = _player;
    }

    private void OnEnable() {
        if(data == null)    return;
        
        _spriteRenderer.sprite = data.sprite;
        _spriteRenderer.color = data.color;
        
        _aiPath.radius = data.pathfindingRadius;
        _aiPath.endReachedDistance = data.approachRadius;
        _aiPath.maxSpeed = data.speed;
        
        Health.Respawned(data.health);
    }

    private void Update() {
        for (int i = 0; i < pathUpdateIntervals.data.Length; i++) {
            if (!((transform.position - _player.position).sqrMagnitude >
                  pathUpdateIntervals.data[i].Distance * pathUpdateIntervals.data[i].Distance)) continue;
            
            _aiPath.repathRate = pathUpdateIntervals.data[i].Value;
            break;
        }
    }
    
    protected virtual void OnDeath(object sender, EventArgs e) {
        _objectPooler.Return(data.poolTag, gameObject);
    }
}