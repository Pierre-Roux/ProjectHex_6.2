using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvocGA : GameAction
{
    public int Amount { get; set; }
    public DynamicAmount DynamicAmount;
    public List<CardData> CardsToInvoc;
    public List<EnemyPermanentData> EnemyToInvoc;

    public InvocGA(int amount, DynamicAmount dynamicAmount, List<CardData> cardsToInvoc = null, List<EnemyPermanentData> enemyToInvoc = null)
    {
        Amount = amount;
        DynamicAmount = dynamicAmount;
        CardsToInvoc = cardsToInvoc;
        EnemyToInvoc = enemyToInvoc;
    }
}
