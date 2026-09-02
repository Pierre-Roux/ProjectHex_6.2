using System;
using System.Collections.Generic;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class AlterStaminaEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public bool CanBeDisable = true;
    [SerializeField] public override bool CanBeDisableEffect => CanBeDisable;
    [SerializeField] public int alterAmount;
    [SerializeField] public int multiplyAmount = 1;
    [SerializeField] public DynamicAmountInfo DynamicAmountInfo;
    [SerializeField] public bool IncludeCardsInDeck;
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

    public AlterStaminaEffect() { }

    public AlterStaminaEffect(string effectID, bool activateToolTip, int priority, bool hollowEffect, bool includeCardsInDeck, int AlterAmount, int MultiplyAmount, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, bool orChoice, List<DynamicConditionInfo> dynamicConditionInfos, TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, List<EventInfo> Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, EventInfo durationType, bool Passive, bool Aditive, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, List<Card> targetForLinked_Card, DynamicAmountInfo dynamicAmountInfo, EventReference sfx,CounterTypeInfo typeOfCounter, int counterValue, bool moduloValue)
    {
        Priority = priority;
        HollowEffect = hollowEffect;
        ActivateToolTip = activateToolTip;
        EffectID = effectID;
        IncludeCardsInDeck = includeCardsInDeck;
        alterAmount = AlterAmount;
        multiplyAmount = MultiplyAmount;
        PayXEffect = payXEffect;
        PayXValue = payXValue;
        MultiHit = multiHit;
        ActivateNumber = activateNumber;
        ActivateLeft = activateLeft;
        ORChoice = orChoice;
        DynamicConditionInfos = dynamicConditionInfos;
        targetModeInfo = TargetModeInfo;
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
        TargetForLinked_Card = targetForLinked_Card;
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
            alterAmount = PayXValue;
        }

        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetModeInfo.targetMode == TargetMode.Manual)
            {
                AlterStaminaGA alterStaminaGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, null, null, null, targetModeInfo);
                alterStaminaGA.CardActionner = CardActionner;
                alterStaminaGA.SourceEffect = this;
                alterStaminaGA.ActivateToolTip = false;
                alterStaminaGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterDurabilitySound : SFX;

                if (targetModeInfo.PlayerOrEnemy == Enemy_Player_ENUM.Card)
                {
                    StartCardTargetingGA startCardTargetingGA = new(alterStaminaGA, targetNumber, TargetUpTo, this, targetLimitations);
                    startCardTargetingGA.SourceEffect = this;
                    startCardTargetingGA.ActivateToolTip = ActivateToolTip;
                    return startCardTargetingGA;
                }
                else
                {
                    StartManualTargetingGA startManualTargetingGA = new(alterStaminaGA, targetNumber, TargetUpTo, this, targetLimitations);
                    startManualTargetingGA.SourceEffect = this;
                    startManualTargetingGA.ActivateToolTip = ActivateToolTip;
                    return startManualTargetingGA;
                }
            }
            else if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
            {
                AlterStaminaGA alterStaminaGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy, ParentEffect.TargetForLinked_Card, targetModeInfo);
                alterStaminaGA.CardActionner = CardActionner;
                alterStaminaGA.SourceEffect = this;
                alterStaminaGA.ActivateToolTip = ActivateToolTip;
                alterStaminaGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterDurabilitySound : SFX;
                return alterStaminaGA;
            }
            else
            {
                if (targetModeInfo.PlayerOrEnemy == Enemy_Player_ENUM.Card)
                {
                    var cardTargets = TargetSystem.GetCardsTargets(targetModeInfo, null, IncludeCardsInDeck);

                    TargetForLinked_Card = cardTargets;

                    AlterStaminaGA alterStaminaGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, null, null, cardTargets, targetModeInfo);
                    alterStaminaGA.CardActionner = CardActionner;
                    alterStaminaGA.SourceEffect = this;
                    alterStaminaGA.ActivateToolTip = ActivateToolTip;
                    alterStaminaGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterDurabilitySound : SFX;
                    return alterStaminaGA;
                }
                else
                {
                    var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, null, this);

                    TargetForLinked_Player = playerTargets;
                    TargetForLinked_Enemy = enemyTargets;

                    AlterStaminaGA alterStaminaGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, playerTargets, enemyTargets, null, targetModeInfo);
                    alterStaminaGA.CardActionner = CardActionner;
                    alterStaminaGA.SourceEffect = this;
                    alterStaminaGA.ActivateToolTip = ActivateToolTip;
                    alterStaminaGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterDurabilitySound : SFX;
                    return alterStaminaGA;
                }
            }
        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    EnemyAlterStaminaGA enemyAlterStaminaGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, null, null, null, targetModeInfo);
                    enemyAlterStaminaGA.Actionner = Actionner;
                    enemyAlterStaminaGA.SourceEffect = this;
                    enemyAlterStaminaGA.ActivateToolTip = false;
                    enemyAlterStaminaGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterDurabilitySound : SFX;
                    if (targetModeInfo.PlayerOrEnemy == Enemy_Player_ENUM.Card)
                    {
                        StartCardTargetingGA startCardTargetingGA = new(enemyAlterStaminaGA, targetNumber, TargetUpTo, this, targetLimitations);
                        startCardTargetingGA.SourceEffect = this;
                        startCardTargetingGA.ActivateToolTip = ActivateToolTip;
                        return startCardTargetingGA;
                    }
                    else
                    {
                        StartManualTargetingGA startManualTargetingGA = new(enemyAlterStaminaGA, targetNumber, TargetUpTo, this, targetLimitations);
                        startManualTargetingGA.SourceEffect = this;
                        startManualTargetingGA.ActivateToolTip = ActivateToolTip;
                        return startManualTargetingGA;
                    }
                }
                else
                {
                    List<PermanentView> playerTargets = new();
                    List<EnemySlotView> enemyTargets = new();
                    List<Card> cardTargets = new();

                    if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
                    {
                        playerTargets = ParentEffect.TargetForLinked_Player;
                        enemyTargets = ParentEffect.TargetForLinked_Enemy;
                        cardTargets = ParentEffect.TargetForLinked_Card;
                    }
                    else
                    {
                        if (targetModeInfo.PlayerOrEnemy == Enemy_Player_ENUM.Card)
                        {
                            cardTargets = TargetSystem.GetCardsTargets(targetModeInfo, null, IncludeCardsInDeck);

                            TargetForLinked_Card = cardTargets;
                        }
                        else
                        {
                            (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, Actionner, this);

                            TargetForLinked_Player = playerTargets;
                            TargetForLinked_Enemy = enemyTargets;
                        }
                    }

                    EnemyAlterStaminaGA enemyAlterStaminaGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, playerTargets, enemyTargets, cardTargets, targetModeInfo);
                    enemyAlterStaminaGA.Actionner = Actionner;
                    enemyAlterStaminaGA.SourceEffect = this;
                    enemyAlterStaminaGA.ActivateToolTip = ActivateToolTip;
                    enemyAlterStaminaGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterDurabilitySound : SFX;
                    return enemyAlterStaminaGA;
                }
            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    PlayerAlterStaminaGA playerAlterStaminaGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, null, null, null, targetModeInfo);
                    playerAlterStaminaGA.Actionner = Actionner;
                    playerAlterStaminaGA.SourceEffect = this;
                    playerAlterStaminaGA.ActivateToolTip = false;
                    playerAlterStaminaGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterDurabilitySound : SFX;
                    if (targetModeInfo.PlayerOrEnemy == Enemy_Player_ENUM.Card)
                    {
                        StartCardTargetingGA startCardTargetingGA = new(playerAlterStaminaGA, targetNumber, TargetUpTo, this, targetLimitations);
                        startCardTargetingGA.SourceEffect = this;
                        startCardTargetingGA.ActivateToolTip = ActivateToolTip;
                        return startCardTargetingGA;
                    }
                    else
                    {
                        StartManualTargetingGA startManualTargetingGA = new(playerAlterStaminaGA, targetNumber, TargetUpTo, this, targetLimitations);
                        startManualTargetingGA.SourceEffect = this;
                        startManualTargetingGA.ActivateToolTip = ActivateToolTip;
                        return startManualTargetingGA;
                    }
                }
                else
                {
                    List<PermanentView> playerTargets = new();
                    List<EnemySlotView> enemyTargets = new();
                    List<Card> cardTargets = new();

                    if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
                    {
                        playerTargets = ParentEffect.TargetForLinked_Player;
                        enemyTargets = ParentEffect.TargetForLinked_Enemy;
                        cardTargets = ParentEffect.TargetForLinked_Card;
                    }
                    else
                    {
                        if (targetModeInfo.PlayerOrEnemy == Enemy_Player_ENUM.Card)
                        {
                            cardTargets = TargetSystem.GetCardsTargets(targetModeInfo, null, IncludeCardsInDeck);

                            TargetForLinked_Card = cardTargets;
                        }
                        else
                        {
                            (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, Actionner, this);

                            TargetForLinked_Player = playerTargets;
                            TargetForLinked_Enemy = enemyTargets;
                        }
                    }

                    PlayerAlterStaminaGA playerAlterStaminaGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, playerTargets, enemyTargets, cardTargets, targetModeInfo);
                    playerAlterStaminaGA.Actionner = Actionner;
                    playerAlterStaminaGA.SourceEffect = this;
                    playerAlterStaminaGA.ActivateToolTip = ActivateToolTip;
                    playerAlterStaminaGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterDurabilitySound : SFX;
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
    
    public override GameAction GetCounterMesure()
    {
        Disabled = true;
        AlterStaminaGA alterStaminaGA = new(-alterAmount, multiplyAmount, DynamicAmountInfo, passive, aditive, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy, ParentEffect.TargetForLinked_Card, targetModeInfo);
        alterStaminaGA.CardActionner = CardActionner;
        alterStaminaGA.SourceEffect = this;
        alterStaminaGA.ActivateToolTip = false;
        alterStaminaGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterDurabilitySound : SFX;
        return alterStaminaGA;
    }

    public override Effect Clone()
    {
        var clonedPlayerTargets = TargetForLinked_Player != null 
            ? new List<PermanentView>(TargetForLinked_Player) 
            : null;

        var clonedEnemyTargets = TargetForLinked_Enemy != null
            ? new List<EnemySlotView>(TargetForLinked_Enemy)
            : null;
            
        var clonedCardTargets = TargetForLinked_Card != null 
            ? new List<Card>(TargetForLinked_Card) 
            : null;

        Effect clonedLinked = LinkedEffect != null ? LinkedEffect.Clone() : null;

        return new AlterStaminaEffect(
            EffectID,
            ActivateToolTip,
            Priority,
            HollowEffect,
            IncludeCardsInDeck,
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
            clonedCardTargets,
            DynamicAmountInfo,
            SFX,
            TypeOfCounter,
            CounterValue,
            ModuloValue
        );
    }
}
