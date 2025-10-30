using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using FMODUnity;

public class TargetSystem : Singleton<TargetSystem>
{
    [SerializeField] private LayerMask TargetingLayerMask;
    [SerializeField] private LayerMask CardviewTargetingLayerMask;

    [SerializeField] private GameObject CursorGameobject;
    private bool TargetingActive;
    public bool CardTargetingActive;
    private int InitTargetingNumber;
    private bool TargetingUpTo;
    private int TargetingNumber;
    private List<TargetLimitationInfo> CurrentLimitations;
    private List<EnemySlotView> ETargets_ForAura = new();
    private List<PermanentView> PTargets_ForAura = new();
    private List<EnemySlotView> enemySlots = new();
    private List<PermanentView> permanents = new();
    private List<CardView> CardTargets = new();

    public void OnEnable()
    {
        ActionSystem.AttachPerformer<StartManualTargetingGA>(GetTargetsManualPerformer);
        ActionSystem.AttachPerformer<StartCardTargetingGA>(GetCardTargetsPerformer);

    }

    public void OnDisable()
    {
        ActionSystem.DetachPerformer<StartManualTargetingGA>();
        ActionSystem.DetachPerformer<StartCardTargetingGA>();
    }

    public IEnumerator GetTargetsManualPerformer(StartManualTargetingGA startManualTargetingGA)
    {
        List<PermanentView> playerTargets = new();
        List<EnemySlotView> enemyTargets = new();

        ActivateAuraForTargets(startManualTargetingGA.TargetLimitations);

        TargetingNumber = InitTargetingNumber = startManualTargetingGA.TargetNumber;
        TargetingUpTo = startManualTargetingGA.TargetUpTo;
        if (startManualTargetingGA.TargetLimitations != null)
        {
            CurrentLimitations = startManualTargetingGA.TargetLimitations;
        }
        else
        {
            CurrentLimitations = null;
        }

        StartManualTargeting();
        while (TargetingActive)
            yield return null;

        (enemyTargets, playerTargets) = EndManualTargeting();

        startManualTargetingGA.EffectRef.TargetForLinked_Player = new List<PermanentView>(playerTargets);
        startManualTargetingGA.EffectRef.TargetForLinked_Enemy = new List<EnemySlotView>(enemyTargets);

        var action = startManualTargetingGA.ActionToRealiseAfterTargetting;
        var type = action.GetType();

        // Vérifie qu'il y a bien les propriétés attendues
        var playerTargetsProp = type.GetProperty("playerTargets");
        var enemyTargetsProp = type.GetProperty("enemyTargets");

        if (playerTargetsProp != null && enemyTargetsProp != null)
        {
            playerTargetsProp.SetValue(action, playerTargets);
            enemyTargetsProp.SetValue(action, enemyTargets);
        }
        else
        {
            Debug.LogError("L'action ne contient pas les propriétés playerTargets ou enemyTargets");
        }

        ActionSystem.Instance.AddReaction(startManualTargetingGA.ActionToRealiseAfterTargetting);
    }

    public IEnumerator GetCardTargetsPerformer(StartCardTargetingGA startCardTargetingGA)
    {
        List<CardView> cardViewTargets = new();
        TargetingNumber = InitTargetingNumber = startCardTargetingGA.TargetNumber;
        TargetingUpTo = startCardTargetingGA.TargetUpTo;
        CombatSystem.Instance.Interactable = false;

        if (startCardTargetingGA.TargetLimitations != null)
        {
            CurrentLimitations = startCardTargetingGA.TargetLimitations;
        }
        else
        {
            CurrentLimitations = null;
        }

        StartCardTargeting();

        while (CardTargetingActive)
            yield return null;

        cardViewTargets = EndCardTargeting();

        List<Card> CardTargets = new();
        foreach (CardView item in cardViewTargets)
        {
            CardTargets.Add(item.Card);
        }
        startCardTargetingGA.EffectRef.TargetForLinked_Card = new List<Card>(CardTargets);

        // Vérifie qu'il y a bien les propriétés attendues
        var action = startCardTargetingGA.ActionToRealiseAfterTargetting;
        var type = action.GetType();
        var CardviewTargetsProp = type.GetProperty("CardViews");

        if (CardviewTargetsProp != null)
        {
            CardviewTargetsProp.SetValue(action, cardViewTargets);
        }
        else
        {
            Debug.LogError("L'action ne contient pas la propriétés CardViews");
        }

        CombatSystem.Instance.Interactable = true;
        ActionSystem.Instance.AddReaction(startCardTargetingGA.ActionToRealiseAfterTargetting);
    }

