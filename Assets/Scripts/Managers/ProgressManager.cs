using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class ProgressManager : SerializedMonoBehaviour
{
    [SerializeField]
    private ScriptableObjectReferenceCache soReferenceCache;
    [SerializeField]
    private ISaveLoad[] saveableItems;
    [SerializeField]
    private DataContainer defaultSave;
    [ShowInInspector, ReadOnly]
    public DataContainer Data { get; private set; }

    private void Awake() {
        Load();
    }

    private void Start() {
        if(ReferenceManager.Inst.EnemySpawner != null)
            ReferenceManager.Inst.EnemySpawner.OnLevelEnd += Save;
    }

    private void Save() {
        for (int i = 0; i < saveableItems.Length; i++)
            saveableItems[i].Save();

        SerializationContext context = new SerializationContext
                                       {
                                           StringReferenceResolver = soReferenceCache
                                       };
        
        byte[] data = SerializationUtility.SerializeValue(Data, DataFormat.JSON, context);
        File.WriteAllBytes(Application.persistentDataPath + "/savegame.json", data);
    }

    private void Load() {
        if (File.Exists(Application.persistentDataPath + "/savegame.json")) {
            byte[] data = File.ReadAllBytes(Application.persistentDataPath + "/savegame.json");
            DeserializationContext context = new DeserializationContext
                                             {
                                                 StringReferenceResolver = soReferenceCache
                                             };
            Data = SerializationUtility.DeserializeValue<DataContainer>(data, DataFormat.JSON, context);
        }
        else
            Data = defaultSave;

        for (int i = 0; i < saveableItems.Length; i++)
            saveableItems[i].Load();
    }

    [Button]
    private void DeleteSave() {
        if(File.Exists(Application.persistentDataPath + "/savegame.json"))
            File.Delete(Application.persistentDataPath + "/savegame.json");
    }
}

public class DataContainer
{
    public Dictionary<AmmoType, AmmoTracker> Ammo;
    public GunData[] EquippedGuns;
    public Dictionary<GunShopItemData, bool> AllGuns;
    public List<AmmoDropData> DroppableAmmo;
    public int Currency;
    public int Level;
}