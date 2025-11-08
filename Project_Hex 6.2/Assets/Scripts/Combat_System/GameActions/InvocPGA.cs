using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvocPGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;
    public List<CardData> CardsToInvoc;

    public InvocPGA(int amount, int MultiplyAmount, DynamicAmount dynamicAmount, List<CardData> cardsToInvoc)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
        CardsToInvoc = cardsToInvoc;
    }
}
