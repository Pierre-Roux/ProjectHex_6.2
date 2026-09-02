using System.Collections.Generic;

public class PlayerAlterStaminaGA : GameAction
{
    public int Amount;
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;
    public bool passive;
    public bool aditive;
    public TargetModeInfo targetModeInfo;
    public List<PermanentView> playerTargets { get; set; }
    public List<EnemySlotView> enemyTargets { get; set; }
    public List<Card> cardTargets { get; set; }
    public PlayerAlterStaminaGA(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo, bool Passive, bool Aditive, List<PermanentView> PlayerTargets, List<EnemySlotView> EnemyTargets, List<Card> CardTargets = null, TargetModeInfo TargetModeInfo = null)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
        passive = Passive;
        aditive = Aditive;
        targetModeInfo = TargetModeInfo;
        playerTargets = PlayerTargets;
        enemyTargets = EnemyTargets;
        cardTargets = CardTargets;
    }
}
