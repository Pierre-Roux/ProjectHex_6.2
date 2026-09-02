using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainManaGA : GameAction
{
    public int GainAmount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;
    public GainManaGA(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo)
    {
        GainAmount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
    }
}
