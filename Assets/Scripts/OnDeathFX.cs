using UnityEngine;

public class OnDeathFX : MonoBehaviour
{
    [SerializeField]
    private string poolTag;

    private ParticleSystem _particleSystem;
    private float _lifetimer;

    private void Awake() {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnEnable() {
        _particleSystem.Play();
        _lifetimer = _particleSystem.main.duration;
    }

    private void Update() {
        _lifetimer -= Time.deltaTime;
        if (_lifetimer <= 0f)
            ReferenceManager.Inst.ObjectPooler.Return(poolTag, gameObject);
    }
}