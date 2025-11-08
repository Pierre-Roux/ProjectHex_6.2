using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetChoiceGA : GameAction
{
    public List<Effect> ChoicesEffects;
    public bool OnSelectMode;
    public bool MayChoice;
    public LetChoiceGA(List<Effect> choicesEffects, bool onSelectMode = false, bool mayChoice = false)
    {
        ChoicesEffects = choicesEffects;
        OnSelectMode = onSelectMode;
        MayChoice = mayChoice;
    }
}
