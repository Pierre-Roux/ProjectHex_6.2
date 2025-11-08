using System;
using System.Collections.Generic;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class AlterPowerEffect : Effect
{
    [Header("Effect Param")]

    [SerializeField] public int alterAmount;
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

    public AlterPowerEffect() { }

    public AlterPowerEffect(int AlterAmount, int MultiplyAmount, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, List<DynamicConditionInfo> dynamicConditionInfos ,TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, Events Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool Passive, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount, EventReference sfx)
    {
        alterAmount = AlterAmount;
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
            alterAmount = PayXValue;
        }

        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (passive)
            {
                AlterPowerGA alterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, null, null, targetModeInfo);
                alterPowerGA.CardActionner = CardActionner;
                if (AudioManager.Instance.IsValid(SFX)) { alterPowerGA.SFX = SFX; }
                return alterPowerGA;
            }
            else
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    AlterPowerGA alterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, null);
                    alterPowerGA.CardActionner = CardActionner;
                    if (AudioManager.Instance.IsValid(SFX)) { alterPowerGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(alterPowerGA, targetNumber,TargetUpTo, this,targetLimitations);
                    return startManualTargetingGA;
                }
                else if (targetModeInfo.targetMode  == TargetMode.EffectParent_Targets)
                {
                    AlterPowerGA alterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                    alterPowerGA.CardActionner = CardActionner;
                    if (AudioManager.Instance.IsValid(SFX)) { alterPowerGA.SFX = SFX; }
                    return alterPowerGA;
                }
                else
                {
                    var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, null);

                    TargetForLinked_Player = playerTargets;
                    TargetForLinked_Enemy = enemyTargets;

                    AlterPowerGA alterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, playerTargets, enemyTargets);
                    alterPowerGA.CardActionner = CardActionner;
                    if (AudioManager.Instance.IsValid(SFX)) { alterPowerGA.SFX = SFX; }
                    return alterPowerGA;
                }
            }

        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY)
            {
                if (passive)
                {
                    EnemyAlterPowerGA enemyAlterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, null, null, targetModeInfo);
                    enemyAlterPowerGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { enemyAlterPowerGA.SFX = SFX; }
                    return enemyAlterPowerGA;
                }
                else
                {
                    if (targetModeInfo.targetMode == TargetMode.Manual)
                    {
                        EnemyAlterPowerGA enemyAlterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, null, null, targetModeInfo);
                        enemyAlterPowerGA.Actionner = Actionner;
                        if (AudioManager.Instance.IsValid(SFX)) { enemyAlterPowerGA.SFX = SFX; }
                        StartManualTargetingGA startManualTargetingGA = new(enemyAlterPowerGA, targetNumber,TargetUpTo, this,targetLimitations);
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

                        EnemyAlterPowerGA enemyAlterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, playerTargets, enemyTargets);
                        enemyAlterPowerGA.Actionner = Actionner;
                        if (AudioManager.Instance.IsValid(SFX)) { enemyAlterPowerGA.SFX = SFX; }
                        return enemyAlterPowerGA;
                    }
                }

            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (passive)
                {
                    PlayerAlterPowerGA playerAlterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, null, null, targetModeInfo);
                    playerAlterPowerGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { playerAlterPowerGA.SFX = SFX; }
                    return playerAlterPowerGA;
                }
                else
                {
                    if (targetModeInfo.targetMode == TargetMode.Manual)
                    {
                        PlayerAlterPowerGA playerAlterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, null, null, targetModeInfo);
                        playerAlterPowerGA.Actionner = Actionner;
                        if (AudioManager.Instance.IsValid(SFX)) { playerAlterPowerGA.SFX = SFX; }
                        StartManualTargetingGA startManualTargetingGA = new(playerAlterPowerGA, targetNumber,TargetUpTo, this,targetLimitations);
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

                        PlayerAlterPowerGA playerAlterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, playerTargets, enemyTargets);
                        playerAlterPowerGA.Actionner = Actionner;
                        if (AudioManager.Instance.IsValid(SFX)) { playerAlterPowerGA.SFX = SFX; }
                        return playerAlterPowerGA;
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

        return new AlterPowerEffect(
            alterAmount,
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
