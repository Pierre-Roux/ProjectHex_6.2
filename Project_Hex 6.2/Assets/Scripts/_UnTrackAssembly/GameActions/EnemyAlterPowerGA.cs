using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlterPowerGA : GameAction
{
    public int Amount;
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public List<Card> cardTargets { get; set; }
    public TargetModeInfo targetModeInfo;
    public bool passive;
    public bool aditive;
    public EnemyAlterPowerGA(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo, bool Passive, bool Aditive, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets, List<Card> CardTargets, TargetModeInfo TargetModeInfo = null)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
        passive = Passive;
        aditive = Aditive;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
        cardTargets = CardTargets;
        targetModeInfo = TargetModeInfo;
    }
}
