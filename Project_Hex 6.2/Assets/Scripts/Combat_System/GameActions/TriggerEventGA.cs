using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventGA : GameAction
{
    public Events gameEvent;
    public Card Card;
    public PermanentView permanentView;
    public EnemySlotView enemySlotView;

    public CounterType counterTypeConcerned;

    public TriggerEventGA(Events events, Card card = null, PermanentView PermanentView = null, EnemySlotView EnemySlotView = null,CounterType CounterTypeConcerned = CounterType.NULL)
    {
        gameEvent = events;
        Card = card;
        permanentView = PermanentView;
        enemySlotView = EnemySlotView;
        counterTypeConcerned = CounterTypeConcerned;
    }
}
