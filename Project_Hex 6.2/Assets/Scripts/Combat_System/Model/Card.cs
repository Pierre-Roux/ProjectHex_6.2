using System.Collections.Generic;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class Card
{
    public readonly CardData data;

    public string Title => data.Title;
    public string Description => data.Description;
    public Sprite Image => data.Image;
    public PermanentArea permanentArea => data.permanentArea;
    public bool UnShieldable => data.UnShieldable;
    public bool UnTargetable => data.UnTargetable;
    

    public bool IsSpell { get; private set; }
    public int cost { get; private set; }
    public int life { get; private set; }
    public int Shield { get; private set; }
    public int Durability { get; set; }
    public int DecayCounter { get; set; }
    public int MaxDurability { get; set; }
    public int Money_Cost { get; set; }
    public bool isInvoc;

    //Audio
    public EventReference PlayCardSound;
    public EventReference CannotPlayCardSound;
    public EventReference DiscardCardSound;
    public EventReference DrawCardSound;
    public EventReference HoverCardSound;
    public EventReference PlaySpellSound;
    public EventReference SummonPPermanentSound;
    public EventReference BeingDamageSound;
    public EventReference DieSound;
    public EventReference HollowDieSound;
    public EventReference BeingHealSound;
    public EventReference BeingShieldSound;
    public EventReference LoseShieldSound;
    public EventReference GainPowerSound;
    public EventReference LosePowerSound;
    public EventReference TakeLifeLossSound;
    public EventReference BuffLifeSound;
    public EventReference DebuffLifeSound;
    public EventReference ActivateSound;
    public EventReference SelectedSound;
    public EventReference UnSelectedSound;

    public List<Effect> Effects => data.Effects;


    public Card(CardData cardData)
    {
        data = cardData;
        cost = cardData.cost;
        IsSpell = cardData.IsSpell;
        Money_Cost = data.Money_Cost;
        isInvoc = data.isInvoc;
        if (!cardData.IsSpell)
        {
            life = cardData.life;
            Durability = cardData.Durability;
            DecayCounter = cardData.DecayCounter;
            MaxDurability = cardData.MaxDurability;
        }

        if (AudioManager.Instance.IsValid(cardData.PlayCardSound)) PlayCardSound = cardData.PlayCardSound;
        if (AudioManager.Instance.IsValid(cardData.CannotPlayCardSound)) CannotPlayCardSound = cardData.CannotPlayCardSound;
        if (AudioManager.Instance.IsValid(cardData.DiscardCardSound)) DiscardCardSound = cardData.DiscardCardSound;
        if (AudioManager.Instance.IsValid(cardData.DrawCardSound)) DrawCardSound = cardData.DrawCardSound;
        if (AudioManager.Instance.IsValid(cardData.HoverCardSound)) HoverCardSound = cardData.HoverCardSound;
        if (AudioManager.Instance.IsValid(cardData.PlaySpellSound)) PlaySpellSound = cardData.PlaySpellSound;
        if (AudioManager.Instance.IsValid(cardData.SummonPPermanentSound)) SummonPPermanentSound = cardData.SummonPPermanentSound;
        if (AudioManager.Instance.IsValid(cardData.BeingDamageSound)) BeingDamageSound = cardData.BeingDamageSound;
        
        if (AudioManager.Instance.IsValid(cardData.DieSound)) DieSound = cardData.DieSound;
        if (AudioManager.Instance.IsValid(cardData.HollowDieSound)) HollowDieSound = cardData.HollowDieSound;
        if (AudioManager.Instance.IsValid(cardData.BeingHealSound)) BeingHealSound = cardData.BeingHealSound;
        if (AudioManager.Instance.IsValid(cardData.BeingShieldSound)) BeingShieldSound = cardData.BeingShieldSound;
        if (AudioManager.Instance.IsValid(cardData.LoseShieldSound)) LoseShieldSound = cardData.LoseShieldSound;
        if (AudioManager.Instance.IsValid(cardData.GainPowerSound)) GainPowerSound = cardData.GainPowerSound;
        if (AudioManager.Instance.IsValid(cardData.LosePowerSound)) LosePowerSound = cardData.LosePowerSound;
        if (AudioManager.Instance.IsValid(cardData.TakeLifeLossSound)) TakeLifeLossSound = cardData.TakeLifeLossSound;
        if (AudioManager.Instance.IsValid(cardData.BuffLifeSound)) BuffLifeSound = cardData.BuffLifeSound;
        if (AudioManager.Instance.IsValid(cardData.DebuffLifeSound)) DebuffLifeSound = cardData.DebuffLifeSound;
        if (AudioManager.Instance.IsValid(cardData.ActivateSound)) ActivateSound = cardData.ActivateSound;
        if (AudioManager.Instance.IsValid(cardData.SelectedSound)) SelectedSound = cardData.SelectedSound;
        if (AudioManager.Instance.IsValid(cardData.UnSelectedSound)) UnSelectedSound = cardData.UnSelectedSound;
    }
}
