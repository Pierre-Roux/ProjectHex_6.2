using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FMODUnity;
using TMPro;
using UnityEngine;

public class EnemySlotView : MonoBehaviour
{
    [SerializeField] public GameObject WrapperGM;
    [SerializeField] public List<Effect> PossibleIntent;
    [SerializeField] public EnemyPermanentData PermanentData;
    [SerializeField] public TMP_Text LifeText;
    [SerializeField] public TMP_Text PowerText;
    [SerializeField] TMP_Text ArmorText;
    [SerializeField] public SpriteRenderer IntentLogo;
    [SerializeField] public SpriteRenderer DownLogo;
    [SerializeField] public SpriteRenderer LockLogo;
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] SpriteRenderer AuraSpriteRenderer;
    [SerializeField] public GameObject ShieldVisual;
    [SerializeField] public bool UnShieldable;

    [SerializeField] public EventReference DieSound;
    [SerializeField] public EventReference BeingDamageSound;
    [SerializeField] public EventReference BeingDamageOnArmorSound;
    [SerializeField] public EventReference ArmorBreakSound;
    [SerializeField] public EventReference CollateralSound;
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

    [HideInInspector] public Effect IntentAction;
    [HideInInspector] public int currentLife { get; set; }
    [HideInInspector] public int currentPower { get; set; }
    [HideInInspector] public int currentArmor { get; set; }
    [HideInInspector] public int MaxLife { get; set; }
    [HideInInspector] public bool IsCore { get; set; }
    [HideInInspector] public bool IsDisabled = false;
    [HideInInspector] public bool IsDead = false;
    [HideInInspector] public bool IsVirtualDead = false;
    [HideInInspector] public PermanentView VirtualTarget = null;
    [HideInInspector] public bool AsGoodTarget = false;
    [HideInInspector] public int VirtualHP = 0;
    [SerializeField] public bool TargetLocked = false;
    [HideInInspector] public Vector3 InitialPosition { get; set; }
    [HideInInspector] public int BonusPower { get; set; }
    [HideInInspector] public int BonusLife { get; set; }

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

    [HideInInspector] public List<GameAction> AffectedGA = new List<GameAction>();
    [HideInInspector] public List<KeyWord> KeyWords = new List<KeyWord>();
    [HideInInspector] public CounterModel InternCounters = new();
    [HideInInspector] public List<Effect> ToggleableEffects = new();

    [HideInInspector] private CombatSystem combatSystem;

    public void setup()
    {
        combatSystem = CombatSystem.Instance;
        KeyWords = new List<KeyWord>(PermanentData.KeyWords);
        InternCounters.ClearAll();
        PossibleIntent = PermanentData.PossibleIntent;
        spriteRenderer.sprite = PermanentData.PermanentImage;
        UpdatePower();
        currentLife = PermanentData.Life;
        UpdateMaxLife();
        currentLife = MaxLife;
        UpdateLifeText();
        currentArmor = PermanentData.Armor;
        UpdateArmorText();
        IsCore = PermanentData.IsCore;
        ShieldVisual.SetActive(false);
        RDMSequence = PermanentData.RDMSequence;
        IntentSequence = PermanentData.IntentSequence;
        LoopingSequence = PermanentData.LoopingSequence;
        deactivateAuraVisual();

        // Gestion des logos
        DownLogo.sprite = null;
        foreach (Effect effect in PossibleIntent)
        {
            if (ContainsDeathTrigger(effect))
            {
                DownLogo.sprite = combatSystem.DeathTriggerSprite;
                break;
            }
        }

        var WardbreakKeyword = KeyWords.FirstOrDefault(k => k.keyWordType == KeyWordType.Collateral);
        if (WardbreakKeyword != null)
        {
            DownLogo.sprite = combatSystem.WardbreakSprite;
        }

        if (DownLogo.sprite == null)
        {
            DownLogo.gameObject.SetActive(false);
        }

        UpdateLockLogo(false);

        // Gestion de l'affichage des textes
        bool HasPower = false;
        foreach (Effect effect in PossibleIntent)
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
            LifeText.transform.localPosition = new Vector3(0, -0.2f, 0);
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

        if (IsCore)
        {
            DownLogo.gameObject.SetActive(false);
            IntentLogo.gameObject.SetActive(false);
            KeyWord keyWord = new(KeyWordType.Core, 0);
            KeyWords.Add(keyWord);
        }

        //Audio
        if (AudioManager.Instance.IsValid(PermanentData.DieSound)) DieSound = PermanentData.DieSound;
        if (AudioManager.Instance.IsValid(PermanentData.BeingDamageSound)) BeingDamageSound = PermanentData.BeingDamageSound;
        if (AudioManager.Instance.IsValid(PermanentData.BeingDamageOnArmorSound)) BeingDamageOnArmorSound = PermanentData.BeingDamageOnArmorSound;
        if (AudioManager.Instance.IsValid(PermanentData.ArmorBreakSound)) ArmorBreakSound = PermanentData.ArmorBreakSound;
        if (AudioManager.Instance.IsValid(PermanentData.CollateralSound)) CollateralSound = PermanentData.CollateralSound;
        if (AudioManager.Instance.IsValid(PermanentData.BeingArmorSound)) BeingArmorSound = PermanentData.BeingArmorSound;
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

    private bool ContainsDeathTrigger(Effect effect)
    {
        foreach (EventInfo eventInfo in effect.EventInfos)
        {
            if (eventInfo.Events == Events.OnSelfDeath)
            {
                return true;
            }
        }

        if (effect is EffectGroup effectGroup)
        {
            foreach (Effect childEffect in effectGroup.EffectGroups)
            {
                if (ContainsDeathTrigger(childEffect))
                    return true;
            }
        }

        if (effect is ChoiceEffect choiceEffect)
        {
            foreach (Effect childEffect in choiceEffect.EffectsForPlayerChoice)
            {
                if (ContainsDeathTrigger(childEffect))
                    return true;
            }
        }

        return false;
    }

    private void OnEnable()
    {
        if (CombatSystem.Instance != null)
        {
            CombatSystem.Instance.PassivesChanged += UpdatePower;
            CombatSystem.Instance.PassivesChanged += UpdateMaxLife;
        }
    }

    private void OnDisable()
    {
        if (CombatSystem.Instance != null)
        {
            CombatSystem.Instance.PassivesChanged -= UpdatePower;
            CombatSystem.Instance.PassivesChanged -= UpdateMaxLife;
        }
    }

    private void UpdatePower()
    {
        int oldPower = currentPower;
        CalculateBonusPower();
        currentPower = PermanentData.Power + BonusPower;

        if (currentPower > oldPower)
        {
            EventInfo eventInfo = new EventInfo(Events.WhenPermaGainParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Power);
            TriggerEventGA triggerEventGA = new(eventInfo, null, null, null, this);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Power);
            triggerEventGA = new(eventInfo, null, null, null, this);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }
        else if (oldPower > currentPower)
        {
            EventInfo eventInfo = new EventInfo(Events.WhenPermaLoseParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Power);
            TriggerEventGA triggerEventGA = new(eventInfo, null, null, null, this);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Power);
            triggerEventGA = new(eventInfo, null, null, null, this);
            ActionSystem.Instance.AddReaction(triggerEventGA);            
        }

        UpdatePowerText();
    }

    public void SetPosition(Vector3 pos)
    {
        InitialPosition = pos;
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

    public void UpdateLifeText()
    {
        LifeText.text = Mathf.Max(0,currentLife).ToString();
    }

    public void UpdatePowerText()
    {
        PowerText.text = Mathf.Max(0,currentPower).ToString();
    }

    public void UpdateIntent()
    {
        if (PossibleIntent.Count <= 0)
        {
            IntentLogo.gameObject.SetActive(false);
            return;
        } 
        Effect selectedEffect = null;

        if (RDMSequence)
        {
            List<Effect> valid = PossibleIntent.FindAll(e =>  e.EventInfos[0].Events == Events.EnemyTurn);

            if (valid.Count > 0)
            {
                selectedEffect = valid[UnityEngine.Random.Range(0, valid.Count)];
            }
        }
        else
        {
            if (IntentSequence.Count == 0)
            {
                IntentLogo.gameObject.SetActive(false);
                return;
            }

            if (sequenceIndex >= IntentSequence.Count)
            {
                if (LoopingSequence)
                    sequenceIndex = 0;
                else
                {
                    IntentLogo.gameObject.SetActive(false);
                    return;
                }
            }

            string currentKey = IntentSequence[sequenceIndex];
            if (currentKey != "")
            {
                for (int i = 0; i < PossibleIntent.Count; i++)
                {
                    for (int y = 0; y < PossibleIntent[i].EventInfos.Count; y++)
                    {
                        if (PossibleIntent[i].EventInfos[y].Events == Events.EnemyTurn && PossibleIntent[i].number == currentKey)
                        {
                            selectedEffect = PossibleIntent[i];
                        }                        
                    }
                }

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
            selectedEffect.CopyEnemyDataTo(IntentAction);
            if (IntentAction is EffectGroup)
            {
                IntentAction.Actionner = this.gameObject;
                EffectGroup group = (EffectGroup)IntentAction;
                foreach (var Effect in group.EffectGroups)
                {
                    Effect.Actionner = this.gameObject;
                }
                UpdateIntentLogo(selectedEffect); 
            }
            else
            {
                IntentAction.Actionner = this.gameObject;
                UpdateIntentLogo(selectedEffect);                
            }
        }
    }

    public void UpdateLockLogo(bool active)
    {
        if (active)
        {
            LockLogo.gameObject.SetActive(true);
        }
        else
        {
            LockLogo.gameObject.SetActive(false);
        }
    }

    public void UpdateIntentLogoColor(bool Lock)
    {
        if (Lock)
        {

            IntentLogo.color = Color.red;
        }
        else
        {
            IntentLogo.color = Color.orange;
        }
    }

    public void UpdateIntentLogo(Effect selectedEffect)
    {
        if (selectedEffect == null) return;

        Sprite sprite = null;
        switch (selectedEffect)
        {
            case DealDamageEffect dmg:
                if (dmg.targetModeInfo.targetMode == TargetMode.All)
                {
                    sprite = combatSystem.DamageAOESprite;
                }
                else if (dmg.MultiHit > 1)
                {
                    sprite = combatSystem.DamageMultiSprite;
                }
                else
                {
                    sprite = combatSystem.DamageMonoSprite;
                }
                break;

            case HealEffect heal:
                sprite = combatSystem.HealSprite;
                break;

            case ArmorEffect Armor:
                sprite = combatSystem.BuffSprite;
                break;

            case ShieldEffect shield:
                sprite = combatSystem.WardSprite;
                break;

            case AlterPowerEffect alter:
                if (alter.alterAmount > 0)
                {
                    sprite = combatSystem.BuffSprite;
                }
                else
                {
                    sprite = combatSystem.DebuffSprite;
                }
                break;
            case AlterPowerGridEffect alter:
                if (alter.Amount > 0)
                {
                    sprite = combatSystem.BuffSprite;
                }
                else
                {
                    sprite = combatSystem.DebuffSprite;
                }
                break;
            case AlterCostEffect alter:
                if (alter.alterAmount > 0)
                {
                    sprite = combatSystem.BuffSprite;
                }
                else
                {
                    sprite = combatSystem.DebuffSprite;
                }
                break;
            case AlterStaminaEffect alter:
                if (alter.alterAmount > 0)
                {
                    sprite = combatSystem.BuffSprite;
                }
                else
                {
                    sprite = combatSystem.DebuffSprite;
                }
                break;
            case GainHPEffect alter:
                if (alter.GainAmount > 0)
                {
                    sprite = combatSystem.BuffSprite;
                }
                else
                {
                    sprite = combatSystem.DebuffSprite;
                }
                break;
            case DisableEffect disable:
                sprite = combatSystem.DebuffSprite;
                break;
            case EnableEffect enable:
                sprite = combatSystem.BuffSprite;
                break;
            case InvocEffect invoc:
                sprite = combatSystem.InvocSprite;
                break;
            case LifeLossEffect LifeLoss:
                sprite = combatSystem.LifeLossSprite;
                break;
            case SacEffect Sac:
                sprite = combatSystem.DebuffSprite;
                break;
        }

        if (sprite == null)
        {
            IntentLogo.gameObject.SetActive(false);
        }
        else
        {
            IntentLogo.gameObject.SetActive(true);
            IntentLogo.sprite = sprite;
            UpdateIntentLogoColor(false);
        }
    }
    public int CalculateBonusPower()
    {
        int passiveBonus = combatSystem.GetPassive(BasicParam.Power, Enemy_Player_ENUM.Enemy, null, null, this);
        int finalDMG = BonusPower + passiveBonus;

        return finalDMG;
    }

    public int CalculateBonusLife()
    {
        int passiveBonus = combatSystem.GetPassive(BasicParam.Life,Enemy_Player_ENUM.Enemy,null,null,this);;
        int finalHP = BonusLife + passiveBonus;

        return finalHP;
    }

    public void UpdateMaxLife()
    {
        int oldMaxLife = MaxLife;
        int passiveBonus = CalculateBonusLife();
        MaxLife = PermanentData.Life + passiveBonus;

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

        if (currentLife <= 0 && !IsDead)
        {
            DieEnemySlotGA dieEnemySlotGA = new(this);
            ActionSystem.Instance.AddReaction(dieEnemySlotGA);
            IsDead = true;
        }

        if (MaxLife > oldMaxLife)
        {
            EventInfo eventInfo = new EventInfo(Events.WhenPermaGainParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Life);
            TriggerEventGA triggerEventGA = new(eventInfo, null, null, null, this);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Life);
            triggerEventGA = new(eventInfo, null, null, null, this);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }
        else if (oldMaxLife > MaxLife)
        {
            EventInfo eventInfo = new EventInfo(Events.WhenPermaLoseParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Life);
            TriggerEventGA triggerEventGA = new(eventInfo, null, null, null, this);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Life);
            triggerEventGA = new(eventInfo, null, null, null, this);
            ActionSystem.Instance.AddReaction(triggerEventGA);            
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

        PermanentView Pstriker = null;
        EnemySlotView Estriker = null;
        Card Cstriker = null;
        EventInfo eventInfo = new EventInfo();
        TriggerEventGA triggerEventGA;

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
            triggerEventGA = new(eventInfo, null, null, null, this);
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

            eventInfo = new EventInfo(Events.WhenPermaLoseParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Armor);
            triggerEventGA = new(eventInfo, null, null, null, this);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Armor);
            triggerEventGA = new(eventInfo, null, null, null, this);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            Amount = DamageAmountToLife;
        }

        if (Amount > 0)
        {
            eventInfo = new EventInfo(Events.WhenPermaLoseParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Life);
            triggerEventGA = new(eventInfo, null, null, null, this);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Life);
            triggerEventGA = new(eventInfo, null, null, null, this);
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

        EventInfo eventInfo = new EventInfo(Events.WhenPermaGainParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Life);
        TriggerEventGA triggerEventGA = new(eventInfo, null, null, null, this);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Life);
        triggerEventGA = new(eventInfo, null, null, null, this);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        RuntimeManager.PlayOneShot(BeingHealSound);
        transform.DOShakePosition(0f, 0.1f);
        UpdateLifeText();
    }

    public void TakeArmor(int Amount)
    {
        currentArmor += Amount;
        if (currentArmor < 0)
        {
            currentArmor = 0;
        }

        EventInfo eventInfo = new EventInfo(Events.WhenPermaGainParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Armor);
        TriggerEventGA triggerEventGA = new(eventInfo, null, null, null, this);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Armor);
        triggerEventGA = new(eventInfo, null, null, null, this);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        RuntimeManager.PlayOneShot(BeingArmorSound);
        transform.DOShakePosition(0f, 0.1f);
        UpdateArmorText();
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
                RuntimeManager.PlayOneShot(BeingShieldSound);
                if (!EnemyShielder.Contains(enemyShielder))
                {
                    EnemyShielder.Add(enemyShielder);
                    enemyShielder.GetComponent<EnemySlotView>().EnemyShielded.Add(this);
                }
            }
            if ((playerShielder != null) || (enemyShielder != null))
            {
                UpdateShield();
            }
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
        if ((playerShielder != null) || (enemyShielder != null))
        {
            UpdateShield();
        }       
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
            enemyShielder.GetComponent<EnemySlotView>().EnemyShielded.Remove(this);
            EnemyShielder.Remove(enemyShielder);
        }
        if ((playerShielder != null) || (enemyShielder != null))
        {
            UpdateShield();
        }
    }

    public void UpdateShield()
    {
        if (ShieldVisual != null)
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

        if (transform != null)
        {
            transform.DOShakePosition(0f, 0.1f);
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
        EventInfo eventInfo = new EventInfo(Events.OnSelfDamaged, Enemy_Player_ENUM.NULL, KeyWordType.NULL,BasicParam.NULL);
        TriggerEventGA triggerEventGA = new(eventInfo, null, null, null, this);
        ActionSystem.Instance.AddReaction(triggerEventGA);

        if (Amount > 0)
        {
            eventInfo = new EventInfo(Events.WhenPermaGainParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Life);
            triggerEventGA = new(eventInfo, null, null, null, this);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Life);
            triggerEventGA = new(eventInfo, null, null, null, this);
            ActionSystem.Instance.AddReaction(triggerEventGA);
        }
        else
        {
            eventInfo = new EventInfo(Events.WhenPermaLoseParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Life);
            triggerEventGA = new(eventInfo, null, null, null, this);
            ActionSystem.Instance.AddReaction(triggerEventGA);

            eventInfo = new EventInfo(Events.WhenPermaChangeParam, Enemy_Player_ENUM.Enemy, KeyWordType.NULL, BasicParam.Life);
            triggerEventGA = new(eventInfo, null, null, null, this);
            ActionSystem.Instance.AddReaction(triggerEventGA);            
        }

        currentLife -= Amount;
        if (currentLife <= 0 && !IsDead)
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
