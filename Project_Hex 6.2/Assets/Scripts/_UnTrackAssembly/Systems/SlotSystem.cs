using UnityEditor;
using UnityEngine;

public class SlotSystem : Singleton<SlotSystem>
{
    [SerializeField] public GameObject SlotGroupsGO;
    [SerializeField] public GameObject EnemySlotGroupsGO;

    public SlotView GetFreeSlot(Enemy_Player_ENUM Owner)
    {
        if (Owner == Enemy_Player_ENUM.Player)
        {
            SlotView[] slots = SlotGroupsGO.GetComponentsInChildren<SlotView>();
            foreach (SlotView Slot in slots)
            {
                if (Slot.permanentView == null)
                {
                    return Slot;
                }
            }
        }
        else if (Owner == Enemy_Player_ENUM.Enemy)
        {
            SlotView[] slots = EnemySlotGroupsGO.GetComponentsInChildren<SlotView>();
            foreach (SlotView Slot in slots)
            {
                if (Slot.enemySlotView == null)
                {
                    return Slot;
                }
            }
        }
        return null;
    }

    public void ClearSlot(PermanentView permanentView = null, EnemySlotView enemySlotView = null)
    {
        if (permanentView != null)
        {
            SlotView[] slots = SlotGroupsGO.GetComponentsInChildren<SlotView>();
            foreach (SlotView Slot in slots)
            {
                if (Slot.permanentView == permanentView)
                {
                    Slot.permanentView = null;
                }
            }            
        }
        else if (enemySlotView != null)
        {
            SlotView[] slots = EnemySlotGroupsGO.GetComponentsInChildren<SlotView>();
            foreach (SlotView Slot in slots)
            {
                if (Slot.enemySlotView == enemySlotView)
                {
                    Slot.enemySlotView = null;
                }
            }            
        }
    }
}
