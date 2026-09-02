using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class AlterCostEffect : Effect
{
    [Header("Effect Param")]

    [SerializeField] public bool CanBeDisable = true;
    [SerializeField] public override bool CanBeDisableEffect => CanBeDisable;
    [SerializeField] public int alterAmount;
    [SerializeField] public int multiplyAmount = 1;
    [SerializeField] public DynamicAmountInfo DynamicAmountInfo;
    [SerializeField] public bool IncludeCardsInDeck;
    [SerializeField] public TargetModeInfo targetModeInfo;
    [SerializeField] public override TargetModeInfo EffectTargetModeInfo => targetModeInfo;
    [SerializeField] public bool passive;

    [Header("For Manual Target only")]
    [SerializeField] private bool TargetUpTo = true;
    public override bool EffectTargetUpTo => TargetUpTo;

    [SerializeField] private int targetNumber = 1;
    public override int EffectTargetNumber => targetNumber;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public AlterCostEffect() { }

    public AlterCostEffect(string effectID, bool activateToolTip, int priority, bool hollowEffect, int AlterAmount, int MultiplyAmount, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, bool orChoice, List<DynamicConditionInfo> dynamicConditionInfos, bool includeCardsInDeck, TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, List<EventInfo> Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, EventInfo durationType, bool Passive, bool triggerOnDurationEnd, Effect linkedEffect, List<Card> targetForLinked_Card, DynamicAmountInfo dynamicAmountInfo, EventReference sfx,CounterTypeInfo typeOfCounter, int counterValue, bool moduloValue)
    {
        Priority = priority;
        HollowEffect = hollowEffect;
        ActivateToolTip = activateToolTip;
        Priority = priority;
        EffectID = effectID;
        alterAmount = AlterAmount;
        multiplyAmount = MultiplyAmount;
        PayXEffect = payXEffect;
        PayXValue = payXValue;
        MultiHit = multiHit;
        ActivateNumber = activateNumber;
        ActivateLeft = activateLeft;
        ORChoice = orChoice;
        DynamicConditionInfos = dynamicConditionInfos;
        IncludeCardsInDeck = includeCardsInDeck;
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
        TriggerOnDurationEnd = triggerOnDurationEnd;
        LinkedEffect = linkedEffect;
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

        if (PayXValue != 0)
        {
            DynamicAmountInfo.DynamicAmount = DynamicAmount.NULL;
            alterAmount = PayXValue;
        }

        Disabled = false;

        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetModeInfo.targetMode == TargetMode.Manual && targetModeInfo.PlayerOrEnemy == Enemy_Player_ENUM.Card)
            {
                AlterCardCostGA alterCardCostGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, null, targetModeInfo);
                alterCardCostGA.CardActionner = CardActionner;
                alterCardCostGA.SourceEffect = this;
                alterCardCostGA.ActivateToolTip = false;
                alterCardCostGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterCostSound : SFX;
                StartCardTargetingGA startCardTargetingGA = new(alterCardCostGA, targetNumber, TargetUpTo, this, targetLimitations);
                startCardTargetingGA.SourceEffect = this;
                startCardTargetingGA.ActivateToolTip = ActivateToolTip;
                return startCardTargetingGA;
            }
            else if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
            {
                AlterCardCostGA alterCardCostGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, ParentEffect.TargetForLinked_Card, targetModeInfo);
                alterCardCostGA.CardActionner = CardActionner;
                alterCardCostGA.SourceEffect = this;
                alterCardCostGA.ActivateToolTip = ActivateToolTip;
                alterCardCostGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterCostSound : SFX;
                return alterCardCostGA;
            }
            else
            {
                var cardsTargets = TargetSystem.GetCardsTargets(targetModeInfo, null, IncludeCardsInDeck);

                TargetForLinked_Card = cardsTargets;

                AlterCardCostGA alterCardCostGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, cardsTargets, targetModeInfo);
                alterCardCostGA.CardActionner = CardActionner;
                alterCardCostGA.SourceEffect = this;
                alterCardCostGA.ActivateToolTip = ActivateToolTip;
                alterCardCostGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterCostSound : SFX;
                return alterCardCostGA;
            }
        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual && targetModeInfo.PlayerOrEnemy == Enemy_Player_ENUM.Card)
                {
                    EnemyAlterCardCostGA enemyAlterCardCostGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, null, targetModeInfo);
                    enemyAlterCardCostGA.Actionner = Actionner;
                    enemyAlterCardCostGA.SourceEffect = this;
                    enemyAlterCardCostGA.ActivateToolTip = false;
                    enemyAlterCardCostGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterCostSound : SFX;
                    StartCardTargetingGA startCardTargetingGA = new(enemyAlterCardCostGA, targetNumber, TargetUpTo, this, targetLimitations);
                    startCardTargetingGA.SourceEffect = this;
                    startCardTargetingGA.ActivateToolTip = ActivateToolTip;
                    return startCardTargetingGA;
                }
                else
                {
                    List<Card> CardsTargets;

                    if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
                    {
                        CardsTargets = ParentEffect.TargetForLinked_Card;
                    }
                    else
                    {
                        CardsTargets = TargetSystem.GetCardsTargets(targetModeInfo, null, IncludeCardsInDeck);

                        TargetForLinked_Card = CardsTargets;
                    }

                    EnemyAlterCardCostGA enemyAlterCardCostGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, CardsTargets, targetModeInfo);
                    enemyAlterCardCostGA.Actionner = Actionner;
                    enemyAlterCardCostGA.SourceEffect = this;
                    enemyAlterCardCostGA.ActivateToolTip = ActivateToolTip;
                    enemyAlterCardCostGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterCostSound : SFX;
                    return enemyAlterCardCostGA;
                }
            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual && targetModeInfo.PlayerOrEnemy == Enemy_Player_ENUM.Card)
                {
                    PlayerAlterCardCostGA playerAlterCardCostGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, null, targetModeInfo);
                    playerAlterCardCostGA.Actionner = Actionner;
                    playerAlterCardCostGA.SourceEffect = this;
                    playerAlterCardCostGA.ActivateToolTip = false;
                    playerAlterCardCostGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterCostSound : SFX;
                    StartCardTargetingGA startCardTargetingGA = new(playerAlterCardCostGA, targetNumber, TargetUpTo, this, targetLimitations);
                    startCardTargetingGA.SourceEffect = this;
                    startCardTargetingGA.ActivateToolTip = ActivateToolTip;
                    return startCardTargetingGA;
                }
                else
                {
                    List<Card> cardsTargets;

                    if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
                    {
                        cardsTargets = ParentEffect.TargetForLinked_Card;
                    }
                    else
                    {
                        cardsTargets = TargetSystem.GetCardsTargets(targetModeInfo, null, IncludeCardsInDeck);

                        TargetForLinked_Card = cardsTargets;
                    }

                    PlayerAlterCardCostGA playerAlterCardCostGA = new(alterAmount, multiplyAmount, DynamicAmountInfo, passive, cardsTargets, targetModeInfo);
                    playerAlterCardCostGA.Actionner = Actionner;
                    playerAlterCardCostGA.SourceEffect = this;
                    playerAlterCardCostGA.ActivateToolTip = ActivateToolTip;
                    playerAlterCardCostGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterCostSound : SFX;
                    return playerAlterCardCostGA;
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
        AlterCardCostGA alterCardCostGA = new(-alterAmount, multiplyAmount, DynamicAmountInfo, passive, ParentEffect.TargetForLinked_Card, targetModeInfo);
        alterCardCostGA.CardActionner = CardActionner;
        alterCardCostGA.SourceEffect = this;
        alterCardCostGA.ActivateToolTip = false;
        alterCardCostGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterCostSound : SFX;
        return alterCardCostGA;
    }

    public override Effect Clone()
    {
        var clonedCardTargets = TargetForLinked_Card != null 
            ? new List<Card>(TargetForLinked_Card) 
            : null;

        Effect clonedLinked = LinkedEffect != null ? LinkedEffect.Clone() : null;

        return new AlterCostEffect(
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
            IncludeCardsInDeck,
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
            TriggerOnDurationEnd,
            clonedLinked,
            clonedCardTargets,
            DynamicAmountInfo,
            SFX,
            TypeOfCounter,
            CounterValue,
            ModuloValue
        );
    }
}
