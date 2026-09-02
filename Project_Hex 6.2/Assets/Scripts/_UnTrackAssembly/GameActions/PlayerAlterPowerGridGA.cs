using UnityEngine;

public class PlayerAlterPowerGridGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;

    public PlayerAlterPowerGridGA(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
    }
}
