using DG.Tweening;
using UnityEngine;

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private CardView cardViewPrefab;
    [SerializeField] private CardView cardViewPrefabUI;

    public CardView CreateCardView(Card Card, Vector3 position, Quaternion rotation, Transform Parent = null)
    {
        CardView cardView = Instantiate(cardViewPrefab, position, rotation, Parent);
        cardView.Setup(Card);

        if (Card.Effects != null)
        {
            foreach (Effect effect in Card.Effects)
            {
                effect.CardActionner = Card;
            }
        }

        cardView.transform.localScale = Vector3.zero;
        cardView.transform.DOScale(cardViewPrefab.transform.localScale, 0.15f);
        return cardView;
    }

    public CardView CreateUICardView(Card Card, Vector3 position, Quaternion rotation, Transform Parent = null)
    {
        CardView cardView = Instantiate(cardViewPrefabUI, Parent);

        RectTransform rt = cardView.GetComponent<RectTransform>();
        rt.localPosition = position;
        rt.localRotation = rotation;

        cardView.Setup(Card);
        if (Card.Effects != null)
        {
            foreach (Effect effect in Card.Effects)
            {
                if (effect != null)
                {
                    effect.CardActionner = Card;   
                }
            }
        }

        rt.localScale = Vector3.zero;
        rt.DOScale(0.45f, 0.20f);
        return cardView;
    }
}
