using System.Collections.Generic;

public class DrawCardsGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;
    public bool countAsDraw_INGAME;

    public DrawCardsGA(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo, bool CountAsDraw_INGAME)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
        countAsDraw_INGAME = CountAsDraw_INGAME;
    }
}
