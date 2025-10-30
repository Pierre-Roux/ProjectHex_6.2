using System;
using System.Collections.Generic;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

[CreateAssetMenu(menuName = ("Data/EnemyPermanent"))]
public class EnemyPermanentData : ScriptableObject
{
    [field: Header("Mandatory")]
    [field: SerializeField] public String Title;
    [field: SerializeField] public Sprite PermanentImage;
    [field: SerializeField] public int PermanentLife;
    [field: SerializeField] public bool IsCore;
    [field: SerializeField] public bool IsInvoc;
    [field: SerializeField] public bool UnShieldable;
    [field: SerializeField] public bool UnTargetable;
    [field: SerializeField] public PermanentArea permanentArea;
    [field: SerializeField] public int DecayCounter;

    [field: Header("Effects")]
    [field: SerializeReference, SR] public List<Effect> PossibleIntent { get; private set; }

    [field: Header("Sequence")]
    [field: SerializeField] public bool RDMSequence;
    [field: SerializeField] public List<string> IntentSequence { get; private set; }
    [field: SerializeField] public bool LoopingSequence;

    [field: Header("Audio")]
    [field: SerializeField] public EventReference SummonEPermanentSound;
    [field: SerializeField] public EventReference DieSound;
    [field: SerializeField] public EventReference BeingDamageSound;
    [field: SerializeField] public EventReference BeingHealSound;
    [field: SerializeField] public EventReference BeingShieldSound;
    [field: SerializeField] public EventReference LoseShieldSound;
    [field: SerializeField] public EventReference GainPowerSound;
    [field: SerializeField] public EventReference LosePowerSound;
    [field: SerializeField] public EventReference TakeLifeLossSound;
    [field: SerializeField] public EventReference BuffLifeSound;
    [field: SerializeField] public EventReference DebuffLifeSound;
    [field: SerializeField] public EventReference ActivateSound;
    [field: SerializeField] public EventReference SelectedSound;
    [field: SerializeField] public EventReference UnSelectedSound;
}
