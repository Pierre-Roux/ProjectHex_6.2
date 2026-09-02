using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageGA : GameAction
{
    public bool powerBased { get; set; }
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }

    public DealDamageGA(bool PowerBased, int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo, List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null)
    {
        powerBased = PowerBased;
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
        playerTargets = targets_Player;
        enemyTargets = targets_Enemy;
    }
}
