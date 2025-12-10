using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FMODUnity;
using SerializeReferenceEditor;
using System.Linq;

public class ShieldEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public TargetModeInfo targetModeInfo;
    [SerializeField] public override TargetModeInfo EffectTargetModeInfo => targetModeInfo;

    [Header("For Manual Target only")]
    [SerializeField] private bool TargetUpTo = true;
    public override bool EffectTargetUpTo => TargetUpTo;

    [SerializeField] private int targetNumber = 1;
    public override int EffectTargetNumber => targetNumber;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public ShieldEffect() { }

    public ShieldEffect(string effectID, bool activateToolTip, int priority, bool hollowEffect, int activateNumber, int activateLeft, bool orChoice,TargetModeInfo TargetModeInfo, List<DynamicConditionInfo> dynamicConditionInfos, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, List<Events> Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, EventReference sfx,CounterType typeOfCounter, int counterValue, bool moduloValue)
    {
        Priority = priority;
        HollowEffect = hollowEffect;
        ActivateToolTip = activateToolTip;
        EffectID = effectID;
        ActivateNumber = activateNumber;
        ActivateLeft = activateLeft;
        ORChoice = orChoice;
        targetModeInfo = TargetModeInfo;
        targetNumber = TargetNumber;
        TargetUpTo = targetUpTo;
        targetLimitations = TargetLimitations;
        actionnerType = ActionnerType;
        Events = Event;
        DynamicConditionInfos = dynamicConditionInfos;
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
        // SI CARTE
        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetModeInfo.targetMode == TargetMode.Manual)
            {
                ShieldGA shieldGA = new(null, null);
                shieldGA.CardActionner = CardActionner;
                shieldGA.SourceEffect = this;
                shieldGA.ActivateToolTip = false;
                shieldGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_ShieldSound : SFX;
                StartManualTargetingGA startManualTargetingGA = new(shieldGA, targetNumber, TargetUpTo, this, targetLimitations);
                startManualTargetingGA.SourceEffect = this;
                startManualTargetingGA.ActivateToolTip = ActivateToolTip;
                return startManualTargetingGA;
            }
            else if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
            {
                ShieldGA shieldGA = new(ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                shieldGA.SourceEffect = this;
                shieldGA.ActivateToolTip = ActivateToolTip;
                shieldGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_ShieldSound : SFX;
                return shieldGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, null,this);
                TargetForLinked_Player = playerTargets;
                TargetForLinked_Enemy = enemyTargets;

                ShieldGA shieldGA = new(playerTargets, enemyTargets);
                shieldGA.SourceEffect = this;
                shieldGA.ActivateToolTip = ActivateToolTip;
                shieldGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_ShieldSound : SFX;
                return shieldGA;
            }
        }
        // SI PERMANENT
        else
        {
            // SI ENEMY
            if (actionnerType == ActionnerType.ENEMY && Actionner != null)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    ShieldEnemyGA shieldEnemyGA = new(null, null);
                    shieldEnemyGA.Actionner = Actionner;
                    shieldEnemyGA.SourceEffect = this;
                    shieldEnemyGA.ActivateToolTip = false;
                    shieldEnemyGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_ShieldSound : SFX;
                    StartManualTargetingGA startManualTargetingGA = new(shieldEnemyGA, targetNumber, TargetUpTo, this, targetLimitations);
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

                    ShieldEnemyGA shieldEnemyGA = new(playerTargets, enemyTargets);
                    shieldEnemyGA.Actionner = Actionner;
                    shieldEnemyGA.SourceEffect = this;
                    shieldEnemyGA.ActivateToolTip = ActivateToolTip;
                    shieldEnemyGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_ShieldSound : SFX;
                    return shieldEnemyGA;
                }
            }
            // SI PLAYER
            else if (actionnerType == ActionnerType.PLAYER && Actionner != null)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    ShieldPlayerGA shieldPlayerGA = new(null, null);
                    shieldPlayerGA.Actionner = Actionner;
                    shieldPlayerGA.SourceEffect = this;
                    shieldPlayerGA.ActivateToolTip = false;
                    shieldPlayerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_ShieldSound : SFX;
                    StartManualTargetingGA startManualTargetingGA = new(shieldPlayerGA, targetNumber, TargetUpTo, this, targetLimitations);
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

                    ShieldPlayerGA shieldPlayerGA = new(playerTargets, enemyTargets);
                    shieldPlayerGA.Actionner = Actionner;
                    shieldPlayerGA.SourceEffect = this;
                    shieldPlayerGA.ActivateToolTip = ActivateToolTip;
                    shieldPlayerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_ShieldSound : SFX;
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
            EffectID,
            ActivateToolTip,
            Priority,
            HollowEffect,
            ActivateNumber,
            ActivateLeft,
            ORChoice,
            targetModeInfo,
            DynamicConditionInfos,
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
