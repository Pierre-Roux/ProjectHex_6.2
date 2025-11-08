using System;
using System.Collections.Generic;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class GainHPEffect : Effect
{
    [Header("Effect Param")]

    [SerializeField] public int GainAmount;
    [SerializeField] public int multiplyAmount = 1;
    [SerializeField] public DynamicAmount DynamicAmount;
    [SerializeField] public TargetModeInfo targetModeInfo;
    [SerializeField] public override TargetModeInfo EffectTargetModeInfo => targetModeInfo;
    [SerializeField] public bool passive;

    [Header("For Manual Target only")]
    [SerializeField] private bool TargetUpTo = true;
    public override bool EffectTargetUpTo => TargetUpTo;

    [SerializeField] private int targetNumber;
    public override int EffectTargetNumber => targetNumber;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public GainHPEffect() { }

    public GainHPEffect(int gainAmount, int MultiplyAmount, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft,List<DynamicConditionInfo> dynamicConditionInfos, TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, Events Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool Passive, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount, EventReference sfx)
    {
        GainAmount = gainAmount;
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
        passive = Passive;
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
            GainAmount = PayXValue;
        }

        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (passive)
            {
                GainLifeGA gainLifeGA = new(GainAmount,multiplyAmount, DynamicAmount, passive, null, null, targetModeInfo);
                gainLifeGA.CardActionner = CardActionner;
                if (AudioManager.Instance.IsValid(SFX)) { gainLifeGA.SFX = SFX; }
                return gainLifeGA;
            }
            else
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    GainLifeGA gainLifeGA = new(GainAmount,multiplyAmount, DynamicAmount, passive, null);
                    gainLifeGA.CardActionner = CardActionner;
                    if (AudioManager.Instance.IsValid(SFX)) { gainLifeGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(gainLifeGA, targetNumber,TargetUpTo, this,targetLimitations);
                    return startManualTargetingGA;
                }
                else if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
                {
                    GainLifeGA gainLifeGA = new(GainAmount,multiplyAmount, DynamicAmount, passive, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                    gainLifeGA.CardActionner = CardActionner;
                    if (AudioManager.Instance.IsValid(SFX)) { gainLifeGA.SFX = SFX; }
                    return gainLifeGA;
                }
                else
                {
                    var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, null);

                    TargetForLinked_Player = playerTargets;
                    TargetForLinked_Enemy = enemyTargets;

                    GainLifeGA gainLifeGA = new(GainAmount,multiplyAmount, DynamicAmount, passive, playerTargets, enemyTargets);
                    gainLifeGA.CardActionner = CardActionner;
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
                    EnemyGainLifeGA enemyGainLifeGA = new(GainAmount,multiplyAmount, DynamicAmount, passive, null, null, targetModeInfo);
                    enemyGainLifeGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { enemyGainLifeGA.SFX = SFX; }
                    return enemyGainLifeGA;
                }
                else
                {
                    if (targetModeInfo.targetMode == TargetMode.Manual)
                    {
                        EnemyGainLifeGA enemyGainLifeGA = new(GainAmount,multiplyAmount, DynamicAmount, passive, null, null);
                        enemyGainLifeGA.Actionner = Actionner;
                        if (AudioManager.Instance.IsValid(SFX)) { enemyGainLifeGA.SFX = SFX; }
                        StartManualTargetingGA startManualTargetingGA = new(enemyGainLifeGA, targetNumber,TargetUpTo, this,targetLimitations);
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

                        EnemyGainLifeGA enemyGainLifeGA = new(GainAmount,multiplyAmount, DynamicAmount, passive, playerTargets, enemyTargets);
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
                    PlayerGainLifeGA playerGainLifeGA = new(GainAmount,multiplyAmount, DynamicAmount, passive, null, null, targetModeInfo);
                    playerGainLifeGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { playerGainLifeGA.SFX = SFX; }
                    return playerGainLifeGA;
                }
                else
                {
                    if (targetModeInfo.targetMode == TargetMode.Manual)
                    {
                        PlayerGainLifeGA playerGainLifeGA = new(GainAmount,multiplyAmount, DynamicAmount, passive, null, null);
                        playerGainLifeGA.Actionner = Actionner;
                        if (AudioManager.Instance.IsValid(SFX)) { playerGainLifeGA.SFX = SFX; }
                        StartManualTargetingGA startManualTargetingGA = new(playerGainLifeGA, targetNumber,TargetUpTo, this,targetLimitations);
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

                        PlayerGainLifeGA playerGainLifeGA = new(GainAmount,multiplyAmount, DynamicAmount, passive, playerTargets, enemyTargets);
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
            passive,
            TriggerOnDurationEnd,
            clonedLinked,
            clonedPlayerTargets,
            clonedEnemyTargets,
            DynamicAmount,
            SFX
        );
    }
}
