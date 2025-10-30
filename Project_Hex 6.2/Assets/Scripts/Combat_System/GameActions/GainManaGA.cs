using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainManaGA : GameAction
{
    public int GainAmount { get; set; }
    public DynamicAmount DynamicAmount;
    public GainManaGA(int amount, DynamicAmount dynamicAmount)
    {
        GainAmount = amount;
        DynamicAmount = dynamicAmount;
    }
}
