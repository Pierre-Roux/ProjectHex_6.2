using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScryGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;

    public ScryGA(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
    }
}
