using SerializeReferenceEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using FMODUnity;

[System.Serializable]
public abstract class Effect
{
    [Header("Mandatory")]
    [SerializeField] public bool ActivateToolTip = true;
    [SerializeField] public int Priority = 0;
    [SerializeField] public ActionnerType actionnerType;
    [SerializeField] public List<EventInfo> EventInfos;
    [SerializeField] public CounterTypeInfo TypeOfCounter;
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
    [SerializeField] public EventInfo DurationType;
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
    [HideInInspector] public virtual bool CanBeDisableEffect => false;
    [HideInInspector] public GameObject Actionner;
    [HideInInspector] public Card CardActionner;
    [HideInInspector] public List<PermanentView> TargetForLinked_Player;
    [HideInInspector] public List<EnemySlotView> TargetForLinked_Enemy;
    [HideInInspector] public List<Card> TargetForLinked_Card;
    [HideInInspector] public Effect ParentEffect;
    [HideInInspector] public int PayXValue;
    [HideInInspector] public int ActivateLeft;


    [HideInInspector] public bool BypassEntryCondition = false;
    [HideInInspector] public bool Disabled = false;
    [HideInInspector] public string EffectID;

    protected Effect()
    {
    }

    public abstract GameAction GetGameAction();
    public abstract GameAction GetCounterMesure();
    public virtual Effect Clone()
    {
        return null;
    }
}
