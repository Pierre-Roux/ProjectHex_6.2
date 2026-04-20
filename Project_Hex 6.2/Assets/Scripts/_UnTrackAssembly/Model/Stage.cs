using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Stage")]
public class Stage : ScriptableObject
{
    [SerializeField] public string Name;
    [SerializeField] public int Tier;
    [SerializeField] public NavCardPool StageNavCardPool;
    [SerializeField] public List<GameObject> Enemies;
    [SerializeField] public List<GameObject> Elite_Enemies;
    [SerializeField] public List<GameObject> Boss_Enemies;
}
