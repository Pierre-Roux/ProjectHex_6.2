using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCardTargetingGA : GameAction
{
    public GameAction ActionToRealiseAfterTargetting;
    public int TargetNumber;
    public bool TargetUpTo;
    public List<TargetLimitationInfo> TargetLimitations;
    public Effect EffectRef;
    public StartCardTargetingGA(GameAction actionToRealiseAfterTargetting, int targetNumber, bool targetUpTo, Effect effectRef = null, List<TargetLimitationInfo> targetLimitations = null)
    {
        ActionToRealiseAfterTargetting = actionToRealiseAfterTargetting;
        TargetNumber = targetNumber;
        TargetLimitations = targetLimitations;
        EffectRef = effectRef;
        TargetUpTo = targetUpTo;
    }
}
