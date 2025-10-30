using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardAllCardsGA : GameAction
{
    public bool CountAsDiscard;
    public DiscardAllCardsGA(bool countAsDiscard)
    {
        CountAsDiscard = countAsDiscard;
    }
}
