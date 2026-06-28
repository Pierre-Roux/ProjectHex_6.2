using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class Card
{
    public readonly CardData data;

    public string Title;
    public string Description => data.Description;
    public Sprite SpriteImage => data.SpriteImage;
    public PermanentArea permanentArea => data.permanentArea;

    public int Rarity { get; private set; }
    public bool IsSpell { get; private set; }
    public int InitialCost { get; private set; }
    public int cost { get; private set; }
    public int GridCost { get; private set; }
    public int BonusCost { get; set; }
    public bool PayX { get; private set; }
    public int PayXValue;
    public int life { get; private set; }
    public int Shield { get; private set; }
    public int Durability { get; set; }
    public int MaxDurability { get; set; }
    public int Money_Cost { get; set; }
    public List<KeyWord> KeyWords = new List<KeyWord>();
    public bool isInvoc;
    public CardView RefCardView = null;

    //Audio
    public EventReference PlayCardSound;
    public EventReference CannotPlayCardSound;
    public EventReference DiscardCardSound;
    public EventReference DrawCardSound;
    public EventReference HoverCardSound;
    public EventReference PlaySpellSound;
    public EventReference SummonPPermanentSound;
    public EventReference BeingDamageSound;
    public EventReference CollateralSound;
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
    public EventReference CardSelectedSound;
    public EventReference CardUnSelectedSound;

    public List<Effect> Effects => data.Effects;
    [HideInInspector] public CounterModel InternCounters = new();


    public Card(CardData cardData)
    {
        data = cardData;
        InternCounters.ClearAll();
        KeyWords = new List<KeyWord>(data.KeyWords);
        Title = cardData.Title;
        Rarity = cardData.Rarity;
        InitialCost = cardData.cost;
        cost = InitialCost; //+ CalculatePassiveCost();
        GridCost = cardData.GridCost;
        BonusCost = 0;
        PayX = cardData.PayX;
        IsSpell = cardData.IsSpell;
        Money_Cost = data.Money_Cost;
        if (!cardData.IsSpell)
        {
            life = cardData.life;
            Durability = cardData.Durability;
            MaxDurability = cardData.MaxDurability;
        }

        if (IsSpell)
        {
            KeyWord keyWord = new(KeyWordType.SpellCard, 0);
            KeyWords.Add(keyWord);
        }
        else
        {
            KeyWord keyWord = new(KeyWordType.PermaCard, 0);
            KeyWords.Add(keyWord);
        }

        if (AudioManager.Instance.IsValid(cardData.PlayCardSound)) PlayCardSound = cardData.PlayCardSound;
        if (AudioManager.Instance.IsValid(cardData.CannotPlayCardSound)) CannotPlayCardSound = cardData.CannotPlayCardSound;
        if (AudioManager.Instance.IsValid(cardData.DiscardCardSound)) DiscardCardSound = cardData.DiscardCardSound;
        if (AudioManager.Instance.IsValid(cardData.DrawCardSound)) DrawCardSound = cardData.DrawCardSound;
        if (AudioManager.Instance.IsValid(cardData.HoverCardSound)) HoverCardSound = cardData.HoverCardSound;
        if (AudioManager.Instance.IsValid(cardData.PlaySpellSound)) PlaySpellSound = cardData.PlaySpellSound;
        if (AudioManager.Instance.IsValid(cardData.SummonPPermanentSound)) SummonPPermanentSound = cardData.SummonPPermanentSound;
        if (AudioManager.Instance.IsValid(cardData.BeingDamageSound)) BeingDamageSound = cardData.BeingDamageSound;
        if (AudioManager.Instance.IsValid(cardData.CollateralSound)) CollateralSound = cardData.CollateralSound;

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
        if (AudioManager.Instance.IsValid(cardData.CardSelectedSound)) CardSelectedSound = cardData.CardSelectedSound;
        if (AudioManager.Instance.IsValid(cardData.CardUnSelectedSound)) CardUnSelectedSound = cardData.CardUnSelectedSound;
    }

    public void TakeAlterCardCost(int Amount)
    {
        BonusCost += Amount;
        if (RefCardView != null)
        {
            RefCardView.UpdateCostText();
        }
    }

    public int CalculatePassiveCost()
    {
        if (CombatSystem.Instance != null)
        {
            int passiveBonus = 0; 
            /*foreach (var keyWord in KeyWords)
            {
                passiveBonus += CombatSystem.Instance.GetCost(keyWord.keyWordType, Enemy_Player_ENUM.NULL);
            }

            // Bonus globaux (NULL)
            passiveBonus += CombatSystem.Instance.GetCost(KeyWordType.NULL, Enemy_Player_ENUM.NULL);

            return passiveBonus;*/
            return 0;
        }
        else
        {
            return 0;
        }
    }

    public void UpdateCost(int passiveCost)
    {
        cost = InitialCost + passiveCost;

        if (RefCardView != null)
        {
            RefCardView.UpdateCostText();
        }
    }
    
    public void TakeAlterStamina(int Amount)
    {
        if (IsSpell) return;
        Durability += Amount;

        if (Durability >= MaxDurability)
        {
            Durability = MaxDurability;
        }

        if (Durability < 0)
        {
            Durability = 0;
        }

        if (RefCardView != null)
        {
            RefCardView.UpdateDurabilityText();
        }
    }
}