    public static (List<PermanentView> playerTargets, List<EnemySlotView> enemyTargets) GetTargets(TargetMode mode, GameObject actionner)
    {
        List<PermanentView> playerTargets = new();
        List<EnemySlotView> enemyTargets = new();

        var playerPermanents = CombatSystem.Instance.Player_Permanents;
        var enemyPermanents = CombatSystem.Instance.Enemy_Permanents;

        List<PermanentView> TampontargetsP = new List<PermanentView>();
        List<EnemySlotView> TampontargetsE = new List<EnemySlotView>();

        //Redirection de cible vers core dans le cas d'une attaque enemy et pas de target
        bool RedirectionActive = false;
        if (actionner != null)
        {
            PermanentView TestIfPlayerPermanent = actionner.GetComponent<PermanentView>();
            if (TestIfPlayerPermanent)
            {
                RedirectionActive = false;
            }
            else
            {
                RedirectionActive = true;
            }              
        }
      

        switch (mode)
        {
            case TargetMode.Self:
                if (actionner != null)
                {
                    PermanentView TestIfPlayerPermanent = actionner.GetComponent<PermanentView>();
                    if (TestIfPlayerPermanent)
                    {
                        var self = actionner.GetComponent<PermanentView>();
                        if (self != null)
                            playerTargets.Add(self);
                    }
                    else
                    {
                        var self = actionner.GetComponent<EnemySlotView>();
                        if (self != null)
                            enemyTargets.Add(self);
                    }              
                }
                break;

            case TargetMode.Random_Player:
                var targetablePlayers = playerPermanents
                    .Where(p => !p.UnTargetable)
                    .ToList();

                if (targetablePlayers.Count > 0)
                {
                    var rnd = Random.Range(0, targetablePlayers.Count);
                    playerTargets.Add(targetablePlayers[rnd]);
                }
                break;

            case TargetMode.Core_Player:
                foreach (var perm in playerPermanents)
                    if (perm.IsCore && !perm.UnTargetable) playerTargets.Add(perm);
                break;

            case TargetMode.HighHP_Player:
                int maxTotal = playerPermanents.Max(p => p.currentLife);
                var highestTargets = playerPermanents
                    .Where(p => p.currentLife == maxTotal && !p.UnTargetable)
                    .ToList();

                if (highestTargets.Count > 0)
                {
                    var selected = highestTargets[Random.Range(0, highestTargets.Count)];
                    playerTargets.Add(selected);
                }
                break;

            case TargetMode.LowHP_Player:
                int minTotal = playerPermanents.Min(p => p.currentLife);
                var lowestTargets = playerPermanents
                    .Where(p => p.currentLife == minTotal && !p.UnTargetable)
                    .ToList();

                if (lowestTargets.Count > 0)
                {
                    var selected = lowestTargets[Random.Range(0, lowestTargets.Count)];
                    playerTargets.Add(selected);
                }
                break;

            case TargetMode.Random_Enemy:
                var targetableEnemies = enemyPermanents
                    .Where(p => !p.UnTargetable)
                    .ToList();

                if (playerPermanents.Count > 0 && targetableEnemies.Count > 0)
                {
                    var rnd = Random.Range(0, targetableEnemies.Count);
                    enemyTargets.Add(targetableEnemies[rnd]);
                }
                break;

            case TargetMode.Core_Enemy:
                foreach (var perm in enemyPermanents)
                {
                    if (perm.IsCore && !perm.UnTargetable)
                    {
                        enemyTargets.Add(perm);
                    }
                }
                break;

            case TargetMode.HighHP_Enemy:
                int maxTotal2 = enemyPermanents.Max(p => p.currentLife);
                var highestTargets2 = enemyPermanents
                    .Where(p => p.currentLife == maxTotal2 && !p.UnTargetable)
                    .ToList();

                if (highestTargets2.Count > 0)
                {
                    var selected = highestTargets2[Random.Range(0, highestTargets2.Count)];
                    enemyTargets.Add(selected);
                }
                break;

            case TargetMode.LowHP_Enemy:
                int minTotal2 = enemyPermanents.Min(p => p.currentLife);
                var lowestTargets2 = enemyPermanents
                    .Where(p => p.currentLife == minTotal2 && !p.UnTargetable)
                    .ToList();

                if (lowestTargets2.Count > 0)
                {
                    var selected = lowestTargets2[Random.Range(0, lowestTargets2.Count)];
                    enemyTargets.Add(selected);
                }
                break;

            case TargetMode.All_Player:
                foreach (var perm in playerPermanents)
                {
                    if (perm.UnTargetable) continue;
                    playerTargets.Add(perm);

                }
                break;

            case TargetMode.All_Enemy:
                foreach (var perm in enemyPermanents)
                {
                    if (perm.UnTargetable) continue;
                    enemyTargets.Add(perm);

                }
                break;

            case TargetMode.All_All:
                foreach (var perm in playerPermanents)
                {
                    if (perm.UnTargetable) continue;
                    playerTargets.Add(perm);

                }
                foreach (var perm in enemyPermanents)
                {
                    if (perm.UnTargetable) continue;
                    enemyTargets.Add(perm);

                }
                break;
            case TargetMode.ALL_Player_Weapons:
                foreach (var perm in playerPermanents)
                {
                    if (perm.UnTargetable) continue;
                    if (perm.permanentArea == PermanentArea.Weapon)
                    {
                        playerTargets.Add(perm);
                    }
                }
                if (RedirectionActive)
                {
                    if (playerTargets.Count <= 0)
                    {
                        foreach (var Core in playerPermanents)
                            if (Core.IsCore && !Core.UnTargetable) playerTargets.Add(Core);
                    }                    
                }
                break;
            case TargetMode.ALL_Player_Shields:
                foreach (var perm in playerPermanents)
                {
                    if (perm.UnTargetable) continue;
                    if (perm.permanentArea == PermanentArea.Shield)
                    {
                        playerTargets.Add(perm);
                    }
                }
                if (RedirectionActive)
                {
                    if (playerTargets.Count <= 0)
                    {
                        foreach (var Core in playerPermanents)
                            if (Core.IsCore && !Core.UnTargetable) playerTargets.Add(Core);
                    }                    
                }
                break;
            case TargetMode.ALL_Player_Supports:
                foreach (var perm in playerPermanents)
                {
                    if (perm.UnTargetable) continue;
                    if (perm.permanentArea == PermanentArea.Support)
                    {
                        playerTargets.Add(perm);
                    }
                }
                if (RedirectionActive)
                {
                    if (playerTargets.Count <= 0)
                    {
                        foreach (var Core in playerPermanents)
                            if (Core.IsCore && !Core.UnTargetable) playerTargets.Add(Core);
                    }                    
                }
                break;
            case TargetMode.ALL_Enemy_Weapons:
                foreach (var perm in enemyPermanents)
                {
                    if (perm.UnTargetable) continue;
                    if (perm.permanentArea == PermanentArea.Weapon)
                    {
                        enemyTargets.Add(perm);
                    }
                }
                if (RedirectionActive)
                {
                    if (enemyTargets.Count <= 0)
                    {
                        foreach (var Core in enemyPermanents)
                            if (Core.IsCore && !Core.UnTargetable) enemyTargets.Add(Core);
                    }                    
                }
                break;
            case TargetMode.ALL_Enemy_Shields:
                foreach (var perm in enemyPermanents)
                {
                    if (perm.UnTargetable) continue;
                    if (perm.permanentArea == PermanentArea.Shield)
                    {
                        enemyTargets.Add(perm);
                    }
                }
                if (RedirectionActive)
                {
                    if (enemyTargets.Count <= 0)
                    {
                        foreach (var Core in enemyPermanents)
                            if (Core.IsCore && !Core.UnTargetable) enemyTargets.Add(Core);
                    }                    
                }
                break;
            case TargetMode.ALL_Enemy_Supports:
                foreach (var perm in enemyPermanents)
                {
                    if (perm.UnTargetable) continue;
                    if (perm.permanentArea == PermanentArea.Support)
                    {
                        enemyTargets.Add(perm);
                    }
                }
                if (RedirectionActive)
                {
                    if (enemyTargets.Count <= 0)
                    {
                        foreach (var Core in enemyPermanents)
                            if (Core.IsCore && !Core.UnTargetable) enemyTargets.Add(Core);
                    }                    
                }
                break;
            case TargetMode.RDM_Player_Weapons:
                TampontargetsP = new List<PermanentView>();
                foreach (var perm in playerPermanents)
                {
                    if (perm.UnTargetable) continue;
                    if (perm.permanentArea == PermanentArea.Weapon)
                    {
                        TampontargetsP.Add(perm);
                    }
                }

                if (TampontargetsP.Count <= 0)
                {
                    foreach (var Core in playerPermanents)
                        if (Core.IsCore && !Core.UnTargetable) playerTargets.Add(Core);
                }
                else
                {
                    playerTargets.Add(TampontargetsP[Random.Range(0, TampontargetsP.Count - 1)]);
                }

                break;
            case TargetMode.RDM_Player_Shields:
                TampontargetsP = new List<PermanentView>();
                foreach (var perm in playerPermanents)
                {
                    if (perm.UnTargetable) continue;
                    if (perm.permanentArea == PermanentArea.Shield)
                    {
                        TampontargetsP.Add(perm);
                    }
                }

                if (TampontargetsP.Count <= 0)
                {
                    foreach (var Core in playerPermanents)
                        if (Core.IsCore && !Core.UnTargetable) playerTargets.Add(Core);
                }
                else
                {
                    playerTargets.Add(TampontargetsP[Random.Range(0, TampontargetsP.Count - 1)]);
                }

                break;
            case TargetMode.RDM_Player_Supports:
                TampontargetsP = new List<PermanentView>();
                foreach (var perm in playerPermanents)
                {
                    if (perm.UnTargetable) continue;
                    if (perm.permanentArea == PermanentArea.Support)
                    {
                        TampontargetsP.Add(perm);
                    }
                }

                if (TampontargetsP.Count <= 0)
                {
                    foreach (var Core in playerPermanents)
                        if (Core.IsCore && !Core.UnTargetable) playerTargets.Add(Core);
                }
                else
                {
                    playerTargets.Add(TampontargetsP[Random.Range(0, TampontargetsP.Count - 1)]);
                }

                break;
            case TargetMode.RDM_Enemy_Weapons:
                TampontargetsE = new List<EnemySlotView>();
                foreach (var perm in enemyPermanents)
                {
                    if (perm.UnTargetable) continue;
                    if (perm.permanentArea == PermanentArea.Weapon)
                    {
                        TampontargetsE.Add(perm);
                    }
                }

                if (TampontargetsE.Count <= 0)
                {
                    foreach (var Core in enemyPermanents)
                        if (Core.IsCore && !Core.UnTargetable) enemyTargets.Add(Core);
                }
                else
                {
                    enemyTargets.Add(TampontargetsE[Random.Range(0, TampontargetsE.Count - 1)]);
                }

                break;
            case TargetMode.RDM_Enemy_Shields:
                TampontargetsE = new List<EnemySlotView>();
                foreach (var perm in enemyPermanents)
                {
                    if (perm.UnTargetable) continue;
                    if (perm.permanentArea == PermanentArea.Shield)
                    {
                        TampontargetsE.Add(perm);
                    }
                }

                if (TampontargetsE.Count <= 0)
                {
                    foreach (var Core in enemyPermanents)
                        if (Core.IsCore && !Core.UnTargetable) enemyTargets.Add(Core);
                }
                else
                {
                    enemyTargets.Add(TampontargetsE[Random.Range(0, TampontargetsE.Count - 1)]);
                }

                break;
            case TargetMode.RDM_Enemy_Supports:
                TampontargetsE = new List<EnemySlotView>();
                foreach (var perm in enemyPermanents)
                {
                    if (perm.UnTargetable) continue;
                    if (perm.permanentArea == PermanentArea.Support)
                    {
                        TampontargetsE.Add(perm);
                    }
                }

                if (TampontargetsE.Count <= 0)
                {
                    foreach (var Core in enemyPermanents)
                        if (Core.IsCore && !Core.UnTargetable) enemyTargets.Add(Core);
                }
                else
                {
                    enemyTargets.Add(TampontargetsE[Random.Range(0, TampontargetsE.Count - 1)]);
                }

                break;
        }

        return (playerTargets, enemyTargets);
    }

