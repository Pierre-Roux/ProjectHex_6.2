using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public enum TargetingMode
{
    Soft,
    Preview,
    Hard
}

public class EnemySystem : Singleton<EnemySystem>
{
    public EnemyView enemyView; 

    //bool ForceHardTargeting = false;
    private const int MaxRemapTries = 10;
    public bool EnemyTurnInProgress;

    void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        ActionSystem.AttachPerformer<ExecEnemyTurnActionOnceGA>(ExecuteOnceEnemyTurnAction);
        ActionSystem.AttachPerformer<AttackPlayerGA>(AttackPlayerPerformer);
        ActionSystem.AttachPerformer<HealEnemyGA>(HealEnemyPerformer);
        ActionSystem.AttachPerformer<ArmorEnemyGA>(ArmorEnemyPerformer);
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
        ActionSystem.AttachPerformer<EnemyAlterPowerGridGA>(AlterPowerGridEnemyPerformer);
        ActionSystem.AttachPerformer<EnemyAddACopyGa>(EnemyAddACopyPlayerPerformer);
        ActionSystem.AttachPerformer<EnemyDisableGA>(DisableEnemyPerformer);
        ActionSystem.AttachPerformer<EnemyEnableGA>(EnableEnemyPerformer);

        ActionSystem.AttachPerformer<SpawnConstructGA>(PerformIntentConstructPerformer);

