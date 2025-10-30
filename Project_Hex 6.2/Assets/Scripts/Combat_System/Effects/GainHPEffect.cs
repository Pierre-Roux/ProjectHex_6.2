using System;
using System.Collections.Generic;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class GainHPEffect : Effect
{
    [Header("Effect Param")]

    [SerializeField] public int GainAmount;
    [SerializeField] public DynamicAmount DynamicAmount;
    [SerializeField] public TargetMode targetMode;
    [SerializeField] public bool passive;
    [SerializeField] public PermaTypes permaTypes;

    [Header("For Manual Target only")]
    [SerializeField] private bool TargetUpTo;
    public override bool EffectTargetUpTo => TargetUpTo;

    [SerializeField] private int targetNumber;
    public override int EffectTargetNumber => targetNumber;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public GainHPEffect() { }

    public GainHPEffect(int gainAmount, int multiHit, int testValue,DynamicCondition dynamicCondition, DynamicAmount testDynamicAmount,PermaTypes testType, TargetMode TargetMode, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, Events Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool Passive, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, PermaTypes PermaTypes, DynamicAmount dynamicAmount, EventReference sfx)
    {
        GainAmount = gainAmount;
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
        passive = Passive;
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
            if (passive)
            {
                GainLifeGA gainLifeGA = new(GainAmount, DynamicAmount, passive, permaTypes, null, null, targetMode);
                if (AudioManager.Instance.IsValid(SFX)) { gainLifeGA.SFX = SFX; }
                return gainLifeGA;
            }
            else
            {
                if (targetMode == TargetMode.Manual)
                {
                    GainLifeGA gainLifeGA = new(GainAmount, DynamicAmount, passive, permaTypes, null);
                    if (AudioManager.Instance.IsValid(SFX)) { gainLifeGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(gainLifeGA, targetNumber,TargetUpTo, this,targetLimitations);
                    return startManualTargetingGA;
                }
                else if (targetMode == TargetMode.EffectParent_Targets)
                {
                    GainLifeGA gainLifeGA = new(GainAmount, DynamicAmount, passive, permaTypes, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                    if (AudioManager.Instance.IsValid(SFX)) { gainLifeGA.SFX = SFX; }
                    return gainLifeGA;
                }
                else
                {
                    var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetMode, null);

                    TargetForLinked_Player = playerTargets;
                    TargetForLinked_Enemy = enemyTargets;

                    GainLifeGA gainLifeGA = new(GainAmount, DynamicAmount, passive, permaTypes, playerTargets, enemyTargets);
                    if (AudioManager.Instance.IsValid(SFX)) { gainLifeGA.SFX = SFX; }
                    return gainLifeGA;
                }
            }

        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY)
            {
                if (passive)
                {
                    EnemyGainLifeGA enemyGainLifeGA = new(GainAmount, DynamicAmount, passive, permaTypes, null, null, targetMode);
                    enemyGainLifeGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { enemyGainLifeGA.SFX = SFX; }
                    return enemyGainLifeGA;
                }
                else
                {
                    if (targetMode == TargetMode.Manual)
                    {
                        EnemyGainLifeGA enemyGainLifeGA = new(GainAmount, DynamicAmount, passive, permaTypes, null, null);
                        enemyGainLifeGA.Actionner = Actionner;
                        if (AudioManager.Instance.IsValid(SFX)) { enemyGainLifeGA.SFX = SFX; }
                        StartManualTargetingGA startManualTargetingGA = new(enemyGainLifeGA, targetNumber,TargetUpTo, this,targetLimitations);
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

                        EnemyGainLifeGA enemyGainLifeGA = new(GainAmount, DynamicAmount, passive, permaTypes, playerTargets, enemyTargets);
                        enemyGainLifeGA.Actionner = Actionner;
                        if (AudioManager.Instance.IsValid(SFX)) { enemyGainLifeGA.SFX = SFX; }
                        return enemyGainLifeGA;
                    }
                }

            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (passive)
                {
                    PlayerGainLifeGA playerGainLifeGA = new(GainAmount, DynamicAmount, passive, permaTypes, null, null, targetMode);
                    playerGainLifeGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { playerGainLifeGA.SFX = SFX; }
                    return playerGainLifeGA;
                }
                else
                {
                    if (targetMode == TargetMode.Manual)
                    {
                        PlayerGainLifeGA playerGainLifeGA = new(GainAmount, DynamicAmount, passive, permaTypes, null, null);
                        playerGainLifeGA.Actionner = Actionner;
                        if (AudioManager.Instance.IsValid(SFX)) { playerGainLifeGA.SFX = SFX; }
                        StartManualTargetingGA startManualTargetingGA = new(playerGainLifeGA, targetNumber,TargetUpTo, this,targetLimitations);
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

                        PlayerGainLifeGA playerGainLifeGA = new(GainAmount, DynamicAmount, passive, permaTypes, playerTargets, enemyTargets);
                        playerGainLifeGA.Actionner = Actionner;
                        if (AudioManager.Instance.IsValid(SFX)) { playerGainLifeGA.SFX = SFX; }
                        return playerGainLifeGA;
                    }
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

        return new GainHPEffect(
            GainAmount,
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
            passive,
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
