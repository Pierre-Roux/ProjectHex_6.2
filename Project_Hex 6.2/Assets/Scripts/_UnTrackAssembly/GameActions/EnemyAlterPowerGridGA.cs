using UnityEngine;

public class EnemyAlterPowerGridGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;

    public EnemyAlterPowerGridGA(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
    }
}
