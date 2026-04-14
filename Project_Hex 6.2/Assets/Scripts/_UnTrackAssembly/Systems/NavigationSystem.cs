using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class NavigationSystem : Singleton<NavigationSystem>
{
    [SerializeField] public GameObject NavPanel;
    [SerializeField] public GameObject NavPanelContent;
    private List<NavCardData> NavDeckData;
    private DataBase dataBase;
    [HideInInspector] public int NavOptionNumber;
    [HideInInspector] public List<NavCardData> NavCardChoice;

    private void Start()
    {
        dataBase = DataBase.Instance;
        NavDeckData = dataBase.NavDeckList;

        //DataBase.Instance.CurrentStageTier = CombatSystem.Instance.CurrentStageTier + 1;
        NavOptionNumber = dataBase.NavOptionNumber;
        NavDeckData.Shuffle();
        Debug.Log(NavDeckData.Count);
        StartCoroutine(DrawNavigation());
    }

    public IEnumerator DrawNavigation()
    {
        List<NavCardData> choices = new List<NavCardData>();
        NavCardChoice.Clear();
        for (int i = 0; i < NavOptionNumber - 1; i++)
        {
            NavCardData NavCard = NavDeckData.Draw();
            if (NavCard != null)
            {
                choices.Add(NavCard);
            }
        }

        Debug.Log("Choices : " + choices.Count);

        for (int i = 0; i < choices.Count; i++)
        {
            NavCardView navCardView = NavCardViewCreator.Instance.CreateNavCardView(choices[i], Vector3.zero, Quaternion.identity, NavPanelContent.transform);
        }

        yield return null;
    }

    public IEnumerator HandleFight()
    {
        yield return null;
        List<GameObject> EnemyPool = dataBase.CurrentStage.Enemies;
        SceneManager.LoadScene("CombatScene");
    }
    
    public IEnumerator HandleShop()
    {
        yield return null;
        SceneManager.LoadScene("ShopScene");
    }
}
