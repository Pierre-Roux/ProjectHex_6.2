using DG.Tweening;
using UnityEngine;

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private CardView cardViewPrefab;
    [SerializeField] private CardView cardViewPrefabUI;
    [SerializeField] private GameObject CardParent;

    [SerializeField] private float CardInfoScale;
    [SerializeField] private float CardInfoAnimSpeed;
    [SerializeField] private Vector3 CardInfoPos;
    [SerializeField] private Quaternion CardInfoRot;


    public CardView CreateCardView(Card Card, Vector3 position, Quaternion rotation, Transform Parent = null)
    {
        CardView cardView;
        if (Parent != null)
        {
            cardView = Instantiate(cardViewPrefab, position, rotation, Parent.transform);
        }
        else
        {
            cardView = Instantiate(cardViewPrefab, position, rotation, CardParent.transform);
        }
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

    public CardView CreateUICardView(Card Card, Vector3 position, Quaternion rotation, Transform Parent = null, bool infoCard = false, EnemyPermanentData enemyPermanentData = null)
    {
        CardView cardView = Instantiate(cardViewPrefabUI, Parent);

        RectTransform rt = cardView.GetComponent<RectTransform>();
        
        rt.localPosition = position;
        rt.localRotation = rotation;

        if (Card == null)
        {
            cardView.SetupEnemy(enemyPermanentData);
        }
        else
        {
            cardView.Setup(Card);
        }
        

        if (infoCard)
        {
            cardView.IsInfoCard = true;
            cardView.transform.localScale = Vector3.zero;
            cardView.transform.DOScale(cardViewPrefab.transform.localScale * CardInfoScale, CardInfoAnimSpeed);
            cardView.transform.DOMove(position + CardInfoPos, CardInfoAnimSpeed);
        }
        else
        {
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
        }

        return cardView;
    }
}
