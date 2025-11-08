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
    
    public List<Card> drawPile = new();
    public List<Card> discardPile = new();
    public List<Card> hand = new();

    void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
        ActionSystem.AttachPerformer<DeckShuffleGA>(DeckShuffleGA);
        ActionSystem.SubscribeReaction<EndPlayerTurnGA>(EndPlayerTurnPreReaction, ReactionTiming.PRE);
        

    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardsGA>();
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
        ActionSystem.DetachPerformer<DeckShuffleGA>();
        ActionSystem.UnsubscribeReaction<EndPlayerTurnGA>(EndPlayerTurnPreReaction, ReactionTiming.PRE);

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


        int actualAmount = Mathf.Min(drawCardsGA.Amount, drawPile.Count);
        int notDrawAmount = drawCardsGA.Amount - actualAmount;
        for (int i = 0; i < actualAmount; i++)
        {
            yield return DrawCard();
        }
        if (notDrawAmount > 0)
        {
            RefillDeck();
            drawPile.Shuffle();
            if (drawPile.Count < notDrawAmount)
            {
                notDrawAmount = drawPile.Count;
            }
            for (int i = 0; i < notDrawAmount; i++)
            {
                yield return DrawCard();
            }
        }
    }

    private IEnumerator DrawCard()
    {
        Card card = drawPile.Draw();
        UpdatePiles();
        hand.Add(card);
        CardView cardView = CardViewCreator.Instance.CreateCardView(card, drawPilePoint.position, drawPilePoint.rotation);
        TriggerEventGA triggerEventGA = new(Events.OnDraw, cardView.Card);
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
    }

    private void RefillDeck()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        UpdatePiles();
    }

    private IEnumerator DiscardAllCardsPerformer(DiscardAllCardsGA discardAllCardsGA)
    {
        foreach (var card in hand)
        {
            CardView cardView = handView.RemoveCard(card);
            yield return DiscardCard(cardView, discardAllCardsGA.CountAsDiscard);
        }
        hand.Clear();
    }

    public IEnumerator DiscardCard(CardView cardView, bool countAsDiscard_INGAME)
    {
        if (countAsDiscard_INGAME)
        {
            TriggerEventGA triggerEventGA = new(Events.OnDiscard, cardView.Card);
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

                cardView.transform.DOScale(Vector3.zero, 0.15f);
                Tween tween = cardView.transform.DOMove(discardPilePoint.position, 0.15f);
                yield return tween.WaitForCompletion();
                discardPile.Add(cardView.Card);
                UpdatePiles();
                Destroy(cardView.gameObject);
            }            
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
        TriggerEventGA triggerEventGA = new(Events.OnPlayCard);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        triggerEventGA = new(Events.OnPlaySpell);
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

        SpendManaGA spendManaGA = new(playCardGA.Card.cost);
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
                    //Pas de visuel pour les enemy :/
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
            if (ManaSystem.Instance.currentMana - PayXCardView.Card.cost - 1 >= 0)
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
