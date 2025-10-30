using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvocPGA : GameAction
{
    public int Amount { get; set; }
    public DynamicAmount DynamicAmount;
    public List<CardData> CardsToInvoc;

    public InvocPGA(int amount, DynamicAmount dynamicAmount, List<CardData> cardsToInvoc)
    {
        Amount = amount;
        DynamicAmount = dynamicAmount;
        CardsToInvoc = cardsToInvoc;
    }
}
