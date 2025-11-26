using System.Collections.Generic;
using UnityEngine;

public class PlayerAlterCardCostGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;
    public bool passive;
    public TargetModeInfo targetModeInfo;
    public List<Card> cardTargets { get; set; }

    public PlayerAlterCardCostGA(int amount, int MultiplyAmount, DynamicAmount dynamicAmount, bool Passive, List<Card> CardTargets = null, TargetModeInfo TargetModeInfo = null)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
        passive = Passive;
        targetModeInfo = TargetModeInfo;
        cardTargets = CardTargets;
    }    
}
