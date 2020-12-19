using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "QuadraticValueGenerator", menuName = "Value Generator/Quadratic")]
public class QuadraticValueGenerator : ValueGenerator
{
    [SerializeField, HorizontalGroup("H"), LabelWidth(10f)]
    private float a;
    [SerializeField, HorizontalGroup("H"), LabelWidth(10f)]
    private float b;
    [SerializeField, HorizontalGroup("H"), LabelWidth(10f)]
    private float c;

    [NonSerialized, ShowInInspector]
    private bool _test;
    [NonSerialized, ShowInInspector, ShowIf("_test"), OnValueChanged("@_value=Generate(_input)")]
    private float _input;
    [NonSerialized, ReadOnly, ShowInInspector, ShowIf("_test")]
    private float _value;

    public override float Generate(float x) {
        return a * x * x + b * x + c;
    }
}
