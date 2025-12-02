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
    [SerializeField] public TMP_Text NameText;
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
    [HideInInspector] public int BonusLife { get; set; }
    [HideInInspector] public int CurrentHPBonus { get; set; }
    [HideInInspector] public PermanentArea permanentArea;

    [HideInInspector] public List<PermanentView> PlayerShielder = new();
    [HideInInspector] public List<EnemySlotView> EnemyShielder = new();
    [HideInInspector] public List<PermanentView> PlayerShielded = new();
    [HideInInspector] public List<EnemySlotView> EnemyShielded = new();

    [HideInInspector] public bool UnTargetable;
    [HideInInspector] public bool Shielded;

    [HideInInspector] public bool RDMSequence;
    [HideInInspector] public List<string> IntentSequence = new List<string>();
    [HideInInspector] public bool LoopingSequence;
    [HideInInspector] public int sequenceIndex = 0;

    [HideInInspector] public List<PermaTypes> permaTypes = new List<PermaTypes>();
    [HideInInspector] public List<GameAction> AffectedGA = new List<GameAction>();
    [HideInInspector] public CounterManager InternCounters = new();
    public void setup()
    {
        InternCounters.ClearAll();
        PossibleIntent = PermanentData.PossibleIntent;
        spriteRenderer.sprite = PermanentData.PermanentImage;
        baseLife = PermanentData.PermanentLife;
        MaxLife = baseLife;
        currentLife = MaxLife;
        UpdateLife();
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
            permanentArea = PermanentArea.NONE;
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
            List<Effect> valid = PossibleIntent.FindAll(e =>  e.Events.Contains(Events.EnemyTurn));

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
                selectedEffect = PossibleIntent.Find(e => e.Events.Contains(Events.EnemyTurn) && e.number == currentKey);

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
                int damagetext = CalculateBonusPowerForText(dmg.damageAmount);

                intentText = $"Deal {damagetext} damage to {dmg.targetModeInfo.targetMode}";
                break;

            case HealEffect heal:
                intentText = $"Heal {heal.amount} HP to {heal.targetModeInfo.targetMode}";
                break;

            case DrawCardsEffect draw:
                intentText = $"Draw {draw.drawAmount} cards";
                break;

            case ShieldEffect shield:
                intentText = $"Shield {shield.targetModeInfo.targetMode} ";
                break;

            case AlterPowerEffect alter:
                intentText = $"Alter power by {alter.alterAmount} of {alter.targetModeInfo.targetMode}";
                break;
        }

        IntentText.text = intentText;
    }

    public int CalculateBonusPowerForText(int BaseAmount)
    {
        int passiveBonus = 0;

        foreach (var type in permaTypes)
        {
            if (CombatSystem.Instance.PowerByTypeGeneral.TryGetValue(type, out var powerGroup))
            {
                passiveBonus += powerGroup.Enemy + powerGroup.Global;
            }
        }

        int finalDMG = BaseAmount
                    + BonusPower
                    + passiveBonus
                    + CombatSystem.Instance.GetPower(PermaTypes.NULL, Enemy_Player_ENUM.Enemy)
                    + CombatSystem.Instance.GetPower(PermaTypes.NULL, Enemy_Player_ENUM.NULL);

        return Mathf.Max(finalDMG, 0);
    }

    public int CalculateBonusPower()
    {
        int passiveBonus = 0;

        foreach (var type in permaTypes)
        {
            if (CombatSystem.Instance.PowerByTypeGeneral.TryGetValue(type, out var powerGroup))
            {
                passiveBonus += powerGroup.Enemy + powerGroup.Global;
            }
        }

        int finalDMG = BonusPower 
                    + passiveBonus 
                    + CombatSystem.Instance.GetPower(PermaTypes.NULL, Enemy_Player_ENUM.Enemy)
                    + CombatSystem.Instance.GetPower(PermaTypes.NULL, Enemy_Player_ENUM.NULL);

        return finalDMG;
    }

    public int CalculateBonusLife()
    {
        int passiveBonus = 0;

        foreach (var type in permaTypes)
        {
            passiveBonus += CombatSystem.Instance.GetHP(type, Enemy_Player_ENUM.Enemy);
            passiveBonus += CombatSystem.Instance.GetHP(type, Enemy_Player_ENUM.NULL);
        }

        int finalHP = BonusLife
                    + BonusPower
                    + passiveBonus
                    + CombatSystem.Instance.GetHP(PermaTypes.NULL, Enemy_Player_ENUM.Enemy)
                    + CombatSystem.Instance.GetHP(PermaTypes.NULL, Enemy_Player_ENUM.NULL);

        return finalHP;
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
            DieEnemySlotGA dieEnemySlotGA = new(this);
            ActionSystem.Instance.AddReaction(dieEnemySlotGA);
            IsDead = true;
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
            TriggerEventGA triggerEventGA;
            if (IsCore)
            {
                if (Actionner != null)
                {
                    if (Actionner.GetComponent<PermanentView>() != null)
                    {
                        triggerEventGA = new(Events.WhenECoreDamaged,null,Actionner.GetComponent<PermanentView>(),null);
                        ActionSystem.Instance.AddReaction(triggerEventGA);
                    }
                    else if (Actionner.GetComponent<EnemySlotView>() != null)
                    {
                        triggerEventGA = new(Events.WhenECoreDamaged,null,null,Actionner.GetComponent<EnemySlotView>());
                        ActionSystem.Instance.AddReaction(triggerEventGA);                    
                    }                    
                }
            }
            transform.DOShakePosition(0.2f, 0.5f);
            triggerEventGA = new(Events.WhenPermaDamaged,null,null,this);
            ActionSystem.Instance.AddReaction(triggerEventGA);
            triggerEventGA = new(Events.OnDamaged,null,null,this);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }

        currentLife -= Amount;
        if (currentLife <= 0)
        {
            if (!IsDead)
            {
                RuntimeManager.PlayOneShot(BeingDamageSound);
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
        if (transform != null)
        {
            transform.DOShakePosition(0f, 0.1f);
        }
        UpdateIntentText(IntentAction);
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

    public void Refresh()
    {
        foreach (Effect effect in GameEventSystem.Instance.RetrieveEffectsFor(null,null,this))
        {
            if (effect.Events.Contains(Events.OnSelect))
            {
                effect.ActivateLeft = effect.ActivateNumber;
            }
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
