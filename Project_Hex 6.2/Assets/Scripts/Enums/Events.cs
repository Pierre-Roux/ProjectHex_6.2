using System;
using System.Collections.Generic;

public enum Events
{
    Instant,

    // CardEvent
    OnSelfDiscard,
    OnSelfDraw,
    OnSelfExaust,

    //Permanent or EnemyPermanent Event
    OnSelect,
    OnSelfDeath,
    OnSelfSacrifice,
    OnSelfSelfSelect,
    OnSelfDamaged,
    OnSelfDestroy,
    OnSelfKill,
    _SelfEvent1,
    _SelfEvent2,
    _SelfEvent3,

    //Whenever Events
    WhenPlayType,
    WhenPermaDie,
    WhenPermaSac,
    WhenPermaExaust,
    WhenCardExitExhaust,
    WhenPermaETB,
    WhenDiscard,
    WhenDraw,
    WhenShuffle,

    _WhenEvent1,
    _WhenEvent2,
    _WhenEvent3,

    WhenPermaGainType,
    WhenPermaLoseType,
    WhenPermaGainParam,
    WhenPermaLoseParam,
    WhenPermaChangeParam,

    // Counter
    WhenGlobalCounter,
    WhenInternCounter,

    // General Event
    StartFight,
    EndFight,
    StartTurn,
    EndTurn,
    EnemyTurn,
    EmptyHanded,
    HandNoLongerEmpty,
    HandFull,
    TypeCountChanged,

    NULL,
}

[System.Serializable]
public class EventInfo
{
    public Events Events;
    public KeyWordType TestType;
    public Enemy_Player_ENUM Owner;
    public BasicParam BasicParam;

    public EventInfo(){}

    public EventInfo(Events events, Enemy_Player_ENUM owner = Enemy_Player_ENUM.NULL, KeyWordType testType = KeyWordType.NULL, BasicParam basicParam = BasicParam.NULL)
    {
        Events = events;
        TestType = testType;
        Owner = owner;
        BasicParam = basicParam;
    }

    public static implicit operator List<object>(EventInfo v)
    {
        throw new NotImplementedException();
    }
}