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
    
    [PropertySpace]
    
    [MinValue(0f), HorizontalGroup("L1"), LabelWidth(70f)]
    public float health;
    [MinValue(0f), HorizontalGroup("L1"), LabelWidth(110f)]
    public float damage;
    [MinValue(0.1f), HorizontalGroup("L2"), LabelWidth(70f)]
    public float speed;
    [MinValue(0.1f), HorizontalGroup("L2"), LabelWidth(110f)]
    public float approachRadius;
    [MinValue(0.1f)]
    public float pathfindingRadius;
    [MinMaxSlider(0, 50)]
    public Vector2Int coinsToDrop;
    [PropertyRange(0f, 1f)]
    public float ammoDropChance;
    [MinMaxSlider(0f, 5f)]
    public Vector2 scatterRadius;
}