using System.Collections.Generic;

public class EnemyAddACopyGa : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmountInfo DynamicAmountInfo;
    public Enemy_Player_ENUM AffectedSide;
    public CopyTokenType TypeOfCopy;

    public EnemyAddACopyGa(int amount, int MultiplyAmount, DynamicAmountInfo dynamicAmountInfo, Enemy_Player_ENUM affectedSide, CopyTokenType typeOfCopy)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmountInfo = dynamicAmountInfo;
        AffectedSide = affectedSide;
        TypeOfCopy = typeOfCopy;
    } 
}
