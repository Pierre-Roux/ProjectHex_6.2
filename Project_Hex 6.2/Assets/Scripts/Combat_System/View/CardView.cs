using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] public bool IsHoverCard;
    [SerializeField] public TMP_Text cost;
    [SerializeField] public TMP_Text Title;
    [SerializeField] public TMP_Text Description;
    [SerializeField] public SpriteRenderer Image;
    [SerializeField] public TMP_Text Life;
    [SerializeField] public TMP_Text Durability;
    [SerializeField] public GameObject Wrapper;
    [SerializeField] private LayerMask DropAreaLayer;
    [SerializeField] private LayerMask DropDeckLayer;
    [SerializeField] private LayerMask DropDiscardLayer;
    [SerializeField] public SpriteRenderer PermanentSpriteRenderer;

    [SerializeField] public EventReference CardSelectedSound;
    [SerializeField] public EventReference CardUnSelectedSound;


    [HideInInspector] public bool IsReward;
    [HideInInspector] public bool RewardTaken;
    [HideInInspector] public ShopSlot shopSlot;
    [HideInInspector] public bool isDragging = false;

    [HideInInspector] public Vector3 OriginalPos;
    [HideInInspector] public Quaternion OriginalRotation;

    [HideInInspector] public Card Card { get; private set; }

    [HideInInspector] public bool IsScryCard;
    [HideInInspector] public bool IsVisualDeckCard;
    [HideInInspector] public bool IsChoiceCard;
    [HideInInspector] public bool IsInvalidChoice;
    [HideInInspector] public bool IsPayXValidate;
    [HideInInspector] public bool WhaitForPayX;
    [HideInInspector] public int PayXValue;
    [HideInInspector] public Effect EffectHolder;

    public void Setup(Card card)
    {
        Card = card;
        if (!IsHoverCard && !IsVisualDeckCard && !IsChoiceCard && !IsScryCard && !IsReward)
        {
            Card.RefCardView = this;
        }
        Title.text = Card.Title;
        name = Title.text;
        Description.text = Card.Description;
        UpdateCostText();
        Image.sprite = Card.Image;

        if (!Card.IsSpell)
        {
            Life.gameObject.SetActive(true);
            Durability.gameObject.SetActive(true);

            Life.text = Card.life.ToString();
            UpdateDurabilityText();
        }
        else
        {
            Life.gameObject.SetActive(false);
            Durability.gameObject.SetActive(false);
        }

        if (AudioManager.Instance.IsValid(card.CardSelectedSound)) CardSelectedSound = card.CardSelectedSound;
        if (AudioManager.Instance.IsValid(card.CardUnSelectedSound)) CardUnSelectedSound = card.CardUnSelectedSound;

        //UpdateDescription();
    }

    public void UpdateDescription()
    {
        List<string> effectDescriptions = new();

        foreach (Effect effect in Card.Effects)
        {
            if (effect is EffectGroup group)
            {
                foreach (Effect subEffect in group.EffectGroups)
                {
                    effectDescriptions.Add(" And ");
                    effectDescriptions.Add(subEffect.GetParsedDescription());
                }
            }
            else if (effect is ChoiceEffect choice)
            {
                foreach (Effect subEffect in choice.EffectsForPlayerChoice)
                {
                    effectDescriptions.Add(" Or ");
                    effectDescriptions.Add(subEffect.GetParsedDescription());
                }
            }
            else
            {
                effectDescriptions.Add(effect.GetParsedDescription());
            }
        }

        Description.text = string.Join("\n", effectDescriptions);
    }

    public void UpdateCostText()
    {
        cost.text = Mathf.Max(0, Card.cost + Card.BonusCost).ToString();
    }
    
    public void UpdateDurabilityText()
    {
        Durability.text = Card.Durability.ToString() + "/" + Card.MaxDurability.ToString();        
    }

    void OnMouseEnter()
    {
        if (!IsReward)
        {
            if (!TargetSystem.Instance.CardTargetingActive)
            {
                if (ActionSystem.Instance.IsPerforming) return;
                if (!CombatSystem.Instance.Interactable) return;
            }
            if (isDragging) return;
            Wrapper.SetActive(false);
            Vector3 pos = new(transform.position.x, transform.position.y + 1, 0);
            if (IsVisualDeckCard)
            {
                CardViewHover.Instance.Show(this, pos, true);
            }
            else
            {
                CardViewHover.Instance.Show(this, pos);
            }
            if (!AudioManager.Instance.IsValid(Card.HoverCardSound))
            {
                RuntimeManager.PlayOneShot(AudioManager.Instance.HoverCardSound);
            }
            else
            {
                RuntimeManager.PlayOneShot(Card.HoverCardSound);
            }
        }
    }

    void OnMouseExit()
    {
        if (!IsReward)
        {
            if (isDragging) return;
            CardViewHover.Instance.Hide();
            Wrapper.SetActive(true);
        }
    }

    void OnMouseDown()
    {
        if (!IsReward)
        {
            if (IsChoiceCard && !IsInvalidChoice)
            {
                if (EffectHolder != null)
                {
                    CardSystem.Instance.EffectChoosed = EffectHolder;
                }
                else
                {
                    ZZZ_EmptyEffect DoNothingEffect = new ZZZ_EmptyEffect();
                    CardSystem.Instance.EffectChoosed = DoNothingEffect;
                }
            }
            else
            {
                if (!IsVisualDeckCard)
                {
                    if (!IsScryCard)
                    {
                        if (ActionSystem.Instance.IsPerforming) return;
                        if (!CombatSystem.Instance.Interactable) return;
                    }
                    isDragging = true;
                    transform.rotation = Quaternion.identity;
                    CardViewHover.Instance.Hide();
                    Wrapper.SetActive(true);                     
                }               
            }
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            StartCoroutine(ManageMouseUp());
        }
    }

    public IEnumerator ManageMouseUp()
    {
        if (Card.PayX)
        {
            bool EffectCanceled = false;
            yield return StartCoroutine(CardSystem.Instance.ManagePayX(false, (result) =>
            {
                EffectCanceled = result;
            },null,this));

            if (EffectCanceled)
            {
                returnCardToHand();
                yield break;
            }
        }
        if (IsScryCard)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + new Vector3(0, 0, -1), Vector3.forward, out hit, 10f, DropDeckLayer))
            {
                isDragging = false;
                CardSystem.Instance.ScryCardViews.Remove(this);
                CardSystem.Instance.drawPile.PutTop(new[] { this.Card });
                Destroy(this.gameObject);

            }
            else if (Physics.Raycast(transform.position + new Vector3(0, 0, -1), Vector3.forward, out hit, 10f, DropDiscardLayer))
            {
                isDragging = false;
                CardSystem.Instance.ScryCardViews.Remove(this);
                CardSystem.Instance.discardPile.PutTop(new[] { this.Card });
                Destroy(this.gameObject);
            }
            else
            {
                isDragging = false;
                yield break;
            }
        }
        else
        {
            if (ManaSystem.Instance.HasEnoughMana(Mathf.Max(0, Card.cost + Card.BonusCost)))
            {
                if (Card.IsSpell)
                {
                    if (Physics.Raycast(transform.position + new Vector3(0, 0, -1), Vector3.forward, out RaycastHit hit, 10f, DropAreaLayer))
                    {
                        foreach (var effect in Card.Effects)
                        {
                            if (effect?.EffectTargetLimitations != null && effect.EffectTargetLimitations.Count > 0)
                            {
                                if (effect.MultiHit < 1) effect.MultiHit = 1;
                                if (!TargetSystem.Instance.limitationHasEnoughtTarget(effect.EffectTargetLimitations, effect.EffectTargetNumber, effect.MultiHit))
                                {
                                    returnCardToHand(true);
                                    yield break;
                                }
                            }
                        }

                        isDragging = false;   
                        PlayCardGA playCardGA = new(Card);
                        playCardGA.CardActionner = Card;
                        ActionSystem.Instance.Perform(playCardGA);
                        CounterSystem.Instance.Add(CounterType.SpellCast_This_Turn);
                        CounterSystem.Instance.Add(CounterType.SpellCast_Since_Load);
                    }
                    else
                    {
                        returnCardToHand();
                    }
                }
                else
                {
                    if (Physics.Raycast(transform.position + new Vector3(0, 0, -1), Vector3.forward, out RaycastHit hit, 10f, DropAreaLayer))
                    {
                        GameObject Parent = null;
                        switch (Card.permanentArea)
                        {
                            case PermanentArea.Weapon:
                                Parent = CombatSystem.Instance.PlayerWeaponZone.gameObject;
                                break;
                            case PermanentArea.Shield:
                                Parent = CombatSystem.Instance.PlayerShieldZone.gameObject;
                                break;
                            case PermanentArea.Support:
                                Parent = CombatSystem.Instance.PlayerSupportZone.gameObject;
                                break;
                            default:
                                Debug.LogError("No Type For Perm " + Card.data.name);
                                break;
                        }
                        if (Parent != null)
                        {
                            int childCount = Parent.transform.childCount;
                            if (childCount >= CombatSystem.Instance.MaxPermPlayer)
                            {
                                // LimitReached
                                returnCardToHand(true);
                            }
                            else
                            {
                                foreach (var effect in Card.Effects)
                                {
                                    if (effect?.EffectTargetLimitations != null && effect.EffectTargetLimitations.Count > 0)
                                    {
                                        if (effect.MultiHit < 1) effect.MultiHit = 1;
                                        if (!TargetSystem.Instance.limitationHasEnoughtTarget(effect.EffectTargetLimitations, effect.EffectTargetNumber, effect.MultiHit))
                                        {
                                            returnCardToHand(true);
                                            yield break;
                                        }
                                    }
                                }
                                isDragging = false;
                                SummonGA summonGA = new(Card);
                                ActionSystem.Instance.Perform(summonGA);
                                CounterSystem.Instance.Add(CounterType.PermanentCast_This_Turn);
                                CounterSystem.Instance.Add(CounterType.PermanentCast_Since_Load);
                            }
                        }
                    }
                    else
                    {
                        returnCardToHand();
                    }
                }
            }
            else
            {
                returnCardToHand(true);
            }
        }

        yield return null;
    }

    public void returnCardToHand(bool ErrorSound = false)
    {
        isDragging = false;
        transform.DOMove(OriginalPos, 0.25f).SetEase(Ease.InOutBack);
        transform.DORotate(OriginalRotation.eulerAngles, 0.25f).SetEase(Ease.OutCubic);
        if (ErrorSound)
        {
            if (!AudioManager.Instance.IsValid(Card.CannotPlayCardSound))
            {
                RuntimeManager.PlayOneShot(AudioManager.Instance.CannotPlayCardSound);
            }
            else
            {
                RuntimeManager.PlayOneShot(Card.CannotPlayCardSound);
            }            
        }
    }

    void Update()
    {
        if (isDragging && !WhaitForPayX)
        {
            if (IsScryCard)
            {
                Vector3 mousePos = GetMouseWorldPositionOnZ(-1);
                transform.DOMove(mousePos, 0.25f).SetEase(Ease.OutCubic);
            }
            else
            {
                Vector3 mousePos = GetMouseWorldPositionOnZ(0);
                transform.DOMove(mousePos, 0.25f).SetEase(Ease.OutCubic);
            }           
        }
    }

    public static Vector3 GetMouseWorldPositionOnZ(float z)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, z));

        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }

    public void ActiveSelectEffect()
    {
        PermanentSpriteRenderer.color = Color.red;
        RuntimeManager.PlayOneShot(CardSelectedSound);
    }

    public void RemoveSelectEffect(bool SoundUp)
    {
        PermanentSpriteRenderer.color = Color.white;
        if (SoundUp)
        {
            RuntimeManager.PlayOneShot(CardUnSelectedSound);
        }
    }

    public void UnvalidChoiceCard()
    {
        IsInvalidChoice = true;
        Color c = PermanentSpriteRenderer.color;
        c.a = 0.3f;
        PermanentSpriteRenderer.color = c;
    }
}
