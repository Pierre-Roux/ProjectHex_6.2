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
    [field: SerializeField] public int Rarity { get; private set; }
    [field: SerializeField] public int cost { get; private set; }
    [field: SerializeField] public int GridCost { get; private set; }
    [field: SerializeField] public bool PayX { get; private set; }
    [field: SerializeField] public Sprite SpriteImage { get; private set; }
    [field: SerializeField] public int Money_Cost { get; private set; } = 20;
    [field: SerializeReference, SR] public List<KeyWord> KeyWords = new List<KeyWord>();

    [field: Header("Permanent")]
    [field: SerializeField] public int life { get; private set; }
    [field: SerializeField] public int Durability { get; private set; }
    [field: SerializeField] public int MaxDurability { get; private set; } = 1;
    [field: SerializeField] public Sprite PermanentImage { get; private set; }
    [field: SerializeField] public PermanentArea permanentArea { get; private set; }

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
    [field: SerializeField] public EventReference CollateralSound; 
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
    [field: SerializeField] public EventReference CardSelectedSound;
    [field: SerializeField] public EventReference CardUnSelectedSound;

    public CardData Clone()
    {
        CardData clone = CreateInstance<CardData>();

        // Mandatory
        clone.Title = Title;
        clone.Description = Description;
        clone.Rarity = Rarity;
        clone.cost = cost;
        clone.GridCost = GridCost;
        clone.PayX = PayX;
        clone.SpriteImage = SpriteImage;
        clone.Money_Cost = Money_Cost;
        clone.KeyWords = new List<KeyWord>(KeyWords);

        // Permanent
        clone.life = life;
        clone.Durability = Durability;
        clone.MaxDurability = MaxDurability;
        clone.PermanentImage = PermanentImage;
        clone.permanentArea = permanentArea;

        // Spell
        clone.IsSpell = IsSpell;
        clone.Effects = new List<Effect>(Effects);

        // Audio
        clone.PlayCardSound = PlayCardSound;
        clone.CannotPlayCardSound = CannotPlayCardSound;
        clone.DiscardCardSound = DiscardCardSound;
        clone.DrawCardSound = DrawCardSound;
        clone.HoverCardSound = HoverCardSound;
        clone.PlaySpellSound = PlaySpellSound;
        clone.SummonPPermanentSound = SummonPPermanentSound;
        clone.DieSound = DieSound;
        clone.HollowDieSound = HollowDieSound;
        clone.BeingDamageSound = BeingDamageSound;
        clone.CollateralSound = CollateralSound;
        clone.BeingHealSound = BeingHealSound;
        clone.BeingShieldSound = BeingShieldSound;
        clone.LoseShieldSound = LoseShieldSound;
        clone.GainPowerSound = GainPowerSound;
        clone.LosePowerSound = LosePowerSound;
        clone.TakeLifeLossSound = TakeLifeLossSound;
        clone.BuffLifeSound = BuffLifeSound;
        clone.DebuffLifeSound = DebuffLifeSound;
        clone.ActivateSound = ActivateSound;
        clone.SelectedSound = SelectedSound;
        clone.UnSelectedSound = UnSelectedSound;
        clone.CardSelectedSound = CardSelectedSound;
        clone.CardUnSelectedSound = CardUnSelectedSound;

        return clone;
    }
}
