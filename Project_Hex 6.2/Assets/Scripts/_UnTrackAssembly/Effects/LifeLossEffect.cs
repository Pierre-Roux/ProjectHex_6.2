using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class LifeLossEffect : Effect
{
    [Header("Effect Param")]

    [SerializeField] public int LifeLossAmount;
    [SerializeField] public int multiplyAmount = 1;
    [SerializeField] public DynamicAmount DynamicAmount;
    [SerializeField] public TargetModeInfo targetModeInfo;
    [SerializeField] public override TargetModeInfo EffectTargetModeInfo => targetModeInfo;

    [Header("For Manual Target only")]
    [SerializeField] private bool TargetUpTo = true;
    public override bool EffectTargetUpTo => TargetUpTo;

    [SerializeField] private int targetNumber = 1;
    public override int EffectTargetNumber => targetNumber;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public LifeLossEffect() { }

    public LifeLossEffect(string effectID, bool activateToolTip, int priority, bool hollowEffect, int lifeLossAmount, int MultiplyAmount, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, bool orChoice, List<DynamicConditionInfo> dynamicConditionInfos, TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, List<Events> Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount, EventReference sfx,CounterType typeOfCounter, int counterValue, bool moduloValue)
    {
        Priority = priority;
        HollowEffect = hollowEffect;
        ActivateToolTip = activateToolTip;
        EffectID = effectID;
        LifeLossAmount = lifeLossAmount;
        multiplyAmount = MultiplyAmount;
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
        DynamicAmount = dynamicAmount;
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

        if (PayXValue != 0)
        {
            DynamicAmount = DynamicAmount.NULL;
            LifeLossAmount = PayXValue;
        }

        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetModeInfo.targetMode == TargetMode.Manual)
            {
                LifeLossGA lifeLossGA = new(LifeLossAmount, multiplyAmount, DynamicAmount, null, null);
                lifeLossGA.CardActionner = CardActionner;
                lifeLossGA.SourceEffect = this;
                lifeLossGA.ActivateToolTip = false;
                lifeLossGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_LifeLossSound : SFX;
                StartManualTargetingGA startManualTargetingGA = new(lifeLossGA, targetNumber, TargetUpTo, this, targetLimitations);
                startManualTargetingGA.SourceEffect = this;
                startManualTargetingGA.ActivateToolTip = ActivateToolTip;
                return startManualTargetingGA;
            }
            else if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
            {
                LifeLossGA lifeLossGA = new(LifeLossAmount, multiplyAmount, DynamicAmount, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                lifeLossGA.CardActionner = CardActionner;
                lifeLossGA.SourceEffect = this;
                lifeLossGA.ActivateToolTip = ActivateToolTip;
                lifeLossGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_LifeLossSound : SFX;
                return lifeLossGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, null, this);
                TargetForLinked_Player = playerTargets;
                TargetForLinked_Enemy = enemyTargets;

                LifeLossGA lifeLossGA = new(LifeLossAmount, multiplyAmount, DynamicAmount, playerTargets, enemyTargets);
                lifeLossGA.CardActionner = CardActionner;
                lifeLossGA.SourceEffect = this;
                lifeLossGA.ActivateToolTip = ActivateToolTip;
                lifeLossGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_LifeLossSound : SFX;
                return lifeLossGA;
            }
        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    EnemyLifeLossGA enemyLifeLossGA = new(LifeLossAmount, multiplyAmount, DynamicAmount, null, null);
                    enemyLifeLossGA.Actionner = Actionner;
                    enemyLifeLossGA.SourceEffect = this;
                    enemyLifeLossGA.ActivateToolTip = false;
                    enemyLifeLossGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_LifeLossSound : SFX;
                    StartManualTargetingGA startManualTargetingGA = new(enemyLifeLossGA, targetNumber, TargetUpTo, this, targetLimitations);
                    startManualTargetingGA.SourceEffect = this;
                    startManualTargetingGA.ActivateToolTip = ActivateToolTip;
                    return startManualTargetingGA;
                }
                else
                {
                    List<PermanentView> playerTargets;
                    List<EnemySlotView> enemyTargets;

                    if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
                    {
                        playerTargets = ParentEffect.TargetForLinked_Player;
                        enemyTargets = ParentEffect.TargetForLinked_Enemy;
                    }
                    else
                    {
                        (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, Actionner, this);

                        TargetForLinked_Player = playerTargets;
                        TargetForLinked_Enemy = enemyTargets;
                    }

                    EnemyLifeLossGA enemyLifeLossGA = new(LifeLossAmount, multiplyAmount, DynamicAmount, playerTargets, enemyTargets);
                    enemyLifeLossGA.Actionner = Actionner;
                    enemyLifeLossGA.SourceEffect = this;
                    enemyLifeLossGA.ActivateToolTip = ActivateToolTip;
                    enemyLifeLossGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_LifeLossSound : SFX;
                    return enemyLifeLossGA;
                }
            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    PlayerLifeLossGA playerLifeLossGA = new(LifeLossAmount, multiplyAmount, DynamicAmount, null, null);
                    playerLifeLossGA.Actionner = Actionner;
                    playerLifeLossGA.SourceEffect = this;
                    playerLifeLossGA.ActivateToolTip = false;
                    playerLifeLossGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_LifeLossSound : SFX;
                    StartManualTargetingGA startManualTargetingGA = new(playerLifeLossGA, targetNumber, TargetUpTo, this, targetLimitations);
                    startManualTargetingGA.SourceEffect = this;
                    startManualTargetingGA.ActivateToolTip = ActivateToolTip;
                    return startManualTargetingGA;
                }
                else
                {
                    List<PermanentView> playerTargets;
                    List<EnemySlotView> enemyTargets;

                    if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
                    {
                        playerTargets = ParentEffect.TargetForLinked_Player;
                        enemyTargets = ParentEffect.TargetForLinked_Enemy;
                    }
                    else
                    {
                        (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, Actionner, this);

                        TargetForLinked_Player = playerTargets;
                        TargetForLinked_Enemy = enemyTargets;
                    }

                    PlayerLifeLossGA playerLifeLossGA = new(LifeLossAmount, multiplyAmount, DynamicAmount, playerTargets, enemyTargets);
                    playerLifeLossGA.Actionner = Actionner;
                    playerLifeLossGA.SourceEffect = this;
                    playerLifeLossGA.ActivateToolTip = ActivateToolTip;
                    playerLifeLossGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_LifeLossSound : SFX;
                    return playerLifeLossGA;
                }
            }
            else
            {
                Debug.LogError("Effect.GetGameAction returned Null");
                return null;
            }
        }
    }
    
    public override GameAction GetCounterMesure()
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

        return new LifeLossEffect(
            EffectID,
            ActivateToolTip,
            Priority,
            HollowEffect,
            LifeLossAmount,
            multiplyAmount,
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
            DynamicAmount,
            SFX,
            TypeOfCounter,
            CounterValue,
            ModuloValue
        );
    }
}
