using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPermanentGA : GameAction
{
    public PermanentView PermanentView;
    public EnemySlotView enemySlotView;

    public DestroyPermanentGA(PermanentView permanent, EnemySlotView enemy)
    {
        PermanentView = permanent;
        enemySlotView = enemy;
    }
}
