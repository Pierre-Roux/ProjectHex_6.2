using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ManaSystem : Singleton<ManaSystem>
{
    [SerializeField] private ManaUI manaUI;
    public int MAX_MANA;
    public int currentMana;
    public int Mana_Spent_Count;
    public int PayXInitialMana;

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
        if (gainManaGA.DynamicAmountInfo.DynamicAmount != DynamicAmount.NULL)
        {
            gainManaGA.GainAmount = TargetSystem.Instance.GetDynamicAmount(gainManaGA.DynamicAmountInfo);
        }
        currentMana += gainManaGA.GainAmount;
        UpdateManaText();
        yield return null;
    }

    private IEnumerator SpendManaPerformer(SpendManaGA spendManaGA)
    {
        currentMana -= spendManaGA.Amount;
        Mana_Spent_Count += spendManaGA.Amount;
        UpdateManaText();
        yield return null;
    }

    private IEnumerator RefillManaPerformer(ReffilManaGA reffilManaGA)
    {
        currentMana = MAX_MANA;
        Mana_Spent_Count = 0;
        UpdateManaText();
        yield return null;
    }

    public bool HasEnoughMana(int manacost)
    {
        return currentMana >= manacost;
    }

    public void UpdateManaText()
    {
        manaUI.UpdateManaText(currentMana);
    }

    public void VisualAddMana(int Amount)
    {
        currentMana += Amount;
        Mana_Spent_Count += Amount;
        UpdateManaText();
    }
    
    public void VisualsubtractMana(int Amount)
    {
        currentMana -= Amount;
        Mana_Spent_Count -= Amount;
        UpdateManaText();
    }
}
