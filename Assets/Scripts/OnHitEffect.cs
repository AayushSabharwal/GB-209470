using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class OnHitEffect : MonoBehaviour
{
    [SerializeField]
    private string poolTag;
    
    private ParticleSystem _particleSystem;
    private ParticleSystem.MainModule _main;
    private Light2D _light;
    private float _lifetimer;
    private ObjectPooler _objectPooler;
    
    private void Init() {
        _particleSystem = GetComponent<ParticleSystem>();
        _main = _particleSystem.main;
        _light = GetComponent<Light2D>();
        _lifetimer = 0f;
        _objectPooler = ReferenceManager.Inst.ObjectPooler;
    }

    private void Update() {
        _lifetimer -= Time.deltaTime;
        if(_lifetimer < 0f)
            _objectPooler.Return(poolTag, gameObject);
    }

    public void StartEffect(float radius, Color colour, Sprite sprite) {
        if(_particleSystem == null) Init();
        _main.startSize = radius * 2f;
        _light.pointLightOuterRadius = radius;
        _main.startColor = colour;
        _light.color = colour;
        _particleSystem.textureSheetAnimation.SetSprite(0, sprite);
        _lifetimer = _main.duration;
        gameObject.SetActive(true);
        _particleSystem.Play();
    }
}
