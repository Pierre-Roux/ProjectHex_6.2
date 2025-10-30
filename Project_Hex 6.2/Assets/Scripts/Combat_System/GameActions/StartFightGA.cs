using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFightGA : GameAction
{
    public EnemyView enemyView;

    public StartFightGA(EnemyView EnemyView)
    {
        enemyView = EnemyView;
    }
}
