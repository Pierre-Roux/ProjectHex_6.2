public enum CounterType
{
    NULL,

    SpellCast,
    PermanentCast,
    CardsDraw,
    CardsDiscard,
    CardsExhaust,
    DamageAmount,
    ManaSpent,

    _counter1,
    _counter2,
    _counter3,
    _counter4,
}

[System.Serializable]
public record CounterTypeInfo
{
    public CounterType CounterType;
    public KeyWordType TestType;
    public Enemy_Player_ENUM Owner;
    public bool SinceLoad;
    public bool Intern;

    public CounterTypeInfo(){}

    public CounterTypeInfo(bool sinceLoad, bool intern, Enemy_Player_ENUM owner, KeyWordType testType, CounterType counterType)
    {
        TestType = testType;
        CounterType = counterType;
        Owner = owner;
        SinceLoad = sinceLoad;
        Intern = intern;
    }
}
