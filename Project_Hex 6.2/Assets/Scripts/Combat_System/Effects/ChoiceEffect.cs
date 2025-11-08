using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class ChoiceEffect : Effect
{
    [Header ("For player choice")]
    [SerializeField] bool PlayerChoice;
    [SerializeField] bool MayChoice;
    [field: SerializeReference, SR] public List<Effect> EffectsForPlayerChoice;

    [Header ("For non player choice")]
    [field: SerializeReference, SR] public Effect EffectOnTrue;
    [field: SerializeReference, SR] public Effect EffectOnFalse;

    public override GameAction GetGameAction()
    {
        if (PlayerChoice)
        {
            LetChoiceGA letChoiceGA = new(EffectsForPlayerChoice,false,MayChoice);
            return letChoiceGA; 
        }
        else
        {
            if (Actionner == null)
            {
                if (ConditionSystem.Instance.TestCondition(DynamicConditionInfos, CardActionner, null, null))
                {
                    return EffectOnTrue.GetGameAction();
                }
                else
                {
                    return EffectOnFalse.GetGameAction();
                }
            }
            else
            {
                if (ConditionSystem.Instance.TestCondition(DynamicConditionInfos, CardActionner, Actionner.GetComponent<PermanentView>(), Actionner.GetComponent<EnemySlotView>()))
                {
                    return EffectOnTrue.GetGameAction();
                }
                else
                {
                    return EffectOnFalse.GetGameAction();
                }
            }
        }        
    }

    public ChoiceEffect() { }

    public ChoiceEffect(List<DynamicConditionInfo> dynamicConditionInfos, ActionnerType ActionnerType, Events Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, EventReference sfx, Effect effectOnTrue, Effect effectOnFalse, bool playerChoice,bool mayChoice, List<Effect> effectsForPlayerChoice)
    {
        Events = Event;
        DynamicConditionInfos = dynamicConditionInfos;
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
        EffectOnTrue = effectOnTrue;
        EffectOnFalse = effectOnFalse;
        PlayerChoice = playerChoice;
        MayChoice = mayChoice;
        EffectsForPlayerChoice = effectsForPlayerChoice;
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

        List<Effect> ClonedChoiceEffects = new List<Effect>();
        foreach (Effect effect in EffectsForPlayerChoice)
        {
            ClonedChoiceEffects.Add(effect.Clone()); 
        }

        return new ChoiceEffect(
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
            EffectOnTrue,
            EffectOnFalse,
            PlayerChoice,
            MayChoice,
            ClonedChoiceEffects
        );
    }
}
