using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class RefreshEffect : Effect
{
    [Header("Effect Param")]

    [SerializeField] public TargetModeInfo targetModeInfo;
    [SerializeField] public override TargetModeInfo EffectTargetModeInfo => targetModeInfo;

    [Header("For Manual Target only")]

    [SerializeField] private bool TargetUpTo = true;
    public override bool EffectTargetUpTo => TargetUpTo;

    [SerializeField] private int targetNumber;
    public override int EffectTargetNumber => targetNumber;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public RefreshEffect() { }

    public RefreshEffect(int multiHit, int activateNumber, int activateLeft, List<DynamicConditionInfo> dynamicConditionInfos, TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, Events Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy,EventReference sfx)
    {
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
                RefreshGA refreshGA = new(null, null);
                refreshGA.CardActionner = CardActionner;
                if (AudioManager.Instance.IsValid(SFX)) { refreshGA.SFX = SFX; }
                Debug.Log("Refresh start");
                StartManualTargetingGA startManualTargetingGA = new(refreshGA, targetNumber, TargetUpTo, this, targetLimitations);
                return startManualTargetingGA;
            }
            else if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
            {
                RefreshGA refreshGA = new(ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                refreshGA.CardActionner = CardActionner;
                if (AudioManager.Instance.IsValid(SFX)) { refreshGA.SFX = SFX; }
                return refreshGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, null);
                TargetForLinked_Player = playerTargets;
                TargetForLinked_Enemy = enemyTargets;

                RefreshGA refreshGA = new(playerTargets, enemyTargets);
                refreshGA.CardActionner = CardActionner;
                if (AudioManager.Instance.IsValid(SFX)) { refreshGA.SFX = SFX; }
                return refreshGA;
            }
        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    EnemyRefreshGA enemyRefreshGA = new(null, null);
                    enemyRefreshGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { enemyRefreshGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(enemyRefreshGA, targetNumber,TargetUpTo, this,targetLimitations);
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

                    EnemyRefreshGA enemyRefreshGA = new(playerTargets, enemyTargets);
                    enemyRefreshGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { enemyRefreshGA.SFX = SFX; }
                    return enemyRefreshGA;
                }
            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    PlayerRefreshGA playerRefreshGA = new(null, null);
                    playerRefreshGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { playerRefreshGA.SFX = SFX; }
                    StartManualTargetingGA startManualTargetingGA = new(playerRefreshGA, targetNumber,TargetUpTo, this,targetLimitations);
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

                    PlayerRefreshGA playerRefreshGA = new(playerTargets, enemyTargets);
                    playerRefreshGA.Actionner = Actionner;
                    if (AudioManager.Instance.IsValid(SFX)) { playerRefreshGA.SFX = SFX; }
                    return playerRefreshGA;
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

        return new RefreshEffect(
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
            SFX
        );
    }
}
