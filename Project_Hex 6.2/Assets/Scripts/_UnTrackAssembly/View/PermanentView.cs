using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using FMODUnity;
using System.Linq;
using System.Collections;

public class PermanentView : MonoBehaviour
{
    [SerializeField] public SpriteRenderer PermanentSpriteRenderer;
    [SerializeField] public GameObject WrapperGM;
    [SerializeField] SpriteRenderer AuraSpriteRenderer;
    [SerializeField] TMP_Text HealthText;
    [SerializeField] TMP_Text PowerText;
    [SerializeField] TMP_Text ArmorText;
    [SerializeField] TMP_Text StaminaText;
    [SerializeField] public TMP_Text NameText;
    [SerializeField] public GameObject ShieldVisual;
    [HideInInspector] public bool UnShieldable;

    [SerializeField] public EventReference DieSound;
    [SerializeField] public EventReference HollowDieSound;
    [SerializeField] public EventReference CollateralSound;
    [SerializeField] public EventReference BeingDamageSound;
    [SerializeField] public EventReference BeingDamageOnArmorSound;
    [SerializeField] public EventReference ArmorBreakSound;
    [SerializeField] public EventReference BeingHealSound;
    [SerializeField] public EventReference BeingArmorSound;
    [SerializeField] public EventReference BeingShieldSound;
    [SerializeField] public EventReference LoseShieldSound;
    [SerializeField] public EventReference GainPowerSound;
    [SerializeField] public EventReference LosePowerSound;
    [SerializeField] public EventReference TakeLifeLossSound;
    [SerializeField] public EventReference BuffLifeSound;
    [SerializeField] public EventReference DebuffLifeSound;
    [SerializeField] public EventReference ActivateSound;
    [SerializeField] public EventReference SelectedSound;
    [SerializeField] public EventReference UnSelectedSound;

    [HideInInspector] public bool IsCore { get; set; }
    [HideInInspector] public int MaxLife { get; set; }
    [HideInInspector] public int currentLife { get; set; }
    [HideInInspector] public int currentPower { get; set; }
    [HideInInspector] public int currentArmor { get; set; }
    [HideInInspector] public int MaxDurability { get; set; }
    [HideInInspector] public int Durability { get; set; } 
    [HideInInspector] public int BaseMaxDurability { get; set; }
    [HideInInspector] public int CurrentHPBonus { get; set; }
    [HideInInspector] public Card CardReferenceArchive;
    [SerializeField] public bool IsDisabled = false;
    [HideInInspector] public bool IsDead = false;
    [HideInInspector] public Vector3 InitialPosition { get; set; }
    [HideInInspector] public PermanentArea permanentArea;

    [HideInInspector] public List<PermanentView> PlayerShielder;
    [HideInInspector] public List<EnemySlotView> EnemyShielder;
    [HideInInspector] public List<PermanentView> PlayerShielded;
    [HideInInspector] public List<EnemySlotView> EnemyShielded;
    [HideInInspector] public bool UnTargetable;
    [HideInInspector] public bool Shielded;

    [HideInInspector] public List<GameAction> AffectedGA = new List<GameAction>();
    [HideInInspector] public List<KeyWord> KeyWords = new List<KeyWord>();
    [HideInInspector] public CounterModel InternCounters = new();
    [HideInInspector] public List<Effect> ToggleableEffects = new();

    [HideInInspector] private CombatSystem combatSystem;

    public void Setup(Card cardReference)
    {
        combatSystem = CombatSystem.Instance;
        InternCounters.ClearAll();
        KeyWords = new List<KeyWord>(cardReference.KeyWords);
        IsCore = false;
        CardReferenceArchive = cardReference;

        PermanentSpriteRenderer.sprite = cardReference.data.PermanentImage;
        UpdatePower();
        currentLife = CardReferenceArchive.Life;
        UpdateMaxLife();
        currentLife = MaxLife;
        UpdateLifeText();
        currentArmor = CardReferenceArchive.Armor;
        UpdateArmorText();
        BaseMaxDurability = cardReference.MaxDurability;
        MaxDurability = cardReference.MaxDurability;
        Durability = cardReference.Durability;
        UpdateMaxStam();
        permanentArea = cardReference.data.permanentArea;
        deactivateAuraVisual();
        UpdateNameText(cardReference.Title);

        // On cache les Text param inutiles
        bool HasPower = false;

        foreach (Effect effect in CardReferenceArchive.Effects)
        {
            if (ContainsPowerBasedDamage(effect))
            {
                HasPower = true;
                break;
            }
        }
        if (HasPower)
        {
            PowerText.gameObject.SetActive(true);
        }
        else
        {
            PowerText.gameObject.SetActive(false);
            HealthText.transform.localPosition = new Vector3(0, -0.5f, 0);
        }
        if (currentArmor > 0)
        {
            ArmorText.gameObject.SetActive(true);
        }
        else
        {
            ArmorText.gameObject.SetActive(false);
        }



        var WardKeyword = KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Ward);
        if (WardKeyword != null)
        {
            KeyWord keyWord = new(KeyWordType.UnShieldable, 0);
            KeyWords.Add(keyWord);
            UnShieldable = true;
        }

