using System.Collections.Generic;

public class ArmorGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }

    public ArmorGA(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo, List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        playerTargets = targets_Player;
        enemyTargets = targets_Enemy;
        DynamicAmountInfo = dynamicAmountInfo;
    }
}
