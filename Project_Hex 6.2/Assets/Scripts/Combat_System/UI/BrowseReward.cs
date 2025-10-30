using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class BrowseReward : MonoBehaviour
{
    [SerializeField] public GameObject VictoryPanel;
    [SerializeField] public GameObject CardRewardPanel;
    [SerializeField] public GameObject RewardPanelContent;

    private List<Card> CardChoice = new List<Card>();
    public void OnClick()
    {
        CardChoice.Clear();
        var selected = new HashSet<CardData>();
        for (int i = 0; i < 3; i++)
        {
            var data = DataBase.Instance.GlobalCardList[Random.Range(0, DataBase.Instance.GlobalCardList.Count)];
            if (selected.Add(data))
            {
                CardChoice.Add(new Card(data));
            }
            else
            {
                i--;
            }
        }
        Vector3 Pos = new Vector3(0, 0, 0);
        for (int i = 0; i < 3; i++)
        {
            CardView cardView = CardViewCreator.Instance.CreateCardView(CardChoice[i], Pos, Quaternion.identity, RewardPanelContent.transform);
            cardView.gameObject.GetComponent<SortingGroup>().sortingOrder = 2;
            cardView.transform.DOScale(50f, 0.5f);
            cardView.IsReward = true;
            cardView.gameObject.layer = LayerMask.NameToLayer("CardReward");
            cardView.gameObject.GetComponent<SortingGroup>().sortingLayerName = "UI";
        }
        CardRewardPanel.SetActive(true);
        RewardSystem.Instance.CardSelectionMode = true;
    }
}
