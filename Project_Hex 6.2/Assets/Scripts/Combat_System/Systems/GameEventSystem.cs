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
        bool EffectCanceled = false;
        List<Effect> OnSelectEffects = new List<Effect>();

        if (!effectsByEvent.TryGetValue(triggerEventGA.gameEvent, out var effectList))
            yield break;

        //Debug.Log("Event déclenché " + triggerEventGA.gameEvent);

        //Son et animation si OnSelect
        if (triggerEventGA.gameEvent == Events.OnSelect)
        {
            if (triggerEventGA.permanentView != null)
            {
                RuntimeManager.PlayOneShot(triggerEventGA.permanentView.ActivateSound);
                triggerEventGA.permanentView.GetComponent<Animator>().SetTrigger("Activate");
            }
            else if (triggerEventGA.enemySlotView != null)
            {
                RuntimeManager.PlayOneShot(triggerEventGA.enemySlotView.ActivateSound);
                triggerEventGA.enemySlotView.GetComponent<Animator>().SetTrigger("Activate");
            }
        }

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
            // Ajout des effets Onselect de l'entité pour post traitment
            if (triggerEventGA.gameEvent == Events.OnSelect && isActionnerMatch)
            {
                OnSelectEffects.Add(effect);
            }

            if (triggerEventGA.gameEvent != Events.WhenPermaDie && triggerEventGA.gameEvent != Events.OnSelect && isActionnerMatch)
            {
                // Gestion du Payx effect
                if (effect.PayXEffect == true)
                {
                    yield return StartCoroutine(CardSystem.Instance.ManagePayX(false, (result) =>
                    {
                        EffectCanceled = result;
                    }, effect));
                }

                if (!EffectCanceled)
                {
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
                        //Debug.Log("effect déclanché : " + effect);
                        GameAction ga = effect.GetGameAction();
                        if (ga != null)
                            ActionSystem.Instance.AddReaction(ga);
                    }
                }
            }

            // Fonctionnement pour les Events Concernant d'autre déclancheur que eux même
            if (!EffectCanceled)
            {
                if (triggerEventGA.gameEvent == Events.WhenPermaDie && !isActionnerMatch
                || triggerEventGA.gameEvent == Events.WhenPermaExaust && !isActionnerMatch
                || triggerEventGA.gameEvent == Events.WhenPermaBecomeType && !isActionnerMatch
                || triggerEventGA.gameEvent == Events.WhenPermaSac && !isActionnerMatch
                || triggerEventGA.gameEvent == Events.WhenPermaETB && !isActionnerMatch
                || triggerEventGA.gameEvent == Events.WhenPermaLossDurability && !isActionnerMatch
                || triggerEventGA.gameEvent == Events.WhenPermaDamaged && !isActionnerMatch
                || triggerEventGA.gameEvent == Events.WhenPCoreDamaged && !isActionnerMatch
                || triggerEventGA.gameEvent == Events.WhenECoreDamaged && !isActionnerMatch
                )
                {
                    if (effect.DynamicConditionInfos.Count != 0)
                    {
                        if (ConditionSystem.Instance.TestCondition(effect.DynamicConditionInfos, triggerEventGA.Card, triggerEventGA.permanentView, triggerEventGA.enemySlotView))
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
        }

        // Traitement des OnSelectEvents
        if (triggerEventGA.gameEvent == Events.OnSelect)
        {
            if (OnSelectEffects.Count == 1)
            {
                OnSelectEffects[0].ActivateLeft--;
                ActionSystem.Instance.AddReaction(OnSelectEffects[0].GetGameAction());
                if (OnSelectEffects[0].LinkedEffect != null)
                {
                    //Debug.Log("Resiter Event : " + effect.LinkedEffect);
                    Effect Linked = OnSelectEffects[0].LinkedEffect.Clone();
                    Linked.ParentEffect = OnSelectEffects[0];
                    Linked.Actionner = OnSelectEffects[0].Actionner;
                    AddEffectToEvent(Linked);
                }
            }
            else if (OnSelectEffects.Count > 1)
            {
                if (triggerEventGA.permanentView != null)
                {
                    foreach (var item in OnSelectEffects)
                    {
                        Debug.Log("->" + item + " -> " + item.Actionner + " -> " + item.CardActionner);
                    }
                    LetChoiceGA letChoiceGA = new(OnSelectEffects, true);
                    ActionSystem.Instance.AddReaction(letChoiceGA);
                }
                else if (triggerEventGA.enemySlotView != null)
                {
                    LetChoiceGA letChoiceGA = new(OnSelectEffects, true);
                    ActionSystem.Instance.AddReaction(letChoiceGA);
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

    public List<Effect> RetrieveEffectsFor(Card card, PermanentView permanentView, EnemySlotView enemySlotView)
    {
        List<Effect> result = new();

        foreach (var kvp in effectsByEvent) // kvp = KeyValuePair<Events, List<Effect>>
        {
            var effectList = kvp.Value;
            foreach (var effect in effectList)
            {
                if (card != null && effect.CardActionner == card)
                {
                    result.Add(effect);
                }
                else if (permanentView != null)
                {
                    var perma = effect.Actionner != null ? effect.Actionner.GetComponent<PermanentView>() : null;
                    if (perma == permanentView)
                        result.Add(effect);
                }
                else if (enemySlotView != null)
                {
                    var enemy = effect.Actionner != null ? effect.Actionner.GetComponent<EnemySlotView>() : null;
                    if (enemy == enemySlotView)
                        result.Add(effect);
                }
            }
        }

        return result;
    }

    public void ManageEffects(Card card, PermanentView permanentView, EnemySlotView enemySlotView, bool SetupMode = false)
    {
        if (card != null)
        {
            foreach (var effect in card.Effects)
            {
                Debug.Log("Registering effect : " + effect);
                int MultiHit = effect.MultiHit;
                if (MultiHit < 1) MultiHit = 1;
                for (int i = 0; i < MultiHit; i++)
                {
                    // On clone l’effet de base pour éviter les références partagées
                    Effect clonedEffect = effect.Clone();

                    // Attribution des Actionner et CardActionner
                    clonedEffect.Actionner = null;
                    clonedEffect.CardActionner = card; 

                    if (effect is ChoiceEffect)
                    {
                        ChoiceEffect choiceEffect = (ChoiceEffect)clonedEffect;
                        foreach (Effect effect1 in choiceEffect.EffectsForPlayerChoice)
                        {
                            effect1.Actionner = null;
                            effect1.CardActionner = card;                            
                        }
                    }
                    else if (effect is EffectGroup)
                    {
                        EffectGroup choiceEffect = (EffectGroup)clonedEffect;
                        foreach (Effect effect1 in choiceEffect.EffectGroups)
                        {
                            effect1.Actionner = null;
                            effect1.CardActionner = card;                            
                        }
                    }

                    //Boucle de Register
                    while (clonedEffect != null)
                    {
                        clonedEffect.CardActionner = card;    
                        
                        if (clonedEffect.Events == Events.Instant)
                        {
                            //Debug.Log("Register " + effect + " CardActionner : " + clonedEffect.CardActionner);
                            ActionSystem.Instance.AddReaction(clonedEffect.GetGameAction());
                        }
                        else
                        {
                            // Ajout aux Events (sauf cas spéciaux)
                            if (clonedEffect.Events != Events.OnDeath &&
                                clonedEffect.Events != Events.OnDestroy &&
                                clonedEffect.Events != Events.OnDamaged &&
                                clonedEffect.Events != Events.OnSelect &&
                                clonedEffect.Events != Events.EnemyTurn)
                            {
                                Instance.AddEffectToEvent(clonedEffect);
                            }
                        }

                        if (clonedEffect.LinkedEffect != null)
                        {
                            clonedEffect.LinkedEffect.ParentEffect = clonedEffect;
                        }

                        // Avancer dans la chaîne de linked
                        clonedEffect = clonedEffect.LinkedEffect;
                    }
                }
            }
        }
        else if (permanentView != null)
        {
            //Pour le moment pas de setup pour le player
            if (SetupMode)
            {

            }
            else
            {
                foreach (var effect in permanentView.CardReferenceArchive.Effects)
                {
                    int MultiHit = effect.MultiHit;
                    if (MultiHit < 1) MultiHit = 1;
                    for (int i = 0; i < MultiHit; i++)
                    {
                        // Vérifie Hollow
                        bool canApply = (permanentView.permaTypes.Contains(PermaTypes.Hollow) && effect.HollowEffect)
                                    || (!permanentView.permaTypes.Contains(PermaTypes.Hollow) && !effect.HollowEffect);
                        if (!canApply) continue;

                        // On démarre par l’effet cloné
                        Effect clonedEffect = effect.Clone();

                        // Attribution des Actionner et CardActionner
                        clonedEffect.Actionner = permanentView.gameObject;
                        clonedEffect.CardActionner = null; 

                        if (effect is ChoiceEffect)
                        {
                            ChoiceEffect choiceEffect = (ChoiceEffect)clonedEffect;
                            foreach (Effect effect1 in choiceEffect.EffectsForPlayerChoice)
                            {
                                effect1.Actionner = permanentView.gameObject;
                                effect1.CardActionner = null;                            
                            }
                        }
                        else if (effect is EffectGroup)
                        {
                            EffectGroup choiceEffect = (EffectGroup)clonedEffect;
                            foreach (Effect effect1 in choiceEffect.EffectGroups)
                            {
                                effect1.Actionner = permanentView.gameObject;
                                effect1.CardActionner = null;                            
                            }
                        }

                        while (clonedEffect != null)
                        {
                            clonedEffect.Actionner = permanentView.gameObject;

                            if (clonedEffect.Events == Events.OnSelect)
                            {
                                clonedEffect.ActivateLeft = clonedEffect.ActivateNumber;
                            }

                            if (clonedEffect.Events == Events.Instant)
                            {
                                DoEffectGA performEffectGA = new(clonedEffect);
                                ActionSystem.Instance.AddReaction(performEffectGA);
                            }
                            else
                            {
                                if (clonedEffect.Events != Events.EnemyTurn &&
                                    clonedEffect.Events != Events.Instant)
                                {
                                    //Debug.Log("Register " + clonedEffect);
                                    Instance.AddEffectToEvent(clonedEffect);
                                }
                            }

                            if (clonedEffect.Events != Events.OnSelect)
                            {
                                if (clonedEffect.LinkedEffect != null)
                                {
                                    clonedEffect.LinkedEffect.ParentEffect = clonedEffect;
                                }
                                clonedEffect = clonedEffect.LinkedEffect;
                            }
                            else
                            {
                                clonedEffect = null;
                            }
                        }
                    }
                }
            }
        }
        else if (enemySlotView != null)
        {
            // Si Setup on Manage en deux fois de façon que l'init dans CombatSystem passe avant l'ajout d'effet instant
            if (SetupMode)
            {
                // Ici on enregistre les effets dans SetupActions qui seront eux même register une fois que l'init sera terminé
                foreach (Effect effect in enemySlotView.PossibleIntent)
                {
                    int MultiHit = effect.MultiHit;
                    if (MultiHit < 1) MultiHit = 1;
                    for (int i = 0; i < MultiHit; i++)
                    {
                        Effect clonedEffect = effect.Clone();

                        while (clonedEffect != null)
                        {
                            clonedEffect.Actionner = enemySlotView.gameObject;

                            if (clonedEffect.Events == Events.OnSelect)
                            {
                                clonedEffect.ActivateLeft = clonedEffect.ActivateNumber;
                            }

                            if (clonedEffect.Events == Events.Instant)
                            {
                                if (clonedEffect.EffectTargetModeInfo != null) if (effect.EffectTargetModeInfo.targetMode == TargetMode.Manual) TargetSystem.Instance.ActivateAuraForTargets(effect.EffectTargetLimitations);
                                CombatSystem.Instance.currentEnemy.SetupActions.Add(clonedEffect.GetGameAction());
                            }

                            if (clonedEffect.LinkedEffect != null)
                            {
                                clonedEffect.LinkedEffect.ParentEffect = clonedEffect;
                            }
                            clonedEffect.Actionner = enemySlotView.gameObject;
                            clonedEffect = clonedEffect.LinkedEffect;
                        }
                    }
                }
                // En Revanche on peut enregistrer les effets non instant
                foreach (Effect effect in enemySlotView.PossibleIntent)
                {
                    int MultiHit = effect.MultiHit;
                    if (MultiHit < 1) MultiHit = 1;
                    for (int i = 0; i < MultiHit; i++)
                    {
                        Effect clonedEffect = effect.Clone();
                        clonedEffect.Actionner = enemySlotView.gameObject;
                        while (clonedEffect != null)
                        {
                            if (clonedEffect.Events != Events.EnemyTurn &&
                                clonedEffect.Events != Events.Instant
                                )
                            {
                                Instance.AddEffectToEvent(clonedEffect);
                            }

                            if (clonedEffect.LinkedEffect != null)
                            {
                                clonedEffect.LinkedEffect.ParentEffect = clonedEffect;
                            }
                            clonedEffect.Actionner = enemySlotView.gameObject;
                            clonedEffect = clonedEffect.LinkedEffect;

                        }
                    }
                }
            }
            // Si pas en SetupMode On fait tout d'un coup
            else
            {
                foreach (Effect effect in enemySlotView.PossibleIntent)
                {
                    int MultiHit = effect.MultiHit;
                    if (MultiHit < 1) MultiHit = 1;
                    for (int i = 0; i < MultiHit; i++)
                    {
                        Effect clonedEffect = effect.Clone();

                        while (clonedEffect != null)
                        {
                            clonedEffect.Actionner = enemySlotView.gameObject;

                            if (clonedEffect.Events == Events.OnSelect)
                            {
                                clonedEffect.ActivateLeft = clonedEffect.ActivateNumber;
                            }

                            if (clonedEffect.Events == Events.Instant)
                            {
                                if (clonedEffect.EffectTargetModeInfo != null) if (effect.EffectTargetModeInfo.targetMode == TargetMode.Manual) TargetSystem.Instance.ActivateAuraForTargets(effect.EffectTargetLimitations);
                                ActionSystem.Instance.AddReaction(clonedEffect.GetGameAction());
                            }
                            else
                            {
                                if (clonedEffect.Events != Events.EnemyTurn &&
                                    clonedEffect.Events != Events.Instant)
                                {
                                    Instance.AddEffectToEvent(clonedEffect);
                                }
                            }
                            if (clonedEffect.LinkedEffect != null)
                            {
                                clonedEffect.LinkedEffect.ParentEffect = clonedEffect;
                            }
                            clonedEffect.Actionner = enemySlotView.gameObject;
                            clonedEffect = clonedEffect.LinkedEffect;
                        }
                    }
                }
            }
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
