using UnityEngine;

public class PlayerAlterPowerGridGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;

    public PlayerAlterPowerGridGA(int amount, int MultiplyAmount, DynamicAmount dynamicAmount)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
    }
}
