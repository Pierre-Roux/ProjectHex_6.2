using System.Collections.Generic;
using UnityEngine;
using System;
using FMODUnity;
using SerializeReferenceEditor;

public class ArmorEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public int amount;
    [SerializeField] public int multiplyAmount = 1;
    [SerializeField] public DynamicAmountInfo DynamicAmountInfo;
    [SerializeField] public TargetModeInfo targetModeInfo;
    [SerializeField] public override TargetModeInfo EffectTargetModeInfo => targetModeInfo;

    [Header("For Manual Target only")]
    [SerializeField] private bool TargetUpTo = true;
    public override bool EffectTargetUpTo => TargetUpTo;

    [SerializeField] private int targetNumber = 1;
    public override int EffectTargetNumber => targetNumber;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public ArmorEffect() { }

    public ArmorEffect(string effectID, bool activateToolTip, int priority, bool hollowEffect, int Amount, int MultiplyAmount, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, bool orChoice, List<DynamicConditionInfo> dynamicConditionInfos, TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, List<EventInfo> Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, EventInfo durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmountInfo dynamicAmountInfo, EventReference sfx,CounterTypeInfo typeOfCounter, int counterValue, bool moduloValue)
    {
        Priority = priority;
        HollowEffect = hollowEffect;
        ActivateToolTip = activateToolTip;
        EffectID = effectID;
        amount = Amount;
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
        EventInfos = Event;
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
        DynamicAmountInfo = dynamicAmountInfo;
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
            DynamicAmountInfo.DynamicAmount = DynamicAmount.NULL;
            amount = PayXValue;
        }

        // SI CARTE
        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetModeInfo.targetMode == TargetMode.Manual)
            {
                ArmorGA ArmorGA = new(amount, multiplyAmount, DynamicAmountInfo, null, null);
                ArmorGA.CardActionner = CardActionner;
                ArmorGA.SourceEffect = this;
                ArmorGA.ActivateToolTip = false;
                ArmorGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_ArmorSound : SFX;
                StartManualTargetingGA startManualTargetingGA = new(ArmorGA, targetNumber, TargetUpTo, this, targetLimitations);
                startManualTargetingGA.SourceEffect = this;
                startManualTargetingGA.ActivateToolTip = ActivateToolTip;
                return startManualTargetingGA;
            }
            else if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
            {
                ArmorGA ArmorGA = new(amount, multiplyAmount, DynamicAmountInfo, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                ArmorGA.CardActionner = CardActionner;
                ArmorGA.SourceEffect = this;
                ArmorGA.ActivateToolTip = ActivateToolTip;
                ArmorGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_ArmorSound : SFX;
                return ArmorGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, null, this);
                TargetForLinked_Player = playerTargets;
                TargetForLinked_Enemy = enemyTargets;

                ArmorGA ArmorGA = new(amount, multiplyAmount, DynamicAmountInfo, playerTargets, enemyTargets);
                ArmorGA.CardActionner = CardActionner;
                ArmorGA.SourceEffect = this;
                ArmorGA.ActivateToolTip = ActivateToolTip;
                ArmorGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_ArmorSound : SFX;
                return ArmorGA;
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
                    ArmorEnemyGA ArmorEnemyGA = new(amount, multiplyAmount, DynamicAmountInfo, null, null);
                    ArmorEnemyGA.Actionner = Actionner;
                    ArmorEnemyGA.SourceEffect = this;
                    ArmorEnemyGA.ActivateToolTip = false;
                    ArmorEnemyGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_ArmorSound : SFX;
                    StartManualTargetingGA startManualTargetingGA = new(ArmorEnemyGA, targetNumber, TargetUpTo, this, targetLimitations);
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

                    ArmorEnemyGA ArmorEnemyGA = new(amount, multiplyAmount, DynamicAmountInfo, playerTargets, enemyTargets);
                    ArmorEnemyGA.Actionner = Actionner;
                    ArmorEnemyGA.SourceEffect = this;
                    ArmorEnemyGA.ActivateToolTip = ActivateToolTip;
                    ArmorEnemyGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_ArmorSound : SFX;
                    return ArmorEnemyGA;
                }
            }
            // SI PLAYER
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    ArmorPlayerGA ArmorPlayerGA = new(amount, multiplyAmount, DynamicAmountInfo, null, null);
                    ArmorPlayerGA.Actionner = Actionner;
                    ArmorPlayerGA.SourceEffect = this;
                    ArmorPlayerGA.ActivateToolTip = false;
                    ArmorPlayerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_ArmorSound : SFX;
                    StartManualTargetingGA startManualTargetingGA = new(ArmorPlayerGA, targetNumber, TargetUpTo, this, targetLimitations);
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

                    ArmorPlayerGA ArmorPlayerGA = new(amount, multiplyAmount, DynamicAmountInfo, playerTargets, enemyTargets);
                    ArmorPlayerGA.Actionner = Actionner;
                    ArmorPlayerGA.SourceEffect = this;
                    ArmorPlayerGA.ActivateToolTip = ActivateToolTip;
                    ArmorPlayerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_ArmorSound : SFX;
                    return ArmorPlayerGA;
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

        return new ArmorEffect(
            EffectID,
            ActivateToolTip,
            Priority,
            HollowEffect,
            amount,
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
            EventInfos,
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
            DynamicAmountInfo,
            SFX,
            TypeOfCounter,
            CounterValue,
            ModuloValue
        );
    }
}
