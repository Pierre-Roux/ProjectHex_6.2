using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class NavigationSystem : Singleton<NavigationSystem>
{
    [SerializeField] public GameObject NavPanel;
    [SerializeField] public GameObject NavPanelContent;
    [SerializeField] private Transform drawPilePoint;
    [SerializeField] private Transform discardPilePoint;
    [SerializeField] private NavDeckView DrawNavDeck;
    [SerializeField] private NavDeckView DiscardNavDeck;

    [HideInInspector] public int NavOptionNumber;
    [HideInInspector] public List<NavCardData> NavCardChoice;

    private void Start()
    {
        DataBase dataBase = DataBase.Instance;
        DrawNavDeck.NavDeckData = dataBase.NavDeckList;

        //DataBase.Instance.CurrentStageTier = CombatSystem.Instance.CurrentStageTier + 1;
        NavOptionNumber = dataBase.NavOptionNumber;
        DrawNavDeck.NavDeckData.Shuffle();
        Debug.Log(DrawNavDeck.NavDeckData.Count);
        StartCoroutine(DrawNavigation());
    }

    public IEnumerator DrawNavigation()
    {
        List<NavCardData> choices = new List<NavCardData>(); 
        NavCardChoice.Clear();
        for (int i = 0; i < NavOptionNumber-1; i++)
        {
            NavCardData NavCard = DrawNavDeck.NavDeckData.Draw();
            if (NavCard != null)
            {
                choices.Add(NavCard);
            }
        }

        foreach (NavCardData NavCardChoice in choices)
        {
            NavCardView navCardView = NavCardViewCreator.Instance.CreateNavCardView(NavCardChoice, Vector3.zero, Quaternion.identity, NavPanelContent.transform);
            if (navCardView != null)
            {
                Vector3 finalpos = navCardView.transform.position;
                Debug.Log(finalpos);
                navCardView.gameObject.SetActive(false);
                navCardView.transform.position = drawPilePoint.transform.position;
                navCardView.gameObject.SetActive(true);
                Tween Mouvement = navCardView.transform.DOMove(finalpos,1);
                yield return Mouvement.WaitForCompletion();
            }
        }

        yield return null;
    }
}
