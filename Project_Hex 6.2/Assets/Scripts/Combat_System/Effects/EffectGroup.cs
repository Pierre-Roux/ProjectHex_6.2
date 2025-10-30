using System.Collections;
using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;
using FMODUnity;
using System;

public class EffectGroup : Effect
{
    [Header("Effect Group")]
    [field: SerializeReference, SR] public List<Effect> EffectGroups;

    public EffectGroup() { }

    public EffectGroup(List<Effect> effectGroup, int testValue,DynamicCondition dynamicCondition, DynamicAmount testDynamicAmount,PermaTypes testType, ActionnerType ActionnerType, Events Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, EventReference sfx)
    {
        EffectGroups = effectGroup;
        TestValue = testValue;
        TestDynamicAmount = testDynamicAmount;
        DynamicCondition = dynamicCondition;
        TestType = testType;
        actionnerType = ActionnerType;
        CardActionner = cardActionner;
        Events = Event;
        CancelOnDeath = cancelOnDeath;
        Actionner = actionner;
        Intent_Title = intent_Title;
        number = Number;
        Duration = duration;
        DurationType = durationType;
        TriggerOnDurationEnd = triggerOnDurationEnd;
        LinkedEffect = linkedEffect;
        TargetForLinked_Player = targetForLinked_Player;
        TargetForLinked_Enemy = targetForLinked_Enemy;
        SFX = sfx;
    }

    public override GameAction GetGameAction()
    {
        return null;
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

        List<Effect> ClonedEffectGroup = new List<Effect>();
        foreach (Effect item in EffectGroups)
        {
            ClonedEffectGroup.Add(item.Clone());
        }

        return new EffectGroup(
            ClonedEffectGroup,
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
            SFX
        );
    }
}
