using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CardView : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler,IPointerUpHandler
{
    [Header("Params")]
    [SerializeField] public bool IsUI;
    [SerializeField] private float hoverScale = 1.5f;
    [SerializeField] private float HoverScaleAnimationSpeed = 10f;

    [SerializeField] public TMP_Text cost;
    [SerializeField] public TMP_Text Title;
    [SerializeField] public TMP_Text Description;
    [SerializeField] public SpriteRenderer ImageSpriteRenderer;
    [SerializeField] public SpriteRenderer BackGroundSpriteRenderer;
    [SerializeField] public Image ImageUI;
    [SerializeField] public Image BackGroundImage;
    [SerializeField] public TMP_Text Life;
    [SerializeField] public TMP_Text Power;
    [SerializeField] public TMP_Text Durability;
    [SerializeField] public GameObject Wrapper;
    [SerializeField] private LayerMask DropAreaLayer;
    [SerializeField] private LayerMask DropDeckLayer;
    [SerializeField] private LayerMask DropDiscardLayer;

    [SerializeField] public EventReference CardSelectedSound;
    [SerializeField] public EventReference CardUnSelectedSound;

    [HideInInspector] public bool IsReward;
    [HideInInspector] public bool IsShopCard;
    [HideInInspector] public bool RewardTaken;
    [HideInInspector] public ShopSlot shopSlot;
    [HideInInspector] public bool isDragging = false;

    [HideInInspector] public int OriginalSortingOrder;

    [HideInInspector] public Card Card { get; private set; }

    [HideInInspector] public bool IsScryCard;
    [HideInInspector] public bool IsChoiceCard;
    [HideInInspector] public bool IsInvalidChoice;
    [HideInInspector] public bool IsPayXValidate;
    [HideInInspector] public bool WhaitForPayX;
    [HideInInspector] public int PayXValue;
    [HideInInspector] public Effect EffectHolder;

    [HideInInspector] public int CurrentCost;
    [SerializeField] public int CardBonuspassiveCost;


    public void Setup(Card card)
    {
        if (TryGetComponent<SortingGroup>(out var sr))
        {
            OriginalSortingOrder = sr.sortingOrder;
        }

        Card = card;

        if (!IsChoiceCard && !IsScryCard && !IsReward)
        {
            Card.RefCardView = this;
        }

        Title.text = Card.Title;
        name = Title.text;
        Description.text = Card.Description;
        UpdateCostText();

        if (IsUI)
        {
            ImageUI.sprite = Card.SpriteImage;
            ImageUI.SetNativeSize();
        }
        else
        {
            ImageSpriteRenderer.sprite = Card.SpriteImage;
        }

        if (!Card.IsSpell)
        {
            Life.gameObject.SetActive(true);
            Power.gameObject.SetActive(true);
            Durability.gameObject.SetActive(true);

            Power.text = Card.Power.ToString();
            Life.text = Card.Life.ToString();
            UpdateDurabilityText();
        }
        else
        {
            Life.gameObject.SetActive(false);
            Power.gameObject.SetActive(false);
            Durability.gameObject.SetActive(false);
        }

        if (AudioManager.Instance.IsValid(card.CardSelectedSound)) CardSelectedSound = card.CardSelectedSound;
        if (AudioManager.Instance.IsValid(card.CardUnSelectedSound)) CardUnSelectedSound = card.CardUnSelectedSound;

        StartCoroutine(RealTimeUpdate());
        //UpdateDescription();
    }

    /*public void UpdateDescription()
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
    }*/

    private IEnumerator RealTimeUpdate()
    {
        while (true)
        {
            UpdateCost();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void UpdateCost()
    {
        Card.CalculateCost();
        CardBonuspassiveCost = Card.passiveCost;
        CurrentCost = Card.CurrentCost;
        UpdateCostText();
    }

    public void UpdateCostText()
    {
        cost.text = Mathf.Max(0, CurrentCost).ToString();
    }
    
    public void UpdateDurabilityText()
    {
        Durability.text = Card.Durability.ToString() + "/" + Card.MaxDurability.ToString();        
    }

    // UI HOVER SYSTEM
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsUI) return;
        Wrapper.transform.DOKill(); 
        Wrapper.transform.DOScale(Vector3.one * hoverScale, HoverScaleAnimationSpeed).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsUI) return;
        Wrapper.transform.DOKill();
        Wrapper.transform.DOScale(Vector3.one, HoverScaleAnimationSpeed).SetEase(Ease.OutBack);
    }

    // Traitement des manipulations de carte sur l'UI
    public void OnPointerDown(PointerEventData eventData)
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
                if (!IsScryCard)
                {
                    if (ActionSystem.Instance.IsPerforming) return;
                    if (!CombatSystem.Instance.Interactable) return;
                }
                isDragging = true;
                Wrapper.transform.DOKill();
                Wrapper.transform.DOLocalMove(Vector3.zero, HoverScaleAnimationSpeed).SetEase(Ease.OutBack);
                Wrapper.transform.DOLocalRotate(Vector3.zero, HoverScaleAnimationSpeed).SetEase(Ease.OutBack);          
                Wrapper.SetActive(true);
            }
        }
        else if (IsReward)
        {
            if (IsShopCard)
            {
                if (ShopSlotSystem.Instance.ShopInterractable)
                {
                    StartCoroutine(ShopSlotSystem.Instance.BuyCard(Card,this));
                }
            }
            else
            {
                RewardSystem.Instance.PickCardFromRewardPanel(this);
            }
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging && IsUI)
        {
            if (IsScryCard || IsReward)
            {
                StartCoroutine(ManageMouseUp());
            }
        }
    }

    // Traitement des manipulations de carte en jeu
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
            if (TryGetComponent<SortingGroup>(out var sr))
            {
                sr.sortingOrder = 10;
            }
            Wrapper.transform.DOKill(); 
            Wrapper.transform.DOScale(Vector3.one * hoverScale, HoverScaleAnimationSpeed).SetEase(Ease.OutBack);
            Wrapper.transform.DOLocalMove(new Vector3(0, 7, 0f), HoverScaleAnimationSpeed).SetEase(Ease.OutBack);
            Wrapper.transform.DOLocalRotate(new Vector3(-5,0,-transform.eulerAngles.z), HoverScaleAnimationSpeed).SetEase(Ease.OutBack);

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
            Wrapper.transform.DOKill(); 
            Wrapper.transform.DOScale(Vector3.one, HoverScaleAnimationSpeed).SetEase(Ease.OutBack);
            Wrapper.transform.DOLocalMove(Vector3.zero, HoverScaleAnimationSpeed).SetEase(Ease.OutBack);
            Wrapper.transform.DOLocalRotate(Vector3.zero, HoverScaleAnimationSpeed).SetEase(Ease.OutBack);          
            if (TryGetComponent<SortingGroup>(out var sr))
            {
                sr.sortingOrder = OriginalSortingOrder;
            }
        }
    }

    void OnMouseDown()
    {
        if (!IsReward)
        {
            if (ActionSystem.Instance.IsPerforming) return;
            if (!CombatSystem.Instance.Interactable) return;
            isDragging = true;
            Wrapper.transform.DOKill();
            Wrapper.transform.DOLocalMove(Vector3.zero, HoverScaleAnimationSpeed).SetEase(Ease.OutBack);
            Wrapper.transform.DOLocalRotate(new Vector3(-5,0,-transform.eulerAngles.z), HoverScaleAnimationSpeed).SetEase(Ease.OutBack);                          
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
            // Récupérer le canvas de l'UI
            Canvas canvas = GetComponentInParent<Canvas>();
            GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(pointerData, results);

            bool hitDropDeckLayer = false;
            bool hitDropDiscardLayer = false;

            foreach (var result in results)
            {
                // Vérifie par layer ou tag
                if (((1 << result.gameObject.layer) & DropDeckLayer) != 0)
                {
                    hitDropDeckLayer = true;
                }
                else if (((1 << result.gameObject.layer) & DropDiscardLayer) != 0)
                {
                    hitDropDiscardLayer = true;
                }
            }

            if (hitDropDeckLayer)
            {
                isDragging = false;
                CardSystem.Instance.ScryCardViews.Remove(this);
                CardSystem.Instance.drawPile.PutTop(new[] { Card });
                DOTween.Kill(gameObject);
                Destroy(gameObject);
            }
            else if (hitDropDiscardLayer)
            {
                isDragging = false;
                CardSystem.Instance.ScryCardViews.Remove(this);
                CardSystem.Instance.discardPile.PutTop(new[] { Card });
                DOTween.Kill(gameObject);
                Destroy(gameObject);
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
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, 15f, DropAreaLayer);

                bool hitDropArea = false;

                foreach (var hit in hits)
                {
                    if (hit.collider != null)
                    {
                        hitDropArea = true;
                    }
                }

                if (Card.IsSpell)
                {
                    if (hitDropArea)
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
                    if (hitDropArea)
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
        HandView handView = HandView.Instance;
        transform.DOKill();
        handView.UpdateCardPos(0.15f);
        Wrapper.transform.DOKill(); 
        Wrapper.transform.DOScale(Vector3.one, HoverScaleAnimationSpeed).SetEase(Ease.OutBack);
        Wrapper.transform.DOLocalMove(Vector3.zero, HoverScaleAnimationSpeed).SetEase(Ease.OutBack);
        Wrapper.transform.DOLocalRotate(new Vector3(0, 0, 0), HoverScaleAnimationSpeed).SetEase(Ease.OutBack);
        if (TryGetComponent<SortingGroup>(out var sr))
        {
            sr.sortingOrder = OriginalSortingOrder;
        }
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
                RectTransform rect = (RectTransform)transform;
                Canvas canvas = GetComponentInParent<Canvas>();

                RectTransform parentRect = rect.parent as RectTransform;

                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect,
                    Input.mousePosition,
                    canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                    out pos
                );

                rect.DOKill();
                rect.DOAnchorPos(pos, 0.25f).SetEase(Ease.OutCubic);
            }
            else
            {
                Vector3 mousePos = GetMouseWorldPositionOnZ(0);
                DOTween.Kill(gameObject);
                transform.DOMove(mousePos, 0.25f).SetEase(Ease.OutCubic);
                transform.DORotate(Vector3.zero,0.25f);
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
        if (IsUI)
        {
            BackGroundImage.color = Color.red;
        }
        else
        {
            BackGroundSpriteRenderer.color = Color.red;
        }
        RuntimeManager.PlayOneShot(CardSelectedSound);  
    }

    public void RemoveSelectEffect(bool SoundUp)
    {
        if (IsUI)
        {
            BackGroundImage.color = Color.white;
        }
        else
        {
            BackGroundSpriteRenderer.color = Color.white;
        }
        
        if (SoundUp)
        {
            RuntimeManager.PlayOneShot(CardUnSelectedSound);
        }
    }

    public void UnvalidChoiceCard()
    {
        IsInvalidChoice = true;
        if (IsUI)
        {
            Color c = BackGroundImage.color;
            c.a = 0.3f;
            BackGroundImage.color = c;
        }
        else
        {
            Color c = BackGroundSpriteRenderer.color;
            c.a = 0.3f;
            BackGroundSpriteRenderer.color = c;
        }
    }
}
