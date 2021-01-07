using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class SharedDataManager : MonoBehaviour
{
    public DistanceThreshold[] repathRateScaling;

    [ShowInInspector, ReadOnly]
    public Vector2 playerStartPosition;
}

[Serializable]
public struct DistanceThreshold
{
    [HorizontalGroup, LabelWidth(100f), MinValue(0f)]
    public float distance;
    [HorizontalGroup, LabelWidth(100f)]
    public float value;
}
