using System;
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
    public void OpenNextSlide(int slideIndex)
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

        Destroy(this.gameObject, 0.1f);
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
        navigationSystem.Interractable = BecomeInteractable;

        navigationSystem.Redraw();

        Destroy(this.gameObject, 0.1f);
    }
}
