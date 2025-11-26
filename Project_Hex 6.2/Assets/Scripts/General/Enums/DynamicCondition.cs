public enum DynamicCondition
{
    NULL,

    DynamicAmountSupOrEqualsToValue,
    DynamicAmountInfOrEqualsToValue,
    DynamicAmountSupToValue,
    DynamicAmountInfToValue,

    isHollow,
    isDecay,
    isInvoc,
    isArtillery,

    ifYouControlHollow,
    ifYouControlDecay,
    ifYouControlInvoc,
    ifYouControlArtillery,

    ifEventPermanentIsTypeOfTestType,
    ifEventPermanentIsPlayer,
    ifEventPermanentIsEnemy,
    ifEventPermanentIsVessel,

    ifEventCardTriggerIsVessel,
    ifEventCardTriggerIsSpell,

    NoCardsInHands,
}

[System.Serializable]
public class DynamicConditionInfo
{
    public DynamicCondition DynamicCondition;
    public int TestValue;
    public DynamicAmount TestDynamicAmount;
    public PermaTypes TestType;

    public DynamicConditionInfo(){}

    public DynamicConditionInfo(int testValue, DynamicCondition dynamicCondition, DynamicAmount testDynamicAmount, PermaTypes testType)
    {
        TestValue = testValue;
        DynamicCondition = dynamicCondition;
        TestDynamicAmount = testDynamicAmount;
        TestType = testType;
    }
}