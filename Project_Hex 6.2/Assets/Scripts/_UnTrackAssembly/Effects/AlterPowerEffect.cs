using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class AlterPowerEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public bool CanBeDisable = true;
    [SerializeField] public override bool CanBeDisableEffect => CanBeDisable;
    [SerializeField] public int alterAmount;
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

    public AlterPowerEffect() { }

    public AlterPowerEffect(string effectID, bool activateToolTip, int priority, bool hollowEffect, int AlterAmount, int MultiplyAmount, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, bool orChoice , List<DynamicConditionInfo> dynamicConditionInfos ,TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, List<EventInfo> Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, EventInfo durationType, bool Passive, bool Aditive, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmountInfo dynamicAmountInfo, EventReference sfx,CounterTypeInfo typeOfCounter, int counterValue, bool moduloValue)
    {
        Priority = priority;
        HollowEffect = hollowEffect;
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

        if (PayXValue != 0)
        {
            DynamicAmountInfo.DynamicAmount = DynamicAmount.NULL;
            alterAmount = PayXValue;
        }

        Disabled = false;

        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (passive)
            {
                AlterPowerGA alterPowerGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, null, null, null, targetModeInfo);
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
                    AlterPowerGA alterPowerGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, null,null,null);
                    alterPowerGA.CardActionner = CardActionner;
                    alterPowerGA.SourceEffect = this;
                    alterPowerGA.ActivateToolTip = false;
                    alterPowerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterPowerSound : SFX;
                    StartManualTargetingGA startManualTargetingGA = new(alterPowerGA, targetNumber, TargetUpTo, this, targetLimitations);
                    startManualTargetingGA.SourceEffect = this;
                    startManualTargetingGA.ActivateToolTip = ActivateToolTip;
                    return startManualTargetingGA;
                }
                else if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
                {
                    AlterPowerGA alterPowerGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy, ParentEffect.TargetForLinked_Card);
                    alterPowerGA.CardActionner = CardActionner;
                    alterPowerGA.SourceEffect = this;
                    alterPowerGA.ActivateToolTip = ActivateToolTip;
                    alterPowerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterPowerSound : SFX;
                    return alterPowerGA;
                }
                else
                {
                    var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, null, this);

                    TargetForLinked_Player = playerTargets;
                    TargetForLinked_Enemy = enemyTargets;

                    AlterPowerGA alterPowerGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, playerTargets, enemyTargets,null);
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
                    EnemyAlterPowerGA enemyAlterPowerGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, null, null, null, targetModeInfo);
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
                        EnemyAlterPowerGA enemyAlterPowerGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, null, null, null, targetModeInfo);
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
                        List<Card> cardTargets;

                        if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
                        {
                            playerTargets = ParentEffect.TargetForLinked_Player;
                            enemyTargets = ParentEffect.TargetForLinked_Enemy;
                            cardTargets = ParentEffect.TargetForLinked_Card;
                        }
                        else
                        {
                            (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, Actionner, this);

                            TargetForLinked_Player = playerTargets;
                            TargetForLinked_Enemy = enemyTargets;

                            // Ligne a modifier pour que les cartes recoivent du AlterPower TODO
                            //TargetForLinked_Card = cardTargets = TargetSystem.GetCardsTargets(targetModeInfo, CardActionner);
                        }

                        EnemyAlterPowerGA enemyAlterPowerGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, playerTargets, enemyTargets, null);
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
                    PlayerAlterPowerGA playerAlterPowerGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, null, null, null, targetModeInfo);
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
                        PlayerAlterPowerGA playerAlterPowerGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, null, null, null, targetModeInfo);
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
                            (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, Actionner, this);

                            TargetForLinked_Player = playerTargets;
                            TargetForLinked_Enemy = enemyTargets;
                        }

                        PlayerAlterPowerGA playerAlterPowerGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, playerTargets, enemyTargets, null);
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
    
    public override GameAction GetCounterMesure()
    {
        Disabled = true;
        AlterPowerGA alterPowerGA = new(-alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, TargetForLinked_Player, TargetForLinked_Enemy,null);
        alterPowerGA.Actionner = Actionner;
        alterPowerGA.SourceEffect = this;
        alterPowerGA.ActivateToolTip = false;
        alterPowerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterPowerSound : SFX;
        return alterPowerGA;
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
            HollowEffect,
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
