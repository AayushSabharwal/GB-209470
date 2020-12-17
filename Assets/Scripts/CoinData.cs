using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "CoinData", menuName = "Coin Type")]
public class CoinData : ScriptableObject
{
    [PreviewField, LabelWidth(60f), HorizontalGroup("L0")]
    public Sprite sprite;
    [LabelWidth(60f), HorizontalGroup("L0"), VerticalGroup("L0/C")]
    public Color color;
    [LabelWidth(60f), HorizontalGroup("L0"), VerticalGroup("L0/C")]
    public string poolTag;
    [MinValue(0.1f)]
    public float lifetime;
    [MinValue(1)]
    public int value;
}