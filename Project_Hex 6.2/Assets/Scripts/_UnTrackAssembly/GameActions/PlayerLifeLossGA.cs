using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeLossGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public int Amount;
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;
    public PlayerLifeLossGA(int amount, int MultiplyAmount, DynamicAmount dynamicAmount, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
        DynamicAmount = dynamicAmount;
    }
}
