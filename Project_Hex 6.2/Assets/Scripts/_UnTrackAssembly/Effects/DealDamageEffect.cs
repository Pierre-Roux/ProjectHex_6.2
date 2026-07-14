using System;
using System.Collections.Generic;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

public class DealDamageEffect : Effect
{
    [Header("Effect Param")]

    [SerializeField] public int damageAmount;
    [SerializeField] public int multiplyAmount = 1;
    [SerializeField] public DynamicAmount DynamicAmount;
    [SerializeField] public TargetModeInfo targetModeInfo;
    [SerializeField] public override TargetModeInfo EffectTargetModeInfo => targetModeInfo;

    [Header("For Manual Target only")]

    [SerializeField] private bool TargetUpTo = true;
    public override bool EffectTargetUpTo => TargetUpTo;

    [SerializeField] private int targetNumber = 1;
    public override int EffectTargetNumber => targetNumber;

    [SerializeField] private string Description = "@ConditionsDeal @Amount@Multiply damage@TargetDuration@TargetNumber@TargetActivate";
    public override string EffectDescription => Description;

    [field: SerializeReference, SR] private List<TargetLimitationInfo> targetLimitations;
    public override List<TargetLimitationInfo> EffectTargetLimitations => targetLimitations;

    public DealDamageEffect() { }

    public DealDamageEffect(string effectID, bool activateToolTip, int priority, bool hollowEffect, string description, int DamageAmount, int MultiplyAmount, bool payXEffect, int payXValue, int multiHit, int activateNumber, int activateLeft, bool orChoice,List<DynamicConditionInfo> dynamicConditionInfos, TargetModeInfo TargetModeInfo, List<TargetLimitationInfo> TargetLimitations, int TargetNumber, bool targetUpTo, ActionnerType ActionnerType, List<Events> Event, bool cancelOnDeath, GameObject actionner, Card cardActionner, String intent_Title, String Number, int duration, Events durationType, bool triggerOnDurationEnd, Effect linkedEffect, List<PermanentView> targetForLinked_Player, List<EnemySlotView> targetForLinked_Enemy, DynamicAmount dynamicAmount, EventReference sfx,CounterType typeOfCounter, int counterValue, bool moduloValue)
    {
        Priority = priority;
        ActivateToolTip = activateToolTip;
        EffectID = effectID;
        Description = description;
        HollowEffect = hollowEffect;
        damageAmount = DamageAmount;
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
        DynamicAmount = dynamicAmount;
        SFX = sfx;
        TypeOfCounter = typeOfCounter;
        CounterValue = counterValue;
        ModuloValue = moduloValue;
    }

