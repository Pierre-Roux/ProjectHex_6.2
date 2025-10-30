using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private CardView cardViewPrefab;

    public CardView CreateCardView(Card Card, Vector3 position, Quaternion rotation, Transform Parent = null)
    {
        CardView cardView = Instantiate(cardViewPrefab, position, rotation, Parent);
        cardView.transform.localScale = Vector3.zero;
        cardView.transform.DOScale(cardViewPrefab.transform.localScale, 0.15f);
        cardView.Setup(Card);
        foreach (Effect effect in Card.Effects)
        {
            effect.CardActionner = Card;
        }
        return cardView;
    }

    public CardView CreateCardViewRewardUI(Card Card, Vector3 position, Quaternion rotation, ShopSlot shopslot ,Transform Parent = null)
    {
        CardView cardView = Instantiate(cardViewPrefab, position, rotation, Parent);
        cardView.transform.localScale = Vector3.zero;
        cardView.transform.DOScale(50f, 0.15f);
        cardView.Setup(Card);
        cardView.IsReward = true;
        cardView.gameObject.GetComponent<SortingGroup>().sortingOrder = 1;
        cardView.gameObject.layer = LayerMask.NameToLayer("CardReward");
        return cardView;
    }
}
