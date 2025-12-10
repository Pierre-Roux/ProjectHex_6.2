using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class DeckViewSystem : Singleton<DeckViewSystem>
{
    [SerializeField] public GameObject UIDeckViewPanel;
    [SerializeField] public GameObject UIDeckViewPanelContent;
    public void DisplayCards(List<Card> CardsToDisplay, bool DisplayExhaustedCardview)
    {
        CleanDisplay();
        // Instantiate new
        var randomized = new List<Card>(CardsToDisplay);
        randomized.Shuffle();

        foreach (var card in randomized)
        {
            CardView cardView = CardViewCreator.Instance.CreateCardView(card, Vector3.zero, Quaternion.identity, UIDeckViewPanelContent.transform);
            if (DisplayExhaustedCardview)
            {
                cardView.gameObject.layer = LayerMask.NameToLayer("ExhaustedCardView");
                // Ici IsReward n'est pas utilisé réellement en tant que Reward mais juste pour bloquer les fonctionalitées de la carte 
                cardView.IsReward = true;
            }
            else
            {
                cardView.IsVisualDeckCard = true;                
            }
            cardView.gameObject.GetComponent<SortingGroup>().sortingOrder = 2;
            cardView.gameObject.GetComponent<SortingGroup>().sortingLayerName = "UI";
            cardView.transform.DOScale(50, 0.5f);
            cardView.transform.DORotate(new Vector3(-20,0,0), 0.5f);
        }
    }

    public void CleanDisplay()
    {
        // Clean previous
        foreach (Transform child in UIDeckViewPanelContent.transform)
            Destroy(child.gameObject);
    }
}
