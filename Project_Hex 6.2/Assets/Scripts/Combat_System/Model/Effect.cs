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
    [SerializeField] public Events Events;
    [SerializeField] public bool HollowEffect;
    [SerializeField] public int MultiHit;
    [SerializeField] public EventReference SFX;

    [Header("Enemy_Only")]
    [SerializeField] public String Intent_Title;
    [SerializeField] public string number;

    [Header("On Delayed Events")]

    [SerializeField] public int Duration;
    [SerializeField] public Events DurationType;
    [SerializeField] public bool TriggerOnDurationEnd;
    [SerializeField] public bool CancelOnDeath;

    [Header("On Condition Effect")]
    public DynamicCondition DynamicCondition;
    public int TestValue;
    public DynamicAmount TestDynamicAmount;
    public PermaTypes TestType;

    [field: SerializeReference, SR] public Effect LinkedEffect;


    [HideInInspector] public virtual List<TargetLimitationInfo> EffectTargetLimitations => null;
    [HideInInspector] public virtual TargetMode EffectTargetMode => TargetMode.Null;
    [HideInInspector] public virtual int EffectTargetNumber => 0;
    [HideInInspector] public virtual bool EffectTargetUpTo => true;
    [HideInInspector] public GameObject Actionner;
    [HideInInspector] public Card CardActionner;
    [HideInInspector] public List<PermanentView> TargetForLinked_Player;
    [HideInInspector] public List<EnemySlotView> TargetForLinked_Enemy;
    [HideInInspector] public List<Card> TargetForLinked_Card;
    [HideInInspector] public Effect ParentEffect;

    [HideInInspector] public bool BypassEntryCondition = false;
    [HideInInspector] public string EffectID;

    protected Effect()
    {
        // Génère un identifiant unique si non encore défini
        if (string.IsNullOrEmpty(EffectID))
            EffectID = System.Guid.NewGuid().ToString();
    }
    public abstract GameAction GetGameAction();

    public virtual Effect Clone()
    {
        return null;
    }
}
