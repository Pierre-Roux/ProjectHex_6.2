using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }

    public SacGA(List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null)
    {
        playerTargets = targets_Player;
        enemyTargets = targets_Enemy;
    }
}
