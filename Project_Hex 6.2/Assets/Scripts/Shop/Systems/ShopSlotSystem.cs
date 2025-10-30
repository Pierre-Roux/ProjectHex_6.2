using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using FMODUnity;

public class ShopSlotSystem : Singleton<ShopSlotSystem>
{
    [SerializeField] public List<ShopSlot> ShopSlots;
    [SerializeField] public int PromoChance;
    [SerializeField] private LayerMask TargetingLayerMask;
    [SerializeField] public Transform PilePoint;
    [SerializeField] private GameObject CursorGameobject;

    [HideInInspector] public List<CardData> GlobalCardList;
    [HideInInspector] public bool ShopSelectionMode;
    [HideInInspector] public int CurrentMoney;

    private bool isBuyingCard;

    public void Start()
    {
        GlobalCardList = DataBase.Instance.GlobalCardList;
        SetupShop();
        ShopSelectionMode = true;
    }

    public void SetupShop()
    {
        List<CardData> tempList = new List<CardData>(GlobalCardList);

        foreach (ShopSlot shopSlot in ShopSlots)
        {
            if (tempList.Count == 0)
            {
                Debug.LogWarning("Pas assez de cartes dans GlobalCardList pour remplir tous les slots !");
                break;
            }

            int randomIndex = Random.Range(0, tempList.Count);
            Card selectedCard = new Card(tempList[randomIndex]);
            // Retire la carte choisie de la liste temporaire pour éviter les doublons
            tempList.RemoveAt(randomIndex);

            CardView cardView = CardViewCreator.Instance.CreateCardViewRewardUI(
                selectedCard,
                shopSlot.CardParent.transform.position,
                shopSlot.CardParent.transform.rotation,
                shopSlot,
                shopSlot.CardParent.transform
            );

            cardView.shopSlot = shopSlot;

            int PromoNumber = Random.Range(0,PromoChance);
            if(PromoNumber == 0)
            {
                selectedCard.Money_Cost = (selectedCard.Money_Cost/2);
                shopSlot.Cost.color = new Color(0f, 0.8f, 0f, 1f);
            }

            shopSlot.Cost.text = selectedCard.Money_Cost.ToString();
        }

        CurrentMoney = DataBase.Instance.Money;
    }

    public void Update()
    {
        if (ShopSelectionMode)
        {
            if (Input.GetMouseButtonDown(0)) // 0 = clic gauche 1 = clic droit
            {
                Vector3 origin = CursorGameobject.transform.position + new Vector3(0, 0, -1);
                Vector3 direction = Vector3.forward;
                float distance = 10f;

                Debug.DrawRay(origin, direction * distance, Color.red, 1f);

                RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance, TargetingLayerMask);

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider != null && hit.transform.TryGetComponent(out CardView cardView))
                    {
                        if (cardView.IsReward && CurrentMoney - cardView.Card.Money_Cost >= 0 && !isBuyingCard)
                        {
                            StartCoroutine(BuyCard(cardView.Card, cardView));
                        }
                    }
                }
            }
        }
    }

    public IEnumerator BuyCard(Card card, CardView cardView)
    {
        isBuyingCard = true;
        DataBase.Instance.DeckList.Add(card.data);
        CurrentMoney -= cardView.Card.Money_Cost;
        DataBase.Instance.Money = CurrentMoney;
        Money_Manager.Instance.UpdateMoneyText();

        RuntimeManager.PlayOneShot(AudioManager.Instance.BuyCardSound);
        CardView newCardView = CardViewCreator.Instance.CreateCardViewRewardUI(cardView.Card,cardView.shopSlot.CardParent.transform.position,cardView.shopSlot.CardParent.transform.rotation, cardView.shopSlot ,cardView.shopSlot.CardParent.gameObject.transform);
        newCardView.shopSlot = cardView.shopSlot;

        cardView.transform.DOMove(PilePoint.position, 0.20f);
        Tween tween = cardView.transform.DOScale(Vector3.zero, 0.15f);
        yield return tween.WaitForCompletion();
        Destroy(cardView.gameObject);
        isBuyingCard = false;
    }

    public void CloseShop()
    {
        SceneManager.LoadScene("TransitionScene");
    }
}
