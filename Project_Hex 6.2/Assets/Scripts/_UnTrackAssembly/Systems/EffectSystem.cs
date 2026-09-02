using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectSystem : Singleton<EffectSystem>
{
    public float AnimDelay = 0.25f;
    public ConditionSystem conditionSystem;

    void OnEnable()
    {
        ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
        ActionSystem.AttachPerformer<HealGA>(DealHealPerformer);
        ActionSystem.AttachPerformer<ArmorGA>(DealArmorPerformer);
        ActionSystem.AttachPerformer<ShieldGA>(DealShieldPerformer);
        ActionSystem.AttachPerformer<LoseShieldGA>(LoseShieldPerformer);
        ActionSystem.AttachPerformer<UnShieldGA>(UnShieldPerformer);
        ActionSystem.AttachPerformer<DecountPlayerDecayGA>(DecountDecayPlayerPerformer);
        ActionSystem.AttachPerformer<DecountEnemyDecayGA>(DecountDecayEnemyPerformer);
        ActionSystem.AttachPerformer<AlterPowerGA>(AlterPowerPerformer);
        ActionSystem.AttachPerformer<AlterStaminaGA>(AlterStamPerformer);
        ActionSystem.AttachPerformer<AlterCardCostGA>(AlterCardCostPerformer);
        ActionSystem.AttachPerformer<LifeLossGA>(LifeLossPerformer);
        ActionSystem.AttachPerformer<DiscardCardGA>(DiscardCardPerformer);
        ActionSystem.AttachPerformer<GainLifeGA>(GainLifePerformer);
        ActionSystem.AttachPerformer<ScryGA>(ScryPerformer);
        ActionSystem.AttachPerformer<InvocGA>(InvocPerformer);
        ActionSystem.AttachPerformer<SacGA>(SacPerformer);
        ActionSystem.AttachPerformer<RefreshGA>(RefreshPerformer);
        ActionSystem.AttachPerformer<ExhaustGA>(ExhaustedPerformer);
        ActionSystem.AttachPerformer<RetrieveExhaustedGA>(RetrieveExhaustedPerformer);
        ActionSystem.AttachPerformer<AlterPowerGridGA>(AlterPowerGridPerformer);
        ActionSystem.AttachPerformer<AddACopyGa>(AddACopyPerformer);
        ActionSystem.AttachPerformer<DisableGA>(DisablePerformer);
        ActionSystem.AttachPerformer<EnableGA>(EnablePerformer);
        ActionSystem.AttachPerformer<LetChoiceGA>(PlayerChoicePerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<DealDamageGA>();
        ActionSystem.DetachPerformer<HealGA>();
        ActionSystem.DetachPerformer<ArmorGA>();
        ActionSystem.DetachPerformer<ShieldGA>();
        ActionSystem.DetachPerformer<LoseShieldGA>();
        ActionSystem.DetachPerformer<UnShieldGA>();
        ActionSystem.DetachPerformer<DecountPlayerDecayGA>();
        ActionSystem.DetachPerformer<DecountEnemyDecayGA>();
        ActionSystem.DetachPerformer<AlterPowerGA>();
        ActionSystem.DetachPerformer<AlterStaminaGA>();
        ActionSystem.DetachPerformer<AlterCardCostGA>();
        ActionSystem.DetachPerformer<LifeLossGA>();
        ActionSystem.DetachPerformer<DiscardCardGA>();
        ActionSystem.DetachPerformer<GainLifeGA>();
        ActionSystem.DetachPerformer<ScryGA>();
        ActionSystem.DetachPerformer<InvocGA>();
        ActionSystem.DetachPerformer<SacGA>();
        ActionSystem.DetachPerformer<RefreshGA>();
        ActionSystem.DetachPerformer<ExhaustGA>();
        ActionSystem.DetachPerformer<RetrieveExhaustedGA>();
        ActionSystem.DetachPerformer<AlterPowerGridGA>();
        ActionSystem.DetachPerformer<AddACopyGa>();
        ActionSystem.DetachPerformer<DisableGA>();
        ActionSystem.DetachPerformer<EnableGA>();
        ActionSystem.DetachPerformer<LetChoiceGA>();
    }

    public void Start()
    {
        conditionSystem = ConditionSystem.Instance;
    }

    private IEnumerator DealDamagePerformer(DealDamageGA dealDamageGA)
    {
        int DamageAmount = dealDamageGA.Amount;

        if (dealDamageGA.DynamicAmountInfo.DynamicAmount != DynamicAmount.NULL)
        {
            if (dealDamageGA.Actionner == null)
            {
                if (dealDamageGA.CardActionner != null)
                {
                    DamageAmount = TargetSystem.Instance.GetDynamicAmount(dealDamageGA.DynamicAmountInfo, null, null, dealDamageGA.CardActionner);
                }
                else
                {
                    DamageAmount = TargetSystem.Instance.GetDynamicAmount(dealDamageGA.DynamicAmountInfo, null, null);
                }
            }
            else if (dealDamageGA.Actionner.GetComponent<PermanentView>() != null)
            {
                DamageAmount = TargetSystem.Instance.GetDynamicAmount(dealDamageGA.DynamicAmountInfo, dealDamageGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                DamageAmount = TargetSystem.Instance.GetDynamicAmount(dealDamageGA.DynamicAmountInfo, null, dealDamageGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        if (dealDamageGA.powerBased)
        {
            if (dealDamageGA.Actionner == null)
            {
                Debug.LogWarning("Power base Dealdamage without actionner");
            }
            else
            {
                if (dealDamageGA.Actionner.GetComponent<PermanentView>() != null)
                {
                    DamageAmount = dealDamageGA.Actionner.GetComponent<PermanentView>().currentPower;
                }
                else
                {
                    DamageAmount = dealDamageGA.Actionner.GetComponent<EnemySlotView>().currentPower;
                }
            }
        }

        if (dealDamageGA.playerTargets != null)
        {
            foreach (PermanentView target in dealDamageGA.playerTargets)
            {
                if (!conditionSystem.TestCondition(dealDamageGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, target, null, dealDamageGA.Actionner)) continue;
                if (dealDamageGA.DynamicAmountInfo.DynamicAmount == DynamicAmount.TargetParam)
                {
                    DamageAmount = TargetSystem.Instance.GetDynamicAmount(dealDamageGA.DynamicAmountInfo, target, null, null);
                }

                if (target.Shielded)
                {
                    if (target.PlayerShielder.Count != 0 && target.EnemyShielder.Count != 0)
                    {
                        var newtargetP = target.PlayerShielder[Random.Range(0, target.PlayerShielder.Count)];
                        var newtargetE = target.EnemyShielder[Random.Range(0, target.EnemyShielder.Count)];
                        if (Random.Range(0, 1) == 0)
                        {
                            newtargetP.TakeDamage(DamageAmount * dealDamageGA.multiplyAmount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                        }
                        else
                        {
                            newtargetE.TakeDamage(DamageAmount * dealDamageGA.multiplyAmount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                        }
                    }
                    else if (target.EnemyShielder.Count != 0)
                    {
                        var newtargetE = target.EnemyShielder[Random.Range(0, target.EnemyShielder.Count)];
                        newtargetE.TakeDamage(DamageAmount * dealDamageGA.multiplyAmount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                    }
                    else if (target.PlayerShielder.Count != 0)
                    {
                        var newtargetP = target.PlayerShielder[Random.Range(0, target.PlayerShielder.Count)];
                        newtargetP.TakeDamage(DamageAmount * dealDamageGA.multiplyAmount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                    }
                    yield return new WaitForSeconds(AnimDelay);
                }
                else
                {
                    target.TakeDamage(DamageAmount * dealDamageGA.multiplyAmount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }

        if (dealDamageGA.enemyTargets != null)
        {
            foreach (EnemySlotView target in dealDamageGA.enemyTargets)
            {
                if (!conditionSystem.TestCondition(dealDamageGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, null, target, dealDamageGA.Actionner)) continue;
                if (dealDamageGA.DynamicAmountInfo.DynamicAmount == DynamicAmount.TargetParam)
                {
                    DamageAmount = TargetSystem.Instance.GetDynamicAmount(dealDamageGA.DynamicAmountInfo, null, target, null);
                }
                if (target.Shielded)
                {
                    if (target.PlayerShielder.Count != 0 && target.EnemyShielder.Count != 0)
                    {
                        var newtargetP = target.PlayerShielder[Random.Range(0, target.PlayerShielder.Count)];
                        var newtargetE = target.EnemyShielder[Random.Range(0, target.EnemyShielder.Count)];
                        if (Random.Range(0, 1) == 0)
                        {
                            newtargetP.TakeDamage(DamageAmount * dealDamageGA.multiplyAmount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                        }
                        else
                        {
                            newtargetE.TakeDamage(DamageAmount * dealDamageGA.multiplyAmount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                        }
                    }
                    else if (target.EnemyShielder.Count != 0)
                    {
                        var newtargetE = target.EnemyShielder[Random.Range(0, target.EnemyShielder.Count)];
                        newtargetE.TakeDamage(DamageAmount * dealDamageGA.multiplyAmount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                    }
                    else if (target.PlayerShielder.Count != 0)
                    {
                        var newtargetP = target.PlayerShielder[Random.Range(0, target.PlayerShielder.Count)];
                        newtargetP.TakeDamage(DamageAmount * dealDamageGA.multiplyAmount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                    }
                    yield return new WaitForSeconds(AnimDelay);
                }
                else
                {

                    target.TakeDamage(DamageAmount * dealDamageGA.multiplyAmount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }
    }

    private IEnumerator DealHealPerformer(HealGA healGA)
    {
        int HealAmount = healGA.Amount;

        if (healGA.DynamicAmountInfo.DynamicAmount != DynamicAmount.NULL)
        {
            if (healGA.Actionner == null)
            {
                if (healGA.CardActionner != null)
                {
                    HealAmount = TargetSystem.Instance.GetDynamicAmount(healGA.DynamicAmountInfo, null, null, healGA.CardActionner);
                }
                else
                {
                    HealAmount = TargetSystem.Instance.GetDynamicAmount(healGA.DynamicAmountInfo, null, null);
                }
            }
            else if (healGA.Actionner.GetComponent<PermanentView>() != null)
            {
                HealAmount = TargetSystem.Instance.GetDynamicAmount(healGA.DynamicAmountInfo, healGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                HealAmount = TargetSystem.Instance.GetDynamicAmount(healGA.DynamicAmountInfo, null, healGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        if (healGA.playerTargets != null)
        {
            foreach (var target in healGA.playerTargets)
            {
                if (!conditionSystem.TestCondition(healGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, target, null, healGA.Actionner)) continue;
                if (healGA.DynamicAmountInfo.DynamicAmount == DynamicAmount.TargetParam)
                {
                    HealAmount = TargetSystem.Instance.GetDynamicAmount(healGA.DynamicAmountInfo, target, null, null);
                }
                target.TakeHeal(HealAmount * healGA.multiplyAmount);
                yield return new WaitForSeconds(AnimDelay);
            }
        }

        if (healGA.enemyTargets != null)
        {
            foreach (var target in healGA.enemyTargets)
            {
                if (!conditionSystem.TestCondition(healGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, null, target, healGA.Actionner)) continue;
                if (healGA.DynamicAmountInfo.DynamicAmount == DynamicAmount.TargetParam)
                {
                    HealAmount = TargetSystem.Instance.GetDynamicAmount(healGA.DynamicAmountInfo, null, target, null);
                }
                target.TakeHeal(HealAmount * healGA.multiplyAmount);
                yield return new WaitForSeconds(AnimDelay);
            }
        }
    }

    private IEnumerator DealArmorPerformer(ArmorGA ArmorGA)
    {
        int ArmorAmount = ArmorGA.Amount;

        if (ArmorGA.DynamicAmountInfo.DynamicAmount != DynamicAmount.NULL)
        {
            if (ArmorGA.Actionner == null)
            {
                if (ArmorGA.CardActionner != null)
                {
                    ArmorAmount = TargetSystem.Instance.GetDynamicAmount(ArmorGA.DynamicAmountInfo, null, null, ArmorGA.CardActionner);
                }
                else
                {
                    ArmorAmount = TargetSystem.Instance.GetDynamicAmount(ArmorGA.DynamicAmountInfo, null, null);
                }
            }
            else if (ArmorGA.Actionner.GetComponent<PermanentView>() != null)
            {
                ArmorAmount = TargetSystem.Instance.GetDynamicAmount(ArmorGA.DynamicAmountInfo, ArmorGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                ArmorAmount = TargetSystem.Instance.GetDynamicAmount(ArmorGA.DynamicAmountInfo, null, ArmorGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        if (ArmorGA.playerTargets != null)
        {
            foreach (var target in ArmorGA.playerTargets)
            {
                if (!conditionSystem.TestCondition(ArmorGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, target, null, ArmorGA.Actionner)) continue;
                if (ArmorGA.DynamicAmountInfo.DynamicAmount == DynamicAmount.TargetParam)
                {
                    ArmorAmount = TargetSystem.Instance.GetDynamicAmount(ArmorGA.DynamicAmountInfo, target, null, null);
                }
                target.TakeArmor(ArmorAmount * ArmorGA.multiplyAmount);
                yield return new WaitForSeconds(AnimDelay);
            }
        }

        if (ArmorGA.enemyTargets != null)
        {
            foreach (var target in ArmorGA.enemyTargets)
            {
                if (!conditionSystem.TestCondition(ArmorGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, null, target, ArmorGA.Actionner)) continue;
                if (ArmorGA.DynamicAmountInfo.DynamicAmount == DynamicAmount.TargetParam)
                {
                    ArmorAmount = TargetSystem.Instance.GetDynamicAmount(ArmorGA.DynamicAmountInfo, null, target, null);
                }
                target.TakeArmor(ArmorAmount * ArmorGA.multiplyAmount);
                yield return new WaitForSeconds(AnimDelay);
            }
        }
    }

    private IEnumerator DealShieldPerformer(ShieldGA shieldGA)
    {
        if (shieldGA.playerTargets != null)
        {
            if (shieldGA.Actionner.GetComponent<PermanentView>() != null)
            {
                foreach (var target in shieldGA.playerTargets)
                {
                    if (!conditionSystem.TestCondition(shieldGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, target, null, shieldGA.Actionner)) continue;
                    target.TakeShield(shieldGA.Actionner.GetComponent<PermanentView>(), null);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
            else if (shieldGA.Actionner.GetComponent<EnemySlotView>() != null)
            {
                foreach (var target in shieldGA.playerTargets)
                {
                    if (!conditionSystem.TestCondition(shieldGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, target, null, shieldGA.Actionner)) continue;
                    target.TakeShield(null, shieldGA.Actionner.GetComponent<EnemySlotView>());
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }

        if (shieldGA.enemyTargets != null)
        {
            if (shieldGA.Actionner.GetComponent<PermanentView>() != null)
            {
                foreach (var target in shieldGA.enemyTargets)
                {
                    if (!conditionSystem.TestCondition(shieldGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, null, target, shieldGA.Actionner)) continue;
                    target.TakeShield(shieldGA.Actionner.GetComponent<PermanentView>(), null);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
            else if (shieldGA.Actionner.GetComponent<EnemySlotView>() != null)
            {
                foreach (var target in shieldGA.enemyTargets)
                {
                    if (!conditionSystem.TestCondition(shieldGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, null, target, shieldGA.Actionner)) continue;
                    target.TakeShield(null, shieldGA.Actionner.GetComponent<EnemySlotView>());
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }
    }

    private IEnumerator LoseShieldPerformer(LoseShieldGA loseShieldGA)
    {
        if (loseShieldGA.PermanentView != null)
        {
            foreach (PermanentView perm in loseShieldGA.PermanentView.PlayerShielded)
            {
                if (perm != null)
                {
                    perm.RemoveShield(loseShieldGA.PermanentView, null);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
            foreach (EnemySlotView perm in loseShieldGA.PermanentView.EnemyShielded)
            {
                if (perm != null)
                {
                    perm.RemoveShield(loseShieldGA.PermanentView, null);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }

        if (loseShieldGA.EnemySlotView != null)
        {
            foreach (PermanentView perm in loseShieldGA.EnemySlotView.PlayerShielded)
            {
                if (perm != null)
                {
                    perm.RemoveShield(null, loseShieldGA.EnemySlotView);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
            foreach (EnemySlotView perm in loseShieldGA.EnemySlotView.EnemyShielded)
            {
                if (perm != null)
                {
                    perm.RemoveShield(null, loseShieldGA.EnemySlotView);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }
    }
    
    private IEnumerator UnShieldPerformer(UnShieldGA unShieldGA)
    {
        if (unShieldGA.playerTargets != null)
        {
            if (unShieldGA.Actionner.GetComponent<PermanentView>() != null)
            {
                foreach (var target in unShieldGA.playerTargets)
                {
                    if (!conditionSystem.TestCondition(unShieldGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, target, null, unShieldGA.Actionner)) continue;
                    target.UnShield(unShieldGA.Actionner.GetComponent<PermanentView>(), null);
                    yield return null;
                }
            }
            else if (unShieldGA.Actionner.GetComponent<EnemySlotView>() != null)
            {
                foreach (var target in unShieldGA.playerTargets)
                {
                    if (!conditionSystem.TestCondition(unShieldGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, target, null, unShieldGA.Actionner)) continue;
                    target.UnShield(null, unShieldGA.Actionner.GetComponent<EnemySlotView>());
                    yield return null;
                }
            } 
        }

        if (unShieldGA.enemyTargets != null)
        {
            if (unShieldGA.Actionner.GetComponent<PermanentView>() != null)
            {
                foreach (var target in unShieldGA.enemyTargets)
                {
                    if (!conditionSystem.TestCondition(unShieldGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, null, target, unShieldGA.Actionner)) continue;
                    target.UnShield(unShieldGA.Actionner.GetComponent<PermanentView>(), null);
                    yield return null;
                }
            }
            else if (unShieldGA.Actionner.GetComponent<EnemySlotView>() != null)
            {
                foreach (var target in unShieldGA.enemyTargets)
                {
                    if (!conditionSystem.TestCondition(unShieldGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, null, target, unShieldGA.Actionner)) continue;
                    target.UnShield(null, unShieldGA.Actionner.GetComponent<EnemySlotView>());
                    yield return null;
                }
            } 
        }
    }

    private IEnumerator RefreshPerformer(RefreshGA refreshGA)
    {
        if (refreshGA.playerTargets != null)
        {
            foreach (var target in refreshGA.playerTargets)
            {
                if (!conditionSystem.TestCondition(refreshGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, target, null, refreshGA.Actionner)) continue;
                target.Refresh();
                yield return null;
            }
        }

        if (refreshGA.enemyTargets != null)
        {
            foreach (var target in refreshGA.enemyTargets)
            {
                if (!conditionSystem.TestCondition(refreshGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, null, target, refreshGA.Actionner)) continue;
                target.Refresh();
                yield return null;
            }
        }
    }

    private IEnumerator DecountDecayPlayerPerformer(DecountPlayerDecayGA decountPlayerDecayGA)
    {
        foreach (PermanentView permanentView in CombatSystem.Instance.Player_Permanents)
        {
            var decayKeyword = permanentView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Decay);
            if (decayKeyword == null) continue;
            if (decayKeyword.keyWordValue > 0)
            {
                decayKeyword.keyWordValue--;
                if (decayKeyword.keyWordValue == 0)
                {
                    EventInfo eventInfo = new EventInfo(Events.OnSelfSacrifice, Enemy_Player_ENUM.Player, KeyWordType.NULL);
                    TriggerEventGA triggerEventGA = new(eventInfo, null, null, permanentView, null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);

                    DiePermanentGA diepermanentGA = new(permanentView.IsCore, permanentView.Durability, permanentView.CardReferenceArchive, permanentView);
                    ActionSystem.Instance.AddReaction(diepermanentGA);
                }
            }
        }
        yield return null;
    }

    private IEnumerator DecountDecayEnemyPerformer(DecountEnemyDecayGA decountEnemyDecayGA)
    {
        foreach (EnemySlotView EnemySlot in CombatSystem.Instance.Enemy_Permanents)
        {
            var decayKeyword = EnemySlot.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Decay);
            if (decayKeyword == null) continue;
            if (decayKeyword.keyWordValue > 0)
            {
                decayKeyword.keyWordValue--;
                if (decayKeyword.keyWordValue == 0)
                {
                    EventInfo eventInfo = new EventInfo(Events.OnSelfSacrifice, Enemy_Player_ENUM.Enemy, KeyWordType.NULL);
                    TriggerEventGA triggerEventGA = new(eventInfo, null, null, null, EnemySlot);
                    ActionSystem.Instance.AddReaction(triggerEventGA);
                    
                    DieEnemySlotGA dieEnemySlotGA = new(EnemySlot);
                    ActionSystem.Instance.AddReaction(dieEnemySlotGA);
                }
            }
        }
        yield return null;
    }

    private IEnumerator AlterPowerPerformer(AlterPowerGA alterPowerGA)
    {
        int AlterAmount = alterPowerGA.Amount;

        if (alterPowerGA.DynamicAmountInfo.DynamicAmount != DynamicAmount.NULL)
        {
            if (alterPowerGA.Actionner == null)
            {
                if (alterPowerGA.CardActionner != null)
                {
                    AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterPowerGA.DynamicAmountInfo, null, null, alterPowerGA.CardActionner);
                }
                else
                {
                    AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterPowerGA.DynamicAmountInfo, null, null);
                }
            }
            else if (alterPowerGA.Actionner.GetComponent<PermanentView>() != null)
            {
                AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterPowerGA.DynamicAmountInfo, alterPowerGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterPowerGA.DynamicAmountInfo, null, alterPowerGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        if (alterPowerGA.passive)
        {
            CombatSystem.Instance.AddPassive(alterPowerGA.Actionner ,AlterAmount * alterPowerGA.multiplyAmount, BasicParam.Power, alterPowerGA.targetModeInfo, alterPowerGA.SourceEffect.DynamicConditionInfos);
        }
        else
        {
            if (alterPowerGA.playerTargets != null)
            {
                foreach (var target in alterPowerGA.playerTargets)
                {
                    if (!conditionSystem.TestCondition(alterPowerGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, target, null, alterPowerGA.Actionner)) continue;
                    if (alterPowerGA.DynamicAmountInfo.DynamicAmount == DynamicAmount.TargetParam)
                    {
                        AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterPowerGA.DynamicAmountInfo, target, null, null);
                    }
                    alterPowerGA.Amount = AlterAmount * alterPowerGA.multiplyAmount;
                    target.TakeAlterPower(alterPowerGA);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }

            if (alterPowerGA.enemyTargets != null)
            {
                foreach (var target in alterPowerGA.enemyTargets)
                {
                    if (!conditionSystem.TestCondition(alterPowerGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, null, target, alterPowerGA.Actionner)) continue;
                    if (alterPowerGA.DynamicAmountInfo.DynamicAmount == DynamicAmount.TargetParam)
                    {
                        AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterPowerGA.DynamicAmountInfo, null, target, null);
                    }
                    alterPowerGA.Amount = AlterAmount * alterPowerGA.multiplyAmount;
                    target.TakeAlterPower(alterPowerGA);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }

            if (alterPowerGA.cardTargets != null)
            {
                foreach (var target in alterPowerGA.cardTargets)
                {
                    if (!conditionSystem.TestCondition(alterPowerGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, target, null, null, alterPowerGA.Actionner)) continue;
                    if (alterPowerGA.DynamicAmountInfo.DynamicAmount == DynamicAmount.TargetParam)
                    {
                        AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterPowerGA.DynamicAmountInfo, null, null, target);
                    }
                    target.TakeAlterPower(AlterAmount * alterPowerGA.multiplyAmount);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }
    }

    private IEnumerator AlterStamPerformer(AlterStaminaGA alterStaminaGA)
    {
        int AlterAmount = alterStaminaGA.Amount;

        if (alterStaminaGA.DynamicAmountInfo.DynamicAmount != DynamicAmount.NULL)
        {
            if (alterStaminaGA.Actionner == null)
            {
                if (alterStaminaGA.CardActionner != null)
                {
                    AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterStaminaGA.DynamicAmountInfo, null, null, alterStaminaGA.CardActionner);
                }
                else
                {
                    AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterStaminaGA.DynamicAmountInfo, null, null);
                }
            }
            else if (alterStaminaGA.Actionner.GetComponent<PermanentView>() != null)
            {
                AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterStaminaGA.DynamicAmountInfo, alterStaminaGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterStaminaGA.DynamicAmountInfo, null, alterStaminaGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        if (alterStaminaGA.passive)
        {
            CombatSystem.Instance.AddPassive(alterStaminaGA.Actionner, AlterAmount * alterStaminaGA.multiplyAmount, BasicParam.Durability, alterStaminaGA.targetModeInfo, alterStaminaGA.SourceEffect.DynamicConditionInfos);
        }
        else
        {
            if (alterStaminaGA.playerTargets != null)
            {
                foreach (var target in alterStaminaGA.playerTargets)
                {
                    if (!conditionSystem.TestCondition(alterStaminaGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, target, null, alterStaminaGA.Actionner)) continue;
                    if (alterStaminaGA.DynamicAmountInfo.DynamicAmount == DynamicAmount.TargetParam)
                    {
                        AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterStaminaGA.DynamicAmountInfo, target, null, null);
                    }
                    alterStaminaGA.Amount = AlterAmount * alterStaminaGA.multiplyAmount;
                    target.TakeAlterStamina(alterStaminaGA);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }            
            if (alterStaminaGA.cardTargets != null)
            {
                foreach (var target in alterStaminaGA.cardTargets)
                {
                    if (!conditionSystem.TestCondition(alterStaminaGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, target, null, null, alterStaminaGA.Actionner)) continue;
                    if (alterStaminaGA.DynamicAmountInfo.DynamicAmount == DynamicAmount.TargetParam)
                    {
                        AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterStaminaGA.DynamicAmountInfo, null, null, target);
                    }
                    target.TakeAlterStamina(AlterAmount * alterStaminaGA.multiplyAmount);
                    yield return new WaitForSeconds(AnimDelay);
                }                
            }
        }
    }

    private IEnumerator AlterCardCostPerformer(AlterCardCostGA alterCardCostGA)
    {
        int AlterAmount = alterCardCostGA.Amount;

        if (alterCardCostGA.DynamicAmountInfo.DynamicAmount != DynamicAmount.NULL)
        {
            if (alterCardCostGA.Actionner == null)
            {
                if (alterCardCostGA.CardActionner != null)
                {
                    AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterCardCostGA.DynamicAmountInfo, null, null, alterCardCostGA.CardActionner);
                }
                else
                {
                    AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterCardCostGA.DynamicAmountInfo, null, null);
                }
            }
            else if (alterCardCostGA.Actionner.GetComponent<PermanentView>() != null)
            {
                AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterCardCostGA.DynamicAmountInfo, alterCardCostGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterCardCostGA.DynamicAmountInfo, null, alterCardCostGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        if (alterCardCostGA.passive)
        {
            CombatSystem.Instance.AddPassive(alterCardCostGA.Actionner, AlterAmount * alterCardCostGA.multiplyAmount, BasicParam.Cost, alterCardCostGA.targetModeInfo, alterCardCostGA.SourceEffect.DynamicConditionInfos);
        }
        else
        {
            if (alterCardCostGA.cardTargets != null)
            {
                foreach (var target in alterCardCostGA.cardTargets)
                {
                    if (!conditionSystem.TestCondition(alterCardCostGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, target, null, null, alterCardCostGA.Actionner)) continue;
                    if (alterCardCostGA.DynamicAmountInfo.DynamicAmount == DynamicAmount.TargetParam)
                    {
                        AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterCardCostGA.DynamicAmountInfo, null, null, target);
                    }
                    target.TakeAlterCardCost(AlterAmount * alterCardCostGA.multiplyAmount);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }
        yield return null;
    }

    private IEnumerator GainLifePerformer(GainLifeGA gainLifeGA)
    {
        int LifeAmount = gainLifeGA.Amount;

        if (gainLifeGA.DynamicAmountInfo.DynamicAmount != DynamicAmount.NULL)
        {
            if (gainLifeGA.Actionner == null)
            {
                if (gainLifeGA.CardActionner != null)
                {
                    LifeAmount = TargetSystem.Instance.GetDynamicAmount(gainLifeGA.DynamicAmountInfo, null, null, gainLifeGA.CardActionner);
                }
                else
                {
                    LifeAmount = TargetSystem.Instance.GetDynamicAmount(gainLifeGA.DynamicAmountInfo, null, null);
                }
            }
            else if (gainLifeGA.Actionner.GetComponent<PermanentView>() != null)
            {
                LifeAmount = TargetSystem.Instance.GetDynamicAmount(gainLifeGA.DynamicAmountInfo, gainLifeGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                LifeAmount = TargetSystem.Instance.GetDynamicAmount(gainLifeGA.DynamicAmountInfo, null, gainLifeGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        if (gainLifeGA.passive)
        {
            CombatSystem.Instance.AddPassive(gainLifeGA.Actionner, LifeAmount * gainLifeGA.multiplyAmount, BasicParam.Life, gainLifeGA.targetModeInfo, gainLifeGA.SourceEffect.DynamicConditionInfos);
        }
        else
        {
            if (gainLifeGA.playerTargets != null)
            {
                foreach (var target in gainLifeGA.playerTargets)
                {
                    if (!conditionSystem.TestCondition(gainLifeGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, target, null, gainLifeGA.Actionner)) continue;
                    if (gainLifeGA.DynamicAmountInfo.DynamicAmount == DynamicAmount.TargetParam)
                    {
                        LifeAmount = TargetSystem.Instance.GetDynamicAmount(gainLifeGA.DynamicAmountInfo, target, null, null);
                    }
                    gainLifeGA.Amount = LifeAmount * gainLifeGA.multiplyAmount;
                    target.TakeAlterLife(gainLifeGA);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }

            if (gainLifeGA.enemyTargets != null)
            {
                foreach (var target in gainLifeGA.enemyTargets)
                {
                    if (!conditionSystem.TestCondition(gainLifeGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, null, target, gainLifeGA.Actionner)) continue;
                    if (gainLifeGA.DynamicAmountInfo.DynamicAmount == DynamicAmount.TargetParam)
                    {
                        LifeAmount = TargetSystem.Instance.GetDynamicAmount(gainLifeGA.DynamicAmountInfo, null, target, null);
                    }
                    gainLifeGA.Amount = LifeAmount * gainLifeGA.multiplyAmount;
                    target.TakeAlterLife(gainLifeGA);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }

            // à modifier pour permettre le gain de vie pour les cartes TODO
            /*if (gainLifeGA.cardTargets != null)
            {
                foreach (var target in gainLifeGA.cardTargets)
                {
                    if (!conditionSystem.TestCondition(gainLifeGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, null, target, gainLifeGA.Actionner)) continue;
                    if (gainLifeGA.DynamicAmountInfo.DynamicAmount == DynamicAmount.TargetParam)
                    {
                        LifeAmount = TargetSystem.Instance.GetDynamicAmount(gainLifeGA.DynamicAmountInfo, null, null, target);
                    }
                    gainLifeGA.Amount = LifeAmount * gainLifeGA.multiplyAmount;
                    target.TakeAlterLife(gainLifeGA);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }*/
        }
    }
    
    private IEnumerator AddACopyPerformer(AddACopyGa addACopyGa)
    {
        int CopyAmount = addACopyGa.Amount;

        // Vérifier le foncionnement de AddACopy TODO
        if (addACopyGa.DynamicAmountInfo.DynamicAmount != DynamicAmount.NULL)
        {
            if (addACopyGa.Actionner == null)
            {
                if (addACopyGa.CardActionner != null)
                {
                    CopyAmount = TargetSystem.Instance.GetDynamicAmount(addACopyGa.DynamicAmountInfo, null, null, addACopyGa.CardActionner);
                }
                else
                {
                    CopyAmount = TargetSystem.Instance.GetDynamicAmount(addACopyGa.DynamicAmountInfo, null, null);
                }
            }
            else if (addACopyGa.Actionner.GetComponent<PermanentView>() != null)
            {
                CopyAmount = TargetSystem.Instance.GetDynamicAmount(addACopyGa.DynamicAmountInfo, addACopyGa.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                CopyAmount = TargetSystem.Instance.GetDynamicAmount(addACopyGa.DynamicAmountInfo, null, addACopyGa.Actionner.GetComponent<EnemySlotView>());
            }
        }

        for (int i = 0; i < CopyAmount; i++)
        {
            //CombatSystem.Instance.AddPassive(CopyAmount * addACopyGa.multiplyAmount, addACopyGa.TypeOfCopy, addACopyGa.AffectedSide, addACopyGa.SourceEffect.DynamicConditionInfos);
        }
        
        yield return null;
    }

    private IEnumerator AlterPowerGridPerformer(AlterPowerGridGA alterPowerGridGA)
    {
        int AlterAmount = alterPowerGridGA.Amount;

        if (alterPowerGridGA.DynamicAmountInfo.DynamicAmount != DynamicAmount.NULL)
        {
            if (alterPowerGridGA.Actionner == null)
            {
                if (alterPowerGridGA.CardActionner != null)
                {
                    AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterPowerGridGA.DynamicAmountInfo, null, null, alterPowerGridGA.CardActionner);
                }
                else
                {
                    AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterPowerGridGA.DynamicAmountInfo, null, null);
                }
            }
            else if (alterPowerGridGA.Actionner.GetComponent<PermanentView>() != null)
            {
                AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterPowerGridGA.DynamicAmountInfo, alterPowerGridGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                AlterAmount = TargetSystem.Instance.GetDynamicAmount(alterPowerGridGA.DynamicAmountInfo, null, alterPowerGridGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        CombatSystem.Instance.MaxPowerGrid += AlterAmount * alterPowerGridGA.multiplyAmount;
        CombatSystem.Instance.UpdatePowerGridText();
        yield return null;
    }

    private IEnumerator LifeLossPerformer(LifeLossGA lifeLossGA)
    {
        int LifeAmount = lifeLossGA.Amount;

        if (lifeLossGA.DynamicAmountInfo.DynamicAmount != DynamicAmount.NULL)
        {
            if (lifeLossGA.Actionner == null)
            {
                if (lifeLossGA.CardActionner != null)
                {
                    LifeAmount = TargetSystem.Instance.GetDynamicAmount(lifeLossGA.DynamicAmountInfo, null, null, lifeLossGA.CardActionner);
                }
                else
                {
                    LifeAmount = TargetSystem.Instance.GetDynamicAmount(lifeLossGA.DynamicAmountInfo, null, null);
                }
            }
            else if (lifeLossGA.Actionner.GetComponent<PermanentView>() != null)
            {
                LifeAmount = TargetSystem.Instance.GetDynamicAmount(lifeLossGA.DynamicAmountInfo, lifeLossGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                LifeAmount = TargetSystem.Instance.GetDynamicAmount(lifeLossGA.DynamicAmountInfo, null, lifeLossGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        if (lifeLossGA.playerTargets != null)
        {
            foreach (var target in lifeLossGA.playerTargets)
            {
                if (!conditionSystem.TestCondition(lifeLossGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, target, null, lifeLossGA.SourceEffect.Actionner)) continue;
                if (lifeLossGA.DynamicAmountInfo.DynamicAmount == DynamicAmount.TargetParam)
                {
                    LifeAmount = TargetSystem.Instance.GetDynamicAmount(lifeLossGA.DynamicAmountInfo, target, null, null);
                }
                target.TakeLifeLoss(LifeAmount * lifeLossGA.multiplyAmount);
                yield return new WaitForSeconds(AnimDelay);
            }
        }

        if (lifeLossGA.enemyTargets != null)
        {
            foreach (var target in lifeLossGA.enemyTargets)
            {
                if (!conditionSystem.TestCondition(lifeLossGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, null, target, lifeLossGA.SourceEffect.Actionner)) continue;
                if (lifeLossGA.DynamicAmountInfo.DynamicAmount == DynamicAmount.TargetParam)
                {
                    LifeAmount = TargetSystem.Instance.GetDynamicAmount(lifeLossGA.DynamicAmountInfo, null, target, null);
                }
                target.TakeLifeLoss(LifeAmount * lifeLossGA.multiplyAmount);
                yield return new WaitForSeconds(AnimDelay);
            }
        }
    }

    private IEnumerator DisablePerformer(DisableGA disableGA)
    {
        if (disableGA.playerTargets != null)
        {
            foreach (var target in disableGA.playerTargets)
            {
                if (target != null)
                {
                    if (!conditionSystem.TestCondition(disableGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, target, null, disableGA.Actionner)) continue;
                    target.IsDisabled = true;
                    foreach (Effect effect in target.ToggleableEffects)
                    {
                        if (effect.Disabled == false)
                            ActionSystem.Instance.AddReaction(effect.GetCounterMesure());
                    }
                }
                yield return new WaitForSeconds(AnimDelay);
            }
        }

        if (disableGA.enemyTargets != null)
        {
            foreach (var target in disableGA.enemyTargets)
            {
                if (target != null)
                {
                    if (!conditionSystem.TestCondition(disableGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, null, target, disableGA.Actionner)) continue;
                    target.IsDisabled = true;
                    foreach (Effect effect in target.ToggleableEffects)
                    {
                        if (effect.Disabled == false)
                            ActionSystem.Instance.AddReaction(effect.GetCounterMesure());
                    }
                }
                yield return new WaitForSeconds(AnimDelay);
            }
        }
    }

    private IEnumerator EnablePerformer(EnableGA enableGA)
    {
        if (enableGA.playerTargets != null)
        {
            foreach (var target in enableGA.playerTargets)
            {
                if (target != null)
                {
                    if (!conditionSystem.TestCondition(enableGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, target, null, enableGA.Actionner)) continue;
                    target.IsDisabled = false;
                    foreach (Effect effect in target.ToggleableEffects)
                    {
                        if (effect.Disabled == true)
                            ActionSystem.Instance.AddReaction(effect.GetGameAction());
                    }
                }
                yield return new WaitForSeconds(AnimDelay);
            }
        }

        if (enableGA.enemyTargets != null)
        {
            foreach (var target in enableGA.enemyTargets)
            {
                if (target != null)
                {
                    if (!conditionSystem.TestCondition(enableGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, null, target, enableGA.Actionner)) continue;
                    target.IsDisabled = false;
                    foreach (Effect effect in target.ToggleableEffects)
                    {             
                        if (effect.Disabled == true)         
                            ActionSystem.Instance.AddReaction(effect.GetGameAction());
                    }
                }
                yield return new WaitForSeconds(AnimDelay);
            }
        }
    }

    private IEnumerator DiscardCardPerformer(DiscardCardGA discardCardGA)
    {
        List<CardView> cardViewsToDiscard = new();
        foreach (CardView cardView in discardCardGA.CardViews)
        {
            if (!conditionSystem.TestCondition(discardCardGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, cardView.Card, null, null, discardCardGA.Actionner)) continue;
            cardViewsToDiscard.Add(cardView);
        }

        DiscardOnceGA discardOnceGA = new(cardViewsToDiscard, true);
        ActionSystem.Instance.AddReaction(discardOnceGA);

        yield return null;
    }

    private IEnumerator ScryPerformer(ScryGA scryGA)
    {
        int ScryAmount = scryGA.Amount;

        if (scryGA.DynamicAmountInfo.DynamicAmount != DynamicAmount.NULL)
        {
            if (scryGA.Actionner == null)
            {
                if (scryGA.CardActionner != null)
                {
                    ScryAmount = TargetSystem.Instance.GetDynamicAmount(scryGA.DynamicAmountInfo, null, null, scryGA.CardActionner);
                }
                else
                {
                    ScryAmount = TargetSystem.Instance.GetDynamicAmount(scryGA.DynamicAmountInfo, null, null);
                }
            }
            else if (scryGA.Actionner.GetComponent<PermanentView>() != null)
            {
                ScryAmount = TargetSystem.Instance.GetDynamicAmount(scryGA.DynamicAmountInfo, scryGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                ScryAmount = TargetSystem.Instance.GetDynamicAmount(scryGA.DynamicAmountInfo, null, scryGA.Actionner.GetComponent<EnemySlotView>());
            }
        }
        
        List<Card> topCards = CardSystem.Instance.drawPile.TakeTop(ScryAmount * scryGA.multiplyAmount);
        if (topCards.Count == 0) yield break;

        CardSystem.Instance.ShowScryPanel(topCards);

        CardSystem.Instance.ScryScrollRect.enabled = false;

        yield return new WaitUntil(() => CardSystem.Instance.ScryCardViews.Count == 0);

        CardSystem.Instance.HideScryPanel();
        yield return null;
    }
    private IEnumerator InvocPerformer(InvocGA invocGA)
    {
        int InvocAmount = invocGA.Amount;

        if (invocGA.DynamicAmountInfo.DynamicAmount != DynamicAmount.NULL)
        {
            if (invocGA.Actionner == null)
            {
                if (invocGA.CardActionner != null)
                {
                    InvocAmount = TargetSystem.Instance.GetDynamicAmount(invocGA.DynamicAmountInfo, null, null, invocGA.CardActionner);
                }
                else
                {
                    InvocAmount = TargetSystem.Instance.GetDynamicAmount(invocGA.DynamicAmountInfo, null, null);
                }
            }
            else if (invocGA.Actionner.GetComponent<PermanentView>() != null)
            {
                InvocAmount = TargetSystem.Instance.GetDynamicAmount(invocGA.DynamicAmountInfo, invocGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                InvocAmount = TargetSystem.Instance.GetDynamicAmount(invocGA.DynamicAmountInfo, null, invocGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        if (invocGA.CardsToInvoc != null)
        {
            if (invocGA.CardsToInvoc.Count != 0)
            {
                for (int i = 0; i < InvocAmount * invocGA.multiplyAmount; i++)
                {
                    foreach (var item in invocGA.CardsToInvoc)
                    {
                        Card card = new(item);
                        PermanentView newPerm = PermanentViewCreator.Instance.CreatePermanentViewCreator(card, card.permanentArea);
                        
                        if (newPerm != null)
                        {
                            var InvocKeyword = newPerm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Invoc);
                            if (InvocKeyword == null)
                            {
                                KeyWord keyWord = new KeyWord(KeyWordType.Invoc,0);
                                newPerm.KeyWords.Add(keyWord);
                            }
                        }
                    }
                }
            }
        }

        if (invocGA.EnemyToInvoc != null)
        {
            if (invocGA.EnemyToInvoc.Count != 0)
            {
                for (int i = 0; i < InvocAmount * invocGA.multiplyAmount; i++)
                {
                    foreach (var item in invocGA.EnemyToInvoc)
                    {
                        EnemySlotView newEnemy = EnemySlotViewCreator.Instance.CreateEnemySlotViewCreator(item, item.permanentArea, false);
                        if (newEnemy != null)
                        {
                            var InvocKeyword = newEnemy.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Invoc);
                            if (InvocKeyword == null)
                            {
                                KeyWord keyWord = new KeyWord(KeyWordType.Invoc,0);
                                newEnemy.KeyWords.Add(keyWord);
                            }
                        }
                    }
                }
            }
        }

        yield return null;
    }

    private IEnumerator SacPerformer(SacGA sacGA)
    {
        if (sacGA.playerTargets != null)
        {
            foreach (var target in sacGA.playerTargets)
            {
                if (!conditionSystem.TestCondition(sacGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, target, null, sacGA.Actionner)) continue;

                EventInfo eventInfo = new EventInfo(Events.WhenPermaSac, Enemy_Player_ENUM.Player, KeyWordType.NULL);
                TriggerEventGA triggerEventGA = new(eventInfo, null, null, target, null);
                ActionSystem.Instance.AddReaction(triggerEventGA);

                eventInfo = new EventInfo(Events.OnSelfSacrifice, Enemy_Player_ENUM.Enemy, KeyWordType.NULL);
                triggerEventGA = new(eventInfo, null, null, target, null);
                ActionSystem.Instance.AddReaction(triggerEventGA);

                DiePermanentGA diePermanentGA = new(target.IsCore, target.Durability, target.CardReferenceArchive, target);
                ActionSystem.Instance.AddReaction(diePermanentGA);

                yield return new WaitForSeconds(AnimDelay);
            }
        }

        if (sacGA.enemyTargets != null)
        {
            foreach (var target in sacGA.enemyTargets)
            {
                if (!conditionSystem.TestCondition(sacGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, null, target, sacGA.Actionner)) continue;

                EventInfo eventInfo = new EventInfo(Events.WhenPermaSac, Enemy_Player_ENUM.Enemy, KeyWordType.NULL);
                TriggerEventGA triggerEventGA = new(eventInfo, null, null, null, target);
                ActionSystem.Instance.AddReaction(triggerEventGA);

                eventInfo = new EventInfo(Events.OnSelfSacrifice, Enemy_Player_ENUM.Enemy, KeyWordType.NULL);
                triggerEventGA = new(eventInfo, null, null, null, target);
                ActionSystem.Instance.AddReaction(triggerEventGA);

                DieEnemySlotGA dieEnemySlotGA = new(target);
                ActionSystem.Instance.AddReaction(dieEnemySlotGA);

                yield return new WaitForSeconds(AnimDelay);
            }
        }

        yield return null;
    }

    private IEnumerator ExhaustedPerformer(ExhaustGA exhaustGA)
    {
        if (exhaustGA.playerTargets != null)
        {
            foreach (var perm in exhaustGA.playerTargets)
            {
                if (!conditionSystem.TestCondition(exhaustGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, null, perm, null, exhaustGA.Actionner)) continue;
                DiePermanentGA diePermanentGA = new(perm.IsCore, 0, perm.CardReferenceArchive, perm);
                ActionSystem.Instance.AddReaction(diePermanentGA);                
            }            
        }

        if (exhaustGA.cardTargets != null)
        {
            CardSystem cardsystem = CardSystem.Instance;
            foreach (var card in exhaustGA.cardTargets)
            {
                if (!conditionSystem.TestCondition(exhaustGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, card, null, null, exhaustGA.Actionner)) continue;
                if (!card.IsSpell)
                {
                    card.Durability = 0;
                }
                cardsystem.hand.Remove(card);
                CardView cardView = CardSystem.Instance.handView.RemoveCard(card);
                cardsystem.ExhaustPile.Add(card);

                StartCoroutine(cardsystem.DestroyCard(cardView));

                yield return new WaitForSeconds(AnimDelay);
            }
        }
    }

    private IEnumerator RetrieveExhaustedPerformer(RetrieveExhaustedGA retrieveExhaustedGA)
    {
        if (retrieveExhaustedGA.cardTargets != null)
        {
            foreach (var card in retrieveExhaustedGA.cardTargets)
            {
                if (!conditionSystem.TestCondition(retrieveExhaustedGA.SourceEffect.DynamicConditionInfos, null, null, null, null, null, null, true, card, null, null, retrieveExhaustedGA.Actionner)) continue;
                CardSystem.Instance.ExhaustPile.Remove(card);
                int randomIndex = Random.Range(0, CardSystem.Instance.drawPile.Count + 1);
                CardSystem.Instance.drawPile.Insert(randomIndex, card);

                if (!card.IsSpell)
                {
                    card.Durability = card.MaxDurability;
                }

                EventInfo eventInfo = new EventInfo(Events.WhenCardExitExhaust, Enemy_Player_ENUM.Player, KeyWordType.NULL);
                TriggerEventGA triggerEventGA = new(eventInfo, null, card, null, null);
                ActionSystem.Instance.AddReaction(triggerEventGA);

                yield return new WaitForSeconds(AnimDelay);
            }
        }
    }

    private IEnumerator PlayerChoicePerformer(LetChoiceGA letChoiceGA)
    {
        CardSystem.Instance.ShowChoicePanel(letChoiceGA.ChoicesEffects, letChoiceGA.OnSelectMode);
        if (letChoiceGA.MayChoice)
        {
            CardSystem.Instance.ShowMonoChoiceButtons();       
        }
        CardSystem.Instance.EffectChoosed = null;

        yield return new WaitUntil(() => CardSystem.Instance.EffectChoosed != null);

        CardSystem.Instance.HideMonoChoiceButtons();
        CardSystem.Instance.HideChoicePanel();

        if (CardSystem.Instance.EffectChoosed is ZZZ_EmptyEffect)
        {
            yield break;
        }

        Effect EffectToManage = CardSystem.Instance.EffectChoosed;

        if (EffectToManage.ORChoice)
        {
            foreach (Effect effect in letChoiceGA.ChoicesEffects)
            {
                if (effect.ORChoice)
                {
                    effect.ActivateLeft = 0;
                }
            }
        }

        PermanentView permanentView = null;
        EnemySlotView enemySlotView = null;
        if (EffectToManage.Actionner != null)
        {
            if (EffectToManage.Actionner.GetComponent<PermanentView>() != null)
            {
                permanentView = EffectToManage.Actionner.GetComponent<PermanentView>();
            }
            if (EffectToManage.Actionner.GetComponent<EnemySlotView>() != null)
            {
                enemySlotView = EffectToManage.Actionner.GetComponent<EnemySlotView>();
            }
        }

        if (letChoiceGA.OnSelectMode && EffectToManage.ActivateLeft != 999 && EffectToManage.ActivateLeft > 0)
        {
            EffectToManage.ActivateLeft--;
        }

        if (letChoiceGA.OnSelectMode)
        {
            if (EffectToManage is EffectGroup choiceEffect)
            {
                foreach (Effect SubEffect in choiceEffect.EffectGroups)
                {
                    SubEffect.Actionner = EffectToManage.Actionner;
                    SubEffect.CardActionner = EffectToManage.CardActionner;
                    GameEventSystem.Instance.RegisterEffect(SubEffect);
                }
            }
            else
            {
                if (EffectToManage.ActivateLeft >= 0)
                {
                    if (EffectToManage.EventInfos.Count == 1)
                    {
                        Effect effectToExecute = EffectToManage.Clone();
                        effectToExecute.EventInfos = new List<EventInfo> {new EventInfo(Events.Instant, Enemy_Player_ENUM.NULL, KeyWordType.NULL)} ;
                        GameEventSystem.Instance.RegisterEffect(effectToExecute);
                    }
                    else
                    {
                        Effect effectToExecute = EffectToManage.Clone();
                        for (int i = 0; i < effectToExecute.EventInfos.Count; i++)
                        {
                            if (effectToExecute.EventInfos[i].Events == Events.OnSelect)
                            {
                                effectToExecute.EventInfos.Remove(effectToExecute.EventInfos[i]);
                            }                                
                        }
                        GameEventSystem.Instance.RegisterEffect(effectToExecute);    
                    }
                }
            }
        }
        else
        {
            if (EffectToManage is EffectGroup choiceEffect)
            {
                foreach (Effect SubEffect in choiceEffect.EffectGroups)
                {
                    SubEffect.Actionner = EffectToManage.Actionner;
                    SubEffect.CardActionner = EffectToManage.CardActionner;
                    GameEventSystem.Instance.RegisterEffect(SubEffect);
                }
            }
            else
            {
                GameEventSystem.Instance.RegisterEffect(EffectToManage);
            }
        }
    } 
}