    public void StartManualTargeting()
    {
        enemySlots.Clear();
        permanents.Clear();
        TargetingActive = true;
    }

    public (List<EnemySlotView> enemyTargets, List<PermanentView> playerTargets) EndManualTargeting()
    {
        TargetingActive = false;
        return (enemySlots, permanents);
    }

    public void StartCardTargeting()
    {
        CardTargets.Clear();
        CardTargetingActive = true;
    }

    public List<CardView> EndCardTargeting()
    {
        CardTargetingActive = false;
        return CardTargets;
    }

    public void Update()
    {
        if (TargetingActive)
        {
            if (Input.GetKeyDown(KeyCode.Space)) // Espace = confirmer
            {
                if (TargetingUpTo)
                {
                    TargetingActive = false;
                    foreach (EnemySlotView enemy in enemySlots)
                    {
                        enemy.RemoveSelectEffect(false);
                    }
                    foreach (PermanentView permanent in permanents)
                    {
                        permanent.RemoveSelectEffect(false);
                    }
                    foreach (EnemySlotView enemy in ETargets_ForAura)
                    {
                        enemy.deactivateAuraVisual();
                    }
                    foreach (PermanentView permanent in PTargets_ForAura)
                    {
                        permanent.deactivateAuraVisual();
                    }
                }
                else
                {
                    if (TargetingNumber == 0)
                    {
                        TargetingActive = false;
                        foreach (EnemySlotView enemy in enemySlots)
                        {
                            enemy.RemoveSelectEffect(false);
                        }
                        foreach (PermanentView permanent in permanents)
                        {
                            permanent.RemoveSelectEffect(false);
                        }    
                        foreach (EnemySlotView enemy in ETargets_ForAura)
                        {
                            enemy.deactivateAuraVisual();
                        }
                        foreach (PermanentView permanent in PTargets_ForAura)
                        {
                            permanent.deactivateAuraVisual();
                        }                    
                    }
                }
            }
            if (Input.GetMouseButtonDown(0)) // 0 = clic gauche 1 = clic droit
            {
                Debug.DrawRay(CursorGameobject.transform.position + new Vector3(0, 0, -1), Vector3.forward * 10f, Color.red, 1f);
                if (Physics.Raycast(CursorGameobject.transform.position + new Vector3(0, 0, -1), Vector3.forward, out RaycastHit raycastHit, 10f, TargetingLayerMask) && raycastHit.collider != null && raycastHit.transform.TryGetComponent(out EnemySlotView enemyView))
                {
                    if (!enemyView.UnTargetable && PassesAllLimitations(CurrentLimitations, null, null, enemyView))
                    {
                        if (!enemySlots.Contains(enemyView))
                        {
                            if (TargetingNumber > 0)
                            {
                                enemySlots.Add(enemyView);
                                enemyView.ActiveSelectEffect();
                                TargetingNumber -= 1;
                            }
                        }
                        else
                        {
                            if (TargetingNumber < InitTargetingNumber)
                            {
                                enemySlots.Remove(enemyView);
                                enemyView.RemoveSelectEffect();
                                TargetingNumber += 1;
                            }
                        }
                    }
                }
                else if (Physics.Raycast(CursorGameobject.transform.position + new Vector3(0, 0, -1), Vector3.forward, out RaycastHit raycastHit2, 10f, TargetingLayerMask) && raycastHit2.collider != null && raycastHit2.transform.TryGetComponent(out PermanentView permanentView))
                {
                    if (!permanentView.UnTargetable && PassesAllLimitations(CurrentLimitations, null, permanentView, null))
                    {
                        if (!permanents.Contains(permanentView))
                        {
                            if (TargetingNumber > 0)
                            {
                                permanents.Add(permanentView);
                                permanentView.ActiveSelectEffect();
                                TargetingNumber -= 1;
                            }
                        }
                        else
                        {
                            if (TargetingNumber < InitTargetingNumber)
                            {
                                permanents.Remove(permanentView);
                                permanentView.RemoveSelectEffect();
                                TargetingNumber += 1;
                            }
                        }
                    }
                }
            }
        }
        else if (CardTargetingActive)
        {
            if (Input.GetKeyDown(KeyCode.Space)) // Espace = confirmer
            {
                if (TargetingUpTo)
                {
                    CardTargetingActive = false;
                    foreach (CardView card in CardTargets)
                    {
                        card.RemoveSelectEffect(false);
                    }
                }
                else
                {
                    if (TargetingNumber == 0)
                    {
                        CardTargetingActive = false;
                        foreach (CardView card in CardTargets)
                        {
                            card.RemoveSelectEffect(false);
                        }
                    }                    
                }
            }
            if (Input.GetMouseButtonDown(0)) // 0 = clic gauche 1 = clic droit
            {
                Debug.DrawRay(CursorGameobject.transform.position + new Vector3(0, 0, -1), Vector3.forward * 10f, Color.red, 1f);
                if (Physics.Raycast(CursorGameobject.transform.position + new Vector3(0, 0, -1), Vector3.forward, out RaycastHit raycastHit, 10f, CardviewTargetingLayerMask) && raycastHit.collider != null && raycastHit.transform.TryGetComponent(out CardView cardView))
                {
                    if (PassesAllLimitations(CurrentLimitations, cardView.Card, null, null))
                    {
                        if (!CardTargets.Contains(cardView))
                        {
                            if (TargetingNumber > 0)
                            {
                                CardTargets.Add(cardView);
                                cardView.ActiveSelectEffect();
                                TargetingNumber -= 1;
                            }
                        }
                        else
                        {
                            if (TargetingNumber < InitTargetingNumber)
                            {
                                CardTargets.Remove(cardView);
                                cardView.RemoveSelectEffect(true);
                                TargetingNumber += 1;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0)) // 0 = clic gauche 1 = clic droit
            {
                if (!CombatSystem.Instance.Interactable) return;
                if (ActionSystem.Instance.IsPerforming) return;
                if (Physics.Raycast(CursorGameobject.transform.position + new Vector3(0, 0, -1), Vector3.forward, out RaycastHit raycastHit, 10f, TargetingLayerMask) && raycastHit.collider != null && raycastHit.transform.TryGetComponent(out EnemySlotView enemyView))
                {
                    if (!enemyView.Activated)
                    {
                        TriggerEventGA triggerEnemyEventGA = new(Events.OnActivate, null, null, enemyView);
                        ActionSystem.Instance.Perform(triggerEnemyEventGA);
                    }

                }
                else if (Physics.Raycast(CursorGameobject.transform.position + new Vector3(0, 0, -1), Vector3.forward, out RaycastHit raycastHit2, 10f, TargetingLayerMask) && raycastHit2.collider != null && raycastHit2.transform.TryGetComponent(out PermanentView permanentView))
                {
                    if (!permanentView.Activated)
                    {
                        TriggerEventGA triggerPermanentEventGA = new(Events.OnActivate, null, permanentView, null);
                        ActionSystem.Instance.Perform(triggerPermanentEventGA);
                    }
                }
            }
        }
    }

    public int GetDynamicAmount(DynamicAmount dynamicAmount, PermanentView permanentView = null, EnemySlotView enemySlotView = null)
    {
        int FinalAmount = 0;

        switch (dynamicAmount)
        {
            case DynamicAmount.Vessel_Count:
                FinalAmount = CombatSystem.Instance.Player_Permanents.Count + CombatSystem.Instance.Enemy_Permanents.Count;
                break;

            case DynamicAmount.Player_Vessel_Count:
                FinalAmount = CombatSystem.Instance.Player_Permanents.Count;
                break;

            case DynamicAmount.Enemy_Vessel_Count:
                FinalAmount = CombatSystem.Instance.Enemy_Permanents.Count;
                break;

            case DynamicAmount.Player_Vessel_Shielded:
                foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                {
                    if (item.Shielded)
                    {
                        FinalAmount++;
                    }
                }
                break;

            case DynamicAmount.Enemy_Vessel_Shielded:
                foreach (EnemySlotView item in CombatSystem.Instance.Enemy_Permanents)
                {
                    if (item.Shielded)
                    {
                        FinalAmount++;
                    }
                }
                break;

            case DynamicAmount.SpellCast_This_Turn:
                FinalAmount = CombatSystem.Instance.SpellCast_This_Turn;
                break;

            case DynamicAmount.PermanentCast_This_Turn:
                FinalAmount = CombatSystem.Instance.PermanentCast_This_Turn;
                break;

            case DynamicAmount.Artiley_Count:
                foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                {
                    if (item.permaTypes.Contains(PermaTypes.Artillery))
                    {
                        FinalAmount++;
                    }
                }
                break;

            case DynamicAmount.Decay_Count:
                foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                {
                    if (item.permaTypes.Contains(PermaTypes.Decay))
                    {
                        FinalAmount++;
                    }
                }
                break;

            case DynamicAmount.Hollow_Count:
                foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                {
                    if (item.permaTypes.Contains(PermaTypes.Hollow))
                    {
                        FinalAmount++;
                    }
                }
                break;

            case DynamicAmount.Invoc_Count:
                foreach (PermanentView item in CombatSystem.Instance.Player_Permanents)
                {
                    if (item.permaTypes.Contains(PermaTypes.Invoc))
                    {
                        FinalAmount++;
                    }
                }
                break;

            case DynamicAmount.Mana_Count:
                FinalAmount = ManaSystem.Instance.currentMana;
                break;

            case DynamicAmount.Mana_Spent_Count:
                FinalAmount = ManaSystem.Instance.Mana_Spent_Count;
                break;

            case DynamicAmount.Permanent_HP:
                if (permanentView != null)
                {
                    FinalAmount = permanentView.currentLife;
                }
                else if (enemySlotView != null)
                {
                    FinalAmount = enemySlotView.currentLife;
                }
                break;

            case DynamicAmount.Permanent_Endurance:
                if (permanentView != null)
                {
                    FinalAmount = permanentView.Durability;
                }
                else if (enemySlotView != null)
                {
                    FinalAmount = 0;
                }
                break;
            case DynamicAmount.CardsInHand_Count:
                FinalAmount = CardSystem.Instance.hand.Count;
                break;

            case DynamicAmount.NULL:
                break;

            default:
                break;
        }

        return FinalAmount;
    }

    public bool CheckTargetLimitation(TargetLimitationInfo info, Card Card = null, PermanentView permanent = null, EnemySlotView enemySlot = null)
    {
        switch (info.targetLimitations)
        {
            case TargetLimitations.NULL:
                return true; // aucune contrainte


            case TargetLimitations.Only_Player_Permanent:
                return permanent != null && CombatSystem.Instance.Player_Permanents.Contains(permanent);

            case TargetLimitations.Only_Enemy_Permanent:
                return enemySlot != null && CombatSystem.Instance.Enemy_Permanents.Contains(enemySlot);

            case TargetLimitations.Only_Type_Permanent:
                if (permanent != null)
                    return permanent.permaTypes.Contains(info.PermaType);
                if (enemySlot != null)
                    return enemySlot.permaTypes.Contains(info.PermaType);
                return false;

            case TargetLimitations.Only_ActivablePermanent:
                if (permanent != null)
                {
                    if (permanent.CardReferenceArchive != null)
                    {
                        foreach (Effect effect in permanent.CardReferenceArchive.Effects)
                        {
                            if (effect.Events == Events.OnActivate)
                            {
                                return true;
                            }
                        }                        
                    }
                }
                if (enemySlot != null)             
                {
                    foreach (Effect effect in enemySlot.PossibleIntent)
                    {
                        if (effect.Events == Events.OnActivate)
                        {
                            return true;
                        }
                    }                       
                }
                return false;

            case TargetLimitations.NO_Player_Core:
                if (permanent != null)
                    return !permanent.IsCore;
                if (enemySlot != null)
                    return true;
                return false;

            case TargetLimitations.NO_Enemy_Core:
                if (permanent != null)
                    return true;
                if (enemySlot != null)
                {
                    return !enemySlot.IsCore;                    
                }
                return false;

            case TargetLimitations.Permanent_HP:
                if (permanent != null)
                    return permanent.currentLife == info.IntValue;
                if (enemySlot != null)
                    return enemySlot.currentLife == info.IntValue;
                return false;
            case TargetLimitations.Permanent_HP_More_Than_Value:
                if (permanent != null)
                    return permanent.currentLife > info.IntValue;
                if (enemySlot != null)
                    return enemySlot.currentLife > info.IntValue;
                return false;
            case TargetLimitations.Permanent_HP_Less_Than_Value:
                if (permanent != null)
                    return permanent.currentLife < info.IntValue;
                if (enemySlot != null)
                    return enemySlot.currentLife < info.IntValue;
                return false;

            case TargetLimitations.Permanent_Endurance:
                if (permanent != null)
                    return permanent.Durability == info.IntValue;
                return false;
            case TargetLimitations.Permanent_Endurance_More_Than_Value:
                if (permanent != null)
                    return permanent.Durability > info.IntValue;
                return false;
            case TargetLimitations.Permanent_Endurance_Less_Than_Value:
                if (permanent != null)
                    return permanent.Durability < info.IntValue;
                return false;

            case TargetLimitations.Card_Cost_Value:
                return Card != null && Card.cost == info.IntValue;

            case TargetLimitations.Card_Cost_More_Than_Value:
                return Card != null && Card.cost > info.IntValue;

            case TargetLimitations.Card_Cost_Less_Than_Value:
                return Card != null && Card.cost < info.IntValue;

            default:
                return false;
        }
    }

    public bool PassesAllLimitations(List<TargetLimitationInfo> limitations, Card Card, PermanentView playerPerm, EnemySlotView enemyPerm, bool checkEnoughtTarget = false)
    {
        if (limitations == null || limitations.Count == 0)
            return true;

        foreach (var limitation in limitations)
        {
            if (checkEnoughtTarget)
            {
                if (!limitation.MandatoryLimitation) continue;
            }

            if (playerPerm != null)
            {
                if (playerPerm.UnTargetable)
                {
                    return false;
                }
            }
            else if (enemyPerm != null)
            {
                if (enemyPerm.UnTargetable)
                {
                    return false;
                }
            }
            else if (Card != null)
            {
                if(Card.UnTargetable)
                {
                    return false;
                }
            }
            
            if (!CheckTargetLimitation(limitation, Card, playerPerm, enemyPerm))
            {
                return false;
            }
        }
        return true;
    }

    public bool limitationHasEnoughtTarget(List<TargetLimitationInfo> limitations, int EffectTargetNumber, int MultiHit)
    {
        List<PermanentView> playerPermanents = CombatSystem.Instance.Player_Permanents;
        List<EnemySlotView> enemyPermanents = CombatSystem.Instance.Enemy_Permanents;
        List<Card> allCards = CardSystem.Instance.hand;

        List<object> validTargets = new List<object>();

        foreach (var p in playerPermanents)
        {
            if (PassesAllLimitations(limitations, null, p, null, true))
            {
                validTargets.Add(p);
            }
        }

        foreach (var e in enemyPermanents)
        {
            if (PassesAllLimitations(limitations, null, null, e, true))
            {
                validTargets.Add(e);
            }
        }

        foreach (var c in allCards)
        {
            if (PassesAllLimitations(limitations, c, null, null, true))
                validTargets.Add(c);
        }

        //Debug.Log("There is " + validTargets.Count + " valid cible if mandatory limitation");

        // On regarde si il y a assez de cibles valide
        return (validTargets.Count * MultiHit) >= EffectTargetNumber;
    }
    
    public void ActivateAuraForTargets(List<TargetLimitationInfo> limitations)
    {
        List<PermanentView> playerPermanents = CombatSystem.Instance.Player_Permanents;
        List<EnemySlotView> enemyPermanents = CombatSystem.Instance.Enemy_Permanents;
        List<Card> allCards = CardSystem.Instance.hand;
        PTargets_ForAura = new List<PermanentView>();
        ETargets_ForAura = new List<EnemySlotView>();

        List<object> validTargets = new List<object>();

        foreach (var p in playerPermanents)
        {
            if (PassesAllLimitations(limitations, null, p, null, false))
            {
                validTargets.Add(p);
                p.ActivateAuraVisual();
            }
        }

        foreach (var e in enemyPermanents)
        {
            if (PassesAllLimitations(limitations, null, null, e, false))
            {
                validTargets.Add(e);
                e.ActivateAuraVisual();
            }
        }

        foreach (var c in allCards)
        {
            if (PassesAllLimitations(limitations, c, null, null, false))
                validTargets.Add(c);
        }

        //Debug.Log("There is " + validTargets.Count + " AuraActivated");

        foreach (object item in validTargets)
        {
            if (item is PermanentView)
            {
                PTargets_ForAura.Add((PermanentView)item);
            }
            else if (item is EnemySlotView)
            {
                ETargets_ForAura.Add((EnemySlotView)item);
            }
        }

        return;        
    }
}
