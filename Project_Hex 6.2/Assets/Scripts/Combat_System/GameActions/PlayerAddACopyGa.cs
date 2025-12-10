using System.Collections.Generic;

public class PlayerAddACopyGa : GameAction
{
    public int Amount { get; set; }
    public int multiplyAmount { get; set; }
    public DynamicAmount DynamicAmount;
    public Enemy_Player_ENUM AffectedSide;
    public CopyTokenType TypeOfCopy;
    public List<DynamicConditionInfo> ConditionToCopy;

    public PlayerAddACopyGa(int amount, int MultiplyAmount, DynamicAmount dynamicAmount, Enemy_Player_ENUM affectedSide, CopyTokenType typeOfCopy, List<DynamicConditionInfo> conditionToCopy)
    {
        Amount = amount;
        multiplyAmount = MultiplyAmount;
        DynamicAmount = dynamicAmount;
        AffectedSide = affectedSide;
        TypeOfCopy = typeOfCopy;
        ConditionToCopy = conditionToCopy;
    } 
}
