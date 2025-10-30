using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [Header("Params")]
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
    [SerializeField] SpriteRenderer PermanentSpriteRenderer;

    [SerializeField] public EventReference SelectedSound;
    [SerializeField] public EventReference UnSelectedSound;


    [HideInInspector] public bool IsReward;
    [HideInInspector] public bool RewardTaken;
    [HideInInspector] public ShopSlot shopSlot;
    [HideInInspector] public bool isDragging = false;

    [HideInInspector] public Vector3 OriginalPos;
    [HideInInspector] public Quaternion OriginalRotation;

    [HideInInspector] public Card Card { get; private set; }

    [HideInInspector] public bool IsScryCard;
    [HideInInspector] public bool IsChoiceCard;
    [HideInInspector] public Effect EffectHolder;

    public void Setup(Card card)
    {
        Card = card;
        Title.text = card.Title;
        name = Title.text;
        if (card.DecayCounter <= 0)
        {
            Description.text = card.Description;
        }
        else
        {
            Description.text = card.Description + "\n" + "Decay " + card.DecayCounter.ToString();
        }
        cost.text = card.cost.ToString();
        Image.sprite = card.Image;

        if (!card.IsSpell)
        {
            Life.gameObject.SetActive(true);
            Durability.gameObject.SetActive(true);

            Life.text = card.life.ToString();
            Durability.text = card.Durability.ToString() + "/" + card.MaxDurability.ToString();
        }
        else
        {
            Life.gameObject.SetActive(false);
            Durability.gameObject.SetActive(false);
        }
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
            CardViewHover.Instance.Show(this, pos);
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
            if (IsChoiceCard)
            {
                CardSystem.Instance.EffectChoosed = EffectHolder;
            }
            else
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

    void OnMouseUp()
    {
        if (isDragging)
        {
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
                    return;
                }
            }
            else
            {
                if (ManaSystem.Instance.HasEnoughMana(Card.cost))
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
                                        return;
                                    }
                                }
                            }

                            isDragging = false;
                            PlayCardGA playCardGA = new(Card);
                            playCardGA.CardActionner = Card;
                            ActionSystem.Instance.Perform(playCardGA);
                            CombatSystem.Instance.SpellCast_This_Turn++;
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
                                            if (!TargetSystem.Instance.limitationHasEnoughtTarget(effect.EffectTargetLimitations, effect.EffectTargetNumber,effect.MultiHit))
                                            {
                                                returnCardToHand(true);
                                                return;
                                            }
                                        }
                                    }
                                    isDragging = false;
                                    SummonGA summonGA = new(Card);
                                    ActionSystem.Instance.Perform(summonGA);
                                    CombatSystem.Instance.PermanentCast_This_Turn++;
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
        }
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
        if (isDragging)
        {
            if (IsScryCard)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = -1;
                transform.DOMove(mousePos, 0.25f).SetEase(Ease.OutCubic);
            }
            else
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                transform.DOMove(mousePos, 0.25f).SetEase(Ease.OutCubic);
            }
        }
    }

    public void ActiveSelectEffect()
    {
        PermanentSpriteRenderer.color = Color.red;
        RuntimeManager.PlayOneShot(SelectedSound);
    }

    public void RemoveSelectEffect(bool SoundUp)
    {
        if (SoundUp)
        {
            PermanentSpriteRenderer.color = Color.white;
            RuntimeManager.PlayOneShot(UnSelectedSound);            
        }
    }
}
