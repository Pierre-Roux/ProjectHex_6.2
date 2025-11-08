using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlterStaminaGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;
    public PermaTypes permaTypes;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }

    public AlterStaminaGA(int amount, int MultiplyAmount, DynamicAmount dynamicAmount, PermaTypes PermaTypes, List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
        permaTypes = PermaTypes;
        playerTargets = targets_Player;
        enemyTargets = targets_Enemy;
    }
}
