using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class AlterStaminaEffect : Effect
{
    [Header("Effect Param")]

    [SerializeField] public int alterAmount;
    [SerializeField] public int multiplyAmount = 1;
    [SerializeField] public DynamicAmount DynamicAmount;
    [SerializeField] public TargetModeInfo targetModeInfo;
    [SerializeField] public override TargetModeInfo EffectTargetModeInfo => targetModeInfo;
    [SerializeField] public PermaTypes permaTypes;

    [Header("For Manual Target only")]
    [SerializeField] private bool TargetUpTo = true;
    public override bool EffectTargetUpTo => TargetUpTo;

    [SerializeField] private int targetNumber;
    public override int EffectTargetNumber => targetNumber;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public AlterStaminaEffect() { }

    public AlterStaminaEffect(int AlterAmount, int MultiplyAmount, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, List<DynamicConditionInfo> dynamicConditionInfos, TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, Events Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, PermaTypes PermaTypes, DynamicAmount dynamicAmount, EventReference sfx)
    {
        alterAmount = AlterAmount;
        multiplyAmount = MultiplyAmount;
        PayXEffect = payXEffect;
        PayXValue = payXValue;
        MultiHit = multiHit;
        ActivateNumber = activateNumber;
        ActivateLeft = activateLeft;
        DynamicConditionInfos = dynamicConditionInfos;
        targetModeInfo = TargetModeInfo;
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
            if (targetModeInfo.targetMode == TargetMode.Manual)
            {
                AlterStaminaGA alterStaminaGA = new(alterAmount,multiplyAmount, DynamicAmount, permaTypes, null);
                alterStaminaGA.CardActionner = CardActionner;
                if (AudioManager.Instance.IsValid(SFX)) { alterStaminaGA.SFX = SFX; }
                StartManualTargetingGA startManualTargetingGA = new(alterStaminaGA, targetNumber,TargetUpTo, this,targetLimitations);
                return startManualTargetingGA;
            }
            else if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
            {
                AlterStaminaGA alterStaminaGA = new(alterAmount,multiplyAmount, DynamicAmount, permaTypes, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                alterStaminaGA.CardActionner = CardActionner;
                if (AudioManager.Instance.IsValid(SFX)) { alterStaminaGA.SFX = SFX; }
                return alterStaminaGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, null);

                TargetForLinked_Player = playerTargets;
                TargetForLinked_Enemy = enemyTargets;

                AlterStaminaGA alterStaminaGA = new(alterAmount,multiplyAmount, DynamicAmount, permaTypes, playerTargets, enemyTargets);
                alterStaminaGA.CardActionner = CardActionner;
                if (AudioManager.Instance.IsValid(SFX)) { alterStaminaGA.SFX = SFX; }
                return alterStaminaGA;
            }
        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    EnemyAlterStaminaGA enemyAlterStaminaGA = new(alterAmount,multiplyAmount, DynamicAmount, permaTypes, null, null);
                    enemyAlterStaminaGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { enemyAlterStaminaGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(enemyAlterStaminaGA, targetNumber,TargetUpTo, this,targetLimitations);
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

                    EnemyAlterStaminaGA enemyAlterStaminaGA = new(alterAmount,multiplyAmount, DynamicAmount, permaTypes, playerTargets, enemyTargets);
                    enemyAlterStaminaGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { enemyAlterStaminaGA.SFX = SFX; }
                    return enemyAlterStaminaGA;
                }
            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    PlayerAlterStaminaGA playerAlterStaminaGA = new(alterAmount,multiplyAmount, DynamicAmount, permaTypes, null, null);
                    playerAlterStaminaGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { playerAlterStaminaGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(playerAlterStaminaGA, targetNumber,TargetUpTo, this,targetLimitations);
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

                    PlayerAlterStaminaGA playerAlterStaminaGA = new(alterAmount,multiplyAmount, DynamicAmount, permaTypes, playerTargets, enemyTargets);
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

        return new AlterStaminaEffect(
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
