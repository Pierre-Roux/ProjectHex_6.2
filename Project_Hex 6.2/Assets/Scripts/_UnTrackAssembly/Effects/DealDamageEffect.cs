using System;
using System.Collections.Generic;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class DealDamageEffect : Effect
{
    [Header("Effect Param")]

    [SerializeField] public bool powerBased;
    [SerializeField] public int damageAmount;
    [SerializeField] public int multiplyAmount = 1;
    [SerializeField] public DynamicAmountInfo DynamicAmountInfo;
    [SerializeField] public TargetModeInfo targetModeInfo;
    [SerializeField] public override TargetModeInfo EffectTargetModeInfo => targetModeInfo;

    [Header("For Manual Target only")]

    [SerializeField] private bool TargetUpTo = true;
    public override bool EffectTargetUpTo => TargetUpTo;

    [SerializeField] private int targetNumber = 1;
    public override int EffectTargetNumber => targetNumber;

    [HideInInspector] private string Description = "@ConditionsDeal @Amount@Multiply damage@TargetDuration@TargetNumber@TargetActivate";
    public override string EffectDescription => Description;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public DealDamageEffect() { }

    public DealDamageEffect(string effectID, bool activateToolTip, int priority, bool hollowEffect, string description, bool PowerBased, int DamageAmount, int MultiplyAmount, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, bool orChoice,List<DynamicConditionInfo> dynamicConditionInfos, TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, List<EventInfo> Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, EventInfo durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmountInfo dynamicAmountInfo, EventReference sfx,CounterTypeInfo typeOfCounter, int counterValue, bool moduloValue)
    {
        Priority = priority;
        ActivateToolTip = activateToolTip;
        EffectID = effectID;
        Description = description;
        HollowEffect = hollowEffect;
        damageAmount = DamageAmount;
        powerBased = PowerBased;
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
            damageAmount = PayXValue;
        }

        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetModeInfo.targetMode == TargetMode.Manual)
            {
                DealDamageGA dealDamageGA = new(powerBased, damageAmount, multiplyAmount, DynamicAmountInfo, null, null);
                dealDamageGA.CardActionner = CardActionner;
                dealDamageGA.SourceEffect = this;
                dealDamageGA.ActivateToolTip = false;
                dealDamageGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_DealDamageSound : SFX;
                StartManualTargetingGA startManualTargetingGA = new(dealDamageGA, targetNumber, TargetUpTo, this, targetLimitations);
                startManualTargetingGA.SourceEffect = this;
                startManualTargetingGA.ActivateToolTip = ActivateToolTip;
                return startManualTargetingGA;
            }
            else if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
            {
                DealDamageGA dealDamageGA = new(powerBased, damageAmount, multiplyAmount, DynamicAmountInfo, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
                dealDamageGA.CardActionner = CardActionner;
                dealDamageGA.SourceEffect = this;
                dealDamageGA.ActivateToolTip = ActivateToolTip;
                dealDamageGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_DealDamageSound : SFX;
                return dealDamageGA;
            }
            else
            {
                var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, null, this);
                TargetForLinked_Player = playerTargets;
                TargetForLinked_Enemy = enemyTargets;

                DealDamageGA dealDamageGA = new(powerBased, damageAmount, multiplyAmount, DynamicAmountInfo, playerTargets, enemyTargets);
                dealDamageGA.CardActionner = CardActionner;
                dealDamageGA.SourceEffect = this;
                dealDamageGA.ActivateToolTip = ActivateToolTip;
                dealDamageGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_DealDamageSound : SFX;
                return dealDamageGA;
            }
        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    AttackPlayerGA attackPlayerGA = new(powerBased, damageAmount, multiplyAmount, DynamicAmountInfo, null, null);
                    attackPlayerGA.Actionner = Actionner;
                    attackPlayerGA.SourceEffect = this;
                    attackPlayerGA.ActivateToolTip = false;
                    attackPlayerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_DealDamageSound : SFX;
                    StartManualTargetingGA startManualTargetingGA = new(attackPlayerGA, targetNumber, TargetUpTo, this, targetLimitations);
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

                    AttackPlayerGA attackPlayerGA = new(powerBased, damageAmount, multiplyAmount, DynamicAmountInfo, playerTargets, enemyTargets);
                    attackPlayerGA.Actionner = Actionner;
                    attackPlayerGA.SourceEffect = this;
                    attackPlayerGA.ActivateToolTip = ActivateToolTip;
                    attackPlayerGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_DealDamageSound : SFX;
                    return attackPlayerGA;
                }
            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    AttackEnemyGA attackEnemyGA = new(powerBased, damageAmount, multiplyAmount, DynamicAmountInfo, null, null);
                    attackEnemyGA.Actionner = Actionner;
                    attackEnemyGA.SourceEffect = this;
                    attackEnemyGA.ActivateToolTip = false;
                    attackEnemyGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_DealDamageSound : SFX;
                    StartManualTargetingGA startManualTargetingGA = new(attackEnemyGA, targetNumber, TargetUpTo, this, targetLimitations);
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

                    AttackEnemyGA attackEnemyGA = new(powerBased, damageAmount, multiplyAmount, DynamicAmountInfo, playerTargets, enemyTargets);
                    attackEnemyGA.Actionner = Actionner;
                    attackEnemyGA.SourceEffect = this;
                    attackEnemyGA.ActivateToolTip = ActivateToolTip;
                    attackEnemyGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_DealDamageSound : SFX;
                    return attackEnemyGA;
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
        return null;
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

        return new DealDamageEffect(
            EffectID,
            ActivateToolTip,
            Priority,
            HollowEffect,
            Description,
            powerBased,
            damageAmount,
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
