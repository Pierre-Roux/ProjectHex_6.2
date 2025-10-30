using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlterStaminaGA : GameAction
{
    public int Amount;
    public DynamicAmount DynamicAmount;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public TargetMode targetMode;
    public PermaTypes permaTypes;
    public EnemyAlterStaminaGA(int amount, DynamicAmount dynamicAmount, PermaTypes PermaTypes, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets, TargetMode TargetMode = TargetMode.Self)
    {
        Amount = amount;
        DynamicAmount = dynamicAmount;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
        permaTypes = PermaTypes;
        targetMode = TargetMode;
    }
}
