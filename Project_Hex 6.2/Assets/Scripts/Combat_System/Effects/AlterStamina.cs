using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class AlterStamina : Effect
{
    [Header("Effect Param")]

    [SerializeField] public int alterAmount;
    [SerializeField] public DynamicAmount DynamicAmount;
    [SerializeField] public TargetMode targetMode;
    [SerializeField] public PermaTypes permaTypes;

    [Header("For Manual Target only")]
    [SerializeField] private bool TargetUpTo;
    public override bool EffectTargetUpTo => TargetUpTo;

    [SerializeField] private int targetNumber;
    public override int EffectTargetNumber => targetNumber;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public AlterStamina() { }

    public AlterStamina(int AlterAmount, int multiHit, int testValue, DynamicCondition dynamicCondition, DynamicAmount testDynamicAmount, PermaTypes testType, TargetMode TargetMode, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, Events Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, PermaTypes PermaTypes, DynamicAmount dynamicAmount, EventReference sfx)
    {

        alterAmount = AlterAmount;
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
        permaTypes = PermaTypes;
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
                AlterStaminaGA alterStaminaGA = new(alterAmount, DynamicAmount, permaTypes, null);
                if (AudioManager.Instance.IsValid(SFX)) { alterStaminaGA.SFX = SFX; }
                StartManualTargetingGA startManualTargetingGA = new(alterStaminaGA, targetNumber,TargetUpTo, this,targetLimitations);
                return startManualTargetingGA;
            }
            else if (targetMode == TargetMode.EffectParent_Targets)
            {
                AlterStaminaGA alterStaminaGA = new(alterAmount, DynamicAmount, permaTypes, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                if (AudioManager.Instance.IsValid(SFX)) { alterStaminaGA.SFX = SFX; }
                return alterStaminaGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, null);

                TargetForLinked_Player = playerTargets;
                TargetForLinked_Enemy = enemyTargets;

                AlterStaminaGA alterStaminaGA = new(alterAmount, DynamicAmount, permaTypes, playerTargets, enemyTargets);
                if (AudioManager.Instance.IsValid(SFX)) { alterStaminaGA.SFX = SFX; }
                return alterStaminaGA;
            }
        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY)
            {
                if (targetMode == TargetMode.Manual)
                {
                    EnemyAlterStaminaGA enemyAlterStaminaGA = new(alterAmount, DynamicAmount, permaTypes, null, null, targetMode);
                    enemyAlterStaminaGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { enemyAlterStaminaGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(enemyAlterStaminaGA, targetNumber,TargetUpTo, this,targetLimitations);
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

                    EnemyAlterStaminaGA enemyAlterStaminaGA = new(alterAmount, DynamicAmount, permaTypes, playerTargets, enemyTargets);
                    enemyAlterStaminaGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { enemyAlterStaminaGA.SFX = SFX; }
                    return enemyAlterStaminaGA;
                }
            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (targetMode == TargetMode.Manual)
                {
                    PlayerAlterStaminaGA playerAlterStaminaGA = new(alterAmount, DynamicAmount, permaTypes, null, null, targetMode);
                    playerAlterStaminaGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { playerAlterStaminaGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(playerAlterStaminaGA, targetNumber,TargetUpTo, this,targetLimitations);
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

                    PlayerAlterStaminaGA playerAlterStaminaGA = new(alterAmount, DynamicAmount, permaTypes, playerTargets, enemyTargets);
                    playerAlterStaminaGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { playerAlterStaminaGA.SFX = SFX; }
                    return playerAlterStaminaGA;
                }
            }
            else
            {
                Debug.Log("Effect.GetGameAction returned Null");
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

        return new AlterStamina(
            alterAmount,
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
            permaTypes,
            DynamicAmount,
            SFX
        );
    }
}
