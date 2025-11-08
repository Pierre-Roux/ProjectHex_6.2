using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private int targetNumber;
    public override int EffectTargetNumber => targetNumber;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public LifeLossEffect() { }

    public LifeLossEffect(int lifeLossAmount, int MultiplyAmount, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, List<DynamicConditionInfo> dynamicConditionInfos, TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, Events Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount, EventReference sfx)
    {
        LifeLossAmount = lifeLossAmount;
        multiplyAmount = MultiplyAmount;
        PayXEffect = payXEffect;
        PayXValue = payXValue;
        MultiHit = multiHit;
        ActivateNumber = activateNumber;
        ActivateLeft = activateLeft;
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
                LifeLossGA lifeLossGA = new(LifeLossAmount,multiplyAmount, DynamicAmount, null, null);
                lifeLossGA.CardActionner = CardActionner;
                if (AudioManager.Instance.IsValid(SFX)) { lifeLossGA.SFX = SFX; }
                StartManualTargetingGA startManualTargetingGA = new(lifeLossGA, targetNumber,TargetUpTo, this,targetLimitations);
                return startManualTargetingGA;
            }
            else if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
            {
                LifeLossGA lifeLossGA = new(LifeLossAmount,multiplyAmount, DynamicAmount, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                lifeLossGA.CardActionner = CardActionner;
                if (AudioManager.Instance.IsValid(SFX)) { lifeLossGA.SFX = SFX; }
                return lifeLossGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, null);
                TargetForLinked_Player = playerTargets;
                TargetForLinked_Enemy = enemyTargets;

                LifeLossGA lifeLossGA = new(LifeLossAmount,multiplyAmount, DynamicAmount, playerTargets, enemyTargets);
                lifeLossGA.CardActionner = CardActionner;
                if (AudioManager.Instance.IsValid(SFX)) { lifeLossGA.SFX = SFX; }
                return lifeLossGA;
            }
        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    EnemyLifeLossGA enemyLifeLossGA = new(LifeLossAmount,multiplyAmount, DynamicAmount, null, null);
                    enemyLifeLossGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { enemyLifeLossGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(enemyLifeLossGA, targetNumber,TargetUpTo, this,targetLimitations);
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
                        (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, Actionner);

                        TargetForLinked_Player = playerTargets;
                        TargetForLinked_Enemy = enemyTargets;
                    }

                    EnemyLifeLossGA enemyLifeLossGA = new(LifeLossAmount,multiplyAmount, DynamicAmount, playerTargets, enemyTargets);
                    enemyLifeLossGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { enemyLifeLossGA.SFX = SFX; }
                    return enemyLifeLossGA;
                }
            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    PlayerLifeLossGA playerLifeLossGA = new(LifeLossAmount,multiplyAmount, DynamicAmount, null, null);
                    playerLifeLossGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { playerLifeLossGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(playerLifeLossGA, targetNumber,TargetUpTo, this,targetLimitations);
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
                        (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, Actionner);

                        TargetForLinked_Player = playerTargets;
                        TargetForLinked_Enemy = enemyTargets;
                    }

                    PlayerLifeLossGA playerLifeLossGA = new(LifeLossAmount,multiplyAmount, DynamicAmount, playerTargets, enemyTargets);
                    playerLifeLossGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { playerLifeLossGA.SFX = SFX; }
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
            LifeLossAmount,
            multiplyAmount,
            PayXEffect,
            PayXValue,
            MultiHit,
            ActivateNumber,
            ActivateLeft,
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
            SFX
        );
    }
}
