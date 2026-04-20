using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Life_Manager : Singleton<Life_Manager>
{
    [SerializeField] TMP_Text LifeText;

    public void Start()
    {
        UpdateLifeTextText();
    }

    public void UpdateLifeTextText()
    {
        LifeText.text = DataBase.Instance.CoreLife.ToString();
    }
}
