using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/CardPool")]
[System.Serializable]
public class CardPool : ScriptableObject
{
    public string PoolName;
    public List<CardData> CardDataList;

    public CardPool(string poolName, List<CardData> cardsList)
    {
        PoolName = poolName;
        CardDataList = cardsList;
    }
}
