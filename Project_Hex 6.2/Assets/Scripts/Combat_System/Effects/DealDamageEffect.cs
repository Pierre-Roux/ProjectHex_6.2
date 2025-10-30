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
    [SerializeField] public DynamicAmount DynamicAmount;
    public TargetMode targetMode;
    public override TargetMode EffectTargetMode => targetMode;

    [Header("For Manual Target only")]

    [SerializeField] private bool TargetUpTo;
    public override bool EffectTargetUpTo => TargetUpTo;

    [SerializeField] private int targetNumber;
    public override int EffectTargetNumber => targetNumber;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public DealDamageEffect() { }

    public DealDamageEffect(int DamageAmount, int multiHit, int testValue,DynamicCondition dynamicCondition, DynamicAmount testDynamicAmount,PermaTypes testType, TargetMode TargetMode, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, Events Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount, EventReference sfx)
    {
        damageAmount = DamageAmount;
        MultiHit = multiHit;
        targetMode = TargetMode;
        TestValue = testValue;
        TestDynamicAmount = testDynamicAmount;
        DynamicCondition = dynamicCondition;
        TestType = testType;
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

        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetMode == TargetMode.Manual)
            {
                DealDamageGA dealDamageGA = new(damageAmount, DynamicAmount, null, null);
                dealDamageGA.CardActionner = CardActionner;
                if (AudioManager.Instance.IsValid(SFX)) { dealDamageGA.SFX = SFX; }
                StartManualTargetingGA startManualTargetingGA = new(dealDamageGA, targetNumber, TargetUpTo, this, targetLimitations);
                return startManualTargetingGA;
            }
            else if (targetMode == TargetMode.EffectParent_Targets)
            {
                DealDamageGA dealDamageGA = new(damageAmount, DynamicAmount, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                dealDamageGA.CardActionner = CardActionner;
                if (AudioManager.Instance.IsValid(SFX)) { dealDamageGA.SFX = SFX; }
                return dealDamageGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, null);
                TargetForLinked_Player = playerTargets;
                TargetForLinked_Enemy = enemyTargets;

                DealDamageGA dealDamageGA = new(damageAmount, DynamicAmount, playerTargets, enemyTargets);
                dealDamageGA.CardActionner = CardActionner;
                if (AudioManager.Instance.IsValid(SFX)) { dealDamageGA.SFX = SFX; }
                return dealDamageGA;
            }
        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY)
            {
                if (targetMode == TargetMode.Manual)
                {
                    AttackPlayerGA attackPlayerGA = new(damageAmount, DynamicAmount, null, null);
                    attackPlayerGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { attackPlayerGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(attackPlayerGA, targetNumber,TargetUpTo, this,targetLimitations);
                    return startManualTargetingGA;
                }
                else
                {
                    List<PermanentView> playerTargets;
                    List<EnemySlotView> enemyTargets;

                    if (targetMode == TargetMode.EffectParent_Targets)
                    {
                        playerTargets = ParentEffect.TargetForLinked_Player;
                        enemyTargets = ParentEffect.TargetForLinked_Enemy;
                    }
                    else
                    {
                        (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, Actionner);

                        TargetForLinked_Player = playerTargets;
                        TargetForLinked_Enemy = enemyTargets;
                    }

                    AttackPlayerGA attackPlayerGA = new(damageAmount, DynamicAmount, playerTargets, enemyTargets);
                    attackPlayerGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { attackPlayerGA.SFX = SFX; }
                    return attackPlayerGA;
                }
            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (targetMode == TargetMode.Manual)
                {
                    AttackEnemyGA attackEnemyGA = new(damageAmount, DynamicAmount, null, null);
                    attackEnemyGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { attackEnemyGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(attackEnemyGA, targetNumber,TargetUpTo, this,targetLimitations);
                    return startManualTargetingGA;
                }
                else
                {
                    List<PermanentView> playerTargets;
                    List<EnemySlotView> enemyTargets;

                    if (targetMode == TargetMode.EffectParent_Targets)
                    {
                        playerTargets = ParentEffect.TargetForLinked_Player;
                        enemyTargets = ParentEffect.TargetForLinked_Enemy;
                    }
                    else
                    {
                        (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, Actionner);

                        TargetForLinked_Player = playerTargets;
                        TargetForLinked_Enemy = enemyTargets;
                    }

                    AttackEnemyGA attackEnemyGA = new(damageAmount, DynamicAmount, playerTargets, enemyTargets);
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
            MultiHit,
            TestValue,
            DynamicCondition,
            TestDynamicAmount,
            TestType,
            targetMode,
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
