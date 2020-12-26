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

    private float MaxShieldHp => shield.effectiveness[shield.Level].effectiveness;

    public override void Respawned(float maxHealth) {
        CurShieldHp = MaxShieldHp;
        shieldSlider.gameObject.SetActive(MaxShieldHp > 0f);
        
        base.Respawned(maxHealth);
    }

    protected override void UpdateUI() {
        shieldSlider.fillAmount = CurShieldHp / MaxShieldHp;
        shieldSlider.color = lerpShieldColour ? shieldGradient.Evaluate(shieldSlider.fillAmount) : baseShieldColour;
        base.UpdateUI();
    }

    public override void TakeDamage(float damage) {
        float shieldDamage = Mathf.Min(damage, CurShieldHp);

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