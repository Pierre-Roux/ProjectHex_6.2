using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using FMODUnity;
using TMPro;

public class ShopSlotSystem : Singleton<ShopSlotSystem>
{
    [SerializeField] public List<ShopSlot> ShopSlots;
    [SerializeField] public int PromoChance;
    [SerializeField] public int RefreshCost;
    [SerializeField] public Transform PilePoint;
    [SerializeField] private TMP_Text RefreshCost_Text;

    [HideInInspector] public List<CardData> PotentialRewards;
    [HideInInspector] public bool ShopInterractable;
    [HideInInspector] public int CurrentMoney;

    public void Start()
    {
        DataBase dataBase = DataBase.Instance;
        foreach (CardData cardData in dataBase.ColorLessCardPool.CardDataList)
        {
            PotentialRewards.Add(cardData.Clone());
        }
        foreach (CardData cardData in dataBase.ChoosedCardPool.CardDataList)
        {
            PotentialRewards.Add(cardData.Clone());
        }

        RefreshCost_Text.text = RefreshCost.ToString();
        CurrentMoney = DataBase.Instance.Money;

        StartCoroutine(SetupShop());
    }

    public IEnumerator SetupShop()
    {
        List<CardData> tempList = new List<CardData>(PotentialRewards);

        foreach (ShopSlot shopSlot in ShopSlots)
        {
            shopSlot.gameObject.SetActive(true);
            if (tempList.Count == 0)
            {
                Debug.LogWarning("Pas assez de cartes dans GlobalCardList pour remplir tous les slots !");
                break;
            }
            CardData cardDataSelected;

            if (shopSlot.OverrideRarity != -1)
            {
                cardDataSelected = PickWeightedRandomCard(tempList, shopSlot.OverrideRarity);
            }
            else
            {
                cardDataSelected = PickWeightedRandomCard(tempList);
            }

            if (cardDataSelected != null)
            {
                Card selectedCard = new Card(cardDataSelected);

                // Retire la carte choisie de la liste temporaire pour éviter les doublons
                tempList.Remove(cardDataSelected);

                CardView cardView = CardViewCreator.Instance.CreateUICardView(
                    selectedCard,
                    shopSlot.CardParent.transform.localPosition,
                    shopSlot.CardParent.transform.rotation,
                    shopSlot.CardParent.transform
                );
                cardView.IsReward= true;
                cardView.IsShopCard = true;

                yield return new WaitForSeconds(0.08f);

                cardView.shopSlot = shopSlot;
                shopSlot.HoldedCardView = cardView;

                selectedCard.Money_Cost = Mathf.RoundToInt((selectedCard.Money_Cost + shopSlot.PriceAdded) * shopSlot.PriceMultiply);

                int PromoNumber = Random.Range(0, PromoChance);
                if (PromoNumber == 0)
                {
                    selectedCard.Money_Cost = selectedCard.Money_Cost / 2;
                    shopSlot.Cost.color = new Color(0f, 0.8f, 0f, 1f);
                }

                shopSlot.Cost.text = selectedCard.Money_Cost.ToString();
            }
            else
            {
                shopSlot.gameObject.SetActive(false);
            }

        }
        ShopInterractable = true;
    }

    public void RefreshShop()
    {
        if (ShopInterractable && CurrentMoney-RefreshCost >= 0)
        {
            ShopInterractable = false;
            CurrentMoney -= RefreshCost;
            DataBase.Instance.Money = CurrentMoney;
            Money_Manager.Instance.UpdateMoneyText();
            StartCoroutine(RefreshShopCoroutine());            
        }
    }

    public IEnumerator RefreshShopCoroutine()
    {
        foreach (ShopSlot Slot in ShopSlots)
        {
            if (Slot.HoldedCardView != null)
            {
                Slot.Cost.color = new Color(1f, 1f, 1f, 1f);
                CardView cardView = Slot.HoldedCardView;
                Tween tween = cardView.transform.DOScale(Vector3.zero, 0.08f);
                yield return tween.WaitForCompletion();
                Destroy(cardView.gameObject);
            }
        }

        yield return SetupShop();
    }

    private Dictionary<int, List<CardData>> GroupByRarity(List<CardData> cards)
    {
        Dictionary<int, List<CardData>> byRarity = new();

        foreach (CardData card in cards)
        {
            if (!byRarity.ContainsKey(card.Rarity))
                byRarity[card.Rarity] = new List<CardData>();

            byRarity[card.Rarity].Add(card);
        }
        return byRarity;
    }

    private int WeightFromRarity(int rarity)
    {
        DataBase dataBase = DataBase.Instance;
        return rarity switch
        {
            0 => dataBase.Common_Weigh,
            1 => dataBase.Uncommon_Weigh, 
            2 => dataBase.Rare_Weigh,
            _ => 0     // Default
        };
    }

    public CardData PickWeightedRandomCard(List<CardData> cardDataList, int OnlyThisRarity = -1)
    {
        var grouped = GroupByRarity(cardDataList);

        if (OnlyThisRarity != -1)
        {
            if (!grouped.TryGetValue(OnlyThisRarity, out var list) || list.Count == 0)
                return null;

            return list[Random.Range(0, list.Count)];
        }
        else
        {
            int totalWeight = 0;
            foreach (var kvp in grouped)
            {
                int rarity = kvp.Key;
                int weight = WeightFromRarity(rarity);
                totalWeight += weight * kvp.Value.Count;
            }

            int roll = Random.Range(0, totalWeight);
            int cumulative = 0;

            foreach (var kvp in grouped)
            {
                int rarity = kvp.Key;
                int weight = WeightFromRarity(rarity);
                int groupWeight = weight * kvp.Value.Count;

                if (roll < cumulative + groupWeight)
                {
                    // Selection Aléatoire dans le pool de rareté ou est tombé le roll
                    return kvp.Value[Random.Range(0, kvp.Value.Count)];
                }

                cumulative += groupWeight;
            }
        }       
        return null;
    }

    public IEnumerator BuyCard(Card card, CardView cardView)
    {
        if (CurrentMoney - cardView.Card.Money_Cost >= 0)
        {
            ShopInterractable = false;
            DataBase.Instance.DeckList.Add(card.data);
            CurrentMoney -= cardView.Card.Money_Cost;
            DataBase.Instance.Money = CurrentMoney;
            Money_Manager.Instance.UpdateMoneyText();
            RuntimeManager.PlayOneShot(AudioManager.Instance.BuyCardSound);

            cardView.transform.DOMove(PilePoint.position, 0.20f);
            Tween tween = cardView.transform.DOScale(Vector3.zero, 0.15f);
            yield return tween.WaitForCompletion();


            if (cardView.shopSlot != null && cardView.shopSlot.InfinitSlot)
            {
                CardView newCardView = CardViewCreator.Instance.CreateUICardView(cardView.Card, cardView.shopSlot.CardParent.transform.position, cardView.shopSlot.CardParent.transform.rotation, cardView.shopSlot.CardParent.gameObject.transform);
                newCardView.shopSlot = cardView.shopSlot;
            }
            else
            {
                cardView.shopSlot.gameObject.SetActive(false);
            }

            Destroy(cardView.gameObject);
            ShopInterractable = true;
        }
    }

    public void CloseShop()
    {
        SceneManager.LoadScene("NavigationScene");
    }
}
