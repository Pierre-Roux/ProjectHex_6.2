public class DrawCardsGA : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;
    public bool countAsDraw_INGAME;

    public DrawCardsGA(int amount, int MultiplyAmount, DynamicAmount dynamicAmount, bool CountAsDraw_INGAME)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
        countAsDraw_INGAME = CountAsDraw_INGAME;
    }
}
