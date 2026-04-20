using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class NavigationSystem : Singleton<NavigationSystem>
{
    [SerializeField] public GameObject NavPanel;
    [SerializeField] public GameObject NavPanelContent;
    [SerializeField] public TMP_Text CardCounterText;

    [HideInInspector] private List<NavCardData> NavDeckData;
    [HideInInspector] private DataBase dataBase;
    [HideInInspector] public int NavOptionNumber;
    [HideInInspector] public int CardCounter;
    [HideInInspector] public List<NavCardData> NavCardChoice;

    [HideInInspector] public bool Interractable;


    private void Start()
    {
        Interractable = false;
        dataBase = DataBase.Instance;
        NavDeckData = dataBase.NavDeckList;
        CardCounter = NavDeckData.Count;

        UpdateCardCounterText();

        //DataBase.Instance.CurrentStageTier = CombatSystem.Instance.CurrentStageTier + 1;
        NavOptionNumber = dataBase.NavOptionNumber;
        NavDeckData.Shuffle();
        StartCoroutine(DrawNavigation());
    }
    
    public void UpdateCardCounterText()
    {
        CardCounterText.text = CardCounter.ToString();
    }

    public IEnumerator DrawNavigation()
    {
        NavCardChoice.Clear();
        NavCardChoice = new List<NavCardData>();

        if (NavDeckData.Count == 0)
        {
            StartCoroutine(HandleBossFight());
            yield break;
        }

        for (int i = 0; i < NavOptionNumber; i++)
        {
            NavCardData NavCard = NavDeckData.Draw();
            if (NavCard != null)
            {
                NavCardChoice.Add(NavCard);
            }
        }

        for (int i = 0; i < NavCardChoice.Count; i++)
        {
            NavCardView navCardView = NavCardViewCreator.Instance.CreateNavCardView(NavCardChoice[i], Vector3.zero, Quaternion.identity, NavPanelContent.transform);
            CardCounter--;
            UpdateCardCounterText();
            yield return new WaitForSeconds(0.2f);
        }

        yield return null;
        Interractable = true;
    }

    public IEnumerator RedrawNavigation()
    {
        Interractable = false;
        Coroutine ClearCoroutine = StartCoroutine(ClearNavigation());
        yield return ClearCoroutine;
        StartCoroutine(DrawNavigation());
    }    
    
    public IEnumerator ClearNavigation()
    {
        Interractable = false;
        List<Transform> children = new List<Transform>();

        for (int i = 0; i < NavPanelContent.transform.childCount; i++)
        {
            children.Add(NavPanelContent.transform.GetChild(i));
        }

        foreach (Transform child in children)
        {
            Destroy(child.gameObject);
            yield return new WaitForSeconds(0.2f);
        }
    }

    public IEnumerator HandleFight()
    {
        Interractable = false;
        yield return null;
        List<GameObject> EnemyPool = dataBase.CurrentStage.Enemies;
        if (EnemyPool.Count > 0)
        {
            dataBase.NavDeckList = NavDeckData;
            dataBase.SelectedEnemy = EnemyPool[0];
            yield return null;
            SceneManager.LoadScene("CombatScene");
        }
    }

    public IEnumerator HandleEliteFight()
    {
        Interractable = false;
        yield return null;
        List<GameObject> EnemyPool = dataBase.CurrentStage.Elite_Enemies;
        if (EnemyPool.Count > 0)
        {
            dataBase.NavDeckList = NavDeckData;
            dataBase.SelectedEnemy = EnemyPool[0];
            yield return null;
            SceneManager.LoadScene("CombatScene");
        }
    }


    public IEnumerator HandleBossFight()
    {
        Interractable = false;
        yield return null;
        List<GameObject> EnemyPool = dataBase.CurrentStage.Boss_Enemies;
        if (EnemyPool.Count > 0)
        {
            dataBase.NavDeckList = NavDeckData;
            dataBase.SelectedEnemy = EnemyPool[0];
            dataBase.BossFight = true;

            yield return null;
            SceneManager.LoadScene("CombatScene");
        }
    }

    public IEnumerator HandleShop()
    {
        Interractable = false;
        dataBase.NavDeckList = NavDeckData;
        yield return null;
        SceneManager.LoadScene("ShopScene");
    }

    public IEnumerator HandleEvent(NavCardData EventData)
    {
        Interractable = false;
        yield return null;
    }

    public IEnumerator HandleCampsite(NavCardData CampSite)
    {
        Interractable = false;
        if (dataBase.CoreLife != dataBase.BaseCoreLife)
        {
            if (dataBase.CoreLife + CampSite.HealAmount >= dataBase.BaseCoreLife)
            {
                dataBase.CoreLife = dataBase.BaseCoreLife;
            }
            else
            {
                dataBase.CoreLife += CampSite.HealAmount;
            }
        }

        Life_Manager.Instance.UpdateLifeTextText();

        StartCoroutine(RedrawNavigation());
        yield return null;
    }


}
