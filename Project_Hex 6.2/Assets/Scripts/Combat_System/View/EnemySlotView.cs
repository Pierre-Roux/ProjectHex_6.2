using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using TMPro;
using UnityEngine;

public class EnemySlotView : MonoBehaviour
{
    [SerializeField] public List<Effect> PossibleIntent;
    [HideInInspector] public EnemyPermanentData PermanentData;
    [SerializeField] public TMP_Text LifeText;
    [SerializeField] public TMP_Text IntentText;
    [SerializeField] TMP_Text NameText;
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] SpriteRenderer AuraSpriteRenderer;
    [SerializeField] public GameObject ShieldVisual;
    [SerializeField] public bool UnShieldable;

    [SerializeField] public EventReference DieSound;
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

    [HideInInspector] public Effect IntentAction;
    [HideInInspector] public int currentLife { get; set; }
    [HideInInspector] public int baseLife { get; set; }
    [HideInInspector] public int MaxLife { get; set; }
    [HideInInspector] public bool IsCore { get; set; }
    [HideInInspector] public bool IsDead = false;
    [HideInInspector] public Vector3 InitialPosition { get; set; }
    [HideInInspector] public int DecayCounter { get; set; }
    [HideInInspector] public int BonusPower { get; set; }
    [HideInInspector] public int CurrentHPBonus { get; set; }
    [HideInInspector] public PermanentArea permanentArea;

    [HideInInspector] public List<PermaTypes> permaTypes = new List<PermaTypes>();

    [HideInInspector] public List<PermanentView> PlayerShielder = new();
    [HideInInspector] public List<EnemySlotView> EnemyShielder = new();
    [HideInInspector] public List<PermanentView> PlayerShielded = new();
    [HideInInspector] public List<EnemySlotView> EnemyShielded = new();

    [HideInInspector] public bool UnTargetable;
    [HideInInspector] public bool Shielded;
    [HideInInspector] public bool Activated;

    [HideInInspector] public bool RDMSequence;
    [HideInInspector] public List<string> IntentSequence = new List<string>();
    [HideInInspector] public bool LoopingSequence;
    private int sequenceIndex = 0;
    public void setup()
    {
        PossibleIntent = PermanentData.PossibleIntent;
        spriteRenderer.sprite = PermanentData.PermanentImage;
        baseLife = PermanentData.PermanentLife;
        MaxLife = CalculateBonusLife(baseLife);
        currentLife = MaxLife; 
        IsCore = PermanentData.IsCore;
        UnShieldable = PermanentData.UnShieldable;
        ShieldVisual.SetActive(false);
        UnTargetable = PermanentData.UnTargetable;
        RDMSequence = PermanentData.RDMSequence;
        IntentSequence = PermanentData.IntentSequence;
        LoopingSequence = PermanentData.LoopingSequence;
        DecayCounter = PermanentData.DecayCounter;
        UpdateNameText(PermanentData.Title);
        deactivateAuraVisual();


        if (PermanentData.IsInvoc) permaTypes.Add(PermaTypes.Invoc);
        if (PermanentData.DecayCounter > 0) permaTypes.Add(PermaTypes.Decay);

        if (IsCore)
        {
            permanentArea = PermanentArea.none;
        }
        else
        {
            permanentArea = PermanentData.permanentArea;
        }

        //Audio
        if (AudioManager.Instance.IsValid(PermanentData.DieSound)) DieSound = PermanentData.DieSound;
        if (AudioManager.Instance.IsValid(PermanentData.BeingDamageSound)) BeingDamageSound = PermanentData.BeingDamageSound;
        if (AudioManager.Instance.IsValid(PermanentData.BeingHealSound)) BeingHealSound = PermanentData.BeingHealSound;
        if (AudioManager.Instance.IsValid(PermanentData.BeingShieldSound)) BeingShieldSound = PermanentData.BeingShieldSound;
        if (AudioManager.Instance.IsValid(PermanentData.LoseShieldSound)) LoseShieldSound = PermanentData.LoseShieldSound;
        if (AudioManager.Instance.IsValid(PermanentData.GainPowerSound)) GainPowerSound = PermanentData.GainPowerSound;
        if (AudioManager.Instance.IsValid(PermanentData.LosePowerSound)) LosePowerSound = PermanentData.LosePowerSound;
        if (AudioManager.Instance.IsValid(PermanentData.TakeLifeLossSound)) TakeLifeLossSound = PermanentData.TakeLifeLossSound;
        if (AudioManager.Instance.IsValid(PermanentData.BuffLifeSound)) BuffLifeSound = PermanentData.BuffLifeSound;
        if (AudioManager.Instance.IsValid(PermanentData.DebuffLifeSound)) DebuffLifeSound = PermanentData.DebuffLifeSound;
        if (AudioManager.Instance.IsValid(PermanentData.ActivateSound)) ActivateSound = PermanentData.ActivateSound;
        if (AudioManager.Instance.IsValid(PermanentData.SelectedSound)) SelectedSound = PermanentData.SelectedSound;
        if (AudioManager.Instance.IsValid(PermanentData.UnSelectedSound)) UnSelectedSound = PermanentData.UnSelectedSound;

        UpdateIntent();
        UpdateLifeText();
    }

    public void SetPosition(Vector3 pos)
    {
        InitialPosition = pos;
    }
    public void UpdateNameText(string name)
    {
        NameText.text = name;
    }

    public void UpdateLifeText()
    {
        LifeText.text = currentLife.ToString();
    }

    public void UpdateIntent()
    {
        if (PossibleIntent.Count <= 0) return;
        Effect selectedEffect = null;

        if (RDMSequence)
        {
            List<Effect> valid = PossibleIntent.FindAll(e => e.Events == Events.EnemyTurn);

            if (valid.Count > 0)
            {
                selectedEffect = valid[UnityEngine.Random.Range(0, valid.Count)];
            }
        }
        else
        {
            if (IntentSequence.Count == 0)
            {
                return;
            }

            if (sequenceIndex >= IntentSequence.Count)
            {
                if (LoopingSequence)
                    sequenceIndex = 0;
                else
                    return;
            }

            string currentKey = IntentSequence[sequenceIndex];
            if (currentKey != "")
            {
                selectedEffect = PossibleIntent.Find(e => e.Events == Events.EnemyTurn && e.number == currentKey);

                if (selectedEffect == null)
                {
                    Debug.LogWarning($"No matching Effect with number '{currentKey}' in {name}");
                }
            }
            sequenceIndex++;
        }

        if (selectedEffect != null)
        {
            IntentAction = selectedEffect.Clone();
            if (IntentAction is EffectGroup)
            {
                IntentAction.Actionner = this.gameObject;
                EffectGroup group = (EffectGroup)IntentAction;
                foreach (var Effect in group.EffectGroups)
                {
                    Effect.Actionner = this.gameObject;
                }
                UpdateIntentText(selectedEffect); 
            }
            else
            {
                IntentAction.Actionner = this.gameObject;
                UpdateIntentText(selectedEffect);                
            }
        }
        else
        {
            IntentText.text = "!";
        }
    }

    public void UpdateIntentText(Effect selectedEffect)
    {
        if (selectedEffect == null) return;

        string intentText = selectedEffect.Intent_Title; // fallback

        switch (selectedEffect)
        {
            case DealDamageEffect dmg:
                int damagetext = CalculateBonusPower(dmg.damageAmount);

                intentText = $"Deal {damagetext} damage to {dmg.targetMode}";
                break;

            case HealEffect heal:
                intentText = $"Heal {heal.amount} HP to {heal.targetMode}";
                break;

            case DrawCardsEffect draw:
                intentText = $"Draw {draw.drawAmount} cards";
                break;

            case ShieldEffect shield:
                intentText = $"Shield {shield.targetMode} ";
                break;

            case AlterPowerEffect alter:
                intentText = $"Alter power by {alter.alterAmount} of {alter.targetMode}";
                break;
        }

        IntentText.text = intentText;
    }

    public int CalculateBonusPower(int BaseAmount)
    {
        int passiveBonus = 0;
        foreach (var type in permaTypes)
        {
            switch (type)
            {
                case PermaTypes.Invoc:
                    passiveBonus += CombatSystem.Instance.Invoc_EnemyGeneralPower + CombatSystem.Instance.Invoc_GeneralPower;
                    break;
                case PermaTypes.Decay:
                    passiveBonus += CombatSystem.Instance.Decay_EnemyGeneralPower + CombatSystem.Instance.Decay_GeneralPower;
                    break;
                case PermaTypes.Hollow:
                    passiveBonus += CombatSystem.Instance.Hollow_EnemyGeneralPower + CombatSystem.Instance.Hollow_GeneralPower;
                    break;
                case PermaTypes.Artillery:
                    passiveBonus += CombatSystem.Instance.Artillery_EnemyGeneralPower + CombatSystem.Instance.Artillery_GeneralPower;
                    break;
            }
        }

        int finalDMG = BaseAmount + BonusPower + passiveBonus + CombatSystem.Instance.EnemyGeneralPower + CombatSystem.Instance.GeneralPower;
        return Mathf.Max(0, finalDMG);
    }

    public int CalculateBonusLife(int BaseAmount)
    {
        int passiveBonus = 0;
        foreach (var type in permaTypes)
        {
            switch (type)
            {
                case PermaTypes.Invoc:
                    passiveBonus += CombatSystem.Instance.Invoc_EnemyGeneralHPGain + CombatSystem.Instance.Invoc_GeneralHPGain;
                    break;
                case PermaTypes.Decay:
                    passiveBonus += CombatSystem.Instance.Decay_EnemyGeneralHPGain + CombatSystem.Instance.Decay_GeneralHPGain;
                    break;
                case PermaTypes.Hollow:
                    passiveBonus += CombatSystem.Instance.Hollow_EnemyGeneralHPGain + CombatSystem.Instance.Hollow_GeneralHPGain;
                    break;
                case PermaTypes.Artillery:
                    passiveBonus += CombatSystem.Instance.Artillery_EnemyGeneralHPGain + CombatSystem.Instance.Artillery_GeneralHPGain;
                    break;
            }
        }

        int finalHP = BaseAmount + BonusPower + passiveBonus + CombatSystem.Instance.EnemyGeneralHPGain + CombatSystem.Instance.GeneralHPGain;
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

    public void ActivateAuraVisual()
    {
        if (AuraSpriteRenderer != null)
        {
            AuraSpriteRenderer.gameObject.SetActive(true);   
        }
    }

    public void deactivateAuraVisual()
    {
        if (AuraSpriteRenderer != null)
        {
            AuraSpriteRenderer.gameObject.SetActive(false);            
        }
    }

    public void TakeDamage(int Amount, Card CardActionner = null, GameObject Actionner = null)
    {
        if (Amount <= 0) return;
        if (!IsDead)
        {
            transform.DOShakePosition(0.2f, 0.5f);
            TriggerEventGA triggerEventGA = new(Events.OnDamaged,null,null,this);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }

        currentLife -= Amount;
        if (currentLife <= 0)
        {
            if (!IsDead)
            {
                DieEnemySlotGA dieEnemySlotGA = new(this);
                ActionSystem.Instance.AddReaction(dieEnemySlotGA);
                OnKillTrigger(CardActionner, Actionner);
                IsDead = true;
            }
        }
        else
        {
            RuntimeManager.PlayOneShot(BeingDamageSound);
        }

        UpdateLifeText();
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
        transform.DOShakePosition(0f, 0.1f);
        UpdateLifeText();
    }

    public void TakeShield(PermanentView playerShielder = null, EnemySlotView enemyShielder = null)
    {
        if (!UnShieldable)
        {
            if (playerShielder != null)
            {
                RuntimeManager.PlayOneShot(BeingShieldSound);
                if (!PlayerShielder.Contains(playerShielder))
                {
                    PlayerShielder.Add(playerShielder);
                    playerShielder.GetComponent<PermanentView>().EnemyShielded.Add(this);
                }
            }

            if (enemyShielder != null)
            {
                if (!EnemyShielder.Contains(enemyShielder))
                {
                    EnemyShielder.Add(enemyShielder);
                    enemyShielder.GetComponent<EnemySlotView>().EnemyShielded.Add(this);
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
            playerShielder.GetComponent<PermanentView>().EnemyShielded.Remove(this);
            PlayerShielder.Remove(playerShielder);
        }
        if (enemyShielder != null)
        {
            playerShielder.GetComponent<PermanentView>().EnemyShielded.Remove(this);
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
        UpdateIntentText(IntentAction);
    }

    public void TakeLifeLoss(int Amount)
    {
        if (IsDead) return;
        if (Amount <= 0) return;

        transform.DOShakePosition(0.2f, 0.5f);
        TriggerEventGA triggerEventGA = new(Events.OnDamaged,null,null,this);
        ActionSystem.Instance.AddReaction(triggerEventGA);
        

        currentLife -= Amount;
        if (currentLife <= 0)
        {
            DieEnemySlotGA dieEnemySlotGA = new(this);
            ActionSystem.Instance.AddReaction(dieEnemySlotGA);
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
            DieEnemySlotGA dieEnemySlotGA = new(this);
            ActionSystem.Instance.AddReaction(dieEnemySlotGA);
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
        spriteRenderer.color = Color.red;
        RuntimeManager.PlayOneShot(SelectedSound);
    }

    public void RemoveSelectEffect(bool SoundUp = true)
    {
        spriteRenderer.color = Color.white;
        if (SoundUp)
        {
            RuntimeManager.PlayOneShot(UnSelectedSound);            
        }
    }
}
