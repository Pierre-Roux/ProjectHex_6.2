using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class PlayerSystem : Singleton<PlayerSystem>
{
    void OnEnable()
    {
        ActionSystem.AttachPerformer<AttackEnemyGA>(AttackEnemyPerformer); 
        ActionSystem.AttachPerformer<HealPlayerGA>(HealPlayerPerformer); 
        ActionSystem.AttachPerformer<ShieldPlayerGA>(ShieldPlayerPerformer);
        ActionSystem.AttachPerformer<PlayerUnShieldGA>(UnShieldPlayerPerformer);
        ActionSystem.AttachPerformer<PlayerAlterPowerGA>(AlterPlayerPerformer);
        ActionSystem.AttachPerformer<PlayerAlterStaminaGA>(AlterStamPlayerPerformer);
        ActionSystem.AttachPerformer<PlayerAlterCardCostGA>(AlterCardCostPlayerPerformer);
        ActionSystem.AttachPerformer<PlayerLifeLossGA>(LifeLossPlayerPerformer);
        ActionSystem.AttachPerformer<PlayerGainLifeGA>(GainHPEnemyPerformer);
        ActionSystem.AttachPerformer<InvocPGA>(InvocPPerformer);
        ActionSystem.AttachPerformer<SacPGA>(SacPPerformer);
        ActionSystem.AttachPerformer<PlayerRefreshGA>(RefreshPlayerPerformer);

        ActionSystem.SubscribeReaction<AttackEnemyGA>(BeforeAttackPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<HealPlayerGA>(BeforeHealPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<ShieldPlayerGA>(BeforeShieldPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<PlayerUnShieldGA>(BeforeUnShieldPPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<PlayerAlterPowerGA>(BeforeAlterPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<PlayerAlterStaminaGA>(BeforeAlterStamPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<PlayerAlterCardCostGA>(BeforeAlterCardCostPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<PlayerLifeLossGA>(BeforeLifeLossPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<PlayerGainLifeGA>(BeforeGainHPPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<InvocPGA>(BeforeInvocPPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<SacPGA>(BeforeSacPPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<PlayerRefreshGA>(BeforeRefreshPPerformerPreReaction, ReactionTiming.PRE);

    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<AttackEnemyGA>();
        ActionSystem.DetachPerformer<HealPlayerGA>();
        ActionSystem.DetachPerformer<ShieldPlayerGA>();
        ActionSystem.DetachPerformer<PlayerUnShieldGA>();
        ActionSystem.DetachPerformer<PlayerAlterPowerGA>();
        ActionSystem.DetachPerformer<PlayerAlterStaminaGA>();
        ActionSystem.DetachPerformer<PlayerAlterCardCostGA>();
        ActionSystem.DetachPerformer<PlayerLifeLossGA>();
        ActionSystem.DetachPerformer<PlayerGainLifeGA>();
        ActionSystem.DetachPerformer<InvocPGA>();
        ActionSystem.DetachPerformer<SacPGA>();
        ActionSystem.DetachPerformer<PlayerRefreshGA>();

        ActionSystem.UnsubscribeReaction<AttackEnemyGA>(BeforeAttackPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<HealPlayerGA>(BeforeHealPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<ShieldPlayerGA>(BeforeShieldPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<PlayerUnShieldGA>(BeforeUnShieldPPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<PlayerAlterPowerGA>(BeforeAlterPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<PlayerAlterStaminaGA>(BeforeAlterStamPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<PlayerAlterCardCostGA>(BeforeAlterCardCostPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<PlayerLifeLossGA>(BeforeLifeLossPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<PlayerGainLifeGA>(BeforeGainHPPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<InvocPGA>(BeforeInvocPPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<SacPGA>(BeforeSacPPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<PlayerRefreshGA>(BeforeRefreshPPerformerPreReaction, ReactionTiming.PRE);
    }

    private IEnumerator AttackEnemyPerformer(AttackEnemyGA attackEnemyGA)
    {
        int DamageBonus = 0;
        if (attackEnemyGA.Actionner != null)
        {
            PermanentView Attacker = attackEnemyGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            DamageBonus = Attacker.CalculateBonusPower();
        }

        if (attackEnemyGA.playerTargets != null && attackEnemyGA.playerTargets.Count > 0)
        {
            DealDamageGA dealDamageGA = new(attackEnemyGA.Damage, DamageBonus, attackEnemyGA.multiplyAmount, attackEnemyGA.DynamicAmount, attackEnemyGA.playerTargets, null);
            dealDamageGA.Actionner = attackEnemyGA.Actionner;
            dealDamageGA.SourceEffect = attackEnemyGA.SourceEffect;
            dealDamageGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(dealDamageGA);
        }

        if (attackEnemyGA.enemyTargets != null && attackEnemyGA.enemyTargets.Count > 0)
        {
            DealDamageGA dealDamageGA = new(attackEnemyGA.Damage,DamageBonus,attackEnemyGA.multiplyAmount, attackEnemyGA.DynamicAmount, null, attackEnemyGA.enemyTargets);
            dealDamageGA.Actionner = attackEnemyGA.Actionner;
            dealDamageGA.SourceEffect = attackEnemyGA.SourceEffect;
            dealDamageGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(dealDamageGA);
        }
    }

    private IEnumerator HealPlayerPerformer(HealPlayerGA healPlayerGA)
    {
        if (healPlayerGA.Actionner != null)
        {
            PermanentView Attacker = healPlayerGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);
        }

        if (healPlayerGA.playerTargets != null && healPlayerGA.playerTargets.Count > 0)
        {
            HealGA healGA = new HealGA(healPlayerGA.HealAmount, healPlayerGA.multiplyAmount, healPlayerGA.DynamicAmount, healPlayerGA.playerTargets, null);
            healGA.SourceEffect = healPlayerGA.SourceEffect;
            healGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(healGA);            
        }

        if (healPlayerGA.enemyTargets != null && healPlayerGA.enemyTargets.Count > 0)
        {
            HealGA healGA = new HealGA(healPlayerGA.HealAmount, healPlayerGA.multiplyAmount, healPlayerGA.DynamicAmount, null, healPlayerGA.enemyTargets);
            healGA.SourceEffect = healPlayerGA.SourceEffect;
            healGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(healGA);            
        }

        
    }

    private IEnumerator ShieldPlayerPerformer(ShieldPlayerGA shieldPlayerGA)
    {
        if (shieldPlayerGA.Actionner != null)
        {
            PermanentView Attacker = shieldPlayerGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (shieldPlayerGA.playerTargets != null && shieldPlayerGA.playerTargets.Count > 0)
            {
                ShieldGA shieldGA = new ShieldGA(shieldPlayerGA.playerTargets, null);
                shieldGA.SourceEffect = shieldPlayerGA.SourceEffect;
                shieldGA.Actionner = shieldPlayerGA.Actionner;
                shieldGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(shieldGA);
            }

            if (shieldPlayerGA.enemyTargets != null && shieldPlayerGA.enemyTargets.Count > 0)
            {
                ShieldGA shieldGA = new ShieldGA(null, shieldPlayerGA.enemyTargets);
                shieldGA.SourceEffect = shieldPlayerGA.SourceEffect;
                shieldGA.Actionner = shieldPlayerGA.Actionner;
                shieldGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(shieldGA);
            }
        }
    }

    private IEnumerator UnShieldPlayerPerformer(PlayerUnShieldGA playerUnShieldGA)
    {
        if (playerUnShieldGA.Actionner != null)
        {
            PermanentView Attacker = playerUnShieldGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (playerUnShieldGA.playerTargets != null && playerUnShieldGA.playerTargets.Count > 0)
            {
                UnShieldGA shieldGA = new UnShieldGA(playerUnShieldGA.playerTargets, null);
                shieldGA.SourceEffect = playerUnShieldGA.SourceEffect;
                shieldGA.Actionner = playerUnShieldGA.Actionner;
                shieldGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(shieldGA);
            }

            if (playerUnShieldGA.enemyTargets != null && playerUnShieldGA.enemyTargets.Count > 0)
            {
                UnShieldGA shieldGA = new UnShieldGA(null, playerUnShieldGA.enemyTargets);
                shieldGA.SourceEffect = playerUnShieldGA.SourceEffect;
                shieldGA.Actionner = playerUnShieldGA.Actionner;
                shieldGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(shieldGA);
            }
        }
    }

    private IEnumerator RefreshPlayerPerformer(PlayerRefreshGA playerRefreshGA)
    {
        if (playerRefreshGA.Actionner != null)
        {
            PermanentView Attacker = playerRefreshGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (playerRefreshGA.playerTargets != null && playerRefreshGA.playerTargets.Count > 0)
            {
                RefreshGA refreshGA = new RefreshGA(playerRefreshGA.playerTargets, null);
                refreshGA.Actionner = playerRefreshGA.Actionner;
                refreshGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(refreshGA);
            }

            if (playerRefreshGA.enemyTargets != null && playerRefreshGA.enemyTargets.Count > 0)
            {
                RefreshGA refreshGA = new RefreshGA(null, playerRefreshGA.enemyTargets);
                refreshGA.Actionner = playerRefreshGA.Actionner;
                refreshGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(refreshGA);
            }
        }
    }

    private IEnumerator AlterPlayerPerformer(PlayerAlterPowerGA playerAlterPowerGA)
    {
        if (playerAlterPowerGA.Actionner != null)
        {
            PermanentView Attacker = playerAlterPowerGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);
            if (playerAlterPowerGA.passive)
            {
                AlterPowerGA alterPowerGA = new AlterPowerGA(playerAlterPowerGA.Amount, playerAlterPowerGA.multiplyAmount, playerAlterPowerGA.DynamicAmount, playerAlterPowerGA.passive, playerAlterPowerGA.aditive, null, null, playerAlterPowerGA.targetModeInfo);
                alterPowerGA.SourceEffect = playerAlterPowerGA.SourceEffect;
                alterPowerGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(alterPowerGA);
            }
            else
            {
                if (playerAlterPowerGA.playerTargets != null && playerAlterPowerGA.playerTargets.Count > 0)
                {
                    AlterPowerGA alterPowerGA = new AlterPowerGA(playerAlterPowerGA.Amount, playerAlterPowerGA.multiplyAmount, playerAlterPowerGA.DynamicAmount, playerAlterPowerGA.passive, playerAlterPowerGA.aditive, playerAlterPowerGA.playerTargets, null);
                    alterPowerGA.SourceEffect = playerAlterPowerGA.SourceEffect;
                    alterPowerGA.ActivateToolTip = false;
                    ActionSystem.Instance.AddReaction(alterPowerGA);
                }

                if (playerAlterPowerGA.enemyTargets != null && playerAlterPowerGA.enemyTargets.Count > 0)
                {
                    AlterPowerGA alterPowerGA = new AlterPowerGA(playerAlterPowerGA.Amount, playerAlterPowerGA.multiplyAmount, playerAlterPowerGA.DynamicAmount, playerAlterPowerGA.passive, playerAlterPowerGA.aditive, null, playerAlterPowerGA.enemyTargets);
                    alterPowerGA.SourceEffect = playerAlterPowerGA.SourceEffect;
                    alterPowerGA.ActivateToolTip = false;
                    ActionSystem.Instance.AddReaction(alterPowerGA);
                }  
            }
        }
    }

    private IEnumerator AlterStamPlayerPerformer(PlayerAlterStaminaGA playerAlterStaminaGA)
    {
        if (playerAlterStaminaGA.Actionner != null)
        {
            PermanentView Attacker = playerAlterStaminaGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (playerAlterStaminaGA.passive)
            {
                AlterStaminaGA alterStaminaGA = new AlterStaminaGA(playerAlterStaminaGA.Amount, playerAlterStaminaGA.multiplyAmount, playerAlterStaminaGA.DynamicAmount, playerAlterStaminaGA.passive, playerAlterStaminaGA.aditive, null, null, playerAlterStaminaGA.targetModeInfo);
                alterStaminaGA.SourceEffect = playerAlterStaminaGA.SourceEffect;
                alterStaminaGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(alterStaminaGA);
            }
            else
            {
                if (playerAlterStaminaGA.playerTargets != null && playerAlterStaminaGA.playerTargets.Count > 0)
                {
                    AlterStaminaGA alterStaminaGA = new AlterStaminaGA(playerAlterStaminaGA.Amount, playerAlterStaminaGA.multiplyAmount, playerAlterStaminaGA.DynamicAmount, playerAlterStaminaGA.passive, playerAlterStaminaGA.aditive, playerAlterStaminaGA.playerTargets, null, playerAlterStaminaGA.targetModeInfo);
                    alterStaminaGA.SourceEffect = playerAlterStaminaGA.SourceEffect;
                    alterStaminaGA.ActivateToolTip = false;
                    ActionSystem.Instance.AddReaction(alterStaminaGA);
                }

                if (playerAlterStaminaGA.enemyTargets != null && playerAlterStaminaGA.enemyTargets.Count > 0)
                {
                    AlterStaminaGA alterStaminaGA = new AlterStaminaGA(playerAlterStaminaGA.Amount, playerAlterStaminaGA.multiplyAmount, playerAlterStaminaGA.DynamicAmount, playerAlterStaminaGA.passive, playerAlterStaminaGA.aditive, null, playerAlterStaminaGA.enemyTargets, playerAlterStaminaGA.targetModeInfo);
                    alterStaminaGA.SourceEffect = playerAlterStaminaGA.SourceEffect;
                    alterStaminaGA.ActivateToolTip = false;
                    ActionSystem.Instance.AddReaction(alterStaminaGA);
                }
            }
        }
    }
    
    private IEnumerator AlterCardCostPlayerPerformer(PlayerAlterCardCostGA playerAlterCardCostGA)
    {
        if (playerAlterCardCostGA.Actionner != null)
        {
            PermanentView Attacker = playerAlterCardCostGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (playerAlterCardCostGA.passive)
            {
                AlterCardCostGA alterCardCostGA = new AlterCardCostGA(playerAlterCardCostGA.Amount, playerAlterCardCostGA.multiplyAmount, playerAlterCardCostGA.DynamicAmount, playerAlterCardCostGA.passive,null, playerAlterCardCostGA.targetModeInfo);
                alterCardCostGA.SourceEffect = playerAlterCardCostGA.SourceEffect;
                alterCardCostGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(alterCardCostGA);
            }
            else
            {
                if (playerAlterCardCostGA.cardTargets != null && playerAlterCardCostGA.cardTargets.Count > 0)
                {
                    AlterCardCostGA alterCardCostGA = new AlterCardCostGA(playerAlterCardCostGA.Amount, playerAlterCardCostGA.multiplyAmount, playerAlterCardCostGA.DynamicAmount, playerAlterCardCostGA.passive, playerAlterCardCostGA.cardTargets, playerAlterCardCostGA.targetModeInfo);
                    alterCardCostGA.SourceEffect = playerAlterCardCostGA.SourceEffect;
                    alterCardCostGA.ActivateToolTip = false;
                    ActionSystem.Instance.AddReaction(alterCardCostGA);
                }
            }
        }
    }

    private IEnumerator LifeLossPlayerPerformer(PlayerLifeLossGA playerLifeLossGA)
    {
        if (playerLifeLossGA.Actionner != null)
        {
            PermanentView Attacker = playerLifeLossGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (playerLifeLossGA.playerTargets != null && playerLifeLossGA.playerTargets.Count > 0)
            {
                LifeLossGA lifeLossGA = new LifeLossGA(playerLifeLossGA.Amount, playerLifeLossGA.multiplyAmount, playerLifeLossGA.DynamicAmount, playerLifeLossGA.playerTargets, null);
                lifeLossGA.SourceEffect = playerLifeLossGA.SourceEffect;
                lifeLossGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(lifeLossGA);
            }
            if (playerLifeLossGA.enemyTargets != null && playerLifeLossGA.enemyTargets.Count > 0)
            {
                LifeLossGA lifeLossGA = new LifeLossGA(playerLifeLossGA.Amount, playerLifeLossGA.multiplyAmount, playerLifeLossGA.DynamicAmount, null, playerLifeLossGA.enemyTargets);
                lifeLossGA.SourceEffect = playerLifeLossGA.SourceEffect;
                lifeLossGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(lifeLossGA);
            }
        }
    }

    private IEnumerator GainHPEnemyPerformer(PlayerGainLifeGA playerGainLifeGA)
    {
        if (playerGainLifeGA.Actionner != null)
        {
            PermanentView Attacker = playerGainLifeGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);
            if (playerGainLifeGA.passive)
            {
                GainLifeGA gainLifeGA = new GainLifeGA(playerGainLifeGA.Amount, playerGainLifeGA.multiplyAmount, playerGainLifeGA.DynamicAmount, playerGainLifeGA.passive, playerGainLifeGA.aditive, null, null, playerGainLifeGA.targetModeInfo);
                gainLifeGA.SourceEffect = playerGainLifeGA.SourceEffect;
                gainLifeGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(gainLifeGA);
            }
            else
            {
                if (playerGainLifeGA.playerTargets != null && playerGainLifeGA.playerTargets.Count > 0)
                {
                    GainLifeGA gainLifeGA = new GainLifeGA(playerGainLifeGA.Amount, playerGainLifeGA.multiplyAmount, playerGainLifeGA.DynamicAmount, playerGainLifeGA.passive, playerGainLifeGA.aditive, playerGainLifeGA.playerTargets, null);
                    gainLifeGA.SourceEffect = playerGainLifeGA.SourceEffect;
                    gainLifeGA.ActivateToolTip = false;
                    ActionSystem.Instance.AddReaction(gainLifeGA);                    
                }

                if (playerGainLifeGA.enemyTargets != null && playerGainLifeGA.enemyTargets.Count > 0)
                {
                    GainLifeGA gainLifeGA = new GainLifeGA(playerGainLifeGA.Amount, playerGainLifeGA.multiplyAmount, playerGainLifeGA.DynamicAmount, playerGainLifeGA.passive, playerGainLifeGA.aditive, null, playerGainLifeGA.enemyTargets);
                    gainLifeGA.SourceEffect = playerGainLifeGA.SourceEffect;
                    gainLifeGA.ActivateToolTip = false;
                    ActionSystem.Instance.AddReaction(gainLifeGA);                    
                }
                    
            }
        }
    }

    private IEnumerator InvocPPerformer(InvocPGA invocPGA)
    {
        if (invocPGA.Actionner != null)
        {
            PermanentView Attacker = invocPGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            InvocGA invocGA = new(invocPGA.Amount, invocPGA.multiplyAmount, invocPGA.DynamicAmount, invocPGA.CardsToInvoc);
            invocGA.SourceEffect = invocPGA.SourceEffect;
            invocGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(invocGA);
        }
    }

    private IEnumerator SacPPerformer(SacPGA sacPGA)
    {
        if (sacPGA.Actionner != null)
        {
            PermanentView Attacker = sacPGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            SacGA sacGA = new(sacPGA.playerTargets, sacPGA.enemyTargets);
            sacGA.SourceEffect = sacPGA.SourceEffect;
            sacGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(sacGA);
        }
    }

    private void BeforeAttackPreReaction(AttackEnemyGA attackEnemyGA)
    {
        if (attackEnemyGA.Actionner != null)
        {
            PermanentView Attacker = attackEnemyGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeHealPreReaction(HealPlayerGA healPlayerGA)
    {
        if (healPlayerGA.Actionner != null)
        {
            PermanentView Attacker = healPlayerGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeShieldPreReaction(ShieldPlayerGA shieldPlayerGA)
    {
        if (shieldPlayerGA.Actionner != null)
        {
            PermanentView Attacker = shieldPlayerGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeAlterPreReaction(PlayerAlterPowerGA playerAlterPowerGA)
    {
        if (playerAlterPowerGA.Actionner != null)
        {
            PermanentView Attacker = playerAlterPowerGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeAlterStamPreReaction(PlayerAlterStaminaGA playerAlterStaminaGA)
    {
        if (playerAlterStaminaGA.Actionner != null)
        {
            PermanentView Attacker = playerAlterStaminaGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeAlterCardCostPreReaction(PlayerAlterCardCostGA playerAlterCardCostGA)
    {
        if (playerAlterCardCostGA.Actionner != null)
        {
            PermanentView Attacker = playerAlterCardCostGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeLifeLossPreReaction(PlayerLifeLossGA playerLifeLossGA)
    {
        if (playerLifeLossGA.Actionner != null)
        {
            PermanentView Attacker = playerLifeLossGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeGainHPPreReaction(PlayerGainLifeGA playerGainLifeGA)
    {
        if (playerGainLifeGA.Actionner != null)
        {
            PermanentView Attacker = playerGainLifeGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeInvocPPerformerPreReaction(InvocPGA invocPGA)
    {
        if (invocPGA.Actionner != null)
        {
            PermanentView Attacker = invocPGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }
    
    private void BeforeSacPPerformerPreReaction(SacPGA sacPGA)
    {
        if (sacPGA.Actionner != null)
        {
            PermanentView Attacker = sacPGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }
    
    private void BeforeUnShieldPPerformerPreReaction(PlayerUnShieldGA playerUnShieldGA)
    {
        if (playerUnShieldGA.Actionner != null)
        {
            PermanentView Attacker = playerUnShieldGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }
    
    private void BeforeRefreshPPerformerPreReaction(PlayerRefreshGA playerRefreshGA)
    {
        if (playerRefreshGA.Actionner != null)
        {
            PermanentView Attacker = playerRefreshGA.Actionner.GetComponent<PermanentView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }
}
