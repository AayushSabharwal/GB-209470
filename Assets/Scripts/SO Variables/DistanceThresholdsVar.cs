
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Distance Thresholds Variable", menuName = "SOVars/Distance Threshold")]
public class DistanceThresholdsVar : SOVarBase<DistanceThreshold[]>
{ }

public struct DistanceThreshold
{
    [HorizontalGroup, LabelWidth(100f), MinValue(0f)]
    public float Distance;
    [HorizontalGroup, LabelWidth(100f)]
    public float Value;
}
