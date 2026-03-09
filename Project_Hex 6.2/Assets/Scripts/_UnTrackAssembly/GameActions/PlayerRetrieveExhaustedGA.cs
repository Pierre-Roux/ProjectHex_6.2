using System.Collections.Generic;

public class PlayerRetrieveExhaustedGA : GameAction
{
    public List<Card> cardTargets { get; set; }

    public PlayerRetrieveExhaustedGA(List<Card> targets_Cards = null)
    {
        cardTargets = targets_Cards;
    }     
}
