using System;
using System.Collections.Generic;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class ExhaustEffect : Effect
{
    [Header("Effect Param")]

    [SerializeField] public bool IncludeCardsInDeck;
    [SerializeField] public TargetModeInfo targetModeInfo;
    [SerializeField] public override TargetModeInfo EffectTargetModeInfo => targetModeInfo;

    [Header("For Manual Target only")]
    [SerializeField] private bool TargetUpTo = true;
    public override bool EffectTargetUpTo => TargetUpTo;

    [SerializeField] private int targetNumber = 1;
    public override int EffectTargetNumber => targetNumber;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public ExhaustEffect() { }

    public ExhaustEffect(string effectID, bool activateToolTip, int priority, bool hollowEffect, bool includeCardsInDeck, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, bool orChoice, List<DynamicConditionInfo> dynamicConditionInfos, TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, List<Events> Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, EventReference sfx,CounterType typeOfCounter, int counterValue, bool moduloValue)
    {
        Priority = priority;
        HollowEffect = hollowEffect;
        ActivateToolTip = activateToolTip;
        EffectID = effectID;
        IncludeCardsInDeck = includeCardsInDeck;
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
                ExhaustGA exhaustGA = new(null, null, null, targetModeInfo);
                exhaustGA.CardActionner = CardActionner;
                exhaustGA.SourceEffect = this;
                exhaustGA.ActivateToolTip = false;
                exhaustGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterDurabilitySound : SFX;

                if (targetModeInfo.PlayerOrEnemy == Enemy_Player_ENUM.Card)
                {
                    StartCardTargetingGA startCardTargetingGA = new(exhaustGA, targetNumber, TargetUpTo, this, targetLimitations);
                    startCardTargetingGA.SourceEffect = this;
                    startCardTargetingGA.ActivateToolTip = ActivateToolTip;
                    return startCardTargetingGA;                    
                }
                else
                {
                    StartManualTargetingGA startManualTargetingGA = new(exhaustGA, targetNumber, TargetUpTo, this, targetLimitations);
                    startManualTargetingGA.SourceEffect = this;
                    startManualTargetingGA.ActivateToolTip = ActivateToolTip;
                    return startManualTargetingGA;                   
                }
            }
            else if (targetModeInfo.targetMode == TargetMode.EffectParent_Targets)
            {
                ExhaustGA exhaustGA = new(ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy, ParentEffect.TargetForLinked_Card, targetModeInfo);
                exhaustGA.CardActionner = CardActionner;
                exhaustGA.SourceEffect = this;
                exhaustGA.ActivateToolTip = ActivateToolTip;
                exhaustGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterDurabilitySound : SFX;
                return exhaustGA;
            }
            else
            {
                if (targetModeInfo.PlayerOrEnemy == Enemy_Player_ENUM.Card)
                {
                    var cardTargets = TargetSystem.GetCardsTargets(targetModeInfo, null,IncludeCardsInDeck);

                    TargetForLinked_Card = cardTargets;

                    ExhaustGA exhaustGA = new(null, null, cardTargets, targetModeInfo);
                    exhaustGA.CardActionner = CardActionner;
                    exhaustGA.SourceEffect = this;
                    exhaustGA.ActivateToolTip = ActivateToolTip;
                    exhaustGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterDurabilitySound : SFX;
                    return exhaustGA;  
                }
                else
                {
                    var (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, null,this);

                    TargetForLinked_Player = playerTargets;
                    TargetForLinked_Enemy = enemyTargets;

                    ExhaustGA exhaustGA = new(playerTargets, enemyTargets, null, targetModeInfo);
                    exhaustGA.CardActionner = CardActionner;
                    exhaustGA.SourceEffect = this;
                    exhaustGA.ActivateToolTip = ActivateToolTip;
                    exhaustGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterDurabilitySound : SFX;
                    return exhaustGA;                    
                }
            }
        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    EnemyExhaustGA enemyExhaustGA = new(null, null, null, targetModeInfo);
                    enemyExhaustGA.Actionner = Actionner;
                    enemyExhaustGA.SourceEffect = this;
                    enemyExhaustGA.ActivateToolTip = false;
                    enemyExhaustGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterDurabilitySound : SFX;
                    if (targetModeInfo.PlayerOrEnemy == Enemy_Player_ENUM.Card)
                    {
                        StartCardTargetingGA startCardTargetingGA = new(enemyExhaustGA, targetNumber, TargetUpTo, this, targetLimitations);
                        startCardTargetingGA.SourceEffect = this;
                        startCardTargetingGA.ActivateToolTip = ActivateToolTip;
                        return startCardTargetingGA;                        
                    }
                    else
                    {
                        StartManualTargetingGA startManualTargetingGA = new(enemyExhaustGA, targetNumber, TargetUpTo, this, targetLimitations);
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
                            (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, Actionner,this);

                            TargetForLinked_Player = playerTargets;
                            TargetForLinked_Enemy = enemyTargets;                            
                        }
                    }

                    EnemyExhaustGA enemyExhaustGA = new(playerTargets, enemyTargets, cardTargets, targetModeInfo);
                    enemyExhaustGA.Actionner = Actionner;
                    enemyExhaustGA.SourceEffect = this;
                    enemyExhaustGA.ActivateToolTip = ActivateToolTip;
                    enemyExhaustGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterDurabilitySound : SFX;
                    return enemyExhaustGA;
                }
            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                if (targetModeInfo.targetMode == TargetMode.Manual)
                {
                    PlayerExhaustGA playerExhaustGA = new(null, null, null, targetModeInfo);
                    playerExhaustGA.Actionner = Actionner;
                    playerExhaustGA.SourceEffect = this;
                    playerExhaustGA.ActivateToolTip = false;
                    playerExhaustGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterDurabilitySound : SFX;
                    if (targetModeInfo.PlayerOrEnemy == Enemy_Player_ENUM.Card)
                    {
                        StartCardTargetingGA startCardTargetingGA = new(playerExhaustGA, targetNumber, TargetUpTo, this, targetLimitations);
                        startCardTargetingGA.SourceEffect = this;
                        startCardTargetingGA.ActivateToolTip = ActivateToolTip;
                        return startCardTargetingGA;                        
                    }
                    else
                    {
                        StartManualTargetingGA startManualTargetingGA = new(playerExhaustGA, targetNumber, TargetUpTo, this, targetLimitations);
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
                            (playerTargets, enemyTargets) = TargetSystem.GetTargets(targetModeInfo, Actionner,this);

                            TargetForLinked_Player = playerTargets;
                            TargetForLinked_Enemy = enemyTargets;                            
                        }
                    }

                    PlayerExhaustGA playerExhaustGA = new(playerTargets, enemyTargets, cardTargets, targetModeInfo);
                    playerExhaustGA.Actionner = Actionner;
                    playerExhaustGA.SourceEffect = this;
                    playerExhaustGA.ActivateToolTip = ActivateToolTip;
                    playerExhaustGA.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AlterDurabilitySound : SFX;
                    return playerExhaustGA;
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

        return new ExhaustEffect(
            EffectID,
            ActivateToolTip,
            Priority,
            HollowEffect,
            IncludeCardsInDeck,
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
