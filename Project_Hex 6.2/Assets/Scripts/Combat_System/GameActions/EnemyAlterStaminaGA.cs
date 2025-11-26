using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlterStaminaGA : GameAction
{
    public int Amount;
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;
    public bool passive;
    public bool aditive;
    public TargetModeInfo targetModeInfo;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public EnemyAlterStaminaGA(int amount, int MultiplyAmount, DynamicAmount dynamicAmount, bool Passive, bool Aditive, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets, TargetModeInfo TargetModeInfo = null)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
        passive = Passive;
        aditive = Aditive;
        targetModeInfo = TargetModeInfo;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
