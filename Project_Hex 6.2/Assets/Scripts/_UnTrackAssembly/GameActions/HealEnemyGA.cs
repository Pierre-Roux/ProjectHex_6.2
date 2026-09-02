using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEnemyGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public int HealAmount;
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;

    public HealEnemyGA(int healAmount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        HealAmount = healAmount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
