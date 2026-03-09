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

        List<CopyVarGroup> copyVarGroup = CombatSystem.Instance.GetCopyValues(CopyTokenType.Permanent, Enemy_Player_ENUM.Player);
        List<CopyVarGroup> copyVarGroupUsed = new();
        int nbCopie = 1;
        if (copyVarGroup != null)
        {
            foreach (CopyVarGroup SubVarGroup in copyVarGroup)
            {
                if (SubVarGroup.Conditions.Count == 0)
                {
                    nbCopie += SubVarGroup.value;
                    copyVarGroupUsed.Add(SubVarGroup);
                }
                else
                {
                    if (ConditionSystem.Instance.TestCondition(SubVarGroup.Conditions, cardToSummon,null,null,cardToSummon))
                    {
                        nbCopie += SubVarGroup.value;
                        copyVarGroupUsed.Add(SubVarGroup);
                    }                      
                }
            }
        }
        for (int i = 0; i < nbCopie; i++)
        {
            PermanentViewCreator.Instance.CreatePermanentViewCreator(cardToSummon, cardToSummon.permanentArea);
        }
    
        foreach (CopyVarGroup varGroup in copyVarGroupUsed)
        {
            CombatSystem.Instance.RemoveCopyGroup(CopyTokenType.Permanent, Enemy_Player_ENUM.Player, varGroup);
        }

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
