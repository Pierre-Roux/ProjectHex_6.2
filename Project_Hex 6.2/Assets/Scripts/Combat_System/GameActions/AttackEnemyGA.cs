using System;
using System.Collections.Generic;

public class AttackEnemyGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public int Damage;
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;

    public AttackEnemyGA(int damage, int MultiplyAmount, DynamicAmount dynamicAmount, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        Damage = damage;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
