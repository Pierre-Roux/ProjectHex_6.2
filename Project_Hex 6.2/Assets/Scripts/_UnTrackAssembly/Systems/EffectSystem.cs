using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectSystem : Singleton<EffectSystem>
{
    public float AnimDelay = 0.25f;
    //public GameObject EffectDisplayCardView;

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

    private IEnumerator DealDamagePerformer(DealDamageGA dealDamageGA)
    {
        if (dealDamageGA.DynamicAmount != DynamicAmount.NULL)
        {
            if (dealDamageGA.Actionner == null)
            {
                if (dealDamageGA.CardActionner != null)
                {
                    dealDamageGA.Amount = TargetSystem.Instance.GetDynamicAmount(dealDamageGA.DynamicAmount, null, null, dealDamageGA.CardActionner);
                }
                else
                {
                    dealDamageGA.Amount = TargetSystem.Instance.GetDynamicAmount(dealDamageGA.DynamicAmount, null, null);
                }
            }
            else if (dealDamageGA.Actionner.GetComponent<PermanentView>() != null)
            {
                dealDamageGA.Amount = TargetSystem.Instance.GetDynamicAmount(dealDamageGA.DynamicAmount, dealDamageGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                dealDamageGA.Amount = TargetSystem.Instance.GetDynamicAmount(dealDamageGA.DynamicAmount, null, dealDamageGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        dealDamageGA.Amount = dealDamageGA.Amount * dealDamageGA.multiplyAmount;

        dealDamageGA.Amount += dealDamageGA.BonusAmount;

        if (dealDamageGA.playerTargets != null)
        {
            foreach (PermanentView target in dealDamageGA.playerTargets)
            {
                if (target.Shielded)
                {
                    if (target.PlayerShielder.Count != 0 && target.EnemyShielder.Count != 0)
                    {
                        var newtargetP = target.PlayerShielder[Random.Range(0, target.PlayerShielder.Count)];
                        var newtargetE = target.EnemyShielder[Random.Range(0, target.EnemyShielder.Count)];
                        if (Random.Range(0, 1) == 0)
                        {
                            newtargetP.TakeDamage(dealDamageGA.Amount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                        }
                        else
                        {
                            newtargetE.TakeDamage(dealDamageGA.Amount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                        }
                    }
                    else if (target.EnemyShielder.Count != 0)
                    {
                        var newtargetE = target.EnemyShielder[Random.Range(0, target.EnemyShielder.Count)];
                        newtargetE.TakeDamage(dealDamageGA.Amount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                    }
                    else if (target.PlayerShielder.Count != 0)
                    {
                        var newtargetP = target.PlayerShielder[Random.Range(0, target.PlayerShielder.Count)];
                        newtargetP.TakeDamage(dealDamageGA.Amount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                    }
                    yield return new WaitForSeconds(AnimDelay);
                }
                else
                {
                    target.TakeDamage(dealDamageGA.Amount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }

        if (dealDamageGA.enemyTargets != null)
        {
            foreach (EnemySlotView target in dealDamageGA.enemyTargets)
            {
                if (target.Shielded)
                {
                    if (target.PlayerShielder.Count != 0 && target.EnemyShielder.Count != 0)
                    {
                        var newtargetP = target.PlayerShielder[Random.Range(0, target.PlayerShielder.Count)];
                        var newtargetE = target.EnemyShielder[Random.Range(0, target.EnemyShielder.Count)];
                        if (Random.Range(0, 1) == 0)
                        {
                            newtargetP.TakeDamage(dealDamageGA.Amount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                        }
                        else
                        {
                            newtargetE.TakeDamage(dealDamageGA.Amount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                        }
                    }
                    else if (target.EnemyShielder.Count != 0)
                    {
                        var newtargetE = target.EnemyShielder[Random.Range(0, target.EnemyShielder.Count)];
                        newtargetE.TakeDamage(dealDamageGA.Amount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                    }
                    else if (target.PlayerShielder.Count != 0)
                    {
                        var newtargetP = target.PlayerShielder[Random.Range(0, target.PlayerShielder.Count)];
                        newtargetP.TakeDamage(dealDamageGA.Amount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                    }
                    yield return new WaitForSeconds(AnimDelay);
                }
                else
                {
                    target.TakeDamage(dealDamageGA.Amount, dealDamageGA.CardActionner, dealDamageGA.Actionner);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }
    }

    private IEnumerator DealHealPerformer(HealGA healGA)
    {
        if (healGA.DynamicAmount != DynamicAmount.NULL)
        {
            if (healGA.Actionner == null)
            {
                if (healGA.CardActionner != null)
                {
                    healGA.Amount = TargetSystem.Instance.GetDynamicAmount(healGA.DynamicAmount, null, null, healGA.CardActionner);
                }
                else
                {
                    healGA.Amount = TargetSystem.Instance.GetDynamicAmount(healGA.DynamicAmount, null, null);
                }
            }
            else if (healGA.Actionner.GetComponent<PermanentView>() != null)
            {
                healGA.Amount = TargetSystem.Instance.GetDynamicAmount(healGA.DynamicAmount, healGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                healGA.Amount = TargetSystem.Instance.GetDynamicAmount(healGA.DynamicAmount, null, healGA.Actionner.GetComponent<EnemySlotView>());
            }
        }
        
        healGA.Amount = healGA.Amount * healGA.multiplyAmount;

        if (healGA.playerTargets != null)
        {
            foreach (var target in healGA.playerTargets)
            {
                target.TakeHeal(healGA.Amount);
                yield return new WaitForSeconds(AnimDelay);
            }
        }

        if (healGA.enemyTargets != null)
        {
            foreach (var target in healGA.enemyTargets)
            {
                target.TakeHeal(healGA.Amount);
                yield return new WaitForSeconds(AnimDelay);
            }
        }
    }

    private IEnumerator DealArmorPerformer(ArmorGA ArmorGA)
    {
        if (ArmorGA.DynamicAmount != DynamicAmount.NULL)
        {
            if (ArmorGA.Actionner == null)
            {
                if (ArmorGA.CardActionner != null)
                {
                    ArmorGA.Amount = TargetSystem.Instance.GetDynamicAmount(ArmorGA.DynamicAmount, null, null, ArmorGA.CardActionner);
                }
                else
                {
                    ArmorGA.Amount = TargetSystem.Instance.GetDynamicAmount(ArmorGA.DynamicAmount, null, null);
                }
            }
            else if (ArmorGA.Actionner.GetComponent<PermanentView>() != null)
            {
                ArmorGA.Amount = TargetSystem.Instance.GetDynamicAmount(ArmorGA.DynamicAmount, ArmorGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                ArmorGA.Amount = TargetSystem.Instance.GetDynamicAmount(ArmorGA.DynamicAmount, null, ArmorGA.Actionner.GetComponent<EnemySlotView>());
            }
        }
        
        ArmorGA.Amount = ArmorGA.Amount * ArmorGA.multiplyAmount;

        if (ArmorGA.playerTargets != null)
        {
            foreach (var target in ArmorGA.playerTargets)
            {
                target.TakeArmor(ArmorGA.Amount);
                yield return new WaitForSeconds(AnimDelay);
            }
        }

        if (ArmorGA.enemyTargets != null)
        {
            foreach (var target in ArmorGA.enemyTargets)
            {
                target.TakeArmor(ArmorGA.Amount);
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
                    target.TakeShield(shieldGA.Actionner.GetComponent<PermanentView>(), null);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
            else if (shieldGA.Actionner.GetComponent<EnemySlotView>() != null)
            {
                foreach (var target in shieldGA.playerTargets)
                {
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
                    target.TakeShield(shieldGA.Actionner.GetComponent<PermanentView>(), null);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
            else if (shieldGA.Actionner.GetComponent<EnemySlotView>() != null)
            {
                foreach (var target in shieldGA.enemyTargets)
                {
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
                    target.UnShield(unShieldGA.Actionner.GetComponent<PermanentView>(), null);
                    Debug.Log(unShieldGA.Actionner.GetComponent<PermanentView>() + " UnShield " + target);
                    yield return null;
                }
            }
            else if (unShieldGA.Actionner.GetComponent<EnemySlotView>() != null)
            {
                foreach (var target in unShieldGA.playerTargets)
                {
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
                    target.UnShield(unShieldGA.Actionner.GetComponent<PermanentView>(), null);
                    yield return null;
                }
            }
            else if (unShieldGA.Actionner.GetComponent<EnemySlotView>() != null)
            {
                foreach (var target in unShieldGA.enemyTargets)
                {
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
                target.Refresh();
                yield return null;
            }
        }

        if (refreshGA.enemyTargets != null)
        {
            foreach (var target in refreshGA.enemyTargets)
            {
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
                    TriggerEventGA triggerEventGA = new(Events.OnSacrifice,null,permanentView,null);
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
                    DieEnemySlotGA dieEnemySlotGA = new(EnemySlot);
                    ActionSystem.Instance.AddReaction(dieEnemySlotGA);
                }
            }
        }
        yield return null;
    }

    private IEnumerator AlterPowerPerformer(AlterPowerGA alterPowerGA)
    {
        if (alterPowerGA.DynamicAmount != DynamicAmount.NULL)
        {
            if (alterPowerGA.Actionner == null)
            {
                if (alterPowerGA.CardActionner != null)
                {
                    alterPowerGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterPowerGA.DynamicAmount, null, null, alterPowerGA.CardActionner);
                }
                else
                {
                    alterPowerGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterPowerGA.DynamicAmount, null, null);
                }
            }
            else if (alterPowerGA.Actionner.GetComponent<PermanentView>() != null)
            {
                alterPowerGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterPowerGA.DynamicAmount, alterPowerGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                alterPowerGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterPowerGA.DynamicAmount, null, alterPowerGA.Actionner.GetComponent<EnemySlotView>());
            }
        }
        
        alterPowerGA.Amount = alterPowerGA.Amount * alterPowerGA.multiplyAmount;

        if (alterPowerGA.passive)
        {
            KeyWordType type = alterPowerGA.targetModeInfo.keyWordType;
            var side = alterPowerGA.targetModeInfo.PlayerOrEnemy;

            CombatSystem.Instance.AddPower(type, side, alterPowerGA.Amount);

            foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
            {
                // Update l'afichage pour les permanents
            }

            foreach (EnemySlotView item in CombatSystem.Instance.Enemy_Permanents)
            {
                item.UpdateIntentText(item.IntentAction);
            }
        }
        else
        {
            if (alterPowerGA.playerTargets != null)
            {
                foreach (var target in alterPowerGA.playerTargets)
                {
                    target.TakeAlterPower(alterPowerGA);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }

            if (alterPowerGA.enemyTargets != null)
            {
                foreach (var target in alterPowerGA.enemyTargets)
                {
                    target.TakeAlterPower(alterPowerGA);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }
    }

    private IEnumerator AlterStamPerformer(AlterStaminaGA alterStaminaGA)
    {
        if (alterStaminaGA.DynamicAmount != DynamicAmount.NULL)
        {
            if (alterStaminaGA.Actionner == null)
            {
                if (alterStaminaGA.CardActionner != null)
                {
                    alterStaminaGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterStaminaGA.DynamicAmount, null, null, alterStaminaGA.CardActionner);
                }
                else
                {
                    alterStaminaGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterStaminaGA.DynamicAmount, null, null);
                }
            }
            else if (alterStaminaGA.Actionner.GetComponent<PermanentView>() != null)
            {
                alterStaminaGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterStaminaGA.DynamicAmount, alterStaminaGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                alterStaminaGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterStaminaGA.DynamicAmount, null, alterStaminaGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        alterStaminaGA.Amount = alterStaminaGA.Amount * alterStaminaGA.multiplyAmount;

        if (alterStaminaGA.passive)
        {
            KeyWordType type = alterStaminaGA.targetModeInfo.keyWordType;
            var side = alterStaminaGA.targetModeInfo.PlayerOrEnemy;

            CombatSystem.Instance.AddStam(type, side, alterStaminaGA.Amount);

            foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
            {
                item.UpdateStam();
            }
        }
        else
        {
            if (alterStaminaGA.playerTargets != null)
            {
                foreach (var target in alterStaminaGA.playerTargets)
                {
                    target.TakeAlterStamina(alterStaminaGA);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }            
            if (alterStaminaGA.cardTargets != null)
            {
                foreach (var target in alterStaminaGA.cardTargets)
                {
                    target.TakeAlterStamina(alterStaminaGA.Amount);
                    yield return new WaitForSeconds(AnimDelay);
                }                
            }
        }
    }

    private IEnumerator AlterCardCostPerformer(AlterCardCostGA alterCardCostGA)
    {
        if (alterCardCostGA.DynamicAmount != DynamicAmount.NULL)
        {
            if (alterCardCostGA.Actionner == null)
            {
                if (alterCardCostGA.CardActionner != null)
                {
                    alterCardCostGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterCardCostGA.DynamicAmount, null, null, alterCardCostGA.CardActionner);
                }
                else
                {
                    alterCardCostGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterCardCostGA.DynamicAmount, null, null);
                }
            }
            else if (alterCardCostGA.Actionner.GetComponent<PermanentView>() != null)
            {
                alterCardCostGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterCardCostGA.DynamicAmount, alterCardCostGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                alterCardCostGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterCardCostGA.DynamicAmount, null, alterCardCostGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        alterCardCostGA.Amount = alterCardCostGA.Amount * alterCardCostGA.multiplyAmount;

        if (alterCardCostGA.passive)
        {
            KeyWordType type = alterCardCostGA.targetModeInfo.keyWordType;
            var side = alterCardCostGA.targetModeInfo.PlayerOrEnemy;

            CombatSystem.Instance.AddCost(type, side, alterCardCostGA.Amount);

            foreach (Card item in CardSystem.Instance.hand)
            {
                item.UpdateCost(CalculateCardPassiveCost());
            }
            foreach (Card item in CardSystem.Instance.discardPile)
            {
                item.UpdateCost(CalculateCardPassiveCost());
            }
            foreach (Card item in CardSystem.Instance.drawPile)
            {
                item.UpdateCost(CalculateCardPassiveCost());
            }
        }
        else
        {
            if (alterCardCostGA.cardTargets != null)
            {
                foreach (var target in alterCardCostGA.cardTargets)
                {
                    target.TakeAlterCardCost(alterCardCostGA.Amount);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }
    }
    
    public int CalculateCardPassiveCost()
    {
        if (CombatSystem.Instance != null)
        {
            int passiveBonus = 0;
            /*foreach (var keyWord in KeyWords)
            {
                passiveBonus += CombatSystem.Instance.GetCost(keyWord.keyWordType, Enemy_Player_ENUM.NULL);
            }*/

            // Bonus globaux (NULL)
            passiveBonus += CombatSystem.Instance.GetCost(KeyWordType.NULL, Enemy_Player_ENUM.NULL);

            return passiveBonus;
        }
        else
        {
            return 0;
        }
    }
    
    private IEnumerator AddACopyPerformer(AddACopyGa addACopyGa)
    {
        if (addACopyGa.DynamicAmount != DynamicAmount.NULL)
        {
            if (addACopyGa.Actionner == null)
            {
                if (addACopyGa.CardActionner != null)
                {
                    addACopyGa.Amount = TargetSystem.Instance.GetDynamicAmount(addACopyGa.DynamicAmount, null, null, addACopyGa.CardActionner);
                }
                else
                {
                    addACopyGa.Amount = TargetSystem.Instance.GetDynamicAmount(addACopyGa.DynamicAmount, null, null);
                }
            }
            else if (addACopyGa.Actionner.GetComponent<PermanentView>() != null)
            {
                addACopyGa.Amount = TargetSystem.Instance.GetDynamicAmount(addACopyGa.DynamicAmount, addACopyGa.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                addACopyGa.Amount = TargetSystem.Instance.GetDynamicAmount(addACopyGa.DynamicAmount, null, addACopyGa.Actionner.GetComponent<EnemySlotView>());
            }
        }

        addACopyGa.Amount = addACopyGa.Amount * addACopyGa.multiplyAmount;

        for (int i = 0; i < addACopyGa.Amount; i++)
        {
            CombatSystem.Instance.AddCopyValue(addACopyGa.TypeOfCopy, addACopyGa.AffectedSide, addACopyGa.Amount, addACopyGa.ConditionToCopy);
        }
        
        yield return null;
    }

    private IEnumerator AlterPowerGridPerformer(AlterPowerGridGA alterPowerGridGA)
    {
        if (alterPowerGridGA.DynamicAmount != DynamicAmount.NULL)
        {
            if (alterPowerGridGA.Actionner == null)
            {
                if (alterPowerGridGA.CardActionner != null)
                {
                    alterPowerGridGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterPowerGridGA.DynamicAmount, null, null, alterPowerGridGA.CardActionner);
                }
                else
                {
                    alterPowerGridGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterPowerGridGA.DynamicAmount, null, null);
                }
            }
            else if (alterPowerGridGA.Actionner.GetComponent<PermanentView>() != null)
            {
                alterPowerGridGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterPowerGridGA.DynamicAmount, alterPowerGridGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                alterPowerGridGA.Amount = TargetSystem.Instance.GetDynamicAmount(alterPowerGridGA.DynamicAmount, null, alterPowerGridGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        alterPowerGridGA.Amount = alterPowerGridGA.Amount * alterPowerGridGA.multiplyAmount;

        CombatSystem.Instance.MaxPowerGrid += alterPowerGridGA.Amount;
        CombatSystem.Instance.UpdatePowerGridText();
        yield return null;
    }

    private IEnumerator LifeLossPerformer(LifeLossGA lifeLossGA)
    {
        if (lifeLossGA.DynamicAmount != DynamicAmount.NULL)
        {
            if (lifeLossGA.Actionner == null)
            {
                if (lifeLossGA.CardActionner != null)
                {
                    lifeLossGA.Amount = TargetSystem.Instance.GetDynamicAmount(lifeLossGA.DynamicAmount, null, null, lifeLossGA.CardActionner);
                }
                else
                {
                    lifeLossGA.Amount = TargetSystem.Instance.GetDynamicAmount(lifeLossGA.DynamicAmount, null, null);
                }
            }
            else if (lifeLossGA.Actionner.GetComponent<PermanentView>() != null)
            {
                lifeLossGA.Amount = TargetSystem.Instance.GetDynamicAmount(lifeLossGA.DynamicAmount, lifeLossGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                lifeLossGA.Amount = TargetSystem.Instance.GetDynamicAmount(lifeLossGA.DynamicAmount, null, lifeLossGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        lifeLossGA.Amount = lifeLossGA.Amount * lifeLossGA.multiplyAmount;

        if (lifeLossGA.playerTargets != null)
        {
            foreach (var target in lifeLossGA.playerTargets)
            {
                target.TakeLifeLoss(lifeLossGA.Amount);
                yield return new WaitForSeconds(AnimDelay);
            }
        }

        if (lifeLossGA.enemyTargets != null)
        {
            foreach (var target in lifeLossGA.enemyTargets)
            {
                target.TakeLifeLoss(lifeLossGA.Amount);
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
                target.IsDisabled = true;
                if (target != null)
                {
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
                target.IsDisabled = true;
                if (target != null)
                {
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
                target.IsDisabled = false;
                if (target != null)
                {
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
                target.IsDisabled = false;
                if (target != null)
                {
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
            cardViewsToDiscard.Add(cardView);
        }

        DiscardOnceGA discardOnceGA = new(cardViewsToDiscard, true);
        ActionSystem.Instance.AddReaction(discardOnceGA);

        yield return null;
    }

    private IEnumerator ScryPerformer(ScryGA scryGA)
    {
        if (scryGA.DynamicAmount != DynamicAmount.NULL)
        {
            if (scryGA.Actionner == null)
            {
                if (scryGA.CardActionner != null)
                {
                    scryGA.Amount = TargetSystem.Instance.GetDynamicAmount(scryGA.DynamicAmount, null, null, scryGA.CardActionner);
                }
                else
                {
                    scryGA.Amount = TargetSystem.Instance.GetDynamicAmount(scryGA.DynamicAmount, null, null);
                }
            }
            else if (scryGA.Actionner.GetComponent<PermanentView>() != null)
            {
                scryGA.Amount = TargetSystem.Instance.GetDynamicAmount(scryGA.DynamicAmount, scryGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                scryGA.Amount = TargetSystem.Instance.GetDynamicAmount(scryGA.DynamicAmount, null, scryGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        scryGA.Amount = scryGA.Amount * scryGA.multiplyAmount;
        
        List<Card> topCards = CardSystem.Instance.drawPile.TakeTop(scryGA.Amount);
        if (topCards.Count == 0) yield break;

        CardSystem.Instance.ShowScryPanel(topCards);

        CardSystem.Instance.ScryScrollRect.enabled = false;

        yield return new WaitUntil(() => CardSystem.Instance.ScryCardViews.Count == 0);

        CardSystem.Instance.HideScryPanel();
        yield return null;
    }

    private IEnumerator GainLifePerformer(GainLifeGA gainLifeGA)
    {
        if (gainLifeGA.DynamicAmount != DynamicAmount.NULL)
        {
            if (gainLifeGA.Actionner == null)
            {
                if (gainLifeGA.CardActionner != null)
                {
                    gainLifeGA.Amount = TargetSystem.Instance.GetDynamicAmount(gainLifeGA.DynamicAmount, null, null, gainLifeGA.CardActionner);
                }
                else
                {
                    gainLifeGA.Amount = TargetSystem.Instance.GetDynamicAmount(gainLifeGA.DynamicAmount, null, null);
                }
            }
            else if (gainLifeGA.Actionner.GetComponent<PermanentView>() != null)
            {
                gainLifeGA.Amount = TargetSystem.Instance.GetDynamicAmount(gainLifeGA.DynamicAmount, gainLifeGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                gainLifeGA.Amount = TargetSystem.Instance.GetDynamicAmount(gainLifeGA.DynamicAmount, null, gainLifeGA.Actionner.GetComponent<EnemySlotView>());
            }
        }
        
        gainLifeGA.Amount = gainLifeGA.Amount * gainLifeGA.multiplyAmount;

        if (gainLifeGA.passive)
        {
            KeyWordType type = gainLifeGA.targetModeInfo.keyWordType;
            var side = gainLifeGA.targetModeInfo.PlayerOrEnemy;

            CombatSystem.Instance.AddHP(type, side, gainLifeGA.Amount);

            foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
            {
                item.UpdateLife();
            }

            foreach (EnemySlotView item in CombatSystem.Instance.Enemy_Permanents)
            {
                item.UpdateLife();
            }
        }
        else
        {
            if (gainLifeGA.playerTargets != null)
            {
                foreach (var target in gainLifeGA.playerTargets)
                {
                    target.TakeAlterLife(gainLifeGA);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }

            if (gainLifeGA.enemyTargets != null)
            {
                foreach (var target in gainLifeGA.enemyTargets)
                {
                    target.TakeAlterLife(gainLifeGA);
                    yield return new WaitForSeconds(AnimDelay);
                }
            }
        }
    }
    private IEnumerator InvocPerformer(InvocGA invocGA)
    {
        if (invocGA.DynamicAmount != DynamicAmount.NULL)
        {
            if (invocGA.Actionner == null)
            {
                if (invocGA.CardActionner != null)
                {
                    invocGA.Amount = TargetSystem.Instance.GetDynamicAmount(invocGA.DynamicAmount, null, null, invocGA.CardActionner);
                }
                else
                {
                    invocGA.Amount = TargetSystem.Instance.GetDynamicAmount(invocGA.DynamicAmount, null, null);
                }
            }
            else if (invocGA.Actionner.GetComponent<PermanentView>() != null)
            {
                invocGA.Amount = TargetSystem.Instance.GetDynamicAmount(invocGA.DynamicAmount, invocGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                invocGA.Amount = TargetSystem.Instance.GetDynamicAmount(invocGA.DynamicAmount, null, invocGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        invocGA.Amount = invocGA.Amount * invocGA.multiplyAmount;

        if (invocGA.CardsToInvoc != null)
        {
            if (invocGA.CardsToInvoc.Count != 0)
            {
                for (int i = 0; i < invocGA.Amount; i++)
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
                for (int i = 0; i < invocGA.Amount; i++)
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
                TriggerEventGA triggerEventGA = new(Events.WhenPermaSac, null, target, null);
                ActionSystem.Instance.AddReaction(triggerEventGA);

                triggerEventGA = new(Events.OnSacrifice, null, target, null);
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
                TriggerEventGA triggerEventGA = new(Events.WhenPermaSac, null, null, target);
                ActionSystem.Instance.AddReaction(triggerEventGA);

                triggerEventGA = new(Events.OnSacrifice, null, null, target);
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
                DiePermanentGA diePermanentGA = new(perm.IsCore, 0, perm.CardReferenceArchive, perm);
                ActionSystem.Instance.AddReaction(diePermanentGA);                
            }            
        }

        if (exhaustGA.cardTargets != null)
        {
            CardSystem cardsystem = CardSystem.Instance;
            foreach (var card in exhaustGA.cardTargets)
            {
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
                CardSystem.Instance.ExhaustPile.Remove(card);
                int randomIndex = Random.Range(0, CardSystem.Instance.drawPile.Count + 1);
                CardSystem.Instance.drawPile.Insert(randomIndex, card);

                if (!card.IsSpell)
                {
                    card.Durability = card.MaxDurability;
                }

                TriggerEventGA triggerEventGA = new(Events.WhenCardExitExhaust, card);
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

        Debug.Log("EffectToManage : " + EffectToManage);

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
                    if (EffectToManage.Events.Count == 1)
                    {
                        Effect effectToExecute = EffectToManage.Clone();
                        effectToExecute.Events = new List<Events> { Events.Instant };
                        GameEventSystem.Instance.RegisterEffect(effectToExecute);
                    }
                    else
                    {
                        Effect effectToExecute = EffectToManage.Clone();
                        effectToExecute.Events.Remove(Events.OnSelect);
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
