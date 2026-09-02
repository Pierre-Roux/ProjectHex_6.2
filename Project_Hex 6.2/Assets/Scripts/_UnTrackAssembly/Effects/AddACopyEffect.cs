using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class AddACopyEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public int Amount;
    [SerializeField] public int multiplyAmount = 1;
    [SerializeField] public DynamicAmountInfo DynamicAmountInfo;
    [SerializeField] public Enemy_Player_ENUM AffectedSide;
    [SerializeField] public CopyTokenType TypeOfCopy;

    public AddACopyEffect(){}

    public AddACopyEffect(string effectID, bool activateToolTip, int priority, bool hollowEffect, int amount, Enemy_Player_ENUM affectedSide, CopyTokenType typeOfCopy, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, bool orChoice, List<DynamicConditionInfo> dynamicConditionInfos, ActionnerType ActionnerType, List<EventInfo> Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, EventInfo durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmountInfo dynamicAmountInfo, EventReference sfx,CounterTypeInfo typeOfCounter, int counterValue, bool moduloValue)
    {
        Priority = priority;
        HollowEffect = hollowEffect;
        ActivateToolTip = activateToolTip;
        EffectID = effectID;
        Amount = amount;
        AffectedSide = affectedSide;
        TypeOfCopy = typeOfCopy;
        PayXEffect = payXEffect;
        PayXValue = payXValue;
        MultiHit = multiHit;
        ActivateNumber = activateNumber;
        ActivateLeft = activateLeft;
        ORChoice = orChoice;
        EventInfos = Event;
        DynamicConditionInfos = dynamicConditionInfos;
        CancelOnDeath = cancelOnDeath;
        actionnerType = ActionnerType;
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
            Amount = PayXValue;
        }

        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            AddACopyGa addACopyGa = new(Amount, multiplyAmount, DynamicAmountInfo, AffectedSide, TypeOfCopy);
            addACopyGa.CardActionner = CardActionner;
            addACopyGa.SourceEffect = this;
            addACopyGa.ActivateToolTip = ActivateToolTip;
            addACopyGa.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AddACopySound : SFX;
            return addACopyGa;
        }
        else
        {
            if (actionnerType == ActionnerType.ENEMY)
            {
                PlayerAddACopyGa playerAddACopyGa = new(Amount, multiplyAmount, DynamicAmountInfo, AffectedSide, TypeOfCopy);
                playerAddACopyGa.Actionner = Actionner;
                playerAddACopyGa.SourceEffect = this;
                playerAddACopyGa.ActivateToolTip = ActivateToolTip;
                playerAddACopyGa.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AddACopySound : SFX;
                return playerAddACopyGa;
            }
            else if (actionnerType == ActionnerType.PLAYER)
            {
                EnemyAddACopyGa enemyAddACopyGa = new(Amount, multiplyAmount, DynamicAmountInfo, AffectedSide, TypeOfCopy);
                enemyAddACopyGa.Actionner = Actionner;
                enemyAddACopyGa.SourceEffect = this;
                enemyAddACopyGa.ActivateToolTip = ActivateToolTip;
                enemyAddACopyGa.SFX = !AudioManager.Instance.IsValid(SFX) ? AudioManager.Instance.Effect_AddACopySound : SFX;
                return enemyAddACopyGa;
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

        return new AddACopyEffect(
            EffectID,
            ActivateToolTip,
            Priority,
            HollowEffect,
            Amount,
            AffectedSide,
            TypeOfCopy,
            PayXEffect,
            PayXValue,
            MultiHit,
            ActivateNumber,
            ActivateLeft,
            ORChoice,
            DynamicConditionInfos,
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
