using DG.Tweening;
using UnityEngine;

public class NavCardViewCreator : Singleton<NavCardViewCreator>
{
    [SerializeField] private NavCardView navCardViewPrefab;

    public NavCardView CreateNavCardView(NavCardData Card, Vector3 position, Quaternion rotation, Transform Parent = null)
    {
        NavCardView cardView = Instantiate(navCardViewPrefab, Parent);

        RectTransform rt = cardView.GetComponent<RectTransform>();
        rt.localPosition = position;
        rt.localRotation = rotation;

        cardView.Setup(Card);

        rt.localScale = Vector3.zero;
        rt.DOScale(0.45f, 0.20f);
        return cardView;
    }
}
