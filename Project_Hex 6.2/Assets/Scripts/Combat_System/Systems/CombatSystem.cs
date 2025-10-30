using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using FMODUnity;

public class CombatSystem : Singleton<CombatSystem>
{
    [HideInInspector] public PlayerData Player;
    [SerializeField] public PermanentView PlayerCore;
    [HideInInspector] private List<GameObject> EnemiesDataBase;

    [HideInInspector] public bool Interactable;
    [HideInInspector] public bool Win;

    [SerializeField] public int CurrentTurn;

    [SerializeField] private Transform EnemySpawn;

    [SerializeField] public GameObject EndGameDefeatPanel;
    [SerializeField] public GameObject EndGameVictoryPanel;

    [SerializeField] public ZoneView PlayerWeaponZone;
    [SerializeField] public ZoneView PlayerShieldZone;
    [SerializeField] public ZoneView PlayerSupportZone;
    [HideInInspector] public EnemyZoneView EnemyWeaponZone;
    [HideInInspector] public EnemyZoneView EnemyShieldZone;
    [HideInInspector] public EnemyZoneView EnemySupportZone;

    [HideInInspector] public int MaxPermPlayer;
    [HideInInspector] public int MaxPermEnemy;

    [HideInInspector] public int PlayerGeneralPower;
    [HideInInspector] public int EnemyGeneralPower;
    [HideInInspector] public int GeneralPower;
    [HideInInspector] public int Invoc_GeneralPower;
    [HideInInspector] public int Invoc_PlayerGeneralPower;
    [HideInInspector] public int Invoc_EnemyGeneralPower;
    [HideInInspector] public int Hollow_GeneralPower;
    [HideInInspector] public int Hollow_PlayerGeneralPower;
    [HideInInspector] public int Hollow_EnemyGeneralPower;
    [HideInInspector] public int Decay_GeneralPower;
    [HideInInspector] public int Decay_PlayerGeneralPower;
    [HideInInspector] public int Decay_EnemyGeneralPower;
    [HideInInspector] public int Artillery_GeneralPower;
    [HideInInspector] public int Artillery_PlayerGeneralPower;
    [HideInInspector] public int Artillery_EnemyGeneralPower;

    [HideInInspector] public int PlayerGeneralHPGain;
    [HideInInspector] public int EnemyGeneralHPGain;
    [HideInInspector] public int GeneralHPGain;
    [HideInInspector] public int Invoc_GeneralHPGain;
    [HideInInspector] public int Invoc_PlayerGeneralHPGain;
    [HideInInspector] public int Invoc_EnemyGeneralHPGain;
    [HideInInspector] public int Hollow_GeneralHPGain;
    [HideInInspector] public int Hollow_PlayerGeneralHPGain;
    [HideInInspector] public int Hollow_EnemyGeneralHPGain;
    [HideInInspector] public int Decay_GeneralHPGain;
    [HideInInspector] public int Decay_PlayerGeneralHPGain;
    [HideInInspector] public int Decay_EnemyGeneralHPGain;
    [HideInInspector] public int Artillery_GeneralHPGain;
    [HideInInspector] public int Artillery_PlayerGeneralHPGain;
    [HideInInspector] public int Artillery_EnemyGeneralHPGain;

    [HideInInspector] public int SpellCast_This_Turn;
    [HideInInspector] public int PermanentCast_This_Turn;

    public EnemyView currentEnemy;

    public List<EnemySlotView> Enemy_Permanents;
    public List<PermanentView> Player_Permanents;

    private bool startFightSubscribed = false;

