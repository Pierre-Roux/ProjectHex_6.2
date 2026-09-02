using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvocPGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;
    public List<CardData> CardsToInvoc;

    public InvocPGA(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo, List<CardData> cardsToInvoc)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
        CardsToInvoc = cardsToInvoc;
    }
}
