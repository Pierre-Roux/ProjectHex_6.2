using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuDefeat : MonoBehaviour
{
    public void OnClick()
    {
        DataBase.Instance.CurrentStage = null;
        DataBase.Instance.CoreLife = DataBase.Instance.BaseCoreLife;
        DataBase.Instance.Money = 0;
        DataBase.Instance.DeckList.Clear();
        DataBase.Instance.NavDeckList.Clear();

        //SaveSystem.Instance.SaveGame();
        SceneManager.LoadScene("MainMenu");
    }
}
