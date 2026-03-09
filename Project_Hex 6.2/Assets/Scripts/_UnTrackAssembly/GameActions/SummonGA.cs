using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonGA : GameAction
{
    public Card cardToInvoke;

    public SummonGA(Card card)
    {
        cardToInvoke = card;
    }
}