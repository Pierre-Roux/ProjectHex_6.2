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

    ifGlobalCounterOfTypeSupToValue,
    ifGlobalCounterOfTypeInfToValue,
    ifInternCounterOfTypeSupToValue,
    ifInternCounterOfTypeInfToValue,

    NoCardsInHands,
}

[System.Serializable]
public class DynamicConditionInfo
{
    public DynamicCondition DynamicCondition;
    public CounterType CounterType;
    public PermaTypes TestType;
    public int TestValue;
    public DynamicAmount TestDynamicAmount;

    public DynamicConditionInfo(){}

    public DynamicConditionInfo(int testValue, DynamicCondition dynamicCondition, DynamicAmount testDynamicAmount, PermaTypes testType, CounterType counterType)
    {
        TestValue = testValue;
        DynamicCondition = dynamicCondition;
        TestDynamicAmount = testDynamicAmount;
        TestType = testType;
        CounterType = counterType;
    }
}