using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FMODUnity;
using SerializeReferenceEditor;

public class ShieldEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public TargetMode targetMode;

    [Header("For Manual Target only")]
    [SerializeField] private bool TargetUpTo;
    public override bool EffectTargetUpTo => TargetUpTo;

    [SerializeField] private int targetNumber;
    public override int EffectTargetNumber => targetNumber;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public ShieldEffect() { }

    public ShieldEffect(TargetMode TargetMode, int testValue,DynamicCondition dynamicCondition, DynamicAmount testDynamicAmount,PermaTypes testType, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, Events Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, EventReference sfx)
    {
        targetMode = TargetMode;
        targetNumber = TargetNumber;
        TargetUpTo = targetUpTo;
        targetLimitations = TargetLimitations;
        actionnerType = ActionnerType;
        Events = Event;
        TestValue = testValue;
        TestDynamicAmount = testDynamicAmount;
        DynamicCondition = dynamicCondition;
        TestType = testType;
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
        // SI CARTE
        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetMode == TargetMode.Manual)
            {
                ShieldGA shieldGA = new(null, null);
                if (AudioManager.Instance.IsValid(SFX)) { shieldGA.SFX = SFX; }
                StartManualTargetingGA startManualTargetingGA = new(shieldGA, targetNumber,TargetUpTo, this,targetLimitations);
                return startManualTargetingGA;
            }
            else if (targetMode == TargetMode.EffectParent_Targets)
            {
                ShieldGA shieldGA = new(ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                if (AudioManager.Instance.IsValid(SFX)) { shieldGA.SFX = SFX; }
                return shieldGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, null);
                TargetForLinked_Player = playerTargets;
                TargetForLinked_Enemy = enemyTargets;

                ShieldGA shieldGA = new(playerTargets, enemyTargets);
                if (AudioManager.Instance.IsValid(SFX)) { shieldGA.SFX = SFX; }
                return shieldGA;
            }
        }
        // SI PERMANENT
        else
        {
            // SI ENEMY
            if (actionnerType == ActionnerType.ENEMY && Actionner != null)
            {
                if (targetMode == TargetMode.Manual)
                {
                    ShieldEnemyGA shieldEnemyGA = new(null, null);
                    shieldEnemyGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { shieldEnemyGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(shieldEnemyGA, targetNumber,TargetUpTo, this,targetLimitations);
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

                    ShieldEnemyGA shieldEnemyGA = new(playerTargets, enemyTargets);
                    shieldEnemyGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { shieldEnemyGA.SFX = SFX; }
                    return shieldEnemyGA;
                }
            }
            // SI PLAYER
            else if (actionnerType == ActionnerType.PLAYER && Actionner != null)
            {
                if (targetMode == TargetMode.Manual)
                {
                    ShieldPlayerGA shieldPlayerGA = new(null, null);
                    shieldPlayerGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { shieldPlayerGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(shieldPlayerGA, targetNumber,TargetUpTo, this,targetLimitations);
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

                    ShieldPlayerGA shieldPlayerGA = new(playerTargets, enemyTargets);
                    shieldPlayerGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { shieldPlayerGA.SFX = SFX; }
                    return shieldPlayerGA;
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

        return new ShieldEffect(
            targetMode,
            TestValue,
            DynamicCondition,
            TestDynamicAmount,
            TestType,
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
            SFX
        );
    }

}
