using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Dash")]
    private float dashCooldown;
    [SerializeField, FoldoutGroup("Dash")]
    private Image dashCooldownImage;

    private float _dashTimer;
    private PlayerController _playerController;

    private void Start() {
        _playerController = GetComponent<PlayerController>();
    }

    private void Update() {
        if (_dashTimer > 0f)
            _dashTimer -= Time.deltaTime;

        UpdateUI();
    }

    private void UpdateUI() {
        dashCooldownImage.fillAmount = _dashTimer / dashCooldown;
    }

    public void Dash() {
        if (_dashTimer > 0f || !_playerController.Dash()) return;
        _dashTimer = dashCooldown;
    }
}