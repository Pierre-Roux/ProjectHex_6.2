using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using FMODUnity;

public class PermanentView : MonoBehaviour
{
    [SerializeField] SpriteRenderer PermanentSpriteRenderer;
    [SerializeField] SpriteRenderer AuraSpriteRenderer;
    [SerializeField] TMP_Text HealthText;
    [SerializeField] TMP_Text StaminaText;
    [SerializeField] TMP_Text NameText;
    [SerializeField] public GameObject ShieldVisual;
    [SerializeField] public bool UnShieldable;

    [SerializeField] public EventReference DieSound;
    [SerializeField] public EventReference HollowDieSound;
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
    [HideInInspector] private int MaxLife { get; set; }
    [HideInInspector] public int currentLife { get; set; }
    [HideInInspector] public int baseLife { get; set; }
    [HideInInspector] public int MaxDurability { get; set; }
    [HideInInspector] public int Durability { get; set; }
    [HideInInspector] public int DecayCounter { get; set; }
    [HideInInspector] public int BonusPower { get; set; }
    [HideInInspector] public int CurrentHPBonus { get; set; }
    [HideInInspector] public Card CardReferenceArchive;
    [HideInInspector] public bool IsDead = false;
    [HideInInspector] public Vector3 InitialPosition { get; set; }
    [HideInInspector] public PermanentArea permanentArea;

    [HideInInspector] public List<PermanentView> PlayerShielder;
    [HideInInspector] public List<EnemySlotView> EnemyShielder;
    [HideInInspector] public List<PermanentView> PlayerShielded;
    [HideInInspector] public List<EnemySlotView> EnemyShielded;
    [HideInInspector] public bool UnTargetable;
    [HideInInspector] public bool Shielded;
    [HideInInspector] public bool Activated;

    [HideInInspector] public List<PermaTypes> permaTypes = new List<PermaTypes>();

