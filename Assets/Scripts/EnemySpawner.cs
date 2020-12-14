using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    
    [PropertySpace]
    
    [SerializeField, Probability("enemies")]
    private float[] probabilities;
    [SerializeField, InlineEditor]
    private EnemyData[] enemies;
    
    
}
