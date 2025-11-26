using SerializeReferenceEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using FMODUnity;

[System.Serializable]
public abstract class Effect
{
    [Header("Mandatory")]
    [SerializeField] public ActionnerType actionnerType;
    [SerializeField] public List<Events> Events;
    [SerializeField] public CounterType TypeOfCounter;
    [SerializeField] public int CounterValue;
    [SerializeField] public bool ModuloValue;
    [SerializeField] public bool HollowEffect;
    [SerializeField] public bool PayXEffect;
    [SerializeField] public int MultiHit;
    [SerializeField] public EventReference SFX;

    [Header("For Select Event")]
    [SerializeField] public int ActivateNumber = 1;
    [SerializeField] public bool ORChoice = false;

    [Header("Enemy_Only")]
    [SerializeField] public String Intent_Title;
    [SerializeField] public string number;

    [Header("On Delayed Events")]

    [SerializeField] public int Duration;
    [SerializeField] public Events DurationType;
    [SerializeField] public bool TriggerOnDurationEnd;
    [SerializeField] public bool CancelOnDeath = true;

    [Header("On Condition Effect")]
    [field: SerializeReference, SR] public List<DynamicConditionInfo> DynamicConditionInfos;

    [Header("Linked Effect")]
    [field: SerializeReference, SR] public Effect LinkedEffect;

    [HideInInspector] public virtual string EffectDescription => "";
    [HideInInspector] public virtual List<TargetLimitationInfo> EffectTargetLimitations => null;
    [HideInInspector] public virtual TargetModeInfo EffectTargetModeInfo => null;
    [HideInInspector] public virtual int EffectTargetNumber => 0;
    [HideInInspector] public virtual bool EffectTargetUpTo => true;
    [HideInInspector] public GameObject Actionner;
    [HideInInspector] public Card CardActionner;
    [HideInInspector] public List<PermanentView> TargetForLinked_Player;
    [HideInInspector] public List<EnemySlotView> TargetForLinked_Enemy;
    [HideInInspector] public List<Card> TargetForLinked_Card;
    [HideInInspector] public Effect ParentEffect;
    [HideInInspector] public int PayXValue;
    [HideInInspector] public int ActivateLeft;

    [HideInInspector] public bool BypassEntryCondition = false;
    [HideInInspector] public string EffectID;

    protected Effect()
    {
        // Génère un identifiant unique
        //if (string.IsNullOrEmpty(EffectID))
        //    EffectID = System.Guid.NewGuid().ToString();
    }
    public abstract GameAction GetGameAction();

    public virtual string GetParsedDescription()
    {
        string desc = EffectDescription;

        if (string.IsNullOrEmpty(desc))
            return "";

        // Dictionnaire de base pour les marqueurs communs
        Dictionary<string, string> replacements = new()
        {
            { "@Duration", Duration.ToString() },
            { "@Event", Events.ToString() }
        };

        foreach (var kvp in replacements)
            desc = desc.Replace(kvp.Key, kvp.Value);

        return desc;
    }

    public virtual Effect Clone()
    {
        return null;
    }
}
