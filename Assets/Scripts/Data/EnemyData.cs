using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Type")]
public class EnemyData : ScriptableObject
{
    [PreviewField, LabelWidth(60f), HorizontalGroup("L0")]
    public Sprite sprite;
    [LabelWidth(60f), HorizontalGroup("L0"), VerticalGroup("L0/C")]
    public Color color;
    [LabelWidth(60f), HorizontalGroup("L0"), VerticalGroup("L0/C")]
    public string poolTag;
    [BoxGroup("Positioning")]
    public Vector3 scale;
    [BoxGroup("Positioning")]
    public Vector2 boxColliderOffset;
    [BoxGroup("Positioning")]
    public Vector2 boxColliderSize;
    [MinValue(0f), HorizontalGroup("L1"), LabelWidth(110f), PropertySpace]
    public float health;
    [MinValue(0.1f), HorizontalGroup("L1"), LabelWidth(110f), PropertySpace]
    public float speed;
    [MinValue(0.1f), HorizontalGroup("L2"), LabelWidth(110f)]
    public float approachRadius;
    [MinValue(0.1f), HorizontalGroup("L2"), LabelWidth(110f)]
    public float pathfindingRadius;
    
    [BoxGroup("Attack")]
    public bool isMelee;
    [MinValue(0f), HorizontalGroup("Attack/L3", VisibleIf = "isMelee"), LabelWidth(110f)]
    public float damage;
    [MinValue(0.1f), HorizontalGroup("Attack/L3"), LabelWidth(110f)]
    public float hitRate;
    [BoxGroup("Attack")]
    public bool isRanged;
    [InlineEditor, ShowIf("isRanged"), BoxGroup("Attack")]
    public GunData gun;
    [ShowIf("isRanged"), BoxGroup("Attack")]
    public Vector3 shootPointPosition;

    [MinMaxSlider(0, 50), BoxGroup("Drops")]
    public Vector2Int coinsToDrop;
    [PropertyRange(0f, 1f),BoxGroup("Drops")]
    public float ammoDropChance;
    [MinMaxSlider(1, 10), BoxGroup("Drops")]
    public Vector2Int ammoDropCount;
    [MinMaxSlider(0f, 5f), BoxGroup("Drops")]
    public Vector2 scatterRadius;
}