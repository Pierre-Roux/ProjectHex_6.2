using System;
using System.Collections.Generic;

public class AttackPlayerGA : GameAction
{
    public bool powerBased { get; set; }
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public int Damage;
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;

    public AttackPlayerGA(bool PowerBased, int damage, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        powerBased = PowerBased;
        Damage = damage;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
