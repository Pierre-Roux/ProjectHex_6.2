using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaSystem : Singleton<ManaSystem>
{
    [SerializeField] private ManaUI manaUI;
    public int MAX_MANA;
    public int currentMana;
    public int Mana_Spent_Count;

    public void OnEnable()
    {
        ActionSystem.AttachPerformer<SpendManaGA>(SpendManaPerformer);
        ActionSystem.AttachPerformer<ReffilManaGA>(RefillManaPerformer);
        ActionSystem.AttachPerformer<GainManaGA>(GainManaPerformer);
    }

    public void OnDisable()
    {
        ActionSystem.DetachPerformer<SpendManaGA>();
        ActionSystem.DetachPerformer<ReffilManaGA>();
        ActionSystem.DetachPerformer<GainManaGA>();
    }

    public void SetManaMax(int Amount)
    {
        MAX_MANA = Amount;
    }

    //performers

    private IEnumerator GainManaPerformer(GainManaGA gainManaGA)
    {
        if (gainManaGA.DynamicAmount != DynamicAmount.NULL)
        {
            gainManaGA.GainAmount = TargetSystem.Instance.GetDynamicAmount(gainManaGA.DynamicAmount);
        }
        currentMana += gainManaGA.GainAmount;
        manaUI.UpdateManaText(currentMana);
        yield return null;
    }

    private IEnumerator SpendManaPerformer(SpendManaGA spendManaGA)
    {
        currentMana -= spendManaGA.Amount;
        Mana_Spent_Count += spendManaGA.Amount;
        manaUI.UpdateManaText(currentMana);
        yield return null;
    }

    private IEnumerator RefillManaPerformer(ReffilManaGA reffilManaGA)
    {
        currentMana = MAX_MANA;
        Mana_Spent_Count = 0;
        manaUI.UpdateManaText(currentMana);
        yield return null;
    }

    public bool HasEnoughMana(int manacost)
    {
        return currentMana >= manacost;
    }
}
