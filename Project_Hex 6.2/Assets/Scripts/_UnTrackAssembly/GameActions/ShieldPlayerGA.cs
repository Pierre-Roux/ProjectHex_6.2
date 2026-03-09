using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPlayerGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }

    public ShieldPlayerGA(List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
