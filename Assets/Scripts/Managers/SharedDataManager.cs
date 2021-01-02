using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class SharedDataManager : SerializedMonoBehaviour
{
    public DistanceThreshold[] repathRateScaling;

    [NonSerialized]
    public Vector2 PlayerStartPosition;
}

public struct DistanceThreshold
{
    [HorizontalGroup, LabelWidth(100f), MinValue(0f)]
    public float Distance;
    [HorizontalGroup, LabelWidth(100f)]
    public float Value;
}
