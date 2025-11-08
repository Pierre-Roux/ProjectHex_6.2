using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAlterStaminaGA : GameAction
{
    public int Amount;
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public PermaTypes permaTypes;
    public PlayerAlterStaminaGA(int amount, int MultiplyAmount, DynamicAmount dynamicAmount, PermaTypes PermaTypes, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
        permaTypes = PermaTypes;
    }
}
