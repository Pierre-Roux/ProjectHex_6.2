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

    public EffectGroup(string effectID, bool hollowEffect, int activateNumber, int activateLeft, bool orChoice,List<Effect> effectGroup,List<DynamicConditionInfo> dynamicConditionInfos, ActionnerType ActionnerType, List<Events> Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, EventReference sfx,CounterType typeOfCounter, int counterValue, bool moduloValue)
    {
        EffectID = effectID;
        HollowEffect = hollowEffect;
        EffectGroups = effectGroup;
        ActivateNumber = activateNumber;
        ActivateLeft = activateLeft;
        ORChoice = orChoice;
        DynamicConditionInfos = dynamicConditionInfos;
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
        TypeOfCounter = typeOfCounter;
        CounterValue = counterValue;
        ModuloValue = moduloValue;
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
            EffectID,
            HollowEffect,
            ActivateNumber,
            ActivateLeft,
            ORChoice,
            ClonedEffectGroup,
            DynamicConditionInfos,
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
            SFX,
            TypeOfCounter,
            CounterValue,
            ModuloValue
        );
    }
}
