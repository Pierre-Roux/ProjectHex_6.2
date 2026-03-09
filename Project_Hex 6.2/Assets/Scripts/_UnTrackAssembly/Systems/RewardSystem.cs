using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using FMODUnity;
using TMPro;
public class RewardSystem : Singleton<RewardSystem>
{

    [SerializeField] public Transform PilePoint;
    [SerializeField] public GameObject CardRewardPanel;
    [SerializeField] private Button ButtonCardReward;
    [SerializeField] private Button ButtonMoneyReward;
    [SerializeField] private TMP_Text ButtonMoneyReward_text;

    public List<CardData> PotentialRewards = new List<CardData>();

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

    public CardData PickWeightedRandomCard()
    {
        var grouped = GroupByRarity(PotentialRewards);

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
        return null;
    }

    public void GainCard(Card card, CardView cardView = null)
    {
        if (cardView != null)
        {
            RuntimeManager.PlayOneShot(AudioManager.Instance.TakeCardRewardSound);
            cardView.RewardTaken = true;
            StartCoroutine(GainCardAnim(cardView));
        }
        else
        {
            CardRewardPanel.SetActive(false);
        }

        DataBase.Instance.DeckList.Add(card.data);
    }

    public IEnumerator GainCardAnim(CardView cardView)
    {
        cardView.transform.DOMove(PilePoint.position, 0.20f);
        Tween tween = cardView.transform.DOScale(Vector3.zero, 0.15f);
        //Tween tween = cardView.transform.DOMove(discardPilePoint.position, 0.15f);
        yield return tween.WaitForCompletion();
        Destroy(cardView.gameObject);
        CardRewardPanel.SetActive(false);
    }

    public void PickCardFromRewardPanel(CardView cardView)
    {
        GainCard(cardView.Card, cardView);
        ButtonCardReward.interactable = false;
    }

    public void CloseCardRewardPanel()
    {
        CardRewardPanel.SetActive(false);
    }

    public void AddEndFightMoney()
    {
        DataBase.Instance.Money += CombatSystem.Instance.MoneyReward;
        ButtonMoneyReward.interactable = false;
    }

    public void UpdateEndFightMoneyText(int Amount)
    {
        ButtonMoneyReward_text.text = Amount.ToString();
    }
}
