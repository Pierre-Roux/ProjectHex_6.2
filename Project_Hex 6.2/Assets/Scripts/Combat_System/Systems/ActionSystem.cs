using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class ActionSystem : Singleton<ActionSystem>
{
    [SerializeField] private List<GameAction> reactions = null;

    public bool IsPerforming { get; private set; } = false;

    public static Dictionary<Type, List<Action<GameAction>>> preSubs = new();
    public static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();
    public static Dictionary<Type, List<Action<GameAction>>> postSubs = new();

    public void Perform(GameAction Action, System.Action OnperformFinished = null)
    {
        if (Action != null)
        {
            //Debug.Log("StartingFlowWith " + Action.GetType());
        }
        if (IsPerforming) return;
        IsPerforming = true;
        StartCoroutine(Flow(Action, () =>
        {
            IsPerforming = false;
            //OnperformFinished?.Invoke();
        }));
    }

    public void ResetActions()
    {
        reactions = null;
        preSubs.Clear();
        performers.Clear();
        postSubs.Clear();
    }

    public void AddReaction(GameAction gameAction)
    {
        reactions?.Add(gameAction);
    }

    public IEnumerator RunAction(GameAction action)
    {
        if (action == null) yield break;

        // On utilise une pile temporaire locale
        var savedReactions = reactions;
        reactions = new List<GameAction>();

        IsPerforming = true;
        yield return Flow(action, () =>
        {
            IsPerforming = false;
        });
        reactions = savedReactions;
    }

    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
    {
        // Vérification stricte de nullité
        if (action == null)
        {
            Debug.LogError("[Flow] action == null (référence C# nulle)");
            yield break;
        }

        reactions = action.PreReactions;
        PerformSubscribers(action, preSubs);
        yield return PerformReactions();

        if (AudioManager.Instance.IsValid(action.SFX))
        {
            RuntimeManager.PlayOneShot(action.SFX);
        }

        reactions = action.PerformReactions;
        yield return PerformPerformer(action);
        yield return PerformReactions();

        reactions = action.PostReactions;
        PerformSubscribers(action, postSubs);
        yield return PerformReactions();

        OnFlowFinished?.Invoke();
    }

    private IEnumerator PerformPerformer(GameAction action)
    {
        Type type = action.GetType();
        if (performers.ContainsKey(type))
        {
            yield return performers[type](action);
        }
    }

    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
    {
        Type type = action.GetType();
        if (!subs.ContainsKey(type))
        {
            return;
        }

        List<Action<GameAction>> actionList = subs[type];

        foreach (var sub in actionList)
        {
            sub(action);
        }
    }

    private IEnumerator PerformReactions()
    {
        foreach (var reaction in reactions)
        {
            yield return Flow(reaction);
        }
    }

    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    {
        Type type = typeof(T);
        IEnumerator wrappedPerformer(GameAction action) => performer((T)action);
        if (performers.ContainsKey(type)) performers[type] = wrappedPerformer;
        else performers.Add(type, wrappedPerformer);
    }

    public static void DetachPerformer<T>() where T : GameAction
    {
        Type type = typeof(T);
        if (performers.ContainsKey(type)) performers.Remove(type);
    }

    public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        //Debug.Log($"[Access] ActionSystem.Instance at {Time.time} from: {new System.Diagnostics.StackTrace().GetFrame(1).GetMethod()}");

        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        void wrappedReaction(GameAction action) => reaction((T)action);
        if (subs.ContainsKey(typeof(T)))
        {
            subs[typeof(T)].Add(wrappedReaction);
        }
        else
        {
            subs.Add(typeof(T), new());
            subs[typeof(T)].Add(wrappedReaction);
        }
    }
    public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        if (subs.ContainsKey(typeof(T)))
        {
            void wrappedReaction(GameAction action) => reaction((T)action);
            subs[typeof(T)].Remove(wrappedReaction);
        }
                  
    }
}
