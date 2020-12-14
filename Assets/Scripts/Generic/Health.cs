using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField]
    private float maxHp;

    [ShowInInspector]
    [ReadOnly]
    public float CurHp { get; private set; }

    [SerializeField]
    private bool hasUI;

    [ShowIfGroup("hasUI"), BoxGroup("hasUI/HPUI"), SerializeField]
    private Image _sliderImage;
    [ShowIfGroup("hasUI"), BoxGroup("hasUI/HPUI"), SerializeField]
    private bool lerpColour;
    [BoxGroup("hasUI/HPUI"), ShowIf("@!lerpColour"), SerializeField]
    private Color baseColour = Color.red;
    [BoxGroup("hasUI/HPUI"), ShowIf("lerpColour"), SerializeField]
    private Gradient colourGradient;

    [ShowInInspector]
    public bool test;

    [ShowIf("test"), OnValueChanged("@(_testHp-CurHp > 0f) ? Heal(_testHp-CurHp) : TakeDamage(CurHp-_testHp)"),
     PropertyRange(0f, "maxHp"), ShowInInspector]
    private float _testHp;

    public event EventHandler<DamageTakenArgs> OnTakeDamage;
    public event EventHandler<DamageTakenArgs> OnHeal;
    public event EventHandler OnDeath;

    private void Start()
    {
        CurHp = maxHp;
    }

    private void UpdateUI()
    {
        if (!hasUI)
            return;

        _sliderImage.fillAmount = CurHp / maxHp;
        _sliderImage.color = lerpColour ? colourGradient.Evaluate(_sliderImage.fillAmount) : baseColour;
    }

    public void TakeDamage(float damage)
    {
        CurHp -= damage;
        OnTakeDamage?.Invoke(this, new DamageTakenArgs(damage));

        if (CurHp <= 0f)
        {
            CurHp = 0f;
            OnDeath?.Invoke(this, EventArgs.Empty);
        }

        UpdateUI();
    }

    public void Heal(float damage)
    {
        damage = Mathf.Min(damage, maxHp - CurHp);
        CurHp += damage;

        OnHeal?.Invoke(this, new DamageTakenArgs(damage));

        UpdateUI();
    }
}

public class DamageTakenArgs : EventArgs
{
    public readonly float Damage;

    public DamageTakenArgs(float damage)
    {
        Damage = damage;
    }
}