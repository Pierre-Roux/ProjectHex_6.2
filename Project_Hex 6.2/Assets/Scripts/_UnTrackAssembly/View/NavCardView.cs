using UnityEngine;
using DG.Tweening;
using FMODUnity;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class NavCardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Params")]
    [SerializeField] private float hoverScale = 1.5f;
    [SerializeField] private float HoverScaleAnimationSpeed = 10f;

    [SerializeField] public TMP_Text Title;
    [SerializeField] public TMP_Text Description;
    [SerializeField] public Image ImageUI;
    [SerializeField] public Image BackGroundImage;

    [HideInInspector] public NavCardData NavCard { get; private set; }

    public void Setup(NavCardData navCard)
    {
        NavCard = navCard;
        ImageUI.sprite = NavCard.Image;
        BackGroundImage.sprite = NavCard.BackgroundImage;
        Title.text = NavCard.Title;
        Description.text = NavCard.Description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    // Traitement de la carte sur l'UI
    public void OnPointerDown(PointerEventData eventData)
    {
        NavigationSystem NavSys = NavigationSystem.Instance;

        if (NavSys.Interractable == false) return;

        if (NavCard.navCardType == NavCardType.fight)
        {
            NavSys.Interractable = false;
            StartCoroutine(NavSys.HandleFight());
        }
        else if (NavCard.navCardType == NavCardType.eliteFight)
        {
            NavSys.Interractable = false;
            StartCoroutine(NavSys.HandleEliteFight());
        }
        else if (NavCard.navCardType == NavCardType.shop)
        {
            NavSys.Interractable = false;
            StartCoroutine(NavSys.HandleShop());
        }
        else if (NavCard.navCardType == NavCardType.events)
        {
            NavSys.Interractable = false;
            StartCoroutine(NavSys.HandleEvent(NavCard));
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {

    }

}
