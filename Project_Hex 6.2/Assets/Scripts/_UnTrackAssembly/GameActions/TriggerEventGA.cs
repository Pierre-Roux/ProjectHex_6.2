using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventGA : GameAction
{
    public EventInfo EventInfo;
    public Card Card;
    public PermanentView PermanentView;
    public EnemySlotView EnemySlotView;
    public CounterTypeInfo CounterTypeInfo;

    public TriggerEventGA(EventInfo eventinfo,CounterTypeInfo counterTypeInfo, Card card = null, PermanentView permanentView = null, EnemySlotView enemySlotView = null)
    {
        EventInfo = eventinfo;
        Card = card;
        PermanentView = permanentView;
        EnemySlotView = enemySlotView;
        CounterTypeInfo = counterTypeInfo;
    }
}
