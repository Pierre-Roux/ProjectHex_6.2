using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvocEGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;
    public List<EnemyPermanentData> EnemyToInvoc;

    public InvocEGA(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo, List<EnemyPermanentData> enemyToInvoc)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
        EnemyToInvoc = enemyToInvoc;
    }
}
