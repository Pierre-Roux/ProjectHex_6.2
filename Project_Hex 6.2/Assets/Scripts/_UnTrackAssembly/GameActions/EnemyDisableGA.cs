using System.Collections.Generic;

public class EnemyDisableGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }

    public EnemyDisableGA(List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null)
    {
        playerTargets = targets_Player;
        enemyTargets = targets_Enemy;
    }  
}
