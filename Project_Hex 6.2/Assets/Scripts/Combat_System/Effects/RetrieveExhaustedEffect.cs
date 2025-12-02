using UnityEngine;
using System.Collections.Generic;
using System;
using FMODUnity;
using SerializeReferenceEditor;

public class RetrieveExhaustedEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public TargetModeInfo targetModeInfo;
    [SerializeField] public override TargetModeInfo EffectTargetModeInfo => targetModeInfo;

    [Header("For Manual Target only")]
    [SerializeField] private bool TargetUpTo = true;
    public override bool EffectTargetUpTo => TargetUpTo;

    [SerializeField] private int targetNumber = 1;
    public override int EffectTargetNumber => targetNumber;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public RetrieveExhaustedEffect() { }

    public RetrieveExhaustedEffect(string effectID, bool activateToolTip, int priority, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, bool orChoice, List<DynamicConditionInfo> dynamicConditionInfos, TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, List<Events> Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, EventReference sfx,CounterType typeOfCounter, int counterValue, bool moduloValue)
    {
        Priority = priority;
        ActivateToolTip = activateToolTip;
        EffectID = effectID;
        PayXEffect = payXEffect;
        PayXValue = payXValue;
        MultiHit = multiHit;
        ActivateNumber = activateNumber;
        ActivateLeft = activateLeft;
        ORChoice = orChoice;
        targetModeInfo = TargetModeInfo;
        DynamicConditionInfos = dynamicConditionInfos;
        targetNumber = TargetNumber;
        TargetUpTo = targetUpTo;
        targetLimitations = TargetLimitations;
        actionnerType = ActionnerType;
        Events = Event;
        CancelOnDeath = cancelOnDeath;
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
        SFX = sfx;
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

        // SI CARTE
        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetModeInfo.targetMode == TargetMode.Manual)
            {
                RetrieveExhaustedGA retrieveExhaustedGA = new(null);
                retrieveExhaustedGA.CardActionner = CardActionner;
                retrieveExhaustedGA.SourceEffect = this;
                retrieveExhaustedGA.ActivateToolTip = false;
                retrieveExhaustedGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_HealSound : SFX;
                StartCardTargetingGA startCardTargetingGA = new(retrieveExhaustedGA, targetNumber, TargetUpTo, this, targetLimitations,true);
                startCardTargetingGA.SourceEffect = this;
                startCardTargetingGA.ActivateToolTip = ActivateToolTip;
                return startCardTargetingGA;
            }
            else if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
            {
                RetrieveExhaustedGA retrieveExhaustedGA = new(ParentEffect.TargetForLinked_Card);
                retrieveExhaustedGA.CardActionner = CardActionner;
                retrieveExhaustedGA.SourceEffect = this;
                retrieveExhaustedGA.ActivateToolTip = ActivateToolTip;
                retrieveExhaustedGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_HealSound : SFX;
                return retrieveExhaustedGA;
            }
            else
            {
                var cardsTargets = TargetSystem.GetCardsTargets(targetModeInfo, null, false,true);
                TargetForLinked_Card = cardsTargets;

                RetrieveExhaustedGA retrieveExhaustedGA = new(cardsTargets);
                retrieveExhaustedGA.CardActionner = CardActionner;
                retrieveExhaustedGA.SourceEffect = this;
                retrieveExhaustedGA.ActivateToolTip = ActivateToolTip;
                retrieveExhaustedGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_HealSound : SFX;
                return retrieveExhaustedGA;
            }
        }
        // SI PERMANENT
        else
        {
            // SI ENEMY
            if (actionnerType == ActionnerType.ENEMY)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    EnemyRetrieveExhaustedGA enemyRetrieveExhaustedGA = new(null);
                    enemyRetrieveExhaustedGA.Actionner = Actionner;
                    enemyRetrieveExhaustedGA.SourceEffect = this;
                    enemyRetrieveExhaustedGA.ActivateToolTip = false;
                    enemyRetrieveExhaustedGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_HealSound : SFX;
                    StartCardTargetingGA startManualTargetingGA = new(enemyRetrieveExhaustedGA, targetNumber, TargetUpTo, this, targetLimitations,true);
                    startManualTargetingGA.SourceEffect = this;
                    startManualTargetingGA.ActivateToolTip = ActivateToolTip;
                    return startManualTargetingGA;
                }
                else
                {
                    List<Card> cardsTargets;

                    if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
                    {
                        cardsTargets = ParentEffect.TargetForLinked_Card;
                    }
                    else
                    {
                        cardsTargets = TargetSystem.GetCardsTargets(targetModeInfo, CardActionner,false,true);

                        TargetForLinked_Card = cardsTargets;
                    }

                    EnemyRetrieveExhaustedGA enemyRetrieveExhaustedGA = new(cardsTargets);
                    enemyRetrieveExhaustedGA.Actionner = Actionner;
                    enemyRetrieveExhaustedGA.SourceEffect = this;
                    enemyRetrieveExhaustedGA.ActivateToolTip = ActivateToolTip;
                    enemyRetrieveExhaustedGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_HealSound : SFX;
                    return enemyRetrieveExhaustedGA;
                }
            }
            // SI PLAYER
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    PlayerRetrieveExhaustedGA playerRetrieveExhaustedGA = new(null);
                    playerRetrieveExhaustedGA.Actionner = Actionner;
                    playerRetrieveExhaustedGA.SourceEffect = this;
                    playerRetrieveExhaustedGA.ActivateToolTip = false;
                    playerRetrieveExhaustedGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_HealSound : SFX;
                    StartCardTargetingGA startCardTargetingGA = new(playerRetrieveExhaustedGA, targetNumber, TargetUpTo, this, targetLimitations,true);
                    startCardTargetingGA.SourceEffect = this;
                    startCardTargetingGA.ActivateToolTip = ActivateToolTip;
                    return startCardTargetingGA;
                }
                else
                {
                    List<Card> cardsTargets;

                    if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
                    {
                        cardsTargets = ParentEffect.TargetForLinked_Card;
                    }
                    else
                    {
                        cardsTargets = TargetSystem.GetCardsTargets(targetModeInfo, CardActionner,false,true);

                        TargetForLinked_Card = cardsTargets;
                    }

                    PlayerRetrieveExhaustedGA playerRetrieveExhaustedGA = new(cardsTargets);
                    playerRetrieveExhaustedGA.Actionner = Actionner;
                    playerRetrieveExhaustedGA.SourceEffect = this;
                    playerRetrieveExhaustedGA.ActivateToolTip = ActivateToolTip;
                    playerRetrieveExhaustedGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_HealSound : SFX;
                    return playerRetrieveExhaustedGA;
                }
            }
            // NEVER
            else
            {
                Debug.LogError("Effect.GetGameAction returned Null");
                return null;
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

        return new RetrieveExhaustedEffect(
            EffectID,
            ActivateToolTip,
            Priority,
            PayXEffect,
            PayXValue,
            MultiHit,
            ActivateNumber,
            ActivateLeft,
            ORChoice,
            DynamicConditionInfos,
            targetModeInfo,
            targetLimitations,
            targetNumber,
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
            SFX,
            TypeOfCounter,
            CounterValue,
            ModuloValue
        );
    }    
}
