using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeLossGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public int Amount;
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;
    public PlayerLifeLossGA(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets, List<DynamicConditionInfo> dynamicConditionInfos = null)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
        DynamicAmountInfo = dynamicAmountInfo;
    }
}
