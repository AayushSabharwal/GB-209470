using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField, InlineEditor]
    protected BulletData data;

    protected ObjectPooler ObjectPooler;

    private void Start() {
        ObjectPooler = ReferenceManager.Inst.ObjectPooler;
    }

    protected void TryDamage(GameObject target) {
        if (target.TryGetComponent(out Health health)) {
            health.TakeDamage(data.damage);
        }
    }
}
