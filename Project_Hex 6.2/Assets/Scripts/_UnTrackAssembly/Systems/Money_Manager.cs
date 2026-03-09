using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Money_Manager : Singleton<Money_Manager>
{
    [SerializeField] TMP_Text MoneyText;

    public void Start()
    {
        UpdateMoneyText();
    }

    public void UpdateMoneyText()
    {
        MoneyText.text = DataBase.Instance.Money.ToString();
    }
}
