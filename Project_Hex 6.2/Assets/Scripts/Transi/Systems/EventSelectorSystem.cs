using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventSelectorSystem : Singleton<EventSelectorSystem>
{

    [SerializeField] public List<Choice> choices;


    public void Start()
    {
        ShuffleChoice();
    }

    public void ShuffleChoice()
    {
        int CurrentStage = DataBase.Instance.CurrentStage;
        foreach (Choice choice in choices)
        {
            int Choicetype = new System.Random().Next(0, 2);
            switch (Choicetype)
            {
                case 0 :
                    choice.UpdateText("Stage " + CurrentStage + " Enemy", "");
                break;

                case 1 :
                    choice.UpdateText("Stage " + CurrentStage + " Enemy", "ELITE");
                    choice.isElite = true;
                break;

                case 2 :
                    choice.UpdateText("Stage " + CurrentStage + " Event", "");
                break;

                default:
                break;
            }
        }
    }

    public void StartFight(int index)
    {
        DataBase.Instance.IsElite = choices[index-1].isElite;

        SceneManager.LoadScene("CombatScene");
    }
}
