using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;

[System.Serializable]
public abstract class IntentConstruct
{
    [SerializeField] public string number;
    [SerializeField] public List<EnemyPermanentData> EnemyData;

}
