using System.Collections.Generic;

public class AlterStaminaGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;
    public bool passive;
    public bool aditive;
    public TargetModeInfo targetModeInfo;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public List<Card> cardTargets { get; set; }

    public AlterStaminaGA(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo, bool Passive, bool Aditive, List<PermanentView> targets_Player = null, List<EnemySlotView> targets_Enemy = null, List<Card> CardTargets = null, TargetModeInfo TargetModeInfo = null)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
        passive = Passive;
        aditive = Aditive;
        targetModeInfo = TargetModeInfo;
        playerTargets = targets_Player;
        enemyTargets = targets_Enemy;
        cardTargets = CardTargets;
    }
}
