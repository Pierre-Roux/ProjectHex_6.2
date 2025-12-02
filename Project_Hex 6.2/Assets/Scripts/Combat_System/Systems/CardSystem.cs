using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CardSystem : Singleton<CardSystem>
{
    [SerializeField] public HandView handView;
    [SerializeField] private Transform drawPilePoint;
    [SerializeField] private Transform discardPilePoint;

    [SerializeField] private DeckView DrawDeck;
    [SerializeField] private DeckView DiscardDeck;

    [SerializeField] public GameObject ScryPanel;
    [SerializeField] public GameObject ScryPanelContent;

    [SerializeField] public GameObject ChoicePanel;
    [SerializeField] public GameObject ChoicePanelContent;

    [SerializeField] public GameObject PayXPanel;
    [SerializeField] public TMP_Text PayXCounter;
    [SerializeField] public Transform CardTamponPoint;

    [SerializeField] public ScrollRect ScryScrollRect;

    [HideInInspector] public List<CardView> ScryCardViews;
    [HideInInspector] public Effect EffectChoosed;
    [HideInInspector] public CardView PayXCardView;
    [HideInInspector] public bool IsPayXValidate;
    [HideInInspector] public int PayXValue;
    [HideInInspector] public int MaxHandCount;
    [HideInInspector] public int NBCardDrawAtStartTurn;
    
    public List<Card> drawPile = new();
    public List<Card> discardPile = new();
    public List<Card> hand = new();
    public List<Card> ExhaustPile = new();

    void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        ActionSystem.AttachPerformer<DrawOnceGA>(DrawOncePerformer);
        ActionSystem.AttachPerformer<DiscardOnceGA>(DiscardOncePerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
        ActionSystem.AttachPerformer<DeckShuffleGA>(DeckShuffleGA);
        ActionSystem.SubscribeReaction<EndPlayerTurnGA>(EndPlayerTurnPreReaction, ReactionTiming.PRE);
        

    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardsGA>();
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.DetachPerformer<DrawOnceGA>();
        ActionSystem.DetachPerformer<DiscardOnceGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
        ActionSystem.DetachPerformer<DeckShuffleGA>();
        ActionSystem.UnsubscribeReaction<EndPlayerTurnGA>(EndPlayerTurnPreReaction, ReactionTiming.PRE);

    }

    void Start()
    {
        DOTween.Init();
        DOTween.SetTweensCapacity(200, 10);
        var dummy = CardViewCreator.Instance.CreateCardView(new Card(new CardData ()), new Vector3(-1000,-1000,0), Quaternion.identity);
        Destroy(dummy.gameObject);
    }

    // DECK Setup

    public void Setup(List<CardData> deckdata)
    {
        foreach (var cardData in deckdata)
        {
            Card card = new(cardData);
            drawPile.Add(card);
        }
        UpdatePiles();
    }

    // PERFORMERS

    private IEnumerator DeckShuffleGA(DeckShuffleGA deckShuffleGA)
    {
        drawPile.Shuffle();
        yield return null;
    }
    private IEnumerator DrawOncePerformer(DrawOnceGA drawOnceGA)
    {
        if (drawOnceGA.CardToDrawCount == 0)
            yield break;

        if (drawPile.Count == 0)
        {
            RefillDeck();
            drawPile.Shuffle();
            if (drawPile.Count == 0)
                yield break;
        }

        yield return DrawCard(drawOnceGA.CountAsDiscard);

        drawOnceGA.CardToDrawCount -= 1;

        if (drawOnceGA.CardToDrawCount > 0)
        {
            ActionSystem.Instance.AddReaction(new DrawOnceGA(drawOnceGA.CardToDrawCount, drawOnceGA.CountAsDiscard));
        }
    }
    private IEnumerator DrawCardsPerformer(DrawCardsGA drawCardsGA)
    {
        if (drawCardsGA.DynamicAmount != DynamicAmount.NULL)
        {
            if (drawCardsGA.Actionner == null)
            {
                if (drawCardsGA.CardActionner != null)
                {
                    drawCardsGA.Amount = TargetSystem.Instance.GetDynamicAmount(drawCardsGA.DynamicAmount, null, null, drawCardsGA.CardActionner);
                }
                else
                {
                    drawCardsGA.Amount = TargetSystem.Instance.GetDynamicAmount(drawCardsGA.DynamicAmount, null, null);
                }
            }
            else if (drawCardsGA.Actionner.GetComponent<PermanentView>() != null)
            {
                drawCardsGA.Amount = TargetSystem.Instance.GetDynamicAmount(drawCardsGA.DynamicAmount, drawCardsGA.Actionner.GetComponent<PermanentView>(), null);
            }
            else
            {
                drawCardsGA.Amount = TargetSystem.Instance.GetDynamicAmount(drawCardsGA.DynamicAmount, null, drawCardsGA.Actionner.GetComponent<EnemySlotView>());
            }
        }

        ActionSystem.Instance.AddReaction(new DrawOnceGA(drawCardsGA.Amount, drawCardsGA.countAsDraw_INGAME));
        yield return null;
    }

    private IEnumerator DrawCard(bool countAsDraw_INGAME)
    {
        float FinalTime = 0;
        FinalTime = Time.time;
        if (hand.Count < MaxHandCount)
        {
            TriggerEventGA triggerEventGA = null;

            Card card = drawPile.Draw();
            UpdatePiles();

            if (hand.Count == 0)
            {
                triggerEventGA = new(Events.HandNoLongerEmpty, null, null, null);
                ActionSystem.Instance.AddReaction(triggerEventGA);
            }

            hand.Add(card);

            if (hand.Count == MaxHandCount)
            {
                triggerEventGA = new(Events.HandFull, null, null, null);
                ActionSystem.Instance.AddReaction(triggerEventGA);
            }

            CardView cardView = CardViewCreator.Instance.CreateCardView(card, drawPilePoint.position, drawPilePoint.rotation);

            if (countAsDraw_INGAME)
            {
                CounterSystem.Instance.Add(CounterType.CardsDraw_This_Turn);
                CounterSystem.Instance.Add(CounterType.CardsDraw_Since_Load);
                triggerEventGA = new(Events.WhenGlobalCounter, null, null, null, CounterType.CardsDraw_This_Turn);
                ActionSystem.Instance.AddReaction(triggerEventGA);
                triggerEventGA = new(Events.WhenInternCounter, null, null, null, CounterType.CardsDraw_This_Turn);
                ActionSystem.Instance.AddReaction(triggerEventGA);
                triggerEventGA = new(Events.WhenGlobalCounter, null, null, null, CounterType.CardsDraw_Since_Load);
                ActionSystem.Instance.AddReaction(triggerEventGA);
                triggerEventGA = new(Events.WhenInternCounter, null, null, null, CounterType.CardsDraw_Since_Load);
                ActionSystem.Instance.AddReaction(triggerEventGA);
            }
            triggerEventGA = new(Events.WhenDraw, cardView.Card);
            ActionSystem.Instance.AddReaction(triggerEventGA);
            triggerEventGA = new(Events.OnDraw, cardView.Card);
            ActionSystem.Instance.AddReaction(triggerEventGA);


            if (!AudioManager.Instance.IsValid(card.DrawCardSound))
            {
                RuntimeManager.PlayOneShot(AudioManager.Instance.DrawCardSound);
            }
            else
            {
                RuntimeManager.PlayOneShot(card.DrawCardSound);
            }

            yield return handView.AddCard(cardView);

            FinalTime -= Time.time;
            Debug.Log("Time to draw " + cardView.Card + " : " + FinalTime);
        }

    }

    private void RefillDeck()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        UpdatePiles();
    }

    private IEnumerator DiscardOncePerformer(DiscardOnceGA discardOnceGA)
    {
        if (discardOnceGA.RestCardView == null || discardOnceGA.RestCardView.Count == 0)
            yield break;

        CardView cv = discardOnceGA.RestCardView[0];
        yield return DiscardCard(cv, discardOnceGA.CountAsDiscard);

        handView.RemoveCard(cv.Card);
        hand.Remove(cv.Card);
        discardOnceGA.RestCardView.RemoveAt(0);

        if (discardOnceGA.RestCardView.Count > 0)
        {
            ActionSystem.Instance.AddReaction(
                new DiscardOnceGA(discardOnceGA.RestCardView, discardOnceGA.CountAsDiscard)
            );
        }
        yield return null;
    }
    
    private IEnumerator DiscardAllCardsPerformer(DiscardAllCardsGA discardAllCardsGA)
    {
        List<CardView> cardViewsToDiscard = new();
        foreach (Card card in hand)
        {
            cardViewsToDiscard.Add(handView.GetCardView(card));
        }

        DiscardOnceGA discardOnceGA = new(cardViewsToDiscard,discardAllCardsGA.CountAsDiscard);
        ActionSystem.Instance.AddReaction(discardOnceGA);

        yield return null;
    }

    public IEnumerator DiscardCard(CardView cardView, bool countAsDiscard_INGAME)
    {
        if (countAsDiscard_INGAME)
        {
            CounterSystem.Instance.Add(CounterType.CardsDiscard_This_Turn);
            CounterSystem.Instance.Add(CounterType.CardsDiscard_Since_Load);
            TriggerEventGA triggerEventGA = new(Events.WhenDiscard, cardView.Card);
            ActionSystem.Instance.AddReaction(triggerEventGA);
            triggerEventGA = new(Events.OnDiscard, cardView.Card);
            ActionSystem.Instance.AddReaction(triggerEventGA);      
            triggerEventGA = new(Events.WhenGlobalCounter,null,null,null,CounterType.CardsDiscard_This_Turn);
            ActionSystem.Instance.AddReaction(triggerEventGA);
            triggerEventGA = new(Events.WhenInternCounter,null,null,null,CounterType.CardsDiscard_This_Turn);
            ActionSystem.Instance.AddReaction(triggerEventGA);
            triggerEventGA = new(Events.WhenGlobalCounter,null,null,null,CounterType.CardsDiscard_Since_Load);
            ActionSystem.Instance.AddReaction(triggerEventGA);
            triggerEventGA = new(Events.WhenInternCounter,null,null,null,CounterType.CardsDiscard_Since_Load);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }

        if (cardView != null)
        {
            if (cardView.Card != null)
            {
                if (!AudioManager.Instance.IsValid(cardView.Card.DiscardCardSound))
                {
                    RuntimeManager.PlayOneShot(AudioManager.Instance.DiscardCardSound);
                }
                else
                {
                    RuntimeManager.PlayOneShot(cardView.Card.DiscardCardSound);
                }

                cardView.Card.RefCardView = null;
                cardView.transform.DOScale(Vector3.zero, 0.15f);
                Tween tween = cardView.transform.DOMove(discardPilePoint.position, 0.15f);
                yield return tween.WaitForCompletion();
                discardPile.Add(cardView.Card);
                UpdatePiles();
                Destroy(cardView.gameObject);
            }
        }
        
        if (hand.Count == 0)
        {
            TriggerEventGA triggerEventGA = new(Events.EmptyHanded,null,null,null);
            ActionSystem.Instance.AddReaction(triggerEventGA);            
        }
    }

    public IEnumerator DestroyCard(CardView cardView)
    {
        Tween tween = cardView.transform.DOScale(Vector3.zero, 0.15f);
        yield return tween.WaitForCompletion();
        Destroy(cardView.gameObject);
    }


    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
    {
        // Si on joue une carte toute les event OnPlay ce joue (il faudrait faire des OnPlaySpell, OnPlayPermanent ect...)
        TriggerEventGA triggerEventGA = new(Events.WhenPlayCard);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        triggerEventGA = new(Events.WhenPlaySpell);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        //Gestion des events de counter interne et globaux
        triggerEventGA = new(Events.WhenGlobalCounter,null,null,null,CounterType.SpellCast_This_Turn);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        triggerEventGA = new(Events.WhenInternCounter,null,null,null,CounterType.SpellCast_This_Turn);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        triggerEventGA = new(Events.WhenGlobalCounter,null,null,null,CounterType.SpellCast_Since_Load);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        triggerEventGA = new(Events.WhenInternCounter,null,null,null,CounterType.SpellCast_Since_Load);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        if (!AudioManager.Instance.IsValid(playCardGA.Card.PlayCardSound))
        {
            RuntimeManager.PlayOneShot(AudioManager.Instance.PlayCardSound);
        }
        else
        {
            RuntimeManager.PlayOneShot(playCardGA.Card.PlayCardSound);
        }

        if (!AudioManager.Instance.IsValid(playCardGA.Card.PlaySpellSound))
        {
            RuntimeManager.PlayOneShot(AudioManager.Instance.PlaySpellSound);
        }
        else
        {
            RuntimeManager.PlayOneShot(playCardGA.Card.PlaySpellSound);
        }

        hand.Remove(playCardGA.Card);
        CardView cardView = handView.RemoveCard(playCardGA.Card);
        yield return DiscardCard(cardView, false);

        SpendManaGA spendManaGA = new(playCardGA.Card.cost + playCardGA.Card.BonusCost);
        ActionSystem.Instance.AddReaction(spendManaGA);

        GameEventSystem.Instance.ManageEffects(playCardGA.Card,null,null);
    }

    public IEnumerator InsertCard(CardView card)
    {
        yield return DiscardCard(card, false);
    }

    public void ShowScryPanel(List<Card> cards)
    {
        DisplayScryCards(cards);
        ScryPanel.SetActive(true);
        CombatSystem.Instance.Interactable = false;
    }

    public void HideScryPanel()
    {
        ScryPanel.SetActive(false);
        CombatSystem.Instance.Interactable = true;
    }

    public void ShowChoicePanel(List<Effect> effects, bool SelectMode, bool MayChoice)
    {
        DisplayChoiceCards(effects,SelectMode,MayChoice);
        ChoicePanel.SetActive(true);
        CombatSystem.Instance.Interactable = false;
    }
    
    public void HideChoicePanel()
    {
        ChoicePanel.SetActive(false);
        CombatSystem.Instance.Interactable = true;
    }

    public void DisplayScryCards(List<Card> CardsToDisplay)
    {
        CleanScryPanel();
        foreach (var card in CardsToDisplay)
        {
            CardView cardView = CardViewCreator.Instance.CreateCardView(card, Vector3.zero, Quaternion.identity, ScryPanelContent.transform);
            cardView.IsScryCard = true;
            cardView.gameObject.GetComponent<SortingGroup>().sortingOrder = 5;
            cardView.gameObject.GetComponent<SortingGroup>().sortingLayerName = "UI";
            cardView.gameObject.transform.position.Set(cardView.gameObject.transform.position.x, cardView.gameObject.transform.position.y, 0);
            cardView.transform.DOScale(60, 0.5f);
            ScryCardViews.Add(cardView);
        }
    }

    public void DisplayChoiceCards(List<Effect> effectsToDisplay, bool SelectMode, bool MayChoice)
    {
        CleanChoicePanel();
        if (MayChoice)
        {
            CardData noneChoiceCardData = ScriptableObject.CreateInstance<CardData>();
            noneChoiceCardData.Title = "Do Nothing";
            noneChoiceCardData.Description = "Do Nothing";
            Card noneChoiceCard = new(noneChoiceCardData);
            CardView cardView = CardViewCreator.Instance.CreateCardView(noneChoiceCard, Vector3.zero, Quaternion.identity, ChoicePanelContent.transform);
            cardView.IsChoiceCard = true;
            cardView.gameObject.GetComponent<SortingGroup>().sortingOrder = 5;
            cardView.gameObject.GetComponent<SortingGroup>().sortingLayerName = "UI";
            cardView.gameObject.transform.position.Set(cardView.gameObject.transform.position.x, cardView.gameObject.transform.position.y, 0);
            cardView.transform.DOScale(60, 0.5f);
        }

        foreach (var effect in effectsToDisplay)
        {
            Card cardVisual = null;
            if (effect.Actionner != null)
            {
                if (effect.Actionner.GetComponent<PermanentView>() != null)
                {
                    cardVisual = effect.Actionner.GetComponent<PermanentView>().CardReferenceArchive;
                }
                else if (effect.Actionner.GetComponent<EnemySlotView>() != null)
                {
                    //Pas de card pour les enemy :/
                    //cardVisual = effect.Actionner.GetComponent<EnemySlotView>().;
                }
            }
            else if (effect.CardActionner != null)
            {
                cardVisual = effect.CardActionner;
            }

            CardView cardView = CardViewCreator.Instance.CreateCardView(cardVisual, Vector3.zero, Quaternion.identity, ChoicePanelContent.transform);
            cardView.IsChoiceCard = true;
            cardView.EffectHolder = effect;
            cardView.EffectHolder.Actionner = effect.Actionner;
            cardView.EffectHolder.CardActionner = effect.CardActionner;
            cardView.gameObject.GetComponent<SortingGroup>().sortingOrder = 5;
            cardView.gameObject.GetComponent<SortingGroup>().sortingLayerName = "UI";
            cardView.gameObject.transform.position.Set(cardView.gameObject.transform.position.x, cardView.gameObject.transform.position.y, 0);
            cardView.transform.DOScale(60, 0.5f);

            if (effect.ActivateLeft == 0 && SelectMode)
            {
                cardView.UnvalidChoiceCard();
            }
        }
    }

    public void CleanScryPanel()
    {
        foreach (Transform child in ScryPanelContent.transform)
            Destroy(child.gameObject);

        ScryCardViews.Clear();
    }

    public void CleanChoicePanel()
    {
        foreach (Transform child in ChoicePanelContent.transform)
            Destroy(child.gameObject);
    }

    public IEnumerator PutCardViewOnSide(CardView cardView)
    {
        Tween tween = cardView.gameObject.transform.DOMove(new Vector3 (CardTamponPoint.position.x,CardTamponPoint.position.y, 0f),0.25f);
        yield return tween.WaitForCompletion();
    }

    public void PayXPlus()
    {
        if (PayXCardView != null)
        {
            if (ManaSystem.Instance.currentMana - PayXCardView.Card.cost + PayXCardView.Card.BonusCost - 1 >= 0)
            {
                int value = int.Parse(PayXCounter.text);
                value++;
                PayXCounter.text = value.ToString();
                ManaSystem.Instance.VisualsubtractMana(1);
            }
        }
        else
        {
            if (ManaSystem.Instance.currentMana - 1 >= 0)
            {
                int value = int.Parse(PayXCounter.text);
                value++;
                PayXCounter.text = value.ToString();
                ManaSystem.Instance.VisualsubtractMana(1);
            }            
        }
    }

    public void PayXMinus()
    {
        if (ManaSystem.Instance.currentMana + 1 <= ManaSystem.Instance.PayXInitialMana)
        {
            int value = int.Parse(PayXCounter.text);
            value--;
            PayXCounter.text = value.ToString();
            ManaSystem.Instance.VisualAddMana(1);
        }
    }

    public void PayXValidate()
    {
        if (PayXCardView != null)
        {
            PayXCardView.PayXValue = int.Parse(PayXCounter.text);
            int i = 0;
            PayXCounter.text = i.ToString();
            PayXCardView.IsPayXValidate = true;
        }
        else
        {
            PayXValue = int.Parse(PayXCounter.text);
            int i = 0;
            PayXCounter.text = i.ToString();
            IsPayXValidate = true;            
        }
    }
    
    public void PayXCancel()
    {
        ManaSystem.Instance.currentMana = ManaSystem.Instance.PayXInitialMana;
        ManaSystem.Instance.UpdateManaText();
        ManaSystem.Instance.Mana_Spent_Count -= int.Parse(PayXCounter.text);
        if (PayXCardView != null)
        {
            PayXCardView.PayXValue = -1;
            int i = 0;
            PayXCounter.text = i.ToString();
            PayXCardView.IsPayXValidate = true;
        }
        else
        {
            PayXValue = -1;
            int i = 0;
            PayXCounter.text = i.ToString();
            IsPayXValidate = true;            
        }
    }

    public IEnumerator ManagePayX(bool Canceled, System.Action<bool> onFinished, Effect effect = null, CardView cardview = null)
    {
        PayXPanel.SetActive(true);
        CombatSystem.Instance.Interactable = false;
        PayXValue = 0;
        ManaSystem.Instance.PayXInitialMana = ManaSystem.Instance.currentMana;

        if (cardview != null)
        {
            cardview.WhaitForPayX = true;
            cardview.PayXValue = 0;
            yield return StartCoroutine(PutCardViewOnSide(cardview));
        }

        while (!IsPayXValidate)
        {
            yield return null;
        }

        if (PayXValue != -1)
        {
            if (effect != null)
            {
                effect.PayXValue = PayXValue;
            }
            else if (cardview.Card != null)
            {
                cardview.Card.PayXValue = PayXValue;
                cardview.WhaitForPayX = false;
            }
        }
        else
        {
            if (effect != null)
            {
                effect.PayXValue = 0;
            }
            else if (cardview.Card != null)
            {
                cardview.Card.PayXValue = 0;
                cardview.WhaitForPayX = false;
            }
            Canceled = true;
        }

        IsPayXValidate = false;
        CombatSystem.Instance.Interactable = true;
        PayXCardView = null;
        PayXPanel.SetActive(false);

        onFinished?.Invoke(Canceled);
    }
    

    // REACTIONS

    private void EndPlayerTurnPreReaction(EndPlayerTurnGA endPlayerTurnGA)
    {
        DiscardAllCardsGA discardAllCardsGA = new(false);
        ActionSystem.Instance.AddReaction(discardAllCardsGA);
    }

    public void UpdatePiles()
    {
        DrawDeck.UpdateDeckData(drawPile);
        DiscardDeck.UpdateDeckData(discardPile);
    }
}
