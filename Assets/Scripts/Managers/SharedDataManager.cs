using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class SharedDataManager : SerializedMonoBehaviour
{
    [FormerlySerializedAs("pathfindingThresholds")]
    public DistanceThreshold[] repathRateScaling;

    [NonSerialized]
    public Vector2Int PlayerStartPosition;
}

public struct DistanceThreshold
{
    [HorizontalGroup, LabelWidth(100f), MinValue(0f)]
    public float Distance;
    [HorizontalGroup, LabelWidth(100f)]
    public float Value;
}
