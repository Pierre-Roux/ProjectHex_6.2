using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEnemyGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public int HealAmount;
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;

    public HealEnemyGA(int healAmount, int MultiplyAmount, DynamicAmount dynamicAmount, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        HealAmount = healAmount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
