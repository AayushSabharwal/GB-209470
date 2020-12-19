using UnityEngine;

public abstract class ValueGenerator : ScriptableObject
{
    public abstract float Generate(float x);
}
