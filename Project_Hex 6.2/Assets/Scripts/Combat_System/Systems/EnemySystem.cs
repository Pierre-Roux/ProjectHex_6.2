using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class EnemySystem : Singleton<EnemySystem>
{
    public EnemyView enemyView;

    void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        ActionSystem.AttachPerformer<AttackPlayerGA>(AttackPlayerPerformer);
        ActionSystem.AttachPerformer<HealEnemyGA>(HealEnemyPerformer);
        ActionSystem.AttachPerformer<ShieldEnemyGA>(ShieldEnemyPerformer);
        ActionSystem.AttachPerformer<EnemyUnShieldGA>(UnShieldEnemyPerformer);
        ActionSystem.AttachPerformer<EnemyAlterPowerGA>(AlterEnemyPerformer);
        ActionSystem.AttachPerformer<EnemyAlterStaminaGA>(AlterStamEnemyPerformer);
        ActionSystem.AttachPerformer<EnemyLifeLossGA>(LifeLossEnemyPerformer);
        ActionSystem.AttachPerformer<EnemyGainLifeGA>(GainHPEnemyPerformer);
        ActionSystem.AttachPerformer<InvocEGA>(InvocEPerformer);
        ActionSystem.AttachPerformer<SacEGA>(SacEPerformer);
        ActionSystem.AttachPerformer<EnemyRefreshGA>(RefreshEnemyPerformer);

        ActionSystem.AttachPerformer<SpawnConstructGA>(PerformIntentConstructPerformer);

        ActionSystem.SubscribeReaction<AttackPlayerGA>(BeforeAttackPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<HealEnemyGA>(BeforeHealPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<ShieldEnemyGA>(BeforeShieldPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyUnShieldGA>(BeforeUnShieldEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyAlterPowerGA>(BeforeAlterPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyAlterStaminaGA>(BeforeAlterStamPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyLifeLossGA>(BeforeLifeLossPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyGainLifeGA>(BeforeGainHPPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<InvocEGA>(BeforeInvocEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<SacEGA>(BeforeSacEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyRefreshGA>(BeforeRefreshEPerformerPreReaction, ReactionTiming.PRE);

    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<AttackPlayerGA>();
        ActionSystem.DetachPerformer<HealEnemyGA>();
        ActionSystem.DetachPerformer<ShieldEnemyGA>();
        ActionSystem.DetachPerformer<EnemyUnShieldGA>();
        ActionSystem.DetachPerformer<EnemyAlterPowerGA>();
        ActionSystem.DetachPerformer<EnemyAlterStaminaGA>();
        ActionSystem.DetachPerformer<EnemyLifeLossGA>();
        ActionSystem.DetachPerformer<EnemyGainLifeGA>();
        ActionSystem.DetachPerformer<InvocEGA>();
        ActionSystem.DetachPerformer<SacEGA>();
        ActionSystem.DetachPerformer<EnemyRefreshGA>();

        ActionSystem.DetachPerformer<SpawnConstructGA>();

        ActionSystem.UnsubscribeReaction<AttackPlayerGA>(BeforeAttackPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<HealEnemyGA>(BeforeHealPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<ShieldEnemyGA>(BeforeShieldPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyUnShieldGA>(BeforeUnShieldEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyAlterPowerGA>(BeforeAlterPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyAlterStaminaGA>(BeforeAlterStamPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyLifeLossGA>(BeforeLifeLossPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyGainLifeGA>(BeforeGainHPPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<InvocEGA>(BeforeInvocEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<SacEGA>(BeforeSacEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyRefreshGA>(BeforeRefreshEPerformerPreReaction, ReactionTiming.PRE);
    }


    // Performers
    private IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurnGA)
    {
        var intents = CombatSystem.Instance.Enemy_Permanents
            .Where(e => e.IntentAction != null && e.IntentAction.Events == Events.EnemyTurn)
            .Select(e => e.IntentAction)
            .ToList();

        foreach (var intent in intents)
        {
            // Vérifier que l’ennemi existe encore et est valide
            var enemySlotViewGO = intent.Actionner;
            EnemySlotView enemySlotView = null;
            if (enemySlotViewGO != null)
            {
                enemySlotView = enemySlotViewGO.GetComponent<EnemySlotView>();
            }
            if (enemySlotView == null || !CombatSystem.Instance.Enemy_Permanents.Contains(enemySlotView))
                continue;

            if (intent is EffectGroup)
            {
                EffectGroup group = (EffectGroup)intent;
                foreach (var effect in group.EffectGroups)
                {
                    int MultiHit = effect.MultiHit;
                    if (MultiHit < 1) MultiHit = 1;
                    for (int i = 0; i < MultiHit; i++)
                    {
                        // Exécuter action
                        GameAction action = effect.GetGameAction();
                        yield return StartCoroutine(ActionSystem.Instance.RunAction(action));                        
                    }
                }

                if (CombatSystem.Instance.Enemy_Permanents.Contains(enemySlotView))
                    enemySlotView.UpdateIntent();  
            }
            else
            {
                int MultiHit = intent.MultiHit;
                if (MultiHit < 1) MultiHit = 1;
                for (int i = 0; i < MultiHit; i++)
                {
                    // Exécuter action
                    GameAction action = intent.GetGameAction();
                    yield return StartCoroutine(ActionSystem.Instance.RunAction(action));
                }

                if (CombatSystem.Instance.Enemy_Permanents.Contains(enemySlotView))
                    enemySlotView.UpdateIntent();                
            }
        }

        EndEnemyTurnGA endEnemyTurnGA = new();
        ActionSystem.Instance.AddReaction(endEnemyTurnGA);
        yield return null;
    }

    public int CalculateBonusPower(int BaseAmount, EnemySlotView enemySlotView)
    {
        int PassiveBonus = 0;

        if (enemySlotView.permaTypes.Contains(PermaTypes.Invoc))
        {
            PassiveBonus += CombatSystem.Instance.Invoc_EnemyGeneralPower + CombatSystem.Instance.Invoc_GeneralPower;
        }
        if (enemySlotView.permaTypes.Contains(PermaTypes.Decay))
        {
            PassiveBonus += CombatSystem.Instance.Decay_EnemyGeneralPower + CombatSystem.Instance.Decay_GeneralPower;
        }


        int finalDMG = 0;
        finalDMG = BaseAmount + enemySlotView.BonusPower + PassiveBonus + CombatSystem.Instance.EnemyGeneralPower + CombatSystem.Instance.GeneralPower; ;
        if (finalDMG < 0) finalDMG = 0;
        return finalDMG;
    }

    private IEnumerator AttackPlayerPerformer(AttackPlayerGA attackPlayerGA)
    {
        if (attackPlayerGA.Actionner != null)
        {
            EnemySlotView Attacker = attackPlayerGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (attackPlayerGA.playerTargets != null && attackPlayerGA.playerTargets.Count > 0)
            {
                int DamageAmount = CalculateBonusPower(attackPlayerGA.Damage, Attacker);
                DealDamageGA dealDamageGA = new(DamageAmount, attackPlayerGA.DynamicAmount, attackPlayerGA.playerTargets, null);
                dealDamageGA.Actionner = attackPlayerGA.Actionner;
                ActionSystem.Instance.AddReaction(dealDamageGA);
            }

            if (attackPlayerGA.enemyTargets != null && attackPlayerGA.enemyTargets.Count > 0)
            {
                int DamageAmount = CalculateBonusPower(attackPlayerGA.Damage, Attacker);
                DealDamageGA dealDamageGA = new(DamageAmount, attackPlayerGA.DynamicAmount, null, attackPlayerGA.enemyTargets);
                dealDamageGA.Actionner = attackPlayerGA.Actionner;
                ActionSystem.Instance.AddReaction(dealDamageGA);
            }
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            if (attackPlayerGA.playerTargets != null && attackPlayerGA.playerTargets.Count > 0)
            {
                ActionSystem.Instance.AddReaction(new DealDamageGA(attackPlayerGA.Damage, attackPlayerGA.DynamicAmount, attackPlayerGA.playerTargets, null));
            }

            if (attackPlayerGA.enemyTargets != null && attackPlayerGA.enemyTargets.Count > 0)
            {
                ActionSystem.Instance.AddReaction(new DealDamageGA(attackPlayerGA.Damage, attackPlayerGA.DynamicAmount, null, attackPlayerGA.enemyTargets));
            }
        }            
    }

    private IEnumerator HealEnemyPerformer(HealEnemyGA healEnemyGA)
    {
        if (healEnemyGA.Actionner != null)
        {
            EnemySlotView Attacker = healEnemyGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (healEnemyGA.playerTargets != null && healEnemyGA.playerTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new HealGA(healEnemyGA.HealAmount, healEnemyGA.DynamicAmount, healEnemyGA.playerTargets, null));

            if (healEnemyGA.enemyTargets != null && healEnemyGA.enemyTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new HealGA(healEnemyGA.HealAmount, healEnemyGA.DynamicAmount, null, healEnemyGA.enemyTargets));
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            if (healEnemyGA.playerTargets != null && healEnemyGA.playerTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new HealGA(healEnemyGA.HealAmount, healEnemyGA.DynamicAmount, healEnemyGA.playerTargets, null));

            if (healEnemyGA.enemyTargets != null && healEnemyGA.enemyTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new HealGA(healEnemyGA.HealAmount, healEnemyGA.DynamicAmount, null, healEnemyGA.enemyTargets));
        }
    }

    private IEnumerator ShieldEnemyPerformer(ShieldEnemyGA shieldEnemyGA)
    {
        if (shieldEnemyGA.Actionner != null)
        {
            EnemySlotView Attacker = shieldEnemyGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (shieldEnemyGA.playerTargets != null && shieldEnemyGA.playerTargets.Count > 0)
            {
                ShieldGA shieldGA = new ShieldGA(shieldEnemyGA.playerTargets, null);
                shieldGA.Actionner = shieldEnemyGA.Actionner;
                ActionSystem.Instance.AddReaction(shieldGA);
            }

            if (shieldEnemyGA.enemyTargets != null && shieldEnemyGA.enemyTargets.Count > 0)
            {
                ShieldGA shieldGA = new ShieldGA(null, shieldEnemyGA.enemyTargets);
                shieldGA.Actionner = shieldEnemyGA.Actionner;
                ActionSystem.Instance.AddReaction(shieldGA);
            }
        }
    }

    private IEnumerator UnShieldEnemyPerformer(EnemyUnShieldGA enemyUnShieldGA)
    {
        if (enemyUnShieldGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyUnShieldGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (enemyUnShieldGA.playerTargets != null && enemyUnShieldGA.playerTargets.Count > 0)
            {
                UnShieldGA shieldGA = new UnShieldGA(enemyUnShieldGA.playerTargets, null);
                shieldGA.Actionner = enemyUnShieldGA.Actionner;
                ActionSystem.Instance.AddReaction(shieldGA);
            }

            if (enemyUnShieldGA.enemyTargets != null && enemyUnShieldGA.enemyTargets.Count > 0)
            {
                UnShieldGA shieldGA = new UnShieldGA(null, enemyUnShieldGA.enemyTargets);
                shieldGA.Actionner = enemyUnShieldGA.Actionner;
                ActionSystem.Instance.AddReaction(shieldGA);
            }
        }
    }

    private IEnumerator RefreshEnemyPerformer(EnemyRefreshGA enemyRefreshGA)
    {
        if (enemyRefreshGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyRefreshGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (enemyRefreshGA.playerTargets != null && enemyRefreshGA.playerTargets.Count > 0)
            {
                RefreshGA shieldGA = new RefreshGA(enemyRefreshGA.playerTargets, null);
                shieldGA.Actionner = enemyRefreshGA.Actionner;
                ActionSystem.Instance.AddReaction(shieldGA);
            }

            if (enemyRefreshGA.enemyTargets != null && enemyRefreshGA.enemyTargets.Count > 0)
            {
                RefreshGA shieldGA = new RefreshGA(null, enemyRefreshGA.enemyTargets);
                shieldGA.Actionner = enemyRefreshGA.Actionner;
                ActionSystem.Instance.AddReaction(shieldGA);
            }
        }
    }

    private IEnumerator AlterEnemyPerformer(EnemyAlterPowerGA enemyAlterPowerGA)
    {
        if (enemyAlterPowerGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyAlterPowerGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);
            if (enemyAlterPowerGA.passive)
            {
                ActionSystem.Instance.AddReaction(new AlterPowerGA(enemyAlterPowerGA.Amount, enemyAlterPowerGA.DynamicAmount, enemyAlterPowerGA.passive, enemyAlterPowerGA.permaTypes, null, null, enemyAlterPowerGA.targetMode));
            }
            else
            {
                if (enemyAlterPowerGA.playerTargets != null && enemyAlterPowerGA.playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new AlterPowerGA(enemyAlterPowerGA.Amount, enemyAlterPowerGA.DynamicAmount, enemyAlterPowerGA.passive, enemyAlterPowerGA.permaTypes, enemyAlterPowerGA.playerTargets, null));

                if (enemyAlterPowerGA.enemyTargets != null && enemyAlterPowerGA.enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new AlterPowerGA(enemyAlterPowerGA.Amount, enemyAlterPowerGA.DynamicAmount, enemyAlterPowerGA.passive, enemyAlterPowerGA.permaTypes, null, enemyAlterPowerGA.enemyTargets));
            }
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            if (enemyAlterPowerGA.passive)
            {
                ActionSystem.Instance.AddReaction(new AlterPowerGA(enemyAlterPowerGA.Amount, enemyAlterPowerGA.DynamicAmount, enemyAlterPowerGA.passive, enemyAlterPowerGA.permaTypes, null, null, enemyAlterPowerGA.targetMode));
            }
            else
            {
                if (enemyAlterPowerGA.playerTargets != null && enemyAlterPowerGA.playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new AlterPowerGA(enemyAlterPowerGA.Amount, enemyAlterPowerGA.DynamicAmount, enemyAlterPowerGA.passive, enemyAlterPowerGA.permaTypes, enemyAlterPowerGA.playerTargets, null));

                if (enemyAlterPowerGA.enemyTargets != null && enemyAlterPowerGA.enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new AlterPowerGA(enemyAlterPowerGA.Amount, enemyAlterPowerGA.DynamicAmount, enemyAlterPowerGA.passive, enemyAlterPowerGA.permaTypes, null, enemyAlterPowerGA.enemyTargets));
            }
        }
    }

    private IEnumerator AlterStamEnemyPerformer(EnemyAlterStaminaGA enemyAlterStaminaGA)
    {
        if (enemyAlterStaminaGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyAlterStaminaGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (enemyAlterStaminaGA.playerTargets != null && enemyAlterStaminaGA.playerTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new AlterStaminaGA(enemyAlterStaminaGA.Amount, enemyAlterStaminaGA.DynamicAmount, enemyAlterStaminaGA.permaTypes, enemyAlterStaminaGA.playerTargets, null));

            if (enemyAlterStaminaGA.enemyTargets != null && enemyAlterStaminaGA.enemyTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new AlterStaminaGA(enemyAlterStaminaGA.Amount, enemyAlterStaminaGA.DynamicAmount, enemyAlterStaminaGA.permaTypes, null, enemyAlterStaminaGA.enemyTargets));
            
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            if (enemyAlterStaminaGA.playerTargets != null && enemyAlterStaminaGA.playerTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new AlterStaminaGA(enemyAlterStaminaGA.Amount, enemyAlterStaminaGA.DynamicAmount, enemyAlterStaminaGA.permaTypes, enemyAlterStaminaGA.playerTargets, null));

            if (enemyAlterStaminaGA.enemyTargets != null && enemyAlterStaminaGA.enemyTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new AlterStaminaGA(enemyAlterStaminaGA.Amount, enemyAlterStaminaGA.DynamicAmount, enemyAlterStaminaGA.permaTypes, null, enemyAlterStaminaGA.enemyTargets));
        }
    }

    private IEnumerator LifeLossEnemyPerformer(EnemyLifeLossGA enemyLifeLossGA)
    {
        if (enemyLifeLossGA.Actionner != null)
        {
            PermanentView Attacker = enemyLifeLossGA.Actionner.GetComponent<PermanentView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (enemyLifeLossGA.playerTargets != null && enemyLifeLossGA.playerTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new LifeLossGA(enemyLifeLossGA.Amount, enemyLifeLossGA.DynamicAmount, enemyLifeLossGA.playerTargets, null));

            if (enemyLifeLossGA.enemyTargets != null && enemyLifeLossGA.enemyTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new LifeLossGA(enemyLifeLossGA.Amount, enemyLifeLossGA.DynamicAmount, null, enemyLifeLossGA.enemyTargets));
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            if (enemyLifeLossGA.playerTargets != null && enemyLifeLossGA.playerTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new LifeLossGA(enemyLifeLossGA.Amount, enemyLifeLossGA.DynamicAmount, enemyLifeLossGA.playerTargets, null));

            if (enemyLifeLossGA.enemyTargets != null && enemyLifeLossGA.enemyTargets.Count > 0)
                ActionSystem.Instance.AddReaction(new LifeLossGA(enemyLifeLossGA.Amount, enemyLifeLossGA.DynamicAmount, null, enemyLifeLossGA.enemyTargets));
        }
    }

    private IEnumerator GainHPEnemyPerformer(EnemyGainLifeGA enemyGainLifeGA)
    {
        if (enemyGainLifeGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyGainLifeGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);
            if (enemyGainLifeGA.passive)
            {
                ActionSystem.Instance.AddReaction(new GainLifeGA(enemyGainLifeGA.Amount, enemyGainLifeGA.DynamicAmount, enemyGainLifeGA.passive, enemyGainLifeGA.permaTypes, null, null, enemyGainLifeGA.targetMode));
            }
            else
            {
                if (enemyGainLifeGA.playerTargets != null && enemyGainLifeGA.playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new GainLifeGA(enemyGainLifeGA.Amount, enemyGainLifeGA.DynamicAmount, enemyGainLifeGA.passive, enemyGainLifeGA.permaTypes, enemyGainLifeGA.playerTargets, null));

                if (enemyGainLifeGA.enemyTargets != null && enemyGainLifeGA.enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new GainLifeGA(enemyGainLifeGA.Amount, enemyGainLifeGA.DynamicAmount, enemyGainLifeGA.passive, enemyGainLifeGA.permaTypes, null, enemyGainLifeGA.enemyTargets));
            }
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            if (enemyGainLifeGA.passive)
            {
                ActionSystem.Instance.AddReaction(new GainLifeGA(enemyGainLifeGA.Amount, enemyGainLifeGA.DynamicAmount, enemyGainLifeGA.passive, enemyGainLifeGA.permaTypes, null, null, enemyGainLifeGA.targetMode));
            }
            else
            {
                if (enemyGainLifeGA.playerTargets != null && enemyGainLifeGA.playerTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new GainLifeGA(enemyGainLifeGA.Amount, enemyGainLifeGA.DynamicAmount, enemyGainLifeGA.passive, enemyGainLifeGA.permaTypes, enemyGainLifeGA.playerTargets, null));

                if (enemyGainLifeGA.enemyTargets != null && enemyGainLifeGA.enemyTargets.Count > 0)
                    ActionSystem.Instance.AddReaction(new GainLifeGA(enemyGainLifeGA.Amount, enemyGainLifeGA.DynamicAmount, enemyGainLifeGA.passive, enemyGainLifeGA.permaTypes, null, enemyGainLifeGA.enemyTargets));
            }
        }
    }

    private IEnumerator PerformIntentConstructPerformer(SpawnConstructGA spawnConstructGA)
    {
        if (!CombatSystem.Instance.Win)
        {
            if (enemyView.IntentConstructs != null || enemyView.IntentConstructs.Count != 0)
            {
                if (enemyView.ConstructSequence != null || enemyView.ConstructSequence.Count != 0)
                {

                    bool SequenceFinished = false;

                    if (enemyView.sequenceIndex >= enemyView.ConstructSequence.Count)
                    {
                        if (enemyView.LoopingSequence)
                        {
                            enemyView.sequenceIndex = 0;
                        }
                        else
                        {
                            SequenceFinished = true;
                        }
                    }

                    if (!SequenceFinished)
                    {
                        string currentKey = enemyView.ConstructSequence[enemyView.sequenceIndex];
                        if (currentKey != "")
                        {
                            IntentConstruct selected = enemyView.IntentConstructs.Find(ic => ic.number == currentKey);

                            if (selected == null)
                            {
                                Debug.LogWarning($"No IntentConstruct found for key '{currentKey}'");
                            }
                            else
                            {
                                foreach (EnemyPermanentData data in selected.EnemyData)
                                {
                                    EnemySlotViewCreator.Instance.CreateEnemySlotViewCreator(data, data.permanentArea, false, enemyView);
                                }
                            }
                        }
                    }
                }
            }
        }

        enemyView.sequenceIndex++;
        yield return null;
    }

    private IEnumerator InvocEPerformer(InvocEGA invocEGA)
    {
        if (invocEGA.Actionner != null)
        {
            EnemySlotView Attacker = invocEGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            InvocGA invocGA = new(invocEGA.Amount, invocEGA.DynamicAmount, null, invocEGA.EnemyToInvoc);
            ActionSystem.Instance.AddReaction(invocGA);
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            InvocGA invocGA = new(invocEGA.Amount, invocEGA.DynamicAmount, null, invocEGA.EnemyToInvoc);
            ActionSystem.Instance.AddReaction(invocGA);
        }
        yield return null;
    }

    private IEnumerator SacEPerformer(SacEGA sacEGA)
    {
        if (sacEGA.Actionner != null)
        {
            EnemySlotView Attacker = sacEGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            SacGA sacGA = new(sacEGA.playerTargets,sacEGA.enemyTargets);
            ActionSystem.Instance.AddReaction(sacGA);
        }
        // dans le cas ou il n'y a pas de d'actionner c'est que c'est une attaque non directe mais du a un effet spécifique qui n'est pas cancel en cas de mort
        else
        {
            SacGA sacGA = new(sacEGA.playerTargets,sacEGA.enemyTargets);
            ActionSystem.Instance.AddReaction(sacGA);
        }
        yield return null;
    }

    // REACTIONS

    private void BeforeAttackPreReaction(AttackPlayerGA attackPlayerGA)
    {
        if (attackPlayerGA.Actionner != null)
        {
            EnemySlotView Attacker = attackPlayerGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeHealPreReaction(HealEnemyGA healEnemyGA)
    {
        if (healEnemyGA.Actionner != null)
        {
            EnemySlotView Attacker = healEnemyGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeShieldPreReaction(ShieldEnemyGA shieldEnemyGA)
    {
        if (shieldEnemyGA.Actionner != null)
        {
            EnemySlotView Attacker = shieldEnemyGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeAlterPreReaction(EnemyAlterPowerGA enemyAlterPowerGA)
    {
        if (enemyAlterPowerGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyAlterPowerGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeAlterStamPreReaction(EnemyAlterStaminaGA enemyAlterStaminaGA)
    {
        if (enemyAlterStaminaGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyAlterStaminaGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeLifeLossPreReaction(EnemyLifeLossGA enemyLifeLossGA)
    {
        if (enemyLifeLossGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyLifeLossGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeGainHPPreReaction(EnemyGainLifeGA enemyGainLifeGA)
    {
        if (enemyGainLifeGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyGainLifeGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeInvocEPerformerPreReaction(InvocEGA invocEGA)
    {
        if (invocEGA.Actionner != null)
        {
            EnemySlotView Attacker = invocEGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeSacEPerformerPreReaction(SacEGA sacEGA)
    {
        if (sacEGA.Actionner != null)
        {
            EnemySlotView Attacker = sacEGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeUnShieldEPerformerPreReaction(EnemyUnShieldGA enemyUnShieldGA)
    {
        if (enemyUnShieldGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyUnShieldGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }
    
    private void BeforeRefreshEPerformerPreReaction(EnemyRefreshGA enemyRefreshGA)
    {
        if (enemyRefreshGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyRefreshGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }
}
