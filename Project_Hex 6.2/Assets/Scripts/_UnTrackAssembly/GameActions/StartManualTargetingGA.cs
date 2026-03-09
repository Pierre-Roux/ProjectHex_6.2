using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManualTargetingGA : GameAction
{
    public GameAction ActionToRealiseAfterTargetting;
    public List<TargetLimitationInfo> TargetLimitations;
    public int TargetNumber;
    public bool TargetUpTo;
    public Effect EffectRef;
    public StartManualTargetingGA(GameAction actionToRealiseAfterTargetting, int targetNumber, bool targetUpTo, Effect effectRef = null, List<TargetLimitationInfo> targetLimitations = null)
    {
        ActionToRealiseAfterTargetting = actionToRealiseAfterTargetting;
        TargetNumber = targetNumber;
        TargetUpTo = targetUpTo;
        EffectRef = effectRef;
        TargetLimitations = targetLimitations;
    }
}