    public void OnEnable()
    {
        if (!startFightSubscribed)
        {
            ActionSystem.AttachPerformer<DiePermanentGA>(DiePermanentPerformer);
            ActionSystem.AttachPerformer<DieEnemySlotGA>(DieEnemySlotView);
            ActionSystem.AttachPerformer<DestroyPermanentGA>(DestroyPerformer);
            ActionSystem.AttachPerformer<GlobalResetActivationGA>(GlobalResetActivationPerformer);
            ActionSystem.AttachPerformer<EndCombatGA>(EndCombat);

            ActionSystem.SubscribeReaction<StartFightGA>(StartFightPreReaction, ReactionTiming.PRE);
            ActionSystem.SubscribeReaction<PlayerTurnGA>(PlayerTurnPreReaction, ReactionTiming.PRE);

            ActionSystem.SubscribeReaction<EndEnemyTurnGA>(EndEnemyTurnPostReaction, ReactionTiming.POST);
            ActionSystem.SubscribeReaction<EndPlayerTurnGA>(EndPlayerTurnPostReaction, ReactionTiming.POST);

            startFightSubscribed = true;
        }
    }

    public void OnDisable()
    {
        if (startFightSubscribed)
        {
            ActionSystem.DetachPerformer<DiePermanentGA>();
            ActionSystem.DetachPerformer<DieEnemySlotGA>();
            ActionSystem.DetachPerformer<DestroyPermanentGA>();
            ActionSystem.DetachPerformer<GlobalResetActivationGA>();
            ActionSystem.DetachPerformer<EndCombatGA>();

            ActionSystem.UnsubscribeReaction<StartFightGA>(StartFightPreReaction, ReactionTiming.PRE);
            ActionSystem.UnsubscribeReaction<PlayerTurnGA>(PlayerTurnPreReaction, ReactionTiming.PRE);

            ActionSystem.UnsubscribeReaction<EndEnemyTurnGA>(EndEnemyTurnPostReaction, ReactionTiming.POST);
            ActionSystem.UnsubscribeReaction<EndPlayerTurnGA>(EndPlayerTurnPostReaction, ReactionTiming.POST);

            startFightSubscribed = false;
        }
    }

    private void Start()
    {
        ClassicStartUp();
    }

