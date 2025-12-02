using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using FMODUnity;
using TMPro;

public class TargetSystem : Singleton<TargetSystem>
{
    [SerializeField] private LayerMask TargetingLayerMask;
    [SerializeField] private LayerMask CardviewTargetingLayerMask;
    [SerializeField] private LayerMask CardviewExhaustTargetingLayerMask;
    [SerializeField] private TMP_Text TopPrompt;
    [SerializeField] public List<Card> DeckData;
    [SerializeField] public GameObject DisplayDeckZone;
    [SerializeField] private GameObject CursorGameobject;
    private bool TargetingActive;
    public bool CardTargetingActive;
    private int InitTargetingNumber;
    private bool TargetingUpTo;
    private bool TargetExhaust;
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

        ActivateAuraForTargets(startManualTargetingGA.TargetLimitations);

        StartManualTargeting();
        SetPrompt(TargetingNumber, TargetingUpTo, "Target");
        
        while (TargetingActive)
            yield return null;

        (enemyTargets, playerTargets) = EndManualTargeting();
        ResetPrompt();

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
        TargetExhaust = false;
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

        // Pour les cas ou on Target l'Exhaust
        if (startCardTargetingGA.TargetExhaust)
        {
            TargetExhaust = true;
            DeckData = CardSystem.Instance.ExhaustPile;
            if (DisplayDeckZone.activeSelf)
            {
                DisplayDeckZone.SetActive(false);
                DeckViewSystem.Instance.CleanDisplay();
            }

            DisplayDeckZone.SetActive(true);
            DeckViewSystem.Instance.DisplayCards(DeckData, true);
        }

        StartCardTargeting();
        SetPrompt(TargetingNumber,TargetingUpTo,"Card");

        while (CardTargetingActive)
            yield return null;

        cardViewTargets = EndCardTargeting();
        ResetPrompt();

        List<Card> CardTargets = new();
        foreach (CardView item in cardViewTargets)
        {
            CardTargets.Add(item.Card);
        }
        startCardTargetingGA.EffectRef.TargetForLinked_Card = new List<Card>(CardTargets);

        // Vérifie qu'il y a bien les propriétés attendues
        var action = startCardTargetingGA.ActionToRealiseAfterTargetting;
        var type = action.GetType();
        var CardTargetsProp = type.GetProperty("cardTargets");
        var CardviewTargetsProp = type.GetProperty("CardViews");

        if (CardviewTargetsProp != null)
        {
            CardviewTargetsProp.SetValue(action, cardViewTargets);
        }
        else if (CardTargetsProp != null)
        {
            CardTargetsProp.SetValue(action,CardTargets);
        }
        else
        {
            Debug.LogError("L'action ne contient pas la propriétés CardViews ni cardTargets");
        }

