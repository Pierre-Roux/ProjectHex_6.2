using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainLifeGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;
    public bool passive;
    public bool aditive;
    public TargetModeInfo targetModeInfo;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }

    public GainLifeGA(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo, bool Passive, bool Aditive, List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null, TargetModeInfo TargetModeInfo = null)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
        passive = Passive;
        aditive = Aditive;
        playerTargets = targets_Player;
        enemyTargets = targets_Enemy;
        targetModeInfo = TargetModeInfo;
    }
}
