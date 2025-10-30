using System.Collections.Generic;
using UnityEngine;

public class DeckView : MonoBehaviour
{
    public List<Card> DeckData;
    public GameObject DisplayDeckZone;

    void OnMouseDown()
    {
        if (DisplayDeckZone.activeSelf)
        {
            DisplayDeckZone.SetActive(false);
            DeckViewSystem.Instance.CleanDisplay();
        }
        else
        {
            if (!CombatSystem.Instance.Interactable) return;
            if (ActionSystem.Instance.IsPerforming) return;
            DisplayDeckZone.SetActive(true);
            DeckViewSystem.Instance.DisplayCards(DeckData);            
        }
    }

    public void UpdateDeckData(List<Card> newDeck)
    {
        DeckData = newDeck;
    }
}
