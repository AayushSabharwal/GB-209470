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
    public bool hasAudio;
    [ShowIfGroup("Audio/hasAudio")]
    public AudioClip shootSound;
    [ShowIfGroup("Audio/hasAudio")]
    public float pitch;
    [ShowIfGroup("Audio/hasAudio")]
    public float pitchVariation;
    [FoldoutGroup("Lighting")]
    public Color muzzleFlashColour;
    [FoldoutGroup("Lighting")]
    public float muzzleFlashDuration;
    [FoldoutGroup("Lighting"), PropertyRange(0f, 360f), MaxValue("muzzleFlashOuterAngle")]
    public float muzzleFlashInnerAngle;
    [FoldoutGroup("Lighting"), PropertyRange(0f, 360f), MinValue("muzzleFlashInnerAngle")]
    public float muzzleFlashOuterAngle;
    [FoldoutGroup("Lighting"), MinValue(0f)]
    public float muzzleFlashInnerRadius;
    [FoldoutGroup("Lighting"), MinValue("muzzleFlashInnerRadius")]
    public float muzzleFlashOuterRadius;
    
    [ValidateInput("@shots.Length > 0", DefaultMessage = "Must have at least one shot"), SerializeField]
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