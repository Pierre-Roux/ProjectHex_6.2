using System.Collections.Generic;
using UnityEngine;

public class PlayerAlterCardCostGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;
    public bool passive;
    public TargetModeInfo targetModeInfo;
    public List<Card> cardTargets { get; set; }

    public PlayerAlterCardCostGA(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo, bool Passive, List<Card> CardTargets = null, TargetModeInfo TargetModeInfo = null)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
        passive = Passive;
        targetModeInfo = TargetModeInfo;
        cardTargets = CardTargets;
    }    
}
