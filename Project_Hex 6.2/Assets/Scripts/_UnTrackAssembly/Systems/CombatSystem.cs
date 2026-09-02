using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using FMODUnity;
using System;

public class CombatSystem : Singleton<CombatSystem>
{
    [HideInInspector] public PlayerData Player;
    [SerializeField] public Transform CoreSpawn;
    [SerializeField] public GameObject CoreParent;
    [SerializeField] private GameObject PermanentViewPrefab;
    [HideInInspector] public PermanentView PlayerCore;

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

    [HideInInspector] public List<PassiveVarGroup> Passives = new List<PassiveVarGroup>();
    public event Action PassivesChanged;


    [HideInInspector] public CounterModel GlobalCounters = new();

    public EnemyView currentEnemy;
    public int CurrentStageTier;
    public int MoneyReward;

    public List<EnemySlotView> Enemy_Permanents;
    public List<PermanentView> Player_Permanents;

    private bool startFightSubscribed = false;

    private ConditionSystem conditionSystem;

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
        conditionSystem = ConditionSystem.Instance;
        ClassicStartUp();
    }

    // Mise en place classique
    public void ClassicStartUp()
    {
        Win = false;

        DataBase dataBase = DataBase.Instance;

        Player = dataBase.CurrentPlayer;
        CardSystem.Instance.Setup(dataBase.DeckList);

        GameObject CoreObject = Instantiate(PermanentViewPrefab, CoreSpawn.transform.position, Quaternion.identity, CoreParent.transform);
        PermanentView CoreView = CoreObject.GetComponent<PermanentView>();
        CoreView.transform.localScale = Vector3.zero;
        CoreView.transform.DOScale(PermanentViewPrefab.transform.localScale, 0.15f);
        CoreView.gameObject.name = "Core player";
        Card CoreCard = new Card(Player.Core);
        CoreView.Setup(CoreCard);

        PlayerCore = CoreView;

        MaxPermPlayer = 6;
        MaxPermEnemy = 9;
        CardSystem.Instance.MaxHandCount = dataBase.MaxHandCount;
        CardSystem.Instance.NBCardDrawAtStartTurn = dataBase.NBCardDrawAtStartTurn;
        MaxPowerGrid = dataBase.MaxPowerGrid;
        CurrentPowerGrid = 0;

        UpdatePowerGridText();
        CoreView.currentLife = dataBase.CoreLife;
        CoreView.UpdateLifeText();

        if (dataBase.CurrentStage.Tier <= 1)
        {
            CurrentStageTier = 1;
        }
        else
        {
            CurrentStageTier = dataBase.CurrentStage.Tier;
        }

        GameEventSystem.Instance.ClearAllEvents();

        Player_Permanents.Add(CoreView);

        // Choix aléatoire
        GameObject selectedEnemy = dataBase.SelectedEnemy;
        GameObject SpawnedEnemy = Instantiate(selectedEnemy, EnemySpawn.position, EnemySpawn.rotation, EnemySpawn);
        EnemyView enemyView = SpawnedEnemy.GetComponent<EnemyView>();
        currentEnemy = enemyView;
        EnemySystem.Instance.enemyView = enemyView;
        EnemySlotViewCreator.Instance.WeaponZone = EnemyWeaponZone = enemyView.WeaponZone;
        EnemySlotViewCreator.Instance.ShieldZone = EnemyShieldZone = enemyView.ShieldZone;
        EnemySlotViewCreator.Instance.SupportZone = EnemySupportZone = enemyView.SupportZone;
        enemyView.Setup();

        ManaSystem.Instance.SetManaMax(DataBase.Instance.MaxMana);

        //DefinePotentialRewardsEnemy
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
    public int GetPassive(BasicParam basicParam, Enemy_Player_ENUM enemy_Player_ENUM, Card card, PermanentView permanentView, EnemySlotView enemySlotView)
    {
        int TotalValue = 0;
        foreach (PassiveVarGroup passive in Passives)
        {
            
            // on test d'abord si la nature du passif est la bonne
            if (passive.basicParam == basicParam)
            {
                if (card != null && (enemy_Player_ENUM == Enemy_Player_ENUM.Card || passive.targetModeInfo.PlayerOrEnemy == Enemy_Player_ENUM.NULL))
                {
                    // on test si le passif est relatif à un keyword
                    if (card.KeyWords.FirstOrDefault(k => k.keyWordType == passive.targetModeInfo.keyWordType) != null || passive.targetModeInfo.keyWordType == KeyWordType.NULL)
                    {
                        // On test si les conditions du passif sont remplie
                        if (!conditionSystem.TestCondition(passive.conditions, null, null, null, null, null, null, true, card, null, null, card.RefCardView.gameObject)) continue;
                        {
                            TotalValue += passive.value;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (permanentView != null && enemy_Player_ENUM == Enemy_Player_ENUM.Player)
                {
                    // on test si le passif est relatif à un keyword
                    if (permanentView.KeyWords.FirstOrDefault(k => k.keyWordType == passive.targetModeInfo.keyWordType) != null || passive.targetModeInfo.keyWordType == KeyWordType.NULL)
                    {
                        // On test si les conditions du passif sont remplie
                        if (!conditionSystem.TestCondition(passive.conditions, null, null, null, null, null, null, true, null, permanentView, null, permanentView.gameObject)) continue;
                        {
                            TotalValue += passive.value;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (enemySlotView != null && enemy_Player_ENUM == Enemy_Player_ENUM.Enemy)
                {
                    // on test si le passif est relatif à un keyword
                    if (enemySlotView.KeyWords.FirstOrDefault(k => k.keyWordType == passive.targetModeInfo.keyWordType) != null || passive.targetModeInfo.keyWordType == KeyWordType.NULL)
                    {
                        // On test si les conditions du passif sont remplie
                        if (!conditionSystem.TestCondition(passive.conditions, null, null, null, null, null, null, true, null, null, enemySlotView, enemySlotView.gameObject)) continue;
                        {
                            TotalValue += passive.value;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                continue;
            }
        }

        return TotalValue;
    }

    public void AddPassive(GameObject owner, int value, BasicParam basicParam, TargetModeInfo targetModeInfo, List<DynamicConditionInfo> dynamicConditions)
    {
        Debug.Log("add passive of " + owner.name);
        PassiveVarGroup passiveVarGroup = new PassiveVarGroup(owner, value, basicParam, targetModeInfo, dynamicConditions);
        Passives.Add(passiveVarGroup);
        PassivesChanged?.Invoke();
    }

    public void RemovePassive(GameObject Owner)
    {
        Passives.RemoveAll(x => x.owner == Owner);
        PassivesChanged?.Invoke();
    }

    //Utils
    public void UpdatePowerGridText()
    {
        PowerGridUI.UpdatePowerGridText(CurrentPowerGrid, MaxPowerGrid);
    }

    // PERFORMER
    public IEnumerator DiePermanentPerformer(DiePermanentGA diePermanentGA)
    {
        Debug.Log("PermanentDie : " + diePermanentGA.PermanentView.name + " core ? : " + diePermanentGA.IsCore);
        if (!diePermanentGA.IsCore)
        {
            var InvocKeyword = diePermanentGA.PermanentView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Invoc);
            if (diePermanentGA.Durability == 0 || InvocKeyword != null)
            {
                if (diePermanentGA.PermanentView != null)
                {
                    CurrentPowerGrid -= diePermanentGA.PermanentView.CardReferenceArchive.GridCost;
                    UpdatePowerGridText();

                    RemovePassive(diePermanentGA.PermanentView.gameObject);

                    LoseShieldGA loseShieldGA = new(diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(loseShieldGA);

                    EventInfo eventInfo = new EventInfo(Events.WhenPermaDie, Enemy_Player_ENUM.Player, KeyWordType.NULL);
                    TriggerEventGA triggerEventGA = new(eventInfo, null, null, diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);

                    eventInfo = new EventInfo(Events.WhenPermaExaust, Enemy_Player_ENUM.Player, KeyWordType.NULL);
                    triggerEventGA = new(eventInfo, null, null, diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);

                    eventInfo = new EventInfo(Events.OnSelfDeath, Enemy_Player_ENUM.Player, KeyWordType.NULL);
                    triggerEventGA = new(eventInfo, null, null, diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);

                    eventInfo = new EventInfo(Events.OnSelfDestroy, Enemy_Player_ENUM.Player, KeyWordType.NULL);
                    triggerEventGA = new(eventInfo, null, null, diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);

                    Player_Permanents.Remove(diePermanentGA.PermanentView);

                    DestroyPermanentGA destroyPermanentGA = new(diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(destroyPermanentGA);

                    foreach (KeyWord keyword in diePermanentGA.PermanentView.KeyWords)
                    {
                        eventInfo = new EventInfo(Events.TypeCountChanged, Enemy_Player_ENUM.Player, keyword.keyWordType);
                        triggerEventGA = new(eventInfo, null, null, diePermanentGA.PermanentView, null);
                        ActionSystem.Instance.AddReaction(triggerEventGA);
                    }
                    eventInfo = new EventInfo(Events.TypeCountChanged, Enemy_Player_ENUM.Player, KeyWordType.NULL);
                    triggerEventGA = new(eventInfo, null, null, diePermanentGA.PermanentView, null);
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

                    RemovePassive(diePermanentGA.PermanentView.gameObject);

                    LoseShieldGA loseShieldGA = new(diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(loseShieldGA);

                    diePermanentGA.CardReferenceArchive.Durability -= 1;
                    CardView newCardView = CardViewCreator.Instance.CreateCardView(diePermanentGA.CardReferenceArchive, diePermanentGA.PermanentView.transform.position, diePermanentGA.PermanentView.transform.rotation);

                    EventInfo eventInfo = new EventInfo(Events.WhenPermaDie, Enemy_Player_ENUM.Player, KeyWordType.NULL);
                    TriggerEventGA triggerEventGA = new(eventInfo, null, null, diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);

                    eventInfo = new EventInfo(Events.OnSelfDeath, Enemy_Player_ENUM.Player, KeyWordType.NULL);
                    triggerEventGA = new(eventInfo, null, null, diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);

                    DestroyPermanentGA destroyPermanentGA = new(diePermanentGA.PermanentView, null);
                    ActionSystem.Instance.AddReaction(destroyPermanentGA);

                    foreach (KeyWord keyword in diePermanentGA.PermanentView.KeyWords)
                    {
                        eventInfo = new EventInfo(Events.TypeCountChanged, Enemy_Player_ENUM.Player, keyword.keyWordType);
                        triggerEventGA = new(eventInfo, null, null, diePermanentGA.PermanentView, null);
                        ActionSystem.Instance.AddReaction(triggerEventGA);
                    }
                    eventInfo = new EventInfo(Events.TypeCountChanged, Enemy_Player_ENUM.Player, KeyWordType.NULL);
                    triggerEventGA = new(eventInfo, null, null, diePermanentGA.PermanentView, null);
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
        RemovePassive(dieEnemySlotGA.EnemySlotView.gameObject);

        LoseShieldGA loseShieldGA = new(null, dieEnemySlotGA.EnemySlotView);
        ActionSystem.Instance.AddReaction(loseShieldGA);

        EventInfo eventInfo = new EventInfo(Events.WhenPermaDie, Enemy_Player_ENUM.Enemy, KeyWordType.NULL);
        TriggerEventGA triggerEventGA = new(eventInfo, null, null, null, dieEnemySlotGA.EnemySlotView);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        eventInfo = new EventInfo(Events.OnSelfDeath, Enemy_Player_ENUM.Enemy, KeyWordType.NULL);
        triggerEventGA = new(eventInfo, null, null, null, dieEnemySlotGA.EnemySlotView);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        Enemy_Permanents.Remove(dieEnemySlotGA.EnemySlotView);

        DestroyPermanentGA destroyPermanentGA = new(null, dieEnemySlotGA.EnemySlotView);

        foreach (KeyWord keyword in dieEnemySlotGA.EnemySlotView.KeyWords)
        {
            eventInfo = new EventInfo(Events.TypeCountChanged, Enemy_Player_ENUM.Enemy, keyword.keyWordType);
            triggerEventGA = new(eventInfo, null, null, null, dieEnemySlotGA.EnemySlotView);
            ActionSystem.Instance.AddReaction(triggerEventGA);            
        }
        eventInfo = new EventInfo(Events.TypeCountChanged, Enemy_Player_ENUM.Enemy, KeyWordType.NULL);
        triggerEventGA = new(eventInfo, null, null, null, dieEnemySlotGA.EnemySlotView);
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
                //if (effect.Events.Contains(Events.OnSelect))
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
        CounterTypeInfo counterTypeInfo = new CounterTypeInfo(false, true, Enemy_Player_ENUM.NULL, KeyWordType.NULL,CounterType.SpellCast);
        CounterSystem.Instance.Reset(counterTypeInfo);
        counterTypeInfo = new CounterTypeInfo(false, true, Enemy_Player_ENUM.NULL, KeyWordType.NULL,CounterType.PermanentCast);
        CounterSystem.Instance.Reset(counterTypeInfo);
        counterTypeInfo = new CounterTypeInfo(false, true, Enemy_Player_ENUM.NULL, KeyWordType.NULL,CounterType.CardsDraw);
        CounterSystem.Instance.Reset(counterTypeInfo);
        counterTypeInfo = new CounterTypeInfo(false, true, Enemy_Player_ENUM.NULL, KeyWordType.NULL,CounterType.CardsDiscard);
        CounterSystem.Instance.Reset(counterTypeInfo);

        ReffilManaGA reffilManaGA = new();
        ActionSystem.Instance.AddReaction(reffilManaGA);

        DynamicAmountInfo dynamicAmountInfo = new DynamicAmountInfo(DynamicAmount.NULL,Enemy_Player_ENUM.NULL,KeyWordType.NULL,new CounterTypeInfo(),BasicParam.NULL,false,CardLocation.NULL);
        DrawCardsGA drawCardsGA = new(CardSystem.Instance.NBCardDrawAtStartTurn,1,dynamicAmountInfo,false);
        ActionSystem.Instance.AddReaction(drawCardsGA);

        DecountPlayerDecayGA decountPlayerDecayGA = new();
        ActionSystem.Instance.AddReaction(decountPlayerDecayGA);

        EventInfo eventInfo = new EventInfo(Events.StartTurn, Enemy_Player_ENUM.Player, KeyWordType.NULL);
        TriggerEventGA triggerEventGA = new(eventInfo, null, null, null, null);
        ActionSystem.Instance.AddReaction(triggerEventGA);
    }

    private void EndPlayerTurnPostReaction(EndPlayerTurnGA endPlayerTurnGA)
    {
        EventInfo eventInfo = new EventInfo(Events.EndTurn, Enemy_Player_ENUM.Player, KeyWordType.NULL);
        TriggerEventGA triggerEventGA = new(eventInfo, null, null, null, null);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        
        GlobalResetActivationGA globalResetActivationGA = new();
        ActionSystem.Instance.AddReaction(globalResetActivationGA);
        EnemyTurnGA enemyTurnGA = new();
        ActionSystem.Instance.AddReaction(enemyTurnGA);
    }

    private void EndEnemyTurnPostReaction(EndEnemyTurnGA endEnemyTurnGA)
    {
        EventInfo eventInfo = new EventInfo(Events.EndTurn, Enemy_Player_ENUM.Enemy, KeyWordType.NULL);
        TriggerEventGA triggerEventGA = new(eventInfo, null, null, null, null);
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
 