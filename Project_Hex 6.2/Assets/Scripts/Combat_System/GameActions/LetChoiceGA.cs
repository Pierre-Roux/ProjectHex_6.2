using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetChoiceGA : GameAction
{
    public List<Effect> ChoicesEffects;
    public Card CardVisual;
    public LetChoiceGA(List<Effect> choicesEffects, Card cardVisual,GameObject actionner = null)
    {
        ChoicesEffects = choicesEffects;
        CardVisual = cardVisual;
        Actionner = actionner;
    }
}
