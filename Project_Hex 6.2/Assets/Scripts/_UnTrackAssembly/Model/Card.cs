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
    public int cost { get; private set; }
    [HideInInspector] public int InitCost { get; set; }
    public int GridCost { get; private set; }
    [HideInInspector] public int BonusCost { get; set; }
    public bool PayX { get; private set; }
    public int PayXValue;
    public int Life { get; private set; }
    [HideInInspector] public int InitLife { get; set; }
    [HideInInspector] public int BonusLife { get; set; }
    public int Power { get; private set; }
    [HideInInspector] public int InitPower { get; set; }
    [HideInInspector] public int BonusPower { get; set; }
    public int Armor { get; private set; }
    [HideInInspector] public int InitArmor { get; set; }
    public int Durability { get; set; }
    [HideInInspector] public int InitDurability { get; set; }
    [HideInInspector] public int BonusStam { get; set; }
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
    public EventReference BeingDamageOnArmorSound;
    public EventReference ArmorBreakSound;
    public EventReference CollateralSound;
    public EventReference DieSound;
    public EventReference HollowDieSound;
    public EventReference BeingHealSound;
    public EventReference BeingArmorSound;
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

    [HideInInspector] private CombatSystem combatSystem;


    public Card(CardData cardData)
    {
        combatSystem = CombatSystem.Instance;
        data = cardData;
        InternCounters.ClearAll();
        KeyWords = new List<KeyWord>(data.KeyWords);
        Title = cardData.Title;
        Rarity = cardData.Rarity;
        InitCost = cost = cardData.cost;
        GridCost = cardData.GridCost;
        BonusCost = 0;
        PayX = cardData.PayX;
        IsSpell = cardData.IsSpell;
        Money_Cost = data.Money_Cost;
        if (!IsSpell)
        {
            InitLife = Life = cardData.Life;
            InitPower = Power = cardData.Power;
            InitArmor = Armor = cardData.Armor;
            InitDurability = Durability = cardData.Durability;
            MaxDurability = cardData.MaxDurability;
            KeyWord keyWord = new(KeyWordType.PermaCard, 0);
            KeyWords.Add(keyWord);
        }
        else
        {
            KeyWord keyWord = new(KeyWordType.SpellCard, 0);
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
        if (AudioManager.Instance.IsValid(cardData.BeingDamageOnArmorSound)) BeingDamageOnArmorSound = cardData.BeingDamageOnArmorSound;
        if (AudioManager.Instance.IsValid(cardData.ArmorBreakSound)) ArmorBreakSound = cardData.ArmorBreakSound;
        if (AudioManager.Instance.IsValid(cardData.CollateralSound)) CollateralSound = cardData.CollateralSound;

        if (AudioManager.Instance.IsValid(cardData.DieSound)) DieSound = cardData.DieSound;
        if (AudioManager.Instance.IsValid(cardData.HollowDieSound)) HollowDieSound = cardData.HollowDieSound;
        if (AudioManager.Instance.IsValid(cardData.BeingHealSound)) BeingHealSound = cardData.BeingHealSound;
        if (AudioManager.Instance.IsValid(cardData.BeingArmorSound)) BeingArmorSound = cardData.BeingArmorSound;
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

    public int CalculateBonusCost()
    {
        if (combatSystem == null) return 0;
        int FinalCost;
        int passiveBonus = combatSystem.GetPassive(BasicParam.Cost, Enemy_Player_ENUM.Card, this, null, null);

        FinalCost = BonusCost + passiveBonus;
        return FinalCost;
    }
    
    public int CalculateBonusMaxLife(Card card, PermanentView permanentView)
    {
        if (combatSystem == null) return 0;
        int passiveBonus = combatSystem.GetPassive(BasicParam.Life,Enemy_Player_ENUM.Player,card,permanentView,null);;
        int finalHP = BonusLife + passiveBonus;

        return finalHP;
    }

    public int CalculateBonusStam(Card card, PermanentView permanentView)
    {
        if (combatSystem == null) return 0;
        int passiveBonus = combatSystem.GetPassive(BasicParam.Durability,Enemy_Player_ENUM.Player,card,permanentView,null);;
        int finalStamina = BonusStam + passiveBonus;

        return finalStamina;
    }
    public int CalculateBonusPower(Card card, PermanentView permanentView)
    {
        if (combatSystem == null) return 0;
        int passiveBonus = combatSystem.GetPassive(BasicParam.Power,Enemy_Player_ENUM.Player,card,permanentView,null);
        int finalDMG = BonusPower + passiveBonus;
        
        return finalDMG;
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

        }
    }
    
    public void TakeAlterPower(int Amount)
    {
        if (IsSpell) return;
        BonusPower += Amount;
    }
}
