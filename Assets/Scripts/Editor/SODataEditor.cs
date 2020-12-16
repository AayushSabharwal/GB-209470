using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class SODataEditor : OdinMenuEditorWindow
{
    [EnumToggleButtons, ShowInInspector, HideLabel, OnValueChanged("@_rebuildTree = true")]
    private SOType _soType;

    private bool _rebuildTree;
    private bool _canDrawEnum;
    private int _enumIndex;

    private DrawSelected<EnemyData> _drawEnemies = new DrawSelected<EnemyData>();
    private DrawSelected<GunData> _drawGuns = new DrawSelected<GunData>();
    private DrawSelected<BulletData> _drawBullets = new DrawSelected<BulletData>();

    private string _enemiesPath = "Assets/SO/Enemies";
    private string _gunsPath = "Assets/SO/Guns";
    private string _bulletsPath = "Assets/So/Bullets";

    [MenuItem("Tools/SO Data Editor")]
    private static void OpenWindow() {
        GetWindow<SODataEditor>().Show();
    }

    protected override void Initialize() {
        _canDrawEnum = false;
        _drawEnemies.SetPath(_enemiesPath);
        _drawGuns.SetPath(_gunsPath);
        _drawBullets.SetPath(_bulletsPath);
    }

    protected override void OnGUI() {
        if (_rebuildTree && Event.current.type == EventType.Layout) {
            ForceMenuTreeRebuild();
            _rebuildTree = false;
        }

        if (_canDrawEnum)
            DrawEditor(_enumIndex);

        base.OnGUI();
        _canDrawEnum = true;
    }

    protected override void DrawEditors() {
        switch (_soType) {
            case SOType.Enemy:
                _drawEnemies.SetSelected(MenuTree.Selection.SelectedValue);
                break;
            case SOType.Gun:
                _drawGuns.SetSelected(MenuTree.Selection.SelectedValue);
                break;
            case SOType.Bullet:
                _drawBullets.SetSelected(MenuTree.Selection.SelectedValue);
                break;
        }

        DrawEditor((int) _soType);
    }

    protected override IEnumerable<object> GetTargets() {
        List<object> targets = new List<object>();
        targets.Add(_drawEnemies);
        targets.Add(_drawGuns);
        targets.Add(_drawBullets);
        targets.Add(base.GetTarget());

        _enumIndex = targets.Count - 1;

        return targets;
    }

    protected override OdinMenuTree BuildMenuTree() {
        OdinMenuTree tree = new OdinMenuTree();

        switch (_soType) {
            case SOType.Enemy:
                tree.AddAllAssetsAtPath("Bullet Data", _enemiesPath, typeof(EnemyData));
                break;

            case SOType.Gun:
                tree.AddAllAssetsAtPath("Gun Data", _gunsPath, typeof(GunData));
                break;

            case SOType.Bullet:
                tree.AddAllAssetsAtPath("Bullet Data", _bulletsPath, typeof(BulletData));
                break;
        }

        return tree;
    }
}

public enum SOType
{
    Enemy,
    Gun,
    Bullet
}

public class DrawSelected<T> where T : ScriptableObject
{
    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    public T Selected;

    [LabelWidth(100)]
    [PropertyOrder(-1)]
    [HorizontalGroup("Horizontal")]
    public string NameForNew;

    private string _path;

    [HorizontalGroup("Horizontal")]
    [GUIColor(0.7f, 0.7f, 1f)]
    [Button]
    public void CreateNew() {
        if (NameForNew == "")
            return;

        T newItem = ScriptableObject.CreateInstance<T>();

        if (_path == "")
            _path = "Assets/";

        AssetDatabase.CreateAsset(newItem, _path + "\\" + NameForNew + ".asset");
        AssetDatabase.SaveAssets();

        NameForNew = "";
    }

    [HorizontalGroup("Horizontal")]
    [GUIColor(1f, 0.7f, 0.7f)]
    [Button]
    public void DeleteSelected() {
        if (Selected == null) return;
        string path = AssetDatabase.GetAssetPath(Selected);
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.SaveAssets();
    }

    public void SetSelected(object item) {
        T attempt = item as T;
        if (attempt != null)
            Selected = attempt;
    }

    public void SetPath(string path) {
        _path = path;
    }
}