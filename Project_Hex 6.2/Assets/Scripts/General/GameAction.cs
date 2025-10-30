using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[System.Serializable]
public abstract class GameAction
{
    [HideInInspector] public GameObject Actionner;
    [HideInInspector] public Card CardActionner;

    public List<GameAction> PreReactions { get; private set; } = new();
    public List<GameAction> PerformReactions { get; private set; } = new();
    public List<GameAction> PostReactions { get; private set; } = new();

    public EventReference SFX;
}
