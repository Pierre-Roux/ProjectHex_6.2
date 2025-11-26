public enum Events
{
    Instant,

    // General Event
    EnemyTurn,
    EndEnemyTurn,
    StartTurn,
    EndTurn,
    OnDiscard,
    OnDraw,

    //Permanent or EnemyPermanent Event
    OnDeath,
    OnSacrifice,
    OnSelect,
    OnDamaged,
    OnDestroy,
    OnKill,

    //Whenever Events
    WhenPlayCard,
    WhenPlaySpell,
    WhenPlayPerma,
    WhenPermaDie,
    WhenPermaSac,
    WhenPermaExaust,
    WhenPermaBecomeType,
    WhenPermaETB,
    WhenPermaLossDurability,
    WhenPermaDamaged,

    WhenPCoreDamaged,
    WhenECoreDamaged,

    WhenDiscard,
    WhenDraw,

    WhenGlobalCounter,
    WhenInternCounter,

    // FlagEvents
    EmptyHanded,
    HandNoLongerEmpty,
    HandFull,
    HollowCountChanged,
    DecayCountChanged,
    InvocCountChanged,
    ArtilleryCountChanged,

    NULL,
}