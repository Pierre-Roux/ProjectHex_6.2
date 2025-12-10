using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using FMODUnity;
using System.Linq;

public class PermanentView : MonoBehaviour
{
    [SerializeField] public SpriteRenderer PermanentSpriteRenderer;
    [SerializeField] SpriteRenderer AuraSpriteRenderer;
    [SerializeField] TMP_Text HealthText;
    [SerializeField] TMP_Text StaminaText;
    [SerializeField] public TMP_Text NameText;
    [SerializeField] public GameObject ShieldVisual;
    [HideInInspector] public bool UnShieldable;

    [SerializeField] public EventReference DieSound;
    [SerializeField] public EventReference HollowDieSound;
    [SerializeField] public EventReference CollateralSound;
    [SerializeField] public EventReference BeingDamageSound;
    [SerializeField] public EventReference BeingHealSound;
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
    [HideInInspector] public int baseLife { get; set; }
    [HideInInspector] public int MaxDurability { get; set; }
    [HideInInspector] public int Durability { get; set; } 
    [HideInInspector] public int BaseMaxDurability { get; set; }
    [HideInInspector] public int BonusPower { get; set; }
    [HideInInspector] public int BonusStam { get; set; }
    [HideInInspector] public int BonusLife { get; set; }
    [HideInInspector] public int CurrentHPBonus { get; set; }
    [HideInInspector] public Card CardReferenceArchive;
    [HideInInspector] public bool IsDisabled = false;
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
    [HideInInspector] public CounterManager InternCounters = new();

