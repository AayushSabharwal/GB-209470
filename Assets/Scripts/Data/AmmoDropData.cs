using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDropData", menuName = "Ammo Drop")]
public class AmmoDropData : DropData, IFloatingProbability
{
    [EnumToggleButtons]
    public AmmoType type;
    [SerializeField]
    private float probabilityWeight;
    public float FloatingProbability => probabilityWeight;
}
