using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGainLifeGA : GameAction
{
    public int Amount;
    public DynamicAmount DynamicAmount;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public TargetMode targetMode;
    public bool passive;
    public PermaTypes permaTypes;
    public EnemyGainLifeGA(int amount, DynamicAmount dynamicAmount, bool Passive, PermaTypes PermaTypes, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets, TargetMode TargetMode = TargetMode.Self)
    {
        Amount = amount;
        DynamicAmount = dynamicAmount;
        passive = Passive;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
        permaTypes = PermaTypes;
        targetMode = TargetMode;
    }
}
