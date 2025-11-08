public enum Events
{
    Instant,

    // General Event
    EnemyTurn,
    EndEnemyTurn,
    StartTurn,
    EndTurn,
    OnPlayCard,
    OnPlaySpell,
    OnPlayPerma,
    OnInvoc,
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
    WhenPermaDie,
    WhenPermaSac,
    WhenPermaExaust,
    WhenPermaBecomeType,
    WhenPermaETB,
    WhenPermaLossDurability,
    WhenPermaDamaged,

    WhenPCoreDamaged,
    WhenECoreDamaged,

    //Card Event (ON DrawThis, onDiscardThis ...)
    NULL,
}