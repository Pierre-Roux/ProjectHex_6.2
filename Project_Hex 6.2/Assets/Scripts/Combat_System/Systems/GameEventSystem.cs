using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class GameEventSystem : Singleton<GameEventSystem>
{
    public Dictionary<Events, List<Effect>> effectsByEvent = new();

    void OnEnable()
    {
        ActionSystem.AttachPerformer<TriggerEventGA>(TriggerEvent);

        ActionSystem.SubscribeReaction<TriggerEventGA>(UpdateDurationReaction, ReactionTiming.POST);

    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<TriggerEventGA>();

        ActionSystem.UnsubscribeReaction<TriggerEventGA>(UpdateDurationReaction, ReactionTiming.POST);
    }

    public void SetEvents(Dictionary<Events, List<Effect>> effectsbyevent)
    {
        ClearAllEvents();
        effectsByEvent = effectsbyevent;
    }

    //PERFORMERS

    public void AddEffectToEvent(Effect effectToExecute)
    {
        if (!effectsByEvent.TryGetValue(effectToExecute.Events, out var list))
        {
            list = new List<Effect>();
            effectsByEvent[effectToExecute.Events] = list;
        }
        list.Add(effectToExecute);
    }

    public IEnumerator TriggerEvent(TriggerEventGA triggerEventGA)
    {
        if (!effectsByEvent.TryGetValue(triggerEventGA.gameEvent, out var effectList))
            yield break;

        //Debug.Log("Event déclenché " + triggerEventGA.gameEvent);

        foreach (var effect in new List<Effect>(effectList))
        {
            bool isActionnerMatch = false;
            // Cas 1 : Permanent
            if (triggerEventGA.permanentView != null)
            {
                if (effect.Actionner != null)
                {
                    isActionnerMatch = effect.Actionner.GetComponent<PermanentView>() == triggerEventGA.permanentView;
                }
                    
            }

            // Cas 2 : Enemy
            else if (triggerEventGA.enemySlotView != null)
            {
                if (effect.Actionner != null)
                    isActionnerMatch = effect.Actionner.GetComponent<EnemySlotView>() == triggerEventGA.enemySlotView;
            }

            // Cas 3 : Card
            else if (triggerEventGA.Card != null)
            {
                if (effect.CardActionner != null)
                    isActionnerMatch = effect.CardActionner == triggerEventGA.Card;
            }

            // Cas 4 : Aucun actionner attendu (par exemple événements de carte globale)
            else
            {
                isActionnerMatch = true;
            }

            //Debug.Log("ActionnerMatch : " + isActionnerMatch);

            if (triggerEventGA.gameEvent != Events.WhenPermaDie && isActionnerMatch)
            {
                if (triggerEventGA.gameEvent == Events.OnActivate)
                {
                    if (triggerEventGA.permanentView != null)
                    {
                        RuntimeManager.PlayOneShot(triggerEventGA.permanentView.ActivateSound);
                        triggerEventGA.permanentView.GetComponent<Animator>().SetTrigger("Activate");
                        triggerEventGA.permanentView.Activated = true;
                    }
                    else if (triggerEventGA.enemySlotView != null)
                    {
                        RuntimeManager.PlayOneShot(triggerEventGA.enemySlotView.ActivateSound);
                        triggerEventGA.enemySlotView.GetComponent<Animator>().SetTrigger("Activate");
                        triggerEventGA.enemySlotView.Activated = true;
                    }      
                }

                // Gestion des effets avec durée
                if (effect.TriggerOnDurationEnd)
                {
                    if (effect.Duration == 1)
                    {
                        GameAction ga = effect.GetGameAction();
                        if (ga != null)
                            ActionSystem.Instance.AddReaction(ga);
                    }
                }
                else
                {
                    Debug.Log("effect déclanché : " + effect);
                    GameAction ga = effect.GetGameAction();
                    if (ga != null)
                        ActionSystem.Instance.AddReaction(ga);
                }
                
                /*if (effect.LinkedEffect != null && effect.Events == Events.OnActivate)
                {
                    Debug.Log("Resiter Event : " + effect.LinkedEffect);
                    Effect Linked = effect.LinkedEffect;
                    AddEffectToEvent(Linked);               
                }*/

            }

            // Fonctionnement pour les Events Concernant d'autre déclancheur que lui même
            if (triggerEventGA.gameEvent == Events.WhenPermaDie && !isActionnerMatch
            || triggerEventGA.gameEvent == Events.WhenPermaExaust && !isActionnerMatch
            || triggerEventGA.gameEvent == Events.WhenPermaBecomeType && !isActionnerMatch
            || triggerEventGA.gameEvent == Events.WhenPermaSac && !isActionnerMatch
            || triggerEventGA.gameEvent == Events.WhenPermaETB && !isActionnerMatch
            || triggerEventGA.gameEvent == Events.WhenPermaLossDurability && !isActionnerMatch
            )
            {
                if (effect.DynamicCondition != DynamicCondition.NULL)
                {
                    if (ConditionSystem.Instance.TestCondition(effect.DynamicCondition, effect.TestDynamicAmount, effect.TestValue, triggerEventGA.Card, triggerEventGA.permanentView, triggerEventGA.enemySlotView, effect.TestType))
                    {
                        effect.BypassEntryCondition = true;
                        GameAction ga = effect.GetGameAction();
                        if (ga != null)
                            ActionSystem.Instance.AddReaction(ga);
                    }
                }
                else
                {
                    GameAction ga = effect.GetGameAction();
                    if (ga != null)
                        ActionSystem.Instance.AddReaction(ga);
                }
            }
        }
        yield return null;
    }

    public void ClearAllEvents()
    {
        effectsByEvent.Clear();
    }

    public void RemoveEffect(Effect effect)
    {
        if (effectsByEvent.TryGetValue(effect.Events, out var list))
        {
            list.Remove(effect);
        }
    }

    public void RemoveEffectsByActionner(GameObject GOToSuppr)
    {
        GameObject actionnerToRemove = GOToSuppr;
        var eventsToCleanUp = new List<Events>();

        foreach (var eventEntry in effectsByEvent)
        {
            Events gameEvent = eventEntry.Key;
            List<Effect> effectList = eventEntry.Value;

            for (int i = effectList.Count - 1; i >= 0; i--)
            {
                if (effectList[i].Actionner == actionnerToRemove)
                {
                    // si l'effet se détruit quand l'Actionner meurt on l'enlève sinon il reste même après la mort 
                    if (effectList[i].CancelOnDeath)
                    {
                        effectList.RemoveAt(i);
                    }
                }
            }

            if (effectList.Count == 0)
            {
                eventsToCleanUp.Add(gameEvent);
            }
        }

        // Nettoyer les événements devenus vides
        foreach (var gameEvent in eventsToCleanUp)
        {
            effectsByEvent.Remove(gameEvent);
        }
    }

    // REACTIONS

    private void UpdateDurationReaction(TriggerEventGA triggerEventGA)
    {
        List<Effect> effectsToRemove = new();

        foreach (var kvp in effectsByEvent)
        {
            var eventType = kvp.Key;
            var effectList = kvp.Value;
            foreach (var effect in effectList)
            {
                if (effect.Duration >= 0 && triggerEventGA.gameEvent == effect.DurationType)
                {
                    effect.Duration--;
                    Debug.Log(effect + " Lost 1 duration : " + effect.Duration + " Left");

                    if (effect.Duration <= 0)
                    {
                        effectsToRemove.Add(effect);
                    }
                }
            }
        }

        foreach (var effect in effectsToRemove)
        {
            RemoveEffect(effect);
        }
    }
}