        var UnTargetableKeyword = KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnTargetable);
        if (UnTargetableKeyword != null)
        {
            UnTargetable = true;
        }

        var CoreKeyword = KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Core);
        if (CoreKeyword != null)
        {
            IsCore = true;
        }


        // Gère les Changement de Counter // Hollow géré par UpdateMaxStam
        TriggerEventGA triggerEventGA = null;
        var InvocKeyword = KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Invoc);
        if (InvocKeyword != null)
        {
            EventInfo eventInfo = new EventInfo(Events.TypeCountChanged, Enemy_Player_ENUM.Player, KeyWordType.Invoc);
            triggerEventGA = new(eventInfo, null, null, null, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }
        var EffigyKeyword = KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Effigy);
        if (EffigyKeyword != null)
        {
            EventInfo eventInfo = new EventInfo(Events.TypeCountChanged, Enemy_Player_ENUM.Player, KeyWordType.Effigy);
            triggerEventGA = new(eventInfo, null, null, null, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }
        var DecayKeyword = KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Decay);
        if (DecayKeyword != null && DecayKeyword.keyWordValue > 0)
        {
            EventInfo eventInfo = new EventInfo(Events.TypeCountChanged, Enemy_Player_ENUM.Player, KeyWordType.Decay);
            triggerEventGA = new(eventInfo, null, null, null, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }

        ShieldVisual.SetActive(false);

        //Audio
        if (AudioManager.Instance.IsValid(cardReference.DieSound)) DieSound = cardReference.DieSound;
        if (AudioManager.Instance.IsValid(cardReference.HollowDieSound)) HollowDieSound = cardReference.HollowDieSound;
        if (AudioManager.Instance.IsValid(cardReference.BeingDamageSound)) BeingDamageSound = cardReference.BeingDamageSound;
        if (AudioManager.Instance.IsValid(cardReference.BeingDamageOnArmorSound)) BeingDamageOnArmorSound = cardReference.BeingDamageOnArmorSound;
        if (AudioManager.Instance.IsValid(cardReference.ArmorBreakSound)) ArmorBreakSound = cardReference.ArmorBreakSound;
        if (AudioManager.Instance.IsValid(cardReference.CollateralSound)) CollateralSound = cardReference.CollateralSound;
        if (AudioManager.Instance.IsValid(cardReference.BeingHealSound)) BeingHealSound = cardReference.BeingHealSound;
        if (AudioManager.Instance.IsValid(cardReference.BeingArmorSound)) BeingArmorSound = cardReference.BeingArmorSound;
        if (AudioManager.Instance.IsValid(cardReference.BeingShieldSound)) BeingShieldSound = cardReference.BeingShieldSound;
        if (AudioManager.Instance.IsValid(cardReference.LoseShieldSound)) LoseShieldSound = cardReference.LoseShieldSound;
        if (AudioManager.Instance.IsValid(cardReference.GainPowerSound)) GainPowerSound = cardReference.GainPowerSound;
        if (AudioManager.Instance.IsValid(cardReference.LosePowerSound)) LosePowerSound = cardReference.LosePowerSound;
        if (AudioManager.Instance.IsValid(cardReference.TakeLifeLossSound)) TakeLifeLossSound = cardReference.TakeLifeLossSound;
        if (AudioManager.Instance.IsValid(cardReference.BuffLifeSound)) BuffLifeSound = cardReference.BuffLifeSound;
        if (AudioManager.Instance.IsValid(cardReference.DebuffLifeSound)) DebuffLifeSound = cardReference.DebuffLifeSound;
        if (AudioManager.Instance.IsValid(cardReference.ActivateSound)) ActivateSound = cardReference.ActivateSound;
        if (AudioManager.Instance.IsValid(cardReference.SelectedSound)) SelectedSound = cardReference.SelectedSound;
        if (AudioManager.Instance.IsValid(cardReference.UnSelectedSound)) UnSelectedSound = cardReference.UnSelectedSound;
    }

    private bool ContainsPowerBasedDamage(Effect effect)
    {
        if (effect is DealDamageEffect dealDamageEffect)
        {
            return dealDamageEffect.powerBased;
        }

        if (effect is EffectGroup effectGroup)
        {
            foreach (Effect childEffect in effectGroup.EffectGroups)
            {
                if (ContainsPowerBasedDamage(childEffect))
                    return true;
            }
        }

        if (effect is ChoiceEffect choiceEffect)
        {
            foreach (Effect childEffect in choiceEffect.EffectsForPlayerChoice)
            {
                if (ContainsPowerBasedDamage(childEffect))
                    return true;
            }
        }

        return false;
    }

    public void SetPosition(Vector3 pos)
    {
        InitialPosition = pos;
    }

    private void OnEnable()
    {
        if (combatSystem != null)
        {
            combatSystem.PassivesChanged += UpdatePower;
            combatSystem.PassivesChanged += UpdateMaxLife;
            combatSystem.PassivesChanged += UpdateMaxStam;
        }  
    }

    private void OnDisable()
    {
        if (combatSystem != null)
        {
            combatSystem.PassivesChanged -= UpdatePower;
            combatSystem.PassivesChanged -= UpdateMaxLife;
            combatSystem.PassivesChanged -= UpdateMaxStam;
        }
    }

    public void UpdateLifeText()
    {
        HealthText.text = Mathf.Max(0,currentLife).ToString();
    }
    public void UpdatePowerText()
    {
        PowerText.text = Mathf.Max(0,currentPower).ToString();
    }
    public void UpdateArmorText()
    {
        ArmorText.text = Mathf.Max(0, currentArmor).ToString();
        if (currentArmor > 0)
        {
            ArmorText.gameObject.SetActive(true);
        }
        else
        {
            ArmorText.gameObject.SetActive(false);
        }
    }

    public void UpdateNameText(string name)
    {
        NameText.text = name;
    }

    public void UpdateHollowVisual()
    {
        var HollowKeyword = KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Hollow);
        if (HollowKeyword != null)
        {
            Color c = PermanentSpriteRenderer.color;
            c.a = 0.3f;
            PermanentSpriteRenderer.color = c;
        }
        else
        {
            Color c = PermanentSpriteRenderer.color;
            c.a = 1f;
            PermanentSpriteRenderer.color = c;
        }
    }
    
    public void ActivateAuraVisual()
    {
        AuraSpriteRenderer.gameObject.SetActive(true);
    }

    public void deactivateAuraVisual()
    {
        AuraSpriteRenderer.gameObject.SetActive(false);
    }

    public void ChangeHollowState(bool IsHollow)
    {
        TriggerEventGA triggerEventGA = null;
        if (IsHollow)
        {
            var HollowKeyword = KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Hollow);
            if (HollowKeyword == null)
            {
                KeyWord NewKeyWord = new(KeyWordType.Hollow,0);
                KeyWords.Add(NewKeyWord);

                UpdateHollowVisual();

                EventInfo eventInfo = new EventInfo(Events.WhenPermaGainType, Enemy_Player_ENUM.Player, KeyWordType.Hollow);
                triggerEventGA = new(eventInfo, null, null, this, null);
                ActionSystem.Instance.AddReaction(triggerEventGA);
                
                eventInfo = new EventInfo(Events.TypeCountChanged, Enemy_Player_ENUM.Player, KeyWordType.Hollow);
                triggerEventGA = new(eventInfo, null, null, null, null);
                ActionSystem.Instance.AddReaction(triggerEventGA);
            }
        }
        else
        {
            var HollowKeyword = KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Hollow);
            if (HollowKeyword != null)
            {
                KeyWords.Remove(HollowKeyword);
                UpdateHollowVisual();

                EventInfo eventInfo = new EventInfo(Events.WhenPermaLoseType, Enemy_Player_ENUM.Player, KeyWordType.Hollow);
                triggerEventGA = new(eventInfo, null, null, this, null);
                ActionSystem.Instance.AddReaction(triggerEventGA);

                eventInfo = new EventInfo(Events.TypeCountChanged, Enemy_Player_ENUM.Player, KeyWordType.Hollow);
                triggerEventGA = new(eventInfo, null, null, null, null);
                ActionSystem.Instance.AddReaction(triggerEventGA);
            }
        }

    }

    private void UpdatePower()
    {
        int oldPower = currentPower;
        if (CardReferenceArchive == null) return;
        int PowerBonus = CardReferenceArchive.CalculateBonusPower(null, this);
        currentPower = CardReferenceArchive.Power + PowerBonus;

        if (currentPower > oldPower)
        {
            EventInfo eventInfo = new EventInfo(Events.WhenPermaGainParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Power);
            TriggerEventGA triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Power);
            triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }
        else if (oldPower > currentPower)
        {
            EventInfo eventInfo = new EventInfo(Events.WhenPermaLoseParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Power);
            TriggerEventGA triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Power);
            triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);            
        }  

        UpdatePowerText(); 
    }
    
    public void UpdateMaxLife()
    {
        if (CardReferenceArchive == null) return;
        int oldMaxLife = MaxLife;
        int passiveBonus = CardReferenceArchive.CalculateBonusMaxLife(null,this);
        MaxLife = CardReferenceArchive.Life + passiveBonus;

        if (MaxLife <= 0)
        {
            MaxLife = 0;
        }

        if (currentLife > MaxLife)
        {
            currentLife = MaxLife;
        }
        else
        {
            if (currentLife + passiveBonus > MaxLife)
            {
                currentLife = MaxLife;
            }
            else
            {
                currentLife = currentLife + passiveBonus;
            }
        }

        if (currentLife < 0)
        {
            currentLife = 0;
        }
        else if (currentLife > MaxLife)
        {
            currentLife = MaxLife;
        }

        if (currentLife <= 0)
        {
            DiePermanentGA diePermanentGA = new(IsCore, Durability, CardReferenceArchive, this);
            ActionSystem.Instance.AddReaction(diePermanentGA);
            IsDead = true;
        }

        if (MaxLife > oldMaxLife)
        {
            EventInfo eventInfo = new EventInfo(Events.WhenPermaGainParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Life);
            TriggerEventGA triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Life);
            triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }
        else if (oldMaxLife > MaxLife)
        {
            EventInfo eventInfo = new EventInfo(Events.WhenPermaLoseParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Life);
            TriggerEventGA triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Life);
            triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);            
        }  

        UpdateLifeText();
    }
    
    public void UpdateMaxStam()
    {
        if (IsCore) return;
        if (CardReferenceArchive == null) return;
        int OldMaxDurability = MaxDurability;
        int passiveBonus = CardReferenceArchive.CalculateBonusStam(null,this);
        MaxDurability = BaseMaxDurability + passiveBonus;

        if (Durability > MaxDurability)
        {
            Durability = MaxDurability;
        }
        else
        {
            if (Durability + passiveBonus > MaxDurability)
            {
                Durability = MaxDurability;
            }
            else
            {
                Durability = Durability + passiveBonus;
            }
        }

        if (MaxDurability <= 0)
        {
            MaxDurability = 1;
        }

        if (Durability < 0)
        {
            Durability = 0;
        }
        else if (Durability > MaxDurability)
        {
            Durability = MaxDurability;
        }

        var HollowKeyword = KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Hollow);
        if (HollowKeyword == null && Durability == 0)
        {
            ChangeHollowState(true);
        }
        else if (HollowKeyword != null && Durability != 0)
        {
            ChangeHollowState(false);
        }

        if (MaxDurability > OldMaxDurability)
        {
            EventInfo eventInfo = new EventInfo(Events.WhenPermaGainParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Durability);
            TriggerEventGA triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Durability);
            triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }
        else if (OldMaxDurability > MaxDurability)
        {
            EventInfo eventInfo = new EventInfo(Events.WhenPermaLoseParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Durability);
            TriggerEventGA triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Durability);
            triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);            
        }         
    }

    public void TakeDamage(int Amount, Card CardActionner = null, GameObject Actionner = null)
    {
        if (Amount <= 0) return;

        PermanentView Pstriker = null;
        EnemySlotView Estriker = null;
        Card Cstriker = null;
        TriggerEventGA triggerEventGA;
        EventInfo eventInfo = new EventInfo();

        if (Actionner != null)
        {
            if (Actionner.GetComponent<PermanentView>() != null)
            {
                Pstriker = Actionner.GetComponent<PermanentView>();
            }
            else if (Actionner.GetComponent<EnemySlotView>() != null)
            {
                Estriker = Actionner.GetComponent<EnemySlotView>();
            }
        }
        else if (CardActionner != null)
        {
            Cstriker = CardActionner;
        }

        if (!IsDead)
        {
            transform.DOShakePosition(0.2f, 0.5f);

            eventInfo = new EventInfo(Events.OnSelfDamaged, Enemy_Player_ENUM.NULL, KeyWordType.NULL,BasicParam.NULL);
            triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }

        if (currentArmor > 0)
        {
            int DamageAmountToLife = 0;
            if (currentArmor >= Amount)
            {
                currentArmor -= Amount;
                RuntimeManager.PlayOneShot(BeingDamageOnArmorSound);
            }
            else
            {
                RuntimeManager.PlayOneShot(ArmorBreakSound);
                DamageAmountToLife = Amount - currentArmor;
                currentArmor = 0;
            }

            eventInfo = new EventInfo(Events.WhenPermaLoseParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Armor);
            triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Armor);
            triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            Amount = DamageAmountToLife;
        }

        if (Amount > 0)
        {
            eventInfo = new EventInfo(Events.WhenPermaLoseParam, Enemy_Player_ENUM.Player, KeyWordType.NULL,BasicParam.Life);
            triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Life);
            triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }

        currentLife -= Amount;
        if (currentLife <= 0)
        {
            if (!IsDead)
            {
                RuntimeManager.PlayOneShot(BeingDamageSound);
                DiePermanentGA diePermanentGA = new(IsCore, Durability, CardReferenceArchive, this);
                ActionSystem.Instance.AddReaction(diePermanentGA);
                
                if (Pstriker != null)
                {
                    if (Pstriker.KeyWords.Any(k => k.keyWordType == KeyWordType.Collateral) && KeyWords.Any(k => k.keyWordType == KeyWordType.Ward))
                    {
                        CollateralTrigger(-currentLife, Pstriker, Estriker, Cstriker);
                        currentLife = 0;
                    }
                    OnKillTrigger(Pstriker, Estriker, Cstriker);
                }
                else if (Estriker != null)
                {
                    if (Estriker.KeyWords.Any(k => k.keyWordType == KeyWordType.Collateral) && KeyWords.Any(k => k.keyWordType == KeyWordType.Ward))
                    {
                        CollateralTrigger(-currentLife, Pstriker, Estriker, Cstriker);
                        currentLife = 0;
                    }
                    OnKillTrigger(Pstriker, Estriker, Cstriker);
                }
                else if (Cstriker != null)
                {
                    if (Cstriker.KeyWords.Any(k => k.keyWordType == KeyWordType.Collateral) && KeyWords.Any(k => k.keyWordType == KeyWordType.Ward))
                    {
                        CollateralTrigger(-currentLife, Pstriker, Estriker, Cstriker);
                        currentLife = 0;
                    }

                    OnKillTrigger(Pstriker, Estriker, Cstriker);
                }
                IsDead = true;
            }
        }
        else
        {
            RuntimeManager.PlayOneShot(BeingDamageSound);
        }

        UpdateLifeText();
        UpdateArmorText();
    }

    public void OnKillTrigger(PermanentView Pstriker, EnemySlotView Estriker, Card Cstriker)
    {
        EventInfo eventInfo = new EventInfo(Events.OnSelfKill, Enemy_Player_ENUM.NULL, KeyWordType.NULL,BasicParam.NULL);
        TriggerEventGA triggerEventGA = new(eventInfo, null, Cstriker, Pstriker, Estriker);
        ActionSystem.Instance.AddReaction(triggerEventGA);
    }

    public void CollateralTrigger(int CollateralAmount, PermanentView Pstriker, EnemySlotView Estriker, Card Cstriker)
    {
        if (CollateralAmount == 0) return;
        if (IsCore) return;
        if (Pstriker != null)
        {
            List<PermanentView> targets_Player = new List<PermanentView>{};
            foreach (PermanentView item in PlayerShielded)
            {
                targets_Player.Add(item);
            }
            List<EnemySlotView> targets_Enemy = new List<EnemySlotView>{};
            foreach (EnemySlotView item in EnemyShielded)
            {
                targets_Enemy.Add(item);
            }

            CounterTypeInfo counterTypeInfo = new();
            DynamicAmountInfo dynamicAmountInfo = new(DynamicAmount.NULL,Enemy_Player_ENUM.NULL,KeyWordType.NULL,counterTypeInfo,BasicParam.NULL,false,CardLocation.NULL);
            DealDamageGA dealDamageGA = new(false, CollateralAmount, 1, dynamicAmountInfo, targets_Player, targets_Enemy);
            dealDamageGA.Actionner = Pstriker.gameObject;
            dealDamageGA.SourceEffect = new DealDamageEffect();
            dealDamageGA.powerBased = false;
            dealDamageGA.ActivateToolTip = false;
            dealDamageGA.SFX = !AudioManager.Instance.IsValid(CollateralSound) ? AudioManager.Instance.CollateralSound : CollateralSound;
            ActionSystem.Instance.AddReaction(dealDamageGA);
        }
        else if (Estriker != null)
        {
            List<PermanentView> targets_Player = new List<PermanentView>{};
            foreach (PermanentView item in PlayerShielded)
            {
                targets_Player.Add(item);
            }
            List<EnemySlotView> targets_Enemy = new List<EnemySlotView>{};
            foreach (EnemySlotView item in EnemyShielded)
            {
                targets_Enemy.Add(item);
            }

            CounterTypeInfo counterTypeInfo = new();
            DynamicAmountInfo dynamicAmountInfo = new(DynamicAmount.NULL,Enemy_Player_ENUM.NULL,KeyWordType.NULL,counterTypeInfo,BasicParam.NULL,false,CardLocation.NULL);
            DealDamageGA dealDamageGA = new(false, CollateralAmount, 1, dynamicAmountInfo, targets_Player, targets_Enemy);
            dealDamageGA.Actionner = Estriker.gameObject;
            dealDamageGA.SourceEffect = new DealDamageEffect();
            dealDamageGA.powerBased = false;
            dealDamageGA.ActivateToolTip = false;
            dealDamageGA.SFX = !AudioManager.Instance.IsValid(CollateralSound) ? AudioManager.Instance.CollateralSound : CollateralSound;
            ActionSystem.Instance.AddReaction(dealDamageGA);
        }
        else if (Cstriker != null)
        {
            List<PermanentView> targets_Player = new List<PermanentView>{};
            foreach (PermanentView item in PlayerShielded)
            {
                targets_Player.Add(item);
            }
            List<EnemySlotView> targets_Enemy = new List<EnemySlotView>{};
            foreach (EnemySlotView item in EnemyShielded)
            {
                targets_Enemy.Add(item);
            }

            CounterTypeInfo counterTypeInfo = new();
            DynamicAmountInfo dynamicAmountInfo = new(DynamicAmount.NULL,Enemy_Player_ENUM.NULL,KeyWordType.NULL,counterTypeInfo,BasicParam.NULL,false,CardLocation.NULL);
            DealDamageGA dealDamageGA = new(false, CollateralAmount, 1, dynamicAmountInfo, targets_Player, targets_Enemy);
            dealDamageGA.CardActionner = Cstriker;
            dealDamageGA.SourceEffect = new DealDamageEffect();
            dealDamageGA.powerBased = false;
            dealDamageGA.ActivateToolTip = false;
            dealDamageGA.SFX = !AudioManager.Instance.IsValid(CollateralSound) ? AudioManager.Instance.CollateralSound : CollateralSound;
            ActionSystem.Instance.AddReaction(dealDamageGA);
        }
    }

    public void TakeHeal(int Amount)
    {
        currentLife += Amount;
        if (currentLife > MaxLife)
        {
            currentLife = MaxLife;
        }

        EventInfo eventInfo = new EventInfo(Events.WhenPermaGainParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Life);
        TriggerEventGA triggerEventGA = new(eventInfo, null, null, this, null);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Life);
        triggerEventGA = new(eventInfo, null, null, this, null);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        RuntimeManager.PlayOneShot(BeingHealSound);
        transform.DOShakePosition(0.1f, 0.1f);
        UpdateLifeText();
    }

    public void TakeArmor(int Amount)
    {
        currentArmor += Amount;
        if (currentArmor < 0)
        {
            currentArmor = 0;
        }

        EventInfo eventInfo = new EventInfo(Events.WhenPermaGainParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Armor);
        TriggerEventGA triggerEventGA = new(eventInfo, null, null, this, null);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Armor);
        triggerEventGA = new(eventInfo, null, null, this, null);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        RuntimeManager.PlayOneShot(BeingArmorSound);
        transform.DOShakePosition(0.1f, 0.1f);
        UpdateArmorText();
    }

    public void TakeShield(PermanentView playerShielder = null, EnemySlotView enemyShielder = null)
    {
        if (!UnShieldable)
        {
            RuntimeManager.PlayOneShot(BeingShieldSound);
            if (playerShielder != null)
            {
                if (!PlayerShielder.Contains(playerShielder))
                {
                    PlayerShielder.Add(playerShielder);
                    playerShielder.GetComponent<PermanentView>().PlayerShielded.Add(this);
                }
            }

            if (enemyShielder != null)
            {
                if (!EnemyShielder.Contains(enemyShielder))
                {
                    EnemyShielder.Add(enemyShielder);
                    enemyShielder.GetComponent<EnemySlotView>().PlayerShielded.Add(this);
                }
            }
            UpdateShield();
        }
    }

    public void RemoveShield(PermanentView playerShielder = null, EnemySlotView enemyShielder = null)
    {
        if (playerShielder != null)
        {
            PlayerShielder.Remove(playerShielder);
        }
        if (enemyShielder != null)
        {
            EnemyShielder.Remove(enemyShielder);
        }
        UpdateShield();        
    }

    public void UnShield(PermanentView playerShielder = null, EnemySlotView enemyShielder = null)
    {
        if (playerShielder != null)
        {
            playerShielder.GetComponent<PermanentView>().PlayerShielded.Remove(this);
            PlayerShielder.Remove(playerShielder);
        }
        if (enemyShielder != null)
        {
            enemyShielder.GetComponent<EnemySlotView>().PlayerShielded.Remove(this);
            EnemyShielder.Remove(enemyShielder);
        }
        UpdateShield();
    }

    public void UpdateShield()
    {
        if (PlayerShielder.Count != 0 || EnemyShielder.Count != 0)
        {
            ShieldVisual.SetActive(true);
            Shielded = true;
        }
        else
        {
            RuntimeManager.PlayOneShot(LoseShieldSound);
            ShieldVisual.SetActive(false);
            Shielded = false;
        }
    }

    public void TakeAlterPower(AlterPowerGA Ga)
    {
        if (IsDead) return;

        if (Ga.Amount > 0)
        {
            RuntimeManager.PlayOneShot(GainPowerSound);
        }
        else if (Ga.Amount < 0)
        {
            RuntimeManager.PlayOneShot(LosePowerSound);
        }
        else { return; }

        if (Ga.aditive)
        {
            // Cas additif : on ajoute toujours
            AffectedGA.Add(Ga);
        }
        else
        {
            // Cas normal : ajouter ou remplacer selon l'ID
            int index = AffectedGA.FindIndex(x => x.SourceEffect.EffectID == Ga.SourceEffect.EffectID);
            if (index == -1)
            {
                AffectedGA.Add(Ga);
            }
            else
            {
                AffectedGA[index] = Ga;
            }
        }

        UpdateBonusPowerAmount();
        UpdatePower();
    }

    public void TakeAlterLife(GainLifeGA Ga)
    {
        if (IsDead) return;

        if (Ga.Amount > 0)
        {
            RuntimeManager.PlayOneShot(BuffLifeSound);
        }
        else if (Ga.Amount < 0)
        {
            RuntimeManager.PlayOneShot(DebuffLifeSound);
        }
        else { return; }

        if (Ga.aditive)
        {
            // Cas additif : on ajoute toujours
            AffectedGA.Add(Ga);
        }
        else
        {
            // Cas normal : ajouter ou remplacer selon l'ID
            int index = AffectedGA.FindIndex(x => x.SourceEffect.EffectID == Ga.SourceEffect.EffectID);
            if (index == -1)
            {
                AffectedGA.Add(Ga);
            }
            else
            {
                AffectedGA[index] = Ga;
            }
        }

        UpdateBonusLifeAmount();
        UpdateMaxLife();
    }

    public void TakeAlterStamina(AlterStaminaGA Ga)
    {
        if (IsDead) return;
        if (Ga.aditive)
        {
            // Cas additif : on ajoute toujours
            AffectedGA.Add(Ga);
        }
        else
        {
            // Cas normal : ajouter ou remplacer selon l'ID
            int index = AffectedGA.FindIndex(x => x.SourceEffect.EffectID == Ga.SourceEffect.EffectID);
            if (index == -1)
            {
                AffectedGA.Add(Ga);
            }
            else
            {
                AffectedGA[index] = Ga;
            }
        }
        UpdateBonusStamAmount();
        UpdateMaxStam();
    }
    
    public void UpdateBonusPowerAmount()
    {
        CardReferenceArchive.BonusPower = 0;
        foreach (GameAction Ga in AffectedGA)
        {
            if (Ga is AlterPowerGA)
            {
                AlterPowerGA alterPowerGa = (AlterPowerGA) Ga;
                CardReferenceArchive.BonusPower += alterPowerGa.Amount;
            }
        }
    }

    public void UpdateBonusLifeAmount()
    {
        CardReferenceArchive.BonusLife = 0;
        foreach (GameAction Ga in AffectedGA)
        {
            if (Ga is GainLifeGA)
            {
                GainLifeGA alterLifeGa = (GainLifeGA)Ga;
                CardReferenceArchive.BonusLife += alterLifeGa.Amount;
            }
        }
    }
    
    public void UpdateBonusStamAmount()
    {
        CardReferenceArchive.BonusStam = 0;
        foreach (GameAction Ga in AffectedGA)
        {
            if (Ga is AlterStaminaGA)
            {
                AlterStaminaGA alterStamGa = (AlterStaminaGA)Ga;
                CardReferenceArchive.BonusStam += alterStamGa.Amount;
            }
        }
    }

    public void TakeLifeLoss(int Amount)
    {
        if (IsDead) return;
        if (Amount <= 0) return;

        transform.DOShakePosition(0.2f, 0.5f);
        EventInfo eventInfo = new EventInfo(Events.OnSelfDamaged, Enemy_Player_ENUM.NULL, KeyWordType.NULL,BasicParam.NULL);
        TriggerEventGA triggerEventGA = new(eventInfo, null, null, this, null);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        if (Amount > 0)
        {
            eventInfo = new EventInfo(Events.WhenPermaGainParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Life);
            triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Life);
            triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }
        else
        {
            eventInfo = new EventInfo(Events.WhenPermaLoseParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Life);
            triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Player, KeyWordType.NULL, BasicParam.Life);
            triggerEventGA = new(eventInfo, null, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);            
        }

        currentLife -= Amount;
        if (currentLife <= 0)
        {
            DiePermanentGA diePermanentGA = new(IsCore, Durability, CardReferenceArchive, this);
            ActionSystem.Instance.AddReaction(diePermanentGA);
            IsDead = true;
        }
        else
        {
            RuntimeManager.PlayOneShot(TakeLifeLossSound);
        }

        UpdateLifeText();
    }
    
    public void Refresh()
    {
        foreach (Effect effect in GameEventSystem.Instance.RetrieveEffectsFor(null,this,null))
        {
            foreach (EventInfo item in effect.EventInfos)
            {
                if (item.Events == Events.OnSelect)
                {
                    effect.ActivateLeft = effect.ActivateNumber;
                }
            }
        }
    }

    public void ActiveSelectEffect()
    {
        PermanentSpriteRenderer.color = Color.red;
        RuntimeManager.PlayOneShot(SelectedSound);
    }

    public void RemoveSelectEffect(bool SoundUp = true)
    {
        PermanentSpriteRenderer.color = Color.white;

        var HollowKeyword = KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Hollow);
        if (HollowKeyword != null)
        {
            Color c = PermanentSpriteRenderer.color;
            c.a = 0.3f;
            PermanentSpriteRenderer.color = c;
        }
        else
        {
            Color c = PermanentSpriteRenderer.color;
            c.a = 1f;
            PermanentSpriteRenderer.color = c;
        }
        
        if(SoundUp)
        {
            RuntimeManager.PlayOneShot(UnSelectedSound);
        }
    }

}
