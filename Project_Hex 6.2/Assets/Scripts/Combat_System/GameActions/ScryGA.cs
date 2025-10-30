using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScryGA : GameAction
{
    public int Amount { get; set; }
    public DynamicAmount DynamicAmount;

    public ScryGA(int amount, DynamicAmount dynamicAmount)
    {
        Amount = amount;
        DynamicAmount = dynamicAmount;
    }
}
