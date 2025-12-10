using System.Collections.Generic;

public class PlayerDisableGA : GameAction
{
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }

    public PlayerDisableGA(List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null)
    {
        playerTargets = targets_Player;
        enemyTargets = targets_Enemy;
    }  
}
