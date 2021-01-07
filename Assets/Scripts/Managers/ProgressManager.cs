using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressManager : SerializedMonoBehaviour
{
    [SerializeField]
    private ScriptableObjectReferenceCache soReferenceCache;
    [SerializeField, ValidateInput("@ValidateMB()")]
    private List<MonoBehaviour> saveableMonoBehaviours;
    [SerializeField, LabelText("Saveable SOs"), ValidateInput("@ValidateSO()")]
    private List<ScriptableObject> saveableSOs;
    [SerializeField]
    private DataContainer defaultSave;
    [ShowInInspector, ReadOnly]
    public DataContainer Data { get; private set; }
    [ShowInInspector]
    private List<ISaveLoad> _saveableItems;

    private void Awake() {
        _saveableItems = new List<ISaveLoad>();
        for (int i = 0; i < saveableMonoBehaviours.Count; i++) {
            if (saveableMonoBehaviours[i] is ISaveLoad) {
                _saveableItems.Add(saveableMonoBehaviours[i] as ISaveLoad);
            }
        }
        
        
        for (int i = 0; i < saveableSOs.Count; i++) {
            if (saveableSOs[i] is ISaveLoad) {
                _saveableItems.Add(saveableSOs[i] as ISaveLoad);
            }
        }
        Load();
    }

    private void Start() {
        if (ReferenceManager.Inst.EnemySpawner != null)
            ReferenceManager.Inst.EnemySpawner.OnLevelEnd += Save;
    }

    private bool ValidateMB() {
        foreach (MonoBehaviour mb in saveableMonoBehaviours) {
            if (!(mb is ISaveLoad))
                return false;
        }

        return true;
    }
    
    private bool ValidateSO() {
        foreach (ScriptableObject mb in saveableSOs) {
            if (!(mb is ISaveLoad))
                return false;
        }

        return true;
    }

    [Button("Find All ISaveLoad")]
    private void FindAllISaveLoad() {
        _saveableItems = new List<ISaveLoad>();
        GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject g in roots) {
            ISaveLoad[] isl = g.GetComponentsInChildren<ISaveLoad>();
            foreach (ISaveLoad sl in isl) {
                _saveableItems.Add(sl);
            }
        }
    }

    public void Save() {
        for(int i = 0; i < _saveableItems.Count; i++) 
            _saveableItems[i].Save();
        
        SerializationContext context = new SerializationContext
                                       {
                                           StringReferenceResolver = soReferenceCache
                                       };

        byte[] data = SerializationUtility.SerializeValue(Data, DataFormat.Binary, context);
        File.WriteAllBytes(Application.persistentDataPath + "/savegame.sgm", data);
    }

    private void Load() {
        if (File.Exists(Application.persistentDataPath + "/savegame.sgm")) {
            byte[] data = File.ReadAllBytes(Application.persistentDataPath + "/savegame.sgm");
            DeserializationContext context = new DeserializationContext
                                             {
                                                 StringReferenceResolver = soReferenceCache
                                             };
            Data = SerializationUtility.DeserializeValue<DataContainer>(data, DataFormat.Binary, context);
        }
        else
            Data = defaultSave;

        
        for(int i = 0; i < _saveableItems.Count; i++) 
            _saveableItems[i].Load();
    }

    [Button]
    private void DeleteSave() {
        if (File.Exists(Application.persistentDataPath + "/savegame.sgm"))
            File.Delete(Application.persistentDataPath + "/savegame.sgm");
    }
}


public class DataContainer
{
    public Dictionary<AmmoType, AmmoTracker> Ammo;
    public GunData[] EquippedGuns;
    public Dictionary<GunShopItemData, bool> AllGuns;
    [FloatingProbabilitySettings]
    public List<AmmoDropData> DroppableAmmo;
    public Dictionary<string, int> UpgradableItemLevels;
    public int Currency;
    public int Level;
}