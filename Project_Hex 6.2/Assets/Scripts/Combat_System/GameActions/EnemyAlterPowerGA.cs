using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlterPowerGA : GameAction
{
    public int Amount;
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public TargetModeInfo targetModeInfo;
    public bool passive;
    public EnemyAlterPowerGA(int amount, int MultiplyAmount, DynamicAmount dynamicAmount, bool Passive, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets, TargetModeInfo TargetModeInfo = null)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
        passive = Passive;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
        targetModeInfo = TargetModeInfo;
    }
}
