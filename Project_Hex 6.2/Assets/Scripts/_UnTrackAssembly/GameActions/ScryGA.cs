using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScryGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;

    public ScryGA(int amount, int MultiplyAmount, DynamicAmount dynamicAmount)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
    }
}
