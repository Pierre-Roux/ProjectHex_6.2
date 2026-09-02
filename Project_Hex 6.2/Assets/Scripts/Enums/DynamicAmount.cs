public enum DynamicAmount
{
    NULL,

    CounterType,
    Count,

    TargetParam,
    SelfParam,
    PayXValue,

    // General
    ShieldedCount,
    CurrentMana,
    ManaSpent,

}

[System.Serializable]
public class DynamicAmountInfo
{
    public DynamicAmount DynamicAmount;
    public CounterTypeInfo CounterType;
    public KeyWordType TestType;
    public Enemy_Player_ENUM Enemy_Player;
    public BasicParam BasicParam;
    public CardLocation CardLocation;
    public bool CurrentParam;

    public DynamicAmountInfo(){}

    public DynamicAmountInfo(DynamicAmount dynamicAmount, Enemy_Player_ENUM enemy_Player, KeyWordType testType, CounterTypeInfo counterType, BasicParam basicParam ,bool currentParam, CardLocation cardLocation)
    {
        DynamicAmount = dynamicAmount;
        TestType = testType;
        CounterType = counterType;
        Enemy_Player = enemy_Player;
        BasicParam = basicParam;
        CurrentParam = currentParam;
        CardLocation = cardLocation;
    }
}
