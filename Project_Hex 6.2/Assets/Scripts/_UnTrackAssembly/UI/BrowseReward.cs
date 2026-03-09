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
        if (RewardPanelContent.transform.childCount == 0)
        {
            CardChoice.Clear();
            var selected = new HashSet<CardData>();
            for (int i = 0; i < 3; i++)
            {
                var data = RewardSystem.Instance.PickWeightedRandomCard();
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
                CardView cardView = CardViewCreator.Instance.CreateUICardView(CardChoice[i], Pos, Quaternion.identity, RewardPanelContent.transform);
                cardView.IsReward = true;
            }
            CardRewardPanel.SetActive(true);
        }
        else
        {
            CardRewardPanel.SetActive(true); 
        }
    }
}
