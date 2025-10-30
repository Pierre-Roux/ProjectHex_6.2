using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageGA : GameAction
{
    public int Amount { get; set; }
    public DynamicAmount DynamicAmount;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }

    public DealDamageGA(int amount, DynamicAmount dynamicAmount, List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null)
    {
        Amount = amount;
        DynamicAmount = dynamicAmount;
        playerTargets = targets_Player;
        enemyTargets = targets_Enemy;
    }
}
