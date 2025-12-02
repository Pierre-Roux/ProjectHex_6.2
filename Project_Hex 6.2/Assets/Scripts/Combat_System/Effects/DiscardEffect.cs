using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class DiscardEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public int DiscardAmount;
    [SerializeField] public int multiplyAmount = 1;
    [SerializeField] public DynamicAmount DynamicAmount;
    [SerializeField] public bool DiscardAll;
    [SerializeField] private bool TargetUpTo = true;
    public override bool EffectTargetUpTo => TargetUpTo;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;
    

    private bool ConditionTested = false;

    public DiscardEffect(){}

    public DiscardEffect(string effectID, bool activateToolTip, int priority, int Amount, int MultiplyAmount, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, bool orChoice,List<DynamicConditionInfo> dynamicConditionInfos, List<TargetLimitationInfo> TargetLimitations, bool targetUpTo, ActionnerType ActionnerType, List<Events> Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount, bool discardAll, EventReference sfx, bool conditionTested,CounterType typeOfCounter, int counterValue, bool moduloValue)
    {
        Priority = priority;
        ActivateToolTip = activateToolTip;
        EffectID = effectID;
        DiscardAmount = Amount;
        multiplyAmount = MultiplyAmount;
        PayXEffect = payXEffect;
        PayXValue = payXValue;
        MultiHit = multiHit;
        ActivateNumber = activateNumber;
        ActivateLeft = activateLeft;
        ORChoice = orChoice;
        Events = Event;
        DynamicConditionInfos = dynamicConditionInfos;
        CancelOnDeath = cancelOnDeath;
        actionnerType = ActionnerType;
        Actionner = actionner;
        CardActionner = cardActionner;
        Intent_Title = intent_Title;
        targetLimitations = TargetLimitations;
        TargetUpTo = targetUpTo;
        number = Number;
        Duration = duration;
        DurationType = durationType;
        TriggerOnDurationEnd = triggerOnDurationEnd;
        LinkedEffect = linkedEffect;
        TargetForLinked_Player = targetForLinked_Player;
        TargetForLinked_Enemy = targetForLinked_Enemy;
        DynamicAmount = dynamicAmount;
        DiscardAll = discardAll;
        SFX = sfx;
        ConditionTested = conditionTested;
        TypeOfCounter = typeOfCounter;
        CounterValue = counterValue;
        ModuloValue = moduloValue;
    }

    public override GameAction GetGameAction()
    {
        if (!BypassEntryCondition)
        {
            if (DynamicConditionInfos.Count != 0)
            {
                if (Actionner == null)
                {
                    if (!ConditionSystem.Instance.TestCondition(DynamicConditionInfos, CardActionner, null, null))
                    {
                        return null;
                    }
                }
                else
                {
                    if (!ConditionSystem.Instance.TestCondition(DynamicConditionInfos, CardActionner, Actionner.GetComponent<PermanentView>(), Actionner.GetComponent<EnemySlotView>()))
                    {
                        return null;
                    }
                }
            }
        }
        
        if (DiscardAll)
        {
            DiscardAllCardsGA discardAllCardsGA = new(true);
            discardAllCardsGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_DiscardSound : SFX;
            return discardAllCardsGA;
        }
        else
        {
            if (DynamicAmount != DynamicAmount.NULL)
            {
                if (Actionner == null)
                {
                    if (CardActionner != null)
                    {
                        DiscardAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount, null, null, CardActionner);
                    }
                    else
                    {
                        DiscardAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount, null, null);
                    }
                }
                else if (Actionner.GetComponent<PermanentView>() != null)
                {
                    DiscardAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount, Actionner.GetComponent<PermanentView>(), null);
                }
                else
                {
                    DiscardAmount = TargetSystem.Instance.GetDynamicAmount(DynamicAmount, null, Actionner.GetComponent<EnemySlotView>());
                }
            }

            if (PayXValue != 0)
            {
                DynamicAmount = DynamicAmount.NULL;
                DiscardAmount = PayXValue;
            }

            if (!ConditionTested)
            {
                DiscardEffect DiscardallEffect = (DiscardEffect)Clone();
                DiscardallEffect.DiscardAll = true;

                DiscardEffect DiscardManuEffect = (DiscardEffect)Clone();
                DiscardManuEffect.ConditionTested = true;

                DynamicConditionInfo Condition = new(DiscardAmount * multiplyAmount, DynamicCondition.DynamicAmountInfOrEqualsToValue, DynamicAmount.CardsInHand_Count, PermaTypes.NULL, CounterType.NULL);
                List<DynamicConditionInfo> Conditions = new List<DynamicConditionInfo> {Condition};

                if (ConditionSystem.Instance.TestCondition(Conditions, null, null, null))
                {
                    return DiscardallEffect.GetGameAction();
                }
                else
                {
                    return DiscardManuEffect.GetGameAction();
                }
            }
            else
            {
                DiscardAmount = DiscardAmount * multiplyAmount;

                DiscardCardGA discardCardGA = new(new List<CardView>());
                discardCardGA.SourceEffect = this;
                discardCardGA.ActivateToolTip = false;
                discardCardGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_DiscardSound : SFX;
                StartCardTargetingGA startCardTargetingGA = new(discardCardGA, DiscardAmount, TargetUpTo, this, targetLimitations);
                startCardTargetingGA.SourceEffect = this;
                startCardTargetingGA.ActivateToolTip = ActivateToolTip;
                return startCardTargetingGA;
            }
        }
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

        return new DiscardEffect(
            EffectID,
            ActivateToolTip,
            Priority,
            DiscardAmount,
            multiplyAmount,
            PayXEffect,
            PayXValue,
            MultiHit,
            ActivateNumber,
            ActivateLeft,
            ORChoice,
            DynamicConditionInfos,
            targetLimitations,
            TargetUpTo,
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
            DiscardAll,
            SFX,
            ConditionTested,
            TypeOfCounter,
            CounterValue,
            ModuloValue
        );
    }
}
