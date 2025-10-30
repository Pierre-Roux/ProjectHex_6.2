using Unity.VisualScripting;
using UnityEngine;

public class EndTurnButtonUI : MonoBehaviour
{

    public void OnClick()
    {
        if (!CombatSystem.Instance.Interactable) return;
        if (ActionSystem.Instance.IsPerforming) return;
        EndPlayerTurnGA endPlayerTurnGA = new();
        ActionSystem.Instance.Perform(endPlayerTurnGA);
    }

}