    // Mise en place classique
    public void ClassicStartUp()
    {
        Win = false;

        Player = DataBase.Instance.StartingPlayer;
        if (DataBase.Instance.DeckList.Count == 0)
        {
            DataBase.Instance.DeckList = new List<CardData>(Player.deckData);
            DataBase.Instance.INITIALDeckList = new List<CardData>(Player.deckData);
        }
        CardSystem.Instance.Setup(DataBase.Instance.DeckList);
        PlayerCore.SetupCore(Player);

        EnemiesDataBase = DataBase.Instance.EnemiesDataBase;

        int stage = 0;
        int targetTier = 0;
        MaxPermPlayer = 9;
        MaxPermEnemy = 9;

        if (DataBase.Instance.CurrentStage <= 0)
        {
            stage = 0;
        }
        else
        {
            stage = DataBase.Instance.CurrentStage;
        }

        if (DataBase.Instance.CurrentStage == 0)
        {
            PlayerCore.currentLife = DataBase.Instance.BaseCoreLife = Player.CoreHealth;
            PlayerCore.UpdateLifeText();
        }
        else
        {
            PlayerCore.currentLife = DataBase.Instance.CoreLife;
            PlayerCore.UpdateLifeText();
        }


        // Détermine le Tier selon le Stage
        if (stage < 2)
            targetTier = 0;
        else if (stage == 2)
            targetTier = 1;
        else if (stage < 5)
            targetTier = 2;
        else if (stage == 5)
            targetTier = 3;
        else if (stage < 8)
            targetTier = 4;
        else if (stage == 8)
            targetTier = 5;
        else
            targetTier = 0;

        //if (DataBase.Instance.IsElite)
        //targetTier++;

        // Filtrage
        List<GameObject> validEnemies = EnemiesDataBase
        .Where(e => e.GetComponent<EnemyView>().Tier == targetTier)
        .ToList();

        // Si aucun ennemi trouvé pour ce Tier
        if (validEnemies.Count == 0)
        {
            Debug.LogWarning($"⚠ Aucun ennemi trouvé pour le Tier {targetTier}, sélection aléatoire globale.");
            validEnemies = EnemiesDataBase;
        }

        GameEventSystem.Instance.ClearAllEvents();

        Player_Permanents.Add(PlayerCore);

        // Choix aléatoire
        GameObject selectedEnemy = validEnemies[Random.Range(0, validEnemies.Count - 1)];
        GameObject SpawnedEnemy = Instantiate(selectedEnemy, EnemySpawn.position, EnemySpawn.rotation, EnemySpawn);
        EnemyView enemyView = SpawnedEnemy.GetComponent<EnemyView>();
        currentEnemy = enemyView;
        EnemySystem.Instance.enemyView = enemyView;
        EnemySlotViewCreator.Instance.WeaponZone = EnemyWeaponZone = enemyView.WeaponZone;
        EnemySlotViewCreator.Instance.ShieldZone = EnemyShieldZone = enemyView.ShieldZone;
        EnemySlotViewCreator.Instance.SupportZone = EnemySupportZone = enemyView.SupportZone;
        enemyView.Setup();
        foreach (EnemySlotView enemySlotView in Enemy_Permanents)
        {
            if (enemySlotView.PossibleIntent == null) continue;
            foreach (Effect effect in enemySlotView.PossibleIntent)
            {
                int MultiHit = effect.MultiHit;
                if (MultiHit < 1) MultiHit = 1;
                for (int i = 0; i < MultiHit; i++)
                {
                    Effect clonedEffect = effect.Clone();
                    while (clonedEffect != null)
                    {
                        if (clonedEffect.Events != Events.EnemyTurn &&
                            clonedEffect.Events != Events.Instant
                            )
                        {
                            GameEventSystem.Instance.AddEffectToEvent(clonedEffect);
                        }

                        if (clonedEffect.LinkedEffect != null)
                        {
                            clonedEffect.LinkedEffect.ParentEffect = clonedEffect;
                        }
                        clonedEffect.Actionner = enemySlotView.gameObject;
                        clonedEffect = clonedEffect.LinkedEffect;

                    }
                }
            }
        }

        ManaSystem.Instance.SetManaMax(DataBase.Instance.MaxMana);

        StartFightGA startFight = new(enemyView);
        ActionSystem.Instance.Perform(startFight);

        Interactable = true;
    }

