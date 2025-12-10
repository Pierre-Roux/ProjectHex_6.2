using System;
using System.Collections.Generic;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class DisableEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public TargetModeInfo targetModeInfo;
    [SerializeField] public override TargetModeInfo EffectTargetModeInfo => targetModeInfo;

    [Header("For Manual Target only")]

    [SerializeField] private bool TargetUpTo = true;
    public override bool EffectTargetUpTo => TargetUpTo;

    [SerializeField] private int targetNumber = 1;
    public override int EffectTargetNumber => targetNumber;

    [SerializeField] private string Description = "@ConditionsDeal @Amount@Multiply damage@TargetDuration@TargetNumber@TargetActivate";
    public override string EffectDescription => Description;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public DisableEffect() { }

    public DisableEffect(string effectID, bool activateToolTip, int priority, bool hollowEffect, string description, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, bool orChoice,List<DynamicConditionInfo> dynamicConditionInfos, TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, List<Events> Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, EventReference sfx,CounterType typeOfCounter, int counterValue, bool moduloValue)
    {
        Priority = priority;
        HollowEffect = hollowEffect;
        ActivateToolTip = activateToolTip;
        EffectID = effectID;
        Description = description;
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
        TriggerOnDurationEnd = triggerOnDurationEnd;
        LinkedEffect = linkedEffect;
        TargetForLinked_Player = targetForLinked_Player;
        TargetForLinked_Enemy = targetForLinked_Enemy;
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

        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetModeInfo.targetMode == TargetMode.Manual)
            {
                DisableGA disableGA = new(null, null);
                disableGA.CardActionner = CardActionner;
                disableGA.SourceEffect = this;
                disableGA.ActivateToolTip = false;
                disableGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_DisableSound : SFX;
                StartManualTargetingGA startManualTargetingGA = new(disableGA, targetNumber, TargetUpTo, this, targetLimitations);
                startManualTargetingGA.SourceEffect = this;
                startManualTargetingGA.ActivateToolTip = ActivateToolTip;
                return startManualTargetingGA;
            }
            else if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
            {
                DisableGA disableGA = new(ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                disableGA.CardActionner = CardActionner;
                disableGA.SourceEffect = this;
                disableGA.ActivateToolTip = ActivateToolTip;
                disableGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_DisableSound : SFX;
                return disableGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, null,this);
                TargetForLinked_Player = playerTargets;
                TargetForLinked_Enemy = enemyTargets;

                DisableGA disableGA = new(playerTargets, enemyTargets);
                disableGA.CardActionner = CardActionner;
                disableGA.SourceEffect = this;
                disableGA.ActivateToolTip = ActivateToolTip;
                disableGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_DisableSound : SFX;
                return disableGA;
            }
        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    EnemyDisableGA enemyDisableGA = new(null, null);
                    enemyDisableGA.Actionner = Actionner;
                    enemyDisableGA.SourceEffect = this;
                    enemyDisableGA.ActivateToolTip = false;
                    enemyDisableGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_DisableSound : SFX;
                    StartManualTargetingGA startManualTargetingGA = new(enemyDisableGA, targetNumber, TargetUpTo, this, targetLimitations);
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

                    EnemyDisableGA enemyDisableGA = new(playerTargets, enemyTargets);
                    enemyDisableGA.Actionner = Actionner;
                    enemyDisableGA.SourceEffect = this;
                    enemyDisableGA.ActivateToolTip = ActivateToolTip;
                    enemyDisableGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_DisableSound : SFX;
                    return enemyDisableGA;
                }
            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    PlayerDisableGA playerDisableGA = new(null, null);
                    playerDisableGA.Actionner = Actionner;
                    playerDisableGA.SourceEffect = this;
                    playerDisableGA.ActivateToolTip = false;
                    playerDisableGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_DisableSound : SFX;
                    StartManualTargetingGA startManualTargetingGA = new(playerDisableGA, targetNumber, TargetUpTo, this, targetLimitations);
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

                    PlayerDisableGA playerDisableGA = new(playerTargets, enemyTargets);
                    playerDisableGA.Actionner = Actionner;
                    playerDisableGA.SourceEffect = this;
                    playerDisableGA.ActivateToolTip = ActivateToolTip;
                    playerDisableGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_DisableSound : SFX;
                    return playerDisableGA;
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

        return new DisableEffect(
            EffectID,
            ActivateToolTip,
            Priority,
            HollowEffect,
            Description,
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
            TriggerOnDurationEnd,
            clonedLinked,
            clonedPlayerTargets,
            clonedEnemyTargets,
            SFX,
            TypeOfCounter,
            CounterValue,
            ModuloValue
        );
    }
}
