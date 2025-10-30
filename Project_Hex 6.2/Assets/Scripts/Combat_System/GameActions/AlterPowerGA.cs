using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlterPowerGA : GameAction
{
    public int Amount { get; set; }
    public DynamicAmount DynamicAmount;
    public bool passive;
    public PermaTypes permaTypes;
    public TargetMode targetMode;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }

    public AlterPowerGA(int amount, DynamicAmount dynamicAmount, bool Passive, PermaTypes PermaTypes, List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null, TargetMode TargetMode = TargetMode.Self)
    {
        Amount = amount;
        DynamicAmount = dynamicAmount;
        passive = Passive;
        permaTypes = PermaTypes;
        playerTargets = targets_Player;
        enemyTargets = targets_Enemy;
        targetMode = TargetMode;
    }
}
