public enum DynamicCondition
{
    NULL,

    // DynamicAmount
    DynamicAmountSupOrEqualsToValue,
    DynamicAmountInfOrEqualsToValue,
    DynamicAmountSupToValue,
    DynamicAmountInfToValue,
    _DynamicAmountCondition,

    // SelfPermanent
    ifPermanentIsTypeOfTestType,
    ifPermanentIsNotTypeOfTestType,

    // General
    ifYouControlTypeOfTestType,
    NoCardsInHands,
    CardsInHands,
    _GeneralCondition2,
    _GeneralCondition3,
    _GeneralCondition4,
    _GeneralCondition5,

    // Event
    ifEventPermanentIsTypeOfTestType,
    ifEventPermanentIsNotTypeOfTestType,
    ifEventPermanentIsPlayer,
    ifEventPermanentIsEnemy,
    _EventCondition,
    _EventCondition2,
    _EventCondition3,
    _EventCondition4,

    // GlobalCounter
    ifGlobalCounterOfTypeSupToValue,
    ifGlobalCounterOfTypeInfToValue,
    ifInternCounterOfTypeSupToValue,
    ifInternCounterOfTypeInfToValue,

    // Target
    ifTargetIsTypeOfTestType,
    ifTargetIsNotTypeOfTestType,
    ifTargetParamIsSupToValue,
    ifTargetParamIsInfToValue,
    ifTargetParamIsEqualToValue,
    ifTargetIsNotSelf,
    ifTargetIsSelf,
}

[System.Serializable]
public class DynamicConditionInfo
{
    public DynamicCondition DynamicCondition;
    public CounterTypeInfo CounterTypeInfo;
    public KeyWordType TestType;
    public int TestValue;
    public DynamicAmountInfo TestDynamicAmountInfo;
    public BasicParam basicParam;
    public bool currentParam;

    public DynamicConditionInfo(){}

    public DynamicConditionInfo(int testValue, DynamicCondition dynamicCondition, DynamicAmountInfo testDynamicAmount, KeyWordType testType, CounterTypeInfo counterTypeInfo, BasicParam BasicParam, bool CurrentParam)
    {
        TestValue = testValue;
        DynamicCondition = dynamicCondition;
        TestDynamicAmountInfo = testDynamicAmount;
        TestType = testType;
        CounterTypeInfo = counterTypeInfo;
        basicParam = BasicParam;
        currentParam = CurrentParam;
    }
}