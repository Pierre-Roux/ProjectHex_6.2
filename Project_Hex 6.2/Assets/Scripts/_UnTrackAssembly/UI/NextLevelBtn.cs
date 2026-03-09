using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelBtn : MonoBehaviour
{
    public void onClick()
    {
        DataBase.Instance.CoreLife = CombatSystem.Instance.PlayerCore.currentLife;
        SceneManager.LoadScene("NavigationScene");
    }
}
