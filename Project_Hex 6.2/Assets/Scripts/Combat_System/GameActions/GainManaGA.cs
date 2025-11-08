using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainManaGA : GameAction
{
    public int GainAmount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;
    public GainManaGA(int amount, int MultiplyAmount, DynamicAmount dynamicAmount)
    {
        GainAmount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
    }
}
