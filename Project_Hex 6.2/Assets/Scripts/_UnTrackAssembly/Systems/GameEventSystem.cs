using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using SerializeReferenceEditor;
using Unity.VisualScripting;
using UnityEngine;

public class GameEventSystem : Singleton<GameEventSystem>
{
    [SerializeReference, SR] public List<Effect> PriorityList = new();
    [SerializeField] public EffectToolTip EffectToolTip;
    public Dictionary<EventInfo, List<Effect>> effectsByEvent = new();

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

    //PERFORMERS
    public void AddEffectToEvent(EventInfo ev, Effect effectToExecute)
    {
        foreach (var entry in effectsByEvent)
        {
            if (entry.Key.Events == ev.Events && entry.Key.Owner == ev.Owner && entry.Key.TestType == ev.TestType && entry.Key.BasicParam == ev.BasicParam)
            {
                entry.Value.Add(effectToExecute);
                return;
            }
        }

        // Aucune entrée trouvée
        effectsByEvent.Add(ev, new List<Effect> { effectToExecute });
    }

    public IEnumerator TriggerEvent(TriggerEventGA triggerEventGA)
    {
        bool EffectCanceled = false;
        List<Effect> OnSelectEffects = new List<Effect>();
        List<Effect> effectList = new List<Effect>();

        foreach (var DictionnaireEntry in effectsByEvent)
        {
            if (DictionnaireEntry.Key.Events == triggerEventGA.EventInfo.Events)
            {
                foreach (Effect effect in DictionnaireEntry.Value)
                {
                    effectList.Add(effect);
                }
            }
        }
        if (effectList.Count <= 0)
            yield break;

        List<Effect> Effects_Triggered = new List<Effect>();
        foreach (Effect Effect in effectList)
        {
            foreach (EventInfo EventInfo in Effect.EventInfos)
            {
                if (EventInfo.Events != triggerEventGA.EventInfo.Events) continue;

                // On vérifie que l'effet qui à été enregistré à soit un Owner = null (Donc ça passe) ou que l'owner est bien égal à celui de l'Event Triggered
                if (EventInfo.Owner != Enemy_Player_ENUM.NULL)
                {
                    if (EventInfo.Owner != triggerEventGA.EventInfo.Owner)
                    {
                        continue;
                    }
                }

                // OnTest pour les when Events si le type du trigger est le même que le type du permanent ou card qui trigger
                if (EventInfo.TestType != KeyWordType.NULL)
                {
                    if (triggerEventGA.Card != null)
                    {
                        if (triggerEventGA.Card.KeyWords.FirstOrDefault(k => k.keyWordType == EventInfo.TestType) == null)
                        {
                            continue;
                        }
                    }
                    else if (triggerEventGA.PermanentView != null)
                    {
                        if (triggerEventGA.PermanentView.KeyWords.FirstOrDefault(k => k.keyWordType == EventInfo.TestType) == null)
                        {
                            continue;
                        }
                    }
                    else if (triggerEventGA.EnemySlotView != null)
                    {
                        if (triggerEventGA.EnemySlotView.KeyWords.FirstOrDefault(k => k.keyWordType == EventInfo.TestType) == null)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                Effects_Triggered.Add(Effect);
            }
        }

        // Triage par ordre de priorité
        Effects_Triggered = Effects_Triggered.OrderBy(e => e.Priority).ToList();

        // Gestion des effets qui concerne les Counters globaux ou interne
        if (triggerEventGA.EventInfo.Events == Events.WhenInternCounter || triggerEventGA.EventInfo.Events == Events.WhenGlobalCounter)
        {
            CounterTypeInfo counterTypeInfo = triggerEventGA.CounterTypeInfo;
            if (counterTypeInfo.CounterType != CounterType.NULL)
            {
                List<Effect> matchingEffects = Effects_Triggered
                    .Where(e => e.TypeOfCounter == counterTypeInfo)
                    .ToList();

                if (matchingEffects.Count == 0)
                    yield break;

                CombatSystem combatSystem = CombatSystem.Instance;
                foreach (Effect effect in matchingEffects)
                {
                    if (CheckDisabledState(effect)) continue;

                    int CounterValue = 0;
                    if (triggerEventGA.EventInfo.Events == Events.WhenGlobalCounter)
                    {
                        // Si c'est par rapport à un counter global on prend la valeur du counter global 
                        CounterValue = combatSystem.GlobalCounters.Get(counterTypeInfo);
                    }
                    else
                    {
                        CounterModel counterManager = new();
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
                        CounterValue = counterManager.Get(counterTypeInfo);
                    }

                    //Debug.Log("Modulo ? : " + effect.ModuloValue + " CounterValue : " + CounterValue + " effectCounterValue" + effect.CounterValue);

                    if (effect.ModuloValue)
                    {
                        // On déclenche quand Modulo(globalCounter, effect.CounterValue) == 0
                        if (Modulo(CounterValue, effect.CounterValue) == 0)
                        {
                            if (effect is EffectGroup effectGroup)
                            {
                                foreach (Effect subEffect in effectGroup.EffectGroups)
                                {
                                    subEffect.Actionner = effectGroup.Actionner;
                                    subEffect.CardActionner = effectGroup.CardActionner;
                                    RegisterEffect(subEffect);
                                }
                            }
                            else
                            {
                                DoAction(effect);
                            }
                        }
                    }
                    else
                    {
                        if (effect is EffectGroup effectGroup)
                        {
                            foreach (Effect subEffect in effectGroup.EffectGroups)
                            {
                                subEffect.Actionner = effectGroup.Actionner;
                                subEffect.CardActionner = effectGroup.CardActionner;
                                RegisterEffect(subEffect);
                            }
                        }
                        else
                        {
                            DoAction(effect);
                        }
                    }
                }
                yield break;
            }
            yield break;
        }

        //Son et animation si OnSelect
        if (triggerEventGA.EventInfo.Events == Events.OnSelect)
        {
            if (triggerEventGA.PermanentView != null)
            {
                RuntimeManager.PlayOneShot(triggerEventGA.PermanentView.ActivateSound);
                triggerEventGA.PermanentView.GetComponent<Animator>().SetTrigger("Activate");
            }
            else if (triggerEventGA.EnemySlotView != null)
            {
                RuntimeManager.PlayOneShot(triggerEventGA.EnemySlotView.ActivateSound);
                triggerEventGA.EnemySlotView.GetComponent<Animator>().SetTrigger("Activate");
            }
        }

        // Gestion des effets Standards et globaux
        foreach (var effect in new List<Effect>(Effects_Triggered))
        {
            if (CheckDisabledState(effect)) continue;
            EffectCanceled = false;
            bool isActionnerMatch = false;
            PermanentView permanentView = null;
            EnemySlotView enemySlotView = null;
            Card cardActionner = null;
            // Cas 1 : Permanent
            if (triggerEventGA.PermanentView != null)
            {
                if (effect.Actionner != null)
                {
                    permanentView = effect.Actionner.GetComponent<PermanentView>();
                    isActionnerMatch = permanentView == triggerEventGA.PermanentView;
                }
            }
            // Cas 2 : Enemy
            else if (triggerEventGA.EnemySlotView != null)
            {
                if (effect.Actionner != null)
                {
                    enemySlotView = effect.Actionner.GetComponent<EnemySlotView>();
                    isActionnerMatch = enemySlotView == triggerEventGA.EnemySlotView;
                }
            }
            // Cas 3 : Card
            else if (triggerEventGA.Card != null)
            {
                if (effect.CardActionner != null)
                {
                    cardActionner = effect.CardActionner;
                    isActionnerMatch = cardActionner == triggerEventGA.Card;
                }
            }
            else
            {
                isActionnerMatch = true;
            }

            if (permanentView != null)
            {
                var HollowKeyword = permanentView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Hollow);
                bool canApply = (HollowKeyword != null && effect.HollowEffect)
                            || (HollowKeyword == null && !effect.HollowEffect);

                if (!canApply) EffectCanceled = true;
            }

            if (!EffectCanceled)
            {
                //Debug.Log("ActionnerMatch : " + isActionnerMatch);
                // Ajout des effets Onselect de l'entité pour post traitment
                if (triggerEventGA.EventInfo.Events == Events.OnSelect && isActionnerMatch)
                {
                    OnSelectEffects.Add(effect);
                }

                if (triggerEventGA.EventInfo.Events != Events.WhenPermaDie && triggerEventGA.EventInfo.Events != Events.OnSelect && isActionnerMatch)
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
                                if (effect is EffectGroup effectGroup)
                                {
                                    foreach (Effect subEffect in effectGroup.EffectGroups)
                                    {
                                        subEffect.Actionner = effectGroup.Actionner;
                                        subEffect.CardActionner = effectGroup.CardActionner;
                                        RegisterEffect(subEffect);
                                    }
                                }
                                else
                                {
                                    DoAction(effect);
                                }
                            }
                        }
                        else
                        {
                            if (effect is EffectGroup effectGroup)
                            {
                                foreach (Effect subEffect in effectGroup.EffectGroups)
                                {
                                    subEffect.Actionner = effectGroup.Actionner;
                                    subEffect.CardActionner = effectGroup.CardActionner;
                                    RegisterEffect(subEffect);
                                }
                            }
                            else
                            {
                                DoAction(effect);
                            }
                        }
                    }
                }

