using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseShieldGA : GameAction
{
    public PermanentView PermanentView;
    public EnemySlotView EnemySlotView;

    public LoseShieldGA(PermanentView permanentView = null,EnemySlotView enemySlotView = null)
    {
        PermanentView = permanentView;
        EnemySlotView = enemySlotView;
    }
}