    public override string GetParsedDescription()
    {
        string desc = EffectDescription;
        /*
        //@ConditionsDeal @Amount@Multiply damage@TargetDuration@TargetNumber@TargetActivate

        // ----- [1] Amount -----
        string amountText = PayXEffect ? "X" :
            (DynamicAmount != DynamicAmount.NULL ? DynamicAmount.ToString() : damageAmount.ToString());

        string multiplyText = multiplyAmount > 1 ? $"×{multiplyAmount}" : "";

        // ----- [2] Duration -----
        string durationText = "";
        //if (Duration > 0 && DurationType != Events.NULL)
        {
            string durationTypeText = DurationType.ToString().Replace("On", ""); // ex: OnTurnStart → TurnStart
            durationTypeText = char.ToLower(durationTypeText[0]) + durationTypeText.Substring(1);

            if (TriggerOnDurationEnd)
                durationText = $" after {Duration} {durationTypeText}{(Duration > 1 ? "s" : "")}";
            else
                durationText = $" in {Duration} {durationTypeText}{(Duration > 1 ? "s" : "")}";
        }

        // ----- [3] Activation -----
        string activateText = (ActivateNumber > 0)
            ? $" ({ActivateNumber} time{(ActivateNumber > 1 ? "s" : "")})"
            : "";

        // ----- [4] Conditions -----
        string conditionsText = "";
        if (DynamicConditionInfos != null && DynamicConditionInfos.Count > 0)
        {
            var condTexts = new List<string>();
            foreach (var cond in DynamicConditionInfos)
                if (cond != null)
                    condTexts.Add($"if {cond}");
            conditionsText = string.Join(", ", condTexts) + ", ";
        }

        // ----- [5] Target Mode Info -----
        string targetModeText = "";
        if (targetModeInfo != null)
        {
            // 1️⃣ Type de camp : Player / Enemy
            string sideText = targetModeInfo.PlayerOrEnemy switch
            {
                Enemy_Player_ENUM.Player => "ally",
                Enemy_Player_ENUM.Enemy => "enemy",
                _ => ""
            };

            // 3️⃣ Type de permanent
            string typeText = targetModeInfo.PermaType != PermaTypes.NULL
                ? $" {targetModeInfo.PermaType.ToString().ToLower()}"
                : "";

            // 4️⃣ Mode de ciblage
            string modeText = targetModeInfo.targetMode switch
            {
                TargetMode.Self => "self",
                TargetMode.Core => "core",
                TargetMode.All => $"all {(sideText != "" ? sideText + " " : "")}{typeText.Trim()}s",
                TargetMode.RDM => $"a random {(sideText != "" ? sideText + " " : "")}{typeText.Trim()}",
                TargetMode.HighHP => $"the {(sideText != "" ? sideText + " " : "")}{typeText.Trim()} with the highest HP",
                TargetMode.LowHP => $"the {(sideText != "" ? sideText + " " : "")}{typeText.Trim()} with the lowest HP",
                _ => $"{(sideText != "" ? sideText + " " : "")}{typeText.Trim()}"
            };

            targetModeText = modeText.Trim();
        }

        // ----- [6] Nombre de cibles -----
        string targetNumberText = "";
        if (EffectTargetNumber > 0)
        {
            string upTo = EffectTargetUpTo ? "up to " : "";
            string plural = EffectTargetNumber > 1 ? "s" : "";
            targetNumberText = $" {upTo}{EffectTargetNumber} {targetModeText}{plural}";
        }
        else if (!string.IsNullOrEmpty(targetModeText))
        {
            targetNumberText = $" {targetModeText}";
        }

        // ----- [7] Dictionnaire des remplacements -----
        Dictionary<string, string> replacements = new()
        {
            { "@ConditionsDeal", conditionsText },
            { "@Amount", amountText },
            { "@Multiply", multiplyText },
            { "@TargetDuration", durationText },
            { "@TargetActivate", activateText },
            { "@TargetNumber", targetNumberText },
            { "@TargetMode", "" },
            { "@TargetLimitations", "" },
            { "@TargetUpTo", "" }
        };

        foreach (var kvp in replacements)
            desc = desc.Replace(kvp.Key, kvp.Value);

        // ----- [8] Nettoyage -----
        desc = System.Text.RegularExpressions.Regex.Replace(desc, @"\s+", " ").Trim();
        if (desc.Length > 0)
            desc = char.ToUpper(desc[0]) + desc.Substring(1);
        */
        return desc;
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
            damageAmount = PayXValue;
        }

        if (Actionner == null && actionnerType == ActionnerType.NONE)
        {
            if (targetModeInfo.targetMode == TargetMode.Manual)
            {
                DealDamageGA dealDamageGA = new(damageAmount, 0, multiplyAmount, DynamicAmount, null, null);
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
                DealDamageGA dealDamageGA = new(damageAmount, 0, multiplyAmount, DynamicAmount, ParentEffect.TargetForLinked_Player, ParentEffect.TargetForLinked_Enemy);
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

                DealDamageGA dealDamageGA = new(damageAmount, 0, multiplyAmount, DynamicAmount, playerTargets, enemyTargets);
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
                    AttackPlayerGA attackPlayerGA = new(damageAmount, multiplyAmount, DynamicAmount, null, null);
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

                    AttackPlayerGA attackPlayerGA = new(damageAmount, multiplyAmount, DynamicAmount, playerTargets, enemyTargets);
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
                    AttackEnemyGA attackEnemyGA = new(damageAmount, multiplyAmount, DynamicAmount, null, null);
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

                    AttackEnemyGA attackEnemyGA = new(damageAmount, multiplyAmount, DynamicAmount, playerTargets, enemyTargets);
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
            SFX,
            TypeOfCounter,
            CounterValue,
            ModuloValue
        );
    }
}
