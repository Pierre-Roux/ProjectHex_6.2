using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class DeckViewSystem : Singleton<DeckViewSystem>
{
    [SerializeField] public GameObject UIDeckViewPanel;
    [SerializeField] public GameObject UIDeckViewPanelContent;
    public void DisplayCards(List<Card> CardsToDisplay)
    {
        CleanDisplay();
        // Instantiate new
        var randomized = new List<Card>(CardsToDisplay);
        randomized.Shuffle();

        foreach (var card in randomized)
        {
            CardView cardView = CardViewCreator.Instance.CreateCardView(card, Vector3.zero, Quaternion.identity, UIDeckViewPanelContent.transform);
            cardView.gameObject.GetComponent<SortingGroup>().sortingOrder = 2;
            cardView.gameObject.GetComponent<SortingGroup>().sortingLayerName = "UI";
            cardView.transform.DOScale(50, 0.5f);
        }
    }

    public void CleanDisplay()
    {
        // Clean previous
        foreach (Transform child in UIDeckViewPanelContent.transform)
            Destroy(child.gameObject);
    }
}
