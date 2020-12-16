﻿using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun Type")]
public class GunData : ScriptableObject
{
    [MinValue(0.1f)]
    public float fireRate;
    [MinValue(0f), SuffixLabel("Degrees")]
    public float spreadAngle;
    [ValidateInput("@shots.Length > 0", DefaultMessage = "Must have at least one shot")]
    public Shot[] shots;
}

[Serializable]
public class Shot
{
    [InlineEditor]
    public BulletData bullet;
    [HorizontalGroup("Line1"), MinValue(0f), SuffixLabel("Degrees"), LabelWidth(80f)]
    public float offsetAngle;
    [HorizontalGroup("Line1"), MinValue(1), LabelWidth(80f)]
    public int groupSize;
    public bool applySpread;
}