using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLifeLossGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public int Amount;
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;
    public EnemyLifeLossGA(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
        DynamicAmountInfo = dynamicAmountInfo;
    }
}
