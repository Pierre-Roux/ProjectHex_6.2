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
    [HideInInspector] public bool EndTurnBtnActivable;
    [HideInInspector] public bool Win;

    [SerializeField] public int CurrentTurn;

    [SerializeField] private Transform EnemySpawn;

    [SerializeField] public GameObject EndGameDefeatPanel;
    [SerializeField] public GameObject EndGameVictoryPanel;

    [SerializeField] public PowerGridUI PowerGridUI;

    [SerializeField] public ZoneView PlayerWeaponZone;
    [SerializeField] public ZoneView PlayerShieldZone;
    [SerializeField] public ZoneView PlayerSupportZone;
    [HideInInspector] public EnemyZoneView EnemyWeaponZone;
    [HideInInspector] public EnemyZoneView EnemyShieldZone;
    [HideInInspector] public EnemyZoneView EnemySupportZone;

    [HideInInspector] public int MaxPermPlayer;
    [HideInInspector] public int MaxPermEnemy;
    [HideInInspector] public int MaxPowerGrid;
    [HideInInspector] public int CurrentPowerGrid;

    [HideInInspector] public Dictionary<KeyWordType, PowerVarGroup> PowerByTypeGeneral = new();
    [HideInInspector] public Dictionary<KeyWordType, HPVarGroup> HPByTypeGeneral = new();
    [HideInInspector] public Dictionary<KeyWordType, StamVarGroup> StamByTypeGeneral = new();
    [HideInInspector] public Dictionary<KeyWordType, CostVarGroup> CostByTypeGeneral = new();
    [HideInInspector] public Dictionary<CopyTokenType, List<CopyVarGroup>> playerCopyTokens = new Dictionary<CopyTokenType, List<CopyVarGroup>>();
    [HideInInspector] public Dictionary<CopyTokenType, List<CopyVarGroup>> enemyCopyTokens = new Dictionary<CopyTokenType, List<CopyVarGroup>>();

    [HideInInspector] public CounterModel GlobalCounters = new();

    public EnemyView currentEnemy;
    public int CurrentStageTier;
    public int MoneyReward;

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
        // Init Dictoniary
        foreach (KeyWordType keyWordType in System.Enum.GetValues(typeof(KeyWordType)))
        {
            PowerByTypeGeneral[keyWordType] = new PowerVarGroup();
        }
        foreach (KeyWordType keyWordType in System.Enum.GetValues(typeof(KeyWordType)))
        {
            HPByTypeGeneral[keyWordType] = new HPVarGroup();
        }
        foreach (KeyWordType keyWordType in System.Enum.GetValues(typeof(KeyWordType)))
        {
            StamByTypeGeneral[keyWordType] = new StamVarGroup();
        }
        foreach (KeyWordType keyWordType in System.Enum.GetValues(typeof(KeyWordType)))
        {
            CostByTypeGeneral[keyWordType] = new CostVarGroup();
        }
        foreach (CopyTokenType type in System.Enum.GetValues(typeof(CopyTokenType)))
        {
            playerCopyTokens[type] = new List<CopyVarGroup>();

            enemyCopyTokens[type] = new List<CopyVarGroup>();
        }
        ClassicStartUp();
    }

    // Mise en place classique
    public void ClassicStartUp()
    {
        Win = false;

        DataBase dataBase = DataBase.Instance;

        Player = dataBase.CurrentPlayer;
        if (dataBase.DeckList.Count == 0)
        {
            dataBase.DeckList = new List<CardData>(Player.deckData);
            dataBase.INITIALDeckList = new List<CardData>(Player.deckData);
        }
        CardSystem.Instance.Setup(dataBase.DeckList);
        PlayerCore.SetupCore(Player);

        EnemiesDataBase = dataBase.CurrentStage.Enemies;

        int targetTier = 0;
        MaxPermPlayer = 9;
        MaxPermEnemy = 9;
        CardSystem.Instance.MaxHandCount = dataBase.MaxHandCount;
        CardSystem.Instance.NBCardDrawAtStartTurn = dataBase.NBCardDrawAtStartTurn;
        MaxPowerGrid = dataBase.MaxPowerGrid;
        CurrentPowerGrid = 0;

        UpdatePowerGridText();

        if (dataBase.CurrentStage.Tier <= 1)
        {
            CurrentStageTier = 1;
        }
        else
        {
            CurrentStageTier = dataBase.CurrentStage.Tier;
        }

        if (CurrentStageTier == 1)
        {
            PlayerCore.currentLife = dataBase.BaseCoreLife = Player.CoreHealth;
            PlayerCore.UpdateLifeText();
        }
        else
        {
            PlayerCore.currentLife = dataBase.CoreLife;
            PlayerCore.UpdateLifeText();
        }


        // Détermine le Tier selon le Stage
        if (CurrentStageTier <= 2)
            targetTier = 0;
        else if (CurrentStageTier <= 4)
            targetTier = 1;
        else if (CurrentStageTier <= 6)
            targetTier = 2;
        else if (CurrentStageTier <= 8)
            targetTier = 3;
        else if (CurrentStageTier <= 10)
            targetTier = 4;
        else if (CurrentStageTier <= 12)
            targetTier = 5;
        else
            targetTier = 0;

        //
        //targetTier++;

        // Filtrage
        List<GameObject> validEnemies = EnemiesDataBase
        .Where(e => e.GetComponent<EnemyView>().Tier == targetTier)
        .ToList();

        if (DataBase.Instance.IsElite)
        {
            validEnemies = validEnemies.Where(e => e.GetComponent<EnemyView>().isElite == true).ToList();
        }

        // Si aucun ennemi trouvé pour ce Tier
        if (validEnemies.Count == 0)
        {
            Debug.LogWarning($"⚠ Aucun ennemi trouvé pour le Tier {targetTier} & IsElite = {DataBase.Instance.IsElite}. Selection Aléatoire d'Elite");
            validEnemies = EnemiesDataBase;
            if (DataBase.Instance.IsElite)
            {
                validEnemies = validEnemies.Where(e => e.GetComponent<EnemyView>().isElite == true).ToList();
            }
            if (validEnemies.Count == 0)
            {
                Debug.LogWarning($"⚠ Aucun ennemi d'Elite trouvé, sélection aléatoire globale.");
                validEnemies = EnemiesDataBase;
            }
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

        ManaSystem.Instance.SetManaMax(DataBase.Instance.MaxMana);

        //DefinePotentialRewards
        foreach (CardData cardData in dataBase.ColorLessCardPool.CardDataList)
        {
            RewardSystem.Instance.PotentialRewards.Add(cardData.Clone());
        }
        foreach (CardData cardData in dataBase.ChoosedCardPool.CardDataList)
        {
            RewardSystem.Instance.PotentialRewards.Add(cardData.Clone());
        }
        if (currentEnemy.EnemyRewardCardPool != null)
        {
            if (currentEnemy.EnemyRewardCardPool.CardDataList.Count != 0)
            {
                foreach (CardData cardData in currentEnemy.EnemyRewardCardPool.CardDataList)
                {
                    RewardSystem.Instance.PotentialRewards.Add(cardData.Clone());
                }
            }
        }

        //Define Money Reward
        MoneyReward = CurrentStageTier * 10;
        if (currentEnemy.isElite)
        {
            MoneyReward = MoneyReward * 2;
        }
        RewardSystem.Instance.UpdateEndFightMoneyText(MoneyReward);

        //Set Cost of Card in inspector by priority
        /*foreach (CardData item in RewardSystem.Instance.PotentialRewards)
        {
            if (item.Rarity == 0)
            {
                item.Money_Cost = 20;
            }
            if (item.Rarity == 1)
            {
                item.Money_Cost = 50;
            }
            if (item.Rarity == 2)
            {
                item.Money_Cost = 100;
            }
        }*/

        StartFightGA startFight = new(enemyView);
        ActionSystem.Instance.Perform(startFight);

        Interactable = true;
    }

    // GESTION DES DICTIONNAIRES DE PASSIF
    public int GetPower(KeyWordType keyWordType, Enemy_Player_ENUM side)
    {
        var power = PowerByTypeGeneral[keyWordType];
        return side switch
        {
            Enemy_Player_ENUM.Player => power.Player,
            Enemy_Player_ENUM.Enemy => power.Enemy,
            _ => power.Global
        };
    }

    public void AddPower(KeyWordType keyWordType, Enemy_Player_ENUM side, int amount)
    {
        var power = PowerByTypeGeneral[keyWordType];
        switch (side)
        {
            case Enemy_Player_ENUM.Player:
                power.Player += amount;
                break;
            case Enemy_Player_ENUM.Enemy:
                power.Enemy += amount;
                break;
            default:
                power.Global += amount;
                break;
        }
    }

    public int GetHP(KeyWordType keyWordType, Enemy_Player_ENUM side)
    {
        var HP = HPByTypeGeneral[keyWordType];
        return side switch
        {
            Enemy_Player_ENUM.Player => HP.Player,
            Enemy_Player_ENUM.Enemy => HP.Enemy,
            _ => HP.Global
        };
    }

    public void AddHP(KeyWordType keyWordType, Enemy_Player_ENUM side, int amount)
    {
        var HP = HPByTypeGeneral[keyWordType];
        switch (side)
        {
            case Enemy_Player_ENUM.Player:
                HP.Player += amount;
                break;
            case Enemy_Player_ENUM.Enemy:
                HP.Enemy += amount;
                break;
            default:
                HP.Global += amount;
                break;
        }
    }

    public int GetStam(KeyWordType keyWordType, Enemy_Player_ENUM side)
    {
        var Stam = StamByTypeGeneral[keyWordType];
        return side switch
        {
            _ => Stam.Player
        };
    }

    public void AddStam(KeyWordType keyWordType, Enemy_Player_ENUM side, int amount)
    {
        var Stam = StamByTypeGeneral[keyWordType];
        switch (side)
        {
            case Enemy_Player_ENUM.Player:
                Stam.Player += amount;
                break;
        }
    }

    public int GetCost(KeyWordType keyWordType, Enemy_Player_ENUM side)
    {
        var Cost = CostByTypeGeneral[keyWordType];
        return side switch
        {
            _ => Cost.Card
        };
    }

    public void AddCost(KeyWordType keyWordType, Enemy_Player_ENUM side, int amount)
    {
        var Cost = CostByTypeGeneral[keyWordType];
        switch (side)
        {
            case Enemy_Player_ENUM.NULL:
                Cost.Card += amount;
                break;
        }
    }

    public List<CopyVarGroup> GetCopyValues(CopyTokenType type, Enemy_Player_ENUM side)
    {
        var dict = side == Enemy_Player_ENUM.Player ? playerCopyTokens : enemyCopyTokens;

        if (!dict.ContainsKey(type))
            return new List<CopyVarGroup>();

        return dict[type];
    }

    public void AddCopyValue(CopyTokenType type, Enemy_Player_ENUM side, int amount, List<DynamicConditionInfo> conditions)
    {
        var dict = side == Enemy_Player_ENUM.Player ? playerCopyTokens : enemyCopyTokens;

        if (!dict.ContainsKey(type))
            dict[type] = new List<CopyVarGroup>();

        dict[type].Add(new CopyVarGroup
        {
            value = amount,
            Conditions = new List<DynamicConditionInfo>(conditions)
        });
    }

    public void RemoveCopyGroup(CopyTokenType type, Enemy_Player_ENUM side, CopyVarGroup group)
    {
        var dict = side == Enemy_Player_ENUM.Player ? playerCopyTokens : enemyCopyTokens;

        if (dict.ContainsKey(type))
        {
            dict[type].Remove(group);
        }
    }

    //Utils

    public void UpdatePowerGridText()
    {
        PowerGridUI.UpdatePowerGridText(CurrentPowerGrid, MaxPowerGrid);
    }

    // PERFORMER
    public IEnumerator DiePermanentPerformer(DiePermanentGA diePermanentGA)
    {
        if (!diePermanentGA.IsCore)
        {
            var InvocKeyword = diePermanentGA.PermanentView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Invoc);
            if (diePermanentGA.Durability == 0 || InvocKeyword != null)
            {
                if (diePermanentGA.PermanentView != null)
                {
                    CurrentPowerGrid -= diePermanentGA.PermanentView.CardReferenceArchive.GridCost;
                    UpdatePowerGridText();

                    LoseShieldGA loseShieldGA = new(diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(loseShieldGA);

                    TriggerEventGA triggerEventGA = new(Events.WhenPermaDie, null, diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);

                    triggerEventGA = new(Events.WhenPermaExaust, null, diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);

                    triggerEventGA = new(Events.OnDeath, null, diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);

                    triggerEventGA = new(Events.OnDestroy, null, diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);

                    Player_Permanents.Remove(diePermanentGA.PermanentView);

                    DestroyPermanentGA destroyPermanentGA = new(diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(destroyPermanentGA);

                    triggerEventGA = new(Events.HollowCountChanged,null,null,null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);
                    triggerEventGA = new(Events.DecayCountChanged,null,null,null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);
                    triggerEventGA = new(Events.ArtilleryCountChanged,null,null,null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);
                    triggerEventGA = new(Events.InvocCountChanged,null,null,null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);

                    if (!AudioManager.Instance.IsValid(destroyPermanentGA.PermanentView.CardReferenceArchive.HollowDieSound))
                    {
                        RuntimeManager.PlayOneShot(AudioManager.Instance.HollowDieSound);
                    }
                    else
                    {
                        RuntimeManager.PlayOneShot(destroyPermanentGA.PermanentView.CardReferenceArchive.HollowDieSound);
                    }

                    if (InvocKeyword == null)
                    {
                        CardSystem.Instance.ExhaustPile.Add(diePermanentGA.PermanentView.CardReferenceArchive);
                    }
                }
            }
            else
            {
                if (diePermanentGA.PermanentView != null)
                {
                    CurrentPowerGrid -= diePermanentGA.PermanentView.CardReferenceArchive.GridCost;
                    UpdatePowerGridText();
                    LoseShieldGA loseShieldGA = new(diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(loseShieldGA);

                    diePermanentGA.CardReferenceArchive.Durability -= 1;
                    CardView newCardView = CardViewCreator.Instance.CreateCardView(diePermanentGA.CardReferenceArchive, diePermanentGA.PermanentView.transform.position, diePermanentGA.PermanentView.transform.rotation);

                    TriggerEventGA triggerEventGA = new(Events.WhenPermaDie, null, diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);

                    TriggerEventGA triggerPermanentEventGA = new(Events.OnDeath, null, diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(triggerPermanentEventGA);

                    DestroyPermanentGA destroyPermanentGA = new(diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(destroyPermanentGA);

                    triggerEventGA = new(Events.HollowCountChanged,null,null,null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);
                    triggerEventGA = new(Events.DecayCountChanged,null,null,null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);
                    triggerEventGA = new(Events.ArtilleryCountChanged,null,null,null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);
                    triggerEventGA = new(Events.InvocCountChanged,null,null,null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);

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

        Enemy_Permanents.Remove(dieEnemySlotGA.EnemySlotView);

        DestroyPermanentGA destroyPermanentGA = new(null, dieEnemySlotGA.EnemySlotView);

        triggerEventGA = new(Events.DecayCountChanged,null,null,null);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        triggerEventGA = new(Events.InvocCountChanged,null,null,null);
        ActionSystem.Instance.AddReaction(triggerEventGA);

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
            foreach (Effect effect in GameEventSystem.Instance.RetrieveEffectsFor(null,item,null))
            {
                //if (effect.Events.Contains(Events.OnSelect))
                {
                    effect.ActivateLeft = effect.ActivateNumber;
                }
            }
        }
        foreach (EnemySlotView item in Enemy_Permanents)
        {
            foreach (Effect effect in GameEventSystem.Instance.RetrieveEffectsFor(null,null,item))
            {
                if (effect.Events.Contains(Events.OnSelect))
                {
                    effect.ActivateLeft = effect.ActivateNumber;
                }
            }
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
        List<Effect> effectList = startFightGA.enemyView.SetupEffects.OrderBy(e => e.Priority).ToList();
        foreach (Effect effect in effectList)
        {
            GameEventSystem.Instance.DoAction(effect);
        }
        DeckShuffleGA deckShuffleGA = new();
        ActionSystem.Instance.AddReaction(deckShuffleGA);
        PlayerTurnGA playerTurnGA = new();
        ActionSystem.Instance.AddReaction(playerTurnGA);
    }
    private void PlayerTurnPreReaction(PlayerTurnGA playerTurnGA)
    {
        // Reset NewTurnCounters
        CounterSystem.Instance.Reset(CounterType.SpellCast_This_Turn);
        CounterSystem.Instance.Reset(CounterType.PermanentCast_This_Turn);
        CounterSystem.Instance.Reset(CounterType.CardsDraw_This_Turn);
        CounterSystem.Instance.Reset(CounterType.CardsDiscard_This_Turn);

        ReffilManaGA reffilManaGA = new();
        ActionSystem.Instance.AddReaction(reffilManaGA);
        DrawCardsGA drawCardsGA = new(CardSystem.Instance.NBCardDrawAtStartTurn,1,DynamicAmount.NULL,false);
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
 