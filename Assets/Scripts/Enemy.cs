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
    private Health _health;
    private Transform _player;
    
    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _aiPath = GetComponent<AIPath>();
        _destinationSetter = GetComponent<AIDestinationSetter>();
        _health = GetComponent<Health>();
    }

    private void Start() {
        _player = ReferenceManager.Inst.Player;
        _destinationSetter.target = _player;
    }

    private void OnEnable() {
        _spriteRenderer.sprite = data.sprite;
        _spriteRenderer.color = data.color;
        
        _aiPath.radius = data.pathfindingRadius;
        _aiPath.endReachedDistance = data.approachRadius;
        _aiPath.maxSpeed = data.speed;
        
        _health.Respawned(data.health);
    }

    private void Update() {
        for (int i = 0; i < pathUpdateIntervals.data.Length; i++) {
            if (!((transform.position - _player.position).sqrMagnitude >
                  pathUpdateIntervals.data[i].Distance * pathUpdateIntervals.data[i].Distance)) continue;
            
            _aiPath.repathRate = pathUpdateIntervals.data[i].Value;
            break;
        }
    }
}