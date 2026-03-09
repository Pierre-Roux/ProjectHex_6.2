using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPlayerGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public int HealAmount;
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;

    public HealPlayerGA(int healAmount, int MultiplyAmount, DynamicAmount dynamicAmount, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        HealAmount = healAmount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
