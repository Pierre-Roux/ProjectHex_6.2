using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemyGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }

    public ShieldEnemyGA(List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
