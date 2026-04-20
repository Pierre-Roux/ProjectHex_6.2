using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelBtn : MonoBehaviour
{
    public void onClick()
    {
        DataBase dataBase = DataBase.Instance;
        dataBase.CoreLife = CombatSystem.Instance.PlayerCore.currentLife;
        if (dataBase.BossFight == true)
        {
            // fin d'un boss on crée le deck de nav pour le stage suivant et on selectionne un nouveau stage
            dataBase.BossFight = false;

            foreach (NavCardData navCardData in dataBase.CurrentPlayer.navDeckData.NavCardDataList)
            {
                dataBase.NavDeckList.Add(navCardData);
            }

            List<Stage> LevelStages = new List<Stage>();
            foreach (Stage stage in dataBase.Stages)
            {
                if (stage.Tier == dataBase.CurrentStage.Tier + 1)
                {
                    LevelStages.Add(stage);
                }
            }

            int RandomIndex = Random.Range(0,LevelStages.Count-1);
            dataBase.CurrentStage = LevelStages[RandomIndex];

            foreach (NavCardData navCardData in dataBase.CurrentStage.StageNavCardPool.NavCardDataList)
            {
                dataBase.NavDeckList.Add(navCardData);
            }
            
            SceneManager.LoadScene("NavigationScene");
        }
        else
        {
            // Si ce n'est pas un boss on revient à la navigation scene pour continuer le niveau

            SceneManager.LoadScene("NavigationScene");
        }        
        
    }
}
