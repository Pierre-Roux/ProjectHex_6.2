using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelBtn : MonoBehaviour
{
    public void onClick()
    {
        DataBase.Instance.CurrentStage++;
        DataBase.Instance.CoreLife = CombatSystem.Instance.PlayerCore.currentLife;
        if (DataBase.Instance.CurrentStage == 1 || DataBase.Instance.CurrentStage == 4)
        {
            SceneManager.LoadScene("ShopScene");
        }
        else
        {
            SceneManager.LoadScene("TransitionScene");
        }
        
    }
}
