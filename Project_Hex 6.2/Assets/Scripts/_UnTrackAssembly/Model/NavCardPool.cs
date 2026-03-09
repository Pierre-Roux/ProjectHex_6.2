using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/NavCardPool")]
[System.Serializable]
public class NavCardPool : ScriptableObject
{
    public string PoolName;
    public List<NavCardData> NavCardDataList;
    public NavCardPool(string poolName, List<NavCardData> navCardsList)
    {
        PoolName = poolName;
        NavCardDataList = navCardsList;
    }
}
