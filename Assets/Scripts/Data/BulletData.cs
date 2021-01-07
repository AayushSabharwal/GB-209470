using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet Type")]
public class BulletData : ScriptableObject
{
    public float damage;
    public string poolTag;
    public BulletData[] childProjectiles;
    public bool isProjectile;
    [BoxGroup("Projectile Options", VisibleIf = "@isProjectile")]
    public float speed;
    [BoxGroup("Projectile Options")]
    public float lifetime;
    [BoxGroup("Projectile Options")]
    public Vector3 scale;
    [BoxGroup("Projectile Options")]
    public Sprite sprite;
    [BoxGroup("Projectile Options")]
    public Color colour;
    [BoxGroup("Projectile Options")]
    public bool isExplosive;
    [BoxGroup("Projectile Options"), ShowIf("isExplosive"), MinValue(0.1f)]
    public float explosionRadius;
    
    [BoxGroup("Hitscan Options", VisibleIf = "@!isProjectile")]
    public float range;
    [BoxGroup("Hitscan Options")]
    public Gradient color;
    [BoxGroup("Hitscan Options")]
    public float size;
    
    public LayerMask collisionMask;
    public LayerMask damageMask;
}