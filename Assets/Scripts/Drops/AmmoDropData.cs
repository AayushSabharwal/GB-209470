using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDropDaa", menuName = "Ammo Drop")]
public class AmmoDropData : DropData
{
    [EnumToggleButtons]
    public AmmoType type;
}
