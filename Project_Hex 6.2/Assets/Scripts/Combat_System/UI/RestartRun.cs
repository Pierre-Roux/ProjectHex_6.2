using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartRun : MonoBehaviour
{
    public void OnClick()
    {
        DataBase.Instance.CurrentStage = 0;
        DataBase.Instance.CoreLife = DataBase.Instance.BaseCoreLife;
        DataBase.Instance.Money = 0;
        DataBase.Instance.DeckList = DataBase.Instance.INITIALDeckList;
        
        //SaveSystem.Instance.SaveGame();
        SceneManager.LoadScene("TransitionScene");
    }
}
