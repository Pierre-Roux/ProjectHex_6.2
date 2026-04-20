using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectorSystem : Singleton<CharacterSelectorSystem>
{
    [SerializeField] public List<PlayerData> Characters;


    public void Start()
    {

    }

    public void Play(int CharaIndex)
    {
        // Get player Nav cards

        DataBase dataBase = DataBase.Instance;
        foreach (NavCardData navCardData in Characters[CharaIndex].navDeckData.NavCardDataList)
        {
            dataBase.NavDeckList.Add(navCardData);
        }

        dataBase.CurrentPlayer = Characters[CharaIndex];

        dataBase.CoreLife = dataBase.BaseCoreLife = dataBase.CurrentPlayer.CoreHealth;

        // Choose starting level

        List<Stage> Level1Stages = new List<Stage>();
        foreach (Stage stage in dataBase.Stages)
        {
            if (stage.Tier == 1)
            {
                Level1Stages.Add(stage);
            }
        }

        int RandomIndex = UnityEngine.Random.Range(0,Level1Stages.Count-1);
        dataBase.CurrentStage = Level1Stages[RandomIndex];

        foreach (NavCardData navCardData in dataBase.CurrentStage.StageNavCardPool.NavCardDataList)
        {
            dataBase.NavDeckList.Add(navCardData);
        }

        SceneManager.LoadScene("NavigationScene");
    }
    
}