        CombatSystem.Instance.Interactable = true;
        ActionSystem.Instance.AddReaction(startCardTargetingGA.ActionToRealiseAfterTargetting);
    }

    public void SetPrompt(int TargetNumber, bool TargetUpTo, string NatureOfTarget)
    {
        TopPrompt.gameObject.SetActive(true);
        if (TargetNumber == 1 || TargetNumber == 0)
        {
            TopPrompt.text = "Select " + (TargetUpTo ? "Up To " : "") + TargetNumber + " " + NatureOfTarget;
        }
        else
        {
            TopPrompt.text = "Select " + (TargetUpTo ? "Up To " : "") + TargetNumber + " " + NatureOfTarget + "s";
        }
        
    }

    public void ResetPrompt()
    {
        TopPrompt.gameObject.SetActive(false);
        TopPrompt.text = "";
    }

    public static (List<PermanentView> playerTargets, List<EnemySlotView> enemyTargets) GetTargets(TargetModeInfo TargetModeInfo, GameObject actionner, Effect effect)
    {
        List<PermanentView> ValidatePlayerTargets = new();
        List<PermanentView> playerTargets = new();
        List<EnemySlotView> ValidateEnemyTargets = new();
        List<EnemySlotView> enemyTargets = new();

        List<PermanentView> playerPermanents = CombatSystem.Instance.Player_Permanents;
        List<EnemySlotView> enemyPermanents = CombatSystem.Instance.Enemy_Permanents;

        List<PermanentView> TampontargetsP = new List<PermanentView>();
        List<EnemySlotView> TampontargetsE = new List<EnemySlotView>();

        //Redirection de cible vers core dans le cas d'une attaque enemy et pas de target
        bool RedirectionActive = false;
        if (actionner != null)
        {
            EnemySlotView TestIfPermaIsEnemy = actionner.GetComponent<EnemySlotView>();
            if (TestIfPermaIsEnemy != null)
            {
                // On veut une redirection si l'effet qui va être lancé est une attaque enemie
                if ((effect is DealDamageEffect) || (effect is LifeLossEffect))
                {
                    RedirectionActive = true;
                }
            }
            else
            {
                RedirectionActive = false;
            }
        }

        switch (TargetModeInfo.targetMode)
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
            case TargetMode.All:
                switch (TargetModeInfo.PlayerOrEnemy)
                {
                    case Enemy_Player_ENUM.Player:
                        foreach (var perm in playerPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            playerTargets.Add(perm);
                        }
                        break;

                    case Enemy_Player_ENUM.Enemy:
                        foreach (var perm in enemyPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            enemyTargets.Add(perm);
                        }
                        break;

                    case Enemy_Player_ENUM.NULL:
                        foreach (var perm in playerPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            playerTargets.Add(perm);
                        }
                        foreach (var perm in enemyPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            enemyTargets.Add(perm);
                        }
                        break;
                }
                if (RedirectionActive)
                {
                    if (playerTargets.Count == 0 && enemyTargets.Count == 0)
                    {
                        foreach (var perm in playerPermanents)
                            if (perm.IsCore && !perm.UnTargetable) playerTargets.Add(perm);
                    }
                }
                break;

            case TargetMode.RDM:

                List<PermanentView> targetablePlayers = new();
                List<EnemySlotView> targetableEnemies = new();

                switch (TargetModeInfo.PlayerOrEnemy)
                {
                    case Enemy_Player_ENUM.Player:
                        foreach (var perm in playerPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            targetablePlayers.Add(perm);
                        }
                        if (targetablePlayers.Count > 0)
                        {
                            if (effect is HealEffect)
                            {
                                List<PermanentView> DamagedPermanents = targetablePlayers.Where(p => p.currentLife != p.MaxLife).ToList();
                                if (DamagedPermanents.Count != 0)
                                {
                                    var rnd = Random.Range(0, DamagedPermanents.Count);
                                    playerTargets.Add(DamagedPermanents[rnd]);
                                }
                                else
                                {
                                    var rnd = Random.Range(0, targetablePlayers.Count);
                                    playerTargets.Add(targetablePlayers[rnd]);
                                }
                            }
                            else
                            {
                                var rnd = Random.Range(0, targetablePlayers.Count);
                                playerTargets.Add(targetablePlayers[rnd]);
                            }                            
                        }
                        break;

                    case Enemy_Player_ENUM.Enemy:
                        foreach (var perm in enemyPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            targetableEnemies.Add(perm);
                        }
                        if (targetableEnemies.Count > 0)
                        {
                            if (effect is HealEffect)
                            {
                                List<EnemySlotView> DamagedPermanents = targetableEnemies.Where(p => p.currentLife != p.MaxLife).ToList();
                                if (DamagedPermanents.Count != 0)
                                {
                                    var rnd = Random.Range(0, DamagedPermanents.Count);
                                    enemyTargets.Add(DamagedPermanents[rnd]);
                                }
                                else
                                {
                                    var rnd = Random.Range(0, targetableEnemies.Count);
                                    enemyTargets.Add(targetableEnemies[rnd]);
                                }
                            }
                            else
                            {
                                var rnd = Random.Range(0, targetableEnemies.Count);
                                enemyTargets.Add(targetableEnemies[rnd]);
                            }    
                        }
                        break;

                    case Enemy_Player_ENUM.NULL:
                        foreach (var perm in playerPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            targetablePlayers.Add(perm);
                        }
                        foreach (var perm in enemyPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            targetableEnemies.Add(perm);
                        }

                        PermanentView permanentViewSelected = null;
                        EnemySlotView enemySlotViewSelected = null;

                        if (targetablePlayers.Count > 0)
                        {
                            if (effect is HealEffect)
                            {
                                List<PermanentView> DamagedPermanents = targetablePlayers.Where(p => p.currentLife != p.MaxLife).ToList();
                                if (DamagedPermanents.Count != 0)
                                {
                                    var rnd = Random.Range(0, DamagedPermanents.Count);
                                    permanentViewSelected = DamagedPermanents[rnd];
                                }
                                else
                                {
                                    var rnd = Random.Range(0, targetablePlayers.Count);
                                    permanentViewSelected = targetablePlayers[rnd];
                                }
                            }
                            else
                            {
                                var rnd = Random.Range(0, targetablePlayers.Count);
                                permanentViewSelected = targetablePlayers[rnd];
                            }   
                        }
                        if (targetableEnemies.Count > 0)
                        {
                            if (effect is HealEffect)
                            {
                                List<EnemySlotView> DamagedPermanents = targetableEnemies.Where(p => p.currentLife != p.MaxLife).ToList();
                                if (DamagedPermanents.Count != 0)
                                {
                                    var rnd = Random.Range(0, DamagedPermanents.Count);
                                    enemySlotViewSelected = DamagedPermanents[rnd];
                                }
                                else
                                {
                                    var rnd = Random.Range(0, targetableEnemies.Count);
                                    enemySlotViewSelected = targetableEnemies[rnd];
                                }
                            }
                            else
                            {
                                var rnd = Random.Range(0, targetableEnemies.Count);
                                enemySlotViewSelected = targetableEnemies[rnd];
                            } 
                        }

                        if (permanentViewSelected != null && enemySlotViewSelected == null)
                        {
                            playerTargets.Add(permanentViewSelected);
                        }
                        else if (permanentViewSelected == null && enemySlotViewSelected != null)
                        {
                            enemyTargets.Add(enemySlotViewSelected);
                        }
                        else
                        {
                            var rnd = Random.Range(0, 1);
                            if (rnd == 0)
                            {
                                playerTargets.Add(permanentViewSelected);
                            }
                            else
                            {
                                enemyTargets.Add(enemySlotViewSelected);
                            }
                        }
                        break;
                }
                if (RedirectionActive)
                {
                    if (playerTargets.Count == 0 && enemyTargets.Count == 0)
                    {
                        foreach (var perm in playerPermanents)
                            if (perm.IsCore && !perm.UnTargetable) playerTargets.Add(perm);
                    }
                }
                break;

            case TargetMode.Core:
                switch (TargetModeInfo.PlayerOrEnemy)
                {
                    case Enemy_Player_ENUM.Player:
                        foreach (var perm in playerPermanents)
                            if (perm.IsCore && !perm.UnTargetable) playerTargets.Add(perm);
                        break;
                    case Enemy_Player_ENUM.Enemy:
                        foreach (var perm in enemyPermanents)
                            if (perm.IsCore && !perm.UnTargetable) enemyTargets.Add(perm);
                        break;
                }
                break;

            case TargetMode.HighHP:

                List<PermanentView> ValidPlayers = new();
                List<EnemySlotView> ValidEnemies = new();
                int maxTotal = 0;
                List<PermanentView> highestTargetsP = new();
                List<EnemySlotView> highestTargetsE = new();

                switch (TargetModeInfo.PlayerOrEnemy)
                {
                    case Enemy_Player_ENUM.Player:
                        foreach (var perm in playerPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            ValidPlayers.Add(perm);
                        }
                        maxTotal = highestTargetsP.Max(p => p.currentLife);
                        highestTargetsP = ValidPlayers.Where(p => p.currentLife == maxTotal).ToList();
                        if (highestTargetsP.Count > 0)
                        {
                            var selected = highestTargetsP[Random.Range(0, highestTargetsP.Count)];
                            playerTargets.Add(selected);
                        }
                        break;

                    case Enemy_Player_ENUM.Enemy:
                        foreach (var perm in enemyPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            ValidEnemies.Add(perm);
                        }
                        maxTotal = highestTargetsE.Max(p => p.currentLife);
                        highestTargetsE = ValidEnemies.Where(p => p.currentLife == maxTotal).ToList();
                        if (highestTargetsE.Count > 0)
                        {
                            var selected = highestTargetsE[Random.Range(0, highestTargetsE.Count)];
                            enemyTargets.Add(selected);
                        }
                        break;

                    case Enemy_Player_ENUM.NULL:

                        PermanentView permanentViewSelected = null;
                        EnemySlotView enemySlotViewSelected = null;

                        foreach (var perm in playerPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            ValidPlayers.Add(perm);
                        }
                        maxTotal = highestTargetsP.Max(p => p.currentLife);
                        highestTargetsP = ValidPlayers.Where(p => p.currentLife == maxTotal).ToList();
                        if (highestTargetsP.Count > 0)
                        {
                            var selected = highestTargetsP[Random.Range(0, highestTargetsP.Count)];
                            permanentViewSelected = selected;
                        }

                        foreach (var perm in enemyPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            ValidEnemies.Add(perm);
                        }
                        maxTotal = highestTargetsE.Max(p => p.currentLife);
                        highestTargetsE = ValidEnemies.Where(p => p.currentLife == maxTotal).ToList();
                        if (highestTargetsE.Count > 0)
                        {
                            var selected = highestTargetsE[Random.Range(0, highestTargetsE.Count)];
                            enemySlotViewSelected = selected;
                        }

                        if (permanentViewSelected != null && enemySlotViewSelected == null)
                        {
                            playerTargets.Add(permanentViewSelected);
                        }
                        else if (permanentViewSelected == null && enemySlotViewSelected != null)
                        {
                            enemyTargets.Add(enemySlotViewSelected);
                        }
                        else
                        {
                            var rnd = Random.Range(0, 1);
                            if (rnd == 0)
                            {
                                playerTargets.Add(permanentViewSelected);
                            }
                            else
                            {
                                enemyTargets.Add(enemySlotViewSelected);
                            }
                        }
                        break;
                }
                if (RedirectionActive)
                {
                    if (playerTargets.Count == 0 && enemyTargets.Count == 0)
                    {
                        foreach (var perm in playerPermanents)
                            if (perm.IsCore && !perm.UnTargetable) playerTargets.Add(perm);
                    }
                }
                break;

            case TargetMode.LowHP:
                List<PermanentView> ValidPlayers2 = new();
                List<EnemySlotView> ValidEnemies2 = new();
                int minTotal = 0;
                List<PermanentView> LowestTargetsP = new();
                List<EnemySlotView> LowestTargetsE = new();

                switch (TargetModeInfo.PlayerOrEnemy)
                {
                    case Enemy_Player_ENUM.Player:
                        foreach (var perm in playerPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            ValidPlayers2.Add(perm);
                        }
                        minTotal = LowestTargetsP.Min(p => p.currentLife);
                        LowestTargetsP = ValidPlayers2.Where(p => p.currentLife == minTotal).ToList();
                        if (LowestTargetsP.Count > 0)
                        {
                            var selected = LowestTargetsP[Random.Range(0, LowestTargetsP.Count)];
                            playerTargets.Add(selected);
                        }
                        break;

                    case Enemy_Player_ENUM.Enemy:
                        foreach (var perm in enemyPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            ValidEnemies2.Add(perm);
                        }
                        minTotal = LowestTargetsE.Min(p => p.currentLife);
                        LowestTargetsE = ValidEnemies2.Where(p => p.currentLife == minTotal).ToList();
                        if (LowestTargetsE.Count > 0)
                        {
                            var selected = LowestTargetsE[Random.Range(0, LowestTargetsE.Count)];
                            enemyTargets.Add(selected);
                        }
                        break;

                    case Enemy_Player_ENUM.NULL:

                        PermanentView permanentViewSelected = null;
                        EnemySlotView enemySlotViewSelected = null;

                        foreach (var perm in playerPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            ValidPlayers2.Add(perm);
                        }
                        minTotal = LowestTargetsP.Min(p => p.currentLife);
                        LowestTargetsP = ValidPlayers2.Where(p => p.currentLife == minTotal).ToList();
                        if (LowestTargetsP.Count > 0)
                        {
                            var selected = LowestTargetsP[Random.Range(0, LowestTargetsP.Count)];
                            permanentViewSelected = selected;
                        }

                        foreach (var perm in enemyPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            ValidEnemies2.Add(perm);
                        }
                        minTotal = LowestTargetsE.Min(p => p.currentLife);
                        LowestTargetsE = ValidEnemies2.Where(p => p.currentLife == minTotal).ToList();
                        if (LowestTargetsE.Count > 0)
                        {
                            var selected = LowestTargetsE[Random.Range(0, LowestTargetsE.Count)];
                            enemySlotViewSelected = selected;
                        }

                        if (permanentViewSelected != null && enemySlotViewSelected == null)
                        {
                            playerTargets.Add(permanentViewSelected);
                        }
                        else if (permanentViewSelected == null && enemySlotViewSelected != null)
                        {
                            enemyTargets.Add(enemySlotViewSelected);
                        }
                        else
                        {
                            var rnd = Random.Range(0, 1);
                            if (rnd == 0)
                            {
                                playerTargets.Add(permanentViewSelected);
                            }
                            else
                            {
                                enemyTargets.Add(enemySlotViewSelected);
                            }
                        }
                        break;
                }
                if (RedirectionActive)
                {
                    if (playerTargets.Count == 0 && enemyTargets.Count == 0)
                    {
                        foreach (var perm in playerPermanents)
                            if (perm.IsCore && !perm.UnTargetable) playerTargets.Add(perm);
                    }
                }
                break;

            case TargetMode.HighCost:
                List<PermanentView> ValidPlayers3 = new();
                int maxCostTotal = 0;
                List<PermanentView> highestcostTargetsP = new();
                switch (TargetModeInfo.PlayerOrEnemy)
                {
                    case Enemy_Player_ENUM.Player:
                        foreach (var perm in playerPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            ValidPlayers3.Add(perm);
                        }
                        maxCostTotal = highestcostTargetsP.Max(p => p.CardReferenceArchive.cost + p.CardReferenceArchive.BonusCost);
                        highestcostTargetsP = ValidPlayers3.Where(p => p.CardReferenceArchive.cost + p.CardReferenceArchive.BonusCost == maxCostTotal).ToList();
                        if (highestcostTargetsP.Count > 0)
                        {
                            var selected = highestcostTargetsP[Random.Range(0, highestcostTargetsP.Count)];
                            playerTargets.Add(selected);
                        }
                        break;

                    case Enemy_Player_ENUM.NULL:
                        foreach (var perm in playerPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            ValidPlayers3.Add(perm);
                        }
                        maxCostTotal = highestcostTargetsP.Max(p => p.CardReferenceArchive.cost + p.CardReferenceArchive.BonusCost);
                        highestcostTargetsP = ValidPlayers3.Where(p => p.CardReferenceArchive.cost + p.CardReferenceArchive.BonusCost == maxCostTotal).ToList();
                        if (highestcostTargetsP.Count > 0)
                        {
                            var selected = highestcostTargetsP[Random.Range(0, highestcostTargetsP.Count)];
                            playerTargets.Add(selected);
                        }
                        break;
                }
                if (RedirectionActive)
                {
                    if (playerTargets.Count == 0 && enemyTargets.Count == 0)
                    {
                        foreach (var perm in playerPermanents)
                            if (perm.IsCore && !perm.UnTargetable) playerTargets.Add(perm);
                    }
                }
                break;

            case TargetMode.LowCost:
                List<PermanentView> ValidPlayers4 = new();
                int minCostTotal = 0;
                List<PermanentView> lowestcostTargetsP = new();
                switch (TargetModeInfo.PlayerOrEnemy)
                {
                    case Enemy_Player_ENUM.Player:
                        foreach (var perm in playerPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            ValidPlayers4.Add(perm);
                        }
                        minCostTotal = lowestcostTargetsP.Min(p => p.CardReferenceArchive.cost + p.CardReferenceArchive.BonusCost);
                        lowestcostTargetsP = ValidPlayers4.Where(p => p.CardReferenceArchive.cost + p.CardReferenceArchive.BonusCost == minCostTotal).ToList();
                        if (lowestcostTargetsP.Count > 0)
                        {
                            var selected = lowestcostTargetsP[Random.Range(0, lowestcostTargetsP.Count)];
                            playerTargets.Add(selected);
                        }
                        break;

                    case Enemy_Player_ENUM.NULL:
                        foreach (var perm in playerPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.PermaType != PermaTypes.NULL) if (!perm.permaTypes.Contains(TargetModeInfo.PermaType)) continue;
                            ValidPlayers4.Add(perm);
                        }
                        minCostTotal = lowestcostTargetsP.Min(p => p.CardReferenceArchive.cost + p.CardReferenceArchive.BonusCost);
                        lowestcostTargetsP = ValidPlayers4.Where(p => p.CardReferenceArchive.cost + p.CardReferenceArchive.BonusCost == minCostTotal).ToList();
                        if (lowestcostTargetsP.Count > 0)
                        {
                            var selected = lowestcostTargetsP[Random.Range(0, lowestcostTargetsP.Count)];
                            playerTargets.Add(selected);
                        }
                        break;
                }
                if (RedirectionActive)
                {
                    if (playerTargets.Count == 0 && enemyTargets.Count == 0)
                    {
                        foreach (var perm in playerPermanents)
                            if (perm.IsCore && !perm.UnTargetable) playerTargets.Add(perm);
                    }
                }
                break;
        }

        return (playerTargets, enemyTargets);
    }
    
    public static List<Card> GetCardsTargets(TargetModeInfo TargetModeInfo, Card CardActionner, bool IncludeCardsInDeck = false, bool ExhaustCardList = false)
    {
        List<Card> cardsTargets = new();
        List<Card> cardsList = new();
        List<Card> ValidcardsList = new();
        int Amount = 0;

        if (ExhaustCardList)
        {
            foreach (Card card in CardSystem.Instance.ExhaustPile)
            {
                cardsList.Add(card);
            }
        }
        else
        {
            if (IncludeCardsInDeck)
            {
                foreach (Card card in CardSystem.Instance.hand)
                {
                    cardsList.Add(card);
                }
                foreach (Card card in CardSystem.Instance.discardPile)
                {
                    cardsList.Add(card);
                }
                foreach (Card card in CardSystem.Instance.drawPile)
                {
                    cardsList.Add(card);
                }
            }
            else
            {
                foreach (Card card in CardSystem.Instance.hand)
                {
                    cardsList.Add(card);
                }
            }                
        }

        switch (TargetModeInfo.targetMode)
        {
            case TargetMode.Self:
                if (CardActionner != null)
                {
                    cardsTargets.Add(CardActionner);
                }
                break;
                
            case TargetMode.All:
                switch (TargetModeInfo.PlayerOrEnemy)
                {
                    case Enemy_Player_ENUM.NULL:
                        foreach (var card in cardsList)
                        {
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (card.permanentArea != TargetModeInfo.permanentArea) continue;
                            switch (TargetModeInfo.PermaType)
                            {
                                case PermaTypes.Hollow:
                                    if (card.IsSpell) continue;
                                    if (card.data.Durability == 0)
                                    cardsTargets.Add(card);
                                    break;
                                case PermaTypes.Decay:
                                    if (card.IsSpell) continue;
                                    if (card.data.DecayCounter > 0)
                                    cardsTargets.Add(card);
                                    break;
                                case PermaTypes.Invoc:
                                    if (card.IsSpell) continue;
                                    if (card.data.isInvoc)
                                    cardsTargets.Add(card);
                                    break;
                                case PermaTypes.Artillery:
                                    if (card.IsSpell) continue;
                                    if (card.data.isArtillery)
                                    cardsTargets.Add(card);
                                    break;
                                case PermaTypes.Spell_Card:
                                    if(card.IsSpell) cardsTargets.Add(card);
                                    break;
                                case PermaTypes.Perma_Card:
                                    if(!card.IsSpell) cardsTargets.Add(card);
                                    break;
                                case PermaTypes.NULL:
                                    cardsTargets.Add(card);
                                    break;                                    
                            }
                        }
                        break;
                }
                break;

            case TargetMode.RDM:
                switch (TargetModeInfo.PlayerOrEnemy)
                {
                    case Enemy_Player_ENUM.NULL:
                        foreach (var card in cardsList)
                        {
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (card.permanentArea != TargetModeInfo.permanentArea) continue;
                            switch (TargetModeInfo.PermaType)
                            {
                                case PermaTypes.Hollow:
                                    if (card.IsSpell) continue;
                                    if (card.data.Durability == 0)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Decay:
                                    if (card.IsSpell) continue;
                                    if (card.data.DecayCounter > 0)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Invoc:
                                    if (card.IsSpell) continue;
                                    if (card.data.isInvoc)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Artillery:
                                    if (card.IsSpell) continue;
                                    if (card.data.isArtillery)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Spell_Card:
                                    if (card.IsSpell) ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Perma_Card:
                                    if (!card.IsSpell) ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.NULL:
                                    ValidcardsList.Add(card);
                                    break;
                            }
                        }
                        break;
                }
                if (ValidcardsList.Count > 0)
                {
                    Card selected = ValidcardsList[Random.Range(0, ValidcardsList.Count)];
                    cardsTargets.Add(selected);
                }
                break;
                
            case TargetMode.HighHP:
                switch (TargetModeInfo.PlayerOrEnemy)
                {
                    case Enemy_Player_ENUM.NULL:
                        foreach (var card in cardsList)
                        {
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (card.permanentArea != TargetModeInfo.permanentArea) continue;
                            switch (TargetModeInfo.PermaType)
                            {
                                case PermaTypes.Hollow:
                                    if (card.IsSpell) continue;
                                    if (card.data.Durability == 0)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Decay:
                                    if (card.IsSpell) continue;
                                    if (card.data.DecayCounter > 0)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Invoc:
                                    if (card.IsSpell) continue;
                                    if (card.data.isInvoc)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Artillery:
                                    if (card.IsSpell) continue;
                                    if (card.data.isArtillery)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Spell_Card:
                                    if (card.IsSpell) ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Perma_Card:
                                    if (!card.IsSpell) ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.NULL:
                                    ValidcardsList.Add(card);
                                    break;
                            }
                        }
                        break;
                }
                Amount = ValidcardsList.Max(p => p.life);
                ValidcardsList = ValidcardsList.Where(p => p.life == Amount).ToList();
                if (ValidcardsList.Count > 0)
                {
                    Card selected = ValidcardsList[Random.Range(0, ValidcardsList.Count)];
                    cardsTargets.Add(selected);
                }
                break;
                
            case TargetMode.LowHP:
                switch (TargetModeInfo.PlayerOrEnemy)
                {
                    case Enemy_Player_ENUM.NULL:
                        foreach (var card in cardsList)
                        {
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (card.permanentArea != TargetModeInfo.permanentArea) continue;
                            switch (TargetModeInfo.PermaType)
                            {
                                case PermaTypes.Hollow:
                                    if (card.IsSpell) continue;
                                    if (card.data.Durability == 0)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Decay:
                                    if (card.IsSpell) continue;
                                    if (card.data.DecayCounter > 0)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Invoc:
                                    if (card.IsSpell) continue;
                                    if (card.data.isInvoc)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Artillery:
                                    if (card.IsSpell) continue;
                                    if (card.data.isArtillery)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Spell_Card:
                                    if (card.IsSpell) ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Perma_Card:
                                    if (!card.IsSpell) ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.NULL:
                                    ValidcardsList.Add(card);
                                    break;
                            }
                        }
                        break;
                }
                Amount = ValidcardsList.Min(p => p.life);
                ValidcardsList = ValidcardsList.Where(p => p.life == Amount).ToList();
                if (ValidcardsList.Count > 0)
                {
                    Card selected = ValidcardsList[Random.Range(0, ValidcardsList.Count)];
                    cardsTargets.Add(selected);
                }
                break;

            case TargetMode.HighCost:
                switch (TargetModeInfo.PlayerOrEnemy)
                {
                    case Enemy_Player_ENUM.NULL:
                        foreach (var card in cardsList)
                        {
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (card.permanentArea != TargetModeInfo.permanentArea) continue;
                            switch (TargetModeInfo.PermaType)
                            {
                                case PermaTypes.Hollow:
                                    if (card.IsSpell) continue;
                                    if (card.data.Durability == 0)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Decay:
                                    if (card.IsSpell) continue;
                                    if (card.data.DecayCounter > 0)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Invoc:
                                    if (card.IsSpell) continue;
                                    if (card.data.isInvoc)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Artillery:
                                    if (card.IsSpell) continue;
                                    if (card.data.isArtillery)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Spell_Card:
                                    if (card.IsSpell) ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Perma_Card:
                                    if (!card.IsSpell) ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.NULL:
                                    ValidcardsList.Add(card);
                                    break;
                            }
                        }
                        break;
                }
                Amount = ValidcardsList.Max(p => p.cost);
                ValidcardsList = ValidcardsList.Where(p => p.cost == Amount).ToList();
                if (ValidcardsList.Count > 0)
                {
                    Card selected = ValidcardsList[Random.Range(0, ValidcardsList.Count)];
                    cardsTargets.Add(selected);
                }
                break;

            case TargetMode.LowCost:
                switch (TargetModeInfo.PlayerOrEnemy)
                {
                    case Enemy_Player_ENUM.NULL:
                        foreach (var card in cardsList)
                        {
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (card.permanentArea != TargetModeInfo.permanentArea) continue;
                            switch (TargetModeInfo.PermaType)
                            {
                                case PermaTypes.Hollow:
                                    if (card.IsSpell) continue;
                                    if (card.data.Durability == 0)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Decay:
                                    if (card.IsSpell) continue;
                                    if (card.data.DecayCounter > 0)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Invoc:
                                    if (card.IsSpell) continue;
                                    if (card.data.isInvoc)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Artillery:
                                    if (card.IsSpell) continue;
                                    if (card.data.isArtillery)
                                        ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Spell_Card:
                                    if (card.IsSpell) ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.Perma_Card:
                                    if (!card.IsSpell) ValidcardsList.Add(card);
                                    break;
                                case PermaTypes.NULL:
                                    ValidcardsList.Add(card);
                                    break;
                            }
                        }
                        break;
                }
                Amount = ValidcardsList.Min(p => p.cost);
                ValidcardsList = ValidcardsList.Where(p => p.cost == Amount).ToList();
                if (ValidcardsList.Count > 0)
                {
                    Card selected = ValidcardsList[Random.Range(0, ValidcardsList.Count)];
                    cardsTargets.Add(selected);
                }
                break;
        }

        return cardsTargets;
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
        if (TargetExhaust)
        {
            DisplayDeckZone.SetActive(false);
            DeckViewSystem.Instance.CleanDisplay();
        }
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
                                SetPrompt(TargetingNumber, TargetingUpTo, "Target");
                            }
                        }
                        else
                        {
                            if (TargetingNumber < InitTargetingNumber)
                            {
                                enemySlots.Remove(enemyView);
                                enemyView.RemoveSelectEffect();
                                TargetingNumber += 1;
                                SetPrompt(TargetingNumber, TargetingUpTo, "Target");
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
                                SetPrompt(TargetingNumber, TargetingUpTo, "Target");
                            }
                        }
                        else
                        {
                            if (TargetingNumber < InitTargetingNumber)
                            {
                                permanents.Remove(permanentView);
                                permanentView.RemoveSelectEffect();
                                TargetingNumber += 1;
                                SetPrompt(TargetingNumber, TargetingUpTo, "Target");
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
                    foreach (CardView cardview in CardTargets)
                    {
                        cardview.RemoveSelectEffect(false);
                    }
                }
                else
                {
                    if (TargetingNumber == 0)
                    {
                        CardTargetingActive = false;
                        foreach (CardView cardview in CardTargets)
                        {
                            cardview.RemoveSelectEffect(false);
                        }
                    }
                }
            }
            if (TargetExhaust)
            {
                if (Input.GetMouseButtonDown(0)) // 0 = clic gauche 1 = clic droit
                {
                    Debug.DrawRay(CursorGameobject.transform.position + new Vector3(0, 0, -1), Vector3.forward * 10f, Color.red, 1f);
                    if (Physics.Raycast(CursorGameobject.transform.position + new Vector3(0, 0, -1), Vector3.forward, out RaycastHit raycastHit, 10f, CardviewExhaustTargetingLayerMask) && raycastHit.collider != null && raycastHit.transform.TryGetComponent(out CardView cardView))
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
                                    SetPrompt(TargetingNumber, TargetingUpTo, "Card");
                                }
                            }
                            else
                            {
                                if (TargetingNumber < InitTargetingNumber)
                                {
                                    CardTargets.Remove(cardView);
                                    cardView.RemoveSelectEffect(true);
                                    TargetingNumber += 1;
                                    SetPrompt(TargetingNumber, TargetingUpTo, "Card");
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
                                    SetPrompt(TargetingNumber, TargetingUpTo, "Card");
                                }
                            }
                            else
                            {
                                if (TargetingNumber < InitTargetingNumber)
                                {
                                    CardTargets.Remove(cardView);
                                    cardView.RemoveSelectEffect(true);
                                    TargetingNumber += 1;
                                    SetPrompt(TargetingNumber, TargetingUpTo, "Card");
                                }
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
                    if (enemyView != null)
                    {
                        bool HasEffectsToActivate = false;
                        if (enemyView.IntentAction == null) return;
                        if (enemyView.IntentAction.Events.Contains(Events.OnSelect) && enemyView.IntentAction.ActivateNumber >= 1)
                        {
                            if(enemyView.IntentAction.ActivateLeft > 0)
                            {
                                HasEffectsToActivate = true;
                            }
                        }
                        
                        if (HasEffectsToActivate)
                        {
                            TriggerEventGA triggerEnemyEventGA = new(Events.OnSelect, null, null, enemyView);
                            ActionSystem.Instance.Perform(triggerEnemyEventGA);
                        }                   
                    }
                }
                else if (Physics.Raycast(CursorGameobject.transform.position + new Vector3(0, 0, -1), Vector3.forward, out RaycastHit raycastHit2, 10f, TargetingLayerMask) && raycastHit2.collider != null && raycastHit2.transform.TryGetComponent(out PermanentView permanentView))
                {
                    if (permanents != null)
                    {
                        bool HasEffectsToActivate = false;
                        foreach (Effect effect in GameEventSystem.Instance.RetrieveEffectsFor(null,permanentView,null))
                        {
                            if (effect.Events.Contains(Events.OnSelect) && effect.ActivateNumber >= 1)
                            {
                                if (effect.ActivateLeft > 0)
                                {
                                    HasEffectsToActivate = true;
                                }
                            }                           
                        }                 

                        if (HasEffectsToActivate)
                        {
                            TriggerEventGA triggerPermanentEventGA = new(Events.OnSelect, null, permanentView, null);
                            ActionSystem.Instance.Perform(triggerPermanentEventGA);
                        }                        
                    }
                }
            }
        }
    }

    public int GetDynamicAmount(DynamicAmount dynamicAmount, PermanentView permanentView = null, EnemySlotView enemySlotView = null, Card CardActionner = null)
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

            case DynamicAmount.Player_Weapon_Count:
                int i = 0;
                foreach (var perm in CombatSystem.Instance.Player_Permanents)
                {
                    if (perm.permanentArea == PermanentArea.Weapon)
                    {
                        i++;
                    }
                }
                FinalAmount = i;
                break;

            case DynamicAmount.Player_Shield_Count:
                i = 0;
                foreach (var perm in CombatSystem.Instance.Player_Permanents)
                {
                    if (perm.permanentArea == PermanentArea.Shield)
                    {
                        i++;
                    }
                }
                FinalAmount = i;
                break;

            case DynamicAmount.Player_Support_Count:
                i = 0;
                foreach (var perm in CombatSystem.Instance.Player_Permanents)
                {
                    if (perm.permanentArea == PermanentArea.Support)
                    {
                        i++;
                    }
                }
                FinalAmount = i;
                break;

            case DynamicAmount.Enemy_Vessel_Count:
                FinalAmount = CombatSystem.Instance.Enemy_Permanents.Count;
                break;

            case DynamicAmount.Enemy_Weapon_Count:
                i = 0;
                foreach (var perm in CombatSystem.Instance.Enemy_Permanents)
                {
                    if (perm.permanentArea == PermanentArea.Weapon)
                    {
                        i++;
                    }
                }
                FinalAmount = i;
                break;

            case DynamicAmount.Enemy_Shield_Count:
                i = 0;
                foreach (var perm in CombatSystem.Instance.Enemy_Permanents)
                {
                    if (perm.permanentArea == PermanentArea.Shield)
                    {
                        i++;
                    }
                }
                FinalAmount = i;
                break;

            case DynamicAmount.Enemy_Support_Count:
                i = 0;
                foreach (var perm in CombatSystem.Instance.Enemy_Permanents)
                {
                    if (perm.permanentArea == PermanentArea.Support)
                    {
                        i++;
                    }
                }
                FinalAmount = i;
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
                FinalAmount = CombatSystem.Instance.GlobalCounters.Get(CounterType.SpellCast_This_Turn);
                break;

            case DynamicAmount.PermanentCast_This_Turn:
                FinalAmount = CombatSystem.Instance.GlobalCounters.Get(CounterType.PermanentCast_This_Turn);
                break;

            case DynamicAmount.Artilery_Count:
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

            case DynamicAmount.PayXValue:
                if (permanentView != null)
                {
                    FinalAmount = permanentView.CardReferenceArchive.PayXValue;
                }
                else if (CardActionner != null)
                {
                    FinalAmount = CardActionner.PayXValue;
                }
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

            case TargetLimitations.PermanentIsNotType:
                if (permanent != null)
                    return !permanent.permaTypes.Contains(info.PermaType);
                if (enemySlot != null)
                    return !enemySlot.permaTypes.Contains(info.PermaType);
                return false;

            case TargetLimitations.Only_SelectablePermanent:
                if (permanent != null)
                {
                    if (permanent.CardReferenceArchive != null)
                    {
                        foreach (Effect effect in permanent.CardReferenceArchive.Effects)
                        {
                            if (effect.Events.Contains(Events.OnSelect))
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
                        if (effect.Events.Contains(Events.OnSelect))
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
                return Card != null && Mathf.Max(0, Card.cost + Card.BonusCost)== info.IntValue;

            case TargetLimitations.Card_Cost_More_Than_Value:
                return Card != null && Mathf.Max(0, Card.cost + Card.BonusCost) > info.IntValue;

            case TargetLimitations.Card_Cost_Less_Than_Value:
                return Card != null && Mathf.Max(0, Card.cost + Card.BonusCost) < info.IntValue;

            case TargetLimitations.Only_Activated:
                if (permanent != null)
                {
                    bool HasEffectsActivated = false;
                    foreach (Effect effect in GameEventSystem.Instance.RetrieveEffectsFor(null,permanent,null))
                    {
                        if (effect.Events.Contains(Events.OnSelect) && effect.ActivateNumber >= 1)
                        {
                            if (effect.ActivateLeft != effect.ActivateNumber)
                            {
                                HasEffectsActivated = true;
                            }
                        }
                    }
                    return HasEffectsActivated;       
                }
                if (enemySlot != null)
                {
                    bool HasEffectsToActivate = false;
                    foreach (Effect effect in GameEventSystem.Instance.RetrieveEffectsFor(null, null, enemySlot))
                    {
                        if (effect.Events.Contains(Events.OnSelect) && effect.ActivateNumber >= 1)
                        {
                            if (effect.ActivateLeft != effect.ActivateNumber)
                            {
                                HasEffectsToActivate = true;
                            }
                        }                        
                    }

                    return HasEffectsToActivate;                
                }
                return false;

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
