using System.Collections.Generic;
public class RetrieveExhaustedGA : GameAction
{
    public List<Card> cardTargets { get; set; }

    public RetrieveExhaustedGA(List<Card> targets_Cards = null)
    {
        cardTargets = targets_Cards;
    }    
}
