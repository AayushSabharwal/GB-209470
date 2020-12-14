using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shooter : MonoBehaviour
{
    [InlineEditor, SerializeField]
    private GunData gun;
    [SceneObjectsOnly, SerializeField]
    private Transform shootPoint;

    private float _shotTimer;
    private ObjectPooler _objectPooler;

    public EventHandler OnShoot;

    private void Start() {
        _objectPooler = ReferenceManager.Inst.ObjectPooler;
    }

    private void Update() {
        if (_shotTimer > 0f)
            _shotTimer -= Time.deltaTime;
    }

    public void Shoot() {
        if (_shotTimer > 0f)
            return;

        for (int i = 0; i < gun.shots.Length; i++) {
            MakeBullets(gun.shots[i].bullet.poolTag,
                        gun.shots[i].offsetAngle,
                        gun.shots[i].applySpread ? gun.spreadAngle : 0f,
                        gun.shots[i].groupSize);
        }

        _shotTimer = 1f / gun.fireRate;
        OnShoot?.Invoke(this, EventArgs.Empty);
    }

    private void MakeBullets(string poolTag, float offsetAngle, float spread, int groupSize) {
        for (int j = 0; j < groupSize; j++) {
            GameObject bullet = _objectPooler.Request(poolTag);
            bullet.transform.position = Quaternion.AngleAxis(offsetAngle, Vector3.forward) * shootPoint.position;
            bullet.transform.rotation = shootPoint.rotation *
                                        Quaternion.AngleAxis(offsetAngle + Random.Range(-spread, spread),
                                                             Vector3.forward);
            bullet.SetActive(true);
        }
    }
}