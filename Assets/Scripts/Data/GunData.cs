using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun Type")]
public class GunData : ScriptableObject, ISerializeReferenceByAssetGuid
{
    [PreviewField]
    public Sprite image;
    [MinValue(0.1f)]
    public float fireRate;
    [MinValue(0f), SuffixLabel("Degrees")]
    public float spreadAngle;
    public bool isInfiniteAmmo;
    [EnumToggleButtons, HideIf("isInfiniteAmmo")]
    public AmmoType ammoType;
    [HideIf("isInfiniteAmmo")]
    public int clipSize;
    [HideIf("isInfiniteAmmo")]
    public float reloadTime;
    [FoldoutGroup("Audio")]
    public AudioClip shootSound;
    [FoldoutGroup("Audio")]
    public float pitch;
    [FoldoutGroup("Audio")]
    public float pitchVariation;
    
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

public enum AmmoType
{
    Handgun,
    Automatic,
    Shell,
    Rocket,
    Energy
}