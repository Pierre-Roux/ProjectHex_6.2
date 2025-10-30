using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlterStaminaGA : GameAction
{
    public int Amount { get; set; }
    public DynamicAmount DynamicAmount;
    public PermaTypes permaTypes;
    public TargetMode targetMode;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }

    public AlterStaminaGA(int amount, DynamicAmount dynamicAmount, PermaTypes PermaTypes, List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null, TargetMode TargetMode = TargetMode.Self)
    {
        Amount = amount;
        DynamicAmount = dynamicAmount;
        permaTypes = PermaTypes;
        playerTargets = targets_Player;
        enemyTargets = targets_Enemy;
        targetMode = TargetMode;
    }
}
