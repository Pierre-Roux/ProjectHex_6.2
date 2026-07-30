using System;
using System.Collections.Generic;

public class AttackPlayerGA : GameAction
{
    public bool powerBased { get; set; }
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public int Damage;
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;

    public AttackPlayerGA(bool PowerBased, int damage, int MultiplyAmount, DynamicAmount dynamicAmount, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        powerBased = PowerBased;
        Damage = damage;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
