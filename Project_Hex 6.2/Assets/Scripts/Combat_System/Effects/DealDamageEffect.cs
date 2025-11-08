using System;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class DealDamageEffect : Effect
{
    [Header("Effect Param")]

    [SerializeField] public int damageAmount;
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

    public DealDamageEffect() { }

    public DealDamageEffect(int DamageAmount, int MultiplyAmount, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft,List<DynamicConditionInfo> dynamicConditionInfos, TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, Events Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount, EventReference sfx)
    {
        damageAmount = DamageAmount;
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
            damageAmount = PayXValue;
        }

        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetModeInfo.targetMode == TargetMode.Manual)
            {
                DealDamageGA dealDamageGA = new(damageAmount,0,multiplyAmount, DynamicAmount, null, null);
                dealDamageGA.CardActionner = CardActionner;
                if (AudioManager.Instance.IsValid(SFX)) { dealDamageGA.SFX = SFX; }
                StartManualTargetingGA startManualTargetingGA = new(dealDamageGA, targetNumber, TargetUpTo, this, targetLimitations);
                return startManualTargetingGA;
            }
            else if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
            {
                DealDamageGA dealDamageGA = new(damageAmount,0,multiplyAmount, DynamicAmount, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                dealDamageGA.CardActionner = CardActionner;
                if (AudioManager.Instance.IsValid(SFX)) { dealDamageGA.SFX = SFX; }
                return dealDamageGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, null);
                TargetForLinked_Player = playerTargets;
                TargetForLinked_Enemy = enemyTargets;

                DealDamageGA dealDamageGA = new(damageAmount,0,multiplyAmount, DynamicAmount, playerTargets, enemyTargets);
                dealDamageGA.CardActionner = CardActionner;
                if (AudioManager.Instance.IsValid(SFX)) { dealDamageGA.SFX = SFX; }
                return dealDamageGA;
            }
        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    AttackPlayerGA attackPlayerGA = new(damageAmount,multiplyAmount, DynamicAmount, null, null);
                    attackPlayerGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { attackPlayerGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(attackPlayerGA, targetNumber,TargetUpTo, this,targetLimitations);
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

                    AttackPlayerGA attackPlayerGA = new(damageAmount,multiplyAmount, DynamicAmount, playerTargets, enemyTargets);
                    attackPlayerGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { attackPlayerGA.SFX = SFX; }
                    return attackPlayerGA;
                }
            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    AttackEnemyGA attackEnemyGA = new(damageAmount,multiplyAmount, DynamicAmount, null, null);
                    attackEnemyGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { attackEnemyGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(attackEnemyGA, targetNumber,TargetUpTo, this,targetLimitations);
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

                    AttackEnemyGA attackEnemyGA = new(damageAmount,multiplyAmount, DynamicAmount, playerTargets, enemyTargets);
                    attackEnemyGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { attackEnemyGA.SFX = SFX; }
                    return attackEnemyGA;
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

        return new DealDamageEffect(
            damageAmount,
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
