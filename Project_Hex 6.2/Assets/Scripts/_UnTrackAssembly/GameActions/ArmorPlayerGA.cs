using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPlayerGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public int ArmorAmount;
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;

    public ArmorPlayerGA(int armorAmount, int MultiplyAmount, DynamicAmount dynamicAmount, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        ArmorAmount = armorAmount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
