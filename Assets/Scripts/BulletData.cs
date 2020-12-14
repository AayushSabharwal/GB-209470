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
    
    [BoxGroup("Hitscan Options", VisibleIf = "@!isProjectile")]
    public float range;
    [BoxGroup("Hitscan Options")]
    public Gradient color;
    [BoxGroup("Hitscan Options")]
    public float size;
    
    public LayerMask collisionMask;
}