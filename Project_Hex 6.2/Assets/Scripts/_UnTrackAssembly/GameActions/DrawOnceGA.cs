using System.Collections.Generic;
using UnityEngine;

public class DrawOnceGA : GameAction
{
    public int CardToDrawCount{ get; set; }
    public bool CountAsDiscard { get; set; }

    public DrawOnceGA(int cardToDrawCount, bool countAsDiscard)
    {
        CardToDrawCount = cardToDrawCount;
        CountAsDiscard = countAsDiscard;
    }    
}
