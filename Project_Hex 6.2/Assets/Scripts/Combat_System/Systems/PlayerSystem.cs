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
        ActionSystem.UnsubscribeReaction<PlayerLifeLossGA>(BeforeLifeLossPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<PlayerGainLifeGA>(BeforeGainHPPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<InvocPGA>(BeforeInvocPPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<SacPGA>(BeforeSacPPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<PlayerRefreshGA>(BeforeRefreshPPerformerPreReaction, ReactionTiming.PRE);
    }

    public int CalculateBonusPower(int BaseAmount, PermanentView permanentView)
    {
        int PassiveBonus = 0;

        if (permanentView.permaTypes.Contains(PermaTypes.Invoc))
        {
            PassiveBonus += CombatSystem.Instance.Invoc_PlayerGeneralPower + CombatSystem.Instance.Invoc_GeneralPower;
        }
        if (permanentView.permaTypes.Contains(PermaTypes.Decay))
        {
            PassiveBonus += CombatSystem.Instance.Decay_PlayerGeneralPower + CombatSystem.Instance.Decay_GeneralPower;
        }
        if (permanentView.permaTypes.Contains(PermaTypes.Hollow))
        {
            PassiveBonus += CombatSystem.Instance.Hollow_PlayerGeneralPower + CombatSystem.Instance.Hollow_GeneralPower;
        }
        if (permanentView.permaTypes.Contains(PermaTypes.Artillery))
        {
            PassiveBonus += CombatSystem.Instance.Artillery_PlayerGeneralPower + CombatSystem.Instance.Artillery_GeneralPower;
        }


        int finalDMG = 0;
        finalDMG = BaseAmount + permanentView.BonusPower + PassiveBonus + CombatSystem.Instance.PlayerGeneralPower + CombatSystem.Instance.GeneralPower; ;
        if (finalDMG < 0) finalDMG = 0;
        return finalDMG;
    }

    private IEnumerator AttackEnemyPerformer(AttackEnemyGA attackEnemyGA)
    {
        if (attackEnemyGA.Actionner != null)
        {
            PermanentView Attacker = attackEnemyGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (attackEnemyGA.playerTargets != null && attackEnemyGA.playerTargets.Count > 0)
            {
                int DamageAmount = CalculateBonusPower(attackEnemyGA.Damage, Attacker);
                DealDamageGA dealDamageGA = new(DamageAmount, attackEnemyGA.DynamicAmount, attackEnemyGA.playerTargets, null);
                dealDamageGA.Actionner = attackEnemyGA.Actionner;
                ActionSystem.Instance.AddReaction(dealDamageGA);
            }

            if (attackEnemyGA.enemyTargets != null && attackEnemyGA.enemyTargets.Count > 0)
            {
                int DamageAmount = CalculateBonusPower(attackEnemyGA.Damage, Attacker);
                DealDamageGA dealDamageGA = new(DamageAmount, attackEnemyGA.DynamicAmount, null, attackEnemyGA.enemyTargets);
                dealDamageGA.Actionner = attackEnemyGA.Actionner;
                ActionSystem.Instance.AddReaction(dealDamageGA);
            }
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            if (attackEnemyGA.playerTargets != null && attackEnemyGA.playerTargets.Count > 0)
            {
                ActionSystem.Instance.AddReaction(new DealDamageGA(attackEnemyGA.Damage, attackEnemyGA.DynamicAmount, attackEnemyGA.playerTargets, null));
            }

            if (attackEnemyGA.enemyTargets != null && attackEnemyGA.enemyTargets.Count > 0)
            {
                ActionSystem.Instance.AddReaction(new DealDamageGA(attackEnemyGA.Damage, attackEnemyGA.DynamicAmount, null, attackEnemyGA.enemyTargets));
            }
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

            if (healPlayerGA.playerTargets != null && healPlayerGA.playerTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new HealGA(healPlayerGA.HealAmount, healPlayerGA.DynamicAmount, healPlayerGA.playerTargets, null));

            if (healPlayerGA.enemyTargets != null && healPlayerGA.enemyTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new HealGA(healPlayerGA.HealAmount, healPlayerGA.DynamicAmount, null, healPlayerGA.enemyTargets));
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            if (healPlayerGA.playerTargets != null && healPlayerGA.playerTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new HealGA(healPlayerGA.HealAmount, healPlayerGA.DynamicAmount, healPlayerGA.playerTargets, null));

            if (healPlayerGA.enemyTargets != null && healPlayerGA.enemyTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new HealGA(healPlayerGA.HealAmount, healPlayerGA.DynamicAmount, null, healPlayerGA.enemyTargets));
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
                shieldGA.Actionner = shieldPlayerGA.Actionner;
                ActionSystem.Instance.AddReaction(shieldGA);
            }

            if (shieldPlayerGA.enemyTargets != null && shieldPlayerGA.enemyTargets.Count > 0)
            {
                ShieldGA shieldGA = new ShieldGA(null, shieldPlayerGA.enemyTargets);
                shieldGA.Actionner = shieldPlayerGA.Actionner;
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
                shieldGA.Actionner = playerUnShieldGA.Actionner;
                ActionSystem.Instance.AddReaction(shieldGA);
            }

            if (playerUnShieldGA.enemyTargets != null && playerUnShieldGA.enemyTargets.Count > 0)
            {
                UnShieldGA shieldGA = new UnShieldGA(null, playerUnShieldGA.enemyTargets);
                shieldGA.Actionner = playerUnShieldGA.Actionner;
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
                RefreshGA shieldGA = new RefreshGA(playerRefreshGA.playerTargets, null);
                shieldGA.Actionner = playerRefreshGA.Actionner;
                ActionSystem.Instance.AddReaction(shieldGA);
            }

            if (playerRefreshGA.enemyTargets != null && playerRefreshGA.enemyTargets.Count > 0)
            {
                RefreshGA shieldGA = new RefreshGA(null, playerRefreshGA.enemyTargets);
                shieldGA.Actionner = playerRefreshGA.Actionner;
                ActionSystem.Instance.AddReaction(shieldGA);
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
                ActionSystem.Instance.AddReaction(new AlterPowerGA(playerAlterPowerGA.Amount, playerAlterPowerGA.DynamicAmount, playerAlterPowerGA.passive, playerAlterPowerGA.permaTypes, null, null, playerAlterPowerGA.targetMode));
            }
            else
            {
                if (playerAlterPowerGA.playerTargets != null && playerAlterPowerGA.playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new AlterPowerGA(playerAlterPowerGA.Amount, playerAlterPowerGA.DynamicAmount, playerAlterPowerGA.passive, playerAlterPowerGA.permaTypes, playerAlterPowerGA.playerTargets, null));

                if (playerAlterPowerGA.enemyTargets != null && playerAlterPowerGA.enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new AlterPowerGA(playerAlterPowerGA.Amount, playerAlterPowerGA.DynamicAmount, playerAlterPowerGA.passive, playerAlterPowerGA.permaTypes, null, playerAlterPowerGA.enemyTargets));
            }
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            if (playerAlterPowerGA.passive)
            {
                ActionSystem.Instance.AddReaction(new AlterPowerGA(playerAlterPowerGA.Amount, playerAlterPowerGA.DynamicAmount, playerAlterPowerGA.passive, playerAlterPowerGA.permaTypes, null, null, playerAlterPowerGA.targetMode));
            }
            else
            {
                if (playerAlterPowerGA.playerTargets != null && playerAlterPowerGA.playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new AlterPowerGA(playerAlterPowerGA.Amount, playerAlterPowerGA.DynamicAmount, playerAlterPowerGA.passive, playerAlterPowerGA.permaTypes, playerAlterPowerGA.playerTargets, null));

                if (playerAlterPowerGA.enemyTargets != null && playerAlterPowerGA.enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new AlterPowerGA(playerAlterPowerGA.Amount, playerAlterPowerGA.DynamicAmount, playerAlterPowerGA.passive, playerAlterPowerGA.permaTypes, null, playerAlterPowerGA.enemyTargets));
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

            if (playerAlterStaminaGA.playerTargets != null && playerAlterStaminaGA.playerTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new AlterStaminaGA(playerAlterStaminaGA.Amount, playerAlterStaminaGA.DynamicAmount, playerAlterStaminaGA.permaTypes, playerAlterStaminaGA.playerTargets, null));

            if (playerAlterStaminaGA.enemyTargets != null && playerAlterStaminaGA.enemyTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new AlterStaminaGA(playerAlterStaminaGA.Amount, playerAlterStaminaGA.DynamicAmount, playerAlterStaminaGA.permaTypes, null, playerAlterStaminaGA.enemyTargets));
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            if (playerAlterStaminaGA.playerTargets != null && playerAlterStaminaGA.playerTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new AlterStaminaGA(playerAlterStaminaGA.Amount, playerAlterStaminaGA.DynamicAmount, playerAlterStaminaGA.permaTypes, playerAlterStaminaGA.playerTargets, null));

            if (playerAlterStaminaGA.enemyTargets != null && playerAlterStaminaGA.enemyTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new AlterStaminaGA(playerAlterStaminaGA.Amount, playerAlterStaminaGA.DynamicAmount, playerAlterStaminaGA.permaTypes, null, playerAlterStaminaGA.enemyTargets));
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
                ActionSystem.Instance.AddReaction(new LifeLossGA(playerLifeLossGA.Amount, playerLifeLossGA.DynamicAmount, playerLifeLossGA.playerTargets, null));

            if (playerLifeLossGA.enemyTargets != null && playerLifeLossGA.enemyTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new LifeLossGA(playerLifeLossGA.Amount, playerLifeLossGA.DynamicAmount, null, playerLifeLossGA.enemyTargets));
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            if (playerLifeLossGA.playerTargets != null && playerLifeLossGA.playerTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new LifeLossGA(playerLifeLossGA.Amount, playerLifeLossGA.DynamicAmount, playerLifeLossGA.playerTargets, null));

            if (playerLifeLossGA.enemyTargets != null && playerLifeLossGA.enemyTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new LifeLossGA(playerLifeLossGA.Amount, playerLifeLossGA.DynamicAmount, null, playerLifeLossGA.enemyTargets));
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
                ActionSystem.Instance.AddReaction(new GainLifeGA(playerGainLifeGA.Amount, playerGainLifeGA.DynamicAmount, playerGainLifeGA.passive, playerGainLifeGA.permaTypes, null, null, playerGainLifeGA.targetMode));
            }
            else
            {
                if (playerGainLifeGA.playerTargets != null && playerGainLifeGA.playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new GainLifeGA(playerGainLifeGA.Amount, playerGainLifeGA.DynamicAmount, playerGainLifeGA.passive, playerGainLifeGA.permaTypes, playerGainLifeGA.playerTargets, null));

                if (playerGainLifeGA.enemyTargets != null && playerGainLifeGA.enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new GainLifeGA(playerGainLifeGA.Amount, playerGainLifeGA.DynamicAmount, playerGainLifeGA.passive, playerGainLifeGA.permaTypes, null, playerGainLifeGA.enemyTargets));
            }
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            if (playerGainLifeGA.passive)
            {
                ActionSystem.Instance.AddReaction(new GainLifeGA(playerGainLifeGA.Amount, playerGainLifeGA.DynamicAmount, playerGainLifeGA.passive, playerGainLifeGA.permaTypes, null, null, playerGainLifeGA.targetMode));
            }
            else
            {
                if (playerGainLifeGA.playerTargets != null && playerGainLifeGA.playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new GainLifeGA(playerGainLifeGA.Amount, playerGainLifeGA.DynamicAmount, playerGainLifeGA.passive, playerGainLifeGA.permaTypes, playerGainLifeGA.playerTargets, null));

                if (playerGainLifeGA.enemyTargets != null && playerGainLifeGA.enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new GainLifeGA(playerGainLifeGA.Amount, playerGainLifeGA.DynamicAmount, playerGainLifeGA.passive, playerGainLifeGA.permaTypes, null, playerGainLifeGA.enemyTargets));
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

            InvocGA invocGA = new(invocPGA.Amount, invocPGA.DynamicAmount, invocPGA.CardsToInvoc);
            ActionSystem.Instance.AddReaction(invocGA);
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            InvocGA invocGA = new(invocPGA.Amount, invocPGA.DynamicAmount, invocPGA.CardsToInvoc);
            ActionSystem.Instance.AddReaction(invocGA);
        }
        yield return null;
    }

    private IEnumerator SacPPerformer(SacPGA sacPGA)
    {
        if (sacPGA.Actionner != null)
        {
            PermanentView Attacker = sacPGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            SacGA sacGA = new(sacPGA.playerTargets,sacPGA.enemyTargets);
            ActionSystem.Instance.AddReaction(sacGA);
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            SacGA sacGA = new(sacPGA.playerTargets,sacPGA.enemyTargets);
            ActionSystem.Instance.AddReaction(sacGA);
        }
        yield return null;
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
