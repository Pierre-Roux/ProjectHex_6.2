using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavEvent : MonoBehaviour
{
    [SerializeField] public GameObject NextSlide;
    [SerializeField] public GameObject NextSlide2;

    [SerializeField] public NavCardData NavCardReward;
    [SerializeField] public CardData CardReward;

    private NavigationSystem navigationSystem;
    private DataBase dataBase;

    public void Start()
    {
        navigationSystem = NavigationSystem.Instance;
        dataBase = DataBase.Instance;

    }
    private bool exiting;

    public void OpenNextSlide(int slideIndex)
    {
        Exit(() =>
        {
            switch (slideIndex)
            {
                case 0:
                    Instantiate(NextSlide, transform.position, Quaternion.identity, navigationSystem.EventSLideParent.transform);
                    break;
                case 1:
                    Instantiate(NextSlide2, transform.position, Quaternion.identity, navigationSystem.EventSLideParent.transform);
                    break;
                default:
                    Debug.Log("Slide non trouvé");
                    break;
            }

            Destroy(gameObject, 0.1f);
        });
    }

    public void GainLife(int Amount)
    {
        bool died = false;
        if (dataBase.CoreLife + Amount > 0)
        {
            dataBase.CoreLife += Amount;
        }
        else
        {
            died = true;
            navigationSystem.EndGameDefeatPanel.SetActive(true);
            AudioManager.Instance.ChangeMusic(AudioManager.Instance.DefeatMusic);
        }

        Life_Manager life_Manager = Life_Manager.Instance;
        life_Manager.UpdateLifeTextText();

        closeSlide(!died);
    }
    
    public void Gain_Money(int Amount)
    {
        Money_Manager money_Manager = Money_Manager.Instance;
        dataBase.Money += Amount;
        money_Manager.UpdateMoneyText();
        
        if (!navigationSystem.RedrawStarted)
        {
            closeSlide(true);
        }
    }

    public void DraftReward(int Amount)
    {
        RewardSystem.Instance.OpenCardRewardPanel();
        RewardSystem.Instance.DraftReward(Amount);
    }

    public void Gain_CardReward()
    {
        dataBase.DeckList.Add(CardReward);

        if (!navigationSystem.RedrawStarted)
        {
            closeSlide(true);
        }
    }

    public void Gain_NavCardReward()
    {
        navigationSystem.NavDeckData.Add(NavCardReward);
        navigationSystem.CardCounter++;
        navigationSystem.UpdateCardCounterText();

        if (!navigationSystem.RedrawStarted)
        {
            closeSlide(true);
        }
    }

    // Used by buttons in events
    public void CloseSLide()
    {
        if (!navigationSystem.RedrawStarted)
        {
            closeSlide(true);
        }
    }

    private void closeSlide(bool BecomeInteractable)
    {
        Exit(() =>
        {
            navigationSystem.Interractable = BecomeInteractable;

            navigationSystem.Redraw();

            Destroy(gameObject, 0.1f);
        });
    }

    private void Exit(Action exitAction)
    {
        if (exiting) return;
        exiting = true;
        StartCoroutine(ExitAfterDraft(exitAction));
    }

    private IEnumerator ExitAfterDraft(Action exitAction)
    {
        // On attend une frame : tous les listeners du bouton (DraftReward inclus) ont été exécutés,
        // quel que soit leur ordre dans l'inspecteur.
        yield return null;

        // WaitingForDraft passe à true dans DraftReward, et à false à la fin du draft (carte prise ou panneau fermé).
        yield return new WaitUntil(() => !RewardSystem.Instance.WaitingForDraft);

        exitAction();
    }
}
