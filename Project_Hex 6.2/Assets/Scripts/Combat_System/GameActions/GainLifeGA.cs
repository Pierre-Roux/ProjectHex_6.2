using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainLifeGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;
    public bool passive;
    public TargetModeInfo targetModeInfo;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }

    public GainLifeGA(int amount, int MultiplyAmount, DynamicAmount dynamicAmount, bool Passive, List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null, TargetModeInfo TargetModeInfo = null)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
        passive = Passive;
        playerTargets = targets_Player;
        enemyTargets = targets_Enemy;
        targetModeInfo = TargetModeInfo;
    }
}
