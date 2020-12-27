using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [NonSerialized]
    public float MaxHp = 10f;

    [ShowInInspector]
    [ReadOnly]
    public float CurHp { get; protected set; }

    [SerializeField]
    protected bool hasUI;

    [FoldoutGroup("HP UI", VisibleIf = "hasUI"), SerializeField]
    protected Image sliderImage;
    [FoldoutGroup("HP UI"), SerializeField]
    protected bool lerpColour;
    [HideIfGroup("HP UI/nonlerp", VisibleIf = "lerpColour"), SerializeField]
    protected Color baseColour = Color.red;
    [ShowIfGroup("HP UI/lerp", VisibleIf = "lerpColour"), SerializeField]
    protected Gradient colourGradient;

    [ShowInInspector]
    public bool test;

    [ShowIf("test"), OnValueChanged("@(_testHp-CurHp > 0f) ? Heal(_testHp-CurHp) : TakeDamage(CurHp-_testHp)"),
     PropertyRange(0f, "_maxHp"), ShowInInspector]
    private float _testHp;

    public event EventHandler<DamageTakenArgs> OnTakeDamage;
    public event EventHandler<DamageTakenArgs> OnHeal;
    public event EventHandler OnDeath;

    public virtual void Respawned(float maxHealth) {
        MaxHp = maxHealth;
        CurHp = maxHealth;
        UpdateUI();
    }

    protected virtual void UpdateUI() {
        if (!hasUI)
            return;

        sliderImage.fillAmount = CurHp / MaxHp;
        sliderImage.color = lerpColour ? colourGradient.Evaluate(sliderImage.fillAmount) : baseColour;
    }

    protected void InvokeOnTakeDamage(DamageTakenArgs d) {
        OnTakeDamage?.Invoke(this, d);
    }

    protected void InvokeOnDeath() {
        OnDeath?.Invoke(this, EventArgs.Empty);
    }

    public virtual void TakeDamage(float damage) {
        if (CurHp <= 0f)
            return;
        
        CurHp -= damage;
        InvokeOnTakeDamage(new DamageTakenArgs(damage));

        if (CurHp <= 0f) {
            CurHp = 0f;
            InvokeOnDeath();
        }

        UpdateUI();
    }

    public void Heal(float damage) {
        damage = Mathf.Min(damage, MaxHp - CurHp);
        CurHp += damage;

        OnHeal?.Invoke(this, new DamageTakenArgs(damage));

        UpdateUI();
    }
}

public class DamageTakenArgs : EventArgs
{
    public readonly float Damage;

    public DamageTakenArgs(float damage) {
        Damage = damage;
    }
}