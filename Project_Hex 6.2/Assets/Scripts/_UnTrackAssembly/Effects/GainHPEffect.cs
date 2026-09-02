using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class GainHPEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public bool CanBeDisable = true;
    [SerializeField] public override bool CanBeDisableEffect => CanBeDisable;
    [SerializeField] public int GainAmount;
    [SerializeField] public int multiplyAmount = 1;
    [SerializeField] public DynamicAmountInfo DynamicAmountInfo;
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

    public GainHPEffect() { }

    public GainHPEffect(string effectID, bool activateToolTip, int priority, bool hollowEffect, int gainAmount, int MultiplyAmount, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, bool orChoice, List<DynamicConditionInfo> dynamicConditionInfos, TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, List<EventInfo> Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, EventInfo durationType, bool Passive, bool Aditive, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmountInfo dynamicAmountInfo, EventReference sfx,CounterTypeInfo typeOfCounter, int counterValue, bool moduloValue)
    {
        Priority = priority;
        HollowEffect = hollowEffect;
        ActivateToolTip = activateToolTip;
        EffectID = effectID;
        GainAmount = gainAmount;
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
        EventInfos = Event;
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
        DynamicAmountInfo = dynamicAmountInfo;
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

        if (PayXValue != 0)
        {
            DynamicAmountInfo.DynamicAmount = DynamicAmount.NULL;
            GainAmount = PayXValue;
        }

        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (passive)
            {
                GainLifeGA gainLifeGA = new(GainAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, null, null, targetModeInfo);
                gainLifeGA.CardActionner = CardActionner;
                gainLifeGA.SourceEffect = this;
                gainLifeGA.ActivateToolTip = ActivateToolTip;
                gainLifeGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterIntegritySound : SFX;
                return gainLifeGA;
            }
            else
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    GainLifeGA gainLifeGA = new(GainAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, null, null, null);
                    gainLifeGA.CardActionner = CardActionner;
                    gainLifeGA.SourceEffect = this;
                    gainLifeGA.ActivateToolTip = false;
                    gainLifeGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterIntegritySound : SFX;
                    StartManualTargetingGA startManualTargetingGA = new(gainLifeGA, targetNumber, TargetUpTo, this, targetLimitations);
                    startManualTargetingGA.SourceEffect = this;
                    startManualTargetingGA.ActivateToolTip = ActivateToolTip;
                    return startManualTargetingGA;
                }
                else if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
                {
                    GainLifeGA gainLifeGA = new(GainAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy, null);
                    gainLifeGA.CardActionner = CardActionner;
                    gainLifeGA.SourceEffect = this;
                    gainLifeGA.ActivateToolTip = ActivateToolTip;
                    gainLifeGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterIntegritySound : SFX;
                    return gainLifeGA;
                }
                else
                {
                    var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, null, this);

                    TargetForLinked_Player = playerTargets;
                    TargetForLinked_Enemy = enemyTargets;

                    GainLifeGA gainLifeGA = new(GainAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, playerTargets, enemyTargets, null);
                    gainLifeGA.CardActionner = CardActionner;
                    gainLifeGA.SourceEffect = this;
                    gainLifeGA.ActivateToolTip = ActivateToolTip;
                    gainLifeGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterIntegritySound : SFX;
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
                    EnemyGainLifeGA enemyGainLifeGA = new(GainAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, null, null, targetModeInfo);
                    enemyGainLifeGA.Actionner = Actionner;
                    enemyGainLifeGA.SourceEffect = this;
                    enemyGainLifeGA.ActivateToolTip = ActivateToolTip;
                    enemyGainLifeGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterIntegritySound : SFX;
                    return enemyGainLifeGA;
                }
                else
                {
                    if (targetModeInfo.targetMode == TargetMode.Manual)
                    {
                        EnemyGainLifeGA enemyGainLifeGA = new(GainAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, null, null, null);
                        enemyGainLifeGA.Actionner = Actionner;
                        enemyGainLifeGA.SourceEffect = this;
                        enemyGainLifeGA.ActivateToolTip = false;
                        enemyGainLifeGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterIntegritySound : SFX;
                        StartManualTargetingGA startManualTargetingGA = new(enemyGainLifeGA, targetNumber, TargetUpTo, this, targetLimitations);
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

                        EnemyGainLifeGA enemyGainLifeGA = new(GainAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, playerTargets, enemyTargets, null);
                        enemyGainLifeGA.Actionner = Actionner;
                        enemyGainLifeGA.SourceEffect = this;
                        enemyGainLifeGA.ActivateToolTip = ActivateToolTip;
                        enemyGainLifeGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterIntegritySound : SFX;
                        return enemyGainLifeGA;
                    }
                }

            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (passive)
                {
                    PlayerGainLifeGA playerGainLifeGA = new(GainAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, null, null, targetModeInfo);
                    playerGainLifeGA.Actionner = Actionner;
                    playerGainLifeGA.SourceEffect = this;
                    playerGainLifeGA.ActivateToolTip = ActivateToolTip;
                    playerGainLifeGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterIntegritySound : SFX;
                    return playerGainLifeGA;
                }
                else
                {
                    if (targetModeInfo.targetMode == TargetMode.Manual)
                    {
                        PlayerGainLifeGA playerGainLifeGA = new(GainAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, null, null, null);
                        playerGainLifeGA.Actionner = Actionner;
                        playerGainLifeGA.SourceEffect = this;
                        playerGainLifeGA.ActivateToolTip = false;
                        playerGainLifeGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterIntegritySound : SFX;
                        StartManualTargetingGA startManualTargetingGA = new(playerGainLifeGA, targetNumber, TargetUpTo, this, targetLimitations);
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

                        PlayerGainLifeGA playerGainLifeGA = new(GainAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, playerTargets, enemyTargets, null);
                        playerGainLifeGA.Actionner = Actionner;
                        playerGainLifeGA.SourceEffect = this;
                        playerGainLifeGA.ActivateToolTip = ActivateToolTip;
                        playerGainLifeGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterIntegritySound : SFX;
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

    public override GameAction GetCounterMesure()
    {
        Disabled = true;
        GainLifeGA gainLifeGA = new(-GainAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, TargetForLinked_Player, TargetForLinked_Enemy,null);
        gainLifeGA.CardActionner = CardActionner;
        gainLifeGA.SourceEffect = this;
        gainLifeGA.ActivateToolTip = false;
        gainLifeGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterIntegritySound : SFX;
        return gainLifeGA;
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
            EffectID,
            ActivateToolTip,
            Priority,
            HollowEffect,
            GainAmount,
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
            EventInfos,
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
            DynamicAmountInfo,
            SFX,
            TypeOfCounter,
            CounterValue,
            ModuloValue
        );
    }
}
