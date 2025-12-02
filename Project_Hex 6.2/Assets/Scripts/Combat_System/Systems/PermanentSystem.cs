using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class PermanentSystem : Singleton<PermanentSystem>
{
    [SerializeField] private HandView handView;
    [SerializeField] private CardSystem cardSystem;

    void OnEnable()
    {
        ActionSystem.AttachPerformer<SummonGA>(SummonPermanentPerformer);
        
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<SummonGA>();
    }

    // PERFORMERS (si je veux faire un Perform dans un Performer il faut faire ActionSystem.Instance.AddReaction(GameAction) plutôt que ActionSystem.Instance.Perform(GameAction) )

    private IEnumerator SummonPermanentPerformer(SummonGA summonGA)
    {
        TriggerEventGA triggerEventGA = null;

        Card cardToSummon = summonGA.cardToInvoke;
        cardSystem.hand.Remove(cardToSummon);
        CardView cardView = handView.RemoveCard(cardToSummon);

        if (!AudioManager.Instance.IsValid(cardToSummon.PlayCardSound))
        {
            RuntimeManager.PlayOneShot(AudioManager.Instance.PlayCardSound);
        }
        else
        {
            RuntimeManager.PlayOneShot(cardToSummon.PlayCardSound);
        }

        PermanentViewCreator.Instance.CreatePermanentViewCreator(cardToSummon, cardToSummon.permanentArea);

        yield return cardSystem.DestroyCard(cardView);

        if (cardSystem.hand.Count == 0)
        {
            triggerEventGA = new(Events.EmptyHanded,null,null,null);
            ActionSystem.Instance.AddReaction(triggerEventGA);            
        }

        SpendManaGA spendManaGA = new(summonGA.cardToInvoke.cost + summonGA.cardToInvoke.BonusCost);
        ActionSystem.Instance.AddReaction(spendManaGA);
        triggerEventGA = new(Events.WhenPlayCard);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        triggerEventGA = new(Events.WhenPlayPerma);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        triggerEventGA = new(Events.WhenGlobalCounter,null,null,null,CounterType.PermanentCast_This_Turn);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        triggerEventGA = new(Events.WhenInternCounter,null,null,null,CounterType.PermanentCast_This_Turn);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        triggerEventGA = new(Events.WhenGlobalCounter,null,null,null,CounterType.PermanentCast_Since_Load);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        triggerEventGA = new(Events.WhenInternCounter,null,null,null,CounterType.PermanentCast_Since_Load);
        ActionSystem.Instance.AddReaction(triggerEventGA);
    }

    // REACTIONS
    

    
}
