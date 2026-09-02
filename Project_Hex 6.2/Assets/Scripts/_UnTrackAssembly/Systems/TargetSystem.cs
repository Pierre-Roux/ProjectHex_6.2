using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using TMPro;
using System.Runtime.CompilerServices;

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
    public bool ShieldEffectTargeting = false;
    private int InitTargetingNumber;
    private bool TargetingUpTo;
    private bool TargetExhaust;
    private bool WaitingValidation;
    private int TargetingNumber;
    private int MaxTargeting;
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

        //ActivateAuraForTargets(startManualTargetingGA.TargetLimitations);

        if (startManualTargetingGA.ActionToRealiseAfterTargetting is ShieldGA || startManualTargetingGA.ActionToRealiseAfterTargetting is ShieldPlayerGA || startManualTargetingGA.ActionToRealiseAfterTargetting is ShieldEnemyGA)
        {
            ShieldEffectTargeting = true;
        }

        TargetingNumber = InitTargetingNumber = startManualTargetingGA.TargetNumber;
        MaxTargeting = TargetingNumber;
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

        if ((startManualTargetingGA.EffectRef.CanBeDisableEffect && enemyTargets.Count != 0) || (startManualTargetingGA.EffectRef.CanBeDisableEffect && playerTargets.Count != 0))
        {
            PermanentView permanentView;
            EnemySlotView enemySlotView;
            if (startManualTargetingGA.EffectRef.Actionner != null)
            {
                if (startManualTargetingGA.EffectRef.Actionner.GetComponent<PermanentView>() != null)
                {
                    permanentView = startManualTargetingGA.EffectRef.Actionner.GetComponent<PermanentView>();
                    permanentView.ToggleableEffects.Add(startManualTargetingGA.EffectRef);
                }
                else if (startManualTargetingGA.EffectRef.Actionner.GetComponent<EnemySlotView>() != null)
                {
                    enemySlotView = startManualTargetingGA.EffectRef.Actionner.GetComponent<EnemySlotView>();
                    enemySlotView.ToggleableEffects.Add(startManualTargetingGA.EffectRef);
                }
            }
        }

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

        ShieldEffectTargeting = false;

        ActionSystem.Instance.AddReaction(startManualTargetingGA.ActionToRealiseAfterTargetting);
    }

    public IEnumerator GetCardTargetsPerformer(StartCardTargetingGA startCardTargetingGA)
    {
        TargetExhaust = false;
        List<CardView> cardViewTargets;
        TargetingNumber = InitTargetingNumber = startCardTargetingGA.TargetNumber;
        MaxTargeting = TargetingNumber;
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

        if (startCardTargetingGA.EffectRef.CanBeDisableEffect && cardViewTargets.Count != 0)
        {
            PermanentView permanentView;
            EnemySlotView enemySlotView;
            if (startCardTargetingGA.EffectRef.Actionner != null)
            {
                if (startCardTargetingGA.EffectRef.Actionner.GetComponent<PermanentView>() != null)
                {
                    permanentView = startCardTargetingGA.EffectRef.Actionner.GetComponent<PermanentView>();
                    permanentView.ToggleableEffects.Add(startCardTargetingGA.EffectRef);
                }
                else if (startCardTargetingGA.EffectRef.Actionner.GetComponent<EnemySlotView>() != null)
                {
                    enemySlotView = startCardTargetingGA.EffectRef.Actionner.GetComponent<EnemySlotView>();
                    enemySlotView.ToggleableEffects.Add(startCardTargetingGA.EffectRef);
                }
            }
        }

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

    public void SetPromptValidation(int TargetLeft)
    {
        TopPrompt.gameObject.SetActive(true);
        if (TargetLeft == 1)
        {
            TopPrompt.text = "Are you sure ?  " + TargetLeft + " Target left";
        }
        else
        {
            TopPrompt.text = "Are you sure ?  " + TargetLeft + " Targets left";
        }
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

        // ici On vient check si l'effet est un shiel effect pour ne pas target les permanents qui seraient des shield eux même
        bool ShieldEffectTargeting = false;
        if ( effect is ShieldEffect)
        {
            ShieldEffectTargeting = true;
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
                        if (ShieldEffectTargeting && self.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) break;
                        if (self != null)
                            playerTargets.Add(self);
                    }
                    else
                    {
                        var self = actionner.GetComponent<EnemySlotView>();
                        if (ShieldEffectTargeting && self.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) break;
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
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
                            playerTargets.Add(perm);
                        }
                        break;

                    case Enemy_Player_ENUM.Enemy:
                        foreach (var perm in enemyPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
                            enemyTargets.Add(perm);
                        }
                        break;

                    case Enemy_Player_ENUM.NULL:
                        foreach (var perm in playerPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
                            playerTargets.Add(perm);
                        }
                        foreach (var perm in enemyPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
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
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
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
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
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
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
                            targetablePlayers.Add(perm);
                        }
                        foreach (var perm in enemyPermanents)
                        {
                            if (perm.UnTargetable) continue;
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
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
                        {
                            var CoreKeyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Core);
                            if (CoreKeyword != null && !perm.UnTargetable)
                            {
                                playerTargets.Add(perm); 
                            }                                                    
                        }
                        break;
                    case Enemy_Player_ENUM.Enemy:
                        foreach (var perm in enemyPermanents)
                        {
                            var CoreKeyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Core);
                            if (CoreKeyword != null && !perm.UnTargetable)
                            {
                                enemyTargets.Add(perm); 
                            }                          
                        }
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
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
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
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
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
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
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
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
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
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
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
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
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
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
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
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
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
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
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
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
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
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
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
                            if (ShieldEffectTargeting && perm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null) continue;
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (perm.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = perm.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
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
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = card.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
                            else
                            {
                                cardsTargets.Add(card);
                            }
                        }
                        break;

                    case Enemy_Player_ENUM.Card:
                        foreach (var card in cardsList)
                        {
                            if (TargetModeInfo.permanentArea != PermanentArea.NONE) if (card.permanentArea != TargetModeInfo.permanentArea) continue;
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = card.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
                            else
                            {
                                cardsTargets.Add(card);
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
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = card.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
                            else
                            {
                                ValidcardsList.Add(card);
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
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = card.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
                            else
                            {
                                ValidcardsList.Add(card);
                            }
                        }
                        break;
                }
                Amount = ValidcardsList.Max(p => p.Life);
                ValidcardsList = ValidcardsList.Where(p => p.Life == Amount).ToList();
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
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = card.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
                            else
                            {
                                ValidcardsList.Add(card);
                            }
                        }
                        break;
                }
                Amount = ValidcardsList.Min(p => p.Life);
                ValidcardsList = ValidcardsList.Where(p => p.Life == Amount).ToList();
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
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = card.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
                            else
                            {
                                ValidcardsList.Add(card);
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
                            if (TargetModeInfo.keyWordType != KeyWordType.NULL)
                            {
                                var Keyword = card.KeyWords.FirstOrDefault(k => k.keyWordType == TargetModeInfo.keyWordType);
                                if (Keyword == null) continue;
                            }
                            else
                            {
                                ValidcardsList.Add(card);
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
        WaitingValidation = false;
        TargetingActive = true;
    }

    public (List<EnemySlotView> enemyTargets, List<PermanentView> playerTargets) EndManualTargeting()
    {
        TargetingActive = false;
        WaitingValidation = false;
        return (enemySlots, permanents);
    }

    public void StartCardTargeting()
    {
        CardTargets.Clear();
        WaitingValidation = false;
        CardTargetingActive = true;
    }

    public List<CardView> EndCardTargeting()
    {
        CardTargetingActive = false;
        WaitingValidation = false;
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
                    if (WaitingValidation == true)
                    {
                        WaitingValidation = false;
                    }
                    else
                    {
                        if (TargetingNumber == MaxTargeting)
                        {
                            SetPromptValidation(TargetingNumber);
                            WaitingValidation = true;
                        }
                        else
                        {
                            WaitingValidation = false;
                        }
                    }
                    
                    if (WaitingValidation == false)
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
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, 15f, TargetingLayerMask);

                bool hitTargetingLayerMask = false;
                RaycastHit raycastHit = new RaycastHit();

                foreach (var hit in hits)
                {
                    if (hit.collider != null)
                    {
                        hitTargetingLayerMask = true;
                        raycastHit = hit;
                    }
                }
                if (hitTargetingLayerMask && raycastHit.collider != null && raycastHit.transform.TryGetComponent(out EnemySlotView enemyView))
                {
                    if (!ShieldEffectTargeting || ShieldEffectTargeting && enemyView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) == null)
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
                }
                else if (hitTargetingLayerMask && raycastHit.collider != null && raycastHit.transform.TryGetComponent(out PermanentView permanentView))
                {
                    if (!ShieldEffectTargeting || ShieldEffectTargeting && permanentView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) == null)
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
        }
        else if (CardTargetingActive)
        {
            if (Input.GetKeyDown(KeyCode.Space)) // Espace = confirmer
            {
                if (TargetingUpTo)
                {
                    if (WaitingValidation == true)
                    {
                        WaitingValidation = false;
                    }
                    else
                    {
                        if (TargetingNumber == MaxTargeting)
                        {
                            SetPromptValidation(TargetingNumber);
                            WaitingValidation = true;
                        }
                        else
                        {
                            WaitingValidation = false;
                        }
                    }

                    if (WaitingValidation == false)
                    {
                        // ici ajouter une validation pour être sur
                        CardTargetingActive = false;
                        foreach (CardView cardview in CardTargets)
                        {
                            cardview.RemoveSelectEffect(false);
                        }
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
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit[] hits = Physics.RaycastAll(ray, 15f, CardviewExhaustTargetingLayerMask);

                    bool hitTargetingLayerMask = false;
                    RaycastHit raycastHit = new RaycastHit();

                    foreach (var hit in hits)
                    {
                        if (hit.collider != null)
                        {
                            hitTargetingLayerMask = true;
                            raycastHit = hit;
                        }
                    }
                    if (hitTargetingLayerMask && raycastHit.collider != null && raycastHit.transform.TryGetComponent(out CardView cardView))
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
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit[] hits = Physics.RaycastAll(ray, 15f, CardviewTargetingLayerMask);

                    bool hitTargetingLayerMask = false;
                    RaycastHit raycastHit = new RaycastHit();

                    foreach (var hit in hits)
                    {
                        if (hit.collider != null)
                        {
                            hitTargetingLayerMask = true;
                            raycastHit = hit;
                        }
                    }
                    if (hitTargetingLayerMask && raycastHit.collider != null && raycastHit.transform.TryGetComponent(out CardView cardView))
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
                
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, 15f, TargetingLayerMask);

                bool hitTargetingLayerMask = false;
                RaycastHit raycastHit = new RaycastHit();

                foreach (var hit in hits)
                {
                    if (hit.collider != null)
                    {
                        hitTargetingLayerMask = true;
                        raycastHit = hit;
                    }
                }

                if (hitTargetingLayerMask && raycastHit.collider != null && raycastHit.transform.TryGetComponent(out EnemySlotView enemyView))
                {
                    if (enemyView != null)
                    {
                        bool HasEffectsToActivate = false;
                        if (enemyView.IntentAction == null) return;
                        foreach (EventInfo eventInfo in enemyView.IntentAction.EventInfos)
                        {
                            if (eventInfo.Events == Events.OnSelect && enemyView.IntentAction.ActivateNumber >= 1)
                            {
                                if(enemyView.IntentAction.ActivateLeft > 0)
                                {
                                    HasEffectsToActivate = true;
                                }
                            }
                        }
                        
                        if (HasEffectsToActivate)
                        {
                            EventInfo eventInfo = new EventInfo(Events.OnSelect, Enemy_Player_ENUM.Enemy, KeyWordType.NULL);
                            TriggerEventGA triggerEventGA = new(eventInfo, null, null, null, enemyView);
                            ActionSystem.Instance.Perform(triggerEventGA);
                        }                   
                    }
                }
                else if (hitTargetingLayerMask && raycastHit.collider != null && raycastHit.transform.TryGetComponent(out PermanentView permanentView))
                {
                    if (permanents != null)
                    {
                        bool HasEffectsToActivate = false;
                        foreach (Effect effect in GameEventSystem.Instance.RetrieveEffectsFor(null,permanentView,null))
                        {
                            foreach (EventInfo eventInfo in effect.EventInfos)
                            {
                                if (eventInfo.Events == Events.OnSelect && effect.ActivateNumber >= 1)
                                {
                                    var HollowKeyword = permanentView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Hollow);     
                                    if ((effect.ActivateLeft > 0 && HollowKeyword != null && effect.HollowEffect) || (effect.ActivateLeft > 0  && HollowKeyword == null && !effect.HollowEffect))
                                    {
                                        HasEffectsToActivate = true;
                                    }                                    
                                }
                            }                           
                        }                 

                        if (HasEffectsToActivate)
                        {
                            EventInfo eventInfo = new EventInfo(Events.OnSelect, Enemy_Player_ENUM.Player, KeyWordType.NULL);
                            TriggerEventGA triggerEventGA = new(eventInfo, null, null, permanentView, null);
                            ActionSystem.Instance.Perform(triggerEventGA);
                        }                        
                    }
                }
            }
        }
    }

    public int GetDynamicAmount(DynamicAmountInfo dynamicAmountInfo, PermanentView permanentView = null, EnemySlotView enemySlotView = null, Card CardActionner = null)
    {
        int FinalAmount = 0;
    
        switch (dynamicAmountInfo.DynamicAmount)
        {
            case DynamicAmount.Count:
                FinalAmount = 0;
                if (dynamicAmountInfo.Enemy_Player == Enemy_Player_ENUM.Player)
                {
                    foreach (PermanentView Perma in CombatSystem.Instance.Player_Permanents)
                    {
                        if (dynamicAmountInfo.TestType != KeyWordType.NULL)
                        {
                            var KeywordMatch = Perma.KeyWords.FirstOrDefault(k => k.keyWordType == dynamicAmountInfo.TestType);
                            if (KeywordMatch != null)
                            {
                                FinalAmount++;
                            }
                        }
                        else
                        {
                            FinalAmount++;
                        }
                    }
                }
                else if (dynamicAmountInfo.Enemy_Player == Enemy_Player_ENUM.Enemy)
                {
                    foreach (EnemySlotView Perma in CombatSystem.Instance.Enemy_Permanents)
                    {
                        if (dynamicAmountInfo.TestType != KeyWordType.NULL)
                        {
                            var KeywordMatch = Perma.KeyWords.FirstOrDefault(k => k.keyWordType == dynamicAmountInfo.TestType);
                            if (KeywordMatch != null)
                            {
                                FinalAmount++;
                            }
                        }
                        else
                        {
                            FinalAmount++;
                        }
                    }
                }
                else if (dynamicAmountInfo.Enemy_Player == Enemy_Player_ENUM.Card)
                {
                    List<Card> CardList = new List<Card>();
                    if (dynamicAmountInfo.CardLocation == CardLocation.Hand)
                    {
                        CardList = CardSystem.Instance.hand;
                    }
                    else if (dynamicAmountInfo.CardLocation == CardLocation.Deck)
                    {
                        CardList = CardSystem.Instance.drawPile;
                    }
                    else if (dynamicAmountInfo.CardLocation == CardLocation.Discard)
                    {
                        CardList = CardSystem.Instance.discardPile;
                    }
                    else if (dynamicAmountInfo.CardLocation == CardLocation.Exhaust)
                    {
                        CardList = CardSystem.Instance.ExhaustPile;
                    }
                    else
                    {
                        CardList = CardSystem.Instance.hand;
                        CardList = CardSystem.Instance.drawPile;
                        CardList = CardSystem.Instance.discardPile;
                    }

                    foreach (Card card in CardList)
                    {
                        if (dynamicAmountInfo.TestType != KeyWordType.NULL)
                        {
                            var KeywordMatch = card.KeyWords.FirstOrDefault(k => k.keyWordType == dynamicAmountInfo.TestType);
                            if (KeywordMatch != null)
                            {
                                FinalAmount++;
                            }
                        }
                        else
                        {
                            FinalAmount++;
                        }                     
                    }                    
                }
                else
                {
                    FinalAmount = CombatSystem.Instance.Player_Permanents.Count;
                }
                break;

            case DynamicAmount.CounterType:
                FinalAmount = 0;
                if (dynamicAmountInfo.CounterType.Intern)
                {
                    if (permanentView != null)
                    {
                        foreach (var counters in permanentView.InternCounters.counters)
                        {
                            if (counters.Key.CounterType == dynamicAmountInfo.CounterType.CounterType)
                            {
                                FinalAmount = counters.Value;
                                break;
                            }
                        }
                    }
                    else if (enemySlotView != null)
                    {
                        foreach (var counters in enemySlotView.InternCounters.counters)
                        {
                            if (counters.Key.CounterType == dynamicAmountInfo.CounterType.CounterType)
                            {
                                FinalAmount = counters.Value;
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (var counters in CardActionner.InternCounters.counters)
                        {
                            if (counters.Key.CounterType == dynamicAmountInfo.CounterType.CounterType)
                            {
                                FinalAmount = counters.Value;
                                break;
                            }
                        }                        
                    }
                }
                else
                {
                    foreach (var counters in CombatSystem.Instance.GlobalCounters.counters)
                    {
                        if (counters.Key.CounterType == dynamicAmountInfo.CounterType.CounterType)
                        {
                            FinalAmount = counters.Value;
                            break;
                        }
                    }
                }
                break;

            case DynamicAmount.TargetParam:
                FinalAmount = 0;
                if (permanentView != null)
                {
                    switch (dynamicAmountInfo.BasicParam)
                    {
                        case BasicParam.Life:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = permanentView.currentLife;
                            }
                            else
                            {
                                FinalAmount = permanentView.MaxLife;
                            }
                            break;

                        case BasicParam.Armor:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = permanentView.currentArmor;
                            }
                            else
                            {
                                FinalAmount = permanentView.CardReferenceArchive.Armor;
                            }
                            break;

                        case BasicParam.Power:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = permanentView.currentPower;
                            }
                            else
                            {
                                FinalAmount = permanentView.CardReferenceArchive.Power;
                            }
                            break;

                        case BasicParam.Durability:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = permanentView.Durability;
                            }
                            else
                            {
                                FinalAmount = permanentView.CardReferenceArchive.Durability;
                            }
                            break;

                        case BasicParam.Cost:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = permanentView.CardReferenceArchive.cost;
                            }
                            else
                            {
                                FinalAmount = permanentView.CardReferenceArchive.InitCost;
                            }
                            break;

                        case BasicParam.NULL:
                            break;
                    }
                }
                else if (enemySlotView != null)
                {
                    switch (dynamicAmountInfo.BasicParam)
                    {
                        case BasicParam.Life:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = enemySlotView.currentLife;
                            }
                            else
                            {
                                FinalAmount = enemySlotView.MaxLife;
                            }
                            break;

                        case BasicParam.Armor:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = enemySlotView.currentArmor;
                            }
                            else
                            {
                                FinalAmount = enemySlotView.PermanentData.Armor;
                            }
                            break;

                        case BasicParam.Power:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = enemySlotView.currentPower;
                            }
                            else
                            {
                                FinalAmount = enemySlotView.PermanentData.Power;
                            }
                            break;

                        case BasicParam.NULL:
                            break;
                    }
                }
                else if (CardActionner != null)
                {
                    switch (dynamicAmountInfo.BasicParam)
                    {
                        case BasicParam.Life:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = CardActionner.Life;
                            }
                            else
                            {
                                FinalAmount = CardActionner.Life;
                            }
                            break;

                        case BasicParam.Armor:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = CardActionner.Armor;
                            }
                            else
                            {
                                FinalAmount = CardActionner.Armor;
                            }
                            break;

                        case BasicParam.Power:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = CardActionner.Power;
                            }
                            else
                            {
                                FinalAmount = CardActionner.Power;
                            }
                            break;

                        case BasicParam.Durability:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = CardActionner.Durability;
                            }
                            else
                            {
                                FinalAmount = CardActionner.Durability;
                            }
                            break;

                        case BasicParam.Cost:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = CardActionner.cost;
                            }
                            else
                            {
                                FinalAmount = CardActionner.InitCost;
                            }
                            break;

                        case BasicParam.NULL:
                            break;
                    }
                }
                break;

            case DynamicAmount.SelfParam:
                FinalAmount = 0;
                if (permanentView != null)
                {
                    switch (dynamicAmountInfo.BasicParam)
                    {
                        case BasicParam.Life:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = permanentView.currentLife;
                            }
                            else
                            {
                                FinalAmount = permanentView.CardReferenceArchive.InitLife;
                            }
                            break;

                        case BasicParam.Armor:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = permanentView.currentArmor;
                            }
                            else
                            {
                                FinalAmount = permanentView.CardReferenceArchive.InitArmor;
                            }
                            break;

                        case BasicParam.Power:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = permanentView.currentPower;
                            }
                            else
                            {
                                FinalAmount = permanentView.CardReferenceArchive.InitPower;
                            }
                            break;

                        case BasicParam.Durability:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = permanentView.Durability;
                            }
                            else
                            {
                                FinalAmount = permanentView.CardReferenceArchive.InitDurability;
                            }
                            break;

                        case BasicParam.Cost:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = permanentView.CardReferenceArchive.cost;
                            }
                            else
                            {
                                FinalAmount = permanentView.CardReferenceArchive.InitCost;
                            }
                            break;

                        case BasicParam.NULL:
                            break;
                    }
                }
                else if (enemySlotView != null)
                {
                    switch (dynamicAmountInfo.BasicParam)
                    {
                        case BasicParam.Life:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = enemySlotView.currentLife;
                            }
                            else
                            {
                                FinalAmount = enemySlotView.MaxLife;
                            }
                            break;

                        case BasicParam.Armor:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = enemySlotView.currentArmor;
                            }
                            else
                            {
                                FinalAmount = enemySlotView.PermanentData.Armor;
                            }
                            break;

                        case BasicParam.Power:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = enemySlotView.currentPower;
                            }
                            else
                            {
                                FinalAmount = enemySlotView.PermanentData.Power;
                            }
                            break;

                        case BasicParam.NULL:
                            break;
                    }
                }
                else if (CardActionner != null)
                {
                    switch (dynamicAmountInfo.BasicParam)
                    {
                        case BasicParam.Life:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = CardActionner.Life;
                            }
                            else
                            {
                                FinalAmount = CardActionner.InitLife;
                            }
                            break;

                        case BasicParam.Armor:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = CardActionner.Armor;
                            }
                            else
                            {
                                FinalAmount = CardActionner.InitArmor;
                            }
                            break;

                        case BasicParam.Power:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = CardActionner.Power;
                            }
                            else
                            {
                                FinalAmount = CardActionner.InitPower;
                            }
                            break;

                        case BasicParam.Durability:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = CardActionner.Durability;
                            }
                            else
                            {
                                FinalAmount = CardActionner.InitDurability;
                            }
                            break;

                        case BasicParam.Cost:
                            if (dynamicAmountInfo.CurrentParam)
                            {
                                FinalAmount = CardActionner.cost;
                            }
                            else
                            {
                                FinalAmount = CardActionner.InitCost;
                            }
                            break;

                        case BasicParam.NULL:
                            break;
                    }
                }
                break;

            case DynamicAmount.ShieldedCount:
                FinalAmount = 0;
                if (dynamicAmountInfo.Enemy_Player == Enemy_Player_ENUM.Player)
                {
                    foreach (PermanentView perma in CombatSystem.Instance.Player_Permanents)
                    {
                        if (perma.Shielded)
                        {
                            FinalAmount++;
                        }
                    }
                }
                else if (dynamicAmountInfo.Enemy_Player == Enemy_Player_ENUM.Enemy)
                {
                    foreach (EnemySlotView perma in CombatSystem.Instance.Enemy_Permanents)
                    {
                        if (perma.Shielded)
                        {
                            FinalAmount++;
                        }
                    }
                }
                else
                {
                    foreach (PermanentView perma in CombatSystem.Instance.Player_Permanents)
                    {
                        if (perma.Shielded)
                        {
                            FinalAmount++;
                        }
                    }  
                    foreach (EnemySlotView perma in CombatSystem.Instance.Enemy_Permanents)
                    {
                        if (perma.Shielded)
                        {
                            FinalAmount++;
                        }
                    }                  
                }
                break;

            case DynamicAmount.CurrentMana:
                FinalAmount = ManaSystem.Instance.currentMana;
                break;

            case DynamicAmount.ManaSpent:
                FinalAmount = ManaSystem.Instance.Mana_Spent_Count;
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

            case TargetLimitations.OnlyOwnerType:
                if (permanent != null)
                {
                    if (info.Owner == Enemy_Player_ENUM.NULL || info.Owner == Enemy_Player_ENUM.Player)
                    {
                        if (info.keyWordType != KeyWordType.NULL)
                        {
                            var KeywordMatch = permanent.KeyWords.FirstOrDefault(k => k.keyWordType == info.keyWordType);
                            if (KeywordMatch != null)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else if (enemySlot != null)
                {
                    if (info.Owner == Enemy_Player_ENUM.NULL || info.Owner == Enemy_Player_ENUM.Enemy)
                    {
                        if (info.keyWordType != KeyWordType.NULL)
                        {
                            var KeywordMatch = enemySlot.KeyWords.FirstOrDefault(k => k.keyWordType == info.keyWordType);
                            if (KeywordMatch != null)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else if (Card != null)
                {
                    if (info.Owner == Enemy_Player_ENUM.NULL || info.Owner == Enemy_Player_ENUM.Card)
                    {
                        if (info.keyWordType != KeyWordType.NULL)
                        {
                            var KeywordMatch = Card.KeyWords.FirstOrDefault(k => k.keyWordType == info.keyWordType);
                            if (KeywordMatch != null)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                break;

            case TargetLimitations.ExceptOwnerType:
                if (permanent != null)
                {
                    if (info.Owner == Enemy_Player_ENUM.NULL || info.Owner != Enemy_Player_ENUM.Player)
                    {
                        if (info.keyWordType != KeyWordType.NULL)
                        {
                            var KeywordMatch = permanent.KeyWords.FirstOrDefault(k => k.keyWordType == info.keyWordType);
                            if (KeywordMatch == null)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else if (enemySlot != null)
                {
                    if (info.Owner == Enemy_Player_ENUM.NULL || info.Owner != Enemy_Player_ENUM.Enemy)
                    {
                        if (info.keyWordType != KeyWordType.NULL)
                        {
                            var KeywordMatch = enemySlot.KeyWords.FirstOrDefault(k => k.keyWordType == info.keyWordType);
                            if (KeywordMatch == null)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else if (Card != null)
                {
                    if (info.Owner == Enemy_Player_ENUM.NULL || info.Owner != Enemy_Player_ENUM.Card)
                    {
                        if (info.keyWordType != KeyWordType.NULL)
                        {
                            var KeywordMatch = Card.KeyWords.FirstOrDefault(k => k.keyWordType == info.keyWordType);
                            if (KeywordMatch == null)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                break;

            case TargetLimitations.Param_More_Than_Value:
                if (permanent != null)
                {
                    if (info.Owner == Enemy_Player_ENUM.NULL || info.Owner == Enemy_Player_ENUM.Player)
                    {
                        if (info.Param != BasicParam.NULL)
                        {
                            switch (info.Param)
                            {
                                case BasicParam.Life:
                                    if (permanent.currentLife > info.ParamValue) return true;
                                        break;
                                case BasicParam.Power:
                                    if (permanent.currentPower > info.ParamValue) return true;
                                    break;
                                case BasicParam.Durability:
                                    if (permanent.Durability > info.ParamValue) return true;
                                    break;
                                case BasicParam.Armor:
                                    if (permanent.currentArmor > info.ParamValue) return true;
                                    break;
                                case BasicParam.Cost:
                                    if (permanent.CardReferenceArchive.cost > info.ParamValue) return true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                else if (enemySlot != null)
                {
                    if (info.Owner == Enemy_Player_ENUM.NULL || info.Owner == Enemy_Player_ENUM.Player)
                    {
                        if (info.Param != BasicParam.NULL)
                        {
                            switch (info.Param)
                            {
                                case BasicParam.Life:
                                    if (enemySlot.currentLife > info.ParamValue) return true;
                                        break;
                                case BasicParam.Power:
                                    if (enemySlot.currentPower > info.ParamValue) return true;
                                    break;
                                case BasicParam.Armor:
                                    if (enemySlot.currentArmor > info.ParamValue) return true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                else if (Card != null)
                {
                    if (info.Owner == Enemy_Player_ENUM.NULL || info.Owner == Enemy_Player_ENUM.Player)
                    {
                        if (info.Param != BasicParam.NULL)
                        {
                            switch (info.Param)
                            {
                                case BasicParam.Life:
                                    if (Card.Life > info.ParamValue) return true;
                                        break;
                                case BasicParam.Power:
                                    if (Card.Power > info.ParamValue) return true;
                                    break;
                                case BasicParam.Armor:
                                    if (Card.Armor > info.ParamValue) return true;
                                    break;
                                case BasicParam.Durability:
                                    if (Card.Durability > info.ParamValue) return true;
                                    break;
                                case BasicParam.Cost:
                                    if (Card.cost > info.ParamValue) return true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                break;

            case TargetLimitations.Param_Less_Than_Value:
                if (permanent != null)
                {
                    if (info.Owner == Enemy_Player_ENUM.NULL || info.Owner == Enemy_Player_ENUM.Player)
                    {
                        if (info.Param != BasicParam.NULL)
                        {
                            switch (info.Param)
                            {
                                case BasicParam.Life:
                                    if (permanent.currentLife < info.ParamValue) return true;
                                        break;
                                case BasicParam.Power:
                                    if (permanent.currentPower < info.ParamValue) return true;
                                    break;
                                case BasicParam.Durability:
                                    if (permanent.Durability < info.ParamValue) return true;
                                    break;
                                case BasicParam.Armor:
                                    if (permanent.currentArmor < info.ParamValue) return true;
                                    break;
                                case BasicParam.Cost:
                                    if (permanent.CardReferenceArchive.cost < info.ParamValue) return true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                else if (enemySlot != null)
                {
                    if (info.Owner == Enemy_Player_ENUM.NULL || info.Owner == Enemy_Player_ENUM.Player)
                    {
                        if (info.Param != BasicParam.NULL)
                        {
                            switch (info.Param)
                            {
                                case BasicParam.Life:
                                    if (enemySlot.currentLife < info.ParamValue) return true;
                                        break;
                                case BasicParam.Power:
                                    if (enemySlot.currentPower < info.ParamValue) return true;
                                    break;
                                case BasicParam.Armor:
                                    if (enemySlot.currentArmor < info.ParamValue) return true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                else if (Card != null)
                {
                    if (info.Owner == Enemy_Player_ENUM.NULL || info.Owner == Enemy_Player_ENUM.Player)
                    {
                        if (info.Param != BasicParam.NULL)
                        {
                            switch (info.Param)
                            {
                                case BasicParam.Life:
                                    if (Card.Life < info.ParamValue) return true;
                                        break;
                                case BasicParam.Power:
                                    if (Card.Power < info.ParamValue) return true;
                                    break;
                                case BasicParam.Armor:
                                    if (Card.Armor < info.ParamValue) return true;
                                    break;
                                case BasicParam.Durability:
                                    if (Card.Durability < info.ParamValue) return true;
                                    break;
                                case BasicParam.Cost:
                                    if (Card.cost < info.ParamValue) return true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                break;

            case TargetLimitations.Param_Equal_Value:
                if (permanent != null)
                {
                    if (info.Owner == Enemy_Player_ENUM.NULL || info.Owner == Enemy_Player_ENUM.Player)
                    {
                        if (info.Param != BasicParam.NULL)
                        {
                            switch (info.Param)
                            {
                                case BasicParam.Life:
                                    if (permanent.currentLife == info.ParamValue) return true;
                                        break;
                                case BasicParam.Power:
                                    if (permanent.currentPower == info.ParamValue) return true;
                                    break;
                                case BasicParam.Durability:
                                    if (permanent.Durability == info.ParamValue) return true;
                                    break;
                                case BasicParam.Armor:
                                    if (permanent.currentArmor == info.ParamValue) return true;
                                    break;
                                case BasicParam.Cost:
                                    if (permanent.CardReferenceArchive.cost == info.ParamValue) return true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                else if (enemySlot != null)
                {
                    if (info.Owner == Enemy_Player_ENUM.NULL || info.Owner == Enemy_Player_ENUM.Player)
                    {
                        if (info.Param != BasicParam.NULL)
                        {
                            switch (info.Param)
                            {
                                case BasicParam.Life:
                                    if (enemySlot.currentLife == info.ParamValue) return true;
                                        break;
                                case BasicParam.Power:
                                    if (enemySlot.currentPower == info.ParamValue) return true;
                                    break;
                                case BasicParam.Armor:
                                    if (enemySlot.currentArmor == info.ParamValue) return true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                else if (Card != null)
                {
                    if (info.Owner == Enemy_Player_ENUM.NULL || info.Owner == Enemy_Player_ENUM.Player)
                    {
                        if (info.Param != BasicParam.NULL)
                        {
                            switch (info.Param)
                            {
                                case BasicParam.Life:
                                    if (Card.Life == info.ParamValue) return true;
                                        break;
                                case BasicParam.Power:
                                    if (Card.Power == info.ParamValue) return true;
                                    break;
                                case BasicParam.Armor:
                                    if (Card.Armor == info.ParamValue) return true;
                                    break;
                                case BasicParam.Durability:
                                    if (Card.Durability == info.ParamValue) return true;
                                    break;
                                case BasicParam.Cost:
                                    if (Card.cost == info.ParamValue) return true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                break;

            case TargetLimitations.Only_SelectablePermanent:
                if (permanent != null)
                {
                    if (permanent.CardReferenceArchive != null)
                    {
                        foreach (Effect effect in permanent.CardReferenceArchive.Effects)
                        {
                            foreach (EventInfo item in effect.EventInfos)
                            {
                                if (item.Events == Events.OnSelect)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                if (enemySlot != null)
                {
                    foreach (Effect effect in enemySlot.PossibleIntent)
                    {
                        foreach (EventInfo item in effect.EventInfos)
                        {
                            if (item.Events == Events.OnSelect)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;

            case TargetLimitations.Only_Activated:
                if (permanent != null)
                {
                    bool HasEffectsActivated = false;
                    foreach (Effect effect in GameEventSystem.Instance.RetrieveEffectsFor(null, permanent, null))
                    {
                        foreach (EventInfo item in effect.EventInfos)
                        {
                            if (item.Events == Events.OnSelect && effect.ActivateNumber >= 1)
                            {
                                if (effect.ActivateLeft != effect.ActivateNumber)
                                {
                                    HasEffectsActivated = true;
                                }
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
                        foreach (EventInfo item in effect.EventInfos)
                        {
                            if (item.Events == Events.OnSelect && effect.ActivateNumber >= 1)
                            {
                                if (effect.ActivateLeft != effect.ActivateNumber)
                                {
                                    HasEffectsToActivate = true;
                                }
                            }
                        }
                    }

                    return HasEffectsToActivate;
                }
                return false;

            default:
                return false;
        }
        return false;
    }

    public bool PassesAllLimitations(List<TargetLimitationInfo> limitations, Card Card, PermanentView playerPerm, EnemySlotView enemyPerm, bool checkEnoughtTarget = false, bool ForShieldEffect = false)
    {

        // Check de la limitation du unshieldable dans le cas d'un effect shieldEffect
        if (playerPerm != null)
        {
            if (ShieldEffectTargeting && playerPerm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null)
            {
                return false;
            }
        }
        else if (enemyPerm != null)
        {
            if (ShieldEffectTargeting && enemyPerm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable) != null)
            {
                return false;
            }            
        }


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
                var KeywordUntarget = playerPerm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnTargetable);
                if (KeywordUntarget != null)
                {
                    return false;
                }
            }
            else if (enemyPerm != null)
            {
                var Keyword = enemyPerm.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnTargetable);
                if (Keyword != null)
                {
                    return false;
                }
            }
            else if (Card != null)
            {
                var Keyword = Card.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnTargetable);
                if (Keyword != null)
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
