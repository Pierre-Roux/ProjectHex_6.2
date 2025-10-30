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

        SpendManaGA spendManaGA = new(summonGA.cardToInvoke.cost);
        ActionSystem.Instance.AddReaction(spendManaGA);

        // Si on joue une carte toute les event OnPlay ce joue (il faudrait faire des OnPlaySpell, OnPlayPermanent ect...)
        TriggerEventGA triggerEventGA = new(Events.OnPlayCard);
        ActionSystem.Instance.AddReaction(triggerEventGA);
    }

    // REACTIONS
    

    
}
