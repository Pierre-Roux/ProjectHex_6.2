using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieEnemySlotGA : GameAction
{
    public EnemySlotView EnemySlotView { get; set; }

    public DieEnemySlotGA(EnemySlotView enemySlotView)
    {
        EnemySlotView = enemySlotView;
    }
}
