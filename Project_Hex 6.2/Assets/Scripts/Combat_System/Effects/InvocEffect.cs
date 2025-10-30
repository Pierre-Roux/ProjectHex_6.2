using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class InvocEffect : Effect
{
    [Header("Effect Param")]
    [SerializeField] public int amount;
    [SerializeField] public DynamicAmount DynamicAmount;
    [SerializeField] public List<CardData> CardsToInvoc;
    [SerializeField] public List<EnemyPermanentData> EnemyToInvoc;

    public InvocEffect() { }

    public InvocEffect(int Amount, int multiHit, List<CardData> cardsToInvoc,List<EnemyPermanentData> enemyToInvoc , int testValue,DynamicCondition dynamicCondition, DynamicAmount testDynamicAmount,PermaTypes testType, ActionnerType ActionnerType, Events Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount, EventReference sfx)
    {
        amount = Amount;
        MultiHit = multiHit;
        CardsToInvoc = cardsToInvoc;
        EnemyToInvoc = enemyToInvoc;
        TestValue = testValue;
        TestDynamicAmount = testDynamicAmount;
        DynamicCondition = dynamicCondition;
        TestType = testType;
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
            if (DynamicCondition != DynamicCondition.NULL)
            {
                if (Actionner == null)
                {
                    if (!ConditionSystem.Instance.TestCondition(DynamicCondition, TestDynamicAmount, TestValue, CardActionner, null, null))
                    {
                        return null;
                    }
                }
                else
                {
                    if (!ConditionSystem.Instance.TestCondition(DynamicCondition, TestDynamicAmount, TestValue, CardActionner, Actionner.GetComponent<PermanentView>(), Actionner.GetComponent<EnemySlotView>()))
                    {
                        return null;
                    }
                }
            }
        }
        // SI CARTE
        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
                InvocGA invocGA = new(amount, DynamicAmount, CardsToInvoc, EnemyToInvoc);
                if (AudioManager.Instance.IsValid(SFX)) { invocGA.SFX = SFX; }
                return invocGA;
        }
        // SI PERMANENT
        else
        {
            // SI ENEMY
            if (actionnerType == ActionnerType.ENEMY)
            {
                InvocEGA invocEGA = new(amount, DynamicAmount, EnemyToInvoc);
                invocEGA.Actionner = Actionner;
                if (AudioManager.Instance.IsValid(SFX)) { invocEGA.SFX = SFX; }
                return invocEGA;
            }
            // SI PLAYER
            else if (actionnerType == ActionnerType.PLAYER)
            {
                InvocPGA invocPGA = new(amount, DynamicAmount, CardsToInvoc);
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
            MultiHit,
            CardsToInvoc,
            EnemyToInvoc,
            TestValue,
            DynamicCondition,
            TestDynamicAmount,
            TestType,
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
