using UnityEngine;

public class CardViewHover : Singleton<CardViewHover>
{
    [SerializeField] private CardView cardViewToHover;
    

    public void Show(CardView cardview, Vector3 position)
    {
        if (!TargetSystem.Instance.CardTargetingActive)
        {
            if (!CombatSystem.Instance.Interactable) return;
            if (ActionSystem.Instance.IsPerforming) return;
        }
        cardViewToHover.gameObject.SetActive(true);
        cardViewToHover.Setup(cardview.Card);
        cardViewToHover.transform.position = position;
    }

    public void Hide()
    {
        cardViewToHover.gameObject.SetActive(false);
    }
}
