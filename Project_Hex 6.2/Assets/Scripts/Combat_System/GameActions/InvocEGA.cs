using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvocEGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;
    public List<EnemyPermanentData> EnemyToInvoc;

    public InvocEGA(int amount, int MultiplyAmount, DynamicAmount dynamicAmount, List<EnemyPermanentData> enemyToInvoc)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
        EnemyToInvoc = enemyToInvoc;
    }
}
