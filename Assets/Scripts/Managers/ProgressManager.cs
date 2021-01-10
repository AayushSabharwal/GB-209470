using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
#else
using OdinSerializer;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressManager : MonoBehaviour
{
    [SerializeField]
    private ScriptableObjectReferenceCache soReferenceCache;
    [SerializeField, ValidateInput("@ValidateMB()")]
    private List<MonoBehaviour> saveableMonoBehaviours;
    [SerializeField, LabelText("Saveable SOs"), ValidateInput("@ValidateSO()")]
    private List<ScriptableObject> saveableSOs;
    // [SerializeField]
    // private DataContainer defaultSave;
    [SerializeField]
    private DisplayDataContainer defaultSaveGame;
    [ShowInInspector, ReadOnly]
    public DataContainer Data => _data;
    [ShowInInspector]
    private List<ISaveLoad> _saveableItems;
    private DataContainer _data;

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
        if (ReferenceManager.Inst.PlayerHealth != null)
            ReferenceManager.Inst.PlayerHealth.OnDeath += (_, __) => {
                                                              DeleteSave();
                                                              SaveHighScore();
                                                          };
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

        byte[] data = SerializationUtility.SerializeValue(_data, DataFormat.Binary, context);
        File.WriteAllBytes(Application.persistentDataPath + "/savegame.sgm", data);

        SaveHighScore();
    }

    private void SaveHighScore() {
        int bestLevel = 0;
        if (File.Exists(Application.persistentDataPath + "/highscore.sgm")) {
            byte[] hiscore = File.ReadAllBytes(Application.persistentDataPath + "/highscore.sgm");
            bestLevel = SerializationUtility.DeserializeValue<int>(hiscore, DataFormat.Binary);
        }

        if (bestLevel < _data.Level) {
            byte[] hiscore = SerializationUtility.SerializeValue(_data.Level, DataFormat.Binary);
            File.WriteAllBytes(Application.persistentDataPath + "/highscore.sgm", hiscore);
        }
    }

    private void Load() {
        if (File.Exists(Application.persistentDataPath + "/savegame.sgm")) {
            byte[] data = File.ReadAllBytes(Application.persistentDataPath + "/savegame.sgm");
            DeserializationContext context = new DeserializationContext
                                             {
                                                 StringReferenceResolver = soReferenceCache
                                             };
            _data = SerializationUtility.DeserializeValue<DataContainer>(data, DataFormat.Binary, context);
        }
        else
            _data = defaultSaveGame;
        
        for(int i = 0; i < _saveableItems.Count; i++) 
            _saveableItems[i].Load();
    }

    [Button]
    private void DeleteSave() {
        if (File.Exists(Application.persistentDataPath + "/savegame.sgm"))
            File.Delete(Application.persistentDataPath + "/savegame.sgm");
    }
}

[Serializable]
public class DisplayDataContainer
{
    public List<AmmoTrackerSaveData> ammo;
    public GunData[] equippedGuns;
    public List<AllGunsSaveData> allGuns;
    public List<AmmoDropData> droppableAmmo;
    public List<UpgradableItemLevelSaveData> upgradableItemLevels;
    public int currency;
    public int level;

    public static implicit operator DataContainer(DisplayDataContainer display) {
        DataContainer data = new DataContainer();
        data.Ammo = new Dictionary<AmmoType, AmmoTracker>();
        foreach (AmmoTrackerSaveData s in display.ammo) {
            data.Ammo[s.type] = s.tracker;
        }

        data.EquippedGuns = display.equippedGuns;
        data.AllGuns = new Dictionary<GunShopItemData, bool>();
        foreach (AllGunsSaveData s in display.allGuns) {
            data.AllGuns[s.gun] = s.isOwned;
        }

        data.DroppableAmmo = display.droppableAmmo;
        
        data.UpgradableItemLevels = new Dictionary<string, int>();
        foreach (UpgradableItemLevelSaveData s in display.upgradableItemLevels) {
            data.UpgradableItemLevels.Add(s.name, s.level);
        }
        
        data.Currency = display.currency;
        data.Level = display.level;
        return data;
    }
}


public class DataContainer
{
    public Dictionary<AmmoType, AmmoTracker> Ammo;
    public GunData[] EquippedGuns;
    public Dictionary<GunShopItemData, bool> AllGuns;
    public List<AmmoDropData> DroppableAmmo;
    public Dictionary<string, int> UpgradableItemLevels;
    public int Currency;
    public int Level;
}

[Serializable]
public class AmmoTrackerSaveData
{
    public AmmoType type;
    public AmmoTracker tracker;
}

[Serializable]
public class AllGunsSaveData
{
    public GunShopItemData gun;
    public bool isOwned;
}

[Serializable]
public class UpgradableItemLevelSaveData
{
    public string name;
    public int level;
}