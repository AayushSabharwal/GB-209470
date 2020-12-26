using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [InlineEditor]
    public BulletData data;
    [NonSerialized]
    public float DamageMultiplier = 1f;
    protected ObjectPooler ObjectPooler;

    protected virtual void Start() {
        ObjectPooler = ReferenceManager.Inst.ObjectPooler;
    }

    protected void TryDamage(GameObject target) {
        if (target.TryGetComponent(out Health health)) {
            health.TakeDamage(data.damage * DamageMultiplier);
        }
    }
}