using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameEventSystem : Singleton<GameEventSystem>
{
    [SerializeField] public EffectToolTip EffectToolTip;
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

    public void AddEffectToEvent(Events ev, Effect effectToExecute)
    {
        if (!effectsByEvent.TryGetValue(ev, out var list))
        {
            list = new List<Effect>();
            effectsByEvent[ev] = list;
        }

        list.Add(effectToExecute);
    }

    public IEnumerator TriggerEvent(TriggerEventGA triggerEventGA)
    {
        bool EffectCanceled = false;
        List<Effect> OnSelectEffects = new List<Effect>();

        if (!effectsByEvent.TryGetValue(triggerEventGA.gameEvent, out var effectList))
            yield break;

        // Gestion des effets qui concerne les Counters globaux ou interne
        if (triggerEventGA.gameEvent == Events.WhenInternCounter || triggerEventGA.gameEvent == Events.WhenGlobalCounter)
        {
            CounterType counterType = triggerEventGA.counterTypeConcerned;
            if (counterType != CounterType.NULL)
            {
                List<Effect> matchingEffects = effectList
                    .Where(e => e.TypeOfCounter == counterType)
                    .ToList();

                if (matchingEffects.Count == 0)
                    yield break;

                CombatSystem combatSystem = CombatSystem.Instance;
                foreach (Effect effect in matchingEffects)
                {
                    int CounterValue = 0;
                    if (triggerEventGA.gameEvent == Events.WhenGlobalCounter)
                    {
                        // Si c'est par rapport à un counter global on prend la valeur du counter global 
                        CounterValue = combatSystem.GlobalCounters.Get(counterType);
                    }
                    else
                    {
                        CounterManager counterManager = new();
                        // Sinon on prend la valeur du counter interne de l'entité (PermanentView, EnemySlotView, Card) qui à enregistré l'effet
                        if (effect.Actionner != null)
                        {
                            if (effect.Actionner.GetComponent<PermanentView>() != null)
                            {
                                counterManager = effect.Actionner.GetComponent<PermanentView>().InternCounters;
                            }
                            else if (effect.Actionner.GetComponent<EnemySlotView>() != null)
                            {
                                counterManager = effect.Actionner.GetComponent<EnemySlotView>().InternCounters;
                            }
                        }
                        else if (effect.CardActionner != null)
                        {
                            counterManager = effect.CardActionner.InternCounters;
                        }
                        CounterValue = counterManager.Get(counterType);
                    }

                    //Debug.Log("Modulo ? : " + effect.ModuloValue + " CounterValue : " + CounterValue + " effectCounterValue" + effect.CounterValue);

                    if (effect.ModuloValue)
                    {
                        // On déclenche quand Modulo(globalCounter, effect.CounterValue) == 0
                        if (Modulo(CounterValue, effect.CounterValue) == 0)
                        {
                            effect.Events = new List<Events> { Events.Instant };
                            RegisterEffect(effect);
                        }
                    }
                    else
                    {
                        if (CounterValue >= effect.CounterValue)
                        {
                            effect.Events = new List<Events> { Events.Instant };
                            RegisterEffect(effect);
                        }
                    }
                }
                yield break;
            }
            yield break;
        }

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

        // Gestion des effets Standards et globaux
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

            // Fonctionnement pour les Events Concernant d'autre déclancheur que eux même et les flags
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
                || triggerEventGA.gameEvent == Events.WhenDiscard && !isActionnerMatch
                || triggerEventGA.gameEvent == Events.WhenDraw && !isActionnerMatch
                || triggerEventGA.gameEvent == Events.WhenPlayCard && !isActionnerMatch
                || triggerEventGA.gameEvent == Events.WhenPlaySpell && !isActionnerMatch
                || triggerEventGA.gameEvent == Events.WhenPlayPerma && !isActionnerMatch

                || triggerEventGA.gameEvent == Events.HollowCountChanged && !isActionnerMatch
                || triggerEventGA.gameEvent == Events.DecayCountChanged && !isActionnerMatch
                || triggerEventGA.gameEvent == Events.InvocCountChanged && !isActionnerMatch
                || triggerEventGA.gameEvent == Events.ArtilleryCountChanged && !isActionnerMatch
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

        // Gestion des Effets OnSelectEvents
        if (triggerEventGA.gameEvent == Events.OnSelect)
        {
            if (OnSelectEffects.Count == 1)
            {
                var effect = OnSelectEffects[0];

                effect.ActivateLeft--;

                if (effect is EffectGroup group)
                {
                    foreach (Effect subEffect in group.EffectGroups)
                    {
                        subEffect.Actionner = group.Actionner;
                        subEffect.CardActionner = group.CardActionner;

                        int multiHit = group.MultiHit;
                        if (multiHit < 1) multiHit = 1;

                        for (int i = 0; i < multiHit; i++)
                        {
                            ActionSystem.Instance.AddReaction(subEffect.GetGameAction());

                            if (subEffect.LinkedEffect != null)
                            {
                                Effect linked = subEffect.LinkedEffect.Clone();
                                linked.ParentEffect = subEffect;
                                linked.Actionner = subEffect.Actionner;

                                // Enregistrer linked effect dans TOUS ses events
                                foreach (var ev in linked.Events)
                                    AddEffectToEvent(ev, linked);
                            }
                        }
                    }
                }
                else
                {
                    int multiHit = effect.MultiHit;
                    if (multiHit < 1) multiHit = 1;

                    for (int i = 0; i < multiHit; i++)
                    {
                        ActionSystem.Instance.AddReaction(effect.GetGameAction());

                        if (effect.LinkedEffect != null)
                        {
                            Effect linked = effect.LinkedEffect.Clone();
                            linked.ParentEffect = effect;
                            linked.Actionner = effect.Actionner;

                            // Enregistrer linked effect dans TOUS ses events
                            foreach (var ev in linked.Events)
                                AddEffectToEvent(ev, linked);
                        }
                    }
                }
            }
            else if (OnSelectEffects.Count > 1)
            {
                if (triggerEventGA.permanentView != null || triggerEventGA.enemySlotView != null)
                {
                    LetChoiceGA letChoiceGA = new(OnSelectEffects, true);
                    letChoiceGA.Actionner = OnSelectEffects[0].Actionner;
                    letChoiceGA.CardActionner = OnSelectEffects[0].CardActionner;
                    letChoiceGA.SourceEffect = OnSelectEffects[0];
                    letChoiceGA.ActivateToolTip = false;
                    ActionSystem.Instance.AddReaction(letChoiceGA);
                }
            }
        }

        yield return null;
    }
    
    public IEnumerator ShowEffectToolTip(Effect effect)
    {
        PermanentView permanentView_Origin;
        EnemySlotView EnemySlotView_Origin;
        Card Card_Origin;

        string Title = null;
        string Description = null;
        Sprite image = null;

        Debug.Log("Effect ->>> " + effect + " Actionner ->> " + effect.Actionner + " CardActionner -> " + effect.CardActionner);

        if (effect.Actionner != null)
        {
            if (effect.Actionner.GetComponent<PermanentView>() != null)
            {
                permanentView_Origin = effect.Actionner.GetComponent<PermanentView>();
                Title = permanentView_Origin.NameText.text;
                Description = permanentView_Origin.CardReferenceArchive.Description;
                image = permanentView_Origin.PermanentSpriteRenderer.sprite;
            }
            else if (effect.Actionner.GetComponent<EnemySlotView>() != null)
            {
                EnemySlotView_Origin = effect.Actionner.GetComponent<EnemySlotView>();
                Title = EnemySlotView_Origin.NameText.text;
                Description = "";
                image = EnemySlotView_Origin.spriteRenderer.sprite;
            }
        }
        else if (effect.CardActionner != null)
        {
            Card_Origin = effect.CardActionner;
            Title = Card_Origin.Title;
            Description = Card_Origin.Description;
            image = Card_Origin.Image;
        }

        EffectToolTip.Set(Title, Description, image);
        EffectToolTip.gameObject.SetActive(true);
        yield return EffectToolTip.Appear();
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator HideEffectToolTip()
    {
        EffectToolTip.Reset();
        yield return EffectToolTip.Disappear();
        EffectToolTip.gameObject.SetActive(false);
    }

    public void ClearAllEvents()
    {
        effectsByEvent.Clear();
    }

    public void RemoveEffect(Effect effect)
    {
        foreach (var evt in effect.Events)
        {
            if (effectsByEvent.TryGetValue(evt, out var list))
            {
                list.Remove(effect);
                if (list.Count == 0)
                    effectsByEvent.Remove(evt);
            }
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
                effect.CardActionner = card;
                Debug.Log("Registering Card effect : " + effect);
                RegisterEffect(effect);
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
                    // Vérifie Hollow
                    bool canApply = (permanentView.permaTypes.Contains(PermaTypes.Hollow) && effect.HollowEffect)
                                || (!permanentView.permaTypes.Contains(PermaTypes.Hollow) && !effect.HollowEffect);
                    if (!canApply) continue;

                    effect.Actionner = permanentView.gameObject;

                    Debug.Log("Registering Perma effect : " + effect);
                    RegisterEffect(effect);
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
                    effect.Actionner = enemySlotView.gameObject;
                    Debug.Log("Adding in SetupAction Enemy instant effect on Setup : " + effect);
                    RegisterEffect(effect, false, true);
                }
                // En Revanche on peut enregistrer les effets non instant
                foreach (Effect effect in enemySlotView.PossibleIntent)
                {
                    effect.Actionner = enemySlotView.gameObject;
                    Debug.Log("Registering Enemy non instant effect on Setup : " + effect);
                    RegisterEffect(effect, true);
                }
            }
            // Si pas en SetupMode On fait tout d'un coup
            else
            {
                foreach (Effect effect in enemySlotView.PossibleIntent)
                {
                    effect.Actionner = enemySlotView.gameObject;
                    Debug.Log("Registering Enemy effect : " + effect);
                    RegisterEffect(effect);
                }
            }
        }
    }

    int Modulo(int value, int moduloBase)
    {
        if (moduloBase <= 0) return value;
        return ((value % moduloBase) + moduloBase) % moduloBase;
    }
    
    public void RegisterEffect(Effect effect, bool excludeInstant = false, bool useSetupActions = false)
    {
        if (effect == null) return;

        if (string.IsNullOrEmpty(effect.EffectID))
            effect.EffectID = System.Guid.NewGuid().ToString();

        int multiHit = effect.MultiHit;

        // S'il contient OnSelect → multiHit = 1
        if (effect.Events.Contains(Events.OnSelect))
            multiHit = 1;

        if (multiHit < 1)
            multiHit = 1;

        for (int hit = 0; hit < multiHit; hit++)
        {
            Effect clonedEffect = effect.Clone();

            while (clonedEffect != null)
            {
                foreach (var ev in clonedEffect.Events)
                {
                    // Init ActivateLeft si OnSelect
                    if (ev == Events.OnSelect)
                    {
                        clonedEffect.ActivateLeft = clonedEffect.ActivateNumber;
                    }

                    // INSTANT
                    if (ev == Events.Instant)
                    {
                        if (!excludeInstant)
                        {
                            if (useSetupActions)
                            {
                                CombatSystem.Instance.currentEnemy.SetupActions
                                    .Add(clonedEffect.GetGameAction());
                            }
                            else
                            {
                                ActionSystem.Instance.AddReaction(clonedEffect.GetGameAction());
                            }
                        }
                        else
                        {
                            clonedEffect.LinkedEffect = null;
                        }

                        // Instant ne va pas dans effectsByEvent
                        continue;
                    }

                    // NON-INSTANT
                    if (!useSetupActions && ev != Events.EnemyTurn)
                    {
                        Instance.AddEffectToEvent(ev, clonedEffect);
                    }
                }

                // LinkedEffect propagation
                if (clonedEffect.LinkedEffect != null)
                {
                    clonedEffect.LinkedEffect.ParentEffect = clonedEffect;
                    clonedEffect.LinkedEffect.Actionner = clonedEffect.Actionner;
                    clonedEffect.LinkedEffect.CardActionner = clonedEffect.CardActionner;
                }

                // OnSelect = ne suit pas la chaîne
                if (clonedEffect.Events.Contains(Events.OnSelect))
                    break;

                clonedEffect = clonedEffect.LinkedEffect;
            }

            // EffectGroup
            if (effect is EffectGroup effectGroup)
            {
                if (!effect.Events.Contains(Events.OnSelect))
                {
                    foreach (var grouped in effectGroup.EffectGroups)
                        RegisterEffect(grouped, excludeInstant, useSetupActions);
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