                if (!EffectCanceled)
                {
                    // Fonctionnement pour les Events Concernant d'autre déclancheur que eux même et les flags
                    if (triggerEventGA.EventInfo.Events == Events.WhenPermaDie && !isActionnerMatch
                    || triggerEventGA.EventInfo.Events == Events.WhenPermaExaust && !isActionnerMatch
                    || triggerEventGA.EventInfo.Events == Events.WhenPermaSac && !isActionnerMatch
                    || triggerEventGA.EventInfo.Events == Events.WhenPermaETB && !isActionnerMatch
                    || triggerEventGA.EventInfo.Events == Events.WhenDiscard && !isActionnerMatch
                    || triggerEventGA.EventInfo.Events == Events.WhenDraw && !isActionnerMatch
                    || triggerEventGA.EventInfo.Events == Events.WhenDiscard && !isActionnerMatch
                    || triggerEventGA.EventInfo.Events == Events.WhenPermaGainParam && !isActionnerMatch
                    || triggerEventGA.EventInfo.Events == Events.WhenPermaLoseParam && !isActionnerMatch
                    || triggerEventGA.EventInfo.Events == Events.WhenPermaChangeParam && !isActionnerMatch
                    || triggerEventGA.EventInfo.Events == Events.WhenPermaGainType && !isActionnerMatch
                    || triggerEventGA.EventInfo.Events == Events.WhenPermaLoseType && !isActionnerMatch
                    || triggerEventGA.EventInfo.Events == Events.WhenPermaLoseType && !isActionnerMatch
                    || triggerEventGA.EventInfo.Events == Events.WhenPlayType && !isActionnerMatch
                    )
                    {
                        PermanentView OriginPermanentView = null;
                        EnemySlotView OriginEnemySlotView = null;
                        Card OriginCard = null;
                        if (effect.DynamicConditionInfos.Count != 0)
                        {
                            if (effect.Actionner != null)
                            {
                                if (effect.Actionner.GetComponent<PermanentView>() != null)
                                {
                                    OriginPermanentView = effect.Actionner.GetComponent<PermanentView>();
                                }
                                else if (effect.Actionner.GetComponent<EnemySlotView>() != null)
                                {
                                    OriginEnemySlotView = effect.Actionner.GetComponent<EnemySlotView>();
                                }
                            }
                            else if (effect.CardActionner != null)
                            {
                                OriginCard = effect.CardActionner;
                            }

                            if (ConditionSystem.Instance.TestCondition(effect.DynamicConditionInfos, OriginCard, OriginPermanentView, OriginEnemySlotView, triggerEventGA.Card, triggerEventGA.PermanentView, triggerEventGA.EnemySlotView))
                            {
                                effect.BypassEntryCondition = true;
                                if (effect is EffectGroup effectGroup)
                                {
                                    foreach (Effect subEffect in effectGroup.EffectGroups)
                                    {
                                        subEffect.Actionner = effectGroup.Actionner;
                                        subEffect.CardActionner = effectGroup.CardActionner;
                                        RegisterEffect(subEffect);
                                    }
                                }
                                else
                                {
                                    DoAction(effect);
                                }
                            }
                        }
                        else
                        {
                            if (effect is EffectGroup effectGroup)
                            {
                                foreach (Effect subEffect in effectGroup.EffectGroups)
                                {
                                    subEffect.Actionner = effectGroup.Actionner;
                                    subEffect.CardActionner = effectGroup.CardActionner;
                                    RegisterEffect(subEffect);
                                }
                            }
                            else
                            {
                                DoAction(effect);
                            }
                        }
                    }
                }

            }
        }

        // Gestion des Effets OnSelectEvents
        if (triggerEventGA.EventInfo.Events == Events.OnSelect)
        {
            if (OnSelectEffects.Count == 1)
            {
                if (!CheckDisabledState(OnSelectEffects[0]))
                {
                    Effect effectToManage = OnSelectEffects[0];
                    if (effectToManage.ActivateLeft > 0)
                    {
                        effectToManage.ActivateLeft--;
                        //Debug.Log("Reduce ActivateLeft on : " + effectToManage + ", reste : " + effectToManage.ActivateLeft + " Activation");
                        if (effectToManage.EventInfos.Count == 1)
                        {
                            Effect effectToExecute = effectToManage.Clone();
                            if (effectToExecute is EffectGroup effectGroup)
                            {
                                foreach (Effect subEffect in effectGroup.EffectGroups)
                                {
                                    subEffect.Actionner = effectGroup.Actionner;
                                    subEffect.CardActionner = effectGroup.CardActionner;
                                    RegisterEffect(subEffect);
                                }
                            }
                            else
                            {
                                DoAction(effectToExecute);
                            }
                        }
                        /*else
                        {
                            Effect effectToExecute = effectToManage.Clone();
                            for (int i = 0; i < effectToExecute.EventInfos.Count; i++)
                            {
                                if (effectToExecute.EventInfos[i].Events == Events.OnSelect)
                                {
                                    effectToExecute.EventInfos.Remove(effectToExecute.EventInfos[i]);
                                }                                
                            }
                            RegisterEffect(effectToExecute);                                
                        }*/                                
                    }            
                }
            }
            else if (OnSelectEffects.Count > 1)
            {
                if (!CheckDisabledState(OnSelectEffects[0]))
                {
                    if (triggerEventGA.PermanentView != null || triggerEventGA.EnemySlotView != null)
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

        //Debug.Log("ToolTipEffect ->>> " + effect + " Actionner ->> " + effect.Actionner + " CardActionner -> " + effect.CardActionner);

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
                Title = EnemySlotView_Origin.PermanentData.Title;
                Description = "";
                image = EnemySlotView_Origin.spriteRenderer.sprite;
            }
        }
        else if (effect.CardActionner != null)
        {
            Card_Origin = effect.CardActionner;
            Title = Card_Origin.Title;
            Description = Card_Origin.Description;
            image = Card_Origin.SpriteImage;
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

    public bool CheckDisabledState(Effect effect)
    {
        bool Disabled = false;
        if (effect.Actionner != null)
        {
            if (effect.Actionner.GetComponent<PermanentView>() != null)
            {
                Disabled = effect.Actionner.GetComponent<PermanentView>().IsDisabled;
            }
            else if (effect.Actionner.GetComponent<EnemySlotView>() != null)
            {
                Disabled = effect.Actionner.GetComponent<EnemySlotView>().IsDisabled;
            }
        }

        if (effect.GetType() == typeof(EnableEffect))
        {
            return false;
        }

        return Disabled;
    }

    public void ClearAllEvents()
    {
        effectsByEvent.Clear();
    }

    public void RemoveEffect(Effect effect)
    {
        List<EventInfo> eventsToRemove = new();

        foreach (var entry in effectsByEvent)
        {
            entry.Value.Remove(effect);

            if (entry.Value.Count == 0)
            {
                eventsToRemove.Add(entry.Key);
            }
        }

        foreach (var eventInfo in eventsToRemove)
        {
            effectsByEvent.Remove(eventInfo);
        }
    }

    public void RemoveEffectsByActionner(GameObject actionnerToRemove)
    {
        var eventsToCleanUp = new List<EventInfo>();

        foreach (var eventEntry in effectsByEvent)
        {
            EventInfo eventInfo = eventEntry.Key;
            List<Effect> effectList = eventEntry.Value;

            for (int i = effectList.Count - 1; i >= 0; i--)
            {
                Effect effect = effectList[i];

                if (effect.Actionner == actionnerToRemove && effect.CancelOnDeath)
                {
                    effectList.RemoveAt(i);
                }
            }

            if (effectList.Count == 0)
            {
                eventsToCleanUp.Add(eventInfo);
            }
        }

        foreach (EventInfo eventInfo in eventsToCleanUp)
        {
            effectsByEvent.Remove(eventInfo);
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
            List<Effect> effectListCloned = new();
            foreach (var effect in card.Effects)
            {
                effectListCloned.Add(effect.Clone());
            }
            SetPriority(effectListCloned);

            List<Effect> effectList = effectListCloned.OrderBy(e => e.Priority).ToList();
            foreach (var effect in effectList)
            {
                effect.CardActionner = card;
                Debug.Log("Registering Card effect : " + effect + " With Priority of " + effect.Priority);
                RegisterEffect(effect);
            }
        }
        else if (permanentView != null)
        {
            List<Effect> effectListCloned = new();
            foreach (var effect in permanentView.CardReferenceArchive.Effects)
            {
                effectListCloned.Add(effect.Clone());
            }
            SetPriority(effectListCloned);

            //Pour le moment pas de setup pour le player
            if (SetupMode)
            {

            }
            else
            {
                List<Effect> effectList = permanentView.CardReferenceArchive.Effects.OrderBy(e => e.Priority).ToList();
                foreach (var effect in effectList)
                {
                    effect.Actionner = permanentView.gameObject;
                    Debug.Log("Registering Perma effect : " + effect + " With Priority of " + effect.Priority);
                    RegisterEffect(effect);
                }
            }
        }
        else if (enemySlotView != null)
        {
            List<Effect> effectListCloned = new();

            foreach (var effect in enemySlotView.PossibleIntent)
            {
                effectListCloned.Add(effect.Clone());
            }
            SetPriority(effectListCloned);

            // Si Setup on Manage en deux fois de façon que l'init dans CombatSystem passe avant l'ajout d'effet instant
            if (SetupMode)
            {
                // Ici on enregistre les effets dans SetupActions qui seront eux même register une fois que l'init sera terminé
                foreach (Effect effect in enemySlotView.PossibleIntent)
                {
                    effect.Actionner = enemySlotView.gameObject;
                    RegisterEffect(effect, false, true);
                }
                // En Revanche on peut enregistrer les effets non instant
                List<Effect> effectList = enemySlotView.PossibleIntent.OrderBy(e => e.Priority).ToList();
                foreach (Effect effect in effectList)
                {
                    effect.Actionner = enemySlotView.gameObject;
                    RegisterEffect(effect, true, false);
                }
            }
            // Si pas en SetupMode On fait tout d'un coup
            else
            {
                List<Effect> effectList = enemySlotView.PossibleIntent.OrderBy(e => e.Priority).ToList();
                foreach (Effect effect in effectList)
                {
                    effect.Actionner = enemySlotView.gameObject;
                    Debug.Log("Registering Enemy effect : " + effect + " With Priority of " + effect.Priority);
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

    public void SetPriority(List<Effect> EffectsToManage)
    {
        foreach (Effect effect in EffectsToManage)
        {
            Effect EffectToManage = effect;
            if (EffectToManage is EffectGroup effectGroup)
            {
                SetPriority(effectGroup.EffectGroups);
            }
            else
            {
                while (EffectToManage != null)
                {
                    if (EffectToManage.Priority == 0)
                    {
                        effect.Priority = GetPriorityByType(EffectToManage.GetType());
                    }
                    EffectToManage = EffectToManage.LinkedEffect; 
                }
            }
        }
    }
    
    public int GetPriorityByType(System.Type type)
    {
        Effect effectType = PriorityList.FirstOrDefault(e => e.GetType() == type);
        if (effectType != null)
        {
            return PriorityList.IndexOf(effectType) * 2;
        }
        else
        {
            return 0;
        }
    }

    public void RegisterEffect(Effect effect, bool excludeInstant = false, bool useSetupActions = false)
    {
        if (effect == null) return;

        if (string.IsNullOrEmpty(effect.EffectID))
            effect.EffectID = System.Guid.NewGuid().ToString();

        int multiHit = effect.MultiHit;

        // S'il contient OnSelect → multiHit = 1
        foreach (EventInfo item in effect.EventInfos)
        {
            if(item.Events == Events.OnSelect)
            {
                multiHit = 1;
            }
        }
        if (multiHit < 1)
            multiHit = 1;

        for (int hit = 0; hit < multiHit; hit++)
        {
            Effect clonedEffect = effect.Clone();

            while (clonedEffect != null)
            {
                foreach (var ev in clonedEffect.EventInfos)
                {
                    // Init ActivateLeft si OnSelect
                    if (ev.Events == Events.OnSelect)
                    {
                        clonedEffect.ActivateLeft = clonedEffect.ActivateNumber;
                    }

                    // INSTANT
                    if (ev.Events == Events.Instant)
                    {
                        if (!excludeInstant)
                        {
                            if (useSetupActions)
                            {
                                CombatSystem.Instance.currentEnemy.SetupEffects
                                    .Add(clonedEffect);
                            }
                            else
                            {
                                if (!CheckDisabledState(clonedEffect))
                                {
                                    if (effect is EffectGroup effectGroup)
                                    {
                                        foreach (Effect subEffect in effectGroup.EffectGroups)
                                        {
                                            subEffect.Actionner = effectGroup.Actionner;
                                            subEffect.CardActionner = effectGroup.CardActionner;
                                            RegisterEffect(subEffect);
                                        }
                                    }
                                    else
                                    {
                                        DoAction(clonedEffect);
                                    }  
                                }
                            }
                        }
                        else
                        {
                            clonedEffect.LinkedEffect = null;
                        }
                        continue;
                    }

                    // NON-INSTANT
                    if (!useSetupActions)
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
                foreach (EventInfo item in effect.EventInfos)
                {
                    if (item.Events == Events.OnSelect)
                    {
                        break;
                    }                    
                }

                clonedEffect = clonedEffect.LinkedEffect;
            }
        }
    }

    public void DoAction(Effect effect)
    {
        if (effect.Actionner != null)
        {
            PermanentView permanentView = effect.Actionner.GetComponent<PermanentView>();
            if (permanentView != null)
            {
                var HollowKeyword = permanentView.KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Hollow);
                bool canApply = (HollowKeyword != null && effect.HollowEffect) || (HollowKeyword == null && !effect.HollowEffect);
                if (!canApply) return;
            }
        }

        GameAction ga = effect.GetGameAction();
        if (ga != null)
        {
            ActionSystem.Instance.AddReaction(ga);
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
                if (triggerEventGA.EventInfo.Events == effect.DurationType.Events && triggerEventGA.EventInfo.Owner == effect.DurationType.Owner && triggerEventGA.EventInfo.TestType == effect.DurationType.TestType && triggerEventGA.EventInfo.BasicParam == effect.DurationType.BasicParam)
                {
                    if (effect.Duration >= 0)
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
        }

        foreach (var effect in effectsToRemove)
        {
            RemoveEffect(effect);
        }

        if (triggerEventGA.EventInfo.Events == Events.StartTurn)
        {
            CombatSystem.Instance.EndTurnBtnActivable = true;
        }
    }
}
