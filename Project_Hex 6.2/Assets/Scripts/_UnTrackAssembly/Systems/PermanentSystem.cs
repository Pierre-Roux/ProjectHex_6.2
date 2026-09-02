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

        //List<CopyVarGroup> copyVarGroup = CombatSystem.Instance.GetCopyValues(CopyTokenType.Permanent, Enemy_Player_ENUM.Player);
        List<CopyVarGroup> copyVarGroup = null;
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
                    if (ConditionSystem.Instance.TestCondition(SubVarGroup.Conditions, cardToSummon, null, null, cardToSummon))
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
            //CombatSystem.Instance.RemoveCopyGroup(CopyTokenType.Permanent, Enemy_Player_ENUM.Player, varGroup);
        }

        yield return cardSystem.DestroyCard(cardView);

        EventInfo eventInfo;
        if (cardSystem.hand.Count == 0)
        {
            eventInfo = new EventInfo(Events.EmptyHanded, Enemy_Player_ENUM.NULL, KeyWordType.NULL);
            triggerEventGA = new(eventInfo, null, null, null, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }

        SpendManaGA spendManaGA = new(summonGA.cardToInvoke.cost + summonGA.cardToInvoke.BonusCost);
        ActionSystem.Instance.AddReaction(spendManaGA);

        eventInfo = new EventInfo(Events.WhenPlayType, Enemy_Player_ENUM.Player, KeyWordType.PermaCard);
        triggerEventGA = new(eventInfo, null, null, null, null);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        eventInfo = new EventInfo(Events.WhenPlayType, Enemy_Player_ENUM.Player, KeyWordType.SpellCard);
        triggerEventGA = new(eventInfo, null, null, null, null);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        eventInfo = new EventInfo(Events.WhenPlayType, Enemy_Player_ENUM.Player, KeyWordType.NULL);
        triggerEventGA = new(eventInfo, null, null, null, null);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        eventInfo = new EventInfo(Events.WhenGlobalCounter, Enemy_Player_ENUM.Player, KeyWordType.NULL);
        CounterTypeInfo counterTypeInfo = new CounterTypeInfo(false, false, Enemy_Player_ENUM.Player, KeyWordType.NULL,CounterType.PermanentCast);
        triggerEventGA = new(eventInfo, counterTypeInfo, null, null, null);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        eventInfo = new EventInfo(Events.WhenGlobalCounter, Enemy_Player_ENUM.Player, KeyWordType.NULL);
        counterTypeInfo = new CounterTypeInfo(true, false, Enemy_Player_ENUM.Player, KeyWordType.NULL,CounterType.PermanentCast);
        triggerEventGA = new(eventInfo, counterTypeInfo, null, null, null);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        eventInfo = new EventInfo(Events.WhenInternCounter, Enemy_Player_ENUM.Player, KeyWordType.NULL);
        counterTypeInfo = new CounterTypeInfo(false, true, Enemy_Player_ENUM.Player, KeyWordType.NULL,CounterType.PermanentCast);
        triggerEventGA = new(eventInfo, counterTypeInfo, null, null, null);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        eventInfo = new EventInfo(Events.WhenInternCounter, Enemy_Player_ENUM.Player, KeyWordType.NULL);
        counterTypeInfo = new CounterTypeInfo(true, true, Enemy_Player_ENUM.Player, KeyWordType.NULL,CounterType.PermanentCast);
        triggerEventGA = new(eventInfo, counterTypeInfo, null, null, null);
        ActionSystem.Instance.AddReaction(triggerEventGA);
    }

    // REACTIONS
    

    
}
