using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuDefeat : MonoBehaviour
{
    public void OnClick()
    {
        DataBase.Instance.CurrentStage = 0;
        DataBase.Instance.DeckList = DataBase.Instance.INITIALDeckList;
        SaveSystem.Instance.SaveGame();
        SceneManager.LoadScene("MainMenu");
    }
}