        ActionSystem.SubscribeReaction<AttackPlayerGA>(BeforeAttackPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<HealEnemyGA>(BeforeHealPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<ArmorEnemyGA>(BeforeArmorPreReaction, ReactionTiming.PRE);
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
        ActionSystem.SubscribeReaction<EnemyExhaustGA>(BeforeExhaustedEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyRetrieveExhaustedGA>(BeforeRetrieveExhaustedEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyAlterPowerGridGA>(BeforeAlterPowerGridEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyAddACopyGa>(BeforeAddACopyEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyDisableGA>(BeforeDisableEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyEnableGA>(BeforeEnableEPerformerPreReaction, ReactionTiming.PRE);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<ExecEnemyTurnActionOnceGA>();
        ActionSystem.DetachPerformer<AttackPlayerGA>();
        ActionSystem.DetachPerformer<HealEnemyGA>();
        ActionSystem.DetachPerformer<ArmorEnemyGA>();
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
        ActionSystem.DetachPerformer<EnemyAlterPowerGridGA>();
        ActionSystem.DetachPerformer<EnemyAddACopyGa>();
        ActionSystem.DetachPerformer<EnemyDisableGA>();
        ActionSystem.DetachPerformer<EnemyEnableGA>();

        ActionSystem.DetachPerformer<SpawnConstructGA>();

        ActionSystem.UnsubscribeReaction<AttackPlayerGA>(BeforeAttackPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<HealEnemyGA>(BeforeHealPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<ArmorEnemyGA>(BeforeArmorPreReaction, ReactionTiming.PRE);
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
        ActionSystem.UnsubscribeReaction<EnemyExhaustGA>(BeforeExhaustedEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyRetrieveExhaustedGA>(BeforeRetrieveExhaustedEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyAlterPowerGridGA>(BeforeAlterPowerGridEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyAddACopyGa>(BeforeAddACopyEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyDisableGA>(BeforeDisableEPerformerPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyEnableGA>(BeforeEnableEPerformerPreReaction, ReactionTiming.PRE);
    }


    // Performers
    private IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurnGA)
    {
        EnemyTurnInProgress = true;

        EventInfo eventInfo = new EventInfo(Events.StartTurn, Enemy_Player_ENUM.Enemy, KeyWordType.NULL);
        TriggerEventGA triggerEventGA = new(eventInfo, null, null, null, null);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        List<Effect> EffectToExec = new();
        var intents = CombatSystem.Instance.Enemy_Permanents
            .Where(e => e.IntentAction != null && e.IntentAction.EventInfos[0].Events == Events.EnemyTurn)
            .Select(e => e.IntentAction)
            .ToList();

        foreach (var intent in intents)
        {
            // Vérifier que l'ennemi existe encore et est valide
            var enemySlotViewGO = intent.Actionner;
            EnemySlotView enemySlotView = null;
            if (enemySlotViewGO != null)
            {
                enemySlotView = enemySlotViewGO.GetComponent<EnemySlotView>();
            }

            if (enemySlotView == null || !CombatSystem.Instance.Enemy_Permanents.Contains(enemySlotView))
                continue;

            if (GameEventSystem.Instance.CheckDisabledState(intent))
                continue;

            if (intent is EffectGroup choiceEffect)
            {
                foreach (Effect SubEffect in choiceEffect.EffectGroups)
                {
                    SubEffect.Actionner = intent.Actionner;
                    SubEffect.CardActionner = intent.CardActionner;
                    QueueEnemyTurnEffect(SubEffect.Clone(), EffectToExec);
                }
            }
            else
            {
                QueueEnemyTurnEffect(intent.Clone(), EffectToExec);
            }

            if (CombatSystem.Instance.Enemy_Permanents.Contains(enemySlotView))
                enemySlotView.UpdateIntent();
        }

        ExecEnemyTurnActionOnceGA execEnemyTurnActionOnceGA = new(EffectToExec);
        ActionSystem.Instance.AddReaction(execEnemyTurnActionOnceGA);
        yield return null;
    }

    private static void QueueEnemyTurnEffect(Effect effectToExecute, List<Effect> effectsToExec)
    {
        List<EventInfo> events = effectToExecute.EventInfos;

        if (events.Count == 1 && events[0].Events == Events.EnemyTurn)
        {
            effectToExecute.EventInfos = new List<EventInfo>
            {
                new EventInfo(Events.Instant, Enemy_Player_ENUM.NULL, KeyWordType.NULL)
            };
            effectsToExec.Add(effectToExecute);
        }
        else if (events.Count > 1)
        {
            int removed = events.RemoveAll(e => e.Events == Events.EnemyTurn);
            if (removed > 0)
                effectsToExec.Add(effectToExecute);
        }
        else
        {
            effectsToExec.Add(effectToExecute);
        }
    }

    public void RemapTarget(bool hardTargeting, bool VisualTargeting = false, PermanentView VisualPermanentView = null)
    {
        TargetingMode mode = hardTargeting ? TargetingMode.Hard
                        : VisualTargeting ? TargetingMode.Preview
                        : TargetingMode.Soft;

        Remap(mode, VisualPermanentView);
    }

    private void Remap(TargetingMode mode, PermanentView focusedTarget)
    {
        bool soft = mode == TargetingMode.Soft;

        if (soft)
            RebuildVirtualState();

        ResetAsGoodTarget();

        // 1) Les sacrifices retirent des unités du plan : ils passent avant tout le reste.
        if (soft)
            PlanSupportEffects(sacrificesPhase: true);

        // 2) Dégâts (inchangé)
        int maxTries = soft ? MaxRemapTries : 1;

        for (int attempt = 0; attempt < maxTries; attempt++)
        {
            bool changed = RemapDealDamageTargets(mode, focusedTarget);

            if (!changed || !AnyDynamicEnemyWithoutTarget())
                break;
        }

        // 3) Soins, boucliers, armure, buffs...
        if (soft)
            PlanSupportEffects(sacrificesPhase: false);

        if (mode == TargetingMode.Hard)
            Remap(TargetingMode.Soft, null);
    }

    // ==================================================================
    //  EFFETS DE SUPPORT (Heal, Shield, Sac, Armor, AlterPower, GainHP...)
    // ==================================================================

    private readonly struct TargetRef
    {
        public readonly PermanentView Player;
        public readonly EnemySlotView Enemy;

        public TargetRef(PermanentView player) { Player = player; Enemy = null; }
        public TargetRef(EnemySlotView enemy) { Player = null; Enemy = enemy; }
    }

    /// <summary>Ce que les ennemis ont déjà prévu pendant ce remap (pour l'unicité).</summary>
    private sealed class SupportPlan
    {
        private readonly Dictionary<PermanentView, int> healPlayers = new();
        private readonly Dictionary<EnemySlotView, int> healEnemies = new();

        public int GetHealed(PermanentView p) => healPlayers.TryGetValue(p, out int v) ? v : 0;
        public int GetHealed(EnemySlotView e) => healEnemies.TryGetValue(e, out int v) ? v : 0;
        public int GetHealed(TargetRef t) => t.Player != null ? GetHealed(t.Player) : GetHealed(t.Enemy);

        public void AddHeal(TargetRef t, int amount)
        {
            if (t.Player != null) healPlayers[t.Player] = GetHealed(t.Player) + amount;
            else healEnemies[t.Enemy] = GetHealed(t.Enemy) + amount;
        }
    }
    /// <param name="sacrificesPhase">true : seulement les SacEffect. false : tous les autres.</param>
    private void PlanSupportEffects(bool sacrificesPhase)
    {
        SupportPlan plan = new();

        foreach (EnemySlotView enemy in CombatSystem.Instance.Enemy_Permanents)
        {
            // Un ennemi sacrifié n'agit plus.
            if (enemy.IsCore || enemy.IsVirtualDead || enemy.IntentAction == null) continue;

            foreach (Effect effect in GetDynamicSupportEffects(enemy))
            {
                if ((effect is SacEffect) != sacrificesPhase) continue;

                List<TargetRef> targets = ChooseSupportTargets(enemy, effect, plan);
                SetEffectTargets(effect, targets);

                foreach (TargetRef target in targets)
                    Claim(effect, enemy, target, plan);
            }
        }
    }

    private static List<TargetRef> ChooseSupportTargets(EnemySlotView enemy, Effect effect, SupportPlan plan)
    {
        List<TargetRef> chosen = new();
        int maxTargets = Mathf.Max(1, effect.EffectTargetNumber);

        if (effect.PriorityList == null) return chosen;

        foreach (PriorityInfo prio in effect.PriorityList)
        {
            var (players, enemies) = TargetSystem.Instance.GetTargetbyPriority(
                prio, effect,
                pv => IsExcludedForSupport(effect, pv, plan),
                es => IsExcludedForSupport(effect, es, plan));

            if (players.Count == 0 && enemies.Count == 0) continue;

            List<TargetRef> candidates = new();
            foreach (PermanentView pv in players) candidates.Add(new TargetRef(pv));
            foreach (EnemySlotView es in enemies) candidates.Add(new TargetRef(es));

            // Soins : on commence par la cible qui a le plus de vie manquante restante.
            if (effect is HealEffect)
            {
                candidates = candidates
                    .OrderByDescending(c => GetMissingLife(c) - plan.GetHealed(c))
                    .ToList();
            }

            for (int i = 0; i < candidates.Count && chosen.Count < maxTargets; i++)
                chosen.Add(candidates[i]);

            break; // première priorité non vide
        }

        return chosen;
    }

    private static void SetEffectTargets(Effect effect, List<TargetRef> targets)
    {
        List<PermanentView> players = new();
        List<EnemySlotView> enemies = new();

        foreach (TargetRef t in targets)
        {
            if (t.Player != null) players.Add(t.Player);
            else if (t.Enemy != null) enemies.Add(t.Enemy);
        }

        effect.PreTargetPermanentView = players;
        effect.PreTargetEnemySlotView = enemies;
    }

    private static bool IsExcludedForSupport(Effect effect, PermanentView target, SupportPlan plan)
    {
        switch (effect)
        {
            case SacEffect:
                return target.IsCore;

            case HealEffect:
                return GetMissingLife(target) - plan.GetHealed(target) <= 0;

            default:
                return false;
        }
    }

    private static bool IsExcludedForSupport(Effect effect, EnemySlotView target, SupportPlan plan)
    {
        switch (effect)
        {
            case SacEffect:
                return target.IsCore || target.TargetLocked;

            case HealEffect:
                return GetMissingLife(target) - plan.GetHealed(target) <= 0;

            default:
                return false;
        }
    }

    /// <summary>Enregistre ce que l'effet vient de réserver, pour que les ennemis suivants l'évitent.</summary>
    private static void Claim(Effect effect, EnemySlotView source, TargetRef target, SupportPlan plan)
    {
        switch (effect)
        {
            case HealEffect heal:
                int healAmount = ResolveAmount(heal.amount, heal.DynamicAmountInfo, source, target.Player, target.Enemy)
                            * heal.multiplyAmount;
                plan.AddHeal(target, healAmount);
                break;

            case SacEffect:
                if (target.Player != null)
                {
                    SacrificedPlayers.Add(target.Player);
                    target.Player.IsVirtualDead = true;
                }
                else
                {
                    target.Enemy.IsVirtualDead = true;
                }
                break;
        }
    }

    private static int GetMissingLife(PermanentView p) => p.MaxLife - p.currentLife;
    private static int GetMissingLife(EnemySlotView e) => e.MaxLife - e.currentLife;
    private static int GetMissingLife(TargetRef t) => t.Player != null ? GetMissingLife(t.Player) : GetMissingLife(t.Enemy);

    private static int ResolveAmount(int baseAmount, DynamicAmountInfo info, EnemySlotView source,
                                    PermanentView targetPlayer, EnemySlotView targetEnemy)
    {
        if (info == null || info.DynamicAmount == DynamicAmount.NULL)
            return baseAmount;

        // TargetParam lit les stats de la cible ; les autres modes partent de l'ennemi qui lance l'effet.
        if (info.DynamicAmount == DynamicAmount.TargetParam)
            return TargetSystem.Instance.GetDynamicAmount(info, targetPlayer, targetEnemy);

        return TargetSystem.Instance.GetDynamicAmount(info, null, source);
    }

    private bool RemapDealDamageTargets(TargetingMode mode, PermanentView focusedTarget)
    {
        bool changed = false;

        foreach (PermanentView target in CombatSystem.Instance.Player_Permanents)
        {
            if (mode != TargetingMode.Soft && target != focusedTarget) continue;

            RefreshVirtualDeath(target);
            if (target.IsVirtualDead) continue;

            List<EnemySlotView> candidates = GetCandidateAttackers(target);

            int remainingHP = GetBaseHP(target) - GetLockedDamage(target);

            (List<EnemySlotView> attackers, bool isLethal) = SelectAttackers(target, candidates, remainingHP);

            changed |= ApplySelection(mode, target, attackers, isLethal);
        }

        return changed;
    }

    private List<EnemySlotView> GetCandidateAttackers(PermanentView target)
    {
        List<EnemySlotView> candidates = new();

        foreach (EnemySlotView enemy in CombatSystem.Instance.Enemy_Permanents)
        {
            if (!IsDynamicDamageEnemy(enemy)) continue;
            if (enemy.TargetLocked) continue;
            if (IsCommittedToAnotherKill(enemy, target)) continue;
            if (!GetPriorityTargets(enemy).Contains(target)) continue;
            if (enemy.IsVirtualDead) continue;

            candidates.Add(enemy);
        }

        return candidates;
    }

    private static List<PermanentView> GetPriorityTargets(EnemySlotView enemy)
    {
        List<Effect> effects = GetDynamicDamageEffects(enemy);
        if (effects.Count == 0) return new List<PermanentView>();

        Effect effect = effects[0];
        if (effect.PriorityList == null) return new List<PermanentView>();

        foreach (PriorityInfo prio in effect.PriorityList)
        {
            var (permanents, slots) = TargetSystem.Instance.GetTargetbyPriority(prio, effect);

            if (permanents.Count > 0 || slots.Count > 0)
                return permanents;
        }

        return new List<PermanentView>();
    }

    private static bool IsCommittedToAnotherKill(EnemySlotView enemy, PermanentView target)
    {
        PermanentView current = enemy.VirtualTarget;
        return current != null && current != target && current.IsVirtualDead;
    }

    private (List<EnemySlotView> attackers, bool isLethal) SelectAttackers(
        PermanentView target, List<EnemySlotView> candidates, int remainingHP)
    {
        if (candidates.Count == 0 || target.IsCore)
            return (candidates, false);

        int count = candidates.Count;
        int[] damages = new int[count];
        int totalDamage = 0;

        for (int i = 0; i < count; i++)
        {
            damages[i] = GetDamage(candidates[i], target);
            totalDamage += damages[i];
        }

        if (totalDamage < remainingHP)
            return (candidates, false);

        int bestMask = 0;
        int bestDamage = int.MaxValue;
        int bestSize = int.MaxValue;
        int maskCount = 1 << count;

        for (int mask = 1; mask < maskCount; mask++)
        {
            int damage = 0;
            int size = 0;

            for (int i = 0; i < count; i++)
            {
                if ((mask & (1 << i)) == 0) continue;
                damage += damages[i];
                size++;
            }

            if (damage < remainingHP) continue;

            if (damage < bestDamage || (damage == bestDamage && size < bestSize))
            {
                bestMask = mask;
                bestDamage = damage;
                bestSize = size;
            }
        }

        List<EnemySlotView> best = new();
        for (int i = 0; i < count; i++)
        {
            if ((bestMask & (1 << i)) != 0)
                best.Add(candidates[i]);
        }

        return (best, true);
    }

    private bool ApplySelection(TargetingMode mode, PermanentView target, List<EnemySlotView> attackers, bool isLethal)
    {
        bool changed = false;

        switch (mode)
        {
            case TargetingMode.Preview:
                foreach (EnemySlotView attacker in attackers)
                {
                    attacker.AsGoodTarget = true;
                    attacker.UpdateLockLogo(true);
                    attacker.UpdateIntentLogoColor(true);
                }
                return false;

            case TargetingMode.Hard:
                foreach (EnemySlotView attacker in attackers)
                    changed |= AssignAttacker(attacker, target, lockTarget: true);

                RefreshVirtualDeath(target);
                return changed;

            default: // Soft
                bool lockLoneKiller = isLethal && attackers.Count == 1;

                foreach (EnemySlotView attacker in attackers)
                {
                    if (!isLethal && !CanClaim(attacker, target))
                        continue;

                    changed |= AssignAttacker(attacker, target, lockLoneKiller);
                }

                if (isLethal)
                    changed |= ReleaseSurplusAttackers(target, attackers);

                RefreshVirtualDeath(target);
                return changed;
        }
    }

    private static bool CanClaim(EnemySlotView enemy, PermanentView target)
    {
        return !enemy.AsGoodTarget || IsTargeting(enemy, target);
    }

    private static bool AssignAttacker(EnemySlotView enemy, PermanentView target, bool lockTarget)
    {
        bool changed = false;

        if (!target.IsCore)
        {
            if (enemy.VirtualTarget != target)
            {
                DetachFromVirtualTarget(enemy);
                enemy.VirtualTarget = target;
                changed = true;
            }
            

            List<EnemySlotView> virtualAttackers = GetVirtualAttackers(target);
            if (!virtualAttackers.Contains(enemy))
                virtualAttackers.Add(enemy);
        }

        if (!IsTargeting(enemy, target))
        {
            SetPreTarget(enemy, target);
            changed = true;
        }

        if (!enemy.AsGoodTarget)
        {
            enemy.AsGoodTarget = true;
            changed = true;
        }

        if (lockTarget && !enemy.TargetLocked)
        {
            enemy.TargetLocked = true;
            enemy.UpdateLockLogo(true);
            enemy.UpdateIntentLogoColor(true);
            changed = true;
        }

        return changed;
    }

    /// <summary>
    /// À appeler quand un permanent du joueur meurt PENDANT SON TOUR (sacrifice, attaque sur sa propre créature...),
    /// APRÈS l'avoir retiré de CombatSystem.Instance.Player_Permanents.
    /// Délocke les ennemis lockés dessus (seul moment, avec la fin du tour ennemi, où les locks tombent)
    /// puis refait un Soft remap.
    /// </summary>
    public void OnPlayerPermanentRemoved(PermanentView removed)
    {
        // Pendant le tour ennemi, les locks sont remis à zéro en fin de tour : on ne touche à rien.
        if (EnemyTurnInProgress) return;

        foreach (EnemySlotView enemy in CombatSystem.Instance.Enemy_Permanents)
        {
            if (!enemy.TargetLocked) continue;

            bool lockedOnRemoved = removed != null && IsTargeting(enemy, removed);
            if (!lockedOnRemoved && !IsLockedOnMissingTarget(enemy)) continue;

            enemy.TargetLocked = false;
            enemy.AsGoodTarget = false;
            SetPreTarget(enemy, null); // sinon il garderait une cible morte si le remap ne lui en trouve pas
            enemy.UpdateLockLogo(false);
            enemy.UpdateIntentLogoColor(false);
        }

        Remap(TargetingMode.Soft, null);
    }
    private static bool IsLockedOnMissingTarget(EnemySlotView enemy)
    {
        foreach (Effect effect in GetDynamicDamageEffects(enemy))
        {
            List<PermanentView> targets = effect.PreTargetPermanentView;
            if (targets == null || targets.Count == 0) return true;

            foreach (PermanentView target in targets)
            {
                if (target == null || !CombatSystem.Instance.Player_Permanents.Contains(target))
                    return true;
            }
        }

        return false;
    }

    private static bool ReleaseSurplusAttackers(PermanentView target, List<EnemySlotView> keep)
    {
        bool changed = false;
        foreach (EnemySlotView enemy in new List<EnemySlotView>(GetVirtualAttackers(target)))
        {
            if (enemy.TargetLocked || keep.Contains(enemy))
                continue;

            ReleaseAttacker(enemy);
            changed = true;
        }

        return changed;
    }

    private static void ReleaseAttacker(EnemySlotView enemy)
    {
        DetachFromVirtualTarget(enemy);
        enemy.AsGoodTarget = false;
        SetPreTarget(enemy, null);
    }

    private static void DetachFromVirtualTarget(EnemySlotView enemy)
    {
        PermanentView previous = enemy.VirtualTarget;
        if (previous == null) return;

        GetVirtualAttackers(previous).Remove(enemy);
        enemy.VirtualTarget = null;
        RefreshVirtualDeath(previous);
    }

    private static readonly HashSet<PermanentView> SacrificedPlayers = new();

    private void RebuildVirtualState()
    {
        var enemies = CombatSystem.Instance.Enemy_Permanents;

        SacrificedPlayers.Clear();

        foreach (EnemySlotView enemy in enemies)
        {
            enemy.VirtualTarget = null;
            enemy.IsVirtualDead = false;
        }

        foreach (PermanentView target in CombatSystem.Instance.Player_Permanents)
        {
            target.VirtualHP = GetBaseHP(target);
            target.VirtualAttacker = new List<EnemySlotView>();
            target.IsVirtualDead = false;

            if (target.IsCore) continue;

            foreach (EnemySlotView enemy in enemies)
            {
                if (!enemy.TargetLocked || !IsTargeting(enemy, target)) continue;

                target.VirtualAttacker.Add(enemy);
                enemy.VirtualTarget = target;
            }

            RefreshVirtualDeath(target);
        }
    }

    private static void RefreshVirtualDeath(PermanentView target)
    {
        if (SacrificedPlayers.Contains(target))
        {
            target.IsVirtualDead = true;
            return;
        }

        List<EnemySlotView> attackers = GetVirtualAttackers(target);

        target.IsVirtualDead = !target.IsCore
                            && attackers.Count > 0
                            && GetTotalDamage(attackers, target) >= GetBaseHP(target);
    }

    private static List<EnemySlotView> GetVirtualAttackers(PermanentView target)
    {
        if (target.VirtualAttacker == null)
            target.VirtualAttacker = new List<EnemySlotView>();

        return target.VirtualAttacker;
    }


    private static List<Effect> GetDynamicEffects(EnemySlotView enemy)
    {
        List<Effect> result = new();

        if (enemy.IntentAction != null)
            CollectDynamicEffects(enemy.IntentAction, result);

        return result;
    }

    private static void CollectDynamicEffects(Effect effect, List<Effect> result)
    {
        if (effect is EffectGroup group)
        {
            if (group.EffectGroups == null) return;

            foreach (Effect child in group.EffectGroups)
            {
                if (child == null || !ExecutesOnEnemyTurn(child)) continue;

                if (child.Actionner == null) child.Actionner = group.Actionner;
                if (child.CardActionner == null) child.CardActionner = group.CardActionner;

                CollectDynamicEffects(child, result);
            }
            return;
        }

        // Un effet passif n'a pas de cible : son GA ignore PreTarget.
        if (effect.DynamicTargeting && !effect.IsPassive)
            result.Add(effect);
    }

    private static List<Effect> GetDynamicDamageEffects(EnemySlotView enemy)
    {
        List<Effect> result = GetDynamicEffects(enemy);
        result.RemoveAll(e => !IsDamageEffect(e));
        return result;
    }

    private static List<Effect> GetDynamicSupportEffects(EnemySlotView enemy)
    {
        List<Effect> result = GetDynamicEffects(enemy);
        result.RemoveAll(IsDamageEffect);
        return result;
    }

    private static bool HasDynamicTargeting(Effect effect)
    {
        if (effect is EffectGroup group)
        {
            if (group.EffectGroups == null) return false;

            foreach (Effect child in group.EffectGroups)
            {
                if (child != null && HasDynamicTargeting(child))
                    return true;
            }
            return false;
        }

        return effect.DynamicTargeting && !effect.IsPassive;
    }

    private static bool ExecutesOnEnemyTurn(Effect effect)
    {
        List<EventInfo> events = effect.EventInfos;
        if (events == null || events.Count <= 1) return true;
        return events.Exists(e => e.Events == Events.EnemyTurn);
    }

    private static bool IsEffectTargeting(Effect effect, PermanentView target)
    {
        return effect.PreTargetPermanentView != null
            && effect.PreTargetEnemySlotView != null
            && effect.PreTargetPermanentView.Count == 1
            && effect.PreTargetPermanentView[0] == target
            && effect.PreTargetEnemySlotView.Count == 0;
    }

    private static void SetPreTarget(EnemySlotView enemy, PermanentView target)
    {
        foreach (Effect effect in GetDynamicDamageEffects(enemy))
        {
            effect.PreTargetPermanentView = target != null
                ? new List<PermanentView> { target }
                : new List<PermanentView>();
            effect.PreTargetEnemySlotView = new List<EnemySlotView>();
        }
    }

    private static int GetBaseHP(PermanentView target)
    {
        return target.currentLife + target.currentArmor;
    }

    private static bool IsDamageEffect(Effect effect)
    {
        return effect is DealDamageEffect || effect is LifeLossEffect;
    }

    private static int GetDamage(EnemySlotView enemy, PermanentView target)
    {
        int total = 0;

        foreach (Effect effect in GetDynamicDamageEffects(enemy))
            total += GetEffectDamage(effect, enemy, target);

        return total;
    }

    private static int GetEffectDamage(Effect effect, EnemySlotView enemy, PermanentView target)
    {
        switch (effect)
        {
            case DealDamageEffect dealDamage:
                int baseDamage = dealDamage.powerBased ? enemy.currentPower : dealDamage.damageAmount;
                return ResolveAmount(baseDamage, dealDamage.DynamicAmountInfo, enemy, target, null) * dealDamage.multiplyAmount;

            case LifeLossEffect lifeLoss:
                return ResolveAmount(lifeLoss.LifeLossAmount, lifeLoss.DynamicAmountInfo, enemy, target, null) * lifeLoss.multiplyAmount;

            default:
                return 0;
        }
    }

    private static int GetTotalDamage(List<EnemySlotView> attackers, PermanentView target)
    {
        int total = 0;
        foreach (EnemySlotView attacker in attackers)
            total += GetDamage(attacker, target);
        return total;
    }

    private static int GetLockedDamage(PermanentView target)
    {
        int total = 0;

        foreach (EnemySlotView enemy in CombatSystem.Instance.Enemy_Permanents)
        {
            if (enemy.TargetLocked && IsTargeting(enemy, target))
                total += GetDamage(enemy, target);
        }

        return total;
    }

    private static bool IsTargeting(EnemySlotView enemy, PermanentView target)
    {
        List<Effect> effects = GetDynamicDamageEffects(enemy);
        if (effects.Count == 0) return false;

        foreach (Effect effect in effects)
        {
            if (!IsEffectTargeting(effect, target))
                return false;
        }

        return true;
    }

    private static bool IsDynamicEnemy(EnemySlotView enemy)
    {
        return !enemy.IsCore
            && enemy.IntentAction != null
            && HasDynamicTargeting(enemy.IntentAction);
    }

    private static bool IsDynamicDamageEnemy(EnemySlotView enemy)
    {
        return !enemy.IsCore
            && enemy.IntentAction != null
            && GetDynamicDamageEffects(enemy).Count > 0;
    }

    private static void ResetAsGoodTarget()
    {
        foreach (EnemySlotView enemy in CombatSystem.Instance.Enemy_Permanents)
        {
            if (IsDynamicEnemy(enemy) && !enemy.TargetLocked)
                enemy.AsGoodTarget = false;
        }
    }

    private static bool AnyDynamicEnemyWithoutTarget()
    {
        foreach (EnemySlotView enemy in CombatSystem.Instance.Enemy_Permanents)
        {
            if (IsDynamicDamageEnemy(enemy) && !enemy.TargetLocked && !enemy.AsGoodTarget)
                return true;
        }

        return false;
    }

    public void RemoveVisualTargeting()
    {
        foreach (EnemySlotView enemySlotView in CombatSystem.Instance.Enemy_Permanents)
        {
            if (!enemySlotView.TargetLocked)
            {
                enemySlotView.UpdateLockLogo(false);
                enemySlotView.UpdateIntentLogoColor(false);  
            }
        }        
    }
    
    IEnumerator ExecuteOnceEnemyTurnAction(ExecEnemyTurnActionOnceGA execGA)
    {
        if (execGA.EffectsToExec.Count > 0)
        {
            GameEventSystem.Instance.RegisterEffect(execGA.EffectsToExec[0]);
            execGA.EffectsToExec.RemoveAt(0);
            ExecEnemyTurnActionOnceGA NexecGA = new(execGA.EffectsToExec);
            ActionSystem.Instance.AddReaction(NexecGA);
        }
        else
        {
            EndEnemyTurnGA endGA = new();
            ActionSystem.Instance.AddReaction(endGA);
        }        
        
        yield return null;
    }

    private IEnumerator PerformIntentConstructPerformer(SpawnConstructGA spawnConstructGA)
    {
        bool hasConstructs = enemyView.IntentConstructs != null && enemyView.IntentConstructs.Count != 0;
        bool hasSequence = enemyView.ConstructSequence != null && enemyView.ConstructSequence.Count != 0;

        if (!CombatSystem.Instance.Win && hasConstructs && hasSequence)
        {
            bool sequenceFinished = false;

            if (enemyView.sequenceIndex >= enemyView.ConstructSequence.Count)
            {
                if (enemyView.LoopingSequence)
                    enemyView.sequenceIndex = 0;
                else
                    sequenceFinished = true;
            }

            if (!sequenceFinished)
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
                            EnemySlotViewCreator.Instance.CreateEnemySlotViewCreator(data, false);
                    }
                }
            }
        }

        enemyView.sequenceIndex++;
        yield return null;
    }

    private IEnumerator AttackPlayerPerformer(AttackPlayerGA attackPlayerGA)
    {
        if (attackPlayerGA.Actionner != null)
        {
            EnemySlotView Attacker = attackPlayerGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);
        }

        if (attackPlayerGA.playerTargets != null && attackPlayerGA.playerTargets.Count > 0)
        {
            DealDamageGA dealDamageGA = new(attackPlayerGA.powerBased, attackPlayerGA.Damage, attackPlayerGA.multiplyAmount, attackPlayerGA.DynamicAmountInfo, attackPlayerGA.playerTargets, null);
            dealDamageGA.Actionner = attackPlayerGA.Actionner;
            dealDamageGA.SourceEffect = attackPlayerGA.SourceEffect;
            dealDamageGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(dealDamageGA);
        }

        if (attackPlayerGA.enemyTargets != null && attackPlayerGA.enemyTargets.Count > 0)
        {
            DealDamageGA dealDamageGA = new(attackPlayerGA.powerBased, attackPlayerGA.Damage, attackPlayerGA.multiplyAmount, attackPlayerGA.DynamicAmountInfo, null, attackPlayerGA.enemyTargets);
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
            HealGA healGA = new HealGA(healEnemyGA.HealAmount, healEnemyGA.multiplyAmount, healEnemyGA.DynamicAmountInfo, healEnemyGA.playerTargets, null);
            healGA.SourceEffect = healEnemyGA.SourceEffect;
            healGA.Actionner = healEnemyGA.Actionner;
            healGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(healGA);
        }

        if (healEnemyGA.enemyTargets != null && healEnemyGA.enemyTargets.Count > 0)
        {
            HealGA healGA = new HealGA(healEnemyGA.HealAmount, healEnemyGA.multiplyAmount, healEnemyGA.DynamicAmountInfo, null, healEnemyGA.enemyTargets);
            healGA.SourceEffect = healEnemyGA.SourceEffect;
            healGA.Actionner = healEnemyGA.Actionner;
            healGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(healGA);
        }
    }

    private IEnumerator ArmorEnemyPerformer(ArmorEnemyGA ArmorEnemyGA)
    {
        if (ArmorEnemyGA.Actionner != null)
        {
            EnemySlotView Attacker = ArmorEnemyGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y - 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);
        }

        if (ArmorEnemyGA.playerTargets != null && ArmorEnemyGA.playerTargets.Count > 0)
        {
            ArmorGA ArmorGA = new ArmorGA(ArmorEnemyGA.ArmorAmount, ArmorEnemyGA.multiplyAmount, ArmorEnemyGA.DynamicAmountInfo, ArmorEnemyGA.playerTargets, null);
            ArmorGA.SourceEffect = ArmorEnemyGA.SourceEffect;
            ArmorGA.Actionner = ArmorEnemyGA.Actionner;
            ArmorGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(ArmorGA);
        }

        if (ArmorEnemyGA.enemyTargets != null && ArmorEnemyGA.enemyTargets.Count > 0)
        {
            ArmorGA ArmorGA = new ArmorGA(ArmorEnemyGA.ArmorAmount, ArmorEnemyGA.multiplyAmount, ArmorEnemyGA.DynamicAmountInfo, null, ArmorEnemyGA.enemyTargets);
            ArmorGA.SourceEffect = ArmorEnemyGA.SourceEffect;
            ArmorGA.Actionner = ArmorEnemyGA.Actionner;
            ArmorGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(ArmorGA);
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
            retrieveExhaustedGA.Actionner = enemyRetrieveExhaustedGA.Actionner;
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
            exhaustGA.Actionner = enemyExhaustGA.Actionner;
            exhaustGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(exhaustGA);
        }

        if (enemyExhaustGA.cardTargets != null && enemyExhaustGA.cardTargets.Count > 0)
        {
            ExhaustGA exhaustGA = new ExhaustGA(null, null, enemyExhaustGA.cardTargets, enemyExhaustGA.targetModeInfo);
            exhaustGA.SourceEffect = enemyExhaustGA.SourceEffect;
            exhaustGA.Actionner = enemyExhaustGA.Actionner;
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
            AlterPowerGA alterPowerGA = new AlterPowerGA(enemyAlterPowerGA.Amount, enemyAlterPowerGA.multiplyAmount, enemyAlterPowerGA.DynamicAmountInfo, enemyAlterPowerGA.passive, enemyAlterPowerGA.aditive, null, null, null, enemyAlterPowerGA.targetModeInfo);
            alterPowerGA.SourceEffect = enemyAlterPowerGA.SourceEffect;
            alterPowerGA.Actionner = enemyAlterPowerGA.Actionner;
            alterPowerGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(alterPowerGA);
        }
        else
        {
            if (enemyAlterPowerGA.playerTargets != null && enemyAlterPowerGA.playerTargets.Count > 0)
            {
                AlterPowerGA alterPowerGA = new AlterPowerGA(enemyAlterPowerGA.Amount, enemyAlterPowerGA.multiplyAmount, enemyAlterPowerGA.DynamicAmountInfo, enemyAlterPowerGA.passive, enemyAlterPowerGA.aditive, enemyAlterPowerGA.playerTargets, null, null);
                alterPowerGA.SourceEffect = enemyAlterPowerGA.SourceEffect;
                alterPowerGA.Actionner = enemyAlterPowerGA.Actionner;
                alterPowerGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(alterPowerGA);
            }

            if (enemyAlterPowerGA.enemyTargets != null && enemyAlterPowerGA.enemyTargets.Count > 0)
            {
                AlterPowerGA alterPowerGA = new AlterPowerGA(enemyAlterPowerGA.Amount, enemyAlterPowerGA.multiplyAmount, enemyAlterPowerGA.DynamicAmountInfo, enemyAlterPowerGA.passive, enemyAlterPowerGA.aditive, null, enemyAlterPowerGA.enemyTargets, null);
                alterPowerGA.SourceEffect = enemyAlterPowerGA.SourceEffect;
                alterPowerGA.Actionner = enemyAlterPowerGA.Actionner;
                alterPowerGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(alterPowerGA);
            }

            if (enemyAlterPowerGA.cardTargets != null && enemyAlterPowerGA.cardTargets.Count > 0)
            {
                AlterPowerGA alterPowerGA = new AlterPowerGA(enemyAlterPowerGA.Amount, enemyAlterPowerGA.multiplyAmount, enemyAlterPowerGA.DynamicAmountInfo, enemyAlterPowerGA.passive, enemyAlterPowerGA.aditive, null, enemyAlterPowerGA.enemyTargets, null);
                alterPowerGA.SourceEffect = enemyAlterPowerGA.SourceEffect;
                alterPowerGA.Actionner = enemyAlterPowerGA.Actionner;
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
            AlterStaminaGA alterStaminaGA = new AlterStaminaGA(enemyAlterStaminaGA.Amount, enemyAlterStaminaGA.multiplyAmount, enemyAlterStaminaGA.DynamicAmountInfo, enemyAlterStaminaGA.passive, enemyAlterStaminaGA.aditive, null, null, null, enemyAlterStaminaGA.targetModeInfo);
            alterStaminaGA.SourceEffect = enemyAlterStaminaGA.SourceEffect;
            alterStaminaGA.Actionner = enemyAlterStaminaGA.Actionner;
            alterStaminaGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(alterStaminaGA);
        }
        else
        {
            if (enemyAlterStaminaGA.playerTargets != null && enemyAlterStaminaGA.playerTargets.Count > 0)
            {
                AlterStaminaGA alterStaminaGA = new AlterStaminaGA(enemyAlterStaminaGA.Amount, enemyAlterStaminaGA.multiplyAmount, enemyAlterStaminaGA.DynamicAmountInfo, enemyAlterStaminaGA.passive, enemyAlterStaminaGA.aditive, enemyAlterStaminaGA.playerTargets, null, null, enemyAlterStaminaGA.targetModeInfo);
                alterStaminaGA.SourceEffect = enemyAlterStaminaGA.SourceEffect;
                alterStaminaGA.Actionner = enemyAlterStaminaGA.Actionner;
                alterStaminaGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(alterStaminaGA);
            }

            if (enemyAlterStaminaGA.enemyTargets != null && enemyAlterStaminaGA.enemyTargets.Count > 0)
            {
                AlterStaminaGA alterStaminaGA = new AlterStaminaGA(enemyAlterStaminaGA.Amount, enemyAlterStaminaGA.multiplyAmount, enemyAlterStaminaGA.DynamicAmountInfo, enemyAlterStaminaGA.passive, enemyAlterStaminaGA.aditive, null, enemyAlterStaminaGA.enemyTargets, null, enemyAlterStaminaGA.targetModeInfo);
                alterStaminaGA.SourceEffect = enemyAlterStaminaGA.SourceEffect;
                alterStaminaGA.Actionner = enemyAlterStaminaGA.Actionner;
                alterStaminaGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(alterStaminaGA);
            }

            if (enemyAlterStaminaGA.cardTargets != null && enemyAlterStaminaGA.cardTargets.Count > 0)
            {
                AlterStaminaGA alterStaminaGA = new AlterStaminaGA(enemyAlterStaminaGA.Amount, enemyAlterStaminaGA.multiplyAmount, enemyAlterStaminaGA.DynamicAmountInfo, enemyAlterStaminaGA.passive, enemyAlterStaminaGA.aditive, null, null, enemyAlterStaminaGA.cardTargets, enemyAlterStaminaGA.targetModeInfo);
                alterStaminaGA.SourceEffect = enemyAlterStaminaGA.SourceEffect;
                alterStaminaGA.Actionner = enemyAlterStaminaGA.Actionner;
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
            AlterCardCostGA alterCardCostGA = new AlterCardCostGA(enemyAlterCardCostGA.Amount, enemyAlterCardCostGA.multiplyAmount, enemyAlterCardCostGA.DynamicAmountInfo, enemyAlterCardCostGA.passive, null, enemyAlterCardCostGA.targetModeInfo);
            alterCardCostGA.SourceEffect = enemyAlterCardCostGA.SourceEffect;
            alterCardCostGA.Actionner = enemyAlterCardCostGA.Actionner;
            alterCardCostGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(alterCardCostGA);
        }
        else
        {
            if (enemyAlterCardCostGA.cardTargets != null && enemyAlterCardCostGA.cardTargets.Count > 0)
            {
                AlterCardCostGA alterCardCostGA = new AlterCardCostGA(enemyAlterCardCostGA.Amount, enemyAlterCardCostGA.multiplyAmount, enemyAlterCardCostGA.DynamicAmountInfo, enemyAlterCardCostGA.passive, enemyAlterCardCostGA.cardTargets, enemyAlterCardCostGA.targetModeInfo);
                alterCardCostGA.SourceEffect = enemyAlterCardCostGA.SourceEffect;
                alterCardCostGA.Actionner = enemyAlterCardCostGA.Actionner;
                alterCardCostGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(alterCardCostGA);
            }
        }
    }

    private IEnumerator AlterPowerGridEnemyPerformer(EnemyAlterPowerGridGA enemyAlterPowerGridGA)
    {
        if (enemyAlterPowerGridGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyAlterPowerGridGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            AlterPowerGridGA alterPowerGridGA = new AlterPowerGridGA(enemyAlterPowerGridGA.Amount, enemyAlterPowerGridGA.multiplyAmount, enemyAlterPowerGridGA.DynamicAmountInfo);
            alterPowerGridGA.SourceEffect = enemyAlterPowerGridGA.SourceEffect;
            alterPowerGridGA.Actionner = enemyAlterPowerGridGA.Actionner;
            alterPowerGridGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(alterPowerGridGA);
        }
    }

    private IEnumerator EnemyAddACopyPlayerPerformer(EnemyAddACopyGa enemyAddACopyGa)
    {
        if (enemyAddACopyGa.Actionner != null)
        {
            EnemySlotView Attacker = enemyAddACopyGa.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            AddACopyGa addACopyGa = new AddACopyGa(enemyAddACopyGa.Amount, enemyAddACopyGa.multiplyAmount, enemyAddACopyGa.DynamicAmountInfo, enemyAddACopyGa.AffectedSide, enemyAddACopyGa.TypeOfCopy);
            addACopyGa.SourceEffect = enemyAddACopyGa.SourceEffect;
            addACopyGa.Actionner = enemyAddACopyGa.Actionner;
            addACopyGa.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(addACopyGa);
        }
    }

    private IEnumerator LifeLossEnemyPerformer(EnemyLifeLossGA enemyLifeLossGA)
    {
        if (enemyLifeLossGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyLifeLossGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.transform.DOMoveY(Attacker.transform.position.y + 1f, 0.25f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);
        }

        if (enemyLifeLossGA.playerTargets != null && enemyLifeLossGA.playerTargets.Count > 0)
        {
            LifeLossGA lifeLossGA = new LifeLossGA(enemyLifeLossGA.Amount, enemyLifeLossGA.multiplyAmount, enemyLifeLossGA.DynamicAmountInfo, enemyLifeLossGA.playerTargets, null);
            lifeLossGA.SourceEffect = enemyLifeLossGA.SourceEffect;
            lifeLossGA.Actionner = enemyLifeLossGA.Actionner;
            lifeLossGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(lifeLossGA);
        }

        if (enemyLifeLossGA.enemyTargets != null && enemyLifeLossGA.enemyTargets.Count > 0)
        {
            LifeLossGA lifeLossGA = new LifeLossGA(enemyLifeLossGA.Amount, enemyLifeLossGA.multiplyAmount, enemyLifeLossGA.DynamicAmountInfo, null, enemyLifeLossGA.enemyTargets);
            lifeLossGA.SourceEffect = enemyLifeLossGA.SourceEffect;
            lifeLossGA.Actionner = enemyLifeLossGA.Actionner;
            lifeLossGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(lifeLossGA);
        }
    }

    private IEnumerator DisableEnemyPerformer(EnemyDisableGA enemyDisableGA)
    {
        if (enemyDisableGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyDisableGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.WrapperGM.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 10, 1f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (enemyDisableGA.playerTargets != null && enemyDisableGA.playerTargets.Count > 0)
            {
                DisableGA disableGA = new DisableGA(enemyDisableGA.playerTargets, null);
                disableGA.SourceEffect = enemyDisableGA.SourceEffect;
                disableGA.Actionner = enemyDisableGA.Actionner;
                disableGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(disableGA);
            }
            if (enemyDisableGA.enemyTargets != null && enemyDisableGA.enemyTargets.Count > 0)
            {
                DisableGA disableGA = new DisableGA(null, enemyDisableGA.enemyTargets);
                disableGA.SourceEffect = enemyDisableGA.SourceEffect;
                disableGA.Actionner = enemyDisableGA.Actionner;
                disableGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(disableGA);
            }
        }
    }

    private IEnumerator EnableEnemyPerformer(EnemyEnableGA enemyEnableGA)
    {
        if (enemyEnableGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyEnableGA.Actionner.GetComponent<EnemySlotView>();

            Tween tween = Attacker.WrapperGM.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 10, 1f);
            yield return tween.WaitForCompletion();
            Attacker.transform.DOMoveY(Attacker.InitialPosition.y, 0.35f);

            if (enemyEnableGA.playerTargets != null && enemyEnableGA.playerTargets.Count > 0)
            {
                EnableGA enableGA = new EnableGA(enemyEnableGA.playerTargets, null);
                enableGA.SourceEffect = enemyEnableGA.SourceEffect;
                enableGA.Actionner = enemyEnableGA.Actionner;
                enableGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(enableGA);
            }
            if (enemyEnableGA.enemyTargets != null && enemyEnableGA.enemyTargets.Count > 0)
            {
                EnableGA enableGA = new EnableGA(null, enemyEnableGA.enemyTargets);
                enableGA.SourceEffect = enemyEnableGA.SourceEffect;
                enableGA.Actionner = enemyEnableGA.Actionner;
                enableGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(enableGA);
            }
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
            GainLifeGA gainLifeGA = new GainLifeGA(enemyGainLifeGA.Amount, enemyGainLifeGA.multiplyAmount, enemyGainLifeGA.DynamicAmountInfo, enemyGainLifeGA.passive, enemyGainLifeGA.aditive, null, null, enemyGainLifeGA.targetModeInfo);
            gainLifeGA.SourceEffect = enemyGainLifeGA.SourceEffect;
            gainLifeGA.Actionner = enemyGainLifeGA.Actionner;
            gainLifeGA.ActivateToolTip = false;
            ActionSystem.Instance.AddReaction(gainLifeGA);
        }
        else
        {
            if (enemyGainLifeGA.playerTargets != null && enemyGainLifeGA.playerTargets.Count > 0)
            {
                GainLifeGA gainLifeGA = new GainLifeGA(enemyGainLifeGA.Amount, enemyGainLifeGA.multiplyAmount, enemyGainLifeGA.DynamicAmountInfo, enemyGainLifeGA.passive, enemyGainLifeGA.aditive, enemyGainLifeGA.playerTargets, null, null);
                gainLifeGA.SourceEffect = enemyGainLifeGA.SourceEffect;
                gainLifeGA.Actionner = enemyGainLifeGA.Actionner;
                gainLifeGA.ActivateToolTip = false;
                ActionSystem.Instance.AddReaction(gainLifeGA);
            }

            if (enemyGainLifeGA.enemyTargets != null && enemyGainLifeGA.enemyTargets.Count > 0)
            {
                GainLifeGA gainLifeGA = new GainLifeGA(enemyGainLifeGA.Amount, enemyGainLifeGA.multiplyAmount, enemyGainLifeGA.DynamicAmountInfo, enemyGainLifeGA.passive, enemyGainLifeGA.aditive, null, enemyGainLifeGA.enemyTargets, null);
                gainLifeGA.SourceEffect = enemyGainLifeGA.SourceEffect;
                gainLifeGA.Actionner = enemyGainLifeGA.Actionner;
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

        InvocGA invocGA = new(invocEGA.Amount, invocEGA.multiplyAmount, invocEGA.DynamicAmountInfo, null, invocEGA.EnemyToInvoc);
        invocGA.SourceEffect = invocEGA.SourceEffect;
        invocGA.Actionner = invocEGA.Actionner;
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
        sacGA.Actionner = sacEGA.Actionner;
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

    private void BeforeArmorPreReaction(ArmorEnemyGA ArmorEnemyGA)
    {
        if (ArmorEnemyGA.Actionner != null)
        {
            EnemySlotView Attacker = ArmorEnemyGA.Actionner.GetComponent<EnemySlotView>();
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

    private void BeforeRetrieveExhaustedEPerformerPreReaction(EnemyRetrieveExhaustedGA enemyRetrieveExhaustedGA)
    {
        if (enemyRetrieveExhaustedGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyRetrieveExhaustedGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeExhaustedEPerformerPreReaction(EnemyExhaustGA enemyExhaustGA)
    {
        if (enemyExhaustGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyExhaustGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeAlterPowerGridEPerformerPreReaction(EnemyAlterPowerGridGA enemyAlterPowerGridGA)
    {
        if (enemyAlterPowerGridGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyAlterPowerGridGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeAddACopyEPerformerPreReaction(EnemyAddACopyGa enemyAddACopyGa)
    {
        if (enemyAddACopyGa.Actionner != null)
        {
            EnemySlotView Attacker = enemyAddACopyGa.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeDisableEPerformerPreReaction(EnemyDisableGA enemyDisableGA)
    {
        if (enemyDisableGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyDisableGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }

    private void BeforeEnableEPerformerPreReaction(EnemyEnableGA enemyEnableGA)
    {
        if (enemyEnableGA.Actionner != null)
        {
            EnemySlotView Attacker = enemyEnableGA.Actionner.GetComponent<EnemySlotView>();
            Attacker.SetPosition(Attacker.transform.position);
        }
    }
}
