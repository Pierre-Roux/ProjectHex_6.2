using System.Collections.Generic;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Card")]

public class CardData : ScriptableObject
{
    [field: Header("Mandatory")]
    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public int cost { get; private set; }
    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeField] public int Money_Cost { get; private set; }

    [field: Header("Permanent")]
    [field: SerializeField] public int life { get; private set; }
    [field: SerializeField] public int DecayCounter { get; private set; }
    [field: SerializeField] public int Durability { get; private set; }
    [field: SerializeField] public int MaxDurability { get; private set; }
    [field: SerializeField] public Sprite PermanentImage { get; private set; }
    [field: SerializeField] public PermanentArea permanentArea { get; private set; }
    [field: SerializeField] public bool UnShieldable;
    [field: SerializeField] public bool UnTargetable;
    [field: SerializeField] public bool isInvoc;
    [field: SerializeField] public bool isArtillery;

    [field: Header("Spell")]
    [field: SerializeField] public bool IsSpell { get; private set; }
    [field: SerializeReference, SR] public List<Effect> Effects { get; private set; }

    [field: Header("Audio")]
    [field: SerializeField] public EventReference PlayCardSound;
    [field: SerializeField] public EventReference CannotPlayCardSound;
    [field: SerializeField] public EventReference DiscardCardSound;
    [field: SerializeField] public EventReference DrawCardSound;
    [field: SerializeField] public EventReference HoverCardSound;
    [field: SerializeField] public EventReference PlaySpellSound;
    [field: SerializeField] public EventReference SummonPPermanentSound;
    [field: SerializeField] public EventReference DieSound;
    [field: SerializeField] public EventReference HollowDieSound;
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