    // PERFORMER
    public IEnumerator DiePermanentPerformer(DiePermanentGA diePermanentGA)
    {
        if (!diePermanentGA.IsCore)
        {
            if (diePermanentGA.Durability == 0 || diePermanentGA.PermanentView.permaTypes.Contains(PermaTypes.Invoc))
            {
                if (diePermanentGA.PermanentView != null)
                {
                    LoseShieldGA loseShieldGA = new(diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(loseShieldGA);

                    TriggerEventGA triggerEventGA = new(Events.WhenPermaDie,null,diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);

                    triggerEventGA = new(Events.WhenPermaExaust,null,diePermanentGA.PermanentView,null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);

                    TriggerEventGA triggerPermanentEventGA = new(Events.OnDestroy,null,diePermanentGA.PermanentView,null);
                    ActionSystem.Instance.AddReaction(triggerPermanentEventGA);

                    CombatSystem.Instance.Player_Permanents.Remove(diePermanentGA.PermanentView);

                    DestroyPermanentGA destroyPermanentGA = new(diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(destroyPermanentGA);

                    if (!AudioManager.Instance.IsValid(destroyPermanentGA.PermanentView.CardReferenceArchive.HollowDieSound))
                    {
                        RuntimeManager.PlayOneShot(AudioManager.Instance.HollowDieSound);
                    }
                    else
                    {
                        RuntimeManager.PlayOneShot(destroyPermanentGA.PermanentView.CardReferenceArchive.HollowDieSound);
                    }
                }
            }
            else
            {
                if (diePermanentGA.PermanentView != null)
                {
                    LoseShieldGA loseShieldGA = new(diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(loseShieldGA);

                    diePermanentGA.CardReferenceArchive.Durability -= 1;
                    CardView newCardView = CardViewCreator.Instance.CreateCardView(diePermanentGA.CardReferenceArchive, diePermanentGA.PermanentView.transform.position, diePermanentGA.PermanentView.transform.rotation);

                    TriggerEventGA triggerEventGA = new(Events.WhenPermaDie,null,diePermanentGA.PermanentView,null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);
                    
                    TriggerEventGA triggerPermanentEventGA = new(Events.OnDeath,null,diePermanentGA.PermanentView,null);
                    ActionSystem.Instance.AddReaction(triggerPermanentEventGA);

                    DestroyPermanentGA destroyPermanentGA = new(diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(destroyPermanentGA);

                    if (!AudioManager.Instance.IsValid(destroyPermanentGA.PermanentView.CardReferenceArchive.DieSound))
                    {
                        RuntimeManager.PlayOneShot(AudioManager.Instance.DieSound);
                    }
                    else
                    {
                        RuntimeManager.PlayOneShot(destroyPermanentGA.PermanentView.CardReferenceArchive.DieSound);
                    }

                    newCardView.transform.DOScale(0, 0.01f);
                    Tween tween = newCardView.transform.DOScale(0.4f, 0.2f);
                    yield return tween.WaitForCompletion();
                    yield return new WaitForSeconds(1);
                    yield return CardSystem.Instance.InsertCard(newCardView);
                }
            }
        }
        else
        {
            Interactable = false;
            EndGameDefeatPanel.SetActive(true);
            AudioManager.Instance.ChangeMusic(AudioManager.Instance.DefeatMusic);
        }
    }

    public IEnumerator DieEnemySlotView(DieEnemySlotGA dieEnemySlotGA)
    {
        LoseShieldGA loseShieldGA = new(null, dieEnemySlotGA.EnemySlotView);
        ActionSystem.Instance.AddReaction(loseShieldGA);

        TriggerEventGA triggerEventGA = new(Events.WhenPermaDie,null,null, dieEnemySlotGA.EnemySlotView);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        TriggerEventGA triggerEnemyEventGA = new(Events.OnDeath,null,null,dieEnemySlotGA.EnemySlotView);
        ActionSystem.Instance.AddReaction(triggerEnemyEventGA);

        CombatSystem.Instance.Enemy_Permanents.Remove(dieEnemySlotGA.EnemySlotView);

        DestroyPermanentGA destroyPermanentGA = new(null, dieEnemySlotGA.EnemySlotView);

        ActionSystem.Instance.AddReaction(destroyPermanentGA);
        if (dieEnemySlotGA.EnemySlotView.IsCore)
        {
            EndCombatGA endCombatGA = new();
            ActionSystem.Instance.AddReaction(endCombatGA);
        }
        else
        {
            if (!AudioManager.Instance.IsValid(destroyPermanentGA.enemySlotView.PermanentData.DieSound))
            {
                RuntimeManager.PlayOneShot(AudioManager.Instance.DieSound);
            }
            else
            {
                RuntimeManager.PlayOneShot(destroyPermanentGA.enemySlotView.PermanentData.DieSound);
            }
        }
        yield return null;
    }

    public IEnumerator DestroyPerformer(DestroyPermanentGA destroyPermanentGA)
    {
        yield return null;
        if (destroyPermanentGA.enemySlotView != null)
        {
            GameEventSystem.Instance.RemoveEffectsByActionner(destroyPermanentGA.enemySlotView.gameObject);
            Enemy_Permanents.Remove(destroyPermanentGA.enemySlotView);
            Destroy(destroyPermanentGA.enemySlotView.gameObject);

            yield return null;

            EnemyWeaponZone.RepositionChildrenEnemySlotView();
            EnemyShieldZone.RepositionChildrenEnemySlotView();
            EnemySupportZone.RepositionChildrenEnemySlotViewCenterOut();
        }

        if (destroyPermanentGA.PermanentView != null)
        {
            GameEventSystem.Instance.RemoveEffectsByActionner(destroyPermanentGA.PermanentView.gameObject);
            Player_Permanents.Remove(destroyPermanentGA.PermanentView);
            Destroy(destroyPermanentGA.PermanentView.gameObject);

            yield return null;

            PlayerWeaponZone.RepositionChildrenPermanentView();
            PlayerShieldZone.RepositionChildrenPermanentView();
            PlayerSupportZone.RepositionChildrenPermanentViewCenterOut();
        }
    }

    public IEnumerator GlobalResetActivationPerformer(GlobalResetActivationGA globalResetActivationGA)
    {
        foreach (PermanentView item in Player_Permanents)
        {
            item.Activated = false;
        }
        foreach (EnemySlotView item in Enemy_Permanents)
        {
            item.Activated = false;
        }
        yield return null;
    }

    public IEnumerator EndCombat(EndCombatGA endCombatGA)
    {
        // Bloque l'interactivité du joeur 
        Interactable = false;
        Win = true;
        EndGameVictoryPanel.SetActive(true);
        AudioManager.Instance.ChangeMusic(AudioManager.Instance.VictoryMusic);
        yield return null;
    }

    // REACTIONS
    private void StartFightPreReaction(StartFightGA startFightGA)
    {
        CurrentTurn = 0;
        foreach (GameAction action in startFightGA.enemyView.SetupActions)
        {
            ActionSystem.Instance.AddReaction(action);
        }
        DeckShuffleGA deckShuffleGA = new();
        ActionSystem.Instance.AddReaction(deckShuffleGA);
        PlayerTurnGA playerTurnGA = new();
        ActionSystem.Instance.AddReaction(playerTurnGA);
    }
    private void PlayerTurnPreReaction(PlayerTurnGA playerTurnGA)
    {
        // Reset DynamicVariable
        SpellCast_This_Turn = 0;
        PermanentCast_This_Turn = 0;

        ReffilManaGA reffilManaGA = new();
        ActionSystem.Instance.AddReaction(reffilManaGA);
        DrawCardsGA drawCardsGA = new(5,DynamicAmount.NULL);
        ActionSystem.Instance.AddReaction(drawCardsGA);
        DecountPlayerDecayGA decountPlayerDecayGA = new();
        ActionSystem.Instance.AddReaction(decountPlayerDecayGA);
        TriggerEventGA triggerEventGA = new(Events.StartTurn);
        ActionSystem.Instance.AddReaction(triggerEventGA);
    }

    private void EndPlayerTurnPostReaction(EndPlayerTurnGA endPlayerTurnGA)
    {
        TriggerEventGA triggerEventGA = new(Events.EndTurn);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        GlobalResetActivationGA globalResetActivationGA = new();
        ActionSystem.Instance.AddReaction(globalResetActivationGA);
        EnemyTurnGA enemyTurnGA = new();
        ActionSystem.Instance.AddReaction(enemyTurnGA);
    }

    private void EndEnemyTurnPostReaction(EndEnemyTurnGA endEnemyTurnGA)
    {
        TriggerEventGA triggerEventGA = new(Events.EndEnemyTurn);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        DecountEnemyDecayGA decountEnemyDecayGA = new();
        ActionSystem.Instance.AddReaction(decountEnemyDecayGA);
        SpawnConstructGA spawnConstructGA = new();
        ActionSystem.Instance.AddReaction(spawnConstructGA);

        CurrentTurn++;

        PlayerTurnGA playerTurnGA = new();
        ActionSystem.Instance.AddReaction(playerTurnGA);
    }
}
 