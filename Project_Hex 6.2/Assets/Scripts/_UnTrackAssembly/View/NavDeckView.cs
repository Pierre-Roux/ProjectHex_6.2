using System.Collections.Generic;
using UnityEngine;

public class NavDeckView : MonoBehaviour
{
    public List<NavCardData> NavDeckData;
    public GameObject DisplayDeckZone;

    /*void OnMouseDown()
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
            DeckViewSystem.Instance.DisplayCards(NavDeckData, false);            
        }
    }*/

    public void UpdateDeckData(List<NavCardData> newDeck)
    {
        NavDeckData = newDeck;
    }
}
