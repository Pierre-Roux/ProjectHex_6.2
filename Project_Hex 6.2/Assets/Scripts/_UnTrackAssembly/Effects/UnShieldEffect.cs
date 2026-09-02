using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class UnShieldEffect : Effect
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

    public UnShieldEffect() { }

    public UnShieldEffect(string effectID, bool activateToolTip, int priority, bool hollowEffect, int activateNumber, int activateLeft, bool orChoice,List<DynamicConditionInfo> dynamicConditionInfos, TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, List<EventInfo> Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, EventInfo durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, EventReference sfx,CounterTypeInfo typeOfCounter, int counterValue, bool moduloValue)
    {
        Priority = priority;
        HollowEffect = hollowEffect;
        ActivateToolTip = activateToolTip;
        EffectID = effectID;
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
        EventInfos = Event;
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

        Disabled = false;

        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetModeInfo.targetMode == TargetMode.Manual)
            {
                UnShieldGA unShieldGA = new(null, null);
                unShieldGA.CardActionner = CardActionner;
                unShieldGA.SourceEffect = this;
                unShieldGA.ActivateToolTip = false;
                unShieldGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_UnShieldSound : SFX;
                StartManualTargetingGA startManualTargetingGA = new(unShieldGA, targetNumber, TargetUpTo, this, targetLimitations);
                startManualTargetingGA.SourceEffect = this;
                startManualTargetingGA.ActivateToolTip = ActivateToolTip;
                return startManualTargetingGA;
            }
            else if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
            {
                UnShieldGA unShieldGA = new(ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                unShieldGA.CardActionner = CardActionner;
                unShieldGA.SourceEffect = this;
                unShieldGA.ActivateToolTip = ActivateToolTip;
                unShieldGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_UnShieldSound : SFX;
                return unShieldGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, null, this);
                TargetForLinked_Player = playerTargets;
                TargetForLinked_Enemy = enemyTargets;

                UnShieldGA unShieldGA = new(playerTargets, enemyTargets);
                unShieldGA.CardActionner = CardActionner;
                unShieldGA.SourceEffect = this;
                unShieldGA.ActivateToolTip = ActivateToolTip;
                unShieldGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_UnShieldSound : SFX;
                return unShieldGA;
            }
        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    EnemyUnShieldGA enemyUnShieldGA = new(null, null);
                    enemyUnShieldGA.Actionner = Actionner;
                    enemyUnShieldGA.SourceEffect = this;
                    enemyUnShieldGA.ActivateToolTip = false;
                    enemyUnShieldGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_UnShieldSound : SFX;
                    StartManualTargetingGA startManualTargetingGA = new(enemyUnShieldGA, targetNumber, TargetUpTo, this, targetLimitations);
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
                        (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, Actionner, this);

                        TargetForLinked_Player = playerTargets;
                        TargetForLinked_Enemy = enemyTargets;
                    }

                    EnemyUnShieldGA enemyUnShieldGA = new(playerTargets, enemyTargets);
                    enemyUnShieldGA.Actionner = Actionner;
                    enemyUnShieldGA.SourceEffect = this;
                    enemyUnShieldGA.ActivateToolTip = ActivateToolTip;
                    enemyUnShieldGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_UnShieldSound : SFX;
                    return enemyUnShieldGA;
                }
            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    PlayerUnShieldGA playerUnShieldGA = new(null, null);
                    playerUnShieldGA.Actionner = Actionner;
                    playerUnShieldGA.SourceEffect = this;
                    playerUnShieldGA.ActivateToolTip = false;
                    playerUnShieldGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_UnShieldSound : SFX;
                    StartManualTargetingGA startManualTargetingGA = new(playerUnShieldGA, targetNumber, TargetUpTo, this, targetLimitations);
                    startManualTargetingGA.SourceEffect = this;
                    startManualTargetingGA.ActivateToolTip = ActivateToolTip;
                    return startManualTargetingGA;
                }
                else
                {
                    List<PermanentView> playerTargets;
                    List<EnemySlotView> enemyTargets;

                    Debug.Log("zergz   " + ParentEffect.TargetForLinked_Player);

                    if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
                    {
                        playerTargets = ParentEffect.TargetForLinked_Player;
                        enemyTargets = ParentEffect.TargetForLinked_Enemy;
                    }
                    else
                    {
                        (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, Actionner, this);

                        TargetForLinked_Player = playerTargets;
                        TargetForLinked_Enemy = enemyTargets;
                    }

                    PlayerUnShieldGA playerUnShieldGA = new(playerTargets, enemyTargets);
                    playerUnShieldGA.Actionner = Actionner;
                    playerUnShieldGA.SourceEffect = this;
                    playerUnShieldGA.ActivateToolTip = ActivateToolTip;
                    playerUnShieldGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_UnShieldSound : SFX;
                    return playerUnShieldGA;
                }
            }
            else
            {
                Debug.LogError("Effect.GetGameAction returned Null");
                return null;
            }
        }
    }
    
    public override GameAction GetCounterMesure()
    {
        Disabled = true;
        ShieldGA shieldGA = new(TargetForLinked_Player, TargetForLinked_Enemy);
        shieldGA.Actionner = Actionner;
        shieldGA.SourceEffect = this;
        shieldGA.ActivateToolTip = false;
        shieldGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_ShieldSound : SFX;
        return shieldGA;
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

        return new UnShieldEffect(
            EffectID,
            ActivateToolTip,
            Priority,
            HollowEffect,
            ActivateNumber,
            ActivateLeft,
            ORChoice,
            DynamicConditionInfos,
            targetModeInfo,
            targetLimitations,
            targetNumber,
            TargetUpTo,
            actionnerType,
            EventInfos,
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
