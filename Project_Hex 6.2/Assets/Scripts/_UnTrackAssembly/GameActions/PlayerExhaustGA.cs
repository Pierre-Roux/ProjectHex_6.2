using System.Collections.Generic;
public class PlayerExhaustGA : GameAction
{
    public TargetModeInfo targetModeInfo;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public List<Card> cardTargets { get; set; }

    public PlayerExhaustGA(List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null, List<Card> CardTargets = null, TargetModeInfo TargetModeInfo = null)
    {
        targetModeInfo = TargetModeInfo;
        playerTargets = targets_Player;
        enemyTargets = targets_Enemy;
        cardTargets = CardTargets;
    }
}
