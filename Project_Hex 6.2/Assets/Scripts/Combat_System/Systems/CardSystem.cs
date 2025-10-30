using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
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

    [SerializeField] public ScrollRect ScryScrollRect;

    [HideInInspector] public List<CardView> ScryCardViews;
    [HideInInspector] public Effect EffectChoosed;
    
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
            drawCardsGA.Amount = TargetSystem.Instance.GetDynamicAmount(drawCardsGA.DynamicAmount);
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

        foreach (var effect in playCardGA.Card.Effects)
        {
            int MultiHit = effect.MultiHit;
            if (MultiHit < 1) MultiHit = 1;
            for (int i = 0; i < MultiHit; i++)
            {
                // On clone l’effet de base pour éviter les références partagées
                Effect clonedEffect = effect.Clone();
                clonedEffect.Actionner = null;
                clonedEffect.CardActionner = playCardGA.CardActionner;

                while (clonedEffect != null)
                {
                    if (clonedEffect.Events == Events.Instant)
                    {
                        ActionSystem.Instance.AddReaction(clonedEffect.GetGameAction());
                    }
                    else
                    {
                        // Ajout aux Events (sauf cas spéciaux)
                        if (clonedEffect.Events != Events.OnDeath &&
                            clonedEffect.Events != Events.OnDestroy &&
                            clonedEffect.Events != Events.OnDamaged &&
                            clonedEffect.Events != Events.OnActivate &&
                            clonedEffect.Events != Events.EnemyTurn)
                        {
                            GameEventSystem.Instance.AddEffectToEvent(clonedEffect);
                        }
                    }

                    // On lie le parent au linked effect (utile si la chaîne est clonée)
                    if (clonedEffect.LinkedEffect != null)
                    {
                        clonedEffect.LinkedEffect.ParentEffect = clonedEffect;
                    }

                    // Avancer dans la chaîne
                    clonedEffect = clonedEffect.LinkedEffect;
                }                
            }
        }
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

    public void ShowChoicePanel(List<Effect> effects, Card cardVisual, GameObject actionner = null)
    {
        DisplayChoiceCards(effects,cardVisual,actionner);
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

    public void DisplayChoiceCards(List<Effect> effectsToDisplay, Card cardVisual, GameObject Actionner)
    {
        CleanChoicePanel();
        foreach (var effect in effectsToDisplay)
        {
            CardView cardView = CardViewCreator.Instance.CreateCardView(cardVisual, Vector3.zero, Quaternion.identity, ChoicePanelContent.transform);
            cardView.IsChoiceCard = true;
            cardView.EffectHolder = effect;
            cardView.EffectHolder.Actionner = Actionner;
            cardView.gameObject.GetComponent<SortingGroup>().sortingOrder = 5;
            cardView.gameObject.GetComponent<SortingGroup>().sortingLayerName = "UI";
            cardView.gameObject.transform.position.Set(cardView.gameObject.transform.position.x, cardView.gameObject.transform.position.y, 0);
            cardView.transform.DOScale(60, 0.5f);
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
