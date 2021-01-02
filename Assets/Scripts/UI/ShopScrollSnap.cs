using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ShopScrollSnap : MonoBehaviour
{
    [SerializeField]
    private RectTransform content;
    [SerializeField]
    private float shiftWidth;
    [SerializeField]
    private float tweenDuration;
    [SerializeField]
    private Ease easeType;
    [SerializeField]
    private int panels;
    [SerializeField]
    private Button fButton;
    [SerializeField]
    private Button bButton;
    
    private int _currentPanel;
    private bool _isTweening;
    
    private void Start() {
        bButton.interactable = false;
        fButton.interactable = true;
        content.anchoredPosition = new Vector2(0f, content.anchoredPosition.y);
    }

    public void Forward() {
        if (_currentPanel == panels - 1 || _isTweening) return;
        _currentPanel++;
        _isTweening = true;
        Tween tween = content.DOAnchorPosX(-_currentPanel * shiftWidth, tweenDuration).SetEase(easeType);
        tween.onComplete += () => _isTweening = false;
        if (_currentPanel == panels - 1)
            fButton.interactable = false;
        bButton.interactable = true;
    }
    
    public void Backward() {
        if (_currentPanel == 0 || _isTweening) return;
        _currentPanel--;
        _isTweening = true;
        Tween tween = content.DOAnchorPosX(-_currentPanel * shiftWidth, tweenDuration).SetEase(easeType);
        tween.onComplete += () => _isTweening = false;
        if (_currentPanel == 0)
            bButton.interactable = false;
        fButton.interactable = true;
    }
}
