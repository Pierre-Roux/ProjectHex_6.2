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
        ActionSystem.AttachPerformer<EnemyAlterCardCostGA>(AlterCardCostEnemyPerformer);
        ActionSystem.AttachPerformer<EnemyLifeLossGA>(LifeLossEnemyPerformer);
        ActionSystem.AttachPerformer<EnemyGainLifeGA>(GainHPEnemyPerformer);
        ActionSystem.AttachPerformer<InvocEGA>(InvocEPerformer);
        ActionSystem.AttachPerformer<SacEGA>(SacEPerformer);
        ActionSystem.AttachPerformer<EnemyRefreshGA>(RefreshEnemyPerformer);
        ActionSystem.AttachPerformer<EnemyExhaustGA>(ExhaustEnemyPerformer);
        ActionSystem.AttachPerformer<EnemyRetrieveExhaustedGA>(EnemyRetrieveExhaustedPerformer);

        ActionSystem.AttachPerformer<SpawnConstructGA>(PerformIntentConstructPerformer);

        ActionSystem.SubscribeReaction<AttackPlayerGA>(BeforeAttackPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<HealEnemyGA>(BeforeHealPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<ShieldEnemyGA>(BeforeShieldPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyUnShieldGA>(BeforeUnShieldEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyAlterPowerGA>(BeforeAlterPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyAlterStaminaGA>(BeforeAlterStamPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyAlterCardCostGA>(BeforeAlterCardCostPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyLifeLossGA>(BeforeLifeLossPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyGainLifeGA>(BeforeGainHPPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<InvocEGA>(BeforeInvocEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<SacEGA>(BeforeSacEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyRefreshGA>(BeforeRefreshEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyExhaustGA>(BeforeExhaustedPPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyRetrieveExhaustedGA>(BeforeRetrieveExhaustedPPerformerPreReaction, ReactionTiming.PRE);

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
        ActionSystem.DetachPerformer<EnemyAlterCardCostGA>();
        ActionSystem.DetachPerformer<EnemyLifeLossGA>();
        ActionSystem.DetachPerformer<EnemyGainLifeGA>();
        ActionSystem.DetachPerformer<InvocEGA>();
        ActionSystem.DetachPerformer<SacEGA>();
        ActionSystem.DetachPerformer<EnemyRefreshGA>();
        ActionSystem.DetachPerformer<EnemyExhaustGA>();
        ActionSystem.DetachPerformer<EnemyRetrieveExhaustedGA>();

        ActionSystem.DetachPerformer<SpawnConstructGA>();

        ActionSystem.UnsubscribeReaction<AttackPlayerGA>(BeforeAttackPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<HealEnemyGA>(BeforeHealPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<ShieldEnemyGA>(BeforeShieldPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyUnShieldGA>(BeforeUnShieldEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyAlterPowerGA>(BeforeAlterPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyAlterStaminaGA>(BeforeAlterStamPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyAlterCardCostGA>(BeforeAlterCardCostPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyLifeLossGA>(BeforeLifeLossPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyGainLifeGA>(BeforeGainHPPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<InvocEGA>(BeforeInvocEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<SacEGA>(BeforeSacEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyRefreshGA>(BeforeRefreshEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyExhaustGA>(BeforeExhaustedPPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyRetrieveExhaustedGA>(BeforeRetrieveExhaustedPPerformerPreReaction, ReactionTiming.PRE);
    }


    // Performers
    private IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurnGA)
    {
        var intents = CombatSystem.Instance.Enemy_Permanents
            .Where(e => e.IntentAction != null && e.IntentAction.Events.Contains(Events.EnemyTurn))
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
                                    EnemySlotViewCreator.Instance.CreateEnemySlotViewCreator(data, data.permanentArea, false);
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

    private IEnumerator AttackPlayerPerformer(AttackPlayerGA attackPlayerGA)
    {
        int DamageBonus = 0;

        if (attackPlayerGA.Actionner != null)
        {
            EnemySlotView Attacker = attackPlayerGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            DamageBonus = Attacker.CalculateBonusPower();
        }    

        if (attackPlayerGA.playerTargets != null && attackPlayerGA.playerTargets.Count > 0)
        {
            DealDamageGA dealDamageGA = new(attackPlayerGA.Damage, DamageBonus, attackPlayerGA.multiplyAmount, attackPlayerGA.DynamicAmount, attackPlayerGA.playerTargets, null);
            dealDamageGA.Actionner = attackPlayerGA.Actionner;
            dealDamageGA.SourceEffect = attackPlayerGA.SourceEffect;
            dealDamageGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(dealDamageGA);
        }

        if (attackPlayerGA.enemyTargets != null && attackPlayerGA.enemyTargets.Count > 0)
        {
            DealDamageGA dealDamageGA = new(attackPlayerGA.Damage,DamageBonus,attackPlayerGA.multiplyAmount, attackPlayerGA.DynamicAmount, null, attackPlayerGA.enemyTargets);
            dealDamageGA.Actionner = attackPlayerGA.Actionner;
            dealDamageGA.SourceEffect = attackPlayerGA.SourceEffect;
            dealDamageGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(dealDamageGA);
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
        }

        if (healEnemyGA.playerTargets != null && healEnemyGA.playerTargets.Count > 0)
        {
            HealGA healGA = new HealGA(healEnemyGA.HealAmount, healEnemyGA.multiplyAmount, healEnemyGA.DynamicAmount, healEnemyGA.playerTargets, null);
            healGA.SourceEffect = healEnemyGA.SourceEffect;
            healGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(healGA);
        }

        if (healEnemyGA.enemyTargets != null && healEnemyGA.enemyTargets.Count > 0)
        {
            HealGA healGA = new HealGA(healEnemyGA.HealAmount, healEnemyGA.multiplyAmount, healEnemyGA.DynamicAmount, null, healEnemyGA.enemyTargets);
            healGA.SourceEffect = healEnemyGA.SourceEffect;
            healGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(healGA);
        }
    }

    private IEnumerator EnemyRetrieveExhaustedPerformer(EnemyRetrieveExhaustedGA enemyRetrieveExhaustedGA)
    {
        if (enemyRetrieveExhaustedGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyRetrieveExhaustedGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);
        }

        if (enemyRetrieveExhaustedGA.cardTargets != null && enemyRetrieveExhaustedGA.cardTargets.Count > 0)
        {
            RetrieveExhaustedGA retrieveExhaustedGA = new RetrieveExhaustedGA(enemyRetrieveExhaustedGA.cardTargets);
            retrieveExhaustedGA.SourceEffect = enemyRetrieveExhaustedGA.SourceEffect;
            retrieveExhaustedGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(retrieveExhaustedGA);
        }
    }
    
    private IEnumerator ExhaustEnemyPerformer(EnemyExhaustGA enemyExhaustGA)
    {
        if (enemyExhaustGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyExhaustGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);
        }

        if (enemyExhaustGA.playerTargets != null && enemyExhaustGA.playerTargets.Count > 0)
        {
            ExhaustGA exhaustGA = new ExhaustGA(enemyExhaustGA.playerTargets, null, null, enemyExhaustGA.targetModeInfo);
            exhaustGA.SourceEffect = enemyExhaustGA.SourceEffect;
            exhaustGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(exhaustGA);
        }

        if (enemyExhaustGA.cardTargets != null && enemyExhaustGA.cardTargets.Count > 0)
        {
            ExhaustGA exhaustGA = new ExhaustGA(null, null, enemyExhaustGA.cardTargets, enemyExhaustGA.targetModeInfo);
            exhaustGA.SourceEffect = enemyExhaustGA.SourceEffect;
            exhaustGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(exhaustGA);
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
                shieldGA.SourceEffect = shieldEnemyGA.SourceEffect;
                shieldGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(shieldGA);
            }

            if (shieldEnemyGA.enemyTargets != null && shieldEnemyGA.enemyTargets.Count > 0)
            {
                ShieldGA shieldGA = new ShieldGA(null, shieldEnemyGA.enemyTargets);
                shieldGA.Actionner = shieldEnemyGA.Actionner;
                shieldGA.SourceEffect = shieldEnemyGA.SourceEffect;
                shieldGA.ActivateToolTip = false;
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
                shieldGA.SourceEffect = enemyUnShieldGA.SourceEffect;
                shieldGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(shieldGA);
            }

            if (enemyUnShieldGA.enemyTargets != null && enemyUnShieldGA.enemyTargets.Count > 0)
            {
                UnShieldGA shieldGA = new UnShieldGA(null, enemyUnShieldGA.enemyTargets);
                shieldGA.Actionner = enemyUnShieldGA.Actionner;
                shieldGA.SourceEffect = enemyUnShieldGA.SourceEffect;
                shieldGA.ActivateToolTip = false;
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
                RefreshGA refreshGA = new RefreshGA(enemyRefreshGA.playerTargets, null);
                refreshGA.Actionner = enemyRefreshGA.Actionner;
                refreshGA.SourceEffect = enemyRefreshGA.SourceEffect;
                refreshGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(refreshGA);
            }

            if (enemyRefreshGA.enemyTargets != null && enemyRefreshGA.enemyTargets.Count > 0)
            {
                RefreshGA refreshGA = new RefreshGA(null, enemyRefreshGA.enemyTargets);
                refreshGA.Actionner = enemyRefreshGA.Actionner;
                refreshGA.SourceEffect = enemyRefreshGA.SourceEffect;
                refreshGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(refreshGA);
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
        }

        if (enemyAlterPowerGA.passive)
        {
            AlterPowerGA alterPowerGA = new AlterPowerGA(enemyAlterPowerGA.Amount, enemyAlterPowerGA.multiplyAmount, enemyAlterPowerGA.DynamicAmount, enemyAlterPowerGA.passive, enemyAlterPowerGA.aditive, null, null, enemyAlterPowerGA.targetModeInfo);
            alterPowerGA.SourceEffect = enemyAlterPowerGA.SourceEffect;
            alterPowerGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(alterPowerGA);
        }
        else
        {
            if (enemyAlterPowerGA.playerTargets != null && enemyAlterPowerGA.playerTargets.Count > 0)
            {
                AlterPowerGA alterPowerGA = new AlterPowerGA(enemyAlterPowerGA.Amount, enemyAlterPowerGA.multiplyAmount, enemyAlterPowerGA.DynamicAmount, enemyAlterPowerGA.passive, enemyAlterPowerGA.aditive, enemyAlterPowerGA.playerTargets, null);
                alterPowerGA.SourceEffect = enemyAlterPowerGA.SourceEffect;
                alterPowerGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(alterPowerGA);
            }

            if (enemyAlterPowerGA.enemyTargets != null && enemyAlterPowerGA.enemyTargets.Count > 0)
            {
                AlterPowerGA alterPowerGA = new AlterPowerGA(enemyAlterPowerGA.Amount, enemyAlterPowerGA.multiplyAmount, enemyAlterPowerGA.DynamicAmount, enemyAlterPowerGA.passive, enemyAlterPowerGA.aditive, null, enemyAlterPowerGA.enemyTargets);
                alterPowerGA.SourceEffect = enemyAlterPowerGA.SourceEffect;
                alterPowerGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(alterPowerGA);
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
        }

        if (enemyAlterStaminaGA.passive)
        {
            AlterStaminaGA alterStaminaGA = new AlterStaminaGA(enemyAlterStaminaGA.Amount, enemyAlterStaminaGA.multiplyAmount, enemyAlterStaminaGA.DynamicAmount, enemyAlterStaminaGA.passive, enemyAlterStaminaGA.aditive, null, null, null, enemyAlterStaminaGA.targetModeInfo);
            alterStaminaGA.SourceEffect = enemyAlterStaminaGA.SourceEffect;
            alterStaminaGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(alterStaminaGA);
        }
        else
        {
            if (enemyAlterStaminaGA.playerTargets != null && enemyAlterStaminaGA.playerTargets.Count > 0)
            {
                AlterStaminaGA alterStaminaGA = new AlterStaminaGA(enemyAlterStaminaGA.Amount, enemyAlterStaminaGA.multiplyAmount, enemyAlterStaminaGA.DynamicAmount, enemyAlterStaminaGA.passive, enemyAlterStaminaGA.aditive, enemyAlterStaminaGA.playerTargets, null, null, enemyAlterStaminaGA.targetModeInfo);
                alterStaminaGA.SourceEffect = enemyAlterStaminaGA.SourceEffect;
                alterStaminaGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(alterStaminaGA);
            }

            if (enemyAlterStaminaGA.enemyTargets != null && enemyAlterStaminaGA.enemyTargets.Count > 0)
            {
                AlterStaminaGA alterStaminaGA = new AlterStaminaGA(enemyAlterStaminaGA.Amount, enemyAlterStaminaGA.multiplyAmount, enemyAlterStaminaGA.DynamicAmount, enemyAlterStaminaGA.passive, enemyAlterStaminaGA.aditive, null, enemyAlterStaminaGA.enemyTargets, null, enemyAlterStaminaGA.targetModeInfo);
                alterStaminaGA.SourceEffect = enemyAlterStaminaGA.SourceEffect;
                alterStaminaGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(alterStaminaGA);
            }

            if (enemyAlterStaminaGA.cardTargets != null && enemyAlterStaminaGA.cardTargets.Count > 0)
            {
                AlterStaminaGA alterStaminaGA = new AlterStaminaGA(enemyAlterStaminaGA.Amount, enemyAlterStaminaGA.multiplyAmount, enemyAlterStaminaGA.DynamicAmount, enemyAlterStaminaGA.passive, enemyAlterStaminaGA.aditive, null, null, enemyAlterStaminaGA.cardTargets, enemyAlterStaminaGA.targetModeInfo);
                alterStaminaGA.SourceEffect = enemyAlterStaminaGA.SourceEffect;
                alterStaminaGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(alterStaminaGA);
            }
        }       
    }

    private IEnumerator AlterCardCostEnemyPerformer(EnemyAlterCardCostGA enemyAlterCardCostGA)
    {
        if (enemyAlterCardCostGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyAlterCardCostGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);
        }

        if (enemyAlterCardCostGA.passive)
        {
            AlterCardCostGA alterCardCostGA = new AlterCardCostGA(enemyAlterCardCostGA.Amount, enemyAlterCardCostGA.multiplyAmount, enemyAlterCardCostGA.DynamicAmount, enemyAlterCardCostGA.passive, null, enemyAlterCardCostGA.targetModeInfo);
            alterCardCostGA.SourceEffect = enemyAlterCardCostGA.SourceEffect;
            alterCardCostGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(alterCardCostGA);
        }
        else
        {
            if (enemyAlterCardCostGA.cardTargets != null && enemyAlterCardCostGA.cardTargets.Count > 0)
            {
                AlterCardCostGA alterCardCostGA = new AlterCardCostGA(enemyAlterCardCostGA.Amount, enemyAlterCardCostGA.multiplyAmount, enemyAlterCardCostGA.DynamicAmount, enemyAlterCardCostGA.passive, enemyAlterCardCostGA.cardTargets, enemyAlterCardCostGA.targetModeInfo);
                alterCardCostGA.SourceEffect = enemyAlterCardCostGA.SourceEffect;
                alterCardCostGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(alterCardCostGA);
            }
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
        }

        if (enemyLifeLossGA.playerTargets != null && enemyLifeLossGA.playerTargets.Count > 0)
        {
            LifeLossGA lifeLossGA = new LifeLossGA(enemyLifeLossGA.Amount, enemyLifeLossGA.multiplyAmount, enemyLifeLossGA.DynamicAmount, enemyLifeLossGA.playerTargets, null);
            lifeLossGA.SourceEffect = enemyLifeLossGA.SourceEffect;
            lifeLossGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(lifeLossGA);
        }
            
        if (enemyLifeLossGA.enemyTargets != null && enemyLifeLossGA.enemyTargets.Count > 0)
        {
            LifeLossGA lifeLossGA = new LifeLossGA(enemyLifeLossGA.Amount, enemyLifeLossGA.multiplyAmount, enemyLifeLossGA.DynamicAmount, null, enemyLifeLossGA.enemyTargets);
            lifeLossGA.SourceEffect = enemyLifeLossGA.SourceEffect;
            lifeLossGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(lifeLossGA);
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
        }

        if (enemyGainLifeGA.passive)
        {
            GainLifeGA gainLifeGA = new GainLifeGA(enemyGainLifeGA.Amount, enemyGainLifeGA.multiplyAmount, enemyGainLifeGA.DynamicAmount, enemyGainLifeGA.passive, enemyGainLifeGA.aditive, null, null, enemyGainLifeGA.targetModeInfo);
            gainLifeGA.SourceEffect = enemyGainLifeGA.SourceEffect;
            gainLifeGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(gainLifeGA);
        }
        else
        {
            if (enemyGainLifeGA.playerTargets != null && enemyGainLifeGA.playerTargets.Count > 0)
            {
                GainLifeGA gainLifeGA = new GainLifeGA(enemyGainLifeGA.Amount,enemyGainLifeGA.multiplyAmount, enemyGainLifeGA.DynamicAmount, enemyGainLifeGA.passive, enemyGainLifeGA.aditive, enemyGainLifeGA.playerTargets, null);
                gainLifeGA.SourceEffect = enemyGainLifeGA.SourceEffect;
                gainLifeGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(gainLifeGA);                
            }

            if (enemyGainLifeGA.enemyTargets != null && enemyGainLifeGA.enemyTargets.Count > 0)
            {
                GainLifeGA gainLifeGA = new GainLifeGA(enemyGainLifeGA.Amount,enemyGainLifeGA.multiplyAmount, enemyGainLifeGA.DynamicAmount, enemyGainLifeGA.passive, enemyGainLifeGA.aditive, null, enemyGainLifeGA.enemyTargets);
                gainLifeGA.SourceEffect = enemyGainLifeGA.SourceEffect;
                gainLifeGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(gainLifeGA);
            }
        }
    }

    private IEnumerator InvocEPerformer(InvocEGA invocEGA)
    {
        if (invocEGA.Actionner != null)
        {
            EnemySlotView Attacker = invocEGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);
        }

        InvocGA invocGA = new(invocEGA.Amount, invocEGA.multiplyAmount, invocEGA.DynamicAmount, null, invocEGA.EnemyToInvoc);
        invocGA.SourceEffect = invocEGA.SourceEffect;
        invocGA.ActivateToolTip = false;
        ActionSystem.Instance.AddReaction(invocGA);
    }

    private IEnumerator SacEPerformer(SacEGA sacEGA)
    {
        if (sacEGA.Actionner != null)
        {
            EnemySlotView Attacker = sacEGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);
        }

        SacGA sacGA = new(sacEGA.playerTargets, sacEGA.enemyTargets);
        sacGA.SourceEffect = sacEGA.SourceEffect;
        sacGA.ActivateToolTip = false;
        ActionSystem.Instance.AddReaction(sacGA);
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

    private void BeforeAlterCardCostPreReaction(EnemyAlterCardCostGA enemyAlterCardCostGA)
    {
        if (enemyAlterCardCostGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyAlterCardCostGA.Actionner.GetComponent<EnemySlotView>();
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

    private void BeforeRetrieveExhaustedPPerformerPreReaction(EnemyRetrieveExhaustedGA enemyRetrieveExhaustedGA)
    {
        if (enemyRetrieveExhaustedGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyRetrieveExhaustedGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }
    
    private void BeforeExhaustedPPerformerPreReaction(EnemyExhaustGA enemyExhaustGA)
    {
        if (enemyExhaustGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyExhaustGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }
}
