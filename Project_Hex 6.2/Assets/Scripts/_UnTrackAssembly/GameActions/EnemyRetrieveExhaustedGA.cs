using System.Collections.Generic;
public class EnemyRetrieveExhaustedGA : GameAction
{
    public List<Card> cardTargets { get; set; }

    public EnemyRetrieveExhaustedGA(List<Card> targets_Cards = null)
    {
        cardTargets = targets_Cards;
    }     
}
