using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlterStaminaGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;
    public bool passive;
    public bool aditive;
    public TargetModeInfo targetModeInfo;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }

    public AlterStaminaGA(int amount, int MultiplyAmount, DynamicAmount dynamicAmount, bool Passive, bool Aditive, List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null, TargetModeInfo TargetModeInfo = null)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
        passive = Passive;
        aditive = Aditive;
        targetModeInfo = TargetModeInfo;
        playerTargets = targets_Player;
        enemyTargets = targets_Enemy;
    }
}
