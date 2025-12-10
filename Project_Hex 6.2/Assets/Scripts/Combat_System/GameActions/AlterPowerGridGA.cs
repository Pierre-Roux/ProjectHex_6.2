using UnityEngine;

public class AlterPowerGridGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;

    public AlterPowerGridGA(int amount, int MultiplyAmount, DynamicAmount dynamicAmount)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
    }
}
