using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardCardGA : GameAction
{
    public List<CardView> CardViews { get; set; }

    public DiscardCardGA(List<CardView> cardViews)
    {
        CardViews = cardViews;
    }
}
