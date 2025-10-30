public class DrawCardsGA : GameAction
{
    public int Amount { get; set; }
    public DynamicAmount DynamicAmount;

    public DrawCardsGA(int amount, DynamicAmount dynamicAmount)
    {
        Amount = amount;
        DynamicAmount = dynamicAmount;
    }
}