    public void Setup(Card cardReference)
    {
        UnTargetable = cardReference.UnTargetable;
        IsCore = false;
        CardReferenceArchive = cardReference;
        PermanentSpriteRenderer.sprite = cardReference.data.PermanentImage;
        baseLife = cardReference.data.life;
        MaxLife = CalculateBonusLife(baseLife);
        currentLife = MaxLife;
        MaxDurability = cardReference.MaxDurability;
        Durability = cardReference.Durability;
        permanentArea = cardReference.data.permanentArea;
        UnShieldable = cardReference.UnShieldable;
        DecayCounter = cardReference.DecayCounter;
        UpdateNameText(cardReference.Title);
        deactivateAuraVisual();

        // Gère les types
        permaTypes.Clear();
        if (cardReference.data.isInvoc) permaTypes.Add(PermaTypes.Invoc);
        if (DecayCounter > 0) permaTypes.Add(PermaTypes.Decay);
        if (cardReference.MaxDurability > 0 && cardReference.Durability == 0) permaTypes.Add(PermaTypes.Hollow);
        if (cardReference.data.isArtillery) permaTypes.Add(PermaTypes.Artillery);

        ShieldVisual.SetActive(false);
        UpdateLifeText();

        // affichage graphique du hollow
        if (permaTypes.Contains(PermaTypes.Hollow))
        {
            UpdateHollowVisual();
        }

        //Audio
        if (AudioManager.Instance.IsValid(cardReference.DieSound)) DieSound = cardReference.DieSound;
        if (AudioManager.Instance.IsValid(cardReference.HollowDieSound)) HollowDieSound = cardReference.HollowDieSound;
        if (AudioManager.Instance.IsValid(cardReference.BeingDamageSound)) BeingDamageSound = cardReference.BeingDamageSound;
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
        permanentArea = PermanentArea.none;
        baseLife = CoreData.CoreHealth;
        MaxLife = CalculateBonusLife(baseLife);
        currentLife = MaxLife; 
        UnShieldable = false;
        ShieldVisual.SetActive(false);
        UpdateLifeText();
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
        if (permaTypes.Contains(PermaTypes.Hollow))
        {
            Color c = PermanentSpriteRenderer.color;
            c.a = 0.5f;
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

    public int CalculateBonusPower(int baseAmount)
    {
        int passiveBonus = 0;

        if (permaTypes.Contains(PermaTypes.Invoc))
            passiveBonus += CombatSystem.Instance.Invoc_PlayerGeneralHPGain + CombatSystem.Instance.Invoc_GeneralPower;
        if (permaTypes.Contains(PermaTypes.Decay))
            passiveBonus += CombatSystem.Instance.Decay_PlayerGeneralHPGain + CombatSystem.Instance.Decay_GeneralPower;
        if (permaTypes.Contains(PermaTypes.Hollow))
            passiveBonus += CombatSystem.Instance.Hollow_PlayerGeneralHPGain + CombatSystem.Instance.Hollow_GeneralPower;
        if (permaTypes.Contains(PermaTypes.Artillery))
            passiveBonus += CombatSystem.Instance.Artillery_PlayerGeneralHPGain + CombatSystem.Instance.Artillery_GeneralPower;

        int finalDMG = baseAmount + BonusPower + passiveBonus + CombatSystem.Instance.EnemyGeneralPower + CombatSystem.Instance.GeneralPower;
        return Mathf.Max(0, finalDMG);
    }
    public int CalculateBonusLife(int baseAmount)
    {
        int passiveBonus = 0;

        if (permaTypes.Contains(PermaTypes.Invoc))
            passiveBonus += CombatSystem.Instance.Invoc_PlayerGeneralHPGain + CombatSystem.Instance.Invoc_GeneralHPGain;
        if (permaTypes.Contains(PermaTypes.Decay))
            passiveBonus += CombatSystem.Instance.Decay_PlayerGeneralHPGain + CombatSystem.Instance.Decay_GeneralHPGain;
        if (permaTypes.Contains(PermaTypes.Hollow))
            passiveBonus += CombatSystem.Instance.Hollow_PlayerGeneralHPGain + CombatSystem.Instance.Hollow_GeneralHPGain;
        if (permaTypes.Contains(PermaTypes.Artillery))
            passiveBonus += CombatSystem.Instance.Artillery_PlayerGeneralHPGain + CombatSystem.Instance.Artillery_GeneralHPGain;

        int finalHP = baseAmount + passiveBonus + CombatSystem.Instance.PlayerGeneralHPGain + CombatSystem.Instance.GeneralHPGain;
        return Mathf.Max(0, finalHP);
    }

    public void UpdateLife()
    {
        int passiveBonus = CalculateBonusLife(0);
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


        UpdateLifeText();
    }

    public void TakeDamage(int Amount, Card CardActionner = null, GameObject Actionner = null)
    {
        if (Amount <= 0) return;
        currentLife -= Amount;
        UpdateLifeText();

        if (!IsDead)
        {
            transform.DOShakePosition(0.2f, 0.5f);
            TriggerEventGA triggerPermanentEventGA = new(Events.OnDamaged,null,this,null);
            ActionSystem.Instance.AddReaction(triggerPermanentEventGA);
        }

        if (currentLife <= 0)
        {
            if (!IsDead)
            {
                DiePermanentGA diePermanentGA = new(IsCore, Durability, CardReferenceArchive, this);
                ActionSystem.Instance.AddReaction(diePermanentGA);
                OnKillTrigger(CardActionner, Actionner);
                IsDead = true;
            }
        }
        else
        {
            RuntimeManager.PlayOneShot(BeingDamageSound);
        }
    }

    public void OnKillTrigger(Card CardActionner, GameObject Actionner)
    {
        if (Actionner != null)
        {
            if (Actionner.GetComponent<PermanentView>() != null)
            {
                TriggerEventGA triggerEventGA = new(Events.OnKill, null, Actionner.GetComponent<PermanentView>(), null);
                ActionSystem.Instance.AddReaction(triggerEventGA);
            }
            else if (Actionner.GetComponent<EnemySlotView>())
            {
                TriggerEventGA triggerEventGA = new(Events.OnKill, null, null, Actionner.GetComponent<EnemySlotView>());
                ActionSystem.Instance.AddReaction(triggerEventGA);
            }
        }
        else if (CardActionner != null)
        {
            TriggerEventGA triggerEventGA = new(Events.OnKill, CardActionner, null, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
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

    public void TakeAlterPower(int Amount)
    {
        if (IsDead) return;

        if (Amount > 0)
        {
            RuntimeManager.PlayOneShot(GainPowerSound);
        }
        else if (Amount < 0)
        {
            RuntimeManager.PlayOneShot(LosePowerSound);
        }
        else { return; }

        BonusPower += Amount;
        if (transform != null)
        {
            transform.DOShakePosition(0f, 0.1f);
        }
    }
    
    public void TakeAlterStamina(int Amount)
    {
        if (IsDead) return;

        /*if (Amount > 0)
        {
            RuntimeManager.PlayOneShot(GainPowerSound);
        }
        else if (Amount < 0)
        {
            RuntimeManager.PlayOneShot(LosePowerSound);
        }
        else { return; }*/

        if(Amount > 0)
        {
            TriggerEventGA triggerEventGA = new(Events.WhenPermaLossDurability,null,this,null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }

        Durability += Amount;
        if (Durability < 0)
        {
            Durability = 0;
        }
        else if (Durability > MaxDurability)
        {
            Durability = MaxDurability;
        }

        if (!permaTypes.Contains(PermaTypes.Hollow) && Durability == 0)
        {
            permaTypes.Add(PermaTypes.Hollow);
            UpdateHollowVisual();

            TriggerEventGA triggerEventGA = new(Events.WhenPermaBecomeType,null,this,null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }
        else if (permaTypes.Contains(PermaTypes.Hollow) && Durability != 0)
        {
            
            permaTypes.Remove(PermaTypes.Hollow);
            UpdateHollowVisual();
        }

        if (transform != null)
        {
            transform.DOShakePosition(0f, 0.1f);
        }
    }

    public void TakeLifeLoss(int Amount)
    {
        if (IsDead) return;
        if (Amount <= 0) return;

        transform.DOShakePosition(0.2f, 0.5f);
        TriggerEventGA triggerEventGA = new(Events.OnDamaged,null,this,null);
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

    public void GainLife(int Amount)
    {
        if (IsDead) return;

        if (Amount > 0)
        {
            RuntimeManager.PlayOneShot(BuffLifeSound);
        }
        else if (Amount < 0)
        {
            RuntimeManager.PlayOneShot(DebuffLifeSound);
        }
        else { return; }

        currentLife += Amount;
        MaxLife += Amount;

        if (currentLife <= 0)
        {
            DiePermanentGA diePermanentGA = new(IsCore, Durability, CardReferenceArchive, this);
            ActionSystem.Instance.AddReaction(diePermanentGA);
            IsDead = true;
        }

        UpdateLifeText();
    }
    
    public void Refresh()
    {
        if (Activated)
        {
            Activated = false;
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
        if(SoundUp)
        {
            RuntimeManager.PlayOneShot(UnSelectedSound);
        }
    }

}
