using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class AlterPowerEffect : Effect
{
    [Header("Effect Param")]

    [SerializeField] public int alterAmount;
    [SerializeField] public int multiplyAmount = 1;
    [SerializeField] public DynamicAmount DynamicAmount;
    [SerializeField] public bool aditive = true;
    [SerializeField] public bool passive;
    [SerializeField] public TargetModeInfo targetModeInfo;
    [SerializeField] public override TargetModeInfo EffectTargetModeInfo => targetModeInfo;


    [Header("For Manual Target only")]
    [SerializeField] private bool TargetUpTo = true;
    public override bool EffectTargetUpTo => TargetUpTo;

    [SerializeField] private int targetNumber = 1;
    public override int EffectTargetNumber => targetNumber;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public AlterPowerEffect() { }

    public AlterPowerEffect(string effectID, bool activateToolTip, int priority, int AlterAmount, int MultiplyAmount, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, bool orChoice , List<DynamicConditionInfo> dynamicConditionInfos ,TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, List<Events> Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool Passive, bool Aditive, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount, EventReference sfx,CounterType typeOfCounter, int counterValue, bool moduloValue)
    {
        Priority = priority;
        ActivateToolTip = activateToolTip;
        EffectID = effectID;
        alterAmount = AlterAmount;
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
        CardActionner = cardActionner;
        Events = Event;
        CancelOnDeath = cancelOnDeath;
        Actionner = actionner;
        Intent_Title = intent_Title;
        number = Number;
        Duration = duration;
        DurationType = durationType;
        passive = Passive;
        aditive = Aditive;
        TriggerOnDurationEnd = triggerOnDurationEnd;
        LinkedEffect = linkedEffect;
        TargetForLinked_Player = targetForLinked_Player;
        TargetForLinked_Enemy = targetForLinked_Enemy;
        DynamicAmount = dynamicAmount;
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
            DynamicAmount = DynamicAmount.NULL;
            alterAmount = PayXValue;
        }

        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (passive)
            {
                AlterPowerGA alterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, aditive, null, null, targetModeInfo);
                alterPowerGA.CardActionner = CardActionner;
                alterPowerGA.SourceEffect = this;
                alterPowerGA.ActivateToolTip = ActivateToolTip;
                alterPowerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterPowerSound : SFX;
                return alterPowerGA;
            }
            else
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    AlterPowerGA alterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, aditive, null);
                    alterPowerGA.CardActionner = CardActionner;
                    alterPowerGA.SourceEffect = this;
                    alterPowerGA.ActivateToolTip = false;
                    alterPowerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterPowerSound : SFX;
                    StartManualTargetingGA startManualTargetingGA = new(alterPowerGA, targetNumber, TargetUpTo, this, targetLimitations);
                    startManualTargetingGA.SourceEffect = this;
                    startManualTargetingGA.ActivateToolTip = ActivateToolTip;
                    return startManualTargetingGA;
                }
                else if (targetModeInfo.targetMode  == TargetMode.EffectParent_Targets)
                {
                    AlterPowerGA alterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, aditive, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                    alterPowerGA.CardActionner = CardActionner;
                    alterPowerGA.SourceEffect = this;
                    alterPowerGA.ActivateToolTip = ActivateToolTip;
                    alterPowerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterPowerSound : SFX;
                    return alterPowerGA;
                }
                else
                {
                    var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, null,this);

                    TargetForLinked_Player = playerTargets;
                    TargetForLinked_Enemy = enemyTargets;

                    AlterPowerGA alterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, aditive, playerTargets, enemyTargets);
                    alterPowerGA.CardActionner = CardActionner;
                    alterPowerGA.SourceEffect = this;
                    alterPowerGA.ActivateToolTip = ActivateToolTip;
                    alterPowerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterPowerSound : SFX;
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
                    EnemyAlterPowerGA enemyAlterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, aditive, null, null, targetModeInfo);
                    enemyAlterPowerGA.Actionner = Actionner;
                    enemyAlterPowerGA.SourceEffect = this;
                    enemyAlterPowerGA.ActivateToolTip = ActivateToolTip;
                    enemyAlterPowerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterPowerSound : SFX;
                    return enemyAlterPowerGA;
                }
                else
                {
                    if (targetModeInfo.targetMode == TargetMode.Manual)
                    {
                        EnemyAlterPowerGA enemyAlterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, aditive, null, null, targetModeInfo);
                        enemyAlterPowerGA.Actionner = Actionner;
                        enemyAlterPowerGA.SourceEffect = this;
                        enemyAlterPowerGA.ActivateToolTip = false;
                        enemyAlterPowerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterPowerSound : SFX;
                        StartManualTargetingGA startManualTargetingGA = new(enemyAlterPowerGA, targetNumber, TargetUpTo, this, targetLimitations);
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
                            (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, Actionner,this);

                            TargetForLinked_Player = playerTargets;
                            TargetForLinked_Enemy = enemyTargets;
                        }

                        EnemyAlterPowerGA enemyAlterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, aditive, playerTargets, enemyTargets);
                        enemyAlterPowerGA.Actionner = Actionner;
                        enemyAlterPowerGA.SourceEffect = this;
                        enemyAlterPowerGA.ActivateToolTip = ActivateToolTip;
                        enemyAlterPowerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterPowerSound : SFX;
                        return enemyAlterPowerGA;
                    }
                }

            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (passive)
                {
                    PlayerAlterPowerGA playerAlterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, aditive, null, null, targetModeInfo);
                    playerAlterPowerGA.Actionner = Actionner;
                    playerAlterPowerGA.SourceEffect = this;
                    playerAlterPowerGA.ActivateToolTip = ActivateToolTip;
                    playerAlterPowerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterPowerSound : SFX;
                    return playerAlterPowerGA;
                }
                else
                {
                    if (targetModeInfo.targetMode == TargetMode.Manual)
                    {
                        PlayerAlterPowerGA playerAlterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, aditive, null, null, targetModeInfo);
                        playerAlterPowerGA.Actionner = Actionner;
                        playerAlterPowerGA.SourceEffect = this;
                        playerAlterPowerGA.ActivateToolTip = false;
                        playerAlterPowerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterPowerSound : SFX;
                        StartManualTargetingGA startManualTargetingGA = new(playerAlterPowerGA, targetNumber, TargetUpTo, this, targetLimitations);
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
                            (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, Actionner,this);

                            TargetForLinked_Player = playerTargets;
                            TargetForLinked_Enemy = enemyTargets;
                        }

                        PlayerAlterPowerGA playerAlterPowerGA = new(alterAmount,multiplyAmount, DynamicAmount, passive, aditive, playerTargets, enemyTargets);
                        playerAlterPowerGA.Actionner = Actionner;
                        playerAlterPowerGA.SourceEffect = this;
                        playerAlterPowerGA.ActivateToolTip = ActivateToolTip;
                        playerAlterPowerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterPowerSound : SFX;
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
            EffectID,
            ActivateToolTip,
            Priority,
            alterAmount,
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
            Events,
            CancelOnDeath,
            Actionner,
            CardActionner,
            Intent_Title,
            number,
            Duration,
            DurationType,
            passive,
            aditive,
            TriggerOnDurationEnd,
            clonedLinked,
            clonedPlayerTargets,
            clonedEnemyTargets,
            DynamicAmount,
            SFX,
            TypeOfCounter,
            CounterValue,
            ModuloValue
        );
    }
}
