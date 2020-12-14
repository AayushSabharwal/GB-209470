using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(AIDestinationSetter))]
[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    [InlineEditor, SerializeField]
    private EnemyData data;

    private SpriteRenderer _spriteRenderer;
    private AIPath _aiPath;
    private AIDestinationSetter _destinationSetter;
    private Health _health;
    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _aiPath = GetComponent<AIPath>();
        _destinationSetter = GetComponent<AIDestinationSetter>();
        _health = GetComponent<Health>();
    }

    private void Start() {
        _destinationSetter.target = ReferenceManager.Inst.Player;
    }

    private void OnEnable() {
        _spriteRenderer.sprite = data.sprite;
        _spriteRenderer.color = data.color;
        
        _aiPath.radius = data.pathfindingRadius;
        _aiPath.endReachedDistance = data.approachRadius;
        _aiPath.maxSpeed = data.speed;
        
        _health.Respawned(data.health);
    }
}