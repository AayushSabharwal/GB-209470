using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;

#endif

public interface IFloatingProbability
{
    float FloatingProbability { get; }
}

public static class IFloatingProbabilityExtensions
{
    public static float GetCummulativeWeight<T>(this IList<T> floating)
        where T : IFloatingProbability {
        float cumulativeWeight = 0;
        for (int i = 0; i < floating.Count; ++i)
            cumulativeWeight += floating[i].FloatingProbability;
        return cumulativeWeight;
    }

    public static int GetResult<T>(this IList<T> floating, System.Random random = null)
        where T : IFloatingProbability {
        float cumulativeWeight = floating.GetCummulativeWeight();

        if (cumulativeWeight == 0)
            return -1;

        float randomWeight = (random == null ? Random.value : (float) random.NextDouble()) * cumulativeWeight;
        for (int i = 0; i < floating.Count; ++i) {
            randomWeight -= floating[i].FloatingProbability;
            if (randomWeight < 0)
                return i;
        }

        return -1;
    }
}

public class FloatingProbabilitySettingsAttribute : System.Attribute
{
    public float Width { get; private set; } = 50f;
    public string Format { get; private set; } = "{0,8:##0.00}";

    public FloatingProbabilitySettingsAttribute() { }

    public FloatingProbabilitySettingsAttribute(float width) {
        Width = width;
    }

    public FloatingProbabilitySettingsAttribute(string format) {
        Format = format;
    }

    public FloatingProbabilitySettingsAttribute(float width, string format) {
        Width = width;
        Format = format;
    }
}

#if UNITY_EDITOR && ODIN_INSPECTOR
public class FloatingProbabilityDrawer<T> : OdinValueDrawer<T>
    where T : IFloatingProbability
{
    private GUIStyle _rightAlign;

    protected override bool CanDrawValueProperty(InspectorProperty property) {
        return property.Parent?.ChildResolver is IOrderedCollectionResolver &&
               property.Parent.ValueEntry.WeakSmartValue is IList<T>;
    }

    protected FloatingProbabilitySettingsAttribute Attribute;

    protected override void Initialize() {
        base.Initialize();

        Attribute = Property.GetAttribute<FloatingProbabilitySettingsAttribute>();
        if (Attribute == null)
            Attribute = new FloatingProbabilitySettingsAttribute();
    }

    protected override void DrawPropertyLayout(GUIContent label) {
        if (_rightAlign == null) {
            _rightAlign = new GUIStyle(GUI.skin.textField) {alignment = TextAnchor.MiddleRight};
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        CallNextDrawer(label);
        EditorGUILayout.EndVertical();

        IList<T> parentList = Property.Parent.ValueEntry.WeakSmartValue as IList<T>;
        float cummulativeWeight = parentList.GetCummulativeWeight();

        Sirenix.Utilities.Editor.GUIHelper.PushGUIEnabled(false);
        EditorGUILayout.LabelField("%", GUILayout.Width(12));

        if (cummulativeWeight == 0)
            EditorGUILayout.TextField("N/A", _rightAlign, GUILayout.Width(Attribute.Width));
        else
            EditorGUILayout
                .TextField(string.Format(Attribute.Format, ValueEntry.SmartValue.FloatingProbability / cummulativeWeight * 100f),
                           _rightAlign, GUILayout.Width(Attribute.Width));
        Sirenix.Utilities.Editor.GUIHelper.PopGUIEnabled();

        EditorGUILayout.EndHorizontal();
    }
}
#endif