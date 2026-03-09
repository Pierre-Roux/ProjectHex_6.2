using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageGA : GameAction
{
    public int Amount { get; set; }
    public int BonusAmount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }

    public DealDamageGA(int amount, int bonusAmount, int MultiplyAmount, DynamicAmount dynamicAmount, List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null)
    {
        Amount = amount;
        BonusAmount = bonusAmount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
        playerTargets = targets_Player;
        enemyTargets = targets_Enemy;
    }
}
