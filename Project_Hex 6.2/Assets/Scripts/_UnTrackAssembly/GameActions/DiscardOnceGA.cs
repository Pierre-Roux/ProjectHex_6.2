using System.Collections.Generic;
using UnityEngine;

public class DiscardOnceGA : GameAction
{
    public List<CardView> RestCardView { get; set; }
    public bool CountAsDiscard { get; set; }

    public DiscardOnceGA(List<CardView> CardViewList, bool countAsDiscard)
    {
        RestCardView = CardViewList;
        CountAsDiscard = countAsDiscard;
    }    
}
