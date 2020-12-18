using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class ProgressManager : SerializedMonoBehaviour
{
    [SerializeField]
    private ISaveLoad[] saveableItems;
    [SerializeField]
    private DataContainer defaultSave;
    public DataContainer Data { get; private set; }

    public void Save() {
        for (int i = 0; i < saveableItems.Length; i++)
            saveableItems[i].Save();

        byte[] data = SerializationUtility.SerializeValue(Data, DataFormat.JSON);
        File.WriteAllBytes(Application.persistentDataPath + "/savegame.json", data);
    }

    public void Load() {
        if (File.Exists(Application.persistentDataPath + "/savegame.json")) {
            byte[] data = File.ReadAllBytes(Application.persistentDataPath + "/savegame.json");
            Data = SerializationUtility.DeserializeValue<DataContainer>(data, DataFormat.JSON);
        }
        else
            Data = defaultSave;

        for (int i = 0; i < saveableItems.Length; i++)
            saveableItems[i].Load();
    }
}

public class DataContainer
{
    public Dictionary<AmmoType, int> Ammo;
    public int currency;
}