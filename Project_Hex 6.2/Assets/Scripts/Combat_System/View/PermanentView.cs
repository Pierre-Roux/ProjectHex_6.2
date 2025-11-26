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
    [HideInInspector] public int BaseMaxDurability { get; set; }
    [HideInInspector] public int DecayCounter { get; set; }
    [HideInInspector] public int BonusPower { get; set; }
    [HideInInspector] public int BonusStam { get; set; }
    [HideInInspector] public int BonusLife { get; set; }
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

    [HideInInspector] public List<PermaTypes> permaTypes = new List<PermaTypes>();
    [HideInInspector] public List<GameAction> AffectedGA = new List<GameAction>();
    [HideInInspector] public CounterManager InternCounters = new();

    public void Setup(Card cardReference)
    {
        InternCounters.ClearAll();
        UnTargetable = cardReference.UnTargetable;
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
        UnShieldable = cardReference.UnShieldable;
        DecayCounter = cardReference.DecayCounter;
        deactivateAuraVisual();
        UpdateNameText(cardReference.Title);

        // Gère les types // Hollow géré par UpdateStam
        TriggerEventGA triggerEventGA = null;
        if (cardReference.data.isInvoc)
        {
            permaTypes.Add(PermaTypes.Invoc);
            triggerEventGA = new(Events.InvocCountChanged, null, null, null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        } 
        if (DecayCounter > 0)
        {
            permaTypes.Add(PermaTypes.Decay);
            triggerEventGA = new(Events.DecayCountChanged,null,null,null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        } 
        if (cardReference.data.isArtillery)
        {
            permaTypes.Add(PermaTypes.Artillery);
            triggerEventGA = new(Events.ArtilleryCountChanged,null,null,null);
            ActionSystem.Instance.AddReaction(triggerEventGA);

        } 

        ShieldVisual.SetActive(false);

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
        if (permaTypes.Contains(PermaTypes.Hollow))
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
            permaTypes.Add(PermaTypes.Hollow);
            UpdateHollowVisual();

            triggerEventGA = new(Events.WhenPermaBecomeType,null,this,null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
            triggerEventGA = new(Events.HollowCountChanged,null,this,null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }
        else
        {
            permaTypes.Remove(PermaTypes.Hollow);
            UpdateHollowVisual();

            triggerEventGA = new(Events.HollowCountChanged,null,this,null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }

    }

    public int CalculateBonusPower()
    {
        int passiveBonus = 0;

        foreach (var type in permaTypes)
        {
            if (CombatSystem.Instance.PowerByTypeGeneral.TryGetValue(type, out var powerGroup))
            {
                passiveBonus += powerGroup.Player + powerGroup.Global;
            }
        }

        int finalDMG = BonusPower 
                    + passiveBonus 
                    + CombatSystem.Instance.GetPower(PermaTypes.NULL, Enemy_Player_ENUM.Player)
                    + CombatSystem.Instance.GetPower(PermaTypes.NULL, Enemy_Player_ENUM.NULL);

        return finalDMG;
    }

    public int CalculateBonusLife()
    {
        int passiveBonus = 0;

        foreach (var type in permaTypes)
        {
            passiveBonus += CombatSystem.Instance.GetHP(type, Enemy_Player_ENUM.Player);
            passiveBonus += CombatSystem.Instance.GetHP(type, Enemy_Player_ENUM.NULL);
        }

        int finalHP = BonusLife
                    + passiveBonus
                    + CombatSystem.Instance.GetHP(PermaTypes.NULL, Enemy_Player_ENUM.Player)
                    + CombatSystem.Instance.GetHP(PermaTypes.NULL, Enemy_Player_ENUM.NULL);

        return finalHP;
    }

    public int CalculateBonusStam()
    {
        int passiveBonus = 0;

        foreach (var type in permaTypes)
        {
            passiveBonus += CombatSystem.Instance.GetStam(type, Enemy_Player_ENUM.Player);
        }

        int finalStamina = BonusStam
                        + passiveBonus
                        + CombatSystem.Instance.GetStam(PermaTypes.NULL, Enemy_Player_ENUM.Player);

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

        if (!permaTypes.Contains(PermaTypes.Hollow) && Durability == 0)
        {
            ChangeHollowState(true);
        }
        else if (permaTypes.Contains(PermaTypes.Hollow) && Durability != 0)
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
        currentLife -= Amount;
        UpdateLifeText();

        if (!IsDead)
        {
            transform.DOShakePosition(0.2f, 0.5f);
            TriggerEventGA triggerEventGA;
            if (IsCore)
            {
                triggerEventGA = new(Events.WhenPCoreDamaged,null,this,null);
                ActionSystem.Instance.AddReaction(triggerEventGA);
            }
            triggerEventGA = new(Events.WhenPermaDamaged,null,this,null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
            triggerEventGA = new(Events.OnDamaged,null,this,null);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }

        if (currentLife <= 0)
        {
            if (!IsDead)
            {
                RuntimeManager.PlayOneShot(BeingDamageSound);
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

        if (permaTypes.Contains(PermaTypes.Hollow))
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
