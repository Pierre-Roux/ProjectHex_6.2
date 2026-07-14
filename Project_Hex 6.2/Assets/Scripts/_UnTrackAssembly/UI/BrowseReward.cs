using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class BrowseReward : MonoBehaviour
{
    [SerializeField] public GameObject CardRewardPanel;
    [SerializeField] public GameObject RewardPanelContent;
    
    public void OnClick()
    {
        if (RewardPanelContent.transform.childCount == 0)
        {
            RewardSystem.Instance.DraftReward(3);
            CardRewardPanel.SetActive(true);
        }
        else
        {
            CardRewardPanel.SetActive(true); 
        }        
    }
}
