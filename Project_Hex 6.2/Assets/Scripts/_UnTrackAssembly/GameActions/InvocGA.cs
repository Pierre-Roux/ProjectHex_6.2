using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvocGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;
    public List<CardData> CardsToInvoc;
    public List<EnemyPermanentData> EnemyToInvoc;

    public InvocGA(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo, List<CardData> cardsToInvoc = null, List<EnemyPermanentData> enemyToInvoc = null)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
        CardsToInvoc = cardsToInvoc;
        EnemyToInvoc = enemyToInvoc;
    }
}
