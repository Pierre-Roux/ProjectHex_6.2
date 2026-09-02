using System.Collections.Generic;
using UnityEngine;

public class AlterPowerGridGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;

    public AlterPowerGridGA(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
    }
}