    public void Setup(Card cardReference)
    {
        InternCounters.ClearAll();
        KeyWords = new List<KeyWord>(cardReference.KeyWords);
        IsCore = false;
        CardReferenceArchive = cardReference;
        PermanentSpriteRenderer.sprite = cardReference.data.PermanentImage;
        baseLife = cardReference.data.life;
        MaxLife = baseLife;
        currentLife = MaxLife;
        UpdateLife();
        BaseMaxDurability = cardReference.MaxDurability;
        MaxDurability = cardReference.MaxDurability;
        Durability = cardReference.Durability;
        UpdateStam();
        permanentArea = cardReference.data.permanentArea;
        deactivateAuraVisual();
        UpdateNameText(cardReference.Title);

        var UnShieldableKeyword = KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnShieldable);
        if (UnShieldableKeyword != null)
        {
            UnShieldable = true;
        }
        var UnTargetableKeyword = KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.UnTargetable);
        if (UnTargetableKeyword != null)
        {
            UnTargetable = true;
        }

        // Gère les Changement de Counter // Hollow géré par UpdateStam
        TriggerEventGA triggerEventGA = null;
        var InvocKeyword = KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Invoc);
        if (InvocKeyword != null)
        {
            triggerEventGA = new(Events.InvocCountChanged, null, null, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }
        var ArtilleryKeyword = KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Artillery);
        if (InvocKeyword != null)
        {
            triggerEventGA = new(Events.ArtilleryCountChanged, null, null, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);

        }
        var decayKeyword = KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Decay);
        if (decayKeyword != null && decayKeyword.keyWordValue > 0)
        {
            triggerEventGA = new(Events.DecayCountChanged,null,null,null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }

        ShieldVisual.SetActive(false);

        //Audio
        if (AudioManager.Instance.IsValid(cardReference.DieSound)) DieSound = cardReference.DieSound;
        if (AudioManager.Instance.IsValid(cardReference.HollowDieSound)) HollowDieSound = cardReference.HollowDieSound;
        if (AudioManager.Instance.IsValid(cardReference.BeingDamageSound)) BeingDamageSound = cardReference.BeingDamageSound;
        if (AudioManager.Instance.IsValid(cardReference.CollateralSound)) CollateralSound = cardReference.CollateralSound;
        if (AudioManager.Instance.IsValid(cardReference.BeingHealSound)) BeingHealSound = cardReference.BeingHealSound;
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

    public void SetPosition(Vector3 pos)
    {
        InitialPosition = pos;
    }

    public void SetupCore(PlayerData CoreData)
    {
        UnTargetable = false;
        IsCore = true;
        PermanentSpriteRenderer.sprite = CoreData.CoreImage;
        permanentArea = PermanentArea.NONE;
        baseLife = CoreData.CoreHealth;
        MaxLife = baseLife;
        currentLife = MaxLife;
        UpdateLife();
        UnShieldable = false;
        ShieldVisual.SetActive(false);
        deactivateAuraVisual();

        if (AudioManager.Instance.IsValid(CoreData.DieSound)) DieSound = CoreData.DieSound;
        if (AudioManager.Instance.IsValid(CoreData.BeingDamageSound)) BeingDamageSound = CoreData.BeingDamageSound;
        if (AudioManager.Instance.IsValid(CoreData.BeingHealSound)) BeingHealSound = CoreData.BeingHealSound;
        if (AudioManager.Instance.IsValid(CoreData.BeingShieldSound)) BeingShieldSound = CoreData.BeingShieldSound;
        if (AudioManager.Instance.IsValid(CoreData.LoseShieldSound)) LoseShieldSound = CoreData.LoseShieldSound;
        if (AudioManager.Instance.IsValid(CoreData.TakeLifeLossSound)) TakeLifeLossSound = CoreData.TakeLifeLossSound;
        if (AudioManager.Instance.IsValid(CoreData.BuffLifeSound)) BuffLifeSound = CoreData.BuffLifeSound;
        if (AudioManager.Instance.IsValid(CoreData.DebuffLifeSound)) DebuffLifeSound = CoreData.DebuffLifeSound;
        if (AudioManager.Instance.IsValid(CoreData.SelectedSound)) SelectedSound = CoreData.SelectedSound;
        if (AudioManager.Instance.IsValid(CoreData.UnSelectedSound)) UnSelectedSound = CoreData.UnSelectedSound;
    }

    public void UpdateLifeText()
    {
        HealthText.text = currentLife.ToString();
    }

    /*public void UpdateStaminaText()
    {
        StaminaText.text = Durability.ToString() + "/" + MaxDurability.ToString();
    }*/

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

                triggerEventGA = new(Events.WhenPermaBecomeType,null,this,null);
                ActionSystem.Instance.AddReaction(triggerEventGA);
                triggerEventGA = new(Events.HollowCountChanged,null,this,null);
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

                triggerEventGA = new(Events.HollowCountChanged,null,this,null);
                ActionSystem.Instance.AddReaction(triggerEventGA);
            }
        }

    }

    public int CalculateBonusPower()
    {
        int passiveBonus = 0;

        foreach (var keyword in KeyWords)
        {
            if (CombatSystem.Instance.PowerByTypeGeneral.TryGetValue(keyword.keyWordType, out var powerGroup))
            {
                passiveBonus += powerGroup.Player + powerGroup.Global;
                //Debug.Log("passiveBonus augment by powerGroupPlayer " + powerGroup.Player + " & GeneralGroup " + powerGroup.Global + " For " + keyword.keyWordType);
            }

        }

        int finalDMG = BonusPower 
                    + passiveBonus 
                    + CombatSystem.Instance.GetPower(KeyWordType.NULL, Enemy_Player_ENUM.Player)
                    + CombatSystem.Instance.GetPower(KeyWordType.NULL, Enemy_Player_ENUM.NULL);

        /*Debug.Log("FinalDamage : " + finalDMG + " =  BonusPower " 
        + BonusPower + " passiveBonus " +
        + passiveBonus + " passivePlayerGeneral " +
        + CombatSystem.Instance.GetPower(PermaTypes.NULL, Enemy_Player_ENUM.Player) + " passiveGeneral " +
        + CombatSystem.Instance.GetPower(PermaTypes.NULL, Enemy_Player_ENUM.NULL));*/

        return finalDMG;
    }

    public int CalculateBonusLife()
    {
        int passiveBonus = 0;

        foreach (var keyword in KeyWords)
        {
            passiveBonus += CombatSystem.Instance.GetHP(keyword.keyWordType, Enemy_Player_ENUM.Player);
            passiveBonus += CombatSystem.Instance.GetHP(keyword.keyWordType, Enemy_Player_ENUM.NULL);
        }

        int finalHP = BonusLife
                    + passiveBonus
                    + CombatSystem.Instance.GetHP(KeyWordType.NULL, Enemy_Player_ENUM.Player)
                    + CombatSystem.Instance.GetHP(KeyWordType.NULL, Enemy_Player_ENUM.NULL);

        return finalHP;
    }

    public int CalculateBonusStam()
    {
        int passiveBonus = 0;

        foreach (var keyword in KeyWords)
        {
            passiveBonus += CombatSystem.Instance.GetStam(keyword.keyWordType, Enemy_Player_ENUM.Player);
        }

        int finalStamina = BonusStam
                        + passiveBonus
                        + CombatSystem.Instance.GetStam(KeyWordType.NULL, Enemy_Player_ENUM.Player);

        return finalStamina;
    }

    public void UpdateStam()
    {
        if (IsCore) return;

        int passiveBonus = CalculateBonusStam();
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

    }

    public void UpdateLife()
    {
        int passiveBonus = CalculateBonusLife();
        MaxLife = baseLife + passiveBonus;

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

        if (MaxLife <= 0)
        {
            MaxLife = 1;
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

        UpdateLifeText();
    }

    public void TakeDamage(int Amount, Card CardActionner = null, GameObject Actionner = null)
    {
        if (Amount <= 0) return;

        PermanentView Pstriker = null;
        EnemySlotView Estriker = null;
        Card Cstriker = null;

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
            TriggerEventGA triggerEventGA;
            if (IsCore)
            {
                if (Pstriker != null)
                {
                    triggerEventGA = new(Events.WhenPCoreDamaged, null, Pstriker, null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);
                }
                else if (Estriker != null)
                {
                    triggerEventGA = new(Events.WhenPCoreDamaged, null, null, Estriker);
                    ActionSystem.Instance.AddReaction(triggerEventGA);
                }
                else if (Cstriker != null)
                {
                    triggerEventGA = new(Events.WhenPCoreDamaged, Cstriker, null, null);
                    ActionSystem.Instance.AddReaction(triggerEventGA);
                }
            }
            triggerEventGA = new(Events.WhenPermaDamaged,null,this,null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
            triggerEventGA = new(Events.OnDamaged,null,this,null);
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
                    if (Pstriker.KeyWords.Any(k => k.keyWordType == KeyWordType.Collateral))
                    {
                        CollateralTrigger(-currentLife, Pstriker, Estriker, Cstriker);
                        currentLife = 0;
                    }
                    OnKillTrigger(Pstriker, Estriker, Cstriker);
                }
                else if (Estriker != null)
                {
                    if (Estriker.KeyWords.Any(k => k.keyWordType == KeyWordType.Collateral))
                    {
                        CollateralTrigger(-currentLife, Pstriker, Estriker, Cstriker);
                        currentLife = 0;
                    }
                    OnKillTrigger(Pstriker, Estriker, Cstriker);
                }
                else if (Cstriker != null)
                {
                    if (Cstriker.KeyWords.Any(k => k.keyWordType == KeyWordType.Collateral))
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
    }

    public void OnKillTrigger(PermanentView Pstriker, EnemySlotView Estriker, Card Cstriker)
    {
        if (Pstriker != null)
        {
            TriggerEventGA triggerEventGA = new(Events.OnKill, null, Pstriker, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }
        else if (Estriker != null)
        {
            TriggerEventGA triggerEventGA = new(Events.OnKill, null, null, Estriker);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }
        else if (Cstriker != null)
        {
            TriggerEventGA triggerEventGA = new(Events.OnKill, Cstriker, null, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }
    }

    public void CollateralTrigger(int CollateralAmount, PermanentView Pstriker, EnemySlotView Estriker, Card Cstriker)
    {
        if (CollateralAmount == 0) return;
        if (IsCore) return;
        if (Pstriker != null)
        {
            List<PermanentView> targets_Player = new List<PermanentView> { CombatSystem.Instance.PlayerCore };
            DealDamageGA dealDamageGA = new(CollateralAmount, 0, 1, DynamicAmount.NULL, targets_Player, null);
            dealDamageGA.Actionner = Pstriker.gameObject;
            dealDamageGA.SourceEffect = null;
            dealDamageGA.ActivateToolTip = false;
            dealDamageGA.SFX = !AudioManager.Instance.IsValid(CollateralSound) ? AudioManager.Instance.CollateralSound : CollateralSound;
            ActionSystem.Instance.AddReaction(dealDamageGA);
        }
        else if (Estriker != null)
        {
            List<PermanentView> targets_Player = new List<PermanentView> { CombatSystem.Instance.PlayerCore };
            DealDamageGA dealDamageGA = new(CollateralAmount, 0, 1, DynamicAmount.NULL, targets_Player, null);
            dealDamageGA.Actionner = Estriker.gameObject;
            dealDamageGA.SourceEffect = null;
            dealDamageGA.ActivateToolTip = false;
            dealDamageGA.SFX = !AudioManager.Instance.IsValid(CollateralSound) ? AudioManager.Instance.CollateralSound : CollateralSound;
            ActionSystem.Instance.AddReaction(dealDamageGA);
        }
        else if (Cstriker != null)
        {
            List<PermanentView> targets_Player = new List<PermanentView> { CombatSystem.Instance.PlayerCore };
            DealDamageGA dealDamageGA = new(CollateralAmount, 0, 1, DynamicAmount.NULL, targets_Player, null);
            dealDamageGA.CardActionner = Cstriker;
            dealDamageGA.SourceEffect = null;
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
        RuntimeManager.PlayOneShot(BeingHealSound);
        transform.DOShakePosition(0.1f, 0.1f);
        UpdateLifeText();
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
            playerShielder.GetComponent<PermanentView>().PlayerShielded.Remove(this);
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

        Debug.Log("Take Alter with aditive at " + Ga.aditive);
        if (Ga.aditive)
        {
            // Cas additif : on ajoute toujours
            AffectedGA.Add(Ga);
        }
        else
        {
            // Cas normal : ajouter ou remplacer selon l'ID
            Debug.Log("AlterPowerEffectID ->>>>> " + Ga.SourceEffect.EffectID);
            int index = AffectedGA.FindIndex(x => x.SourceEffect.EffectID == Ga.SourceEffect.EffectID);
            if (index == -1)
            {
                AffectedGA.Add(Ga);
                Debug.Log("ADD");
            }
            else
            {
                AffectedGA[index] = Ga;
                Debug.Log("Replace");
            }
        }

        UpdateBonusPowerAmount();
    }

    public void UpdateBonusPowerAmount()
    {
        BonusPower = 0;
        foreach (GameAction Ga in AffectedGA)
        {
            if (Ga is AlterPowerGA)
            {
                AlterPowerGA alterPowerGa = (AlterPowerGA) Ga;
                BonusPower += alterPowerGa.Amount;
            }
        }
    }

    public void TakeAlterStamina(AlterStaminaGA Ga)
    {
        if (IsDead) return;

        if (Ga.Amount < 0)
        {
            TriggerEventGA triggerEventGA = new(Events.WhenPermaLossDurability, null, this, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }

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
        UpdateStam();
    }

    public void UpdateBonusStamAmount()
    {
        BonusStam = 0;
        foreach (GameAction Ga in AffectedGA)
        {
            if (Ga is AlterStaminaGA)
            {
                AlterStaminaGA alterStamGa = (AlterStaminaGA)Ga;
                BonusStam += alterStamGa.Amount;
            }
        }
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
        UpdateLife();
    }
    
    public void UpdateBonusLifeAmount()
    {
        BonusLife = 0;
        foreach (GameAction Ga in AffectedGA)
        {
            if (Ga is GainLifeGA)
            {
                GainLifeGA alterLifeGa = (GainLifeGA)Ga;
                BonusLife += alterLifeGa.Amount;
            }
        }
    }

    public void TakeLifeLoss(int Amount)
    {
        if (IsDead) return;
        if (Amount <= 0) return;

        transform.DOShakePosition(0.2f, 0.5f);
        TriggerEventGA triggerEventGA = new(Events.OnDamaged, null, this, null);
        ActionSystem.Instance.AddReaction(triggerEventGA);


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
            if (effect.Events.Contains(Events.OnSelect))
            {
                effect.ActivateLeft = effect.ActivateNumber;
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
