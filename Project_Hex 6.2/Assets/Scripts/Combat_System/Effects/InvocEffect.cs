using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class InvocEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public int amount = 1;
    [SerializeField] public int multiplyAmount = 1;
    [SerializeField] public DynamicAmount DynamicAmount;
    [SerializeField] public List<CardData> CardsToInvoc;
    [SerializeField] public List<EnemyPermanentData> EnemyToInvoc;

    public InvocEffect() { }

    public InvocEffect(int Amount, int MultiplyAmount, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, List<CardData> cardsToInvoc,List<EnemyPermanentData> enemyToInvoc , List<DynamicConditionInfo> dynamicConditionInfos, ActionnerType ActionnerType, Events Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount, EventReference sfx)
    {
        amount = Amount;
        multiplyAmount = MultiplyAmount;
        PayXEffect = payXEffect;
        PayXValue = payXValue;
        MultiHit = multiHit;
        ActivateNumber = activateNumber;
        ActivateLeft = activateLeft;
        CardsToInvoc = cardsToInvoc;
        EnemyToInvoc = enemyToInvoc;
        DynamicConditionInfos = dynamicConditionInfos;
        actionnerType = ActionnerType;
        Events = Event;
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
        DynamicAmount = dynamicAmount;
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

        if (PayXValue != 0)
        {
            DynamicAmount = DynamicAmount.NULL;
            amount = PayXValue;
        }

        // SI CARTE
        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            InvocGA invocGA = new(amount,multiplyAmount, DynamicAmount, CardsToInvoc, EnemyToInvoc);
            invocGA.CardActionner = CardActionner;
            if (AudioManager.Instance.IsValid(SFX)) { invocGA.SFX = SFX; }
            return invocGA;
        }
        // SI PERMANENT
        else
        {
            // SI ENEMY
            if (actionnerType == ActionnerType.ENEMY)
            {
                InvocEGA invocEGA = new(amount,multiplyAmount, DynamicAmount, EnemyToInvoc);
                invocEGA.Actionner = Actionner;
                if (AudioManager.Instance.IsValid(SFX)) { invocEGA.SFX = SFX; }
                return invocEGA;
            }
            // SI PLAYER
            else if (actionnerType == ActionnerType.PLAYER)
            {
                InvocPGA invocPGA = new(amount,multiplyAmount, DynamicAmount, CardsToInvoc);
                invocPGA.Actionner = Actionner;
                if (AudioManager.Instance.IsValid(SFX)) { invocPGA.SFX = SFX; }
                return invocPGA;
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

        return new InvocEffect(
            amount,
            multiplyAmount,
            PayXEffect,
            PayXValue,
            MultiHit,
            ActivateNumber,
            ActivateLeft,
            CardsToInvoc,
            EnemyToInvoc,
            DynamicConditionInfos,
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
            DynamicAmount,
            SFX
        );
    }
}
