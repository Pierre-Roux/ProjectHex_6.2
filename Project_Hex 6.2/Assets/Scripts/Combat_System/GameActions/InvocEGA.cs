using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvocEGA : GameAction
{
    public int Amount { get; set; }
    public DynamicAmount DynamicAmount;
    public List<EnemyPermanentData> EnemyToInvoc;

    public InvocEGA(int amount, DynamicAmount dynamicAmount, List<EnemyPermanentData> enemyToInvoc)
    {
        Amount = amount;
        DynamicAmount = dynamicAmount;
        EnemyToInvoc = enemyToInvoc;
    }
}
