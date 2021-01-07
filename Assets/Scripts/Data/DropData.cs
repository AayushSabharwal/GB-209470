using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "DropData", menuName = "Drop")]
public class DropData : ScriptableObject
{
    [PreviewField, LabelWidth(60f), HorizontalGroup("L0")]
    public Sprite sprite;
    [LabelWidth(60f), HorizontalGroup("L0"), VerticalGroup("L0/C")]
    public Color color;
    [LabelWidth(60f), HorizontalGroup("L0"), VerticalGroup("L0/C")]
    public string poolTag;
    public Vector3 scale;
    public Vector2 colliderDimensions;
    [MinValue(0.1f)]
    public float lifetime;
    [MinValue(1)]
    public int value;
    [FoldoutGroup("Audio")]
    public AudioClip pickupSound;
    [FoldoutGroup("Audio")]
    public float pitch;
    [FoldoutGroup("Audio")]
    public float pitchVariance;
}