using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class ManaGainEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public int GainAmount;
    [SerializeField] public DynamicAmount DynamicAmount;

    public override GameAction GetGameAction()
    {
        if (!BypassEntryCondition)
        {
            if (DynamicCondition != DynamicCondition.NULL)
            {
                if (Actionner == null)
                {
                    if (!ConditionSystem.Instance.TestCondition(DynamicCondition, TestDynamicAmount, TestValue, CardActionner, null, null))
                    {
                        return null;
                    }
                }
                else
                {
                    if (!ConditionSystem.Instance.TestCondition(DynamicCondition, TestDynamicAmount, TestValue, CardActionner, Actionner.GetComponent<PermanentView>(), Actionner.GetComponent<EnemySlotView>()))
                    {
                        return null;
                    }
                }
            }
        }
        GainManaGA gainManaGA = new(GainAmount,DynamicAmount);
        if (AudioManager.Instance.IsValid(SFX)){ gainManaGA.SFX = SFX; }
        return gainManaGA;
    }
    public ManaGainEffect(){}

    public ManaGainEffect(int Amount, int multiHit, int testValue,DynamicCondition dynamicCondition, DynamicAmount testDynamicAmount,PermaTypes testType, ActionnerType ActionnerType, Events Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount, EventReference sfx)
    {
        GainAmount = Amount;
        MultiHit = multiHit;
        Events = Event;
        TestValue = testValue;
        TestDynamicAmount = testDynamicAmount;
        DynamicCondition = dynamicCondition;
        TestType = testType;
        CancelOnDeath = cancelOnDeath;
        actionnerType = ActionnerType;
        Actionner = actionner;
        CardActionner = cardActionner;
        Intent_Title = intent_Title;
        number = Number;
        Duration = duration;
        DurationType = durationType;
        TriggerOnDurationEnd = triggerOnDurationEnd;
        LinkedEffect = linkedEffect;
        TargetForLinked_Player = targetForLinked_Player;
        TargetForLinked_Enemy = targetForLinked_Enemy;
        DynamicAmount = dynamicAmount;
        SFX = sfx;
    }

    public override Effect Clone()
    {
        var clonedPlayerTargets = TargetForLinked_Player != null 
            ? new List<PermanentView>(TargetForLinked_Player) 
            : null;

        var clonedEnemyTargets = TargetForLinked_Enemy != null 
            ? new List<EnemySlotView>(TargetForLinked_Enemy) 
            : null;

        Effect clonedLinked = LinkedEffect != null ? LinkedEffect.Clone() : null;

        return new ManaGainEffect(
            GainAmount,
            MultiHit,
            TestValue,
            DynamicCondition,
            TestDynamicAmount,
            TestType,
            actionnerType,
            Events,
            CancelOnDeath,
            Actionner,
            CardActionner,
            Intent_Title,
            number,
            Duration,
            DurationType,
            TriggerOnDurationEnd,
            clonedLinked,
            clonedPlayerTargets,
            clonedEnemyTargets,
            DynamicAmount,
            SFX
        );
    }
}
