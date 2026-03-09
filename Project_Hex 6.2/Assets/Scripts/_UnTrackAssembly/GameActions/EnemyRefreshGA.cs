using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRefreshGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public EnemyRefreshGA(List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets)
    {
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
    }
}
