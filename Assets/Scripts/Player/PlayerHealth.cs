using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    [SerializeField, InlineEditor, ValidateInput("@shield != null && shield.itemName == \"Shield\"")]
    private UpgradableShopItemData shield;
    [ShowInInspector, ReadOnly]
    public float CurShieldHp { get; private set; }
    [FoldoutGroup("Shield UI"), SerializeField]
    private Image shieldSlider;
    [FoldoutGroup("Shield UI"), SerializeField]
    private bool lerpShieldColour;
    [HideIfGroup("Shield UI/nonlerp", VisibleIf = "lerpShieldColour"), SerializeField]
    private Color baseShieldColour = Color.red;
    [ShowIfGroup("Shield UI/lerp", VisibleIf = "lerpShieldColour"), SerializeField]
    private Gradient shieldGradient;
    [SerializeField]
    private float outOfCombatTime = 4f;
    [SerializeField]
    private float outOfCombatHealRate = 3f;
    private float MaxShieldHp => shield.effectiveness[shield.Level].effectiveness;
    private float _outOfCombatTimer;
    private PlayerController _controller;

    private void Start() {
        _controller = GetComponent<PlayerController>();
        ReferenceManager.Inst.PlayerShooter.OnShoot += (_, __) => _outOfCombatTimer = outOfCombatTime;
    }

    public override void Respawned(float maxHealth) {
        CurShieldHp = MaxShieldHp;
        shieldSlider.transform.parent.gameObject.SetActive(MaxShieldHp > 0f);

        base.Respawned(maxHealth);
    }

    protected override void UpdateUI() {
        shieldSlider.fillAmount = CurShieldHp / MaxShieldHp;
        shieldSlider.color = lerpShieldColour ? shieldGradient.Evaluate(shieldSlider.fillAmount) : baseShieldColour;
        base.UpdateUI();
    }

    private void Update() {
        if (_outOfCombatTimer > 0f)
            _outOfCombatTimer -= Time.deltaTime;
        else {
            _outOfCombatTimer = 0f;
            Heal(outOfCombatHealRate * Time.deltaTime);
        }
    }

    public override void TakeDamage(float damage) {
        if (_controller.IsDashing)
            return;
        float shieldDamage = Mathf.Min(damage, CurShieldHp);
        _outOfCombatTimer = outOfCombatTime;
        CurShieldHp -= shieldDamage;
        CurHp -= damage - shieldDamage;
        InvokeOnTakeDamage(new DamageTakenArgs(damage));

        if (CurHp <= 0f) {
            CurHp = 0f;
            InvokeOnDeath();
        }

        UpdateUI();
    }
}