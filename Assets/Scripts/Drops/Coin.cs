using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Coin : Drop
{
    private CurrencyManager _currencyManager;
    [SerializeField, ValidateInput("@magnet != null && magnet.itemName == \"Magnet\"")]
    private UpgradableShopItemData magnet;

    private Rigidbody2D _rb;
    private Transform _player;
    
    protected override void Awake() {
        base.Awake();
        _currencyManager = ReferenceManager.Inst.CurrencyManager;
        _player = ReferenceManager.Inst.PlayerHealth.transform;
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        Vector2 dir = (Vector2)_player.position - _rb.position;
        _rb.MovePosition(_rb.position + dir * magnet.effectiveness[magnet.Level].effectiveness / dir.sqrMagnitude * Time.fixedDeltaTime);
    }

    protected override void OnPickup() {
        _currencyManager.AddCurrency(data.value);
    }
}