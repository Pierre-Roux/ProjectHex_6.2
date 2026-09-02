using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorEnemyGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public int ArmorAmount;
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;

    public ArmorEnemyGA(int armorAmount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        ArmorAmount = armorAmount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
